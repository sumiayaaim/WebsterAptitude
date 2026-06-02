using System;
using System.ComponentModel.DataAnnotations;

namespace WebsterAptitude.Models
{
    public class CandidateProfile
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = default!;  // Identity UserId

        [Required, StringLength(120)]
        public string FullName { get; set; } = default!;

        [EmailAddress, StringLength(120)]
        public string? Email { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
