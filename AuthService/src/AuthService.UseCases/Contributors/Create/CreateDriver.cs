using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Core.Entities;

namespace AuthService.UseCases.Contributors.Create
{
    public static class CreateDriver
    {
        public static Driver NewDriver(String name, Phone phone, string password,string stateDriver,string urlPhoto,double numberUnit, string LicensePlate)
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            var newDriver = new Driver
            {
                BasicInfo = new BasicInfo
                {
                    Name = name,
                    Password = passwordHash,
                    Phone = phone,
                    ProfilePicture = urlPhoto,
                },
                Performance = new Performance
                {
                    AcceptanceRate = 0,
                    TotalTrips = 0,
                    CanceledTrips = 0,
                    AverageResponseTime = 0,
                    CompletedTrips = 0,
                    HistoricalAvailability = 0,
                    TotalEarnings = 0
                },
                Unit = new Unit
                {
                    LicensePlate = LicensePlate,
                    Number = numberUnit,
                    PassengerLimit = 3,
                    LuggageCapacity = "",
                    Status = "buen estado"
                },
                StateDriver = stateDriver,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            return newDriver;
        }
    }
}