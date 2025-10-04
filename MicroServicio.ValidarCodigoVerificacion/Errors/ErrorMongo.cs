using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroServicio.ValidarCodigoVerificacion.Errors
{
    public class ErrorMongo:Exception
    {
        public string details { get; set; }
        public ErrorMongo(string message, string details) : base(message)
        {
            this.details = details;
        }
    }
}