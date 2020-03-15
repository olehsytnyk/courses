using STP.Identity.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace STP.Identity.Infrastructure.Abstraction
{
  
    public interface IUserAvatarManagerService
    {
        Task<UserAvatar> GetByUserIdAsync(string userId);
        Task UpdateAvatarAsync(string userId, string fileName, string extension);
    }
}

