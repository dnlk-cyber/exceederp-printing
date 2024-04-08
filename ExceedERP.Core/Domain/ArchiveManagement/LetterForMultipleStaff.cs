using System;
using System.ComponentModel;

namespace ExceedERP.Core.Domain.ArchiveManagement
{
    public class LetterForMultipleStaff
    {
        public int LetterForMultipleStaffId { get; set; }
        public DateTime SendingDate { get; set; }
        [DisplayName("Sending Date")]
        public string SendingDateAm { get; set; }
        public string Title { get; set; }
        public bool IsTransfered { get; set; }
        public bool IsForAllEmployee { get; set; }
    }
}
