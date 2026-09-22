
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
 using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetTireManagement
{
    public partial class TireIssue : Operations
    {
        public Guid TireIssueID { get; set; }
        public Guid TireReturnId { get; set; }
        public Guid EquipmentTireId { get; set; }
        public Guid? EquipmentIdentityID { get; set; }
        [DisplayName("  Equipment Type ")]
        public Guid? EquipmentTypeID { get; set; }
        public Guid? EquipmentNameID { get; set; }
        [DisplayName("Fleet")]
        public Guid? FleetId { get; set; }

        [DisplayName(" Equipment")]
        public string PlateNumber { get; set; }
       
        public string StoreCode { get; set; }
           [DisplayName(" Operator Name")]
        public string OperatorName { get; set; }
        [DisplayName("Tyre Size")]
        [Required]
        public string TireSize { get; set; }
        public FleetBatteryOrTyreType IssueType { get; set; }
        [DisplayName("Type")]
        public string TireType { get; set; }
        public bool IsCreatedOnFleet { get; set; }
        public enum TireTypeenum { Wire, Fiber }
        [DisplayName("Installed Date")]
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? InstalledDate { get; set; } [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? IssueDate { get; set; }
        public string IssueReference { get; set; }

        [Required]
        public string Brand { get; set; }
        [Required]
        [DisplayName("Serial Number ")]
        public string SerialNo { get; set; }
        public string SerialNoB { get; set; }
        public string SerialNoC { get; set; }
        public string SerialNoD { get; set; }
        public enum TireTubeenum { With, WithOut }
        [Required]
        [DisplayName("Tyre Tube  ")]
        public string TireTube { get; set; }
        [Required]
        [DisplayName("KM Reading  ")]
        public decimal KMReading { get; set; }
        [Required]
        [DisplayName("Unit Price ")]
        public decimal UnitPrice { get; set; }
        
         [DataType(DataType.MultilineText)]
        [Required]
        [DisplayName("Description Reason")]
        public string DescriptionReason { get; set; }
        public int IssueId { get; set; } 
        public int IssueItemId { get; set; } 

        

    }
    public class TireIssueDetail
    {
        public int TireIssueDetailId { get; set; }
        public string TireIssueId { get; set; }
        [DisplayName("Serial Number ")]
        public string SerialNo { get; set; }
        public string TireSize { get; set; }
        public FleetBatteryOrTyreType IssueType { get; set; }
        [DisplayName("Type")]
        public string TireType { get; set; }
        [Required]
        [DisplayName("Tyre Tube  ")]
        public string TireTube { get; set; }
        [Required]
        [DisplayName("Unit Price ")]
        public decimal UnitPrice { get; set; }

        [Required]
        public string Brand { get; set; }
    }
    public enum FleetBatteryOrTyreType
    {
        Tyre,
        Battery
    }
}