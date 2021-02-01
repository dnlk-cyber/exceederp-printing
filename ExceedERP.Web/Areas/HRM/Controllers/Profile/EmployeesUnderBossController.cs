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
    public class EmployeesUnderBossController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(HrmConstants.HRPlacementAdmin, HrmConstants.HRPlacementUser, HrmConstants.HRAdmin)]
        public ActionResult EmployeesUnderBoss_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            List<EmployeesUnderBossVM> employeesList = new List<EmployeesUnderBossVM>();
            var place = db.Placements.FirstOrDefault(x => x.PlacementId == id && x.IsHead && x.isCurrent);
            if (place != null)
            {
                var employees = db.Placements.Where(x => x.OrgStructureId == place.OrgStructureId && x.BranchId == place.BranchId && x.isCurrent).ToList();

                employees.Remove(place);

                foreach (var emp in employees)
                {
                    var employee = new EmployeesUnderBossVM();

                  
                    var person = db.Person_Employee.FirstOrDefault(x=>x.EmployeeId == emp.EmployeeId);
                    if(person != null)
                    {
                        employee.EmployeeCode = person.EmployeeCode;
                        employee.FullName = person.FullNameaEnglish;
                        employee.IsCurrent = emp.isCurrent;
                        employeesList.Add(employee);
                    }
                 
                }
            }

                DataSourceResult result = employeesList.ToDataSourceResult(request, p => new
                {
                    EmployeesUnderBossId = p.EmployeesUnderBossId,
                    EmployeeCode = p.EmployeeCode,
                    FullName = p.FullName,
                  IsCurrent = p.IsCurrent
                });
            
            return Json(result);


        }

     
    }
}