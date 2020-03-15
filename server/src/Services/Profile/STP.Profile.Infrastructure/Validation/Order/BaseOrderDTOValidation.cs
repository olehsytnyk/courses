using FluentValidation;
using STP.Profile.Domain.DTO.Order;

namespace STP.Profile.Infrastructure.Validation
{
    public class BaseOrderDTOValidation : AbstractValidator<BaseOrderDTO>
    {
        public BaseOrderDTOValidation()
        {
            RuleFor(o => o.MarketId)
                .GreaterThan(0)
                .WithMessage("{PropertyName} must be greater than 0");

            
            RuleFor(o => o.Quantity)
                .GreaterThan(0)
                .WithMessage("{PropertyName} must be greater than 0");


            RuleFor(o => o.Action)
                .IsInEnum()
                .WithMessage("Action is out of range");

        }
    }
}
