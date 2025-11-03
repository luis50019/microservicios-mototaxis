using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroServicio.Reservaciones.Errors
{
    public class ErrorResevation: Exception
    {
        public string MessageErrorClient { get; set; } = "Ocurrio un error al registrar su reservacion";
        public string MessageErrorDriver { get; set; } = "Error al crear la reservacion";
        public string DetailError { get; set; }
        public string Suggest { get; set; }
        public string IdDriver { get; set; }
        public string IdClient { get; set; }
        public int CodeStatus { get; set; }

        public ErrorResevation(string DetailError, string Suggest, string IdDriver, string IdClient, int CodeStatus):base("ErrorReservation")
        {
            this.MessageErrorClient = MessageErrorClient;
            this.MessageErrorDriver = MessageErrorDriver;
            this.DetailError = DetailError;
            this.Suggest = Suggest;
            this.IdDriver = IdDriver;
            this.IdClient = IdClient;
            this.CodeStatus = CodeStatus;
        }
    }
}