using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.ArchiveManagement.DepartmentToStaff;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.Archive.Controllers.DepartmentToStaff
{
    public class Receiver_DepartmentToStaffsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Receiver_DepartmentToStaffs_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var receivers = db.Receiver_DepartmentToStaffs.Where(x => x.OutGoing_DepartmentToStaffId == id).ToList();

            DataSourceResult result = receivers.ToDataSourceResult(request, receiver_DepartmentToStaff => new
            {
                Receiver_DepartmentToStaffId = receiver_DepartmentToStaff.Receiver_DepartmentToStaffId,
                OutGoing_DepartmentToStaffId = receiver_DepartmentToStaff.OutGoing_DepartmentToStaffId,
                EmployeeId = receiver_DepartmentToStaff.EmployeeId,
                Date = receiver_DepartmentToStaff.Date,
                DateEt = receiver_DepartmentToStaff.DateEt,
                IsSent = receiver_DepartmentToStaff.IsSent
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Receiver_DepartmentToStaffs_Create([DataSourceRequest]DataSourceRequest request, Receiver_DepartmentToStaff receiver_DepartmentToStaff, int id)
        {
            if (receiver_DepartmentToStaff.IsForAll)
            {
                //string username = User.Identity.Name;
                //var orgId = UserInformationHelper.GetEmployeeDepartmentId(username);
                //var placements = db.Placements.Where(x => x.OrgStructureId == orgId && x.isCurrent).ToList();

                var employeeid = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                List<int> employeeids = ExceedERP.Helpers.Common.EmployeeHelper.GetSubordinates(employeeid);
                string time = DateTime.Now.ToShortTimeString();

                receiver_DepartmentToStaff.Date = DateTime.Now;
                receiver_DepartmentToStaff.DateEt = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);


                //foreach (var emp in placements)
                foreach (var empId in employeeids)
                {
                   // var receiver = db.Receiver_DepartmentToStaffs.FirstOrDefault(x => x.OutGoing_DepartmentToStaffId == id && x.EmployeeId == emp.EmployeeId);
                    var receiver = db.Receiver_DepartmentToStaffs.FirstOrDefault(x => x.OutGoing_DepartmentToStaffId == id && x.EmployeeId == empId);
                    if (receiver == null)
                    {
                        var entity = new Receiver_DepartmentToStaff
                        {
                            OutGoing_DepartmentToStaffId = id,
                            //EmployeeId = emp.EmployeeId,
                            EmployeeId = empId,
                            Date = receiver_DepartmentToStaff.Date,
                            DateEt = receiver_DepartmentToStaff.DateEt,
                            IsSent = receiver_DepartmentToStaff.IsSent,
                            Time = time
                        };

                        db.Receiver_DepartmentToStaffs.Add(entity);
                        db.SaveChanges();
                        receiver_DepartmentToStaff.Receiver_DepartmentToStaffId = entity.Receiver_DepartmentToStaffId;
                    }
                }
            }
            else if (receiver_DepartmentToStaff.ReceiverSelected.Any())
            {
                var receivers = receiver_DepartmentToStaff.ReceiverSelected.ToList();
                foreach (var rec in receivers)
                {
                    var receiver = db.Receiver_DepartmentToStaffs.FirstOrDefault(x => x.OutGoing_DepartmentToStaffId == id && x.EmployeeId == rec);
                    if (receiver == null)
                    {
                        receiver_DepartmentToStaff.EmployeeId = rec;

                        string time = DateTime.Now.ToShortTimeString();

                        receiver_DepartmentToStaff.Date = DateTime.Now;
                        receiver_DepartmentToStaff.DateEt = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);

                        if (ModelState.IsValid)
                        {
                            var entity = new Receiver_DepartmentToStaff
                            {
                                OutGoing_DepartmentToStaffId = id,
                                EmployeeId = receiver_DepartmentToStaff.EmployeeId,
                                Date = receiver_DepartmentToStaff.Date,
                                DateEt = receiver_DepartmentToStaff.DateEt,
                                IsSent = receiver_DepartmentToStaff.IsSent,
                                Time = time
                            };

                            db.Receiver_DepartmentToStaffs.Add(entity);
                            db.SaveChanges();
                            receiver_DepartmentToStaff.Receiver_DepartmentToStaffId = entity.Receiver_DepartmentToStaffId;
                        }
                    }
                }
            }

            return Json(new[] { receiver_DepartmentToStaff }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Receiver_DepartmentToStaffs_Update([DataSourceRequest]DataSourceRequest request, Receiver_DepartmentToStaff receiver_DepartmentToStaff)
        {
            if (ModelState.IsValid)
            {
                receiver_DepartmentToStaff.Date = DateTime.Now;
                receiver_DepartmentToStaff.DateEt = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);

                var receivers = receiver_DepartmentToStaff.ReceiverSelected.ToList();

                foreach (var rec in receivers)
                {
                    var receiver = db.Receiver_DepartmentToStaffs.Find(receiver_DepartmentToStaff.Receiver_DepartmentToStaffId);
                    if (receiver != null)
                    {
                        receiver.EmployeeId = rec;

                        db.Entry(receiver).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }

            }

            return Json(new[] { receiver_DepartmentToStaff }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Receiver_DepartmentToStaffs_Destroy([DataSourceRequest]DataSourceRequest request, Receiver_DepartmentToStaff receiver_DepartmentToStaff)
        {
            if (ModelState.IsValid)
            {
                var entity = new Receiver_DepartmentToStaff
                {
                    Receiver_DepartmentToStaffId = receiver_DepartmentToStaff.Receiver_DepartmentToStaffId,
                    OutGoing_DepartmentToStaffId = receiver_DepartmentToStaff.OutGoing_DepartmentToStaffId,
                    EmployeeId = receiver_DepartmentToStaff.EmployeeId,
                    Date = receiver_DepartmentToStaff.Date,
                    DateEt = receiver_DepartmentToStaff.DateEt,
                    Time = receiver_DepartmentToStaff.Time,
                    IsSent = receiver_DepartmentToStaff.IsSent
                };

                db.Receiver_DepartmentToStaffs.Attach(entity);
                db.Receiver_DepartmentToStaffs.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { receiver_DepartmentToStaff }.ToDataSourceResult(request, ModelState));
        }
        public string FinishSending(int id)
        {
            var outDep = db.OutGoing_DepartmentToStaffs.Find(id);
            if (outDep != null)
            {
                outDep.IsSent = true;

                db.Entry(outDep).State = EntityState.Modified;

                var receivers = db.Receiver_DepartmentToStaffs.Where(x => x.OutGoing_DepartmentToStaffId == id).ToList();
                foreach (var rec in receivers)
                {
                    rec.IsSent = true;

                    db.Entry(rec).State = EntityState.Modified;
                }

                db.SaveChanges();

                return "Successfully Sent";
            }

            return "Can't send the letter";
        }
        public string FinishForwarding(int id)
        {
            var outDep = db.OutGoing_DepartmentToStaffs.Find(id);
            if (outDep != null)
            {
                outDep.IsSent = true;

                db.Entry(outDep).State = EntityState.Modified;

                string username = User.Identity.Name;
                int orgId = UserInformationHelper.GetEmployeeDepartmentId(username);
                var placements = db.Placements.Where(x => x.OrgStructureId == orgId && x.isCurrent).Select(x => x.EmployeeId).ToList();
                var receivers = db.Receiver_DepartmentToStaffs.Where(x => x.OutGoing_DepartmentToStaffId == id && placements.Contains(x.EmployeeId)).ToList();
                foreach (var rec in receivers)
                {
                    rec.IsSent = true;

                    db.Entry(rec).State = EntityState.Modified;
                }

                db.SaveChanges();

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
