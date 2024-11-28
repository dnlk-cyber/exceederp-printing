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
        public bool IsApproved { get; set; }
        public bool IsRejected { get; set; }
        public string CheckedBy { get; set; }
        public string ApprovedBy { get; set; }
        public string RejectedBy { get; set; }
        [DataType(DataType.Date)] 
        public DateTime? ApprovedDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? CheckedDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? RejectedDate { get; set; }
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
}
