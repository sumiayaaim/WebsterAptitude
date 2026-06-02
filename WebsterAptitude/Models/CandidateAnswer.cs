namespace WebsterAptitude.Models
{
    public class CandidateAnswer
    {
        public int Id { get; set; }

        public int SectionAttemptId { get; set; }
        public int QuestionId { get; set; }

        public string SelectedOption { get; set; } = default!;
        public bool IsCorrect { get; set; }
        public int MarksObtained { get; set; }
    }
}
