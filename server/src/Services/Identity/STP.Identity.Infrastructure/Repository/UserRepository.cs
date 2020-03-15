using STP.Identity.Domain.DTOs.User;
using STP.Identity.Domain.Entities;
using STP.Identity.Infrastructure.Abstraction;
using STP.Identity.Persistence.Context;
using STP.Infrastructure.DataAccess;
using STP.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Dapper;
using System.Threading.Tasks;
using System.Linq;
using System.Collections;
using Microsoft.EntityFrameworkCore;

namespace STP.Identity.Infrastructure.Repository
{
    public sealed class UserRepository : BaseRepository<User, string>, IUserRepository
    {
        private readonly IDbConnection _db;

        public UserRepository(ApplicationDbContext context, IDbConnection db) : base(context)
        {
            UnitOfWork = context;
            _db = db;
        }

        public override IUnitOfWork UnitOfWork
        {
            get;
            protected set;
        }

        public async Task<User[]> FullTextSearchAsync(string text)
        {
            string sql = $@"SELECT * FROM aspnetusers
                         WHERE MATCH(UserName, FirstName, LastName, Email) AGAINST('{text}*' IN BOOLEAN MODE)";

            var result = await _db.QueryAsync<User>(sql);

            return result.ToArray();
        }
    }
}
