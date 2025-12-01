using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceLocation.Domain.Entities
{
    public class UserRedis
    {
        public string Id { get; set; } = "";
        public string TypeUser { get; set; } = "";
        public string State { get; set; } = "";
        public string ConnectionString { get; set; } = "";
        public string? ConnectionClient { get; set; } = "";
    }

}