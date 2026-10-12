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
using System.Web;
using System.IO;

namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{

    [AuthorizeRoles(HrmConstants.HRTemporaryPlacementAdmin)]
    public class EmployeePlacmentTemporaryWithOJController : BaseController
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
        public ActionResult PlacementWithSwap()
        {
            GetRank();
            GetStep();
            PopulateViewData();
            return View();

        }
        private void PopulateViewData()
        {
            var employeeIds = db.Placements.Select(x => x.EmployeeId).ToList();
            // ViewData["EmployeeList"] = db.Person_Employee.Where(x => employeeIds.Contains(x.EmployeeId)).Select(x => new { EmployeeId = x.EmployeeId, EmployeeCode = x.EmployeeCode, FullNameAmharic = x.FirstName + " " + x.MiddleName + " " + x.LastName, FullNameaEnglish = x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish }).ToList();// inputs.GetEmployeesForPlacment(User.Identity.Name);
            ViewData["EmployeeList"] = db.Person_Employee.Where(x => employeeIds.Contains(x.EmployeeId)).Select(x => new { EmployeeId = x.EmployeeId, EmployeeCode = x.EmployeeCode, FullNameAmharic = x.EmployeeCode + " - " + x.FirstName + " " + x.MiddleName + " " + x.LastName, FullNameaEnglish = x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish }).ToList();// inputs.GetEmployeesForPlacment(User.Identity.Name);
            ViewData["jobTitle"] = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE).ToList();
            //var branches = db.Branch;
            //ViewData["branches"] = branches;
            //var workunits = db.MainCompanyStructures.ToList();
            //ViewData["workunits"] = workunits;
            var branches = db.Branch.Select(x => new { x.BranchId, Name = x.Name.Replace("\n", "").Replace("\r", "").Replace("#", "").Replace("+", " ") });
            ViewData["branches"] = branches;
            var businessUnits = db.BusinessUnits.Select(x => new { x.BusinessUnitId, Name = x.Name.Replace("\n", "").Replace("\r", "").Replace("#", "").Replace("+", " ") });
            ViewData["businessUnits"] = businessUnits;
            var workunits = db.MainCompanyStructures.Select(x => new { x.OrgStructureId, Name = x.Name.Replace("\n", "").Replace("\r", "").Replace("#", "").Replace("+", " ") }).ToList();
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
            if (placement != null&& placement.IsHead)
            {
                var bossPlace = db.Placements.FirstOrDefault(x => x.OrgStructureId == placement.ParentOrgStructureId /*&& x.BranchId == placement.BranchId*/ && x.isCurrent && x.IsHead);
                if (bossPlace != null)
                {
                    var profile = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == bossPlace.EmployeeId);
                    if (profile != null)
                    {
                        boss = profile.FullNameaEnglish;
                    }
                }

            }
            else if(placement != null && !placement.IsHead)
            {
                var bossPlace = db.Placements.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId /*&& x.BranchId == placement.BranchId*/ && x.isCurrent && x.IsHead);
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
        [AuthorizeRoles(HrmConstants.HRPlacementAdmin, HrmConstants.HRPlacementUser, HrmConstants.HRAdmin)]
        public ActionResult PlacmentTemporaries_Read([DataSourceRequest]DataSourceRequest request)
        {
            //List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            //var placements = db.Placements.Where(x => x.isCurrent || !x.IsApprove && mybraches.Contains(x.BranchId.ToString())).Where(x=>x.IsTemporary).ToList();

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
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
            IQueryable<Placement> placements = db.Placements.Where(x => x.isCurrent || !x.IsApprove)
                                                            .Where(x => x.IsTemporary)
                                                            .Where(x => mybraches.Contains(x.BranchId));
            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                List<int> myBusinessUnits = UserInformationHelper.GetUserBusinessUnits(User.Identity.Name);
                placements = placements.Where(x => myBusinessUnits.Contains(x.BusinessUnitId));
            }
            DataSourceResult result = placements.ToDataSourceResult(request, placement => new
            {
                PlacementId = placement.PlacementId,
                EmployeeId = placement.EmployeeId,
                ImmediateSupervisor = placement.ImmediateSupervisor,
                IsHead = placement.IsHead,
                IsTemporary = placement.IsTemporary,
                AssignmentDate = placement.AssignmentDate,
                OrgStructureId = placement.OrgStructureId,
                ParentOrgStructureId = placement.ParentOrgStructureId,
                LookupId = placement.LookupId,
                MappingId = placement.MappingId,
                JobTitleName = GetTitleName(placement.LookupId),
                ReportsToMappingId = placement.ReportsToMappingId,
                RankId = placement.RankId,
                StepId = placement.StepId,
                BranchId = placement.BranchId,
                HeadId = placement.HeadId,
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
                //AssignmentDateAmharic = placement.AssignmentDateAmharic,
                AssignmentDateAmharic = placement.AssignmentDateAmharic + " (" + placement.AssignmentDate.ToString("MMMM dd, yyyy") + ")",
                EmployeeCode = placement.EmployeeCode,
                Boss = GetBoss(placement.EmployeeId),
                EmployementType = placement.EmployementType,
                IsPend = placement.IsPend,
                BusinessUnitId = placement.BusinessUnitId,
                IsApprove = placement.IsApprove,
                FullName = GetFullName(placement.EmployeeId),
                HireDate = HrHelper.GetHireDate(placement.EmployeeId),
                Age = EmployeeHelper.GetAge(placement.EmployeeId)
            });

            return Json(result);
        }
        [AuthorizeRoles(HrmConstants.HRPlacementUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PlacmentTemporaries_Create([DataSourceRequest]DataSourceRequest request, Placement placement)
        {
            ModelState.Clear();
            var orgStructure = db.MainCompanyStructures.Find(placement.OrgStructureId);
            if (orgStructure != null)
            {
                placement.ParentOrgStructureId = orgStructure.Parent;
            }
            #region validation
            int index = placement.AssignmentDateAmharic.IndexOf(" (");
            placement.AssignmentDateAmharic = (index > 0) ? placement.AssignmentDateAmharic.Substring(0, index) : placement.AssignmentDateAmharic;
            if (placement.AssignmentDateAmharic != null)
            {
                var splitDate = placement.AssignmentDateAmharic.Split('/');
                placement.AssignmentDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }

            var checkNotApproved = db.Placements.FirstOrDefault(x => x.EmployeeId == placement.EmployeeId && !x.IsApprove);
            if (checkNotApproved != null)
            {
                ModelState.AddModelError("", "This employee already have not approved placement");
                return Json(new[] { placement }.ToDataSourceResult(request, ModelState));

            }

            var checkAssignment = db.Placements.FirstOrDefault(x => x.EmployeeId == placement.EmployeeId);
            if (checkAssignment != null && placement.AssignmentType == AssignmentType.NEW)
            {
                ModelState.AddModelError("", "You can not assign this employee as a new assignment!!");
                return Json(new[] { placement }.ToDataSourceResult(request, ModelState));

            }
            var checkexistance =
                    db.Placements.FirstOrDefault(x => x.EmployeeId == placement.EmployeeId && x.OrgStructureId == placement.OrgStructureId &&
                            x.LookupId == placement.LookupId && x.BranchId == placement.BranchId &&
                            x.RankId == placement.RankId && x.StepId == placement.StepId &&
                            x.Salary == placement.Salary &&
                            x.AssignmentType == placement.AssignmentType);

            if (checkexistance != null)
            {
                ModelState.AddModelError("", "This employee is already assigened ");
                return Json(new[] { placement }.ToDataSourceResult(request, ModelState));

            }
            //if (placement.AssignmentDate > DateTime.Now)
            //{
            //    ModelState.AddModelError("", "Assigment date should be less or equal to the current date!!");
            //    return Json(new[] { placement }.ToDataSourceResult(request, ModelState));

            //}

            var workunitHead = db.Placements.FirstOrDefault(x => x.BranchId == placement.BranchId 
                          && x.OrgStructureId == placement.OrgStructureId
                         && x.BusinessUnitId == placement.BusinessUnitId
                         && x.IsHead && 
            x.PlacementId !=placement.PlacementId);
            if (placement.IsHead && workunitHead != null)
            {
                ModelState.AddModelError("", "This Work unit already have Head");
                return Json(new[] { placement }.ToDataSourceResult(request, ModelState));

            }

            var emp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == placement.EmployeeId);
            if (emp != null)
            {
                placement.HireDate = emp.HiredDateAmharic;
                if (placement.AssignmentDate < emp.HiredDate)
                {
                    ModelState.AddModelError("", "assignment date can't less than hire date");
                    return Json(new[] { placement }.ToDataSourceResult(request, ModelState));

                }

                //emp.Status =placement.EmployementType;
                //db.Entry(emp).State = EntityState.Modified;
                //db.SaveChanges();

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
                        return Json(new[] { placement }.ToDataSourceResult(request, ModelState));

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
                            return Json(new[] { placement }.ToDataSourceResult(request, ModelState));

                        }
                    }
                }
            }

            #endregion
            //var contractEmp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == placement.EmployeeId);
            //if (contractEmp != null && contractEmp.Status == EmployeeStatus.CONTRACT)
            if (placement.EmployementType == EmployeeStatus.CONTRACT)
            {
                var contractLast = db.Contract.Where(x => x.EmployeeId == placement.EmployeeId).OrderByDescending(x => x.To).ToList().FirstOrDefault();
                if (contractLast != null && contractLast.To <= placement.AssignmentDate)
                {
                    ModelState.AddModelError("", "First Extend Contract date for this employee");
                    return Json(new[] { placement }.ToDataSourceResult(request, ModelState));

                }
            }
            //var freelancerEmp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == placement.EmployeeId);
            //if (freelancerEmp != null && freelancerEmp.Status == EmployeeStatus.FREELANCER)
            if ( placement.EmployementType == EmployeeStatus.FREELANCER)
            {
                var freelancerLast = db.Freelancers.Where(x => x.EmployeeId == placement.EmployeeId).OrderByDescending(x => x.To).ToList().FirstOrDefault();
                if (freelancerLast != null && freelancerLast.To <= placement.AssignmentDate)
                {
                    ModelState.AddModelError("", "First Extend Freelancer date for this employee ");
                    return Json(new[] { placement }.ToDataSourceResult(request, ModelState));

                }
            }
            if (placement.EmployementType == EmployeeStatus.NOTSELECTED)
            {
                ModelState.AddModelError("", "Please Select Employement status Type");
                return Json(new[] { placement }.ToDataSourceResult(request, ModelState));

            }
            if (placement.AssignmentType == AssignmentType.NOTSELECTED)
            {
                ModelState.AddModelError("", "Please Select Employee Assignment Type");
                return Json(new[] { placement }.ToDataSourceResult(request, ModelState));

            }
            if (placement.ResponsibilityLevel == ResponsibilityLevel.NOTSELECTED)
            {
                ModelState.AddModelError("", "Please Select Employee Responsibility Level");
                return Json(new[] { placement }.ToDataSourceResult(request, ModelState));

            }
            if (placement.HeadId == null
                || placement.HeadId == 0
                || placement.EmployeeId == placement.HeadId)
            {
                placement.HeadId = EmployeeHelper.GetSupervisorEmployeeId(placement.EmployeeId, placement.OrgStructureId, placement.BranchId, placement.BusinessUnitId);
                if (placement.HeadId == 0 || placement.HeadId == null)
                {
                    ModelState.AddModelError("", "Please Select Your Corresponding  Head the system doesn't found  !");
                    return Json(new[] { placement }.ToDataSourceResult(request, ModelState));

                }
            }
            if (placement.EmployeeId == placement.HeadId)
            {
                ModelState.AddModelError("", "Please Select your Corresponding  Head!");
                return Json(new[] { placement }.ToDataSourceResult(request, ModelState));

            }
            if (placement.Salary <= 0)
            {
                // Retrieve the salary for the given rank and step setting
                var salary = db.SalariesJobTitles
                    .FirstOrDefault(s => s.RankId == placement.RankId
                                      && s.StepId == placement.StepId);
                // Handle case where no salary setting  is found
                if (salary == null)
                {
                    ModelState.AddModelError("", "The salary setting for the specified rank and step is not configured. Please update the salary settings.");
                    return Json(new[] { placement }.ToDataSourceResult(request, ModelState));
                }

                // Set the salary if a valid one is found
                if (salary.Salary > 0)
                {
                    placement.Salary = salary.Salary;
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
                    HeadId = placement.HeadId,
                    HireDate = placement.HireDate,
                    ImmediateSupervisor = placement.ImmediateSupervisor,
                    IsHead = placement.IsHead,
                    AssignmentDate = placement.AssignmentDate,
                    MappingId = 0,
                    RankId = placement.RankId,
                    StepId = placement.StepId,
                    LookupId = placement.LookupId,
                    OrgStructureId = placement.OrgStructureId,
                    ParentOrgStructureId = placement.ParentOrgStructureId,
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
                    IsTemporary = placement.IsTemporary? placement.IsTemporary:true,
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
                    person.Status = entity.EmployementType;
                    person.OrgBranchName = entity.BranchId.ToString();

                    db.Entry(person).State = EntityState.Modified;
                    db.SaveChanges();
                }
                if (entity.IsHead)
                {
                    var subordinateEmployeeIds = EmployeeHelper.GetSubordinates_FilteredByBranchAlso(entity.EmployeeId);
                    foreach (var subEmployeeId in subordinateEmployeeIds)
                    {
                        var changeHead = db.Placements
                                           .FirstOrDefault(x => x.EmployeeId == subEmployeeId
                                                             && x.isCurrent);

                        if (changeHead != null)
                        {
                            changeHead.HeadId = entity.EmployeeId;
                            db.Entry(changeHead).State = EntityState.Modified;

                        }

                    }
                    db.SaveChanges();
                }
                else
                {
                    //var subordinateEmployeeIds = EmployeeHelper.GetSubordinates_FilteredByBranchAlso(entity.EmployeeId).Where(x => x != entity.EmployeeId);
                    var placedEmployees = db.Placements.Where(x => x.HeadId == entity.EmployeeId && x.isCurrent).ToList();
                    foreach (var placedEmployee in placedEmployees)
                    {

                        var headId = EmployeeHelper.GetSupervisor(placedEmployee.EmployeeId);
                        placedEmployee.HeadId = headId;
                        db.Entry(placedEmployee).State = EntityState.Modified;
                    }

                    db.SaveChanges();
                }
                PerapreEmployeeAllowance(placement.EmployeeId, placement.LookupId);

                placement.PlacementId = entity.PlacementId;
                // GeneratePdf(placement);
                UserInformationHelper.UpdateDefalutBranchAndBusinessunit(placement.EmployeeId, placement.BranchId, placement.BusinessUnitId);


            }

            return Json(new[] { placement }.ToDataSourceResult(request, ModelState));
        }
      
        //[AuthorizeRoles(HrmConstants.HRPlacementUser)]
        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult PlacmentTemporaries_Update([DataSourceRequest]DataSourceRequest request, Placement placement)
        //{
        //    ModelState.Clear();
        //    var orgStructure = db.MainCompanyStructures.Find(placement.OrgStructureId);
        //    if (orgStructure != null)
        //    {
        //        placement.ParentOrgStructureId = orgStructure.Parent;
        //    }
        //    if (placement.AssignmentDateAmharic != null)
        //    {
        //        var splitDate = placement.AssignmentDateAmharic.Split('/');
        //        placement.AssignmentDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
        //    }

        //    var emp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == placement.EmployeeId);
        //    if (emp != null)
        //    {
        //        if (placement.AssignmentDate.Date < emp.HiredDate.Date)
        //        {
        //            ModelState.AddModelError("", "assignment date can't less than hire date");
        //        }
        //    }
        //    if (placement.AssignmentDate > DateTime.Now)
        //    {

        //        ModelState.AddModelError("", "Assigment date should be less or equal to the current date!!");

        //    }
        //    var pp = db.Placements.FirstOrDefault(x => x.EmployeeId == placement.EmployeeId && x.isCurrent && x.PlacementId != placement.PlacementId);
        //    if (pp != null && placement.isCurrent)
        //    {
        //        ModelState.AddModelError("", "this employee have other current placement ");
        //    }

        //    var workunitHead = db.Placements.FirstOrDefault(x => x.PlacementId != placement.PlacementId && x.BranchId == placement.BranchId && x.OrgStructureId == placement.OrgStructureId && x.IsHead && x.isCurrent);
        //    if (placement.IsHead && workunitHead != null)
        //    {
        //        ModelState.AddModelError("", "This Workunit already have Head");
        //    }
        //    var profile = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == placement.EmployeeId);           
        //    if ( placement.EmployementType == EmployeeStatus.CONTRACT)
        //    {
        //        var contractLast = db.Contract.Where(x => x.EmployeeId == placement.EmployeeId).OrderByDescending(x => x.To).ToList().FirstOrDefault();
        //        if (contractLast != null && contractLast.To <= placement.AssignmentDate)
        //        {
        //            ModelState.AddModelError("", "First Extend Contract date for this employee ");
        //        }
        //    }
        //    if ( placement.EmployementType == EmployeeStatus.FREELANCER)
        //    {
        //        var freelancerLast = db.Freelancers.Where(x => x.EmployeeId == placement.EmployeeId).OrderByDescending(x => x.To).ToList().FirstOrDefault();
        //        if (freelancerLast != null && freelancerLast.To <= placement.AssignmentDate)
        //        {
        //            ModelState.AddModelError("", "First Extend Freelancer date for this employee ");
        //        }
        //    }
        //    if (placement.EmployementType == EmployeeStatus.NOTSELECTED)
        //    {
        //        ModelState.AddModelError("", "Please Select Employement status Type");

        //    }
        //    if (placement.AssignmentType == AssignmentType.NOTSELECTED)
        //    {
        //        ModelState.AddModelError("", "Please Select Employee Assignment Type");

        //    }
        //    if (placement.ResponsibilityLevel == ResponsibilityLevel.NOTSELECTED)
        //    {
        //        ModelState.AddModelError("", "Please Select Employee Responsibility Level");

        //    }

        //    if (ModelState.IsValid)
        //    {
        //        var place = db.Placements.Find(placement.PlacementId);
        //        if (place != null)
        //        {
        //            place.PlacementId = placement.PlacementId;
        //            place.IsHead = placement.IsHead;
        //            place.IsTemporary = placement.IsTemporary;
        //            place.EmployementType = placement.EmployementType;
        //            place.AssignmentDate = placement.AssignmentDate;
        //            place.BranchId = placement.BranchId;
        //            place.HeadId = placement.HeadId;
        //            place.JobTitleName = GetTitleName(placement.LookupId);
        //            place.OrgStructureId = placement.OrgStructureId;
        //            place.ParentOrgStructureId = placement.ParentOrgStructureId;
        //            place.ReportsTo = placement.ReportsTo;
        //            place.RankId = placement.RankId;
        //            place.StepId = placement.StepId;
        //            place.LookupId = placement.LookupId;
        //            place.Department = placement.ReportsTo;
        //            place.Salary = placement.Salary;
        //            place.AssignmentType = placement.AssignmentType;
        //            place.ResponsibilityLevel = placement.ResponsibilityLevel;
        //            place.Remark = placement.Remark;
        //            place.BusinessUnitId = placement.BusinessUnitId;
        //            place.DateCreated = placement.DateCreated;
        //            place.LastModified = DateTime.Now;
        //            place.ModifiedBy = User.Identity.Name;
        //            place.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
        //            place.AssignmentDateAmharic = placement.AssignmentDateAmharic;
        //            place.EmployeeCode = inputs.GetEmployees().FirstOrDefault(x => x.PersonId == placement.EmployeeId)?.EmployeeCode;

        //            db.Entry(place).State = EntityState.Modified;
        //            db.SaveChanges();

        //            var employee = db.Person_Employee.FirstOrDefault(p => p.EmployeeId == place.EmployeeId);

        //            if (employee != null)
        //            {
        //                employee.OrgBranchName = placement.BranchId.ToString();
        //                employee.ModifiedBy = User.Identity.Name;
        //                employee.LastModified = DateTime.Now;
        //                employee.BranchId = placement.BranchId;
        //                employee.Status = placement.EmployementType;

        //                db.Entry(employee).State = EntityState.Modified;
        //                db.SaveChanges();
        //            }

        //            PerapreEmployeeAllowance(place.EmployeeId, placement.LookupId);

        //            #region operation log
        //            var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == place.EmployeeId);
        //            if (person != null)
        //            {
        //                UserHelper.OperationLog(Request.UserHostAddress, "Employee Placement", "Success", User.Identity.Name, "Placement Info of " + person.EmployeeCode + " is Updated By  " + User.Identity.Name, Modules.HRM);
        //            }
        //            #endregion
        //            //GeneratePdf(placement);
        //        }
        //    }
        //    return Json(new[] { placement }.ToDataSourceResult(request, ModelState));
        //}

        [AuthorizeRoles(HrmConstants.HRPlacementUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PlacmentTemporaries_Update([DataSourceRequest]DataSourceRequest request, Placement placement)
        {
            ModelState.Clear();
            var orgStructure = db.MainCompanyStructures.Find(placement.OrgStructureId);
            if (orgStructure != null)
            {
                placement.ParentOrgStructureId = orgStructure.Parent;
            }
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
                    ModelState.AddModelError("", "assignment date can't less than hire date");
                    return Json(new[] { placement }.ToDataSourceResult(request, ModelState));

                }
            }
            //if (placement.AssignmentDate > DateTime.Now)
            //{

            //    ModelState.AddModelError("", "Assigment date should be less or equal to the current date!!");
            //    return Json(new[] { placement }.ToDataSourceResult(request, ModelState));


            //}
            var pp = db.Placements.FirstOrDefault(x => x.EmployeeId == placement.EmployeeId && x.isCurrent && x.PlacementId != placement.PlacementId);
            if (pp != null && placement.isCurrent)
            {
                ModelState.AddModelError("", "this employee have other current placement ");
                return Json(new[] { placement }.ToDataSourceResult(request, ModelState));

            }

            var workunitHead = db.Placements.FirstOrDefault(x => x.PlacementId != placement.PlacementId && x.BranchId == placement.BranchId && x.OrgStructureId == placement.OrgStructureId && x.IsHead && x.isCurrent);
            if (placement.IsHead && workunitHead != null)
            {
                ModelState.AddModelError("", "This Workunit already have Head");
                return Json(new[] { placement }.ToDataSourceResult(request, ModelState));

            }
            var profile = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == placement.EmployeeId);
            if (placement.EmployementType == EmployeeStatus.CONTRACT)
            {
                var contractLast = db.Contract.Where(x => x.EmployeeId == placement.EmployeeId).OrderByDescending(x => x.To).ToList().FirstOrDefault();
                if (contractLast != null && contractLast.To <= placement.AssignmentDate)
                {
                    ModelState.AddModelError("", "First Extend Contract date for this employee ");
                    return Json(new[] { placement }.ToDataSourceResult(request, ModelState));

                }
            }
            if (placement.EmployementType == EmployeeStatus.FREELANCER)
            {
                var freelancerLast = db.Freelancers.Where(x => x.EmployeeId == placement.EmployeeId).OrderByDescending(x => x.To).ToList().FirstOrDefault();
                if (freelancerLast != null && freelancerLast.To <= placement.AssignmentDate)
                {
                    ModelState.AddModelError("", "First Extend Freelancer date for this employee ");
                    return Json(new[] { placement }.ToDataSourceResult(request, ModelState));

                }
            }
            if (placement.EmployementType == EmployeeStatus.NOTSELECTED)
            {
                ModelState.AddModelError("", "Please Select Employement status Type");
                return Json(new[] { placement }.ToDataSourceResult(request, ModelState));

            }
            if (placement.AssignmentType == AssignmentType.NOTSELECTED)
            {
                ModelState.AddModelError("", "Please Select Employee Assignment Type");
                return Json(new[] { placement }.ToDataSourceResult(request, ModelState));

            }
            if (placement.ResponsibilityLevel == ResponsibilityLevel.NOTSELECTED)
            {
                ModelState.AddModelError("", "Please Select Employee Responsibility Level");
                return Json(new[] { placement }.ToDataSourceResult(request, ModelState));


            }

            if (placement.HeadId == null
                || placement.HeadId == 0
                || placement.EmployeeId == placement.HeadId)
            {
                // placement.HeadId = EmployeeHelper.GetSupervisor(placement.EmployeeId);
                placement.HeadId = EmployeeHelper.GetSupervisorEmployeeId(placement.EmployeeId, placement.OrgStructureId, placement.BranchId, placement.BusinessUnitId);
                if (placement.HeadId == 0 || placement.HeadId == null
                    || placement.EmployeeId == placement.HeadId)
                {
                    ModelState.AddModelError("", "Please Select Your Corresponding  Head the system doesn't found  !");
                    return Json(new[] { placement }.ToDataSourceResult(request, ModelState));

                }
            }
           
            if (placement.Salary <= 0)
            {
                // Retrieve the salary for the given rank and step setting
                var salary = db.SalariesJobTitles
                    .FirstOrDefault(s => s.RankId == placement.RankId
                                      && s.StepId == placement.StepId);
                // Handle case where no salary setting  is found
                if (salary == null)
                {
                    ModelState.AddModelError("", "The salary setting for the specified rank and step is not configured. Please update the salary settings.");
                    return Json(new[] { placement }.ToDataSourceResult(request, ModelState));
                }

                // Set the salary if a valid one is found
                if (salary.Salary > 0)
                {
                    placement.Salary = salary.Salary;
                }
            }

            if (ModelState.IsValid)
            {
                placement.PreparedByEmpId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                //placement.Salary = HrHelper.GetSalaryFromSalarySetting(placement.StepId, placement.RankId, placement.LookupId);
                placement.JobTitleName = GetTitleName(placement.LookupId);
                placement.LastModified = DateTime.Now;
                placement.ModifiedBy = User.Identity.Name;
                placement.EmployeeCode = inputs.GetEmployees().FirstOrDefault(x => x.PersonId == placement.EmployeeId)?.EmployeeCode;
                placement.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);

                var entity = GetMappedObject(placement);
                #region comments 
                //var place = new Placement();
                //place.PlacementId = placement.PlacementId;
                //place.HeadId = placement.HeadId;
                //place.IsHead = placement.IsHead;
                //place.EmployementType = placement.EmployementType;
                //place.AssignmentDate = placement.AssignmentDate;
                //place.BranchId = placement.BranchId;
                //place.JobTitleName = GetTitleName(placement.LookupId);
                //place.OrgStructureId = placement.OrgStructureId;
                //place.ParentOrgStructureId = placement.ParentOrgStructureId;
                //place.ReportsTo = placement.ReportsTo;
                //place.RankId = placement.RankId;
                //place.StepId = placement.StepId;
                //place.LookupId = placement.LookupId;
                //place.Department = placement.ReportsTo;
                //place.Salary = placement.Salary;
                //place.AssignmentType = placement.AssignmentType;
                //place.ResponsibilityLevel = placement.ResponsibilityLevel;
                //place.Remark = placement.Remark;
                //place.BusinessUnitId = placement.BusinessUnitId;
                //place.DateCreated = placement.DateCreated;
                //place.LastModified = DateTime.Now;
                //place.ModifiedBy = User.Identity.Name;
                //place.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                //place.AssignmentDateAmharic = placement.AssignmentDateAmharic;
                //place.EmployeeCode = inputs.GetEmployees().FirstOrDefault(x => x.PersonId == placement.EmployeeId)?.EmployeeCode;
                #endregion
                db.Placements.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                var employee = db.Person_Employee.FirstOrDefault(p => p.EmployeeId == entity.EmployeeId);

                if (employee != null)
                {
                    employee.OrgBranchName = placement.BranchId.ToString();
                    employee.ModifiedBy = User.Identity.Name;
                    employee.LastModified = DateTime.Now;
                    employee.BranchId = placement.BranchId;
                    employee.Status = placement.EmployementType;

                    db.Entry(employee).State = EntityState.Modified;
                    db.SaveChanges();
                }

                PerapreEmployeeAllowance(entity.EmployeeId, placement.LookupId);
                UserInformationHelper.UpdateDefalutBranchAndBusinessunit(placement.EmployeeId, placement.BranchId, placement.BusinessUnitId);

                #region operation log
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                if (person != null)
                {
                    UserHelper.OperationLog(Request.UserHostAddress, "Employee Placement", "Success", User.Identity.Name, "Placement Info of " + person.EmployeeCode + " is Updated By  " + User.Identity.Name, Modules.HRM);
                }
                #endregion
                //GeneratePdf(placement);

            }
            return Json(new[] { placement }.ToDataSourceResult(request, ModelState));
        }


        [AuthorizeRoles(HrmConstants.HRPlacementAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PlacmentTemporaries_Destroy([DataSourceRequest]DataSourceRequest request, Placement placement)
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
        private Placement GetMappedObject(Placement placement)
        {
            var entity = new Placement
            {
                PlacementId = placement.PlacementId,
                EmployeeId = placement.EmployeeId,
                HeadId = placement.HeadId,
                ImmediateSupervisor = placement.ImmediateSupervisor,
                IsHead = placement.IsHead,
                AssignmentDate = placement.AssignmentDate,
                AssignmentDateAmharic = placement.AssignmentDateAmharic,
                MappingId = placement.MappingId,
                RankId = placement.RankId,
                StepId = placement.StepId,
                LookupId = placement.LookupId,
                OrgStructureId = placement.OrgStructureId,
                ParentOrgStructureId = placement.ParentOrgStructureId,
                Salary = placement.Salary,
                ReportsTo = placement.ReportsTo,
                JobTitleName = placement.JobTitleName,
                Department = placement.ReportsTo,
                ReportsToMappingId = placement.ReportsToMappingId,
                AssignmentType = placement.AssignmentType,
                ResponsibilityLevel = placement.ResponsibilityLevel,
                BranchId = placement.BranchId,
                BusinessUnitId = placement.BusinessUnitId,
                RankName = placement.RankName,
                StepName = placement.StepName,
                Remark = placement.Remark,
                DateCreated = placement.DateCreated,
                LastModified = placement.LastModified,
                CreatedBy = placement.CreatedBy,
                ModifiedBy = placement.ModifiedBy,
                isCurrent = placement.isCurrent, // this will be true after approval
                IsApprove = placement.IsApprove,
                IsPend = placement.IsPend,
                IsTemporary = placement.IsTemporary,
                Index = placement.Index,
                EmployementType = placement.EmployementType,
                EmployeeCode = placement.EmployeeCode,
                OrgBranchName = placement.OrgBranchName,
                ApprovedByEmpId = placement.PreparedByEmpId,
                HaveFixedAsset = placement.HaveFixedAsset,
                PreparedByEmpId = placement.PreparedByEmpId

            };
            return entity;
        }

        public ActionResult EmployeeSwapping(int employeeId1, int employeeId2, DateTime swapDate)
        {
            var swapDateAmh = "";
            if (swapDate != null)
            {
               swapDateAmh = XAPI.EthiopicDateTime.GetEthiopicDate(swapDate.Day, swapDate.Month, swapDate.Year);
            }
            var employeePlacement1 = db.Placements.Where(x => x.isCurrent && x.EmployeeId == employeeId1).FirstOrDefault();
            
            if (employeePlacement1 != null)
            {
                var employeePlacement2 = db.Placements
                    .Where(x => x.isCurrent && x.EmployeeId == employeeId2)
                    .FirstOrDefault();

                if (employeePlacement2 != null)
                {
                    employeePlacement1.EmployeeId = employeeId2;
                    employeePlacement1.AssignmentDate = swapDate;
                    employeePlacement1.AssignmentDateAmharic = swapDateAmh;
                    employeePlacement2.EmployeeId = employeeId1;
                    employeePlacement2.AssignmentDate = swapDate;
                    employeePlacement2.AssignmentDateAmharic = swapDateAmh;

                    db.Entry(employeePlacement1).State = EntityState.Modified;
                    db.Entry(employeePlacement2).State = EntityState.Modified;

                    db.SaveChanges();

                    var formattedData = new
                    {
                        Message = "You successfully swapped the placement.",
                        Status = "OK"
                    };

                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

                var formattedData1 = new
                {
                    Message = "The 2nd Employee has no current placement.",
                    Status = "OK"
                };

                return Json(formattedData1, JsonRequestBehavior.AllowGet);
            }
            var formattedData2 = new
            {
                Message = "The 1st Employee  has no Current  placement.",
                Status = "OK"
            };
            return Json(formattedData2, JsonRequestBehavior.AllowGet);
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
            var placement = db.Placements.Find(id);

            if (placement != null)
            {
                var employeeId = placement.EmployeeId;
                var branchId = placement.BranchId;
                var user = User.Identity.Name;
                placement.IsApprove = true;
                placement.IsTemporary = true;
                placement.isCurrent = true;
                placement.ApprovedByEmpId = UserInformationHelper.GetEmployeeId(user);
                db.Entry(placement).State = EntityState.Modified;
                db.SaveChanges();
                // Detach placement entity to prevent multiple tracking
                db.Entry(placement).State = EntityState.Detached;
                var placementsToUpdate = db.Placements
                    .Where(x => x.isCurrent && x.EmployeeId == employeeId
                    && x.PlacementId != id)
                    .ToList();
                foreach (var p in placementsToUpdate)
                {
                    // Detach entity before modifying to prevent multiple tracking
                    db.Entry(p).State = EntityState.Detached;

                    p.MappingId = 0;
                    p.JobTitleName = GetTitleName(p.LookupId);
                    p.ReportsToMappingId = 0;
                    p.LastModified = DateTime.Now;
                    p.ModifiedBy = user;
                    p.isCurrent = false;
                    p.IsTemporary = false;
                    p.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(user);
                    p.ApprovedByEmpId = p.EmployeeId;
                    // Attach modified entity to current context
                    db.Entry(p).State = EntityState.Modified;
                }

                var employee = db.Person_Employee.FirstOrDefault(p => p.EmployeeId == employeeId);
                if (employee != null)
                {
                    // Detach employee entity before modifying
                    db.Entry(employee).State = EntityState.Detached;

                    employee.IsActive = true;
                    employee.IsPlaced = true;
                    employee.OrgBranchName = branchId.ToString();
                    employee.ModifiedBy = user;
                    employee.LastModified = DateTime.Now;
                    employee.BranchId = branchId;
                    // Attach modified employee entity to current context
                    db.Entry(employee).State = EntityState.Modified;
                }
                if (placement.IsHead)
                {
                    // employees under him  which are not have assigned   head or those don't have the right head 
                    var subordinateEmployeeIds = EmployeeHelper.GetSubordinates_FilteredByBranchAlso(placement.EmployeeId);
                    subordinateEmployeeIds.Remove(placement.EmployeeId);

                    if (subordinateEmployeeIds.Any())
                    {
                        var changeHeads = db.Placements
                                              .Where(x => subordinateEmployeeIds.Contains(x.EmployeeId)
                                                          && x.HeadId != placement.EmployeeId
                                                          && x.isCurrent);

                        foreach (var changeHead in changeHeads)
                        {
                            //if the emplyee is not  Assigned correct  headId or assign the Right one
                            //var changeHead = db.Placements
                            //                   .FirstOrDefault(x => x.EmployeeId == subEmployeeId
                            //                   &&x.HeadId !=placement.EmployeeId&& x.isCurrent);

                            //if (changeHead != null)
                            //{
                            changeHead.HeadId = placement.EmployeeId;
                            db.Entry(changeHead).State = EntityState.Modified;

                            //}

                        }
                        //db.SaveChanges();
                        //var employees = db.Placements.Where(x => x.he == placement.HeadId
                        //                                   && x.OrgStructureId != placement.OrgStructureId
                        //                                   && x.isCurrent).ToList();
                        //foreach (var employeep in employees)
                        //{
                        //    var headId = EmployeeHelper.GetSupervisorEmployeeId(employeep.EmployeeId, employeep.OrgStructureId, employeep.BranchId, employeep.BusinessUnitId);
                        //    employeep.HeadId = headId;
                        //    db.Entry(employee).State = EntityState.Modified;
                        //}
                    }


                    // db.SaveChanges();
                }

                db.SaveChanges();

                var formattedData = new
                {
                    Message = "You successfully approved this placement.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            var formattedData1 = new
            {
                Message = "This placement is not approved.",
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
        public ActionResult UploadDocument(string employeeId, HttpPostedFileBase file, string txtValue)
        {
            try
            {
                if (file.ContentLength > 0)
                {

                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.DOCUMENT), HRMInputs.EmployeeId.ToString() + "-" + txtValue + "-" + fileName);

                    var mainPath = Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.DOCUMENT));

                    if (!Directory.Exists(mainPath))
                    {
                        Directory.CreateDirectory(mainPath);
                    }
                    if (!System.IO.File.Exists(path))
                    {
                        file.SaveAs(path);

                        var entity = new Attachment
                        {

                            Reference = employeeId,
                            Description = HRMInputs.EmployeeId.ToString() + "-" + txtValue + "-" + fileName,
                            FileName = txtValue,
                            FileType = file.ContentType,
                            ReferenceType = AttachmentType.DOCUMENT,
                            Index = 0,
                            URL = path,
                            Remark = employeeId
                        };
                        db.Attachments.Add(entity);

                        db.SaveChanges();
                        ViewBag.Message = "Upload successful";
                    }

                }
                else
                {

                    ViewBag.Message = "The file is existed";

                }
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Message = "Upload failed";
                return RedirectToAction("Upload");
            }
        }
        public ActionResult UploadPdfToEmployeeArchive(int employeeId, string fileName, string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName))
                {


                    string mainPath = Path.Combine(filePath, fileName);

                    if (!Directory.Exists(mainPath))
                    {
                        Directory.CreateDirectory(mainPath);
                    }
                    if (!System.IO.File.Exists(filePath))
                    {

                        var entity = new Attachment
                        {

                            Reference = employeeId.ToString(),
                            Description = HRMInputs.EmployeeId.ToString() + "-" + fileName ,
                            FileName = fileName,
                            FileType = "Pdf",
                            ReferenceType = AttachmentType.DOCUMENT,
                            Index = 0,
                            URL = filePath,
                            Remark = employeeId.ToString()
                        };
                        db.Attachments.Add(entity);

                        db.SaveChanges();
                        ViewBag.Message = "Upload successful";
                    }

                }
                else
                {

                    ViewBag.Message = "The file is existed";

                }
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Message = "Upload failed";
                return RedirectToAction("Upload");
            }
        }
        public ActionResult DownloadAttachment(int attachmentId)
        {
            var record = db.Attachments.Find(attachmentId);

            return File(record.URL, "application/force-download", Path.GetFileName(record.URL));
        }
        //public  void GeneratePdf(Placement placement)
        //{
           

        //    // Create a PdfWriter object to write to a file
        //    var employee = db.Person_Employee.Find(placement.EmployeeId);
        //    string FileDirectory = "D:/Projects/ExceedERP-HR/ExceedERP.Web/ContentFiles/PDFFIle/";
        //    string fileName = $"Employee_{employee.EmployeeCode}_{employee.EmployeeId}_{employee.FirstNameEnglish}_{employee.LastNameEnglish}_{placement.AssignmentType.ToString()}.pdf";
        //    string filePath = Path.Combine(FileDirectory, fileName);
        //    if (!Directory.Exists(filePath))
        //    {
        //        Directory.CreateDirectory(filePath);
        //    }
        //    using (var writer = new PdfWriter(filePath))
        //    {
        //        // Create a PdfDocument object
        //        using (var pdf = new PdfDocument(writer))
        //        {
        //            // Create a Document object
        //            var document = new Document(pdf);

        //            // Add content to the document
        //            document.Add(new Paragraph($"Employee Data for EmployeeId: {employee.EmployeeId}"));
        //            document.Add(new Paragraph($"Name: {employee.FirstName} {employee.LastName}"));
        //            // Add other employee details as needed
        //        }
        //    }
        //    UploadPdfToEmployeeArchive(employee.EmployeeId, fileName, filePath);
        //    //Console.WriteLine("PDF generated successfully.");
        //}
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}