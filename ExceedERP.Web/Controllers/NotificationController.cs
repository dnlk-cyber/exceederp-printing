using System;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Notification = ExceedERP.Core.Domain.Common.Notification;

namespace ExceedERP.Web.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Notification()
        {
            return View();
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Preview()
        {
            var unread = CurrentNotifications().Where(notification => !notification.IsRead);
            var count = unread.Count();
            var items = unread.OrderByDescending(notification => notification.Id)
                .Take(10)
                .Select(notification => new { notification.Title, notification.Details, notification.DetailsUrl })
                .ToList()
                .Select(notification => new
                {
                    notification.Title,
                    notification.Details,
                    Url = NotificationUrl(notification.DetailsUrl)
                })
                .ToList();

            return Json(new { Count = count, Items = items }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MarkReadAll()
        {
            foreach (var notification in CurrentNotifications().Where(item => !item.IsRead).ToList())
                notification.IsRead = true;

            _db.SaveChanges();
            return RedirectToAction("Notification", "Notification", new { area = "" });
        }

        public ActionResult Notifications_Read([DataSourceRequest] DataSourceRequest request)
        {
            var result = CurrentNotifications().OrderByDescending(notification => notification.Id)
                .ToDataSourceResult(request, notification => new
                {
                    notification.Id,
                    notification.Type,
                    notification.Title,
                    notification.Details,
                    notification.Date,
                    notification.DeadLine,
                    DetailsUrl = NotificationUrl(notification.DetailsUrl)
                });

            return Json(result);
        }

        private IQueryable<Notification> CurrentNotifications()
        {
            var username = User.Identity.Name;
            return _db.Notifications.Where(notification => notification.SentTo == username && !notification.IsDeleted);
        }

        private string NotificationUrl(string path)
        {
            if (!Url.IsLocalUrl(path) || path.StartsWith("~//", StringComparison.Ordinal)
                || path.Contains("\\") || path.IndexOfAny(new[] { '\r', '\n', '\t' }) >= 0)
                return null;

            // Resolve both formats used by notification senders when the ERP is hosted under a virtual directory.
            var applicationPath = Request.ApplicationPath;
            if (path.StartsWith("~/"))
                return Url.Content(path);
            if (applicationPath != "/" && !path.Equals(applicationPath, StringComparison.OrdinalIgnoreCase)
                && !path.StartsWith(applicationPath + "/", StringComparison.OrdinalIgnoreCase))
                return Url.Content("~" + path);

            return path;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _db.Dispose();

            base.Dispose(disposing);
        }
    }
}
