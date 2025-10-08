using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroServicio.Conductores.Erros
{
    public class DriverNull : Exception
    {
        public string details { get; set; }
        public DriverNull(string message, string detail) : base(message)
        {
            this.details = detail;
        }
    }
}