using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace ExceedERP.Core.Domain.Procurement.Tender
{
    public class TenderMinuteViewModel
    {
        public int TenderID { get; set; }
        public virtual Tender Tender { get; set; }
        [DisplayName("Please select and upload file")]
        [Required]
        public HttpPostedFileBase File { get; set; }
    }
}
