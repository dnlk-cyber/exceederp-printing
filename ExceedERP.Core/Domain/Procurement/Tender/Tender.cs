using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Procurement.Settings;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.Procurement.Tender
{

    public class Tender : Operations
    {
        public int TenderID { get; set; }
        [Required]
        [Display(Name = "Tender Reference")]
        public string TenderReference { get; set; }
        [Required]
        public string Description { get; set; }
        [Display(Name = "Float Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd :hh mm}", ApplyFormatInEditMode = true)]
        public DateTime FloatDate { get; set; }
        [Display(Name = "Closing Date ")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd :hh mm}", ApplyFormatInEditMode = true)]
        public DateTime ClosingDate { get; set; }
        [Display(Name = "Technical Opening Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd :hh mm}", ApplyFormatInEditMode = true)]
        public DateTime OpenigDate { get; set; }
        [Display(Name = "Technical Opening Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd :hh mm}", ApplyFormatInEditMode = true)]
        public DateTime? TechnicalOpenigDate { get; set; }
        [Display(Name = "Financial Opening Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd :hh mm}", ApplyFormatInEditMode = true)]
        public DateTime? FinancialOpenigDate { get; set; }
        [Required]
        [Display(Name = "Tender Type")]
        public TenderType TenderType { get; set; }
        [Required]
        [Display(Name = "Category")]
        public ProcurementCategory ProcurementCategory { get; set; }
        [Required]
        [Display(Name = "Purchase Type")]
        public PurchaseCategory PurchaseCategory { get; set; }
        [Display(Name = "Procurement Officer")]
        public int ProcurementOfficer { get; set; }

        [DisplayName("Is Fullfilled")]
        public bool IsFullfilled { get; set; }
        [DisplayName("Is Cancelled")]
        public bool IsCancelled { get; set; }
        [DisplayName("Is Technical Opened")]
        public bool IsTechnicalOpened { get; set; }
        [DisplayName("Is Financial Opened")]
        public bool IsFinancialOpened { get; set; }
        [DisplayName("Purchase Status")]
        public PurchaseStatus PurchaseStatus { get; set; }
        public int BranchId { get; set; }
        public int BusinessUnitId { get; set; }
        [NotMapped]
        public bool CloseStatus { get; set; }
        [NotMapped]
        public bool OpenStatus { get; set; }
        public virtual ICollection<Lots.Lot> Lots { get; set; }
        public virtual ICollection<TenderValidation> TenderValidation { get; set; }
        public virtual ICollection<TenderMinuteUploadDBModel> TenderMinuteUploadDBModel { get; set; }
        public virtual ICollection<TenderCancellation> TenderCancellations { get; set; }
        public virtual ICollection<TenderExtension> TenderExtensions { get; set; }
    }
}
