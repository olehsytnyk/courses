using FluentValidation;
using STP.Markets.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STP.Markets.API.Validators {
    public class WatchlistPostDtoValidator: AbstractValidator<WatchlistPostDto> {
        public WatchlistPostDtoValidator() {
            RuleFor(w => w.Name).NotNull();
        }
    }
}
