using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AuthService.Core.Entities
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        public BasicInfo BasicInfo { get; set; } = new BasicInfo();
        public Security Security { get; set; } = new Security();
        public RidePreferences RidePreferences { get; set; } = new RidePreferences();
        public Location Location { get; set; } = new Location();
        public Reservations Reservations { get; set; } = new Reservations();
        public Reminders Reminders { get; set; } = new Reminders();
        public Settings Settings { get; set; } = new Settings();
        public Statistics Statistics { get; set; } = new Statistics();

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreatedAt { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime UpdatedAt { get; set; }
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

    public class Security
    {
        public List<AuthenticationMethod> AuthenticationMethods { get; set; } = new();
        public EmergencyContact? EmergencyContact { get; set; }
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

    public class RidePreferences
    {
        public List<ObjectId> FavoriteDrivers { get; set; } = new();
        public List<VehicleType> PreferredVehicles { get; set; } = new();
        public DefaultConfiguration DefaultConfiguration { get; set; } = new();
    }

    public enum VehicleType
    {
        Sedan,
        Suv,
        Motorcycle
    }

    public class DefaultConfiguration
    {
        public bool AirConditioning { get; set; }
        public bool Music { get; set; }
        public bool Conversation { get; set; }
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
        public double? Lat { get; set; }
        public double? Lng { get; set; }
    }

    public class Reservations
    {
        public List<AutomaticReservation> Automatic { get; set; } = new();

        // Puedes agregar propiedad para historial si decides implementarlo
    }

    public class AutomaticReservation
    {
        public string? Alias { get; set; }
        public Coordinates Destination { get; set; } = new();
        public Schedule Schedule { get; set; } = new();
        public ReservationConfiguration Configuration { get; set; } = new();
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

    public class ReservationConfiguration
    {
        public VehicleType Vehicle { get; set; }
        public bool ShareRide { get; set; }
    }

    public class Reminders
    {
        public List<ScheduledRide> ScheduledRides { get; set; } = new();

        // Puedes agregar pending payments si decides implementarlo
    }

    public class ScheduledRide
    {
        public Coordinates Destination { get; set; } = new();
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? DateTime { get; set; }
        public int NotifyMinutesBefore { get; set; } = 30;
        public ScheduledRideStatus Status { get; set; } = ScheduledRideStatus.Pending;
    }

    public enum ScheduledRideStatus
    {
        Pending,
        Completed
    }

    public class Settings
    {
        public Notifications Notifications { get; set; } = new();
        public Privacy Privacy { get; set; } = new();
    }

    public class Notifications
    {
        public RideNotification Ride { get; set; } = new();
        public PromotionNotification Promotions { get; set; } = new();
    }

    public class RideNotification
    {
        public bool Sms { get; set; } = true;
        public bool Push { get; set; } = true;
    }

    public class PromotionNotification
    {
        public bool Email { get; set; }
    }

    public class Privacy
    {
        public ShareLocation ShareLocation { get; set; } = ShareLocation.OnlyWhileRiding;
    }

    public enum ShareLocation
    {
        OnlyWhileRiding,
        Never
    }

    public class Statistics
    {
        public MonthlyRides MonthlyRides { get; set; } = new();
        public NightPreferences NightPreferences { get; set; } = new();
    }

    public class MonthlyRides
    {
        public int Total { get; set; }
        public double? AverageRating { get; set; }
    }

    public class NightPreferences
    {
        public List<VehicleType> NightVehicles { get; set; } = new();
        public string? CommonSchedule { get; set; }
    }
}
