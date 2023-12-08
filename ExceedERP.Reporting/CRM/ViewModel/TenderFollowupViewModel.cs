using System;
using System.Collections.Generic;

namespace ExceedERP.Reporting.CRM.ViewModel
{
    public class TenderFollowupCustomerViewModel
    {
        public string Customer { get; set; }
        public List<TenderFollowupViewModel> TendersList { get; set; } = new List<TenderFollowupViewModel>();

    }
    public class TenderFollowupViewModel
    {
        public decimal BidBondAmount { get; set; }
        public DateTime? DateAnnounceBidValidatiy { get; set; }
        public decimal? ServiceCharge { get; set; }

        public DateTime ClosingDate { get; set; }

        public DateTime OpeningDate { get; set; }
        public string Location { get; set; }

        public string CountDownTillSubmission { get; set; }

        public DateTime? DocumentPreparationStartDate { get; set; }

        public string Status { get; set; }

        public string TenderResult { get; set; }
        public string Remark { get; set; }
        public string LatestLetter { get; set; }
        public string ContactPerson { get; set; }

        public decimal BidAmount { get; set; }
        public decimal BidSecurityCharge { get; set; }
        public decimal BidValidty { get; set; }

        public string ReasonOfNotParticipating { get; set; }
        public string ReasonOfLosing { get; set; }
        public List<TenderFollowupPrductViewModel> ProductList { get; set; } = new List<TenderFollowupPrductViewModel>();
    }
    public class TenderFollowupPrductViewModel
    {
        public string Product { get; set; }
        public decimal Quantity { get; set; }
    }
}
