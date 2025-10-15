using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Core.Entities;

namespace AuthService.Core.DTOs
{
    public class LoginResult
    {
        public bool State { get; set; }
        public string? Id { get; set; }
        public string? Type { get; set; }
        public string nombre { get; set; } = string.Empty;
        public Phone phone { get; set; } = new();
        public string urlPhoto { get; set; } = string.Empty;
        public string? Error { get; set; }
    }

}