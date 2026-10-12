using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using System;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.HRM.Prohibition;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Helpers.HR;

namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{
  
    [AuthorizeRoles(HrmConstants.HRRecruitmentUser, HrmConstants.HRRecruitmentAdmin)]
    public class EmployeeProbationController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
      
        //HRMInputs inputs = new HRMInputs();
        private readonly CacheData CacheData = new CacheData();

        // GET: HRM/EmployeeExperience
        public ActionResult Index()
        {

            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            ViewData["EmployeeList"] = CacheData.GetEmployeeCaches().Select(
           i => new
           {
               FullNameAmharic = i.AmharicName,
               FullNameaEnglish = i.Code+"-"+i.Name,
               EmployeeId = i.Id,
               HeadId = i.Id
           }).AsQueryable();
           // ViewData["EmployeeList"] = listt.Where(x => x.IsActive);
            ViewData["Decistion"] = db.Lookups.Where(x=>x.LookupType == LookUpTypes.PROBATION_STATUS);

        }
        public void SetDate(string name, string value)
        {
            if (name == "EndDateAm")
            {
                var EndDate = HrHelper.ParseEthiopicDate(value);
            }


        }
      
        [AuthorizeRoles(HrmConstants.HRRecruitmentUser, HrmConstants.HRRecruitmentAdmin, HrmConstants.HRAdmin)]
        public ActionResult Prohibitions_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
            IQueryable<Prohibition> prohibitions = db.Prohibitions/*.Where(x => mybraches.Contains(x.br))*/;
            DataSourceResult result = prohibitions.ToDataSourceResult(request, prohibition => new
            {
                EmployeeId = prohibition.EmployeeId,
                ProhibitionId = prohibition.ProhibitionId,
                EndDate = prohibition.EndDate,
                LookupId = prohibition.LookupId,
                EvaluationResult = prohibition.EvaluationResult,
                Remark = prohibition.Remark,
                DateCreated = prohibition.DateCreated,
                LastModified = prohibition.LastModified,
                CreatedBy = prohibition.CreatedBy,
                ModifiedBy = prohibition.ModifiedBy,
                EndDateAm = prohibition.EndDateAm,
                EmployeeCode = prohibition.EmployeeCode,
                Decision = prohibition.Decision,
                OrgBranchName = prohibition.OrgBranchName
            });

            return Json(result);
        }
   
        [AuthorizeRoles(HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Prohibitions_Create([DataSourceRequest]DataSourceRequest request, Prohibition prohibition)
        {
            ModelState.Clear();
            if ( string.IsNullOrWhiteSpace(prohibition.EndDateAm))
            {
                ModelState.AddModelError("", "End date is mandatory!!");
                return Json(new[] { prohibition }.ToDataSourceResult(request, ModelState));

            }
            prohibition.EndDate = HrHelper.ParseEthiopicDate(prohibition.EndDateAm);

            var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == prohibition.EmployeeId);
            if (person ==null)
            {
                ModelState.AddModelError("", "employee not found!!");
                return Json(new[] { prohibition }.ToDataSourceResult(request, ModelState));

            }
            if (ModelState.IsValid)
            {
            

                var entity = new Prohibition
                {
                    EmployeeId = prohibition.EmployeeId,
                    EndDate = prohibition. EndDate,
                    LookupId = prohibition.LookupId,
                    EvaluationResult = prohibition.EvaluationResult,
                    Remark = prohibition.Remark,
                    DateCreated = DateTime.Now,
                    LastModified = prohibition.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = prohibition.ModifiedBy,
                    Decision = prohibition.Decision,                        
                    EndDateAm = prohibition.EndDateAm,
                    EmployeeCode = person.EmployeeCode,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)
                };

                db.Prohibitions.Add(entity);
                db.SaveChanges();
                prohibition.ProhibitionId = entity.ProhibitionId;

                #region operation log


                UserHelper.OperationLog(Request.UserHostAddress, "Employee Probation", "Success", User.Identity.Name, "Employee Probation of " + person.EmployeeCode + " is Created By  " + User.Identity.Name,Modules.HRM);
                        
                #endregion
                
            }

            return Json(new[] { prohibition }.ToDataSourceResult(request, ModelState));
        }
      
        [AuthorizeRoles(HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Prohibitions_Update([DataSourceRequest]DataSourceRequest request, Prohibition prohibition)
        {
            ModelState.Clear();
            if (string.IsNullOrWhiteSpace(prohibition.EndDateAm))
            {
                ModelState.AddModelError("", "End date is mandatory!!");
                return Json(new[] { prohibition }.ToDataSourceResult(request, ModelState));

            }
            var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == prohibition.EmployeeId);
            if (person == null)
            {
                ModelState.AddModelError("", "employee not found!!");
                return Json(new[] { prohibition }.ToDataSourceResult(request, ModelState));

            }
            prohibition.EndDate = HrHelper.ParseEthiopicDate(prohibition.EndDateAm);
            if (ModelState.IsValid)
            {
                
                var entity = new Prohibition
                {
                    EmployeeId = prohibition.EmployeeId,
                    ProhibitionId = prohibition.ProhibitionId,
                    EndDate = prohibition.EndDate,
                    LookupId = prohibition.LookupId,
                    EvaluationResult = prohibition.EvaluationResult,
                    Remark = prohibition.Remark,
                    DateCreated = prohibition.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = prohibition.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    EndDateAm = prohibition.EndDateAm,
                    Decision = prohibition.Decision,
                    EmployeeCode = person.EmployeeCode,
                    OrgBranchName = prohibition.OrgBranchName
                };

                db.Prohibitions.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log


                UserHelper.OperationLog(Request.UserHostAddress, "Employee Probation", "Success", User.Identity.Name, "Probation of " + person.EmployeeCode + " is Updated By  " + User.Identity.Name,Modules.HRM);
                    
                
                #endregion
            }

            return Json(new[] { prohibition }.ToDataSourceResult(request, ModelState));
        }
       
        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Prohibitions_Destroy([DataSourceRequest]DataSourceRequest request, Prohibition prohibition)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new Prohibition
                {
                    EmployeeId = prohibition.EmployeeId,
                    ProhibitionId = prohibition.ProhibitionId,
                    EndDate = prohibition.EndDate,
                    LookupId = prohibition.LookupId,
                    EvaluationResult = prohibition.EvaluationResult,
                    Remark = prohibition.Remark,
                    DateCreated = prohibition.DateCreated,
                    LastModified = prohibition.LastModified,
                    CreatedBy = prohibition.CreatedBy,
                    ModifiedBy = prohibition.ModifiedBy,
                    Decision = prohibition.Decision,
                    EndDateAm = prohibition.EndDateAm,
                    EmployeeCode = prohibition.EmployeeCode,
                    OrgBranchName = prohibition.OrgBranchName
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Probation information of employee Id" + prohibition.EmployeeCode);
                #endregion
                db.Prohibitions.Attach(entity);
                db.Prohibitions.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { prohibition }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}