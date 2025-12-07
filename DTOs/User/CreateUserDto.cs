using System.ComponentModel.DataAnnotations;

namespace Pikmi.API.DTOs.User
{
    public class CreateUserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [RegularExpression("^(Male|Female)$", ErrorMessage = "Gender must be Male or Female.")]
        public string Gender { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
