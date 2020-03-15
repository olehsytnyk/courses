using FluentValidation;
using STP.Identity.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace STP.Identity.Infrastructure.Validation
{
    public class ChangePasswordValidator : AbstractValidator<ChangePasswordDto>
    {
        public ChangePasswordValidator()
        {
            RuleFor(c => c.OldPassword)
                .NotEmpty()
                .NotNull()
                .WithMessage("Old Password don't be null or empty and")
                .MaximumLength(20)
                .WithMessage("Old Password must has maximum 20 characters");


            RuleFor(p => p.NewPassword)
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])")
                .WithMessage("Pawssword must has at least one lowercase letter and one uppercase letter")
                .Matches(@"^(?=.*\d)") 
                .WithMessage("Pawssword must has at least one number")
                .Matches(@"^(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
                .WithMessage("Password must has at least one special character")
                .Length(8, 20)
                .WithMessage("Password must has minimum 8 and maximum 20 characters");

            RuleFor(p => p.NewPassword)
                .Equal(p => p.ConfirmNewPassword)
                .WithMessage("Passwords don`t match");
        }
    }
}
