using Countries.Domain.DTOs;
using Countries.Domain.Interfaces;
using Countries.Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;

namespace Countries.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly ICountryRepository _countryRepository;
        public CountriesController(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }




        /// <summary>
        /// Get all countries with pagination and optional search
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10, max: 100)</param>
        /// <param name="search">Search term for name or code</param>
        /// <returns>Paginated list of countries</returns>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResultDto<CountryDto>>>> GetCountries(
            [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0")] int pageNumber = 1,
            [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")] int pageSize = 10,
            string? search = null)
        {
            try
            {
                var result = await _countryRepository.GetAllAsync(pageNumber, pageSize, search);

                var response = new PagedResultDto<CountryDto>
                {
                    Data = result.Data.Select(c => new CountryDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Code = c.Code,
                        CreatedDate = c.CreatedDate
                    }).ToList(),
                    TotalRecords = result.TotalRecords,
                    PageNumber = result.PageNumber,
                    PageSize = result.PageSize,
                    TotalPages = result.TotalPages,
                    HasPreviousPage = result.HasPreviousPage,
                    HasNextPage = result.HasNextPage
                };

                return Ok(new ApiResponse<PagedResultDto<CountryDto>>
                {
                    Success = true,
                    Message = "Countries retrieved successfully",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<PagedResultDto<CountryDto>>
                {
                    Success = false,
                    Message = "An error occurred while retrieving countries",
                    Errors = new List<string> { "Internal server error" }
                });
            }
        }

        /// <summary>
        /// Get a specific country by ID
        /// </summary>
        /// <param name="id">Country ID</param>
        /// <returns>Country details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CountryDto>>> GetCountry(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new ApiResponse<CountryDto>
                    {
                        Success = false,
                        Message = "Invalid country ID",
                        Errors = new List<string> { "Country ID must be greater than 0" }
                    });
                }

                var country = await _countryRepository.GetByIdAsync(id);
                if (country == null)
                {
                    return NotFound(new ApiResponse<CountryDto>
                    {
                        Success = false,
                        Message = "Country not found",
                        Errors = new List<string> { $"Country with ID {id} was not found" }
                    });
                }

                var countryDto = new CountryDto
                {
                    Id = country.Id,
                    Name = country.Name,
                    Code = country.Code,
                    CreatedDate = country.CreatedDate
                };

                return Ok(new ApiResponse<CountryDto>
                {
                    Success = true,
                    Message = "Country retrieved successfully",
                    Data = countryDto
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<CountryDto>
                {
                    Success = false,
                    Message = "An error occurred while retrieving the country",
                    Errors = new List<string> { "Internal server error" }
                });
            }
        }

        /// <summary>
        /// Create a new country
        /// </summary>
        /// <param name="createCountryDto">Country creation data</param>
        /// <returns>Created country</returns>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<CountryDto>>> CreateCountry([FromBody] CreateCountryDto createCountryDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(new ApiResponse<CountryDto>
                    {
                        Success = false,
                        Message = "Validation failed",
                        Errors = errors
                    });
                }

                if (await _countryRepository.ExistsByNameAsync(createCountryDto.Name.Trim()))
                {
                    return BadRequest(new ApiResponse<CountryDto>
                    {
                        Success = false,
                        Message = "Country name already exists",
                        Errors = new List<string> { "Please use a different country name" }
                    });
                }

                if (await _countryRepository.ExistsByCodeAsync(createCountryDto.Code.Trim().ToUpper()))
                {
                    return BadRequest(new ApiResponse<CountryDto>
                    {
                        Success = false,
                        Message = "Country code already exists",
                        Errors = new List<string> { "Please use a different country code" }
                    });
                }

                var country = new Country
                {
                    Name = createCountryDto.Name.Trim(),
                    Code = createCountryDto.Code.Trim().ToUpper(),
                    CreatedDate = DateTime.UtcNow,
                    IsDeleted = false
                };

                await _countryRepository.AddAsync(country);

                var countryDto = new CountryDto
                {
                    Id = country.Id,
                    Name = country.Name,
                    Code = country.Code,
                    CreatedDate = country.CreatedDate
                };

                return CreatedAtAction(nameof(GetCountry), new { id = country.Id }, new ApiResponse<CountryDto>
                {
                    Success = true,
                    Message = "Country created successfully",
                    Data = countryDto
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<CountryDto>
                {
                    Success = false,
                    Message = ex.Message,
                    Errors = new List<string> { ex.InnerException?.Message ?? ex.Message }
                });
            }
        }

        /// <summary>
        /// Update an existing country
        /// </summary>
        /// <param name="id">Country ID</param>
        /// <param name="updateCountryDto">Country update data</param>
        /// <returns>Updated country</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<CountryDto>>> UpdateCountry(int id, [FromBody] UpdateCountryDto updateCountryDto)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new ApiResponse<CountryDto>
                    {
                        Success = false,
                        Message = "Invalid country ID",
                        Errors = new List<string> { "Country ID must be greater than 0" }
                    });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(new ApiResponse<CountryDto>
                    {
                        Success = false,
                        Message = "Validation failed",
                        Errors = errors
                    });
                }

                var existingCountry = await _countryRepository.GetByIdAsync(id);
                if (existingCountry == null)
                {
                    return NotFound(new ApiResponse<CountryDto>
                    {
                        Success = false,
                        Message = "Country not found",
                        Errors = new List<string> { $"Country with ID {id} was not found" }
                    });
                }

                
                var name = updateCountryDto.Name.Trim();
                var code = updateCountryDto.Code.Trim().ToUpper();

                
                if (await _countryRepository.ExistsByNameAsync(name, id))
                {
                    return BadRequest(new ApiResponse<CountryDto>
                    {
                        Success = false,
                        Message = "Country name already exists",
                        Errors = new List<string> { $"A country with the name '{name}' already exists" }
                    });
                }

                if (await _countryRepository.ExistsByCodeAsync(code, id))
                {
                    return BadRequest(new ApiResponse<CountryDto>
                    {
                        Success = false,
                        Message = "Country code already exists",
                        Errors = new List<string> { $"A country with the code '{code}' already exists" }
                    });
                }

                
                existingCountry.Name = name;
                existingCountry.Code = code;

                await _countryRepository.UpdateAsync(existingCountry);

                var countryDto = new CountryDto
                {
                    Id = existingCountry.Id,
                    Name = existingCountry.Name,
                    Code = existingCountry.Code,
                    CreatedDate = existingCountry.CreatedDate
                };

                return Ok(new ApiResponse<CountryDto>
                {
                    Success = true,
                    Message = "Country updated successfully",
                    Data = countryDto
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<CountryDto>
                {
                    Success = false,
                    Message = "An error occurred while updating the country",
                    Errors = new List<string> { "Internal server error" }
                });
            }
        }

        /// <summary>
        /// Delete a country (soft delete)
        /// </summary>
        /// <param name="id">Country ID</param>
        /// <returns>Confirmation of deletion</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteCountry(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid country ID",
                        Errors = new List<string> { "Country ID must be greater than 0" }
                    });
                }

                var existingCountry = await _countryRepository.GetByIdAsync(id);
                if (existingCountry == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Country not found",
                        Errors = new List<string> { $"Country with ID {id} was not found" }
                    });
                }

                await _countryRepository.SoftDeleteAsync(id);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Country deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while deleting the country",
                    Errors = new List<string> { "Internal server error" }
                });
            }
        }
    }
}
