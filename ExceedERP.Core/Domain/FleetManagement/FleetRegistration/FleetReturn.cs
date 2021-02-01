
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetRegistration
{
    public partial class FleetReturn : Operations
    {

        [DisplayName("Return")]
        public int FleetReturnId { get; set; }
        [DisplayName("Equipment")]
        public Guid FleetsId { get; set; }

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
}
