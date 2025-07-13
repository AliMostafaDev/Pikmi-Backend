using RestSharp;
using Newtonsoft.Json.Linq;
using Pikmi.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.RegularExpressions;

namespace Pikmi.API.Services.Implementations
{
    public class MailjetApiEmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public MailjetApiEmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body, bool isHtml = false)
        {
            var client = new RestClient("https://api.mailjet.com/v3.1/send");
            var request = new RestRequest() { Method = Method.Post };

            // Auth using API Key/Secret
            request.AddHeader("Authorization",
                $"Basic {Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{_config["MailjetSettings:ApiKey"]}:{_config["MailjetSettings:SecretKey"]}"))}");

            var message = new
            {
                From = new
                {
                    Email = _config["MailjetSettings:SenderEmail"],
                    Name = _config["MailjetSettings:SenderName"]
                },
                To = new[] { new { Email = toEmail } },
                Subject = subject,
                HTMLPart = isHtml ? WrapInHtmlTemplate(body) : null,
                TextPart = isHtml ? StripHtml(body) : body
            };

            request.AddJsonBody(new { Messages = new[] { message } });
            var response = await client.ExecuteAsync(request);

            return response.IsSuccessful;
        }
        private string WrapInHtmlTemplate(string content)
        {
            return $@"<!DOCTYPE html>
                    <html>
                    <head>
                        <meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"">
                    </head>
                    <body style=""font-family: Arial, sans-serif;"">
                        <div style=""max-width: 600px; margin: 0 auto; padding: 20px;"">
                            {content}
                            <p style=""color: #666; font-size: 12px; margin-top: 20px;"">
                                This is an automated message - please do not reply.
                            </p>
                        </div>
                    </body>
                    </html>";
        }

        private string StripHtml(string html)
        {
            return Regex.Replace(html, "<[^>]*>", "");
        }
    }
}
