
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
 using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetMaintenance
{
    public partial class OtherCompanyMaintenance : Operations
    {
        [ScaffoldColumn(false)]
        public Guid OtherCompanyMaintenanceID { get; set; }
        [DisplayName("Equipment Name   ")]
        [Required]
        public string EquipmentName { get; set; }
        [Required]
        [DisplayName("Name of Customer  ")]
        public string NameofCustomer { get; set; }
        [DisplayName("Customer Phone ")]
        [Required]
        public string CustomerPhone { get; set; }
        [DisplayName("Customer Email ")]
        public string CustomerEmail { get; set; }
        [DisplayName("Operator Name  ")]
        public string OperatorName { get; set; }
        [DisplayName("Bank Slip No  ")]
        [Required]
        public string BankSlipNo { get; set; }

 
        [DisplayName("Kmhr Reading")]
        public string KmhrReading { get; set; }
        [DisplayName("Serial or Tag No.")]
        public string SerialNo { get; set; }
        [DisplayName("Plate No ")]
        [Required]
        public string PlateNo { get; set; }
        [DisplayName("Bank Branch Name ")]
        [Required]
        public string BankBranchName { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        public enum ApprovedStatus { Approved, Pending }
        public string ApprovedBy { get; set; }


    }
}