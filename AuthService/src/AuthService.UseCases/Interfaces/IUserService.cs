using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Core.DTOs;

namespace AuthService.UseCases.Interfaces
{
    public interface IUserService
    {
        Task<RegisterResponse> RegisterUser(RegisterRequest dto);
        Task<RegisterResponse> RegisterDriver(RegisterRequest dto);
        Task<LoginResponse> LoginUser(LoginRequest dto);
    }
}