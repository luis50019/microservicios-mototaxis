using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceReservation.Application.DTOs
{

    public class ResponseDriverFound
    {
        public string Event { get; set; } = string.Empty;
        public Data Data { get; set; } = new Data();
    }
    public class Data
    {
        public string id { get; set; } = string.Empty;
        public Coordinates locationStart { get; set; } = new Coordinates();
        public Coordinates locationEnd { get; set; } = new Coordinates();
        public double priceTraveled { get; set; } //*costo del viaje
        public string client { get; set; } = string.Empty;
        public InfoPassenger infoPassager { get; set; } = new InfoPassenger();
        public Coordinates coordinates { get; set; } = new Coordinates();
        public InfoRideFare rideFare { get; set; } = new InfoRideFare();
        public string typeService { get; set; } = string.Empty;
    }
    

}

