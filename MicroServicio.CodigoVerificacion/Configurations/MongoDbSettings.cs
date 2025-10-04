using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroServicio.CodigoVerificacion.Configurations
{
   public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string Database { get; set; } = null!;
    }
}