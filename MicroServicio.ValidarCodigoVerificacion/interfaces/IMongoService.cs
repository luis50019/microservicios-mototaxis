using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroServicio.ValidarCodigoVerificacion.DTOs;

namespace MicroServicio.ValidarCodigoVerificacion.interfaces
{
    public interface IMongoService
    {
        public Task<ResponseValidateCode> validateCode(RequestValidateCode request);
    }
}