namespace JobBoard.Api.Models
{
    public class EmployerProfile
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;  // string, matches IdentityUser.Id
        public string CompanyName { get; set; } = string.Empty;
        public string CompanyDescription { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;

        public User User { get; set; } = new User();
        public ICollection<JobListing> JobListings { get; set; } = new List<JobListing>();
    }
}
