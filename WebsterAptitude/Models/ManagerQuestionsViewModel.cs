using System.Collections.Generic;

namespace WebsterAptitude.Models
{
    public class ManagerQuestionsViewModel
    {
        public TestSection Section { get; set; } = default!;
        public List<Question> Questions { get; set; } = new();
    }
}
