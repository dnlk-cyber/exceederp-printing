using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ExceedERP.Core.Domain.CRM;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Web.Areas.CRM.Models
{
     public class BindingModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
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