using JobBoard.Api.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace JobBoard.Api.Application.DTOs
{
    public class JobListingFilterParams
    {
        public string? SearchTerm { get; set; } // Searches title and description

        public JobType? Type { get; set; }

        public ExperienceLevel? Experience { get; set; }

        public string? Location { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? MinSalary { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? MaxSalary { get; set; }

        public bool? IsRemote { get; set; } // If location contains "remote" or is null
    }
}
