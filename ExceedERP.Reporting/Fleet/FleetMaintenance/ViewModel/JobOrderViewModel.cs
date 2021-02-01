using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  ExceedERP.Reporting.Fleet.FleetMaintenance.ViewModel
{
        public  class JobOrderViewModel : Organization
    {

                 public Guid JobOrderID { get; set; }
         public Guid FleetsID { get; set; }
         public Nullable<int> JobOrderNum { get; set; }
         public string Type { get; set; }
         public int RequestNumber { get; set; }
         public decimal LastKMHR { get; set; }
         public string IsPM { get; set; }
         public string IsM { get; set; }
         public string Cause { get; set; }
         public string PlateNumber { get; set; }
         public string EquipmentName { get; set; }
         public string EquipmentType { get; set; }
         public string Manufacture { get; set; }
         public string Model { get; set; }
         public string Requester { get; set; }
         public string OperatorName { get; set; }
         public string FuelAmount { get; set; }
         public decimal Odometer { get; set; }
         public string AssetNum { get; set; }
         public string ProjectName { get; set; }
 
        public string MaintenanceCenter { get; set; }
         
        public string PercentCompleted { get; set; }
         public Nullable<DateTime> DateIssued { get; set; }
         public Nullable<DateTime> DateCompleted { get; set; }
        public string InspectedBy { get; set; }
        public enum InspectionStatus { Approve, Reject, RollBack}

        public string Section { get; set; }
        public string ServiceCode { get; set; }
 
       public string Inpectorcomment { get; set; }
        public string ChassisNumber { get; set; }
        public string EngineNumber { get; set; }
        public string Driver { get; set; }
        public string TecnicallyApproved { get; set; }
        public string  AdminstrativeApproved { get; set; }
        public string odometer { get; set; }
        public string ServiceName { get; set; }
        public string Status { get; set; }
        public string ApprovalStatus { get; set; }



        public double Duration { get; set; }
        public decimal SpareCost { get; set; }
        public decimal TyreCost { get; set; }
        public decimal OilCost { get; set; }
        public decimal OtherCost { get; set; }
        public decimal LabourCost { get; set; }
        public decimal ExternalRepairCost { get; set; }
        public decimal SalvageCost { get; set; }
        public decimal TotalCost { get; set; }


    }
}
