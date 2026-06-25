using JobBoard.Api.Application.Interfaces;
using JobBoard.Api.Domain.Entities;
using JobBoard.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Api.Infrastructure.Repositories
{
    public class CandidateRepository : Repository<Candidate>, ICandidateRepository
    {
        public CandidateRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Candidate?> GetByEmailAsync(string email)
        {
            return await _context.Candidates
                .FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<Candidate?> GetByEmailWithApplicationsAsync(string email)
        {
            return await _context.Candidates
                .Include(c => c.Applications)
                .ThenInclude(a => a.JobListing)
                .FirstOrDefaultAsync(c => c.Email == email);
        }
    }
}
