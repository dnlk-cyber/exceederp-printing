
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Procurement.Tender
{
   public class TenderExtension
    {
       public int TenderExtensionId { get; set; }
        [Display(Name = "Closing Date ")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd :hh mm}", ApplyFormatInEditMode = true)]
        public DateTime ClosingDate { get; set; }
        [Display(Name = "Technical Opening Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd :hh mm}", ApplyFormatInEditMode = true)]
        public DateTime OpenigDate { get; set; }
        [Display(Name = "Technical Opening Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd :hh mm}", ApplyFormatInEditMode = true)]
        public DateTime? TechnicalOpenigDate { get; set; }
        [Display(Name = "Financial Opening Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd :hh mm}", ApplyFormatInEditMode = true)]
        public DateTime? FinancialOpenigDate { get; set; }
        public string Remark { get; set;  }
        public Tender Tender { get; set;  }
        public int TenderID { get; set;  }
       
   
    }
}

