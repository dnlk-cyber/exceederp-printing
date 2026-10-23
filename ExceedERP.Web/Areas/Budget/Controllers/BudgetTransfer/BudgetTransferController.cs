using ExceedERP.Core.Domain.Budget;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;

using System.Linq;
using System.Web.Mvc;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;
using ExceedERP.Core.Domain.Common.ViewModel;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Web.Areas.Budget.Controllers
{
    [AuthorizeRoles(BudgetRoles.BudgetUser)]
    public class BudgetTransferController : Controller
    {

        private readonly ExceedDbContext db = new ExceedDbContext();

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
            ViewData["CostCenters"] = costCenterList;

            var structure = db.MainCompanyStructures.ToList();
            ViewData["OrgStructure"] = structure;

            ViewData["GLPeriods"] = db.GLPeriods.ToList();
            ViewData["FiscalYear"] = db.GLFiscalYears.ToList();
            ViewData["BudgetCategory"] = db.BudgetCategorys.ToList();

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


        public ActionResult BudgetTransfer_Read([DataSourceRequest] DataSourceRequest request)
        {
            IQueryable<BudgetTransfer> BudgetTransfer = db.BudgetTransfers;
            DataSourceResult result = BudgetTransfer.ToDataSourceResult(request, budgetTransfer => new BudgetTransfer
            {
                BudgetTransferID = budgetTransfer.BudgetTransferID,
                GlFiscalYearId = budgetTransfer.GlFiscalYearId,
                PlanCategoryId = budgetTransfer.PlanCategoryId,
                FromCostCenter = budgetTransfer.FromCostCenter,
                FromOrgStructureId = budgetTransfer.FromOrgStructureId,
                FromBranchId = budgetTransfer.FromBranchId,
                FromBusinessUnitId = budgetTransfer.FromBusinessUnitId,
                FromGlPeriodId = budgetTransfer.FromGlPeriodId,
                FromAccountCode = budgetTransfer.FromAccountCode,

                ToCostCenter = budgetTransfer.ToCostCenter,
                ToOrgStructureId = budgetTransfer.ToOrgStructureId,
                ToBranchId = budgetTransfer.ToBranchId,
                ToBusinessUnitId = budgetTransfer.ToBusinessUnitId,
                ToGlPeriodId = budgetTransfer.ToGlPeriodId,
                EqualToAllPeriods = budgetTransfer.EqualToAllPeriods,
                ToAccountCode = budgetTransfer.ToAccountCode,
                Transferred = budgetTransfer.Transferred,

                CreatedBy = budgetTransfer.CreatedBy,
                DateCreated = budgetTransfer.DateCreated,
                LastModified = budgetTransfer.LastModified,
                ModifiedBy = budgetTransfer.ModifiedBy,
                OrgBranchName = budgetTransfer.OrgBranchName

            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetTransfer_Create([DataSourceRequest] DataSourceRequest request, BudgetTransfer budgetTransfer)
        {
            #region validations
            //From
            BudgetPlan FromPlan = db.BudgetPlans.Where(x => x.PlanCategoryId == budgetTransfer.PlanCategoryId &&
               x.AccountCode == budgetTransfer.FromAccountCode //&& //from AccountCode
               /*x.GLChartOfAccountCostCenterValue.ToString() == BudgetTransfer.FromWorkUnit*/ //from Workunit
               && x.BranchId == budgetTransfer.FromBranchId).FirstOrDefault(); //from Branch
                                                                                        //To
            BudgetPlan ToPlan = db.BudgetPlans.Where(x => x.PlanCategoryId == budgetTransfer.PlanCategoryId &&
                x.AccountCode == budgetTransfer.ToAccountCode && //To AccountCode
                /* x.GLChartOfAccountCostCenterValue.ToString() == BudgetTransfer.ToWorkUnit &&*/ //To workuit
                x.BranchId == budgetTransfer.ToBranchId).FirstOrDefault(); //To Branch

            //var value = FromPlan.BudgetPlanMonthly.Select(x => x.CurrentBudget).Sum(); //changed not working for reasons
            var fromMonthlyPlans = db.BudgetPlanMonthlys.Where(m => m.BudgetDefineID == FromPlan.BudgetPlanID).ToList();
            var currentBudgetTotal = fromMonthlyPlans.Select(x => x.CurrentBudget).Sum();

            if ((FromPlan != null && ToPlan != null) && currentBudgetTotal > budgetTransfer.Transferred)
            {
                //add the transfered amount to ToPlan's currentyearlybudget field
                ToPlan.CurrentYearlyBudget = ToPlan.CurrentYearlyBudget + budgetTransfer.Transferred;

                //subtract from the sum of [CurrentBudget] from BudgetMonthly the transfered amount
                FromPlan.CurrentYearlyBudget = fromMonthlyPlans.Select(x => x.CurrentBudget).Sum() - budgetTransfer.Transferred;
                //FromPlan.CurrentYearlyBudget = FromPlan.BudgetPlanMonthly.Sum(x => x.CurrentBudget) - BudgetTransfer.Transferred; //not working for unkown reasons changed

                db.BudgetPlans.Attach(FromPlan);
                db.Entry(FromPlan).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                db.BudgetPlans.Attach(ToPlan);
                db.Entry(ToPlan).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                //this part divides the totalyearlybudget by 12 then divides the transfered amount by 12
                //subtracts from monthlyBudget the transfered amount that is divided by 12
                //then assign it to the monthly current budget and the tansfered amount w/ what is transfered 
                List<BudgetPlanMonthly> fromMonths = db.BudgetPlanMonthlys.Where(x => x.BudgetDefineID == FromPlan.BudgetPlanID).ToList();
                decimal frommonthlybudget = FromPlan.TotalYearlyBudget / 12;
                decimal transferedForEach = budgetTransfer.Transferred / 12;
                decimal FromCurentBudget = frommonthlybudget - transferedForEach;
                foreach (var months in fromMonths)
                {
                    months.CurrentBudget = FromCurentBudget;
                    months.TransferedBudget = transferedForEach;
                    db.BudgetPlanMonthlys.Attach(months);
                    db.Entry(months).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                }

                List<BudgetPlanMonthly> toMonths = db.BudgetPlanMonthlys.Where(x => x.BudgetDefineID == ToPlan.BudgetPlanID).ToList();
                decimal tomonthlybudget = ToPlan.TotalYearlyBudget / 12;
                decimal ToCurentBudget = tomonthlybudget + transferedForEach;
                foreach (var months in toMonths)
                {
                    months.CurrentBudget = ToCurentBudget;
                    months.TransferedBudget = transferedForEach;

                    db.BudgetPlanMonthlys.Attach(months);
                    db.Entry(months).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                }
            }
            else
            {
                ModelState.AddModelError("InsufficentBalance", " Budget Plan Have Insufficient Balance for transfer! ");
            }
            #endregion

            if (ModelState.IsValid)
            {

                var entity = new BudgetTransfer
                {

                    BudgetTransferID = budgetTransfer.BudgetTransferID,
                    GlFiscalYearId = budgetTransfer.GlFiscalYearId,
                    PlanCategoryId = budgetTransfer.PlanCategoryId,
                    FromCostCenter = budgetTransfer.FromCostCenter,
                    FromOrgStructureId = budgetTransfer.FromOrgStructureId,
                    FromBranchId = budgetTransfer.FromBranchId,
                    FromBusinessUnitId = budgetTransfer.FromBusinessUnitId,
                    FromGlPeriodId = budgetTransfer.FromGlPeriodId,
                    FromAccountCode = budgetTransfer.FromAccountCode,

                    ToCostCenter = budgetTransfer.ToCostCenter,
                    ToOrgStructureId = budgetTransfer.ToOrgStructureId,
                    ToBranchId = budgetTransfer.ToBranchId,
                    ToBusinessUnitId = budgetTransfer.ToBusinessUnitId,
                    ToGlPeriodId = budgetTransfer.ToGlPeriodId,
                    EqualToAllPeriods = budgetTransfer.EqualToAllPeriods,
                    ToAccountCode = budgetTransfer.ToAccountCode,
                    Transferred = budgetTransfer.Transferred,

                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name

                };

                db.BudgetTransfers.Add(entity);
                db.SaveChanges();



                budgetTransfer.BudgetTransferID = entity.BudgetTransferID;

            }


            return Json(new[] { budgetTransfer }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetTransfer_Update([DataSourceRequest] DataSourceRequest request, BudgetTransfer budgetTransfer)
        {

            if (ModelState.IsValid)
            {
                var entity = new BudgetTransfer
                {
                    GlFiscalYearId = budgetTransfer.GlFiscalYearId,
                    PlanCategoryId = budgetTransfer.PlanCategoryId,
                    FromCostCenter = budgetTransfer.FromCostCenter,
                    FromOrgStructureId = budgetTransfer.FromOrgStructureId,
                    FromBranchId = budgetTransfer.FromBranchId,
                    FromBusinessUnitId = budgetTransfer.FromBusinessUnitId,
                    FromGlPeriodId = budgetTransfer.FromGlPeriodId,
                    FromAccountCode = budgetTransfer.FromAccountCode,

                    ToCostCenter = budgetTransfer.ToCostCenter,
                    ToOrgStructureId = budgetTransfer.ToOrgStructureId,
                    ToBranchId = budgetTransfer.ToBranchId,
                    ToBusinessUnitId = budgetTransfer.ToBusinessUnitId,
                    ToGlPeriodId = budgetTransfer.ToGlPeriodId,
                    EqualToAllPeriods = budgetTransfer.EqualToAllPeriods,
                    ToAccountCode = budgetTransfer.ToAccountCode,
                    Transferred = budgetTransfer.Transferred,

                    CreatedBy = budgetTransfer.CreatedBy,
                    DateCreated = budgetTransfer.DateCreated,

                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name
                };

                db.BudgetTransfers.Attach(entity);
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

            }



            return Json(new[] { budgetTransfer }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetTransfer_Destroy([DataSourceRequest] DataSourceRequest request, BudgetTransfer budgetTransfer)
        {

            var entity = new BudgetTransfer
            {
                BudgetTransferID = budgetTransfer.BudgetTransferID,
                GlFiscalYearId = budgetTransfer.GlFiscalYearId,
                PlanCategoryId = budgetTransfer.PlanCategoryId,
                FromCostCenter = budgetTransfer.FromCostCenter,
                FromOrgStructureId = budgetTransfer.FromOrgStructureId,
                FromBranchId = budgetTransfer.FromBranchId,
                FromBusinessUnitId = budgetTransfer.FromBusinessUnitId,
                FromGlPeriodId = budgetTransfer.FromGlPeriodId,
                FromAccountCode = budgetTransfer.FromAccountCode,

                ToCostCenter = budgetTransfer.ToCostCenter,
                ToOrgStructureId = budgetTransfer.ToOrgStructureId,
                ToBranchId = budgetTransfer.ToBranchId,
                ToBusinessUnitId = budgetTransfer.ToBusinessUnitId,
                ToGlPeriodId = budgetTransfer.ToGlPeriodId,
                EqualToAllPeriods = budgetTransfer.EqualToAllPeriods,
                ToAccountCode = budgetTransfer.ToAccountCode,
                Transferred = budgetTransfer.Transferred,

            };

            db.BudgetTransfers.Attach(entity);
            db.BudgetTransfers.Remove(entity);
            db.SaveChanges();

            UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Budget Transfer number " + budgetTransfer.BudgetTransferID);

            return Json(new[] { budgetTransfer }.ToDataSourceResult(request, ModelState));
        }

        public decimal GetTransferTotalPlan(int PlanCategory)
        {
            decimal TotalPerCategory = 0;
            try
            {
                TotalPerCategory = db.BudgetTransfers.Where(x => x.PlanCategoryId == PlanCategory).Sum(x => x.Transferred);
            }
            catch (Exception e)
            { return 0; }
            return TotalPerCategory;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }


    }
}