using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Core.DTOs;
using AuthService.Core.Entities;

namespace AuthService.Core.Interfaces
{
    public interface IUserRepository
    {
        Task AddAsync(User newUser);
        Task AddAsyncDriver(Driver driver);
        Task<LoginResult> GetUser(Phone phone, string password);
        Task<User?> GetUserById(string id);
        Task<bool> ExistPhoneAsync(string phone);
    }


}