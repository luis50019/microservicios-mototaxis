using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AuthService.Core.Entities
{
    public class Driver
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        public BasicInfo BasicInfo { get; set; } = new();
        public string? StateDriver { get; set; }

        public Security Security { get; set; } = new();

        public double? Rating { get; set; }
        public TypesComment TypesComment { get; set; } = new();

        public Unit Unit { get; set; } = new();

        public string? ProfilePhoto { get; set; }

        public Performance Performance { get; set; } = new();

        public Operation Operation { get; set; } = new();

        public Location Location { get; set; } = new();

        public Preferences Preferences { get; set; } = new();

        public List<Reminder> Reminders { get; set; } = new();

        public Device Device { get; set; } = new();
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreatedAt { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime UpdatedAt { get; set; }
    }
    public class License
    {
        public string? Number { get; set; }
        public DateTime? Expiration { get; set; }
        public string? PhotoUrl { get; set; }
    }

    public class VehicleInsurance
    {
        public string? Number { get; set; }
        public DateTime? Validity { get; set; }
        public string? Coverage { get; set; }
    }

    public class BackgroundCheck
    {
        public string? Status { get; set; }
        public DateTime? VerificationDate { get; set; }
    }

    public class TypesComment
    {
        public int? ExellentComments { get; set; }
        public int? GoodGrades { get; set; }
        public int? NeutralComments { get; set; }
        public int? RegularCommnets { get; set; }
        public int? BadComments { get; set; }
    }

    public class Unit
    {
        public string? Number { get; set; }
        public string? Type { get; set; }
        public string? LuggageCapacity { get; set; }
        public int? PassengerLimit { get; set; }
        public string? Status { get; set; }
    }

    public class Performance
    {
        public int? TotalTrips { get; set; }
        public double? TotalEarnings { get; set; }
        public int? CanceledTrips { get; set; }
        public int? CompletedTrips { get; set; }
        public double? AverageResponseTime { get; set; }

        [Range(0, 100)]
        public double? AcceptanceRate { get; set; }

        [Range(0, 100)]
        public double? HistoricalAvailability { get; set; }
    }

    public class Operation
    {
        public List<Schedule> Schedules { get; set; } = new();
        public List<string> ActiveZones { get; set; } = new();
        public Rate Rates { get; set; } = new();
    }

    public class Rate
    {
        public double? Base { get; set; }
        public double? PerKm { get; set; }
        public double? PerMinute { get; set; }
    }
    public class LocationHistory
    {
        public Coordinates Coordinates { get; set; } = new();
        public DateTime? Timestamp { get; set; }
    }

    public class Preferences
    {
        public List<string> Tags { get; set; } = new();
        public string? RideMode { get; set; }
    }

    public class Reminder
    {
        public string? Type { get; set; }
        public string? Title { get; set; }
        public DateTime? Date { get; set; }
        public bool? Completed { get; set; }
    }

    public class Device
    {
        public DateTime? LastConnection { get; set; }
        public string? OperatingSystem { get; set; }
        public string? AppVersion { get; set; }
    }
}