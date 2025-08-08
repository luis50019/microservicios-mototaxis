using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Core.DTOs;

namespace AuthService.UseCases.Interfaces
{
    public interface ITokenGenerator
    {
        string GenerateToken(RegisterRequest dto);

    
    }
}