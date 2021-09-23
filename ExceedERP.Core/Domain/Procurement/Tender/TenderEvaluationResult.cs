using System.Collections.Generic;
using System.ComponentModel;
using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Procurement.Proforma;

namespace ExceedERP.Core.Domain.Procurement.Tender
{
    public class TenderEvaluationResult : TrackUserOperation
    {
        public int TenderEvaluationResultID { get; set; }
        public int LotID { get; set; }
        public virtual Lot.Lot Lot { get; set; }
        public float Financial { get; set; }
        public float Technical { get; set; }
        public PassFailStatus Pass { get; set; }
        public float Total { get; set; }

        [Display(Name = "Supplier ID")] //supplier database
        public string SupplierId { get; set; }

        [Display(Name = "New Supplier")] //manual supplier
        public bool IsNewSupplier { get; set; }

        [Display(Name = "Supplier Name")] //manual supplier
        public string SupplierName { get; set; }
        public virtual ICollection<TenderEvaluationItemResult> TechEvaluationItemResults { get; set; }
    }

    public class TenderEvaluationItemResult : TrackUserOperation
    {
        public int TenderEvaluationItemResultId { get; set; }
        public int TenderEvaluationResultID { get; set; }
        public virtual TenderEvaluationResult TenderEvaluationResult { get; set; }
        public float Financial { get; set; }
        public float Technical { get; set; }
        public PassFailStatus Pass { get; set; }
        public float Total { get; set; }

        [DisplayName("Category Code")]
        public string CatCode { get; set; }
        [DisplayName("Category")]
        public string CategoryName { get; set; }
        [DisplayName("Item Code")]
        public string ItmCode { get; set; }
        [DisplayName("Item")]
        public string ItemName { get; set; }
        public string Name { get; set; }//fixed asset
        [Display(Name = "Model No.")]
        public string ModelNo { get; set; }
        public int LotItemID { get; set; }//link to lot item
        public string ItemSpecification { get; set; }
    }

    public class PassFailResult
    {
        public int PassFailResultId { get; set; }
        [Display(Name = "Supplier ID")]//supplier database
        public string SupplierId { get; set; }
        [Display(Name = "New Supplier")]//manual supplier
        public bool IsNewSupplier { get; set; }
        [Display(Name = "Supplier Name")]//manual supplier
        public string SupplierName { get; set; }
        [DisplayName("Category Code")] public string CatCode { get; set; }
        [DisplayName("Category")] public string CategoryName { get; set; }
        [DisplayName("Item Code")] public string ItmCode { get; set; }
        [DisplayName("Item")] public string ItemName { get; set; }
        public string Name { get; set; }//fixed asset
        [Display(Name = "Model No.")]
        public string ModelNo { get; set; }
        public PassFailStatus Result { get; set; }
        public int LotID { get; set; }
        public virtual Lot.Lot Lot { get; set; }
        public string Remark { get; set; }
        public int LotItemID { get; set; }//link to lot item
        public string ItemSpecification { get; set; }
    }

    public class TenderTechnical
    {
        public int TenderTechnicalId { get; set; }
        [Display(Name = "Supplier ID")]//supplier database
        public string SupplierId { get; set; }
        [Display(Name = "New Supplier")]//manual supplier
        public bool IsNewSupplier { get; set; }
        [Display(Name = "Supplier Name")]//manual supplier
        public string SupplierName { get; set; }
        [DisplayName("Category Code")]
        public string CatCode { get; set; }
        [DisplayName("Category")]
        public string CategoryName { get; set; }
        [DisplayName("Item Code")]
        public string ItmCode { get; set; }
        [DisplayName("Item")]
        public string ItemName { get; set; }
        public string Name { get; set; }//fixed asset
        [Display(Name = "Model No.")]
        public string ModelNo { get; set; }
        public float Result { get; set; }
        public int LotID { get; set; }
        public Lot.Lot Lot { get; set; }
        public int LotItemID { get; set; }//link to lot item
        public string ItemSpecification { get; set; }
    }
}
