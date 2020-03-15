using FluentValidation;
using STP.Profile.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace STP.Profile.Infrastructure.Validation.TraderInfo
{
    public class TraderInfoEntityValidation : AbstractValidator<TraderInfoEntity>
    {
        public TraderInfoEntityValidation()
        {
            RuleFor(ti => ti.Id)
                .NotEmpty();

            RuleFor(ti => ti.CopyCount)
                .GreaterThanOrEqualTo(0);

            RuleFor(ti => ti.ProfitLoss)
                .GreaterThanOrEqualTo(0);
        }
    }
}
