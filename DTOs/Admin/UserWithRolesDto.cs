namespace Pikmi.API.DTOs.Admin
{
    public class UserWithRolesDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string ProfileImage { get; set; }
        public decimal Balance { get; set; }
        public double AverageRating { get; set; }
        public bool IsDocumentVerified { get; set; }
        public bool EmailConfirmed { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public IList<string> Roles { get; set; }
    }
}
