using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.ViewModels
{
    public class DumpTruckTimesheetHeaderViewModel
    {
        public string Project { get; set; } 
        public string OwnerName { get; set; }
        public string EquipmentType { get; set; }
        public DateTime? DurationFrom { get; set; }
        public DateTime? DurationTo { get; set; }
        public string Duration { get; set; }
        public string Period { get; set; }
        public string PlateNumber { get; set; }
        public string OperatorName { get; set; }
        public string EquipmentName { get; set; }
        public string PreparedBy { get; set; }
        public string PreparedDate { get; set; }
        public string ApprovedBy { get; set; }
        public string ApprovedDate { get; set; }
        public string AuthorizedBy { get; set; }
        public string AuthorizedDate { get; set; }
        public string CheckedBY { get; set; }
        public string CheckedDate { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedDate { get; set; }
        public string TimesheetapprovedBy { get; set; }
        public string TimesheetApprovedDate { get; set; }
        public string TimesheetSendforapprovalBy { get; set; }
        public string TimesheetSendforapprovalDate { get; set; }
    }
    public class DumpTruckTimesheetViewModel
    {
        public int SN { get; set; }
        public string PlateNo { get; set; }
        public string Category { get; set; }
        public string EquipmentName { get; set; }
        public string Branch { get; set; }
        public DateTime Date { get; set; }    
        public string WorkItem { get; set; }       
        public decimal OperationNetWorkingHour { get; set; }        
        public decimal NumberOfTrips { get; set; }        
        public decimal ActualDistance { get; set; }        
        public decimal capacity { get; set; }       
        public decimal OperationRateperHour { get; set; }        
        public decimal OperationRevenue { get; set; }        
        public decimal IdleHour { get; set; }        
        public decimal IdleRateperHour { get; set; }        
        public decimal IdleRevenue { get; set; }
        public decimal TotalRevenue { get; set; }       
        public decimal DownHour { get; set; }       
        public decimal FuelInLiter { get; set; }
        public decimal FuelRate { get; set; }
        public decimal FuelCost { get; set; }
        public decimal LubricantCost { get; set; }
        public decimal MaintenaceCost { get; set; }
        public decimal TireCost { get; set; }
        public decimal Salary { get; set; }
        public decimal Perdium { get; set; }
        public decimal TotalCost { get; set; }
        public decimal NetIncome { get; set; }
        public decimal TotalHaulage { get; set; }

    }
}
