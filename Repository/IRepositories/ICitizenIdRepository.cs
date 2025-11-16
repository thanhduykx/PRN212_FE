using Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepositories
{
    public interface ICitizenIdRepository
    {
        Task<IEnumerable<CitizenId>> GetAllCitizenIdsAsync();
        Task<CitizenId?> GetCitizenIdByIdAsync(int id);
        Task<CitizenId?> GetCitizenIdsByOrderIdAsync(int Id);
        Task AddCitizenIdAsync(CitizenId citizenId);
        Task UpdateCitizenIdAsync(CitizenId citizenId);
        Task DeleteCitizenIdAsync(CitizenId citizenId);
    }
}
