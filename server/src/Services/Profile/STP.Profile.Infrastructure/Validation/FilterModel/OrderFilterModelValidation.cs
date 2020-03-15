using FluentValidation;
using STP.Profile.Domain.FilterModels;

namespace STP.Profile.Infrastructure.Validation
{
    public class OrderFilterModelValidation : AbstractValidator<OrderFilterModel>
    {
        public OrderFilterModelValidation()
        {
            RuleFor(ofm => ofm.UserId)
                .NotEqual(string.Empty)
                .WithMessage("{PropertyName} must be null, not empty");

            RuleFor(ofm => ofm.Action)
                .IsInEnum()
                .WithMessage("{PropertyName} is out of range");
        }
    }
}