using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceLocation.Domain.Entities;

namespace ServiceLocation.Application.DTOs
{
    public class ConnectionUserDTO
    {
        public string IdUser { get; set; }
        public Coordinates LocationUser { get; set; }
        public string TypeUser { get; set; }
    }
}