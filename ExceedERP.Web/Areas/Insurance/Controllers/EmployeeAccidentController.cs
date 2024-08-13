using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Insurance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Insurance.Controllers
{
    public class EmployeeAccidentController : Controller
    {
        // GET: HRM/EmployeeAccident
        private ExceedDbContext db = new ExceedDbContext();
    
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
            var jobtitle = db.Lookups.Where(x=>x.LookupType == Core.Domain.Common.LookUpTypes.JOB_TITLE).ToList();
            ViewData["Jobtitle"] = jobtitle;

        }       
        public void SendForApproval(int id)
        {
            var accident = db.EmployeeAccidents.FirstOrDefault(x=>x.EmployeeAccidentId==id);
            if (accident !=null)
            {
                accident.IsSendForApproval = true;

               
                db.Entry(accident).State = EntityState.Modified;
                db.SaveChanges();
            }

        }
        public decimal GetSalary(int id)
        {
            decimal salary = 0;
            var placement = db.Placements.FirstOrDefault(x=>x.EmployeeId == id && x.isCurrent);
            if(placement != null)
            {
                salary = placement.Salary;
            }
            return salary;
        }
        public string GetEmployementType(int id)
        {
            string type = "";
            var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == id && x.IsActive );
            if (person != null)
            {
                type = person.Status.ToString();
              
            }
            return type;
        }
        [AuthorizeRoles(HrmConstants.HRInsuranceAdmin, HrmConstants.HRInsuranceUser)]
        public ActionResult EmployeeAccident_Read([DataSourceRequest]DataSourceRequest request)
        {
            var  accidents = db.EmployeeAccidents.Where(x=>x.IsSendForApproval).ToList();
            DataSourceResult result = accidents.ToDataSourceResult(request, accident => new
            {
                EmployeeAccidentId = accident.EmployeeAccidentId,
                EmployeeId = accident.EmployeeId,
                DailyLabour = accident.DailyLabour,
                IsOutOFSystem = accident.IsOutOFSystem,
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
        [AuthorizeRoles(HrmConstants.HRInsuranceUser)]
        public ActionResult EmployeeAccident_Create([DataSourceRequest]DataSourceRequest request, EmployeeAccident accident)
        {
            ModelState.Clear();
            if (!accident.IsOutOFSystem)
            {
                accident.EmployeeId = 0;


            }
            if (ModelState.IsValid)
            {
                
                    var entity = new EmployeeAccident
                    {
                        EmployeeAccidentId = accident.EmployeeAccidentId,
                        EmployeeId = accident.EmployeeId,
                        DailyLabour = accident.DailyLabour,
                        IsOutOFSystem = accident.IsOutOFSystem,
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
                        Description = accident.Description
                    };

                    db.EmployeeAccidents.Add(entity);
                    db.SaveChanges();
                    accident.EmployeeAccidentId = entity.EmployeeAccidentId;
                
            }

            return Json(new[] { accident }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [AuthorizeRoles(HrmConstants.HRInsuranceUser)]
        public ActionResult EmployeeAccident_Update([DataSourceRequest]DataSourceRequest request,EmployeeAccident accident)
        {
            ModelState.Clear();
            if (!accident.IsOutOFSystem)
            {
                accident.EmployeeId = 0;


            }
            if (ModelState.IsValid)
            {
                var entity = new EmployeeAccident
                {
                    EmployeeAccidentId = accident.EmployeeAccidentId,
                    EmployeeId = accident.EmployeeId,
                    DailyLabour = accident.DailyLabour,
                    IsOutOFSystem = accident.IsOutOFSystem,
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
            }

            return Json(new[] { accident }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [AuthorizeRoles(HrmConstants.HRInsuranceAdmin)]
        public ActionResult EmployeeAccident_Destroy([DataSourceRequest]DataSourceRequest request, EmployeeAccident accident)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new EmployeeAccident
                {
                    EmployeeAccidentId = accident.EmployeeAccidentId,
                    EmployeeId = accident.EmployeeId,
                    DailyLabour = accident.DailyLabour,
                    IsOutOFSystem = accident.IsOutOFSystem,
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
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Employee Accident Recoreds " + accident.EmployeeId);
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
