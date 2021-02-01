using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.ViewModel
{
   public class PlanVsAccomplishmentViewModel
    {

        public string Project { get; set; }
        public string Period { get; set; }
        public string NoofDay { get; set; }
        public string NoWorkingDay { get; set; }
        public Guid EquipmentTypeId { get; set; }
        public string EquipmentType { get; set; }
        public decimal PlanNo { get; set; }
        public decimal PlanKmHr { get; set; }

        public decimal AvailableNo { get; set; }
        public decimal AvailableKmHr { get; set; }


        public decimal UtilizedNo { get; set; }
        public decimal UtilizedNormalKmHr { get; set; }
        public decimal UtilizedOverKmHr { get; set; }
        public decimal UtilizedTotalKmHr { get; set; }
        public decimal UtilizedTotalKm { get; set; }

        public decimal DiffrenceUnassignedNo { get; set; }
        public decimal DiffrenceUnassignedHr { get; set; }
        public decimal DiffrenceIdealHr { get; set; }
        public decimal IdealReason { get; set; }


        public decimal DiffrenceRentassignedNo { get; set; }
        public decimal DiffrenceRentassignedHr { get; set; }

        public decimal DiffrenceOtherassignedHr { get; set; }

        public decimal EnterprisePercent { get; set; }
        public decimal RentAssigned { get; set; }

        public decimal Total { get; set; }
     
    
    }
}
