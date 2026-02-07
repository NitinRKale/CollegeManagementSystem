using System.ComponentModel.DataAnnotations;

namespace CollegeWebApplication.Models
{
    public class StudentInfo
    {
        [Key]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "student first name is required.")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "first name must be between 3 and 20 characters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter student last name.")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "last name must be between 3 and 20 characters.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please select birth date.")]
        [Display(Name = "Birth Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateBirth { get; set; }

        [Required(ErrorMessage ="Please select gender"), StringLength(15)]
        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Please enter student address.")]
        [Display(Name = "Student Address")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Please enter student city.")]
        [Display(Name = "Student Address")]
        public string City { get; set; }

        [Required(ErrorMessage = "Please enter student email.")]
        [Display(Name = "Student Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter phone number.")]
        [StringLength(13, MinimumLength = 10, ErrorMessage = "Please enter valid 10 digit phone number.")]
        public string PhoneNumber { get; set; }
        public string Stream { get; set; }
        public  string YearOfStudy { get; set; }
    }
}
