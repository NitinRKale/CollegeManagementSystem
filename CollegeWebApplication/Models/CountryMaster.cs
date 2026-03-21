using System.ComponentModel.DataAnnotations;

namespace CollegeWebApplication.Models
{
    public class CountryMaster
    {
        [Key]
        public int CountryId { get; set; }
        [Required]
        public string CountryName { get; set; }
    }
}
