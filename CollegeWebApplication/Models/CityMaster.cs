using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeWebApplication.Models
{
    public class CityMaster
    {
        [Key]
        public int CityId { get; set; }
        
        [Required(ErrorMessage = "Please enter City Name.")]
        public string CityName { get; set; }

        [ForeignKey("StateMaster")]
        [Required(ErrorMessage = "Please Select State")]
        [Display(Name = "State")]
        public int StateId { get; set; }

        public string ? StateName { get; set; }

        public StateMaster? StateMaster { get; set; }
    }
}
