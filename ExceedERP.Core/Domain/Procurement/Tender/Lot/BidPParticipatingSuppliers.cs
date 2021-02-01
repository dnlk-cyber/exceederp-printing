using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Procurement.Tender.Lot
{

    public enum PreQualification
    {
        Pass, Fail
    }
    public class BidPParticipatingSuppliers : TrackUserOperation
    {
        public int BidPParticipatingSuppliersID { get; set; }
        public int LotID { get; set; }
        public virtual Lot Lot { get; set; }
        [Display(Name = "Supplier ID")]//supplier database
        public string SupplierId { get; set; }
        [Display(Name = "New Supplier")]//manual supplier
        public bool IsNewSupplier { get; set; }
        [Display(Name = "Supplier Name")]//manual supplier
        public string SupplierName { get; set; }
        [Display(Name = "Representative")]
        public string Representative { get; set; }
        [Display(Name = "Address")]
        public string Address { get; set; }
        [Display(Name = "Telephone")]
        //[DataType(DataType.PhoneNumber)]
        //[RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{4})[-. ]?([0-9]{6})$", ErrorMessage = "Not a valid Phone number")]
        public string Telephone { get; set; }
        [Display(Name = "Submission Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime BidSubmissionDate { get; set; }
        [Display(Name = "Submission Time")]
        public string BidSubmissionTime { get; set; }
        [Display(Name = "Number of Document")]
        public int NumberOfDocument { get; set; }
        [Display(Name = "Pre Qualification")]
        public PreQualification IsQualified { get; set; }
        public string Remark { get; set; }
    }
}
