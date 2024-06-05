using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Web.Areas.FleetManagement.Models
{
    public partial class FleetReturnViewModel : Operations
    {

        [DisplayName("Return")]
        public int FleetReturnId { get; set; }
        [DisplayName("Equipment")]
        public string FleetsId { get; set; }

        public string From { get; set; }
        [DisplayName("Delivered To")]
        public string DeliveredTo { get; set; }
        [DisplayName("Debit Account No")]
        public string DebitAccountNo { get; set; }
        [DisplayName("Credit Account No")]
        public string CreditAccountNo { get; set; }

        [DisplayName("Plate No")]
        public string PlateNo { get; set; }
        [DisplayName("Equipment Name")]
        public string EquipmentName { get; set; }
        public string Manufacturer { get; set; }
        [DisplayName("Chances / Serial No ")]
        public string ChancesSerialNo { get; set; }
        public string EngineNo { get; set; }
        [DisplayName("Owner Certificate No ")]
        public string OwnerCertificateNo { get; set; }

        public string Condition { get; set; }

        [DisplayName("Asset No")]
        public string AssetNo { get; set; }
        [DisplayName("Cause of Return")]
        public string Causeofreturn { get; set; }
        [DisplayName("Check List")]
        public string CheckList { get; set; }

        [NotMapped]
        [DisplayName("Category")]
        public string EquipmentCategoryId { get; set; }
        [DisplayName("Type")]
        [NotMapped]
        public string EquipmentTypeId { get; set; }

    }
    public class EquipmentTyreViewModel 
    {
        public Guid EquipmentTyreId { get; set; }

        public string StoreCode { get; set; }
        [Required]
        public string SerialNo { get; set; }
        public string Make { get; set; }
        public string MakeCountry { get; set; }
        public string Type { get; set; }
        public string Tube { get; set; }
        public string Size { get; set; }
        public decimal UnitPrice { get; set; }
         public Guid fleetId { get; set; }
        public bool Isattached { get; set; }
        public string Condition { get; set; }

        public decimal OdoMeter { get; set; }  
        public string Duration { get; set; }
        public DateTime DateCreated { get; set; }

    }
    public class FleetViewSelectedModel
    {
  

        public Guid FleetsID { get; set; }
        public string PlateNumber { get; set; }
        public string SerialNo { get; set; }
        public string EquipmentName { get; set; }
        public string UnitNum { get; set; }
        public string ChasisNumber { get; set; }

    }
    public class FleetViewModel
    {

        public Guid FleetsID { get; set; }
        public Guid OperatorID { get; set; }
        public Guid InsuranceID { get; set; }
        public Guid SheduleID { get; set; }
        public Guid EquipmentIdentityID { get; set; }
        public string EquipmentName { get; set; }
        public Guid EquipmentNameID { get; set; }
        public string Type { get; set; }
        public string UnitNum { get; set; }
        public string Manufacturer { get; set; }
        public Nullable<DateTime> ManufacturerYear { get; set; }
        public string Model { get; set; }
        public string Capacity { get; set; }
        public string Color { get; set; }
        public string TyreSize { get; set; }
        public string Status { get; set; }
        public string PlateNumber { get; set; }
        public string EngineNumber { get; set; }
        public string ChasisNumber { get; set; }
        public string FuelStandard { get; set; }
        public string RentalRate { get; set; }
        public enum WayOfSupply { Purchased, Donated, Transferred }
        public string SupplierCompany { get; set; }

        public Nullable<int> PreviousOdoMeter { get; set; }
        public Nullable<int> CurentOdoMeter { get; set; }
        public Nullable<int> PurchasingCost { get; set; }
        public Nullable<int> AnnualDepreciationCost { get; set; }
        public Nullable<int> DepreciationCost { get; set; }
        public string SerialNo { get; set; }
        public string Libre { get; set; }
    
        public string FuelType { get; set; }
        public string OdoMeter { get; set; }
        public enum StateOfOdoMeter { Functional, Malfucntioned }
        public string Attachment { get; set; }
        public string FleetPhoto { get; set; }
        public Nullable<System.DateTime> PurchasedDate { get; set; }
        public Nullable<System.DateTime> LastMaintained { get; set; }
    }
}