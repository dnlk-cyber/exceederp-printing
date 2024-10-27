using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Finance.ViewModel;
using ExceedERP.Reporting.Finance.ViewModel.Print;

namespace ExceedERP.Reporting.Finance.DataSource.AP
{
    [DataObject]
    public class DirectPaymentUsersOds : List<string>
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

       
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetDirectPaymentUsers()
        {
            List<string> list = _db.ApDirectPayments.Select(x=>x.CreatedBy).Distinct().OrderBy(x=>x).ToList();
           
            return list;
        }

       
    }
}
