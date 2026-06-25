using JobBoard.Api.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace JobBoard.Api.Application.DTOs
{
    public class UpdateApplicationStatusDto
    {
        [Required]
        [EnumDataType(typeof(ApplicationStatus))]
        public ApplicationStatus Status { get; set; }
    }
}
