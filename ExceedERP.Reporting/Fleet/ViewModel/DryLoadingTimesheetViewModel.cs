using ExceedERP.Core.Domain.FleetManagement.FleetTimeSheet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.ViewModel
{
    public class DryLoadingTimesheetHeaderVM
    {
        public string EquipmentName { get; set; }
        public string OperatorName { get; set; }
        public string OwnerName { get; set; }
        public string EquipmentType { get; set; }
        public DateTime DurationFrom { get; set; }
        public DateTime DurationTo { get; set; }
        public string Duration { get; set; }
        public string Period { get; set; }
        public string PlateNumber { get; set; }
        public string Project { get; set; }
        public string EqipmentName { get; set; }
        public string PreparedBy { get; set; }
        public string PreparedDate { get; set; }
        public string ApprovedBy { get; set; }
        public string ApprovedDate { get; set; }
        public string AuthorizedBy { get; set; }
        public string AuthorizedDate { get; set; }
        public string CheckedBY { get; set; }
        public string CheckedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public string ModifiedDate { get; set; }
        public string TimesheetSendForApprovalBY { get; set; }
        public string TimesheetSendForApprovalDate { get; set; }
        public string TimesheetApporvedBy { get; set; }
        public string TimesheetApporvedDate { get; set; }

    }
    public class DryLoadingTimesheetViewModel
    {
        public int  Sn { get; set; }
        public string WorkType { get; set; }
        public string PlateNumber { get; set; }
        public string EquipmentCategory { get; set; }
        public string EquipmentName { get; set; }
        public string DeparturePlace { get; set; }
        public string ArrivalPlace { get; set; }
        public DateTime StartingDate { get; set; }
        public DateTime EndDate { get; set; }
        public FleetMesuringCategory Category { get; set; }
        public int NoOfTripe { get; set; }
        public decimal AsphaltTraveled { get; set; }
        public decimal GravelTraveled { get; set; }
        public decimal AsphaltRate { get; set; }
        public decimal GravelRate { get; set; }
        public string LoadedType { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal FuelInLiter { get; set; }
        public decimal FuelRate { get; set; }
        public decimal FuelCost { get; set; }
        public decimal LubricantCost { get; set; }
        public decimal MaintenaceCost { get; set; }
        public decimal TireCost { get; set; }
        public decimal LoadingUnloadingCost { get; set; }
        public decimal Perdium { get; set; }
        public decimal Salary { get; set; }
        public decimal TotalCost { get; set; }
        public decimal NetIncome { get; set; }
        public string PreparedBy { get; set; }
        public DateTime? PreparedDate { get; set; }
        public string Name { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string ApproverName { get; set; }

    }
}
