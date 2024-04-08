﻿using System;
using System.Collections.Generic;
﻿using System.Data.Entity;
﻿using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
﻿using ExceedERP.Core.Domain.Common;
﻿using ExceedERP.Core.Domain.Common.HRM;
﻿using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.Archive;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain;

namespace ExceedERP.Web.Areas.Archive.Controllers
{

    public class InstitutionArchiveNoController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            ViewData["InstuType"] = db.InstitutionType.AsQueryable();
            ViewData["Branch"] = db.Branch.ToList();
            ViewData["SubjectType"] = db.SubjectTypes.AsQueryable();
           
            return View();
        }
        public ActionResult InstitutionArchiveNo_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<InstitutionArchiveNo> institutionarchiveno = db.InstitutionArchiveNo;
            DataSourceResult result = institutionarchiveno.ToDataSourceResult(request, institutionArchiveNo => new {
                InstiArchiveNoId = institutionArchiveNo.InstiArchiveNoId,
                FileNo = institutionArchiveNo.FileNo,
                InstitutionTypeId = institutionArchiveNo.InstitutionTypeId,
                ZoneId = institutionArchiveNo.ZoneId,
                Shelve = institutionArchiveNo.Shelve,
                Row = institutionArchiveNo.Row,
                Folder = institutionArchiveNo.Folder,
                BranchId = institutionArchiveNo.BranchId,
                RoomNo = institutionArchiveNo.RoomNo,
                Volume = institutionArchiveNo.Volume,
                SubjectTypeId = institutionArchiveNo.SubjectTypeId,
                Remark = institutionArchiveNo.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]

        public ActionResult InstitutionArchiveNo_Create([DataSourceRequest]DataSourceRequest request, InstitutionArchiveNo institutionArchiveNo)
        {
            //var checkExistance = db.PersonalArchiveNo.FirstOrDefault(x => x.Folder == institutionArchiveNo.Folder);
            //if (checkExistance != null)
            //{
            //    ModelState.AddModelError("", "This folder no are taken!!");
            //}
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
                    BranchId = institutionArchiveNo.BranchId,
                    RoomNo = institutionArchiveNo.RoomNo,
                    Volume = institutionArchiveNo.Volume,
                    SubjectTypeId = institutionArchiveNo.SubjectTypeId,
                    Remark = institutionArchiveNo.Remark
                };

                db.InstitutionArchiveNo.Add(entity);
                db.SaveChanges();
                institutionArchiveNo.InstiArchiveNoId = entity.InstiArchiveNoId;

                #region operation log

              //  var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
               
                 //   UserHelper.OperationLog(Request.UserHostAddress, "Institution Folder", "Success", User.Identity.Name, User.Identity.Name + " add institution folder for " + entity.InstitutionName + " at date " + DateTime.Today,Modules.ArchiveAndLetters);

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
                    BranchId = institutionArchiveNo.BranchId,
                    RoomNo = institutionArchiveNo.RoomNo,
                    Volume = institutionArchiveNo.Volume,
                    SubjectTypeId = institutionArchiveNo.SubjectTypeId,
                    Remark = institutionArchiveNo.Remark
                };

                db.InstitutionArchiveNo.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                
                  //  UserHelper.OperationLog(Request.UserHostAddress, "Institution Folder", "Success", User.Identity.Name, User.Identity.Name + " updated institution folder of " + entity.InstitutionName + " at date " + DateTime.Today,Modules.ArchiveAndLetters);
                
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
                    BranchId = institutionArchiveNo.BranchId,
                    RoomNo = institutionArchiveNo.RoomNo,
                    Volume = institutionArchiveNo.Volume,
                    SubjectTypeId = institutionArchiveNo.SubjectTypeId,
                    Remark = institutionArchiveNo.Remark
                };
                #region delete log
               // UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted institution archive of name" + institutionArchiveNo.InstitutionName);
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
