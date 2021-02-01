using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Budget.ViewModel
{
        public  class BudgetControlViewModel : Organization
    {
            public string BudgetCode { get; set; }
            public string BudgetDate { get; set; }
            public string ActivtyStatus { get; set; }
            public string TransactionOnType { get; set; }
            public string TransactionOnNumber { get; set; }
            public string TransactionOnAction { get; set; }
            public string TransactionType { get; set; }
            public string ReservationAmount { get; set; }
            public string LiquidationAmount { get; set; }
            public string ActiveBalance { get; set; }
    }
}
