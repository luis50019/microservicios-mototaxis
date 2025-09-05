using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroServicio.Tarifas.DTOs
{
    public class FareResponseMessage
    {
        public string IdUser { get; set; } = string.Empty;
        public bool Success { get; set; }
        public ResponseRideFare? Fare { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}