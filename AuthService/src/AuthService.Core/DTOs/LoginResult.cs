using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.Core.DTOs
{
    public class LoginResult
    {
        public bool State { get; set; }
        public string? Id { get; set; }
        public string? Type { get; set; }
        public string? Error { get; set; }
    }

}