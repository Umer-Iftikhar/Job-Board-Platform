using JobBoard.Api.Application.DTOs;
using JobBoard.Api.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.RateLimiting;

namespace JobBoard.Api.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationsController : ControllerBase
    {
        private readonly IJobApplicationService _applicationService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IFileValidationService _fileValidationService;

        public ApplicationsController(
            IJobApplicationService applicationService,
            IFileStorageService fileStorageService,
            IFileValidationService fileValidationService)
        {
            _applicationService = applicationService;
            _fileStorageService = fileStorageService;
            _fileValidationService = fileValidationService;
        }

        [HttpPost]
        [AllowAnonymous]
        [EnableRateLimiting("ApplyPolicy")]
        public async Task<ActionResult<JobApplicationDto>> Apply(
            [FromForm] CreateJobApplicationDto dto,
            IFormFile resume)
        {
            var validation = _fileValidationService.ValidateResume(resume);
            if (!validation.IsValid)
                return BadRequest(new { message = validation.ErrorMessage });

            var fileUrl = await _fileStorageService.SaveFileAsync(resume, "resumes");

            try
            {
                var application = await _applicationService.CreateAsync(dto, fileUrl);
                return CreatedAtAction(nameof(GetById), new { id = application.Id }, application);
            }
            catch (InvalidOperationException ex)
            {
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
