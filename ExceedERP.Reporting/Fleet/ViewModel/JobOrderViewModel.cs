using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Fleet.ViewModel
{
    public class JobOrderViewModel  :Organization
    {


        public Nullable<int> JobOrderNum { get; set; }
          public string Equipment { get; set; }
        public string Type { get; set; }
        public string EquipmentName { get; set; }
        public string PlateNum { get; set; }
        public string MaintenanceCenter { get; set; }
        public string ProjectName { get; set; }
        public string PercentCompleted { get; set; }
        public decimal LabourCost { get; set; }
        public decimal Efficency { get; set; }
        public decimal MaterialCost { get; set; }

        public string Status { get; set; }

        public string ApprovalStatus { get; set; }
        public Nullable<DateTime> DateIssued { get; set; }
        public Nullable<DateTime> DateCompleted { get; set; }
        public string InspectedBy { get; set; }
        public enum InspectionStatus { Approve, Reject, RollBack }
        public string InspectorComments { get; set; }
    }
}
