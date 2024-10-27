using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Helpers.Finance;
using ExceedERP.Core.Domain.Finance.AP.FundTransfer;

namespace ExceedERP.Web.Areas.Finance.Controllers.AP.FundReceives
{
    [AuthorizeRoles(FinanceConstants.AccountPayableView, FinanceConstants.AccountPayableFundTransferApprover, FinanceConstants.AccountPayableFundTransfer)]
    public class ApFundReceiveController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        public ActionResult Index()
        {
            
            PopulateViewData();

            return View();
        }
        private void PopulateViewData()
        {
            var employeName = _db.Person_Employee.ToList();
            ViewData["EmployeeName"] = employeName;

            var tax = _db.GLTaxRates.ToList();
            ViewData["GLTaxRate"] = tax;

            var branch = _db.Branch.ToList();
            ViewData["Branch"] = branch;

            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                var businessUnits = _db.BusinessUnits.ToList();
                ViewData["BusinessUnits"] = businessUnits;
            }

            var structure = _db.MainCompanyStructures.ToList();
            ViewData["OrgStructure"] = structure;

            var accounts = _db.BankAccounts.Select(c => new
            {
                Code = c.BankAccountId,
                Name = c.AccountNumber + "-" + c.AccountName
            });
            ViewData["BankAccounts"] = accounts;

        }


        [AuthorizeRoles(FinanceConstants.AccountPayableView, FinanceConstants.AccountPayableFundTransferApprover, FinanceConstants.AccountPayableFundTransfer)]
        public ActionResult FundReceive_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);

            IQueryable<FundReceive> FundReceives = _db.FundReceives
                .Where(x => mybraches.Contains(x.BranchId))
                .Where(x => x.IsDepartmentOnlineApproved);

            
            DataSourceResult result = FundReceives.ToDataSourceResult(request, FundReceive => new
            {
                FundReceiveId = FundReceive.FundReceiveId,
                FundTransferId = FundReceive.FundTransferId,
                AmountReceived = FundReceive.AmountReceived,
               
                BranchId = FundReceive.BranchId,
                BusinessUnitId = FundReceive.BusinessUnitId,

                
                FromBranchId = FundReceive.FromBranchId,
                FromBusinessUnitId = FundReceive.FromBusinessUnitId,
                CreatedAtBranchId = FundReceive.CreatedAtBranchId,
                CreatedAtBusinessUnitId = FundReceive.CreatedAtBusinessUnitId,
                CreatedAtOrgStructureId = FundReceive.CreatedAtOrgStructureId,
                BankAccountId = FundReceive.BankAccountId,
                ReceiveDate = FundReceive.ReceiveDate,
               
                Status =FundReceive.Status,
                Remark =FundReceive.Remark,
                IsVoid =FundReceive.IsVoid,
                VoidBy =FundReceive.VoidBy,
                VoidTime =FundReceive.VoidTime,

              

                IsDepartmentOnlineApproved = FundReceive.IsDepartmentOnlineApproved,
                DepartmentOnlineApprovedBy = FundReceive.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = FundReceive.DepartmentOnlineApprovedTime,
                IsDepartmentSendForApproval = FundReceive.IsDepartmentSendForApproval,
                DepartmentSendForApprovalBy = FundReceive.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = FundReceive.DepartmentSendForApprovalTime,

                ForwardedFrom = FundReceive.ForwardedFrom,
                ForwardedTo = FundReceive.ForwardedTo,
                IsFinalApproval = FundReceive.IsFinalApproval,
                CurrentApprovalStep = FundReceive.CurrentApprovalStep,

                IsOnlineApproved =FundReceive.IsOnlineApproved,
                OnlineApprovedBy =FundReceive.OnlineApprovedBy,
                OnlineApprovedTime =FundReceive.OnlineApprovedTime,
                IsSendForApproval =FundReceive.IsSendForApproval,
                SendForApprovalBy =FundReceive.SendForApprovalBy,
                SendForApprovalTime =FundReceive.SendForApprovalTime,
                IsOnlineTransferred =FundReceive.IsOnlineTransferred,
                IsOnlineTransferredBy =FundReceive.IsOnlineTransferredBy,
                OnlineTransferredTime =FundReceive.OnlineTransferredTime,
                OrgBranchName = FundReceive.OrgBranchName,

                DateCreated =FundReceive.DateCreated,
                CreatedBy =FundReceive.CreatedBy,
                LastModified =FundReceive.LastModified,
                ModifiedBy =FundReceive.ModifiedBy,
            });

            return Json(result);
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableFundTransfer)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FundReceive_Create([DataSourceRequest]DataSourceRequest request, FundReceive FundReceive)
        {

            #region data
            FundReceive.CurrentApprovalStep = 0;           



            FundReceive.CreatedAtBranchId = UserInformationHelper.GetUserDefaultBranchId(User.Identity.Name);
            FundReceive.CreatedAtBusinessUnitId = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name);
            FundReceive.CreatedAtOrgStructureId = UserInformationHelper.GetUserPlacementWorkUnit(User.Identity.Name);

            #endregion

            if (ModelState.IsValid)
            {

                var entity = new FundReceive
                {
                    
                    BranchId = FundReceive.BranchId,
                    BusinessUnitId = FundReceive.BusinessUnitId,
                   
                    FromBranchId = FundReceive.FromBranchId,
                    FromBusinessUnitId = FundReceive.FromBusinessUnitId,
                    CreatedAtBranchId = FundReceive.CreatedAtBranchId,
                    CreatedAtBusinessUnitId = FundReceive.CreatedAtBusinessUnitId,
                    CreatedAtOrgStructureId = FundReceive.CreatedAtOrgStructureId,
                    BankAccountId = FundReceive.BankAccountId,
                  
                    AmountReceived = FundReceive.AmountReceived,
                    ReceiveDate = FundReceive.ReceiveDate,
                    Remark = FundReceive.Remark,
                    IsVoid = FundReceive.IsVoid,
                    VoidBy = FundReceive.VoidBy,
                    VoidTime = FundReceive.VoidTime,
                    IsOnlineApproved = FundReceive.IsOnlineApproved,
                    OnlineApprovedBy = FundReceive.OnlineApprovedBy,
                    OnlineApprovedTime = FundReceive.OnlineApprovedTime,
                    IsSendForApproval = FundReceive.IsSendForApproval,
                    SendForApprovalBy = FundReceive.SendForApprovalBy,
                    SendForApprovalTime = FundReceive.SendForApprovalTime,
                    IsOnlineTransferred = FundReceive.IsOnlineTransferred,
                    IsOnlineTransferredBy = FundReceive.IsOnlineTransferredBy,
                    OnlineTransferredTime = FundReceive.OnlineTransferredTime,

                   
                    ForwardedFrom = User.Identity.Name,
                    ForwardedTo = User.Identity.Name,
                    IsFinalApproval = FundReceive.IsFinalApproval,
                    CurrentApprovalStep = FundReceive.CurrentApprovalStep,

                    IsDepartmentOnlineApproved = FundReceive.IsDepartmentOnlineApproved,
                    DepartmentOnlineApprovedBy = FundReceive.DepartmentOnlineApprovedBy,
                    DepartmentOnlineApprovedTime = FundReceive.DepartmentOnlineApprovedTime,
                    IsDepartmentSendForApproval = FundReceive.IsDepartmentSendForApproval,
                    DepartmentSendForApprovalBy = FundReceive.DepartmentSendForApprovalBy,
                    DepartmentSendForApprovalTime = FundReceive.DepartmentSendForApprovalTime,

                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),

                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    LastModified = FundReceive.LastModified,
                    ModifiedBy = FundReceive.ModifiedBy,
                };

                _db.FundReceives.Add(entity);
                _db.SaveChanges();
                FundReceive.FundReceiveId = entity.FundReceiveId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "fund transfer receive", "Success", User.Identity.Name, $"created fund transfer receive {entity.FundReceiveId} ", Modules.Finance);
                #endregion
            }



            return Json(new[] { FundReceive }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableFundTransfer)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FundReceive_Update([DataSourceRequest]DataSourceRequest request, FundReceive FundReceive)
        {

            #region data          

            #endregion
            if (ModelState.IsValid)
            {
                var entity = new FundReceive
                {
                    FundTransferId = FundReceive.FundTransferId,
                    BranchId = FundReceive.BranchId,
                    BusinessUnitId = FundReceive.BusinessUnitId,
                    FundReceiveId = FundReceive.FundReceiveId,
                    FromBranchId = FundReceive.FromBranchId,
                    FromBusinessUnitId = FundReceive.FromBusinessUnitId,
                    CreatedAtBranchId = FundReceive.CreatedAtBranchId,
                    CreatedAtBusinessUnitId = FundReceive.CreatedAtBusinessUnitId,
                    CreatedAtOrgStructureId = FundReceive.CreatedAtOrgStructureId,
                    BankAccountId = FundReceive.BankAccountId,
                  
                    AmountReceived = FundReceive.AmountReceived,
                    ReceiveDate = FundReceive.ReceiveDate,

                    
                    ForwardedFrom = FundReceive.ForwardedFrom,
                    ForwardedTo = FundReceive.ForwardedTo,
                    IsFinalApproval = FundReceive.IsFinalApproval,
                    CurrentApprovalStep = FundReceive.CurrentApprovalStep,
                   

                    IsDepartmentOnlineApproved = FundReceive.IsDepartmentOnlineApproved,
                    DepartmentOnlineApprovedBy = FundReceive.DepartmentOnlineApprovedBy,
                    DepartmentOnlineApprovedTime = FundReceive.DepartmentOnlineApprovedTime,
                    IsDepartmentSendForApproval = FundReceive.IsDepartmentSendForApproval,
                    DepartmentSendForApprovalBy = FundReceive.DepartmentSendForApprovalBy,
                    DepartmentSendForApprovalTime = FundReceive.DepartmentSendForApprovalTime,

                    IsOnlineApproved = FundReceive.IsOnlineApproved,
                    OnlineApprovedBy = FundReceive.OnlineApprovedBy,
                    OnlineApprovedTime = FundReceive.OnlineApprovedTime,
                    IsSendForApproval = FundReceive.IsSendForApproval,
                    SendForApprovalBy = FundReceive.SendForApprovalBy,
                    SendForApprovalTime = FundReceive.SendForApprovalTime,
                    IsOnlineTransferred = FundReceive.IsOnlineTransferred,
                    IsOnlineTransferredBy = FundReceive.IsOnlineTransferredBy,
                    OnlineTransferredTime = FundReceive.OnlineTransferredTime,
                    OrgBranchName = FundReceive.OrgBranchName,

                    Remark = FundReceive.Remark,
                    IsVoid = FundReceive.IsVoid,
                    VoidBy = FundReceive.VoidBy,
                    VoidTime = FundReceive.VoidTime,


                    DateCreated = FundReceive.DateCreated,
                    CreatedBy = FundReceive.CreatedBy,
                    LastModified =DateTime.Now,
                    ModifiedBy = User.Identity.Name,
                };

                _db.FundReceives.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Self care fund transfer receive", "Success", User.Identity.Name, $"updated fund transfer receive {entity.FundReceiveId} ", Modules.Finance);
                #endregion

            }

            return Json(new[] { FundReceive }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableFundTransfer)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FundReceive_Destroy([DataSourceRequest]DataSourceRequest request, FundReceive FundReceive)
        {
           
            if (ModelState.IsValid)
            {
                var entity = new FundReceive
                {
                    FundTransferId = FundReceive.FundTransferId,
                    BranchId = FundReceive.BranchId,
                    BusinessUnitId = FundReceive.BusinessUnitId,
                    FundReceiveId = FundReceive.FundReceiveId,
                    FromBranchId = FundReceive.FromBranchId,
                    FromBusinessUnitId = FundReceive.FromBusinessUnitId,
                    CreatedAtBranchId = FundReceive.CreatedAtBranchId,
                    CreatedAtBusinessUnitId = FundReceive.CreatedAtBusinessUnitId,
                    CreatedAtOrgStructureId = FundReceive.CreatedAtOrgStructureId,
                    BankAccountId = FundReceive.BankAccountId,

                    AmountReceived = FundReceive.AmountReceived,
                    ReceiveDate = FundReceive.ReceiveDate,


                    ForwardedFrom = FundReceive.ForwardedFrom,
                    ForwardedTo = FundReceive.ForwardedTo,
                    IsFinalApproval = FundReceive.IsFinalApproval,
                    CurrentApprovalStep = FundReceive.CurrentApprovalStep,


                    IsDepartmentOnlineApproved = FundReceive.IsDepartmentOnlineApproved,
                    DepartmentOnlineApprovedBy = FundReceive.DepartmentOnlineApprovedBy,
                    DepartmentOnlineApprovedTime = FundReceive.DepartmentOnlineApprovedTime,
                    IsDepartmentSendForApproval = FundReceive.IsDepartmentSendForApproval,
                    DepartmentSendForApprovalBy = FundReceive.DepartmentSendForApprovalBy,
                    DepartmentSendForApprovalTime = FundReceive.DepartmentSendForApprovalTime,

                    IsOnlineApproved = FundReceive.IsOnlineApproved,
                    OnlineApprovedBy = FundReceive.OnlineApprovedBy,
                    OnlineApprovedTime = FundReceive.OnlineApprovedTime,
                    IsSendForApproval = FundReceive.IsSendForApproval,
                    SendForApprovalBy = FundReceive.SendForApprovalBy,
                    SendForApprovalTime = FundReceive.SendForApprovalTime,
                    IsOnlineTransferred = FundReceive.IsOnlineTransferred,
                    IsOnlineTransferredBy = FundReceive.IsOnlineTransferredBy,
                    OnlineTransferredTime = FundReceive.OnlineTransferredTime,
                    OrgBranchName = FundReceive.OrgBranchName,

                    Remark = FundReceive.Remark,
                    IsVoid = FundReceive.IsVoid,
                    VoidBy = FundReceive.VoidBy,
                    VoidTime = FundReceive.VoidTime,

                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted fund transfer receive ID  " + FundReceive.FundReceiveId);
                #endregion
                _db.FundReceives.Attach(entity);
                _db.FundReceives.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { FundReceive }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableFundTransfer)]
        public JsonResult SendForApproval(int id)
        {
            var record = _db.FundReceives.Find(id);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved && record.FundReceiveDistributions.Count > 0)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                _db.FundReceives.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();
                #region approval workflow
                //bool isNewRecord = true;
                //var validations = record.ApTravelOrderValidations.Where(x => x.Status == ApprovalStatuses.Approve).AsQueryable();
                //if (validations.Any())
                //    isNewRecord = false;
                //decimal docamount = record.AdvanceAmount;
                //int docid = record.ApTravelOrderId;
                //DocApprovalWorkflowHelper.ForwardToApprover(docid, docamount, "", User.Identity.Name, ApprovalDocumentType.ApTravelOrder, isNewRecord);
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

            if (record.FundReceiveDistributions.Count == 0)
            {
                var formattedData = new
                {
                    Message = "Please add  distribution line items first.",
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

        [AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
        public JsonResult TransferDistribution(string id, string description)
        {
            int myid = int.Parse(id);
            var record = _db.FundReceives.Find(myid);
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
                    .Where(x => x.Source == JournalSources.AP_Fund_Receive.ToString())
                    .Where(x => x.SourceId == record.FundReceiveId.ToString())
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
                foreach (var dist in record.FundReceiveDistributions)
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
                var distribution = record.FundReceiveDistributions.ToList();
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

                string glyear = "";
                string glperiod = "";
                #region get period
                var period = _db.GLPeriods.Where(x => x.DateFrom <= record.ReceiveDate.Date)
                    .Where(x => x.DateTo >= record.ReceiveDate.Date).ToList();
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

                    else
                    {
                        var formattedData = new
                        {
                            Message = "There are multiple periods defined that contain the Invoice date.",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                }
                else
                {
                    var formattedData = new
                    {
                        Message = "There are no periods defined that contain the payment date.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

                #endregion

                if (record.FundReceiveDistributions.Any())
                {
                    GlRecordJournal journal = new GlRecordJournal
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        JournalName = "AP Direct Payment",
                        Description = !String.IsNullOrEmpty(description) ? description : "AP Direct Payment",
                        FiscalYear = glyear,
                        PeriodName = glperiod,
                        CategoryName = "AP Fund Transfer Receive",
                        SourcesName = "AP",
                        JournalNumber = "APFR." + record.FundReceiveId.ToString(),
                        SourceId = record.FundReceiveId.ToString(),
                        Source = JournalSources.AP_Direct_Payment.ToString(),
                        EffectiveDate = record.ReceiveDate.Date,
                        OrgBranchName = record.OrgBranchName,
                        Status = "Approved",
                        BranchId = record.BranchId,
                        BusinessUnitId = record.BusinessUnitId,
                        Remark = record.Remark,
                        PaymentReference = "",
                        ReferenceDoc = record.Remark,
                        InvoiceReferenceDoc = "",
                        DebitSum = debit,
                        IsOnlineApproved = true,
                        OnlineApprovedBy = User.Identity.Name,
                        OnlineApprovedTime = DateTime.Now
                    };
                    _db.GLRecordJournals.Add(journal);
                    _db.SaveChanges();
                    foreach (var i in record.FundReceiveDistributions)
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
                            AdditionalDescription = i.AdditionalDescription,
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
            var record = _db.ApTravelOrders.Find(id);
            if (record != null && record.IsOnlineApproved && !record.IsVoid && !record.IsOnlineTransferred && record.ApTravelOrderDepartures.Count > 0)
            {
                #region validate claim

                var payments = _db.ApTravelOrders.Where(x => x.ApTravelOrderId == record.ApTravelOrderId)
                    .ToList();
                if (payments.Any())
                {
                    var formattedData1 = new
                    {
                        Message = $"The travel order number {record.ApTravelOrderId} is already applied in Travel Order \n hence cannot be void.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData1, JsonRequestBehavior.AllowGet);
                }
                #endregion
                record.Status = Enum.GetName(typeof(OperationStatuses), 10);
                record.IsOnlineApproved = false;
                record.IsSendForApproval = false;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                record.IsVoid = false;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                _db.ApTravelOrders.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();


                //copy
                //CopyRecord(id);

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "travel order", "Success", User.Identity.Name, $"void travel order {id} ", Modules.Finance);
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

            if (record.ApTravelOrderDepartures.Count == 0)
            {
                var formattedData = new
                {
                    Message = "Please add departure lines and try again.",
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
