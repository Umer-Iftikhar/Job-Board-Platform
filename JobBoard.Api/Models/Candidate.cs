using static System.Net.Mime.MediaTypeNames;

namespace JobBoard.Api.Models
{
    public class Candidate
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;        // key for lookup
        public string Phone { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;

        public ICollection<Resume> Resumes { get; set; }
        public ICollection<Application> Applications { get; set; }
    }
}
