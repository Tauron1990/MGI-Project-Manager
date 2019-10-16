using System.Threading.Tasks;

namespace Tauron.CQRS.Services.Specifications
{
    public interface ISpecification
    {
        string Message { get; }

        bool IsSatisfiedBy(object obj);
    }
}