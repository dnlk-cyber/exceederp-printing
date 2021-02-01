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
    public class MaintenaceCostViewModel : Organization
    {
        public string EquipmentCategory { get; set; }

        public string EquipmentType { get; set; }
        public string AssetCode { get; set; }
        public string PlateNo { get; set; }
        public decimal SpareRepairCost { get; set; }
        public decimal BatteryRepairCost { get; set; }
                   public decimal LabourCost { get; set; }
        public decimal OtherRepairCost { get; set; }
        public decimal TyreRepairCost { get; set; }
        public decimal OilRepairCost { get; set; }
    }

    public class MaintenaceMechanicViewModel :Organization
    {
        public string EquipmentCategory { get; set; }

        public string EquipmentType { get; set; }
        public string AssetCode { get; set; }
        public int JobOrderNo { get; set; }
        public string Mechanic { get; set; }
        public DateTime Date { get; set; }
        public string ServiceName { get; set; }
        public decimal TotalWorked { get; set; }
      }
    public class MaintenanceViewModel : Organization
    {
        public Guid JobOrderID { get; set; }
        [DisplayName("Plate Number")]
        public Guid FleetsID { get; set; }
        [DisplayName("Job Order Number ")]

        public Nullable<int> JobOrderNum { get; set; }
        [DisplayName("Type ")]
        public string Type { get; set; }
        [DisplayName("Equipment Name ")]
        public string EquipmentName { get; set; }
        [DisplayName("Plate Number ")]
        public string PlateNum { get; set; }
        public string MaintenanceCenter { get; set; }
        [Required]
        [DisplayName("Work Unit")]
        public string ProjectName { get; set; }
        public string PercentCompleted { get; set; }
        [DisplayName(" Labor Cost ")]
        public decimal? LabourCost { get; set; }
        [DisplayName(" Material Cost ")]
        public decimal? MaterialCost { get; set; }

        public string Status { get; set; }

        [DisplayName("Date Issued  ")]
        public Nullable<DateTime> DateIssued { get; set; }
        [DisplayName("Date Completed   ")]
        public Nullable<DateTime> DateCompleted { get; set; }


        public string PlateNumber { get; set; }
    }
}
