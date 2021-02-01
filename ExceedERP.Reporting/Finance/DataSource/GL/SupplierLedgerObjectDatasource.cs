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
    public class SupplierLedgerObjectDatasource : List<SupplierLedgerVm>
    {
        
            private readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly AccountHelper _accountHelper = new AccountHelper();

        [DataObjectMethod(DataObjectMethodType.Select)]

        public List<SupplierLedgerVm> GetSupplierLedger(DateTime from, DateTime to, string supplierid, string transactionType, string acct)
        {
            List<SupplierLedgerVm> supplierLedgerList = new List<SupplierLedgerVm> { };

            var journalDetails = _db.GLRecordJournalDetails
                .Where(x => x.GLRecordJournal.IsOnlineApproved)
                .Where(x => x.GLRecordJournal.isVoid == false)
                .Where(x => x.GLRecordJournal.EffectiveDate >= from.Date &&
                            x.GLRecordJournal.EffectiveDate <= to.Date)
                .Select(x => new { x.GLRecordJournal, x.GlRecordJournalId, x.AccountCode, x.SubAccountSegment, x.AccountSegment, x.AdditionalDescription, x.Debit, x.Credit }).AsQueryable();

            List<GLPeriod> periods = new List<GLPeriod>();

            if (!String.IsNullOrEmpty(acct))
            {
                journalDetails = journalDetails.Where(x => x.AccountSegment == acct).AsQueryable();

                if (!String.IsNullOrEmpty(supplierid))
                {
                    journalDetails = journalDetails.Where(x => x.SubAccountSegment == supplierid).AsQueryable();
                }

                

                #region get covered periods for sorting data

                var journalids = journalDetails.Select(x => x.GlRecordJournalId).Distinct();

                var periodList = _db.GLRecordJournals.Where(x => journalids.Contains(x.GlRecordJournalId)).Select(x => x.PeriodName).Distinct();
                foreach (var p in periodList)
                {
                    periods.Add(_db.GLPeriods.FirstOrDefault(x => x.Name == p));
                }

                #endregion

                var sortedPeriods = periods.OrderBy(x => x.DateTo);

                var journalDetailRange = journalDetails.AsEnumerable().DistinctByField(x => x.AccountCode).ToList();

                foreach (var detail in journalDetailRange)
                {
                    GlAccountTypes accountType = _accountHelper.GetAccountType(detail.AccountCode);

                    var seletctedDetails = journalDetails
                          .Where(x => x.AccountCode == detail.AccountCode).ToList();

                    var supplierLedgerVm = new SupplierLedgerVm
                    {
                        SupplierId = detail.AccountCode, //to be updated
                        SupplierName = _accountHelper.GetAccountDesc(detail.AccountCode), //to be updated
                        SupplierLedgerVmLines = new List<TransactionVm>()
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


                                supplierLedgerVm.SupplierLedgerVmLines.Add(lines);
                            }

                            #endregion

                        }

                        #endregion
                    }

                    supplierLedgerVm.Debit = supplierLedgerVm.SupplierLedgerVmLines.Sum(x => x.Debit);
                    supplierLedgerVm.Credit = supplierLedgerVm.SupplierLedgerVmLines.Sum(x => x.Credit);
                    supplierLedgerVm.Balance = supplierLedgerVm.SupplierLedgerVmLines.Sum(x => x.Balance);

                    supplierLedgerList.Add(supplierLedgerVm);
                }
            }

          

            return supplierLedgerList;
        }
        //public List<SupplierLedgerVm> GetSupplierLedger1(DateTime from, DateTime to, string supplierid, string transactionType, string acct)
        //    {
        //        List<SupplierLedgerVm> supplierLedgerList = new List<SupplierLedgerVm> { };
        //    var ledgerposts = _db.GlSecondayLedgers.AsQueryable();
        //    List<GlSecondayLedger> ledgers = new List<GlSecondayLedger>();
        //    List<GLPeriod> periods = new List<GLPeriod>();


        //        if (!String.IsNullOrEmpty(acct))
        //        {
        //            ledgerposts = ledgerposts.Where(x => x.AccountSegment == acct).AsQueryable();

                   
        //            //if (AccountHelper.IsDynamicCoA())
        //            //{
        //            //    AccountHelper accountHelper = new AccountHelper();
        //            //     string[] account = acct.Split('-');
        //            //    string acccountSeG = account[AccountHelper.GetAccountSequenceIndex(GlSegmentTypes.Account)];
        //            //    string locationSeG = account[AccountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location)];
        //            //    string costCenterSeG = account[AccountHelper.GetAccountSequenceIndex(GlSegmentTypes.CostCenter)];
        //            //    string subAccountSeG = supplierid;
        //            //    string newacc = accountHelper.RearrangeAccountSequence(acccountSeG, locationSeG,costCenterSeG, subAccountSeG);
        //            //    acct = newacc;
        //            //}
        //            //else
        //            //{
        //            //    int subaccountindex = DynamicAccountHelper.GetSubAccountSequenceIndex();
        //            //    string subAccountSeG = supplierid;
        //            //    string newacc = DynamicAccountHelper.ReplaceSegment(acct, subaccountindex, subAccountSeG);
        //            //    acct = newacc;
        //            //}

                    
                   

        //         }

        //        if (!String.IsNullOrEmpty(supplierid))
        //            {
        //                ledgerposts = ledgerposts.Where(x => x.SubAccountSegment == supplierid).AsQueryable();
        //            }

        //     var ledgerPostingLines = _db.GlSecondayLedgerLines.Where(b => b.EffectiveDate >= from && b.EffectiveDate <= to).ToList();
        //    #region get covered periods for sorting data

        //    var periodList = ledgerPostingLines.Select(x => x.GLPeriodId).Distinct();
        //    foreach (var p in periodList)
        //    {
        //        periods.Add(_db.GLPeriods.Find(p));
        //    }

        //    #endregion
        //    var sortedPeriods = periods.OrderBy(x => x.DateTo);

        //    foreach (var ledgerPosting in ledgerposts)
        //    {
        //        var selectedledgerPostingLines = ledgerPostingLines.Where(x => x.GlSecondayLedger.Description == ledgerPosting.Description).ToList();

        //        var supplierLedgerVm = new SupplierLedgerVm
        //        {
        //            SupplierId = ledgerPosting.Description,//to be updated
        //            SupplierName = _accountHelper.GetAccountDesc(ledgerPosting.Description),//to be updated
        //            SupplierLedgerVmLines = new List<TransactionVm>()
        //        };

        //        if (selectedledgerPostingLines.Any())
        //        {
        //            #region current transaction
        //            foreach (var period in sortedPeriods)
        //            {
        //                #region beginning

        //                var begginningRecords = selectedledgerPostingLines
        //                    .Where(x => x.GLPeriodId == period.GLPeriodId)
        //                    .Where(x => x.Type == LedgerPostTypes.Begining).ToList();
        //                foreach (var balance in begginningRecords)
        //                {
        //                    var lines = new TransactionVm
        //                    {
        //                        TranssactionId = balance.JournalNumber,
        //                        TransactionDate = balance.EffectiveDate,
        //                        TransReference = balance.JournalReference,
        //                        Debit = balance.Debit,
        //                        Credit = balance.Credit,
        //                        Balance = balance.Balance,
        //                        TransactionType = balance.SourcesName//to be updated
        //                    };

        //                    supplierLedgerVm.SupplierLedgerVmLines.Add(lines);
        //                }

        //                #endregion
        //                #region standard

        //                var standardRecords = selectedledgerPostingLines
        //                    .Where(x => x.GLPeriodId == period.GLPeriodId)
        //                    .Where(x => x.Type == LedgerPostTypes.Standard)
        //                    .ToList().OrderBy(x => x.EffectiveDate);
        //                foreach (var balance in standardRecords)
        //                {
        //                    var lines = new TransactionVm
        //                    {
        //                        TranssactionId = balance.JournalNumber,
        //                        TransactionDate = balance.EffectiveDate,
        //                        TransReference = balance.JournalReference,
        //                        Debit = balance.Debit,
        //                        Credit = balance.Credit,
        //                        Balance = balance.Balance,
        //                        TransactionType = balance.SourcesName//to be updated
        //                    };

        //                    supplierLedgerVm.SupplierLedgerVmLines.Add(lines);
        //                }

        //                #endregion
        //                #region ending

        //                var endingRecords = selectedledgerPostingLines
        //                    .Where(x => x.GLPeriodId == period.GLPeriodId)
        //                    .Where(x => x.Type == LedgerPostTypes.Ending).ToList();
        //                foreach (var balance in begginningRecords)
        //                {
        //                    var lines = new TransactionVm
        //                    {
        //                        TranssactionId = balance.JournalNumber,
        //                        TransactionDate = balance.EffectiveDate,
        //                        TransReference = balance.JournalReference,
        //                        Debit = balance.Debit,
        //                        Credit = balance.Credit,
        //                        Balance = balance.Balance,
        //                        TransactionType = balance.SourcesName//to be updated
        //                    };

        //                    supplierLedgerVm.SupplierLedgerVmLines.Add(lines);
        //                }

        //                #endregion
        //            }

        //            #endregion
        //        }



        //        supplierLedgerList.Add(supplierLedgerVm);
        //    }

        //    return supplierLedgerList;
        //    }
        //public List<SupplierLedgerVm> GetSupplierLedger(DateTime from, DateTime to, string supplierid, string transactionType)
        //{
        //    List<SupplierLedgerVm> supplierLedgerList = new List<SupplierLedgerVm> { };
        //    var invoice = db.APPaybleInvoices.Where(x => x.IsOnlineApproved & x.IsVoid == false).AsQueryable();
        //    var standardCollections = db.APPayments.Where(x => x.IsOnlineApproved & x.IsVoid == false).AsQueryable();
        //    var miscelaneousCollections = db.APMiscellaneousPayments.Where(x => x.IsOnlineApproved & x.IsVoid == false).AsQueryable();



        //    if (!String.IsNullOrWhiteSpace(supplierid))
        //    {

        //        invoice = invoice.Where(x => x.SupplierName == supplierid).AsQueryable();
        //        standardCollections = standardCollections.Where(x => x.SupplierName == supplierid).AsQueryable();
        //        miscelaneousCollections = miscelaneousCollections.Where(x => x.SupplierName == supplierid).AsQueryable();
        //    }

        //    if (String.IsNullOrWhiteSpace(transactionType) || transactionType == "Invoice")
        //    {
        //        #region invoice

        //        invoice = invoice
        //           .Where(x => x.InvoiceDate >= from)
        //           .Where(x => x.InvoiceDate <= to).AsQueryable();
        //        foreach (var i in invoice)
        //        {
        //            var distribution = i.APPaybleInvoiceDistribution;
        //            foreach (var d in distribution)
        //            {
        //                var transact = new SupplierLedgerVm
        //                {
        //                    SupplierId = i.SupplierId,
        //                    SupplierName = i.SupplierName,
        //                    TranssactionId = i.ApPaybleInvoicesId,
        //                    TransactionDate = i.InvoiceDate,
        //                    TransReference = i.Remark,
        //                    TransactionType = "Invoice",
        //                    Debit = d.Debit,
        //                    Credit = d.Credit
        //                };
        //                supplierLedgerList.Add(transact);
        //            }
        //        }



        //        #endregion
        //    }

        //    if (String.IsNullOrWhiteSpace(transactionType) || transactionType == "Payment")
        //    {
        //        #region collection

        //        standardCollections = standardCollections
        //           .Where(x => x.PaymentDate >= from)
        //           .Where(x => x.PaymentDate <= to).AsQueryable();
        //        foreach (var i in standardCollections)
        //        {
        //            var distribution = i.ApPaymentDistribution;
        //            foreach (var d in distribution)
        //            {
        //                var transact = new SupplierLedgerVm
        //                {
        //                    SupplierId = i.SupplierId,
        //                    SupplierName = i.SupplierName,
        //                    TranssactionId = i.ApPaymentId,
        //                    TransactionDate = i.PaymentDate,
        //                    TransReference = i.Remark,
        //                    TransactionType = "Payment",
        //                    Debit = d.Debit,
        //                    Credit = d.Credit
        //                };
        //                supplierLedgerList.Add(transact);
        //            }
        //        }



        //        #endregion
        //    }

        //    if (String.IsNullOrWhiteSpace(transactionType) || transactionType == "Misc Payment")
        //    {
        //        #region misc collection

        //        miscelaneousCollections = miscelaneousCollections
        //           .Where(x => x.PaymentDate >= from)
        //           .Where(x => x.PaymentDate <= to).AsQueryable();
        //        foreach (var i in miscelaneousCollections)
        //        {
        //            var distribution = i.ApMiscellaneousPaymentDistribution;
        //            foreach (var d in distribution)
        //            {
        //                var transact = new SupplierLedgerVm
        //                {
        //                    SupplierId = i.SupplierId,
        //                    SupplierName = i.SupplierName,
        //                    TranssactionId = i.ApMiscellaneousPaymentId,
        //                    TransactionDate = i.PaymentDate,
        //                    TransReference = i.Remark,
        //                    TransactionType = "Misc. Payment",
        //                    Debit = d.Debit,
        //                    Credit = d.Credit
        //                };
        //                supplierLedgerList.Add(transact);
        //            }
        //        }



        //        #endregion
        //    }

        //    return supplierLedgerList;
        //}
    }
    
}
