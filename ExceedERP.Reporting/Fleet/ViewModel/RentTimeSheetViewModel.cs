using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.ViewModel
{
   public class RentTimeSheetViewModel : Organization
    {

        public string Project { get; set; }
        public bool IsRentHeadOffice { get; set; }
        public bool IsRentProject { get; set; }
        public string RentLicenseNo { get; set; }
        public bool ByHourly { get; set; }
        public bool ByAmount { get; set; }
        public bool ByDay { get; set; }
        public bool ByOther { get; set; }
        public string RentedDate { get; set; }
        public decimal intialOdometer { get; set; }
        public decimal FinalOdometer { get; set; }
        public decimal TotalFuelIssue { get; set; }

        public string EquipmentType { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string SerialNo { get; set; }

        public string OperatorName { get; set; }
        public string RenterName { get; set; }


       //Detail

        public DateTime? Date { get; set; }
        public string TypeofWork { get; set; }
        public string Location { get; set; }
        public string Amount { get; set; }
        public string UOW { get; set; }
        public string LocationType { get; set; }
        public string MaterialType { get; set; }
        public string WeatherType { get; set; }
        public decimal FuelIssue { get; set; }
    }

   public class RentTimeSheetPerformanceViewModel : Organization
   {

       public string Project { get; set; }
       public bool IsRentHeadOffice { get; set; }
       public bool IsRentProject { get; set; }
       public string RentLicenseNo { get; set; }
       public bool ByHourly { get; set; }
       public bool ByAmount { get; set; }
       public bool ByDay { get; set; }
       public bool ByOther { get; set; }
       public string RentedDate { get; set; }
       public decimal intialOdometer { get; set; }
       public decimal FinalOdometer { get; set; }
       public decimal TotalFuelIssue { get; set; }

       public string EquipmentType { get; set; }
       public string Manufacturer { get; set; }
       public string Model { get; set; }
       public string SerialNo { get; set; }

       public string OperatorName { get; set; }
       public string RenterName { get; set; }

       //Detail

       public DateTime? Date { get; set; }
       public string StartedTime { get; set; }
       public string FinishedTime { get; set; }
       public string DiffrenceTime { get; set; }
       public string IdealStartedTime { get; set; }
       public string IdealFinishedTime { get; set; }
       public string IdealReason { get; set; }
       public string FailureStartedTime { get; set; }
       public string FailureFinishedTime { get; set; }
       public string IntialOdometerdetail { get; set; }
   }
}
