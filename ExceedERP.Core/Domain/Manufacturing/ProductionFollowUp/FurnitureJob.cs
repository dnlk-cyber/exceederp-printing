using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Manufacturing.ProductionFollowUp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ExceedERP.Core.Domain.Manufacturing.ProductionFollowUp
{
    public class FurnitureJob : TrackUserSettingOperation
    {
        public int FurnitureJobId { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Recived Date")]
        public DateTime Time { get; set; }
        public bool hv { get; set; }
        public bool IsExistingCustomer { get; set; }
        [Display(Name = "Proforma ")]
        public int? FurnitureProformaId { get; set; }
        public int FurnitureCustomerId { get; set; }
        //[RegularExpression(@"^(\d{5})$", ErrorMessage = "Enter a valid 5 digit Job No")]
        [Display(Name = "Job No.")]
        public string JobNo { get; set; }
        [Display(Name = "Order Type")]
        public FurnitureProductionJobOrderStatus? Status { get; set; }
        [Display(Name = "Customer Type")]
        public int ProductionCustomertypeId { get; set; }
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }
        [Display(Name = "Estimated By")]
        public string EstimatedBy { get; set; }
        public string Code { get; set; }
        [Display(Name = "Brand Name")]
        public string BrandName { get; set; }
        [Display(Name = "Tin No ")]
        public string TinNo { get; set; }
        [Display(Name = "Contact Phone")]
        public string ContactPhone { get; set; }

        [Display(Name = "Furniture Job Type")]
        public int FurnitureJobTypeId { get; set; }
        [Display(Name = "Job Type")]
        public string JT { get; set; }
        [Display(Name = "Pages")]
       
        public decimal Pages { get; set; }
        public decimal Copies { get; set; }
        public decimal Quantity { get; set; }
        [Display(Name = "Unit")]
        public string UnitId { get; set; }
        [Display(Name = "Unit Price")]
        public double UnitPrice { get; set; }
        [Display(Name = "Before Tax")]
        public double BeforeTax { get; set; }
      
        [Display(Name = "Total Price")]
        public decimal TotalPrice { get; set; }
        public bool Contract { get; set; }

        [Display(Name = "Total Impression")]
        public int TotalImpression { get; set; }
        [Display(Name = "Delivery Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DeliveryDate { get; set; }
        [Display(Name = "Received Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ReceivedDate { get; set; }

        [Display(Name = "Text Weight")]
        public string TextWeight { get; set; }
        [Display(Name = "Cover Weight")]
        public string CoverWeight { get; set; }
        [Display(Name = "Color Text")]
        public string ColorText { get; set; }
        [Display(Name = "Color Cover")]
        public string Colorcover { get; set; }
        [StringLength(15)]
        [Display(Name = "Finishing Size")]
        public string FinishingSize { get; set; }
        [Display(Name = "Line")]
        [StringLength(15)]
        public string PrintingLine { get; set; }
        [Display(Name = "Machine")]
        [StringLength(15)]
        public string PrintingMachine { get; set; }
        public bool Sewing { get; set; }
        public bool New { get; set; }
        public bool PerfectBinding { get; set; }
        public bool Repeat { get; set; }
        public bool Lamnating { get; set; }
        public bool Varnish { get; set; }
        public bool Perforate { get; set; }
        public bool Camsur { get; set; }
        [Display(Name = "Graphic Design Expected Completetion")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? GraphicDesignExpectedCompletetion { get; set; }
        [Display(Name = "Expected Completetion")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? PrintingExpectedCompletetion { get; set; }
        [Display(Name = "Finish Expected Completetion")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? FinishExpectedCompletetion { get; set; }
        [StringLength(50)]
        [Display(Name = "Finishing Workflow")]
        public string FinishingWorkflow { get; set; }
        [Display(Name = "Machine")]
        public int? MachineId { get; set; }      
        [Display(Name = "Approved By")]
        public string ApprovedBy { get; set; }
        public string CheckedBy { get; set; }
        public bool IsApproved { get; set; }
        public bool IsChecked { get; set; }
        public bool IsMaga { get; set; }
        public string Reason { get; set; }

    }
   public enum FurnitureProductionJobOrderStatus
    {
        Normal,
        ChangedOrder,
        ReOrdered,
        ChangeOfOrderVM
    }

}
