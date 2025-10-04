using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MicroServicio.Reservaciones.models
{
    public class Reservation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("passage")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Passage { get; set; }

        [BsonElement("numberPassage")]
        public int? NumberPassage { get; set; }

        [BsonElement("driver")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Driver { get; set; }

        [BsonElement("rate")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Rate { get; set; }

        [BsonElement("route")]
        public Route Route { get; set; }

        [BsonElement("state")]
        public State State { get; set; }

        [BsonElement("security")]
        public Security Security { get; set; }

        [BsonElement("comments")]
        public Comments Comments { get; set; }

        [BsonElement("pay")]
        public Pay Pay { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }

    public class Route
    {
        [BsonElement("destination")]
        public Coordinate Destination { get; set; }

        [BsonElement("start")]
        public Coordinate Start { get; set; }

        [BsonElement("distance")]
        public double Distance { get; set; }
    }

    public class Coordinate
    {
        [BsonElement("lat")]
        public double Lat { get; set; } = 0.0;

        [BsonElement("lng")]
        public double Lng { get; set; } = 0.0;
    }

    public class State
    {
        [BsonElement("general")]
        public string General { get; set; }

        [BsonElement("details")]
        public StateDetails Details { get; set; }
    }

    public class StateDetails
    {
        [BsonElement("detail")]
        public string Detail { get; set; }

        [BsonElement("spacenNumber")]
        public int? SpacenNumber { get; set; }
    }

    public class Security
    {
        [BsonElement("codeVerification")]
        public string CodeVerification { get; set; }
        public bool IsVerified { get; set; }
    }

    public class Comments
    {
        [BsonElement("rating")]
        public Rating Rating { get; set; }
    }

    public class Rating
    {
        [BsonElement("overall")]
        public int? Overall { get; set; }

        [BsonElement("categories")]
        public RatingCategories Categories { get; set; }
    }

    public class RatingCategories
    {
        [BsonElement("punctuality")]
        public int? Punctuality { get; set; }

        [BsonElement("vehicle")]
        public int? Vehicle { get; set; }

        [BsonElement("driving")]
        public int? Driving { get; set; }
    }

    public class Pay
    {
        [BsonElement("methodo")]
        public string Methodo { get; set; }

        [BsonElement("state")]
        public string State { get; set; }
    }
}