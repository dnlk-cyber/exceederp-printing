using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement.FuelManagement;

namespace ExceedERP.Core.Domain.Finance.AP.Travel
{

	public class ApTravelExpenseClaim : DepartmentNewOperations
	{
		public int ApTravelExpenseClaimId { get; set; }
		[Display(Name = "Travel Order No.")]
		public int? ApTravelOrderId { get; set; }
		[Required]
		[Display(Name = "Name")]
		public int EmployeeId { get; set; }
		
		[Required]
		[DisplayName("Work Unit")]
		public int OrgStructureId { get; set; }
		[Display(Name = "Purpose of Travel")]
		public String PurposeOfTravel { get; set; }
		[Display(Name = "Claim Date")]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		public DateTime ClaimDate { get; set; }
	   // public string PaymentMethod { get; set; }
		[Display(Name = "Reference")]
		public string Reference { get; set; }
		public bool IsForOther { get; set; }
		[Display(Name = "Shortage/Excess")]
		public decimal Difference { get; set; }
		public bool Paid { get; set; }
		//budget controll
		public bool IsBudgetApproved { get; set; }
		public string BudgetApprovedBy { get; set; }
		public DateTime? BudgetApprovedTime { get; set; }
		[Required]
		[Display(Name = "Branch")]
		public int BranchId { get; set; }
		[Display(Name = "Business Unit")]
		public int BusinessUnitId { get; set; }
		[Display(Name = "From Branch/Project")]
		public int FromBranchId { get; set; }
		[Display(Name = "From Business Unit")]
		public int FromBusinessUnitId { get; set; }
		//for access or view
		public int CreatedAtBranchId { get; set; }
		public int CreatedAtBusinessUnitId { get; set; }
		public int CreatedAtOrgStructureId { get; set; }
		public virtual ICollection<ApTravelExpenseExpense> Expenses { get; set; }
		public virtual ICollection<ApTravelExpensePayment> Payments { get; set; }
		public virtual ICollection<ApTravelExpenseRefund> Refunds { get; set; }
		public virtual ICollection<ApTravelExpenseClaimAttachment> Attachments { get; set; }
		public virtual ICollection<ApTravelExpenseValidation> Validations { get; set; }
		public virtual ICollection<ApTravelExpenseDepartmentValidation> TravelExpenseDepartmentValidations { get; set; }
		public virtual ICollection<ApTravelExpenseBudgetValidation> ApTravelExpenseBudgetValidations { get; set; }
		public virtual ICollection<ApTravelExpenseDistribution> Distributions { get; set; }
		public virtual ICollection<APTravelClaimDeparture> APTravelClaimDepartures { get; set; }
		public virtual ICollection<ApTravelFuelExpenseClaim> ApTravelFuelExpenseClaims { get; set; }
	}

	public class ApTravelExpenseClaimAttachment
	{
		public int ApTravelExpenseClaimAttachmentId { get; set; }
		public int ApTravelExpenseClaimId { get; set; }
		public virtual ApTravelExpenseClaim ApTravelExpenseClaim { get; set; }
		public string FileName { get; set; }
		public string FilePath { get; set; }
	}

	public class ApTravelFuelExpenseClaim
	{
		public int ApTravelFuelExpenseClaimId { get; set; }
		public int ApTravelExpenseClaimId { get; set; }
		public virtual ApTravelExpenseClaim ApTravelExpenseClaim { get; set; }
		public string PlateNo { get; set; }
		public string InvoiceNo { get; set; }
		public string FuelStation { get; set; }
		public decimal DepartureKM { get; set; }
		public decimal ReturnKM { get; set; }
		public decimal DifferenceKM { get; set; }
		public FuelTypes FuelType { get; set; }
		public decimal FuelQTY { get; set; }
		public decimal FuelTotal { get; set; }
		public decimal OilQTY { get; set; }
		public decimal OilTotal { get; set; }
		[Display(Name = "Minor Maintenance")]
		public decimal Minormaintenance { get; set; }
		public decimal Total { get; set; }
	}

	public enum FuelTypes
	{
		Gasoline,
		Diesel,
		Other

	}

}
