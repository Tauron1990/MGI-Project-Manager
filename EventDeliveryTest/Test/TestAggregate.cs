using System;
using JetBrains.Annotations;
using Tauron.CQRS.Services;

namespace EventDeliveryTest.Test
{
    public class TestAggregate : CoreAggregateRoot
    {
        public static readonly Guid IdField = new Guid("10FC8F67-4F4F-427B-A734-D6F2BD22A376");

        public TestAggregate() => Id = IdField;

        [UsedImplicitly]
        private void Apply(TestEvent testEvent) => LastValue = testEvent.Result;

        public string LastValue
        {
            get => GetValue<string>();
            private set => SetValue(value);
        }

        public void SetLastValue(string value) => ApplyChange(new TestEvent(IdField, Version, value));
    }
}