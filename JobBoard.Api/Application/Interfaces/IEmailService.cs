namespace JobBoard.Api.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendApplicationStatusChangedAsync(string candidateEmail, string candidateName, string jobTitle, string newStatus);
    }
}
