using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Archive.ViewModel
{
    public class IncomingLetterVM
    {
        public string DocumentNumber { get; set; }
        public string IncomingDate { get; set; }
        public string TypeOfLetter { get; set; }
        public string ReceivedFrom { get; set; }
        public string ForwardTo { get; set; }

    }
}
