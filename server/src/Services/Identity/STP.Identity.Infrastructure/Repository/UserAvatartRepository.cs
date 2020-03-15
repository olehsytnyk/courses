using Microsoft.EntityFrameworkCore;
using STP.Identity.Domain.Entities;
using STP.Identity.Infrastructure.Abstraction;
using STP.Identity.Persistence.Context;
using STP.Infrastructure.DataAccess;
using STP.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace STP.Identity.Infrastructure.Repository
{
    public sealed class UserAvatarRepository : BaseRepository<UserAvatar, long>, IUserAvatarRepository
    {
        public UserAvatarRepository(ApplicationDbContext context) : base(context)
        {
            UnitOfWork = context;
        }

        public override IUnitOfWork UnitOfWork
        {
            get;
            protected set;
        }

        public async Task<UserAvatar> GetUserAvatarByUserIdAsync(string userId)
        {
            var item = await _entities.AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == userId);
            return item;
        }

        public async  Task UpdataUserAvatarAsync(string userId, string fileName, string extension)
        {
            var item = await _entities.AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == userId);
            item.FileName = fileName;
            item.FileExtension = extension;
            _entities.Update(item);
        }
    }
}
