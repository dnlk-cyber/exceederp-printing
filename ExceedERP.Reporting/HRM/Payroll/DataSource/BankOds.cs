using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common.ViewModel;
using ExceedERP.Core.Domain.HRM.Payroll;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.Payroll.ViewModel;

namespace ExceedERP.Reporting.HRM.Payroll.DataSource
{
    [DataObject]
    public class BankOds : List<SelectorVm>
    {
        ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SelectorVm> GetBankList()
        {
            List<SelectorVm> list = new List<SelectorVm> { };

            var banks = db.Banks.OrderBy(x=>x.Name).ToList();

            foreach(var bank in banks)
            {
                var record = new SelectorVm();
                record.Code = bank.BankId.ToString();
                record.Name = bank.Name;
                list.Add(record);
            }

            return list;
        }
    }
}