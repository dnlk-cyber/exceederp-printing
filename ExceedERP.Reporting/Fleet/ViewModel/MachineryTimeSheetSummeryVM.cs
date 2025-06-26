using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.ViewModel
{
    public class MachineryTimeSheetSummeryHeaderVM
    {
        public string EquipmentName { get; set; } 
        public string OwnerName { get; set; }
        public string EquipmentType { get; set; }
        public DateTime DurationFrom { get; set; }
        public DateTime DurationTo { get; set; }
        public string Duration { get; set; }
        public string Period { get; set; }
        public string PlateNumber { get; set; }
        public string OperatorName { get; set; }
        public string Project { get; set; }
        public string PreparedBy { get; set; }
        public string PreparedDate { get; set; }
        public string CheckedBY { get; set; }
        public string CheckedDate { get; set; }
        public string ApprovedBy { get; set; }
        public string ApprovedDate { get; set; }
        public string AuthorizedBy { get; set; }
        public string AuthorizedDate { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedDate { get; set; }
        public string TimesheetSendForApprovalBy { get; set; }
        public string TimesheetSendForApprovalDate { get; set; }
        public string TimesheetApprovedBy { get; set; }
        public string TimesheetApprovedDate { get; set; }

    }
    public class MachineryTimeSheetSummeryVM
    {
        public DateTime Date { get; set; }
        public string WorkItem { get; set; }
        public string Category { get; set; }
        public string EquipmentName { get; set; }
        public string PlateNumber { get; set; }
        public decimal OperationNetworkinHour { get; set; }
        public decimal OperationRatePerHour { get; set; }
        public decimal OpertionRevenue { get; set; }
        public decimal IdleHour { get; set; }
        public decimal IdleRatePerHour { get; set; }
        public decimal IdleRevenue { get; set; }
        public decimal DownHour { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal FuelInLiter { get; set; }
        public decimal FuelRate { get; set; }
        public decimal FuelCost { get; set; }
        public decimal LubricantCost { get; set; }
        public decimal MaintenanceCost { get; set; }
        public decimal TireCost { get; set; }
        public decimal Perdium { get; set; }
        public decimal Salary { get; set; }
        public decimal TotalCost { get; set; }
        public decimal NetIncome { get; set; }
    }
    
}
