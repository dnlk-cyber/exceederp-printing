using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.ViewModel
{
    public class ExternalRentViewModel : Organization
    {
         public int RentNo { get; set; }

        public enum EquipmentTypeenum { DoubleCap, Dump, Track, Excavator }
        public string EquipmentType { get; set; }
        public string EquipmentCategory { get; set; }
         public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string Equipment { get; set; }
        public decimal TotalCost { get; set; }
        public decimal EquipmentCost { get; set; }
         public string CompanyPhno { get; set; }

         public string CompanyEmail { get; set; }
         public string WorkingCapacity { get; set; }
          public bool RentalRateIncludingVAT { get; set; }

        public string Status { get; set; }
        public Nullable<int> Quantity { get; set; }
        public string FuelCoveredBy { get; set; }

         public string ProjectLocation { get; set; }
        public string UnitOfWorkMeasurement { get; set; }

         public string Description { get; set; }

         public string NameBankBranch { get; set; }

          public string BankFullAddress { get; set; }

        public string BankSlipNo { get; set; }

        public Nullable<int> AdvancePayment { get; set; }

         
    }
}
