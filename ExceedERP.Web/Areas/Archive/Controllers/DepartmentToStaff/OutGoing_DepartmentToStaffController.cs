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
    public class OutGoing_DepartmentToStaffController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["Departments"] = db.MainCompanyStructures.ToList();
            ViewData["Employees"] = db.Person_Employee.Select(x => new { x.EmployeeId, FullNameaEnglish = x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish }).ToList();
            return View();
        }
        public ActionResult Index_Receiver()
        {
            ViewData["Departments"] = db.MainCompanyStructures.ToList();
            ViewData["Employees"] = db.Person_Employee.Select(x => new { x.EmployeeId, FullNameaEnglish = x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish }).ToList();

            return View();
        }
        public ActionResult OutGoing_DepartmentToStaffs_Read([DataSourceRequest]DataSourceRequest request)
        {
            int orgId = 0;
            string username = User.Identity.Name;
            int empId = UserInformationHelper.GetEmployeeId(username);
            var placement = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == empId);
            if (placement != null)
            {
                orgId = placement.OrgStructureId;
            }
            var letters = db.OutGoing_DepartmentToStaffs.Where(x => x.OrgStructureId == orgId).ToList();

            DataSourceResult result = letters.ToDataSourceResult(request, outGoing_DepartmentToStaff => new
            {
                OutGoing_DepartmentToStaffId = outGoing_DepartmentToStaff.OutGoing_DepartmentToStaffId,
                OrgStructureId = outGoing_DepartmentToStaff.OrgStructureId,
                IsForAll = outGoing_DepartmentToStaff.IsForAll,
                IsSent = outGoing_DepartmentToStaff.IsSent,
                Message = outGoing_DepartmentToStaff.Message,
                Date = outGoing_DepartmentToStaff.Date,
                DateEt = outGoing_DepartmentToStaff.DateEt,
                Time = outGoing_DepartmentToStaff.Time,
                IsLogedInUserHead = CheckLoggedInUserIfHead()
            });

            return Json(result);
        }
        public ActionResult OutGoing_DepartmentToStaffs_Sent_Read([DataSourceRequest]DataSourceRequest request)
        {
            string username = User.Identity.Name;
            int empId = UserInformationHelper.GetEmployeeId(username);
            int orgId = UserInformationHelper.GetEmployeeDepartmentId(username);

            var receivers = db.Receiver_DepartmentToStaffs.Where(x => x.EmployeeId == empId).Select(x => x.OutGoing_DepartmentToStaffId).ToList();

            var letters = db.OutGoing_DepartmentToStaffs/*.Where(x=>x.OrgStructureId == orgId)*/.Where(x => x.IsForAll || receivers.Contains(x.OutGoing_DepartmentToStaffId)).Where(x=>x.IsSent).ToList();

            DataSourceResult result = letters.ToDataSourceResult(request, outGoing_DepartmentToStaff => new
            {
                OutGoing_DepartmentToStaffId = outGoing_DepartmentToStaff.OutGoing_DepartmentToStaffId,
                OrgStructureId = outGoing_DepartmentToStaff.OrgStructureId,
                IsForAll = outGoing_DepartmentToStaff.IsForAll,
                IsSent = outGoing_DepartmentToStaff.IsSent,
                Message = outGoing_DepartmentToStaff.Message,
                Date = outGoing_DepartmentToStaff.Date,
                DateEt = outGoing_DepartmentToStaff.DateEt,
                Time = outGoing_DepartmentToStaff.Time,
                IsLogedInUserHead = CheckLoggedInUserIfHead()
            });

            return Json(result);
        }
        public bool CheckLoggedInUserIfHead()
        {
            string username = User.Identity.Name;
            int empId = UserInformationHelper.GetEmployeeId(username);
            var place = db.Placements.FirstOrDefault(x => x.EmployeeId == empId && x.isCurrent && x.IsHead);
            if(place != null)
            {
                return true;
            }

            return false;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OutGoing_DepartmentToStaffs_Create([DataSourceRequest]DataSourceRequest request, OutGoing_DepartmentToStaff outGoing_DepartmentToStaff)
        {
            string username = User.Identity.Name;
            int empId = UserInformationHelper.GetEmployeeId(username);
            var placement = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == empId);
            if (placement != null)
            {
                outGoing_DepartmentToStaff.OrgStructureId = placement.OrgStructureId;
            }
            if (ModelState.IsValid)
            {
                var entity = new OutGoing_DepartmentToStaff
                {
                    OrgStructureId = outGoing_DepartmentToStaff.OrgStructureId,
                    IsForAll = outGoing_DepartmentToStaff.IsForAll,
                    IsSent =   outGoing_DepartmentToStaff.IsSent,
                    Message =  outGoing_DepartmentToStaff.Message,
                    Date = DateTime.Now,
                    DateEt = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year),
                    Time = DateTime.Now.ToShortTimeString()
                };

                db.OutGoing_DepartmentToStaffs.Add(entity);
                db.SaveChanges();
                outGoing_DepartmentToStaff.OutGoing_DepartmentToStaffId = entity.OutGoing_DepartmentToStaffId;
            }

            return Json(new[] { outGoing_DepartmentToStaff }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OutGoing_DepartmentToStaffs_Update([DataSourceRequest]DataSourceRequest request, OutGoing_DepartmentToStaff outGoing_DepartmentToStaff)
        {
            if (ModelState.IsValid)
            {
                var letter = db.OutGoing_DepartmentToStaffs.Find(outGoing_DepartmentToStaff.OutGoing_DepartmentToStaffId);
                if (letter != null)
                {
                    letter.Message = outGoing_DepartmentToStaff.Message;
                    letter.IsForAll = outGoing_DepartmentToStaff.IsForAll;

                    db.Entry(letter).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Json(new[] { outGoing_DepartmentToStaff }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OutGoing_DepartmentToStaffs_Destroy([DataSourceRequest]DataSourceRequest request, OutGoing_DepartmentToStaff outGoing_DepartmentToStaff)
        {
            if (ModelState.IsValid)
            {
                var letter = db.OutGoing_DepartmentToStaffs.Find(outGoing_DepartmentToStaff.OutGoing_DepartmentToStaffId);
                if (letter != null)
                {
                    letter.IsForAll = outGoing_DepartmentToStaff.IsForAll;

                    db.Entry(letter).State = EntityState.Deleted;
                    db.SaveChanges();
                }

            }

            return Json(new[] { outGoing_DepartmentToStaff }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
