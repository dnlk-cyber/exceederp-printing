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
    public class EmployeeLedgerObjectDatasource : List<EmployeeLedgerVm>, IDisposable
    {
        private readonly ExceedDbContext _db;
        private readonly AccountHelper _accountHelper;
        public EmployeeLedgerObjectDatasource()
        {
            _db = new ExceedDbContext();
            _accountHelper = new AccountHelper();
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeLedgerVm> GetEmployeeLedger(DateTime from, DateTime to, string employeeid, string acct)
        {
            List<EmployeeLedgerVm> employeeLedgerList = new List<EmployeeLedgerVm> { };
            var journalDetails = _db.GLRecordJournalDetails
                .Where(x => x.GLRecordJournal.IsOnlineApproved)
                .Where(x => x.GLRecordJournal.isVoid == false)
                .Where(x => x.GLRecordJournal.EffectiveDate >= from.Date &&
                            x.GLRecordJournal.EffectiveDate <= to.Date)
                .Select(x => new { x.GLRecordJournal, x.GlRecordJournalId, x.AccountCode, x.SubAccountSegment, x.AccountSegment, x.AdditionalDescription, x.Debit, x.Credit })
                            .AsQueryable();

            // List<GLPeriod> periods = new List<GLPeriod>();

            if (!String.IsNullOrEmpty(acct))
            {
                journalDetails = journalDetails.Where(x => x.AccountSegment == acct).AsQueryable();

                if (!String.IsNullOrEmpty(employeeid))
                {
                    //#region get sub account employee
                    //var employeeSubAccount = _db.EmployeeSubAccounts.FirstOrDefault(x => x.EmployeeId.ToString() == employeeid);
                    //if (employeeSubAccount == null)
                    //    return employeeLedgerList;
                    //#endregion

                    journalDetails = journalDetails.Where(x => x.SubAccountSegment == employeeid)
                        .AsQueryable();
                }




                var journalDetailRange = journalDetails.AsEnumerable().DistinctByField(x => x.AccountCode)
                       .ToList();
                foreach (var detail in journalDetailRange)
                {
                    GlAccountTypes accountType = _accountHelper.GetAccountType(detail.AccountCode);

                    var seletctedDetails = journalDetails
                        .Where(x => x.AccountCode == detail.AccountCode).ToList();

                    var employeeLedgerVm = new EmployeeLedgerVm
                    {
                        EmployeeId = detail.AccountCode, //to be updated
                        EmployeeName = _accountHelper.GetAccountDesc(detail.AccountCode), //to be updated
                        EmployeeLedgerVmLines = new List<TransactionVm>()
                    };

                    if (seletctedDetails.Any())
                    {
                        #region current transaction

                        #region records

                        foreach (var journalDetail in seletctedDetails)
                        {
                            var lines = new TransactionVm
                            {
                                TranssactionId = journalDetail.GLRecordJournal.JournalNumber,
                                TransactionDate = journalDetail.GLRecordJournal.EffectiveDate,
                                TransReference = !String.IsNullOrEmpty(journalDetail.AdditionalDescription) ? journalDetail.AdditionalDescription : journalDetail.GLRecordJournal.Description,
                                ReferenceDoc = journalDetail.GLRecordJournal.ReferenceDoc,
                                PaymentReference = journalDetail.GLRecordJournal.PaymentReference,
                                InvoiceReferenceDoc = journalDetail.GLRecordJournal.InvoiceReferenceDoc,
                                Debit = journalDetail.Debit,
                                Credit = journalDetail.Credit,
                                Balance = 0,
                                Remark = journalDetail.GLRecordJournal.Remark
                                //TransactionType = journalDetail.SourcesName //to be updated
                            };

                            if (accountType == GlAccountTypes.Asset || accountType == GlAccountTypes.Expense)
                                lines.Balance = journalDetail.Debit - journalDetail.Credit;
                            else
                            {
                                lines.Balance = journalDetail.Credit - journalDetail.Debit;
                            }


                            employeeLedgerVm.EmployeeLedgerVmLines.Add(lines);
                        }

                        #endregion

                        #endregion
                    }

                    employeeLedgerVm.Debit = employeeLedgerVm.EmployeeLedgerVmLines.Sum(x => x.Debit);
                    employeeLedgerVm.Credit = employeeLedgerVm.EmployeeLedgerVmLines.Sum(x => x.Credit);
                    employeeLedgerVm.Balance = employeeLedgerVm.EmployeeLedgerVmLines.Sum(x => x.Balance);

                    employeeLedgerList.Add(employeeLedgerVm);
                }
            }

            return employeeLedgerList;
        }

        public void Dispose()
        {
            _db?.Dispose();
        }
    }

}
