using STP.Identity.Domain.Entities;
using STP.Identity.Infrastructure.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using STP.Interfaces.Enums;
using STP.Common.Exceptions;

namespace STP.Identity.Infrastructure.Services
{
    public class UserAvatarManagerService : IUserAvatarManagerService
    {
        private readonly IUserAvatarRepository _repository;

        public UserAvatarManagerService(IUserAvatarRepository repository)
        {
            _repository = repository;
        }

        public async Task<UserAvatar> GetByUserIdAsync(string userId)
        {
            return await _repository.GetUserAvatarByUserIdAsync(userId);
        }

        public async Task UpdateAvatarAsync(string userId, string fileName, string extension)
        {
            await _repository.UpdataUserAvatarAsync(userId, fileName, extension);
            var result = await _repository.UnitOfWork.SaveChangesAsync();
            if (result < 0)
            {
                throw new InvalidDataException(ErrorCode.CannotUploadAvatar);
            }
        }
    }
}
