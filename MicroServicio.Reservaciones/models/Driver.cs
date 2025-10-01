using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MicroServicio.Reservaciones.models
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
    public class BasicInfo
    {
        public string Name { get; set; } = null!;
        public string Password { get; set; } = null!;
        public Email Email { get; set; } = new Email();
        public Phone Phone { get; set; } = new Phone();
        public int? Age { get; set; }
        public string? ProfilePicture { get; set; }
        public string LanguagePreference { get; set; } = "es_MX";
    }


    public class Email
    {
        public string? Address { get; set; }
        public bool Verified { get; set; }
    }

    public class Phone
    {
        public string Number { get; set; } = null!;
        public string CountryCode { get; set; } = "+52";
        public bool Verified { get; set; }
    }

    public class AuthenticationMethod
    {
        public AuthType Type { get; set; }
        public string? ExternalId { get; set; }
    }

    public enum AuthType
    {
        Google,
        Facebook,
        Email
    }

    public class EmergencyContact
    {
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public Relationship? Relationship { get; set; }
    }

    public enum Relationship
    {
        Family,
        Friend
    }

    public class Location
    {
        public CurrentLocation Current { get; set; } = new();
        public List<FrequentPlace> FrequentPlaces { get; set; } = new();
    }

    public class CurrentLocation
    {
        public Coordinates Coordinates { get; set; } = new();
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }

    public class FrequentPlace
    {
        public FrequentPlaceAlias? Alias { get; set; }
        public Coordinates Coordinates { get; set; } = new();
        public double? GeofenceRadius { get; set; }
    }

    public enum FrequentPlaceAlias
    {
        Home,
        Work
    }

    public class Coordinates
    {
        public double Lat { get; set; } = 0.0;
        public double Lng { get; set; } = 0.0;
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

    public class Schedule
    {
        public List<DayOfWeekEnum> Days { get; set; } = new();
        public string? DepartureTime { get; set; }
        public int? ToleranceMinutes { get; set; }
    }

    public enum DayOfWeekEnum
    {
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
        Sunday
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