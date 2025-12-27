using System.ComponentModel.DataAnnotations;

namespace Pikmi.API.DTOs.Admin
{
    public class ManageRoleDto
    {
        [Required]
        public string UserId { get; set; }
    }
}
