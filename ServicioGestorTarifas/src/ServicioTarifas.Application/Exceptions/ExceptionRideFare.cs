using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicioTarifas.Application.Exceptions
{
    public class ExceptionRideFare : Exception
    {
        public object info { get; set; }
        public ExceptionRideFare(string message,object errorInfo):base(message)
        {
            info = errorInfo;
        }
    }
}