using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroServicio.Conductores.Data;
using MicroServicio.Reservaciones.DTOs;
using MicroServicio.Reservaciones.models;
using MicroServicio.Reservaciones.utils;
using MongoDB.Bson;

namespace MicroServicio.Reservaciones.useCases.create
{
    public static class ReservationCase
    {
        public static ResponseReservation CreateResponseReservation(Reservation reservation, Driver driver)
        {
            return new ResponseReservation
            {
                IdReservation = reservation.Id.ToString(),
                IdClient = reservation.Passage.ToString(),
                IdDriver = reservation.Driver.ToString(),
                CodeVerification = reservation.Security.CodeVerification,
                InfoDriver = new InfoDriver
                {
                    idDriver = reservation.Driver.ToString(),
                    LicensePlate = driver.Unit.LicensePlate,
                    name = driver.BasicInfo.Name,
                    numberUnit = driver.Unit.Number,
                    Phone = driver.BasicInfo.Phone.Number,
                    PhotoDriver = driver.BasicInfo.ProfilePicture,
                },

            };

        }
        public static Reservation CreateReservation(RequestReservations request)
        {
            string code = VerificationCode.GenerarCodigo();

            return new Reservation
            {
                Driver = ObjectId.Parse(request.infoDriver.data.id),
                Rate = ObjectId.Parse(request.infoDriver.data.rideFare.fareinfo.FareId),
                Route = new Route
                {
                    Start = new Coordinate
                    {
                        Lat = request.infoDriver.data.locationStart.Lat.Value,
                        Lng = request.infoDriver.data.locationStart.Lng.Value,
                    },
                    Destination = new Coordinate
                    {
                        Lat = request.infoDriver.data.locationEnd.Lat.Value,
                        Lng = request.infoDriver.data.locationEnd.Lng.Value,
                    },
                    Distance = request.infoDriver.data.rideFare.fareinfo.DistanceMax
                },
                NumberPassage = 1,//TODO: falta enviar como una opcion desde que se crea la reservacion
                Passage = ObjectId.Parse(request.infoDriver.data.rideFare.idUser),
                State = new State
                {
                    General = "En curso",
                    Details = new StateDetails
                    {
                        Detail = "Conductor en camino",
                        SpacenNumber = 0,
                    }
                },
                Security = new Security
                {
                    CodeVerification = code,
                    IsVerified = false
                },
                Comments = new Comments
                {
                    Rating = new Rating //* Despues se creara un servicio para modificar estas estadisticas 
                    //* por el momento se dejan con valores base
                    {
                        Overall = 2,
                        Categories = new RatingCategories
                        {
                            Punctuality = 2,
                            Driving = 2,
                            Vehicle = 2
                        }
                    }
                },
                Pay = new Pay
                {
                    Methodo = "efectivo",
                    State = "Pendiente"
                },
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

        }
    }
}