using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.UseCases.Exceptions
{
    public class PhoneAlreadyExistException:Exception
    {
        public PhoneAlreadyExistException(string phone)
        :base($"El numero de telefono {phone} ya esta registrado")
        {
            
        }
    }
}