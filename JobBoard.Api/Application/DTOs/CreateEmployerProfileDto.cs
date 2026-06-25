using System.ComponentModel.DataAnnotations;

namespace JobBoard.Api.Application.DTOs
{
    public class CreateEmployerProfileDto
    {
        [Required]
        [StringLength(200)]
        public string CompanyName { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        [StringLength(500)]
        [Url]
        public string? Website { get; set; }

        [StringLength(200)]
        public string? Location { get; set; }
    }
}
