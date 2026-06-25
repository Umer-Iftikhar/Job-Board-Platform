using JobBoard.Api.Application.DTOs;
using JobBoard.Api.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JobBoard.Api.API.Controllers
{
    public class JobListingsController : ControllerBase
    {
        private readonly IJobListingService _jobListingService;
        private readonly IEmployerProfileService _employerProfileService;

        public JobListingsController(
            IJobListingService jobListingService,
            IEmployerProfileService employerProfileService)
        {
            _jobListingService = jobListingService;
            _employerProfileService = employerProfileService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<JobListingDto>>> GetAll()
        {
            var jobs = await _jobListingService.GetAllActiveAsync();
            return Ok(jobs);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<JobListingDto>> GetById(Guid id)
        {
            var job = await _jobListingService.GetByIdAsync(id);
            if (job == null) return NotFound();
            return Ok(job);
        }

        [HttpPost]
        [Authorize(Roles = "Employer")]
        public async Task<ActionResult<JobListingDto>> Create(CreateJobListingDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var employer = await _employerProfileService.GetByUserIdAsync(userId!);
            if (employer == null)
                return BadRequest(new { message = "Employer profile not found." });

            var job = await _jobListingService.CreateAsync(employer.Id, dto);
            return CreatedAtAction(nameof(GetById), new { id = job.Id }, job);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Employer")]
        public async Task<ActionResult<JobListingDto>> Update(Guid id, UpdateJobListingDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var job = await _jobListingService.UpdateAsync(id, dto, userId!);
            if (job == null) return NotFound();
            return Ok(job);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _jobListingService.DeleteAsync(id, userId!);
            if (!result) return NotFound();
            return NoContent(); 
        }
    }
}
