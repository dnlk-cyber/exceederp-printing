﻿using System;
using System.Collections.Generic;


using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
 using ExceedERP.Core.Domain;
 using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.PerformanceManagment;
using ExceedERP.DataAccess.Context;
﻿using ExceedERP.Web.Areas.HRM.Controllers.Profile;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Helpers.Common;
 using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Performance
{
    
    [AuthorizeRoles(HrmConstants.HRPerformanceAdmin, HrmConstants.HRPerformanceUser)]
    public class EmployeePerformanceActionController : Controller
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
            var list = inputs.GetEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);
        }
        public ActionResult EmployeePerformanceAction_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<EmployeePerformanceAction> employeeperformanceaction = db.EmployeePerformanceAction;
            DataSourceResult result = employeeperformanceaction.ToDataSourceResult(request, employeePerformanceAction => new
            {
                EmpActionId = employeePerformanceAction.EmpActionId,
                MeasurementDefinitionId = employeePerformanceAction.MeasurementDefinitionId,
                MeasurementId = employeePerformanceAction.MeasurementId,
                EmployeeId = employeePerformanceAction.EmployeeId,
                EmpPerfPercentageofWeight = employeePerformanceAction.EmpPerfPercentageofWeight,
                Result = employeePerformanceAction.Result,
                Remark = employeePerformanceAction.Remark
            });

            return Json(result);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
