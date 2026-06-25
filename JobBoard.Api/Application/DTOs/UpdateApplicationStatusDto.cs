using JobBoard.Api.Domain.Enums;

namespace JobBoard.Api.Application.DTOs
{
    public class UpdateApplicationStatusDto
    {
        public ApplicationStatus Status { get; set; }
    }
}
