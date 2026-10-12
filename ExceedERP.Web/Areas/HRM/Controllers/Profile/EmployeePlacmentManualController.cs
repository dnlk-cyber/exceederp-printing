using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.Allowance;
using ExceedERP.Core.Domain.HRM.Company;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Core.Domain.HRM.SalaryStructure;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using Kendo.Mvc;
using ExceedERP.Helpers.HR;

namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{
    [AuthorizeRoles(HrmConstants.HRPlacementAdmin, HrmConstants.HRPlacementUser)]
    public class EmployeePlacmentManualController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private readonly CacheData CacheData = new CacheData();
        HRMInputs inputs = new HRMInputs();
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            //GetRank();
            //GetStep();
            var ranks = db.Ranks.AsQueryable();
            ViewData["RankList"] = CacheData.GetRankCaches().Select(
               i => new
               {
                   RankName = i.Name,
                   RankId = i.Id,
               }).AsQueryable(); ;
            var stepss = db.Steps.AsQueryable();
            ViewData["StepList"] = CacheData.GetStepCaches().Select(
               i => new
               {
                   StepName = i.Name,
                   StepId = i.Id,

               }).AsQueryable();
            ViewData["EmployeeList"] = CacheData.GetEmployeeCaches().Select(
               i => new
               {
                   FullNameAmharic = i.AmharicName,
                   FullNameaEnglish = i.Name,
                   EmployeeId = i.Id,
                   HeadId = i.Id
               }).AsQueryable();
            ViewData["jobTitle"] = CacheData.GetJobTitlesCaches().Select(
               i => new
               {
                   Description = i.Name,
                   LookupId = i.Id,
               }).AsQueryable();
            ViewData["AllJobTitles"] = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE
                                                        || x.LookupType == LookUpTypes.OLD_JOB_TITLE
                                                        || x.LookupType == LookUpTypes.Jobtitle_External_Experience).Select(
               i => new
               {
                   Description = i.Description,
                   LookupId = i.LookupId,
               }).ToList();
            ViewData["branches"] = CacheData.GetBranchCaches().Select(
               i => new
               {
                   Name = i.Name,
                   BranchId = i.Id,
               }).AsQueryable(); ;

            ViewData["businessUnits"] = CacheData.GetBusinessUnitCaches().Select(
               i => new
               {
                   Name = i.Name,
                   BusinessUnitId = i.Id,
               }).AsQueryable(); ; ;
            ViewData["workunits"] = CacheData.GetWorkUnitCaches().Select(
               i => new
               {
                   Name = i.Name,
                   OrgStructureId = i.Id,
               }).AsQueryable();
            var datePreference = db.DatePreferenceSettings.FirstOrDefault();

            ViewData["IsEthiopicDate"] = datePreference?.UseEthiopianDate ?? true;
            ViewData["isManual"] = db.GeneralJobtitleSettings.FirstOrDefault()?.IsManual;

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
                var bossPlace = db.Placements.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId /*&& x.BranchId == placement.BranchId */&& x.isCurrent && x.IsHead);
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
                            var bossesBoss = db.Placements.FirstOrDefault(x => x.OrgStructureId == bossStructure.OrgStructureId /*&& x.BranchId == placementHead.BranchId*/ && x.isCurrent && x.IsHead);
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
        public ActionResult GetBranchs()
        {
            var branches = db.Branch.ToList();

            return Json(branches.Select(p => new
            {
                BranchId = p.BranchId,
                Name = p.Name
            }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult DeActivateEmployeesByBranch(int branchId)
        {

            var placements = db.Placements.Where(x => x.isCurrent && x.BranchId == branchId).ToList();

            int countPlacements = placements.Count();

            foreach (var place in placements)
            {
                place.isCurrent = false;

                db.Entry(place).State = EntityState.Modified;
                db.SaveChanges();

                var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == place.EmployeeId);
                if (employee != null)
                {
                    employee.IsActive = false;

                    db.Entry(employee).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            var checkPlacementsCount = db.Placements.Where(x => x.isCurrent && x.BranchId == branchId).ToList().Count();

            if (checkPlacementsCount == 0)
            {
                var formattedData1 = new
                {
                    Message = "Successfuly DeActivated",
                    Status = "OK"
                };
                return Json(formattedData1, JsonRequestBehavior.AllowGet);
            }
            else if (checkPlacementsCount > 0 && checkPlacementsCount < countPlacements)
            {
                var formattedData2 = new
                {
                    Message = "Some Employees Are Not DeActivated",
                    Status = "FAIL"
                };
                return Json(formattedData2, JsonRequestBehavior.AllowGet);
            }
            else if (checkPlacementsCount == countPlacements)
            {
                var formattedData3 = new
                {
                    Message = "Not Successfull",
                    Status = "FAIL"
                };
                return Json(formattedData3, JsonRequestBehavior.AllowGet);
            }
            else if (countPlacements == 0)
            {
                var formattedData4 = new
                {
                    Message = "No Placement under this branch",
                    Status = "FAIL"
                };
                return Json(formattedData4, JsonRequestBehavior.AllowGet);
            }
            var formattedData5 = new
            {
                Message = "",
                Status = "FAIL"
            };
            return Json(formattedData5, JsonRequestBehavior.AllowGet);
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
        public ActionResult Placements_Read([DataSourceRequest] DataSourceRequest request)
        {
            bool useEthDate = true; // default

            try
            {
                useEthDate = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate ?? true;
            }
            catch (Exception ex)
            {
                // Optional: Log the error if needed
                // logger.LogError(ex, "Failed to read DatePreferenceSettings");
                useEthDate = true;
            }
            string username = User.Identity.Name;
            List<int> myBranches = UserInformationHelper.GetUserBranchsAsInt(username);
            List<int> myBusinessUnits = GlobalHelper.IsMultipleBusinessUnit()
                ? UserInformationHelper.GetUserBusinessUnits(username)
                : new List<int>();

            var placements = db.Placements
                .Where(x => !x.IsTemporary && (x.isCurrent || !x.IsApprove) &&
                            myBranches.Contains(x.BranchId));

            if (myBusinessUnits.Any())
            {
                placements = placements.Where(x => myBusinessUnits.Contains(x.BusinessUnitId));
            }
            var result = placements
                .Select(x => new
                {
                    x.PlacementId,
                    x.EmployeeId,
                    EmployeeCode = x.Employee.EmployeeCode,
                    x.HeadId,
                    x.ImmediateSupervisor,
                    x.IsHead,
                    x.AssignmentDate,
                    x.SalarChangeDate,
                    x.SalarChangeDateAmharic,
                    x.OrgStructureId,
                    x.ParentOrgStructureId,
                    x.LookupId,
                    x.MappingId,
                    x.JobTitleName,
                    x.ReportsToMappingId,
                    x.RankId,
                    x.StepId,
                    x.BranchId,
                    x.Salary,
                    x.Department,
                    x.ReportsTo,
                    x.AssignmentType,
                    x.ResponsibilityLevel,
                    x.Remark,
                    x.DateCreated,
                    x.LastModified,
                    x.CreatedBy,
                    x.ModifiedBy,
                    x.isCurrent,
                    x.IsEditable,
                    x.AssignmentDateAmharic,
                    x.EmployementType,
                    x.IsPend,
                    x.BusinessUnitId,
                    x.IsApprove,
                    x.Age,
                    x.StepName,
                    x.HaveFixedAsset,
                    x.ApprovedByEmpId,
                    x.NextSalaryChangeDateAmharic,
                    x.NextSalaryChangeDate,
                    HireDate = x.Employee.HiredDateAmharic,
                    HireDateGorg = x.Employee.HiredDate,
                    Gender = x.Employee.Gender.ToString().ToLower(),
                    CheckCEO = x.HeadId == null && x.IsHead,

                })
                .ToDataSourceResult(request);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult Placements_Read([DataSourceRequest]DataSourceRequest request)
        //{
        //    //List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
        //    ////int branchId = UserInformationHelper.GetUserBranchByUserName(User.Identity.Name);
        //    //var placements = db.Placements.Where(x => !x.IsTemporary).Where(x => x.isCurrent || !x.IsApprove && mybraches.Contains(x.BranchId.ToString())).ToList();//.Where(x => mybraches.Contains(x.BranchId.ToString()) && x.isCurrent);
        //    List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
        //    IQueryable<Placement> placements = db.Placements.Where(x => !x.IsTemporary)
        //                                                    .Where(x => x.isCurrent || !x.IsApprove)
        //                                                    .Where(x => mybraches.Contains(x.BranchId));
        //    if (GlobalHelper.IsMultipleBusinessUnit())
        //    {
        //        List<int> myBusinessUnits = UserInformationHelper.GetUserBusinessUnits(User.Identity.Name);
        //        placements = placements.Where(x => myBusinessUnits.Contains(x.BusinessUnitId));
        //    }
        //    DataSourceResult result = placements.ToDataSourceResult(request, placement => new
        //    {
        //        PlacementId = placement.PlacementId,
        //        EmployeeId = placement.EmployeeId,
        //        ImmediateSupervisor = placement.ImmediateSupervisor,
        //        IsHead = placement.IsHead,
        //        AssignmentDate = placement.AssignmentDate,
        //        OrgStructureId = placement.OrgStructureId,
        //        ParentOrgStructureId = placement.ParentOrgStructureId,
        //        LookupId = placement.LookupId,
        //        MappingId = placement.MappingId,
        //        JobTitleName = placement.JobTitleName,
        //        ReportsToMappingId = placement.ReportsToMappingId,
        //        RankId = placement.RankId,
        //        StepId = placement.StepId,
        //        BranchId = placement.BranchId,
        //        HeadId = placement.HeadId,
        //        Salary = placement.Salary,
        //        Department = placement.Department,
        //        ReportsTo = placement.ReportsTo,
        //        AssignmentType = placement.AssignmentType,
        //        ResponsibilityLevel = placement.ResponsibilityLevel,
        //        Remark = placement.Remark,
        //        DateCreated = placement.DateCreated,
        //        LastModified = placement.LastModified,
        //        CreatedBy = placement.CreatedBy,
        //        ModifiedBy = placement.ModifiedBy,
        //        //DateCreatedAM = placement.DateCreatedAM,
        //        //DateLastModifiedAM = placement.DateLastModifiedAM,
        //        isCurrent = placement.isCurrent,
        //        IsPend = placement.IsPend,
        //        //AssignmentDateAmharic = placement.AssignmentDateAmharic,
        //        AssignmentDateAmharic = placement.AssignmentDateAmharic + " (" + placement.AssignmentDate.ToString("MMMM dd, yyyy") + ")",
        //        EmployeeCode = placement.EmployeeCode,
        //        HaveFixedAsset = placement.HaveFixedAsset,
        //        Boss = GetBoss(placement.EmployeeId),
        //        EmployementType = placement.EmployementType,
        //        BusinessUnitId = placement.BusinessUnitId,
        //        IsApprove = placement.IsApprove,
        //        PreparedByEmpId = placement.PreparedByEmpId,
        //        FullName = GetFullName(placement.EmployeeId),
        //        HireDate = HrHelper.GetHireDate(placement.EmployeeId),
        //        Age = EmployeeHelper.GetAge(placement.EmployeeId)

        //    });

        //    return Json(result);
        //}

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

            #region validation

            bool datePreference = HrHelper.GetDatePreference();
            if (datePreference)
            {
                if (string.IsNullOrEmpty(placement.AssignmentDateAmharic))
                {
                    ModelState.AddModelError("", "placement Assignment Date is required");
                    return Json(new[] { placement }.ToDataSourceResult(request, ModelState));
                }
                placement.AssignmentDate = HrHelper.ParseEthiopicDate(placement.AssignmentDateAmharic);
                if (!string.IsNullOrWhiteSpace(placement.SalarChangeDateAmharic))
                {
                    placement.SalarChangeDate =
                        HrHelper.ParseEthiopicDate(placement.SalarChangeDateAmharic);

                    placement.NextSalaryChangeDate =
                        placement.SalarChangeDate?.AddYears(2);

                    placement.NextSalaryChangeDateAmharic =
                        placement.NextSalaryChangeDate.HasValue
                            ? HrHelper.GetECDate(placement.NextSalaryChangeDate.Value)
                            : null;
                }
                
            }
            else
            {
                placement.AssignmentDateAmharic = HrHelper.GetECDate(placement.AssignmentDate);
                if (placement.SalarChangeDate.HasValue)
                {
                    placement.SalarChangeDateAmharic = HrHelper.GetECDate(placement.SalarChangeDate.Value);
                    placement.NextSalaryChangeDateAmharic = HrHelper.GetECDate(placement.SalarChangeDate.Value.AddYears(2));


                }

            }

            var checkNotApproved = db.Placements.FirstOrDefault(x => x.EmployeeId == placement.EmployeeId && !x.IsApprove);
            if(checkNotApproved != null)
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

            var workunitHead = db.Placements.FirstOrDefault(x => x.BranchId == placement.BranchId && x.OrgStructureId == placement.OrgStructureId && x.IsHead);
            if (placement.IsHead && workunitHead != null)
            {
                ModelState.AddModelError("", "This Workunit already have Head");
                return Json(new[] { placement }.ToDataSourceResult(request, ModelState));
            }

            var emp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == placement.EmployeeId);
            if (emp != null)
            {
                if (placement.AssignmentDate < emp.HiredDate)
                {
                    ModelState.AddModelError("", "assignment date can't lessthan hire date");
                    return Json(new[] { placement }.ToDataSourceResult(request, ModelState));
                }

                emp.Status = placement.EmployementType;
                db.Entry(emp).State = EntityState.Modified;
                db.SaveChanges();
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
                    ModelState.AddModelError("", "First Extend Contract date for this employee ");
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

            var manPower = db.JobTitleMapping.FirstOrDefault(x => x.BranchId == placement.BranchId && x.OrgStructureId == placement.OrgStructureId && x.LookupId == placement.LookupId);
            if (manPower != null && manPower.PlanType == JobTitlePlan.BOUNDED)
            {
            
                var allPlacementOnThisJob = db.Placements.Where(x => x.BranchId == manPower.BranchId
                                                                       && x.BusinessUnitId == manPower.BusinessUnitId
                                                                       && x.OrgStructureId == manPower.OrgStructureId
                                                                       && x.LookupId == manPower.LookupId
                                                                       && x.RankId == placement.RankId
                                                                       && x.StepId == placement.StepId
                                                                       && x.EmployeeId != placement.EmployeeId
                                                                       && x.isCurrent).ToList();
                if (allPlacementOnThisJob.Any())
                {
                    int totalPlaced = allPlacementOnThisJob.Count();
                    if (totalPlaced >= manPower.Quntity)
                    {
                        ModelState.AddModelError("", "This Jobtitle is set to Bounded so you can't assign more");
                        return Json(new[] { placement }.ToDataSourceResult(request, ModelState));
                    }
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

            if (placement.EmployeeId == placement.HeadId)
            {
                ModelState.AddModelError("", "Please Select your Corresponding  Head!");
                return Json(new[] { placement }.ToDataSourceResult(request, ModelState));

            }
            if (placement.HeadId == null || placement.HeadId == 0)
            {
                // placement.HeadId = EmployeeHelper.GetSupervisor(placement.EmployeeId);
                placement.HeadId = EmployeeHelper.GetSupervisorEmployeeId(placement.EmployeeId, placement.OrgStructureId, placement.BranchId, placement.BusinessUnitId);
                if (placement.HeadId == 0 || placement.HeadId == null)
                {
                    ModelState.AddModelError("", "Please Select Your Corresponding  Head the system doesn't found  !");
                    return Json(new[] { placement }.ToDataSourceResult(request, ModelState));

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
                placement.JobTitleName = GetTitleName(placement.LookupId);
                placement.DateCreated = DateTime.Now;
                placement.CreatedBy = User.Identity.Name;
                placement.EmployeeCode = inputs.GetEmployees().FirstOrDefault(x => x.PersonId == placement.EmployeeId)?.EmployeeCode;
                placement.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                placement.isCurrent = false; // this will be true after approval
                placement.IsApprove = false;
                placement.IsPend = false;
                placement.Index = i;
                placement.MappingId = 0;
                var entity = GetMappedObject(placement);
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
                if (person != null)
                {
                    UserHelper.OperationLog(
                        Request.UserHostAddress,
                        "Employee Placement",
                        "Success",
                        User.Identity.Name,
                        $"Placement Created for Employee [{person.EmployeeCode}] by [{User.Identity.Name}]. " +
                        $"Branch: {placement.BranchId}, " +
                        $"JobTitle: {placement.LookupId}, " +
                        $"Rank: {placement.RankId}, " +
                        $"Salary: {placement.Salary:N2}",
                        Modules.HRM);
                }
            }
            UserInformationHelper.UpdateDefalutBranchAndBusinessunit(placement.EmployeeId, placement.BranchId, placement.BusinessUnitId);

            return Json(new[] { placement }.ToDataSourceResult(request, ModelState));
        }
        [AuthorizeRoles(HrmConstants.HRPlacementUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Placements_Update([DataSourceRequest]DataSourceRequest request, Placement placement)
        {
            ModelState.Clear();
            int index = placement.AssignmentDateAmharic.IndexOf(" (");
            placement.AssignmentDateAmharic = (index > 0) ? placement.AssignmentDateAmharic.Substring(0, index) : placement.AssignmentDateAmharic;
            var orgStructure = db.MainCompanyStructures.Find(placement.OrgStructureId);
            if (orgStructure != null)
            {
                placement.ParentOrgStructureId = orgStructure.Parent;
            }
            bool datePreference = HrHelper.GetDatePreference();
            if (datePreference)
            {
                if (string.IsNullOrEmpty(placement.AssignmentDateAmharic))
                {
                    ModelState.AddModelError("", "placement Assignment Date is required");
                    return Json(new[] { placement }.ToDataSourceResult(request, ModelState));
                }
                placement.AssignmentDate = HrHelper.ParseEthiopicDate(placement.AssignmentDateAmharic);
                if (!string.IsNullOrWhiteSpace(placement.SalarChangeDateAmharic))
                {
                    placement.SalarChangeDate =
                        HrHelper.ParseEthiopicDate(placement.SalarChangeDateAmharic);

                    placement.NextSalaryChangeDate =
                        placement.SalarChangeDate?.AddYears(2);

                    placement.NextSalaryChangeDateAmharic =
                        placement.NextSalaryChangeDate.HasValue
                            ? HrHelper.GetECDate(placement.NextSalaryChangeDate.Value)
                            : null;
                }

            }
            else
            {
                placement.AssignmentDateAmharic = HrHelper.GetECDate(placement.AssignmentDate);
                if (placement.SalarChangeDate.HasValue)
                {
                    placement.SalarChangeDateAmharic = HrHelper.GetECDate(placement.SalarChangeDate.Value);
                    placement.NextSalaryChangeDateAmharic = HrHelper.GetECDate(placement.SalarChangeDate.Value.AddYears(2));


                }

            }

            var emp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == placement.EmployeeId);
            if (emp != null)
            {
                if (placement.AssignmentDate.Date < emp.HiredDate.Date)
                {
                    ModelState.AddModelError("", "assignment date can't lessthan hire date");
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

            //var profile = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == placement.EmployeeId);
            //if (profile != null)
            //{
            //    profile.Status = placement.EmployementType;
            //    db.Entry(profile).State = EntityState.Modified;
            //    db.SaveChanges();
            //}

            //var contractEmp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == placement.EmployeeId);
            //if (contractEmp != null && contractEmp.Status == EmployeeStatus.CONTRACT)
            if (placement.EmployementType == EmployeeStatus.CONTRACT)
            {
                var contractLast = db.Contract.Where(x => x.EmployeeId == placement.EmployeeId).OrderByDescending(x => x.To).ToList().FirstOrDefault();
                if (contractLast != null && contractLast.To <= placement.AssignmentDate)
                {
                    ModelState.AddModelError("", "First Extend Contract date for this employee ");
                    return Json(new[] { placement }.ToDataSourceResult(request, ModelState));
                }
            }

            //var freelancerEmp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == placement.EmployeeId);
            //if (freelancerEmp != null && freelancerEmp.Status == EmployeeStatus.FREELANCER)
            if (placement.EmployementType == EmployeeStatus.FREELANCER)
            {
                var freelancerLast = db.Freelancers.Where(x => x.EmployeeId == placement.EmployeeId).OrderByDescending(x => x.To).ToList().FirstOrDefault();
                if (freelancerLast != null && freelancerLast.To <= placement.AssignmentDate)
                {
                    ModelState.AddModelError("", "First Extend Freelancer date for this employee ");
                    return Json(new[] { placement }.ToDataSourceResult(request, ModelState));
                }
            }

            var manPower = db.JobTitleMapping.FirstOrDefault(x => x.BranchId == placement.BranchId && x.OrgStructureId == placement.OrgStructureId && x.LookupId == placement.LookupId);
            if (manPower != null && manPower.PlanType == JobTitlePlan.BOUNDED)
            {
                
                var allPlacementOnThisJob = db.Placements.Where(x => x.BranchId == manPower.BranchId
                                                                       && x.BusinessUnitId == manPower.BusinessUnitId
                                                                       && x.OrgStructureId == manPower.OrgStructureId
                                                                       && x.LookupId == manPower.LookupId
                                                                       && x.RankId == placement.RankId
                                                                       && x.StepId == placement.StepId
                                                                       && x.EmployeeId != placement.EmployeeId
                                                                       && x.PlacementId != placement.PlacementId
                                                                       && x.isCurrent).ToList();
                if (allPlacementOnThisJob.Any())
                {
                    int totalPlaced = allPlacementOnThisJob.Count();
                    if (totalPlaced >= manPower.Quntity)
                    {
                        ModelState.AddModelError("", "This Jobtitle is set to Bounded so you can't assign more");
                        return Json(new[] { placement }.ToDataSourceResult(request, ModelState));
                    }
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
            
            if (placement.EmployeeId == placement.HeadId)
            {
                ModelState.AddModelError("", "Please select a valid head —> you cannot assign yourself as your own head.!");
                return Json(new[] { placement }.ToDataSourceResult(request, ModelState));

            }
            if (placement.HeadId == null || placement.HeadId == 0)
            {
                placement.HeadId = EmployeeHelper.GetSupervisorEmployeeId(placement.EmployeeId, placement.OrgStructureId,
                                                                         placement.BranchId, placement.BusinessUnitId);

                if (!placement.CheckCEO && (placement.HeadId == null || placement.HeadId == 0))
                {
                    ModelState.AddModelError("", "Please select your corresponding head. It was not found!");
                    return Json(new[] { placement }.ToDataSourceResult(request, ModelState));
                }

            }

            var oldPlacement = db.Placements.Find(placement.PlacementId);
            var changes = new List<string>();

            if (oldPlacement.BranchId != placement.BranchId)
                changes.Add($"Branch: {oldPlacement.BranchId} → {placement.BranchId}");

            if (oldPlacement.LookupId != placement.LookupId)
                changes.Add($"JobTitle: {oldPlacement.LookupId} → {placement.LookupId}");

            if (oldPlacement.RankId != placement.RankId)
                changes.Add($"Rank: {oldPlacement.RankId} → {placement.RankId}");

            if (oldPlacement.StepId != placement.StepId)
                changes.Add($"Step: {oldPlacement.StepId} → {placement.StepId}");

            if (oldPlacement.Salary != placement.Salary)
                changes.Add($"Salary: {oldPlacement.Salary:N2} → {placement.Salary:N2}");

            if (oldPlacement.HeadId != placement.HeadId)
                changes.Add($"Head: {oldPlacement.HeadId} → {placement.HeadId}");
            if (ModelState.IsValid)
            {
                placement.PreparedByEmpId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                placement.JobTitleName = GetTitleName(placement.LookupId);
                placement.LastModified = DateTime.Now;
                placement.ModifiedBy = User.Identity.Name;
                placement.EmployeeCode = inputs.GetEmployees().FirstOrDefault(x => x.PersonId == placement.EmployeeId)?.EmployeeCode;
                placement.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);

                var entity = GetMappedObject(placement);
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

              
                #region operation log
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                if (person != null)
                {
                    
                        string changeDetails = changes.Any()
                            ? string.Join(", ", changes)
                            : "No significant changes";

                        UserHelper.OperationLog(
                            Request.UserHostAddress,
                            "Employee Placement",
                            "Success",
                            User.Identity.Name,
                            $"Placement of Employee [{person.EmployeeCode}] updated by [{User.Identity.Name}]. Changes: {changeDetails}",
                            Modules.HRM);
                    
                }
                #endregion
                //GeneratePdf(placement);

            }

            UserInformationHelper.UpdateDefalutBranchAndBusinessunit(placement.EmployeeId, placement.BranchId, placement.BusinessUnitId);

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


                    UserHelper.DeleteLog(
                                     Request.UserHostAddress,
                                     "Employee Placement",
                                     "Success",
                                     User.Identity.Name,
                                     $"Placement deleted for Employee [{placement.EmployeeCode}] and placement Id: {placement.PlacementId} by [{User.Identity.Name}]. " +
                                     $"BranchId: {placement.BranchId}, " +
                                     $"JobTitleId: {placement.LookupId}, " +
                                     $"RankId: {placement.RankId}, " +
                                     $"Salary: {placement.Salary:N2}"
                                 );
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
                CheckCEO = placement.CheckCEO,
                IsTemporary = placement.IsTemporary,
                ImmediateSupervisor = placement.ImmediateSupervisor,
                IsHead = placement.IsHead,
                AssignmentDate = placement.AssignmentDate,
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
                Remark = placement.Remark,
                DateCreated = placement.DateCreated,
                LastModified = placement.LastModified,
                CreatedBy = placement.CreatedBy,
                ModifiedBy = placement.ModifiedBy,
                isCurrent = placement.isCurrent, // this will be true after approval
                IsApprove = placement.IsApprove,
                IsPend = placement.IsPend,
                Index = placement.Index,
                EmployementType = placement.EmployementType,
                AssignmentDateAmharic = placement.AssignmentDateAmharic,
                EmployeeCode = placement.EmployeeCode,
                OrgBranchName = placement.OrgBranchName,
                PreparedByEmpId = placement.PreparedByEmpId,
                SalarChangeDate = placement.SalarChangeDate,
                NextSalaryChangeDate = placement.NextSalaryChangeDate,
                NextSalaryChangeDateAmharic = placement.NextSalaryChangeDateAmharic,
                SalarChangeDateAmharic = placement.SalarChangeDateAmharic

            };
            return entity;
        }
        public JsonResult AutoCompleteHead(int employeeId, int orgId, int branId, int businUinitId)
        {
            var headId = EmployeeHelper.GetSupervisorEmployeeId(employeeId, orgId, branId, businUinitId);
            var head = db.Person_Employee.Find(headId);

            return Json(new
            {
                headId = headId,
                fullName = head != null
                    ? $"{head.EmployeeCode} - {head.FirstNameEnglish} {head.MiddleNameEnglish} {head.LastNameEnglish}"
                    : ""
            }, JsonRequestBehavior.AllowGet);
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
        public string GetFullName(int empId)
        {
            var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == empId);
            if (employee != null)
            {
                return employee.FullNameaEnglish;
            }

            return "";
        }
        public ActionResult Placements_History_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            List<OldPlacementVM> placeList = new List<OldPlacementVM>();

            var placements = db.Placements.Where(x => x.EmployeeId == id && !x.isCurrent).ToList();//.Where(x => mybraches.Contains(x.BranchId.ToString()));

            foreach (var place in placements)
            {
                var pp = new OldPlacementVM();

                pp.PlaceId = place.PlacementId;
                var branch = db.Branch.FirstOrDefault(x => x.BranchId == place.BranchId);
                if (branch != null)
                {
                    pp.Branch = branch.Name;
                }
                var department = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == place.OrgStructureId);
                if (department != null)
                {
                    pp.Department = department.Name;
                }
                var jobtitle = db.Lookups.FirstOrDefault(x => x.LookupId == place.LookupId && x.LookupType == LookUpTypes.JOB_TITLE);
                if (jobtitle != null)
                {
                    pp.JobTitle = jobtitle.Description;
                }
                pp.LookupId = place.LookupId;
                pp.AssignmentDate = place.AssignmentDateAmharic;
                pp.IsHead = place.IsHead;
                pp.IsCurrent = place.isCurrent;
                pp.Salary = place.Salary;
                pp.AssignmentType = place.AssignmentType;

                placeList.Add(pp);
            }
            DataSourceResult result = placeList.ToDataSourceResult(request, placement => new
            {
                PlaceId = placement.PlaceId,
                Branch = placement.Branch,
                Department = placement.Department,
                LookupId = placement.LookupId,
                JobTitle = placement.JobTitle,
                IsHead = placement.IsHead,
                IsCurrent = placement.IsCurrent,
                AssignmentDate = placement.AssignmentDate,
                Salary = placement.Salary,
                AssignmentType = placement.AssignmentType

            });

            return Json(result);


        }
        public ActionResult Placements_History_Update([DataSourceRequest]DataSourceRequest request, OldPlacementVM oldPlacement, int id)
        {
            if (oldPlacement.IsCurrent)
            {
                ModelState.AddModelError("exist", "This is the current Placement could you can edit it in Placement ");
            }
            var placement = db.Placements.Where(x => x.EmployeeId == id && x.PlacementId == oldPlacement.PlaceId && !x.isCurrent).FirstOrDefault();

            if (placement != null)
            {

               
                placement.LookupId = oldPlacement.LookupId;
                if (oldPlacement.AssignmentDate != null)
                {

                    var splitDateH = oldPlacement.AssignmentDate.Split('/');
                    placement.AssignmentDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateH[0]), Convert.ToInt32(splitDateH[1]), Convert.ToInt32(splitDateH[2]));

                }
                placement.IsHead = oldPlacement.IsHead;
                placement.Salary = oldPlacement.Salary;
                placement.AssignmentType = oldPlacement.AssignmentType;

                db.Entry(placement).State = EntityState.Modified;
                db.SaveChanges();
            }


            return Json(new[] { oldPlacement }.ToDataSourceResult(request, ModelState));



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

            var setting = db.SalarySettings.FirstOrDefault();
            if (setting != null && setting.IsJobtitleIncludedInSalary)
            {
                salary = db.SalariesJobTitles.FirstOrDefault(s => s.RankId == rank && s.StepId == step && s.LookupId == job);
                if (salary != null)
                {
                    return salary.Salary; //Json(salary.SalaryAmount, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                salary = db.SalariesJobTitles.FirstOrDefault(s => s.RankId == rank && s.StepId == step);
                if (salary != null)
                {
                    return salary.Salary; //Json(salary.SalaryAmount, JsonRequestBehavior.AllowGet);
                }
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
        public ActionResult ApproveAllPreviousPlacement()
        {
            var placements = db.Placements.ToList();
            foreach (var place in placements)
            {
                place.IsApprove = true;
                place.ApprovedByEmpId = 0;

                db.Entry(place).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
        public ActionResult Pend(int id)
        {

            var placement = db.Placements.FirstOrDefault(x => x.PlacementId == id);

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
        //public ActionResult ApprovePlacement(int id)
        //{
        //    var placement = db.Placements.FirstOrDefault(x => x.PlacementId == id);

        //    if (placement != null)
        //    {
        //        int i = 0;

        //        var placements = db.Placements.Where(x => x.isCurrent && x.EmployeeId == placement.EmployeeId && x.PlacementId != placement.PlacementId).ToList();
        //        foreach (var p in placements)
        //        {
        //            p.MappingId = 0;
        //            p.JobTitleName = GetTitleName(p.LookupId);
        //            p.ReportsToMappingId = 0;
        //            p.LastModified = DateTime.Now;
        //            p.CreatedBy = p.CreatedBy;
        //            p.ModifiedBy = User.Identity.Name;
        //            p.isCurrent = false;
        //            p.Index = i;
        //            p.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
        //            p.ApprovedByEmpId = p.EmployeeId;

        //            db.Entry(p).State = EntityState.Modified;

        //            i += 1;
        //        }

        //        placement.IsApprove = true;
        //        placement.isCurrent = true;
        //        placement.ApprovedByEmpId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

        //        db.Entry(placement).State = EntityState.Modified;
        //        db.SaveChanges();

        //        var employee = db.Person_Employee.FirstOrDefault(p => p.EmployeeId == placement.EmployeeId);

        //        if (employee != null)
        //        {
        //            employee.IsActive = true;
        //            employee.IsPlaced = true;
        //            employee.OrgBranchName = placement.BranchId.ToString();
        //            employee.ModifiedBy = User.Identity.Name;
        //            employee.LastModified = DateTime.Now;
        //            employee.BranchId = placement.BranchId;

        //            db.Entry(employee).State = EntityState.Modified;
        //            db.SaveChanges();
        //        }

        //        var formattedData = new
        //        {
        //            Message = "You successfully approve this placement.",
        //            Status = "OK"
        //        };
        //        return Json(formattedData, JsonRequestBehavior.AllowGet);
        //    }
        //    var formattedData1 = new
        //    {
        //        Message = "this placement is not approved.",
        //        Status = "FAIL"
        //    };
        //    return Json(formattedData1, JsonRequestBehavior.AllowGet);

        //}
        public ActionResult ApprovePlacement(int id)
        {
            var placement = db.Placements.Find(id);
            // Return a failure response if placement is not found
            if (placement == null)
            {
                return Json(new
                {
                    Message = "This placement does not exist.",
                    Status = "Not-Ok"
                }, JsonRequestBehavior.AllowGet);
            }
            if (placement != null)
            {
                var employeeId = placement.EmployeeId;
                var branchId = placement.BranchId;
                var user = User.Identity.Name;
                placement.IsApprove = true;
                placement.isCurrent = true;
                placement.ApprovedByEmpId = UserInformationHelper.GetEmployeeId(user);
                db.Entry(placement).State = EntityState.Modified;
                db.SaveChanges();
                // Detach placement entity to prevent multiple tracking
                db.Entry(placement).State = EntityState.Detached;
                var placementsToUpdate = db.Placements
                    .Where(x => x.isCurrent
                    && x.EmployeeId == employeeId && x.PlacementId != id)
                    .OrderByDescending(x => x.PlacementId)
                    .ToList();
                var prevPlacment = placementsToUpdate.FirstOrDefault();
                //update prev placement
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
                db.SaveChanges();
                //check if head  assign head name  employee under him
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
                //else//if not head
                //{
                //    // if head 
                //    //var subordinateEmployeeIds = EmployeeHelper.GetSubordinates_FilteredByBranchAlso(entity.EmployeeId).Where(x => x != entity.EmployeeId);
                //    var placedEmployees = db.Placements.Where(x => x.HeadId == placement.EmployeeId && x.isCurrent).ToList();
                //    foreach (var placedEmployee in placedEmployees)
                //    {
                //        var headId = EmployeeHelper.GetSupervisorEmployeeId(placedEmployee.EmployeeId,placedEmployee.OrgStructureId,placedEmployee.BranchId,placedEmployee.BusinessUnitId);
                //        placedEmployee.HeadId = headId;
                //        db.Entry(placedEmployee).State = EntityState.Modified;
                //    }


                //}
                db.SaveChanges();
                #region add to Employee Card
                if (placement.AssignmentType == AssignmentType.NEW)
                {
                    HandlePlacementToEmployeeCard(placement, EmployeeCardType.New_Placement);

                }
                if (placement.AssignmentType != AssignmentType.NEW)
                {
                    HandlePlacementToEmployeeCard(placement, EmployeeCardType.Placement_Change);

                }
                #endregion
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
        private void HandlePlacementToEmployeeCard(Placement placement, EmployeeCardType employeeCardType)
        {
            DateTime now = DateTime.Now;
            var entity = new EmployeeCard
            {
                EmployeeId = placement.EmployeeId,
                ToEmployeeId = null,
                OrgStructureId = placement.OrgStructureId,
                LookupId = placement.LookupId,
                BranchId = placement.BranchId,
                BusinessUnitId = placement.BusinessUnitId,
                EachPrimaryId = placement.PlacementId,
                CardType = employeeCardType,
                IsActive = false,
                ActionTaken = placement.AssignmentType.ToString(),
                Salary = placement.Salary,
                Remark = placement.AssignmentType.ToString(),
                FromDate = DateTime.Now,
                FromDateAm = XAPI.EthiopicDateTime.GetEthiopicDate(now.Day, now.Month, now.Year),
                ToDate = null,
                ToDateTimeAm = null,
                CreatedBy = User.Identity.Name,
                DateCreated = now,
                ModifiedBy = null,
                LastModified = null,
                OrgBranchName = placement.OrgBranchName
            };
            db.EmployeeCards.Add(entity);
            db.SaveChanges();
        }

        public ActionResult ApprovePlacementCC(int id)
        {
            var placement = db.Placements.Find(id);

            if (placement != null)
            {
                var employeeId = placement.EmployeeId;
                var branchId = placement.BranchId;
                var user = User.Identity.Name;
                placement.IsApprove = true;
                placement.isCurrent = true;
                placement.ApprovedByEmpId = UserInformationHelper.GetEmployeeId(user);
                db.Entry(placement).State = EntityState.Modified;
                db.SaveChanges();
                // Detach placement entity to prevent multiple tracking
                db.Entry(placement).State = EntityState.Detached;

                var placementsToUpdate = db.Placements
                    .Where(x => x.isCurrent && x.EmployeeId == employeeId && x.PlacementId != id)
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