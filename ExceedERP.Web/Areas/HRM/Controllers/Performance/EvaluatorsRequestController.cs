﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM.PerformanceManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain;

namespace ExceedERP.Web.Areas.HRM.Controllers.Performance
{
    [AuthorizeRoles(HrmConstants.HRDepartmentHead)]
    public class EvaluatorsRequestController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static int EmployeeId { get; set; }
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        public void PopulateViewData()
        {
            var fisaclYear = db.GLFiscalYears.Select(x=>new {
                FiscalYearId= x.GlFiscalYearId,
                x.Name
            }).ToList();
           
            var quarters = db.Quarters.Select(x => new
            {
                PeriodId = x.QuarterId,
                Name = x.Name
            }).ToList();
            var departmentLists = db.MainCompanyStructures.Select(x => new
            {
                x.OrgStructureId,
                x.Name
            }).ToList();
            var employeesName = db.Person_Employee.ToList();
            ViewData["EmployeeName"] = employeesName;
            EmployeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            //  var list = UserInformationHelper.GetEmployeesByReportsTo(User.Identity.Name);
            var list = db.Person_Employee.Select(x=>new
            {
                PersonId =x.EmployeeId,
                FullNameEng =x.EmployeeCode+"-"+x.FirstNameEnglish + " " + x.MiddleNameEnglish+ " "+ x.LastNameEnglish
            });
            ViewData["EmployeeList"] = list/*.Where(x => x.IsActive)*/;
            ViewData["Fiscalyears"] = fisaclYear;
            ViewData["Periods"] = quarters;
            ViewData["PeriodList"] = fisaclYear;
            ViewData["departmentLists"] = departmentLists;
        }
        private string GetHead(int empId)
        {
            var boss = db.Placements.Where(x=>x.EmployeeId == empId&&x.IsHead).Where(x=>x.isCurrent).FirstOrDefault();

            if(boss != null)
            {
                var employee = db.Person_Employee.Find(boss.EmployeeId);
                if(employee != null)
                {
                    return employee.FullNameaEnglish;
                }
            }
            return "";
        }
        public string GetBoss(int empId)
        {
            var boss = "";
            var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == empId && x.isCurrent);
            if (placement != null && placement.IsHead)
            {
                var bossPlace = db.Placements.FirstOrDefault(x => x.OrgStructureId == placement.ParentOrgStructureId && x.BranchId == placement.BranchId && x.isCurrent && x.IsHead);
                if (bossPlace != null)
                {
                    var profile = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == bossPlace.EmployeeId);
                    if (profile != null)
                    {
                        boss = profile.FirstNameEnglish+" "+profile.MiddleNameEnglish+" "+profile.LastNameEnglish;
                    }
                }

            }
            else if (placement != null && !placement.IsHead)
            {
                var bossPlace = db.Placements.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId && x.BranchId == placement.BranchId && x.isCurrent && x.IsHead);
                if (bossPlace != null)
                {
                    var profile = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == bossPlace.EmployeeId);
                    if (profile != null)
                    {
                        boss = profile.FullNameaEnglish;
                    }
                }
            }

            return boss;
        }
        public ActionResult EvaluatorsRequest_Read([DataSourceRequest]DataSourceRequest request)
        {
            var id = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var orgstructure = db.Placements.FirstOrDefault(x => x.EmployeeId == id&&x.isCurrent&&x.IsHead);
            int currentUserDepId = orgstructure != null ? orgstructure.OrgStructureId : 0;

            IQueryable<PerformanceRequestPeriod> performancerequestperiods = db.PerformanceRequestPeriods.Where(x=>x.OrgStructureId== currentUserDepId);
            DataSourceResult result = performancerequestperiods.ToDataSourceResult(request, performanceRequestPeriod => new {
                PerformanceRequestPeriodId = performanceRequestPeriod.PerformanceRequestPeriodId,
                FiscalYearId = performanceRequestPeriod.FiscalYearId,
                PeriodId = performanceRequestPeriod.PeriodId,
                OrgStructureId = performanceRequestPeriod.OrgStructureId,
                Head = GetHead(id),
                DateCreated = performanceRequestPeriod.DateCreated,
                LastModified = performanceRequestPeriod.LastModified,
                CreatedBy = performanceRequestPeriod.CreatedBy,
                ModifiedBy = performanceRequestPeriod.ModifiedBy
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EvaluatorsRequest_Create([DataSourceRequest]DataSourceRequest request, PerformanceRequestPeriod performanceRequestPeriod)
        {
            var id = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var orgstructure = db.Placements.FirstOrDefault(x => x.EmployeeId == id &&  x.isCurrent&&x.IsHead);
            if(orgstructure != null)
            {
                performanceRequestPeriod.OrgStructureId =orgstructure.OrgStructureId;

            }
            else
            {
                ModelState.AddModelError("", "You are not Head");
            }
            if (ModelState.IsValid)
            {
                performanceRequestPeriod.DateCreated = DateTime.Now.Date;
                performanceRequestPeriod.CreatedBy = User.Identity.Name;
                var entity = GetMappedObject(performanceRequestPeriod);
                db.PerformanceRequestPeriods.Add(entity);
                db.SaveChanges();
                performanceRequestPeriod.PerformanceRequestPeriodId = entity.PerformanceRequestPeriodId;
            }

            return Json(new[] { performanceRequestPeriod }.ToDataSourceResult(request, ModelState),JsonRequestBehavior.AllowGet);
        }
      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EvaluatorsRequest_Update([DataSourceRequest]DataSourceRequest request, PerformanceRequestPeriod performanceRequestPeriod)
        {
            var id = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var orgstructure = db.Placements.FirstOrDefault(x => x.EmployeeId == id&&x.isCurrent&&x.IsHead);
            if (orgstructure != null)
            {
                performanceRequestPeriod.OrgStructureId = orgstructure.OrgStructureId;

            }
            else
            {
                ModelState.AddModelError("", "You are not Head");
            }
            if (ModelState.IsValid)
            {
                performanceRequestPeriod.LastModified = DateTime.Now.Date;
                performanceRequestPeriod.ModifiedBy= User.Identity.Name;
                var entity = GetMappedObject(performanceRequestPeriod);
                db.PerformanceRequestPeriods.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { performanceRequestPeriod }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EvaluatorsRequest_Destroy([DataSourceRequest]DataSourceRequest request, PerformanceRequestPeriod performanceRequestPeriod)
        {
            
            var peerToPeerEvaluationsExist = db.PeerToPeerPerformanceEvaluations.Any(x => x.PerformanceRequestPeriodId == performanceRequestPeriod.PerformanceRequestPeriodId);
            var internalCustomerEvaluationsExist = db.InternalCustomerPerformanceEvaluations.Any(x => x.PerformanceRequestPeriodId == performanceRequestPeriod.PerformanceRequestPeriodId);

            if (peerToPeerEvaluationsExist || internalCustomerEvaluationsExist)
            {
                if (peerToPeerEvaluationsExist && internalCustomerEvaluationsExist)
                {
                    ModelState.AddModelError("", "You cannot delete this record because it is linked to both Peer-to-Peer and Internal Customer performance evaluations. Please delete the related evaluations first.");
                }
                else if (peerToPeerEvaluationsExist)
                {
                    ModelState.AddModelError("", "You cannot delete this record because it is linked to existing Peer-to-Peer performance evaluations. Please delete the related evaluations first.");
                }
                else if (internalCustomerEvaluationsExist)
                {
                    ModelState.AddModelError("", "You cannot delete this record because it is linked to existing Internal Customer performance evaluations. Please delete the related evaluations first.");
                }

                return Json(new[] { performanceRequestPeriod }.ToDataSourceResult(request, ModelState));
            }


            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(performanceRequestPeriod);
                db.PerformanceRequestPeriods.Attach(entity);
                db.PerformanceRequestPeriods.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { performanceRequestPeriod }.ToDataSourceResult(request, ModelState));
        }
        private PerformanceRequestPeriod GetMappedObject(PerformanceRequestPeriod performanceRequestPeriod)
        {
            var entity = new PerformanceRequestPeriod
            {
                PerformanceRequestPeriodId = performanceRequestPeriod.PerformanceRequestPeriodId,
                FiscalYearId = performanceRequestPeriod.FiscalYearId,
                PeriodId = performanceRequestPeriod.PeriodId,
                OrgStructureId = performanceRequestPeriod.OrgStructureId,
                DateCreated = performanceRequestPeriod.DateCreated,
                LastModified = performanceRequestPeriod.LastModified,
                CreatedBy = performanceRequestPeriod.CreatedBy,
                ModifiedBy = performanceRequestPeriod.ModifiedBy
            };
            return entity;
        }
        public JsonResult GetFiscalYear()
        {

            var fiscalYear = db.GLFiscalYears.ToList();

            return Json(fiscalYear.Select(p =>
            new
            {
                FiscalYearId = p.GlFiscalYearId,
                Name = p.Name

            }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetEmployees(int depId)
        {
            //List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            //List<EmployeeIdView> EmployeeIdViews = new List<EmployeeIdView>();
            //EmployeeIdView EmployeeIdView = new EmployeeIdView();
            //var employeeList = db.Person_Employee.Where(x => x.IsActive);
            //var placments = db.Placements.Where(x => mybraches.Contains(x.BranchId.ToString()) && x.isCurrent);
            //if (employeeList.Any())
            //{
            //    employeeList = employeeList.AsQueryable();
            //}
            var placementEmployeeIds = db.Placements.Where(x => x.OrgStructureId == depId ).Select(x=>x.EmployeeId).ToList();
            var Employeelists = from e in db.Person_Employee
                        join p in db.Placements
                            on e.EmployeeId equals p.EmployeeId
                        select (new
                        {
                            OrgStructureId = p.OrgStructureId,
                            EmployeeId = e.EmployeeId,
                            PersonId = e.EmployeeId,
                            EmployeeCode = e.EmployeeCode,
                            FullName = e.FirstName + " " + e.MiddleName + " " + e.LastName,
                            FullNameCode = e.EmployeeCode + "- " + e.FirstName + " " + e.MiddleName + " " + e.LastName,
                            FullNameEng = e.EmployeeCode + "- " + e.FirstNameEnglish + " " + e.MiddleNameEnglish + " " + e.LastNameEnglish,

                            IsActive = e.IsActive,
                        });
            Employeelists = Employeelists.Where(x=>x.OrgStructureId ==depId);
            //HRMInputs inputs = new HRMInputs();
            //db = new ExceedDbContext();
            //var employees = inputs.GetEmployees(User.Identity.Name);
            var employees = db.Person_Employee.Where(x => placementEmployeeIds.Contains(x.EmployeeId)).ToList();
            return Json(employees.OrderBy(x => x.EmployeeId).Select(x=>new { PersonId=x.EmployeeId, FullNameEng = x.EmployeeCode + "- " + x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPeriodList(int fiscalYearId)
        {
            List<Quarter> quarterList = new List<Quarter>();
            var quarterLength = db.QuarterLengths.FirstOrDefault(x => x.GlFiscalYearId == fiscalYearId);
            if (quarterLength != null)
            {
                var quarter = db.Quarters.Where(x => x.QuarterLengthId == quarterLength.QuarterLengthId).ToList();
                quarterList.AddRange(quarter);
            }

            return Json(quarterList.Select(p => new
            {
                PeriodId = p.QuarterId,
                Name = p.Name

            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetWorkUnit()
        {
            var id = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var branchId = EmployeeHelper.GetEmployeePlacementBranch(id);
            //List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            var orgstucturIds = db.Placements.Where(x => x.BranchId==branchId && x.isCurrent).Select(x=>x.OrgStructureId).Distinct().ToList();

            return Json(db.MainCompanyStructures.Where(x=> orgstucturIds.Contains(x.OrgStructureId)).Select(p => new
            {
                Name = p.Name,
                OrgStructureId = p.OrgStructureId
            }), JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
