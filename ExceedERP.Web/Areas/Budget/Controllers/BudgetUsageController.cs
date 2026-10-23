using ExceedERP.DataAccess.Context;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Budget;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;
using System.Text.RegularExpressions;
using ExceedERP.Core.Domain.Common.ViewModel;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Web.Areas.Budget.Controllers
{
    [AuthorizeRoles(BudgetRoles.BudgetUser)]
    public class BudgetUsageController : Controller
    {


        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            List<SelectorVm> costCenterList = new List<SelectorVm>();

            var costCenterType =
                db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.CostCenter);
            if (costCenterType != null)
            {
                var segmentSetups = db.GlChartOfAccountSegments
                        .Where(x => x.GLSegmentSetupId == costCenterType.GLSegmentSetupId)
                        .Where(x => x.Enable).AsQueryable();

                foreach (var accountSegment in segmentSetups)
                {
                    var record = new SelectorVm
                    {
                        Name = accountSegment.Values + "-" + accountSegment.Description,
                        Code = accountSegment.Values
                    };
                    costCenterList.Add(record);
                }
            }

            var employee = db.Person_Employee;
            var selectedEmployee = employee.Select(
               i => new
               {
                   Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                   Code = i.EmployeeId
               }).AsQueryable();
            ViewData["personEmployees"] = selectedEmployee;

            var workunits = db.MainCompanyStructures.ToList();
            ViewData["workunits"] = workunits;
            ViewData["GLPeriods"] = db.GLPeriods.ToList();
            ViewData["FiscalYear"] = db.GLFiscalYears.ToList();
            ViewData["BudgetCurrency"] = db.FinSetupCurrencies.ToList();
            ViewData["CostCenters"] = costCenterList;
            ViewData["BudgetCategory"] = db.BudgetCategorys.ToList();
            ViewData["BudgetItems"] = db.BudgetCategoryDetails.ToList().Select(
              i => new
              {
                  BudgetItem = Regex.Replace(i.BudgetItem.Trim(), "[^a-zA-Z0-9]+", "", RegexOptions.Compiled),
                  BudgetCategoryDetailID = i.BudgetCategoryDetailID
              }).AsQueryable(); ;

            GetBranch();
            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                GetBusinessUnits();
            }

            return View();
        }
        public ActionResult GetBranch()
        {

            var branches = db.Branch;
            ViewData["branches"] = branches.Select(
              i => new
              {
                  Name = i.Name,
                  BranchId = i.BranchId
              }).AsQueryable(); ;
            return Json(branches.Select(p => new { BranchId = p.BranchId, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetBusinessUnits()
        {
            var bizUnits = db.BusinessUnits.ToList();
            var bizUnitdisplay = bizUnits.ToList();

            bizUnitdisplay.Add(new BusinessUnit
            {
                BusinessUnitId = 0,
                Name = "Not set"
            });
            ViewData["businesUnits"] = bizUnitdisplay.Select(
              i => new
              {
                  Name = i.Name.Trim(),
                  BusinessUnitId = i.BusinessUnitId
              }).AsQueryable(); ;
            return Json(bizUnits.Select(p => new { BusinessUnitId = p.BusinessUnitId, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }


        public ActionResult BudgetUsage_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<BudgetUsage> budgetUsages = db.BudgetUsages;
            DataSourceResult result = budgetUsages.ToDataSourceResult(request, budgetUsage => new
            {
                BudgetUsageID = budgetUsage.BudgetUsageID,
                GlFiscalYearId = budgetUsage.GlFiscalYearId,
                GLPeriodId = budgetUsage.GLPeriodId,
                BranchId = budgetUsage.BranchId,
                BusinessUnitId = budgetUsage.BusinessUnitId,
                EmployeeId = budgetUsage.EmployeeId,
                ReasonOfPayment = budgetUsage.ReasonOfPayment,
                RequestDate = budgetUsage.RequestDate,
                Amount = budgetUsage.Amount,
                NetAmount = budgetUsage.NetAmount,

                IsOnlineApproved = budgetUsage.IsOnlineApproved,
                OnlineApprovedBy = budgetUsage.OnlineApprovedBy,
                OnlineApprovedTime = budgetUsage.OnlineApprovedTime,
                IsVoid = budgetUsage.IsVoid,
                VoidBy = budgetUsage.VoidBy,
                VoidTime = budgetUsage.VoidTime,
                IsSendForApproval = budgetUsage.IsSendForApproval,
                SendForApprovalBy = budgetUsage.SendForApprovalBy,
                SendForApprovalTime = budgetUsage.SendForApprovalTime,

                DateCreated = budgetUsage.DateCreated,
                LastModified = budgetUsage.LastModified,
                CreatedBy = budgetUsage.CreatedBy,
                ModifiedBy = budgetUsage.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetUsage_Create([DataSourceRequest]DataSourceRequest request, BudgetUsage budgetUsage)
        {

            if (ModelState.IsValid)
            {
                var entity = new BudgetUsage
                {
                    GlFiscalYearId = budgetUsage.GlFiscalYearId,
                    GLPeriodId = budgetUsage.GLPeriodId,
                    BranchId = budgetUsage.BranchId,
                    BusinessUnitId = budgetUsage.BusinessUnitId,
                    EmployeeId = budgetUsage.EmployeeId,
                    ReasonOfPayment = budgetUsage.ReasonOfPayment,
                    RequestDate = budgetUsage.RequestDate,
                    Amount = budgetUsage.Amount,
                    NetAmount = budgetUsage.NetAmount,

                    IsOnlineApproved = budgetUsage.IsOnlineApproved,
                    OnlineApprovedBy = budgetUsage.OnlineApprovedBy,
                    OnlineApprovedTime = budgetUsage.OnlineApprovedTime,
                    IsVoid = budgetUsage.IsVoid,
                    VoidBy = budgetUsage.VoidBy,
                    VoidTime = budgetUsage.VoidTime,
                    IsSendForApproval = budgetUsage.IsSendForApproval,
                    SendForApprovalBy = budgetUsage.SendForApprovalBy,
                    SendForApprovalTime = budgetUsage.SendForApprovalTime,

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
                    GlFiscalYearId = budgetUsage.GlFiscalYearId,
                    GLPeriodId = budgetUsage.GLPeriodId,
                    BranchId = budgetUsage.BranchId,
                    BusinessUnitId = budgetUsage.BusinessUnitId,
                    EmployeeId = budgetUsage.EmployeeId,
                    ReasonOfPayment = budgetUsage.ReasonOfPayment,
                    RequestDate = budgetUsage.RequestDate,
                    Amount = budgetUsage.Amount,
                    NetAmount = budgetUsage.NetAmount,

                    IsOnlineApproved = budgetUsage.IsOnlineApproved,
                    OnlineApprovedBy = budgetUsage.OnlineApprovedBy,
                    OnlineApprovedTime = budgetUsage.OnlineApprovedTime,
                    IsVoid = budgetUsage.IsVoid,
                    VoidBy = budgetUsage.VoidBy,
                    VoidTime = budgetUsage.VoidTime,
                    IsSendForApproval = budgetUsage.IsSendForApproval,
                    SendForApprovalBy = budgetUsage.SendForApprovalBy,
                    SendForApprovalTime = budgetUsage.SendForApprovalTime,

                    DateCreated =budgetUsage.DateCreated,
                    CreatedBy = budgetUsage.CreatedBy,
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
                    GlFiscalYearId = budgetUsage.GlFiscalYearId,
                    GLPeriodId = budgetUsage.GLPeriodId,
                    BranchId = budgetUsage.BranchId,
                    BusinessUnitId = budgetUsage.BusinessUnitId,
                    EmployeeId = budgetUsage.EmployeeId,
                    ReasonOfPayment = budgetUsage.ReasonOfPayment,
                    RequestDate = budgetUsage.RequestDate,
                    Amount = budgetUsage.Amount,
                    NetAmount = budgetUsage.NetAmount,
                   
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






