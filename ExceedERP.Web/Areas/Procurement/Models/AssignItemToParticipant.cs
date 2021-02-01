using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExceedERP.Web.Areas.Procurement.Models
{
    public class AssignItemToParticipant
    {
        public string ItemCode { get; set;  }
        public string ItemName { get; set;  }
        public float Technical { get; set;  }
        public bool Assigned { get; set;  }
    }
}