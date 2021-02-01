using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AR;
using ExceedERP.Core.Domain.Finance.AR.SalesOrder;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Finance.ViewModel.Print
{
    public class ApPettyCashVm : Organization

    {
        public string VoucherId { get; set; }
        public string CashierName { get; set; }
        public string PurposeOfPayment { get; set; }
        public string Reference { get; set; }
        public string Employee { get; set; }
        public string PreparedBy { get; set; }
        public string ApprovedBy { get; set; }
    }


    
}
