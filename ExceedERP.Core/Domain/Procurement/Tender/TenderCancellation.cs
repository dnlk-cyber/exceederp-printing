using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Procurement.Tender
{
    public class TenderCancellation : TrackUserOperation
    {
        public int TenderCancellationId { get; set; }
        public int TenderID { get; set; }
        public virtual Tender Tender { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        [Required]
        public string Description { get; set; }
    }

    public class TenderCancellationUploadDbModel
    {
        public int TenderCancellationUploadDbModelId { get; set; }
        public int TenderCancellationId { get; set; }
        public virtual TenderCancellation Cancellation { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }

}
