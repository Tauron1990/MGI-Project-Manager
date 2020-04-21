﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using JetBrains.Annotations;
using JKang.IpcServiceFramework.IO;
using JKang.IpcServiceFramework.Services;

namespace JKang.IpcServiceFramework
{
    internal static class IpcServiceClient
    {
        internal static readonly ProxyGenerator ProxyGenerator = new ProxyGenerator();
    }

    [PublicAPI]
    public abstract class IpcServiceClient<TInterface>
        where TInterface : class
    {

        private readonly IValueConverter _converter;
        private readonly IIpcMessageSerializer _serializer;

        protected IpcServiceClient(
            IIpcMessageSerializer serializer,
            IValueConverter converter)
        {
            _serializer = serializer;
            _converter = converter;
        }

        public async Task InvokeAsync(Expression<Action<TInterface>> exp,
            CancellationToken cancellationToken = default)
        {
            var request = GetRequest(exp, new MyInterceptor());
            var response = await GetResponseAsync(request, cancellationToken).ConfigureAwait(false);

            if (response.Succeed)
                return;
            throw new InvalidOperationException(response.Failure);
        }

        [return:MaybeNull]
        public async Task<TResult> InvokeAsync<TResult>(Expression<Func<TInterface, TResult>> exp,
            CancellationToken cancellationToken = default)
        {
            var request = GetRequest(exp, new MyInterceptor<TResult>());
            var response = await GetResponseAsync(request, cancellationToken).ConfigureAwait(false);

            if (!response.Succeed) throw new InvalidOperationException(response.Failure);

            var data = response.Data;
            if (data != null && _converter.TryConvert(data, typeof(TResult), out var @return))
                return  (@return is TResult value ? value : default)!;
            
            throw new InvalidOperationException($"Unable to convert returned value to '{typeof(TResult).Name}'.");

        }

        public async Task InvokeAsync(Expression<Func<TInterface, Task>> exp,
            CancellationToken cancellationToken = default)
        {
            var request = GetRequest(exp, new MyInterceptor<Task>());
            var response = await GetResponseAsync(request, cancellationToken).ConfigureAwait(false);

            if (response.Succeed)
                return;
            throw new InvalidOperationException(response.Failure);
        }

        [return:MaybeNull]
        public async Task<TResult> InvokeAsync<TResult>(Expression<Func<TInterface, Task<TResult>>> exp,
            CancellationToken cancellationToken = default)
        {
            var request = GetRequest(exp, new MyInterceptor<Task<TResult>>());
            var response = await GetResponseAsync(request, cancellationToken).ConfigureAwait(false);
            var data = response.Data;

            if (response.Succeed && data != null)
            {
                if (_converter.TryConvert(data, typeof(TResult), out var @return))
                    return (@return is TResult value ? value : default)!;
                throw new InvalidOperationException($"Unable to convert returned value to '{typeof(TResult).Name}'.");
            }

            throw new InvalidOperationException(response.Failure);
        }


        private static IpcRequest GetRequest(Expression exp, MyInterceptor interceptor)
        {
            if (!(exp is LambdaExpression lamdaExp)) throw new ArgumentException("Only support lamda expresion, ex: x => x.GetData(a, b)");

            if (!(lamdaExp.Body is MethodCallExpression)) throw new ArgumentException("Only support calling method, ex: x => x.GetData(a, b)");

            var proxy = IpcServiceClient.ProxyGenerator.CreateInterfaceProxyWithoutTarget<TInterface>(interceptor);
            var @delegate = lamdaExp.Compile();
            @delegate.DynamicInvoke(proxy);

            return new IpcRequest
                   {
                       MethodName = interceptor.LastInvocation.Method.Name,
                       Parameters = interceptor.LastInvocation.Arguments,

                       ParameterTypes = interceptor.LastInvocation.Method.GetParameters()
                          .Select(p => p.ParameterType)
                          .ToArray(),


                       GenericArguments = interceptor.LastInvocation.GenericArguments
                   };
        }

        protected abstract Task<Stream> ConnectToServerAsync(CancellationToken cancellationToken);

        private async Task<IpcResponse> GetResponseAsync(IpcRequest request, CancellationToken cancellationToken)
        {
            await using var client = await ConnectToServerAsync(cancellationToken).ConfigureAwait(false);
            using var writer = new IpcWriter(client, _serializer, true);
            using var reader = new IpcReader(client, _serializer, true);
            // send request
            await writer.WriteAsync(request, cancellationToken).ConfigureAwait(false);

            // receive response
            return await reader.ReadIpcResponseAsync(cancellationToken).ConfigureAwait(false);
        }

        private class MyInterceptor : IInterceptor
        {
            private IInvocation? _lastInvocation;

            public IInvocation LastInvocation
            {
                get => _lastInvocation ?? throw new InvalidOperationException("Las Invocation was not det");
                private set => _lastInvocation = value;
            }

            public virtual void Intercept(IInvocation invocation)
            {
                LastInvocation = invocation;
            }
        }

        private class MyInterceptor<TResult> : MyInterceptor
        {
            public override void Intercept(IInvocation invocation)
            {
                base.Intercept(invocation);
                invocation.ReturnValue = default(TResult);
            }
        }
    }
}