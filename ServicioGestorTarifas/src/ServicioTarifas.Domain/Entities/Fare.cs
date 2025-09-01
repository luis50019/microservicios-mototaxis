using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace ServicioTarifas.Domain
{

    //TODO: despues añadir la ubicacion de la tarifa, para saber a que municipio le pertenece la tarifa

    //!Define las tarifas dinamicas dependiendo de
    //?StarTime y EndTime
    //?ApplicableDays: Dias de la semana que se aplicaran
    //?Price: precio para esa franja
    //?IsActive: si esta en uso o no
    public class CustomFare
    {
        public string StartTime { get; set; } = string.Empty; // Format: "HH:MM"
        public string EndTime { get; set; } = string.Empty;
        public double Price { get; set; }
        public List<string> ApplicableDays { get; set; } = new List<string>(); // ["Mon", "Tue", ...]
        public bool IsActive { get; set; } = true;
    }

    //!Defina la tarifa por paradas adicionales durante un viaje
    //?PricePerStop: Cuanto cobra por cada parada
    //?MaxStopsAllowed: limite por paradas que se pueden hacer
    //?ISActive: si la regla esta activa
    public class StopFare
    {
        public double PricePerStop { get; set; }
        public int MaxStopsAllowed { get; set; } = 5;
        public bool IsActive { get; set; } = true;
    }

    //! Define una tarifa fija que aplica a todos los viajes, sin importar la hora del dia
    //? price:(precio global por km/viaje)
    //?IsActive( si esta activa o no)
    public class GlobalFare
    {
        public double? Price { get; set; }
        public bool IsActive { get; set; } = false;
    }

    //!Agrupa los tipos de tarifa que puedan existir
    //?Tarifas personalizadas, una tarifa global
    public class FareType
    {
        public GlobalFare Global { get; set; } = new GlobalFare();
        //!Despues en un futuro se añadira esta opcion
        //public List<CustomFare> Customized { get; set; } = new List<CustomFare>();
    }

    /*
    !Representa la tarifa general que se aplica a un viaje
    ?Por el momento solo estaremos manejando la distancia min y la distancia max y el precio
    ?Despues añadimos la funcion de poder personalizar las tarifas
    */
    public class Fare
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; } 

        public FareType FareType { get; set; } = new FareType();
        public StopFare StopFare { get; set; } = new StopFare();

        public List<string> AcceptedPaymentMethods { get; set; } = new List<string> { "cash" };

        public double DistanceMin { get; set; }
        public double DistanceMax { get; set; }
        public bool IsActive { get; set; } = true;

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        // Timestamps (similar to Mongoose { timestamps: true })
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
