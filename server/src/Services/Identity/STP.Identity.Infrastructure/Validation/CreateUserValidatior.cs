using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using STP.Identity.Domain.DTOs.User;

namespace STP.Identity.Infrastructure.Validation
{
    public class CreateUserValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserValidator()
        {
            RuleFor(p => p.UserName)
                .NotEmpty()
                .Length(5, 16)
                .WithMessage("Username must has minimum 5 and maximum 16 characters")
                .Matches(@"^[a-zA-Z][a-zA-Z0-9]")
                .WithMessage("Username must start with a letter");

            RuleFor(p => p.Email)
                .EmailAddress()
                .WithMessage("incorrect email address");

            RuleFor(p => p.Password)
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])")
                .WithMessage("Pawssword must has at least one lowercase letter and one uppercase letter")
                .Matches(@"^(?=.*\d)")
                .WithMessage("Pawssword must has at least one number")
                .Matches(@"^(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
                .WithMessage("Password must has at least one special character")
                .Length(8, 20)
                .WithMessage("Password must has minimum 8 and maximum 20 characters");

            RuleFor(p => p.Password)
                .Equal(p => p.PasswordConfirm)
                .WithMessage("Passwords don`t match");

        }
    }
}
