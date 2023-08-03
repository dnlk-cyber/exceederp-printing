using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Procurement.Tender;

namespace ExceedERP.Core.Domain.Procurement.Proforma
{
    public enum PassFailStatus
    {
        NA,
        Pass,
        Fail
    }

    public class ProformaEvaluationResult : TrackUserOperation
    {
        public int ProformaEvaluationResultID { get; set; }
        public int ProformaPurchaseID { get; set; }
        public virtual ProformaPurchase ProformaPurchase { get; set; }
        public float Financial { get; set; }
        public float Technical { get; set; }
        public float Total { get; set; }
        public PassFailStatus Pass { get; set; }

        [Display(Name = "Supplier ID")] //supplier database
        public string SupplierId { get; set; }

        [Display(Name = "New Supplier")] //manual supplier
        public bool IsNewSupplier { get; set; }

        [Display(Name = "Supplier Name")] //manual supplier
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
        public int ProformaItemID { get; set; }//proforma item
        public virtual ICollection<ProformaEvaluationItemResult> ProformaEvaluationItemResults { get; set; }
    }

    public class ProformaEvaluationItemResult : TrackUserOperation
    {
        public int ProformaEvaluationItemResultId { get; set; }
        public int ProformaEvaluationResultID { get; set; }
        public virtual ProformaEvaluationResult ProformaEvaluationResult { get; set; }
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
        public string ItemSpecification { get; set; }
        public string ItemName { get; set; }
        public string Name { get; set; }//fixed asset
        public string ModelNo { get; set; }
        public int ProformaItemID { get; set; }//proforma item
    }
}
