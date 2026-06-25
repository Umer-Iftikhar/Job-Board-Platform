using JobBoard.Api.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace JobBoard.Api.Application.DTOs
{
    public class UpdateJobListingDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(5000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [EnumDataType(typeof(JobType))]
        public JobType Type { get; set; }

        [Required]
        [EnumDataType(typeof(ExperienceLevel))]
        public ExperienceLevel Experience { get; set; }

        [StringLength(200)]
        public string? Location { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? SalaryMin { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? SalaryMax { get; set; }

        public bool IsActive { get; set; }
    }
}
