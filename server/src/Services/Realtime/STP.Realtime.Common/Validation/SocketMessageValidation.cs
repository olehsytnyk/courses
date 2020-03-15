using FluentValidation;
using STP.Realtime.Common.WebSocketMessages;


namespace STP.Realtime.Common.Validation
{
    public class SocketMessageValidator : AbstractValidator<SocketMessage>
    {
        public SocketMessageValidator()
        {
            RuleFor(m => m.Action)
                .NotNull();

            RuleFor(m => m.RequestId)
                .NotNull()
                .NotEmpty();

            RuleFor(m => m.Subject)
                .NotNull();

            RuleFor(m => m.SubjectAction)
                .NotNull();

            RuleFor(m => m.SubjectId)
                .NotNull();
        }
    }
}
