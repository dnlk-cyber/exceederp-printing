using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Archive.ViewModel
{
    public class OutgoingngLetterVM
    {
        public string DocumentNumber { get; set; }
        public string OutgoingDate { get; set; }
        public string TypeOfLetter { get; set; }
        public string OutgoingLetterTo { get; set; }
        public string DeliveredDate { get; set; }
        public string ReasonNotSend { get; set; }

    }
}
