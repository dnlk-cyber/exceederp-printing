using System;
using ExceedERP.Core.Domain.Dashboard;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.HealthAndInsurance;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Controllers
{

  

    public class NotificationController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

     
        public ActionResult Notification()
        {
            
            return View();
        }

        public ActionResult MarkReadAll()
        {
            var unread = _db.Notifications.Where(x => x.SentTo == User.Identity.Name).Where(x=>x.IsRead==false).ToList();
            foreach (var u in unread)
            {
                u.IsRead =true;
                _db.Entry(u).State = EntityState.Modified;
                _db.SaveChanges();
            }
            return RedirectToAction("Notification","Notification");
        }

        public ActionResult Notifications_Read([DataSourceRequest] DataSourceRequest request)
        {
            IQueryable<Core.Domain.Common.Notification> notifications = _db.Notifications.Where(x=>x.SentTo==User.Identity.Name);
            var result = notifications.ToDataSourceResult(request, notification => new
            {
                Id = notification.Id,
                Type = notification.Type,
                Title = notification.Title,
                Details = notification.Details,
                Date = notification.Date,
                DetailsUrl = notification.DetailsUrl

            });

            return Json(result);
        }

    }
}