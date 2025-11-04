namespace Backend.Requests
{
    public class VerifyEmailRequest
    {

        public string Email { get; set; }      // The email address of the user
        public string Code { get; set; }       // The verification code sent by email
    }
}
