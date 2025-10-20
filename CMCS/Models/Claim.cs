using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using CMCS.Models;




namespace CMCS.Models
{


    public class Claim
    {
        [Key]
        public int ClaimId { get; set; }

        [Display(Name = "Claim Period")]
        [DataType(DataType.Date)]
        public DateTime ClaimDate { get; set; }

        [Display(Name = "Hours Worked")]
        [Range(1, 200, ErrorMessage = "Hours must be between 1 and 200.")]
        public decimal HoursWorked { get; set; }

        [Display(Name = "Hourly Rate")]
        [Range(0, 500, ErrorMessage = "Hourly Rate must be between 0 and 500.")]
        public decimal HourlyRate { get; set; }

        public string Description { get; set; }

        public string Department { get; set; }

        public ClaimStatus Status { get; set; } = ClaimStatus.Pending;

        
        public int LecturerId { get; set; }
        [ForeignKey("LecturerId")]
        public Lecturer Lecturer { get; set; }


        public int? ApprovedByCoordinatorId { get; set; }
        [ForeignKey("ApprovedByCoordinatorId")]
        public ProgrammeCoordinator? ApprovedByCoordinator { get; set; }
        public string? CoordinatorNotes { get; set; }


        public int? ApprovedByManagerId { get; set; }
        [ForeignKey("ApprovedByManagerId")]
        public AcademicManager? ApprovedByManager { get; set; }
        public string? ManagerNotes { get; set; }

        // Add these missing date fields
        public DateTime? CoordinatorApprovalDate { get; set; }
        public DateTime? ManagerApprovalDate { get; set; }

        public DateTime CreatedDate { get; set; }
        public ICollection<Document> Documents { get; set; }
    }

    public enum ClaimStatus
    {
        Pending,
        ApprovedByCoordinator,
        ApprovedByManager,
        RejectedByManager,
        ReturnedToCoordinator,
        FullyApproved,
        Rejected
    }
}
