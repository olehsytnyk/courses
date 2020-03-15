using FluentValidation;
using STP.Profile.Domain.FilterModels;

namespace STP.Profile.Infrastructure.Validation
{
    public class PositionFilterModelValidation : AbstractValidator<PositionFilterModel>
    {
        public PositionFilterModelValidation()
        {
            RuleFor(pfm => pfm.UserId)
                .NotEqual(string.Empty)
                .WithMessage("{PropertyName} must be null, not empty");
        }
    }
}
