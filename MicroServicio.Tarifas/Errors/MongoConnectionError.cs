using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroServicio.Tarifas.Errors
{
    public class MongoConnectionError
    {
        public string Details { get; set; }

        public MongoConnectionError(string message, string details) : base(message)
        {
            Details = details;
        }
    }
}