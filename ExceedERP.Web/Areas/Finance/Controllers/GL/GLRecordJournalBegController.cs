using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Hangfire;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.GL
{
    public class GLRecordJournalBegController : Controller
    {
        ExceedDbContext _db = new ExceedDbContext();
        private readonly AccountHelper accountHelper = new AccountHelper();
        private readonly CacheData CacheData = new CacheData();

        [AuthorizeRoles(FinanceConstants.GeneralLedgerUser ,FinanceConstants.GeneralLedgerApprover  , FinanceConstants.GeneralLedgerView)]
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {

            ViewData["categories"] = CacheData.GetJournalCategoriesCaches().Select(
               e => new
               {
                   Category = e.Name
               })
               .AsQueryable();


            ViewData["periods"] = CacheData.GetPeriodsCaches().Select(
               i => new
               {
                   Name = i.Name,
               }).AsQueryable();
        }

        [AuthorizeRoles(FinanceConstants.GeneralLedgerUser , FinanceConstants.GeneralLedgerApprover ,FinanceConstants.GeneralLedgerView)]
        public ActionResult GLRecordJournalBegs_Read([DataSourceRequest]DataSourceRequest request)
        {
            
            //if(!_db.GlRecordJournalBegs.Any())
            //{
            //    #region create beginning header
            //    var begPeriod = _db.GlBeginningPeriods.FirstOrDefault();
                
            //    if (begPeriod != null)
            //    {
            //        var period = _db.GLPeriods.Find(begPeriod.GLPeriodId);
            //        var year = _db.GLFiscalYears.Find(begPeriod.GlFiscalYearId);
            //        var entity = new GLRecordJournalBeg
            //        {
            //            JournalName = "Beginning Journal",
            //            Description = "Beginning Journal",
            //            FiscalYear = year?.Name,
            //            PeriodName = period?.Name,
            //            CategoryName = "Beginning Journal",
            //            SourcesName = "GL",
            //            JournalNumber = "Beginning Journal-"+ period?.Name,
            //            EffectiveDate = period.DateFrom,

            //            Status = "Editing",
            //            DateCreated = DateTime.Now,
            //            CreatedBy = User.Identity.Name
            //        };

            //        _db.GlRecordJournalBegs.Add(entity);
            //        _db.SaveChanges();
            //    }
            //    #endregion
            //}

            IQueryable<GLRecordJournalBeg> glRecordJournals = _db.GlRecordJournalBegs;

            DataSourceResult result = glRecordJournals.ToDataSourceResult(request, glRecordJournal => new
            {
                GLRecordJournalBegId = glRecordJournal.GLRecordJournalBegId,
                BranchId = glRecordJournal.BranchId,
                JournalName = glRecordJournal.JournalName,
                Description = glRecordJournal.Description,
                FiscalYear = glRecordJournal.FiscalYear,
                PeriodName = glRecordJournal.PeriodName,
                CategoryName = glRecordJournal.CategoryName,
                SourcesName = glRecordJournal.SourcesName,
                JournalNumber = glRecordJournal.JournalNumber,
                EffectiveDate = glRecordJournal.EffectiveDate,
                //DebitSum = glRecordJournal.GLRecordJournalBegDistributions.Sum(x=>x.Debit),
                Posting = glRecordJournal.Posting,
                isReversed = glRecordJournal.isReversed,
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
        public ActionResult GLRecordJournalBegs_Create([DataSourceRequest]DataSourceRequest request, GLRecordJournalBeg glRecordJournal)
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

            var exists = _db.GlRecordJournalBegs.Where(x => x.JournalNumber == glRecordJournal.JournalNumber).ToList();

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
                var entity = new GLRecordJournalBeg
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

                _db.GlRecordJournalBegs.Add(entity);
                _db.SaveChanges();
                glRecordJournal.GLRecordJournalBegId = entity.GLRecordJournalBegId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GL record beginning journal", "Success", User.Identity.Name, $"created journal {entity.GLRecordJournalBegId}", Modules.Finance);
                #endregion
            }

            return Json(new[] { glRecordJournal }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.GeneralLedgerUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GLRecordJournalBegs_Update([DataSourceRequest]DataSourceRequest request, GLRecordJournalBeg glRecordJournal)
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

            var exists = _db.GlRecordJournalBegs.Where(x => x.GLRecordJournalBegId != glRecordJournal.GLRecordJournalBegId).Where(x => x.JournalNumber == glRecordJournal.JournalNumber).ToList();

            if (exists.Any())
                ModelState.AddModelError("exist", glRecordJournal.JournalNumber + " already exists in the system.");

            #endregion
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
            if (ModelState.IsValid)
            {

                #region validated record
                var parent = _db.GlRecordJournalBegs.Find(glRecordJournal.GLRecordJournalBegId);
                if (parent != null)
                {
                    if (parent.IsOnlineApproved)
                    {
                        isvalid = false;
                        ModelState.AddModelError("approved", "this record is approved and cannot be updated.");
                    }

                    #region detach local version
                    var local = _db.Set<GLRecordJournalBeg>().Local.FirstOrDefault(f => f.GLRecordJournalBegId == parent.GLRecordJournalBegId);
                    if (local != null)
                    {
                        _db.Entry(local).State = EntityState.Detached;
                    }
                    #endregion

                }


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

                    var entity = new GLRecordJournalBeg
                    {
                        GLRecordJournalBegId = glRecordJournal.GLRecordJournalBegId,
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
                        IsVoidDoc = glRecordJournal.IsVoidDoc,
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

                    _db.GlRecordJournalBegs.Attach(entity);
                    _db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "GL record beginning journal", "Success", User.Identity.Name, $"updated journal {entity.GLRecordJournalBegId}", Modules.Finance);
                    #endregion
                
            }

            return Json(new[] { glRecordJournal }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.GeneralLedgerUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GLRecordJournalBegs_Destroy([DataSourceRequest]DataSourceRequest request, GLRecordJournalBeg glRecordJournal)
        {
            bool isvalid = true;
            if (ModelState.IsValid)
            {
                #region validated record

                var parent = _db.GlRecordJournalBegs.Find(glRecordJournal.GLRecordJournalBegId);
                if (parent != null)
                {
                    if (parent.IsOnlineApproved)
                    {
                        isvalid = false;
                        //ViewBag.Note = "You cannot manipulate this record";
                        //return View("Error");
                    }

                    #region detach local version
                    var local = _db.Set<GLRecordJournalBeg>()
                        .Local
                        .FirstOrDefault(f => f.GLRecordJournalBegId == parent.GLRecordJournalBegId);
                    if (local != null)
                    {
                        _db.Entry(local).State = EntityState.Detached;
                    }
                    #endregion

                }


                #endregion

                if (isvalid)
                {
                    var entity = new GLRecordJournalBeg
                    {
                        GLRecordJournalBegId = glRecordJournal.GLRecordJournalBegId,
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
                        "Deleted journal number: " + entity.GLRecordJournalBegId + "-" + entity.JournalName);

                    #endregion

                    _db.GlRecordJournalBegs.Attach(entity);
                    _db.GlRecordJournalBegs.Remove(entity);
                    _db.SaveChanges();
                }
            }

            return Json(new[] { glRecordJournal }.ToDataSourceResult(request, ModelState));
        }
         
       
        [AuthorizeRoles(FinanceConstants.GeneralLedgerBeginning)]
        public JsonResult Refresh(string id)
        {
            string errMsg = "";
            int myid = int.Parse(id);
            var record = _db.GlRecordJournalBegs.Find(myid);

            if (record != null)
            {
                var fiscalYear = _db.GLFiscalYears.Where(x => x.Name == record.FiscalYear).FirstOrDefault();
                if (fiscalYear != null)
                {
                    if (fiscalYear.Status == FiscalYearStatus.Close || fiscalYear.Status == FiscalYearStatus.PermanetlyClosed)
                    {
                        errMsg = "The fiscal is closed, you can't update a closed period.";
                        var formattedData = new
                        {
                            Message = errMsg,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }
                }
            }

            if (record != null && record.IsOnlineApproved)
            {
                errMsg = "This record is already closed.";
                var formattedData = new
                {
                    Message = errMsg,
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record != null && ! record.IsOnlineApproved)
            {
                var glperiod = _db.GLPeriods.Where(x => x.Name == record.PeriodName).FirstOrDefault();
                if(glperiod!=null)
                {
                    var beglines = _db.GLLedgerPostingLines
                                     .Where(x => x.GLPeriodId == glperiod.GLPeriodId)
                                     .Where(x => x.GLLedgerPosting.AccountType != GlAccountTypes.Expense)
                                     .Where(x => x.GLLedgerPosting.AccountType != GlAccountTypes.Revenue)
                                     .Where(x=>x.Debit!=0 ||x.Credit!=0)
                                     .Where(x => x.Type == LedgerPostTypes.Begining).AsQueryable();

                    var lines = _db.GLRecordJournalBegDistributions.Where(x => x.GLRecordJournalBegId == myid);
                    _db.GLRecordJournalBegDistributions.RemoveRange(lines);
                    _db.SaveChanges();

                    foreach(var line in beglines)
                    {
                        ExceedDbContext dbContext = new ExceedDbContext();
                        decimal debit = 0;
                        decimal credit = 0;

                        if (line.GLLedgerPosting.AccountType == GlAccountTypes.Asset ||
                                line.GLLedgerPosting.AccountType == GlAccountTypes.Expense)
                        {
                            if(line.Balance>0)
                            {
                                debit = line.Balance;
                                credit = 0;
                            }
                            else
                            {
                                debit = 0;
                                credit = Math.Abs(line.Balance);
                            }

                        }
                        else if (line.GLLedgerPosting.AccountType == GlAccountTypes.Liability ||
                                 line.GLLedgerPosting.AccountType == GlAccountTypes.Capital ||
                                 line.GLLedgerPosting.AccountType == GlAccountTypes.Revenue)
                        {
                            if (line.Balance > 0)
                            {
                                debit = 0;
                                credit = line.Balance;
                            }
                            else
                            {
                                debit = Math.Abs(line.Balance);
                                credit = 0;
                            }
                        }

                        var dist = new GLRecordJournalBegDistribution
                        {
                            GLRecordJournalBegId = myid,
                            AccountCode = line.GLLedgerPosting.AccountCode,
                            AccountDesc = line.GLLedgerPosting.AccountDesc,
                            Debit = debit,
                            Credit = credit,
                        };

                        dbContext.GLRecordJournalBegDistributions.Add(dist);
                        dbContext.SaveChanges();

                    }
                   
                }

                var jobId = BackgroundJob.Enqueue(() => new AccountHelper().SplitBegJournalAccountCombination());

                var formattedData = new
                {
                    Message = "Operation Complete.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);

            }


            return null;

        }
        [AuthorizeRoles(FinanceConstants.GeneralLedgerBeginning)]
        public JsonResult Close(string id)
        {
            string errMsg = "";
            int myid = int.Parse(id);
            var record = _db.GlRecordJournalBegs.Find(myid);


            if (record != null && record.IsOnlineApproved)
            {
                errMsg = "This record is already closed.";
                var formattedData = new
                {
                    Message = errMsg,
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record != null)
            {
                AccountHelper _accountHelper = new AccountHelper();
                var lines = _db.GLRecordJournalBegDistributions.Where(x => x.GLRecordJournalBegId == myid).AsNoTracking();
                string validForPostError = _accountHelper.IsValidBegJournal(lines);
                if (!string.IsNullOrEmpty(validForPostError))
                {
                    var formattedData = new
                    {
                        Message = validForPostError,
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsOnlineApproved = true;
                record.OnlineApprovedBy = User.Identity.Name;
                record.OnlineApprovedTime = DateTime.Now;
                _db.GlRecordJournalBegs.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();
                var formattedData1 = new
                {
                    Message = "Closed.",
                    Status = "OK"
                };
                return Json(formattedData1, JsonRequestBehavior.AllowGet);
            }


            return null;

        }

        [AuthorizeRoles(FinanceConstants.GeneralLedgerBeginning)]
        public JsonResult Post(string id)
        {
            string errMsg = "";
            int myid = int.Parse(id);
            var record = _db.GlRecordJournalBegs.Find(myid);


            if (record != null && record.isPosted)
            {
                errMsg = "This record is already Posted.";
                var formattedData = new
                {
                    Message = errMsg,
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record != null && record.IsOnlineApproved && ! record.isPosted)
            {
                AccountHelper _accountHelper = new AccountHelper();
                var lines = _db.GLRecordJournalBegDistributions.Where(x => x.GLRecordJournalBegId == myid).AsNoTracking();
                string validForPostError = _accountHelper.IsValidBegJournal(lines);
                if (!string.IsNullOrEmpty(validForPostError))
                {
                    var formattedData = new
                    {
                        Message = validForPostError,
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

                #region post
                Post(record.GLRecordJournalBegId, User.Identity.Name);
                #endregion
                record.Status = Enum.GetName(typeof(OperationStatuses), 15);
                record.isPosted = true;
                record.OnlineApprovedBy = User.Identity.Name;
                record.OnlineApprovedTime = DateTime.Now;
                _db.GlRecordJournalBegs.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();
                var formattedData1 = new
                {
                    Message = "Posted.",
                    Status = "OK"
                };
                return Json(formattedData1, JsonRequestBehavior.AllowGet);
            }


            return null;

        }

        [AuthorizeRoles(FinanceConstants.GeneralLedgerBeginning)]
        public JsonResult OpenOpeningRecord(string id)
        {
            string errMsg = "";
            int myid = int.Parse(id);
            var period = _db.GLPeriods.Find(myid);

            if (period == null)
            {
                errMsg = "The period is not found.";
                var formattedData = new
                {
                    Message = errMsg,
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            var record = _db.GlRecordJournalBegs.Where(x => x.PeriodName == period.Name).FirstOrDefault();

            if (record != null)
            {
                errMsg = "This record is already added, you can modify it.";
                var formattedData = new
                {
                    Message = errMsg,
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record == null)
            {
                var entity = new GLRecordJournalBeg
                {
                    JournalName = "Beginning Journal",
                    Description = "Beginning Journal",
                    FiscalYear = period.GlFiscalYear.Name,
                    PeriodName = period.Name,
                    CategoryName = "Beginning Journal",
                    SourcesName = "GL",
                    JournalNumber = $"Beginning Journal {period.Name}",
                    EffectiveDate = period.DateFrom,
                    Posting = "",//"POSTED",
                    isReversed = false,
                    IsSendForApproval = false,
                    isPosted =true,
                    PostedBy = "",                  
                                   
                    Status = "Editing",
                    IsOnlineApproved = false,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    BranchId = UserInformationHelper.GetUserDefaultBranchId(User.Identity.Name),
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                };

                _db.GlRecordJournalBegs.Add(entity);
                _db.SaveChanges();
               
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GL record beginning journal", "Success", User.Identity.Name, $"created journal {entity.GLRecordJournalBegId}", Modules.Finance);
                #endregion

                errMsg = "Beginning record opened";
                var formattedData = new
                {
                    Message = errMsg,
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }


            return null;

        }

        public ActionResult ImportBeginningLines([DataSourceRequest] DataSourceRequest request, IEnumerable<HttpPostedFileBase> files, int id)
        {
            ExceedDbContext db = new ExceedDbContext();
            db.Configuration.ProxyCreationEnabled = false;
            List<string> errors = new List<string>();
            #region Import

            System.Data.DataSet ds = new System.Data.DataSet();
            var order = db.GLRecordJournalBegDistributions.FirstOrDefault();//.Find(id);
            List<GLRecordJournalBegDistribution> GLRecordJournalBegDistributions = new List<GLRecordJournalBegDistribution>();
            foreach (HttpPostedFileBase fileBase in files)
            {
                string fileExtension = System.IO.Path.GetExtension(Request.Files["files"].FileName);
                if (fileExtension == ".xls" || fileExtension == ".xlsx" || fileExtension == ".xlsm")
                {
                    string fileLocation = Server.MapPath("~/Content/Import/") + DateTime.Now.Year.ToString() +
                                          DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() +
                                          DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() +
                                          DateTime.Now.Second.ToString() + Request.Files["files"].FileName;
                    if (System.IO.File.Exists(fileLocation))
                    {
                        System.IO.File.Delete(fileLocation);
                    }
                    Request.Files["files"].SaveAs(fileLocation);
                    var excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation +
                                                   ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";

                    if (fileExtension == ".xls")
                    {
                        excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation +
                                                ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                    }
                    //connection String for xlsx file format.
                    else if (fileExtension == ".xlsx")
                    {

                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation +
                                                ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    }
                    else if (fileExtension == ".xlsm")
                    {

                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation +
                                                ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    }
                    //Create Connection to Excel work book and add oledb namespace
                    OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                    excelConnection.Open();
                    System.Data.DataTable dt = new System.Data.DataTable();

                    dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    if (dt == null)
                    {
                        return null;
                    }

                    String[] excelSheets = new String[dt.Rows.Count];
                    int t = 0;
                    //excel data saves in temp file here.
                    foreach (System.Data.DataRow row in dt.Rows)
                    {
                        excelSheets[t] = row["TABLE_NAME"].ToString();
                        t++;
                    }
                    OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);


                    string query = string.Format("Select * from [{0}]", excelSheets[0]);
                    using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                    {
                        dataAdapter.Fill(ds);
                    }

                    #region collect data
                    List<GLRecordJournalBegDistribution> imports = new List<GLRecordJournalBegDistribution>();
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        decimal debit = 0;
                        decimal credit = 0;
                        for (int j = 0; j <= ds.Tables[0].Rows.Count - 1; j++)
                        {
                            string accountcode = ds.Tables[0].Rows[j].ItemArray[0].ToString();
                            string accoudesc = ds.Tables[0].Rows[j].ItemArray[1].ToString();
                            try
                            {
                                debit = decimal.Parse(ds.Tables[0].Rows[j].ItemArray[2].ToString());
                            }
                            catch(Exception e)
                            {
                                ModelState.AddModelError("debit", "Debit should be a number " + j.ToString());
                            }

                            try
                            {
                                credit = decimal.Parse(ds.Tables[0].Rows[j].ItemArray[3].ToString());
                            }
                            catch (Exception e)
                            {
                                ModelState.AddModelError("credi", "Credit should be a number " + j.ToString());
                            }     
                            var import = new GLRecordJournalBegDistribution
                            {
                                GLRecordJournalBegId = id,
                                AccountCode =accountcode,
                                AccountDesc = accoudesc,
                                Debit = debit,
                                Credit = credit,                               
                            };

                            imports.Add(import);
                        }
                    }

                    if (ModelState.IsValid)
                    {
                        db.GLRecordJournalBegDistributions.AddRange(imports);
                        db.SaveChanges();
                    }
                  
                    #endregion

                }

            }


            #endregion
          
            return Json(new[] { order }.ToDataSourceResult(request, ModelState));
        }

        private void Post(int journalId, string user)
        {
            ExceedDbContext db = new ExceedDbContext();
            var gLRecordJournal = db.GlRecordJournalBegs.Find(journalId);
           
            if (gLRecordJournal != null)
            {
                var period = db.GLPeriods.Where(x => x.Name == gLRecordJournal.PeriodName).FirstOrDefault();
                if (period != null)
                {
                    var journallines = db.GLRecordJournalBegDistributions
                        .Where(x => x.GLRecordJournalBegId == journalId).AsNoTracking().ToList();

                    foreach (var journalline in journallines)
                    {
                        using (var context = new ExceedDbContext())
                        {
                            string journalNumber = $"Beg {journalline.GLRecordJournalBegId.ToString() } - {journalline.GLRecordJournalBegDistributionId.ToString()}";
                            

                            var line = context.GLLedgerPostingLines
                                .Where(x => x.GLLedgerPosting.AccountCode.ToUpper() == journalline.AccountCode.ToUpper())
                                 .Where(x => x.GLPeriodId == period.GLPeriodId)
                                 .Where(x => x.Type == LedgerPostTypes.Begining)
                                 .FirstOrDefault();
                           
                            if (line == null)
                            {
                                #region create new post
                                #region search if ledger is already created

                                var ledger = context.GLLedgerPostings.FirstOrDefault(x =>
                                    x.AccountCode.ToUpper() == journalline.AccountCode.ToUpper());                                

                                #endregion

                                if (ledger != null)
                                {
                                    var lineExists = context.GLLedgerPostingLines
                                                            .Where(x => x.GLLedgerPostingID == ledger.GLLedgerPostingID)
                                                             .Where(x => x.GLPeriodId == period.GLPeriodId)
                                                             .Where(x => x.Type == LedgerPostTypes.Begining)
                                                             .FirstOrDefault();
                                    if (lineExists != null)
                                    {
                                        #region update post
                                        lineExists.Debit = journalline.Debit;
                                        lineExists.Credit = journalline.Credit;

                                        if (ledger.AccountType == GlAccountTypes.Asset ||
                                         ledger.AccountType == GlAccountTypes.Expense)
                                        {
                                            lineExists.Balance = lineExists.Debit - lineExists.Credit;
                                        }
                                        else if (ledger.AccountType == GlAccountTypes.Liability ||
                                        ledger.AccountType == GlAccountTypes.Capital ||
                                        ledger.AccountType == GlAccountTypes.Revenue)
                                        {
                                            lineExists.Balance = lineExists.Credit - lineExists.Debit;
                                        }

                                        context.GLLedgerPostingLines.Attach(lineExists);
                                        context.Entry(lineExists).State = EntityState.Modified;
                                        context.SaveChanges();
                                        #endregion
                                    }
                                    else
                                    {
                                        #region post ledger line

                                        var ledgerpostline = new GLLedgerPostingLines
                                        {
                                            GLLedgerPosting = ledger,
                                            JournalNumber = journalNumber,
                                            JournalReference = gLRecordJournal.JournalNumber,
                                            EffectiveDate = gLRecordJournal.EffectiveDate,
                                            JournalName = gLRecordJournal.JournalName,
                                            CategoryName = gLRecordJournal.CategoryName,
                                            SourcesName = gLRecordJournal.SourcesName,
                                            Remark = gLRecordJournal.Remark,
                                            AdditionalDescription = journalline.AdditionalDescription,
                                            ReferenceDoc = gLRecordJournal.ReferenceDoc,
                                            PaymentReference = gLRecordJournal.PaymentReference,
                                            InvoiceReferenceDoc = gLRecordJournal.InvoiceReferenceDoc,
                                            GLPeriodId = period.GLPeriodId,
                                            GlFiscalYearId = period.GlFiscalYearId,
                                            Debit = journalline.Debit,
                                            Credit = journalline.Credit,
                                            Type = LedgerPostTypes.Begining,
                                            CreatedBy = user,
                                            DateCreated = DateTime.Now,
                                        };

                                        if (ledger.AccountType == GlAccountTypes.Asset ||
                                            ledger.AccountType == GlAccountTypes.Expense)
                                        {
                                            ledgerpostline.Balance = ledgerpostline.Debit - ledgerpostline.Credit;
                                        }
                                        else if (ledger.AccountType == GlAccountTypes.Liability ||
                                                ledger.AccountType == GlAccountTypes.Capital ||
                                                 ledger.AccountType == GlAccountTypes.Revenue)
                                        {
                                            ledgerpostline.Balance = ledgerpostline.Credit - ledgerpostline.Debit;
                                        }

                                        context.GLLedgerPostingLines.Add(ledgerpostline);
                                        context.SaveChanges();

                                        #endregion
                                    }
                                }
                                else
                                {
                                    #region create ledger

                                    var newledgerpost = new GLLedgerPosting
                                    {
                                        AccountCode = journalline.AccountCode.ToUpper()
                                    };
                                    
                                    newledgerpost.AccountDesc = accountHelper.GetAccountDesc(newledgerpost.AccountCode);
                                    newledgerpost.AccountType = accountHelper.GetAccountType(newledgerpost.AccountCode);
                                    context.GLLedgerPostings.Add(newledgerpost);
                                    context.SaveChanges();

                                    #endregion

                                    #region post ledger line
                                    var newledgerpostline = new GLLedgerPostingLines
                                    {
                                        GLLedgerPosting = newledgerpost,
                                        JournalNumber = journalNumber,
                                        JournalReference = gLRecordJournal.JournalNumber,
                                        EffectiveDate = gLRecordJournal.EffectiveDate,
                                        JournalName = gLRecordJournal.JournalName,
                                        CategoryName = gLRecordJournal.CategoryName,
                                        SourcesName = gLRecordJournal.SourcesName,
                                        Remark = gLRecordJournal.Remark,
                                        AdditionalDescription = journalline.AdditionalDescription,
                                        ReferenceDoc = gLRecordJournal.ReferenceDoc,
                                        PaymentReference = gLRecordJournal.PaymentReference,
                                        InvoiceReferenceDoc = gLRecordJournal.InvoiceReferenceDoc,
                                        GLPeriodId = period.GLPeriodId,
                                        GlFiscalYearId = period.GlFiscalYearId,
                                        Debit=journalline.Debit,
                                        Credit = journalline.Credit,
                                        Type = LedgerPostTypes.Begining,
                                        CreatedBy = user,
                                        DateCreated = DateTime.Now
                                    };

                                    if (newledgerpost.AccountType == GlAccountTypes.Asset ||
                                       newledgerpost.AccountType == GlAccountTypes.Expense)
                                    {
                                        newledgerpostline.Balance = newledgerpostline.Debit - newledgerpostline.Credit;
                                    }
                                    else if (newledgerpost.AccountType == GlAccountTypes.Liability ||
                                            newledgerpost.AccountType == GlAccountTypes.Capital ||
                                             newledgerpost.AccountType == GlAccountTypes.Revenue)
                                    {
                                        newledgerpostline.Balance = newledgerpostline.Credit - newledgerpostline.Debit;
                                    }

                                    context.GLLedgerPostingLines.Add(newledgerpostline);
                                    context.SaveChanges();

                                    #endregion

                                    #region split ledger account

                                    accountHelper.SplitLedgerAccount(newledgerpost);

                                    #endregion
                                }
                                #endregion
                            }
                            else
                            {
                                #region update  post
                                line.Debit = journalline.Debit;
                                line.Credit = journalline.Credit;

                                if (line.GLLedgerPosting.AccountType == GlAccountTypes.Asset ||
                                 line.GLLedgerPosting.AccountType == GlAccountTypes.Expense)
                                {
                                    line.Balance = line.Debit - line.Credit;
                                }
                                else if (line.GLLedgerPosting.AccountType == GlAccountTypes.Liability ||
                                line.GLLedgerPosting.AccountType == GlAccountTypes.Capital ||
                                line.GLLedgerPosting.AccountType == GlAccountTypes.Revenue)
                                {
                                    line.Balance = line.Credit - line.Debit;
                                }

                                context.GLLedgerPostingLines.Attach(line);
                                context.Entry(line).State =EntityState.Modified;
                                context.SaveChanges();

                                #endregion
                            }

                        }
                    }

                }
                
            }

        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
