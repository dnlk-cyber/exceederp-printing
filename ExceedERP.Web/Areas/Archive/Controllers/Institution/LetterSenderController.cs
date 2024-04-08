using System;
using System.Collections.Generic;

using System.Data.Entity;
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
using ExceedERP.Core.Domain.ArchiveManagement;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Helpers.HR;
using ExceedERP.Web.Hubs;

namespace ExceedERP.Web.Areas.Archive.Controllers
{
    public class LetterSenderController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static string DateAmharic { get; set; }
        public static DateTime Date { get; set; }
        public ActionResult Index()
        {
            var context = new ApplicationDbContext();

            var allUsers = context.Users.ToList();
            ViewData["Users"] = allUsers;
            GetDepartments();

            return View();
        }
        public void GetDepartments()
        {
            List<MainCompanyStructure> allDepartment = new List<MainCompanyStructure>();
            var departments = db.MainCompanyStructures.Where(x => x.Parent == 0).FirstOrDefault();
            if (departments != null)
            {
                var deptlevelOne = db.MainCompanyStructures.Where(x => x.Parent == departments.OrgStructureId).ToList();
                if (deptlevelOne.Any())
                {
                    foreach (var a in deptlevelOne)
                    {
                        var departmentLevelTwo = db.MainCompanyStructures.Where(x => x.Parent == a.OrgStructureId).ToList();
                        if (departmentLevelTwo.Any())
                        {
                            foreach (var b in departmentLevelTwo)
                            {
                                allDepartment.Add(b);
                            }
                        }
                        allDepartment.Add(a);
                    }
                }
            }
            ViewData["Departments"] = allDepartment;
        }
        public ActionResult LetterSender_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var letterSender = db.LetterSender.Where(x => x.InstitutionIncomingLetterId == id).ToList();

            DataSourceResult result = letterSender.ToDataSourceResult(request, sender => new
            {
                LetterSenderId = sender.LetterSenderId,
                Date = sender.Date,
                Receiver = sender.Receiver,
                Description = sender.Description,
                InstitutionIncomingLetterId = sender.InstitutionIncomingLetterId,
                SenderName = GetSenderName(sender.InstitutionIncomingLetterId),
                Status = sender.Status,
                DateAmharic = sender.DateAmharic,
                SenderDepartment = sender.SenderDepartment,
                IsTransfered = sender.IsTransfered,
                Time = sender.Time
            });

            return Json(result);
        }
        public ActionResult LetterSender_Create([DataSourceRequest] DataSourceRequest request, LetterSender letterSender, int id)
        {
            ModelState.Clear();            
            
            if (ModelState.IsValid)
            {
                if(letterSender.ReceiverSelected.Any())
                {
                    var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                    var placment = db.Placements.FirstOrDefault(x => x.EmployeeId == employeeId && x.isCurrent);
                    if (placment != null)
                    {
                        letterSender.SenderDepartment = placment.OrgStructureId;
                    }

                    string time = DateTime.Now.ToShortTimeString();

                    var receivers = letterSender.ReceiverSelected.ToList();
                   
                    foreach (var rec in receivers)
                    {
                        var entity = new LetterSender
                        {
                            InstitutionIncomingLetterId = id,
                            Date = DateTime.Now,
                            DateAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year),
                            Time = time,
                            Description = letterSender.Description,
                            Status = LettersStatus.WAITING,
                            Receiver = rec,
                            SenderDepartment = letterSender.SenderDepartment,
                        };                        

                        db.LetterSender.Add(entity);
                        db.SaveChanges();
                    }
                }     
            }

            return Json(new[] { letterSender }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult LetterSender_Destroy([DataSourceRequest]DataSourceRequest request, LetterSender letterSender)
        {
            ModelState.Clear();
            
            if (ModelState.IsValid)
            {
                var sender = db.LetterSender.Find(letterSender.LetterSenderId);
                if(sender != null)
                {
                    db.Entry(sender).State = EntityState.Deleted;
                    db.SaveChanges();
                }                
            }

            return Json(new[] { letterSender }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult FinishTransfer(int masterIdd)
        {
            // check departments not receive notification before and add in this list / this helps not to send notification for departments received before
            List<int> orgIds = new List<int>();

            var letters = db.LetterSender.Where(x => x.InstitutionIncomingLetterId == masterIdd).ToList();
            foreach(var let in letters)
            {
                if(let.IsTransfered == false)
                {
                    orgIds.Add(let.Receiver);
                }

                let.IsTransfered = true;

                db.Entry(let).State = EntityState.Modified;
                db.SaveChanges();
            }

            var incoming = db.InstitutionIncomingLetters.Find(masterIdd);
            if(incoming != null)
            {
                incoming.IsTransfered = true;

                db.Entry(incoming).State = EntityState.Modified;
                db.SaveChanges();
            }

            foreach(var dep in orgIds)
            {
                var placement = db.Placements.FirstOrDefault(x => x.OrgStructureId == dep && x.isCurrent && x.IsHead);
                if(placement != null)
                {
                    NotifModels notif = new NotifModels
                    {
                        UserId = UserInformationHelper.GetEmployeeUserId(placement.EmployeeId),
                        Title = "Incomming Letter",
                        Message = $" you have new incomming letter",
                        URL = "/Archive/DepartmentSentLetter"
                    };

                    Notify.NotifyUser(notif);
                }               
            }           

            return RedirectToAction("Index", "InstitutionIncomingLetters");
        }
        private string GetSenderName(int id)
        {
            var book = db.InstitutionIncomingLetters.FirstOrDefault(x => x.InstitutionIncomingLetterId == id);
            if (book != null)
            {
                return book.SenderName;
            }
            return "";
        }       
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
