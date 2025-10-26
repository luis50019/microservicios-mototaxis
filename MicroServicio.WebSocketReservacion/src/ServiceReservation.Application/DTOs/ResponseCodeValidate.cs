using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceReservation.Application.DTOs
{
    public class ResponseValidateCode
    {
        public string idReservation { get; set; } = string.Empty;
        public string idClient { get; set; } = string.Empty;
        public string idDriver { get; set; } = string.Empty;
        public bool isCorrect { get; set; } = false;
        public string Message { get; set; } = string.Empty;
    }
}