using Azure.Identity;
using Microsoft.EntityFrameworkCore.Metadata;
using System.ComponentModel.DataAnnotations;

namespace Pikmi.API.DTOs.Identity
{
    public class RegisterDto
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MinLength(6)]
        public string Password { get; set; }
        [Required]
        [RegularExpression("^(Male|Female)$", ErrorMessage = "Gender must be Male or Female.")]
        public string Gender { get; set; }
    }
}
