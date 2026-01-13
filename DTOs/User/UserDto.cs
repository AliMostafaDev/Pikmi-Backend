namespace Pikmi.API.DTOs.User
{
    public class UserDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool IsDocumentVerified { get; set; }
        public decimal Balance { get; set; }
    }
}
