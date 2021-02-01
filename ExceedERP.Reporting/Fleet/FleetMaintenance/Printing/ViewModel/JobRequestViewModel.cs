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
    public class JobRequestViewModel : Organization
    {

        public int Num { get; set; }
        public int RequestNumber { get; set; }
          public bool IsPM { get; set; }
        public bool IsM { get; set; }
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
 
        public string Cause { get; set; }
        public string Status { get; set; }
        public decimal LastKMHR { get; set; }
        public string CauseDescription { get; set; }

        public string RequestedBy { get; set; }
        public string ApprovedBy { get; set; }

        


    }
}
