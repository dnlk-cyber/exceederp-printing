using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace  ExceedERP.Reporting.Fleet.FleetMaintenance.Printing.ViewModel
{
        public  class JobOrderView : Organization
    {

                 public Guid JobOrderID { get; set; }
        [DisplayName("Plate Number")]
        public Guid FleetsID { get; set; }
        [DisplayName("Job Order Number ")]
   
        public Nullable<int> JobOrderNum { get; set; }
         [DisplayName("Maintenance Center ")]
        public string Type { get; set; }
       
        public string MaintenanceCenter { get; set; }
        
          [DisplayName("Project Name")]
        public string ProjectName { get; set; }
        public string PercentCompleted { get; set; }
     //   public string Status { get; set; }
         [DisplayName("Date Issued  ")]
        public Nullable<DateTime> DateIssued { get; set; }
               [DisplayName("Date Completed   ")]
        public Nullable<DateTime> DateCompleted { get; set; }
        public string InspectedBy { get; set; }
        public enum InspectionStatus { Approve, Reject, RollBack}
       
        [DisplayName(" Reason For Job ")]
     
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
