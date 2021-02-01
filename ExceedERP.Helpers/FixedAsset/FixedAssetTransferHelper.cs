using System;
using System.Data.Entity;
using System.Linq;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;

namespace ExceedERP.Helpers.FixedAsset
{
    public  class FixedAssetTransferHelper
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        readonly AccountHelper _helper = new AccountHelper();

        public  string GoodReceive(int id, string description,string user)
        {
            string responseMessage = "";
            var record = _db.FixedAssetReceives.Find(id);

            if (record != null && record.IsOnlineApproved && !record.IsOnlineTransferred)
            {
                #region check segment length validation
                AccountHelper accountHelper = new AccountHelper();
                foreach (var dist in record.FixedAssetReceiveDistribution)
                {
                    string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
                    if (!String.IsNullOrEmpty(message))
                        return message;
                }


                #endregion

                #region check if debit and credit are equal

                decimal debit = 0;
                decimal credit = 0;
                var distribution = record.FixedAssetReceiveDistribution.ToList();
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

                if (debit == 0)
                {
                    responseMessage = "Total debit and total credit is zero.";
                    return responseMessage;
                }

                #endregion

                #region send to GL

                string glperiod = "";
                string glyear = "";

                #region get period

                var period = _db.FixedAssetPeriods.Find(record.GLPeriodId);


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

                if (record.FixedAssetReceiveDistribution.Any())
                {
                    GlRecordJournal journal = new GlRecordJournal
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = user,
                        JournalName = "FixedAssetReceive",
                        Description = !String.IsNullOrEmpty(description) ? description : "FixedAssetReceive",
                        FiscalYear = glyear,
                        PeriodName = glperiod,
                        CategoryName = "FixedAssetReceive",
                        SourcesName = "GR",
                        JournalNumber = "FixedAssetReceive." + record.FixedAssetReceiveId,
                        SourceId = record.FixedAssetReceiveId.ToString(),
                        Source = JournalSources.PPE_Receive.ToString(),
                        EffectiveDate = record.ReceiveDate.Date.Date,
                        Status = "Approved",
                        IsOnlineApproved = true,
                        OnlineApprovedBy = user,
                        OnlineApprovedTime = DateTime.Now,
                        OrgBranchName = record.OrgBranchName
                    };
                    _db.GLRecordJournals.Add(journal);
                    _db.SaveChanges();
                    foreach (var i in record.FixedAssetReceiveDistribution)
                    {
                        #region debit and credit

                        var journalDetail = new GLRecordJournalDetail
                        {
                            DateCreated = DateTime.Now,
                            CreatedBy = user,
                            GLRecordJournal = journal,
                            AccountCode = i.AccountCode,
                            AccountDesc = i.AccountDesc,
                            AdditionalDescription ="",
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
                        _helper.SplitJournal(glRecordJournalDetail.GlRecordJournalDetailId);
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
        //public string Issue(int id, string description, string user)
        //{
        //    string responseMessage = "";
        //    //try
        //    //{
               
        //        var record = _db.Issues.Find(id);

        //        if (record != null && record.IsOnlineApproved && !record.IsOnlineTransferred)
        //        {
        //            #region check segment length validation
        //             AccountHelper accountHelper = new AccountHelper();
        //            foreach (var dist in record.IssueDistribution)
        //                {
        //                    string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
        //                    if (!String.IsNullOrEmpty(message))
        //                        return message;
        //                }


        //            #endregion

        //            #region check if debit and credit are equal

        //            decimal debit = 0;
        //            decimal credit = 0;
        //            var issueDistributions = _db.IssueDistributions.Where(x=>x.IssueID==id).AsNoTracking().ToList();
        //            if (issueDistributions.Any())
        //            {
        //                debit = issueDistributions.Sum(x => x.Debit);
        //                credit = issueDistributions.Sum(x => x.Credit);
        //            }

        //            if (debit != credit)
        //            {
        //                responseMessage = "Total debit and total credit is not balanced.";
        //                return responseMessage;
        //            }

        //            if (debit == 0)
        //            {
        //                responseMessage = "Total debit and total credit is zero.";
        //                return responseMessage;
        //            }

        //            #endregion

        //            #region send to GL

        //            string glperiod = "";
        //            string glyear = "";

        //            #region get period

        //            var period = _db.InventoryPeriods.Find(record.GLPeriodId);


        //            if (period != null)
        //            {
        //                var glperiodObj = _db.GLPeriods.FirstOrDefault(x => x.Name == period.Name);

        //                if (glperiodObj != null)
        //                {
        //                    glperiod = glperiodObj.Name;
        //                    glyear = glperiodObj.GlFiscalYear.Name;
        //                    if (glperiodObj.Status == FiscalYearStatus.Close)
        //                    {
        //                        responseMessage =
        //                            $"The period {glperiodObj.Name} is closed,the record cannot be transfered to closed period.";
        //                        return responseMessage;
        //                    }
        //                }

        //                else
        //                {
        //                    responseMessage = $"The corresponding GL period {period.Name} is not found.";
        //                    return responseMessage;

        //                }
        //            }
        //            else
        //            {
        //                responseMessage = $"The period is not found in inventory period.";
        //                return responseMessage;
        //            }

        //            #endregion

        //            if (issueDistributions.Any())
        //            {
        //                GlRecordJournal journal = new GlRecordJournal
        //                {
        //                    DateCreated = DateTime.Now,
        //                    CreatedBy = user,
        //                    JournalName = "Issue",
        //                    Description = !String.IsNullOrEmpty(description) ? description : "Issue",
        //                    FiscalYear = glyear,
        //                    PeriodName = glperiod,
        //                    CategoryName = "Issue",
        //                    SourcesName = "Issue",
        //                    JournalNumber = "Issue." + record.IssueID,
        //                    EffectiveDate = record.IssueDate.Date.Date,
        //                    Status = "Approved",
        //                    IsOnlineApproved = true,
        //                    OnlineApprovedBy = user,
        //                    OnlineApprovedTime = DateTime.Now,
        //                    OrgBranchName = record.OrgBranchName
        //                };
        //                _db.GLRecordJournals.Add(journal);
        //                _db.SaveChanges();
        //                foreach (var i in issueDistributions)
        //                {
        //                    #region debit and credit

        //                    var journalDetail = new GLRecordJournalDetail
        //                    {
        //                        DateCreated = DateTime.Now,
        //                        CreatedBy = user,
        //                        GLRecordJournal = journal,
        //                        AccountCode = i.AccountCode,
        //                        AccountDesc = i.AccountDesc,
        //                        AdditionalDescription = i.AdditionalDescription,
        //                        Debit = i.Debit,
        //                        Credit = i.Credit
        //                    };
        //                    _db.GLRecordJournalDetails.Add(journalDetail);
        //                    _db.SaveChanges();

        //                    #endregion
        //                }
        //                #region split journal
        //                foreach (var glRecordJournalDetail in journal.GLRecordJournalDetail)
        //                {
        //                    _helper.SplitJournal(glRecordJournalDetail.GlRecordJournalDetailId);
        //                }
        //                #endregion

        //                record.IsOnlineTransferred = true;
        //                record.IsOnlineTransferredBy = user;
        //                record.OnlineTransferredTime = DateTime.Now;
        //                _db.Entry(record).State = EntityState.Modified;
        //                _db.SaveChanges();
        //                responseMessage = "SUCCESS";
        //                return responseMessage;
        //            }

        //            responseMessage = "There is no distribution to be transferred.";
        //            return responseMessage;

        //            #endregion
        //        }
        //        return responseMessage;
        //   // }
        //    //catch (Exception e)
        //    //{
        //    //    responseMessage = e.Message;
        //    //    Logger.Error("Issue Transfer==> ID: " +id + "User: " + user + "Desc: " + description + "Messsage: " + e.Message + "\n StackTrace: " + e.StackTrace + "\n TargetSite: " + e.TargetSite);
        //    //}
        //   // return responseMessage;
        //}
        //public  string IssueFuel(int id, string description, string user)
        //{
        //    string responseMessage = "";
        //    var record = _db.Issues.Find(id);

        //    if (record != null && record.IsOnlineApproved && !record.IsOnlineTransferred)
        //    {
        //        #region check segment length validation

        //        AccountHelper accountHelper = new AccountHelper();

        //        foreach (var dist in record.IssueDistribution)
        //        {
        //            string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
        //            if (!String.IsNullOrEmpty(message))
        //                return message;
        //        }


        //        #endregion

        //        #region check if debit and credit are equal

        //        decimal debit = 0;
        //        decimal credit = 0;
        //        var distribution = record.IssueDistribution.ToList();
        //        if (distribution.Any())
        //        {
        //            debit = distribution.Sum(x => x.Debit);
        //            credit = distribution.Sum(x => x.Credit);
        //        }

        //        if (debit != credit)
        //        {
        //            responseMessage = "Total debit and total credit is not balanced.";
        //            return responseMessage;
        //        }

        //        if (debit == 0)
        //        {
        //            responseMessage = "Total debit and total credit is zero.";
        //            return responseMessage;
        //        }

        //        #endregion

        //        #region send to GL

        //        string glperiod = "";
        //        string glyear = "";

        //        #region get period

        //        var period = _db.InventoryPeriods.Find(record.GLPeriodId);


        //        if (period != null)
        //        {
        //            var glperiodObj = _db.GLPeriods.FirstOrDefault(x => x.Name == period.Name);

        //            if (glperiodObj != null)
        //            {
        //                glperiod = glperiodObj.Name;
        //                glyear = glperiodObj.GlFiscalYear.Name;
        //                if (glperiodObj.Status == FiscalYearStatus.Close)
        //                {
        //                    responseMessage =
        //                        $"The period {glperiodObj.Name} is closed,the record cannot be transfered to closed period.";
        //                    return responseMessage;
        //                }
        //            }

        //            else
        //            {
        //                responseMessage = $"The corresponding GL period {period.Name} is not found.";
        //                return responseMessage;

        //            }
        //        }
        //        else
        //        {
        //            responseMessage = $"The period is not found in inventory period.";
        //            return responseMessage;
        //        }

        //        #endregion

        //        if (record.IssueDistribution.Any())
        //        {
        //            GlRecordJournal journal = new GlRecordJournal
        //            {
        //                DateCreated = DateTime.Now,
        //                CreatedBy = user,
        //                JournalName = "Issue Fuel",
        //                Description = !String.IsNullOrEmpty(description) ? description : "Issue Fuel",
        //                FiscalYear = glyear,
        //                PeriodName = glperiod,
        //                CategoryName = "Issue Fuel",
        //                SourcesName = "Issue Fuel",
        //                JournalNumber = "Issue Fuel." + record.IssueID,
        //                EffectiveDate = record.IssueDate.Date,
        //                Status = "Approved",
        //                IsOnlineApproved = true,
        //                OnlineApprovedBy = user,
        //                OnlineApprovedTime = DateTime.Now,
        //                OrgBranchName = record.OrgBranchName
        //            };
        //            _db.GLRecordJournals.Add(journal);
        //            _db.SaveChanges();
        //            foreach (var i in record.IssueDistribution)
        //            {
        //                #region debit and credit

        //                var journalDetail = new GLRecordJournalDetail
        //                {
        //                    DateCreated = DateTime.Now,
        //                    CreatedBy = user,
        //                    GLRecordJournal = journal,
        //                    AccountCode = i.AccountCode,
        //                    AccountDesc = i.AccountDesc,
        //                    AdditionalDescription = i.AdditionalDescription,
        //                    Debit = i.Debit,
        //                    Credit = i.Credit
        //                };
        //                _db.GLRecordJournalDetails.Add(journalDetail);
        //                _db.SaveChanges();

        //                #endregion
        //            }

        //            #region split journal
        //            foreach (var glRecordJournalDetail in journal.GLRecordJournalDetail)
        //            {
        //                _helper.SplitJournal(glRecordJournalDetail.GlRecordJournalDetailId);
        //            }
        //            #endregion
        //            record.IsOnlineTransferred = true;
        //            record.IsOnlineTransferredBy = user;
        //            record.OnlineTransferredTime = DateTime.Now;
        //            _db.Entry(record).State = EntityState.Modified;
        //            _db.SaveChanges();
        //            responseMessage = "SUCCESS";
        //            return responseMessage;
        //        }

        //        responseMessage = "There is no distribution to be transferred.";
        //        return responseMessage;

        //        #endregion
        //    }
        //    return responseMessage;
        //}
        //public  string TransferSend(int id, string description, string user)
        //{
        //    string responseMessage = "";
        //    var record = _db.ItemTransfers.Find(id);

        //    if (record != null && record.IsOnlineApproved && !record.IsOnlineTransferred)
        //    {
        //        #region check segment length validation
        //        AccountHelper accountHelper = new AccountHelper();
        //        foreach (var dist in record.ItemTransferDistribution)
        //        {
        //            string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
        //            if (!String.IsNullOrEmpty(message))
        //                return message;
        //        }


        //        #endregion

        //        #region check if debit and credit are equal

        //        decimal debit = 0;
        //        decimal credit = 0;
        //        var distribution = record.ItemTransferDistribution.ToList();
        //        if (distribution.Any())
        //        {
        //            debit = distribution.Sum(x => x.Debit);
        //            credit = distribution.Sum(x => x.Credit);
        //        }

        //        if (debit != credit)
        //        {
        //            responseMessage = "Total debit and total credit is not balanced.";
        //            return responseMessage;
        //        }

        //        if (debit == 0)
        //        {
        //            responseMessage = "Total debit and total credit is zero.";
        //            return responseMessage;
        //        }

        //        #endregion

        //        #region send to GL

        //        string glperiod = "";
        //        string glyear = "";

        //        #region get period

        //        var period = _db.InventoryPeriods.Find(record.GLPeriodId);


        //        if (period != null)
        //        {
        //            var glperiodObj = _db.GLPeriods.FirstOrDefault(x => x.Name == period.Name);

        //            if (glperiodObj != null)
        //            {
        //                glperiod = glperiodObj.Name;
        //                glyear = glperiodObj.GlFiscalYear.Name;
        //                if (glperiodObj.Status == FiscalYearStatus.Close)
        //                {
        //                    responseMessage =
        //                        $"The period {glperiodObj.Name} is closed,the record cannot be transfered to closed period.";
        //                    return responseMessage;
        //                }
        //            }

        //            else
        //            {
        //                responseMessage = $"The corresponding GL period {period.Name} is not found.";
        //                return responseMessage;

        //            }
        //        }
        //        else
        //        {
        //            responseMessage = $"The period is not found in inventory period.";
        //            return responseMessage;
        //        }

        //        #endregion

        //        if (record.ItemTransferDistribution.Any())
        //        {
        //            GlRecordJournal journal = new GlRecordJournal
        //            {
        //                DateCreated = DateTime.Now,
        //                CreatedBy = user,
        //                JournalName = "Transfer Send",
        //                Description = !String.IsNullOrEmpty(description) ? description : "Transfer Send",
        //                FiscalYear = glyear,
        //                PeriodName = glperiod,
        //                CategoryName = "Transfer Send",
        //                SourcesName = "Transfer Send",
        //                JournalNumber = "Transfer Send." + record.ItemTransferID,
        //                EffectiveDate = record.TransferDate.Date,
        //                Status = "Approved",
        //                IsOnlineApproved = true,
        //                OnlineApprovedBy = user,
        //                OnlineApprovedTime = DateTime.Now,
        //                OrgBranchName = record.OrgBranchName
        //            };
        //            _db.GLRecordJournals.Add(journal);
        //            _db.SaveChanges();
        //            foreach (var i in record.ItemTransferDistribution)
        //            {
        //                #region debit and credit

        //                var journalDetail = new GLRecordJournalDetail
        //                {
        //                    DateCreated = DateTime.Now,
        //                    CreatedBy = user,
        //                    GLRecordJournal = journal,
        //                    AccountCode = i.AccountCode,
        //                    AccountDesc = i.AccountDesc,
        //                    AdditionalDescription = i.AdditionalDescription,
        //                    Debit = i.Debit,
        //                    Credit = i.Credit
        //                };
        //                _db.GLRecordJournalDetails.Add(journalDetail);
        //                _db.SaveChanges();

        //                #endregion
        //            }

        //            #region split journal
        //            foreach (var glRecordJournalDetail in journal.GLRecordJournalDetail)
        //            {
        //                _helper.SplitJournal(glRecordJournalDetail.GlRecordJournalDetailId);
        //            }
        //            #endregion
        //            record.IsOnlineTransferred = true;
        //            record.IsOnlineTransferredBy = user;
        //            record.OnlineTransferredTime = DateTime.Now;
        //            _db.Entry(record).State = EntityState.Modified;
        //            _db.SaveChanges();
        //            responseMessage = "SUCCESS";
        //            return responseMessage;
        //        }

        //        responseMessage = "There is no distribution to be transferred.";
        //        return responseMessage;

        //        #endregion
        //    }
        //    return responseMessage;
        //}
        //public  string TransferReceive(int id, string description, string user)
        //{
        //    string responseMessage = "";
        //    var record = _db.ItemTransferReceive.Find(id);

        //    if (record != null && record.IsOnlineApproved && !record.IsOnlineTransferred)
        //    {
        //        #region check segment length validation
        //        AccountHelper accountHelper = new AccountHelper();
        //        foreach (var dist in record.ItemTransferReceiveDistribution)
        //        {
        //            string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
        //            if (!String.IsNullOrEmpty(message))
        //                return message;
        //        }


        //        #endregion

        //        #region check if debit and credit are equal

        //        decimal debit = 0;
        //        decimal credit = 0;
        //        var distribution = record.ItemTransferReceiveDistribution.ToList();
        //        if (distribution.Any())
        //        {
        //            debit = distribution.Sum(x => x.Debit);
        //            credit = distribution.Sum(x => x.Credit);
        //        }

        //        if (debit != credit)
        //        {
        //            responseMessage = "Total debit and total credit is not balanced.";
        //            return responseMessage;
        //        }

        //        if (debit == 0)
        //        {
        //            responseMessage = "Total debit and total credit is zero.";
        //            return responseMessage;
        //        }

        //        #endregion

        //        #region send to GL

        //        string glperiod = "";
        //        string glyear = "";

        //        #region get period

        //        var period = _db.InventoryPeriods.Find(record.GLPeriodId);


        //        if (period != null)
        //        {
        //            var glperiodObj = _db.GLPeriods.FirstOrDefault(x => x.Name == period.Name);

        //            if (glperiodObj != null)
        //            {
        //                glperiod = glperiodObj.Name;
        //                glyear = glperiodObj.GlFiscalYear.Name;
        //                if (glperiodObj.Status == FiscalYearStatus.Close)
        //                {
        //                    responseMessage =
        //                        $"The period {glperiodObj.Name} is closed,the record cannot be transfered to closed period.";
        //                    return responseMessage;
        //                }
        //            }

        //            else
        //            {
        //                responseMessage = $"The corresponding GL period {period.Name} is not found.";
        //                return responseMessage;

        //            }
        //        }
        //        else
        //        {
        //            responseMessage = $"The period is not found in inventory period.";
        //            return responseMessage;
        //        }

        //        #endregion

        //        if (record.ItemTransferReceiveDistribution.Any())
        //        {
        //            GlRecordJournal journal = new GlRecordJournal
        //            {
        //                DateCreated = DateTime.Now,
        //                CreatedBy = user,
        //                JournalName = "Transfer Receive",
        //                Description = !String.IsNullOrEmpty(description) ? description : "Transfer Receive",
        //                FiscalYear = glyear,
        //                PeriodName = glperiod,
        //                CategoryName = "Transfer Receive",
        //                SourcesName = "Transfer Receive",
        //                JournalNumber = "Transfer Receive." + record.ItemTransferID,
        //                EffectiveDate = record.ReceiveDate.Date,
        //                Status = "Approved",
        //                IsOnlineApproved = true,
        //                OnlineApprovedBy = user,
        //                OnlineApprovedTime = DateTime.Now,
        //                OrgBranchName = record.OrgBranchName
        //            };
        //            _db.GLRecordJournals.Add(journal);
        //            _db.SaveChanges();
        //            foreach (var i in record.ItemTransferReceiveDistribution)
        //            {
        //                #region debit and credit

        //                var journalDetail = new GLRecordJournalDetail
        //                {
        //                    DateCreated = DateTime.Now,
        //                    CreatedBy = user,
        //                    GLRecordJournal = journal,
        //                    AccountCode = i.AccountCode,
        //                    AccountDesc = i.AccountDesc,
        //                    AdditionalDescription = i.AdditionalDescription,
        //                    Debit = i.Debit,
        //                    Credit = i.Credit
        //                };
        //                _db.GLRecordJournalDetails.Add(journalDetail);
        //                _db.SaveChanges();

        //                #endregion
        //            }

        //            #region split journal
        //            foreach (var glRecordJournalDetail in journal.GLRecordJournalDetail)
        //            {
        //                _helper.SplitJournal(glRecordJournalDetail.GlRecordJournalDetailId);
        //            }
        //            #endregion
        //            record.IsOnlineTransferred = true;
        //            record.IsOnlineTransferredBy = user;
        //            record.OnlineTransferredTime = DateTime.Now;
        //            _db.Entry(record).State = EntityState.Modified;
        //            _db.SaveChanges();
        //            responseMessage = "SUCCESS";
        //            return responseMessage;
        //        }

        //        responseMessage = "There is no distribution to be transferred.";
        //        return responseMessage;

        //        #endregion
        //    }
        //    return responseMessage;
        //}
        //public  string StoreReturn(int id, string description, string user)
        //{
        //    string responseMessage = "";
        //    var record = _db.StoreReturns.Find(id);

        //    if (record != null && record.IsOnlineApproved && !record.IsOnlineTransferred)
        //    {
        //        #region check segment length validation
        //        AccountHelper accountHelper = new AccountHelper();
        //        foreach (var dist in record.StoreReturnDistribution)
        //        {
        //            string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
        //            if (!String.IsNullOrEmpty(message))
        //                return message;
        //        }


        //        #endregion

        //        #region check if debit and credit are equal

        //        decimal debit = 0;
        //        decimal credit = 0;
        //        var distribution = record.StoreReturnDistribution.ToList();
        //        if (distribution.Any())
        //        {
        //            debit = distribution.Sum(x => x.Debit);
        //            credit = distribution.Sum(x => x.Credit);
        //        }

        //        if (debit != credit)
        //        {
        //            responseMessage = "Total debit and total credit is not balanced.";
        //            return responseMessage;
        //        }

        //        if (debit == 0)
        //        {
        //            responseMessage = "Total debit and total credit is zero.";
        //            return responseMessage;
        //        }

        //        #endregion

        //        #region send to GL

        //        string glperiod = "";
        //        string glyear = "";

        //        #region get period

        //        var period = _db.InventoryPeriods.Find(record.GLPeriodId);


        //        if (period != null)
        //        {
        //            var glperiodObj = _db.GLPeriods.FirstOrDefault(x => x.Name == period.Name);

        //            if (glperiodObj != null)
        //            {
        //                glperiod = glperiodObj.Name;
        //                glyear = glperiodObj.GlFiscalYear.Name;
        //                if (glperiodObj.Status == FiscalYearStatus.Close)
        //                {
        //                    responseMessage =
        //                        $"The period {glperiodObj.Name} is closed,the record cannot be transfered to closed period.";
        //                    return responseMessage;
        //                }
        //            }

        //            else
        //            {
        //                responseMessage = $"The corresponding GL period {period.Name} is not found.";
        //                return responseMessage;

        //            }
        //        }
        //        else
        //        {
        //            responseMessage = $"The period is not found in inventory period.";
        //            return responseMessage;
        //        }

        //        #endregion

        //        if (record.StoreReturnDistribution.Any())
        //        {
        //            GlRecordJournal journal = new GlRecordJournal
        //            {
        //                DateCreated = DateTime.Now,
        //                CreatedBy = user,
        //                JournalName = "Store Return",
        //                Description = !String.IsNullOrEmpty(description) ? description : "Store Return",
        //                FiscalYear = glyear,
        //                PeriodName = glperiod,
        //                CategoryName = "Store Return",
        //                SourcesName = "Store Return",
        //                JournalNumber = "Store Return." + record.StoreReturnID,
        //                EffectiveDate = record.ReturnDate.Date,
        //                Status = "Approved",
        //                IsOnlineApproved = true,
        //                OnlineApprovedBy = user,
        //                OnlineApprovedTime = DateTime.Now,
        //                OrgBranchName = record.OrgBranchName
        //            };
        //            _db.GLRecordJournals.Add(journal);
        //            _db.SaveChanges();
        //            foreach (var i in record.StoreReturnDistribution)
        //            {
        //                #region debit and credit

        //                var journalDetail = new GLRecordJournalDetail
        //                {
        //                    DateCreated = DateTime.Now,
        //                    CreatedBy = user,
        //                    GLRecordJournal = journal,
        //                    AccountCode = i.AccountCode,
        //                    AccountDesc = i.AccountDesc,
        //                    AdditionalDescription = i.AdditionalDescription,
        //                    Debit = i.Debit,
        //                    Credit = i.Credit
        //                };
        //                _db.GLRecordJournalDetails.Add(journalDetail);
        //                _db.SaveChanges();

        //                #endregion
        //            }

        //            #region split journal
        //            foreach (var glRecordJournalDetail in journal.GLRecordJournalDetail)
        //            {
        //                _helper.SplitJournal(glRecordJournalDetail.GlRecordJournalDetailId);
        //            }
        //            #endregion
        //            record.IsOnlineTransferred = true;
        //            record.IsOnlineTransferredBy = user;
        //            record.OnlineTransferredTime = DateTime.Now;
        //            _db.Entry(record).State = EntityState.Modified;
        //            _db.SaveChanges();
        //            responseMessage = "SUCCESS";
        //            return responseMessage;
        //        }

        //        responseMessage = "There is no distribution to be transferred.";
        //        return responseMessage;

        //        #endregion
        //    }
        //    return responseMessage;
        //}
    }
}
