using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Procurement.Need
{

    public class AnnualNeedAssesment : TrackTransaction
    {
        public int AnnualNeedAssesmentID { get; set; }
        [Display(Name = "Year")]
        public int GlFiscalYearId { get; set; }
        [Required]
        [DisplayName("Branch or Project")]
        public int BranchId { get; set; }
        [DisplayName("Business Unit")]
        public int  BusinessUnitId { get; set; }
        [Required]
        [DisplayName("Work Unit")]
        public int OrgStructureId { get; set; }
        [Required]
        [Display(Name = "Requester Name")]
        public int EmployeeId { get; set; }
        public string Remark { get; set; }
        public bool IsTransfferedToPlan { get; set; }
        public virtual ICollection<AnnualNeedAssesmentItem> AnnualNeedAssesmentItem { get; set; }
        public virtual ICollection<AnnualNeedAssesmentValidation> AnnualNeedAssesmentValidation { get; set; }
    }
}
