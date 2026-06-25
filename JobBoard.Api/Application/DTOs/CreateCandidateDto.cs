using System.ComponentModel.DataAnnotations;

namespace JobBoard.Api.Application.DTOs
{
    public class CreateCandidateDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(256)]
        public string Email { get; set; } = string.Empty;
    }
}
