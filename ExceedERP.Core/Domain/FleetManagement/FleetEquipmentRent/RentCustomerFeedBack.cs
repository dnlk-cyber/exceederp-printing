
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
 using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent
{
    public class RentCustomerFeedBack : Operations
    {
          [ScaffoldColumn(false)]
        public Guid RentCustomerFeedBackID { get; set; }
          public Guid RequestID { get; set; }
        [Required]
        public string DetailFeedBack { get; set; }
        [DataType(DataType.MultilineText)]
        public Nullable<DateTime> FeedBackDate { get; set; }

        [ForeignKey("RequestID")]
        public virtual RentRequest RentRequest { get; set; }
    }
}