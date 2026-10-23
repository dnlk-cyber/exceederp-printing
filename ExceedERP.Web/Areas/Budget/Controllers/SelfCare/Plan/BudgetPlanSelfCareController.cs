using ExceedERP.DataAccess.Context;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using ExceedERP.Core.Domain.Budget;
using System.Data.Entity;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Areas.Budget.Models;
using System.Net;
using ExceedERP.Core.Domain.Common.HRM;
using System.IO;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Helpers.Finance;
using ExceedERP.Helpers.Budget;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;
using ExceedERP.Helpers.HR;
using ExceedERP.Core.Domain;
using ExceedERP.Web.Helpers;
using ExcelDataReader;

namespace ExceedERP.Web.Areas.Budget.Controllers
{
    [AuthorizeRoles(HrmConstants.HRDepartmentHead)]
    public class BudgetPlanSelfCareController : Controller
    {

        private ExceedDbContext db = new ExceedDbContext();
        private AccountHelper AccountHelper = new AccountHelper();
        private DynamicAccountHelper _dynamicAccount = new DynamicAccountHelper();

        
        public ActionResult BudgetPlan_Read( [DataSourceRequest]DataSourceRequest request, int Id)
        {
            IQueryable<BudgetPlan> BudgetPlan = db.BudgetPlans.Where(x => x.PlanCategoryId == Id);
            DataSourceResult result = BudgetPlan.ToDataSourceResult(request, budgetPlan => new BudgetPlan
            {
                BudgetPlanID = budgetPlan.BudgetPlanID,
                CostCenter = budgetPlan.CostCenter,
                BranchId = budgetPlan.BranchId,
                BusinessUnitId = budgetPlan.BusinessUnitId,
                BudgetCode = budgetPlan.BudgetCode,
                BudgetItem = budgetPlan.BudgetItem,
                AccountCode = budgetPlan.AccountCode,
                AccountDesc = budgetPlan.AccountDesc,
                FiscalYearId = budgetPlan.FiscalYearId,            
                TotalYearlyBudget =budgetPlan.TotalYearlyBudget,// GetTotalPlan(budgetPlan),
                Unit = budgetPlan.Unit,                
                CurrentYearlyBudget =budgetPlan.CurrentYearlyBudget,// GetCurentTotalPlan(budgetPlan),
                PlanCategoryId = budgetPlan.PlanCategoryId,
                CreatedBy = budgetPlan.CreatedBy,
                DateCreated = budgetPlan.DateCreated,
                LastModified = budgetPlan.LastModified,
                ModifiedBy = budgetPlan.ModifiedBy,
            
                OrgBranchName = budgetPlan.OrgBranchName
               
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetPlan_Create( [DataSourceRequest]DataSourceRequest request, BudgetPlan budgetPlan, int Id)
        {
            #region data 
            int employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

            if (employeeId != 0)
            {
                var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == employeeId && x.isCurrent);
                if (placement != null)
                {
                    budgetPlan.BranchId = placement.BranchId;
                    budgetPlan.BusinessUnitId = placement.BusinessUnitId;
                }
                else
                {
                    ModelState.AddModelError("placement", "You have not current placemnt");
                }
            }

            #endregion
            #region validation
            List<BudgetPlan> BudgetPlans = db.BudgetPlans
                .Where(p => p.BudgetItem == budgetPlan.BudgetItem && p.PlanCategoryId == Id)
                .Where(p => p.BusinessUnitId == budgetPlan.BusinessUnitId)
                .Where(p => p.BranchId == budgetPlan.BranchId)
                .Where(p => p.CostCenter == budgetPlan.CostCenter)
                .ToList();

            if (BudgetPlans.Any())
                ModelState.AddModelError("DuplicateEntries", " Budget Plan Already Exists !");

            if(GlobalHelper.IsMultipleBusinessUnit()&&budgetPlan.BusinessUnitId==0)
                ModelState.AddModelError("bizunit", "Please select business unit!");

            if (budgetPlan.BranchId==null && budgetPlan.CostCenter == null)
                ModelState.AddModelError("branch or work unit", "Please select at least branch/project or work unit!");
            #endregion

            #region get account code
            var planCategory = db.PlanCategorys.Find(Id);
            var budgetCategory = db.BudgetCategorys.Find(planCategory.BudgetCategoryID);

            var budgetCategoryDetail = db.BudgetCategoryDetails
                .Where(x => x.BudgetCategoryId == budgetCategory.BudgetCategoryID)
                .Where(x => x.BudgetCategoryDetailID.ToString() == budgetPlan.BudgetItem).FirstOrDefault();

            if (budgetCategoryDetail != null)
            {
                budgetPlan.BudgetCode = budgetCategoryDetail.BudgetCode;
                budgetPlan.AccountCode = budgetCategoryDetail.AccountCode;
                if(budgetPlan.BusinessUnitId!=0)
                {
                    int buziUnitindex = _dynamicAccount.GetBizUnitAccountSequenceIndex();
                    string buziUnitAccountSeG = HrHelper.GetBusinessUnitSectorAccount(budgetPlan.BusinessUnitId);
                    budgetPlan.AccountCode = _dynamicAccount.ReplaceSegment(budgetPlan.AccountCode, buziUnitindex, buziUnitAccountSeG);
                }
                if(budgetPlan.BranchId!=null)
                {
                    int locationindex = _dynamicAccount.GetBranchAccountSequenceIndex();
                    string locationAccountSeG = HrHelper.GetBranchLocationAccount((int)budgetPlan.BranchId);
                    budgetPlan.AccountCode = _dynamicAccount.ReplaceSegment(budgetPlan.AccountCode, locationindex, locationAccountSeG);
                }
                if (budgetPlan.CostCenter != null)
                {
                    int costcenterIndex = _dynamicAccount.GetCostCenterAccountSequenceIndex();
                    budgetPlan.AccountCode = _dynamicAccount.ReplaceSegment(budgetPlan.AccountCode, costcenterIndex, budgetPlan.CostCenter);
                }

            }
            #endregion

            

            if (ModelState.IsValid)
            {
                var entity = new BudgetPlan
                {
                    PlanCategoryId = Id,
                    BudgetPlanID = budgetPlan.BudgetPlanID,
                    CostCenter = budgetPlan.CostCenter,
                    BranchId = budgetPlan.BranchId,
                    BusinessUnitId = budgetPlan.BusinessUnitId,
                    BudgetCode = budgetPlan.BudgetCode,
                    BudgetItem = budgetPlan.BudgetItem,
                    AccountCode = budgetPlan.AccountCode,
                    AccountDesc = AccountHelper.GetAccountDesc(budgetPlan.AccountCode),
                    FiscalYearId = budgetPlan.FiscalYearId,
                   
                   
                    TotalYearlyBudget = budgetPlan.TotalYearlyBudget,
                    CurrentYearlyBudget = budgetPlan.TotalYearlyBudget, //changed from budgetPlan.TotalYearlyBudget

                    Unit = budgetPlan.Unit,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name
                };

                db.BudgetPlans.Add(entity);
                db.SaveChanges();
                UpdateYearlyTotalPlan(entity);
                DistributeMonthlyBudget(entity.BudgetPlanID);


                budgetPlan.BudgetPlanID = entity.BudgetPlanID;
            }


            return Json(new[] { budgetPlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetPlan_Update([DataSourceRequest]DataSourceRequest request, BudgetPlan budgetPlan)
        {
            #region validation
            List<BudgetPlan> BudgetPlans = db.BudgetPlans
                .Where(p => p.BudgetItem == budgetPlan.BudgetItem && p.BudgetPlanID != budgetPlan.BudgetPlanID && p.PlanCategoryId == budgetPlan.PlanCategoryId)
                .Where(p => p.BusinessUnitId == budgetPlan.BusinessUnitId)
                .Where(p => p.BranchId == budgetPlan.BranchId)
                .Where(p => p.CostCenter == budgetPlan.CostCenter)
                .ToList();
            if (BudgetPlans.Any())
                ModelState.AddModelError("DuplicateEntrie", " Budget Plan Already Exists !");


            if (GlobalHelper.IsMultipleBusinessUnit() && budgetPlan.BusinessUnitId == 0)
                ModelState.AddModelError("bizunit", "Please select business unit!");

            if (budgetPlan.BranchId == null && budgetPlan.CostCenter == null)
                ModelState.AddModelError("branch or work unit", "Please select at least branch/project or cost center!");
            #endregion

            #region get account code
            var planCategory = db.PlanCategorys.Find(budgetPlan.PlanCategoryId);
            var budgetCategory = db.BudgetCategorys.Find(planCategory.BudgetCategoryID);

            var budgetCategoryDetail = db.BudgetCategoryDetails
                .Where(x => x.BudgetCategoryId == budgetCategory.BudgetCategoryID)
                .Where(x => x.BudgetCategoryDetailID.ToString() == budgetPlan.BudgetItem).FirstOrDefault();

            if (budgetCategoryDetail != null)
            {
                budgetPlan.BudgetCode = budgetCategoryDetail.BudgetCode;
                budgetPlan.AccountCode = budgetCategoryDetail.AccountCode;
                if (budgetPlan.BusinessUnitId != 0)
                {
                    int buziUnitindex = _dynamicAccount.GetBizUnitAccountSequenceIndex();
                    string buziUnitAccountSeG = HrHelper.GetBusinessUnitSectorAccount(budgetPlan.BusinessUnitId);
                    budgetPlan.AccountCode = _dynamicAccount.ReplaceSegment(budgetPlan.AccountCode, buziUnitindex, buziUnitAccountSeG);
                }
                if (budgetPlan.BranchId != null)
                {
                    int locationindex = _dynamicAccount.GetBranchAccountSequenceIndex();
                    string locationAccountSeG = HrHelper.GetBranchLocationAccount((int)budgetPlan.BranchId);
                    budgetPlan.AccountCode = _dynamicAccount.ReplaceSegment(budgetPlan.AccountCode, locationindex, locationAccountSeG);
                }
                if (budgetPlan.CostCenter != null)
                {
                    int costcenterIndex = _dynamicAccount.GetCostCenterAccountSequenceIndex();
                    budgetPlan.AccountCode = _dynamicAccount.ReplaceSegment(budgetPlan.AccountCode, costcenterIndex, budgetPlan.CostCenter);
                }

            }
            #endregion

            if (ModelState.IsValid)
            {
                var entity = new BudgetPlan
                {
                    BudgetPlanID = budgetPlan.BudgetPlanID,
                    CostCenter = budgetPlan.CostCenter,
                    BranchId = budgetPlan.BranchId,
                    BusinessUnitId = budgetPlan.BusinessUnitId,
                    BudgetCode = budgetPlan.BudgetCode,
                    BudgetItem = budgetPlan.BudgetItem,
                    AccountCode = budgetPlan.AccountCode,
                    AccountDesc = budgetPlan.AccountDesc,
                    FiscalYearId = budgetPlan.FiscalYearId,
                    CurrentYearlyBudget = budgetPlan.TotalYearlyBudget,//editing still not approved
                    TotalYearlyBudget = budgetPlan.TotalYearlyBudget,
                    PlanCategoryId = budgetPlan.PlanCategoryId,
                 
                    Unit = budgetPlan.Unit,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name
                };

                db.BudgetPlans.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                UpdateYearlyTotalPlan(budgetPlan);
                DistributeMonthlyBudget(budgetPlan.BudgetPlanID);

            }

            return Json(new[] { budgetPlan }.ToDataSourceResult(request, ModelState));
        }



        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult BudgetPlan_BatchUpdate([DataSourceRequest]DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<BudgetPlan> BudgetPlan)
        //{
        //    foreach (var budgetPlan in BudgetPlan)
        //    {
        //        List<BudgetPlan> BudgetPlans = db.BudgetPlans.Where(p => p.AccountCode == budgetPlan.AccountCode &&                                                                        
        //                                                                 p.BudgetPlanID != budgetPlan.BudgetPlanID &&
        //                                                                 p.PlanCategoryId == budgetPlan.PlanCategoryId
        //                                                                 ).ToList();
        //        //List<BudgetPlan> BudgetPlans = new List<BudgetPlan>();
        //        if (BudgetPlans.Any())
        //        {
        //            ModelState.AddModelError("DuplicateEntrie", " Budget Plan Already Exists !");
        //        }
        //        else
        //        {
        //            if (ModelState.IsValid)
        //            {
        //                var entity = new BudgetPlan
        //                {
        //                    BudgetPlanID = budgetPlan.BudgetPlanID,
        //                    OrgStructureId = budgetPlan.OrgStructureId,
        //                    BranchId = budgetPlan.BranchId,
        //                    BusinessUnitId = budgetPlan.BusinessUnitId,
        //                    BudgetItem = budgetPlan.BudgetItem,
        //                    AccountCode = budgetPlan.AccountCode,
        //                    AccountDesc = budgetPlan.AccountDesc,
        //                    FiscalYearId = budgetPlan.FiscalYearId,
        //                    TotalYearlyBudget = budgetPlan.TotalYearlyBudget,
        //                    CurrentYearlyBudget = budgetPlan.CurrentYearlyBudget,
        //                    PlanCategoryId = budgetPlan.PlanCategoryId,
                            
        //                    Unit = budgetPlan.Unit,
        //                    LastModified = DateTime.Now,
        //                    ModifiedBy = User.Identity.Name
        //                };

        //                db.BudgetPlans.Attach(entity);
        //                db.Entry(entity).State = EntityState.Modified;
        //                db.SaveChanges();
        //                var BudgetPlanMonths = db.BudgetPlanMonthlys.Where(x => x.BudgetPlanID == budgetPlan.BudgetPlanID).ToList();
        //                foreach (var BudgetPlanMonth in BudgetPlanMonths)
        //                {
        //                    db.BudgetPlanMonthlys.Attach(BudgetPlanMonth);
        //                    db.BudgetPlanMonthlys.Remove(BudgetPlanMonth);
        //                    db.SaveChanges();
        //                }
        //                List<BindingItem> months = GetMonths(budgetPlan.PlanCategoryId);
        //                if (months.Any())
        //                {
        //                    int intervals = months.Count;
        //                    decimal monthBuget = budgetPlan.TotalYearlyBudget / intervals;
        //                    var Currency = db.FinSetupCurrencies.FirstOrDefault();
        //                    foreach (var month in months)
        //                    {

        //                        var budgetpermonth = new BudgetPlanMonthly
        //                        {
        //                            GLPeriodId = month.Value,
        //                            MonthlyBudget = monthBuget,
        //                            CurrentBudget = monthBuget,
        //                            BudgetPlanID = entity.BudgetPlanID,
        //                            DateCreated = DateTime.Now,
        //                            CreatedBy = User.Identity.Name

        //                        };

        //                        db.BudgetPlanMonthlys.Add(budgetpermonth);
        //                        db.SaveChanges();

        //                    }
        //                }

        //            }
        //        }
        //    }

        //    return Json(BudgetPlan.ToDataSourceResult(request, ModelState));
        //}


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetPlan_Destroy([DataSourceRequest]DataSourceRequest request, BudgetPlan budgetPlan)
        {
            if (ModelState.IsValid)
            {
                var entity = new BudgetPlan
                {
                    BudgetPlanID = budgetPlan.BudgetPlanID,
                    CostCenter = budgetPlan.CostCenter,
                    BranchId = budgetPlan.BranchId,
                    BusinessUnitId = budgetPlan.BusinessUnitId,
                    BudgetCode = budgetPlan.BudgetCode,
                    BudgetItem = budgetPlan.BudgetItem,
                    AccountCode = budgetPlan.AccountCode,
                    AccountDesc = budgetPlan.AccountDesc,
                    FiscalYearId = budgetPlan.FiscalYearId,
                    CurrentYearlyBudget = budgetPlan.CurrentYearlyBudget,
                    TotalYearlyBudget = budgetPlan.TotalYearlyBudget,
                    PlanCategoryId = budgetPlan.PlanCategoryId,


                };

                db.BudgetPlans.Attach(entity);
                db.BudgetPlans.Remove(entity);
                db.SaveChanges();

                #region delete related
                var budgetPlansMonthlys = db.BudgetPlanMonthlys.Where(x => x.BudgetDefineID == budgetPlan.BudgetPlanID).ToList();
                db.BudgetPlanMonthlys.RemoveRange(budgetPlansMonthlys);
                db.SaveChanges();
                #endregion

                UpdateYearlyTotalPlan(budgetPlan);
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted budget Plan Monthly number " + budgetPlan.BudgetPlanID);

            }

            return Json(new[] { budgetPlan }.ToDataSourceResult(request, ModelState));
        }


        public decimal GetTotalPlan(BudgetPlan budgetPlan)
        {
            decimal TotalPerCategory = 0;

            try
            {

                TotalPerCategory = db.BudgetPlanMonthlys
                  .Where(x => x.BudgetDefineID == budgetPlan.BudgetPlanID)
                  .Sum(x => x.MonthlyBudget);

            }
            catch (Exception e)
            {
                return 0;
            }
            return TotalPerCategory;

        }
        public decimal GetCurentTotalPlan(BudgetPlan budgetPlan)
        {
            decimal TotalPerCategory = 0;

            try
            {

                TotalPerCategory = db.BudgetPlanMonthlys
                    .Where(x => x.BudgetDefineID == budgetPlan.BudgetPlanID)
                    .Sum(x => x.CurrentBudget);

            }
            catch (Exception e)
            {
                return 0;
            }
            return TotalPerCategory;

        }
        
        public ActionResult BudgetPlanDDBind(string Budgetplan)
        {

            var customers = db.BudgetPlans.Select(s => new SelectListItem
            {
                Text = s.AccountDesc,
                Value = s.BudgetPlanID.ToString()
            });

            return Json(customers, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult AccCodeByGroupYear(int PlanCategoryID)
        {
            List<SelectListItem> result = new List<SelectListItem>();
             var plans = db.BudgetPlans.AsNoTracking().Where(x => x.PlanCategoryId == PlanCategoryID).Select(s => new SelectListItem
            {
                Text = s.AccountCode,
                Value = s.BudgetPlanID.ToString()
            });

            string CompareString = "";
            foreach (var plan in plans)
            {
                if (plan.Text != CompareString)
                {
                    result.Add(plan);
                }
                CompareString = plan.Text;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        
        public ActionResult BudgetGLChartOfAccountCostCenterId(int PlanCategoryID, string AccCode)
        {
            List<SelectListItem> workUnit = new List<SelectListItem>();
            //  PlanCategory planCatagory = db.PlanCategorys.Where(x => x.FiscalYearId == Year && x.Name == Group ).FirstOrDefault();
            var plans = db.BudgetPlans.Where(x => x.PlanCategoryId == PlanCategoryID && x.AccountCode == AccCode).ToList();

            foreach (var plan in plans)
            {
                var costcenterSeg = db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.CostCenter);
                var glChartOfAccountCostCenters = db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == costcenterSeg.GLSegmentSetupId);
                //var strucure = glChartOfAccountCostCenters.Where(x => x.Values == plan.GLChartOfAccountCostCenterValue.ToString()).FirstOrDefault();


                //var item = new SelectListItem
                // {
                //     Text = strucure.Description,
                //     Value = strucure.Values
                // };
                //workUnit.Add(item);
            }

            return Json(workUnit.Distinct().ToList(), JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult BudgetToWorkUnit(int PlanCategoryID, string AccCode, string GLChartOfAccountCostCenterId)
        {
            List<SelectListItem> workUnit = new List<SelectListItem>();
            var plans = db.BudgetPlans.Where(x => x.PlanCategoryId == PlanCategoryID && x.AccountCode == AccCode).ToList();

            foreach (var plan in plans)
            {
                var costcenterSeg = db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.CostCenter);
                var glChartOfAccountCostCenters = db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == costcenterSeg.GLSegmentSetupId);
                //var strucure = glChartOfAccountCostCenters.Where(x => x.Values == plan.GLChartOfAccountCostCenterValue.ToString()).FirstOrDefault();

                //if (strucure != null)
                //{
                //    var item = new SelectListItem
                //    {
                //        Text = strucure.Description,
                //        Value = strucure.Values.ToString()
                //    };
                //    workUnit.Add(item);
                //}
            }

            return Json(workUnit.Distinct().ToList(), JsonRequestBehavior.AllowGet);
        }
        //
        //public ActionResult ApprovalDDBind()
        //{
        //    var approvals = new List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject>();
        //    var approval = new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject()
        //    {
        //        Name = BudgetConstants.Approved,
        //        Id = 1,
        //    };
        //    approvals.Add(approval);

        //    var approval2 = new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject()
        //    {
        //        Name = BudgetConstants.Pending,
        //        Id = 2,
        //    };
        //    approvals.Add(approval2);
        //    var approval3 = new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject()
        //    {
        //        Name = BudgetConstants.Rejected,
        //        Id = 3,
        //    };
        //    approvals.Add(approval3);
        //    return Json(approvals, JsonRequestBehavior.AllowGet);
        //}
        
        public ActionResult WorkUnitDDBind()
        {
            var costcenterSeg = db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.CostCenter);
            var glChartOfAccountCostCenters = db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == costcenterSeg.GLSegmentSetupId);
           
            var workunit = glChartOfAccountCostCenters.Where(x => x.AllowBudgeting == true && x.Enable == true).Select(s => new SelectListItem
            {
                Text = s.Description,
                Value = s.Values
            });

            return Json(workunit, JsonRequestBehavior.AllowGet);
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
        
        public ActionResult GetUnits()
        {
            List<SelectListItem> units = new List<SelectListItem>();
            var currency = db.FinSetupCurrencies.Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.Name.ToString()
            });


            return Json(currency, JsonRequestBehavior.AllowGet);

        }

        public ActionResult BudgetPlan_Import([DataSourceRequest] DataSourceRequest request, IEnumerable<HttpPostedFileBase> file)
        {
            string errors = string.Empty;
            string errorsummary = " Successfully Imported Budget Plan ";

            string addtionalName = DateTime.Now.Year.ToString() +
                DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() +
                DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() +
                DateTime.Now.Second.ToString();
            System.Data.DataSet ds = new System.Data.DataSet();
            Person_Employee issuee = db.Person_Employee.FirstOrDefault();
            List<Person_Employee> issues = new List<Person_Employee>();

            foreach (HttpPostedFileBase fileBase in file)
            {
                Stream stream = fileBase.InputStream;
                // We return the interface, so that
                IExcelDataReader reader = null;
                string fileExtension = System.IO.Path.GetExtension(Request.Files["file"].FileName);
                int t = 0;

                if (fileBase.FileName.EndsWith(".xls"))
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                    t = reader.AsDataSet().Tables.Count;
                }
                else if (fileBase.FileName.EndsWith(".xlsx"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    t = reader.AsDataSet().Tables.Count;
                }
                else
                {
                    ModelState.AddModelError("File", "This file format is not supported");
                    return View();
                }

                int V = t;
                //reader.IsFirstRowAsColumnNames = true;

                ds = reader.AsDataSet();
 
                errors = string.Empty;
                decimal TotalBudget = decimal.Zero;
               int BudgetPlanMonthId = 0;

            var  dt = ds.Tables[0];

            for (int i = 1; i < dt.Rows.Count; i++ )
            {

                var raw = dt.Rows[i][7].ToString().Trim();
                if (raw != "Amount")
                {
                    BudgetPlanMonthId = int.Parse(reader.GetString(0));

                    decimal Amount = decimal.Parse(raw);
                    TotalBudget = TotalBudget + Amount;
                }
            }

                var BudgetMonthly = db.BudgetPlanMonthlys.Where(x=>x.BudgetPlanMonthlyID == BudgetPlanMonthId).FirstOrDefault();

                var PlanCategory = db.PlanCategorys.Where(x => x.PlanCategoryID == BudgetMonthly.BudgetPlan.PlanCategoryId).FirstOrDefault();
                  
                if (PlanCategory!= null)
                {

                    if (PlanCategory.IsSendForApproval && !PlanCategory.IsOnlineApproved)
                    {
                        return Json(new { data = " Plan Is Waiting Approval!" });
                    }
                    if (  PlanCategory.IsOnlineApproved)
                    {
                        return Json(new { data = " Plan Is Approved!" });
                    }
                    if (TotalBudget > PlanCategory.GrandTotal)
                    {
                        errorsummary = " Entered Value is Greater Than Planned Grand Total!";
                     }
                }
                List<BudgetPlanMonthly> BudgetPlanMonthlyList = new List<BudgetPlanMonthly>();
                while (reader.Read())
                {
                    var raw = reader.GetString(7).Trim();
                    if (raw != "Amount")
                    {
                        var CurrentBudget = decimal.Parse(raw);
                        var MonthlyId = int.Parse(reader.GetString(0));

                      
                      
                        var BudgetPlanMonth = db.BudgetPlanMonthlys.Where(x => x.BudgetPlanMonthlyID == MonthlyId).FirstOrDefault();
                        if (BudgetPlanMonth != null)
                        {

                            try
                            {

                                BudgetPlanMonth.CurrentBudget = CurrentBudget;
                                db.BudgetPlanMonthlys.Attach(BudgetPlanMonth);
                                db.Entry(BudgetPlanMonth).State = EntityState.Modified;
               
                                int result = db.SaveChanges();
                                

                            }
                            catch (Exception e)
                            {
                            
                            }

                        }



                        if (errors != string.Empty)
                        {
                            errorsummary = errors + " Numbers Failed Import";
                        }



                    }
                }
            }



            return Json(new { data = errorsummary });

        }
        
        public string GetStatus(int Id)
        {

            PlanCategory Usage = db.PlanCategorys.Where(j => j.PlanCategoryID == Id).FirstOrDefault();
            if (Usage.IsSendForApproval == true || Usage.IsOnlineApproved == true)
            {
                return BudgetConstants.Approved;
            }

            return BudgetConstants.Pending;


        }
        public ActionResult DownloadTemplate(string filename)
        {


            db = new ExceedDbContext();
            try
            {

                WebClient req = new WebClient();
                HttpResponse response = System.Web.HttpContext.Current.Response;
                response.Clear();
                response.ClearContent();
                response.ClearHeaders();
                response.Buffer = true;
                response.AddHeader("Content-Disposition",
                    "attachment;filename=\"" + filename + "\"");
                byte[] datas = req.DownloadData(Server.MapPath("~/Content/Template/Budget/" + filename));
                response.BinaryWrite(datas);
                response.End();

                return File(filename, "application/vnd.ms-excel");
            }

            catch (Exception)
            {

              
            }

            return null;
        } 

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        private void UpdateYearlyTotalPlan(BudgetPlan plan)
        {

            var budgetPlanCat = db.PlanCategorys.Where(x => x.PlanCategoryID == plan.PlanCategoryId).FirstOrDefault();
            if (budgetPlanCat != null)
            {
                var list = db.BudgetPlans
                 .Where(x => x.PlanCategoryId == plan.PlanCategoryId).ToList();

                if (list.Any())
                {
                    budgetPlanCat.GrandTotal = list.Sum(x => x.TotalYearlyBudget);
                    budgetPlanCat.CurrentTotal = list.Sum(x => x.CurrentYearlyBudget);
                    db.PlanCategorys.Attach(budgetPlanCat);
                    db.Entry(budgetPlanCat).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }

        }
        #region functions --new
        private void DistributeMonthlyBudget(int planId)
        {
            var budgetPlan = db.BudgetPlans.Find(planId);
            if (budgetPlan != null)
            {
                #region remove exisiting data
                var planMonthly = db.BudgetPlanMonthlys.Where(x => x.BudgetDefineID == planId).ToList();
                db.BudgetPlanMonthlys.RemoveRange(planMonthly);
                db.SaveChanges();
                #endregion
                decimal amount = budgetPlan.TotalYearlyBudget / 12;
                var plancategory = db.PlanCategorys.Find(budgetPlan.PlanCategoryId);
                var periods = db.GLPeriods.Where(x => x.GlFiscalYearId == plancategory.GlFiscalYearId).OrderBy(x => x.DateFrom).ToList();
                foreach (var p in periods)
                {
                    var entity = new BudgetPlanMonthly
                    {
                      
                        GLPeriodId = p.GLPeriodId,
                        MonthlyBudget = amount,
                        RevisedBudget = 0,
                        CurrentBudget = amount,
                        TransferedBudget = 0,
                        AllocatedBudget = amount,
                        BudgetDefineID = planId,
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name

                    };
                    
                    db.BudgetPlanMonthlys.Add(entity);
                    db.SaveChanges();
                }
            }
        }
        #endregion

        #region Support Func

        private string GetBudgetStatus(int CategoryId, int budgetId)
        {
            string stat = "Bad";
            BudgetGLJournalHelper budgetGLJournalHelper = new BudgetGLJournalHelper();
            try
            {
                var planCategory = db.PlanCategorys.Where(x => x.PlanCategoryID == CategoryId).FirstOrDefault();
                var Category = db.BudgetCategorys.Where(x => x.Name == planCategory.Name).FirstOrDefault();
                var Control = db.BudgetControls.Where(x => x.BudgetControlId == Category.BudgetControlId).FirstOrDefault();

                decimal TotalPerCategory = 0;
                var Budgetplan = db.BudgetPlans.Where(x => x.BudgetPlanID == budgetId).FirstOrDefault();
                TotalPerCategory = Budgetplan.CurrentYearlyBudget;

                var Journal = db.GLRecordJournals.Where(x => x.FiscalYear == planCategory.GlFiscalYear.Name && x.IsOnlineApproved == true).FirstOrDefault();
                var JournalDetail = db.GLRecordJournalDetails.Where(x => x.GlRecordJournalId == Journal.GlRecordJournalId && x.AccountCode == Budgetplan.AccountCode).ToList();


                if (JournalDetail != null && JournalDetail.Any())
                {
                    var UsagePercentage = budgetGLJournalHelper.GetNetJournalsBalance( JournalDetail);
                    var GrowthBudgetPlan = (TotalPerCategory * (Control.Percentage) / 100) + Control.ReserveAmount + TotalPerCategory;
                    if (UsagePercentage > GrowthBudgetPlan)
                    {
                        return "Bad";
                    }
                    else
                    {
                        return "Good";
                    }
                }
                return string.Empty;
            }
            catch (Exception e)
            { return string.Empty; }



        }

        private decimal GetProgress(int CategoryId, int budgetId)
        {
            try
            {
                BudgetGLJournalHelper budgetGLJournalHelper = new BudgetGLJournalHelper();

                var Category = db.BudgetCategorys.Where(x => x.BudgetCategoryID == CategoryId).FirstOrDefault();
                var Control = db.BudgetControls.Where(x => x.Name == Category.Name).FirstOrDefault();

                decimal TotalPerCategory = 0;
                var Budgetplan = db.BudgetPlans.Where(x => x.BudgetPlanID == budgetId).FirstOrDefault();
                TotalPerCategory = db.BudgetPlans.Where(x => x.PlanCategoryId == CategoryId).Sum(x => x.CurrentYearlyBudget);

                var year = db.GLFiscalYears.Find(Budgetplan.FiscalYearId);
                var Journal = db.GLRecordJournals.Where(x => x.FiscalYear == year.Name && x.IsOnlineApproved == true).FirstOrDefault();
                var JournalDetail = db.GLRecordJournalDetails.Where(x => x.GlRecordJournalId == Journal.GlRecordJournalId && x.AccountCode == Budgetplan.AccountCode).ToList();

                var Percentage = (budgetGLJournalHelper.GetJournalNetBalance(JournalDetail) / Budgetplan.TotalYearlyBudget) * 100;

                return 50;
            }
            catch (Exception e)
            { return 0; }



        }



        private string GetGLPeriodIdlyStatus(int CategoryId, int budgetId, string period)
        {
            string stat = "Bad";
            try
            {

                var Category = db.BudgetCategorys.Where(x => x.BudgetCategoryID == CategoryId).FirstOrDefault();
                var Control = db.BudgetControls.Where(x => x.Name == Category.Name).FirstOrDefault();

                decimal TotalPerCategory = 0;
                var Budgetplan = db.BudgetPlans.Where(x => x.BudgetPlanID == budgetId).FirstOrDefault();
                TotalPerCategory = db.BudgetPlans.Where(x => x.PlanCategoryId == CategoryId).Sum(x => x.CurrentYearlyBudget);

                var year = db.GLFiscalYears.Find(Budgetplan.FiscalYearId);
                var Journal = db.GLRecordJournals.Where(x => x.FiscalYear ==year.Name && x.PeriodName == period && x.IsOnlineApproved == true).FirstOrDefault();
                var JournalDetail = db.GLRecordJournalDetails.Where(x => x.GlRecordJournalId == Journal.GlRecordJournalId && x.AccountCode == Budgetplan.AccountCode );

                var Percentage = (JournalDetail.Sum(x => x.Credit) / Budgetplan.TotalYearlyBudget) * 100;
                /* if (Percentage > Control.Percentage)
                 {
                     return "Bad";
                 }
                 else
                 {
                     return "Good";
                 }*/
                return "Good";
            }
            catch (Exception e)
            { return string.Empty; }



        }

        private decimal GetMonthlyProgress(int CategoryId, int budgetId, string period)
        {
            try
            {

                var Category = db.BudgetCategorys.Where(x => x.BudgetCategoryID == CategoryId).FirstOrDefault();
                var Control = db.BudgetControls.Where(x => x.Name == Category.Name).FirstOrDefault();

                decimal TotalPerCategory = 0;
                var Budgetplan = db.BudgetPlans.Where(x => x.BudgetPlanID == budgetId).FirstOrDefault();
                TotalPerCategory = db.BudgetPlans.Where(x => x.PlanCategoryId == CategoryId).Sum(x => x.CurrentYearlyBudget);

                var year = db.GLFiscalYears.Find(Budgetplan.FiscalYearId);
                var Journal = db.GLRecordJournals.Where(x => x.FiscalYear == year.Name && x.PeriodName == period && x.IsOnlineApproved == true).FirstOrDefault();
                var JournalDetail = db.GLRecordJournalDetails.Where(x => x.GlRecordJournalId == Journal.GlRecordJournalId && x.AccountCode == Budgetplan.AccountCode);

                var Percentage = (JournalDetail.Sum(x => x.Credit) / Budgetplan.TotalYearlyBudget) * 100;

                return 50;
            }
            catch (Exception e)
            { return 0; }



        }


        #endregion

        #region Finance


        private ExceedDbContext _db = new ExceedDbContext();




        public JsonResult GetAccountBudgets(string Category)
        {

            BudgetCategory budgetCategory = _db.BudgetCategorys.Where(x => x.Name == Category).FirstOrDefault();
            if (budgetCategory != null)
            {
                string fromSegment = budgetCategory.FromAccountCode.Split('-')[0];
                string toSegment = budgetCategory.ToAccountCode.Split('-')[0];


                List<GLChartOfAccountAccount> accounts = _db.GLChartOfAccountAccounts.Where(x => x.Enable).Where(x => x.Values.ToUpper().CompareTo(fromSegment) >= 0)
                       .Where(x => x.Values.ToUpper().CompareTo(toSegment) <= 0)
                       .ToList();
            
                return Json(accounts.Select(c => new { value = c.Values, desc = c.Values + "_" + c.Description }), JsonRequestBehavior.AllowGet);

            }
            return Json(string.Empty, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetSubAccountBudgets(string Category)
        {

            BudgetCategory budgetCategory = _db.BudgetCategorys.Where(x => x.Name == Category).FirstOrDefault();
            if (budgetCategory != null)
            {
                string fromSegment = budgetCategory.FromAccountCode.Split('-')[1];
                string toSegment = budgetCategory.ToAccountCode.Split('-')[1];


                var costcenterSeg = db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.CostCenter);
                var glChartOfAccountCostCenters = db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == costcenterSeg.GLSegmentSetupId);
                var accounts = glChartOfAccountCostCenters.Where(x => x.Enable).Where(x => x.Values.ToUpper().CompareTo(fromSegment) >= 0)
                       .Where(x => x.Values.ToUpper().CompareTo(toSegment) <= 0)
                       .ToList();
             
                return Json(accounts.Select(c => new { value = c.Values, desc = c.Values + "_" + c.Description }), JsonRequestBehavior.AllowGet);
            }

            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCostCenterBudget(string Category)
        {

            BudgetCategory budgetCategory = _db.BudgetCategorys.Where(x => x.Name == Category).FirstOrDefault();
            if (budgetCategory != null)
            {
                string fromSegment = budgetCategory.FromAccountCode.Split('-')[2];
                string toSegment = budgetCategory.ToAccountCode.Split('-')[2];


                var costcenterSeg = db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.CostCenter);
                var glChartOfAccountCostCenters = db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == costcenterSeg.GLSegmentSetupId);
               var accounts = glChartOfAccountCostCenters.Where(x => x.Enable).Where(x => x.Values.ToUpper().CompareTo(fromSegment) >= 0)
                       .Where(x => x.Values.ToUpper().CompareTo(toSegment) <= 0)
                       .ToList();
         

                return Json(accounts.Select(c => new { value = c.Values, desc = c.Values + "_" + c.Description }), JsonRequestBehavior.AllowGet);
            }


            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLocationBudgets(string Category)
        {

            BudgetCategory budgetCategory = _db.BudgetCategorys.Where(x => x.Name == Category).FirstOrDefault();
            if (budgetCategory != null)
            {
                string fromSegment = budgetCategory.FromAccountCode.Split('-')[3];
                string toSegment = budgetCategory.ToAccountCode.Split('-')[3];


                var costcenterSeg = db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.CostCenter);
                var glChartOfAccountCostCenters = db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == costcenterSeg.GLSegmentSetupId);
                var accounts = glChartOfAccountCostCenters.Where(x => x.Enable).Where(x => x.Values.ToUpper().CompareTo(fromSegment) >= 0)
                       .Where(x => x.Values.ToUpper().CompareTo(toSegment) <= 0)
                       .ToList();
             
                return Json(accounts.Select(c => new { value = c.Values, desc = c.Values + "_" + c.Description }), JsonRequestBehavior.AllowGet);
            }

            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GlAccountSelector(string Category)
        {
            ViewBag.BudgetCategory = Category;
            //var segmentSequence = _db.GlSegmentSequences.FirstOrDefault();
            //if (segmentSequence != null)
            //{
            //    ViewBag.Segment1 = segmentSequence.FirstSegment;
            //    ViewBag.Segment2 = segmentSequence.SecondSegment;
            //    ViewBag.Segment3 = segmentSequence.ThirdSegment;
            //    ViewBag.Segment4 = segmentSequence.FourthSegment;
            //}

            return View();
        }


        #endregion



    }

}




