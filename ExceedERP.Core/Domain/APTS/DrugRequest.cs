using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.APTS
{
   public class DrugRequest : AptsStatus
    {
        public int DrugRequestId { get; set; }
        public int BranchId { get; set; }
        public virtual Branch Branch { get; set; }
        public int GlFiscalYearId { get; set; }
        public virtual GlFiscalYear GlFiscalYear { get; set; }
        public DateTime RequestDate { get; set; }
        public string RequestDateAm { get; set; }
        public string RequestedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public string Remark { get; set; }        
        public bool IsConfirmed { get; set; }
        public bool IsClosed { get; set; }       
        public string StoreCode { get; set; }

    }
}
