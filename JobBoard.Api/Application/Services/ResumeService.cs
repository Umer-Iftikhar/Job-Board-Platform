using JobBoard.Api.Application.DTOs;
using JobBoard.Api.Application.Interfaces;
using JobBoard.Api.Domain.Entities;

namespace JobBoard.Api.Application.Services
{
    public class ResumeService : IResumeService
    {
        private readonly IResumeRepository _repository;

        public ResumeService(IResumeRepository repository)
        {
            _repository = repository;
        }

        public async Task<ResumeDto> CreateAsync(Guid candidateId, string fileName, string fileUrl, string? contentType)
        {
            var resume = new Resume
            {
                CandidateId = candidateId,
                FileName = fileName,
                FileUrl = fileUrl,
                ContentType = contentType
            };

            await _repository.AddAsync(resume);
            return MapToDto(resume);
        }

        public async Task<IEnumerable<ResumeDto>> GetByCandidateAsync(Guid candidateId)
        {
            var resumes = await _repository.GetByCandidateAsync(candidateId);
            return resumes.Select(MapToDto);
        }

        private static ResumeDto MapToDto(Resume resume)
        {
            return new ResumeDto
            {
                Id = resume.Id,
                FileName = resume.FileName,
                FileUrl = resume.FileUrl,
                ContentType = resume.ContentType,
                CreatedAt = resume.CreatedAt
            };
        }
    }
}
