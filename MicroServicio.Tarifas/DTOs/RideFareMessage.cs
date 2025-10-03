using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

/*
    TODO: Recibir la informacion del menasje procesarla y crear el metodo de mongo para busqueda de tarifa
    ? por el momento solo seria la busqueda de la distancia, despues implementar la busqueda de la tarifa por la ubicacion
    ? 
*/

namespace MicroServicio.Tarifas.DTOs
{
    public class RideFareMessage
    {
        public string IdUser { get; set; } = string.Empty;
        public double distanceTraveled { get; set; }
        public string locality { get; set; } = string.Empty;
        public string typeUSer { get; set; } = string.Empty;
    }
}