using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Core.Interfaces;
using AuthService.UseCases.Exceptions;

namespace AuthService.UseCases.Contributors.Validate
{
    public class PhoneValidate
    {
        private readonly IUserRepository _userRepository;
        public PhoneValidate(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<bool> PhoneExist(string phone)
        {
            return await _userRepository.ExistPhoneAsync(phone);
        }
    }
}