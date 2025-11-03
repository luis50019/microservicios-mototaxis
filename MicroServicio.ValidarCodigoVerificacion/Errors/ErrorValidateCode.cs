using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroServicio.ValidarCodigoVerificacion.Errors
{
    public class ErrorValidateCode: Exception
    {
        public string MessageError { get; set; }
        public string DetailError { get; set; }
        public string Suggest { get; set; } = "Vuelva a intentar";
        public string IdDriver { get; set; }
        public int CodeStatus { get; set; }

        public ErrorValidateCode(int CodeStatus,string IdDriver,string MessageError,string DetailError):base("ErrorValidateCode")
        {
            this.CodeStatus = CodeStatus;
            this.DetailError = DetailError;
            this.MessageError = MessageError;
            this.IdDriver = IdDriver;
        }

    }
}