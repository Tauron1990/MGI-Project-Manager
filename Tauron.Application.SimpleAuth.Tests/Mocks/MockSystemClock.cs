using System;
using Microsoft.AspNetCore.Authentication;

namespace Tauron.Application.SimpleAuth.Tests.Mocks
{
    public sealed class MockSystemClock : ISystemClock
    {
        public DateTimeOffset? TargetDate { get; set; }

        public DateTimeOffset UtcNow => TargetDate ?? DateTimeOffset.UtcNow;
    }
}