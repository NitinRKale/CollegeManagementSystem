using System.ComponentModel.DataAnnotations;

namespace CollegeWebApplication.Models
{
    public class CourseMaster
    {
        [Key]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Please enter Course Name.")]
        public string CourseName { get; set; }
    }
}
