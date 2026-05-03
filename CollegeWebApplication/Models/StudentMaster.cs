using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeWebApplication.Models
{
    public class StudentMaster
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

        [Required(ErrorMessage = "Please select gender"), StringLength(15)]
        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Please select birth date.")]
        [Display(Name = "Birth Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        //[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime? BirthDate { get; set; }

        [Required(ErrorMessage = "Please enter valid mobile number.")]
        [StringLength(13, MinimumLength = 10, ErrorMessage = "Please enter valid 10 digit mobile number.")]
        public string MobileNo { get; set; }

        [Required(ErrorMessage = "Please enter valid email.")]
        [Display(Name = "Student Email")]
        public string EmailId { get; set; }

        [Required(ErrorMessage = "Please enter valid address.")]
        [Display(Name = "Student Address")]
        public string Address { get; set; }

        [ForeignKey("StateMaster")]
        [Required(ErrorMessage = "Please select state")]
        [Display(Name = "State Name")]
        public int StateId { get; set; }

        public StateMaster? StateMaster { get; set; }

        [ForeignKey("CityMaster")]
        [Required(ErrorMessage = "Please select city")]
        [Display(Name = "City Name")]
        public int CityId { get; set; }

        public CityMaster? CityMaster { get; set; }

        [Required(ErrorMessage = "Please enter valid pincode.")]
        [Display(Name = "Student Pincode")]
        public string Pincode { get; set; }

        [ForeignKey("CourseMaster")]
        [Required(ErrorMessage = "Please select course")]
        [Display(Name = "Course Name")]
        public int CourseId { get; set; }

        public CourseMaster? CourseMaster { get; set; }

        [Required(ErrorMessage = "Please select academic year.")]
        [Display(Name = "Academic Year")]
        public string YearOfStudy { get; set; }

        [Required(ErrorMessage = "Please select student status")]
        [Display(Name = "IsActive")]
        public bool IsActive { get; set; }

        //public DateTime? CreateDate { get; set; } = DateTime.Now;

        //public DateTime? UpdatedDate { get; set; }
    }
}
