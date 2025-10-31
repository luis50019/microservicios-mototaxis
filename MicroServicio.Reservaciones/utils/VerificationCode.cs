using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MicroServicio.Reservaciones.utils
{
    public class VerificationCode
    {
        public static string GenerarCodigo()
        {
            // Crea un nuevo GUID
            Guid guid = Guid.NewGuid();
            string codigoVerificacion = guid.ToString("N").Substring(0, 6);

            return codigoVerificacion;
        }
    }
}