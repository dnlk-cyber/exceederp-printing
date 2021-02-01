using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common.ViewModel;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.Guarantee;
using ExceedERP.Core.Domain.Procurement.Tender;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.Procurements.DataSource
{
    [DataObject]
    public class GuaranteeTypeOds
    {
       

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetGuaranteeTypes()
        {
            List<string> list = new List<string>
            {
                "AdvancePayment", "BidSecurity","Performance"
            };
            
            return list; 
        }
        
    }
}
