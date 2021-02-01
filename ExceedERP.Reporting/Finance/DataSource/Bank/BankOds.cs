using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Finance.Common;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.Finance.DataSource.Bank
{
    [DataObject]
    public class BankOds : List<BankAccount>
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<BankAccount> GetBankAccountList()
        {
            List<BankAccount> list = _db.BankAccounts.ToList();
            return list;
        }

       
    }
}
