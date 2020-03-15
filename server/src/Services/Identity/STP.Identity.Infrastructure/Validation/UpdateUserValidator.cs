using FluentValidation;
using STP.Identity.Domain.DTOs.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace STP.Identity.Infrastructure.Validation
{
    public class UpdateUserValidatior : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserValidatior()
        {
            RuleFor(p => p.UserName)
                .NotEmpty()
                .Length(5, 20)
                .WithMessage("Username must has minimum 5 and maximum 20 characters")
                .Matches(@"^[a-zA-Z][a-zA-Z0-9]")
                .WithMessage("Username must start with a letter");

            When(x => x.FirstName != null, () => {
                RuleFor(p => p.FirstName)
                .Length(2, 20)
                .WithMessage("FirstName must has minimum 2 and maximum 20 characters")
                .Matches(@"[a-zA-Z]")
                .WithMessage("FirstName must be consist of latin letter");
                });

            When(x => x.LastName != null, () =>
            {
                RuleFor(p => p.LastName)
                .Length(2, 20)
                .WithMessage("LastName must has minimum 2 and maximum 20 characters")
                .Matches(@"[a-zA-Z]")
                .WithMessage("LastName must be consist of latin letter");
            });

            RuleFor(p => p.Gender).IsInEnum();
        }
    }
}
