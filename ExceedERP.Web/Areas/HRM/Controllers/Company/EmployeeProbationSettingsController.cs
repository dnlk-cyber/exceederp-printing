
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM.Company;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Company
{
    [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRCompanyUser)]
    public class EmployeeProbationSettingsController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
       
        public ActionResult Index()
        {
            return View();
        }


        [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRCompanyUser, HrmConstants.HRAdmin)]
        public ActionResult EmployeeProbationSettings_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<EmployeeProbationSetting> employeeProbations = db.EmployeeProbationSettings;
            DataSourceResult result = employeeProbations.ToDataSourceResult(request, employeeProbation => new
            {
                EmployeeProbationSettingId = employeeProbation.EmployeeProbationSettingId,
                IncludeWeekend = employeeProbation.IncludeWeekend,
                IncludeHoliday = employeeProbation.IncludeHoliday,
                DateCreated = employeeProbation.DateCreated,
                LastModified = employeeProbation.LastModified,
                CreatedBy = employeeProbation.CreatedBy,
                ModifiedBy = employeeProbation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(HrmConstants.HRCompanyUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeProbationSettings_Create([DataSourceRequest]DataSourceRequest request, EmployeeProbationSetting employeeProbation)
        {
            ModelState.Clear();
            var exist = db.EmployeeProbationSettings.FirstOrDefault();
            if(exist != null)
            {
                ModelState.AddModelError("", "You can't Add More");
            }

            if (ModelState.IsValid)
            {
                
                var entity = GetMappedObject(employeeProbation);
                db.EmployeeProbationSettings.Add(entity);
                db.SaveChanges();
                employeeProbation.EmployeeProbationSettingId = entity.EmployeeProbationSettingId;

                #region operation log

                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                        
                    UserHelper.OperationLog(Request.UserHostAddress, "Probtion Setting ", "Success", User.Identity.Name, User.Identity.Name + " Add new Probtion Setting  " + entity.EmployeeProbationSettingId,Modules.HRM);

                        

                #endregion
               
                
            }

            return Json(new[] { employeeProbation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRCompanyUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeProbationSettings_Update([DataSourceRequest]DataSourceRequest request, EmployeeProbationSetting employeeProbation)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                
                var entity = GetMappedObject(employeeProbation);
                db.EmployeeProbationSettings.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
              
                    UserHelper.OperationLog(Request.UserHostAddress, "Probation Setting ", "Success", User.Identity.Name, User.Identity.Name + " Updated Probation Setting Info of  created by Employee Id" + empId.ToString(),Modules.HRM);

                #endregion

            }

            return Json(new[] { employeeProbation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PEmployeeProbationSetting_Destroy([DataSourceRequest]DataSourceRequest request, EmployeeProbationSetting employeeProbation )
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(employeeProbation);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Employee Probation Setting " + employeeProbation.EmployeeProbationSettingId);
                #endregion
                db.EmployeeProbationSettings.Attach(entity);
                db.EmployeeProbationSettings.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { employeeProbation }.ToDataSourceResult(request, ModelState));
        }

        private EmployeeProbationSetting GetMappedObject(EmployeeProbationSetting employeeProbation)
        {
            var entity = new EmployeeProbationSetting
            {
                EmployeeProbationSettingId = employeeProbation.EmployeeProbationSettingId,
                IncludeWeekend = employeeProbation.IncludeWeekend,
                IncludeHoliday = employeeProbation.IncludeHoliday,
                DateCreated = employeeProbation.DateCreated,
                LastModified = employeeProbation.LastModified,
                CreatedBy = employeeProbation.CreatedBy,
                ModifiedBy = employeeProbation.ModifiedBy
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
