using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace MicroServicio.CodigoVerificacion.DTOs
{
    public class RequestCode
    {
        public string idReservations { get; set; }
        public string idClient { get; set; }
        public string idDriver { get; set; }
    }
}