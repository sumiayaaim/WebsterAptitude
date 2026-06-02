using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebsterAptitude.Models;

namespace WebsterAptitude.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<CandidateProfile> CandidateProfiles => Set<CandidateProfile>();

        public DbSet<CandidateTestAttempt> CandidateTestAttempts => Set<CandidateTestAttempt>();
        public DbSet<TestSection> TestSections => Set<TestSection>();
        public DbSet<CandidateSectionAttempt> CandidateSectionAttempts => Set<CandidateSectionAttempt>();
        public DbSet<Question> Questions => Set<Question>();
        public DbSet<CandidateAnswer> CandidateAnswers => Set<CandidateAnswer>();



    }
}
