using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Grpc.Core;
using JetBrains.Annotations;

namespace Tauron.Application.Deployment.Server.Services.Validatoren
{
    [PublicAPI]
    public abstract class ValidatorBase<TData, TValidator> : AbstractValidator<TData>
        where TValidator : ValidatorBase<TData, TValidator>, new()
    {
        private static readonly TValidator Inst = new TValidator();

        public static void For(TData data)
        {
            var result = Inst.Validate(data);
            if (result.IsValid) return;

            throw new RpcException(new Status(StatusCode.InvalidArgument, result.Errors.First().ErrorMessage));
        }

        public static ValidationResult IsValid(TData data)
            => Inst.Validate(data);

        public static async Task ForAsync(TData data)
        {
            var result = await Inst.ValidateAsync(data);
            if (result.IsValid) return;

            throw new RpcException(new Status(StatusCode.InvalidArgument, result.Errors.First().ErrorMessage));
        }

        public static Task<ValidationResult> IsValidAsync(TData data)
            => Inst.ValidateAsync(data);
    }
}