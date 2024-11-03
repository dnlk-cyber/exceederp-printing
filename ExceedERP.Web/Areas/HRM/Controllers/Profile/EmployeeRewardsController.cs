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
    public class EmployeeRewardsController : BaseController
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
        public ActionResult EmployeeRewards_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            IQueryable<EmployeeReward> employeeRewards = db.EmployeeRewards.Where(x => mybraches.Contains(x.OrgBranchName));
            DataSourceResult result = employeeRewards.ToDataSourceResult(request, employeeReward => new
            {
                EmployeeRewardId = employeeReward.EmployeeRewardId,
                EmployeeId = employeeReward.EmployeeId,
                BranchId = employeeReward.BranchId,
                RewardType = employeeReward.RewardType,
                Amount = employeeReward.Amount,
                Remark = employeeReward.Remark,
                Date = employeeReward.Date,
                DateAm = employeeReward.DateAm,
                ReasonForReward = employeeReward.ReasonForReward,
                SubBranch = employeeReward.SubBranch,
                Unit = employeeReward.Unit,
                AwardedBy = employeeReward.AwardedBy,
                CreatedBy = employeeReward.CreatedBy,
                DateCreated = employeeReward.DateCreated,
                ModifiedBy = employeeReward.ModifiedBy,
                LastModified = employeeReward.LastModified,
                OrgBranchName = employeeReward.OrgBranchName
            });

            return Json(result);
        }
    
        [AuthorizeRoles(HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeRewards_Create([DataSourceRequest]DataSourceRequest request, EmployeeReward employeeReward)
        {
            if (!string.IsNullOrEmpty(employeeReward.DateAm))
            {
                var splitDate = employeeReward.DateAm.Split('/');
                employeeReward.Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
          
            
            if (ModelState.IsValid)
            {
                
                
                employeeReward.CreatedBy = User.Identity.Name;
                employeeReward.DateCreated = DateTime.Now;
                employeeReward.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                var entity = GetMappedObject(employeeReward);
                db.EmployeeRewards.Add(entity);
                db.SaveChanges();
                employeeReward.EmployeeRewardId = entity.EmployeeRewardId;
                #region operation log
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                if (person != null)
                {
                        UserHelper.OperationLog(Request.UserHostAddress, "Employee employeeReward", "Success", User.Identity.Name, "Employee employeeReward for " + person.EmployeeCode + " is Created By  " + User.Identity.Name,Modules.HRM);
                        
                }
                #endregion
                
            }

            return Json(new[] { employeeReward }.ToDataSourceResult(request, ModelState));
        }
     
        [AuthorizeRoles( HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeRewards_Update([DataSourceRequest]DataSourceRequest request, EmployeeReward employeeReward)
        {
            if (!string.IsNullOrEmpty(employeeReward.DateAm))
            {
                var splitDate = employeeReward.DateAm.Split('/');
                employeeReward.Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            if (ModelState.IsValid)
            {
               
                var entity = GetMappedObject(employeeReward);
                db.EmployeeRewards.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                if (person != null)
                {

                    UserHelper.OperationLog(Request.UserHostAddress, "Employee employeeReward", "Success", User.Identity.Name, "Emoployee employeeReward of " + person.EmployeeCode + " is Updated By  " + User.Identity.Name,Modules.HRM);
                        
                }
                #endregion
                
            }

            return Json(new[] { employeeReward }.ToDataSourceResult(request, ModelState));
        }
      
        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin,HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeRewards_Destroy([DataSourceRequest]DataSourceRequest request, EmployeeReward employeeReward)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(employeeReward);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employeeReward information of employee Id" + employeeReward.Employee.EmployeeCode);
                #endregion
                db.EmployeeRewards.Attach(entity);
                db.EmployeeRewards.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { employeeReward }.ToDataSourceResult(request, ModelState));
        }
        private EmployeeReward GetMappedObject(EmployeeReward employeeReward)
        {
            var entity = new EmployeeReward
            {
                EmployeeRewardId = employeeReward.EmployeeRewardId,
                EmployeeId = employeeReward.EmployeeId,
                BranchId = employeeReward.BranchId,
                RewardType = employeeReward.RewardType,
                Amount = employeeReward.Amount,
                Remark = employeeReward.Remark,
                Date = employeeReward.Date,
                DateAm = employeeReward.DateAm,
                ReasonForReward = employeeReward.ReasonForReward,
                SubBranch = employeeReward.SubBranch,
                Unit = employeeReward.Unit,
                AwardedBy = employeeReward.AwardedBy,
                CreatedBy = employeeReward.CreatedBy,
                DateCreated = employeeReward.DateCreated,
                ModifiedBy = employeeReward.ModifiedBy,
                LastModified = employeeReward.LastModified,
                OrgBranchName = employeeReward.OrgBranchName
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