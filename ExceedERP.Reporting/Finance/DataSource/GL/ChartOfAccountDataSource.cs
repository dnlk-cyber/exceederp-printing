using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;
using ExceedERP.Reporting.Finance.ViewModel;

namespace ExceedERP.Reporting.Finance.DataSource.GL
{

    [DataObject]
    public class ChartOfAccountDataSource : List<ChartOfAccountViewModel>, IDisposable
    {
        private readonly ExceedDbContext _db;
        private readonly AccountHelper _accountHelper;
        public ChartOfAccountDataSource()
        {
            _db=new ExceedDbContext();
            _accountHelper=new AccountHelper();
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ChartOfAccountViewModel> GetChartOfAccount(string segment, string accountType)
        {
            _db.Database.CommandTimeout = 7200;
            var list = new List<ChartOfAccountViewModel>();
            var setup = _db.GlSegmentSetups.FirstOrDefault(x => x.Name == segment);

            if (setup == null) return list;

            if (setup.SegmentType == DynamicSegmentTypes.NaturalAccount)
            {
                var allAccounts = _db.GLChartOfAccountAccounts
                    .Select(x => new { x.AccountType, x.Values, x.Description, x.AllowBudgeting, x.HasParent, x.AllowPosting })
                    .ToList();

                var specificAccounts = (accountType == "All" || string.IsNullOrEmpty(accountType))
                    ? allAccounts
                    : allAccounts.Where(x => FilterAccountByType(x.AccountType, accountType)).ToList();

                foreach (var aaccount in specificAccounts)
                {
                    list.Add(new ChartOfAccountViewModel
                    {
                        AccountCode = aaccount.Values,
                        AccountDescription = aaccount.Description,
                        Parent = aaccount.HasParent,
                        AllowingBudgeting = aaccount.AllowBudgeting ? "Yes" : "No",
                        AllowingPosting = aaccount.AllowPosting ? "Yes" : "No",
                        AccountType = aaccount.AccountType.ToString()
                    });
                }
            }
            else
            {
                // For non-NaturalAccount, process GlChartOfAccountSegments
                var allAccounts = _db.GlChartOfAccountSegments
                    .Where(x => x.GLSegmentSetupId == setup.GLSegmentSetupId)
                    .Select(x => new { x.Values, x.Description, x.AllowBudgeting, x.HasParent, x.AllowPosting })
                    .ToList();

                foreach (var aaccount in allAccounts)
                {
                    list.Add(new ChartOfAccountViewModel
                    {
                        AccountCode = aaccount.Values,
                        AccountDescription = aaccount.Description,
                        Parent = aaccount.HasParent,
                        AllowingBudgeting = aaccount.AllowBudgeting ? "Yes" : "No",
                        AllowingPosting = aaccount.AllowPosting ? "Yes" : "No",
                        AccountType = "" // No AccountType for this case
                    });
                }
            }

            return list;
        }

        private bool FilterAccountByType(GlAccountTypes accountType, string type)
        {
            if (type == "Asset") return accountType == GlAccountTypes.Asset;
            if (type == "Liability") return accountType == GlAccountTypes.Liability;
            if (type == "Capital") return accountType == GlAccountTypes.Capital;
            if (type == "Revenue") return accountType == GlAccountTypes.Revenue;
            if (type == "Expense") return accountType == GlAccountTypes.Expense;

            return false;
        }
      
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> SegmentTypes()
        {
            return new List<string> { "Account", "Branch", "Cost Center", "Sub Account" };
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> AccountTypes()
        {
            return new List<string> { "All", "Asset", "Liability", "Capital", "Revenue", "Expense" };
        }

        public void Dispose()
        {
            _db?.Dispose(); 
        }
    }
}
