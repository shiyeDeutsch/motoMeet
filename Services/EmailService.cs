using System.Net;
using System.Net.Mail;

namespace motoMeet
{
    public interface IEmailService
    {
        Task<SendingEmailResult> SendEmailAsync(MailAddress fromEmail, string toEmail, string subject, string body);
    }

    public class EmailService : IEmailService
    {
        private string smtpHost;
        private int smtpPort;
        private bool enableSSL;
        private string smtpUsername;
        private string smtpPassword;
        public EmailService(string host = "smtp.gmail.com", int port = 587, bool ssl = true, string username = "themapster.app@gmail.com", string password = "nale bryo pjra ekmc")
        {
            smtpHost = host;
            smtpPort = port;
            enableSSL = ssl;
            smtpUsername = username;
            smtpPassword = password;
        }
        public async Task<SendingEmailResult> SendEmailAsync(MailAddress fromEmail, string toEmail, string subject, string body)
        {
            try
            {
                var toAddress = new MailAddress(toEmail);
                var smtp = new SmtpClient
                {
                    Host = smtpHost,
                    Port = smtpPort,
                    EnableSsl = enableSSL,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromEmail.Address, smtpPassword)
                };
                using var message = new MailMessage(fromEmail, toAddress)
                {
                    Subject = subject,
                    Body = body
                };
                await smtp.SendMailAsync(message);
                return new SendingEmailResult { IsSuccess = true };
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                return new SendingEmailResult { IsSuccess = false, ErrorMessage = "Sending Email failed.\n " + ex.Message };
            }
        }

    }
    public class SendingEmailResult : OperationResult<T> 
    {

    }

    public class T
    {
    }
}


