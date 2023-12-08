using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;
using System;
using System.Data.Entity;
using System.Linq;

namespace ExceedERP.Helpers.CRM
{
    public class CRMTransferHelper
    {
        public bool InvoiceJournalExists(int invoiceId)
        {
            var db = new ExceedDbContext();
            return db.GLRecordJournals.Any(r => r.SourcesName == "CRM" && r.JournalNumber == "CRMSALINV." + invoiceId);
        }
        public bool ShippmentJournalExists(int shipmentId)
        {
            var db = new ExceedDbContext();
            return db.GLRecordJournals.Any(r => r.SourcesName == "CRM" && r.JournalNumber == "CRMSHIPMENT." + shipmentId);
        }
        public string CashSalesInvoice(int invoiceId, string description, string userName)
        {
            var db = new ExceedDbContext();
            string suc = "SUCCESS";
            //string err = "ERROR";
            string responseMessage = "";
            var record = db.CashSalesInvoices.Find(invoiceId);
            var journalExists = InvoiceJournalExists(invoiceId);
            if (journalExists)
            {
                if(record != null)
                {
                    record.IsOnlineTransferred = true;

                    db.CashSalesInvoices.Attach(record);
                    db.Entry(record).State = EntityState.Modified;
                    db.SaveChanges();
                }
                responseMessage = $"CRM Sales Invoice Journal with Journal number CRMSALINV.{invoiceId} exists already";
                return responseMessage;
            }
            if (record != null && !record.IsOnlineApproved)
            {
                //var formattedData = new
                //{
                //    Message = "The record is not approved.",
                //    Status = "NOT_OK"
                //};
                responseMessage = "The record is not approved";
                return responseMessage;
            }
            if (record != null && record.IsOnlineTransferred)
            {
                #region check if already transffered 

                responseMessage = "The record is already transffered, it can't be retransferred.";
                return responseMessage;
                #endregion

            }
            if (record != null && !record.IsOnlineTransferred)
            {

                #region check segment length validation
                AccountHelper accountHelper = new AccountHelper();
                var distributions = db.SalesInvoicePaymentDistibutions.Where(d => d.CashSalesInvoiceId == record.CashSalesInvoiceID).ToList();
                foreach (var dist in distributions)
                {
                    string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
                    if (!String.IsNullOrEmpty(message))
                    {

                        responseMessage = message;
                        return responseMessage;
                    }

                    #region debit credit validation
                    if (dist.Debit != 0 && dist.Credit != 0)
                    {
                        //message = $"{dist.AccountCode} Account has both debit and credit value.";
                        //var formattedData = new
                        //{
                        //    Message = message,
                        //    Status = "NOT_OK"
                        //};
                        responseMessage = $"{dist.AccountCode} Account has both debit and credit value.";
                        return responseMessage;
                    }

                    if (dist.Debit == 0 && dist.Credit == 0)
                    {
                        //message = $"{dist.AccountCode} Both debit and credit can't have zero value at the same time.";
                        //var formattedData = new
                        //{
                        //    Message = message,
                        //    Status = "NOT_OK"
                        //};
                        responseMessage = $"{dist.AccountCode} Both debit and credit can't have zero value at the same time.";
                        return responseMessage;
                    }

                    if (dist.Debit > 0 && dist.Credit > 0)
                    {
                        //message = $"{dist.AccountCode} Both debit and credit can't have non zero value at the same time.";
                        //var formattedData = new
                        //{
                        //    Message = message,
                        //    Status = "NOT_OK"
                        //};
                        responseMessage = $"{dist.AccountCode} Both debit and credit can't have non zero value at the same time.";
                        return responseMessage;
                    }

                    if (dist.Debit < 0 || dist.Credit < 0)
                    {
                        //message = $"{dist.AccountCode} Negative values are not allowed.";
                        //var formattedData = new
                        //{
                        //    Message = message,
                        //    Status = "NOT_OK"
                        //};
                        responseMessage = $"{dist.AccountCode} Negative values are not allowed.";
                        return responseMessage;
                    }
                    #endregion

                }


                #endregion
                #region check if debit and credit are equal
                decimal debit = 0;
                decimal credit = 0;
                //var distribution = record.ArSalesOrderDistribution.ToList();
                if (distributions.Any())
                {
                    debit = distributions.Sum(x => x.Debit);
                    credit = distributions.Sum(x => x.Credit);
                }
                if (debit != credit)
                {
                    //var formattedData = new
                    //{
                    //    Message = "Total debit and total credit is not balanced.",
                    //    Status = "NOT_OK"
                    //};
                    responseMessage = "Total debit and total credit is not balanced.";
                    return responseMessage;
                }
                if (debit <= 0)
                {
                    //var formattedData = new
                    //{
                    //    Message = "Total debit and total credit is zero.",
                    //    Status = "NOT_OK"
                    //};
                    responseMessage = "Total debit and total credit should be positive number.";
                    return responseMessage;
                }
                #endregion

                #region send to GL

                string glyear = "";
                string glperiod = "";
                #region get period

                var crmPeriod = db.CRMPeriods.Find(record.CRMPeriodId);
                //var period = _db.InventoryPeriods.Find(record.GLPeriodId);


                if (crmPeriod != null)
                {
                    var glperiodObj = db.GLPeriods.FirstOrDefault(x => x.Name == crmPeriod.Name);

                    if (glperiodObj != null)
                    {
                        glperiod = glperiodObj.Name;
                        glyear = glperiodObj.GlFiscalYear.Name;
                        if (glperiodObj.Status == FiscalYearStatus.Close)
                        {
                            responseMessage =
                                $"The period {glperiodObj.Name} is closed,the record cannot be transfered to closed period.";
                            return responseMessage;
                        }
                    }

                    else
                    {
                        responseMessage = $"The corresponding GL period {crmPeriod.Name} is not found.";
                        return responseMessage;

                    }
                }
                else
                {
                    responseMessage = $"The period is not found in inventory period.";
                    return responseMessage;
                }

                #endregion

                if (distributions.Any())
                {
                    GlRecordJournal journal = new GlRecordJournal
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = userName,
                        JournalName = "CRM Cash Sales Invoice",
                        Description = !String.IsNullOrEmpty(description) ? description : "CRM Cash Sales Invoice",
                        FiscalYear = glyear,
                        PeriodName = glperiod,
                        CategoryName = "CRM Cash Sales Invoice",
                        SourcesName = "CRM",
                        JournalNumber = "CRMSALINV." + record.CashSalesInvoiceID.ToString(),
                        SourceId = record.CashSalesInvoiceID.ToString(),
                        Source = JournalSources.Sales.ToString(),
                        EffectiveDate = record.Date.Date,
                        OrgBranchName = record.OrgBranchName,
                        BranchId = record.BranchId,
                        Status = "Approved",
                        Remark = record.Remark,
                        PaymentReference = record.ReceiptReference,
                        ReferenceDoc = $"FS Invoice No. {record.FsInvoiceNo}",
                        InvoiceReferenceDoc = $"Invoice No. {record.InvoiceNumber}",

                        IsOnlineApproved = true,
                        OnlineApprovedBy = userName,
                        OnlineApprovedTime = DateTime.Now
                    };
                    db.GLRecordJournals.Add(journal);
                    db.SaveChanges();
                    GLRecordJournalDetail journalDetail;
                    foreach (var i in distributions)
                    {
                        #region debit and credit

                        journalDetail = new GLRecordJournalDetail
                        {
                            DateCreated = DateTime.Now,
                            CreatedBy = userName,
                            GLRecordJournal = journal,
                            AccountCode = i.AccountCode,
                            Debit = i.Debit,
                            Credit = i.Credit,
                            AccountDesc = i.AccountDesc,
                            AdditionalDescription = i.AdditionalDescription,
                        };
                        db.GLRecordJournalDetails.Add(journalDetail);
                        db.SaveChanges();
                        #endregion
                    }
                    #region update record
                    record.IsOnlineTransferred = true;
                    record.IsOnlineTransferredBy = userName;
                    record.OnlineTransferredTime = DateTime.Now;
                    db.Entry(record).State = EntityState.Modified;
                    db.SaveChanges();
                    #endregion

                    #region split journal
                    var journaldetails = db.GLRecordJournalDetails
                        .Where(x => x.GlRecordJournalId == journal.GlRecordJournalId).ToList();
                    foreach (var detail in journaldetails)
                    {
                        AccountHelper helper = new AccountHelper();
                        helper.SplitJournal(detail.GlRecordJournalDetailId);
                    }
                    #endregion
                    //var formattedData = new
                    //{
                    //    Message = "Successfully transfered.",
                    //    Status = "OK"
                    //};
                    //return Json(formattedData, JsonRequestBehavior.AllowGet);
                    #region bank reconciliation
                    //CRMSalesHelper.SendSalesToBankReconciliation(record, userName);
                    #endregion
                    responseMessage = "SUCCESS";
                    return responseMessage;
                }

                responseMessage = "There is no distribution to be transferred.";
                return responseMessage;
                #endregion

            }
            else
            {
                //var formattedData = new
                //{
                //    Message = "You cannot transfer this transaction, it is already transfered.",
                //    Status = "NOT_OK"
                //};
                //return Json(formattedData, JsonRequestBehavior.AllowGet);
                responseMessage = "You cannot transfer this transaction, it is already transfered.";
                return responseMessage;
            }

            return responseMessage;
        }
        public string CreditSalesInvoice(int invoiceId, string description, string userName)
        {
            var db = new ExceedDbContext();
            string suc = "SUCCESS";
            //string err = "ERROR";
            string responseMessage = "";
            var record = db.CashSalesInvoices.Find(invoiceId);
            var journalExists = InvoiceJournalExists(invoiceId);
            if (journalExists)
            {
                if (record != null)
                {
                    record.IsOnlineTransferred = true;

                    db.CashSalesInvoices.Attach(record);
                    db.Entry(record).State = EntityState.Modified;
                    db.SaveChanges();
                }
                responseMessage = $"CRM Sales Invoice Journal with Journal number CRMSALINV.{invoiceId} exists already";
                return responseMessage;
            }
            if (record != null && !record.IsOnlineApproved)
            {
                //var formattedData = new
                //{
                //    Message = "The record is not approved.",
                //    Status = "NOT_OK"
                //};
                responseMessage = "The record is not approved";
                return responseMessage;
            }
            if (record != null && record.IsOnlineTransferred)
            {
                #region check if already transffered 

                responseMessage = "The record is already transffered, it can't be retransferred.";
                return responseMessage;

                #endregion

            }
            if (record != null && !record.IsOnlineTransferred)
            {

                #region check segment length validation
                AccountHelper accountHelper = new AccountHelper();
                var distributions = db.SalesInvoicePaymentDistibutions.Where(d => d.CashSalesInvoiceId == record.CashSalesInvoiceID).ToList();
                foreach (var dist in distributions)
                {
                    string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
                    if (!String.IsNullOrEmpty(message))
                    {

                        responseMessage = message;
                        return responseMessage;
                    }

                    #region debit credit validation
                    if (dist.Debit != 0 && dist.Credit != 0)
                    {
                        //message = $"{dist.AccountCode} Account has both debit and credit value.";
                        //var formattedData = new
                        //{
                        //    Message = message,
                        //    Status = "NOT_OK"
                        //};
                        responseMessage = $"{dist.AccountCode} Account has both debit and credit value.";
                        return responseMessage;
                    }

                    if (dist.Debit == 0 && dist.Credit == 0)
                    {
                        //message = $"{dist.AccountCode} Both debit and credit can't have zero value at the same time.";
                        //var formattedData = new
                        //{
                        //    Message = message,
                        //    Status = "NOT_OK"
                        //};
                        responseMessage = $"{dist.AccountCode} Both debit and credit can't have zero value at the same time.";
                        return responseMessage;
                    }

                    if (dist.Debit > 0 && dist.Credit > 0)
                    {
                        //message = $"{dist.AccountCode} Both debit and credit can't have non zero value at the same time.";
                        //var formattedData = new
                        //{
                        //    Message = message,
                        //    Status = "NOT_OK"
                        //};
                        responseMessage = $"{dist.AccountCode} Both debit and credit can't have non zero value at the same time.";
                        return responseMessage;
                    }

                    if (dist.Debit < 0 || dist.Credit < 0)
                    {
                        //message = $"{dist.AccountCode} Negative values are not allowed.";
                        //var formattedData = new
                        //{
                        //    Message = message,
                        //    Status = "NOT_OK"
                        //};
                        responseMessage = $"{dist.AccountCode} Negative values are not allowed.";
                        return responseMessage;
                    }
                    #endregion

                }


                #endregion
                #region check if debit and credit are equal
                decimal debit = 0;
                decimal credit = 0;
                //var distribution = record.ArSalesOrderDistribution.ToList();
                if (distributions.Any())
                {
                    debit = distributions.Sum(x => x.Debit);
                    credit = distributions.Sum(x => x.Credit);
                }
                if (debit != credit)
                {
                    //var formattedData = new
                    //{
                    //    Message = "Total debit and total credit is not balanced.",
                    //    Status = "NOT_OK"
                    //};
                    responseMessage = "Total debit and total credit is not balanced.";
                    return responseMessage;
                }
                if (debit <= 0)
                {
                    //var formattedData = new
                    //{
                    //    Message = "Total debit and total credit is zero.",
                    //    Status = "NOT_OK"
                    //};
                    responseMessage = "Total debit and total credit should be positive number.";
                    return responseMessage;
                }
                #endregion

                #region send to GL

                string glyear = "";
                string glperiod = "";
                #region get period

                var crmPeriod = db.CRMPeriods.Find(record.CRMPeriodId);
                //var period = _db.InventoryPeriods.Find(record.GLPeriodId);


                if (crmPeriod != null)
                {
                    var glperiodObj = db.GLPeriods.FirstOrDefault(x => x.Name == crmPeriod.Name);

                    if (glperiodObj != null)
                    {
                        glperiod = glperiodObj.Name;
                        glyear = glperiodObj.GlFiscalYear.Name;
                        if (glperiodObj.Status == FiscalYearStatus.Close)
                        {
                            responseMessage =
                                $"The period {glperiodObj.Name} is closed,the record cannot be transfered to closed period.";
                            return responseMessage;
                        }
                    }

                    else
                    {
                        responseMessage = $"The corresponding GL period {crmPeriod.Name} is not found.";
                        return responseMessage;

                    }
                }
                else
                {
                    responseMessage = $"The period is not found in inventory period.";
                    return responseMessage;
                }

                #endregion

                if (distributions.Any())
                {
                    GlRecordJournal journal = new GlRecordJournal
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = userName,
                        JournalName = "CRM Credit Sales Invoice",
                        Description = !String.IsNullOrEmpty(description) ? description : "CRM Credit Sales Invoice",
                        FiscalYear = glyear,
                        PeriodName = glperiod,
                        CategoryName = "CRM Credit Sales Invoice",
                        SourcesName = "CRM",
                        JournalNumber = "CRMSALINV." + record.CashSalesInvoiceID.ToString(),
                        SourceId = record.CashSalesInvoiceID.ToString(),
                        Source = JournalSources.Sales.ToString(),
                        EffectiveDate = record.Date.Date,
                        OrgBranchName = record.OrgBranchName,
                        BranchId = record.BranchId,
                        Status = "Approved",
                        Remark = record.Remark,
                        PaymentReference = record.ReceiptReference,
                        ReferenceDoc = $"FS Invoice No. {record.FsInvoiceNo}",
                        InvoiceReferenceDoc = $"Invoice No. {record.InvoiceNumber}",

                        IsOnlineApproved = true,
                        OnlineApprovedBy = userName,
                        OnlineApprovedTime = DateTime.Now
                    };
                    db.GLRecordJournals.Add(journal);
                    db.SaveChanges();
                    GLRecordJournalDetail journalDetail;
                    foreach (var i in distributions)
                    {
                        #region debit and credit

                        journalDetail = new GLRecordJournalDetail
                        {
                            DateCreated = DateTime.Now,
                            CreatedBy = userName,
                            GLRecordJournal = journal,
                            AccountCode = i.AccountCode,
                            Debit = i.Debit,
                            Credit = i.Credit,
                            AccountDesc = i.AccountDesc,
                            AdditionalDescription = i.AdditionalDescription,
                        };
                        db.GLRecordJournalDetails.Add(journalDetail);
                        db.SaveChanges();
                        #endregion
                    }
                    #region update record
                    record.IsOnlineTransferred = true;
                    record.IsOnlineTransferredBy = userName;
                    record.OnlineTransferredTime = DateTime.Now;
                    db.Entry(record).State = EntityState.Modified;
                    db.SaveChanges();
                    #endregion

                    #region split journal
                    var journaldetails = db.GLRecordJournalDetails
                        .Where(x => x.GlRecordJournalId == journal.GlRecordJournalId).ToList();
                    foreach (var detail in journaldetails)
                    {
                        AccountHelper helper = new AccountHelper();
                        helper.SplitJournal(detail.GlRecordJournalDetailId);
                    }
                    #endregion
                    //var formattedData = new
                    //{
                    //    Message = "Successfully transfered.",
                    //    Status = "OK"
                    //};
                    //return Json(formattedData, JsonRequestBehavior.AllowGet);
                    #region bank reconcilation
                    #region bank reconciliation
                    //CRMSalesHelper.SendSalesToBankReconciliation(record, userName);
                    #endregion
                    #endregion
                    responseMessage = "SUCCESS";
                    return responseMessage;
                }
                responseMessage = "There is no distribution to be transferred.";
                return responseMessage;
                #endregion

            }
            else
            {
                //var formattedData = new
                //{
                //    Message = "You cannot transfer this transaction, it is already transfered.",
                //    Status = "NOT_OK"
                //};
                //return Json(formattedData, JsonRequestBehavior.AllowGet);
                responseMessage = "You cannot transfer this transaction, it is already transfered.";
                return responseMessage;
            }

            return responseMessage;
        }

        public string Shipment(int invoiceId, string description, string userName)
        {
            var db = new ExceedDbContext();
            string responseMessage = "";
            var record = db.SalesShipments.Find(invoiceId);
            var journalExists = ShippmentJournalExists(invoiceId);
            if (journalExists)
            {
                if (record != null)
                {
                    record.IsOnlineTransferred = true;

                    db.SalesShipments.Attach(record);
                    db.Entry(record).State = EntityState.Modified;
                    db.SaveChanges();
                }
                responseMessage = $"CRM Shipment Journal with Journal number CRMSHIPMENT.{invoiceId} exists already";
                return responseMessage;
            }
            if (record != null && !record.IsOnlineApproved)
            {
                responseMessage = "The record is not approved";
                return responseMessage;
            }
            if (record != null && record.IsOnlineTransferred)
            {
                #region check if already transffered 
                responseMessage = "The record is already transffered, it can't be retransferred.";
                return responseMessage;
                #endregion
            }
            if (record != null && !record.IsOnlineTransferred)
            {
                #region check segment length validation
                AccountHelper accountHelper = new AccountHelper();
                var distributions = db.SalesShipmentDistribution.Where(d => d.SalesShipmentId == record.SalesShipmentId).ToList();
                foreach (var dist in distributions)
                {
                    string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
                    if (!String.IsNullOrEmpty(message))
                    {
                        responseMessage = message;
                        return responseMessage;
                    }

                    #region debit credit validation
                    if (dist.Debit != 0 && dist.Credit != 0)
                    {
                        responseMessage = $"{dist.AccountCode} Account has both debit and credit value.";
                        return responseMessage;
                    }

                    if (dist.Debit == 0 && dist.Credit == 0)
                    {
                        responseMessage = $"{dist.AccountCode} Both debit and credit can't have zero value at the same time.";
                        return responseMessage;
                    }

                    if (dist.Debit > 0 && dist.Credit > 0)
                    {
                        responseMessage = $"{dist.AccountCode} Both debit and credit can't have non zero value at the same time.";
                        return responseMessage;
                    }

                    if (dist.Debit < 0 || dist.Credit < 0)
                    {
                        responseMessage = $"{dist.AccountCode} Negative values are not allowed.";
                        return responseMessage;
                    }
                    #endregion

                }


                #endregion
                #region check if debit and credit are equal
                decimal debit = 0;
                decimal credit = 0;
                //var distribution = record.ArSalesOrderDistribution.ToList();
                if (distributions.Any())
                {
                    debit = distributions.Sum(x => x.Debit);
                    credit = distributions.Sum(x => x.Credit);
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

                var crmPeriod = db.CRMPeriods.Find(record.CRMPeriodId);

                if (crmPeriod != null)
                {
                    var glperiodObj = db.GLPeriods.FirstOrDefault(x => x.Name == crmPeriod.Name);

                    if (glperiodObj != null)
                    {
                        glperiod = glperiodObj.Name;
                        glyear = glperiodObj.GlFiscalYear.Name;
                        if (glperiodObj.Status == FiscalYearStatus.Close)
                        {
                            responseMessage =
                                $"The period {glperiodObj.Name} is closed,the record cannot be transfered to closed period.";
                            return responseMessage;
                        }
                    }

                    else
                    {
                        responseMessage = $"The corresponding GL period {crmPeriod.Name} is not found.";
                        return responseMessage;

                    }
                }
                else
                {
                    responseMessage = $"The period is not found in inventory period.";
                    return responseMessage;
                }

                #endregion

                if (distributions.Any())
                {
                    GlRecordJournal journal = new GlRecordJournal
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = userName,
                        JournalName = "CRM Shipment (Store Issue)",
                        Description = !String.IsNullOrEmpty(description) ? description : "CRM Shipment (Store Issue)",
                        FiscalYear = glyear,
                        PeriodName = glperiod,
                        CategoryName = "CRM Shipment (Store Issue)",
                        SourcesName = "CRM",
                        JournalNumber = "CRMSHIPMENT." + record.SalesShipmentId.ToString(),
                        SourceId = record.SalesShipmentId.ToString(),
                        Source = JournalSources.Sales.ToString(),
                        EffectiveDate = record.ShipmentDate.Date,
                        OrgBranchName = record.OrgBranchName,
                        BranchId = record.BranchId,
                        Status = "Approved",
                        Remark = record.Remark,
                        PaymentReference = record.SalesShipmentId.ToString(),
                        ReferenceDoc = $"FS Invoice No. {record.FsInvoiceNumber}",
                        InvoiceReferenceDoc = record.Source == ShipmentSource.SalesInvoice ? $"Invoice No. {record.CashSalesInvoiceID}" : $"Sales Order No. {record.SalesOrderVoucherID}",

                        IsOnlineApproved = true,
                        OnlineApprovedBy = userName,
                        OnlineApprovedTime = DateTime.Now
                    };
                    db.GLRecordJournals.Add(journal);
                    db.SaveChanges();
                    GLRecordJournalDetail journalDetail;
                    foreach (var i in distributions)
                    {
                        #region debit and credit

                        journalDetail = new GLRecordJournalDetail
                        {
                            DateCreated = DateTime.Now,
                            CreatedBy = userName,
                            GLRecordJournal = journal,
                            AccountCode = i.AccountCode,
                            Debit = i.Debit,
                            Credit = i.Credit,
                            AccountDesc = i.AccountDesc,
                            AdditionalDescription = i.AdditionalDescription,

                        };
                        db.GLRecordJournalDetails.Add(journalDetail);
                        db.SaveChanges();
                        #endregion
                    }
                    #region update record
                    record.IsOnlineTransferred = true;
                    record.IsOnlineTransferredBy = userName;
                    record.OnlineTransferredTime = DateTime.Now;
                    db.Entry(record).State = EntityState.Modified;
                    db.SaveChanges();
                    #endregion

                    #region split journal
                    var journaldetails = db.GLRecordJournalDetails
                        .Where(x => x.GlRecordJournalId == journal.GlRecordJournalId).ToList();
                    foreach (var detail in journaldetails)
                    {
                        AccountHelper helper = new AccountHelper();
                        helper.SplitJournal(detail.GlRecordJournalDetailId);
                    }
                    #endregion
                    //var formattedData = new
                    //{
                    //    Message = "Successfully transfered.",
                    //    Status = "OK"
                    //};
                    //return Json(formattedData, JsonRequestBehavior.AllowGet);
                    responseMessage = "SUCCESS";
                    return responseMessage;
                }
                responseMessage = "There is no distribution to be transferred.";
                return responseMessage;
                #endregion

            }
            else
            {
                //var formattedData = new
                //{
                //    Message = "You cannot transfer this transaction, it is already transfered.",
                //    Status = "NOT_OK"
                //};
                //return Json(formattedData, JsonRequestBehavior.AllowGet);
                responseMessage = "You cannot transfer this transaction, it is already transfered.";
                return responseMessage;
            }

            return responseMessage;
        }

        public string Deposit(int depositId, string description, string userName)
        {
            var db = new ExceedDbContext();
            string responseMessage = "";
            var record = db.CRMDeposit.Find(depositId);
            if (record != null && !record.IsOnlineApproved)
            {
                responseMessage = "The record is not approved";
                return responseMessage;
            }
            if (record != null && record.IsOnlineTransferred)
            {
                #region check if already transffered 
                responseMessage = "The record is already transffered, it can't be retransferred.";
                return responseMessage;
                #endregion
            }
            if (record != null && !record.IsOnlineTransferred)
            {
                #region check segment length validation
                AccountHelper accountHelper = new AccountHelper();
                var distributions = db.CRMDepositDistribution.Where(d => d.CRMDepositId == record.CRMDepositId).ToList();
                foreach (var dist in distributions)
                {
                    string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
                    if (!String.IsNullOrEmpty(message))
                    {
                        responseMessage = message;
                        return responseMessage;
                    }

                    #region debit credit validation
                    if (dist.Debit != 0 && dist.Credit != 0)
                    {
                        responseMessage = $"{dist.AccountCode} Account has both debit and credit value.";
                        return responseMessage;
                    }

                    if (dist.Debit == 0 && dist.Credit == 0)
                    {
                        responseMessage = $"{dist.AccountCode} Both debit and credit can't have zero value at the same time.";
                        return responseMessage;
                    }

                    if (dist.Debit > 0 && dist.Credit > 0)
                    {
                        responseMessage = $"{dist.AccountCode} Both debit and credit can't have non zero value at the same time.";
                        return responseMessage;
                    }

                    if (dist.Debit < 0 || dist.Credit < 0)
                    {
                        responseMessage = $"{dist.AccountCode} Negative values are not allowed.";
                        return responseMessage;
                    }
                    #endregion

                }


                #endregion
                #region check if debit and credit are equal
                decimal debit = 0;
                decimal credit = 0;
                //var distribution = record.ArSalesOrderDistribution.ToList();
                if (distributions.Any())
                {
                    debit = distributions.Sum(x => x.Debit);
                    credit = distributions.Sum(x => x.Credit);
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

                var crmPeriod = db.CRMPeriods.Find(record.CRMPeriodId);

                if (crmPeriod != null)
                {
                    var glperiodObj = db.GLPeriods.FirstOrDefault(x => x.Name == crmPeriod.Name);

                    if (glperiodObj != null)
                    {
                        glperiod = glperiodObj.Name;
                        glyear = glperiodObj.GlFiscalYear.Name;
                        if (glperiodObj.Status == FiscalYearStatus.Close)
                        {
                            responseMessage =
                                $"The period {glperiodObj.Name} is closed,the record cannot be transfered to closed period.";
                            return responseMessage;
                        }
                    }

                    else
                    {
                        responseMessage = $"The corresponding GL period {crmPeriod.Name} is not found.";
                        return responseMessage;

                    }
                }
                else
                {
                    responseMessage = $"The period is not found in inventory period.";
                    return responseMessage;
                }

                #endregion

                if (distributions.Any())
                {
                    GlRecordJournal journal = new GlRecordJournal
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = userName,
                        JournalName = "CRM Deposit",
                        Description = !String.IsNullOrEmpty(description) ? description : "CRM Deposit",
                        FiscalYear = glyear,
                        PeriodName = glperiod,
                        CategoryName = "CRM Deposit",
                        SourcesName = "CRM",
                        JournalNumber = "CRMDeposit." + record.CRMDepositId.ToString(),
                        SourceId = record.CRMDepositId.ToString(),
                        Source = JournalSources.Sales.ToString(),
                        EffectiveDate = record.Date.Date,
                        OrgBranchName = record.OrgBranchName,
                        BranchId = record.BranchId,
                        Status = "Approved",
                        Remark = record.SalesPersonUserName,
                        PaymentReference = "",
                        ReferenceDoc = "",
                        InvoiceReferenceDoc = "",
                        IsOnlineApproved = true,
                        OnlineApprovedBy = userName,
                        OnlineApprovedTime = DateTime.Now
                    };
                    db.GLRecordJournals.Add(journal);
                    db.SaveChanges();
                    GLRecordJournalDetail journalDetail;
                    foreach (var i in distributions)
                    {
                        #region debit and credit

                        journalDetail = new GLRecordJournalDetail
                        {
                            DateCreated = DateTime.Now,
                            CreatedBy = userName,
                            GLRecordJournal = journal,
                            AccountCode = i.AccountCode,
                            Debit = i.Debit,
                            Credit = i.Credit,
                            AccountDesc = i.AccountDesc,
                            AdditionalDescription = i.AdditionalDescription,

                        };
                        db.GLRecordJournalDetails.Add(journalDetail);
                        db.SaveChanges();
                        #endregion
                    }
                    #region update record
                    record.IsOnlineTransferred = true;
                    record.IsOnlineTransferredBy = userName;
                    record.OnlineTransferredTime = DateTime.Now;
                    db.Entry(record).State = EntityState.Modified;
                    db.SaveChanges();
                    #endregion

                    #region split journal
                    var journaldetails = db.GLRecordJournalDetails
                        .Where(x => x.GlRecordJournalId == journal.GlRecordJournalId).ToList();
                    foreach (var detail in journaldetails)
                    {
                        AccountHelper helper = new AccountHelper();
                        helper.SplitJournal(detail.GlRecordJournalDetailId);
                    }
                    #endregion
                    #region bank reconciliation
                    //CRMSalesHelper.SendDepositToBankReconciliation(record, userName);
                    #endregion
                    responseMessage = "SUCCESS";
                    return responseMessage;
                }
                responseMessage = "There is no distribution to be transferred.";
                return responseMessage;
                #endregion

            }
            else
            {

                responseMessage = "You cannot transfer this transaction, it is already transfered.";
                return responseMessage;
            }

            return responseMessage;
        }

        public string CreditSettlement(int depositId, string description, string userName)
        {
            var db = new ExceedDbContext();
            string responseMessage = "";
            var record = db.CreditSettlement.Find(depositId);
            if (record != null && !record.IsOnlineApproved)
            {
                responseMessage = "The record is not approved";
                return responseMessage;
            }
            if (record != null && record.IsOnlineTransferred)
            {
                #region check if already transffered 
                responseMessage = "The record is already transffered, it can't be retransferred.";
                return responseMessage;
                #endregion
            }
            if (record != null && !record.IsOnlineTransferred)
            {
                #region check segment length validation
                AccountHelper accountHelper = new AccountHelper();
                var distributions = db.CreditSettlementDistribution.Where(d => d.CreditSettlementId == record.CreditSettlementId).ToList();
                foreach (var dist in distributions)
                {
                    string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
                    if (!String.IsNullOrEmpty(message))
                    {
                        responseMessage = message;
                        return responseMessage;
                    }

                    #region debit credit validation
                    if (dist.Debit != 0 && dist.Credit != 0)
                    {
                        responseMessage = $"{dist.AccountCode} Account has both debit and credit value.";
                        return responseMessage;
                    }

                    if (dist.Debit == 0 && dist.Credit == 0)
                    {
                        responseMessage = $"{dist.AccountCode} Both debit and credit can't have zero value at the same time.";
                        return responseMessage;
                    }

                    if (dist.Debit > 0 && dist.Credit > 0)
                    {
                        responseMessage = $"{dist.AccountCode} Both debit and credit can't have non zero value at the same time.";
                        return responseMessage;
                    }

                    if (dist.Debit < 0 || dist.Credit < 0)
                    {
                        responseMessage = $"{dist.AccountCode} Negative values are not allowed.";
                        return responseMessage;
                    }
                    #endregion

                }


                #endregion
                #region check if debit and credit are equal
                decimal debit = 0;
                decimal credit = 0;
                //var distribution = record.ArSalesOrderDistribution.ToList();
                if (distributions.Any())
                {
                    debit = distributions.Sum(x => x.Debit);
                    credit = distributions.Sum(x => x.Credit);
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

                var crmPeriod = db.CRMPeriods.Find(record.CRMPeriodId);

                if (crmPeriod != null)
                {
                    var glperiodObj = db.GLPeriods.FirstOrDefault(x => x.Name == crmPeriod.Name);

                    if (glperiodObj != null)
                    {
                        glperiod = glperiodObj.Name;
                        glyear = glperiodObj.GlFiscalYear.Name;
                        if (glperiodObj.Status == FiscalYearStatus.Close)
                        {
                            responseMessage =
                                $"The period {glperiodObj.Name} is closed,the record cannot be transfered to closed period.";
                            return responseMessage;
                        }
                    }

                    else
                    {
                        responseMessage = $"The corresponding GL period {crmPeriod.Name} is not found.";
                        return responseMessage;

                    }
                }
                else
                {
                    responseMessage = $"The period is not found in inventory period.";
                    return responseMessage;
                }

                #endregion

                if (distributions.Any())
                {
                    GlRecordJournal journal = new GlRecordJournal
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = userName,
                        JournalName = "CRM Credit Settlement",
                        Description = !String.IsNullOrEmpty(description) ? description : "CRM Credit Settlement",
                        FiscalYear = glyear,
                        PeriodName = glperiod,
                        CategoryName = "CRM Credit Settlement",
                        SourcesName = "CRM",
                        JournalNumber = "CRMCrSettl." + record.CreditSettlementId.ToString(),
                        SourceId = record.CreditSettlementId.ToString(),
                        Source = JournalSources.Sales.ToString(),
                        EffectiveDate = record.Date.Date,
                        OrgBranchName = record.OrgBranchName,
                        BranchId = record.BranchId,
                        Status = "Approved",
                        Remark = record.Remark,
                        PaymentReference = "",
                        ReferenceDoc = "",
                        InvoiceReferenceDoc = "",
                        IsOnlineApproved = true,
                        OnlineApprovedBy = userName,
                        OnlineApprovedTime = DateTime.Now
                    };
                    db.GLRecordJournals.Add(journal);
                    db.SaveChanges();
                    GLRecordJournalDetail journalDetail;
                    foreach (var i in distributions)
                    {
                        #region debit and credit

                        journalDetail = new GLRecordJournalDetail
                        {
                            DateCreated = DateTime.Now,
                            CreatedBy = userName,
                            GLRecordJournal = journal,
                            AccountCode = i.AccountCode,
                            Debit = i.Debit,
                            Credit = i.Credit,
                            AccountDesc = i.AccountDesc,
                            AdditionalDescription = i.AdditionalDescription,

                        };
                        db.GLRecordJournalDetails.Add(journalDetail);
                        db.SaveChanges();
                        #endregion
                    }
                    #region update record
                    record.IsOnlineTransferred = true;
                    record.IsOnlineTransferredBy = userName;
                    record.OnlineTransferredTime = DateTime.Now;
                    db.Entry(record).State = EntityState.Modified;
                    db.SaveChanges();
                    #endregion

                    #region split journal
                    var journaldetails = db.GLRecordJournalDetails
                        .Where(x => x.GlRecordJournalId == journal.GlRecordJournalId).ToList();
                    foreach (var detail in journaldetails)
                    {
                        AccountHelper helper = new AccountHelper();
                        helper.SplitJournal(detail.GlRecordJournalDetailId);
                    }
                    #endregion
                    #region bank reconciliation
                    //CRMSalesHelper.SendCreditSettlementToBankReconciliation(record, userName);
                    #endregion
                    responseMessage = "SUCCESS";
                    return responseMessage;
                }
                responseMessage = "There is no distribution to be transferred.";
                return responseMessage;
                #endregion

            }
            else
            {

                responseMessage = "You cannot transfer this transaction, it is already transfered.";
                return responseMessage;
            }

            return responseMessage;
        }

    }
}
