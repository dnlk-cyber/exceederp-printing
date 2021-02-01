using System.Collections.Generic;
using System.Web;

namespace ExceedERP.Web.Areas.InfoDesk.Models
{
    public class BindingModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Remark { get; set; }
    }

     public class CashPaymentViewModel
     {
         public string Id { get; set; }
         public string PaymentType { get; set; }
     }
    public class UploadBindingModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<HttpPostedFileBase> AttachmentFile { get; set; }
    }
}