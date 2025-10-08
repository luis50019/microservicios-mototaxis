using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroServicio.ValidarCodigoVerificacion.DTOs
{
    public class RequestValidateCode
    {

        public string codeVerification { get; set; } = string.Empty;
        public string idDriver { get; set; } = string.Empty;
        public string idClient { get; set; } = string.Empty;
        public string idReservation { get; set; } = string.Empty;
    }
}