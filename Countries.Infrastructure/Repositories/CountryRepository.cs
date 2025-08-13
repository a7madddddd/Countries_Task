using Countries.Domain;
using Countries.Domain.Interfaces;
using Countries.Infrastructure.Context;
using Countries.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Countries.Infrastructure.Repositories
{
    public class CountryRepository : ICountryRepository
    {
        private readonly AppDbContext _context;

        public CountryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Country>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null)
        {
            var query = _context.Countries
                .Where(c => !c.IsDeleted)
                .AsQueryable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c => c.Name.Contains(search) || c.Code.Contains(search));
            }

            // Get total count before pagination
            var totalRecords = await query.CountAsync();

            // Apply pagination
            var countries = await query
                .OrderBy(c => c.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Country>
            {
                Data = countries,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<Country?> GetByIdAsync(int id)
        {
            return await _context.Countries
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }

        public async Task AddAsync(Country country)
        {
            country.CreatedDate = DateTime.UtcNow;
            country.IsDeleted = false;

            await _context.Countries.AddAsync(country);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Country country)
        {
            // Make sure we're tracking the entity
            var existingEntity = await _context.Countries.FindAsync(country.Id);
            if (existingEntity == null)
            {
                throw new Exception($"Country with ID {country.Id} not found");
            }

            
            existingEntity.Name = country.Name;
            existingEntity.Code = country.Code;
            

            var result = await _context.SaveChangesAsync();

            if (result == 0)
            {
                throw new Exception("No changes were saved to the database");
            }
        }

        public async Task SoftDeleteAsync(int id)
        {
            var country = await _context.Countries.FindAsync(id);
            if (country == null)
            {
                throw new Exception($"Country with ID {id} not found");
            }

            if (country.IsDeleted)
            {
                throw new Exception($"Country with ID {id} is already deleted");
            }

            country.IsDeleted = true;
            var result = await _context.SaveChangesAsync();

            if (result == 0)
            {
                throw new Exception("No changes were saved to the database");
            }
        }

        public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
        {
            var query = _context.Countries
                .Where(c => !c.IsDeleted && c.Name == name);

            if (excludeId.HasValue)
            {
                query = query.Where(c => c.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> ExistsByCodeAsync(string code, int? excludeId = null)
        {
            var query = _context.Countries
                .Where(c => !c.IsDeleted && c.Code == code);

            if (excludeId.HasValue)
            {
                query = query.Where(c => c.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }



    }
}
