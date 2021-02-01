using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.CRM
{

    public class CommunicationSource
    {
        public int CommunicationSourceID { get; set; }

        public string Reference { get; set; }

        public string CommunicationType { get; set; }

        public string CommunicationCode { get; set; }

        public string Remark { get; set; }

    }

}
