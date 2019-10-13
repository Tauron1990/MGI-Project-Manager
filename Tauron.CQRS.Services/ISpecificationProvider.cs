using System.Threading.Tasks;
using CQRSlite.Messages;
using Tauron.CQRS.Services.Specifications;

namespace Tauron.CQRS.Services
{
    public interface ISpecificationProvider 
    {
        ISpecification Specification(IMessage mag);

        Task Error(IMessage msg, string message);
    }
}