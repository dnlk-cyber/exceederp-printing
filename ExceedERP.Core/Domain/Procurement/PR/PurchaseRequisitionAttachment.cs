namespace ExceedERP.Core.Domain.Procurement.PR
{
    public class PurchaseRequisitionAttachment
    {
        public int PurchaseRequisitionAttachmentId { get; set; }
        public int PurchaseRequisitionID { get; set; }
        public virtual PurchaseRequisition PurchaseRequisition { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}