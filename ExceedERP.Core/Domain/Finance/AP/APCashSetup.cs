using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.Finance.AP
{
    public class ApCashierSetup : TrackUserSettingOperation
    {
        public int ApCashierSetupId { get; set; }
        [Required]
        [Display(Name = "Cashier Name")]
        public string Cashier { get; set; }
       
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Display(Name = "Branch")]
        public int BranchId { get; set; }
        [Required]
        [Display(Name = "Account Code")]
        public string  AccountCode { get; set; }
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
        [Display(Name = "Active/Inactive Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = false)]
        public DateTime ActiveInactiveTime { get; set; }
    }
}
