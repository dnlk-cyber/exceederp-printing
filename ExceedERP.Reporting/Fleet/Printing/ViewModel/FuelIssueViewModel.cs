using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Fleet.Printing.ViewModel
{
    public class FuelIssueViewModel : Organization
    {
         public string IssueID { get; set; }

        public string Reference { get; set; }
         public string  PlateNo { get; set; }

        public string  ReadingKM {get; set; }

         public string StoreCode { get; set; }

         public string InventoryCategoryName { get; set; }

        public string InventoryItemName { get; set; }
         public DateTime? IssuedDate { get; set; }
   public decimal  UnitPrice { get; set; }
         public decimal  ApprovedQuantity { get; set; }
         public decimal TotalCost { get; set; }
        public string Refrance { get;   set; }
    }






   
}
