﻿using System;
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

namespace ExceedERP.Web.Areas.Archive.Controllers
{
    public class LetterForMultipleStaffsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["Employees"] = db.Person_Employee.ToList();

            return View();
        }

        public ActionResult LetterFolMultipleStaffs_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<LetterForMultipleStaff> letterfolmultiplestaffs = db.LetterFolMultipleStaffs;
            DataSourceResult result = letterfolmultiplestaffs.ToDataSourceResult(request, letterFolMultipleStaff => new {
                LetterForMultipleStaffId = letterFolMultipleStaff.LetterForMultipleStaffId,
                SendingDate = letterFolMultipleStaff.SendingDate,
                SendingDateAm = letterFolMultipleStaff.SendingDateAm,
                Title = letterFolMultipleStaff.Title,
                IsTransfered = letterFolMultipleStaff.IsTransfered,
                IsForAllEmployee = letterFolMultipleStaff.IsForAllEmployee,// CheckIsForAllEmployee(letterFolMultipleStaff.LetterForMultipleStaffId)
            });

            return Json(result);
        }
        public bool CheckIsForAllEmployee(int id)
        {
            var receiver = db.LetterReceivers.FirstOrDefault(x => x.LetterForMultipleStaffId == id);
            if(receiver != null && receiver.IsForAllEmployee)
            {
                return true;
            }

            return false;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LetterFolMultipleStaffs_Create([DataSourceRequest]DataSourceRequest request, LetterForMultipleStaff letterFolMultipleStaff)
        {
            if (ModelState.IsValid)
            {
                DateTime date = DateTime.Now;
                letterFolMultipleStaff.SendingDate = date;

                letterFolMultipleStaff.SendingDateAm = XAPI.EthiopicDateTime.GetEthiopicDate(date.Day, date.Month, date.Year);

                var entity = new LetterForMultipleStaff
                {
                    SendingDate = letterFolMultipleStaff.SendingDate,
                    SendingDateAm = letterFolMultipleStaff.SendingDateAm,
                    Title = letterFolMultipleStaff.Title,
                    IsTransfered = letterFolMultipleStaff.IsTransfered,
                    IsForAllEmployee = letterFolMultipleStaff.IsForAllEmployee
                };

                db.LetterFolMultipleStaffs.Add(entity);
                db.SaveChanges();
                letterFolMultipleStaff.LetterForMultipleStaffId = entity.LetterForMultipleStaffId;
            }

            return Json(new[] { letterFolMultipleStaff }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LetterFolMultipleStaffs_Update([DataSourceRequest]DataSourceRequest request, LetterForMultipleStaff letterFolMultipleStaff)
        {
            if (ModelState.IsValid)
            {
                DateTime date = DateTime.Now;
                letterFolMultipleStaff.SendingDate = date;

                letterFolMultipleStaff.SendingDateAm = XAPI.EthiopicDateTime.GetEthiopicDate(date.Day, date.Month, date.Year);

                var entity = new LetterForMultipleStaff
                {
                    LetterForMultipleStaffId = letterFolMultipleStaff.LetterForMultipleStaffId,
                    SendingDate = letterFolMultipleStaff.SendingDate,
                    SendingDateAm = letterFolMultipleStaff.SendingDateAm,
                    Title = letterFolMultipleStaff.Title,
                    IsTransfered = letterFolMultipleStaff.IsTransfered,
                    IsForAllEmployee = letterFolMultipleStaff.IsForAllEmployee
                };

                db.LetterFolMultipleStaffs.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { letterFolMultipleStaff }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LetterFolMultipleStaffs_Destroy([DataSourceRequest]DataSourceRequest request, LetterForMultipleStaff letterFolMultipleStaff)
        {
            if (ModelState.IsValid)
            {
                var entity = new LetterForMultipleStaff
                {
                    LetterForMultipleStaffId = letterFolMultipleStaff.LetterForMultipleStaffId,
                    SendingDate = letterFolMultipleStaff.SendingDate,
                    SendingDateAm = letterFolMultipleStaff.SendingDateAm,
                    Title = letterFolMultipleStaff.Title,
                    IsTransfered = letterFolMultipleStaff.IsTransfered,
                    IsForAllEmployee = letterFolMultipleStaff.IsForAllEmployee
                };

                db.LetterFolMultipleStaffs.Attach(entity);
                db.LetterFolMultipleStaffs.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { letterFolMultipleStaff }.ToDataSourceResult(request, ModelState));
        }
        public JsonResult SendCompanyDocuments(string id, string compTypeId, string desc)
        {
            int.TryParse(id, out int masterId);
            int.TryParse(compTypeId, out int companyTypeId);
            var announcement = db.LetterFolMultipleStaffs.FirstOrDefault(x => x.LetterForMultipleStaffId == masterId);
           
            if(announcement != null)
            {
                var companyFiles = db.CompanyDocuments.Where(x => x.CompanyDocumentTypeId == companyTypeId).ToList();
                foreach (var com in companyFiles)
                {
                    var entity = new AttachedLettersForStaff
                    {
                        LetterForMultipleStaffId = masterId,
                        FilePath = com.FilePath,
                        FileName = com.FileName,
                        Description = desc,
                        ShortPath = com.ShortPath,
                        FileExtension = com.FileExtension
                    };

                    db.AttachedLettersForStaffs.Add(entity);
                    db.SaveChanges();
                }

                return Json("Successfull");
            }

            return Json("No such announcement");
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
