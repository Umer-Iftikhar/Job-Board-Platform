using JobBoard.Api.Application.Interfaces;
using JobBoard.Api.Domain.Entities;
using JobBoard.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Api.Infrastructure.Repositories
{
    public class ResumeRepository : Repository<Resume>, IResumeRepository
    {
        public ResumeRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Resume>> GetByCandidateAsync(Guid candidateId)
        {
            return await _context.Resumes
                .Where(r => r.CandidateId == candidateId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }
    }
}
