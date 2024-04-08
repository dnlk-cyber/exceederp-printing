using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.ArchiveManagement;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Hubs;

namespace ExceedERP.Web.Areas.Archive.Controllers.DepartmentToDepartment
{
    public class Receiver_DepartmentToDepartmentController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Receiver_DepartmentToDepartment_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var receiver_departmenttodepartments = db.Receiver_DepartmentToDepartments.Where(x => x.OutGoing_DepartmentToDepartmentId == id).ToList();
            DataSourceResult result = receiver_departmenttodepartments.ToDataSourceResult(request, receiver_DepartmentToDepartment => new
            {
                Receiver_DepartmentToDepartmentId = receiver_DepartmentToDepartment.Receiver_DepartmentToDepartmentId,
                OrgStructureId = receiver_DepartmentToDepartment.OrgStructureId,
                Date = receiver_DepartmentToDepartment.Date,
                DateEt = receiver_DepartmentToDepartment.DateEt,
                OutGoing_DepartmentToDepartmentId = receiver_DepartmentToDepartment.OutGoing_DepartmentToDepartmentId,
                Time = receiver_DepartmentToDepartment.Time,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Receiver_DepartmentToDepartment_Create([DataSourceRequest]DataSourceRequest request, Receiver_DepartmentToDepartment receiver_DepartmentToDepartment, int id)
        {
            if (receiver_DepartmentToDepartment.ReceiverSelected.Any())
            {
                string time = DateTime.Now.ToShortTimeString();
                var departments = receiver_DepartmentToDepartment.ReceiverSelected.ToList();
                foreach (var dep in departments)
                {
                    receiver_DepartmentToDepartment.Date = DateTime.Now;
                    receiver_DepartmentToDepartment.DateEt = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);

                    if (ModelState.IsValid)
                    {
                        var entity = new Receiver_DepartmentToDepartment
                        {
                            OutGoing_DepartmentToDepartmentId = id,
                            OrgStructureId = dep,
                            IsSent = false,
                            Time = time,
                            Date = receiver_DepartmentToDepartment.Date,
                            DateEt = receiver_DepartmentToDepartment.DateEt
                        };

                        db.Receiver_DepartmentToDepartments.Add(entity);
                        db.SaveChanges();
                        receiver_DepartmentToDepartment.Receiver_DepartmentToDepartmentId = entity.Receiver_DepartmentToDepartmentId;
                    }
                }
            }

            return Json(new[] { receiver_DepartmentToDepartment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Receiver_DepartmentToDepartment_Update([DataSourceRequest]DataSourceRequest request, Receiver_DepartmentToDepartment receiver_DepartmentToDepartment)
        {
            if (ModelState.IsValid)
            {
                var entity = new Receiver_DepartmentToDepartment
                {
                    Receiver_DepartmentToDepartmentId = receiver_DepartmentToDepartment.Receiver_DepartmentToDepartmentId,
                    OutGoing_DepartmentToDepartmentId = receiver_DepartmentToDepartment.OutGoing_DepartmentToDepartmentId,
                    Time = receiver_DepartmentToDepartment.Time,
                    OrgStructureId = receiver_DepartmentToDepartment.OrgStructureId,
                    Date = receiver_DepartmentToDepartment.Date,
                    DateEt = receiver_DepartmentToDepartment.DateEt,
                    IsSent = receiver_DepartmentToDepartment.IsSent
                };

                db.Receiver_DepartmentToDepartments.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { receiver_DepartmentToDepartment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Receiver_DepartmentToDepartment_Destroy([DataSourceRequest]DataSourceRequest request, Receiver_DepartmentToDepartment receiver_DepartmentToDepartment)
        {
            if (ModelState.IsValid)
            {
                var receiver = db.Receiver_DepartmentToDepartments.Find(receiver_DepartmentToDepartment.Receiver_DepartmentToDepartmentId);

                if (receiver != null)
                {
                    db.Entry(receiver).State = EntityState.Deleted;
                    db.SaveChanges();
                }
            }

            return Json(new[] { receiver_DepartmentToDepartment }.ToDataSourceResult(request, ModelState));
        }
        public string FinishSending(int id)
        {
            List<int> orgIds = new List<int>();
            var outDep = db.OutGoing_DepartmentToDepartments.Find(id);
            if (outDep != null)
            {
                outDep.IsSent = true;

                db.Entry(outDep).State = EntityState.Modified;

                var receivers = db.Receiver_DepartmentToDepartments.Where(x => x.OutGoing_DepartmentToDepartmentId == id).ToList();
                foreach (var rec in receivers)
                {
                    // collecting departments not receive notification before 
                    if (rec.IsSent == false)
                    {
                        orgIds.Add(rec.OrgStructureId);
                    }
                    rec.IsSent = true;

                    db.Entry(rec).State = EntityState.Modified;
                }

                db.SaveChanges();

                foreach (var dep in orgIds)
                {
                    string senderDep = "";
                    var comp = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == outDep.OrgstructureId);
                    if (comp != null)
                    {
                        senderDep = comp.Name;
                    }
                    var placement = db.Placements.FirstOrDefault(x => x.OrgStructureId == dep && x.isCurrent && x.IsHead);
                    if (placement != null)
                    {
                        NotifModels notif = new NotifModels
                        {
                            UserId = UserInformationHelper.GetEmployeeUserId(placement.EmployeeId),
                            Title = "Letter From Department",
                            Message = $" You have new leter sent from " + senderDep,
                            URL = "/Archive/OutGoing_DepartmentToDepartments/Index_Receiver"
                        };

                        Notify.NotifyUser(notif);
                    }
                }

                return "Successfully Sent";
            }

            return "Can't send the letter";
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
