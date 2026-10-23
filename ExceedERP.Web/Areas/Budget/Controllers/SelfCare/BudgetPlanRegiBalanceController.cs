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
using ExceedERP.Web.Areas.Budget.Models;
using System.Threading.Tasks;
using ExceedERP.Helpers.Budget;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;
using ExceedERP.Core.Domain.Common.HRM;
using System.Text.RegularExpressions;
using ExceedERP.Core.Domain.Common.ViewModel;
using ExceedERP.Core.Domain.Inventory;
using System.Data.Entity;
using ExceedERP.Core.Domain;

namespace ExceedERP.Web.Areas.Budget.Controllers.SelfCare
{
    [AuthorizeRoles(HrmConstants.HRDepartmentHead)]
    public class BudgetPlanRegiBalanceController : Controller
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
        public ActionResult BudgetPlanRegistration_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<int> userworkunit = UserInformationHelper.GetUserHeadWorkUnitAndChildWorkUnits(User.Identity.Name);
            List<int> userPlanCategories=db.BudgetPlans                
                .Where(x => userworkunit.Contains((int)x.OrgStructureId)).Select(x=> (int)x.OrgStructureId).ToList();

            IQueryable<PlanCategory> PlanCategory = db.PlanCategorys.Where(x=> userPlanCategories.Contains(x.PlanCategoryID));
            DataSourceResult result = PlanCategory.ToDataSourceResult(request, plan => new PlanCategory
            {
                PlanCategoryID = plan.PlanCategoryID,
                BudgetType = plan.BudgetType,
                BudgetCategoryID = plan.BudgetCategoryID,
                Name = plan.Name,
                GlFiscalYearId = plan.GlFiscalYearId,              
                CurrentTotal = GetCurentTotalPlan(plan),
                GrandTotal =GetTotalPlan(plan),
                ActualConsumption = plan.ActualConsumption,
                BudgetStatus = plan.BudgetStatus,

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

        public ActionResult UsageCategory_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<PlanCategory> PlanCategory = db.PlanCategorys.ToList();

            DataSourceResult result = PlanCategory.ToDataSourceResult(request, budgetCategory => new PlanCategory
            {
                PlanCategoryID = budgetCategory.PlanCategoryID,
                BudgetType = budgetCategory.BudgetType,
                BudgetCategoryID = budgetCategory.BudgetCategoryID,
                Name = budgetCategory.Name,
                GlFiscalYearId = budgetCategory.GlFiscalYearId,

                CreatedBy = budgetCategory.CreatedBy,
                DateCreated = budgetCategory.DateCreated,
                LastModified = budgetCategory.LastModified,
                ModifiedBy = budgetCategory.ModifiedBy,
                GrandTotal =budgetCategory.GrandTotal //GetUsageTotalPlan(budgetCategory.PlanCategoryID)

            });

            db.Dispose();
            return Json(result);
        }

       
        public ActionResult BudgetPlanRegistration_Create([DataSourceRequest]DataSourceRequest request, PlanCategory PlanCategory)
        {
            //check for duplicate entries
            List<PlanCategory> PlanCategorys =  db.PlanCategorys
                .Where(p => p.BudgetCategoryID == PlanCategory.BudgetCategoryID && p.GlFiscalYearId == PlanCategory.GlFiscalYearId)
                .ToList();

            if (PlanCategorys.Any())
            {
                ModelState.AddModelError("DuplicateEntrie", "Budget Group Already Exists !");
            }

            var cat = db.BudgetCategorys.Find(PlanCategory.BudgetCategoryID);
            if (cat != null)
                PlanCategory.BudgetType = cat.BudgetType;

            if (ModelState.IsValid)
            {
                var entity = new PlanCategory
                {
                    PlanCategoryID = PlanCategory.PlanCategoryID,
                    BudgetType = PlanCategory.BudgetType,
                    BudgetCategoryID = PlanCategory.BudgetCategoryID,
                    Name = PlanCategory.Name,
                    GrandTotal = PlanCategory.GrandTotal,
                    GlFiscalYearId = PlanCategory.GlFiscalYearId,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    Remark = PlanCategory.Remark

                };

                db.PlanCategorys.Add(entity);
                 db.SaveChanges();
                PlanCategory.PlanCategoryID = entity.PlanCategoryID;

            }


            return Json(new[] { PlanCategory }.ToDataSourceResult(request, ModelState));
        }
        
        [AcceptVerbs(HttpVerbs.Post)]
        public  ActionResult BudgetPlanRegistration_Update([DataSourceRequest]DataSourceRequest request, PlanCategory plan)
        {
            List<PlanCategory> PlanCategorys = db.PlanCategorys
                .Where(p => p.BudgetCategoryID == plan.BudgetCategoryID && p.GlFiscalYearId == plan.GlFiscalYearId && p.PlanCategoryID != plan.PlanCategoryID)
                .ToList();
            if (PlanCategorys.Any())
            {
                ModelState.AddModelError("DuplicateEntrie", " Budget Group Already Exists !");
            }

            var cat = db.BudgetCategorys.Find(plan.BudgetCategoryID);
            if (cat != null)
                plan.BudgetType = cat.BudgetType;

            if (ModelState.IsValid)
            {
                var entity = new PlanCategory
                {
                    PlanCategoryID = plan.PlanCategoryID,
                    BudgetType = plan.BudgetType,
                    BudgetCategoryID = plan.BudgetCategoryID,
                    Name = plan.Name,
                    GrandTotal = plan.GrandTotal,
                    GlFiscalYearId = plan.GlFiscalYearId,

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
        public ActionResult BudgetPlanRegistration_Destroy([DataSourceRequest]DataSourceRequest request, PlanCategory PlanCategory)
        {
           
                var entity = new PlanCategory
                {
                   PlanCategoryID = PlanCategory.PlanCategoryID
                };


                db.PlanCategorys.Attach(entity);
                db.PlanCategorys.Remove(entity);
                db.SaveChanges();
  
            return Json(new[] { PlanCategory }.ToDataSourceResult(request));
        }

        //  [AuthorizeRoles(BudgetRoles.BudgetUser)]
        //  [AcceptVerbs(HttpVerbs.Get)]
        //public ActionResult ProcessBudgetPlan(string PlanId)
        //{
        //    //get PlanCategory
        //    var PlanCategory = db.PlanCategorys.Where(x => x.PlanCategoryID.ToString() == PlanId).FirstOrDefault();
        //    var IsCreated = db.BudgetPlans.Where(x => x.PlanCategoryId.ToString() == PlanId).Any();
        //    if (PlanCategory != null && !IsCreated)
        //    {
        //        //contains PlanCategory, BudgetCategory, GIFiscalYear, and GLPeriod
        //        AccountCodeRangeViewModel model = new AccountCodeRangeViewModel();
        //        //get BudgetCategory using planCategory.Name
        //        var BudgetCategory = db.BudgetCategorys.Where(x => x.Name == PlanCategory.Name).FirstOrDefault();
        //        //get GlFiscalYear using plancategory.GlfiscalyearId they have 1-1r/n
        //        var GLFiscalYears = db.GLFiscalYears.Where(x => x.GlFiscalYearId == PlanCategory.GlFiscalYearId).FirstOrDefault();
        //        //GlPeriods months in GLFiscalYear 1-mr/n  
        //        var GLPeriods = db.GLPeriods.Where(x => x.GlFiscalYearId == PlanCategory.GlFiscalYearId).ToList();

        //        //assignes them to the AccounCodeRangeViewModel class
        //        if (BudgetCategory != null)
        //        {
        //            model.BudgetCategory = BudgetCategory;
        //            model.PlanCategory = PlanCategory;
        //            model.GlFiscalYear = GLFiscalYears;
        //            model.GLPeriods = GLPeriods;

        //        }
        //        await Task.Run(async () =>
        //        {
        //          await  CreateBudgetPlan(model);
        //        });
        //    }

        //    return Json(true,JsonRequestBehavior.AllowGet);
        //}
       

        public JsonResult SendForApproval(int id)
        {
            var record = db.PlanCategorys.Find(id);
            #region lines validation
            var lines=db.BudgetPlans.Where(x=>x.PlanCategoryId==id).AsQueryable();
            if (!lines.Any())
            {
                var formattedData = new
                {
                    Message = "Please add budget plan line items first.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            #endregion
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                db.PlanCategorys.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();


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

        
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult BudgetPlanRegistration_RemoveDependancy(string PlanId)
        {
         var BudgetPlans = db.BudgetPlans.Where(x => x.PlanCategoryId.ToString() ==  PlanId).ToList();

                  foreach (var plan in BudgetPlans)
                  {
                      var budgetPlansMonthlys = db.BudgetPlanMonthlys.Where(x => x.BudgetDefineID == plan.BudgetPlanID).ToList();

                      db.BudgetPlanMonthlys.RemoveRange(budgetPlansMonthlys);
                        db.SaveChanges();

                  }
                  db.BudgetPlans.RemoveRange(BudgetPlans);
                    db.SaveChanges();
                  UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Plan Category number " +  PlanId);
                  return RedirectToAction("Index", new  { Reset="Reset" });
                }
          


        
        
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

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }

}