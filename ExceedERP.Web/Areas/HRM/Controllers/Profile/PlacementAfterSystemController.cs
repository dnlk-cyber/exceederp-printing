using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.Allowance;
using ExceedERP.Core.Domain.HRM.Company;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Core.Domain.HRM.SalaryStructure;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.Common.Controllers;
using ExceedERP.Web.Areas.HRM.Controllers.Company;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using Kendo.Mvc;

namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{
    public class PlacementAfterSystemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PlacementAfterSystem_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var placementAfterSystem = db.PlacementAfterSystems.Where(x=>x.EmployeeId == id).ToList();

            DataSourceResult result = placementAfterSystem.ToDataSourceResult(request, p => new
            {
                PlacementAfterSystemId = p.PlacementAfterSystemId,
                PlacementId = p.PlacementId,
                BranchId = p.BranchId,
                OrgStructureId = p.OrgStructureId,
                LookupId = p.LookupId,
                EmployeeId = p.EmployeeId,
                EmployementType = p.EmployementType,
                AssignmentType = p.AssignmentType,
                From = p.From,
                FromAm = p.FromAm,
                To = p.To,
                ToAm = p.ToAm,
                IsCurrent = p.IsCurrent,
                IsHead = p.IsHead,
                ResponsibilityLevel = p.ResponsibilityLevel,
                RankId = p.RankId,
                StepId = p.StepId,
                Salary = p.Salary
            });

            return Json(result);


        }


    }
}