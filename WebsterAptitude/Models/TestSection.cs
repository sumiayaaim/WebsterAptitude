using System.ComponentModel.DataAnnotations;

namespace WebsterAptitude.Models
{
    public class TestSection
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public int Order { get; set; }

        // ADD THIS
        public int TimeLimitSeconds { get; set; } = 1900; // 5 minutes
    }

}
