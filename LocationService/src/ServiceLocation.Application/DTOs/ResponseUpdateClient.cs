using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceLocation.Domain.Entities;

namespace ServiceLocation.Application.DTOs
{
    public class ResponseUpdateClient
    {
        public Coordinates? NewLocation { get; set; }
        public string? Message { get; set; }
    }
}