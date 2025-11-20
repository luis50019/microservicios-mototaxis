using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceReservation.Application.DTOs
{
    public class ResponseErrorRideFare
    {
        public string MessageError { get; set; } = string.Empty;
        public string DetailError { get; set; } = string.Empty;
        public string Suggest { get; set; } = string.Empty;
        public string IdClient { get; set; } = string.Empty;
        public int CodeStatus { get; set; } 
    }
}