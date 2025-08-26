using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceLocation.Application.DTOs
{
    public class RequestGetInfo
    {
        public string Id { get; set; } = string.Empty;
        public string TypeUser { get; set; } = string.Empty;
    }
}