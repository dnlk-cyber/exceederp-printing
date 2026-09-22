using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.FleetManagement.FleetTimeSheet
{
    public class RentedEquipmentMonthlyPaymentSummary : TrackUserSettingOperation
    {
        public int RentedEquipmentMonthlyPaymentSummaryId { get; set; }
        public int GlPeriodId { get; set; }
        public string EquipmentTypeId { get; set; }
        public string EquipmentType { get; set; }
        public string FleetId { get; set; }
        public bool IsPaymentLetterAttached { get; set; }
        public string PlateNo { get; set; }
        public decimal OperationDrilledHoleDepth { get;set;}
        public decimal NumberOfHolesDrilled { get;set;}
        public string CustomerName { get; set; }
        public string OperatorName { get; set; }
        public int OperatorId { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public string AgreementNo { get; set; }
        public decimal RentalRateHour { get; set; }
        public decimal RentalRateDay { get; set; }
        public DateTime AgreementDate { get; set; }
        public string TimeSheetPreparedBy { get; set; }
        public DateTime TimeSheetPreparedDate { get; set; }    
        public bool TimeSheetIsPrepared { get; set; }
        public string TimeSheetCheckedBy { get; set; }
        public DateTime TimeSheetCheckedDate { get; set; }
        public bool TimeSheetIsChecked { get; set; }
        public bool IsRejected { get; set; }
        public string TimeSheetApprovedBy { get; set; }        
        public DateTime TimeSheetApprovalDate { get; set; }
        public bool TimeSheetIsApproved { get; set; }
        public string TimeSheetAuthorizedBy { get; set; }
        public DateTime TimeSheetAuthorizedDate { get; set; }
        public bool TimeSheetIsAuthorized { get; set; }
        public string TimeSheetDGMApprovedBy { get; set; }
        public DateTime TimeSheetDGMApprovalDate { get; set; }
        public bool TimeSheetIsDGMApproved { get; set; }
        public decimal GrossBeforeVat { get; set; }
        public decimal VAT { get; set; }
        //public decimal TOT { get; set; }
        public decimal TotalWithVat { get; set; }
        public decimal AdvancePayedWithVat { get; set; }
        public decimal Deductable { get; set; }
        public decimal NetPayable { get; set; }
        public decimal TotalWorkedHR { get; set; }
        public bool IsWagonDrill { get; set; }
        public string TimeSheetEquipmentAdminOfficerPreparedBy { get; set; }
        public DateTime? TimeSheetEquipmentAdminOfficerPreparedDate { get; set; }
        public string TimeSheetEquipmentAdminOfficerPreparedBySign { get; set; }
        public bool TimeSheetIsEquipmentAdminOfficerPrepared { get; set; }
        public string TimeSheetEquipmentAdminCheckedBy { get; set; }
        public DateTime? TimeSheetEquipmentAdminCheckedDate { get; set; }
        public string TimeSheetEquipmentAdminCheckedBySign { get; set; }
        public bool TimeSheetIsEquipmentAdminChecked { get; set; }
        public bool TimeSheetIsOfficeEngineerChecked { get; set; }
        public bool TimeSheetIsConstructionEngineerApproved { get; set; }
        public bool TimeSheetIsDepartmentManagerApproved { get; set; }

        public string TimeSheetOfficeEngineerCheckedBy { get; set; }
        public DateTime? TimeSheetOfficeEngineerCheckedDate { get; set; }
        public string TimeSheetOfficeEngineerCheckedBySign { get; set; }
        public string TimeSheetConstructionEngineerApprovedBy { get; set; }
        public DateTime? TimeSheetConstructionEngineerApprovedDate { get; set; }
        public string TimeSheetConstructionEngineerApprovedBySign { get; set; }
        public string TimeSheetDepartmentManagerApprovedBy { get; set; }
        public DateTime? TimeSheetDepartmentManagerApprovedDate { get; set; }
        public string TimeSheetDepartmentManagerApprovedBySign { get; set; }
        [DisplayName("Refference")]
        public string Refference { get; set; }
        public bool Paid { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal AmountDue { get; set; }
        public decimal SparePartCost { get; set; }
        public decimal OilLubCost { get; set; }
        public decimal MaintenanceCost { get; set; }
        public decimal FuelCost { get; set; }
        // Finace 
        public bool IsFinanceSendForApproval { get; set; }
        public string FinanceSendForApprovalBy { get; set; }
        public DateTime? FinanceSendForApprovalTime { get; set; }
        public bool IsFinanceApproved { get; set; }
        public string FinanceApprovedBy { get; set; }
        public DateTime? FinanceApprovedTime { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
        public bool IsVoid { get; set; }
        public string VoidBy { get; set; }
        public DateTime? VoidTime { get; set; }
    }
    public class RentedEquipmentDailyOperation
    {
        public int RentedEquipmentDailyOperationId { get; set; }
        public int RentedEquipmentMonthlyPaymentSummaryId { get; set; }
        public DateTime Date { get; set; }
        public decimal WorkedHR { get; set; }
        public decimal PayedIdleHR { get; set; }
        public decimal TotalPayedHR { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "Morning Time Start")]
        public DateTime? MorningTimeStart { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "Morning Time End")]
        public DateTime? MorningTimeEnd { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "Afternoon Time Start")]
        public DateTime? AfternoonTimeStart { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "Afternoon Time End")]
        public DateTime? AfternoonTimeEnd { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "Night Time Start")]
        public DateTime? NightTimeStart { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "Night Time End")]
        public DateTime? NightTimeEnd { get; set; }
        public decimal InitialEngineHour { get; set; }
        public decimal FinalEngineHour { get; set; }
        public decimal DifferenceEngineHour { get; set; }
        public decimal DownHour { get; set; }
        public IdleReasonList IdleReasonList { get; set; }
        public DownReasonList DownReasonList { get; set; }
        public int OperatorId { get; set; }
        public string OperatorName { get; set; }
        public decimal SparePartCost { get; set; }
        public decimal OilLubCost { get; set; }
        public decimal MaintenanceCost { get; set; }
        public decimal FuelCost { get; set; }
        public int SiteFormanId { get; set; }
        public string SiteFormanName { get; set; }
        public decimal FuelInLiter { get; set; }
        public decimal FuelKMReading { get; set; }
        
        public string Activity { get; set; }
        public string Unit { get; set; }
        public decimal Volume { get; set; }
    }
    public class WagonDrillDailyOperation
    {
        public int WagonDrillDailyOperationId { get; set; }
        public int RentedEquipmentMonthlyPaymentSummaryId { get; set; }
        public DateTime Date { get; set; }
        public string PlateNumber { get; set; }
        public string Category { get; set; }
        public string EquipmentName { get; set; }
        public decimal TotalTime { get; set; }
        public decimal WorkedHR { get; set; }
        public string Activity { get; set; }
        public decimal OperationDrilledHoleDepth { get; set; }
        public decimal NumberOfHolesDrilled { get; set; }
        public decimal OperationRateForMeter { get; set; }
        public decimal OperationAmount { get; set; }
        public decimal PayedIdleHour { get; set; }
        public decimal IdleRateHour { get; set; }
        public decimal IdleAmount { get; set; }
        public decimal InitialEngineHour { get; set; }
        public decimal FinalEngineHour { get; set; }
        public decimal DifferenceEngineHour { get; set; }
        public decimal DownHour { get; set; }
        public IdleReasonList IdleReasonList { get; set; }
        public DownReasonList DownReasonList { get; set; }
        public int OperatorId { get; set; }
        public string OperatorName { get; set; }
        public int SiteFormanId { get; set; }
        public string SiteFormanName { get; set; }
        public decimal GrossBeforeVat { get; set; }
        public decimal VAT { get; set; }
        public decimal TOT { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalCost { get; set; }
        public decimal SparePartCost { get; set; }
        public decimal OilLubCost { get; set; }
        public decimal MaintenanceCost { get; set; }
        public decimal FuelCost { get; set; }
        public decimal NetAmount { get; set; }
        public decimal FuelInLiter { get; set; }
        public decimal FuelKMReading { get; set; }

        public string Unit { get; set; }
        public decimal Volume { get; set; }

    }
    public class RentalEquipmentPaymentSummaryDocument
    {
        public int RentalEquipmentPaymentSummaryDocumentId { get; set; }
        public int RentedEquipmentMonthlyPaymentSummaryId { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string ShortPath { get; set; }
        public string FileExtension { get; set; }
        public string Description { get; set; }

    }
    public class RentedEquipmentMonthlyPaymentSummaryValidation : VoucherValidation
    {
        public int RentedEquipmentMonthlyPaymentSummaryValidationId { get; set; }
        public int RentedEquipmentMonthlyPaymentSummaryId { get; set; }
        //public virtual RentedEquipmentMonthlyPaymentSummary RentedEquipmentMonthlyPaymentSummary { get; set; }
    }

}
