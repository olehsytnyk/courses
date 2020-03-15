using System;

namespace STP.Profile.Domain.FilterModels
{
    public class TraderInfoFilterModel : BaseFilterModel
    {
        public double? ProfitLoss { get; set; }
        public DateTime? LastChanged { get; set; }
        public int? CopyCount { get; set; }
    }
}
