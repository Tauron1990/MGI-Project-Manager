﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Commands;
using CQRSlite.Events;
using CQRSlite.Messages;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Tauron.CQRS.Common.Configuration;

namespace Tauron.CQRS.Services.Core.Components
{
    [UsedImplicitly]
    public class HandlerManager : IHandlerManager, IDisposable
    {
        private class HandlerListDelegator
        {
            private readonly List<HandlerInstace> _handlers;

            public HandlerListDelegator(List<HandlerInstace> handlers) => _handlers = handlers;

            public async Task Handle(IMessage msg, CancellationToken token)
            {
                foreach (var handlerInstace in _handlers) await handlerInstace.Handle(msg, token);
            }
        }
        private class HandlerInstace
        {
            private abstract class InvokerBase
            {
                public abstract Task Invoke(IMessage msg, CancellationToken token);
            }
            private abstract class InvokerHelper : InvokerBase
            {
                protected delegate object FastInvokeHandler(object target, params object[] paramters);

                private readonly Type _targetType;
                private readonly Type _targetInterface;
                private FastInvokeHandler _methodInfo;

                protected InvokerHelper(Type targetType, Type targetInterface)
                {
                    _targetType = targetType;
                    _targetInterface = targetInterface;
                }

                protected FastInvokeHandler GetMethod()
                {
                    if (_methodInfo != null) return _methodInfo;
                    var temp = _targetType.GetInterfaceMap(_targetInterface.GetInterfaces().Single());
                    _methodInfo = GetMethodInvoker(temp.InterfaceMethods.Single());
                    return _methodInfo;
                }

                private static FastInvokeHandler GetMethodInvoker(MethodInfo methodInfo)
                {
                    if (methodInfo.DeclaringType == null) return null;
                    
                    var dynamicMethod = new DynamicMethod(string.Empty, typeof(object), new[] { typeof(object), typeof(object[]) }, methodInfo.DeclaringType.Module);
                    var il = dynamicMethod.GetILGenerator();
                    var ps = methodInfo.GetParameters();
                    var paramTypes = new Type[ps.Length];
                    for (var i = 0; i < paramTypes.Length; i++)
                    {
                        if (ps[i].ParameterType.IsByRef)
                            paramTypes[i] = ps[i].ParameterType.GetElementType();
                        else
                            paramTypes[i] = ps[i].ParameterType;
                    }
                    var locals = new LocalBuilder[paramTypes.Length];

                    for (var i = 0; i < paramTypes.Length; i++) 
                        locals[i] = il.DeclareLocal(paramTypes[i], true);
                    for (var i = 0; i < paramTypes.Length; i++)
                    {
                        il.Emit(OpCodes.Ldarg_1);
                        EmitFastInt(il, i);
                        il.Emit(OpCodes.Ldelem_Ref);
                        EmitCastToReference(il, paramTypes[i]);
                        il.Emit(OpCodes.Stloc, locals[i]);
                    }
                    if (!methodInfo.IsStatic) 
                        il.Emit(OpCodes.Ldarg_0);
                    for (var i = 0; i < paramTypes.Length; i++) 
                        il.Emit(ps[i].ParameterType.IsByRef ? OpCodes.Ldloca_S : OpCodes.Ldloc, locals[i]);

                    il.EmitCall(methodInfo.IsStatic ? OpCodes.Call : OpCodes.Callvirt, methodInfo, null);
                    if (methodInfo.ReturnType == typeof(void))
                        il.Emit(OpCodes.Ldnull);
                    else
                        EmitBoxIfNeeded(il, methodInfo.ReturnType);

                    for (var i = 0; i < paramTypes.Length; i++)
                    {
                        if (!ps[i].ParameterType.IsByRef) continue;
                            
                        il.Emit(OpCodes.Ldarg_1);
                        EmitFastInt(il, i);
                        il.Emit(OpCodes.Ldloc, locals[i]);
                        var memberInfo = locals[i].LocalType;
                        if (memberInfo != null && memberInfo.IsValueType)
                            il.Emit(OpCodes.Box, locals[i].LocalType);
                        il.Emit(OpCodes.Stelem_Ref);
                    }

                    il.Emit(OpCodes.Ret);
                    var invoder = (FastInvokeHandler)dynamicMethod.CreateDelegate(typeof(FastInvokeHandler));
                    return invoder;

                }

                private static void EmitCastToReference(ILGenerator il, Type type) => il.Emit(type.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, type);

                private static void EmitBoxIfNeeded(ILGenerator il, Type type)
                {
                    if (type.IsValueType) il.Emit(OpCodes.Box, type);
                }

                private static void EmitFastInt(ILGenerator il, int value)
                {
                    switch (value)
                    {
                        case -1:
                            il.Emit(OpCodes.Ldc_I4_M1);
                            return;
                        case 0:
                            il.Emit(OpCodes.Ldc_I4_0);
                            return;
                        case 1:
                            il.Emit(OpCodes.Ldc_I4_1);
                            return;
                        case 2:
                            il.Emit(OpCodes.Ldc_I4_2);
                            return;
                        case 3:
                            il.Emit(OpCodes.Ldc_I4_3);
                            return;
                        case 4:
                            il.Emit(OpCodes.Ldc_I4_4);
                            return;
                        case 5:
                            il.Emit(OpCodes.Ldc_I4_5);
                            return;
                        case 6:
                            il.Emit(OpCodes.Ldc_I4_6);
                            return;
                        case 7:
                            il.Emit(OpCodes.Ldc_I4_7);
                            return;
                        case 8:
                            il.Emit(OpCodes.Ldc_I4_8);
                            return;
                        default:
                            if (value > -129 && value < 128)
                                il.Emit(OpCodes.Ldc_I4_S, (SByte) value);
                            else
                                il.Emit(OpCodes.Ldc_I4, value);
                            return;
                    }
                }
            }

            private class Command : InvokerHelper
            {
                private readonly Func<object> _handler;

                public Command(Func<object> handler, Type targetType, Type targetInterface) : base(targetType, targetInterface) => _handler = handler;

                public override async Task Invoke(IMessage msg, CancellationToken token)
                    => await (Task)GetMethod()(_handler(), msg);
            }
            private class CancelCommand : InvokerHelper
            {
                private readonly Func<object> _handler;

                public CancelCommand(Func<object> handler, Type targetType, Type targetInterface) : base(targetType, targetInterface) => _handler = handler;

                public override async Task Invoke(IMessage msg, CancellationToken token)
                    => await (Task)GetMethod()(_handler(), msg, token);
            }
            private class Event : InvokerHelper
            {
                private readonly Func<object> _handler;

                public Event(Func<object> handler, Type targetType, Type targetInterface) : base(targetType, targetInterface) => _handler = handler;

                public override async Task Invoke(IMessage msg, CancellationToken token)
                    => await (Task)GetMethod()(_handler(), msg);
            }
            private class CancelEvent : InvokerHelper
            {
                private readonly Func<object> _handler;

                public CancelEvent(Func<object> handler, Type targetType, Type targetInterface) : base(targetType, targetInterface) => _handler = handler;
                
                public override async Task Invoke(IMessage msg, CancellationToken token)
                    => await (Task)GetMethod()(_handler(), msg, token);
            }

            private readonly Dictionary<Type, InvokerBase> _invoker = new Dictionary<Type, InvokerBase>();
            
            public bool IsCommand { get; }

            public HandlerInstace(Func<object> target, Type handlerType)
            {
                var inters = handlerType.GetInterfaces();

                foreach (var i in inters.Where(i => i.IsGenericType))
                {
                    var targetType = i.GetGenericTypeDefinition();

                    if(!IsCommand)
                        IsCommand = targetType == typeof(ICommandHandler<>) || targetType == typeof(ICancellableCommandHandler<>);
                    Type key = i.GetGenericArguments()[0];


                    if (targetType == typeof(ICommandHandler<>)) _invoker[key] = new Command(target, handlerType, i);
                    else if (targetType == typeof(ICancellableCommandHandler<>)) _invoker[key] = new CancelCommand(target, handlerType, i);
                    else if (targetType == typeof(IEventHandler<>)) _invoker[key] = new Event(target, handlerType, i);
                    else if (targetType == typeof(ICancellableEventHandler<>)) _invoker[key] = new CancelEvent(target, handlerType, i);
                }

                if(_invoker.Count == 0)
                    throw new InvalidOperationException("No Invoker Found!");
            }

            public async Task Handle(IMessage msg, CancellationToken token) => await _invoker[msg.GetType()].Invoke(msg, token);
        }

        private readonly IOptions<ClientCofiguration> _configuration;
        private readonly IDispatcherClient _client;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        // ReSharper disable once CollectionNeverQueried.Local
        private readonly List<HandlerListDelegator> _handlerInstaces = new List<HandlerListDelegator>();

        public HandlerManager(IOptions<ClientCofiguration> configuration, IDispatcherClient client, IServiceScopeFactory serviceScopeFactory)
        {
            _configuration = configuration;
            _client = client;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task Init(CancellationToken token)
        {
            await _client.Start(token);

            foreach (var handler in _configuration.Value.GetHandlers())
            {
                var commands = new List<HandlerInstace>();
                var events = new List<HandlerInstace>();

                foreach (var handlerInstace in handler.Value
                                .Select(h => new HandlerInstace(() =>
                                                                {
                                                                    using var scope = _serviceScopeFactory.CreateScope();
                                                                    return ActivatorUtilities.GetServiceOrCreateInstance(scope.ServiceProvider, h);
                                                                }, h)))
                { 
                    if(handlerInstace.IsCommand)
                        commands.Add(handlerInstace);
                    else
                        events.Add(handlerInstace);
                }


                if (commands.Count != 0)
                {
                    var del = new HandlerListDelegator(commands);
                    await _client.Subsribe(handler.Key, del.Handle, true);
                    _handlerInstaces.Add(del);
                }
                else
                {
                    var del = new HandlerListDelegator(events);
                    await _client.Subsribe(handler.Key, del.Handle, false);
                    _handlerInstaces.Add(del);
                }
            }
        }

        public void Dispose() 
            => _handlerInstaces.Clear();
    }
}