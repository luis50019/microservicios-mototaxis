using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroServicio.Reservaciones.DTOs;
using MicroServicio.Reservaciones.models;

namespace MicroServicio.Reservaciones.UseCases
{
    public class CreateReservations
    {

        //? Metodo para generar un objeto de reservacion
        public Reservation createNewReservation(RequestReservations newReservation)
        {


            return new Reservation();
        }

    }
}