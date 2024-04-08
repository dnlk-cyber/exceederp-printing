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
using ExceedERP.Core.Domain.Archive;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.Archive.Controllers
{

    [AuthorizeRoles(HrmConstants.HRArchiveAdmin, HrmConstants.HRArchiveUser)]
    public class InstitutionFolderExplorerController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static int Zone { get; set; }
        public static string Folder { get; set; }
        public ActionResult Index()
        {
            ViewData["SubjectType"] = db.SubjectTypes.AsQueryable();
            ViewData["Folders"] = db.InstitutionArchiveNo.AsQueryable();
           // ViewData["EmployeeList"] = db.Person_Employee.Select(x => new { x.EmployeeId, x.FullNameaEnglish }).ToList();

            return View();
        }
        public ActionResult GetZone(int zone)
        {
            Zone = zone;

            return Index();
        }
        public ActionResult GetFolder(string folder)
        {
            Folder = folder;

            return Index();
        }
        public ActionResult InstitutionArchiveNo_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<InstitutionArchiveNo> institutionarchiveno = db.InstitutionArchiveNo;
            DataSourceResult result = institutionarchiveno.ToDataSourceResult(request, institutionArchiveNo => new
            {
                InstiArchiveNoId = institutionArchiveNo.InstiArchiveNoId,
                FileNo = institutionArchiveNo.FileNo,
                InstitutionTypeId = institutionArchiveNo.InstitutionTypeId,
                ZoneId = institutionArchiveNo.ZoneId,
                Shelve = institutionArchiveNo.Shelve,
                Row = institutionArchiveNo.Row,
                Folder = institutionArchiveNo.Folder,
                Volume = institutionArchiveNo.Volume,
                SubjectTypeId = institutionArchiveNo.SubjectTypeId,
                RoomNo = institutionArchiveNo.RoomNo,
                Remark = institutionArchiveNo.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InstitutionArchiveNo_Create([DataSourceRequest]DataSourceRequest request, InstitutionArchiveNo institutionArchiveNo)
        {
            if (ModelState.IsValid)
            {
                var entity = new InstitutionArchiveNo
                {
                    FileNo = institutionArchiveNo.FileNo,
                    InstitutionTypeId = institutionArchiveNo.InstitutionTypeId,
                    ZoneId = institutionArchiveNo.ZoneId,
                    Shelve = institutionArchiveNo.Shelve,
                    Row = institutionArchiveNo.Row,
                    Folder = institutionArchiveNo.Folder,
                    Volume = institutionArchiveNo.Volume,
                    SubjectTypeId = institutionArchiveNo.SubjectTypeId,
                    Remark = institutionArchiveNo.Remark
                };

                db.InstitutionArchiveNo.Add(entity);
                db.SaveChanges();
                institutionArchiveNo.InstiArchiveNoId = entity.InstiArchiveNoId;

                #region operation log

               // var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

                //UserHelper.OperationLog(Request.UserHostAddress, "Institution Folder Explorer", "Success", User.Identity.Name, User.Identity.Name + " add institution folder explorer for " + entity.InstitutionName + " at date " + DateTime.Today, Modules.ArchiveAndLetters);

                #endregion
            }

            return Json(new[] { institutionArchiveNo }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InstitutionArchiveNo_Update([DataSourceRequest]DataSourceRequest request, InstitutionArchiveNo institutionArchiveNo)
        {
            if (ModelState.IsValid)
            {
                var entity = new InstitutionArchiveNo
                {
                    InstiArchiveNoId = institutionArchiveNo.InstiArchiveNoId,
                    FileNo = institutionArchiveNo.FileNo,
                    InstitutionTypeId = institutionArchiveNo.InstitutionTypeId,
                    ZoneId = institutionArchiveNo.ZoneId,
                    Shelve = institutionArchiveNo.Shelve,
                    Row = institutionArchiveNo.Row,
                    Folder = institutionArchiveNo.Folder,
                    Volume = institutionArchiveNo.Volume,
                    SubjectTypeId = institutionArchiveNo.SubjectTypeId,
                    Remark = institutionArchiveNo.Remark
                };

                db.InstitutionArchiveNo.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

              //  UserHelper.OperationLog(Request.UserHostAddress, "Institution Folder Explorer", "Success", User.Identity.Name, User.Identity.Name + " updated institution folder explorer for " + entity.InstitutionName + " at date " + DateTime.Today, Modules.ArchiveAndLetters);

                #endregion
            }

            return Json(new[] { institutionArchiveNo }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InstitutionArchiveNo_Destroy([DataSourceRequest]DataSourceRequest request, InstitutionArchiveNo institutionArchiveNo)
        {
            if (ModelState.IsValid)
            {
                var entity = new InstitutionArchiveNo
                {
                    InstiArchiveNoId = institutionArchiveNo.InstiArchiveNoId,
                    FileNo = institutionArchiveNo.FileNo,
                    InstitutionTypeId = institutionArchiveNo.InstitutionTypeId,
                    ZoneId = institutionArchiveNo.ZoneId,
                    Shelve = institutionArchiveNo.Shelve,
                    Row = institutionArchiveNo.Row,
                    Folder = institutionArchiveNo.Folder,
                    Volume = institutionArchiveNo.Volume,
                    SubjectTypeId = institutionArchiveNo.SubjectTypeId,
                    Remark = institutionArchiveNo.Remark
                };
                #region delete log
               // UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted institution folder explorer of name" + institutionArchiveNo.InstitutionName);
                #endregion
                db.InstitutionArchiveNo.Attach(entity);
                db.InstitutionArchiveNo.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { institutionArchiveNo }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
