using System;
using System.Collections.Generic;
using System.Web;

namespace ExceedERP.Web.Areas.CRM.Models
{
    public class BindingModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
    public class InvoicePosPrintBindingViewModel
    {
        public string Id { get; set; }
        public string BaseURL { get; set; }
        public string PosId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    public class InvoiceSyncFromPosBindingViewModel
    {
        public DateTime FromTime { get; set; }
        //public DateTime ToTime { get; set; }

        public string BaseURL { get; set; }
        //public string PosId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
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