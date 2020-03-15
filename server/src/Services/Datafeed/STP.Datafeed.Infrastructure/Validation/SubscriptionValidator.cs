using FluentValidation;
using STP.Common.Models;
using STP.Datafeed.Application.Abstractions;

namespace STP.Datafeed.Infrastructure.Validation
{
    public class SubscriptionValidator : AbstractValidator<RealtimeMarketIdsDto>
    {
        public  SubscriptionValidator(IGeneratorService generatorService)
        {
            RuleForEach(p => p.MarketIds)
                .Must(m => generatorService.Markets.ContainsKey(m))
                .WithMessage($"Not found Market, try other MarketId");
        }
    }
}
