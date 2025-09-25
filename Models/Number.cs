using System.ComponentModel.DataAnnotations;

namespace test_project_main1.Models
{
    public class Number
    {
        public int Id { get; set; }

        [Required]
        public int Value { get; set; }
    }
}
