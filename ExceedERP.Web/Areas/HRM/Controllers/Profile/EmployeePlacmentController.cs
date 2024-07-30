using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
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
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{

    [AuthorizeRoles(HrmConstants.HRPlacementAdmin, HrmConstants.HRPlacementUser)]
    public class EmployeePlacmentController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static decimal SalaryAmount { get; set; }
        public static int Rankid { get; set; }
        public static int Stepid { get; set; }
        public static int DepartmentId { get; set; }
        public static DateTime AssigmentDate { get; set; }
        public static string AssigmentDateAmharic { get; set; }
        HRMInputs inputs = new HRMInputs();
        public static int EmployeeId { get; set; }
        public ActionResult Index()
        {

            GetRank();
            GetStep();
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            var structres = db.MainCompanyStructures.ToList();
            ViewData["organization"] = structres.AsQueryable();
            var branches = db.Branch;
            ViewData["branches"] = branches;
            GetDepartments();
            ViewData["EmployeeList"] = db.Person_Employee.ToList();// inputs.GetEmployeesForPlacment(User.Identity.Name);
            ViewData["EmployeeListEnglish"] = db.Person_Employee.ToList();//inputs.GetEmployeesForPlacment(User.Identity.Name);
            var workunits = db.MainCompanyStructures.ToList();
            ViewData["workunits"] = workunits;
            var organizationUnits = db.MainCompanyStructures.ToList();
            ViewData["Parent"] = organizationUnits;
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
                        var departmentLevelTwo = db.MainCompanyStructures.Where(x => x.Parent == a.OrgStructureId).ToList();
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
                ParentOrgStructureId = placement.ParentOrgStructureId,
                LookupId = placement.LookupId,
                MappingId = placement.MappingId,
                JobTitleName = GetTitleName(placement.LookupId),
                ReportsToMappingId = placement.ReportsToMappingId,
                RankId = placement.RankId,
                BranchId = placement.BranchId,
                StepId = placement.StepId,
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
                isCurrent = placement.isCurrent,
                Index = placement.Index,
                AssignmentDateAmharic = placement.AssignmentDateAmharic,
                EmployeeCode = placement.EmployeeCode,
                OrgBranchName = placement.OrgBranchName,
                Boss = GetBoss(placement.EmployeeId),
                EmployementType = placement.EmployementType,
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
            var orgStructure = db.MainCompanyStructures.Find(placement.OrgStructureId);
            if (orgStructure != null)
            {
                placement.ParentOrgStructureId = orgStructure.Parent;
            }
            int i = 0;
            db = new ExceedDbContext();
            var checkAssignment = db.Placements.Where(x => x.EmployeeId == placement.EmployeeId).FirstOrDefault();
            if (checkAssignment != null && placement.AssignmentType == AssignmentType.NEW)
            {

                ModelState.AddModelError("", "You can not assign this employee as a new assignment!!");

            }
            else
            {



                var checkexistance =
                    db.Placements.Where(
                        x =>
                            x.EmployeeId == placement.EmployeeId && x.OrgStructureId == placement.OrgStructureId &&
                            x.LookupId == placement.LookupId && x.BranchId == placement.BranchId).FirstOrDefault();
                if (checkexistance != null)
                {

                    ModelState.AddModelError("",
                        "This employee is already assigened under  Work unit:" +
                        /*checkexistance.MainCompanyStructure.Name +*/ ", job title: " +
                        /*checkexistance.Lookup.Description +*/ " and branch: " + /*checkexistance.Branch.Name +*/ "!!");
                }
                else if (AssigmentDateAmharic == null)
                {
                    ModelState.AddModelError("", "Agreement date value is mandatory!!");
                }
                else if (AssigmentDate.Date > DateTime.Now.Date)
                {

                    ModelState.AddModelError("", "Assigment date should be less or equal to the current date!!");
                }
                var workunitHead = db.Placements.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId && x.IsHead);
                if (placement.IsHead && workunitHead != null)
                {
                    ModelState.AddModelError("", "This Workunit already have Head");
                }

                if (ModelState.IsValid)
                {
                    var empId1 = 0;
                    var palcements =
                        db.Placements.Where(x => x.EmployeeId == placement.EmployeeId)
                            .OrderBy(x => x.AssignmentDate)
                            .ToList();
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
                                AssignmentDate = p.AssignmentDate,
                                MappingId = 0, // placement.MappingId,
                                LookupId = p.LookupId,
                                JobTitleName = p.JobTitleName,
                                OrgStructureId = p.OrgStructureId,
                                ParentOrgStructureId = p.ParentOrgStructureId,
                                ReportsTo = p.ReportsTo,
                                BranchId = p.BranchId,
                                RankId = p.RankId,
                                StepId = p.StepId,
                                Department = p.ReportsTo,
                                Salary = p.Salary,
                                ReportsToMappingId = 0, // placement.ReportsToMappingId,
                                AssignmentType = p.AssignmentType,
                                ResponsibilityLevel = p.ResponsibilityLevel,
                                Remark = p.Remark,
                                DateCreated = p.DateCreated,
                                LastModified = DateTime.Now,
                                CreatedBy = placement.CreatedBy,
                                ModifiedBy = User.Identity.Name,
                                isCurrent = false,
                                Index = i,
                                AssignmentDateAmharic = p.AssignmentDateAmharic,
                                EmployementType = p.EmployementType,
                                EmployeeCode = p.EmployeeCode,
                                OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                            };
                            db = new ExceedDbContext();
                            db.Placements.Attach(entityy);
                            db.Entry(entityy).State = EntityState.Modified;
                            db.SaveChanges();
                            var entityp = new PlacementHistory
                            {
                                EmployeeId = p.EmployeeId,
                                From = p.AssignmentDate,
                                To = new DateTime(),
                                JobTitle = p.JobTitleName,

                                Rank = p.RankName,
                                Step = p.StepName,
                                Salary = p.Salary,

                                Remark = p.Remark,
                                DateCreated = DateTime.Now,
                                LastModified = null,
                                CreatedBy = User.Identity.Name,
                                ModifiedBy = null,
                                FromAmharic = p.AssignmentDateAmharic,
                                ToAmharic = "",
                                EmployeeCode = p.EmployeeCode,
                                OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                            };

                            db.PlacementHistories.Add(entityp);
                            db.SaveChanges();

                            i += 1;
                        }
                        var entity = new Placement
                        {
                            EmployeeId = placement.EmployeeId,
                            ImmediateSupervisor = placement.ImmediateSupervisor,
                            IsHead = placement.IsHead,
                            AssignmentDate = AssigmentDate,
                            MappingId = 0, //placement.MappingId,
                            RankId = Rankid,
                            StepId = Stepid,
                            LookupId = placement.LookupId,
                            OrgStructureId = placement.OrgStructureId,
                            ParentOrgStructureId = placement.ParentOrgStructureId,
                            Salary = SalaryAmount,
                            ReportsTo = placement.ReportsTo,
                            BranchId = placement.BranchId,
                            JobTitleName = GetTitleName(placement.LookupId),
                            Department = placement.ReportsTo,
                            ReportsToMappingId = 0, // placement.ReportsToMappingId,
                            AssignmentType = placement.AssignmentType,
                            ResponsibilityLevel = placement.ResponsibilityLevel,
                            Remark = placement.Remark,
                            DateCreated = DateTime.Now,
                            LastModified = placement.LastModified,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = placement.ModifiedBy,
                            isCurrent = true,
                            Index = i,
                            OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                            AssignmentDateAmharic = AssigmentDateAmharic,
                            EmployementType = placement.EmployementType,
                            EmployeeCode =
                                inputs.GetEmployees()
                                    .Where(x => x.PersonId == placement.EmployeeId)
                                    .FirstOrDefault()
                                    .EmployeeCode
                        };

                        empId1 = entity.EmployeeId;

                        db.Placements.Add(entity);
                        db.SaveChanges();
                        PerapreEmployeeAllowance(placement.EmployeeId, placement.LookupId);
                        db = new ExceedDbContext();
                        var employee =
                            db.Person_Employee.Where(p => p.EmployeeId == placement.EmployeeId).FirstOrDefault();
                        if (employee != null)
                        {
                            employee.IsActive = true;
                            employee.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                            employee.ModifiedBy = User.Identity.Name;
                            employee.LastModified = DateTime.Now;
                            db.Person_Employee.Attach(employee);
                            db.Entry(employee).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                        placement.PlacementId = entity.PlacementId;
                        DepartmentId = 0;
                    }
                    else
                    {

                        var entity = new Placement
                        {
                            EmployeeId = placement.EmployeeId,
                            ImmediateSupervisor = placement.ImmediateSupervisor,
                            IsHead = placement.IsHead,
                            AssignmentDate = AssigmentDate,
                            MappingId = 0, //placement.MappingId,
                            RankId = Rankid,
                            StepId = Stepid,
                            LookupId = placement.LookupId,
                            OrgStructureId = placement.OrgStructureId,
                            ParentOrgStructureId = placement.ParentOrgStructureId,
                            Salary = SalaryAmount,
                            ReportsTo = placement.ReportsTo,
                            BranchId = placement.BranchId,
                            JobTitleName = GetTitleName(placement.LookupId),
                            Department = placement.ReportsTo,
                            ReportsToMappingId = 0, // placement.ReportsToMappingId,
                            AssignmentType = placement.AssignmentType,
                            ResponsibilityLevel = placement.ResponsibilityLevel,
                            Remark = placement.Remark,
                            DateCreated = DateTime.Now,
                            LastModified = placement.LastModified,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = placement.ModifiedBy,
                            EmployementType = placement.EmployementType,
                            isCurrent = true,
                            Index = i,
                            OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                            AssignmentDateAmharic = AssigmentDateAmharic,
                            EmployeeCode =
                                inputs.GetEmployees()
                                    .Where(x => x.PersonId == placement.EmployeeId)
                                    .FirstOrDefault()
                                    .EmployeeCode
                        };

                        db.Placements.Add(entity);
                        db.SaveChanges();
                        PerapreEmployeeAllowance(placement.EmployeeId, placement.LookupId);
                        db = new ExceedDbContext();
                        var employee =
                            db.Person_Employee.Where(p => p.EmployeeId == placement.EmployeeId).FirstOrDefault();
                        if (employee != null)
                        {
                            employee.IsActive = true;
                            employee.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                            employee.ModifiedBy = User.Identity.Name;
                            employee.LastModified = DateTime.Now;
                            db.Person_Employee.Attach(employee);
                            db.Entry(employee).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                        placement.PlacementId = entity.PlacementId;
                        DepartmentId = 0;

                        empId1 = entity.EmployeeId;
                    }

                    #region operation log
                    var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == empId1);
                    if (person != null)
                    {
                        
                            UserHelper.OperationLog(Request.UserHostAddress, "Employee Placement", "Success", User.Identity.Name, "Placement Info of " + person.EmployeeCode + " is Created By  " + User.Identity.Name,Modules.HRM);
                        
                    }
                    #endregion

                }
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
    
        [AuthorizeRoles( HrmConstants.HRPlacementUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Placements_Update([DataSourceRequest]DataSourceRequest request, Placement placement)
        {
            ModelState.Clear();
            var orgStructure = db.MainCompanyStructures.Find(placement.OrgStructureId);
            if (orgStructure != null)
            {
                placement.ParentOrgStructureId = orgStructure.Parent;
            }
            if (ModelState.IsValid)
            {
                if (AssigmentDateAmharic == null)
                {
                    AssigmentDateAmharic = placement.AssignmentDateAmharic;
                    var splitDate = AssigmentDateAmharic.Split('/');
                    AssigmentDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                if (AssigmentDate.Date > DateTime.Now.Date)
                {

                    ModelState.AddModelError("", "Assigment date should be less or equal to the current date!!");
                }
                else
                {
                    var entity = new Placement
                    {
                        PlacementId = placement.PlacementId,
                        EmployeeId = placement.EmployeeId,
                        ImmediateSupervisor = placement.ImmediateSupervisor,
                        IsHead = placement.IsHead,
                        AssignmentDate = AssigmentDate,
                        MappingId = 0,// placement.MappingId,
                        LookupId = placement.LookupId,
                        EmployementType = placement.EmployementType,
                        JobTitleName = placement.JobTitleName,
                        OrgStructureId = placement.OrgStructureId,
                        ParentOrgStructureId = placement.ParentOrgStructureId,
                        ReportsTo = placement.ReportsTo,
                        BranchId = placement.BranchId,
                        RankId = Rankid,
                        StepId = Stepid,
                        Department = placement.ReportsTo,
                        Salary = placement.Salary,
                        ReportsToMappingId = 0,// placement.ReportsToMappingId,
                        AssignmentType = placement.AssignmentType,
                        ResponsibilityLevel = placement.ResponsibilityLevel,
                        Remark = placement.Remark,
                        DateCreated = placement.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = placement.CreatedBy,
                        ModifiedBy = User.Identity.Name,
                        isCurrent = placement.isCurrent,
                        Index = placement.Index,
                        AssignmentDateAmharic = AssigmentDateAmharic,
                        EmployeeCode = inputs.GetEmployees().Where(x => x.PersonId == placement.EmployeeId).FirstOrDefault().EmployeeCode,
                        OrgBranchName = placement.OrgBranchName
                    };

                    db.Placements.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    #region operation log
                    var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                    if (person != null)
                    {
                            UserHelper.OperationLog(Request.UserHostAddress, "Employee Placement", "Success", User.Identity.Name, "Placement Info of " + person.EmployeeCode + " is Updated By  " + User.Identity.Name,Modules.HRM);
                        
                    }
                    #endregion
                }
            }

            return Json(new[] { placement }.ToDataSourceResult(request, ModelState));
        }
      
        [AuthorizeRoles( HrmConstants.HRAdmin)]
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
                    ParentOrgStructureId = placement.ParentOrgStructureId,
                    ReportsTo = placement.ReportsTo,
                    JobTitleName = placement.JobTitleName,
                    BranchId = placement.BranchId,
                    EmployementType = placement.EmployementType,
                    RankId = Rankid,
                    StepId = Stepid,
                    Department = placement.ReportsTo,
                    Salary = placement.Salary,
                    ReportsToMappingId = placement.ReportsToMappingId,
                    AssignmentType = placement.AssignmentType,
                    ResponsibilityLevel = placement.ResponsibilityLevel,
                    Remark = placement.Remark,
                    DateCreated = placement.DateCreated,
                    LastModified = placement.LastModified,
                    CreatedBy = placement.CreatedBy,
                    ModifiedBy = placement.ModifiedBy,
                    isCurrent = placement.isCurrent,
                    Index = placement.Index,
                    AssignmentDateAmharic = placement.AssignmentDateAmharic,
                    EmployeeCode = placement.EmployeeCode,
                    OrgBranchName = placement.OrgBranchName
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted placment information of employee Id" + placement.EmployeeCode);
                #endregion
                db.Placements.Attach(entity);
                db.Placements.Remove(entity);
                db.SaveChanges();
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
        public decimal GetSalaryAmount(int rank, int step, int title = 0)
        {
            Salary salary = new Salary();

            salary = db.Salaries.Where(s => s.RankId == rank && s.StepId == step).FirstOrDefault();



            SalaryAmount = 0;
            if (salary != null)
            {
                if (title != 0)
                {
                    var struc =
                        db.SalariesJobTitles.Where(x => x.StepId == salary.StepId && x.LookupId == title && x.RankId == salary.RankId)
                            .FirstOrDefault();
                    if (struc != null)
                    {
                        SalaryAmount = struc.Salary;
                        return Convert.ToDecimal(salary.SalaryAmount);
                    }

                }
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

        public string GetDepartment(int OrgId, int title)
        {
            var structures = db.MainCompanyStructures.Where(x => x.OrgStructureId == OrgId);
            var lookups = db.Lookups;


            foreach (var s in structures)
            {
                if (s.OrgStructureId == OrgId)
                {
                    db = new ExceedDbContext();
                    var types = db.Structure.ToList();
                    var type = types.Where(x => x.StructureId == s.StructureId).FirstOrDefault();
                    if (type != null)
                    {
                        DepartmentId = OrgId;
                        break;

                    }
                    else
                    {
                        if (s.Parent != 0)
                        {
                            if (CheckParent(s.Parent)) ; break;
                        }


                    }

                }
            }
            db = new ExceedDbContext();
            var struc =
                       db.SalariesJobTitles.Where(x => x.LookupId == title).FirstOrDefault();
            if (struc != null)
            {
                SalaryAmount = struc.Salary;
                Rankid = struc.RankId;
                Stepid = struc.StepId;
                var ranks = db.Ranks.Where(x => x.RankId == Rankid).FirstOrDefault();
                var steps = db.Steps.Where(x => x.StepId == Stepid).FirstOrDefault();
                return struc.Salary.ToString() + "-" + ranks.RankName + "-" + steps.StepName.ToString();
            }
            return "";
        }

        private bool CheckParent(int Id)
        {
            db = new ExceedDbContext();
            var structures = db.MainCompanyStructures.Where(x => x.OrgStructureId == Id).ToList();
            if (structures.Any())
            {
                foreach (var s in structures)
                {
                    if (s.OrgStructureId == Id)
                    {

                        var type = db.Structure.Where(x => x.StructureId == s.StructureId).FirstOrDefault();
                        if (type != null)
                        {
                            DepartmentId = Id;
                            return true;

                        }
                        else
                        {
                            CheckParent(s.Parent);
                        }

                    }
                }
            }
            return false;
        }
    }
}