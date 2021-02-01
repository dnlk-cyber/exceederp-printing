using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Reporting.Finance.ViewModel;

namespace ExceedERP.Reporting.Finance.DataSource.GL
{
    [DataObject]
    public class CustomerLedgerObjectDatasource : List<CustomerLedgerVm>
    {
        
        private readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly AccountHelper _accountHelper = new AccountHelper();

        [DataObjectMethod(DataObjectMethodType.Select)]
            public List<CustomerLedgerVm> GetCustomerLedger(DateTime  from, DateTime to, string customerid, string transactionType,string acct)
            {
                List<CustomerLedgerVm> customerLedgerList = new List<CustomerLedgerVm> { };

            

              var journalDetails = _db.GLRecordJournalDetails
                    .Where(x=>x.GLRecordJournal.IsOnlineApproved)
                    .Where(x => x.GLRecordJournal.isVoid==false)
                    .Where(x => x.GLRecordJournal.EffectiveDate >= from.Date &&
                                x.GLRecordJournal.EffectiveDate <= to.Date)
                  //.Where(x => accountList.Any(f => x.Description.StartsWith(f)))
                  .Select(x=> new {x.GLRecordJournal,x.GlRecordJournalId,x.AccountCode,x.SubAccountSegment,x.AccountSegment,x.AdditionalDescription,x.Debit,x.Credit})
                  .AsQueryable();
                
                List<GLPeriod> periods = new List<GLPeriod>();

                if (!String.IsNullOrEmpty(acct))
                {
                    journalDetails = journalDetails.Where(x => x.AccountSegment == acct)
                        .AsQueryable();

                    if (!String.IsNullOrEmpty(customerid))
                    {
                        journalDetails = journalDetails.Where(x => x.SubAccountSegment == customerid).AsQueryable();
                    }

                     #region get covered periods for sorting data

                    var journalids = journalDetails.Select(x => x.GlRecordJournalId).Distinct();

                    var periodList = _db.GLRecordJournals.Where(x=>journalids.Contains(x.GlRecordJournalId)).Select(x => x.PeriodName).Distinct();
                    foreach (var p in periodList)
                    {
                        periods.Add(_db.GLPeriods.FirstOrDefault(x => x.Name==p));
                    }

                    #endregion

                    var sortedPeriods = periods.OrderBy(x => x.DateTo);

                    var journalDetailRange = journalDetails.AsEnumerable().DistinctByField(x => x.AccountCode).ToList();

                    foreach (var detail in journalDetailRange)
                    {
                        GlAccountTypes accountType = _accountHelper.GetAccountType(detail.AccountCode);

                      var seletctedDetails = journalDetails
                            .Where(x => x.AccountCode == detail.AccountCode).ToList();

                        var customerLedgerVm = new CustomerLedgerVm
                        {
                            CustomerId = detail.AccountCode, //to be updated
                            CustomerName = _accountHelper.GetAccountDesc(detail.AccountCode), //to be updated
                            CustomerLedgerVmLines = new List<TransactionVm>()
                        };

                        if (seletctedDetails.Any())
                        {
                            #region current transaction

                            foreach (var period in sortedPeriods)
                            {
                                #region records

                                var recordJournalDetails = seletctedDetails
                                    .Where(x => x.GLRecordJournal.PeriodName == period.Name).ToList();
                                foreach (var journalDetail in recordJournalDetails)
                                {
                                    var lines = new TransactionVm
                                    {
                                        TranssactionId = journalDetail.GLRecordJournal.JournalNumber,
                                        TransactionDate = journalDetail.GLRecordJournal.EffectiveDate,
                                        TransReference =!String.IsNullOrEmpty(journalDetail.AdditionalDescription)? journalDetail.AdditionalDescription: journalDetail.GLRecordJournal.Description,
                                        ReferenceDoc = journalDetail.GLRecordJournal.ReferenceDoc,
                                        PaymentReference = journalDetail.GLRecordJournal.PaymentReference,
                                        InvoiceReferenceDoc = journalDetail.GLRecordJournal.InvoiceReferenceDoc,
                                        Debit = journalDetail.Debit,
                                        Credit = journalDetail.Credit,
                                        Balance = 0,
                                        Remark = journalDetail.GLRecordJournal.Remark,
                                        //TransactionType = journalDetail.SourcesName //to be updated
                                    };

                                    if (accountType == GlAccountTypes.Asset || accountType == GlAccountTypes.Expense)
                                        lines.Balance = journalDetail.Debit - journalDetail.Credit;
                                    else
                                    {
                                        lines.Balance = journalDetail.Credit - journalDetail.Debit;
                                    }
                                

                                    customerLedgerVm.CustomerLedgerVmLines.Add(lines);
                                }

                                #endregion
                               
                            }

                            #endregion
                        }

                        customerLedgerVm.Debit = customerLedgerVm.CustomerLedgerVmLines.Sum(x => x.Debit);
                        customerLedgerVm.Credit = customerLedgerVm.CustomerLedgerVmLines.Sum(x => x.Credit);
                        customerLedgerVm.Balance = customerLedgerVm.CustomerLedgerVmLines.Sum(x => x.Balance);

                        customerLedgerList.Add(customerLedgerVm);
                    }
                }

                return customerLedgerList;
            }


       
    }
    
}
