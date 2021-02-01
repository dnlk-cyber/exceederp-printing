using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.ViewModel
{
    public class TireViewModel :Organization
    {
        public string PlateNumber { get; set; }
        public string OperatorName { get; set; }
        public string TireSize { get; set; }
          public string TireType { get; set; }
        public enum TireTypeenum { Wire, Fiber }
         [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> InstalledDate { get; set; }
         public string Brand { get; set; }
         public string SerialNo { get; set; }
        public enum TireTubeenum { With, WithOut }
         public string TireTube { get; set; }
         public string Location { get; set; }
         public decimal Quantity { get; set; }

        public decimal TotalPrice { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
