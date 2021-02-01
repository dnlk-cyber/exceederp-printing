using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Finance.ViewModel.Print;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.Finance.SubLedger.DataSource
{
    [DataObject]
    public class ParameterOds
    {
       
        
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> TransferStatus()
        {
            List<string> list = new List<string> {"Transferred to GL", "Not Transferred to GL"};
            return list;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> InventoryCategory()
        {
            List<string> list = new List<string> { "GRN","Issue","Transfer","Store Return" };
            return list;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> ArCategory()
        {
            List<string> list = new List<string> { "Invoice", "Collection", "Miscellaneous Collection"};
            return list;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> ApCategory()
        {
            List<string> list = new List<string> { "Invoice", "Payment", "Direct Payment", "Miscellaneous Payment","Petty Cash", "Travel Order", "Expense Claim" };
            return list;
        }
    }
}
