using System;
//using System.Data
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Models;
using ExceedERP.Helpers.Finance;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Helpers.CRM;
using ExceedERP.Core.Domain.Construction;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.CashSalesUser, CRMRoles.CashSalesApprover)]
    public class CRMDepositController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["Branches"] = db.Branch.Select(b => new { b.BranchId, b.Name, b.Abbreviation }).ToList();
            ViewData["CRMPeriods"] = db.CRMPeriods.Select(b => new { b.CRMPeriodId, b.Name }).ToList();
            ViewData["SalesStore"] = db.SalesStores;
            ViewData["PaymentMethod"] = db.PaymentMethods;

            return View();
        }

        public ActionResult CRMDeposit_Read([DataSourceRequest]DataSourceRequest request)
        {


            IQueryable<CRMDeposit> crmdeposit = db.CRMDeposit.Where(d => d.SalesPersonUserName == User.Identity.Name);
            DataSourceResult result = crmdeposit.ToDataSourceResult(request, cRMDeposit => new
            {
                CRMDepositId = cRMDeposit.CRMDepositId,
                SalesPersonUserName = cRMDeposit.SalesPersonUserName,
                SalesPersonFullName = cRMDeposit.SalesPersonFullName,
                BranchId = cRMDeposit.BranchId,
                StoreID = cRMDeposit.StoreID,
                CRMPeriodId = cRMDeposit.CRMPeriodId,
                Date = cRMDeposit.Date,
                Status = cRMDeposit.Status,
                Remark = cRMDeposit.Remark,
                IsVoid = cRMDeposit.IsVoid,
                VoidBy = cRMDeposit.VoidBy,
                VoidTime = cRMDeposit.VoidTime,
                IsOnlineApproved = cRMDeposit.IsOnlineApproved,
                OnlineApprovedBy = cRMDeposit.OnlineApprovedBy,
                OnlineApprovedTime = cRMDeposit.OnlineApprovedTime,
                IsOnlineTransferred = cRMDeposit.IsOnlineTransferred,
                IsOnlineTransferredBy = cRMDeposit.IsOnlineTransferredBy,
                OnlineTransferredTime = cRMDeposit.OnlineTransferredTime,
                IsSendForApproval = cRMDeposit.IsSendForApproval,
                OrgBranchName = cRMDeposit.OrgBranchName,
                SendForApprovalBy = cRMDeposit.SendForApprovalBy,
                SendForApprovalTime = cRMDeposit.SendForApprovalTime,
                DateCreated = cRMDeposit.DateCreated,
                LastModified = cRMDeposit.LastModified,
                CreatedBy = cRMDeposit.CreatedBy,
                ModifiedBy = cRMDeposit.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CRMDeposit_Create([DataSourceRequest]DataSourceRequest request, CRMDeposit cRMDeposit)
        {
            //if (DuplicateRecordExists(cRMDeposit))
            //{
            //    ModelState.AddModelError("", "Dupliate record not allowed.");
            //}
            if (ModelState.IsValid)
            {
                var entity = new CRMDeposit
                {
                    SalesPersonUserName = cRMDeposit.SalesPersonUserName,
                    SalesPersonFullName = GetUserFullName(cRMDeposit.SalesPersonUserName),
                    BranchId = cRMDeposit.BranchId,
                    StoreID = cRMDeposit.StoreID,
                    CRMPeriodId = cRMDeposit.CRMPeriodId,
                    Date = cRMDeposit.Date,

                    Status = OperationStatuses.New.ToString(),
                    Remark = cRMDeposit.Remark,
                    IsVoid = cRMDeposit.IsVoid,
                    VoidBy = cRMDeposit.VoidBy,
                    VoidTime = cRMDeposit.VoidTime,
                    IsOnlineApproved = cRMDeposit.IsOnlineApproved,
                    OnlineApprovedBy = cRMDeposit.OnlineApprovedBy,
                    OnlineApprovedTime = cRMDeposit.OnlineApprovedTime,
                    IsOnlineTransferred = cRMDeposit.IsOnlineTransferred,
                    IsOnlineTransferredBy = cRMDeposit.IsOnlineTransferredBy,
                    OnlineTransferredTime = cRMDeposit.OnlineTransferredTime,
                    IsSendForApproval = cRMDeposit.IsSendForApproval,
                    OrgBranchName = cRMDeposit.OrgBranchName,
                    SendForApprovalBy = cRMDeposit.SendForApprovalBy,
                    SendForApprovalTime = cRMDeposit.SendForApprovalTime,

                    DateCreated = DateTime.Now,
                    LastModified = cRMDeposit.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = cRMDeposit.ModifiedBy
                };

                db.CRMDeposit.Add(entity);
                db.SaveChanges();
                cRMDeposit.CRMDepositId = entity.CRMDepositId;
            }

            return Json(new[] { cRMDeposit }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CRMDeposit_Update([DataSourceRequest]DataSourceRequest request, CRMDeposit cRMDeposit)
        {
            //if (DuplicateRecordExists(cRMDeposit))
            //{
            //    ModelState.AddModelError("", "Dupliate record not allowed.");
            //}
            if (ModelState.IsValid)
            {
                var entity = new CRMDeposit
                {
                    CRMDepositId = cRMDeposit.CRMDepositId,
                    SalesPersonUserName = cRMDeposit.SalesPersonUserName,
                    SalesPersonFullName = GetUserFullName(cRMDeposit.SalesPersonUserName),
                    BranchId = cRMDeposit.BranchId,
                    StoreID = cRMDeposit.StoreID,
                    CRMPeriodId = cRMDeposit.CRMPeriodId,
                    Date = cRMDeposit.Date,
                    Status = cRMDeposit.Status,
                    Remark = cRMDeposit.Remark,
                    IsVoid = cRMDeposit.IsVoid,
                    VoidBy = cRMDeposit.VoidBy,
                    VoidTime = cRMDeposit.VoidTime,
                    IsOnlineApproved = cRMDeposit.IsOnlineApproved,
                    OnlineApprovedBy = cRMDeposit.OnlineApprovedBy,
                    OnlineApprovedTime = cRMDeposit.OnlineApprovedTime,
                    IsOnlineTransferred = cRMDeposit.IsOnlineTransferred,
                    IsOnlineTransferredBy = cRMDeposit.IsOnlineTransferredBy,
                    OnlineTransferredTime = cRMDeposit.OnlineTransferredTime,
                    IsSendForApproval = cRMDeposit.IsSendForApproval,
                    OrgBranchName = cRMDeposit.OrgBranchName,
                    SendForApprovalBy = cRMDeposit.SendForApprovalBy,
                    SendForApprovalTime = cRMDeposit.SendForApprovalTime,

                    DateCreated = cRMDeposit.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = cRMDeposit.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.CRMDeposit.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { cRMDeposit }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CRMDeposit_Destroy([DataSourceRequest]DataSourceRequest request, CRMDeposit cRMDeposit)
        {
            if (ModelState.IsValid)
            {
                var entity = new CRMDeposit
                {
                    CRMDepositId = cRMDeposit.CRMDepositId,
                    SalesPersonUserName = cRMDeposit.SalesPersonUserName,
                    SalesPersonFullName = cRMDeposit.SalesPersonFullName,
                    BranchId = cRMDeposit.BranchId,
                    StoreID = cRMDeposit.StoreID,
                    CRMPeriodId = cRMDeposit.CRMPeriodId,
                    Date = cRMDeposit.Date,
                    Status = cRMDeposit.Status,
                    Remark = cRMDeposit.Remark,
                    IsVoid = cRMDeposit.IsVoid,
                    VoidBy = cRMDeposit.VoidBy,
                    VoidTime = cRMDeposit.VoidTime,
                    IsOnlineApproved = cRMDeposit.IsOnlineApproved,
                    OnlineApprovedBy = cRMDeposit.OnlineApprovedBy,
                    OnlineApprovedTime = cRMDeposit.OnlineApprovedTime,
                    IsOnlineTransferred = cRMDeposit.IsOnlineTransferred,
                    IsOnlineTransferredBy = cRMDeposit.IsOnlineTransferredBy,
                    OnlineTransferredTime = cRMDeposit.OnlineTransferredTime,
                    IsSendForApproval = cRMDeposit.IsSendForApproval,
                    OrgBranchName = cRMDeposit.OrgBranchName,
                    SendForApprovalBy = cRMDeposit.SendForApprovalBy,
                    SendForApprovalTime = cRMDeposit.SendForApprovalTime,
                    DateCreated = cRMDeposit.DateCreated,
                    LastModified = cRMDeposit.LastModified,
                    CreatedBy = cRMDeposit.CreatedBy,
                    ModifiedBy = cRMDeposit.ModifiedBy
                };

                db.CRMDeposit.Attach(entity);
                db.CRMDeposit.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { cRMDeposit }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        private string GetUserFullName(string userName)
        {
            var identityDb = new ApplicationDbContext();
            string name = userName;
            var currentUser = identityDb.Users.FirstOrDefault(u => u.UserName == userName);
            if (currentUser != null)
            {
                name = $"{currentUser.FirstName} {currentUser.MiddleName} {currentUser.LastName}";
            }
            return name;
        }
        //private bool DuplicateRecordExists(CRMDeposit cRMDeposit)
        //{
        //    return db.CRMDeposit.Any(d => d.CRMDepositId != cRMDeposit.CRMDepositId && d.BranchId == cRMDeposit.BranchId && d.StoreID == cRMDeposit.StoreID && d.SalesPersonUserName == cRMDeposit.SalesPersonUserName);
        //}

        public JsonResult TransferDistribution(string id, string description)
        {
            int myid = int.Parse(id);
            var record = db.CRMDeposit.Find(myid);
            if (record != null && !record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is not approved.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record != null && record.IsOnlineTransferred)
            {
                #region check if already transffered 
                var formattedData = new
                {
                    Message = "The record is already transffered, it can't be retransferred.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);

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
                //var distribution = record.ArSalesOrderDistribution.ToList();
                if (distributions.Any())
                {
                    debit = distributions.Sum(x => x.Debit);
                    credit = distributions.Sum(x => x.Credit);
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
                if (debit <= 0)
                {
                    var formattedData = new
                    {
                        Message = "Total debit and total credit should be positive number.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                #endregion

                #region send to GL

                string glyear = "";
                string glperiod = "";
                #region get period
                var period = db.GLPeriods.Where(x => x.DateFrom <= record.Date.Date)
                    .Where(x => x.DateTo >= record.Date.Date).ToList();
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
                        Message = "There are no periods defined that contain the deposit date.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

                #endregion

                if (distributions.Any())
                {
                    GlRecordJournal journal = new GlRecordJournal
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name,
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
                        OnlineApprovedBy = User.Identity.Name,
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
                            CreatedBy = User.Identity.Name,
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
                    record.OnlineTransferredTime = DateTime.Now;
                    record.IsOnlineTransferredBy = User.Identity.Name;

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
                    //CRMSalesHelper.SendDepositToBankReconciliation(record, User.Identity.Name);
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
        public JsonResult RegenerateDistribution(int id)
        {
            var record = db.CRMDeposit.Find(id);
            if ((record != null && !record.IsOnlineTransferred))
            {
                //new CRMPaymentDistibutionHelper().UpdateShipmentDistribution(record);
                new CRMPaymentDistibutionHelper().UpdateCRMDepositDistribution(record);
                return Json(new { Message = $"Distribution Generated for Deposit number {record.CRMDepositId}.", Status = "OK" }, JsonRequestBehavior.AllowGet);
            }
            if (record == null)
            {
                var d = new
                {
                    Message = "Invoice not found!",
                    Status = "NOT_OK"
                };
                return Json(d, JsonRequestBehavior.AllowGet);
            }
            if (record.IsOnlineTransferred)
            {
                var formattedData = new
                {
                    Message = "The record is already transfered you cannot regenerate distribution.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }


            return null;
        }
        public JsonResult SendForApproval(int id)
        {
            var record = db.CRMDeposit.Find(id);
            if ((record != null && !record.IsSendForApproval && !record.IsOnlineApproved))
            {
                #region validate line items
                var lineItems = db.CRMDepositLine.Where(li => li.CRMDepositId == record.CRMDepositId).ToList();
                if (!lineItems.Any())
                {
                    var d = new
                    {
                        Message = "No Line Items found. Please add Line Items first",
                        Status = "NOT_OK"
                    };
                    return Json(d, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //foreach (De lineItem in lineItems)
                    //{
                    //    if (lineItem.TotalQuantity - (lineItem.ShipedQuantity + lineItem.ShipedQuantityTilPreShipment) < 0)
                    //    {
                    //        return Json(new { Message = "Some line items have invalid shipment quantity.", Status = "NOT_OK" });
                    //    }
                    //}
                }
                #endregion

                record.Status = OperationStatuses.WaitingApproval.ToString();
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                db.CRMDeposit.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new { Message = "Successfully Sent.", Status = "OK" }, JsonRequestBehavior.AllowGet);
            }
            if (record == null)
            {
                var d = new
                {
                    Message = "Deposit record not found!",
                    Status = "OK"
                };
                return Json(d, JsonRequestBehavior.AllowGet);
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

        [AuthorizeRoles(CRMRoles.CRMVoid)]
        public JsonResult Void(int id)
        {
            var record = db.CRMDeposit.Find(id);

            if (record != null && record.IsOnlineApproved && !record.IsVoid && !record.IsOnlineTransferred)
            {
                record.Status = OperationStatuses.Void.ToString();
                record.IsOnlineApproved = false;
                record.IsSendForApproval = false;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                record.IsVoid = true;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;

                db.CRMDeposit.Attach(record);
                db.Entry(record).State = EntityState.Modified;

                #region remove validation lines
                var validationLine = db.CRMDepositValidation.Where(l => l.CRMDepositId == record.CRMDepositId).ToList();
                db.CRMDepositValidation.RemoveRange(validationLine);
                #endregion

                db.SaveChanges();

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
                    Message = "The record was not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IsVoid)
            {
                var formattedData = new
                {
                    Message = "The record is already void. You can not void again.",
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
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
