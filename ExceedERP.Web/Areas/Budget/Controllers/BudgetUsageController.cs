using ExceedERP.DataAccess.Context;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Budget;

using ExceedERP.Core.Domain.Finance;
using ExceedERP.Helpers.Common;  
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Helpers.Budget;
using ExceedERP.Web.Areas.Budget.Models;
using ExceedERP.Core.Domain.Finance.GL;

namespace ExceedERP.Web.Areas.Budget.Controllers
{
    [AuthorizeRoles(BudgetRoles.BudgetUser)]
    public class BudgetUsageController : Controller
    {





        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["MainCompanyStructures"] = db.GLChartOfAccountCostCenters.ToList();

            return View();
        }
        public static List<string> GetPeriod(List<GLPeriod> GLPeriod)
        {

            List<string> Periods = new List<string>();
            for(int i=0; i < 12 ; i++ )
            {
                string Name = string.Empty;
                GLPeriod Period = GLPeriod[i];
                if (Period != null)
                {
                    Name = Period.Name;
                }
                Periods.Add(Name);
            }
            return Periods;
        }
        [OutputCache(Duration = 1000)]
        public ActionResult BudgetUsage_Read(string Name, string Year, [DataSourceRequest]DataSourceRequest request)
        {
            BudgetGLJournalHelper BudgetGLJournalHelper = new BudgetGLJournalHelper();
               db = new ExceedDbContext();
                List<BudgetUsageViewModel> NetUsages = new List<BudgetUsageViewModel>();
                List<BudgetUsage> Usages = new List<BudgetUsage>();

            var BudgetUsage = System.Runtime.Caching.MemoryCache.Default["Usage"];
            if (BudgetUsage == null)
            {
             
               
                var JournalDetails = db.GLRecordJournalDetails.Where(x => x.GLRecordJournal.IsOnlineApproved == true && x.GLRecordJournal.isVoid != true).AsQueryable();

                foreach (var Usage in JournalDetails.DistinctByField(x => x.AccountCode))
                {
                    var FiscalYear = db.GLFiscalYears.Where(x => x.Name == Usage.GLRecordJournal.FiscalYear).FirstOrDefault();
                   
                    var Periods = db.GLPeriods.Where(x => x.GlFiscalYearId == FiscalYear.GlFiscalYearId).OrderBy(x=>x.GLPeriodId).ToList();
                    var Period = GetPeriod(Periods);

                    var janu = Period[0];
                    var Febr = Period[1];
                    var March = Period[2];
                    var April = Period[3];
                    var Ma = Period[4];
                    var June = Period[5];
                    var July = Period[6];
                    var August = Period[7];
                    var September = Period[8];
                    var October = Period[9];
                    var November = Period[10];
                    var December = Period[11];

                    var Jan = BudgetGLJournalHelper.GetNetJournalBalance(JournalDetails.Where(x => x.GLRecordJournal.PeriodName == janu && x.AccountCode == Usage.AccountCode && x.CostCenterSegment == Usage.CostCenterSegment).FirstOrDefault());
                    var Feb = BudgetGLJournalHelper.GetNetJournalBalance( JournalDetails.Where(x => x.GLRecordJournal.PeriodName == Febr && x.AccountCode == Usage.AccountCode && x.CostCenterSegment == Usage.CostCenterSegment).FirstOrDefault());
                    var Mar = BudgetGLJournalHelper.GetNetJournalBalance( JournalDetails.Where(x => x.GLRecordJournal.PeriodName == Ma && x.AccountCode == Usage.AccountCode && x.CostCenterSegment == Usage.CostCenterSegment).FirstOrDefault());
                    var Apr = BudgetGLJournalHelper.GetNetJournalBalance( JournalDetails.Where(x => x.GLRecordJournal.PeriodName == April && x.AccountCode == Usage.AccountCode && x.CostCenterSegment == Usage.CostCenterSegment).FirstOrDefault());
                    var May = BudgetGLJournalHelper.GetNetJournalBalance( JournalDetails.Where(x => x.GLRecordJournal.PeriodName == Ma && x.AccountCode == Usage.AccountCode && x.CostCenterSegment == Usage.CostCenterSegment).FirstOrDefault());
                    var Jun = BudgetGLJournalHelper.GetNetJournalBalance( JournalDetails.Where(x => x.GLRecordJournal.PeriodName == June && x.AccountCode == Usage.AccountCode && x.CostCenterSegment == Usage.CostCenterSegment).FirstOrDefault());
                    var Jul = BudgetGLJournalHelper.GetNetJournalBalance( JournalDetails.Where(x => x.GLRecordJournal.PeriodName == July && x.AccountCode == Usage.AccountCode && x.CostCenterSegment == Usage.CostCenterSegment).FirstOrDefault());
                    var Aug = BudgetGLJournalHelper.GetNetJournalBalance( JournalDetails.Where(x => x.GLRecordJournal.PeriodName == August && x.AccountCode == Usage.AccountCode && x.CostCenterSegment == Usage.CostCenterSegment).FirstOrDefault());
                    var Sep = BudgetGLJournalHelper.GetNetJournalBalance( JournalDetails.Where(x => x.GLRecordJournal.PeriodName == September && x.AccountCode == Usage.AccountCode && x.CostCenterSegment == Usage.CostCenterSegment).FirstOrDefault());
                    var Oct = BudgetGLJournalHelper.GetNetJournalBalance( JournalDetails.Where(x => x.GLRecordJournal.PeriodName == October && x.AccountCode == Usage.AccountCode && x.CostCenterSegment == Usage.CostCenterSegment).FirstOrDefault());
                    var Nov = BudgetGLJournalHelper.GetNetJournalBalance( JournalDetails.Where(x => x.GLRecordJournal.PeriodName == November && x.AccountCode == Usage.AccountCode && x.CostCenterSegment == Usage.CostCenterSegment).FirstOrDefault());
                    var Dec = BudgetGLJournalHelper.GetNetJournalBalance( JournalDetails.Where(x => x.GLRecordJournal.PeriodName == December && x.AccountCode == Usage.AccountCode && x.CostCenterSegment == Usage.CostCenterSegment).FirstOrDefault());

                    BudgetUsageViewModel Model = new BudgetUsageViewModel();

                    Model.AccountCode = Usage.AccountCode+" - "+Usage.AccountDesc;
                    Model.BudgetCategory = Usage.GLRecordJournal.FiscalYear;
                    Model.WorkUnit = Usage.CostCenterSegment;
                    Model.Unit = "";
                    Model.Remark = Usage.AdditionalDescription;

                   
                        if (Jan != null)
                            Model.Jan = Jan;
                        if (Feb != null)
                            Model.Feb = Feb;
                        if (Mar != null)
                            Model.Mar = Mar;
                        if (Apr != null)
                            Model.Apr = Apr;
                        if (May != null)
                            Model.May = May;
                        if (Jun != null)
                            Model.Jun = Jun;
                        if (Jul != null)
                            Model.Jul = Jul;
                        if (Aug != null)
                            Model.Aug = Aug;
                        if (Sep != null)
                            Model.Sep = Sep;
                        if (Nov != null)
                            Model.Nov = Nov;
                        if (Oct != null)
                            Model.Oct = Oct;
                        if (Dec != null)
                            Model.Dec = Dec;
                     NetUsages.Add(Model);
                 }

                  System.Runtime.Caching.MemoryCache.Default["Usage"] = NetUsages;
                DataSourceResult result = NetUsages.ToDataSourceResult(request);

                return Json(result);
                 
            }
            else
            {
                DataSourceResult BudGetresult = ((List<BudgetUsageViewModel>)BudgetUsage).ToDataSourceResult(request);

                return Json(BudGetresult);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetUsage_Create(int CCategoryId, [DataSourceRequest]DataSourceRequest request, BudgetUsage budgetUsage)
        {

            if (ModelState.IsValid)
            {
                var entity = new BudgetUsage
                {
                    BudgetUsageID = budgetUsage.BudgetUsageID,
                    AccountCode = budgetUsage.AccountCode,
                    AccountDesc = budgetUsage.AccountDesc,
                    BudgetMonth = budgetUsage.BudgetMonth,
                    BudgetYear = budgetUsage.BudgetYear,
                    PlanCategoryId = CCategoryId,
                    WorkUnit = budgetUsage.WorkUnit,
                    Unit = budgetUsage.Unit,
                    MonthlyActuals = budgetUsage.MonthlyActuals,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name

                };

                db.BudgetUsages.Add(entity);
                db.SaveChanges();
                budgetUsage.BudgetUsageID = entity.BudgetUsageID;
            }


            return Json(new[] { budgetUsage }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetUsage_Update([DataSourceRequest]DataSourceRequest request, BudgetUsage budgetUsage)
        {
            if (ModelState.IsValid)
            {
                var entity = new BudgetUsage
                {
                    BudgetUsageID = budgetUsage.BudgetUsageID,
                    AccountCode = budgetUsage.AccountCode,
                    AccountDesc = budgetUsage.AccountDesc,
                    BudgetMonth = budgetUsage.BudgetMonth,
                    BudgetYear = budgetUsage.BudgetYear,
                    Unit = budgetUsage.Unit,
                    WorkUnit = budgetUsage.WorkUnit,
                    MonthlyActuals = budgetUsage.MonthlyActuals,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name
                };

                db.BudgetUsages.Attach(entity);
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { budgetUsage }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetUsage_Destroy([DataSourceRequest]DataSourceRequest request, BudgetUsage budgetUsage)
        {
            if (ModelState.IsValid)
            {
                var entity = new BudgetUsage
                {
                    BudgetUsageID = budgetUsage.BudgetUsageID,
                    AccountCode = budgetUsage.AccountCode,
                    AccountDesc = budgetUsage.AccountDesc,
                    BudgetMonth = budgetUsage.BudgetMonth,
                    BudgetYear = budgetUsage.BudgetYear,
                    Unit = budgetUsage.Unit,
                    WorkUnit = budgetUsage.WorkUnit,
                    MonthlyActuals = budgetUsage.MonthlyActuals,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name
                };

                db.BudgetUsages.Attach(entity);
                db.BudgetUsages.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Budget Usage number " + budgetUsage.BudgetUsageID);

            }

            return Json(new[] { budgetUsage }.ToDataSourceResult(request, ModelState));
        }

     
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}






