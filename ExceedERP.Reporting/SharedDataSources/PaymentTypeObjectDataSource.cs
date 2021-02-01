using System.Collections.Generic;
using System.ComponentModel;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
    public class PaymentTypeObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetPaymentType()
        {
            List<string> sourceList = new List<string>
            {
                "BANK","CASH"




            };

            return sourceList;
        }
    }
}