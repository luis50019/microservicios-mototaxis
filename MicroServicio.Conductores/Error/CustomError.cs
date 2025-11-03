using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroServicio.Conductores.Errors
{
    public class CustomError : Exception
    {
        public string MessageError { get; set; }
        public string DetailError { get; set; }
        public string Suggest { get; set; }
        public string IdClient { get; set; }
        public int CodeStatus { get; set; }

        public CustomError(string MessageError, string DetailError, string Suggest, string IdClient, int CodeStatus): base(MessageError)
        {
            this.MessageError = MessageError;
            this.DetailError = DetailError;
            this.Suggest = Suggest;
            this.IdClient = IdClient;
            this.CodeStatus = CodeStatus;
        }

    }
    
}