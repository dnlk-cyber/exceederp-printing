using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Areas.Common.Controllers;
using System.IO;
using System.Net;
using System.Web;
using ExceedERP.Web.Controllers;
using System;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{
   
    [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRRecruitmentUser)]
    public class EmployeeCardsController : BaseController
    {
        // GET: HRM/EmployeeExperience
        private ExceedDbContext db = new ExceedDbContext();
        public static DateTime AgreementDate { get; set; }
        public static string AgreementDateAm { get; set; }
        HRMInputs inputs = new HRMInputs();
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            var employeeIds = db.Placements.Select(x => x.EmployeeId).ToList();
            ViewData["EmployeeList"] = db.Person_Employee.Where(x => employeeIds.Contains(x.EmployeeId)).Select(x => new { EmployeeId = x.EmployeeId, ToEmployeeId=x.EmployeeId, HeadId = x.EmployeeId, EmployeeCode = x.EmployeeCode, FullNameAmharic = x.EmployeeCode + " - " + x.FirstName + " " + x.MiddleName + " " + x.LastName, FullNameaEnglish = x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish }).ToList();// inputs.GetEmployeesForPlacment(User.Identity.Name);
            ViewData["jobTitle"] = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE).ToList();
            var branches = db.Branch.Select(x => new { x.BranchId, Name = x.Name.Replace("\n", "").Replace("\r", "").Replace("#", "").Replace("+", " ") });
            ViewData["branches"] = branches;
            var businessUnits = db.BusinessUnits.Select(x => new { x.BusinessUnitId, Name = x.Name.Replace("\n", "").Replace("\r", "").Replace("#", "").Replace("+", " ") });
            ViewData["businessUnits"] = businessUnits;
            var workunits = db.MainCompanyStructures.Select(x => new { x.OrgStructureId, Name = x.Name.Replace("\n", "").Replace("\r", "").Replace("#", "").Replace("+", " ") }).ToList();
            ViewData["workunits"] = workunits;
            var alloanceList = db.AllowanceDefinations.AsQueryable();
            ViewData["AllowanceList"] = alloanceList.OrderBy(o => o.AllowanceName);
        }
    
        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRRecruitmentUser, HrmConstants.HRAdmin)]
        public ActionResult EmployeeCards_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            IQueryable<EmployeeCard> employeeCards = db.EmployeeCards.Where(x => mybraches.Contains(x.OrgBranchName));
            DataSourceResult result = employeeCards.ToDataSourceResult(request, employeeCard => new
            {
                EmployeeCardId = employeeCard.EmployeeCardId,
                EmployeeId = employeeCard.EmployeeId,
                ToEmployeeId = employeeCard.ToEmployeeId,
                OrgStructureId = employeeCard.OrgStructureId,
                LookupId = employeeCard.LookupId,
                BranchId = employeeCard.BranchId,
                BusinessUnitId = employeeCard.BusinessUnitId,
                EachPrimaryId = employeeCard.EachPrimaryId,
                CardType = employeeCard.CardType,
                IsActive = employeeCard.IsActive,
                ActionTaken = employeeCard.ActionTaken,
                Salary = employeeCard.Salary,
                Remark = employeeCard.Remark,
                FromDate = employeeCard.FromDate,
                FromDateAm = employeeCard.FromDateAm,
                ToDate = employeeCard.ToDate,
                ToDateTimeAm = employeeCard.ToDateTimeAm,
                CreatedBy = employeeCard.CreatedBy,
                DateCreated = employeeCard.DateCreated,
                ModifiedBy = employeeCard.ModifiedBy,
                LastModified = employeeCard.LastModified,
                OrgBranchName = employeeCard.OrgBranchName
            });

            return Json(result);
        }
    
        [AuthorizeRoles(HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeCards_Create([DataSourceRequest]DataSourceRequest request, EmployeeCard employeeCard)
        {
            if (!string.IsNullOrEmpty(employeeCard.FromDateAm))
            {
                var splitDate = employeeCard.FromDateAm.Split('/');
                employeeCard.FromDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            if (!string.IsNullOrEmpty(employeeCard.ToDateTimeAm))
            {
                var splitDate = employeeCard.ToDateTimeAm.Split('/');
                employeeCard.ToDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            if (employeeCard.FromDate > employeeCard.ToDate)
            {

                ModelState.AddModelError("", "from date should be less or equal to the to date!!");
            }
            if (ModelState.IsValid)
            {
                
                
                employeeCard.CreatedBy = User.Identity.Name;
                employeeCard.DateCreated = DateTime.Now;
                employeeCard.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                var entity = GetMappedObject(employeeCard);
                db.EmployeeCards.Add(entity);
                db.SaveChanges();
                employeeCard.EmployeeCardId = entity.EmployeeCardId;
                #region operation log
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                if (person != null)
                {
                        UserHelper.OperationLog(Request.UserHostAddress, "Employee employeeCard", "Success", User.Identity.Name, "Employee employeeCard for " + person.EmployeeCode + " is Created By  " + User.Identity.Name,Modules.HRM);
                        
                }
                #endregion
                
            }

            return Json(new[] { employeeCard }.ToDataSourceResult(request, ModelState));
        }
     
        [AuthorizeRoles( HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeCards_Update([DataSourceRequest]DataSourceRequest request, EmployeeCard employeeCard)
        {
            if (!string.IsNullOrEmpty(employeeCard.FromDateAm))
            {
                var splitDate = employeeCard.FromDateAm.Split('/');
                employeeCard.FromDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            if (!string.IsNullOrEmpty(employeeCard.ToDateTimeAm))
            {
                var splitDate = employeeCard.ToDateTimeAm.Split('/');
                employeeCard.ToDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            if (employeeCard.FromDate > employeeCard.ToDate)
            {

                ModelState.AddModelError("", "from date should be less or equal to the to date!!");
            }
            if (ModelState.IsValid)
            {
               
                var entity = GetMappedObject(employeeCard);
                db.EmployeeCards.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                if (person != null)
                {

                    UserHelper.OperationLog(Request.UserHostAddress, "Employee employeeCard", "Success", User.Identity.Name, "Emoployee employeeCard of " + person.EmployeeCode + " is Updated By  " + User.Identity.Name,Modules.HRM);
                        
                }
                #endregion
                
            }

            return Json(new[] { employeeCard }.ToDataSourceResult(request, ModelState));
        }
      
        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin,HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeCards_Destroy([DataSourceRequest]DataSourceRequest request, EmployeeCard employeeCard)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(employeeCard);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employeeCard information of employee Id" + employeeCard.Employee.EmployeeCode);
                #endregion
                db.EmployeeCards.Attach(entity);
                db.EmployeeCards.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { employeeCard }.ToDataSourceResult(request, ModelState));
        }
        private EmployeeCard GetMappedObject(EmployeeCard employeeCard)
        {
            var entity = new EmployeeCard
            {
                EmployeeCardId = employeeCard.EmployeeCardId,
                EmployeeId = employeeCard.EmployeeId,
                ToEmployeeId = employeeCard.ToEmployeeId,
                OrgStructureId = employeeCard.OrgStructureId,
                LookupId = employeeCard.LookupId,
                BranchId = employeeCard.BranchId,
                BusinessUnitId = employeeCard.BusinessUnitId,
                EachPrimaryId = employeeCard.EachPrimaryId,
                CardType = employeeCard.CardType,
                IsActive = employeeCard.IsActive,
                ActionTaken = employeeCard.ActionTaken,
                Salary = employeeCard.Salary,
                Remark = employeeCard.Remark,
                FromDate = employeeCard.FromDate,
                FromDateAm = employeeCard.FromDateAm,
                ToDate = employeeCard.ToDate,
                ToDateTimeAm = employeeCard.ToDateTimeAm,
                CreatedBy = employeeCard.CreatedBy,
                DateCreated = employeeCard.DateCreated,
                ModifiedBy = employeeCard.ModifiedBy,
                LastModified = employeeCard.LastModified,
                OrgBranchName = employeeCard.OrgBranchName
            };
            return entity;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}