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
    public class FleetViewModel : Organization
    {


        public const string ACTIVE = "ACTIVE";
        public const string ONDUTY = "On Duty";
        public const string OnMaintenance = "On Maintenance";
        public int Id { get; set; }
        public Guid FleetsID { get; set; }

      public Guid EquipmentIdentityID { get; set; }
         public Guid EquipmentTypeID { get; set; }
         public Guid PlannerID { get; set; }
        public string EquipmentName { get; set; }
        public string Heading { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }
        public string Operator { get; set; }

        public string CreatedBy { get; set; }
        public string SendForApprovalBy { get; set; }
        public string ApprovedBy { get; set; }
        public string Location { get; set; }
         public string UnitNum { get; set; }
        public string Manufacturer { get; set; }
        public string ManufacturerYear { get; set; }
        public string Model { get; set; }
        public string Capacity { get; set; }
        public string Color { get; set; }
       public string TyreSize { get; set; }
        public string Status { get; set; }
          public string PlateNumber { get; set; }
        public string EngineNumber { get; set; }
        public string ChasisNumber { get; set; }
        public string FuelStandard { get; set; }
         public decimal RentalRate { get; set; }
        public enum WayOfSupplyenum { Purchased, Donated, Transferred }
        public enum manufaturerenum { TOYOTA, NISSAN, SINO }
        public enum Colorenum { BLACK, WHITE, RED, BLUE, GREEN }
        public enum yearenum { }
         public string WayOfSupply { get; set; }
         public string SupplierCompany { get; set; }
        public decimal PreviousOdoMeter { get; set; }
        public decimal CurentOdoMeter { get; set; }
         public decimal? PurchasingCost { get; set; }
        public int? AnnualDepreciationCost { get; set; }
         public int? DepreciationCost { get; set; }
         public string SerialNo { get; set; }
        public string Libre { get; set; }
        public enum TypeOfFuelenum { Benzene, Diesel }
         public string TypeOfFuel { get; set; }
         public string StateOfOdoMeter { get; set; }
        public enum StateOfOdoMeterenum { Functional, Malfucntioned }
        public string Attachment { get; set; }
        public string FleetPhoto { get; set; }
        [DataType(DataType.Date)]
         [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> PurchasedDate { get; set; }
         [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> LastMaintained { get; set; }

    }
}
