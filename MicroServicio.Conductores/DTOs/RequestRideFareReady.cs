using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroServicio.Conductores.Data;

namespace MicroServicio.Conductores.DTOs
{
    public class RequestRideFareReady
    { 
        public Coordinates locationStart { get; set; }
        public Coordinates locationEnd { get; set; }
        public double priceTraveled { get; set; } //*costo del viaje
        public InfoPassenger infoPassenger { get; set; }
        public string typeService { get; set; } = string.Empty;
        public InfoRideFare fare { get; set; } = new InfoRideFare();
    }

    public class InfoRideFare
    {
        public string idUser { get; set; } = string.Empty;// id del cliente que solicita el viaje
        public Fare fareinfo { get; set; } = new Fare();    
    }

    public class InfoPassenger
    {
        public string urlFoto { get; set; }
        public string nombre { get; set; }
        public string phone { get; set; }
        public string lada { get; set; }
    }


}