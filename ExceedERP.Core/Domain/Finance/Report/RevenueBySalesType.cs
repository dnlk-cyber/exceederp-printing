using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Finance.Report
{
    public class RevenueBySalesType
    {
        public int RevenueBySalesTypeId { get; set; }
        public virtual ICollection<RevenueList> Revenues { get; set; }
    }

    public class RevenueList
    {
        public int RevenueListId { get; set; }
        public int RevenueBySalesTypeId { get; set; }
        public virtual RevenueBySalesType RevenueBySalesType { get; set; }
        [Required]
        public string Description { get; set; }
        [Display(Name = "Note Number")]
        public string NoteNumber { get; set; }
        public virtual ICollection<RevenueListRange> RevenueListRanges { get; set; }
    }

    public class RevenueListRange : ReportSetupBase
    {
        public int RevenueListRangeId { get; set; }
        public int RevenueListId { get; set; }
        public virtual RevenueList RevenueList { get; set; }
    }


}
