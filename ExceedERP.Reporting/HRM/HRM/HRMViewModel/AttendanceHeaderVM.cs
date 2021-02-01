using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class AttendanceHeaderVM : AddressVm
    {
            public int OrganizationID { get; set; }
            public string Name { get; set; }
            public string AmharicName { get; set; }
            public string Website { get; set; }
            public string Address { get; set; }
            public Image LogoFileName { get; set; }
            public string Abbreviation { get; set; }
            public string Date { get; set; }
            public string Note { get; set; }
            public string From { get; set; }
            public string To { get; set; }

    }
}
