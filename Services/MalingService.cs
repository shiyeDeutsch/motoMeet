using System.Net.Mail;
using System.Threading.Tasks;

namespace motoMeet
{
    public interface IMailingService
    {
        Task<SendingEmailResult> SendVerificationEmailAsync(string toEmail, string verificationLink);
    }


    public class MailingService : IMailingService
    {
        private readonly IEmailService _emailService;
        private readonly string _fromEmailDisplayName;
        private readonly string _fromEmailAddress;

        public MailingService(IEmailService emailService, string fromEmailAddress = "themapster.app@gmail.com", string fromEmailDisplayName = "themapster")
        {
            _emailService = emailService;
            _fromEmailAddress = fromEmailAddress;
            _fromEmailDisplayName = fromEmailDisplayName;
        }

        public async Task<SendingEmailResult> SendVerificationEmailAsync(string toEmail, string verificationLink)
        {
            var fromEmail = new MailAddress(_fromEmailAddress, _fromEmailDisplayName);
            const string subject = "Verify Your Email";
            string body = $"Please verify your email by clicking on this link: {verificationLink}";

            try
            {
                await _emailService.SendEmailAsync(fromEmail, toEmail, subject, body);
                return new SendingEmailResult { IsSuccess = true };
            }
            catch (Exception ex)
            {
                return new SendingEmailResult { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }
    }
}
