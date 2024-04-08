using ExceedERP.Core.Domain.Archive;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Archive.Controllers
{
    public class ArchiveDataProviderController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        public JsonResult GetInstitutionType()
        {
            var institution = db.InstitutionType.AsQueryable();
            return Json(institution.Select(p => new
            {
                InstitutionName = p.InstitutionName,
                InstitutionTypeId = p.InstitutionTypeId,
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSubjectType()
        {
            IQueryable<SubjectType> subjectTypes = db.SubjectTypes;
            return Json(subjectTypes.Select(p => new
            {
                SubjectTypeId = p.SubjectTypeId,
                SubjectTypeName = p.SubjectTypeName
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBranches()
        {

            var branches = db.Branch.ToList();
            return Json(branches.Select(p => new
            {
                BranchId = p.BranchId,
                Name = p.AmharicName,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDepartments()
        {
            var structures = db.MainCompanyStructures.ToList();
            return Json(structures.Select(p => new
            {
                OrgStructureId = p.OrgStructureId,
                Name = p.Name

            }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetEmployees([DataSourceRequest] DataSourceRequest request)
        {
            var employees = db.Person_Employee.ToList();

            return Json(employees.OrderBy(x => x.EmployeeCode).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllEmployees()
        {
            var employees = db.Person_Employee.Select(x => new { x.EmployeeCode, x.EmployeeId, x.FirstName, x.MiddleName, x.LastName }).ToList();
            return Json(employees.Select(p => new
            {
                EmployeeId = p.EmployeeId,
                FullNameAmharic = p.EmployeeCode + " " + p.FirstName + " " + p.MiddleName + " " + p.LastName,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEmployeesInYourBranchAndDepartment()
        {
            string username = User.Identity.Name;
            int orgId = UserInformationHelper.GetEmployeeDepartmentId(username);
            int branchId = UserInformationHelper.GetEmployeeBranchId(username);

            var employeesInOndeDepartment = db.Placements.Where(x => x.isCurrent && x.OrgStructureId == orgId && x.BranchId == branchId).Select(x => x.EmployeeId).ToList();

            var employees = db.Person_Employee.Where(x => employeesInOndeDepartment.Contains(x.EmployeeId)).Select(x => new { x.EmployeeCode, x.EmployeeId, x.FirstName, x.MiddleName, x.LastName }).ToList();
            return Json(employees.Select(p => new
            {
                EmployeeId = p.EmployeeId,
                FullNameAmharic = p.EmployeeCode + " " + p.FirstName + " " + p.MiddleName + " " + p.LastName,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEmployeesInYourBranchAndDepartmentAndHeadsOneLevelBelow()
        {
            string username = User.Identity.Name;
            int orgId = UserInformationHelper.GetEmployeeDepartmentId(username);
            int branchId = UserInformationHelper.GetEmployeeBranchId(username);
            var childHeads = db.MainCompanyStructures.Where(x => x.Parent == orgId).Select(x => x.OrgStructureId).ToList();
            var employeesInOndeDepartment = db.Placements.Where(x => x.OrgStructureId == orgId).Where(x => x.isCurrent && x.BranchId == branchId).Select(x => x.EmployeeId).ToList();
            var headInChildDepartment = db.Placements.Where(x => childHeads.Contains(x.OrgStructureId) && x.IsHead).Where(x => x.isCurrent && x.BranchId == branchId).Select(x => x.EmployeeId).ToList();

            var employees = db.Person_Employee.Where(x => employeesInOndeDepartment.Contains(x.EmployeeId) || headInChildDepartment.Contains(x.EmployeeId)).Select(x => new { x.EmployeeCode, x.EmployeeId, x.FirstName, x.MiddleName, x.LastName }).ToList();
            return Json(employees.Select(p => new
            {
                EmployeeId = p.EmployeeId,
                FullNameAmharic = p.EmployeeCode + " " + p.FirstName + " " + p.MiddleName + " " + p.LastName,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDepartmentLevel_SubjectTypes()
        {
            var subjectTypes = db.SubjectTypeForDepartmentLevels;
            return Json(subjectTypes.Select(p => new
            {
                SubjectTypeForDepartmentLevelId = p.SubjectTypeForDepartmentLevelId,
                Name = p.Name
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCompanyDocuments()
        {
            var companyDocuments = db.CompanyDocumentTypes.ToList();
            return Json(companyDocuments.Select(p => new
            {
                CompanyDocumentTypeId = p.CompanyDocumentTypeId,
                DocumentType = p.DocumentType
            }), JsonRequestBehavior.AllowGet);
        }
    }
}