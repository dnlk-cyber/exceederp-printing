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

    [AuthorizeRoles(HrmConstants.HRPlacementAdmin, HrmConstants.HRPlacementUser)]
    public class RemovePlacementOfEmployeesController : BaseController
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
        public ActionResult GetBranchs()
        {
            var branches = db.Branch.ToList();

            return Json(branches.Select(p => new
            {
                BranchId = p.BranchId,
                Name = p.Name
            }), JsonRequestBehavior.AllowGet);

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
        public ActionResult RemovePlacementOfEmployees_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);

            int branchId = UserInformationHelper.GetUserBranchByUserName(User.Identity.Name);

            IQueryable<Placement> placements = db.Placements.Where(x =>x.isCurrent);//.Where(x => mybraches.Contains(x.BranchId.ToString()));
             
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
                BusinessUnitId = placement.BusinessUnitId,
                Check = false
            });

            return Json(result);


        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Remove_Group_Placement([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<Placement> placements)
        {
            ModelState.Clear();
           

            if (placements.Any() && ModelState.IsValid)
            {
               
                foreach (var place in placements)
                {
                    if (place.CheckCEO)
                    {
                        place.isCurrent = false;

                        db.Entry(place).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                }

            }

            return Json(placements.ToDataSourceResult(request, ModelState));
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
       
    }
}