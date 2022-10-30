using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.CRM.Sales
{
    public class DuplicateBincardViewModel
    {
        public string ReferenceDoc { get; set; }
        public int StoreItemAssignmentID { get; set; }
        public int TotalOccurance { get; set; }
    }
}
