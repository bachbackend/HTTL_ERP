namespace HTTL_ERP.DTO
{
    public class RegisterDTO
    {
        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;
        public short Role { get; set; }

        public DateOnly Dob { get; set; }

        public string Phone { get; set; } = null!;

        public string Email { get; set; } = null!;

        public DateTime? CreatedAt { get; set; }
    }
}
