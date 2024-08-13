using ExceedERP.Core.Domain;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using ExceedERP.Core.Domain.Global;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using ExceedERP.Core.Domain.Insurance;

namespace ExceedERP.Web.Areas.HRM.Controllers.SelfCare
{
    [AuthorizeRoles(HrmConstants.HRDepartmentHead)]
    public class EmployeeAccidentRequestController : Controller
    {
        // GET: HRM/EmployeeAccidentRequest
        private ExceedDbContext db = new ExceedDbContext();
        public static DateTime Date { get; set; }

        public ActionResult Index()
        {
            PopulateViewDate();
            return View();
        }
        public void PopulateViewDate()
        {
            var name = db.Person_Employee.ToList();
            ViewData["Name"] = name;
            var branch = db.Branch.ToList();
            ViewData["Branch"] = branch;
            var workunit = db.MainCompanyStructures.ToList();
            ViewData["Workunit"] = workunit;
            var jobtitle = db.Lookups.Where(x => x.LookupType == Core.Domain.Common.LookUpTypes.JOB_TITLE).ToList();
            ViewData["Jobtitle"] = jobtitle;

        }
        public JsonResult SendForApproval(int id)
        {
            var accident = db.EmployeeAccidents.FirstOrDefault(x => x.EmployeeAccidentId == id);
            if (accident != null)
            {
                accident.IsSendForApproval = true;


                db.Entry(accident).State = EntityState.Modified;
                db.SaveChanges();
            }
         return Json( RedirectToActionPermanent("index"),JsonRequestBehavior.AllowGet);

        }
        public decimal GetSalary(int id)
        {
            decimal salary = 0;
            var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == id && x.isCurrent);
            if (placement != null)
            {
                salary = placement.Salary;
            }
            return salary;
        }
        public string GetEmployementType(int id)
        {
            string type = "";
            var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == id && x.IsActive);
            if (person != null)
            {
                type = person.Status.ToString();

            }
            return type;
        }
        public ActionResult EmployeeAccidentRequest_Read([DataSourceRequest]DataSourceRequest request)
        {
            var accidents = db.EmployeeAccidents.ToList();
            DataSourceResult result = accidents.ToDataSourceResult(request, accident => new
            {
                EmployeeAccidentId = accident.EmployeeAccidentId,
                EmployeeId = accident.EmployeeId,
                AccidentDate = accident.AccidentDate,
                AccidentTime = accident.AccidentTime,
                IsAtWorkPlace = accident.IsAtWorkPlace,
                IsOnDuety = accident.IsOnDuety,
                LevelOfInjuired = accident.LevelOfInjuired,
                Witness = accident.Witness,
                BranchId = accident.BranchId,
                OrgStructureId = accident.OrgStructureId,
                LookupId = accident.LookupId,
                AccidentDateAmh = accident.AccidentDateAmh,
                IsSendForApproval = accident.IsSendForApproval,
                Description = accident.Description,
                Salary = GetSalary(accident.EmployeeId),
                EmployeementType = GetEmployementType(accident.EmployeeId)
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
   
        public ActionResult EmployeeAccidentRequest_Create([DataSourceRequest]DataSourceRequest request, EmployeeAccident accident)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if(accident.AccidentDateAmh != null)
                {
                    var splitDateTo = accident.AccidentDateAmh.Split('/');
                    Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateTo[0]), Convert.ToInt32(splitDateTo[1]), Convert.ToInt32(splitDateTo[2]));

                }

                var entity = new EmployeeAccident
                {
                    EmployeeAccidentId = accident.EmployeeAccidentId,
                    EmployeeId = accident.EmployeeId,
                    AccidentDate = Date,
                    AccidentDateAmh = accident.AccidentDateAmh,
                    AccidentTime = accident.AccidentTime,
                    IsAtWorkPlace = accident.IsAtWorkPlace,
                    IsOnDuety = accident.IsOnDuety,
                    LevelOfInjuired = accident.LevelOfInjuired,
                    Witness = accident.Witness,
                    BranchId = accident.BranchId,
                    OrgStructureId = accident.OrgStructureId,
                    LookupId = accident.LookupId,
                    Description = accident.Description
                };

                db.EmployeeAccidents.Add(entity);
                db.SaveChanges();
                accident.EmployeeAccidentId = entity.EmployeeAccidentId;

                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "EmployeeAccident ", "Success", User.Identity.Name, User.Identity.Name + " add new Employee Accident at date " + DateTime.Today.ToShortDateString(), Modules.HRM);

                #endregion

            }

            return Json(new[] { accident }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        
        public ActionResult EmployeeAccidentRequest_Update([DataSourceRequest]DataSourceRequest request, EmployeeAccident accident)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new EmployeeAccident
                {
                    EmployeeAccidentId = accident.EmployeeAccidentId,
                    EmployeeId = accident.EmployeeId,
                    AccidentDate = accident.AccidentDate,
                    AccidentTime = accident.AccidentTime,
                    IsAtWorkPlace = accident.IsAtWorkPlace,
                    IsOnDuety = accident.IsOnDuety,
                    LevelOfInjuired = accident.LevelOfInjuired,
                    Witness = accident.Witness,
                    BranchId = accident.BranchId,
                    OrgStructureId = accident.OrgStructureId,
                    LookupId = accident.LookupId,
                    AccidentDateAmh = accident.AccidentDateAmh,
                    IsSendForApproval = accident.IsSendForApproval,
                    CreatedBy = accident.CreatedBy,
                    LastModifiedBy = accident.LastModifiedBy,
                    Description = accident.Description
                };

                db.EmployeeAccidents.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "EmployeeAccident ", "Success", User.Identity.Name, User.Identity.Name + " update Employee Accident at date " + DateTime.Today.ToShortDateString(), Modules.HRM);

                #endregion

            }

            return Json(new[] { accident }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
      
        public ActionResult EmployeeAccidentRequest_Destroy([DataSourceRequest]DataSourceRequest request, EmployeeAccident accident)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new EmployeeAccident
                {
                    EmployeeAccidentId = accident.EmployeeAccidentId,
                    EmployeeId = accident.EmployeeId,
                    AccidentDate = accident.AccidentDate,
                    AccidentTime = accident.AccidentTime,
                    IsAtWorkPlace = accident.IsAtWorkPlace,
                    IsOnDuety = accident.IsOnDuety,
                    LevelOfInjuired = accident.LevelOfInjuired,
                    Witness = accident.Witness,
                    BranchId = accident.BranchId,
                    OrgStructureId = accident.OrgStructureId,
                    LookupId = accident.LookupId,
                    AccidentDateAmh = accident.AccidentDateAmh,
                    IsSendForApproval = accident.IsSendForApproval,
                    Description = accident.Description
                };

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Employee Accident Request information of work unit " + accident.OrgStructureId);
                #endregion

                db.EmployeeAccidents.Attach(entity);
                db.EmployeeAccidents.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { accident }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
