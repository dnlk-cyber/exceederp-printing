using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using System;
using ExceedERP.Core.Domain.InfoDesk.License;
using ExceedERP.Web.Models;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Web.Hubs;

namespace ExceedERP.Web.Areas.InfoDesk.Controllers.License
{
    [Authorize]
    public class AssignedUsersController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private ApplicationDbContext _user = new ApplicationDbContext();

        private bool GetNotificationStatus(string userName, int softwareLicenseId, int? employeeId)
        {
            var licence = db.SoftwareLicenses.Find(softwareLicenseId);
            if (licence == null)
            {
                return false;
            }
            if(employeeId ==null || employeeId == 0)
            {
                employeeId = UserInformationHelper.GetEmployeeId(userName);
            }
            if(licence.RenewalNotificationDate.HasValue && licence.RenewalNotificationDate.Value.Date <= DateTime.Now)
            {
                NotifModels notif = new NotifModels
                {
                    UserId = UserInformationHelper.GetEmployeeUserId((int)employeeId),
                    Title = $"Software Licenses Expire for Name: {licence.SoftwareName} ",
                    Message = $" Software Licenses Expire for Name: {licence.SoftwareName} Id:{licence.LicenseId}",
                    URL = "/InfoDesk/SoftwareLicenses"
                };

                Notify.NotifyUser(notif);
                return true;
            }
            else
            {
                return false;
            }
          
        }
       
        public ActionResult AssignedUsers_Read([DataSourceRequest]DataSourceRequest request,int id)
        {
            IQueryable<AssignedUser> assignedUsers = db.AssignedUsers.Where(x=>x.SoftwareLicenseId==id);
            DataSourceResult result = assignedUsers.ToDataSourceResult(request, assignedUser => new {
                AssignedUserId = assignedUser.AssignedUserId,
                SoftwareLicenseId = assignedUser.SoftwareLicenseId,
                UserName = assignedUser.UserName,
                EmployeeId = assignedUser.EmployeeId,
                //UserId = assignedUser.UserId,
                Remark = assignedUser.Remark,
                DateCreated = assignedUser.DateCreated,
                LastModified = assignedUser.LastModified,
                CreatedBy = assignedUser.CreatedBy,
                IsNotified = GetNotificationStatus(assignedUser.UserName,assignedUser.SoftwareLicenseId,assignedUser.EmployeeId),
                ModifiedBy = assignedUser.ModifiedBy

            });
            return Json(result);
        }
     
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssignedUsers_Create([DataSourceRequest]DataSourceRequest request, AssignedUser assignedUser, int id)
        {
            var licence = db.SoftwareLicenses.Find(id);
            if (licence == null)
            {
                ModelState.AddModelError("exist", "No licence Found");
            }
            assignedUser.SoftwareLicenseId = id;
            assignedUser.UserName = UserInformationHelper.GetEmployeeUserId((int)assignedUser.EmployeeId)?.Trim();
            //var userId = UserInformationHelper.GetUserId(assignedUser.UserName)?.Trim();

            if (ModelState.IsValid)
            {
                assignedUser.DateCreated = DateTime.Now;
                assignedUser.CreatedBy = User.Identity.Name;
                var entity = GetMappedObject(assignedUser);
                db.AssignedUsers.Add(entity);
                db.SaveChanges();
                assignedUser.AssignedUserId = entity.AssignedUserId;
                if (licence.RenewalNotificationDate.HasValue && licence.RenewalNotificationDate.Value.Date <= DateTime.Now)
                {
                    NotifModels notif = new NotifModels
                    {
                        UserId = UserInformationHelper.GetEmployeeUserId((int)assignedUser.EmployeeId),
                        Title = $"Software Licenses Expire for Name: {licence.SoftwareName} ",
                        Message = $" Software Licenses Expire for Name: {licence.SoftwareName} Id:{licence.LicenseId}",
                        URL = "/InfoDesk/SoftwareLicenses"
                    };

                    Notify.NotifyUser(notif);
                  
                }

            }
            return Json(new[] { assignedUser }.ToDataSourceResult(request, ModelState));
        }
      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssignedUsers_Update([DataSourceRequest]DataSourceRequest request, AssignedUser assignedUser)
        {
            assignedUser.UserName = UserInformationHelper.GetEmployeeUserId((int)assignedUser.EmployeeId)?.Trim();

            if (ModelState.IsValid)
            {
                assignedUser.LastModified = DateTime.Now;
                assignedUser.ModifiedBy = User.Identity.Name;
                var entity = GetMappedObject(assignedUser);
                db.AssignedUsers.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { assignedUser }.ToDataSourceResult(request, ModelState));
        }
      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssignedUsers_Destroy([DataSourceRequest]DataSourceRequest request, AssignedUser assignedUser)
        {

            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(assignedUser);
                db.AssignedUsers.Attach(entity);
                db.AssignedUsers.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { assignedUser }.ToDataSourceResult(request, ModelState));
        }

        private AssignedUser GetMappedObject(AssignedUser assignedUser)
        {
            var entity = new AssignedUser
            {
                AssignedUserId = assignedUser.AssignedUserId,
                SoftwareLicenseId = assignedUser.SoftwareLicenseId,
                UserName = assignedUser.UserName,
                EmployeeId = assignedUser.EmployeeId,
                //UserId = assignedUser.UserId,
                Remark = assignedUser.Remark,
                DateCreated = assignedUser.DateCreated,
                LastModified = assignedUser.LastModified,
                CreatedBy = assignedUser.CreatedBy,
                ModifiedBy = assignedUser.ModifiedBy

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
