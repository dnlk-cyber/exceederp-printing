using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.CRM.Tender.ViewModel
{
   public class SaleTenderVM
    {
       
        public string PlaceForTheProduct { get; set; }
        public string BidsParticipatePlace { get; set; }
        public string SpecificNameOFtheProductPlace { get; set; }
        public string TenderRference { get; set; }
        public string EvaluationDate { get; set; }
        public string approvedBy { get; set; }
        public string Preparedby { get; set; }
        public string PreparedDate { get; set; }
        public string ApprovedDate { get; set; }
        public List<winnerVM> winners { get; set; }
     
        
    }
    public class winnerVM
    {
        public string CustomerName { get; set; }
        public string TinNumber { get; set; }
        public string ProductName { get; set; }
        public string ProductArea { get; set; }
        public int PorductAreaNumber { get; set; }
        public string UOM { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Vat { get; set; }
        public decimal TotalIncludingVat { get; set; }
    }
    
}


