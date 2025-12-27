using System.ComponentModel.DataAnnotations;

namespace Pikmi.API.DTOs.Admin
{
    public class VerifyDocumentDto
    {
        [Required]
        public string UserId { get; set; }
    }
}
