﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
 using ExceedERP.Core.Domain;
 using ExceedERP.Core.Domain.Common;
﻿using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.DataAccess.Context;
﻿using ExceedERP.Web.Areas.HRM.Controllers.Profile;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
 using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.HRM.Payroll;

namespace ExceedERP.Web.Areas.HRM.Controllers.SelfCare
{
    [AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRDepartmentHead)]
    public class OverTimeRequestEmployeeController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            HRMInputs inputs = new HRMInputs();
            var list = inputs.GetPayrollPlacmentEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);
            var employeName = db.Person_Employee.ToList();
            ViewData["EmployeeName"] = employeName;
        }

        public ActionResult OverTimeRequestEmployee_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<OverTimeRequestEmployee> overtimerequestemployee = db.OverTimeRequestEmployee.Where(x => x.OverTimeRequestId == id).AsQueryable();
            DataSourceResult result = overtimerequestemployee.ToDataSourceResult(request, overTimeRequestEmployee => new
            {
                OverTimeRequestEmployeeId = overTimeRequestEmployee.OverTimeRequestEmployeeId,
                EmployeeId = overTimeRequestEmployee.EmployeeId,
                Remark = overTimeRequestEmployee.Remark,
                OverTimeRequestId = overTimeRequestEmployee.OverTimeRequestId
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OverTimeRequestEmployee_Create([DataSourceRequest]DataSourceRequest request, OverTimeRequestEmployee overTimeRequestEmployee, int id)
        {
            if (ModelState.IsValid)
            {
                var checkExistance =
                    db.OverTimeRequestEmployee.Where(
                        x => x.EmployeeId == overTimeRequestEmployee.EmployeeId && x.OverTimeRequestId == id)
                        .FirstOrDefault();
                if (checkExistance != null)
                {
                    ModelState.AddModelError("", "This employee is already registered!!");
                }
                else
                {

                    var entity = new OverTimeRequestEmployee
                    {
                        EmployeeId = overTimeRequestEmployee.EmployeeId,
                        Remark = overTimeRequestEmployee.Remark,
                        OverTimeRequestId = id
                    };

                    db.OverTimeRequestEmployee.Add(entity);
                    db.SaveChanges();
                    overTimeRequestEmployee.OverTimeRequestEmployeeId = entity.OverTimeRequestEmployeeId;

                    //var over = db.OverTimeRequest.FirstOrDefault(x=>x.OverTimeRequestId == overTimeRequestEmployee.OverTimeRequestId);
                    //if (over != null)
                    //{
                    //    var overTime = new OverTimeTransfer
                    //    {
                    //        EmployeeId = overTimeRequestEmployee.EmployeeId,
                    //        GLPeriodId = 
                    //    };
                    //}
                }
            }

            return Json(new[] { overTimeRequestEmployee }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OverTimeRequestEmployee_Update([DataSourceRequest]DataSourceRequest request, OverTimeRequestEmployee overTimeRequestEmployee)
        {
            if (ModelState.IsValid)
            {
                var entity = new OverTimeRequestEmployee
                {
                    OverTimeRequestEmployeeId = overTimeRequestEmployee.OverTimeRequestEmployeeId,
                    EmployeeId = overTimeRequestEmployee.EmployeeId,
                    Remark = overTimeRequestEmployee.Remark,
                    OverTimeRequestId = overTimeRequestEmployee.OverTimeRequestId
                };

                db.OverTimeRequestEmployee.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { overTimeRequestEmployee }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OverTimeRequestEmployee_Destroy([DataSourceRequest]DataSourceRequest request, OverTimeRequestEmployee overTimeRequestEmployee)
        {
            if (ModelState.IsValid)
            {
                var entity = new OverTimeRequestEmployee
                {
                    OverTimeRequestEmployeeId = overTimeRequestEmployee.OverTimeRequestEmployeeId,
                    EmployeeId = overTimeRequestEmployee.EmployeeId,
                    Remark = overTimeRequestEmployee.Remark,
                    OverTimeRequestId = overTimeRequestEmployee.OverTimeRequestId
                };
                #region delete log
                var personemp = db.Person_Employee.FirstOrDefault(x=>x.EmployeeId== overTimeRequestEmployee.EmployeeId);
                if (personemp!=null)
                {
                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted over time request of employee code " + personemp.EmployeeCode);

                }
                #endregion
                db.OverTimeRequestEmployee.Attach(entity);
                db.OverTimeRequestEmployee.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { overTimeRequestEmployee }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
