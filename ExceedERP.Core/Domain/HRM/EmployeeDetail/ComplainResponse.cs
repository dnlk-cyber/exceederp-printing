using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
    public class ComplainResponse
    {
        public int ComplainResponseId { get;set; }
        public string Response { get; set; }
        public int ComplainId { get; set; }
        public string Remark { get; set; }
    }
}
