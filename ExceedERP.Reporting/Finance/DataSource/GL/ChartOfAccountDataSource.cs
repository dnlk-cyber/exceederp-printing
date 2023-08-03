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
    public class ChartOfAccountDataSource : List<ChartOfAccountViewModel>
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        readonly AccountHelper _accountHelper = new AccountHelper();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ChartOfAccountViewModel> GetChartOfAccount(string segment, string accountType)
        {
            _db.Database.CommandTimeout = 7200;//added
            List<ChartOfAccountViewModel> list = new List<ChartOfAccountViewModel> { };
            if (_accountHelper.IsDynamicCoA())
            {
                var setup = _db.GlSegmentSetups.FirstOrDefault(x => x.Name == segment);
                if (setup != null)
                {
                    if (setup.SegmentType == DynamicSegmentTypes.NaturalAccount)
                    {
                        ChartOfAccountViewModel record;
                        var allaccounts = _db.GLChartOfAccountAccounts
                            .Select(x=>new {x.AccountType,x.Values,x.Description,x.AllowBudgeting,x.HasParent,x.AllowPosting})
                            .ToList();
                        #region account

                        if (accountType == "All")
                        {
                            var aaccounts = allaccounts;
                            foreach (var aaccount in aaccounts)
                            {
                                record = new ChartOfAccountViewModel
                                {
                                    AccountCode = aaccount.Values,
                                    AccountDescription = aaccount.Description,
                                    Parent = aaccount.HasParent,
                                    AllowingBudgeting = aaccount.AllowBudgeting ? "Yes" : "No",
                                    AllowingPosting = aaccount.AllowPosting ? "Yes" : "No",
                                    AccountType = aaccount.AccountType.ToString()
                                };
                                list.Add(record);
                            }
                        }
                        else
                        {
                            var specificAccounts = allaccounts;

                            #region get specificAccounts

                            if (accountType == "Asset")
                            {
                                specificAccounts = allaccounts.Where(x => x.AccountType == GlAccountTypes.Asset)
                                    .ToList();
                            }
                            else if (accountType == "Liability")
                            {
                                specificAccounts = allaccounts.Where(x => x.AccountType == GlAccountTypes.Liability)
                                    .ToList();
                            }
                            else if (accountType == "Capital")
                            {
                                specificAccounts = allaccounts.Where(x => x.AccountType == GlAccountTypes.Capital)
                                    .ToList();
                            }
                            else if (accountType == "Revenue")
                            {
                                specificAccounts = allaccounts.Where(x => x.AccountType == GlAccountTypes.Revenue)
                                    .ToList();
                            }
                            else if (accountType == "Expense")
                            {
                                specificAccounts = allaccounts.Where(x => x.AccountType == GlAccountTypes.Expense)
                                    .ToList();
                            }
                            else
                            {

                            }

                            #endregion

                            foreach (var aaccount in specificAccounts)
                            {
                                record = new ChartOfAccountViewModel
                                {
                                    AccountCode = aaccount.Values,
                                    AccountDescription = aaccount.Description,
                                    Parent = aaccount.HasParent,
                                    AllowingBudgeting = aaccount.AllowBudgeting ? "Yes" : "No",
                                    AllowingPosting = aaccount.AllowPosting ? "Yes" : "No",
                                    AccountType = aaccount.AccountType.ToString()
                                };
                                list.Add(record);
                            }

                        }

                        #endregion
                    }
                    else
                    {
                        var allaccounts = _db.GlChartOfAccountSegments.Where(x=>x.GLSegmentSetupId==setup.GLSegmentSetupId)
                            .Select(x => new { x.Values, x.Description, x.AllowBudgeting, x.HasParent, x.AllowPosting })
                            .ToList();
                        #region account

                        var aaccounts = allaccounts;
                        foreach (var aaccount in aaccounts)
                        {
                            var record = new ChartOfAccountViewModel
                            {
                                AccountCode = aaccount.Values,
                                AccountDescription = aaccount.Description,
                                Parent = aaccount.HasParent,
                                AllowingBudgeting = aaccount.AllowBudgeting ? "Yes" : "No",
                                AllowingPosting = aaccount.AllowPosting ? "Yes" : "No",
                                AccountType = ""
                            };
                            list.Add(record);
                        }

                        #endregion
                    }
                }
            }
            else
            {

                //ChartOfAccountViewModel record;
                //var allaccounts = _db.GLChartOfAccountAccounts
                //    .Select(x => new { x.AccountType, x.Values, x.Description, x.AllowBudgeting, x.HasParent, x.AllowPosting })
                //    .ToList();
                //if (segment == "Account")
                //{
                //    #region account

                //    if (accountType == "All")
                //    {
                //        var aaccounts = allaccounts;
                //        foreach (var aaccount in aaccounts)
                //        {
                //            record = new ChartOfAccountViewModel
                //            {
                //                AccountCode = aaccount.Values,
                //                AccountDescription = aaccount.Description,
                //                Parent = aaccount.HasParent,
                //                AllowingBudgeting = aaccount.AllowBudgeting ? "Yes" : "No",
                //                AllowingPosting = aaccount.AllowPosting ? "Yes" : "No",
                //                AccountType = aaccount.AccountType.ToString()
                //            };
                //            list.Add(record);
                //        }
                //    }
                //    else
                //    {
                //        var specificAccounts = allaccounts;

                //        #region get specificAccounts

                //        if (accountType == "Asset")
                //        {
                //            specificAccounts = allaccounts.Where(x => x.AccountType == GlAccountTypes.Asset).ToList();
                //        }
                //        else if (accountType == "Liability")
                //        {
                //            specificAccounts = allaccounts.Where(x => x.AccountType == GlAccountTypes.Liability)
                //                .ToList();
                //        }
                //        else if (accountType == "Capital")
                //        {
                //            specificAccounts = allaccounts.Where(x => x.AccountType == GlAccountTypes.Capital).ToList();
                //        }
                //        else if (accountType == "Revenue")
                //        {
                //            specificAccounts = allaccounts.Where(x => x.AccountType == GlAccountTypes.Revenue).ToList();
                //        }
                //        else if (accountType == "Expense")
                //        {
                //            specificAccounts = allaccounts.Where(x => x.AccountType == GlAccountTypes.Expense).ToList();
                //        }
                //        else
                //        {

                //        }

                //        #endregion

                //        foreach (var aaccount in specificAccounts)
                //        {
                //            record = new ChartOfAccountViewModel
                //            {
                //                AccountCode = aaccount.Values,
                //                AccountDescription = aaccount.Description,
                //                Parent = aaccount.HasParent,
                //                AllowingBudgeting = aaccount.AllowBudgeting ? "Yes" : "No",
                //                AllowingPosting = aaccount.AllowPosting ? "Yes" : "No",
                //                AccountType = aaccount.AccountType.ToString()
                //            };
                //            list.Add(record);
                //        }

                //    }

                //    #endregion

                //}
                //else if (segment == "Branch")
                //{
                //    #region Branch

                //    var laccounts = _db.GLChartOfAccountLocations
                //        .Select(x => new {x.Values, x.Description, x.AllowBudgeting, x.HasParent, x.AllowPosting }).ToList();
                //    foreach (var laccount in laccounts)
                //    {
                //        record = new ChartOfAccountViewModel
                //        {
                //            AccountCode = laccount.Values,
                //            AccountDescription = laccount.Description,
                //            Parent = laccount.HasParent,
                //            AllowingBudgeting = laccount.AllowBudgeting ? "Yes" : "No",
                //            AllowingPosting = laccount.AllowPosting ? "Yes" : "No",
                //            AccountType = ""
                //        };
                //        list.Add(record);
                //    }

                //    #endregion
                //}

                //else if (segment == "Cost Center")
                //{
                //    #region Cost Center

                //    var costcenters = _db.GLChartOfAccountCostCenters.Select(x => new {  x.Values, x.Description, x.AllowBudgeting, x.HasParent, x.AllowPosting }).ToList();
                //    foreach (var csaccount in costcenters)
                //    {
                //        record = new ChartOfAccountViewModel
                //        {
                //            AccountCode = csaccount.Values,
                //            AccountDescription = csaccount.Description,
                //            Parent = csaccount.HasParent,
                //            AllowingBudgeting = csaccount.AllowBudgeting ? "Yes" : "No",
                //            AllowingPosting = csaccount.AllowPosting ? "Yes" : "No",
                //            AccountType = ""
                //        };
                //        list.Add(record);
                //    }

                //    #endregion
                //}

                //else if (segment == "Sub Account")
                //{
                //    #region Sub Account

                //    var subaccounts = _db.GLChartOfAccountSubAccounts
                //        .Select(x => new { x.Values, x.Description, x.AllowBudgeting, x.HasParent, x.AllowPosting }).ToList();
                //    foreach (var saaccount in subaccounts)
                //    {
                //        record = new ChartOfAccountViewModel
                //        {
                //            AccountCode = saaccount.Values,
                //            AccountDescription = saaccount.Description,
                //            Parent = saaccount.HasParent,
                //            AllowingBudgeting = saaccount.AllowBudgeting ? "Yes" : "No",
                //            AllowingPosting = saaccount.AllowPosting ? "Yes" : "No",
                //            AccountType = ""
                //        };
                //        list.Add(record);
                //    }

                //    #endregion
                //}
            }

            return list;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> SegmentTypes()
        {

            List<string> list = new List<string> {"Account", "Branch", "Cost Center", "Sub Account"};
            return list;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> AccountTypes()
        {
            List<string> list = new List<string> {"All", "Asset", "Liability", "Capital", "Revenue", "Expense"};
            return list;
        }
    }
}
