using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Furniture.ViewModel
{
    public class FurnitureCommonViewModel
    {
        public string CreatedBy { get; set; }
        public DateTime? DateCreated { get; set; }
        public string CheckedBy { get; set; }
        public DateTime? CheckedDate { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string Status { get; set; }
    }
}
