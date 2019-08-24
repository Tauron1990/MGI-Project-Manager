using CQRSlite.Commands;

namespace EventDeliveryTest.Test
{
    public class TestCommand : ICommand
    {
        public string Parameter { get; set; }
    }
}