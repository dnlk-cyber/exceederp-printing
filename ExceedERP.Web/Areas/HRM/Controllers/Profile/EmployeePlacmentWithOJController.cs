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

namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{

    [AuthorizeRoles(HrmConstants.HRPlacementAdmin, HrmConstants.HRPlacementUser, HrmConstants.HRAdmin)]
    public class EmployeePlacmentWithOJController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static decimal SalaryAmount { get; set; }
        public static int Rankid { get; set; }
        public static int Stepid { get; set; }
        public static int DepartmentId { get; set; }
        public static DateTime AssigmentDate { get; set; }
        public static string AssigmentDateAmharic { get; set; }
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
            ViewData["EmployeeList"] = db.Person_Employee.ToList();// inputs.GetEmployeesForPlacment(User.Identity.Name);
            ViewData["EmployeeListEnglish"] = db.Person_Employee.ToList();//inputs.GetEmployeesForPlacment(User.Identity.Name);
            GetDepartments();
            var structres = db.MainCompanyStructures.AsQueryable();
            ViewData["organization"] = structres;

            ViewData["EmployeeCode"] = db.Person_Employee.Select(x => new { x.EmployeeId, x.EmployeeCode }).ToList();

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
        public void SetDate(string name, string value)
        {
            if (name == "AssignmentDateAmharic")
            {
                AssigmentDateAmharic = value;
                var splitDate = AssigmentDateAmharic.Split('/');
                AssigmentDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }

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
            IQueryable<Placement> placements = db.Placements.Where(x => mybraches.Contains(x.BranchId.ToString()));

            if (request.Filters != null)
            {
                dd: if (request.Filters.Any())
                {
                    foreach (var filter in request.Filters)
                    {
                        var descriptor = filter as FilterDescriptor;
                        if (descriptor != null)
                        {
                            if (descriptor.Member == "JobTitleName")
                            {
                                placements = ModifyFilters(request.Filters, placements).AsQueryable();

                            }

                        }
                        else if (filter is CompositeFilterDescriptor)
                        {

                            request.Filters = ((CompositeFilterDescriptor)filter).FilterDescriptors;
                            goto dd;
                            //ModifyFilters(((CompositeFilterDescriptor) filter).FilterDescriptors, person_employee);

                        }
                    }
                    //request.Filters.Clear();
                }
            }
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
            });

            return Json(result);

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

            db = new ExceedDbContext();

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

            var workunitHead = db.Placements.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId && x.IsHead);
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

                var salary = db.Salaries.FirstOrDefault(s => s.RankId == placement.RankId && s.StepId == placement.StepId);

                if (salary != null)
                {
                    placement.Salary = salary.SalaryAmount;
                }

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
                    isCurrent = false,
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

                    db.Entry(person).State = EntityState.Modified;
                    db.SaveChanges();
                }

                PerapreEmployeeAllowance(placement.EmployeeId, placement.LookupId);

                placement.PlacementId = entity.PlacementId;
                DepartmentId = 0;
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

                var salary = db.Salaries.FirstOrDefault(s => s.RankId == placement.RankId && s.StepId == placement.StepId);

                if (salary != null)
                {
                    placement.Salary = salary.SalaryAmount;
                }

                var entity = new Placement
                {
                    PlacementId = placement.PlacementId,
                    EmployeeId = placement.EmployeeId,
                    ImmediateSupervisor = placement.ImmediateSupervisor,
                    IsHead = placement.IsHead,
                    EmployementType = placement.EmployementType,
                    AssignmentDate = placement.AssignmentDate,
                    MappingId = 0,
                    BranchId = placement.BranchId,
                    JobTitleName = GetTitleName(placement.LookupId),
                    OrgStructureId = placement.OrgStructureId,
                    ReportsTo = placement.ReportsTo,
                    RankId = placement.RankId,
                    StepId = placement.StepId,
                    LookupId = placement.LookupId,
                    Department = placement.ReportsTo,
                    Salary = placement.Salary,
                    ReportsToMappingId = 0,
                    AssignmentType = placement.AssignmentType,
                    ResponsibilityLevel = placement.ResponsibilityLevel,
                    Remark = placement.Remark,
                    BusinessUnitId = placement.BusinessUnitId,
                    DateCreated = placement.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = placement.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    isCurrent = placement.isCurrent,
                    IsApprove = placement.IsApprove,
                    IsPend = placement.IsPend,
                    Index = placement.Index,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    AssignmentDateAmharic = placement.AssignmentDateAmharic,
                    EmployeeCode = inputs.GetEmployees().FirstOrDefault(x => x.PersonId == placement.EmployeeId)?.EmployeeCode,
                    PreparedByEmpId = placement.PreparedByEmpId

                };
                if (SalaryAmount >= 0)
                {
                    entity.Salary = SalaryAmount;
                }
                db.Placements.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                var employee = db.Person_Employee.FirstOrDefault(p => p.EmployeeId == placement.EmployeeId);

                if (employee != null)
                {
                    employee.OrgBranchName = placement.BranchId.ToString();
                    employee.ModifiedBy = User.Identity.Name;
                    employee.LastModified = DateTime.Now;
                    employee.BranchId = placement.BranchId;

                    db.Entry(employee).State = EntityState.Modified;
                    db.SaveChanges();
                }

                PerapreEmployeeAllowance(placement.EmployeeId, placement.LookupId);

                #region operation log
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                if (person != null)
                {
                    UserHelper.OperationLog(Request.UserHostAddress, "Employee Placement", "Success", User.Identity.Name, "Placement Info of " + person.EmployeeCode + " is Updated By  " + User.Identity.Name, Modules.HRM);

                }
                #endregion

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
                var entity = new Placement
                {
                    PlacementId = placement.PlacementId,
                    EmployeeId = placement.EmployeeId,
                    ImmediateSupervisor = placement.ImmediateSupervisor,
                    IsHead = placement.IsHead,
                    AssignmentDate = placement.AssignmentDate,
                    MappingId = placement.MappingId,
                    LookupId = placement.LookupId,
                    OrgStructureId = placement.OrgStructureId,
                    ReportsTo = placement.ReportsTo,
                    JobTitleName = placement.JobTitleName,
                    RankId = placement.RankId,
                    StepId = placement.StepId,
                    Department = placement.ReportsTo,
                    Salary = placement.Salary,
                    ReportsToMappingId = placement.ReportsToMappingId,
                    AssignmentType = placement.AssignmentType,
                    ResponsibilityLevel = placement.ResponsibilityLevel,
                    BranchId = placement.BranchId,
                    Remark = placement.Remark,
                    DateCreated = placement.DateCreated,
                    LastModified = placement.LastModified,
                    //DateCreatedAM = placement.DateCreatedAM,
                    //DateLastModifiedAM = placement.DateLastModifiedAM,
                    CreatedBy = placement.CreatedBy,
                    ModifiedBy = placement.ModifiedBy,
                    isCurrent = placement.isCurrent,
                    IsPend = placement.IsPend,
                    Index = placement.Index,
                    EmployementType = placement.EmployementType,
                    AssignmentDateAmharic = placement.AssignmentDateAmharic,
                    EmployeeCode = placement.EmployeeCode
                };

                #region delete log

                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name,
                    "Deleted placment information of employee Id" + placement.EmployeeCode);

                #endregion

                db.Placements.Attach(entity);
                db.Placements.Remove(entity);
                db.SaveChanges();

                db = new ExceedDbContext();
                var checkAssignment =
                    db.Placements.Where(x => x.EmployeeId == placement.EmployeeId)
                        .OrderByDescending(x => x.PlacementId)
                        .FirstOrDefault();
                if (checkAssignment != null)
                {
                    var entityy = new Placement
                    {
                        PlacementId = checkAssignment.PlacementId,
                        EmployeeId = checkAssignment.EmployeeId,
                        IsHead = checkAssignment.IsHead,
                        AssignmentDate = checkAssignment.AssignmentDate,
                        MappingId = checkAssignment.MappingId,
                        LookupId = placement.LookupId,
                        OrgStructureId = checkAssignment.OrgStructureId,
                        ReportsTo = checkAssignment.ReportsTo,
                        JobTitleName = checkAssignment.JobTitleName,
                        RankId = checkAssignment.RankId,
                        StepId = checkAssignment.StepId,
                        Department = checkAssignment.ReportsTo,
                        Salary = checkAssignment.Salary,
                        ReportsToMappingId = checkAssignment.ReportsToMappingId,
                        AssignmentType = checkAssignment.AssignmentType,
                        ResponsibilityLevel = checkAssignment.ResponsibilityLevel,
                        BranchId = checkAssignment.BranchId,
                        Remark = checkAssignment.Remark,
                        DateCreated = checkAssignment.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = checkAssignment.CreatedBy,
                        ModifiedBy = User.Identity.Name,
                        OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                        isCurrent = true,
                        IsPend = checkAssignment.IsPend,
                        Index = checkAssignment.Index,
                        AssignmentDateAmharic = checkAssignment.AssignmentDateAmharic,
                        EmployementType = checkAssignment.EmployementType,
                        EmployeeCode = checkAssignment.EmployeeCode,


                    };
                    db = new ExceedDbContext();
                    db.Placements.Attach(entityy);
                    db.Entry(entityy).State = EntityState.Modified;
                    db.SaveChanges();
                    string branch = checkAssignment.BranchId.ToString();
                    db = new ExceedDbContext();
                    db.Placements.Attach(entityy);
                    db.Entry(entityy).State = EntityState.Modified;
                    db.SaveChanges();
                    db = new ExceedDbContext();
                    var employee =
                        db.Person_Employee.Where(p => p.EmployeeId == placement.EmployeeId).FirstOrDefault();
                    if (employee != null)
                    {
                        employee.IsActive = true;
                        employee.Status = placement.EmployementType;
                        employee.OrgBranchName = branch;
                        employee.ModifiedBy = User.Identity.Name;
                        employee.LastModified = DateTime.Now;
                        db.Person_Employee.Attach(employee);
                        db.Entry(employee).State = EntityState.Modified;
                        db.SaveChanges();
                        PerapreEmployeeAllowance(checkAssignment.EmployeeId, checkAssignment.LookupId);
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
            Salary salary = new Salary();

            salary = db.Salaries.Where(s => s.RankId == rank && s.StepId == step).FirstOrDefault();
            SalaryAmount = 0;
            if (salary != null)
            {
                SalaryAmount = salary.SalaryAmount;
                return Convert.ToDecimal(salary.SalaryAmount); //Json(salary.SalaryAmount, JsonRequestBehavior.AllowGet);
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

        //public string GetDepartment(int OrgId, int title)
        //{
        //    var structures = db.MainCompanyStructures.Where(x=> x.OrgStructureId ==OrgId );
        //    var lookups = db.Lookups;


        //    foreach (var s in structures)
        //    {
        //        if (s.OrgStructureId == OrgId)
        //        {
        //            db = new ExceedDbContext();
        //            var types = db.Structure.ToList ();
        //           var  type = types.Where(x => x.StructureId == s.Type && x.IsDepartment == true).FirstOrDefault();
        //            if (type!=null)
        //            {
        //                DepartmentId = OrgId;
        //                 break;

        //            }
        //            else
        //            {
        //                if (s.Parent != 0)
        //                {
        //                    if (CheckParent(s.Parent)) ; break;
        //                }


        //            }

        //        }
        //    }
        //    db = new ExceedDbContext();
        //    var struc =
        //               db.SalariesJobTitles.Where(x =>  x.JobTitleid == title).FirstOrDefault();
        //    if (struc != null)
        //    {
        //        SalaryAmount = struc.Salary;
        //        Rankid = struc.RankId;
        //        Stepid = struc.StepId;
        //        return struc.Salary.ToString ()+"-"+struc .RankId .ToString ()+"-"+struc .StepId .ToString ();
        //    }
        //    return "";
        //}

        //private bool CheckParent(int Id)
        //{
        //    db = new ExceedDbContext();
        //    var structures = db.MainCompanyStructures.Where(x => x.OrgStructureId == Id).ToList();
        //    if (structures.Count >0)
        //    {
        //        foreach (var s in structures)
        //        {
        //            if (s.OrgStructureId == Id)
        //            {

        //                var type = db.Structure.Where(x => x.StructureId == s.Type && x.IsDepartment == true).FirstOrDefault();
        //                if (type != null)
        //                {
        //                    DepartmentId = Id;
        //                    return true;

        //                }
        //                else
        //                {
        //                    CheckParent(s.Parent);
        //                }

        //            }
        //        }
        //    }
        //    return false;
        //}
    }
}