using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.HRM.ArchiveManagement
{
    public class RecivedLetters
    {
        [Key]
        public int RecivedLetterId { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime RecivedDate { get; set; }
        [Display (Name ="Institution or Individual Name")]

        public string InstnPerson { get; set; }
      
        public int Receiver { get; set; }
        public string Sender { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
        public string FileName { get; set; }
        public LettersStatus LetterStatus { get; set; }
        ////[NotMapped ]
        public int Status { get; set; }
        public string BookNo { get; set; }
        public virtual IncomingLetterRegistration IncommingLetterRegistration { get; set; }
    }
}