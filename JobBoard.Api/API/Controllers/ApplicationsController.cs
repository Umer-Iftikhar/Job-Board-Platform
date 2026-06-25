using JobBoard.Api.Application.DTOs;
using JobBoard.Api.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JobBoard.Api.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationsController : ControllerBase
    {
        private readonly IJobApplicationService _applicationService;
        private readonly IFileStorageService _fileStorageService;

        public ApplicationsController(
            IJobApplicationService applicationService,
            IFileStorageService fileStorageService)
        {
            _applicationService = applicationService;
            _fileStorageService = fileStorageService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<JobApplicationDto>> Apply(
            [FromForm] CreateJobApplicationDto dto,
            IFormFile resume)
        {
            if (resume == null || resume.Length == 0)
                return BadRequest(new { message = "Resume file is required." });

            var fileUrl = await _fileStorageService.SaveFileAsync(resume, "resumes");

            try
            {
                var application = await _applicationService.CreateAsync(dto, fileUrl);
                // 201 Created with Location header
                return CreatedAtAction(nameof(GetById), new { id = application.Id }, application);
            }
            catch (InvalidOperationException ex)
            {
                // 409 Conflict — duplicate application
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpGet("job/{jobListingId}")]
        [Authorize(Roles = "Employer")]
        public async Task<ActionResult<IEnumerable<JobApplicationDto>>> GetByJobListing(Guid jobListingId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var applications = await _applicationService.GetByJobListingAsync(jobListingId, userId!);
            return Ok(applications);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Employer")]
        public async Task<ActionResult<JobApplicationDto>> GetById(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isOwner = await _applicationService.IsOwnedByEmployerAsync(id, userId!);
            if (!isOwner) return NotFound();

            var application = await _applicationService.GetByIdAsync(id);
            if (application == null) return NotFound();
            return Ok(application);
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Employer")]
        public async Task<ActionResult<JobApplicationDto>> UpdateStatus(
            Guid id,
            UpdateApplicationStatusDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var application = await _applicationService.UpdateStatusAsync(id, dto, userId!);
            if (application == null) return NotFound();
            return Ok(application);
        }
    }
}
