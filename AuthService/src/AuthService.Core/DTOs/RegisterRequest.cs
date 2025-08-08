using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Core.Entities;

namespace AuthService.Core.DTOs
{
    public class RegisterRequest
    {
        [MinLength(6,ErrorMessage = "EL nombre debe tener al menos 6 caracteres"),MaxLength(15,ErrorMessage ="EL maximo de caracteres es de 15")]
        public string Name { get; set; } = string.Empty;
        public Phone Phone { get; set; } = new Phone();

        [MinLength(6,ErrorMessage ="La contraseña debe tener almenos 6 caracteres"),MaxLength(15,ErrorMessage ="El maximo de caracteres es de 15")]
        public string password { get; set; } = string.Empty;

    }
}