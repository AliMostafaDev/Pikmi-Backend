namespace Pikmi.API.DTOs.Identity
{
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpireAt { get; set; }
    }
}
