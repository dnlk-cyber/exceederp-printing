using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class StaffCountViewModel
    {
        public string ResponsibilityLevel { get; set; }

        // Permanent
        public int PermanentMale { get; set; }
        public int PermanentFemale { get; set; }
        public int PermanentTotal => PermanentMale + PermanentFemale;

        // Temporary
        public int TemporaryMale { get; set; }
        public int TemporaryFemale { get; set; }
        public int TemporaryTotal => TemporaryMale + TemporaryFemale;

        // Optional: Overall total
        public int OverallTotal => PermanentTotal + TemporaryTotal;
    }
}
