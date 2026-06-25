namespace JobBoard.Api.Domain.Entities
{
    public class Candidate : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
        public ICollection<Resume> Resumes { get; set; } = new List<Resume>();
    }
}
