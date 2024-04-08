using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.ArchiveManagement;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.Archive.Controllers
{
    public class BroadcastedLettersController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["Employees"] = db.Person_Employee.ToList();

            return View();
        }

        public ActionResult BroadcastedLetters_Read([DataSourceRequest]DataSourceRequest request)
        {
            var username = User.Identity.Name;
            int empId = UserInformationHelper.GetEmployeeId(username);
            var receivers = db.LetterReceivers.Where(x => x.IsTransfered).Where(x => x.EmployeeId == empId).Select(x => x.LetterForMultipleStaffId).ToList();

            var letterfolmultiplestaffs = db.LetterFolMultipleStaffs.Where(x => x.IsForAllEmployee || receivers.Contains(x.LetterForMultipleStaffId)).Where(x => x.IsTransfered).ToList();

            DataSourceResult result = letterfolmultiplestaffs.ToDataSourceResult(request, letterForMultipleStaff => new
            {
                LetterForMultipleStaffId = letterForMultipleStaff.LetterForMultipleStaffId,
                SendingDate = letterForMultipleStaff.SendingDate,
                SendingDateAm = letterForMultipleStaff.SendingDateAm,
                Title = letterForMultipleStaff.Title,
                IsTransfered = letterForMultipleStaff.IsTransfered,
                IsForAllEmployee = CheckIsForAllEmployee(letterForMultipleStaff.LetterForMultipleStaffId)
            });

            return Json(result);
        }
        public bool CheckIsForAllEmployee(int id)
        {
            var receiver = db.LetterReceivers.FirstOrDefault(x => x.LetterForMultipleStaffId == id);
            if (receiver != null && receiver.IsForAllEmployee)
            {
                return true;
            }

            return false;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
