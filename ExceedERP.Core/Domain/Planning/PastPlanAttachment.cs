using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Finance.GL;

namespace ExceedERP.Core.Domain.Planning
{
    public class PastPlanAttachment
    {
        public int PastPlanAttachmentId { get; set; }

        [Required]
        [Display(Name = "Fiscal Year")]
        public int GlFiscalYearId { get; set; }
        public virtual GlFiscalYear GlFiscalYear { get; set; }

        [Required]
        [Display(Name = "Plan Date")]
        [DataType(DataType.Date)]
        public DateTime PlanDate { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [StringLength(2000)]
        public string Description { get; set; }

        [Required]
        [StringLength(260)]
        public string FileName { get; set; }

        [Required]
        [StringLength(100)]
        public string StoredFileName { get; set; }

        [StringLength(200)]
        public string ContentType { get; set; }

        public int FileSize { get; set; }
        public DateTime UploadedAt { get; set; }
        public string UploadedBy { get; set; }
    }
}
