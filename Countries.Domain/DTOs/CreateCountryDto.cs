using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Countries.Domain.DTOs
{
    public class CreateCountryDto
    {
        [Required(ErrorMessage = "Country name is required")]
        [StringLength(100, ErrorMessage = "Country name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Country code is required")]
        [StringLength(5, ErrorMessage = "Country code cannot exceed 5 characters")]
        public string Code { get; set; } = string.Empty;
    }
}
