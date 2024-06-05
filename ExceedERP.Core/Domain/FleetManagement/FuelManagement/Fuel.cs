
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
 using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.FleetManagement.FuelManagement
{
    public partial class Fuel : Operations
    {
        public Guid FuelID { get; set; }
        [DisplayName(" Equipment ")]
        public Guid FleetsID { get; set; }
    
        public string Project { get; set; }
        [DisplayName(" Operator Name")]
        public string OperatorName { get; set; }
        public enum FuelTypeenum { Benzene, Diesel }
        [DisplayName("Fuel Type")]
      
        public string FuelType { get; set; }
        [DisplayName("Fuel Standard")]
       
        public string FuelStandard { get; set; }

        [DisplayName(" Initial KmHr ")]
       
        public int InitialKmHr { get; set; }
        [DisplayName(" Final KmHr ")]
       
        public int FinalKmHr { get; set; }
        [DisplayName(" Plan KmHr ")]
       
        public int PlanKmHr { get; set; }
        [DisplayName(" Fuel Filled Amount  ")]
       
        public decimal FuelFilledAmount { get; set; }
        [DisplayName(" Fuel Source  ")]
       
        public string FuelSource { get; set; }
           [DisplayName("   Source Detail  ")]
        public string FuelSourceDetail { get; set; }
        public enum FuelSourceenum { Company, External }
        [DisplayName(" Unit Price ")]
       
        public decimal UnitPrice { get; set; }
        [DisplayName(" Receipt No ")]
       
        public string ReceiptNo { get; set; }
        [DisplayName("Total Fuel cost")]
        public decimal TotalFuelcost { get; set; }
    
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [NotMapped]
        [DisplayName("Equipment Name")]
        public string EquipmentName { get; set; }
        [NotMapped]
        [DisplayName("Asset No")]
        public string AssetNo { get; set; }
        public virtual Fleets Fleet { get; set; }


    }
}