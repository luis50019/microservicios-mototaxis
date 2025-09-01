using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroServicio.Tarifas.Models;

namespace MicroServicio.Tarifas.Services
{
    public interface IMongoService
    {
        Task<List<Fare>> GetAllFares();
    }
}