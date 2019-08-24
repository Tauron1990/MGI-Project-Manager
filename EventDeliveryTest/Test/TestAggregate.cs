using System;
using CQRSlite.Domain;
using Tauron.CQRS.Services;

namespace EventDeliveryTest.Test
{
    public class TestAggregate : CoreAggregateRoot
    {
        public static readonly Guid IdField = new Guid("10FC8F67-4F4F-427B-A734-D6F2BD22A376");

        public TestAggregate()
        {
            Id = IdField;
        }

        public string LastValue
        {
            get => GetValue<string>();
            private set => SetValue(value);
        }

        public void SetLastValue(string value)
        {
            LastValue = value;
            ApplyChange(new TestEvent { Result = value });
        }
    }
}