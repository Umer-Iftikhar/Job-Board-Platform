using JobBoard.Api.Application.DTOs;
using JobBoard.Api.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Api.API.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("joblistings")]
        public async Task<ActionResult<PagedResult<JobListingDto>>> GetAllJobListings(
            [FromQuery] PaginationParams pagination,
            [FromQuery] bool includeDeleted = false)
        {
            var jobs = await _adminService.GetAllJobListingsAsync(pagination, includeDeleted);
            return Ok(jobs);
        }

        [HttpDelete("joblistings/{id}/hard")]
        public async Task<IActionResult> HardDeleteJobListing(Guid id)
        {
            var result = await _adminService.HardDeleteJobListingAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpPost("employers/{employerProfileId}/ban")]
        public async Task<IActionResult> BanEmployer(Guid employerProfileId)
        {
            var result = await _adminService.BanEmployerAsync(employerProfileId);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpGet("logs")]
        public async Task<ActionResult<IEnumerable<AdminActionLogDto>>> GetLogs([FromQuery] int count = 100)
        {
            var logs = await _adminService.GetActionLogsAsync(count);
            return Ok(logs);
        }
    }
}
