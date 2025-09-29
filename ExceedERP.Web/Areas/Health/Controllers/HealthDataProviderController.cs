using System;
using System.Collections.Generic;
using ExceedERP.DataAccess.Context;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common.ViewModel;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Health;

namespace ExceedERP.Web.Areas.Health.Controllers
{
    public class HealthDataProviderController : Controller
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        public JsonResult GetPaymentCoverage()
        {
            return Json(db.PaymentCoverageSettings.Select(p => new
            {
                PaymentCoverage = p.Payment.ToString()+"%",
                Payment = p.Payment
            }),
                JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetBusinessUnitAccounts()
        {
            return Json(db.BusinessUnits.Select(b => new
            {
                b.BusinessUnitId,
                b.Accountcode,
            }),
            JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetApSetupItem()
        {
            return Json(db.ApSetupItems.Select(b => new
            {
                b.ApSetupItemId,
                b.Name,
            }),
            JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetGLTaxRate()
        {
            return Json(db.GLTaxRates.Select(b => new
            {
                b.GLTaxRateID,
                b.TaxName,
            }),
            JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetBenefitTypeEnumWithOutNA()
        {

            var statuses = Enum.GetValues(typeof(HealthBenefitType))
                           .Cast<HealthBenefitType>()
                           .Where(x => x != HealthBenefitType.NA)
                           .Select(v => v)
                           .ToList();

            return Json(statuses.Select(p => new
            {
                Value = Convert.ToInt32(p),
                Display = p.ToString()
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllBranch()
        {
            var branches = db.Branch.Where(x => x.IsActive).ToList();
            return Json(branches.Select(p => new
            {
                p.BranchId,p.Name,
            }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetEmployeesAskForOther()
        {
            var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

            // Get subordinates + self
            var employeesId = EmployeeHelper.GetSubordinates_FilteredByHead(employeeId);
            employeesId.Add(employeeId);
            // One DB query, only fetch active employees with these IDs
            var employees = db.Person_Employee
                              .Where(e => e.IsActive && employeesId.Contains(e.EmployeeId))
                              .Select(e => new
                              {
                                  e.EmployeeId,
                                  e.EmployeeCode,
                                  e.FirstName,
                                  e.MiddleName,
                                  e.LastName,
                                  e.FirstNameEnglish,
                                  e.MiddleNameEnglish,
                                  e.LastNameEnglish
                              })
                              .ToList();

            // Distinct by EmployeeId (no duplicates)
            var result = employees
                .GroupBy(e => e.EmployeeId)
                .Select(g => g.First())
                .Select(p => new
                {
                    PersonId = p.EmployeeId,
                    FullName = p.EmployeeCode + "-" + p.FirstName + " " + p.MiddleName + " " + p.LastName,
                    FullNameaEnglish = p.EmployeeCode + "-" + p.FirstNameEnglish + " " + p.MiddleNameEnglish + " " + p.LastNameEnglish
                })
                .ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }


    }
}