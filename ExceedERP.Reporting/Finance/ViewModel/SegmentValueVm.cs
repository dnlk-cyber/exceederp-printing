using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Finance.ViewModel
{
    public class SegmentValueVm
    {
        public string Values { get; set; }
        public string Description { get; set; }
        [Display(Name = "Parent")]
        public string HasParent { get; set; }
        [Display(Name = "Is Enabled?")]
        public bool Enable { get; set; }
        [Display(Name = "Is Parent?")]
        public bool Parent { get; set; }
        [Display(Name = "Allow Budgeting")]
        public bool AllowBudgeting { get; set; }
        [Display(Name = "Allow Posting")]
        public bool AllowPosting { get; set; }
    }
}
