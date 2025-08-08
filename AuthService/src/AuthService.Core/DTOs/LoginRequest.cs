using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Core.Entities;

namespace AuthService.Core.DTOs
{
    public class LoginRequest
    {
        public Phone Phone { get; set; } = new Phone();

        [
            MinLength(6, ErrorMessage = "minimo 6 caracteres"),
            MaxLength(15, ErrorMessage = "maximo de 15 caracteres")
        ]
        public string password { get; set; } = string.Empty;
    }
}