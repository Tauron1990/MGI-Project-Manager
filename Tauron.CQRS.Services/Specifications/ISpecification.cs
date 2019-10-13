using System.Threading.Tasks;

namespace Tauron.CQRS.Services.Specifications
{
    public interface ISpecification
    {
        string Message { get; }

        Task<bool> IsSatisfiedBy(object obj);
    }
}