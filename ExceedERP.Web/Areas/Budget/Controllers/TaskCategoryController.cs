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
    public class TaskCategoryController : Controller
    {


        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["MainCompanyStructures"] = db.GLChartOfAccountCostCenters.ToList();
            ViewData["GLPeriods"] = db.GLPeriods.ToList();
            ViewData["FiscalYear"] = db.GLFiscalYears.ToList();
            ViewData["TaskUnits"] = db.TaskUnits.ToList();
            ViewData["Category"] = db.TaskCategorys.ToList();
            ViewData["BudgetControl"] = db.BudgetControls.ToList();

            return View();
        }

        public ActionResult TaskCategory_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<TaskCategory> TaskCategory = db.TaskCategorys;
            DataSourceResult result = TaskCategory.Select(s => new  
            {
                TaskCategoryId = s.TaskCategoryId,
             Name = s.Name,
         Description = s.Description,
        }
                ).ToDataSourceResult(request);

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TaskCategory_Create([DataSourceRequest]DataSourceRequest request, TaskCategory TaskCategory)
        {
            List<TaskCategory> category = db.TaskCategorys.Where(x => x.Name == TaskCategory.Name).ToList();

            if (!ModelState.IsValid)
            {
                return Json(new[] { TaskCategory }.ToDataSourceResult(request, ModelState));

            }

            if (category.Any())
            {
                ModelState.AddModelError("SameName", " Category Name Already Exists !");
                return Json(new[] { TaskCategory }.ToDataSourceResult(request, ModelState));


            }
          
            else
            {
                if (ModelState.IsValid)
                {
                    var entity = new TaskCategory
                    {
                        TaskCategoryId = TaskCategory.TaskCategoryId,
                        Name = TaskCategory.Name,
                       
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name

                    };

                    db.TaskCategorys.Add(entity);
                    db.SaveChanges();



                    TaskCategory.TaskCategoryId = entity.TaskCategoryId;
                }
                else
                {
                    return Json(new[] { TaskCategory }.ToDataSourceResult(request, ModelState));

                }
            }

            return Json(new[] { TaskCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TaskCategory_Update([DataSourceRequest]DataSourceRequest request, TaskCategory TaskCategory)
        {

            List<TaskCategory> category = db.TaskCategorys.Where(x => x.Name == TaskCategory.Name && x.TaskCategoryId != TaskCategory.TaskCategoryId).ToList();

          
                if (ModelState.IsValid)
                {
                    var entity = new TaskCategory
                    {
                        TaskCategoryId = TaskCategory.TaskCategoryId,
                        Name = TaskCategory.Name,
                           LastModified = DateTime.Now,
                        ModifiedBy = User.Identity.Name
                    };

                    db.TaskCategorys.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                }
            
            return Json(new[] { TaskCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TaskCategory_Destroy([DataSourceRequest]DataSourceRequest request, TaskCategory TaskCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = new TaskCategory
                {
                    TaskCategoryId = TaskCategory.TaskCategoryId,


                };

                db.TaskCategorys.Attach(entity);
                db.TaskCategorys.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Budget Category number " + TaskCategory.TaskCategoryId);
            }

            return Json(new[] { TaskCategory }.ToDataSourceResult(request, ModelState));
        }

   
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }


    }
}
