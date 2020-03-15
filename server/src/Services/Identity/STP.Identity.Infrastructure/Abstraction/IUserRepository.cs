using STP.Identity.Domain.Entities;
using STP.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace STP.Identity.Infrastructure.Abstraction
{
    public interface IUserRepository : IRepository<User, string>
    {
        Task<User[]> FullTextSearchAsync(string text);
    }
}
