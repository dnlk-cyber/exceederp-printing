using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.FleetManagement
{
    public class RequestToInspection
    {
        public int RequestToInspectionId { get; set; }
        public string Remark { get; set; }
        public bool GoToInspection { get; set; }
    }
}
