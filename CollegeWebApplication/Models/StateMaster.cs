using System.ComponentModel.DataAnnotations;

namespace CollegeWebApplication.Models
{
    public class StateMaster
    {
        [Key]
        public int StateId { get; set; }

        [Required(ErrorMessage = "Please enter State Name.")]
        public string StateName { get; set; }
    }
}
