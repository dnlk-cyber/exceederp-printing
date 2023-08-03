using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Procurement.Tender
{
    public class TenderMinuteUploadDBModel
    {
        public int TenderMinuteUploadDBModelId { get; set; }
        public int TenderID { get; set; }
        public virtual Tender Tender { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set;  }
    }
    
    public class TenderBidDocument
    {
        public int TenderBidDocumentId { get; set; }
        public int TenderID { get; set; }
        public virtual Tender Tender { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set;  }
    }
}
