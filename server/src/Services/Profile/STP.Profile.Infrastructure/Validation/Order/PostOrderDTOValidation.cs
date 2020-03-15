using FluentValidation;
using STP.Profile.Domain.DTO.Order;

namespace STP.Profile.Infrastructure.Validation
{
    public class PostOrderDTOValidation : AbstractValidator<PostOrderDTO>
    {
        public PostOrderDTOValidation()
        {
            Include(new BaseOrderDTOValidation());
        }
    }
}