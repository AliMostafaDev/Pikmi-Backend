using System.ComponentModel.DataAnnotations;

namespace Pikmi.API.DTOs.Identity
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }   
    }
}
