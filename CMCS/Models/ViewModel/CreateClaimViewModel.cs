using System.ComponentModel.DataAnnotations;

namespace CMCS.ViewModels
{
    public class CreateClaimViewModel
    {
        [Required]
        [Display(Name = "Claim Date")]
        public DateTime ClaimDate { get; set; }

        [Required]
        [Range(0.5, 200, ErrorMessage = "Hours worked must be between 0.5 and 200")]
        [Display(Name = "Hours Worked")]
        public decimal HoursWorked { get; set; }

        [Required]
        [Range(0, 1000, ErrorMessage = "Hourly rate must be between 0 and 1000")]
        [Display(Name = "Hourly Rate")]
        public decimal HourlyRate { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        public string Department { get; set; }

        public List<IFormFile>? Documents { get; set; }
    }
}