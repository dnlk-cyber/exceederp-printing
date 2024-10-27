using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Helpers.Finance
{
    public static class FinanceTransferHelper
    {

        #region AP transferrable list

        public static List<int> GetApMiscPaymentTranferableList()
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                return db.APMiscellaneousPayments.Where(x => x.IsOnlineApproved && x.IsOnlineTransferred == false && x.IsVoid == false).Select(x => x.ApMiscellaneousPaymentId).ToList();
            }
           
        }
        public static List<int> GetApPaymentTranferableList()
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                return db.APPayments.Where(x => x.IsPrepayment == false).Where(x => x.IsOnlineApproved && x.IsOnlineTransferred == false && x.IsVoid == false).Select(x => x.ApPaymentId).ToList();
            }
           
        }
         public static List<int> GetApAdvancePaymentTranferableList()
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                return db.APPayments.Where(x => x.IsPrepayment).Where(x => x.IsOnlineApproved && x.IsOnlineTransferred == false && x.IsVoid == false).Select(x => x.ApPaymentId).ToList();
            }
           
        }
        public static List<int> GetApDirectPaymentTranferableList()
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                return db.ApDirectPayments.Where(x => x.IsOnlineApproved && x.IsOnlineTransferred == false && x.IsVoid == false).Select(x => x.ApDirectPaymentId).ToList();
            }
           
        }
        public static List<int> GetApPaymentSettlementTranferableList()
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                return db.ApPaymentSettlements.Where(x => x.IsOnlineApproved && x.IsOnlineTransferred == false && x.IsVoid == false).Select(x => x.ApPaymentSettlementId).ToList();
            }

        }
        public static List<int> GetApManualInvoiceTranferableList()
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                return db.APPaybleInvoices.Where(x => x.IsManual).Where(x => x.IsOnlineApproved && x.IsOnlineTransferred == false && x.IsVoid == false).Select(x => x.ApPaybleInvoicesId).ToList();
            }
        }
        public static List<int> GetApInvoiceTranferableList()
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                return db.APPaybleInvoices.Where(x => x.IsManual == false).Where(x => x.IsOnlineApproved && x.IsOnlineTransferred == false && x.IsVoid == false).Select(x => x.ApPaybleInvoicesId).ToList();
            }
        }

        #endregion
        #region AR transferrable list

        public static List<int> GetArMiscCollectionTranferableList()
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                return db.ARMiscelaneousCollections.Where(x => x.IsOnlineApproved && x.IsOnlineTransferred == false && x.IsVoid == false).Select(x => x.ARMiscelaneousCollectionID).ToList();
            }
        }
        public static List<int> GetArStandardCollectionTranferableList()
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                return db.ARStandardCollections.Where(x => x.IsOnlineApproved && x.IsOnlineTransferred == false && x.IsVoid == false).Select(x => x.ArStandardCollectionId).ToList();
            }
        }
        public static List<int> GetArInvoiceManualTranferableList()
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                return db.ARInvoices.Where(x => x.IsManual == false).Where(x => x.IsOnlineApproved && x.IsOnlineTransferred == false && x.IsVoid == false).Select(x => x.ArInvoiceId).ToList();
            }
        }
        public static List<int> GetArInvoiceTranferableList()
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                return db.ARInvoices.Where(x => x.IsManual == false).Where(x => x.IsOnlineApproved && x.IsOnlineTransferred == false && x.IsVoid == false).Select(x => x.ArInvoiceId).ToList();
            }
        }
        public static List<int> GetArSalesOrderTranferableList()
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                return db.ArSalesOrders.Where(x => x.IsOnlineApproved && x.IsOnlineTransferred == false && x.isVoid == false).Select(x => x.ArSalesOrderId).ToList();
            }
        }


        #endregion


        #region AP transfer helper
        public static string ApDirectPayment(int id, string description, string user)
        {
            using (ExceedDbContext _db = new ExceedDbContext())
            {
                AccountHelper helper = new AccountHelper();
                string responseMessage = "";

                var record = _db.ApDirectPayments.Find(id);

                if (record != null && record.IsOnlineApproved && !record.IsOnlineTransferred)
                {
                    #region check if already transffered 
                    var exist = _db.GLRecordJournals
                                   .Where(x => x.Source == JournalSources.AP_Direct_Payment.ToString())
                                   .Where(x => x.SourceId == record.ApDirectPaymentId.ToString())
                                   .FirstOrDefault();
                    if (exist != null)
                        return "The record is already transffered, it can't be retransferred.";

                    #endregion
                    #region check segment length validation
                    AccountHelper accountHelper = new AccountHelper();
                    foreach (var dist in record.ApDirectPaymentDistribution)
                    {
                        string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
                        if (!String.IsNullOrEmpty(message))
                            return message;
                    }


                    #endregion

                    #region check if debit and credit are equal

                    decimal debit = 0;
                    decimal credit = 0;
                    var distribution = record.ApDirectPaymentDistribution.ToList();
                    if (distribution.Any())
                    {
                        debit = distribution.Sum(x => x.Debit);
                        credit = distribution.Sum(x => x.Credit);
                    }

                    if (debit != credit)
                    {
                        responseMessage = "Total debit and total credit is not balanced.";
                        return responseMessage;
                    }

                    if (debit <= 0)
                    {
                        responseMessage = "Total debit and total credit should be positive number.";
                        return responseMessage;
                    }

                    #endregion

                    #region send to GL

                    string glyear = "";
                    string glperiod = "";
                    #region get period
                    var period = _db.GLPeriods.Where(x => x.DateFrom <= record.PaymentDate.Date)
                        .Where(x => x.DateTo >= record.PaymentDate.Date).ToList();
                    if (period.Any())
                    {

                        if (period.Count() == 1)
                        {
                            var myPeriod = period.FirstOrDefault();
                            if (myPeriod != null)
                            {
                                glperiod = myPeriod.Name;
                                glyear = myPeriod.GlFiscalYear.Name.ToString();
                                if (myPeriod.Status == FiscalYearStatus.Close)
                                {
                                    responseMessage = "The period " + glperiod +
                                                  " is closed,the record cannot be transfered to closed period.";
                                    return responseMessage;
                                }
                            }
                            else
                            {
                                responseMessage = "Period not found.";
                                return responseMessage;
                            }
                        }

                        else
                        {
                            responseMessage = "There are multiple periods defined that contain the payment date date.";
                            return responseMessage;
                        }

                    }
                    else
                    {
                        responseMessage = "There are no periods defined that contain the payment date.";
                        return responseMessage;
                    }

                    #endregion

                    if (record.ApDirectPaymentDistribution.Any())
                    {
                        GlRecordJournal journal = new GlRecordJournal
                        {
                            DateCreated = DateTime.Now,
                            CreatedBy = user,
                            JournalName = "AP Direct Payment",
                            Description = !String.IsNullOrEmpty(description) ? description : "AP Direct Payment",
                            FiscalYear = glyear,
                            PeriodName = glperiod,
                            CategoryName = "AP Direct Payment",
                            SourcesName = "AP",
                            JournalNumber = "APDPAY." + record.ApDirectPaymentId.ToString(),
                            SourceId = record.ApDirectPaymentId.ToString(),
                            Source = JournalSources.AP_Direct_Payment.ToString(),
                            EffectiveDate = record.PaymentDate.Date,
                            OrgBranchName = record.OrgBranchName,
                            Status = "Approved",
                            BranchId = record.CreatedAtBranchId,
                            BusinessUnitId = record.CreatedAtBusinessUnitId,
                            Remark = record.PaymentReference,
                            PaymentReference = record.PaymentReference,
                            ReferenceDoc = record.Remark,
                            InvoiceReferenceDoc = "",
                            DebitSum = debit,
                            IsOnlineApproved = true,
                            OnlineApprovedBy = user,
                            OnlineApprovedTime = DateTime.Now
                        };
                        _db.GLRecordJournals.Add(journal);
                        _db.SaveChanges();
                        foreach (var i in record.ApDirectPaymentDistribution)
                        {
                            #region debit and credit

                            var journalDetail = new GLRecordJournalDetail
                            {
                                DateCreated = DateTime.Now,
                                CreatedBy = user,
                                GLRecordJournal = journal,
                                AccountCode = i.AccountCode,
                                AccountDesc = i.AccountDesc,
                                AdditionalDescription = i.AdditionalDescription,
                                Debit = i.Debit,
                                Credit = i.Credit
                            };
                            _db.GLRecordJournalDetails.Add(journalDetail);
                            _db.SaveChanges();

                            #endregion
                        }
                        #region split journal
                        foreach (var glRecordJournalDetail in journal.GLRecordJournalDetail)
                        {
                            helper.SplitJournal(glRecordJournalDetail.GlRecordJournalDetailId);
                        }
                        #endregion
                        record.IsOnlineTransferred = true;
                        record.IsOnlineTransferredBy = user;
                        record.OnlineTransferredTime = DateTime.Now;
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                        responseMessage = "SUCCESS";
                        return responseMessage;
                    }

                    responseMessage = "There is no distribution to be transferred.";
                    return responseMessage;

                    #endregion
                }
                return responseMessage;
            }
        }

        public static string ApPayment(int id, string description, string user)
        {
            using (ExceedDbContext _db = new ExceedDbContext())
            {
                AccountHelper helper = new AccountHelper();
                string responseMessage = "";

                var record = _db.APPayments.Find(id);

                if (record != null && record.IsOnlineApproved && !record.IsOnlineTransferred)
                {
                    #region check if already transffered 
                    var exist = _db.GLRecordJournals
                                   .Where(x => x.Source == JournalSources.AP_Direct_Payment.ToString())
                                   .Where(x => x.SourceId == record.ApPaymentId.ToString())
                                   .FirstOrDefault();
                    if (exist != null)
                        return "The record is already transffered, it can't be retransferred.";

                    #endregion
                    #region check segment length validation
                    AccountHelper accountHelper = new AccountHelper();
                    foreach (var dist in record.ApPaymentDistribution)
                    {
                        string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
                        if (!String.IsNullOrEmpty(message))
                            return message;
                    }


                    #endregion

                    #region check if debit and credit are equal

                    decimal debit = 0;
                    decimal credit = 0;
                    var distribution = record.ApPaymentDistribution.ToList();
                    if (distribution.Any())
                    {
                        debit = distribution.Sum(x => x.Debit);
                        credit = distribution.Sum(x => x.Credit);
                    }

                    if (debit != credit)
                    {
                        responseMessage = "Total debit and total credit is not balanced.";
                        return responseMessage;
                    }

                    if (debit <= 0)
                    {
                        responseMessage = "Total debit and total credit should be positive number.";
                        return responseMessage;
                    }

                    #endregion

                    #region send to GL

                    string glyear = "";
                    string glperiod = "";
                    #region get period
                    var period = _db.GLPeriods.Where(x => x.DateFrom <= record.PaymentDate.Date)
                        .Where(x => x.DateTo >= record.PaymentDate.Date).ToList();
                    if (period.Any())
                    {

                        if (period.Count() == 1)
                        {
                            var myPeriod = period.FirstOrDefault();
                            if (myPeriod != null)
                            {
                                glperiod = myPeriod.Name;
                                glyear = myPeriod.GlFiscalYear.Name.ToString();
                                if (myPeriod.Status == FiscalYearStatus.Close)
                                {
                                    responseMessage = "The period " + glperiod +
                                                  " is closed,the record cannot be transfered to closed period.";
                                    return responseMessage;
                                }
                            }
                            else
                            {
                                responseMessage = "Period not found.";
                                return responseMessage;
                            }
                        }

                        else
                        {
                            responseMessage = "There are multiple periods defined that contain the payment date date.";
                            return responseMessage;
                        }

                    }
                    else
                    {
                        responseMessage = "There are no periods defined that contain the payment date.";
                        return responseMessage;
                    }

                    #endregion

                    if (record.ApPaymentDistribution.Any())
                    {
                        GlRecordJournal journal = new GlRecordJournal
                        {
                            DateCreated = DateTime.Now,
                            CreatedBy = user,
                            JournalName = "AP Payment",
                            Description = !String.IsNullOrEmpty(description) ? description : "AP Direct Payment",
                            FiscalYear = glyear,
                            PeriodName = glperiod,
                            CategoryName = "AP Payment",
                            SourcesName = "AP",
                            JournalNumber = "APPAY." + record.ApPaymentId.ToString(),
                            SourceId = record.ApPaymentId.ToString(),
                            Source = record.IsPrepayment
                                            ? JournalSources.AP_PrePayment.ToString()
                                            : JournalSources.AP_Payment.ToString(),
                            EffectiveDate = record.PaymentDate.Date,
                            OrgBranchName = record.OrgBranchName,
                            Status = "Approved",
                            BranchId = record.CreatedAtBranchId,
                            BusinessUnitId = record.CreatedAtBusinessUnitId,
                            Remark = record.PaymentReference,
                            PaymentReference = record.PaymentReference,
                            ReferenceDoc = record.Remark,
                            InvoiceReferenceDoc = "",
                            DebitSum = debit,
                            IsOnlineApproved = true,
                            OnlineApprovedBy = user,
                            OnlineApprovedTime = DateTime.Now
                        };
                        _db.GLRecordJournals.Add(journal);
                        _db.SaveChanges();
                        foreach (var i in record.ApPaymentDistribution)
                        {
                            #region debit and credit

                            var journalDetail = new GLRecordJournalDetail
                            {
                                DateCreated = DateTime.Now,
                                CreatedBy = user,
                                GLRecordJournal = journal,
                                AccountCode = i.AccountCode,
                                AccountDesc = i.AccountDesc,
                                AdditionalDescription = i.AdditionalDescription,
                                Debit = i.Debit,
                                Credit = i.Credit
                            };
                            _db.GLRecordJournalDetails.Add(journalDetail);
                            _db.SaveChanges();

                            #endregion
                        }
                        #region split journal
                        foreach (var glRecordJournalDetail in journal.GLRecordJournalDetail)
                        {
                            helper.SplitJournal(glRecordJournalDetail.GlRecordJournalDetailId);
                        }
                        #endregion
                        record.IsOnlineTransferred = true;
                        record.IsOnlineTransferredBy = user;
                        record.OnlineTransferredTime = DateTime.Now;
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                        responseMessage = "SUCCESS";
                        return responseMessage;
                    }

                    responseMessage = "There is no distribution to be transferred.";
                    return responseMessage;

                    #endregion
                }
                return responseMessage;
            }
        }

        public static string ApMiscPayment(int id, string description, string user)
        {
            using (ExceedDbContext _db = new ExceedDbContext())
            {
                AccountHelper helper = new AccountHelper();
                string responseMessage = "";

                var record = _db.APMiscellaneousPayments.Find(id);

                if (record != null && record.IsOnlineApproved && !record.IsOnlineTransferred)
                {
                    #region check if already transffered 
                    var exist = _db.GLRecordJournals
                                   .Where(x => x.Source == JournalSources.AP_MiscPayment.ToString())
                                   .Where(x => x.SourceId == record.ApMiscellaneousPaymentId.ToString())
                                   .FirstOrDefault();
                    if (exist != null)
                        return "The record is already transffered, it can't be retransferred.";

                    #endregion
                    #region check segment length validation
                    AccountHelper accountHelper = new AccountHelper();
                    foreach (var dist in record.ApMiscellaneousPaymentDistribution)
                    {
                        string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
                        if (!String.IsNullOrEmpty(message))
                            return message;
                    }


                    #endregion

                    #region check if debit and credit are equal

                    decimal debit = 0;
                    decimal credit = 0;
                    var distribution = record.ApMiscellaneousPaymentDistribution.ToList();
                    if (distribution.Any())
                    {
                        debit = distribution.Sum(x => x.Debit);
                        credit = distribution.Sum(x => x.Credit);
                    }

                    if (debit != credit)
                    {
                        responseMessage = "Total debit and total credit is not balanced.";
                        return responseMessage;
                    }

                    if (debit <= 0)
                    {
                        responseMessage = "Total debit and total credit should be positive number.";
                        return responseMessage;
                    }

                    #endregion

                    #region send to GL

                    string glyear = "";
                    string glperiod = "";
                    #region get period
                    var period = _db.GLPeriods.Where(x => x.DateFrom <= record.PaymentDate.Date)
                        .Where(x => x.DateTo >= record.PaymentDate.Date).ToList();
                    if (period.Any())
                    {

                        if (period.Count() == 1)
                        {
                            var myPeriod = period.FirstOrDefault();
                            if (myPeriod != null)
                            {
                                glperiod = myPeriod.Name;
                                glyear = myPeriod.GlFiscalYear.Name.ToString();
                                if (myPeriod.Status == FiscalYearStatus.Close)
                                {
                                    responseMessage = "The period " + glperiod +
                                                  " is closed,the record cannot be transfered to closed period.";
                                    return responseMessage;
                                }
                            }
                            else
                            {
                                responseMessage = "Period not found.";
                                return responseMessage;
                            }
                        }

                        else
                        {
                            responseMessage = "There are multiple periods defined that contain the payment date date.";
                            return responseMessage;
                        }

                    }
                    else
                    {
                        responseMessage = "There are no periods defined that contain the payment date.";
                        return responseMessage;
                    }

                    #endregion

                    if (record.ApMiscellaneousPaymentDistribution.Any())
                    {
                        GlRecordJournal journal = new GlRecordJournal
                        {
                            DateCreated = DateTime.Now,
                            CreatedBy = user,
                            JournalName = "AP Direct Payment",
                            Description = !String.IsNullOrEmpty(description) ? description : "AP Direct Payment",
                            FiscalYear = glyear,
                            PeriodName = glperiod,
                            CategoryName = "AP Misc Payment",
                            SourcesName = "AP",
                            JournalNumber = "APMPAY." + record.ApMiscellaneousPaymentId.ToString(),
                            SourceId = record.ApMiscellaneousPaymentId.ToString(),
                            Source = JournalSources.AP_MiscPayment.ToString(),
                            EffectiveDate = record.PaymentDate.Date,
                            OrgBranchName = record.OrgBranchName,
                            Status = "Approved",
                            BranchId = record.CreatedAtBranchId,
                            BusinessUnitId = record.CreatedAtBusinessUnitId,
                            Remark = record.PaymentReference,
                            PaymentReference = record.PaymentReference,
                            ReferenceDoc = record.Remark,
                            InvoiceReferenceDoc = "",
                            DebitSum = debit,
                            IsOnlineApproved = true,
                            OnlineApprovedBy = user,
                            OnlineApprovedTime = DateTime.Now
                        };
                        _db.GLRecordJournals.Add(journal);
                        _db.SaveChanges();
                        foreach (var i in record.ApMiscellaneousPaymentDistribution)
                        {
                            #region debit and credit

                            var journalDetail = new GLRecordJournalDetail
                            {
                                DateCreated = DateTime.Now,
                                CreatedBy = user,
                                GLRecordJournal = journal,
                                AccountCode = i.AccountCode,
                                AccountDesc = i.AccountDesc,
                                AdditionalDescription = i.AdditionalDescription,
                                Debit = i.Debit,
                                Credit = i.Credit
                            };
                            _db.GLRecordJournalDetails.Add(journalDetail);
                            _db.SaveChanges();

                            #endregion
                        }
                        #region split journal
                        foreach (var glRecordJournalDetail in journal.GLRecordJournalDetail)
                        {
                            helper.SplitJournal(glRecordJournalDetail.GlRecordJournalDetailId);
                        }
                        #endregion
                        record.IsOnlineTransferred = true;
                        record.IsOnlineTransferredBy = user;
                        record.OnlineTransferredTime = DateTime.Now;
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                        responseMessage = "SUCCESS";
                        return responseMessage;
                    }

                    responseMessage = "There is no distribution to be transferred.";
                    return responseMessage;

                    #endregion
                }
                return responseMessage;
            }
        }

        public static string Settlement(int id, string description, string user)
        {
            using (ExceedDbContext _db = new ExceedDbContext())
            {
                AccountHelper helper = new AccountHelper();
                string responseMessage = "";

                var record = _db.ApPaymentSettlements.Find(id);

                if (record != null && record.IsOnlineApproved && !record.IsOnlineTransferred)
                {
                    #region check if already transffered 
                    var exist = _db.GLRecordJournals
                                   .Where(x => x.Source == JournalSources.DirectPaymentSettlement.ToString())
                                   .Where(x => x.SourceId == record.ApPaymentSettlementId.ToString())
                                   .FirstOrDefault();
                    if (exist != null)
                        return "The record is already transffered, it can't be retransferred.";

                    #endregion
                    #region check segment length validation
                    AccountHelper accountHelper = new AccountHelper();
                    foreach (var dist in record.ApPaymentSettlementDistributions)
                    {
                        string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
                        if (!String.IsNullOrEmpty(message))
                            return message;
                    }


                    #endregion

                    #region check if debit and credit are equal

                    decimal debit = 0;
                    decimal credit = 0;
                    var distribution = record.ApPaymentSettlementDistributions.ToList();
                    if (distribution.Any())
                    {
                        debit = distribution.Sum(x => x.Debit);
                        credit = distribution.Sum(x => x.Credit);
                    }

                    if (debit != credit)
                    {
                        responseMessage = "Total debit and total credit is not balanced.";
                        return responseMessage;
                    }

                    if (debit <= 0)
                    {
                        responseMessage = "Total debit and total credit should be positive number.";
                        return responseMessage;
                    }

                    #endregion

                    #region send to GL

                    string glyear = "";
                    string glperiod = "";
                    #region get period
                    var period = _db.GLPeriods.Where(x => x.DateFrom <= record.PaymentDate.Date)
                        .Where(x => x.DateTo >= record.PaymentDate.Date).ToList();
                    if (period.Any())
                    {

                        if (period.Count() == 1)
                        {
                            var myPeriod = period.FirstOrDefault();
                            if (myPeriod != null)
                            {
                                glperiod = myPeriod.Name;
                                glyear = myPeriod.GlFiscalYear.Name.ToString();
                                if (myPeriod.Status == FiscalYearStatus.Close)
                                {
                                    responseMessage = "The period " + glperiod +
                                                  " is closed,the record cannot be transfered to closed period.";
                                    return responseMessage;
                                }
                            }
                            else
                            {
                                responseMessage = "Period not found.";
                                return responseMessage;
                            }
                        }

                        else
                        {
                            responseMessage = "There are multiple periods defined that contain the payment date date.";
                            return responseMessage;
                        }

                    }
                    else
                    {
                        responseMessage = "There are no periods defined that contain the payment date.";
                        return responseMessage;
                    }

                    #endregion

                    if (record.ApPaymentSettlementDistributions.Any())
                    {
                        GlRecordJournal journal = new GlRecordJournal
                        {
                            DateCreated = DateTime.Now,
                            CreatedBy = user,
                            JournalName = "AP Direct Payment",
                            Description = !String.IsNullOrEmpty(description) ? description : "AP Direct Payment",
                            FiscalYear = glyear,
                            PeriodName = glperiod,
                            CategoryName = "AP Direct Payment Settlement",
                            SourcesName = "AP",
                            JournalNumber = "APDPAYSETTLE." + record.ApPaymentSettlementId.ToString(),
                            SourceId = record.ApPaymentSettlementId.ToString(),
                            Source = JournalSources.DirectPaymentSettlement.ToString(),
                            EffectiveDate = record.PaymentDate.Date,
                            OrgBranchName = record.OrgBranchName,
                            Status = "Approved",
                            BranchId = record.CreatedAtBranchId,
                            BusinessUnitId = record.CreatedAtBusinessUnitId,
                            Remark = record.Remark,
                            PaymentReference = record.Reference,
                            ReferenceDoc = record.Remark,
                            InvoiceReferenceDoc = "",
                            DebitSum = debit,
                            IsOnlineApproved = true,
                            OnlineApprovedBy = user,
                            OnlineApprovedTime = DateTime.Now
                        };
                        _db.GLRecordJournals.Add(journal);
                        _db.SaveChanges();
                        foreach (var i in record.ApPaymentSettlementDistributions)
                        {
                            #region debit and credit

                            var journalDetail = new GLRecordJournalDetail
                            {
                                DateCreated = DateTime.Now,
                                CreatedBy = user,
                                GLRecordJournal = journal,
                                AccountCode = i.AccountCode,
                                AccountDesc = i.AccountDesc,
                                AdditionalDescription = i.AdditionalDescription,
                                Debit = i.Debit,
                                Credit = i.Credit
                            };
                            _db.GLRecordJournalDetails.Add(journalDetail);
                            _db.SaveChanges();

                            #endregion
                        }
                        #region split journal
                        foreach (var glRecordJournalDetail in journal.GLRecordJournalDetail)
                        {
                            helper.SplitJournal(glRecordJournalDetail.GlRecordJournalDetailId);
                        }
                        #endregion
                        record.IsOnlineTransferred = true;
                        record.IsOnlineTransferredBy = user;
                        record.OnlineTransferredTime = DateTime.Now;
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                        responseMessage = "SUCCESS";
                        return responseMessage;
                    }

                    responseMessage = "There is no distribution to be transferred.";
                    return responseMessage;

                    #endregion
                }
                return responseMessage;
            }
        }
        #endregion
    }
}
