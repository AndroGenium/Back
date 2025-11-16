namespace Backend.Models
{
    public class PendingUser
    {

        public int Id { get; set; }
        public string Email { get; set; } // <---
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PasswordHash { get; private set; }
        public DateTime BirthDate { get; set; }
        public string VerificationCode { get; set; }
        public DateTime VerificationCodeExpiry { get; set; }
        public DateTime DateCreated { get; set; }
        public void SetPassword(string password)
        {
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
