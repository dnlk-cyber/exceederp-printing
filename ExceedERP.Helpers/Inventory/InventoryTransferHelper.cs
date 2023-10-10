using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.DataAccess.Migrations;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;

namespace ExceedERP.Helpers.Inventory
{
    public  class InventoryTransferHelper
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        AccountHelper helper = new AccountHelper();

        public  string GoodReceive(int id, string description,string user)
        {
            string responseMessage = "";
            var record = _db.GoodReceives.Find(id);

            if (record != null && record.IsOnlineApproved && !record.IsOnlineTransferred)
            {
                #region check if already transffered 
                var exist = _db.GLRecordJournals
                               .Where(x => x.Source == JournalSources.Stock_Receive.ToString())
                               .Where(x => x.SourceId == record.GoodReceiveId.ToString())
                               .FirstOrDefault();
                if (exist != null)
                    return "The record is already transffered, it can't be retransferred.";

                #endregion
                #region check segment length validation
                AccountHelper accountHelper = new AccountHelper();
                foreach (var dist in record.GoodReceiveDistribution)
                {
                    string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
                    if (!String.IsNullOrEmpty(message))
                        return message;
                }


                #endregion

                #region check if debit and credit are equal

                decimal debit = 0;
                decimal credit = 0;
                var distribution = record.GoodReceiveDistribution.ToList();
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

                string glperiod = "";
                string glyear = "";

                #region get period

                var period = _db.InventoryPeriods.Find(record.GLPeriodId);


                if (period != null)
                {
                    var glperiodObj = _db.GLPeriods.FirstOrDefault(x => x.Name == period.Name);

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
                        responseMessage = $"The corresponding GL period {period.Name} is not found.";
                        return responseMessage;

                    }
                }
                else
                {
                    responseMessage = $"The period is not found in inventory period.";
                    return responseMessage;
                }

                #endregion

                if (record.GoodReceiveDistribution.Any())
                {
                    GlRecordJournal journal = new GlRecordJournal
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = user,
                        BusinessUnitId = record.BusinessUnitId,
                        BranchId = record.BranchId,
                        JournalName = "GoodReceive",
                        Description = !String.IsNullOrEmpty(description) ? description : "GoodReceive",
                        FiscalYear = glyear,
                        PeriodName = glperiod,
                        CategoryName = "GoodReceive",
                        SourcesName = "GR",
                        JournalNumber = "GoodRecive." + record.GoodReceiveId,
                        SourceId = record.GoodReceiveId.ToString(),
                        Source = JournalSources.Stock_Receive.ToString(),
                        EffectiveDate = record.ReceiveDate.Date.Date,
                        Status = "Approved",
                        IsOnlineApproved = true,
                        OnlineApprovedBy = user,
                        OnlineApprovedTime = DateTime.Now,
                        OrgBranchName = record.OrgBranchName
                    };
                    _db.GLRecordJournals.Add(journal);
                    _db.SaveChanges();
                    foreach (GoodReceiveDistribution i in record.GoodReceiveDistribution)
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
                    record.IsOnlineTransferredBy =user;
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
        public string Issue(int id, string description, string user)
        {
            string responseMessage = "";
            //try
            //{
               
                var record = _db.Issues.Find(id);

                if (record != null && record.IsOnlineApproved && !record.IsOnlineTransferred)
                {
                #region check if already transffered 
                var exist = _db.GLRecordJournals
                               .Where(x => x.Source == JournalSources.Stock_Issue.ToString())
                               .Where(x => x.SourceId == record.IssueID.ToString())
                               .FirstOrDefault();
                if (exist != null)
                    return "The record is already transffered, it can't be retransferred.";

                #endregion
                #region check segment length validation
                AccountHelper accountHelper = new AccountHelper();
                    foreach (var dist in record.IssueDistribution)
                        {
                            string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
                            if (!String.IsNullOrEmpty(message))
                                return message;
                        }


                    #endregion

                    #region check if debit and credit are equal

                    decimal debit = 0;
                    decimal credit = 0;
                    var issueDistributions = _db.IssueDistributions.Where(x=>x.IssueID==id).AsNoTracking().ToList();
                    if (issueDistributions.Any())
                    {
                        debit = issueDistributions.Sum(x => x.Debit);
                        credit = issueDistributions.Sum(x => x.Credit);
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

                string glperiod = "";
                    string glyear = "";

                    #region get period

                    var period = _db.InventoryPeriods.Find(record.GLPeriodId);


                    if (period != null)
                    {
                        var glperiodObj = _db.GLPeriods.FirstOrDefault(x => x.Name == period.Name);

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
                            responseMessage = $"The corresponding GL period {period.Name} is not found.";
                            return responseMessage;

                        }
                    }
                    else
                    {
                        responseMessage = $"The period is not found in inventory period.";
                        return responseMessage;
                    }

                    #endregion

                    if (issueDistributions.Any())
                    {
                        GlRecordJournal journal = new GlRecordJournal
                        {
                            DateCreated = DateTime.Now,
                            CreatedBy = user,
                            BusinessUnitId = record.BusinessUnitId,
                            BranchId = record.BranchId,
                            JournalName = "Issue",
                            Description = !String.IsNullOrEmpty(description) ? description : "Issue",
                            FiscalYear = glyear,
                            PeriodName = glperiod,
                            CategoryName = "Issue",
                            SourcesName = "Issue",
                            JournalNumber = "Issue." + record.IssueID,
                            ReferenceDoc=record.Reference,
                            SourceId = record.IssueID.ToString(),
                            Source = JournalSources.Stock_Issue.ToString(),
                            EffectiveDate = record.IssueDate.Date.Date,
                            Status = "Approved",
                            IsOnlineApproved = true,
                            OnlineApprovedBy = user,
                            OnlineApprovedTime = DateTime.Now,
                            OrgBranchName = record.OrgBranchName
                        };
                        _db.GLRecordJournals.Add(journal);
                        _db.SaveChanges();
                        foreach (var i in issueDistributions)
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
           // }
            //catch (Exception e)
            //{
            //    responseMessage = e.Message;
            //    Logger.Error("Issue Transfer==> ID: " +id + "User: " + user + "Desc: " + description + "Messsage: " + e.Message + "\n StackTrace: " + e.StackTrace + "\n TargetSite: " + e.TargetSite);
            //}
           // return responseMessage;
        }
        public  string IssueFuel(int id, string description, string user)
        {
            string responseMessage = "";
            var record = _db.Issues.Find(id);

            if (record != null && record.IsOnlineApproved && !record.IsOnlineTransferred)
            {
                #region check if already transffered 
                var exist = _db.GLRecordJournals
                               .Where(x => x.Source == JournalSources.Stock_Issue_fuel.ToString())
                               .Where(x => x.SourceId == record.IssueID.ToString())
                               .FirstOrDefault();
                if (exist != null)
                    return "The record is already transffered, it can't be retransferred.";

                #endregion
                #region check segment length validation

                AccountHelper accountHelper = new AccountHelper();

                foreach (var dist in record.IssueDistribution)
                {
                    string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
                    if (!String.IsNullOrEmpty(message))
                        return message;
                }


                #endregion

                #region check if debit and credit are equal

                decimal debit = 0;
                decimal credit = 0;
                var distribution = record.IssueDistribution.ToList();
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

                string glperiod = "";
                string glyear = "";

                #region get period

                var period = _db.InventoryPeriods.Find(record.GLPeriodId);


                if (period != null)
                {
                    var glperiodObj = _db.GLPeriods.FirstOrDefault(x => x.Name == period.Name);

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
                        responseMessage = $"The corresponding GL period {period.Name} is not found.";
                        return responseMessage;

                    }
                }
                else
                {
                    responseMessage = $"The period is not found in inventory period.";
                    return responseMessage;
                }

                #endregion

                if (record.IssueDistribution.Any())
                {
                    GlRecordJournal journal = new GlRecordJournal
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = user,
                        BusinessUnitId = record.BusinessUnitId,
                        BranchId = record.BranchId,
                        JournalName = "Issue Fuel",
                        Description = !String.IsNullOrEmpty(description) ? description : "Issue Fuel",
                        FiscalYear = glyear,
                        PeriodName = glperiod,
                        CategoryName = "Issue Fuel",
                        SourcesName = "Issue Fuel",
                        JournalNumber = "Issue Fuel." + record.IssueID,
                        SourceId = record.IssueID.ToString(),
                        Source = JournalSources.Stock_Issue_fuel.ToString(),
                        ReferenceDoc = record.Reference,
                        EffectiveDate = record.IssueDate.Date,
                        Status = "Approved",
                        IsOnlineApproved = true,
                        OnlineApprovedBy = user,
                        OnlineApprovedTime = DateTime.Now,
                        OrgBranchName = record.OrgBranchName
                    };
                    _db.GLRecordJournals.Add(journal);
                    _db.SaveChanges();
                    foreach (var i in record.IssueDistribution)
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

        public string GoodSelling(int id, string description, string user)
        {
            string responseMessage = "";
            var record = _db.GoodSellingAndLendings.Find(id);

            if (record != null && record.IsOnlineApproved && !record.IsOnlineTransferred)
            {
                #region check if already transffered 
                var exist = _db.GLRecordJournals
                               .Where(x => x.Source == JournalSources.Stock_SaleOrLend.ToString())
                               .Where(x => x.SourceId == record.GoodSellingAndLendingID.ToString())
                               .FirstOrDefault();
                if (exist != null)
                    return "The record is already transffered, it can't be retransferred.";

                #endregion
                #region check segment length validation

                AccountHelper accountHelper = new AccountHelper();

                foreach (var dist in record.GoodSellingAndLendingDistribution)
                {
                    string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
                    if (!String.IsNullOrEmpty(message))
                        return message;
                }


                #endregion

                #region check if debit and credit are equal

                decimal debit = 0;
                decimal credit = 0;
                var distribution = record.GoodSellingAndLendingDistribution.ToList();
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

                string glperiod = "";
                string glyear = "";

                #region get period

                var period = _db.InventoryPeriods.Find(record.GLPeriodId);


                if (period != null)
                {
                    var glperiodObj = _db.GLPeriods.FirstOrDefault(x => x.Name == period.Name);

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
                        responseMessage = $"The corresponding GL period {period.Name} is not found.";
                        return responseMessage;

                    }
                }
                else
                {
                    responseMessage = $"The period is not found in inventory period.";
                    return responseMessage;
                }

                #endregion

                if (record.GoodSellingAndLendingDistribution.Any())
                {
                    GlRecordJournal journal = new GlRecordJournal
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = user,
                        BusinessUnitId = record.BusinessUnitId,
                        BranchId = record.BranchId,
                        JournalName = "Good Selling",
                        Description = !String.IsNullOrEmpty(description) ? description : "Good Selling",
                        FiscalYear = glyear,
                        PeriodName = glperiod,
                        CategoryName = "Good Selling",
                        SourcesName = "Good Selling",
                        JournalNumber = "Good Selling." + record.GoodSellingAndLendingID,
                        SourceId = record.GoodSellingAndLendingID.ToString(),
                        Source = JournalSources.Stock_SaleOrLend.ToString(),
                        ReferenceDoc = record.Remark,
                        EffectiveDate = record.Date.Date,
                        Status = "Approved",
                        IsOnlineApproved = true,
                        OnlineApprovedBy = user,
                        OnlineApprovedTime = DateTime.Now,
                        OrgBranchName = record.OrgBranchName
                    };
                    _db.GLRecordJournals.Add(journal);
                    _db.SaveChanges();
                    foreach (var i in record.GoodSellingAndLendingDistribution)
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

        public  string TransferSend(int id, string description, string user)
        {
            string responseMessage = "";
            var record = _db.ItemTransfers.Find(id);

            if (record != null && record.IsOnlineApproved && !record.IsOnlineTransferred)
            {
                #region check if already transffered 
                var exist = _db.GLRecordJournals
                               .Where(x => x.Source == JournalSources.Stock_Transfer.ToString())
                               .Where(x => x.SourceId == record.ItemTransferID.ToString())
                               .FirstOrDefault();
                if (exist != null)
                    return "The record is already transffered, it can't be retransferred.";

                #endregion
                #region check segment length validation
                AccountHelper accountHelper = new AccountHelper();
                foreach (var dist in record.ItemTransferDistribution)
                {
                    string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
                    if (!String.IsNullOrEmpty(message))
                        return message;
                }


                #endregion

                #region check if debit and credit are equal

                decimal debit = 0;
                decimal credit = 0;
                var distribution = record.ItemTransferDistribution.ToList();
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

                string glperiod = "";
                string glyear = "";

                #region get period

                var period = _db.InventoryPeriods.Find(record.GLPeriodId);


                if (period != null)
                {
                    var glperiodObj = _db.GLPeriods.FirstOrDefault(x => x.Name == period.Name);

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
                        responseMessage = $"The corresponding GL period {period.Name} is not found.";
                        return responseMessage;

                    }
                }
                else
                {
                    responseMessage = $"The period is not found in inventory period.";
                    return responseMessage;
                }

                #endregion

                if (record.ItemTransferDistribution.Any())
                {
                    GlRecordJournal journal = new GlRecordJournal
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = user,
                        BusinessUnitId =record.BusinessUnitId,
                        BranchId = record.BranchId,                       
                        JournalName = "Transfer Send",
                        Description = !String.IsNullOrEmpty(description) ? description : "Transfer Send",
                        FiscalYear = glyear,
                        PeriodName = glperiod,
                        CategoryName = "Transfer Send",
                        SourcesName = "Transfer Send",
                        JournalNumber = "Transfer Send." + record.ItemTransferID,
                        SourceId = record.ItemTransferID.ToString(),
                        Source = JournalSources.Stock_Transfer.ToString(),
                        ReferenceDoc = record.DocumentRef,
                        EffectiveDate = record.TransferDate.Date,
                        Status = "Approved",
                        IsOnlineApproved = true,
                        OnlineApprovedBy = user,
                        OnlineApprovedTime = DateTime.Now,
                        OrgBranchName = record.OrgBranchName
                    };
                    _db.GLRecordJournals.Add(journal);
                    _db.SaveChanges();
                    foreach (var i in record.ItemTransferDistribution)
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
        public  string TransferReceive(int id, string description, string user)
        {
            string responseMessage = "";
            var record = _db.ItemTransferReceive.Find(id);

            if (record != null && record.IsOnlineApproved && !record.IsOnlineTransferred)
            {
                #region check if already transffered 
                var exist = _db.GLRecordJournals
                               .Where(x => x.Source == JournalSources.Stock_Transfer_Receive.ToString())
                               .Where(x => x.SourceId == record.ItemTransferReceiveId.ToString())
                               .FirstOrDefault();
                if (exist != null)
                    return "The record is already transffered, it can't be retransferred.";

                #endregion
                #region check segment length validation
                AccountHelper accountHelper = new AccountHelper();
                foreach (var dist in record.ItemTransferReceiveDistribution)
                {
                    string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
                    if (!String.IsNullOrEmpty(message))
                        return message;
                }


                #endregion

                #region check if debit and credit are equal

                decimal debit = 0;
                decimal credit = 0;
                var distribution = record.ItemTransferReceiveDistribution.ToList();
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

                string glperiod = "";
                string glyear = "";

                #region get period

                var period = _db.InventoryPeriods.Find(record.GLPeriodId);


                if (period != null)
                {
                    var glperiodObj = _db.GLPeriods.FirstOrDefault(x => x.Name == period.Name);

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
                        responseMessage = $"The corresponding GL period {period.Name} is not found.";
                        return responseMessage;

                    }
                }
                else
                {
                    responseMessage = $"The period is not found in inventory period.";
                    return responseMessage;
                }

                #endregion

                if (record.ItemTransferReceiveDistribution.Any())
                {
                    GlRecordJournal journal = new GlRecordJournal
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = user,
                        BusinessUnitId = record.BusinessUnitId,
                        BranchId = record.BranchId,
                        JournalName = "Transfer Receive",
                        Description = !String.IsNullOrEmpty(description) ? description : "Transfer Receive",
                        FiscalYear = glyear,
                        PeriodName = glperiod,
                        CategoryName = "Transfer Receive",
                        SourcesName = "Transfer Receive",
                        JournalNumber = "Transfer Receive." + record.ItemTransferID,
                        SourceId = record.ItemTransferReceiveId.ToString(),
                        Source = JournalSources.Stock_Transfer_Receive.ToString(),
                        ReferenceDoc = record.DocumentRef,
                        EffectiveDate = record.ReceiveDate.Date,
                        Status = "Approved",
                        IsOnlineApproved = true,
                        OnlineApprovedBy = user,
                        OnlineApprovedTime = DateTime.Now,
                        OrgBranchName = record.OrgBranchName
                    };
                    _db.GLRecordJournals.Add(journal);
                    _db.SaveChanges();
                    foreach (var i in record.ItemTransferReceiveDistribution)
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
        public  string StoreReturn(int id, string description, string user)
        {
            string responseMessage = "";
            var record = _db.StoreReturns.Find(id);

            if (record != null && record.IsOnlineApproved && !record.IsOnlineTransferred)
            {
                #region check if already transffered 
                var exist = _db.GLRecordJournals
                               .Where(x => x.Source == JournalSources.Stock_Return.ToString())
                               .Where(x => x.SourceId == record.StoreReturnID.ToString())
                               .FirstOrDefault();
                if (exist != null)
                    return "The record is already transffered, it can't be retransferred.";

                #endregion
                #region check segment length validation
                AccountHelper accountHelper = new AccountHelper();
                foreach (var dist in record.StoreReturnDistribution)
                {
                    string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
                    if (!String.IsNullOrEmpty(message))
                        return message;
                }


                #endregion

                #region check if debit and credit are equal

                decimal debit = 0;
                decimal credit = 0;
                var distribution = record.StoreReturnDistribution.ToList();
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

                string glperiod = "";
                string glyear = "";

                #region get period

                var period = _db.InventoryPeriods.Find(record.GLPeriodId);


                if (period != null)
                {
                    var glperiodObj = _db.GLPeriods.FirstOrDefault(x => x.Name == period.Name);

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
                        responseMessage = $"The corresponding GL period {period.Name} is not found.";
                        return responseMessage;

                    }
                }
                else
                {
                    responseMessage = $"The period is not found in inventory period.";
                    return responseMessage;
                }

                #endregion

                if (record.StoreReturnDistribution.Any())
                {
                    GlRecordJournal journal = new GlRecordJournal
                    {
                        DateCreated = DateTime.Now,
                        BusinessUnitId = record.BusinessUnitId,
                        BranchId = record.BranchId,
                        CreatedBy = user,
                        JournalName = "Store Return",
                        Description = !String.IsNullOrEmpty(description) ? description : "Store Return",
                        FiscalYear = glyear,
                        PeriodName = glperiod,
                        CategoryName = "Store Return",
                        SourcesName = "Store Return",
                        JournalNumber = "Store Return." + record.StoreReturnID,
                        SourceId = record.StoreReturnID.ToString(),
                        Source = JournalSources.Stock_Return.ToString(),
                        ReferenceDoc = record.Reference,
                        EffectiveDate = record.ReturnDate.Date,
                        Status = "Approved",
                        IsOnlineApproved = true,
                        OnlineApprovedBy = user,
                        OnlineApprovedTime = DateTime.Now,
                        OrgBranchName = record.OrgBranchName
                    };
                    _db.GLRecordJournals.Add(journal);
                    _db.SaveChanges();
                    foreach (var i in record.StoreReturnDistribution)
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
}
