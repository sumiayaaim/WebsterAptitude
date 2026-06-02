using System;

namespace WebsterAptitude.Models
{
    public class CandidateTestAttempt
    {
        public int Id { get; set; }
        public string CandidateUserId { get; set; } = default!;
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public bool IsCompleted { get; set; } = false;
    }
}
