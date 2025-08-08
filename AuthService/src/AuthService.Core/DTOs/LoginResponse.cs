using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.Core.DTOs
{
    public class LoginResponse
    {
        public string? Id { get; set; }
        public string? token { get; set; }
        public string? Type { get; set; }
    }
}