using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroServicio.Conductores.Data;
using RabbitMQ.Client.Exceptions;

namespace MicroServicio.Conductores.DTOs
{
    public class DriverFound
    {
        public Coordinates? coordinates { get; set; } = new Coordinates();
        public string? id { get; set; } = string.Empty;
        public BasicInfo? infoBasic { get; set; } = new BasicInfo();
        public Unit? unit { get; set; } = new Unit();
        // Estado descriptivo del flujo (por ejemplo: "Conductor encontrado, esperando aceptacion")
        public string State { get; set; } = string.Empty;
        // Indica si fue posible encontrar/asignar un conductor
        public bool succes { get; set; } = false;
    }
}