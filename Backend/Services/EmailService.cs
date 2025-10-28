
using System.Net.Mail;
using System.Net;
using System.Text;

namespace Backend.Services
{
    public class EmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly bool _enableSsl;

        public EmailService()
        {
            // Configure for your email provider (e.g., Gmail)
            _smtpServer = "smtp.gmail.com";  // Or "smtp.outlook.com", etc.
            _smtpPort = 587;                // Or the appropriate port for your provider
            _smtpUsername = "chilachava.doc@gmail.com"; // Replace with your email address
            _smtpPassword = "hhsf oqyh qtxb ctys";     // Replace with your App Password (or regular password, if allowed)
            _enableSsl = true;               //  Most providers require this

        }

        public void SendEmail(string subject, string to, string body, int level)
        {
            MailMessage message = new MailMessage();
            message.From = new MailAddress(_smtpUsername);
            message.To.Add(to);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;
            message.BodyEncoding = Encoding.UTF8;

            using (SmtpClient client = new SmtpClient(_smtpServer, _smtpPort))
            {
                client.EnableSsl = _enableSsl;
                client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                try
                {
                    client.Send(message);
                    Console.WriteLine($"Email sent successfully to {to}");
     
                }
                catch (SmtpException ex)
                {
                    Console.WriteLine($"Error sending email: {ex.Message}");

                    throw;
                }
                finally
                {
                    client.Dispose();
                }
            }
        }
    }
}
