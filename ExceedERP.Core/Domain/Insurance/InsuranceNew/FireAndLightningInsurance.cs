using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
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
    public class FireAndLightningInsurance : DepartmentNewOperations
    {
        public int FireAndLightningInsuranceId { get; set; }
        [DisplayName("Request Recorder ")]
        public int EmployeeId { get; set; }
        [DisplayName("Location/Branch")]
        public int BranchId { get; set; }
        [DisplayName("Fiscal Year")]
        public int  GlFiscalYearId { get; set; }
        public virtual  GlFiscalYear GlFiscalYear { get; set; }
        [DisplayName("Accident Detail")]
        [Required]
        public string AccidentDetail { get; set; }
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
        public bool IsVerified { get; set; }
        public bool IsTheDocumentComplete { get; set; }
        public bool IsTheDocumentCompleteByEIC { get; set; }
        public bool IsPaid { get; set; }
        public ICollection<FireAndLightningInsuranceDetail> FireAndLightnings { get; set; }
       


    }
    public class FireAndLightningInsuranceDetail 
    {
        public int FireAndLightningInsuranceDetailId { get; set; }

        [DisplayName("Type of property to be insured ")]
        public string FleetsID { get; set; }
        [Display(Name = "Package value of the Property")]
        public string PackageValue { get; set; }
        public string Remark { get; set; }
        public int FireAndLightningInsuranceId { get; set; }
        public virtual FireAndLightningInsurance FireAndLightning { get; set; }
        public ICollection<FireAndLightningInsuranceAttachment> RenewalAttachments { get; set; }
        public ICollection<FireAndLightningInsuranceDepartmentApproval> DepartmentApprovals { get; set; }
        public ICollection<FireAndLightningInsuranceApproval> RenewalApprovals { get; set; }

    }

    
    public class FireAndLightningInsuranceAttachment
    {
        public int FireAndLightningInsuranceAttachmentId { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string ShortPath { get; set; }
        public string FileExtension { get; set; }
        public string Description { get; set; }
        public int FireAndLightningInsuranceDetailId { get; set; }
        public virtual FireAndLightningInsuranceDetail  InsuranceDetail { get; set; }

    }


    public class FireAndLightningInsuranceDepartmentApproval : TrackUserOperation
    {
        public int FireAndLightningInsuranceDepartmentApprovalId { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
        public int FireAndLightningInsuranceDetailId { get; set; }
        public virtual FireAndLightningInsuranceDetail InsuranceDetail { get; set; }


    }
    public class FireAndLightningInsuranceApproval : TrackUserOperation
    {
        public int FireAndLightningInsuranceApprovalId { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
        public int FireAndLightningInsuranceDetailId { get; set; }
        public virtual FireAndLightningInsuranceDetail InsuranceDetail { get; set; }

    }

}
