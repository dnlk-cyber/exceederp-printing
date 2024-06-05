
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
 using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.FleetManagement.FleetSalvageStore
{
    public partial class Cannibalization : OperationWithSecondValidation
    {
        public Guid CannibalizationID { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Num { get; set; }
        [DisplayName("Equipment Needed ")]
        [Required]
        public Guid PartNeededFleetsID { get; set; }
        [DisplayName("Equipment Taken")]
        [Required]
        public Guid PartTakenFleetsID { get; set; }
        [DisplayName(" Is Operational ")]
        public bool IsOperational { get; set; }
        [DisplayName(" Break Down ")]
        public bool IsBreakDown { get; set; }
        [DisplayName(" Scrap ")]
        public bool IsScrap { get; set; }
        [DisplayName(" Poor ")]
        public bool IsPoor { get; set; }

        [DisplayName(" Is Operational ")]
        public bool IsFromOperational { get; set; }
        [DisplayName(" Break Down ")]
        public bool IsFromBreakDown { get; set; }
        [DisplayName(" Scrap ")]
        public bool IsFromScrap { get; set; }
        [DisplayName(" Poor ")]
        public bool IsFromPoor { get; set; }
        [DisplayName(" Permanent")]
        public bool IsPermanent { get; set; }
        public string Status { get; set; }
        public string TakenBy { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public bool IsThirdOnlineApproved { get; set; }
        public string ThirdOnlineApprovedBy { get; set; }
        public DateTime? ThirdOnlineApprovedTime { get; set; }

        public bool IsFourthSendForApproval { get; set; }
        public string FourthSendForApprovalBy { get; set; }
        public DateTime? FourthSendForApprovalTime { get; set; }
    }
}