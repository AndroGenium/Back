
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

        public void SendEmail(string subject, string to, string body)
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
     
                }
                catch (SmtpException ex)
                {

                    throw;
                }
                finally
                {
                    client.Dispose();
                }
            }
        }


        public void SendEMailConfirmation(string to, string code)
        {
            string body = $"<!DOCTYPE html>\r\n<html lang=\"en\">\r\n<head>\r\n    <meta charset=\"UTF-8\">\r\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\r\n    <title>Confirm Your Email - Toolsnearme</title>\r\n    <style>\r\n        * {{\r\n            margin: 0;\r\n            padding: 0;\r\n            box-sizing: border-box;\r\n        }}\r\n\r\n        body {{\r\n            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Arial, sans-serif;\r\n            background: #f5f5f5;\r\n            padding: 40px 20px;\r\n        }}\r\n\r\n        /* Email Container */\r\n        .email-container {{\r\n            max-width: 600px;\r\n            margin: 0 auto;\r\n            background: white;\r\n            border-radius: 12px;\r\n            box-shadow: 0 4px 16px rgba(0, 0, 0, 0.08);\r\n            overflow: hidden;\r\n        }}\r\n\r\n        /* Header */\r\n        .email-header {{\r\n            background: #601A35;\r\n            padding: 40px 30px;\r\n            text-align: center;\r\n        }}\r\n\r\n        .logo {{\r\n            display: inline-flex;\r\n            align-items: center;\r\n            gap: 12px;\r\n            margin-bottom: 20px;\r\n        }}\r\n\r\n        .logo-icon {{\r\n            width: 50px;\r\n            height: 50px;\r\n            background: white;\r\n            border-radius: 10px;\r\n            display: flex;\r\n            align-items: center;\r\n            justify-content: center;\r\n            color: #601A35;\r\n            font-weight: 800;\r\n            font-size: 28px;\r\n        }}\r\n\r\n        .logo-text {{\r\n            font-size: 28px;\r\n            font-weight: 700;\r\n            color: white;\r\n        }}\r\n\r\n        .email-header h1 {{\r\n            font-size: 24px;\r\n            color: white;\r\n            font-weight: 600;\r\n        }}\r\n\r\n        /* Body */\r\n        .email-body {{\r\n            padding: 40px 30px;\r\n        }}\r\n\r\n        .email-body p {{\r\n            font-size: 16px;\r\n            line-height: 1.6;\r\n            color: #333;\r\n            margin-bottom: 20px;\r\n        }}\r\n\r\n        .greeting {{\r\n            font-weight: 600;\r\n            color: #601A35;\r\n            margin-bottom: 24px;\r\n        }}\r\n\r\n        /* Verification Code Box */\r\n        .code-container {{\r\n            text-align: center;\r\n            margin: 40px 0;\r\n        }}\r\n\r\n        .code-label {{\r\n            font-size: 14px;\r\n            color: #666;\r\n            margin-bottom: 12px;\r\n            text-transform: uppercase;\r\n            letter-spacing: 1px;\r\n            font-weight: 600;\r\n        }}\r\n\r\n        .verification-code {{\r\n            display: inline-block;\r\n            background: #f9f9f9;\r\n            border: 2px dashed #601A35;\r\n            padding: 20px 40px;\r\n            border-radius: 8px;\r\n            font-size: 36px;\r\n            font-weight: 700;\r\n            letter-spacing: 8px;\r\n            color: #601A35;\r\n            user-select: all;\r\n        }}\r\n\r\n        /* Expiry Notice */\r\n        .expiry-notice {{\r\n            background: #fff3cd;\r\n            border: 1px solid #ffeaa7;\r\n            border-radius: 6px;\r\n            padding: 16px;\r\n            font-size: 14px;\r\n            color: #856404;\r\n            margin-top: 30px;\r\n            text-align: center;\r\n        }}\r\n\r\n        /* Divider */\r\n        .divider {{\r\n            border: none;\r\n            border-top: 1px solid #eee;\r\n            margin: 30px 0;\r\n        }}\r\n\r\n        /* Footer */\r\n        .email-footer {{\r\n            background: #f9f9f9;\r\n            padding: 30px;\r\n            text-align: center;\r\n            border-top: 1px solid #eee;\r\n        }}\r\n\r\n        .email-footer p {{\r\n            font-size: 13px;\r\n            color: #666;\r\n            line-height: 1.6;\r\n            margin-bottom: 8px;\r\n        }}\r\n\r\n        .email-footer a {{\r\n            color: #601A35;\r\n            text-decoration: none;\r\n        }}\r\n\r\n        .email-footer a:hover {{\r\n            text-decoration: underline;\r\n        }}\r\n\r\n        /* Security Note */\r\n        .security-note {{\r\n            font-size: 14px;\r\n            color: #666;\r\n            background: #f9f9f9;\r\n            padding: 16px;\r\n            border-radius: 6px;\r\n            border-left: 4px solid #601A35;\r\n            margin-top: 20px;\r\n        }}\r\n\r\n        /* Responsive */\r\n        @media (max-width: 640px) {{\r\n            .email-container {{\r\n                border-radius: 0;\r\n            }}\r\n\r\n            .email-header,\r\n            .email-body {{\r\n                padding: 30px 20px;\r\n            }}\r\n\r\n            .email-header h1 {{\r\n                font-size: 20px;\r\n            }}\r\n\r\n            .logo-text {{\r\n                font-size: 24px;\r\n            }}\r\n\r\n            .logo-icon {{\r\n                width: 40px;\r\n                height: 40px;\r\n                font-size: 22px;\r\n            }}\r\n\r\n            .verification-code {{\r\n                font-size: 28px;\r\n                letter-spacing: 4px;\r\n                padding: 16px 30px;\r\n            }}\r\n        }}\r\n    </style>\r\n</head>\r\n<body>\r\n    <div class=\"email-container\">\r\n        <!-- Header -->\r\n        <div class=\"email-header\">\r\n            <div class=\"logo\">\r\n                <div class=\"logo-icon\">T</div>\r\n                <span class=\"logo-text\">Toolsnearme</span>\r\n            </div>\r\n            <h1>Confirm Your Email Address</h1>\r\n        </div>\r\n\r\n        <!-- Body -->\r\n        <div class=\"email-body\">\r\n            <p class=\"greeting\">Hi {to},</p>\r\n            \r\n            <p>Thank you for signing up with Toolsnearme! We're excited to have you join our community of tool borrowers and sharers.</p>\r\n            \r\n            <p>To complete your registration and start borrowing tools, please enter the verification code below:</p>\r\n\r\n            <!-- Verification Code -->\r\n            <div class=\"code-container\">\r\n                <div class=\"code-label\">Your Verification Code</div>\r\n                <div class=\"verification-code\">{code}</div>\r\n            </div>\r\n\r\n            <!-- Expiry Notice -->\r\n            <div class=\"expiry-notice\">\r\n                ⏰ <strong>Important:</strong> This code will expire in 15 minutes for security reasons.\r\n            </div>\r\n\r\n            <hr class=\"divider\">\r\n\r\n            <div class=\"security-note\">\r\n                🔒 <strong>Security Tip:</strong> Never share this code with anyone. Toolsnearme will never ask you for this code via phone or email.\r\n            </div>\r\n\r\n            <hr class=\"divider\">\r\n\r\n            <p style=\"font-size: 14px; color: #666;\">\r\n                If you didn't create an account with Toolsnearme, you can safely ignore this email.\r\n            </p>\r\n        </div>\r\n\r\n        <!-- Footer -->\r\n        <div class=\"email-footer\">\r\n            <p><strong>Toolsnearme</strong></p>\r\n            <p>Borrow tools. Build community. Share responsibly.</p>\r\n            <p style=\"margin-top: 16px;\">\r\n                Need help? <a href=\"mailto:support@toolsnearme.com\">Contact Support</a>\r\n            </p>\r\n            <p style=\"margin-top: 16px; font-size: 12px; color: #999;\">\r\n                © 2024 Toolsnearme. All rights reserved.\r\n            </p>\r\n        </div>\r\n    </div>\r\n</body>\r\n</html>";
            string subject = "Email Confirmation";
            SendEmail(subject, to, body);
        }
    }
}
