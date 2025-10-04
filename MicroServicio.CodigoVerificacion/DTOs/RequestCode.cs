using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroServicio.CodigoVerificacion.DTOs
{
    public class RequestCode
    {
        public string idReservations { get; set; }
        public string idClient { get; set; }
        public string idDriver { get; set; }
    }
}