using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceLocation.Application.DTOs;
using ServiceLocation.Application.Interfaces;
using ServiceLocation.Domain.Entities;
using ServiceLocation.Domain.Interfaces;

namespace ServiceLocation.Application.Services;

public class ConnectionUserService : ICacheService
{
  private readonly IUserRespository _userRespository;
  public ConnectionUserService(IUserRespository userRespository)
  {
    _userRespository = userRespository;
  }

  public async Task<UserRedis?> Disconnect(RequestUpdateLocation location)
  {
    try
    {
      return await _userRespository.DisconnectUser(location.Id, location.TypeUser, location.newLocation);
    }
    catch (Exception ex)
    {
      throw new Exception(ex.Message);
    }
  }

  public Task<UserRedis?> SaveConnecttion(RequestUpdateLocation updateLocation, string connectionId)
  {

    try
    {
      return _userRespository.SaveUserConnected(updateLocation.Id, updateLocation.TypeUser, updateLocation.newLocation, connectionId);
    }
    catch (Exception ex)
    {
      throw new Exception(ex.Message);
    }
  }

  public async Task<UserRedis?> UpdateLocation(RequestUpdateLocation locationUpdate)
  {
    try
    {
      var resposneRepository = await _userRespository.SetLocationUser(locationUpdate.Id, locationUpdate.newLocation, locationUpdate.IdClient);

      if (resposneRepository == null)
      {
        throw new Exception("Error the repository");
      }

      return resposneRepository;

    }
      catch (Exception ex)
    {
      throw new Exception(ex.Message);
    }
  }
}
