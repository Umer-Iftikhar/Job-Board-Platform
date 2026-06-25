using JobBoard.Api.Application.Interfaces;
using JobBoard.Api.Domain.Entities;
using JobBoard.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Api.Infrastructure.Repositories;

public class JobApplicationRepository : Repository<JobApplication>, IJobApplicationRepository
{
    public JobApplicationRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<JobApplication>> GetByJobListingAsync(Guid jobListingId)
    {
        return await _context.JobApplications
            .Where(a => a.JobListingId == jobListingId)
            .Include(a => a.Candidate)
            .Include(a => a.Resume)
            .OrderByDescending(a => a.AppliedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<JobApplication>> GetByCandidateAsync(Guid candidateId)
    {
        return await _context.JobApplications
            .Where(a => a.CandidateId == candidateId)
            .Include(a => a.JobListing)
            .ThenInclude(j => j.EmployerProfile)
            .Include(a => a.Resume)
            .OrderByDescending(a => a.AppliedDate)
            .ToListAsync();
    }

    public async Task<JobApplication?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _context.JobApplications
            .Include(a => a.Candidate)
            .Include(a => a.JobListing)
            .ThenInclude(j => j.EmployerProfile)
            .Include(a => a.Resume)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<bool> ExistsByCandidateAndJobAsync(Guid candidateId, Guid jobListingId)
    {
        return await _context.JobApplications
            .AnyAsync(a => a.CandidateId == candidateId && a.JobListingId == jobListingId);
    }
}