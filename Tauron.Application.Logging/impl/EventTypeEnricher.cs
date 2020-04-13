﻿using System;
using System.Diagnostics;
using System.Text;
using Murmur;
using Serilog.Core;
using Serilog.Events;

namespace Tauron.Application.Logging.impl
{
    [DebuggerStepThrough]
    public sealed class EventTypeEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var murmur = MurmurHash.Create32();
            var bytes = Encoding.UTF8.GetBytes(logEvent.MessageTemplate.Text);
            var hash = murmur.ComputeHash(bytes);
            var numericHash = BitConverter.ToUInt32(hash, 0);
            var eventType = propertyFactory.CreateProperty("EventType", numericHash.ToString("x8"));
            logEvent.AddPropertyIfAbsent(eventType);
        }
    }
}