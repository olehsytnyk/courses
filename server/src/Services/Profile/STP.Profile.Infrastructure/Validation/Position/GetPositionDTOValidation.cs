using FluentValidation;
using STP.Profile.Domain.DTO.Position;

namespace STP.Profile.Infrastructure.Validation
{
    public class GetPositionDTOValidation : AbstractValidator<GetPositionDTO>
    {
        public GetPositionDTOValidation()
        {
            RuleFor(p => p.Id)
                .GreaterThan(0)
                .WithMessage("{PropertyName} must be greater than 0");

            RuleFor(p => p.MarketId)
                .GreaterThan(0)
                .WithMessage("{PropertyName} must be greater than 0");

            RuleFor(p => p.UserId)
                .NotEmpty()
                .WithMessage("{PropertyName} must be not null and not empty");

            RuleFor(p => p.Kind)
                .IsInEnum()
                .WithMessage("Action is out of range");

            RuleFor(p => p.Quantity)
                .GreaterThan(0)
                .WithMessage("{PropertyName} must be greater than 0");

            RuleFor(p => p.AveragePrice)
                .GreaterThan(0)
                .WithMessage("{PropertyName} must be greater than 0");

            RuleFor(p => p.ProfitLoss)
                .GreaterThan(0)
                .WithMessage("{PropertyName} must be greater than 0");

            RuleFor(p => p.Timestamp)
                .GreaterThan(0)
                .WithMessage("{PropertyName} must be greater than 0");

            RuleFor(p => p.EntryOrderId)
                .GreaterThan(0)
                .WithMessage("{PropertyName} must be greater than 0");

        }
    }
}
