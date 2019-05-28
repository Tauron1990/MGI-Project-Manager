namespace Tauron.CQRS.Common.Dto.Health
{
    public class HealthData
    {
        public long AllRequests { get; set; }

        public int RequestsPerHouer { get; set; }
    }
}