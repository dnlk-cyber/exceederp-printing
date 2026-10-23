using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Budget;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Common.ViewModel;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Budget;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Areas.Budget.Models;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Budget.Controllers
{
    [AuthorizeRoles(HrmConstants.HRDepartmentHead)]
    public class BudgetPlanRegistrationSelfCareController : Controller
    {

        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index(string Reset)
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

            var workunits = db.MainCompanyStructures.ToList();
            //ViewData["workunits"] = workunits;
            ViewData["WorkUnits"] = db.MainCompanyStructures.ToList();
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
        public string GetBudgetCategoryType(string Name)
        {

            var BudgetCategory = db.BudgetCategorys.Where(x => x.Name == Name).FirstOrDefault();
            if (BudgetCategory != null)
            {
                return BudgetCategory.BudgetType; //changed from Description to BudgetType
            }

            return string.Empty;
        }

        public decimal GetTotalPlan(PlanCategory planCategory)
        {
            decimal TotalPerCategory = 0;

            try
            {

                TotalPerCategory = db.BudgetPlans
                  .Where(x => x.PlanCategoryId == planCategory.PlanCategoryID)
                  .Sum(x => x.TotalYearlyBudget);

            }
            catch (Exception e)
            {
                return 0;
            }
            return TotalPerCategory;

        }
        public decimal GetCurentTotalPlan(PlanCategory planCategory)
        {
            decimal TotalPerCategory = 0;

            try
            {

                TotalPerCategory = db.BudgetPlans
                     .Where(x => x.PlanCategoryId == planCategory.PlanCategoryID)
                    .Sum(x => x.CurrentYearlyBudget);

            }
            catch (Exception e)
            {
                return 0;
            }
            return TotalPerCategory;

        }
        public ActionResult BudgetPlanRegistration_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
            List<int> userworkunit = UserInformationHelper.GetUserHeadWorkUnitAndChildWorkUnits(User.Identity.Name);
            IQueryable<PlanCategory> PlanCategory = db.PlanCategorys
                .Where(x => userworkunit.Contains(x.OrgStuructureId)
                || x.ForwardedTo == User.Identity.Name
                || x.ForwardedFrom == User.Identity.Name
                || x.CreatedBy == User.Identity.Name)
                .Where(x => mybraches.Contains(x.BranchId));

            DataSourceResult result = PlanCategory.ToDataSourceResult(request, plan => new PlanCategory
            {
                PlanCategoryID = plan.PlanCategoryID,
                BudgetType = plan.BudgetType,
                BudgetCategoryID = plan.BudgetCategoryID,
                Name = plan.Name,
                GlFiscalYearId = plan.GlFiscalYearId,

                EmployeeId = plan.EmployeeId,
                OrgStuructureId = plan.OrgStuructureId,
                BranchId = plan.BranchId,
                BusinessUnitId = plan.BusinessUnitId,

                CurrentTotal = plan.CurrentTotal,// GetCurentTotalPlan(plan),
                GrandTotal = plan.GrandTotal,// GetTotalPlan(plan),
                ActualConsumption = plan.ActualConsumption,
                BudgetStatus = plan.BudgetStatus,

                IsDepartmentOnlineApproved = plan.IsDepartmentOnlineApproved,
                DepartmentOnlineApprovedBy = plan.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = plan.DepartmentOnlineApprovedTime,
                IsDepartmentSendForApproval = plan.IsDepartmentSendForApproval,
                DepartmentSendForApprovalBy = plan.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = plan.DepartmentSendForApprovalTime,

                ForwardedFrom = plan.ForwardedFrom,
                ForwardedTo = plan.ForwardedTo,
                IsFinalApproval = plan.IsFinalApproval,
                CurrentApprovalStep = plan.CurrentApprovalStep,

                IsSendForApproval = plan.IsSendForApproval,
                SendForApprovalBy = plan.SendForApprovalBy,
                SendForApprovalTime = plan.SendForApprovalTime,

                IsOnlineApproved = plan.IsOnlineApproved,
                OnlineApprovedBy = plan.OnlineApprovedBy,
                OnlineApprovedTime = plan.OnlineApprovedTime,

                IsOpenforrevision = plan.IsOpenforrevision,
                IsRevisionOnlineApproved = plan.IsRevisionOnlineApproved,
                IsRevisionSendForApproval = plan.IsRevisionSendForApproval,
                RevisionOnlineApprovedBy = plan.RevisionOnlineApprovedBy,
                RevisionOnlineApprovedTime = plan.RevisionOnlineApprovedTime,

                Progress = plan.Progress,
                Remark = plan.Remark,

                Status = plan.Status,
                CreatedBy = plan.CreatedBy,
                DateCreated = plan.DateCreated,
                LastModified = plan.LastModified,
                ModifiedBy = plan.ModifiedBy,



            });



            return Json(result);

        }



        public ActionResult BudgetPlanRegistration_Create([DataSourceRequest] DataSourceRequest request, PlanCategory plan)
        {


            #region data 
            plan.EmployeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

            if (plan.EmployeeId != 0)
            {
                var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == plan.EmployeeId && x.isCurrent);
                if (placement != null)
                {
                    plan.OrgStuructureId = placement.OrgStructureId;
                    plan.BranchId = placement.BranchId;
                    plan.BusinessUnitId = placement.BusinessUnitId;
                }
                else
                {
                    ModelState.AddModelError("placement", "You have not current placemnt");
                }
            }

            #endregion
            #region validation

            var PlanCategorys = db.PlanCategorys
                .Where(p => p.BudgetCategoryID == plan.BudgetCategoryID
                && p.GlFiscalYearId == plan.GlFiscalYearId
                && p.OrgStuructureId == plan.OrgStuructureId && plan.OrgStuructureId != 0
                && p.BranchId == plan.BranchId && plan.BranchId != 0)
                .ToList();

            if (PlanCategorys.Any())
            {
                ModelState.AddModelError("DuplicateEntrie", "Budget category for the year and organization already exisits !");
            }

            var cat = db.BudgetCategorys.Find(plan.BudgetCategoryID);
            if (cat != null)
                plan.BudgetType = cat.BudgetType;
            #endregion

            if (ModelState.IsValid)
            {
                var entity = new PlanCategory
                {
                    PlanCategoryID = plan.PlanCategoryID,
                    BudgetType = plan.BudgetType,
                    BudgetCategoryID = plan.BudgetCategoryID,
                    EmployeeId = plan.EmployeeId,
                    OrgStuructureId = plan.OrgStuructureId,
                    BranchId = plan.BranchId,
                    BusinessUnitId = plan.BusinessUnitId,
                    Name = plan.Name,
                    GrandTotal = plan.GrandTotal,
                    GlFiscalYearId = plan.GlFiscalYearId,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    Remark = plan.Remark

                };

                db.PlanCategorys.Add(entity);
                db.SaveChanges();
                plan.PlanCategoryID = entity.PlanCategoryID;

            }


            return Json(new[] { plan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetPlanRegistration_Update([DataSourceRequest] DataSourceRequest request, PlanCategory plan)
        {
            #region validation

            var PlanCategorys = db.PlanCategorys
                .Where(p => p.PlanCategoryID != plan.PlanCategoryID)
                .Where(p => p.BudgetCategoryID == plan.BudgetCategoryID
                && p.GlFiscalYearId == plan.GlFiscalYearId
                && p.OrgStuructureId == plan.OrgStuructureId && plan.OrgStuructureId != 0
                && p.BranchId == plan.BranchId && plan.BranchId != 0)
                .ToList();

            if (PlanCategorys.Any())
            {
                ModelState.AddModelError("DuplicateEntrie", "Budget category for the year and organization already exisits !");
            }

            var cat = db.BudgetCategorys.Find(plan.BudgetCategoryID);
            if (cat != null)
                plan.BudgetType = cat.BudgetType;
            #endregion

            if (ModelState.IsValid)
            {
                var entity = new PlanCategory
                {
                    PlanCategoryID = plan.PlanCategoryID,
                    BudgetType = plan.BudgetType,
                    BudgetCategoryID = plan.BudgetCategoryID,
                    Name = plan.Name,
                    GrandTotal = plan.GrandTotal,
                    CurrentTotal = plan.CurrentTotal,
                    GlFiscalYearId = plan.GlFiscalYearId,

                    EmployeeId = plan.EmployeeId,
                    OrgStuructureId = plan.OrgStuructureId,
                    BranchId = plan.BranchId,
                    BusinessUnitId = plan.BusinessUnitId,

                    IsDepartmentOnlineApproved = plan.IsDepartmentOnlineApproved,
                    DepartmentOnlineApprovedBy = plan.DepartmentOnlineApprovedBy,
                    DepartmentOnlineApprovedTime = plan.DepartmentOnlineApprovedTime,
                    IsDepartmentSendForApproval = plan.IsDepartmentSendForApproval,
                    DepartmentSendForApprovalBy = plan.DepartmentSendForApprovalBy,
                    DepartmentSendForApprovalTime = plan.DepartmentSendForApprovalTime,

                    ForwardedFrom = plan.ForwardedFrom,
                    ForwardedTo = plan.ForwardedTo,
                    IsFinalApproval = plan.IsFinalApproval,
                    CurrentApprovalStep = plan.CurrentApprovalStep,

                    IsSendForApproval = plan.IsSendForApproval,
                    SendForApprovalBy = plan.SendForApprovalBy,
                    SendForApprovalTime = plan.SendForApprovalTime,

                    IsOnlineApproved = plan.IsOnlineApproved,
                    OnlineApprovedBy = plan.OnlineApprovedBy,
                    OnlineApprovedTime = plan.OnlineApprovedTime,

                    IsOpenforrevision = plan.IsOpenforrevision,
                    IsRevisionOnlineApproved = plan.IsRevisionOnlineApproved,
                    IsRevisionSendForApproval = plan.IsRevisionSendForApproval,
                    RevisionOnlineApprovedBy = plan.RevisionOnlineApprovedBy,
                    RevisionOnlineApprovedTime = plan.RevisionOnlineApprovedTime,

                    Status = plan.Status,
                    CreatedBy = plan.CreatedBy,
                    DateCreated = plan.DateCreated,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name,
                    Remark = plan.Remark //added
                };

                db.PlanCategorys.Attach(entity);
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                //AccountCodeRangeViewModel model = new AccountCodeRangeViewModel();
                //var BudgetCategory = db.BudgetCategorys.Where(x => x.BudgetCategoryID == PlanCategory.BudgetCategoryID).FirstOrDefault();
                //if (BudgetCategory != null)
                //{
                //    model.BudgetCategory = BudgetCategory;
                //    model.PlanCategory = entity;
                //    model.GLPeriods = db.GLPeriods.ToList();//fiter the current year periods

                //    await Task.Run(async () =>
                //    {
                //        await CreateBudgetPlan(model);
                //    });

                //}


            }

            return Json(new[] { plan }.ToDataSourceResult(request, ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetPlanRegistration_Destroy([DataSourceRequest] DataSourceRequest request, PlanCategory plan)
        {

            var entity = new PlanCategory
            {
                PlanCategoryID = plan.PlanCategoryID,
                EmployeeId = plan.EmployeeId,
                OrgStuructureId = plan.OrgStuructureId,
                BranchId = plan.BranchId,
                BusinessUnitId = plan.BusinessUnitId,
            };


            db.PlanCategorys.Attach(entity);
            db.PlanCategorys.Remove(entity);
            db.SaveChanges();

            #region remove related
            var plans = db.BudgetPlans.Where(x => x.PlanCategoryId == plan.PlanCategoryID).ToList();
            if (plans.Any())
            {
                foreach (var p in plans)
                {
                    var budgetPlansMonthlys = db.BudgetPlanMonthlys.Where(x => x.BudgetDefineID == p.BudgetPlanID).ToList();
                    db.BudgetPlanMonthlys.RemoveRange(budgetPlansMonthlys);
                    db.SaveChanges();

                }
                db.BudgetPlans.RemoveRange(plans);
                db.SaveChanges();
            }
            #endregion 

            return Json(new[] { plan }.ToDataSourceResult(request));
        }

        public JsonResult SendforDepartmentApproval(int id)
        {
            var record = db.PlanCategorys.Find(id);
            string errMsg = "";

            if (record != null && record.IsDepartmentOnlineApproved)
            {
                errMsg = "You cannot manipulate this record,it is already approved";
                var formattedData = new
                {
                    Message = errMsg,
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record != null && record.IsDepartmentSendForApproval)
            {
                errMsg = "This record is already sent for department approval.";
                var formattedData = new
                {
                    Message = errMsg,
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }



            if (record != null && !record.IsDepartmentOnlineApproved && !record.IsDepartmentSendForApproval)
            {
                #region validation
                var details = db.BudgetPlans.Where(x => x.PlanCategoryId == record.PlanCategoryID);
                if (!details.Any())
                {
                    var error = new
                    {
                        Message = "Add at least one budget plan item.",
                        Status = "NOT_OK"
                    };
                    return Json(error, JsonRequestBehavior.AllowGet);
                }
                #endregion

                record.Status = Enum.GetName(typeof(OperationStatuses), 16);
                record.IsDepartmentSendForApproval = true;
                record.DepartmentSendForApprovalBy = User.Identity.Name;
                record.DepartmentSendForApprovalTime = DateTime.Now;
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();

                var formattedData = new
                {
                    Message = "Successfully Sent.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);

            }
            return null;
        }


        //[AcceptVerbs(HttpVerbs.Get)]
        //public ActionResult BudgetPlanRegistration_RemoveDependancy(string PlanId)
        //{
        //    var BudgetPlans = db.BudgetPlans.Where(x => x.PlanCategoryId.ToString() == PlanId).ToList();

        //    foreach (var plan in BudgetPlans)
        //    {
        //        var budgetPlansMonthlys = db.BudgetPlanMonthlys.Where(x => x.BudgetDefineID == plan.BudgetPlanID).ToList();

        //        db.BudgetPlanMonthlys.RemoveRange(budgetPlansMonthlys);
        //        db.SaveChanges();

        //    }
        //    db.BudgetPlans.RemoveRange(BudgetPlans);
        //    db.SaveChanges();
        //    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Plan Category number " + PlanId);
        //    return RedirectToAction("Index", new { Reset = "Reset" });
        //}





        public List<BindingItem> GetMonths(int Id)
        {
            List<BindingItem> Months = new List<BindingItem>();
            var Category = db.PlanCategorys.Where(x => x.PlanCategoryID == Id).FirstOrDefault();
            if (Category != null)
            {
                Months = db.GLPeriods.Where(x => x.GlFiscalYear.GlFiscalYearId == Category.GlFiscalYearId).Select(s => new BindingItem
                {
                    Text = s.Name,
                    Value = s.GLPeriodId
                }).ToList();

            }
            return Months;

        }
        //public async Task CreateBudgetPlan(AccountCodeRangeViewModel accountCodeRange)
        //{

        //    if (accountCodeRange != null)
        //    {
        //        BudgetGLAccountViewModel budgetGlAccount = new BudgetGLAccountViewModel();
        //        //OtherSpecificationViewModel contains branch and budgetCurrency 
        //        OtherSpecifcationViewModel otherSpecifcation = new OtherSpecifcationViewModel();
        //        //all the branches
        //        var budgetCurrency = db.FinSetupCurrencies.FirstOrDefault();//??
        //        var branch = db.Branch.ToList();
        //        if (branch.Any())
        //        {
        //            otherSpecifcation.Branches = branch;
        //            otherSpecifcation.BudgetCurrency = budgetCurrency;

        //            BudgetGenerationHelper budgetDistribution = new BudgetGenerationHelper();
        //            await budgetDistribution.GenrateAccountCodeByRange(accountCodeRange, otherSpecifcation, budgetGlAccount);
        //        }
        //        else
        //        {
        //            ModelState.AddModelError(string.Empty, "Error Message");
        //        }
        //    }

        //}

        public decimal GetTotalPlan(int PlanCategory)
        {
            decimal TotalPerCategory = 0;

            try
            {
                TotalPerCategory = db.BudgetPlans
                    .Where(x => x.PlanCategoryId == PlanCategory)
                    .Sum(x => x.TotalYearlyBudget);
            }
            catch (Exception e)
            {
                return 0;
            }
            return TotalPerCategory;

        }

        public decimal GetTransferTotalPlan(int PlanCategory)
        {
            decimal TotalPerCategory = 0;
            try
            {
                TotalPerCategory = db.BudgetTransfers
                    .Where(x => x.PlanCategoryId == PlanCategory)
                    .Sum(x => x.Transferred);
            }
            catch (Exception e)
            {
                return 0;
            }
            return TotalPerCategory;
        }

        public decimal GetUsagePerPlan(int PlanCategory)
        {
            decimal UsagePercentage = 0;

            List<BudgetUsage> Usages = new List<BudgetUsage>();

            try
            {

                var PlanCategorys = db.PlanCategorys
                    .Where(x => x.PlanCategoryID == PlanCategory)
                    .FirstOrDefault();

                var BudgetCategorys = db.BudgetCategorys
                    .Where(x => x.Name == PlanCategorys.Name)
                    .FirstOrDefault();
                var BudgetControl = db.BudgetControls
                    .Where(x => x.BudgetControlId == BudgetCategorys.BudgetControlId)
                    .FirstOrDefault();

                var GrowthBudgetPlan = decimal.Zero;
                var BudgetPlan = db.BudgetPlans
                    .Where(x => x.PlanCategoryId == PlanCategory)
                    .ToList();
                foreach (var Plan in BudgetPlan)
                {
                    var BudgetPlanMonthly = db.BudgetPlanMonthlys
                        .Where(x => x.BudgetDefineID == Plan.BudgetPlanID)
                        .ToList();
                    GrowthBudgetPlan = GrowthBudgetPlan + BudgetPlanMonthly.Sum(x => x.CurrentBudget);
                }
                if (GrowthBudgetPlan == 0)
                {
                    GrowthBudgetPlan = 1;
                }

                var JournalDetails = db.GLRecordJournalDetails
                    .Where(x => x.AccountCode.ToUpper().CompareTo(BudgetCategorys.FromAccountCode) >= 0)
                    .Where(x => x.AccountCode.ToUpper().CompareTo(BudgetCategorys.ToAccountCode) <= 0)
                    .Where(x => x.GLRecordJournal.FiscalYear == PlanCategorys.GlFiscalYear.Name);

                UsagePercentage = (JournalDetails.Sum(x => x.Credit));

                return UsagePercentage;
            }
            catch (Exception e)
            {
                return 0;
            }

        }
        public decimal GetCurentTotalPlan(int PlanCategory)
        {
            decimal TotalPerCategory = 0;

            try
            {
                var BudgetPlan = db.BudgetPlans
                    .Where(x => x.PlanCategoryId == PlanCategory)
                    .ToList();
                foreach (var Plan in BudgetPlan)
                {
                    var BudgetPlanMonthly = db.BudgetPlanMonthlys.Where(x => x.BudgetDefineID == Plan.BudgetPlanID).ToList();
                    TotalPerCategory = TotalPerCategory + BudgetPlanMonthly.Sum(x => x.CurrentBudget);
                }
            }
            catch (Exception e)
            {
                return 0;
            }
            return TotalPerCategory;


        }
        public decimal GetUsageTotalPlanPercentage(int PlanCategory)
        {
            decimal TotalPerCategory = 0;
            decimal UsagePercentage = 0;

            List<BudgetUsage> Usages = new List<BudgetUsage>();

            try
            {



                var PlanCategorys = db.PlanCategorys.Where(x => x.PlanCategoryID == PlanCategory).FirstOrDefault();

                var BudgetCategorys = db.BudgetCategorys.Where(x => x.Name == PlanCategorys.Name).FirstOrDefault();
                var BudgetControl =
                  db.BudgetControls.Where(x => x.BudgetControlId == BudgetCategorys.BudgetControlId).FirstOrDefault();

                var GrowthBudgetPlan = decimal.Zero;
                var BudgetPlan = db.BudgetPlans.Where(x => x.PlanCategoryId == PlanCategory).ToList();
                foreach (var Plan in BudgetPlan)
                {
                    var BudgetPlanMonthly = db.BudgetPlanMonthlys.Where(x => x.BudgetDefineID == Plan.BudgetPlanID).ToList();
                    GrowthBudgetPlan = GrowthBudgetPlan + BudgetPlanMonthly.Sum(x => x.CurrentBudget);
                }
                if (GrowthBudgetPlan == 0)
                {
                    GrowthBudgetPlan = 1;
                }

                var JournalDetails = db.GLRecordJournalDetails.Where(
          x => x.AccountCode.ToUpper().CompareTo(BudgetCategorys.FromAccountCode) >= 0)
          .Where(x => x.AccountCode.ToUpper().CompareTo(BudgetCategorys.ToAccountCode) <= 0).Where(x => x.GLRecordJournal.FiscalYear == PlanCategorys.GlFiscalYear.Name);
                UsagePercentage = (JournalDetails.Sum(x => x.Credit));
                GrowthBudgetPlan = (GrowthBudgetPlan * (BudgetControl.Percentage) / 100) + BudgetControl.ReserveAmount + GrowthBudgetPlan;

                UsagePercentage = (UsagePercentage / GrowthBudgetPlan.DivideByZeroHelper()) * 100;
                return UsagePercentage;
            }
            catch (Exception e)
            { return 0; }

        }
        private string GetBudgetStatus(int CategoryId)
        {
            string stat = "Bad";
            decimal TotalPerCategory = 0;
            decimal UsagePercentage = 0;

            List<BudgetUsage> Usages = new List<BudgetUsage>();

            try
            {

                var PlanCategorys = db.PlanCategorys
                    .Where(x => x.PlanCategoryID == CategoryId)
                    .FirstOrDefault();

                var BudgetCategorys = db.BudgetCategorys
                    .Where(x => x.Name == PlanCategorys.Name)
                    .FirstOrDefault();

                var BudgetControl = db.BudgetControls
                    .Where(x => x.BudgetControlId == BudgetCategorys.BudgetControlId)
                    .FirstOrDefault();

                var GrowthBudgetPlan = db.BudgetPlans
                    .Where(x => x.PlanCategoryId == PlanCategorys.PlanCategoryID)
                    .Sum(x => x.TotalYearlyBudget);
                if (GrowthBudgetPlan == 0)
                {
                    GrowthBudgetPlan = 1;
                }

                var JournalDetails = db.GLRecordJournalDetails
                    .Where(x => x.AccountCode.ToUpper().CompareTo(BudgetCategorys.FromAccountCode) >= 0)
                    .Where(x => x.AccountCode.ToUpper().CompareTo(BudgetCategorys.ToAccountCode) <= 0)
                    .Where(x => x.GLRecordJournal.FiscalYear == PlanCategorys.GlFiscalYear.Name);

                UsagePercentage = (JournalDetails.Sum(x => x.Credit));
                GrowthBudgetPlan = (GrowthBudgetPlan * (BudgetControl.Percentage) / 100) + BudgetControl.ReserveAmount + GrowthBudgetPlan;
                if (UsagePercentage > GrowthBudgetPlan)
                {
                    return "Bad";
                }
                else
                {
                    return "Good";
                }

            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }


        public decimal GetRequestTotalPlan(int PlanCategory)
        {
            decimal TotalPerCategory = 0;
            try
            {
                //   List<BudgetRequest> budgetRequest = db.BudgetRequests.Where(x => x.PlanCategoryId == PlanCategory).ToList();

            }
            catch (Exception e)
            {
                return 0;
            }
            return TotalPerCategory;
        }

        public decimal GetCurentAllocationTotalPlan(int PlanCategory)
        {
            decimal TotalPerCategory = 0;
            try
            {
                TotalPerCategory = db.BudgetAllocations
                    .Where(x => x.PlanCategoryId == PlanCategory)
                    .Sum(x => x.Amount);
            }
            catch (Exception e)
            {
                return 0;
            }
            return TotalPerCategory;
        }
        public decimal GetCurentTransferTotalPlan(int PlanCategory)
        {
            decimal TotalPerCategory = 0;
            try
            {
                TotalPerCategory = db.BudgetTransfers
                    .Where(x => x.PlanCategoryId == PlanCategory)
                    .Sum(x => x.Transferred);
            }
            catch (Exception e)
            {
                return 0;
            }
            return TotalPerCategory;
        }



        public decimal GetCurentRequestTotalPlan(int PlanCategory)
        {
            decimal TotalPerCategory = 0;
            try
            {


            }
            catch (Exception e)
            {
                return 0;
            }
            return TotalPerCategory;
        }


        [AuthorizeRoles(HrmConstants.HRDepartmentHead)]
        public JsonResult Void(int id)
        {
            var record = db.PlanCategorys.Find(id);
            if (record != null && record.IsDepartmentOnlineApproved && !record.IsOnlineApproved && !record.IsVoid)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 10);
                record.IsOnlineApproved = false;
                record.IsSendForApproval = false;
                record.IsDepartmentOnlineApproved = false;
                record.IsDepartmentSendForApproval = false;
                record.IsVoid = true;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                record.ForwardedFrom = User.Identity.Name;
                record.ForwardedTo = User.Identity.Name;
                db.PlanCategorys.Attach(record);
                db.Entry(record).State = EntityState.Modified;
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
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }


            if (!record.IsDepartmentOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is not approved hence you can modify by editing.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }



            if (record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is approved at Budget hence you can't void here.",
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