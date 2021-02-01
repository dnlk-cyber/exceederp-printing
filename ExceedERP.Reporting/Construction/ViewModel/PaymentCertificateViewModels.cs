using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Construction.ViewModel
{
    //class PaymentCertificateViewModels
    //{
    //}
    public class CertificateViewModel
    {
        public CertificateViewModel()
        {
            VariationOrderList = new List<MainContractVariationOrderVM>();
            PreviosPaymentList = new List<PreviosPaymentsViewModel>();
        }
        // values from project information
        public string ProjectName { get; set; }
        public string Location { get; set; }
        public string Consultant { get; set; }
        public string Client { get; set; }
        // values from main contract
        public decimal MainContract { get; set; }
        public List<MainContractVariationOrderVM> VariationOrderList { get; set; }
        public decimal  Sum { get; set; }
        public decimal VatOfContractAmount { get; set; }
        public decimal TotalWithVatOfContractAmount { get; set; }

        // values of current payment certificate
        public int PaymentNumber { get; set; }
        public decimal  ExecutedAmount { get; set; }
        public decimal VatOfCurrentAmount { get; set; }
        public decimal TotalWithVatofCurrentAmount { get; set; }
        // deduction 
        public decimal PreviousPayment { get; set; }
        public decimal Retention { get; set; }
        public decimal Penality { get; set; }
        public decimal Correction { get; set; }
        public decimal AdvanceRepayment { get; set; }
        public decimal OtherDeductiveCosts { get; set; }
        public decimal MaterialOnSite { get; set; }

        public decimal SumOfDeductiveCosts { get; set; }
        public decimal NetAmount { get; set; }
        public List<PreviosPaymentsViewModel> PreviosPaymentList { get; set; }


    }
    public class MainContractVariationOrderVM
    {
        public string VariationOrderNo { get; set; }
        public decimal Amount { get; set; }

    }
    public class PreviosPaymentsViewModel
    {
        public string PaymentNumber { get; set; }
        public decimal Amount { get; set; }
        
    }
    // view model for payment numbers drop down
    public class PaymentNumberViewModel
    {
        public int ProjectPaymentCertificateId { get; set; }
        public string ConcatnatedNumber { get; set; }
    }
}
