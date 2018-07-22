using JetBrains.Annotations;

namespace Tauron.Application.ProjectManager.Services.DTO
{
    [PublicAPI]
    public enum PrecisionMode
    {
        Perfect,
        NearCorner,
        AmountMismatchPerfect,
        AmountMismatchNearCorner,
        NoData,
        InValid
    }
}