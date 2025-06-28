using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.CRM.TenderManagement
{
   
    public class SalesLotEvaluationByDocument : TrackUserOperation
    {
        public int SalesLotEvaluationByDocumentId { get; set; }
        public int SalesLotId { get; set; }
       // public virtual Lot Lot { get; set; }
        [Display(Name = "Customer Name")]//supplier database
        public string TinNumber { get; set; }       
        public string Document { get; set; }
        [Display(Name = "Not Available?")]
        public bool IsNotAvailable { get; set; }
        public string Remark { get; set; }
    }
}
