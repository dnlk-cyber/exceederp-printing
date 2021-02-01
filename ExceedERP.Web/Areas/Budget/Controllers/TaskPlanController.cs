﻿using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
﻿using System.Data.Entity;
﻿using System.IO;
﻿using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core;
﻿using ExceedERP.DataAccess;
﻿using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;


using ExceedERP.Helpers.Common;  
using ExceedERP.Web.Areas.Budget.Models;

using ExceedERP.Core.Domain.Budget;

using System.Text;
using ExceedERP.Helpers.Budget;
using System.Diagnostics;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Budget.Task;

namespace ExceedERP.Web.Areas.Budget.Controllers
{
    [AuthorizeRoles(BudgetRoles.BudgetUser, BudgetRoles.BudgetAdmin)]
    public class TaskPlanController : Controller
    {



        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["MainCompanyStructures"] = db.GLChartOfAccountCostCenters.ToList();
            ViewData["GLPeriods"] = db.GLPeriods.ToList();
            ViewData["FiscalYear"] = db.GLFiscalYears.ToList();
            ViewData["TaskUnits"] = db.TaskUnits.ToList();
            ViewData["Category"] = db.TaskCategorys.ToList();

            return View();
        }

        public ActionResult TaskPlan_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<TaskPlan> TaskPlan = db.TaskPlans;
            DataSourceResult result = TaskPlan.ToDataSourceResult(request, t => new TaskPlan
            {
                GlFiscalYearId = t.GlFiscalYearId,
                TaskPlanId = t.TaskPlanId,
                Name = t.Name,
                Unit = t.Unit,
                Amount = t.Amount,
                GLChartOfAccountCostCenterId = t.GLChartOfAccountCostCenterId,
                BudgetCategoryID = t.BudgetCategoryID,
                CreatedBy = t.CreatedBy,
                DateCreated = t.DateCreated,
                LastModified = t.LastModified,
                ModifiedBy = t.ModifiedBy,
                IsOnlineApproved = t.IsOnlineApproved,
                IsSendForApproval = t.IsSendForApproval
            });

            return Json(result);
        }
        public ActionResult TaskPlanMonth_Read([DataSourceRequest]DataSourceRequest request, int Id)
        {
            IQueryable<TaskPlanMonthly> TaskPlan = db.TaskPlanMonthlys.Where(x=>x.BudgetDefineID == Id);
            DataSourceResult result = TaskPlan.ToDataSourceResult(request, t => new  
            {

                GLPeriodId = t.GLPeriodId,
                MonthlyBudget = t.MonthlyBudget,
                CreatedBy = t.CreatedBy,
                DateCreated = t.DateCreated,
                LastModified = t.LastModified,
                ModifiedBy = t.ModifiedBy,

            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TaskPlan_Create([DataSourceRequest]DataSourceRequest request, TaskPlan TaskPlan)
        {
            List<TaskPlan> TaskPlans = db.TaskPlans.Where(p => p.GLChartOfAccountCostCenterId == TaskPlan.GLChartOfAccountCostCenterId && p.BudgetCategoryID == TaskPlan.BudgetCategoryID && p.GlFiscalYearId == TaskPlan.GlFiscalYearId &&  p.Unit == TaskPlan.Unit).ToList();
            if (TaskPlans.Any())
            {
                ModelState.AddModelError("DuplicateEntrie", "  Plan Already Exists !");
                return Json(new[] { TaskPlan }.ToDataSourceResult(request, ModelState));
     
            }
            else
            {

                if (ModelState.IsValid)
                {
                    var entity = new TaskPlan
                    {
                        BudgetCategoryID = TaskPlan.BudgetCategoryID,
                        GLChartOfAccountCostCenterId = TaskPlan.GLChartOfAccountCostCenterId,
                        GlFiscalYearId = TaskPlan.GlFiscalYearId,
                               Name = TaskPlan.Name,
                               Amount = TaskPlan.Amount,
                        Unit = TaskPlan.Unit,
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name

                    };

                    db.TaskPlans.Add(entity);
                    db.SaveChanges();
                    List<BindingItem> months = GetMonths(TaskPlan.GlFiscalYearId);
                    if (months.Any())
                    {
                        int intervals = months.Count;
                        decimal monthBuget = TaskPlan.Amount / intervals;

                        foreach (var month in months)
                        {

                            var budgetpermonth = new TaskPlanMonthly
                            {

                                GLPeriodId = month.Value,
                                MonthlyBudget = monthBuget,
                                BudgetDefineID = entity.TaskPlanId,
                                DateCreated = DateTime.Now,
                                CreatedBy = User.Identity.Name

                            };

                            db.TaskPlanMonthlys.Add(budgetpermonth);
                            db.SaveChanges();

                        }
                    }


                    TaskPlan.TaskPlanId = entity.TaskPlanId;
                }
                else
                {
                    return Json(new[] { TaskPlan }.ToDataSourceResult(request, ModelState));
                }
            }


            return Json(new[] { TaskPlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TaskPlan_Update([DataSourceRequest]DataSourceRequest request, TaskPlan TaskPlan)
        {
            List<TaskPlan> TaskPlans = db.TaskPlans.Where(p => p.GLChartOfAccountCostCenterId == TaskPlan.GLChartOfAccountCostCenterId && p.TaskPlanId != TaskPlan.TaskPlanId && p.BudgetCategoryID == TaskPlan.BudgetCategoryID).ToList();
            if (TaskPlans.Any())
            {
                ModelState.AddModelError("DuplicateEntrie", " Budget Plan Already Exists !");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var entity = new TaskPlan
                    {

                        Unit = TaskPlan.Unit,
                        LastModified = DateTime.Now,
                        ModifiedBy = User.Identity.Name
                    };

                    db.TaskPlans.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    var TaskPlanMonths = db.TaskPlanMonthlys.Where(x => x.BudgetDefineID == TaskPlan.TaskPlanId).ToList();
                    foreach (var TaskPlanMonth in TaskPlanMonths)
                    {
                        db.TaskPlanMonthlys.Attach(TaskPlanMonth);
                        db.TaskPlanMonthlys.Remove(TaskPlanMonth);
                        db.SaveChanges();
                    }
                    List<BindingItem> months = GetMonths(TaskPlan.BudgetCategoryID);
                    if (months.Any())
                    {
                        int intervals = months.Count;
                        decimal monthBuget = TaskPlan.Amount / intervals;

                        foreach (var month in months)
                        {

                            var budgetpermonth = new TaskPlanMonthly
                            {

                                GLPeriodId = month.Value,
                                MonthlyBudget = monthBuget,
                                BudgetDefineID = entity.TaskPlanId,
                                DateCreated = DateTime.Now,
                                CreatedBy = User.Identity.Name

                            };

                            db.TaskPlanMonthlys.Add(budgetpermonth);
                            db.SaveChanges();

                        }
                    }

                }
            }

            return Json(new[] { TaskPlan }.ToDataSourceResult(request, ModelState));
        }

         

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TaskPlan_Destroy([DataSourceRequest]DataSourceRequest request, TaskPlan TaskPlan)
        {

            var Plan = db.TaskPlans.Where(x => x.TaskPlanId == TaskPlan.TaskPlanId).FirstOrDefault();

            db.TaskPlans.Attach(Plan);
            db.TaskPlans.Remove(Plan);
            db.SaveChanges();
            UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Task Plan number " + TaskPlan.TaskPlanId);


            return Json(new[] { TaskPlan }.ToDataSourceResult(request));
        }

     

      

        [AllowAnonymous]
        public string GetStatus(int Id)
        {

            var Usage = db.TaskPlans.Where(j => j.TaskPlanId == Id).FirstOrDefault();
            if (Usage.IsSendForApproval == true)
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

                // throw;
            }

            return null;
        }

        public ActionResult TaskPlan_SendForOnlineApproval(int Id)
        {

            var TaskPlans = db.TaskPlans.Where(x => x.TaskPlanId == Id).FirstOrDefault();
            if (TaskPlans != null)
            {
                TaskPlans.IsSendForApproval = true;
                db.TaskPlans.Attach(TaskPlans);
                db.Entry(TaskPlans).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [AuthorizeRoles(BudgetRoles.BudgetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TaskPlans_Approve([DataSourceRequest]DataSourceRequest request, TaskPlanApproval TaskPlanApproval, int Id)
        {
            try
            {
                if (TaskPlanApproval.Status == ApprovalStatuses.Approve)
                {
                    var entity = db.TaskPlans.Where(x => x.TaskPlanId == Id).FirstOrDefault();

                    entity.IsOnlineApproved = true;
                    entity.OnlineApprovedBy = User.Identity.Name;
                    entity.OnlineApprovedTime = DateTime.Now;

                    db.TaskPlans.Attach(entity);
                    db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                if (TaskPlanApproval.Status == ApprovalStatuses.Reject)
                {
                    var entity = db.TaskPlans.Where(x => x.TaskPlanId == Id).FirstOrDefault();

                    entity.IsOnlineApproved = false;
                    entity.IsSendForApproval = false;
                    entity.IsOnlineTransferred = true;
                    db.TaskPlans.Attach(entity);
                    db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                var approval = new TaskPlanApproval()
                {

                    Status = TaskPlanApproval.Status,
                    BudgetPlanId = Id,
                    Remark = TaskPlanApproval.Remark,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name

                };

                db.TaskPlanApprovals.Add(approval);
                db.SaveChanges();
                TaskPlanApproval.TaskPlanApprovalId = approval.TaskPlanApprovalId;

            }
            catch (Exception e)
            {

                
            }
            return Json(new[] { TaskPlanApproval }.ToDataSourceResult(request));
        }

        public ActionResult TaskPlanApproval_Read([DataSourceRequest]DataSourceRequest request, int Id)
        {


            List<TaskPlanApproval> usageApproval = db.TaskPlanApprovals.Where(x => x.BudgetPlanId == Id).ToList();
            var Category = db.TaskPlans.Where(x => x.TaskPlanId == Id).FirstOrDefault();

            if (Category.IsSendForApproval == true && Category.IsOnlineApproved == false)
            {
                usageApproval.Add(new TaskPlanApproval()
                {
                    BudgetPlanId = Id,
                    Remark = string.Empty,
                    Status = ApprovalStatuses.Pending,

                });
            }



            return Json(usageApproval.ToDataSourceResult(request, approval => new TaskPlanApproval()
            {
                TaskPlanApprovalId = approval.TaskPlanApprovalId,
                BudgetPlanId = approval.BudgetPlanId,
                Remark = approval.Remark,
                Status = approval.Status,
                DateCreated = DateTime.Now,
                CreatedBy = User.Identity.Name


            }));
        }
        [AllowAnonymous]
        public List<BindingItem> GetMonths(int Id)
        {
            List<BindingItem> Months = new List<BindingItem>();

            Months = db.GLPeriods.Where(x => x.GlFiscalYear.GlFiscalYearId == Id).Select(s => new BindingItem
            {
                Text = s.Name,
                Value = s.GLPeriodId
            }).ToList();


            return Months;

        }
        
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }




    }

}





