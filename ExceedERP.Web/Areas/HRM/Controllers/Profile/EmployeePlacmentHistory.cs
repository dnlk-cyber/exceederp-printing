using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.Common.Controllers;
using ExceedERP.Web.Areas.HRM.Controllers.Company;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using System;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{

    [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRProfileUser, HrmConstants.HRAdmin)]
    public class EmployeePlacmentHistoryController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static DateTime From { get; set; }
        public static DateTime To { get; set; }
        public static string FromAmharic { get; set; }
        public static string ToAmharic { get; set; }
        HRMInputs inputs = new HRMInputs();
        // GET: HRM/EmployeePlacmentHistories
        public ActionResult Index()
        {
            PopulateViewData();
            GetOrganizations();
            return View();
        }
        private void PopulateViewData()
        {
            ViewData["EmployeeList"] = inputs.GetEmployees(User.Identity.Name);
        }
        public void SetEmployeeId(string fullName)
        {
            List<EmployeeIdView> EmployeeIdViews = new List<EmployeeIdView>();
            EmployeeIdView EmployeeIdView = new EmployeeIdView();
            var employeeList = db.Person_Employee.AsQueryable();
            if (employeeList.Count() > 0)
            {
                employeeList = employeeList.Where(l => l.IsActive).AsQueryable();
            }
            foreach (var emp in employeeList)
            {
                EmployeeIdView = new EmployeeIdView();
                EmployeeIdView.PersonId = emp.EmployeeId;
                EmployeeIdView.FullName = emp.FirstName + " " + emp.MiddleName + " " + emp.LastName;
                EmployeeIdViews.Add(EmployeeIdView);
            }
            var employees = EmployeeIdViews.Where(p => p.FullName == fullName).FirstOrDefault();
            if (employees != null)
            {
                HRMInputs.EmployeeId = employees.PersonId;
            }

        }


        [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRProfileUser, HrmConstants.HRAdmin)]
        public ActionResult PlacementHistories_Read([DataSourceRequest]DataSourceRequest request, int empId)
        {
            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            IQueryable<PlacementHistory> placementhistories = db.PlacementHistories.Where(x => x.EmployeeId == empId);
            DataSourceResult result = placementhistories.ToDataSourceResult(request, placementHistory => new
            {
                PlacementHistoryId = placementHistory.PlacementHistoryId,
                EmployeeId = placementHistory.EmployeeId,
                From = placementHistory.From,
                To = placementHistory.To,
                JobTitle = placementHistory.JobTitle,

                Rank = placementHistory.Rank,
                Step = placementHistory.Step,
                Salary = placementHistory.Salary,
                LookupId = placementHistory.LookupId,
                Remark = placementHistory.Remark,
                DateCreated = placementHistory.DateCreated,
                LastModified = placementHistory.LastModified,
                CreatedBy = placementHistory.CreatedBy,
                ModifiedBy = placementHistory.ModifiedBy,
                FromAmharic = placementHistory.FromAmharic,
                ToAmharic = placementHistory.ToAmharic,
                EmployeeCode = placementHistory.EmployeeCode,
                OrgBranchName = placementHistory.OrgBranchName,
                EmployementType = placementHistory.EmployementType
            });

            return Json(result);
        }

        [AuthorizeRoles(HrmConstants.HRProfileUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PlacementHistories_Create([DataSourceRequest]DataSourceRequest request, PlacementHistory placementHistory, int empIdT)
        {
            ModelState.Clear();

            placementHistory.EmployeeId = empIdT;

            if (placementHistory.FromAmharic != null)
            {
                var splitDate = placementHistory.FromAmharic.Split('/');
                placementHistory.From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else
            {
                ModelState.AddModelError("", "From Date is required");
            }
            if (placementHistory.ToAmharic != null)
            {
                var splitDate = placementHistory.ToAmharic.Split('/');
                placementHistory.To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else
            {
                ModelState.AddModelError("", "To date is required");
            }

            if (placementHistory.From > placementHistory.To)
            {
                ModelState.AddModelError("", "Please check the date range, The year of experience should be non negative!!");
            }
            else if (placementHistory.From > DateTime.Now)
            {

                ModelState.AddModelError("", "Start date should be less or equal to the current date!!");
            }
            else if (placementHistory.To > DateTime.Now)
            {

                ModelState.AddModelError("", "End date should be less or equal to the current date!!");
            }

            var person = db.Person_Employee.FirstOrDefault(x=>x.EmployeeId == placementHistory.EmployeeId);
            if(person != null)
            {
                if(placementHistory.From < person.HiredDate)
                {
                    ModelState.AddModelError("","Internal Experience date should be after hire date");
                }
            }

            var placements = db.Placements.Where(x => x.EmployeeId == placementHistory.EmployeeId).ToList();
            if (placements.Any())
            {
                DateTime firstAssignment = placements.Select(x => x.AssignmentDate).Min();
                if (firstAssignment.Year != 1)
                {
                    if (placementHistory.From >= firstAssignment || placementHistory.To >= firstAssignment)
                    {
                        ModelState.AddModelError("", "you can register only the experience before the system starts");
                    }
                }
            }

            var lookUp = db.Lookups.FirstOrDefault(x => x.LookupId == placementHistory.LookupId && x.LookupType == LookUpTypes.OLD_JOB_TITLE);
            if (lookUp != null)
            {
                placementHistory.JobTitle = lookUp.Description;
            }
            var emp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == placementHistory.EmployeeId);
            if (emp != null)
            {
                placementHistory.EmployeeCode = emp.EmployeeCode;
            }

            if (ModelState.IsValid)
            {
               
                var entity = new PlacementHistory
                {
                    EmployeeId = placementHistory.EmployeeId,
                    From = placementHistory.From,
                    To = placementHistory.To,
                    JobTitle = placementHistory.JobTitle,
                    Employee = placementHistory.Employee,
                    Rank = placementHistory.Rank,
                    Step = placementHistory.Step,
                    Salary = placementHistory.Salary,
                    LookupId = placementHistory.LookupId,
                    Remark = placementHistory.Remark,
                    DateCreated = DateTime.Now,
                    LastModified = placementHistory.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = placementHistory.ModifiedBy,
                    FromAmharic = placementHistory.FromAmharic,
                    ToAmharic = placementHistory.ToAmharic,
                    EmployeeCode = placementHistory.EmployeeCode,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    EmployementType = placementHistory.EmployementType
                };

                db.PlacementHistories.Add(entity);
                db.SaveChanges();
                placementHistory.PlacementHistoryId = entity.PlacementHistoryId;

            }
            return Json(new[] { placementHistory }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRProfileUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PlacementHistories_Update([DataSourceRequest]DataSourceRequest request, PlacementHistory placementHistory)
        {
            ModelState.Clear();

            if (placementHistory.FromAmharic != null)
            {
                var splitDate = placementHistory.FromAmharic.Split('/');
                placementHistory.From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else
            {
                ModelState.AddModelError("", "From Date is required");
            }
            if (placementHistory.ToAmharic != null)
            {
                var splitDate = placementHistory.ToAmharic.Split('/');
                placementHistory.To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else
            {
                ModelState.AddModelError("", "To date is required");
            }

            var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == placementHistory.EmployeeId);
            if (person != null)
            {
                if (placementHistory.From < person.HiredDate)
                {
                    ModelState.AddModelError("", "Internal Experience date should be after hire date");
                }
            }

            var placements = db.Placements.Where(x => x.EmployeeId == placementHistory.EmployeeId).ToList();
            if (placements.Any())
            {
                DateTime firstAssignment = placements.Select(x => x.AssignmentDate).Min();
                if (firstAssignment.Year != 1)
                {
                    if (placementHistory.From >= firstAssignment || placementHistory.To >= firstAssignment)
                    {
                        ModelState.AddModelError("", "you can register only the experience before the system starts");
                    }
                }
            }

            var lookUp = db.Lookups.FirstOrDefault(x => x.LookupId == placementHistory.LookupId && x.LookupType == LookUpTypes.OLD_JOB_TITLE);
            if (lookUp != null)
            {
                placementHistory.JobTitle = lookUp.Description;
            }
            var emp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == placementHistory.EmployeeId);
            if (emp != null)
            {
                placementHistory.EmployeeCode = emp.EmployeeCode;
            }

            if (ModelState.IsValid)
            {
            
                var entity = new PlacementHistory
                {
                    PlacementHistoryId = placementHistory.PlacementHistoryId,
                    EmployeeId = placementHistory.EmployeeId,
                    From = placementHistory.From,
                    To = placementHistory.To,
                    JobTitle = placementHistory.JobTitle,

                    Rank = placementHistory.Rank,
                    Step = placementHistory.Step,
                    Salary = placementHistory.Salary,
                    LookupId = placementHistory.LookupId,
                    Remark = placementHistory.Remark,
                    DateCreated = placementHistory.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = placementHistory.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    FromAmharic = placementHistory.FromAmharic,
                    ToAmharic = placementHistory.ToAmharic,
                    EmployeeCode = placementHistory.EmployeeCode,
                    OrgBranchName = placementHistory.OrgBranchName,
                    EmployementType = placementHistory.EmployementType
                };

                db.PlacementHistories.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { placementHistory }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PlacementHistories_Destroy([DataSourceRequest]DataSourceRequest request, PlacementHistory placementHistory)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new PlacementHistory
                {
                    PlacementHistoryId = placementHistory.PlacementHistoryId,
                    EmployeeId = placementHistory.EmployeeId,
                    From = placementHistory.From,
                    To = placementHistory.To,
                    JobTitle = placementHistory.JobTitle,
                    Rank = placementHistory.Rank,
                    Step = placementHistory.Step,
                    Salary = placementHistory.Salary,
                    LookupId = placementHistory.LookupId,
                    Remark = placementHistory.Remark,
                    DateCreated = placementHistory.DateCreated,
                    LastModified = placementHistory.LastModified,
                    CreatedBy = placementHistory.CreatedBy,
                    ModifiedBy = placementHistory.ModifiedBy,
                    FromAmharic = placementHistory.FromAmharic,
                    ToAmharic = placementHistory.ToAmharic,
                    EmployeeCode = placementHistory.EmployeeCode,
                    OrgBranchName = placementHistory.OrgBranchName,
                    EmployementType = placementHistory.EmployementType
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted placment history information of employee Id" + placementHistory.EmployeeCode);
                #endregion
                db.PlacementHistories.Attach(entity);
                db.PlacementHistories.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { placementHistory }.ToDataSourceResult(request, ModelState));
        }

        public void SetDate(string name, string value)
        {
            if (name == "FromAmharic")
            {
                FromAmharic = value;
                var splitDate = FromAmharic.Split('/');
                From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else if (name == "ToAmharic")
            {
                ToAmharic = value;
                var splitDate = ToAmharic.Split('/');
                To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
        }
        public JsonResult GetOrganizations()
        {

            var organizationUnits = db.MainCompanyStructures.ToList();

            ViewData["jobTitle"] = organizationUnits;
            return Json(organizationUnits.Select(s => new { OrgStructureId = s.OrgStructureId, Name = s.Name }), JsonRequestBehavior.AllowGet);
        }
    }
}