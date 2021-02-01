using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
    public class EmployeeCodeSequenceSetting
    {
        public int EmployeeCodeSequenceSettingId { get; set; }
        public string Name { get; set; }
        [DisplayName("Code Length")]
        public int ItemCodeTrim { get; set; }

        public int IndexCount { get; set; }
        public bool Status { get; set; }
    }
}
