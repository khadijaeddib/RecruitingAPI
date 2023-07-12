using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SendGrid.Helpers.Mail;
using SendGrid;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;



namespace RecruitingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExampleController : ControllerBase
    {
        [HttpPost("sendTestEmail")]
        public IActionResult SendTestEmail()
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Sender Name", "khadijaeddib05@gmail.com"));
            message.To.Add(new MailboxAddress("Recipient Name", "khadijaeddib06@gmail.com"));
            message.Subject = "Hello, World!";
            message.Body = new TextPart("plain")
            {
                Text = "This is a test email."
            };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                client.Authenticate("khadijaeddib05@gmail.com", "qtgwycholxkjassi");
                client.Send(message);
                client.Disconnect(true);
            }

            return Ok(new { Message = "Test email sent" });
        }

        /*private readonly ISendGridClient _sendGridClient;

        public ExampleController(ISendGridClient sendGridClient)
        {
            _sendGridClient = sendGridClient;
        }

        [HttpPost("sendTestEmail")]
        public async Task<IActionResult> SendTestEmail()
        {
            var from = new EmailAddress("khadijaeddib05@gmail.com", "Example User");
            var to = new EmailAddress("khadijaeddib06@gmail.com", "Recipient Name");
            var subject = "Hello, World!";
            var plainTextContent = "This is a test email.";
            var htmlContent = "<strong>This is a test email.</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await _sendGridClient.SendEmailAsync(msg);

            return Ok(new { Message = "Test email sent" });
        }*/
    }

}
