using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.CRM.TenderManagement

{
    public class SaleTender: Operations
    {
        public int SaleTenderId { get; set; }
        [Display(Name = "Float Date"),
        DataType(DataType.Date),
        DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]   
        public DateTime FloatDate { get; set; } 
        [Display(Name = "Closing Date"),
        DataType(DataType.Date),
        DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ClosingDate { get; set; }
        [Display(Name = "Evaluation Date"),
        DataType(DataType.Date),
        DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EvaluationDate { get; set; }
        public string TenderName { get; set; }
        public string TenderReference { get; set; }
        public ICollection<SalesLot> Lots { get; set; }


    }
}
