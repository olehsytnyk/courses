using STP.Identity.Domain.Entities;
using STP.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace STP.Identity.Infrastructure.Abstraction
{
    public interface IUserAvatarRepository : IRepository<UserAvatar, long>
    {
        Task<UserAvatar> GetUserAvatarByUserIdAsync(string userId);
        Task UpdataUserAvatarAsync(string userId, string fileName, string Extension);
    }
}
