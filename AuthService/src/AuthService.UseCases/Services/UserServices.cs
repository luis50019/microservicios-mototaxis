using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Core.DTOs;
using AuthService.Core.Entities;
using AuthService.Core.Interfaces;
using AuthService.UseCases.Contributors.Create;
using AuthService.UseCases.Contributors.Validate;
using AuthService.UseCases.Exceptions;
using AuthService.UseCases.Interfaces;
using BCrypt.Net;

namespace AuthService.UseCases.Services
{
    public class UserServices : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly PhoneValidate _phoneValidate;

        public UserServices(IUserRepository userRepository,
         ITokenGenerator tokenGenerator,
         PhoneValidate phoneValidate)
        {
            _userRepository = userRepository;
            _tokenGenerator = tokenGenerator;
            _phoneValidate = phoneValidate;
        }


        //*Metodo para registrar un usuario
        public async Task<RegisterResponse> RegisterUser(RegisterRequest dto)
        {
            var newUser = CreateUser.NewUSer(dto.Name, dto.Phone, dto.password);
            var isExist = await _phoneValidate.PhoneExist(dto.Phone.Number);
            if (isExist)
            {
                throw new PhoneAlreadyExistException(dto.Phone.Number);
            }
            await _userRepository.AddAsync(newUser);

            var token = _tokenGenerator.GenerateToken(dto);

            return new RegisterResponse
            {
                Token = token.ToString(),
                Id = newUser.Id.ToString(),
                TypeUser = "client"
            };
        }

        //*Metodo para registrar a un conductor
        public async Task<RegisterResponse> RegisterDriver(RegisterRequest dto)
        {
            var newDriver = CreateDriver.NewDriver(dto.Name, dto.Phone, dto.password);
            var isExist = await _phoneValidate.PhoneExist(dto.Phone.Number);
            if (isExist)
            {
                throw new PhoneAlreadyExistException(dto.Phone.Number);
            }
            await _userRepository.AddAsyncDriver(newDriver);
            var token = _tokenGenerator.GenerateToken(dto);
            return new RegisterResponse
            {
                Token = token.ToString(),
                Id = newDriver.Id.ToString(),
                TypeUser = "driver"
            };
        }

        //*Metodo para el inicio de sesion

        public async Task<LoginResponse> LoginUser(LoginRequest dto)
        {
            var newUser = CreateUser.NewUSer("usuario", dto.Phone, dto.password);
            var info = await _userRepository.GetUser(dto.Phone, dto.password);

            if (!info.State)
            {
                Console.WriteLine(info.Error);
                throw new LoginException("Error usuario no encontrado");
            }
            var token = _tokenGenerator.GenerateToken(new RegisterRequest
            {
                Name = "",
                password = dto.password,
                Phone = dto.Phone
            });

            return new LoginResponse
            {
                Id = info.Id,
                token = token,
                Type = info.Type
            };

        }
    }
}