using System.ComponentModel.DataAnnotations;

namespace WebsterAptitude.Models
{
    public class Question
    {
        public int Id { get; set; }

        public int SectionId { get; set; }

        [Required]
        public string QuestionText { get; set; } = default!;

        [Required]
        public string OptionA { get; set; } = default!;

        [Required]
        public string OptionB { get; set; } = default!;

        [Required]
        public string OptionC { get; set; } = default!;

        [Required]
        public string OptionD { get; set; } = default!;

        [Required]
        public string CorrectOption { get; set; } = default!; // A/B/C/D

        public int Marks { get; set; } = 1;
    }
}
