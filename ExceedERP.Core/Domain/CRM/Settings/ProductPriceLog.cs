using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Inventory;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.CRM
{
    public class ProductPriceLog : TrackUserOperation
    {
        [Key]
        public int ProductPriceLogId { get; set; }

        [NotMapped]
        [Display(Name ="Category")]
        public string ItemCategoryCode { get; set; }

        [NotMapped]
        [Display(Name = "Sub Category")]
        public string SubCategoryCode { get; set; }

        [NotMapped]
        [Display(Name = "Store")]
        public string StoreCode { get; set; }

        [Display(Name ="Item")]
        public int StoreItemAssignmentID { get; set; }

        public virtual StoreItemAssignment StoreItemAssignment { get; set; }

        [NotMapped]
        public string ItemCode { get; set; }

        [NotMapped]
        public string ItemName { get; set; }

        [NotMapped]
        public string UoM { get; set; }


        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime InitialDate { get; set; }

        [Display(Name = "Initial Ex. Rate")]
        public decimal InitialExchangeRate { get; set; }

        [Display(Name = "Initial Price")]
        public decimal InitialPrice { get; set; }


        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime NewDate { get; set; }

        [Display(Name ="New Exchange Rate")]
        public decimal NewExchangeRate { get; set; }

        public string PriceType { get; set; }

        [Display(Name ="New Proposed Price")]
        public decimal ProposedNewPrice { get; set; }

        [Display(Name ="New Actual Price")]
        public decimal ApprovedNewPrice { get; set; }

        [NotMapped]
        public decimal Difference { get; set; }
    }
}
