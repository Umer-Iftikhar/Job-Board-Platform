using System.ComponentModel.DataAnnotations;

namespace JobBoard.Api.Application.DTOs
{
    public class JobListingSortParams
    {
        public string? SortBy { get; set; } // "createdAt", "title", "salaryMin", "salaryMax"

        [RegularExpression("asc|desc", ErrorMessage = "SortOrder must be 'asc' or 'desc'.")]
        public string SortOrder { get; set; } = "desc";
    }
}
