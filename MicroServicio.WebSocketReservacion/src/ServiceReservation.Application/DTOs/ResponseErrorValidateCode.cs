using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceReservation.Application.DTOs
{
    public class ResponseErrorValidateCode
    {
        string MessageError { get; set; } = string.Empty;
        string DetailError { get; set; } = string.Empty;
        string Suggest { get; set; } = string.Empty;
        string IdDriver { get; set; } = string.Empty;
        int CodeStatus { get; set; }
    }
}