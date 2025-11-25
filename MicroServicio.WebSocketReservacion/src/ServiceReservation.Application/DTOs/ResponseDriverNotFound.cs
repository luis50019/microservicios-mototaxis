using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceReservation.Application.DTOs
{
    public class ResponseDriverNotFound
    {
        public string Message { get; set; } = string.Empty;
        public string Suggest { get; set; } = string.Empty;
    }
}