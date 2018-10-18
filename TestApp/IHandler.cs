using System.Threading.Tasks;

namespace Auto_Fan_Control.Bus
{
    public interface IHandler<in TMessage>
    {
        Task Handle(TMessage msg, MessageBus messageBus);
    }
}