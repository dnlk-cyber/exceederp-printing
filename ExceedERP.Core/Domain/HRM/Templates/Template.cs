using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.Templates
{
    public class Template
    {
        public int TemplateId { get; set; }
        public string TempOne { get; set; }
        public TemplateTypes TemplateType { get; set; }
    }
}
