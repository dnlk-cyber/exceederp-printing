using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.TrainingEducation
{
    public class TrainingImpactAssessment: DepartmentOperations
    {
        [Key]
        public int TrainingImpactAssessmentId { get; set; }

        // Employee Info
        [Display(Name = "HR_EmployeeDetail_Placement_PersonEmployeeID", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        // Training Info
        public string TrainingTitle { get; set; }
        public TrainingLocationType TrainingType { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Training  Date")]
        public DateTime? TrainingDate { get; set; }
        [Display(Name = "Training  Date")]
        public string TrainingDateAm { get; set; }
        // Navigation
        public ICollection<TrainingImpactResponse> Responses { get; set; }
        // Open-ended questions
        public string EfficiencyImpactDescription { get; set; }
    }

    public class TrainingImpactResponse:TrackUserOperation
    {
        public int TrainingImpactResponseId { get; set; }

        public int TrainingImpactAssessmentId { get; set; }
        public TrainingImpactAssessment TrainingImpactAssessment { get; set; }

        public int TrainingImpactQuestionId { get; set; }
        public TrainingImpactQuestion Question { get; set; }

        // 1 to 5 scale
        public int Rating { get; set; }
    }

    //setting
    public class TrainingImpactQuestion: TrackUserOperation
    {
        [Key]
        public int TrainingImpactQuestionId { get; set; }

        [Display(Name = "Question")]
        [Required]
        public string QuestionText { get; set; } // e.g., "The training improved your understanding of safety..."

        public TrainingCategory TrainingCategory { get; set; } // To keep them in 1, 2, 3... sequence
        public int DisplayOrder { get; set; } // To keep them in 1, 2, 3... sequence

        public bool IsActive { get; set; } = true; // Easily "delete" a question by setting this to false
    }

    public enum TrainingCategory
    {
        [Display(Name = "Integrated Management System (IMS)")]
        IMS = 1, // IMS = Integrated Management System

        [Display(Name = "Change and Workplace Cooperation (WPC)")]
        WPC, // WPC = Workplace Cooperation

        [Display(Name = "Enterprise Resource Planning (ERP)")]
        ERP, // ERP = Enterprise Resource Planning

        [Display(Name = "Construction Safety")]
        ConstructionSafety, // Construction Safety Training

        [Display(Name = "Primavera P6")]
        PrimaveraP6, // Primavera P6 = Project Planning & Scheduling Tool

        [Display(Name = "Kaizen Management System")]
        Kaizen, // Kaizen = Continuous Improvement Philosophy

        [Display(Name = "International Financial Reporting Standard (IFRS)")]
        IFRS // IFRS = International Financial Reporting Standards
    }
    public enum TrainingLocationType
    {
        Internal = 1,
        OutsideCountry = 2
    }
}
