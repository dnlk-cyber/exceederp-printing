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
    public class TaskPerformanceController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        //
        // GET: /Budget/TaskPerformance/
        public ActionResult Index()
        {
            ViewData["MainCompanyStructures"] = db.GLChartOfAccountCostCenters.ToList();
            ViewData["GLPeriods"] = db.GLPeriods.ToList();
            ViewData["FiscalYear"] = db.GLFiscalYears.ToList();
            ViewData["TaskUnits"] = db.TaskUnits.ToList();
            ViewData["TaskPlan"] = db.TaskPlans.ToList();
            ViewData["Category"] = db.TaskCategorys.ToList();

            return View();
        }


        public ActionResult TaskPerformance_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<TaskPerformance> TaskPerformances = db.TaskPerformances;
            DataSourceResult result = TaskPerformances.ToDataSourceResult(request, TaskPerformance => new
            {
                GlFiscalYearId = TaskPerformance.GlFiscalYearId,
                TaskPerformanceId = TaskPerformance.TaskPerformanceId,
                Unit = TaskPerformance.Unit,
                TaskPlanId = TaskPerformance.TaskPlanId,
                Amount = TaskPerformance.Amount,
                GLPeriodId = TaskPerformance.GLPeriodId,
                GLChartOfAccountCostCenterId = TaskPerformance.GLChartOfAccountCostCenterId,
                BudgetCategoryID = TaskPerformance.BudgetCategoryID,
                CreatedBy = TaskPerformance.CreatedBy,
                DateCreated = TaskPerformance.DateCreated,
                LastModified = TaskPerformance.LastModified,
                ModifiedBy = TaskPerformance.ModifiedBy,
                IsOnlineApproved = TaskPerformance.IsOnlineApproved,
                IsSendForApproval = TaskPerformance.IsSendForApproval

            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TaskPerformance_Create([DataSourceRequest]DataSourceRequest request, TaskPerformance TaskPerformance)
        {
            var TaskPlan = db.TaskPlans.Where(x => x.TaskPlanId == TaskPerformance.TaskPlanId).FirstOrDefault();
            TaskPerformance.Unit = TaskPlan.Unit;

            List<TaskPerformance> TaskPerformances = db.TaskPerformances.Where(p => p.TaskPlanId == TaskPerformance.TaskPlanId && p.GLPeriodId == TaskPerformance.GLPeriodId).ToList();
            if (TaskPerformances.Any())
            {
                ModelState.AddModelError("DuplicateEntrie", " Plan Performace Already Exists !");
            }
            else
            {


                var entity = new TaskPerformance
                {
                    GlFiscalYearId = TaskPerformance.GlFiscalYearId,
                    GLPeriodId = TaskPerformance.GLPeriodId,
                    TaskPlanId = TaskPerformance.TaskPlanId,

                    TaskPerformanceId = TaskPerformance.TaskPerformanceId,
                    Unit = TaskPerformance.Unit,
                    Amount = TaskPerformance.Amount,
                    GLChartOfAccountCostCenterId = TaskPerformance.GLChartOfAccountCostCenterId,
                    BudgetCategoryID = TaskPerformance.BudgetCategoryID,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name

                };


                db.TaskPerformances.Add(TaskPerformance);
                db.SaveChanges();


                TaskPerformance.TaskPerformanceId = entity.TaskPerformanceId;


            }


            return Json(new[] { TaskPerformance }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TaskPerformance_Update([DataSourceRequest]DataSourceRequest request, TaskPerformance TaskPerformance)
        {
            var TaskPlan = db.TaskPlans.Where(x => x.TaskPlanId == TaskPerformance.TaskPlanId).FirstOrDefault();
            TaskPerformance.Unit = TaskPlan.Unit;

            List<TaskPerformance> TaskPerformances = db.TaskPerformances.Where(p => p.GLChartOfAccountCostCenterId == TaskPerformance.GLChartOfAccountCostCenterId && p.TaskPerformanceId != TaskPerformance.TaskPerformanceId && p.BudgetCategoryID == TaskPerformance.BudgetCategoryID && p.GLPeriodId == TaskPerformance.GLPeriodId).ToList();
            if (TaskPerformances.Any())
            {
                ModelState.AddModelError("DuplicateEntrie", " Performance Already Exists !");
            }
            else
            {
                var Performance = db.TaskPerformances.Where(x => x.TaskPerformanceId == TaskPerformance.TaskPerformanceId).FirstOrDefault();

                Performance.GlFiscalYearId = TaskPerformance.GlFiscalYearId;
                Performance.TaskPerformanceId = TaskPerformance.TaskPerformanceId;
                Performance.Unit = Performance.Unit;
                Performance.Amount = TaskPerformance.Amount;
                Performance.GLChartOfAccountCostCenterId = TaskPerformance.GLChartOfAccountCostCenterId;
                Performance.BudgetCategoryID = TaskPerformance.BudgetCategoryID;
                Performance.GLPeriodId = TaskPerformance.GLPeriodId;
                Performance.CreatedBy = TaskPerformance.CreatedBy;
                Performance.DateCreated = TaskPerformance.DateCreated;
                Performance.LastModified = DateTime.Now;
                Performance.ModifiedBy = User.Identity.Name;


                db.TaskPerformances.Attach(Performance);
                db.Entry(Performance).State = EntityState.Modified;
                db.SaveChanges();


            }


            return Json(new[] { TaskPerformance }.ToDataSourceResult(request, ModelState));
        }



        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TaskPerformance_Destroy([DataSourceRequest]DataSourceRequest request, TaskPerformance TaskPerformance)
        {
            var Plan = db.TaskPerformances.Where(x => x.TaskPerformanceId == TaskPerformance.TaskPerformanceId).FirstOrDefault();

            db.TaskPerformances.Attach(Plan);
            db.TaskPerformances.Remove(Plan);
            db.SaveChanges();
            UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted budget Plan  number " + TaskPerformance.TaskPerformanceId);


            return Json(new[] { TaskPerformance }.ToDataSourceResult(request, ModelState));
        }



        [AllowAnonymous]
        public string GetStatus(int Id)
        {

            TaskPerformance Usage = db.TaskPerformances.Where(j => j.TaskPerformanceId == Id).FirstOrDefault();
            if (Usage.IsSendForApproval == true)
            {
                return BudgetConstants.Approved;
            }

            return BudgetConstants.Pending;


        }
        public ActionResult TaskPerformance_SendForOnlineApproval(int Id)
        {

            var TaskPlans = db.TaskPerformances.Where(x => x.TaskPerformanceId == Id).FirstOrDefault();
            if (TaskPlans != null)
            {
                TaskPlans.IsSendForApproval = true;
                db.TaskPerformances.Attach(TaskPlans);
                db.Entry(TaskPlans).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("Index", "TaskPerformance", null);
        }

        [AuthorizeRoles(BudgetRoles.BudgetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TaskPerformance_Approve([DataSourceRequest]DataSourceRequest request, TaskPerformaceApproval TaskPerformaceApproval, int Id)
        {
            try
            {
                if (TaskPerformaceApproval.Status == ApprovalStatuses.Approve)
                {
                    var entity = db.TaskPerformances.Where(x => x.TaskPerformanceId == Id).FirstOrDefault();

                    entity.IsOnlineApproved = true;
                    entity.OnlineApprovedBy = User.Identity.Name;
                    entity.OnlineApprovedTime = DateTime.Now;

                    db.TaskPerformances.Attach(entity);
                    db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                if (TaskPerformaceApproval.Status == ApprovalStatuses.Reject)
                {
                    var entity = db.TaskPerformances.Where(x => x.TaskPerformanceId == Id).FirstOrDefault();

                    entity.IsOnlineApproved = false;
                    entity.IsSendForApproval = false;
                    entity.IsOnlineTransferred = true;
                    db.TaskPerformances.Attach(entity);
                    db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                var approval = new TaskPerformaceApproval()
                {

                    Status = TaskPerformaceApproval.Status,
                    TaskPerformaceId = Id,
                    Remark = TaskPerformaceApproval.Remark,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name

                };

                db.TaskPerformaceApprovals.Add(approval);
                db.SaveChanges();
                TaskPerformaceApproval.TaskPerformaceApprovalId = approval.TaskPerformaceApprovalId;

            }
            catch (Exception e)
            {


            }
            return Json(new[] { TaskPerformaceApproval }.ToDataSourceResult(request));
        }

        public ActionResult TaskPerformaceApproval_Read([DataSourceRequest]DataSourceRequest request, int Id)
        {


            List<TaskPerformaceApproval> usageApproval = db.TaskPerformaceApprovals.Where(x => x.TaskPerformaceId == Id).ToList();
            var Category = db.TaskPerformances.Where(x => x.TaskPerformanceId == Id).FirstOrDefault();

            if (Category.IsSendForApproval == true && Category.IsOnlineApproved == false)
            {
                usageApproval.Add(new TaskPerformaceApproval()
                {
                    TaskPerformaceId = Id,
                    Remark = string.Empty,
                    Status = ApprovalStatuses.Pending,

                });
            }



            return Json(usageApproval.ToDataSourceResult(request, approval => new TaskPerformaceApproval()
            {
                TaskPerformaceApprovalId = approval.TaskPerformaceApprovalId,
                TaskPerformaceId = approval.TaskPerformaceId,
                Remark = approval.Remark,
                Status = approval.Status,
                DateCreated = DateTime.Now,
                CreatedBy = User.Identity.Name


            }));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }


    }

}




