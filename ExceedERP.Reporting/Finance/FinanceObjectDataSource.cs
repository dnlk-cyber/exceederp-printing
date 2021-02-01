using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Finance.AP;
using ExceedERP.Core.Domain.Finance.AR;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Finance.ViewModel;

namespace ExceedERP.Reporting.Finance
{
    public class FinanceObjectDataSource
    {
    }

    

    [DataObject]
    public class CustomerListObjectDataSource : List<CustomerListViewModel>
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<CustomerListViewModel> GetCustomerList(string custype)
        {
            List<CustomerListViewModel> list = new List<CustomerListViewModel> {};
            CustomerListViewModel record;

            //var customerlist = _db.ARCustomerProfiles.ToList();
            //if (!String.IsNullOrEmpty(custype))
            //{
            //    customerlist = customerlist.Where(x => x.CustomerType.ToUpper() == custype.ToUpper()).ToList();
            //}
            //foreach (ArCustomerProfile item in customerlist)
            //{
            //    record = new CustomerListViewModel();
            //    record.CompanyName = item.CompanyName;
            //    record.ID = item.ArCustomerProfileId.ToString();
            //    record.Account = item.Account;
            //    record.TINNo = item.TinNo;
            //    record.CustomerType = item.CustomerType;
            //    //record.ContactFirstName = item.ContactFirstName;
            //    //record.ContactLastName = item.ContactLastName;
            //    //record.PhoneNumber = item.PhoneNumber;
            //    //record.Address1 = item.Address1;
            //    //record.Address2 = item.Address2;
            //    //record.Country = item.Country;
            //    if (item.IsActive)
            //        record.isActive = "Yes";
            //    else
            //        record.isActive = "No";
            //    list.Add(record);
            //}
            return list;
        }
    }

    [DataObject]
    public class AccountAnalysisBySegmentObjectDataSource : List<AccountAnalysisViewModel>
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<AccountAnalysisViewModel> AccountAnalysisBySegment(string acctFrom, string acctTo, DateTime from,
            DateTime to)
        {
            List<AccountAnalysisViewModel> list = new List<AccountAnalysisViewModel> {};
            List<GLRecordJournalDetail> selectedlist = new List<GLRecordJournalDetail> {};
            AccountAnalysisViewModel record;

            bool isValidRange = true;
            string acctsFrom = "";
            string acctsTo = "";
            string branchFrom = "";
            string branchTo = "";
            string costFrom = "";
            string costTo = "";
            string subFrom = "";
            string subTo = "";
            string newAccountCode = "";
            //ranges
            List<GLChartOfAccountAccount> accountRange = new List<GLChartOfAccountAccount> {};
            List<GLChartOfAccountLocation> brachRange = new List<GLChartOfAccountLocation> {};
            List<GLChartOfAccountCostCenter> costcenterRange = new List<GLChartOfAccountCostCenter> {};
            List<GLChartOfAccountSubAccount> subaccountRange = new List<GLChartOfAccountSubAccount> {};

            #region validate from codes

            if (!String.IsNullOrEmpty(acctFrom))
            {
                acctFrom = acctFrom.ToUpper();
                try
                {
                    acctsFrom = acctFrom.Split('-')[0];
                    branchFrom = acctFrom.Split('-')[1];
                    costFrom = acctFrom.Split('-')[2];
                    subFrom = acctFrom.Split('-')[3];

                }
                catch (Exception x)
                {
                    isValidRange = false;
                }
            }

            #endregion

            #region validate  to codes

            if (!String.IsNullOrEmpty(acctTo))
            {
                acctTo = acctTo.ToUpper();
                try
                {
                    acctsTo = acctTo.Split('-')[0];
                    branchTo = acctTo.Split('-')[1];
                    costTo = acctTo.Split('-')[2];
                    subTo = acctTo.Split('-')[3];

                }
                catch (Exception x)
                {
                    isValidRange = false;
                }
            }

            #endregion

            if (isValidRange)
            {
                accountRange = _db.GLChartOfAccountAccounts.Where(x => x.Values.ToUpper().CompareTo(acctsFrom) >= 0)
                    .Where(x => x.Values.ToUpper().CompareTo(acctsTo) <= 0).ToList();

                brachRange = _db.GLChartOfAccountLocations.Where(x => x.Values.ToUpper().CompareTo(branchFrom) >= 0)
                    .Where(x => x.Values.ToUpper().CompareTo(branchTo) <= 0).ToList();

                costcenterRange = _db.GLChartOfAccountCostCenters.Where(x => x.Values.ToUpper().CompareTo(costFrom) >= 0)
                    .Where(x => x.Values.ToUpper().CompareTo(costTo) <= 0).ToList();

                subaccountRange = _db.GLChartOfAccountSubAccounts.Where(x => x.Values.ToUpper().CompareTo(subFrom) >= 0)
                    .Where(x => x.Values.ToUpper().CompareTo(subTo) <= 0).ToList();

            }
            try
            {

                var journals = _db.GLRecordJournalDetails.ToList();
                journals = _db.GLRecordJournalDetails.Where(x => x.GLRecordJournal.IsOnlineApproved == true)
                    .Where(x => x.GLRecordJournal.EffectiveDate >= from)
                    .Where(x => x.GLRecordJournal.EffectiveDate <= to).ToList();

                foreach (GLChartOfAccountAccount account in accountRange)
                {
                    foreach (GLChartOfAccountLocation branch in brachRange)
                    {
                        foreach (GLChartOfAccountCostCenter costcenter in costcenterRange)
                        {
                            foreach (GLChartOfAccountSubAccount sub in subaccountRange)
                            {
                                newAccountCode = account.Values + "-" + branch.Values + "-" + costcenter.Values + "-" +
                                                 sub.Values;

                                var range =
                                    journals.Where(x => x.AccountCode.ToUpper() == newAccountCode.ToUpper()).ToList();
                                selectedlist.AddRange(range);
                            }
                        }
                    }
                }

                foreach (GLRecordJournalDetail journal in selectedlist)
                {
                    record = new AccountAnalysisViewModel();
                    record.JournalName = journal.GLRecordJournal.JournalName;
                    record.Description = journal.GLRecordJournal.Description;
                    record.Period = journal.GLRecordJournal.PeriodName;
                    record.EffectiveDate = journal.GLRecordJournal.EffectiveDate.ToShortDateString();
                    record.JournalReference = journal.GLRecordJournal.JournalNumber;
                    record.Category = journal.GLRecordJournal.CategoryName;
                    record.AccountCode = journal.AccountCode;
                    record.AccountDescription = journal.AccountDesc;
                    record.Debit = journal.Debit;
                    record.Credit = journal.Credit;
                    list.Add(record);
                }
                record = new AccountAnalysisViewModel();
                record.JournalName = "TOTAL";
                record.Description = "";
                record.Period = "";
                record.EffectiveDate = "";
                record.JournalReference = "";
                record.Category = "";
                record.AccountCode = "";
                record.AccountDescription = "";
                record.Debit = list.Sum(x => x.Debit);
                record.Credit = list.Sum(x => x.Credit);
                list.Add(record);
                return list;
            }
            catch (Exception)
            {
                return list;
            }

        }
    }

    

    #region Data Source for Report Parameters

    // Object data Source for Period
    [DataObject]
    public class SupplierTypesObjectDataSource
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetSupplierTypes()
        {
            List<string> list = new List<string> {};
            var periods = _db.APSupplierTypes.Select(s => new
            {
                supplierSelector = s.Supplier
            }).OrderBy(x => x.supplierSelector);


            if (!periods.Any())
            {
                return list;
            }

            return periods.Select(x => x.supplierSelector).ToList();
        }
    }

     // Object data Source for Period
    [DataObject]
    public class BusinessTypesObjectDataSource
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetBusinessTypes()
        {
            List<string> list = new List<string> {};
            var periods = _db.APBusinessTypes.Select(s => new
            {
                businessTypeSelector = s.TypeOfBusiness
            }).OrderBy(x => x.businessTypeSelector);


            if (!periods.Any())
            {
                return list;
            }

            return periods.Select(x => x.businessTypeSelector).ToList();
        }
    }
    
    // Object data Source for Fiscal Year
    [DataObject]
    public class GlFiscalYearsObjectDataSource
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetGlFiscalYears()
        {
            List<string> listInt = new List<string> {};
            var fiscalYears = _db.GLFiscalYears.ToList().OrderByDescending(x => x.Name).Select(s => new
            {
                nameSelector = s.Name
            });

            if (!fiscalYears.Any())
            {
                return listInt;
            }
            return fiscalYears.Select(x => x.nameSelector).ToList();
        }
    }



// Object data Source for Source
    [DataObject]
    public class SourceTypeObjectDataSource
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetSources()
        {
            List<string> sourceList = new List<string>
            {
                "Deduction",
                "Long Term Deduction",
                "Fixed Contribution",
                "Regular Contribution"
            };

            return sourceList;
        }
    }
    #endregion
}











