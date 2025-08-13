using Countries.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Countries.Domain.Interfaces
{
    public interface ICountryRepository
    {
        Task<PagedResult<Country>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null);
        Task<Country?> GetByIdAsync(int id);
        Task AddAsync(Country country);
        Task UpdateAsync(Country country);
        Task SoftDeleteAsync(int id);
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
        Task<bool> ExistsByCodeAsync(string code, int? excludeId = null);
    }
}
