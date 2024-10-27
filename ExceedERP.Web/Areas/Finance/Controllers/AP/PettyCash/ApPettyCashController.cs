using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.DocumentApproval;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AP.PettyCash;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AP.PettyCash
{
    public class ApPettyCashController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(FinanceConstants.AccountPayableView, FinanceConstants.AccountPayablePettyCashApprover, FinanceConstants.AccountPayablePettyCash)]
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
       

        private void PopulateViewData()
        {

            var cashierSetups = _db.CashierSetups;
            ViewData["cashiers"] = cashierSetups;

        }

        [AuthorizeRoles(FinanceConstants.AccountPayableView, FinanceConstants.AccountPayablePettyCashApprover, FinanceConstants.AccountPayablePettyCash)]
        public ActionResult ApPettyCash_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            IQueryable<ApPettyCash> appettycash = _db.ApPettyCash
                .Where(x => mybraches.Contains(x.OrgBranchName)); 
            DataSourceResult result = appettycash.ToDataSourceResult(request, apPettyCash => new {
                ApPettyCashId = apPettyCash.ApPettyCashId,
                PurposeOfPayment = apPettyCash.PurposeOfPayment,
                ApCashierSetupId = apPettyCash.ApCashierSetupId,
                Date = apPettyCash.Date,
                Reference = apPettyCash.Reference,
                CurrentBalance = apPettyCash.CurrentBalance,

                ForwardedFrom = apPettyCash.ForwardedFrom,
                ForwardedTo = apPettyCash.ForwardedTo,
                IsFinalApproval = apPettyCash.IsFinalApproval,

                Status = apPettyCash.Status,
                

                IsSendForApproval = apPettyCash.IsSendForApproval,
                SendForApprovalBy = apPettyCash.SendForApprovalBy,
                SendForApprovalTime = apPettyCash.SendForApprovalTime,

                IsOnlineApproved = apPettyCash.IsOnlineApproved,
                OnlineApprovedBy = apPettyCash.OnlineApprovedBy,
                OnlineApprovedTime = apPettyCash.OnlineApprovedTime,

                IsOnlineTransferred = apPettyCash.IsOnlineTransferred,
                IsOnlineTransferredBy = apPettyCash.IsOnlineTransferredBy,
                OnlineTransferredTime = apPettyCash.OnlineTransferredTime,

                IsVoid = apPettyCash.IsVoid,
                VoidBy = apPettyCash.VoidBy,
                VoidTime = apPettyCash.VoidTime,
              
                OrgBranchName = apPettyCash.OrgBranchName,
                DateCreated = apPettyCash.DateCreated,
                LastModified = apPettyCash.LastModified,
                CreatedBy = apPettyCash.CreatedBy,
                ModifiedBy = apPettyCash.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FinanceConstants.AccountPayablePettyCash)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApPettyCash_Create([DataSourceRequest]DataSourceRequest request, ApPettyCash apPettyCash)
        {
            if (!String.IsNullOrEmpty(apPettyCash.Reference))
            {
                var exists = _db.ApPettyCash.Where(x => x.Reference == apPettyCash.Reference && x.IsVoid == false).ToList();

                if (exists.Any())
                    ModelState.AddModelError("Reference", $"{apPettyCash.Reference}  already exists in the system.");
            }
            #region owner approval

            bool canOwnerApprove = DocApprovalHelper.CanOwnerApprove(ApprovalDocumentType.ApPettyCash);
            if (canOwnerApprove)
                apPettyCash.ForwardedTo = User.Identity.Name;

            #endregion
            if (ModelState.IsValid)
            {
                var entity = new ApPettyCash
                {
                    PurposeOfPayment = apPettyCash.PurposeOfPayment,
                    ApCashierSetupId = apPettyCash.ApCashierSetupId,
                    CurrentBalance = apPettyCash.CurrentBalance,
                    Date = apPettyCash.Date.Date,
                    Reference = apPettyCash.Reference,
                    IsOnlineTransferred = apPettyCash.IsOnlineTransferred,
                    IsOnlineTransferredBy = apPettyCash.IsOnlineTransferredBy,
                    OnlineTransferredTime = apPettyCash.OnlineTransferredTime,
                    ForwardedFrom = User.Identity.Name,
                    ForwardedTo = User.Identity.Name,
                    Status = apPettyCash.Status,
                    
                    IsOnlineApproved = apPettyCash.IsOnlineApproved,
                    OnlineApprovedBy = apPettyCash.OnlineApprovedBy,
                    OnlineApprovedTime = apPettyCash.OnlineApprovedTime,
                    IsVoid = apPettyCash.IsVoid,
                    VoidBy = apPettyCash.VoidBy,
                    VoidTime = apPettyCash.VoidTime,
                    IsSendForApproval = apPettyCash.IsSendForApproval,
                    SendForApprovalBy = apPettyCash.SendForApprovalBy,
                    SendForApprovalTime = apPettyCash.SendForApprovalTime,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    DateCreated = DateTime.Now,
                    LastModified = apPettyCash.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = apPettyCash.ModifiedBy
                };

                _db.ApPettyCash.Add(entity);
                _db.SaveChanges();
                apPettyCash.ApPettyCashId = entity.ApPettyCashId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AP petty cash", "Success", User.Identity.Name, $"created petty cash {entity.ApPettyCashId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { apPettyCash }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountPayablePettyCash)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApPettyCash_Update([DataSourceRequest]DataSourceRequest request, ApPettyCash apPettyCash)
        {
            if (!String.IsNullOrEmpty(apPettyCash.Reference))
            {
                var exists = _db.ApPettyCash
                    .Where(x=>x.ApPettyCashId!=apPettyCash.ApPettyCashId).Where(x => x.Reference == apPettyCash.Reference && x.IsVoid == false).ToList();

                if (exists.Any())
                    ModelState.AddModelError("Reference", $"{apPettyCash.Reference}  already exists in the system.");
            }
            if (ModelState.IsValid)
            {
                var entity = new ApPettyCash
                {
                    ApPettyCashId = apPettyCash.ApPettyCashId,
                    PurposeOfPayment = apPettyCash.PurposeOfPayment,
                    ApCashierSetupId = apPettyCash.ApCashierSetupId,
                    CurrentBalance = apPettyCash.CurrentBalance,
                    Date = apPettyCash.Date,
                    Reference = apPettyCash.Reference,
                    IsOnlineTransferred = apPettyCash.IsOnlineTransferred,
                    IsOnlineTransferredBy = apPettyCash.IsOnlineTransferredBy,
                    OnlineTransferredTime = apPettyCash.OnlineTransferredTime,
                    ForwardedFrom = apPettyCash.ForwardedFrom,
                    ForwardedTo = apPettyCash.ForwardedTo,
                    Status = apPettyCash.Status,
                    
                    IsOnlineApproved = apPettyCash.IsOnlineApproved,
                    OnlineApprovedBy = apPettyCash.OnlineApprovedBy,
                    OnlineApprovedTime = apPettyCash.OnlineApprovedTime,
                    IsVoid = apPettyCash.IsVoid,
                    VoidBy = apPettyCash.VoidBy,
                    VoidTime = apPettyCash.VoidTime,
                    IsSendForApproval = apPettyCash.IsSendForApproval,
                    SendForApprovalBy = apPettyCash.SendForApprovalBy,
                    SendForApprovalTime = apPettyCash.SendForApprovalTime,
                    OrgBranchName = apPettyCash.OrgBranchName,
                    DateCreated = apPettyCash.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = apPettyCash.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                _db.ApPettyCash.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();


                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AP petty cash", "Success", User.Identity.Name, $"updated petty cash {entity.ApPettyCashId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { apPettyCash }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountPayablePettyCash)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApPettyCash_Destroy([DataSourceRequest]DataSourceRequest request, ApPettyCash apPettyCash)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApPettyCash
                {
                    ApPettyCashId = apPettyCash.ApPettyCashId,
                    PurposeOfPayment = apPettyCash.PurposeOfPayment,
                    ApCashierSetupId = apPettyCash.ApCashierSetupId,
                    CurrentBalance = apPettyCash.CurrentBalance,
                    Date = apPettyCash.Date,
                    Reference = apPettyCash.Reference,
                    IsOnlineTransferred = apPettyCash.IsOnlineTransferred,
                    IsOnlineTransferredBy = apPettyCash.IsOnlineTransferredBy,
                    OnlineTransferredTime = apPettyCash.OnlineTransferredTime,
                    ForwardedFrom = apPettyCash.ForwardedFrom,
                    ForwardedTo = apPettyCash.ForwardedTo,
                    Status = apPettyCash.Status,
                    
                    IsOnlineApproved = apPettyCash.IsOnlineApproved,
                    OnlineApprovedBy = apPettyCash.OnlineApprovedBy,
                    OnlineApprovedTime = apPettyCash.OnlineApprovedTime,
                    IsVoid = apPettyCash.IsVoid,
                    VoidBy = apPettyCash.VoidBy,
                    VoidTime = apPettyCash.VoidTime,
                    IsSendForApproval = apPettyCash.IsSendForApproval,
                    SendForApprovalBy = apPettyCash.SendForApprovalBy,
                    SendForApprovalTime = apPettyCash.SendForApprovalTime,
                    OrgBranchName = apPettyCash.OrgBranchName,
                    DateCreated = apPettyCash.DateCreated,
                    LastModified = apPettyCash.LastModified,
                    CreatedBy = apPettyCash.CreatedBy,
                    ModifiedBy = apPettyCash.ModifiedBy
                };

                _db.ApPettyCash.Attach(entity);
                _db.ApPettyCash.Remove(entity);
                _db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted petty cash number " + apPettyCash.ApPettyCashId + " " + apPettyCash.Reference, Modules.Finance);
                #endregion
            }

            return Json(new[] { apPettyCash }.ToDataSourceResult(request, ModelState));
        }
        [AuthorizeRoles(FinanceConstants.AccountPayablePettyCash)]
        public JsonResult SendForApproval(int id)
        {
            var record = _db.ApPettyCash.Find(id);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved && (record.PettyCashPayments.Count > 0 || record.PettyCashUtilizations.Count > 0))
            {
                #region check segment length validation

                var distributions = _db.ApPettyCashDistributions.Where(x => x.ApPettyCashId == id).ToList();
                AccountHelper accountHelper = new AccountHelper();
                foreach (var dist in distributions)
                {
                    string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
                    if (!String.IsNullOrEmpty(message))
                    {
                        var formattedData2 = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData2, JsonRequestBehavior.AllowGet);
                    }

                    #region debit credit validation
                    if (dist.Debit != 0 && dist.Credit != 0)
                    {
                        message = $"{dist.AccountCode} Account has both debit and credit value.";
                        var formattedData1 = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData1, JsonRequestBehavior.AllowGet);
                    }

                    if (dist.Debit == 0 && dist.Credit == 0)
                    {
                        message = $"{dist.AccountCode} Both debit and credit can't have zero value at the same time.";
                        var formattedData1 = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData1, JsonRequestBehavior.AllowGet);
                    }

                    if (dist.Debit > 0 && dist.Credit > 0)
                    {
                        message = $"{dist.AccountCode} Both debit and credit can't have non zero value at the same time.";
                        var formattedData1 = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData1, JsonRequestBehavior.AllowGet);
                    }

                    if (dist.Debit < 0 || dist.Credit < 0)
                    {
                        message = $"{dist.AccountCode} Negative values are not allowed.";
                        var formattedData1 = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData1, JsonRequestBehavior.AllowGet);
                    }
                    #endregion

                }


                #endregion
                #region validate payment

                foreach (var item in record.PettyCashPayments)
                {
                    if (item.Amount <= 0)
                    {
                        var formattedData3 = new
                        {
                            Message = "Please fill payment amount information for all items.",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData3, JsonRequestBehavior.AllowGet);
                    }
                }
                #endregion
                #region validate utilization

                foreach (var item in record.PettyCashUtilizations)
                {
                    if (item.Amount <= 0 || item.AccountCode == "")
                    {
                        var formattedData3 = new
                        {
                            Message = "Please fill utilization amount and account code information for all items.",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData3, JsonRequestBehavior.AllowGet);
                    }
                }
                #endregion
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                _db.ApPettyCash.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();
                #region approval workflow

                bool isNewRecord = true;
                var validations = record.PettyCashValidations.Where(x => x.Status == ApprovalStatuses.Approve).AsQueryable();
                if (validations.Any())
                    isNewRecord = false;

                decimal paymenttotal = record.PettyCashPayments.Sum(x => x.Amount);
                decimal utilizationtotal = record.PettyCashUtilizations.Sum(x => x.Amount);
                decimal docamount = 0;
                docamount = utilizationtotal > paymenttotal ? utilizationtotal : paymenttotal;

                int docid = record.ApPettyCashId;

                DocApprovalWorkflowHelper.ForwardToApprover(docid, docamount, "", record.ForwardedTo, ApprovalDocumentType.ApPettyCash, isNewRecord);
                #endregion
                var formattedData = new
                {
                    Message = "Successfully Sent.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record == null)
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if ((record.PettyCashPayments.Count == 0 || record.PettyCashUtilizations.Count == 0))
            {
                var formattedData = new
                {
                    Message = "Please add line items for payment or utilization first.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IsSendForApproval)
            {
                var formattedData = new
                {
                    Message = "The record is already sent for approval you cannot send again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is already approved.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        [AuthorizeRoles(FinanceConstants.AccountPayablePettyCash)]
        public JsonResult TransferDistribution(string id, string description)
        {
            int myid = int.Parse(id);
            var record = _db.ApPettyCash.Find(myid);
            if (record != null && !record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is not approved.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record != null && !record.IsOnlineTransferred)
            {
                #region check if already transffered 
                var exist = _db.GLRecordJournals
                    .Where(x => x.Source == JournalSources.AP_PettyCash.ToString())
                    .Where(x => x.SourceId == record.ApPettyCashId.ToString())
                    .FirstOrDefault();
                if (exist != null)
                {
                    var formattedData = new
                    {
                        Message = "The record is already transffered, it can't be retransferred.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

                #endregion
                #region check segment length validation
                AccountHelper accountHelper = new AccountHelper();
                foreach (var dist in record.PettyCashDistributions)
                {
                    string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
                    if (!String.IsNullOrEmpty(message))
                    {
                        var formattedData = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                    #region debit credit validation
                    if (dist.Debit != 0 && dist.Credit != 0)
                    {
                        message = $"{dist.AccountCode} Account has both debit and credit value.";
                        var formattedData = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                    if (dist.Debit == 0 && dist.Credit == 0)
                    {
                        message = $"{dist.AccountCode} Both debit and credit can't have zero value at the same time.";
                        var formattedData = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                    if (dist.Debit > 0 && dist.Credit > 0)
                    {
                        message = $"{dist.AccountCode} Both debit and credit can't have non zero value at the same time.";
                        var formattedData = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                    if (dist.Debit < 0 || dist.Credit < 0)
                    {
                        message = $"{dist.AccountCode} Negative values are not allowed.";
                        var formattedData = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }
                    #endregion

                }


                #endregion
                #region check if debit and credit are equal
                decimal debit = 0;
                decimal credit = 0;
                var distribution = record.PettyCashDistributions.ToList();
                if (distribution.Any())
                {
                    debit = distribution.Sum(x => x.Debit);
                    credit = distribution.Sum(x => x.Credit);
                }
                if (debit != credit)
                {
                    var formattedData = new
                    {
                        Message = "Total debit and total credit is not balanced.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                if (debit == 0)
                {
                    var formattedData = new
                    {
                        Message = "Total debit and total credit is zero.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                #endregion

                #region send to GL
                string glyear ="";
                string glperiod = "";
                #region get period
                var period = _db.GLPeriods.Where(x => x.DateFrom <= record.Date)
                    .Where(x => x.DateTo >= record.Date).ToList();
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
                                var formattedData = new
                                {
                                    Message = "The period " + glperiod +
                                              " is closed,the record cannot be transfered to closed period.",
                                    Status = "NOT_OK"
                                };
                                return Json(formattedData, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            var formattedData = new
                            {
                                Message = "Period not found.",
                                Status = "NOT_OK"
                            };
                            return Json(formattedData, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else if (!period.Any())
                    {
                        var formattedData = new
                        {
                            Message = "There are no periods defined that contain the Petty Cash date.",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }
                    else if (period.Any())
                    {
                        var formattedData = new
                        {
                            Message = "There are multiple periods defined that contain the Payment date.",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                }

                #endregion
               
                if (record.PettyCashDistributions.Any())
                {
                    var journal = new GlRecordJournal
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        JournalName = "AP Petty Cash",
                        Description = !String.IsNullOrEmpty(description) ? description : "AP Petty Cash",
                        FiscalYear = glyear,
                        PeriodName = glperiod,
                        CategoryName = "AP Petty Cash",
                        SourcesName = "AP",
                        JournalNumber = "APPETCASH." + record.ApPettyCashId.ToString(),
                        SourceId = record.ApPettyCashId.ToString(),
                        Source = JournalSources.AP_PettyCash.ToString(),
                        EffectiveDate = record.Date.Date,
                        OrgBranchName = record.OrgBranchName,
                        BranchId=0,//to change
                        Status = "Approved",
                        IsOnlineApproved = true,
                        OnlineApprovedBy = User.Identity.Name,
                        OnlineApprovedTime = DateTime.Now
                    };
                    _db.GLRecordJournals.Add(journal);
                    _db.SaveChanges();
                    foreach (var i in record.PettyCashDistributions)
                    {
                        #region debit and credit

                        var journalDetail = new GLRecordJournalDetail
                        {
                            DateCreated = DateTime.Now,
                            CreatedBy = User.Identity.Name,
                            GLRecordJournal = journal,
                            AccountCode = i.AccountCode,
                            AccountDesc = i.AccountDesc,
                            Debit = i.Debit,
                            Credit = i.Credit,
                            AdditionalDescription=i.AdditionalDescription,
                        };
                        _db.GLRecordJournalDetails.Add(journalDetail);
                        _db.SaveChanges();
                        #endregion
                    }

                    #region update record

                    record.IsOnlineTransferred = true;
                    _db.Entry(record).State = EntityState.Modified;
                    _db.SaveChanges();

                    #endregion
                    #region split journal
                    var journaldetails = _db.GLRecordJournalDetails
                        .Where(x => x.GlRecordJournalId == journal.GlRecordJournalId).ToList();
                    foreach (var detail in journaldetails)
                    {
                        AccountHelper helper = new AccountHelper();
                        helper.SplitJournal(detail.GlRecordJournalDetailId);
                    }
                    #endregion

                    var formattedData = new
                    {
                        Message = "Successfully transfered.",
                        Status = "OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);

                }
                #endregion

            }
            else
            {
                var formattedData = new
                {
                    Message = "You cannot transfer this transaction, it is already transfered.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableVoid)]
        public JsonResult Void(int id)
        {
            var record = _db.ApPettyCash.Find(id);
            if (record != null && record.IsOnlineApproved && !record.IsVoid && !record.IsOnlineTransferred)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 10);
                record.IsOnlineApproved = false;
                record.IsSendForApproval = false;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                record.IsVoid = true;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                _db.ApPettyCash.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();


                //copy
                //CopyRecord(id);

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "petty cash", "Success", User.Identity.Name, $"void petty cash {id} ", Modules.Finance);
                #endregion

                var formattedData = new
                {
                    Message = "Void Successful.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record == null)
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            

            if (record.IsVoid)
            {
                var formattedData = new
                {
                    Message = "The record is already void.You can not void again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record.IsOnlineTransferred)
            {
                var formattedData = new
                {
                    Message = "The record is transferred to GL.You can not void .",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (!record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is not approved hence you can modify by editing.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }


            return null;
        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
