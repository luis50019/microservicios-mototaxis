using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using ServiceLocation.Application.DTOs;
using ServiceLocation.Application.Exceptions;
using ServiceLocation.Application.Interfaces;

namespace ServiceLocation.Presentation.HubLocation
{
    public class LocationHub : Hub
    {
        private readonly ILocationService _locationService;
        private readonly ICacheService _cacheService;
        public LocationHub(ILocationService locationService, ICacheService cacheService)
        {
            //?Inicializamos el servicio de ubicación
            _locationService = locationService ?? throw new ArgumentNullException(nameof(locationService));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        }

        //?Evento para detectar cuando un usuario se conecta al hub
        public override async Task OnConnectedAsync()
        {
            try
            {
                await base.OnConnectedAsync();
                Console.WriteLine($"Usuario conectado: {Context.ConnectionId}");
            }
            catch (System.Exception)
            {
                Console.WriteLine("Error en la conexión");
                throw;
            }
        }

        public async Task RegisterConnection(RequestUpdateLocation request)
        {
            try
            {
                var response = await _cacheService.SaveConnecttion(request, Context.ConnectionId);
                Console.WriteLine($"Conexión registrada: {Context.ConnectionId} >-> id: {request.Id}");
                await Clients.Client(response.ConnectionString).SendAsync("ConnectionRegistered", "Conexión registrada exitosamente");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine("Error al registrar la conexión registerConnection");
            }

        }

        public async Task LocationUpdate(RequestUpdateLocation request)
        {
            try
            {
                Console.WriteLine($"Ubicación actualizada: {request.newLocation.Lat}");
                await _locationService.UpdateLocationAsync(
                    request.Id,
                    request.TypeUser,
                    request.newLocation
                );

                var response = await _cacheService.UpdateLocation(request);

                //TODO: Enviar la ubicacion actualizada al cliente

                if (response.ConnectionClient == "" || request.IdClient == "")
                {
                    //* No se regresa las coordenadas a nadie
                    await Clients.Client(response.ConnectionString).SendAsync("UpdateOwner", "Ubicacion actualizada");
                }
                else
                {
                    await Clients.Client(response.ConnectionClient).SendAsync("Update", new ResponseUpdateClient
                    {
                        NewLocation =
                        {
                            Lat = request.newLocation.Lat,
                            Lng = request.newLocation.Lng
                        },
                        Message = "Nueva ubicacion"
                    });

                }

            }
            catch (UpdateException exUpd)
            {
                Console.WriteLine(exUpd.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al actualizar la ubicación === " + ex);
            }
        }


    }
}