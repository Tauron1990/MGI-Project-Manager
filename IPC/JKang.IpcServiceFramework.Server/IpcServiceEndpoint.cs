using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using JKang.IpcServiceFramework.IO;
using JKang.IpcServiceFramework.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace JKang.IpcServiceFramework
{
    public abstract class IpcServiceEndpoint
    {
        protected IpcServiceEndpoint(string name, IServiceProvider serviceProvider)
        {
            Name = name;
            ServiceProvider = serviceProvider;
        }

        public string Name { get; }
        public IServiceProvider ServiceProvider { get; }

        public abstract Task ListenAsync(CancellationToken cancellationToken = default);
    }

    public abstract class IpcServiceEndpoint<TContract> : IpcServiceEndpoint
        where TContract : class
    {
        private readonly IValueConverter _converter;
        private readonly IIpcMessageSerializer _serializer;

        protected IpcServiceEndpoint(string name, IServiceProvider serviceProvider)
            : base(name, serviceProvider)
        {
            _converter = serviceProvider.GetRequiredService<IValueConverter>();
            _serializer = serviceProvider.GetRequiredService<IIpcMessageSerializer>();
        }

        protected async Task ProcessAsync(Stream server, ILogger logger, CancellationToken cancellationToken)
        {
            using var writer = new IpcWriter(server, _serializer, true);
            using var reader = new IpcReader(server, _serializer, true);
            try
            {
                if (cancellationToken.IsCancellationRequested) return;

                logger?.LogDebug($"[thread {Thread.CurrentThread.ManagedThreadId}] client connected, reading request...");
                var request = await reader.ReadIpcRequestAsync(cancellationToken).ConfigureAwait(false);

                cancellationToken.ThrowIfCancellationRequested();

                logger?.LogDebug($"[thread {Thread.CurrentThread.ManagedThreadId}] request received, invoking '{request.MethodName}'...");
                IpcResponse response;
                using (var scope = ServiceProvider.CreateScope()) response = await GetReponse(request, scope).ConfigureAwait(false);

                cancellationToken.ThrowIfCancellationRequested();

                logger?.LogDebug($"[thread {Thread.CurrentThread.ManagedThreadId}] sending response...");
                await writer.WriteAsync(response, cancellationToken).ConfigureAwait(false);

                logger?.LogDebug($"[thread {Thread.CurrentThread.ManagedThreadId}] done.");
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, ex.Message);
                await writer.WriteAsync(IpcResponse.Fail($"Internal server error: {ex.Message}"), cancellationToken).ConfigureAwait(false);
            }
        }

        protected async Task<IpcResponse> GetReponse(IpcRequest request, IServiceScope scope)
        {
            object service = scope.ServiceProvider.GetService<TContract>();
            if (service == null) return IpcResponse.Fail($"No implementation of interface '{typeof(TContract).FullName}' found.");

            var method = GetUnambiguousMethod(request, service);

            if (method == null) return IpcResponse.Fail($"Method '{request.MethodName}' not found in interface '{typeof(TContract).FullName}'.");

            var paramInfos = method.GetParameters();
            if (paramInfos.Length != request.Parameters.Length) return IpcResponse.Fail("Parameter mismatch.");

            var genericArguments = method.GetGenericArguments();
            if (genericArguments.Length != request.GenericArguments.Length) return IpcResponse.Fail("Generic arguments mismatch.");

            var args = new object?[paramInfos.Length];
            for (var i = 0; i < args.Length; i++)
            {
                var origValue = request.Parameters[i];
                var destType = paramInfos[i].ParameterType;
                if (destType.IsGenericParameter) destType = request.GenericArguments[destType.GenericParameterPosition];

                if (_converter.TryConvert(origValue, destType, out var arg))
                    args[i] = arg;
                else
                    return IpcResponse.Fail($"Cannot convert value of parameter '{paramInfos[i].Name}' ({origValue}) from {origValue.GetType().Name} to {destType.Name}.");
            }

            try
            {
                if (method.IsGenericMethod) method = method.MakeGenericMethod(request.GenericArguments);

                var @return = method.Invoke(service, args);

                if (!(@return is Task task)) return IpcResponse.Success(@return);
                
                await task.ConfigureAwait(false);

                var resultProperty = task.GetType().GetProperty("Result");
                return IpcResponse.Success(resultProperty.GetValue(task));

            }
            catch (Exception ex)
            {
                return IpcResponse.Fail($"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        ///     Get the method that matches the requested signature
        /// </summary>
        /// <param name="request">The service call request</param>
        /// <param name="service">The service</param>
        /// <returns>The disambiguated service method</returns>
        public static MethodInfo? GetUnambiguousMethod(IpcRequest request, object service)
        {
            if (request == null || service == null) return null;

            MethodInfo? method = null; // disambiguate - can't just call as before with generics - MethodInfo method = service.GetType().GetMethod(request.MethodName);

            // Thanks https://github.com/luhis for these changes
            var types = service.GetType().GetInterfaces();
            var allMethods = types.SelectMany(t => t.GetMethods());
            var serviceMethods = allMethods.Where(t => t.Name == request.MethodName).ToList();

            foreach (var serviceMethod in serviceMethods)
            {
                var serviceMethodParameters = serviceMethod.GetParameters();
                var parameterTypeMatches = 0;

                if (serviceMethodParameters.Length == request.Parameters.Length && serviceMethod.GetGenericArguments().Length == request.GenericArguments.Length)
                {
                    for (var parameterIndex = 0; parameterIndex < serviceMethodParameters.Length; parameterIndex++)
                    {
                        var serviceParameterType = serviceMethodParameters[parameterIndex].ParameterType.IsGenericParameter ? request.GenericArguments[serviceMethodParameters[parameterIndex].ParameterType.GenericParameterPosition] : serviceMethodParameters[parameterIndex].ParameterType;

                        if (serviceParameterType == request.ParameterTypes[parameterIndex])
                            parameterTypeMatches++;
                        else
                            break;
                    }

                    if (parameterTypeMatches == serviceMethodParameters.Length)
                    {
                        method = serviceMethod; // signatures match so assign
                        break;
                    }
                }
            }

            return method;
        }
    }
}