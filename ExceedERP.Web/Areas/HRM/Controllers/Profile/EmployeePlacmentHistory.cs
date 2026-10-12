using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using System;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.HR;

namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{

    [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRProfileUser)]
    public class EmployeePlacmentHistoryController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        HRMInputs inputs = new HRMInputs();
        // GET: HRM/EmployeePlacmentHistories
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            ViewData["EmployeeList"] = inputs.GetEmployees(User.Identity.Name);
            var organizationUnits = db.MainCompanyStructures.Select(x=>new {
                x.OrgStructureId,
                x.Name
            }).AsQueryable();
            ViewData["organizationUnits"] = organizationUnits;
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
        public ActionResult PlacementHistoriesCollections_Read([DataSourceRequest]DataSourceRequest request, int empId)
        {
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
                BranchId = placementHistory.BranchId,
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
        [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRProfileUser, HrmConstants.HRAdmin)]
        public ActionResult PlacementHistories_Read([DataSourceRequest]DataSourceRequest request, int empId)
        {

            IQueryable<PlacementHistory> placementhistories = db.PlacementHistories.Where(x => x.EmployeeId == empId);
            DataSourceResult result = placementhistories.ToDataSourceResult(request, placementHistory => new
            {
                PlacementHistoryId = placementHistory.PlacementHistoryId,
                EmployeeId = placementHistory.EmployeeId,
                BranchId = placementHistory.BranchId,
                LookupId = placementHistory.LookupId,
                JobTitle = placementHistory.JobTitle,
                FromAmharic = placementHistory.FromAmharic,
                ToAmharic = placementHistory.ToAmharic,
                EmployeeCode = placementHistory.EmployeeCode,
                OrgBranchName = placementHistory.OrgBranchName,
                EmployementType = placementHistory.EmployementType,
                From = placementHistory.From,
                To = placementHistory.To,
                Rank = placementHistory.Rank,
                Step = placementHistory.Step,
                Salary = placementHistory.Salary,
                Remark = placementHistory.Remark,
                DateCreated = placementHistory.DateCreated,
                LastModified = placementHistory.LastModified,
                CreatedBy = placementHistory.CreatedBy,
                ModifiedBy = placementHistory.ModifiedBy
               
            });

            return Json(result);
        }

        [AuthorizeRoles(HrmConstants.HRProfileUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PlacementHistories_Create([DataSourceRequest]DataSourceRequest request, PlacementHistory placementHistory, int empIdT)
        {
            ModelState.Clear();

            placementHistory.EmployeeId = empIdT;

            bool datePreference = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate ?? true;

            if (datePreference)
            {
                if (string.IsNullOrWhiteSpace(placementHistory?.FromAmharic) ||
                    string.IsNullOrWhiteSpace(placementHistory?.ToAmharic))
                {
                    ModelState.AddModelError("", "Both from and To Date fields are mandatory");
                    return Json(new[] { placementHistory }.ToDataSourceResult(request, ModelState));
                }

                placementHistory.From = HrHelper.ParseEthiopicDate(placementHistory.FromAmharic);
                placementHistory.To = HrHelper.ParseEthiopicDate(placementHistory.ToAmharic);
            }
            else
            {
                placementHistory.FromAmharic = XAPI.EthiopicDateTime.GetEthiopicDate( placementHistory.From.Day, 
                                                                                        placementHistory.From.Month, 
                                                                                        placementHistory.From.Year);
                placementHistory.ToAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(placementHistory.To.Day, 
                                                                                    placementHistory.To.Month, 
                                                                                    placementHistory.To.Year);
            }


            if (placementHistory.From.Date > placementHistory.To.Date)
            {
                ModelState.AddModelError("",
                        "Invalid date range: the 'From' date cannot be later than the 'To' date. Please enter a valid experience period.");
                return Json(new[] { placementHistory }.ToDataSourceResult(request, ModelState));

            }
            else if (placementHistory.From.Date > DateTime.Now.Date)
            {

                ModelState.AddModelError("", "Start date should be less or equal to the current date!!");
                return Json(new[] { placementHistory }.ToDataSourceResult(request, ModelState));

            }
            else if (placementHistory.To.Date > DateTime.Now.Date)
            {

                ModelState.AddModelError("", "End date should be less or equal to the current date!!");
                return Json(new[] { placementHistory }.ToDataSourceResult(request, ModelState));

            }

            var person = db.Person_Employee.FirstOrDefault(x=>x.EmployeeId == placementHistory.EmployeeId);
            if(person != null)
            {
                if(placementHistory.From.Date < person.HiredDate.Date && !person.IsReHired)
                {
                    ModelState.AddModelError("",$"Internal Experience date should be after hire date ({person.HiredDate:dd.MM.yyyy})");
                    return Json(new[] { placementHistory }.ToDataSourceResult(request, ModelState));

                }
            }

            var placements = db.Placements.Where(x => x.EmployeeId == placementHistory.EmployeeId).ToList();
            if (placements.Any())
            {
                DateTime firstAssignment = placements.Select(x => x.AssignmentDate).Min();
                if (firstAssignment.Year != 1 && person !=null && !person.IsReHired)
                {
                    if (placementHistory.From.Date >= firstAssignment.Date || placementHistory.To.Date >= firstAssignment.Date)
                    {
                        ModelState.AddModelError("",
                            $"You can register only experiences that occurred before the system start date ({firstAssignment:dd.MM.yyyy}). Please ensure both From and To dates are earlier than this date.");
                        return Json(new[] { placementHistory }.ToDataSourceResult(request, ModelState));

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
            var histories = db.PlacementHistories.AsNoTracking().ToList().Where(x =>x.EmployeeId==empIdT&& x.From.Date >= placementHistory.From.Date && x.To.Date <= placementHistory.To.Date /*&& x.JobTitle.ToLower().Trim() == placementHistory.JobTitle.ToLower().Trim()&&x.LookupId==placementHistory.LookupId*/);
            if (histories.Any())
            {
                ModelState.AddModelError("", "you have already Registered");
                return Json(new[] { placementHistory }.ToDataSourceResult(request, ModelState));


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
                    BranchId = placementHistory.BranchId,
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

            bool datePreference = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate ?? true;

            if (datePreference)
            {
                if (string.IsNullOrWhiteSpace(placementHistory?.FromAmharic) ||
                    string.IsNullOrWhiteSpace(placementHistory?.ToAmharic))
                {
                    ModelState.AddModelError("", "Both from and To Date fields are mandatory");
                    return Json(new[] { placementHistory }.ToDataSourceResult(request, ModelState));
                }

                placementHistory.From = HrHelper.ParseEthiopicDate(placementHistory.FromAmharic);
                placementHistory.To = HrHelper.ParseEthiopicDate(placementHistory.ToAmharic);
            }
            else
            {
                placementHistory.FromAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(placementHistory.From.Day,
                                                                                        placementHistory.From.Month,
                                                                                        placementHistory.From.Year);
                placementHistory.ToAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(placementHistory.To.Day,
                                                                                    placementHistory.To.Month,
                                                                                    placementHistory.To.Year);
            }

            var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == placementHistory.EmployeeId);
            if (person != null&& !person.IsReHired)
            {
                if (placementHistory.From.Date < person.HiredDate.Date)
                {
                    ModelState.AddModelError("", "Internal Experience date should be after hire date");
                    return Json(new[] { placementHistory }.ToDataSourceResult(request, ModelState));

                }
            }

            var placements = db.Placements.Where(x => x.EmployeeId == placementHistory.EmployeeId).ToList();
            if (placements.Any())
            {
                DateTime firstAssignment = placements.Select(x => x.AssignmentDate).Min();
                if (firstAssignment.Year != 1)
                {
                    if (placementHistory.From.Date >= firstAssignment.Date || placementHistory.To.Date >= firstAssignment.Date && person != null && !person.IsReHired)
                    {
                        ModelState.AddModelError("",
                            $"You can register only experiences that occurred before the system start date ({firstAssignment:dd.MM.yyyy}). Please ensure both From and To dates are earlier than this date.");
                        return Json(new[] { placementHistory }.ToDataSourceResult(request, ModelState));

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
                    BranchId = placementHistory.BranchId,
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
                    BranchId = placementHistory.BranchId,
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

      
      
        public JsonResult GetOrganizations()
        {

            var organizationUnits = db.MainCompanyStructures.ToList();

            ViewData["jobTitle"] = organizationUnits;
            return Json(organizationUnits.Select(s => new { OrgStructureId = s.OrgStructureId, Name = s.Name }), JsonRequestBehavior.AllowGet);
        }
    }
}