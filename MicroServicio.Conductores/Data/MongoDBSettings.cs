using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroServicio.Conductores.Data
{
    public class MongoDBSettings
    {
        public string ConnectionString { get; set; }
        public string Database { get; set; }
    }
}