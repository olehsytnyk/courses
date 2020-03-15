using STP.Identity.Domain.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace STP.Identity.Infrastructure.Services
{
    public interface IUserManagerService
    {
        Task<User[]> FullTextSearchAsync(string text);

        Task<User[]> GetAllAsync(int page, int pageSize);
    }
}
