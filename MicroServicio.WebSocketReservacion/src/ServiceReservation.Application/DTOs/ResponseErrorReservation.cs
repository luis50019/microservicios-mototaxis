using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceReservation.Application.DTOs
{
    public class ResponseErrorReservation
    {
        public string MessageErrorClient { get; set; } = string.Empty;
        public string MessageErrorDriver { get; set; } = string.Empty;
        public string DetailError { get; set; } = string.Empty;
        public string Suggest { get; set; } = string.Empty;
        public string IdDriver { get; set; } = string.Empty;
        public string IdClient { get; set; } = string.Empty;
        public int CodeStatus { get; set; }
    }
}