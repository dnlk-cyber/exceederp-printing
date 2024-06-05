
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


    public partial class FleetPettyCash : Operations
    {
        public Guid FleetPettyCashId { get; set; }
        [DisplayName(" Equipment ")]
        public Guid FleetsID { get; set; }
         public decimal Amount { get; set; }

          [DataType(DataType.MultilineText)]
        public string Description { get; set; }
         
          public string Type { get; set; }
        [ForeignKey("FleetsID")]
        public virtual Fleets Fleet { get; set; }
        [NotMapped]
        public string Equipment { get; set; }

        public string JobOrderId { get; set; }
    }



}