using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Core.DTOs;
using AuthService.Core.Entities;

namespace AuthService.UseCases.Contributors.Create
{
    public static class CreateUser
    {
        public static User NewUSer(string name, Phone phone, string password,string urlPhoto)
        {

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var newUser = new User
            {
                BasicInfo = new BasicInfo
                {
                    Name = name,
                    Phone = phone,
                    Password = passwordHash,
                    ProfilePicture = urlPhoto,
                },
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            return newUser;
        }
    }
}