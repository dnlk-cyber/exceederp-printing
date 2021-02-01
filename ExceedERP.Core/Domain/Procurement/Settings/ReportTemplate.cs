using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Procurement.Settings;

namespace ExceedERP.Core.Domain.Procurement.Settings
{
    public class ReportTemplate
    {
        public int ReportTemplateId { get; set; }
       // [DisplayName("Subject")]
        public string Temp1 { get; set; }
        //[DisplayName("Title")]

        public string Temp2 { get; set; }
       // [DisplayName("Detail Title")]
        public string Temp3 { get; set; }
       // [DisplayName("Detail Summary")]

        public string Temp4 { get; set; }
       // [DisplayName("Summary One")]

        public string Temp5 { get; set; }
       // [DisplayName("Summary Two")]
        public string Temp6 { get; set; }
       // [DisplayName("Summary Three")]
        public string Temp7 { get; set; }
        public TemplateType Type { get; set; }
        public bool Status { get; set; }

    }
}
