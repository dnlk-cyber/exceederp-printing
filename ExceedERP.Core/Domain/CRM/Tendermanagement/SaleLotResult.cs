using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.CRM.TenderManagement

{
    public class SaleLotResult
    {
        [Display(Name = "Evaluation Date"),
      DataType(DataType.Date),
      DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EvaluationDate { get; set; }
        public int ProductID { get; set; }
        public int ProductCatagoryId { get; set; }
        public int TotalPrice { get; set; }
        public int UnitPrice { get; set; }

    }
}
