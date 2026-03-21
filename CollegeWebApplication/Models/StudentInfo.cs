using System.ComponentModel.DataAnnotations;

namespace CollegeWebApplication.Models
{
    public class StudentInfo
    {
        [Key]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Please enter First Name.")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "first name must be between 3 and 20 characters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter Last Name.")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "last name must be between 3 and 20 characters.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please select Birth Date.")]
        [Display(Name = "Birth Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateBirth { get; set; }

        [Required(ErrorMessage ="Please select gender"), StringLength(15)]
        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Please enter Resident Address.")]
        [Display(Name = "Student Address")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Please enter City Name.")]
        [Display(Name = "Student City")]
        public string City { get; set; }

        [Required(ErrorMessage = "Please enter Email-ID.")]
        [Display(Name = "Student Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter valid Phone Number.")]
        [StringLength(13, MinimumLength = 10, ErrorMessage = "Please enter valid 10 digit phone number.")]
        public string PhoneNumber { get; set; }
        public string Stream { get; set; }
        public  string YearOfStudy { get; set; }

      //  public DateTime? InsertDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
