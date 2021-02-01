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
    public class TaskUnitController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        //
        // GET: /Budget/Currency/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TaskUnit_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<TaskUnit> TaskUnit = db.TaskUnits;
            DataSourceResult result = TaskUnit.ToDataSourceResult(request, Currency => new TaskUnit
            {
                TaskUnitId = Currency.TaskUnitId,
                Name = Currency.Name,

                Description = Currency.Description,
                CreatedBy = Currency.CreatedBy,
                DateCreated = Currency.DateCreated,
                LastModified = Currency.LastModified,
                ModifiedBy = Currency.ModifiedBy,


            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TaskUnit_Create([DataSourceRequest]DataSourceRequest request, TaskUnit TaskUnit)
        {
            List<TaskUnit> category = db.TaskUnits.Where(x => x.Name == TaskUnit.Name).ToList();


            if (category.Any())
            {
                ModelState.AddModelError("SameName", " Category Name Already Exists !");

            }

            else
            {
                if (ModelState.IsValid)
                {
                    var entity = new TaskUnit
                    {
                        TaskUnitId = TaskUnit.TaskUnitId,
                        Name = TaskUnit.Name,
                        Description = TaskUnit.Description,
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name

                    };

                    db.TaskUnits.Add(entity);
                    db.SaveChanges();



                    TaskUnit.TaskUnitId = entity.TaskUnitId;
                }
            }

            return Json(new[] { TaskUnit }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TaskUnit_Update([DataSourceRequest]DataSourceRequest request, TaskUnit Currency)
        {
            List<TaskUnit> category = db.TaskUnits.Where(x => x.Name == Currency.Name && x.TaskUnitId != Currency.TaskUnitId).ToList();


            if (category.Any())
            {
                ModelState.AddModelError("SameName", " Category Name Already Exists !");

            }
            else
            {

                if (ModelState.IsValid)
                {
                    var entity = new TaskUnit
                    {
                        TaskUnitId = Currency.TaskUnitId,
                        Name = Currency.Name,
                        Description = Currency.Description,
                        LastModified = DateTime.Now,
                        ModifiedBy = User.Identity.Name
                    };

                    db.TaskUnits.Attach(entity);
                    db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                }
            }
            return Json(new[] { Currency }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TaskUnit_Destroy([DataSourceRequest]DataSourceRequest request, TaskUnit TaskUnit)
        {
            if (ModelState.IsValid)
            {
                var entity = new TaskUnit
                {
                    TaskUnitId = TaskUnit.TaskUnitId,


                };

                db.TaskUnits.Attach(entity);
                db.TaskUnits.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Budget Category number " + TaskUnit.TaskUnitId);
            }

            return Json(new[] { TaskUnit }.ToDataSourceResult(request, ModelState));
        }

      
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }


    }
}