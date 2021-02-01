
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using System;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{

    [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRProfileUser)]
    public class EmployeeEmergencyController : BaseController
    {
        // GET: HRM/EmployeeExperience
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {

            return View();
        }
       
        [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRProfileUser, HrmConstants.HRAdmin)]
        public ActionResult Emergency_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<Emergency> emergencies = db.Emergencies.Where(c => c.EmployeeId == id);
            DataSourceResult result = emergencies.ToDataSourceResult(request, emergency => new
            {
                EmergencyId = emergency.EmergencyId,
                Name = emergency.Name,
                LookupId = emergency.LookupId,
                EmployeeId = emergency.EmployeeId,
                HomePhoneNo = emergency.HomePhoneNo,
                MobilePhoneNo = emergency.MobilePhoneNo,
                WorkPhoneno = emergency.WorkPhoneno,
                Remark = emergency.Remark,
                DateCreated = emergency.DateCreated,
                LastModified = emergency.LastModified,
                CreatedBy = emergency.CreatedBy,
                ModifiedBy = emergency.ModifiedBy
            });

            return Json(result);
        }
       
        [AuthorizeRoles(HrmConstants.HRProfileUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Emergency_Create([DataSourceRequest]DataSourceRequest request, Emergency emergency, int id)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new Emergency
                {
                    EmergencyId = emergency.EmergencyId,
                    Name = emergency.Name,
                    LookupId = emergency.LookupId,
                    EmployeeId = id,
                    HomePhoneNo = emergency.HomePhoneNo,
                    MobilePhoneNo = emergency.MobilePhoneNo,
                    WorkPhoneno = emergency.WorkPhoneno,
                    Remark = emergency.Remark,
                    DateCreated = DateTime.Now,
                    LastModified = emergency.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = emergency.ModifiedBy
                };

                db.Emergencies.Add(entity);
                db.SaveChanges();
                emergency.EmergencyId = entity.EmergencyId;

                #region operation log
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                if (person != null)
                {
                        UserHelper.OperationLog(Request.UserHostAddress, "Employee Emergency", "Success", User.Identity.Name, "Emergency Info for " + person.EmployeeCode + " is Created By  " + User.Identity.Name,Modules.HRM);
                    
                }
                #endregion
            }

            return Json(new[] { emergency }.ToDataSourceResult(request, ModelState));
        }
       
        [AuthorizeRoles(HrmConstants.HRProfileUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Emergency_Update([DataSourceRequest]DataSourceRequest request, Emergency emergency)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new Emergency
                {
                    EmergencyId = emergency.EmergencyId,
                    Name = emergency.Name,
                    LookupId = emergency.LookupId,
                    EmployeeId = emergency.EmployeeId,
                    HomePhoneNo = emergency.HomePhoneNo,
                    MobilePhoneNo = emergency.MobilePhoneNo,
                    WorkPhoneno = emergency.WorkPhoneno,
                    Remark = emergency.Remark,
                    DateCreated = emergency.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = emergency.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.Emergencies.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                if (person != null)
                {
                        UserHelper.OperationLog(Request.UserHostAddress, "Employee Emergency", "Success", User.Identity.Name, "Emergency Info of " + person.EmployeeCode + " is Updated By  " + User.Identity.Name,Modules.HRM);
                    
                }
                #endregion
            }

            return Json(new[] { emergency }.ToDataSourceResult(request, ModelState));
        }
      
        [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Emergency_Destroy([DataSourceRequest]DataSourceRequest request, Emergency emergency)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new Emergency
                {
                    EmergencyId = emergency.EmergencyId,
                    Name = emergency.Name,
                    LookupId = emergency.LookupId,
                    EmployeeId = emergency.EmployeeId,
                    HomePhoneNo = emergency.HomePhoneNo,
                    MobilePhoneNo = emergency.MobilePhoneNo,
                    WorkPhoneno = emergency.WorkPhoneno,
                    Remark = emergency.Remark,
                    DateCreated = emergency.DateCreated,
                    LastModified = emergency.LastModified,
                    CreatedBy = emergency.CreatedBy,
                    ModifiedBy = emergency.ModifiedBy
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted emergency information of employee Id" + emergency.EmergencyId);
                #endregion
                db.Emergencies.Attach(entity);
                db.Emergencies.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { emergency }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}