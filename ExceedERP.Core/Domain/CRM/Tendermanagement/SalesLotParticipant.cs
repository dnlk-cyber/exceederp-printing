using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.CRM.TenderManagement

{
    public class SalesLotParticipant : TrackUserOperation
    {
        public int SalesLotParticipantId { get; set; }      
        [Display(Name = "Customer Name")]//manual supplier
        public string CustomerName { get; set; }
        public int PurchaseId { get; set; }
        [Required]
        public string TinNumber { get; set; }
        public string Representative { get; set; }
        [Display(Name = "Address")]
        public string Address { get; set; }
        public string Telephone { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Purchase Date")]
        public DateTime PurchaseDate { get; set; }
        [Display(Name = "Receipt No.")]
        public string ReceiptNo { get; set; }
        [Display(Name = "Receipt Not Required?")]
        public bool ReceiptNotRequired { get; set; }
        public string Remark { get; set; }
        [NotMapped]
        public int Count { get; set; }
        [Display(Name = "Item Name")]
        public int SalesLotId { get; set; }
        public virtual SalesLot Lot { get; set; }

    }
}
