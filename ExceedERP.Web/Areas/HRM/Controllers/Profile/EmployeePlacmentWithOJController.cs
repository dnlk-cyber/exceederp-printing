using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Global;
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
using ExceedERP.Helpers.HR;

namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{

    [AuthorizeRoles(HrmConstants.HRPlacementAdmin, HrmConstants.HRPlacementUser, HrmConstants.HRAdmin)]
    public class EmployeePlacmentWithOJController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        HRMInputs inputs = new HRMInputs();
        public ActionResult Index()
        {
            GetRank();
            GetStep();
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            var employeeIds = db.Placements.Select(x => x.EmployeeId).ToList();
            ViewData["EmployeeList"] = db.Person_Employee.Where(x => employeeIds.Contains(x.EmployeeId)).Select(x => new { EmployeeId = x.EmployeeId, EmployeeCode = x.EmployeeCode, FullNameAmharic = x.FirstName + " " + x.MiddleName + " " + x.LastName, FullNameaEnglish = x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish }).ToList();// inputs.GetEmployeesForPlacment(User.Identity.Name);

            ViewData["jobTitle"] = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE).ToList();
            var branches = db.Branch;
            ViewData["branches"] = branches;
            var workunits = db.MainCompanyStructures.ToList();
            ViewData["workunits"] = workunits;
        }
        public ActionResult Refresh()
        {
            var allPlacments = db.Placements.Where(x => x.isCurrent).ToList();
            decimal salary = 0;
            foreach (var p in allPlacments)
            {
                p.Salary = GetSalaryAmount(p.RankId, p.StepId);
                p.ModifiedBy = User.Identity.Name;
                p.LastModified = DateTime.Now.Date;
                db.Placements.Attach(p);
                db.Entry(p).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");

        }
        public ActionResult GetEmployees([DataSourceRequest] DataSourceRequest request)
        {
            db = new ExceedDbContext();
            var employees = inputs.GetEmployeesForPlacment(User.Identity.Name);

            return Json(employees.ToDataSourceResult(request));
        }
        public ActionResult Employee_ValueMapper(string value)
        {
            var dataItemIndex = -1;
            var data = inputs.GetEmployeesForPlacment(User.Identity.Name);
            if (value != "")
            {
                var index = 0;
                foreach (var vm in data)
                {
                    if (vm.PersonId.ToString() == value)
                    {
                        dataItemIndex = index;
                        break;
                    }

                    index += 1;
                }
            }
            return Json(dataItemIndex, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStructure()
        {
            db = new ExceedDbContext();
            var orgs = db.MainCompanyStructures.ToList();


            return Json(orgs.Select(p => new { OrgStructureId = p.OrgStructureId, Name = p.Name }), JsonRequestBehavior.AllowGet);


        }
        public string GetBoss(int empId)
        {
            var boss = "";
            var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == empId && x.isCurrent);
            if (placement != null)
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
        public void GetDepartments()
        {
            List<MainCompanyStructure> allDepartment = new List<MainCompanyStructure>();
            var departments = db.MainCompanyStructures.Where(x => x.Parent == 0).FirstOrDefault();
            if (departments != null)
            {
                var deptlevelOne = db.MainCompanyStructures.Where(x => x.Parent == departments.OrgStructureId).ToList();
                if (deptlevelOne.Any())
                {
                    foreach (var a in deptlevelOne)
                    {
                        var departmentLevelTwo =
                            db.MainCompanyStructures.Where(x => x.Parent == a.OrgStructureId).ToList();
                        if (departmentLevelTwo.Any())
                        {
                            foreach (var b in departmentLevelTwo)
                            {
                                allDepartment.Add(b);
                            }
                        }
                        allDepartment.Add(a);
                    }
                }
            }
            ViewData["Departments"] = allDepartment;
        }
        private IQueryable<Placement> ModifyFilters(IEnumerable<IFilterDescriptor> filters, IQueryable<Placement> person_employee)
        {
            IQueryable<Placement> person_employees = new Placement[] { }.AsQueryable(); ;
            if (filters.Any())
            {
                string searchedValue = "";

                dd: if (filters.Any())
                {

                    foreach (var filter in filters)
                    {
                        var descriptor = filter as FilterDescriptor;
                        if (descriptor != null)
                        {
                            if (descriptor.Member == "JobTitleName")
                            {

                                searchedValue = descriptor.Value.ToString();
                                var value = searchedValue;
                                if (person_employees.Any())
                                {
                                    var person =
                                        person_employees.ToList().Where(
                                            j =>
                                                j.JobTitleName.ToUpper(XAPI.Common.exceedculture)
                                                    .Contains(value.ToUpper(XAPI.Common.exceedculture)) ==
                                                value.ToUpper(XAPI.Common.exceedculture)
                                                    .Contains(value.ToUpper(XAPI.Common.exceedculture))).AsQueryable();
                                    person_employees = person;
                                }
                                else
                                {
                                    var person =
                                           person_employee.ToList().Where(
                                               j =>
                                                   j.JobTitleName.ToUpper(XAPI.Common.exceedculture)
                                                       .Contains(value.ToUpper(XAPI.Common.exceedculture)) ==
                                                   value.ToUpper(XAPI.Common.exceedculture)
                                                       .Contains(value.ToUpper(XAPI.Common.exceedculture))).AsQueryable();
                                    person_employees = person_employees.Union(person).AsQueryable();
                                }

                            }

                        }
                        else if (filter is CompositeFilterDescriptor)
                        {

                            filters = ((CompositeFilterDescriptor)filter).FilterDescriptors;
                            goto dd;
                            //ModifyFilters(((CompositeFilterDescriptor) filter).FilterDescriptors, person_employee);

                        }

                    }

                }
            }
            return person_employees.AsQueryable();
        }

        [AuthorizeRoles(HrmConstants.HRPlacementAdmin, HrmConstants.HRPlacementUser, HrmConstants.HRAdmin)]
        public ActionResult Placements_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            var placements = db.Placements.Where(x => mybraches.Contains(x.BranchId.ToString())).ToList();

            //if (request.Filters != null)
            //{
            //    dd: if (request.Filters.Any())
            //    {
            //        foreach (var filter in request.Filters)
            //        {
            //            var descriptor = filter as FilterDescriptor;
            //            if (descriptor != null)
            //            {
            //                if (descriptor.Member == "JobTitleName")
            //                {
            //                    placements = ModifyFilters(request.Filters, placements).AsQueryable();

            //                }

            //            }
            //            else if (filter is CompositeFilterDescriptor)
            //            {

            //                request.Filters = ((CompositeFilterDescriptor)filter).FilterDescriptors;
            //                goto dd;
            //                //ModifyFilters(((CompositeFilterDescriptor) filter).FilterDescriptors, person_employee);

            //            }
            //        }
            //        //request.Filters.Clear();
            //    }
            //}

            DataSourceResult result = placements.ToDataSourceResult(request, placement => new
            {
                PlacementId = placement.PlacementId,
                EmployeeId = placement.EmployeeId,
                ImmediateSupervisor = placement.ImmediateSupervisor,
                IsHead = placement.IsHead,
                AssignmentDate = placement.AssignmentDate,
                OrgStructureId = placement.OrgStructureId,
                LookupId = placement.LookupId,
                MappingId = placement.MappingId,
                JobTitleName = GetTitleName(placement.LookupId),
                ReportsToMappingId = placement.ReportsToMappingId,
                RankId = placement.RankId,
                StepId = placement.StepId,
                BranchId = placement.BranchId,
                Salary = placement.Salary,
                Department = placement.Department,
                ReportsTo = placement.ReportsTo,
                AssignmentType = placement.AssignmentType,
                ResponsibilityLevel = placement.ResponsibilityLevel,
                Remark = placement.Remark,
                DateCreated = placement.DateCreated,
                LastModified = placement.LastModified,
                CreatedBy = placement.CreatedBy,
                ModifiedBy = placement.ModifiedBy,
                //DateCreatedAM = placement.DateCreatedAM,
                //DateLastModifiedAM = placement.DateLastModifiedAM,
                isCurrent = placement.isCurrent,
                AssignmentDateAmharic = placement.AssignmentDateAmharic,
                EmployeeCode = placement.EmployeeCode,
                Boss = GetBoss(placement.EmployeeId),
                EmployementType = placement.EmployementType,
                IsPend = placement.IsPend,
                BusinessUnitId = placement.BusinessUnitId,
                IsApprove = placement.IsApprove,
                FullName = GetFullName(placement.EmployeeId),
                HireDate = HrHelper.GetHireDate(placement.EmployeeId)
            });

            return Json(result);
        }
        public string GetFullName(int empId)
        {
            var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == empId);
            if (employee != null)
            {
                return employee.FullNameaEnglish;
            }

            return "";
        }
        private JobTitleMapping GetJobtitleCode(int id)
        {
            if (id != 0)
            {
                db = new ExceedDbContext();
                var title = db.JobTitleMapping.Where(x => x.JobTitleMappingId == id).FirstOrDefault();
                return title;
            }
            return new JobTitleMapping();

        }
        public void PerapreEmployeeAllowance(int employeeId, int jobtitle)
        {
            var alloanceList = db.AllowanceDefinations;
            var mappings = db.AllowanceMappings;

            var placment = db.Placements.Where(x => x.isCurrent && x.EmployeeId == employeeId && x.LookupId == jobtitle).FirstOrDefault();


            db = new ExceedDbContext();

            foreach (var x in mappings)
            {

                if (placment != null)
                {
                    if (x.LookupId == placment.LookupId)
                    {
                        var checkExistance = db.AllowancePerEmployees.Where(g => g.EmployeeId == employeeId && x.AllowanceDefinationId == g.AllowanceDefinationId).FirstOrDefault();
                        if (checkExistance == null)
                        {
                            var entity = new AllowancePerEmployee
                            {
                                EmployeeId = employeeId,
                                AllowanceDefinationId = x.AllowanceDefinationId,
                                Remark = null,
                                DateCreated = DateTime.Now,
                                LastModified = null,
                                CreatedBy = User.Identity.Name,
                                ModifiedBy = null,
                                EmployeeCode = placment.EmployeeCode,
                                From = null,
                                To = null,
                                Pend = false,
                                OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)
                            };

                            db.AllowancePerEmployees.Add(entity);
                            db.SaveChanges();

                        }
                        else
                        {

                        }

                    }
                }


            }

        }

        [AuthorizeRoles(HrmConstants.HRPlacementUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Placements_Create([DataSourceRequest]DataSourceRequest request, Placement placement)
        {
            ModelState.Clear();

            #region validation

            if (placement.AssignmentDateAmharic != null)
            {
                var splitDate = placement.AssignmentDateAmharic.Split('/');
                placement.AssignmentDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }

            var checkNotApproved = db.Placements.FirstOrDefault(x => x.EmployeeId == placement.EmployeeId && !x.IsApprove);
            if (checkNotApproved != null)
            {
                ModelState.AddModelError("", "This employee already have not approved placement");
            }

            var checkAssignment = db.Placements.FirstOrDefault(x => x.EmployeeId == placement.EmployeeId);
            if (checkAssignment != null && placement.AssignmentType == AssignmentType.NEW)
            {
                ModelState.AddModelError("", "You can not assign this employee as a new assignment!!");
            }
            var checkexistance =
                    db.Placements.FirstOrDefault(x => x.EmployeeId == placement.EmployeeId && x.OrgStructureId == placement.OrgStructureId &&
                            x.LookupId == placement.LookupId && x.BranchId == placement.BranchId &&
                            x.RankId == placement.RankId && x.StepId == placement.StepId &&
                            x.AssignmentType == placement.AssignmentType);

            if (checkexistance != null)
            {
                ModelState.AddModelError("", "This employee is already assigened ");
            }
            if (placement.AssignmentDate > DateTime.Now)
            {
                ModelState.AddModelError("", "Assigment date should be less or equal to the current date!!");
            }

            var workunitHead = db.Placements.FirstOrDefault(x => x.BranchId == placement.BranchId && x.OrgStructureId == placement.OrgStructureId && x.IsHead);
            if (placement.IsHead && workunitHead != null)
            {
                ModelState.AddModelError("", "This Workunit already have Head");
            }

            var emp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == placement.EmployeeId);
            if (emp != null)
            {
                if (placement.AssignmentDate < emp.HiredDate)
                {
                    ModelState.AddModelError("", "assignment date can't lessthan hire date");
                }

                placement.EmployementType = emp.Status;
            }

            var palcements = db.Placements.Where(x => x.EmployeeId == placement.EmployeeId).ToList();

            var empPlace = palcements.Where(x => x.AssignmentType == AssignmentType.PROMOTION).ToList().OrderByDescending(x => x.AssignmentDate);
            if (empPlace.Any())
            {
                var placeFirstOrDefault = empPlace.FirstOrDefault();
                if (placeFirstOrDefault != null)
                {
                    var diff = DateTime.Today.Subtract(placeFirstOrDefault.AssignmentDate).Days;
                    if (diff < 365 && placement.AssignmentType == AssignmentType.PROMOTION)
                    {
                        ModelState.AddModelError("", "you were promoted before, you can not promoted two times a year ");
                    }
                }
            }

            var yearProSetting = db.Configurations.FirstOrDefault(c => c.Category == "Promotion" && c.Reference == "YEARLY_PROMOTION_PER_YEAR");
            if (yearProSetting != null)
            {
                var yearPros = palcements.Where(x => x.AssignmentType == AssignmentType.YEARLY_PROMOTION).OrderByDescending(x => x.AssignmentDate).ToList();
                if (yearPros.Any())
                {
                    var placeFirstOrDefault = empPlace.FirstOrDefault();
                    if (placeFirstOrDefault != null)
                    {
                        var days = Convert.ToInt32(yearProSetting.CurrentValue) * 365;
                        var diff = DateTime.Today.Subtract(placeFirstOrDefault.AssignmentDate).Days;

                        if (diff < days && placement.AssignmentType == AssignmentType.YEARLY_PROMOTION)
                        {
                            ModelState.AddModelError("", "you were promoted yearly promotion before, you can not promoted two times a year ");
                        }
                    }
                }
            }

            #endregion

            var contractEmp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == placement.EmployeeId);
            if (contractEmp != null && contractEmp.Status == EmployeeStatus.CONTRACT)
            {
                var contractLast = db.Contract.Where(x => x.EmployeeId == placement.EmployeeId).OrderByDescending(x => x.To).ToList().FirstOrDefault();
                if (contractLast != null && contractLast.To <= placement.AssignmentDate)
                {
                    ModelState.AddModelError("", "First Extend Contract date for this employee ");
                }
            }

            var freelancerEmp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == placement.EmployeeId);
            if (freelancerEmp != null && freelancerEmp.Status == EmployeeStatus.FREELANCER)
            {
                var freelancerLast = db.Freelancers.Where(x => x.EmployeeId == placement.EmployeeId).OrderByDescending(x => x.To).ToList().FirstOrDefault();
                if (freelancerLast != null && freelancerLast.To <= placement.AssignmentDate)
                {
                    ModelState.AddModelError("", "First Extend Freelancer date for this employee ");
                }
            }

            var manPower = db.JobTitleMapping.FirstOrDefault(x => x.BranchId == placement.BranchId && x.OrgStructureId == placement.OrgStructureId && x.LookupId == placement.LookupId);
            if (manPower != null && manPower.PlanType == JobTitlePlan.BOUNDED)
            {
                var allPlacementOnThisJob = db.Placements.Where(x => x.BranchId == manPower.BranchId && x.OrgStructureId == manPower.OrgStructureId && x.LookupId == manPower.LookupId && x.isCurrent).ToList();
                if (allPlacementOnThisJob.Any())
                {
                    int totalPlaced = allPlacementOnThisJob.Count();
                    if (totalPlaced >= manPower.Quntity)
                    {
                        ModelState.AddModelError("", "This Jobtitle is set to Bounded so you can't assign more");
                    }
                }
            }

            if (ModelState.IsValid)
            {
                int i = 0;

                var currentPlacement = db.Placements.FirstOrDefault(x => x.EmployeeId == placement.EmployeeId && x.isCurrent);

                if (currentPlacement != null)
                {
                    // create placement history after the system started
                    // it take its assigment date as 'From' and the new assigment date 'To' 
                    PlacementAfterSystem_Create(currentPlacement, placement.AssignmentDate);
                }

                placement.PreparedByEmpId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

                placement.Salary = HrHelper.GetSalaryFromSalarySetting(placement.StepId, placement.RankId, placement.LookupId);

                var entity = new Placement
                {
                    EmployeeId = placement.EmployeeId,
                    ImmediateSupervisor = placement.ImmediateSupervisor,
                    IsHead = placement.IsHead,
                    AssignmentDate = placement.AssignmentDate,
                    MappingId = 0,
                    RankId = placement.RankId,
                    StepId = placement.StepId,
                    LookupId = placement.LookupId,
                    OrgStructureId = placement.OrgStructureId,
                    Salary = placement.Salary,
                    ReportsTo = placement.ReportsTo,
                    JobTitleName = GetTitleName(placement.LookupId),
                    Department = placement.ReportsTo,
                    ReportsToMappingId = 0,
                    AssignmentType = placement.AssignmentType,
                    ResponsibilityLevel = placement.ResponsibilityLevel,
                    BranchId = placement.BranchId,
                    BusinessUnitId = placement.BusinessUnitId,
                    Remark = placement.Remark,
                    DateCreated = DateTime.Now,
                    LastModified = placement.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = placement.ModifiedBy,
                    isCurrent = false, // this will be true after approval
                    IsApprove = false,
                    IsPend = false,
                    Index = i,
                    EmployementType = placement.EmployementType,
                    AssignmentDateAmharic = placement.AssignmentDateAmharic,
                    EmployeeCode = inputs.GetEmployees().FirstOrDefault(x => x.PersonId == placement.EmployeeId)?.EmployeeCode,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    PreparedByEmpId = placement.PreparedByEmpId
                };

                db.Placements.Add(entity);
                db.SaveChanges();

                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                if (person != null)
                {
                    person.IsActive = true;
                    person.BranchId = entity.BranchId;
                    person.OrgBranchName = entity.BranchId.ToString();

                    db.Entry(person).State = EntityState.Modified;
                    db.SaveChanges();
                }

                PerapreEmployeeAllowance(placement.EmployeeId, placement.LookupId);

                placement.PlacementId = entity.PlacementId;
            }

            return Json(new[] { placement }.ToDataSourceResult(request, ModelState));
        }
        private string GetTitleName(int id)
        {

            try
            {
                db = new ExceedDbContext();
                var name = db.Lookups.Where(x => x.LookupId == id).FirstOrDefault().Description;
                return name;
            }
            catch (Exception)
            {

                return "";
            }
        }

        [AuthorizeRoles(HrmConstants.HRPlacementUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Placements_Update([DataSourceRequest]DataSourceRequest request, Placement placement)
        {
            ModelState.Clear();
            if (placement.AssignmentDateAmharic != null)
            {
                var splitDate = placement.AssignmentDateAmharic.Split('/');
                placement.AssignmentDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }

            var emp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == placement.EmployeeId);
            if (emp != null)
            {
                if (placement.AssignmentDate.Date < emp.HiredDate.Date)
                {
                    ModelState.AddModelError("", "assignment date can't lessthan hire date");
                }
            }


            if (placement.AssignmentDate > DateTime.Now)
            {

                ModelState.AddModelError("", "Assigment date should be less or equal to the current date!!");

            }
            var pp = db.Placements.FirstOrDefault(x => x.EmployeeId == placement.EmployeeId && x.isCurrent && x.PlacementId != placement.PlacementId);
            if (pp != null && placement.isCurrent)
            {
                ModelState.AddModelError("", "this employee have other current placement ");
            }

            var workunitHead = db.Placements.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId && x.IsHead && x.EmployeeId != placement.EmployeeId && x.isCurrent);
            if (placement.IsHead && workunitHead != null)
            {
                ModelState.AddModelError("", "This Workunit already have Head");
            }

            var profile = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == placement.EmployeeId);
            if (profile != null)
            {
                placement.EmployementType = profile.Status;
            }

            var contractEmp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == placement.EmployeeId);
            if (contractEmp != null && contractEmp.Status == EmployeeStatus.CONTRACT)
            {
                var contractLast = db.Contract.Where(x => x.EmployeeId == placement.EmployeeId).OrderByDescending(x => x.To).ToList().FirstOrDefault();
                if (contractLast != null && contractLast.To <= placement.AssignmentDate)
                {
                    ModelState.AddModelError("", "First Extend Contract date for this employee ");
                }
            }

            var freelancerEmp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == placement.EmployeeId);
            if (freelancerEmp != null && freelancerEmp.Status == EmployeeStatus.FREELANCER)
            {
                var freelancerLast = db.Freelancers.Where(x => x.EmployeeId == placement.EmployeeId).OrderByDescending(x => x.To).ToList().FirstOrDefault();
                if (freelancerLast != null && freelancerLast.To <= placement.AssignmentDate)
                {
                    ModelState.AddModelError("", "First Extend Freelancer date for this employee ");
                }
            }

            var manPower = db.JobTitleMapping.FirstOrDefault(x => x.BranchId == placement.BranchId && x.OrgStructureId == placement.OrgStructureId && x.LookupId == placement.LookupId);
            if (manPower != null && manPower.PlanType == JobTitlePlan.BOUNDED)
            {
                var allPlacementOnThisJob = db.Placements.Where(x => x.BranchId == manPower.BranchId && x.OrgStructureId == manPower.OrgStructureId && x.LookupId == manPower.LookupId && x.isCurrent && x.PlacementId != placement.PlacementId).ToList();
                if (allPlacementOnThisJob.Any())
                {
                    int totalPlaced = allPlacementOnThisJob.Count();
                    if (totalPlaced >= manPower.Quntity)
                    {
                        ModelState.AddModelError("", "This Jobtitle is set to Bounded so you can't assign more");
                    }
                }
            }

            if (ModelState.IsValid)
            {
                var place = db.Placements.Find(placement.PlacementId);
                if (place != null)
                {
                    place.PlacementId = placement.PlacementId;
                    place.IsHead = placement.IsHead;
                    place.EmployementType = placement.EmployementType;
                    place.AssignmentDate = placement.AssignmentDate;
                    place.BranchId = placement.BranchId;
                    place.JobTitleName = GetTitleName(placement.LookupId);
                    place.OrgStructureId = placement.OrgStructureId;
                    place.ReportsTo = placement.ReportsTo;
                    place.RankId = placement.RankId;
                    place.StepId = placement.StepId;
                    place.LookupId = placement.LookupId;
                    place.Department = placement.ReportsTo;
                    place.Salary = placement.Salary;
                    place.AssignmentType = placement.AssignmentType;
                    place.ResponsibilityLevel = placement.ResponsibilityLevel;
                    place.Remark = placement.Remark;
                    place.BusinessUnitId = placement.BusinessUnitId;
                    place.DateCreated = placement.DateCreated;
                    place.LastModified = DateTime.Now;
                    place.ModifiedBy = User.Identity.Name;
                    place.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                    place.AssignmentDateAmharic = placement.AssignmentDateAmharic;
                    place.EmployeeCode = inputs.GetEmployees().FirstOrDefault(x => x.PersonId == placement.EmployeeId)?.EmployeeCode;

                    db.Entry(place).State = EntityState.Modified;
                    db.SaveChanges();

                    var employee = db.Person_Employee.FirstOrDefault(p => p.EmployeeId == place.EmployeeId);

                    if (employee != null)
                    {
                        employee.OrgBranchName = placement.BranchId.ToString();
                        employee.ModifiedBy = User.Identity.Name;
                        employee.LastModified = DateTime.Now;
                        employee.BranchId = placement.BranchId;

                        db.Entry(employee).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    PerapreEmployeeAllowance(place.EmployeeId, placement.LookupId);

                    #region operation log
                    var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == place.EmployeeId);
                    if (person != null)
                    {
                        UserHelper.OperationLog(Request.UserHostAddress, "Employee Placement", "Success", User.Identity.Name, "Placement Info of " + person.EmployeeCode + " is Updated By  " + User.Identity.Name, Modules.HRM);
                    }
                    #endregion
                }
            }
            return Json(new[] { placement }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRPlacementAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Placements_Destroy([DataSourceRequest]DataSourceRequest request, Placement placement)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var place = db.Placements.Find(placement.EmployeeId);
                if (place != null)
                {
                    db.Entry(place).State = EntityState.Deleted;
                    db.SaveChanges();

                    #region delete log

                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted placment information of employee Id" + placement.EmployeeCode);

                    #endregion


                    var checkAssignment = db.Placements.Where(x => x.EmployeeId == placement.EmployeeId).OrderByDescending(x => x.PlacementId).FirstOrDefault();

                    if (checkAssignment != null)
                    {
                        checkAssignment.isCurrent = true;

                        db.Entry(checkAssignment).State = EntityState.Modified;
                        db.SaveChanges();

                        string branch = checkAssignment.BranchId.ToString();

                        db = new ExceedDbContext();
                        var employee = db.Person_Employee.FirstOrDefault(p => p.EmployeeId == placement.EmployeeId);
                        if (employee != null)
                        {
                            employee.IsActive = true;
                            employee.Status = checkAssignment.EmployementType;
                            employee.OrgBranchName = branch;
                            employee.ModifiedBy = User.Identity.Name;
                            employee.LastModified = DateTime.Now;

                            db.Entry(employee).State = EntityState.Modified;
                            db.SaveChanges();

                            PerapreEmployeeAllowance(checkAssignment.EmployeeId, checkAssignment.LookupId);
                        }
                    }
                }
            }
            return Json(new[] { placement }.ToDataSourceResult(request, ModelState));
        }
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "Excel-Export", "Success", User.Identity.Name, "Exported employee placment");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "PDF-Export", "Success", User.Identity.Name, "Exported employee placment");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        public JsonResult GetRank()
        {
            db = new ExceedDbContext();
            var ranks = db.Ranks.AsQueryable();
            ViewData["RankList"] = ranks;
            return Json(ranks.Select(p => new { RankId = p.RankId, RankName = p.RankName }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetStep()
        {
            db = new ExceedDbContext();
            var stepss = db.Steps.AsQueryable();
            ViewData["StepList"] = stepss;
            return Json(stepss.Select(p => new { StepId = p.StepId, StepName = p.StepName }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSalary()
        {
            db = new ExceedDbContext();
            var salaries = db.Salaries.AsQueryable();
            return Json(salaries.Select(p => new
            {
                SalaryStructureId = p.SalaryStructureId,
                SalaryAmount = p.SalaryAmount
            }), JsonRequestBehavior.AllowGet);
        }
        public void PlacementAfterSystem_Create(Placement placement, DateTime dateNewAssignmet)
        {
            if (placement != null)
            {
                var place = new PlacementAfterSystem();

                place.PlacementId = placement.PlacementId;
                place.EmployeeId = placement.EmployeeId;
                place.OrgStructureId = placement.OrgStructureId;
                place.LookupId = placement.LookupId;
                place.RankId = placement.RankId;
                place.StepId = placement.StepId;
                place.Salary = placement.Salary;
                place.AssignmentType = placement.AssignmentType;
                place.ResponsibilityLevel = placement.ResponsibilityLevel;
                place.BranchId = placement.BranchId;
                place.IsCurrent = placement.isCurrent;
                place.IsHead = placement.IsHead;
                place.EmployementType = placement.EmployementType;
                place.From = placement.AssignmentDate;
                place.To = dateNewAssignmet;
                place.FromAm = XAPI.EthiopicDateTime.GetEthiopicDate(placement.AssignmentDate.Day, placement.AssignmentDate.Month, placement.AssignmentDate.Year);
                place.ToAm = XAPI.EthiopicDateTime.GetEthiopicDate(dateNewAssignmet.Day, dateNewAssignmet.Month, dateNewAssignmet.Year);


                db.PlacementAfterSystems.Add(place);
                db.SaveChanges();
            }
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
        [HttpPost]
        public decimal GetSalaryAmount(int rank, int step)
        {
            var salary = db.SalariesJobTitles.FirstOrDefault(s => s.RankId == rank && s.StepId == step);

            if (salary != null)
            {
                return salary.Salary; //Json(salary.SalaryAmount, JsonRequestBehavior.AllowGet);
            }

            return 0; //Json(0, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetJobTitle(int OrgStructureId)
        {
            db = new ExceedDbContext();
            var orgs = db.JobTitleMapping.Where(x => x.OrgStructureId == OrgStructureId).ToList();
            List<Lookup> titleList = new List<Lookup>();
            if (orgs.Any())
            {
                titleList = new List<Lookup>();
                foreach (var title in orgs)
                {
                    var lookup = db.Lookups.Where(x => x.LookupId == title.LookupId).FirstOrDefault();
                    if (!titleList.Contains(lookup))
                    {
                        titleList.Add(lookup);
                    }

                }
            }
            return Json(titleList.Select(p => new { LookupId = p.LookupId, Description = p.Description }), JsonRequestBehavior.AllowGet);


        }
        public ActionResult ApprovePlacement(int id)
        {
            var placement = db.Placements.FirstOrDefault(x => x.PlacementId == id);

            if (placement != null)
            {
                int i = 0;

                var placements = db.Placements.Where(x => x.isCurrent && x.EmployeeId == placement.EmployeeId && x.PlacementId != placement.PlacementId).ToList();
                foreach (var p in placements)
                {
                    p.MappingId = 0;
                    p.JobTitleName = GetTitleName(p.LookupId);
                    p.ReportsToMappingId = 0;
                    p.LastModified = DateTime.Now;
                    p.CreatedBy = p.CreatedBy;
                    p.ModifiedBy = User.Identity.Name;
                    p.isCurrent = false;
                    p.Index = i;
                    p.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                    p.ApprovedByEmpId = p.EmployeeId;

                    db.Entry(p).State = EntityState.Modified;

                    i += 1;
                }

                placement.IsApprove = true;
                placement.isCurrent = true;
                placement.ApprovedByEmpId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

                db.Entry(placement).State = EntityState.Modified;
                db.SaveChanges();

                var employee = db.Person_Employee.FirstOrDefault(p => p.EmployeeId == placement.EmployeeId);

                if (employee != null)
                {
                    employee.IsActive = true;
                    employee.IsPlaced = true;
                    employee.OrgBranchName = placement.BranchId.ToString();
                    employee.ModifiedBy = User.Identity.Name;
                    employee.LastModified = DateTime.Now;
                    employee.BranchId = placement.BranchId;

                    db.Entry(employee).State = EntityState.Modified;
                    db.SaveChanges();
                }

                var formattedData = new
                {
                    Message = "You successfully approve this placement.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            var formattedData1 = new
            {
                Message = "this placement is not approved.",
                Status = "FAIL"
            };
            return Json(formattedData1, JsonRequestBehavior.AllowGet);

        }
        public ActionResult UpdateNewSalary()
        {
            var placements = db.Placements.Where(x => x.isCurrent).ToList();
            foreach (var place in placements)
            {
                var salarySetting = db.SalariesJobTitles.FirstOrDefault(x => x.RankId == place.RankId && x.StepId == place.StepId);
                if (salarySetting != null)
                {
                    place.Salary = salarySetting.Salary;

                    db.Entry(place).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }
    }
}