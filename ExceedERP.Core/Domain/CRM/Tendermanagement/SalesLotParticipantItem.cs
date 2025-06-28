using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.CRM.TenderManagement

{
    public class SalesLotParticipantItem : TrackUserOperation
    {
        public int SalesLotParticipantItemId { get; set; }
        public int SalesLotParticipantId { get; set; }
        public int ProductCategoryId { get; set; }
        public int ProductID { get; set; }
        public decimal Quantity { get; set; }
        [Display(Name = "Evaluation Date"),
        DataType(DataType.Date),
        DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EvaluationDate { get; set; }
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }
        [Display(Name = "Base Price")]
        public decimal BasePrice { get; set; }
        [Display(Name = "Total Price")]
        public decimal TotalPrice { get; set; }
      
          public virtual SalesLotParticipant Participant { get; set; }

    }
}
