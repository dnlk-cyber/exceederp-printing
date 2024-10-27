using ExceedERP.Core.Domain.Common;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.Inventory
{


	public class StoreRequisitionBase : OperationWithSecondValidation
	{
		[Display(Name = "Request Reference")]
		public string Reference { get; set; }
		[Required]
		[DisplayName("Branch or Project")]
		public int BranchId { get; set; }
		[DisplayName("Business Unit")]
		public int BusinessUnitId { get; set; }
		[Required]
		[DisplayName("Work Unit")]
		public int OrgStructureId { get; set; }
		[Required]
		[Display(Name = "Requester Name")]
		public int  EmployeeId { get; set; }
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		[Display(Name = "Request Date")]
		public DateTime RequestDate { get; set; }
		[Required]
		[Display(Name = "Issuer(Sender) Store")]
		public string StoreCode { get; set; }
		[DisplayName("Receiver Store")]
		public string ToStoreCode { get; set; }
		[DisplayName("Is Fuel Request")]
		public bool IsFuel { get; set; }
		[DisplayName("Is Transfer")]
		public bool IsTransfer { get; set; }
		[Display(Name = "Period")]
		public int GLPeriodId { get; set; }
		[DisplayName("Is Fullfilled")]
		public bool IsFullfilled { get; set; }
		[DisplayName("Is For Other")]
		public bool IsForOther { get; set; }
	   
		[DisplayName("For Branch or Project")]
		public int? ForBranchId { get; set; }
		
		[Display(Name = "Model No.")]
		public string ModelNo { get; set; }
	   
		[DisplayName("For Maintenance?")]
		public bool IsForMaintenance { get; set; }
		[DisplayName("For Production?")]
		public bool IsForProduction { get; set; }
		[Display(Name = "Job Order No.")]
		public string JobOrderNo { get; set; }
		[Display(Name = "Service Job Order No.")]
		public string ServiceJobOrderNo { get; set; }
		[Display(Name = "Plate No / Tag No")]
		public string PlateNo { get; set; }
		[Display(Name = "External ?")]
		public bool IsExternalDriver { get; set; }
		[Display(Name = "External Driver")]
		public string ExternalDriver { get; set; }
		//for controll
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		[Display(Name = "Prev. Request Date")]
		public DateTime? PrevRequestDate { get; set; }
		[Display(Name = "Prev. KM")]
		public decimal PrevKm { get; set; }
		[Display(Name = "Prev. LT")]
		public decimal PrevLT { get; set; } //litre
		[Display(Name = "Current KM")]
		public decimal CurrentKm { get; set; }
		[Display(Name = "Diff. KM")]
		public decimal DifferenceKm { get; set; }
		[Display(Name = "Consumption  KM/LT")]
		public decimal Consumption { get; set; }
		[Display(Name = "Standard Consumption  KM/LT")]
		public decimal StandardConsumption { get; set; }
		[Display(Name = "External")]
		public bool External { get; set; }
		[Display(Name = "Requester Name")]
		public string ExternalRequesterName { get; set; }
		[Display(Name = "For External")]
		public string SupplierId { get; set; }
		[Display(Name = "Loan ?")]
		public bool IsLoan { get; set; }
		public Guid? VehicleDistributionID { get; set; }
		public int CreatedAtOrgStructureId { get; set; }


	}

	public class StoreRequisitionItemBase : TrackUserSettingOperation
	{
		[Required]
		[DisplayName("Item Category")]
		public string ItemCategoryCode { get; set; }
		public virtual ItemCategory ItemCategory { get; set; }
		[Required]
		[DisplayName("Item Code")]
		public string ItemCode { get; set; }
		//public string ItemName { get; set; }
		public virtual InventoryItem InventoryItem { get; set; }
		[Display(Name = "Part No.")]
		public string PartNo { get; set; }
		[Display(Name = "Model No.")]
		public string ModelNo { get; set; }
		[Display(Name = "Serial No.")]
		public string SerialNo { get; set; }
		[Display(Name = "Chasis No.")]
		public string ChasisNo { get; set; }
		[Display(Name = "Requested QTY")]
		public decimal RequestedQuantity { get; set; }
		[Display(Name = "Approved QTY")]
		public decimal ApprovedQuantity { get; set; }
		[Display(Name = "Issued QTY")]
		public decimal IssuedQuantity { get; set; }
		[Display(Name = "Purchase QTY")]
		public decimal PurchaseQuantity { get; set; }
		public bool IsApproved { get; set; }//item by item
		[Display(Name = "Specification")]
		public string ItemSpecification { get; set; }
		[Display(Name = "Generate PR")]
		public bool GeneratePurchaseReq { get; set; }
		[NotMapped]
		public string Unit { get; set; }
		[Display(Name = "Balance")]
		public decimal CurrentBalance { get; set; }
		[Display(Name = "Avg Price")]
		public decimal CurrentAPrice { get; set; }
		public decimal AgreementPrice { get; set; }
	}

	public class StoreRequisitionValidationBase : TrackUserSettingOperation
	{
		public ApprovalStatuses Status { get; set; }
		public string Remark { get; set; }
	}

	public class StoreRequisitionPropertyValidationBase : TrackUserSettingOperation
	{
		public ApprovalStatuses Status { get; set; }
		public string Remark { get; set; }
		public string Approver { get; set; }
	}

}