using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation
{
    public class FurnitureBoqRequest 
    {
        [Key]
        public int FurnitureBoqRequestId { get; set; }
        public int CustomerId { get; set; }
        public string JobType { get; set; }
        [DataType(DataType.Date)]
        public DateTime RequestDate { get; set; }
        public string Remark { get; set; }
        public bool IsChecked { get; set; }
        public bool IsSent { get; set; } //
        public bool IsApproved { get; set; }
        public bool IsRejected { get; set; }
        public bool IsEstApproved { get; set; }
        public bool IsEstRejected { get; set; }
        public bool IsEstSent { get; set; } 
        public string CheckedBy { get; set; }
        public string ApprovedBy { get; set; }
        public string RejectedBy { get; set; }
        public string SentBy { get; set; }
        [DataType(DataType.Date)] 
        public DateTime? ApprovedDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? CheckedDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? RejectedDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? SentDate { get; set; }
        public string EstApprovedBy { get; set; }
        [DataType(DataType.Date)]
        public DateTime? EstApprovedDate { get; set; }
        public string EstRejectedBy { get; set; }
        [DataType(DataType.Date)]
        public DateTime? EstRejectedDate { get; set; }
        public string EstSentBy { get; set; }
        [DataType(DataType.Date)]
        public DateTime? EstSentDate { get; set; }
        public string Status { get; set; }

    }

    public class FurnitureBoqDocument
    {
        [Key]
        public int FurnitureBoqDocumentId { get; set; }
        public int FurnitureBoqRequestId { get; set; }

        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string ShortPath { get; set; }
        public string FileExtension { get; set; }
        public string Description { get; set; }
    }

    public class FurnitureBoqDocumentComment
    {
        [Key]
        public int FurnitureBoqDocumentCommentId { get; set; }
        public string Comment { get; set; }
        public DateTime DateCreated { get; set; } 
        public string CreatedBy { get; set; }
        public int FurnitureBoqRequestId { get; set; }

    }
}
