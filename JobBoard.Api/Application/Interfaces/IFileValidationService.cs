namespace JobBoard.Api.Application.Interfaces
{
    public interface IFileValidationService
    {
        (bool IsValid, string? ErrorMessage) ValidateResume(IFormFile file);
    }
}
