using System;

namespace WebsterAptitude.Models
{
    public class CandidateSectionAttempt
    {
        public int Id { get; set; }
        public int TestAttemptId { get; set; }
        public int SectionId { get; set; }

        public bool IsCompleted { get; set; } = false;

        // ADD / CONFIRM THESE
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
    }

}
