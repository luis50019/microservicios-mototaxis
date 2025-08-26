using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceLocation.Application.Exceptions
{
    public class UpdateException : Exception
    {
        public object error { get; }
        public UpdateException(object error) : base("Error al actualizar la ubicacion")
        {
            this.error = error;
        }
        
    }
}