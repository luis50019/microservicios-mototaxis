using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Core.Entities;

namespace AuthService.UseCases.Contributors.Create
{
    public static class CreateDriver
    {
        public static Driver NewDriver(String name, Phone phone, string password,string stateDriver)
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            var newDriver = new Driver
            {
                BasicInfo = new BasicInfo
                {
                    Name = name,
                    Password = passwordHash,
                    Phone = phone
                },
                StateDriver = stateDriver,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            return newDriver;
        }
    }
}