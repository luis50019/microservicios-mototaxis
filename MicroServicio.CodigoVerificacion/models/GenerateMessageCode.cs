using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroServicio.CodigoVerificacion.models
{
    public class CodigoGeneradoMessage
    {
        public string Code { get; set; }
        public string IdViaje { get; set; }
        public string IdClient { get; set; }
        public InfoDriver DataDriver { get; set; }
    }

    public class InfoDriver
    {
        public string idDriver { get; set; }
        public string name { get; set; }
        public string PhotoDriver { get; set; }
        public string LicensePlate { get; set; }
        public string Phone { get; set; }
        public double? numberUnit { get; set; } = 0;
    }
}