using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
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

    [AuthorizeRoles(HrmConstants.HRPlacementAdmin, HrmConstants.HRPlacementUser, HrmConstants.HRAdmin)]
    public class EmployeePlacmentWithJController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static decimal SalaryAmount { get; set; }
        public static int Rankid { get; set; }
        public static int Stepid { get; set; }
        public static int DepartmentId { get; set; }
        //public static DateTime AssigmentDate { get; set; }
        //public static string AssigmentDateAmharic { get; set; }
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

            var branches = db.Branch;
            ViewData["branches"] = branches;
            var workunits = db.MainCompanyStructures.ToList();
            ViewData["workunits"] = workunits;
            ViewData["EmployeeCode"] = db.Person_Employee.ToList();
        }
        public ActionResult Refresh()
        {
            var allPlacments = db.Placements.Where(x => x.isCurrent).ToList();
            decimal salary = 0;
            foreach (var p in allPlacments)
            {
                p.Salary = GetSalaryAmount(p.RankId, p.StepId, p.LookupId);
                p.ModifiedBy = User.Identity.Name;
                p.LastModified = DateTime.Now.Date;
                db.Placements.Attach(p);
                db.Entry(p).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");

        }
        public void SetDate(string name, string value)
        {
            //if (name == "AssignmentDateAmharic")
            //{
            //    AssigmentDateAmharic = value;
            //    var splitDate = AssigmentDateAmharic.Split('/');
            //    AssigmentDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            //}

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
        public string GetBoss(int empId)
        {
            var boss = "";
            var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == empId && x.isCurrent && !x.IsHead);
            if (placement != null)
            {
                var bossPlace = db.Placements.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId && x.BranchId == placement.BranchId && x.isCurrent && x.IsHead);
                if (bossPlace != null)
                {
                    var profile = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == bossPlace.EmployeeId && x.IsActive);
                    if (profile != null)
                    {
                        boss = profile.FullNameaEnglish;
                    }
                }
                else
                {
                    var bossPlaceOtherBranch = db.Placements.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId && x.isCurrent && x.IsHead);
                    if (bossPlaceOtherBranch != null)
                    {
                        var profile = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == bossPlaceOtherBranch.EmployeeId && x.IsActive);
                        if (profile != null)
                        {
                            boss = profile.FullNameaEnglish;
                        }
                    }
                }
            }
            else
            {
                var placementHead = db.Placements.FirstOrDefault(x => x.EmployeeId == empId && x.isCurrent && x.IsHead);
                if (placementHead != null)
                {
                    var orgStructure = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == placementHead.OrgStructureId);
                    if (orgStructure != null)
                    {
                        var bossStructure = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == orgStructure.Parent);
                        if (bossStructure != null)
                        {
                            var bossesBoss = db.Placements.FirstOrDefault(x => x.OrgStructureId == bossStructure.OrgStructureId && x.BranchId == placementHead.BranchId && x.isCurrent && x.IsHead);
                            if (bossesBoss != null)
                            {
                                var profile = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == bossesBoss.EmployeeId && x.IsActive);
                                if (profile != null)
                                {
                                    boss = profile.FullNameaEnglish;
                                }
                            }
                            else
                            {
                                var bossesOtherBranch = db.Placements.FirstOrDefault(x => x.OrgStructureId == placementHead.OrgStructureId && x.isCurrent && x.IsHead);
                                if (bossesOtherBranch != null)
                                {
                                    var profile = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == bossesOtherBranch.EmployeeId && x.IsActive);
                                    if (profile != null)
                                    {
                                        boss = profile.FullNameaEnglish;
                                    }
                                }
                                var bossesBossOtherBranch = db.Placements.FirstOrDefault(x => x.OrgStructureId == bossStructure.OrgStructureId && x.isCurrent && x.IsHead);
                                if (bossesBossOtherBranch != null)
                                {
                                    var profile = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == bossesBossOtherBranch.EmployeeId && x.IsActive);
                                    if (profile != null)
                                    {
                                        boss = profile.FullNameaEnglish;
                                    }
                                }
                            }
                        }
                    }
                }

            }


            return boss;
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
                JobTitleName = placement.JobTitleName,
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
                IsPend = placement.IsPend,
                AssignmentDateAmharic = placement.AssignmentDateAmharic,
                EmployeeCode = placement.EmployeeCode,
                HaveFixedAsset = placement.HaveFixedAsset,
                Boss = GetBoss(placement.EmployeeId),
                EmployementType = placement.EmployementType,
                BusinessUnitId = placement.BusinessUnitId
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

        [AuthorizeRoles(HrmConstants.HRPlacementUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Placements_Create([DataSourceRequest]DataSourceRequest request, Placement placement)
        {
            ModelState.Clear();
            //validation
            if (placement.AssignmentDateAmharic != null)
            {
                var splitDate = placement.AssignmentDateAmharic.Split('/');
                placement.AssignmentDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            var empIdP = 0;

            db = new ExceedDbContext();

            var checkAssignment = db.Placements.Where(x => x.EmployeeId == placement.EmployeeId).FirstOrDefault();
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
                ModelState.AddModelError("",
                    "This employee is already assigened under  Work unit:" +
                    checkexistance.MainCompanyStructure.Name + ", job title: " +
                    checkexistance.Lookup.Description + " and branch: " + checkexistance.Branch.Name + "!!");
            }
            if (placement.AssignmentDate.Date > DateTime.Now.Date)
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
                if (placement.AssignmentDate.Date < emp.HiredDate)
                {
                    ModelState.AddModelError("", "assignment date can't lessthan hire date");
                }
            }

            var profile = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == placement.EmployeeId);
            if (profile != null)
            {
                placement.EmployementType = profile.Status;

            }

            if (ModelState.IsValid)
            {

                int i = 0;
                var palcements = db.Placements.Where(x => x.EmployeeId == placement.EmployeeId).ToList();
                if (palcements.Any())
                {
                    foreach (var p in palcements)
                    {
                        var entityy = new Placement
                        {
                            PlacementId = p.PlacementId,
                            EmployeeId = p.EmployeeId,
                            ImmediateSupervisor = p.ImmediateSupervisor,
                            IsHead = p.IsHead,
                            // HaveFixedAsset = p.HaveFixedAsset,
                            AssignmentDate = p.AssignmentDate,
                            MappingId = 0, // placement.MappingId,
                            LookupId = p.LookupId,
                            JobTitleName = GetTitleName(p.LookupId),
                            OrgStructureId = p.OrgStructureId,
                            ReportsTo = p.ReportsTo,
                            BranchId = p.BranchId,
                            RankId = p.RankId,
                            StepId = p.StepId,
                            Department = p.ReportsTo,
                            Salary = SalaryAmount,// p.Salary,
                            ReportsToMappingId = 0, // placement.ReportsToMappingId,
                            AssignmentType = p.AssignmentType,
                            ResponsibilityLevel = p.ResponsibilityLevel,
                            Remark = p.Remark,
                            DateCreated = p.DateCreated,
                            LastModified = DateTime.Now,
                            CreatedBy = placement.CreatedBy,
                            ModifiedBy = User.Identity.Name,
                            //DateCreatedAM = XAPI.EthiopicDateTime.GetEthiopicDate(Convert.ToInt32(p.DateCreated?.Day), Convert.ToInt32(p.DateCreated?.Month), Convert.ToInt32(p.DateCreated?.Year)),
                            //DateLastModifiedAM = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Today.Day, DateTime.Today.Month, DateTime.Today.Year),
                            isCurrent = false,
                            IsPend = p.IsPend,
                            EmployementType = p.EmployementType,
                            Index = i,
                            AssignmentDateAmharic = p.AssignmentDateAmharic,
                            EmployeeCode = p.EmployeeCode,
                            BusinessUnitId = p.BusinessUnitId,
                            OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                        };
                        db = new ExceedDbContext();
                        db.Placements.Attach(entityy);
                        db.Entry(entityy).State = EntityState.Modified;
                        db.SaveChanges();

                        i += 1;
                    }

                    var empPlace = palcements.Where(x => x.AssignmentType == AssignmentType.PROMOTION).ToList().OrderByDescending(x => x.AssignmentDate);
                    if (empPlace.Any())
                    {
                        var diff = DateTime.Today.Subtract(empPlace.FirstOrDefault().AssignmentDate).TotalDays;
                        if (diff < 365 && placement.AssignmentType == AssignmentType.PROMOTION)
                        {
                            ModelState.AddModelError("", "you were promoted before, you can not promoted two times a year ");
                        }
                    }
                    var yearProSetting = db.Configurations.FirstOrDefault(c => c.Category == "Promotion" && c.Reference == "YEARLY_PROMOTION_PER_YEAR");
                    if (yearProSetting != null)
                    {
                        var yearPros = palcements.Where(x => x.AssignmentType == AssignmentType.YEARLY_PROMOTION).OrderByDescending(x => x.AssignmentDate).ToList();
                        if (yearPros.Any())
                        {
                            var days = Convert.ToInt32(yearProSetting.CurrentValue) * 365;
                            var diff = DateTime.Today.Subtract(yearPros.FirstOrDefault().AssignmentDate).TotalDays;
                            if (diff < days && placement.AssignmentType == AssignmentType.YEARLY_PROMOTION)
                            {
                                ModelState.AddModelError("", "you were promoted yearly promotion before, you can not promoted two times a year ");
                            }
                        }
                    }
                    if (ModelState.IsValid)
                    {
                        var entity = new Placement
                        {
                            EmployeeId = placement.EmployeeId,
                            ImmediateSupervisor = placement.ImmediateSupervisor,
                            IsHead = placement.IsHead,
                            // HaveFixedAsset = placement.HaveFixedAsset,
                            AssignmentDate = placement.AssignmentDate,
                            MappingId = 0, //placement.MappingId,
                            RankId = placement.RankId,
                            StepId = placement.StepId,
                            LookupId = placement.LookupId,
                            OrgStructureId = placement.OrgStructureId,
                            Salary = SalaryAmount,
                            ReportsTo = placement.ReportsTo,
                            JobTitleName = GetTitleName(placement.LookupId),
                            Department = placement.ReportsTo,
                            ReportsToMappingId = 0, // placement.ReportsToMappingId,
                            AssignmentType = placement.AssignmentType,
                            ResponsibilityLevel = placement.ResponsibilityLevel,
                            BranchId = placement.BranchId,
                            BusinessUnitId = placement.BusinessUnitId,
                            Remark = placement.Remark,
                            DateCreated = DateTime.Now,
                            LastModified = placement.LastModified,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = placement.ModifiedBy,
                            //DateCreatedAM = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Today.Day, DateTime.Today.Month, DateTime.Today.Year),
                            //DateLastModifiedAM = XAPI.EthiopicDateTime.GetEthiopicDate(Convert.ToInt32(placement.LastModified?.Day),Convert.ToInt32( placement.LastModified?.Month), Convert.ToInt32(placement.LastModified?.Year)),
                            isCurrent = true,
                            IsPend = false,
                            Index = i,
                            EmployementType = placement.EmployementType,
                            AssignmentDateAmharic = placement.AssignmentDateAmharic,
                            EmployeeCode =
                                inputs
                                    .GetEmployees()
                                    .FirstOrDefault(x => x.PersonId == placement.EmployeeId)
                                    ?.EmployeeCode
                            ,
                            OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                        };
                        string branch = placement.BranchId.ToString();
                        db.Placements.Add(entity);
                        db.SaveChanges();


                        empIdP = entity.EmployeeId;

                        PerapreEmployeeAllowance(placement.EmployeeId, placement.LookupId);
                        db = new ExceedDbContext();
                        var employee =
                            db.Person_Employee.FirstOrDefault(p => p.EmployeeId == placement.EmployeeId);
                        if (employee != null)
                        {
                            employee.IsActive = true;
                            //employee.Status = placement.EmployementType;
                            employee.OrgBranchName = branch;
                            employee.ModifiedBy = User.Identity.Name;
                            employee.LastModified = DateTime.Now;


                          //  db.Person_Employee.Attach(employee);
                            db.Entry(employee).State = EntityState.Modified;
                            db.SaveChanges();
                        }


                        placement.PlacementId = entity.PlacementId;
                        DepartmentId = 0;
                    }
                }

                else
                {

                    var entity = new Placement
                    {
                        EmployeeId = placement.EmployeeId,
                        ImmediateSupervisor = placement.ImmediateSupervisor,
                        IsHead = placement.IsHead,
                        // HaveFixedAsset = placement.HaveFixedAsset,
                        AssignmentDate = placement.AssignmentDate,
                        MappingId = 0, //placement.MappingId,
                        RankId = placement.RankId,
                        StepId = placement.StepId,
                        LookupId = placement.LookupId,
                        OrgStructureId = placement.OrgStructureId,
                        Salary = SalaryAmount,
                        ReportsTo = placement.ReportsTo,
                        JobTitleName = GetTitleName(placement.LookupId),
                        Department = placement.ReportsTo,
                        ReportsToMappingId = 0, // placement.ReportsToMappingId,
                        AssignmentType = placement.AssignmentType,
                        ResponsibilityLevel = placement.ResponsibilityLevel,
                        BranchId = placement.BranchId,
                        BusinessUnitId = placement.BusinessUnitId,
                        Remark = placement.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = placement.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = placement.ModifiedBy,
                        //DateCreatedAM = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Today.Day, DateTime.Today.Month, DateTime.Today.Year),
                        //DateLastModifiedAM = XAPI.EthiopicDateTime.GetEthiopicDate(Convert.ToInt32(placement.LastModified?.Day), Convert.ToInt32(placement.LastModified?.Month), Convert.ToInt32(placement.LastModified?.Year)),

                        isCurrent = true,
                        IsPend = false,
                        EmployementType = placement.EmployementType,
                        Index = i,
                        OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                        AssignmentDateAmharic = placement.AssignmentDateAmharic,
                        EmployeeCode =
                            inputs
                                .GetEmployees()
                                .FirstOrDefault(x => x.PersonId == placement.EmployeeId)
                                ?.EmployeeCode
                    };

                    db.Placements.Add(entity);
                    db.SaveChanges();
                    PerapreEmployeeAllowance(placement.EmployeeId, placement.LookupId);
                    db = new ExceedDbContext();
                    var employee =
                        db.Person_Employee.FirstOrDefault(p => p.EmployeeId == placement.EmployeeId);
                    string branch = placement.BranchId.ToString();
                    if (employee != null)
                    {
                        employee.IsActive = true;
                       // employee.Status = placement.EmployementType;
                        employee.OrgBranchName = branch;
                        employee.ModifiedBy = User.Identity.Name;
                        employee.LastModified = DateTime.Now;

                        //db.Person_Employee.Attach(employee);
                        db.Entry(employee).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    placement.PlacementId = entity.PlacementId;
                    DepartmentId = 0;

                    empIdP = entity.EmployeeId;
                }

                #region operation log
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == empIdP);
                if (person != null)
                {
                        UserHelper.OperationLog(Request.UserHostAddress, "Employee Placement", "Success", User.Identity.Name, " Placement of " + person.EmployeeCode + " is Created By  " + User.Identity.Name,Modules.HRM);
                    
                }
                #endregion

            }


            return Json(new[] { placement }.ToDataSourceResult(request, ModelState));
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

          
            if (placement.AssignmentDate.Date > DateTime.Now.Date)
            {

                ModelState.AddModelError("", "Assigment date should be less or equal to the current date!!");

            }
            var pp = db.Placements.FirstOrDefault(x => x.EmployeeId == placement.EmployeeId && x.isCurrent && x.PlacementId != placement.PlacementId);
            if (pp != null && placement.isCurrent)
            {
                ModelState.AddModelError("", "this employee have other current placement ");
            }

            var profile = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == placement.EmployeeId);
            if (profile != null)
            {
                placement.EmployementType = profile.Status;

            }
            if (ModelState.IsValid)
            {



                var entity = new Placement
                {
                    PlacementId = placement.PlacementId,
                    EmployeeId = placement.EmployeeId,
                    ImmediateSupervisor = placement.ImmediateSupervisor,
                    IsHead = placement.IsHead,
                    EmployementType = placement.EmployementType,
                    // HaveFixedAsset = placement.HaveFixedAsset,
                    AssignmentDate = placement.AssignmentDate,
                    MappingId = 0,// placement.MappingId,
                    BranchId = placement.BranchId,
                    JobTitleName = GetTitleName(placement.LookupId),
                    OrgStructureId = placement.OrgStructureId,
                    ReportsTo = placement.ReportsTo,
                    RankId = placement.RankId,
                    StepId = placement.StepId,
                    LookupId = placement.LookupId,
                    Department = placement.ReportsTo,
                    Salary = placement.Salary,
                    ReportsToMappingId = 0,// placement.ReportsToMappingId,
                    AssignmentType = placement.AssignmentType,
                    ResponsibilityLevel = placement.ResponsibilityLevel,
                    Remark = placement.Remark,
                    BusinessUnitId = placement.BusinessUnitId,
                    DateCreated = placement.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = placement.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    //DateCreatedAM = placement.DateCreatedAM,
                    //  DateLastModifiedAM = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Today.Day,DateTime.Today.Month, DateTime.Today.Year),

                    isCurrent = placement.isCurrent,
                    IsPend = placement.IsPend,
                    Index = placement.Index,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    AssignmentDateAmharic = placement.AssignmentDateAmharic,
                    EmployeeCode = inputs.GetEmployees().FirstOrDefault(x => x.PersonId == placement.EmployeeId)?.EmployeeCode,

                };
                if (SalaryAmount >= 0)
                {
                    entity.Salary = SalaryAmount;
                }
                db.Placements.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                var employee =
                        db.Person_Employee.FirstOrDefault(p => p.EmployeeId == placement.EmployeeId);
                if (employee != null)
                {

                  //  employee.Status = placement.EmployementType;
                    employee.OrgBranchName = placement.BranchId.ToString();
                    employee.ModifiedBy = User.Identity.Name;
                    employee.LastModified = DateTime.Now;

                    //db.Person_Employee.Attach(employee);
                    db.Entry(employee).State = EntityState.Modified;
                    db.SaveChanges();
                }
                PerapreEmployeeAllowance(placement.EmployeeId, placement.LookupId);

                #region operation log
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                if (person != null)
                {
                        UserHelper.OperationLog(Request.UserHostAddress, "Employee Placement", "Success", User.Identity.Name, "Placement Info of " + person.EmployeeCode + " is Updated By  " + User.Identity.Name,Modules.HRM);
                    
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
                    // HaveFixedAsset = placement.HaveFixedAsset,
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
                    AssignmentDateAmharic = placement.AssignmentDateAmharic,
                    EmployeeCode = placement.EmployeeCode,
                    EmployementType = placement.EmployementType,
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
                        AssignmentDate = checkAssignment.AssignmentDate,
                        IsHead = checkAssignment.IsHead,
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
        public string GetRankName(int id)
        {
            db = new ExceedDbContext();
            var ranks = db.Ranks.Where(x => x.RankId == id).FirstOrDefault();
            if (ranks != null)
            {
                return ranks.RankName;
            }
            return "";

        }
        public string GetStepName(int id)
        {
            db = new ExceedDbContext();
            var ranks = db.Steps.Where(x => x.StepId == id).FirstOrDefault();
            if (ranks != null)
            {
                return ranks.StepName;
            }
            return "";

        }
        //public ActionResult UpdateSalary(int id)
        //{

        //}
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
            return Json(salaries.Select(p => new { SalaryStructureId = p.SalaryStructureId, SalaryAmount = p.SalaryAmount }), JsonRequestBehavior.AllowGet);
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
        public decimal GetSalaryAmount(int rank, int step, int job)
        {
            SalaryJobTitle salary = new SalaryJobTitle();

            salary = db.SalariesJobTitles.Where(s => s.RankId == rank && s.StepId == step && s.LookupId == job).FirstOrDefault();
            SalaryAmount = 0;
            if (salary != null)
            {
                //if (title != 0)
                //{
                //    var struc =
                //        db.SalariesJobTitles.Where(x => x.StepId == salary.StepId && x.RankId == salary.RankId)
                //            .FirstOrDefault();
                //    if (struc != null)
                //    {
                //        SalaryAmount = struc.Salary;
                //        return Convert.ToDecimal(salary.SalaryAmount);
                //    }

                //}
                SalaryAmount = salary.Salary;
                return Convert.ToDecimal(salary.Salary); //Json(salary.SalaryAmount, JsonRequestBehavior.AllowGet);
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
        public JsonResult GetStructure()
        {
            db = new ExceedDbContext();
            var orgs = db.MainCompanyStructures.ToList();


            return Json(orgs.Select(p => new { OrgStructureId = p.OrgStructureId, Name = p.Name }), JsonRequestBehavior.AllowGet);


        }
        public ActionResult Pend(int id)
        {           

            var placement = db.Placements.FirstOrDefault(x=>x.PlacementId == id);

            if (placement != null)
            {
                placement.IsPend = true;

                db.Entry(placement).State = EntityState.Modified;
                db.SaveChanges();

                var formattedData = new
                {
                    Message = "You successfully pend this employee for payroll proccessing .",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            var formattedData1 = new
            {
                Message = "the employee is not pend.",
                Status = "FAIL"
            };
            return Json(formattedData1, JsonRequestBehavior.AllowGet);

        }
        public ActionResult UnPend(int id)
        {

            var placement = db.Placements.FirstOrDefault(x => x.PlacementId == id);

            if (placement != null)
            {
                placement.IsPend = false;

                db.Entry(placement).State = EntityState.Modified;
                db.SaveChanges();

                var formattedData = new
                {
                    Message = "You successfully remove this employee from Pend lists to start payroll proccessing .",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            var formattedData1 = new
            {
                Message = "this employee is not removed from pend lists.",
                Status = "FAIL"
            };
            return Json(formattedData1, JsonRequestBehavior.AllowGet);

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
    }
}