using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.APTS
{
    public class APTSRequestStatus
    {

        [Key]
        public int APTSRequestStatusId { get; set; }
        public int RequestId { get; set; }
        public string ReferenceType { get; set; }
        [Required]
        public DrugRequestStatus Status { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Required]
       
        public DateTime Date { get; set; }
       
        public string DateReqStatAm { get; set; }
       
        public string ChangedBy { get; set; }
       
        public string Remark { get; set; }
    }
}
