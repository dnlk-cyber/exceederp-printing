using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Insurance.InsuranceNew
{
    public class EquipmentAccidentClaim : DepartmentNewOperations
    {
        public int EquipmentAccidentClaimId { get; set; }
        [DisplayName("Accident")]
        public string AccidentID { get; set; }
        [DisplayName("Accident Detail")]
        [Required]
        public string AccidentDetail { get; set; }
        [DisplayName("Equipment ")]
        public string FleetsID { get; set; }
        [DisplayName("Request Recorder ")]
        public int RequestRecorderEmployeeId { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Accident Date")]
        public DateTime AccidentDate { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh:mm tt}")]
        [Display(Name = "Accident Date")]
        public DateTime AccidentTime { get; set; }
        [Display(Name = "Accident Date")]
        public string AccidentDateAm { get; set; }
        [Display(Name = "Description", ResourceType = typeof(Resources))]
        public string Description { get; set; }
        public string Machine { get; set; }
        [Display(Name = "Machine Operator Name")]
        public string MachineOperatorName { get; set; }
        public string Vehicle { get; set; }
        [Display(Name = "Plate Number")]
        public string PlateNumber { get; set; }
        [Display(Name = "Driver Name")]
        public string DriverName { get; set; }
        [Display(Name = "Is Collission with another Vehicle")]
        public bool  IsCollissionWithAnotherVehicle { get; set; }
        [Display(Name = "Collided Owner Name")]
        public string CollidedOwnerName { get; set; }
        [Display(Name = "Collided Plate No")]
        public string CollidedPlateNo { get; set; }
        [Display(Name = "Collided Adress")]
        public string CollidedAddress { get; set; }
        [Display(Name = "Collided Phone No_")]
        public string CollidedPhone { get; set; }
        [Display(Name = "Insurance Branch to be dealt with")]
        public string InsuranceBranch { get; set; }
        [Display(Name = "Payment Amount")]
        public decimal PaymentAmount { get; set; }
        public bool IsVerified { get; set; }
        public bool IsTheDocumentComplete { get; set; }
        public bool IsTheDocumentCompleteByEIC { get; set; }
        public bool IsPaid { get; set; }
        public AccidentedEquipmentType EquipmentType { get; set; }
        public ICollection<EquipmentAccidentClaimAttachment> EquipmentAttachments { get; set; }
        public ICollection<AccidentClaimDepartmentApproval> DepartmentApprovals { get; set; }
        public ICollection<AccidentClaimApproval> AccidentClaimApprovals { get; set; }

    }

    public enum AccidentedEquipmentType
    {
        Machine,
        Vehicle,
    }

    public class EquipmentAccidentClaimAttachment
    {
        public int EquipmentAccidentClaimAttachmentId { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string ShortPath { get; set; }
        public string FileExtension { get; set; }
        public string Description { get; set; }
        public int EquipmentAccidentClaimId { get; set; }
        public virtual EquipmentAccidentClaim AccidentClaims { get; set; }

    }


    public class AccidentClaimDepartmentApproval : TrackUserOperation
    {
        public int AccidentClaimDepartmentApprovalId { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
        public int EquipmentAccidentClaimId { get; set; }
        public virtual EquipmentAccidentClaim AccidentClaims { get; set; }

    }
    public class AccidentClaimApproval : TrackUserOperation
    {
        public int AccidentClaimApprovalId { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
        public int EquipmentAccidentClaimId { get; set; }
        public virtual EquipmentAccidentClaim AccidentClaims { get; set; }

    }
    public class PeopleInjuredInTheAccident : TrackUserOperation
    {
        public int PeopleInjuredInTheAccidentId { get; set; }
        [DisplayName("Injured Name")]
        public string  InjuredName { get; set; }
        [DisplayName("Accident Type")]
        public string AccidentType { get; set; }
        [DisplayName("Injured Detail")]
        public string  InjureDetail { get; set; }
        public string Adress { get; set; }
        public string Phone { get; set; }
        public int EquipmentAccidentClaimId { get; set; }
        public virtual EquipmentAccidentClaim AccidentClaims { get; set; }

    }

}
