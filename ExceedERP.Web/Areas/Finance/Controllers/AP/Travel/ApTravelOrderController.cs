using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.DocumentApproval;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AP.Travel;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AP.Travel
{
    public class ApTravelOrderController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            var branches = _db.Branch;
            var structure = _db.MainCompanyStructures;
            var employee = _db.Person_Employee;
            var selectedEmployee = employee.Select(
                i => new
                {
                    Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                    Code = i.EmployeeId
                }).AsQueryable();
            ViewData["personEmployees"] = selectedEmployee;

            ViewData["Structure"] = structure;

            ViewData["Branches"] = branches;
        }
        public ActionResult ApTravelOrders_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            var sentRequestIds = _db.RequestesStatus
                .Where(x => x.ReferenceType == "TravelOrder")
                .Where(x => x.Status == RequestStatus.DEPARTMENT_APPROVAL)
                .Select(x => x.RequestId).ToList();

            IQueryable<ApTravelOrder> aptravelorders = _db.ApTravelOrders
                .Where(x => sentRequestIds.Contains(x.ApTravelOrderId))
                .Where(x => mybraches.Contains(x.OrgBranchName));
            DataSourceResult result = aptravelorders.ToDataSourceResult(request, apTravelOrder => new {
                ApTravelOrderId = apTravelOrder.ApTravelOrderId,
               
                BranchId = apTravelOrder.BranchId,
                OrgStructureId = apTravelOrder.OrgStructureId,
                PurposeOfTravel = apTravelOrder.PurposeOfTravel,
                RequestDate = apTravelOrder.RequestDate,
                AdvanceAmount = apTravelOrder.AdvanceAmount,
                IsPostedToSecondLedger = apTravelOrder.IsPostedToSecondLedger,

                Reference = apTravelOrder.Reference,
                EmployeeId = apTravelOrder.EmployeeId,
                IsForOther = apTravelOrder.IsForOther,

                ForwardedFrom = apTravelOrder.ForwardedFrom,
                ForwardedTo = apTravelOrder.ForwardedTo,
                IsFinalApproval = apTravelOrder.IsFinalApproval,

                IsOnlineTransferred = apTravelOrder.IsOnlineTransferred,
                IsOnlineTransferredBy = apTravelOrder.IsOnlineTransferredBy,
                OnlineTransferredTime = apTravelOrder.OnlineTransferredTime,
                Status = apTravelOrder.Status,
                IsOnlineApproved = apTravelOrder.IsOnlineApproved,
                OnlineApprovedBy = apTravelOrder.OnlineApprovedBy,
                OnlineApprovedTime = apTravelOrder.OnlineApprovedTime,
                IsVoid = apTravelOrder.IsVoid,
                VoidBy = apTravelOrder.VoidBy,
                VoidTime = apTravelOrder.VoidTime,
                IsSendForApproval = apTravelOrder.IsSendForApproval,
                SendForApprovalBy = apTravelOrder.SendForApprovalBy,
                SendForApprovalTime = apTravelOrder.SendForApprovalTime,
                OrgBranchName = apTravelOrder.OrgBranchName,
                DateCreated = apTravelOrder.DateCreated,
                LastModified = apTravelOrder.LastModified,
                CreatedBy = apTravelOrder.CreatedBy,
                ModifiedBy = apTravelOrder.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelOrders_Create([DataSourceRequest]DataSourceRequest request, ApTravelOrder apTravelOrder)
        {
            #region owner approval

            bool canOwnerApprove = DocApprovalHelper.CanOwnerApprove(ApprovalDocumentType.ApTravelOrder);
            if (canOwnerApprove)
                apTravelOrder.ForwardedTo = User.Identity.Name;

            #endregion

            if (ModelState.IsValid)
            {
                var entity = new ApTravelOrder
                {
                    
                    BranchId = apTravelOrder.BranchId,
                    OrgStructureId = apTravelOrder.OrgStructureId,
                    PurposeOfTravel = apTravelOrder.PurposeOfTravel,
                    RequestDate = apTravelOrder.RequestDate.Date,
                    AdvanceAmount = apTravelOrder.AdvanceAmount,
                    
                    Reference = apTravelOrder.Reference,
                    IsOnlineTransferred = apTravelOrder.IsOnlineTransferred,
                    IsOnlineTransferredBy = apTravelOrder.IsOnlineTransferredBy,
                    OnlineTransferredTime = apTravelOrder.OnlineTransferredTime,
                    EmployeeId = apTravelOrder.EmployeeId,
                    IsForOther = apTravelOrder.IsForOther,
                    IsPostedToSecondLedger = apTravelOrder.IsPostedToSecondLedger,

                    Status = apTravelOrder.Status,
                    ForwardedFrom = User.Identity.Name,
                    ForwardedTo = User.Identity.Name,

                    IsOnlineApproved = apTravelOrder.IsOnlineApproved,
                    OnlineApprovedBy = apTravelOrder.OnlineApprovedBy,
                    OnlineApprovedTime = apTravelOrder.OnlineApprovedTime,
                    IsVoid = apTravelOrder.IsVoid,
                    VoidBy = apTravelOrder.VoidBy,
                    VoidTime = apTravelOrder.VoidTime,
                    IsSendForApproval = apTravelOrder.IsSendForApproval,
                    SendForApprovalBy = apTravelOrder.SendForApprovalBy,
                    SendForApprovalTime = apTravelOrder.SendForApprovalTime,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    DateCreated = apTravelOrder.DateCreated,
                    LastModified = apTravelOrder.LastModified,
                    CreatedBy = apTravelOrder.CreatedBy,
                    ModifiedBy = apTravelOrder.ModifiedBy
                };

                _db.ApTravelOrders.Add(entity);
                _db.SaveChanges();
                apTravelOrder.ApTravelOrderId = entity.ApTravelOrderId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AP travel order", "Success", User.Identity.Name, $"created expense claim {entity.ApTravelOrderId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { apTravelOrder }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelOrders_Update([DataSourceRequest]DataSourceRequest request, ApTravelOrder apTravelOrder)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApTravelOrder
                {
                    ApTravelOrderId = apTravelOrder.ApTravelOrderId,
                   
                    BranchId = apTravelOrder.BranchId,
                    OrgStructureId = apTravelOrder.OrgStructureId,
                    PurposeOfTravel = apTravelOrder.PurposeOfTravel,
                    RequestDate = apTravelOrder.RequestDate,
                    AdvanceAmount = apTravelOrder.AdvanceAmount,
                    
                    Reference = apTravelOrder.Reference,
                    EmployeeId = apTravelOrder.EmployeeId,
                    IsForOther = apTravelOrder.IsForOther,
                    IsPostedToSecondLedger = apTravelOrder.IsPostedToSecondLedger,

                    IsOnlineTransferred = apTravelOrder.IsOnlineTransferred,
                    IsOnlineTransferredBy = apTravelOrder.IsOnlineTransferredBy,
                    OnlineTransferredTime = apTravelOrder.OnlineTransferredTime,
                    Status = apTravelOrder.Status,
                    ForwardedFrom = apTravelOrder.ForwardedFrom,
                    ForwardedTo = apTravelOrder.ForwardedTo,
                    IsOnlineApproved = apTravelOrder.IsOnlineApproved,
                    OnlineApprovedBy = apTravelOrder.OnlineApprovedBy,
                    OnlineApprovedTime = apTravelOrder.OnlineApprovedTime,
                    IsVoid = apTravelOrder.IsVoid,
                    VoidBy = apTravelOrder.VoidBy,
                    VoidTime = apTravelOrder.VoidTime,
                    IsSendForApproval = apTravelOrder.IsSendForApproval,
                    SendForApprovalBy = apTravelOrder.SendForApprovalBy,
                    SendForApprovalTime = apTravelOrder.SendForApprovalTime,
                    OrgBranchName = apTravelOrder.OrgBranchName,
                    DateCreated = apTravelOrder.DateCreated,
                    LastModified = apTravelOrder.LastModified,
                    CreatedBy = apTravelOrder.CreatedBy,
                    ModifiedBy = apTravelOrder.ModifiedBy
                };

                _db.ApTravelOrders.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AP travel order", "Success", User.Identity.Name, $"updated expense claim {entity.ApTravelOrderId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { apTravelOrder }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelOrders_Destroy([DataSourceRequest]DataSourceRequest request, ApTravelOrder apTravelOrder)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApTravelOrder
                {
                    ApTravelOrderId = apTravelOrder.ApTravelOrderId,
                   
                    BranchId = apTravelOrder.BranchId,
                    OrgStructureId = apTravelOrder.OrgStructureId,
                    PurposeOfTravel = apTravelOrder.PurposeOfTravel,
                    RequestDate = apTravelOrder.RequestDate,
                    AdvanceAmount = apTravelOrder.AdvanceAmount,
                    IsPostedToSecondLedger = apTravelOrder.IsPostedToSecondLedger,

                    Reference = apTravelOrder.Reference,
                    IsOnlineTransferred = apTravelOrder.IsOnlineTransferred,
                    IsOnlineTransferredBy = apTravelOrder.IsOnlineTransferredBy,
                    OnlineTransferredTime = apTravelOrder.OnlineTransferredTime,
                    Status = apTravelOrder.Status,
                    IsOnlineApproved = apTravelOrder.IsOnlineApproved,
                    OnlineApprovedBy = apTravelOrder.OnlineApprovedBy,
                    OnlineApprovedTime = apTravelOrder.OnlineApprovedTime,
                    IsVoid = apTravelOrder.IsVoid,
                    VoidBy = apTravelOrder.VoidBy,
                    VoidTime = apTravelOrder.VoidTime,
                    IsSendForApproval = apTravelOrder.IsSendForApproval,
                    SendForApprovalBy = apTravelOrder.SendForApprovalBy,
                    SendForApprovalTime = apTravelOrder.SendForApprovalTime,
                    OrgBranchName = apTravelOrder.OrgBranchName,
                    DateCreated = apTravelOrder.DateCreated,
                    LastModified = apTravelOrder.LastModified,
                    CreatedBy = apTravelOrder.CreatedBy,
                    ModifiedBy = apTravelOrder.ModifiedBy
                };

                _db.ApTravelOrders.Attach(entity);
                _db.ApTravelOrders.Remove(entity);
                _db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted travel order" + apTravelOrder.ApTravelOrderId + " " + apTravelOrder.Reference, Modules.Finance);
                #endregion
            }

            return Json(new[] { apTravelOrder }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableTravelOrder)]
        public JsonResult SendForApproval(int id)
        {
            var record = _db.ApTravelOrders.Find(id);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved && record.ApTravelOrderDepartures.Count > 0)
            {
                #region check segment length validation

                var distributions = _db.ApTravelOrderDistributions.Where(x => x.ApTravelOrderId == id).ToList();
                AccountHelper accountHelper = new AccountHelper();
                foreach (var dist in distributions)
                {
                    string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
                    if (!String.IsNullOrEmpty(message))
                    {
                        var formattedData1 = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData1, JsonRequestBehavior.AllowGet);
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
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                _db.ApTravelOrders.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();
                #region approval workflow
                bool isNewRecord = true;
                var validations = record.ApTravelOrderValidations.Where(x => x.Status == ApprovalStatuses.Approve).AsQueryable();
                if (validations.Any())
                    isNewRecord = false;
                decimal docamount = record.AdvanceAmount;
                int docid = record.ApTravelOrderId;
                DocApprovalWorkflowHelper.ForwardToApprover(docid, docamount, "", User.Identity.Name, ApprovalDocumentType.ApTravelOrder, isNewRecord);
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

            if (record.ApTravelOrderDepartures.Count == 0)
            {
                var formattedData = new
                {
                    Message = "Please add  departure line items first.",
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

        public JsonResult TransferDistribution(string id, string description)
        {
            int myid = int.Parse(id);
            var record = _db.ApTravelOrders.Find(myid);
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
                    .Where(x => x.Source == JournalSources.AP_TravelOrder.ToString())
                    .Where(x => x.SourceId == record.ApTravelOrderId.ToString())
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
                foreach (var dist in record.ApTravelOrderDistributions)
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
                var distribution = record.ApTravelOrderDistributions.ToList();
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
                var period = _db.GLPeriods.Where(x => x.DateFrom <= record.RequestDate.Date)
                    .Where(x => x.DateTo >= record.RequestDate.Date).ToList();
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
                        Message = "There are no periods defined that contain the Travel date.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

                #endregion
               
                if (record.ApTravelOrderDistributions.Any())
                {
                    GlRecordJournal journal = new GlRecordJournal
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        JournalName = "AP Travel Order",
                        Description = !String.IsNullOrEmpty(description) ? description : "AP Travel Order",
                        FiscalYear = glyear,
                        PeriodName = glperiod,
                        CategoryName = "AP Travel Order",
                        SourcesName = "AP",
                        JournalNumber = "APTRAVORD." + record.ApTravelOrderId.ToString(),
                        SourceId = record.ApTravelOrderId.ToString(),
                        Source = JournalSources.AP_TravelOrder.ToString(),
                        EffectiveDate = record.RequestDate.Date,
                        OrgBranchName = record.OrgBranchName,
                        Status = "Approved",
                        BranchId=record.BranchId,
                        IsOnlineApproved = true,
                        OnlineApprovedBy = User.Identity.Name,
                        OnlineApprovedTime = DateTime.Now
                    };
                    _db.GLRecordJournals.Add(journal);
                    _db.SaveChanges();
                    foreach (var i in record.ApTravelOrderDistributions)
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
