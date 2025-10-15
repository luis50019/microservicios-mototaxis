using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Core.Entities;

namespace AuthService.Core.DTOs
{
    public class LoginResponse
    {
        public string? Id { get; set; }
        public string? token { get; set; }
        public string? Type { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public Phone phone { get; set; } = new();
        public string urlPhoto { get; set; } = string.Empty;

    }
}