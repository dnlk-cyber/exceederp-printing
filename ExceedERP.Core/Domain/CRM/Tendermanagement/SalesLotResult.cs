using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.CRM.TenderManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.TenderManagement
{
    public class SalesLotResult : DepartmentNewOperations
    {
        public int SalesLotResultId { get; set; }
        [Display(Name = "Supplier Name")] 
        public string SupplierName { get; set; }
        public decimal Result { get; set; }
        public ICollection<SalesLotResultItem> SalesLotResultItems { get; set; }
        [Display(Name = "Item Name")]
        public int SalesLotId { get; set; }
        public virtual SalesLot Lot { get; set; }
    }

}
