using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.Core.DTOs
{
    public class RegisterResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public string TypeUser { get; set; } = string.Empty;
    }
}