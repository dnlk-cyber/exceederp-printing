using System;
using System.Drawing;

namespace ExceedERP.Reporting.SharedDataSources.ViewModel
{
    public class ApprovalVm
    {
        public string Description { get; set; }
        public string ApproverName { get; set; }
        public DateTime? Date { get; set; }
        public Image Signature { get; set; }

    }
}
