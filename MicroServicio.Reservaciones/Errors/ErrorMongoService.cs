using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroServicio.Reservaciones.Errors
{
    public class ErrorMongoService : Exception
    {
        public int CodeStatus { get; set; }
        public string Detail { get; set; }
        public ErrorMongoService(int CodeStatus, string Detail): base("Error MongoService")
        {
            this.CodeStatus = CodeStatus;
            this.Detail = Detail;           
        }        
    }
}