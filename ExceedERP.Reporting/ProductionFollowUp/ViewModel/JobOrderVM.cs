using ExceedERP.Reporting.PrintingEstimation.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.ProductionFollowUp.ViewModel
{
   public class JobOrderVM : ApprovalPropertyVM
    {
        public DateTime Time { get; set; }
        public string JobNo { get; set; }
        public string CustomerName { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string City { get; set; }
        public string SubCity { get; set; }
        public string Wereda { get; set; }
        public string Pobox { get; set; }
        public string TypeOfJob { get; set; }
        public string SubJobType { get; set; }
        public decimal Quantity { get; set; }
        public decimal Copies { get; set; }
        public decimal TotalImpression { get; set; }
        public string TextWeight { get; set; }
        public bool Contract { get; set; }
        public string ColorText { get; set; }
        public string FinishingSize { get; set; }
        public string PrintingLine { get; set; }
        public string PrintingMachine { get; set; } 
        public bool CameraReady { get; set; }
        public bool Sewing { get; set; }
        public bool New { get; set; }
        public bool PerfectBinding { get; set; }
        public bool Repeat { get; set; }
        public bool Lamnating { get; set; }
        public bool Varnish { get; set; }
        public bool Perforate { get; set; }
        public bool Camsur { get; set; }
        public DateTime? GraphicDesignExpectedCompletetion { get; set; }
        public DateTime? PrintingExpectedCompletetion { get; set; }
        public DateTime? FinishExpectedCompletetion { get; set; }
        public decimal NoOfPage { get; set; }
        public decimal PriceBeforeVat { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string CoverWeight { get; set; }   
        public string ColorCover { get; set; }  
        public List<JobOrderMaterialVM> PaperTypes { get; set; }
        public List<JobOrderMaterialVM> FilmOrPlates { get; set; }
    }
}
