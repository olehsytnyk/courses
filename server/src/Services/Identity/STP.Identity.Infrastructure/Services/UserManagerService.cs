using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using STP.Identity.Domain.Entities;
using STP.Identity.Infrastructure.Abstraction;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STP.Identity.Infrastructure.Services
{
    public class UserManagerService : UserManager<User>, IUserManagerService
    {
        IUserRepository _userRepository { get; set; }
        public UserManagerService(IUserStore<User> store, 
            IOptions<IdentityOptions> optionsAccessor, 
            IPasswordHasher<User> passwordHasher, 
            IEnumerable<IUserValidator<User>> userValidators, 
            IEnumerable<IPasswordValidator<User>> passwordValidators, 
            ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, 
            IServiceProvider services,
            ILogger<UserManager<User>> logger, 
            IUserRepository userRepository) 
                : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _userRepository = userRepository;
        }

        public async Task<User[]> GetAllAsync(int page, int pageSize)
        {
            var result = await _userRepository.GetAllAsync();
            var pageUsers = result.Skip((page - 1) * pageSize).Take(pageSize).ToArray();
            return pageUsers;
        }

        public async Task<User[]> FullTextSearchAsync(string text)
        {
            var result = await _userRepository.FullTextSearchAsync(text);
            return result;
        }
    }
}
