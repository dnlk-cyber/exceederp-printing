using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.GL
{
    public class GlRecordJournalController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        readonly FinanceHelper _financeHelper = new FinanceHelper();


        [AuthorizeRoles(FinanceConstants.GeneralLedgerUser ,FinanceConstants.GeneralLedgerApprover  , FinanceConstants.GeneralLedgerView)]
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            
            var categories = _db.GLJournalCategoriers;

            ViewData["categories"] = categories
                .Select(e => new 
                {
                    Category = e.Category
                })
                .AsQueryable();

       
            var periods = _db.GLPeriods.Where(p => p.Status == FiscalYearStatus.Open).ToList();

            ViewData["periods"] = periods
                .Select(e => new 
                {
                    Name = e.Name
                })
                .AsQueryable();
        }

        [AuthorizeRoles(FinanceConstants.GeneralLedgerUser , FinanceConstants.GeneralLedgerApprover ,FinanceConstants.GeneralLedgerView)]
        public ActionResult GlRecordJournals_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);

            IQueryable<GlRecordJournal> glRecordJournals = _db.GLRecordJournals
                .Where(x => x.IsOnlineApproved == false);

            //if (!((System.Web.HttpContext.Current.User.IsInRole(FinanceConstants.GeneralLedgerApprover)) 
            //    || (System.Web.HttpContext.Current.User.IsInRole(FinanceConstants.GeneralLedgerView))))
            //{
            //    glRecordJournals = glRecordJournals.Where(x => mybraches.Contains(x.OrgBranchName));
            //}

            if (System.Web.HttpContext.Current.User.IsInRole(FinanceConstants.GeneralLedgerAdmin))
               glRecordJournals = glRecordJournals.Where(x => mybraches.Contains(x.OrgBranchName) ||x.OrgBranchName==null || x.OrgBranchName == "");
            else
                glRecordJournals = glRecordJournals.Where(x => mybraches.Contains(x.OrgBranchName));

            DataSourceResult result = glRecordJournals.ToDataSourceResult(request, glRecordJournal => new
            {
                GlRecordJournalId = glRecordJournal.GlRecordJournalId,
                BranchId = glRecordJournal.BranchId,
                JournalName = glRecordJournal.JournalName,
                Description = glRecordJournal.Description,
                FiscalYear = glRecordJournal.FiscalYear,
                PeriodName = glRecordJournal.PeriodName,
                CategoryName = glRecordJournal.CategoryName,
                SourcesName = glRecordJournal.SourcesName,
                JournalNumber = glRecordJournal.JournalNumber,
                EffectiveDate = glRecordJournal.EffectiveDate,
                DebitSum = glRecordJournal.DebitSum,
                Posting = glRecordJournal.Posting,
                isReversed = glRecordJournal.isReversed,
                IsPostedToSecondLedger = glRecordJournal.IsPostedToSecondLedger,
                IsVoidDoc = glRecordJournal.IsVoidDoc,
                Remark = glRecordJournal.Remark,
                ReferenceDoc = glRecordJournal.ReferenceDoc,
                PaymentReference = glRecordJournal.PaymentReference,
                InvoiceReferenceDoc = glRecordJournal.InvoiceReferenceDoc,

                IsSendForApproval = glRecordJournal.IsSendForApproval,
                Status = glRecordJournal.Status,
                isPosted = glRecordJournal.isPosted,
                PostedBy = glRecordJournal.PostedBy,
                PostedTime = glRecordJournal.PostedTime,
                IsOnlineApproved = glRecordJournal.IsOnlineApproved,
                OnlineApprovedBy = glRecordJournal.OnlineApprovedBy,
                OnlineApprovedTime = glRecordJournal.OnlineApprovedTime,
                isVoid = glRecordJournal.isVoid,
                VoidBy = glRecordJournal.VoidBy,
                VoidTime = glRecordJournal.VoidTime,
                OrgBranchName =glRecordJournal.OrgBranchName,
                DateCreated = glRecordJournal.DateCreated,
                LastModified = glRecordJournal.LastModified,
                CreatedBy = glRecordJournal.CreatedBy,
                ModifiedBy = glRecordJournal.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FinanceConstants.GeneralLedgerUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GlRecordJournals_Create([DataSourceRequest]DataSourceRequest request, GlRecordJournal glRecordJournal)
        {
           
            string periodYear =_db.GLPeriods.Where(p => p.Name == glRecordJournal.PeriodName).Select(period => period.GlFiscalYear.Name).FirstOrDefault();


            string thisYear =_db.GLFiscalYears.Where(p => p.Name == periodYear).Select(year => year.Name).FirstOrDefault();
            #region validation
            var glPeriod = _db.GLPeriods.FirstOrDefault(x => x.Name == glRecordJournal.PeriodName);
            if (glPeriod != null)
            {
                if (!((glRecordJournal.EffectiveDate.Date >= glPeriod.DateFrom) && (glRecordJournal.EffectiveDate.Date <= glPeriod.DateTo)))
                    ModelState.AddModelError("", "Effective Date should be within the period: " + glRecordJournal.PeriodName + "(From " + glPeriod.DateFrom.ToShortDateString() + " To " + glPeriod.DateTo.ToShortDateString() + ")");

            }
            else
            {
                ModelState.AddModelError("", "The selected Period is not defined.");
            }

            var exists = _db.GLRecordJournals.Where(x => x.JournalNumber == glRecordJournal.JournalNumber).ToList();

            if (exists.Any())
                ModelState.AddModelError("exist", glRecordJournal.JournalNumber + " already exists in the system.");

            #endregion

            if (ModelState.IsValid)
            {

                //var currentPeriod = _db.GLPeriods.Where(x => x.DateFrom <= DateTime.Today).FirstOrDefault(x => x.DateTo > DateTime.Today);
                //if (glPeriod != null && (currentPeriod != null && glPeriod.GLPeriodId != currentPeriod.GLPeriodId))
                //{
                //    if (glPeriod.DateTo < currentPeriod.DateFrom)
                //    {
                //        ModelState.AddModelError("previous", "you are recording in previous period");
                //    }
                //    else if (glPeriod.DateFrom > currentPeriod.DateFrom)
                //    {
                //        ModelState.AddModelError("future", "you are recording in future period");
                //    }

                //}
                var entity = new GlRecordJournal
                {
                    JournalName = glRecordJournal.JournalName,
                    Description = glRecordJournal.Description,
                    FiscalYear = thisYear,
                    PeriodName = glRecordJournal.PeriodName,
                    CategoryName = glRecordJournal.CategoryName,
                    SourcesName = "GL",
                    JournalNumber = glRecordJournal.JournalNumber,
                    EffectiveDate = glRecordJournal.EffectiveDate.Date,
                    Posting = glRecordJournal.Posting,
                    isReversed = glRecordJournal.isReversed,
                    IsSendForApproval = glRecordJournal.IsSendForApproval,
                    isPosted = glRecordJournal.isPosted,
                    PostedBy = glRecordJournal.PostedBy,
                    PostedTime = glRecordJournal.PostedTime,
                    IsPostedToSecondLedger = glRecordJournal.IsPostedToSecondLedger,
                    IsVoidDoc = glRecordJournal.IsVoidDoc,
                    Remark = glRecordJournal.Remark,
                    ReferenceDoc = glRecordJournal.ReferenceDoc,
                    PaymentReference = glRecordJournal.PaymentReference,
                    InvoiceReferenceDoc = glRecordJournal.InvoiceReferenceDoc,
                    Status = "Editing",
                    IsOnlineApproved = glRecordJournal.IsOnlineApproved,
                    OnlineApprovedBy = glRecordJournal.OnlineApprovedBy,
                    OnlineApprovedTime = glRecordJournal.OnlineApprovedTime,
                    isVoid = glRecordJournal.isVoid,
                    VoidBy = glRecordJournal.VoidBy,
                    VoidTime = glRecordJournal.VoidTime,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    BranchId = UserInformationHelper.GetUserDefaultBranchId(User.Identity.Name),
                    DateCreated = DateTime.Now,
                    LastModified = glRecordJournal.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = glRecordJournal.ModifiedBy
                };

                _db.GLRecordJournals.Add(entity);
                _db.SaveChanges();
                glRecordJournal.GlRecordJournalId = entity.GlRecordJournalId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GL record journal", "Success", User.Identity.Name, $"created journal {entity.GlRecordJournalId}", Modules.Finance);
                #endregion
            }

            return Json(new[] { glRecordJournal }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.GeneralLedgerUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GlRecordJournals_Update([DataSourceRequest]DataSourceRequest request, GlRecordJournal glRecordJournal)
        {
            string periodYear = _db.GLPeriods.Where(p => p.Name == glRecordJournal.PeriodName).Select(period => period.GlFiscalYear.Name).SingleOrDefault();

            string thisYear = _db.GLFiscalYears.Where(p => p.Name == periodYear).Select(year => year.Name).SingleOrDefault();

            bool isvalid = true;
            #region validation
            var glPeriod = _db.GLPeriods.FirstOrDefault(x => x.Name == glRecordJournal.PeriodName);
            if (glPeriod != null)
            {
                if (!((glRecordJournal.EffectiveDate.Date >= glPeriod.DateFrom) && (glRecordJournal.EffectiveDate.Date <= glPeriod.DateTo)))
                    ModelState.AddModelError("", "Effective Date should be within the period: " + glRecordJournal.PeriodName + "(From " + glPeriod.DateFrom.ToShortDateString() + " To " + glPeriod.DateTo.ToShortDateString() + ")");

            }
            else
            {
                ModelState.AddModelError("", "The selected Period is not defined.");
            }

            var exists = _db.GLRecordJournals.Where(x => x.GlRecordJournalId != glRecordJournal.GlRecordJournalId).Where(x => x.JournalNumber == glRecordJournal.JournalNumber).ToList();

            if (exists.Any())
                ModelState.AddModelError("exist", glRecordJournal.JournalNumber + " already exists in the system.");

            #endregion
            if (ModelState.IsValid)
            {

                #region validated record
                var parent = _db.GLRecordJournals.Find(glRecordJournal.GlRecordJournalId);
                if (parent != null)
                {
                    if (parent.IsOnlineApproved)
                    {
                        isvalid = false;
                        ModelState.AddModelError("approved", "this record is approved and cannot be updated.");
                    }

                    #region detach local version
                    var local = _db.Set<GlRecordJournal>().Local.FirstOrDefault(f => f.GlRecordJournalId == parent.GlRecordJournalId);
                    if (local != null)
                    {
                        _db.Entry(local).State = EntityState.Detached;
                    }
                    #endregion

                }


                #endregion

                if (isvalid)
                {
                    #region validation

                    //var period = _db.GLPeriods.FirstOrDefault(x => x.Name == glRecordJournal.PeriodName);
                    //if (period != null)
                    //{
                    //    if (
                    //        !((glRecordJournal.EffectiveDate >= period.DateFrom) &&
                    //          (glRecordJournal.EffectiveDate <= period.DateTo)))
                    //        ModelState.AddModelError("",
                    //            "Effective Date should be within the period: " + glRecordJournal.PeriodName + "(From " +
                    //            period.DateFrom.ToShortDateString() + " To " + period.DateTo.ToShortDateString() + ")");
                    //}
                    //else
                    //{
                    //    ModelState.AddModelError("", "The selected Period is not defined.");
                    //}

                    #endregion

                    //var currentPeriod = _db.GLPeriods.Where(x => x.DateFrom <= DateTime.Today).FirstOrDefault(x => x.DateTo > DateTime.Today);
                    //if (glPeriod != null && (currentPeriod != null && glPeriod.GLPeriodId != currentPeriod.GLPeriodId))
                    //{
                    //    if (glPeriod.DateTo < currentPeriod.DateFrom)
                    //    {
                    //        ModelState.AddModelError("previous", "you are recording in previous period");
                    //    }
                    //    else if (glPeriod.DateFrom > currentPeriod.DateFrom)
                    //    {
                    //        ModelState.AddModelError("future", "you are recording in future period");
                    //    }

                    //}

                    #region update sum
                    var lines = _db.GLRecordJournalDetails.Where(x=>x.GlRecordJournalId==glRecordJournal.GlRecordJournalId);
                    glRecordJournal.DebitSum = lines.Sum(x => x.Debit);
                    #endregion

                    var entity = new GlRecordJournal
                    {
                        GlRecordJournalId = glRecordJournal.GlRecordJournalId,
                        BranchId=glRecordJournal.BranchId,
                        JournalName = glRecordJournal.JournalName,
                        Description = glRecordJournal.Description,
                        FiscalYear = thisYear,
                        PeriodName = glRecordJournal.PeriodName,
                        CategoryName = glRecordJournal.CategoryName,
                        SourcesName = glRecordJournal.SourcesName,
                        JournalNumber = glRecordJournal.JournalNumber,
                        EffectiveDate = glRecordJournal.EffectiveDate,
                        Posting = glRecordJournal.Posting,
                        isReversed = glRecordJournal.isReversed,
                        isPosted = glRecordJournal.isPosted,
                        PostedBy = glRecordJournal.PostedBy,
                        PostedTime = glRecordJournal.PostedTime,
                        IsPostedToSecondLedger = glRecordJournal.IsPostedToSecondLedger,
                        IsVoidDoc = glRecordJournal.IsVoidDoc,
                        DebitSum= glRecordJournal.DebitSum,
                        Remark = glRecordJournal.Remark,
                        ReferenceDoc = glRecordJournal.ReferenceDoc,
                        PaymentReference = glRecordJournal.PaymentReference,
                        InvoiceReferenceDoc = glRecordJournal.InvoiceReferenceDoc,

                        IsSendForApproval = glRecordJournal.IsSendForApproval,
                        Status = glRecordJournal.Status,
                        IsOnlineApproved = glRecordJournal.IsOnlineApproved,
                        OnlineApprovedBy = glRecordJournal.OnlineApprovedBy,
                        OnlineApprovedTime = glRecordJournal.OnlineApprovedTime,
                        isVoid = glRecordJournal.isVoid,
                        VoidBy = glRecordJournal.VoidBy,
                        VoidTime = glRecordJournal.VoidTime,
                        OrgBranchName = glRecordJournal.OrgBranchName,
                        DateCreated = glRecordJournal.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = glRecordJournal.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    _db.GLRecordJournals.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "GL record journal", "Success", User.Identity.Name, $"updated journal {entity.GlRecordJournalId}", Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { glRecordJournal }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.GeneralLedgerUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GlRecordJournals_Destroy([DataSourceRequest]DataSourceRequest request, GlRecordJournal glRecordJournal)
        {
            bool isvalid = true;
            if (ModelState.IsValid)
            {
                #region validated record

                var parent = _db.GLRecordJournals.Find(glRecordJournal.GlRecordJournalId);
                if (parent != null)
                {
                    if (parent.IsOnlineApproved)
                    {
                        isvalid = false;
                        //ViewBag.Note = "You cannot manipulate this record";
                        //return View("Error");
                    }

                    #region detach local version
                    var local = _db.Set<GlRecordJournal>()
                        .Local
                        .FirstOrDefault(f => f.GlRecordJournalId == parent.GlRecordJournalId);
                    if (local != null)
                    {
                        _db.Entry(local).State = EntityState.Detached;
                    }
                    #endregion

                }


                #endregion

                if (isvalid)
                {
                    var entity = new GlRecordJournal
                    {
                        GlRecordJournalId = glRecordJournal.GlRecordJournalId,
                        BranchId = glRecordJournal.BranchId,
                        JournalName = glRecordJournal.JournalName,
                        Description = glRecordJournal.Description,
                        FiscalYear = glRecordJournal.FiscalYear,
                        PeriodName = glRecordJournal.PeriodName,
                        CategoryName = glRecordJournal.CategoryName,
                        SourcesName = glRecordJournal.SourcesName,
                        JournalNumber = glRecordJournal.JournalNumber,
                        EffectiveDate = glRecordJournal.EffectiveDate,
                        Posting = glRecordJournal.Posting,
                        isReversed = glRecordJournal.isReversed,
                        isPosted = glRecordJournal.isPosted,
                        PostedBy = glRecordJournal.PostedBy,
                        PostedTime = glRecordJournal.PostedTime,
                        IsPostedToSecondLedger = glRecordJournal.IsPostedToSecondLedger,
                        Remark = glRecordJournal.Remark,

                        IsSendForApproval = glRecordJournal.IsSendForApproval,
                        Status = glRecordJournal.Status,
                        IsOnlineApproved = glRecordJournal.IsOnlineApproved,
                        OnlineApprovedBy = glRecordJournal.OnlineApprovedBy,
                        OnlineApprovedTime = glRecordJournal.OnlineApprovedTime,
                        isVoid = glRecordJournal.isVoid,
                        VoidBy = glRecordJournal.VoidBy,
                        VoidTime = glRecordJournal.VoidTime,
                        OrgBranchName = glRecordJournal.OrgBranchName,
                        DateCreated = glRecordJournal.DateCreated,
                        LastModified = glRecordJournal.LastModified,
                        CreatedBy = glRecordJournal.CreatedBy,
                        ModifiedBy = glRecordJournal.ModifiedBy
                    };

                    #region log

                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name,
                        "Deleted journal number: " + entity.GlRecordJournalId + "-" + entity.JournalName,Modules.Finance);

                    #endregion

                    _db.GLRecordJournals.Attach(entity);
                    _db.GLRecordJournals.Remove(entity);
                    _db.SaveChanges();
                }
            }

            return Json(new[] { glRecordJournal }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.GeneralLedgerUser , FinanceConstants.GeneralLedgerApprover , FinanceConstants.GeneralLedgerView)]
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        [AuthorizeRoles(FinanceConstants.GeneralLedgerUser , FinanceConstants.GeneralLedgerApprover , FinanceConstants.GeneralLedgerView)]
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        
        [AuthorizeRoles(FinanceConstants.GeneralLedgerUser)]
        public JsonResult SendforApproval(string id)
        {
            string errMsg = "";
            int myid = int.Parse(id);
            var record = _db.GLRecordJournals.Find(myid);

            if (record != null && record.IsOnlineApproved)
            {
                errMsg = "You cannot manipulate this record";
                var formattedData = new
                {
                    Message = errMsg,
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record != null && record.IsSendForApproval)
            {
                errMsg = "This record is already sent for approval.";
                var formattedData = new
                {
                    Message = errMsg,
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record != null && !record.IsVoidDoc)
            {
                var lines = _db.GLRecordJournalDetails.Where(x => x.GlRecordJournalId == myid).AsNoTracking();

                #region validated lines

                #region check valida journal
                AccountHelper accountHelper = new AccountHelper();
                string message =lines.Any()? accountHelper.IsValidJournal(lines) :"Please add account transaction first";
                if (!String.IsNullOrEmpty(message))
                {
                    var formattedData = new
                    {
                        Message = message,
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }


                #endregion


                #endregion
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                _db.GLRecordJournals.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();
                var formattedData1 = new
                {
                    Message = "Successfully Sent.",
                    Status = "OK"
                };
                return Json(formattedData1, JsonRequestBehavior.AllowGet);
            }
            if (record != null && record.IsVoidDoc)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                _db.GLRecordJournals.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();
                var formattedData = new
                {
                    Message = "Successfully Sent.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            return null;

        }

        [AuthorizeRoles(FinanceConstants.GeneralLedgerUser)]
        public JsonResult Reverse(string id)
        {
            bool isValid = true;
            string errMsg = "";
            int myid = int.Parse(id);
            GlRecordJournal record = _db.GLRecordJournals.Find(myid);
            if (!record.IsOnlineApproved)
            {
                isValid = false;
                errMsg = "You cannot reverse this record since it is not approved.";
                var formattedData = new
                {
                    Message = errMsg,
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record.isReversed)
            {
                isValid = false;
                errMsg = "This record is reversed once.Yo cannot reverse it again.";
                var formattedData = new
                {
                    Message = errMsg,
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (isValid)
            {

                //record reversed
                List<GLRecordJournalDetail> list = new List<GLRecordJournalDetail>();
                list.AddRange(record.GLRecordJournalDetail);

                GlRecordJournal obj = new GlRecordJournal
                {
                    BranchId = record.BranchId,
                    JournalName = "REVERSE-" + record.JournalName,
                    Description = "REVERSE-" + record.Description,
                    JournalNumber = "REVERSE-" + record.JournalNumber,
                    EffectiveDate = record.EffectiveDate,
                    CategoryName = record.CategoryName,
                    SourcesName = record.SourcesName,
                    FiscalYear = record.FiscalYear,
                    PeriodName = record.PeriodName,
                    Status = "Reversed",
                    Posting = record.Posting,
                    isReversed = false,
                    IsOnlineApproved = false,
                    OnlineApprovedBy = "",
                    OnlineApprovedTime = record.OnlineApprovedTime,
                    CreatedBy = User.Identity.Name,
                    OrgBranchName=record.OrgBranchName,
                };
                ;
                obj.DateCreated = DateTime.Now;
                _db.GLRecordJournals.Add(obj);
               // _db.SaveChanges();
                //reverse debit credit
                decimal temp = 0;

                foreach (var i in list)
                {
                    temp = i.Debit;
                    i.Debit = i.Credit;
                    i.Credit = temp;
                    i.GlRecordJournalId = obj.GlRecordJournalId;
                    _db.GLRecordJournalDetails.Add(i);
                    //_db.SaveChanges();
                }
                //update original
                record.isReversed = true;
                _db.GLRecordJournals.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();

                #region  set transfer status false
                try
                {
                    string type = record.JournalNumber.Split('.')[0];
                    string recordid = record.JournalNumber.Split('.')[1];
                    int did = int.Parse(recordid);
                    switch (type)
                    {
                        case "Issue":                           
                            var issue = _db.Issues.Find(did);
                            issue.IsOnlineTransferred = false;
                            _db.Issues.Attach(issue);
                            _db.Entry(issue).State = EntityState.Modified;
                            _db.SaveChanges();
                            break;
                        case "Issue Fuel":
                            var issueFuel = _db.Issues.Find(did);
                            issueFuel.IsOnlineTransferred = false;
                            _db.Issues.Attach(issueFuel);
                            _db.Entry(issueFuel).State = EntityState.Modified;
                            _db.SaveChanges();
                            break;
                        case "GoodRecive":
                            var goodreceive = _db.GoodReceives.Find(did);
                            goodreceive.IsOnlineTransferred = false;
                            _db.GoodReceives.Attach(goodreceive);
                            _db.Entry(goodreceive).State = EntityState.Modified;
                            _db.SaveChanges();
                            break;
                        case "GoodSellingLending":
                            var sell = _db.GoodSellingAndLendings.Find(did);
                            sell.IsOnlineTransferred = false;
                            _db.GoodSellingAndLendings.Attach(sell);
                            _db.Entry(sell).State = EntityState.Modified;
                            _db.SaveChanges();
                            break;
                        case "Store Return":
                            var storeReturn = _db.StoreReturns.Find(did);
                            storeReturn.IsOnlineTransferred = false;
                            _db.StoreReturns.Attach(storeReturn);
                            _db.Entry(storeReturn).State = EntityState.Modified;
                            _db.SaveChanges();
                            break;
                        case "Transfer Receive":
                            var treceive = _db.ItemTransferReceive.Find(did);
                            treceive.IsOnlineTransferred = false;
                            _db.ItemTransferReceive.Attach(treceive);
                            _db.Entry(treceive).State = EntityState.Modified;
                            _db.SaveChanges();
                            break;
                        case "Transfer Send":
                            var tsend = _db.ItemTransfers.Find(did);
                            tsend.IsOnlineTransferred = false;
                            _db.ItemTransfers.Attach(tsend);
                            _db.Entry(tsend).State = EntityState.Modified;
                            _db.SaveChanges();
                            break;
                        case "Adjustment":
                            var adjustment = _db.StockAdjustments.Find(did);
                            adjustment.IsOnlineTransferred = false;
                            _db.StockAdjustments.Attach(adjustment);
                            _db.Entry(adjustment).State = EntityState.Modified;
                            _db.SaveChanges();
                            break;
                        case "APDPAY":
                            var dirpay = _db.ApDirectPayments.Find(did);
                            dirpay.IsOnlineTransferred = false;
                            _db.ApDirectPayments.Attach(dirpay);
                            _db.Entry(dirpay).State = EntityState.Modified;
                            _db.SaveChanges();
                            break;
                        case "APPAY":
                            var preparpay = _db.APPayments.Find(did);
                            preparpay.IsOnlineTransferred = false;
                            _db.APPayments.Attach(preparpay);
                            _db.Entry(preparpay).State = EntityState.Modified;
                            _db.SaveChanges();
                            break;
                    }
                }
                catch(Exception ex)
                {

                }
                #endregion 
            }

            #region operation log
            UserHelper.OperationLog(Request.UserHostAddress, "journal reverse", "Success", User.Identity.Name, $"reverse journal  {id} ", Modules.Finance);
            #endregion
            var formattedData1 = new
            {
                Message = "Reverse completed.",
                Status = "OK"
            };
            return Json(formattedData1, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
