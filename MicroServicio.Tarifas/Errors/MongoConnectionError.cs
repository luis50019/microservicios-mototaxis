using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroServicio.Tarifas.Errors
{
    public class MongoConnectionError : Exception
    {
        public string Details { get; }

        public MongoConnectionError(string message, string details) : base(message)
        {
            Details = details;
        }
    }
}