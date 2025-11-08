using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceReservation.Application.DTOs
{
    public class ResponseErrorReservation
    {
        string MessageErrorClient { get; set; } = string.Empty;
        string MessageErrorDriver { get; set; } = string.Empty;
        string DetailError { get; set; } = string.Empty;
        string Suggest { get; set; } = string.Empty;
        string IdDriver { get; set; } = string.Empty;
        string IdClient { get; set; } = string.Empty;
        int CodeStatus { get; set; }
    }
}