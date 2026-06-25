using JobBoard.Api.Application.Interfaces;

namespace JobBoard.Api.Infrastructure.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly ILogger<SmtpEmailService> _logger;

        public SmtpEmailService(ILogger<SmtpEmailService> logger)
        {
            _logger = logger;
        }

        public Task SendApplicationStatusChangedAsync(string candidateEmail, string candidateName, string jobTitle, string newStatus)
        {
            // TODO: Replace with real SMTP send using MailKit or System.Net.Mail
            _logger.LogInformation(
                "EMAIL TO: {Email} | SUBJECT: Application status updated | BODY: Hi {Name}, your application for '{Job}' is now {Status}.",
                candidateEmail, candidateName, jobTitle, newStatus);

            return Task.CompletedTask;
        }
    }
}
