
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetRegistration
{


    public partial class OwningandOperatingCost : Operations
    {
        public int OwningandOperatingCostId { get; set; }
        [DisplayName(" Equipment ")]
        public Guid FleetsID { get; set; }
          [DisplayName(" Equipment ")]
         public string Model { get; set; }

         public string Manufacture { get; set; }
          [DisplayName(" OwnerShip Period ")]
           public decimal OwnerShipPeriod { get; set; }
          [DisplayName(" Estimated Usage ")]
         public decimal EstimatedUsage { get; set; }
          [DisplayName(" OwnerShip Usage ")]
         public decimal OwnerShipUsage { get; set; }
          [DisplayName(" Delivery Price ")]
         public decimal DeliveryPrice { get; set; }

         public decimal Interest { get; set; }
         public decimal Value { get; set; }
          [DisplayName(" Total Operating Cost ")]
         public decimal TotalOperatingCost { get; set; }
          [DisplayName(" Tyre Replacement Cost ")]
         public decimal TyreReplacementCost  { get; set; }
          [DisplayName(" Total Operating Weight ")]
         public decimal TotalOperatingWeight { get; set; }
          [DisplayName(" Resdual Scrap Value ")]
         public decimal ResdualScrapValue  { get; set; }
           [DisplayName("Yerly Premiuem ")]
         public decimal YerlyPremiuem { get; set; }

         public decimal NetValue { get; set; }
          [DisplayName(" Fuel Consumption ")]
         public decimal FuelConsumption { get; set; }
          [DisplayName(" Operator Driver Monthly Salary ")]
         public decimal OperatorDriverMonthlySalary { get; set; }
         public decimal Depreciation { get; set; }

         public decimal Insurance { get; set; }
         public decimal PropertyTax { get; set; }
         public decimal NetTotal { get; set; }
         public decimal Fuel { get; set; }
             
         public decimal LubOil { get; set; }
         public decimal ServiceParts { get; set; }
         public decimal Tire { get; set; }
          [DisplayName(" Maintenance Repair Cost ")]
         public decimal MaintenanceRepairCost { get; set; }
                    [DisplayName(" Special Wear Items Cost ")]
         public decimal    SpecialWearItemsCost { get; set; }
         public decimal UnderCarrageCost  { get; set; }
         public decimal OperatorCost { get; set; }
         public decimal TotalNetCost { get; set; }

         public decimal Profit { get; set; }
         public decimal OverHead { get; set; }
          [DisplayName(" Rental Rate With Fuel ")]
         public decimal RentalRateWithFuel { get; set; }
          [DisplayName(" Rental Rate With Out Fuel ")]
         public decimal RentalRateWithOutFuel { get; set; }
     

        [ForeignKey("FleetsID")]
        public virtual Fleets Fleet { get; set; }
         
      
         
    }



}