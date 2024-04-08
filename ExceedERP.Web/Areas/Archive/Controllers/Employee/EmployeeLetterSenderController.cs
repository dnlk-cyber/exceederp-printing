using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Models;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Archive;
using ExceedERP.Web.Areas.HRM.Controllers;
using ExceedERP.Web.Hubs;

namespace ExceedERP.Web.Areas.Archive.Controllers
{

    public class EmployeeLetterSenderController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            var context = new ApplicationDbContext();

            var allUsers = context.Users.ToList();
            ViewData["Users"] = allUsers;
            return View();
        }
        public ActionResult EmployeeLetterSender_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var employeeLetterSenders = db.EmployeeLetterSender.Where(x => x.EmployeeIncomingLetterId == id).ToList();
            DataSourceResult result = employeeLetterSenders.ToDataSourceResult(request, employeeLetterSender => new
            {
                LetterSenderId = employeeLetterSender.LetterSenderId,
                Date = employeeLetterSender.Date,
                Receiver = employeeLetterSender.Receiver,
                Description = employeeLetterSender.Description,
                Status = employeeLetterSender.Status,
                DateAmharic = employeeLetterSender.DateAmharic,
                IsTransfered = employeeLetterSender.IsTransfered,
                Time = employeeLetterSender.Time
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeLetterSender_Create([DataSourceRequest]DataSourceRequest request, EmployeeLetterSender employeeLetterSender, int id)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                string time = DateTime.Now.ToShortTimeString();
                var entity = new EmployeeLetterSender
                {
                    EmployeeIncomingLetterId = id,
                    Date = DateTime.Now.Date,
                    Time = time,
                    Receiver = employeeLetterSender.Receiver,
                    Description = employeeLetterSender.Description,
                    Status = LettersStatus.NEW,
                    DateAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year),
                    IsTransfered = employeeLetterSender.IsTransfered
                };

                db.EmployeeLetterSender.Add(entity);
                db.SaveChanges();
                employeeLetterSender.LetterSenderId = entity.LetterSenderId;

                #region operation log

                //    var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

                //       UserHelper.OperationLog(Request.UserHostAddress, "Sent Letter", "Success", User.Identity.Name, User.Identity.Name + " add send new letter to " + entity.OwnerName + " at date " + DateTime.Today,Modules.ArchiveAndLetters);

                #endregion
            }

            return Json(new[] { employeeLetterSender }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeLetterSender_Update([DataSourceRequest]DataSourceRequest request, EmployeeLetterSender EmployeeLetterSender)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new EmployeeLetterSender
                {
                    LetterSenderId = EmployeeLetterSender.LetterSenderId,
                    Date = EmployeeLetterSender.Date,
                    DateAmharic = EmployeeLetterSender.DateAmharic,
                    Receiver = EmployeeLetterSender.Receiver,
                    Description = EmployeeLetterSender.Description,
                    Status = EmployeeLetterSender.Status,
                    Time = EmployeeLetterSender.Time
                };

                db.EmployeeLetterSender.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                //  var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

                // UserHelper.OperationLog(Request.UserHostAddress, "Sent Letter", "Success", User.Identity.Name, User.Identity.Name + " updated sent letter of " + entity.se + " at date " + DateTime.Today,Modules.ArchiveAndLetters);

                #endregion
            }

            return Json(new[] { EmployeeLetterSender }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeLetterSender_Destroy([DataSourceRequest]DataSourceRequest request, EmployeeLetterSender EmployeeLetterSender)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new EmployeeLetterSender
                {
                    LetterSenderId = EmployeeLetterSender.LetterSenderId,
                    Date = EmployeeLetterSender.Date,
                    DateAmharic = EmployeeLetterSender.DateAmharic,
                    Receiver = EmployeeLetterSender.Receiver,
                    Description = EmployeeLetterSender.Description,
                    Status = EmployeeLetterSender.Status,
                    Time = EmployeeLetterSender.Time
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted letter sender information of name " + EmployeeLetterSender.Description);
                #endregion
                db.EmployeeLetterSender.Attach(entity);
                db.EmployeeLetterSender.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { EmployeeLetterSender }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult FinishTransfer(int masterIdd)
        {
            List<int> empIds = new List<int>();
            var letters = db.EmployeeLetterSender.Where(x => x.EmployeeIncomingLetterId == masterIdd).ToList();
            foreach (var let in letters)
            {
                if (let.IsTransfered == false)
                {
                    empIds.Add(let.Receiver);
                }

                let.IsTransfered = true;

                db.Entry(let).State = EntityState.Modified;
                db.SaveChanges();
            }

            var incoming = db.EmployeeIncommingLetter.Find(masterIdd);
            if (incoming != null)
            {
                incoming.IsTransfered = true;

                db.Entry(incoming).State = EntityState.Modified;
                db.SaveChanges();
            }

            foreach (var emp in empIds)
            {
                NotifModels notif = new NotifModels
                {
                    UserId = UserInformationHelper.GetEmployeeUserId(emp),
                    Title = "Incomming Letter",
                    Message = $" you have new incomming letter",
                    URL = ""
                };

                Notify.NotifyUser(notif);
            }

            return RedirectToAction("Index", "EmployeeIncoming");
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
