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
﻿using ExceedERP.Core.Domain.HRM.ArchiveManagement;
﻿using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain;

namespace ExceedERP.Web.Areas.HRM.Controllers.Archive
{
     
    [AuthorizeRoles(HrmConstants.HRArchiveAdmin, HrmConstants.HRArchiveUser)]
    public class InstitutionArchiveNoController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            ViewData["InstuType"] = db.InstitutionType.AsQueryable();
            ViewData["Zones"] = db.Zone.AsQueryable();
            ViewData["Branch"] = db.Branch.ToList();
            ViewData["SubjectType"] = db.SubjectTypes.AsQueryable();
            GetEmployees();
            return View();
        }
     
        public JsonResult GetEmployees()
        {
            List<EmployeeIdView> EmployeeIdViews = new List<EmployeeIdView>();
            EmployeeIdView EmployeeIdView = new EmployeeIdView();
            var employeeList = db.Person_Employee.AsQueryable();
            if (employeeList.Count() > 0)
            {
                employeeList = employeeList.Where(l => l.IsActive).AsQueryable();
            }
            foreach (var emp in employeeList)
            {
                EmployeeIdView = new EmployeeIdView();
                EmployeeIdView.PersonId = emp.EmployeeId;
                EmployeeIdView.FullName = emp.FirstName + " " + emp.MiddleName + " " + emp.LastName;
                EmployeeIdViews.Add(EmployeeIdView);
            }
            ViewData["EmployeeList"] = EmployeeIdViews != null ? EmployeeIdViews.ToList() : new List<EmployeeIdView>();
            return Json(EmployeeIdViews.Select(p => new { PersonId = p.PersonId, FullName = p.FullName }), JsonRequestBehavior.AllowGet);
        }
          
        public ActionResult InstitutionArchiveNo_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<InstitutionArchiveNo> institutionarchiveno = db.InstitutionArchiveNo;
            DataSourceResult result = institutionarchiveno.ToDataSourceResult(request, institutionArchiveNo => new {
                InstiArchiveNoId = institutionArchiveNo.InstiArchiveNoId,
                FileNo = institutionArchiveNo.FileNo,
                InstitutionName = institutionArchiveNo.InstitutionName,
                InstitutionTypeId = institutionArchiveNo.InstitutionTypeId,
                ZoneId = institutionArchiveNo.ZoneId,
                Shelve = institutionArchiveNo.Shelve,
                Row = institutionArchiveNo.Row,
                Folder = institutionArchiveNo.Folder,
                BranchId = institutionArchiveNo.BranchId,
                RoomNo = institutionArchiveNo.RoomNo,
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
                    InstitutionName = institutionArchiveNo.InstitutionName,
                    InstitutionTypeId = institutionArchiveNo.InstitutionTypeId,
                    ZoneId = institutionArchiveNo.ZoneId,
                    Shelve = institutionArchiveNo.Shelve,
                    Row = institutionArchiveNo.Row,
                    Folder = institutionArchiveNo.Folder,
                    BranchId = institutionArchiveNo.BranchId,
                    RoomNo = institutionArchiveNo.RoomNo,
                    Remark = institutionArchiveNo.Remark
                };

                db.InstitutionArchiveNo.Add(entity);
                db.SaveChanges();
                institutionArchiveNo.InstiArchiveNoId = entity.InstiArchiveNoId;

                #region operation log

                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
               
                    UserHelper.OperationLog(Request.UserHostAddress, "Institution Folder", "Success", User.Identity.Name, User.Identity.Name + " add institution folder for " + entity.InstitutionName + " at date " + DateTime.Today,Modules.ArchiveAndLetters);

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
                    InstitutionName = institutionArchiveNo.InstitutionName,
                    InstitutionTypeId = institutionArchiveNo.InstitutionTypeId,
                    ZoneId = institutionArchiveNo.ZoneId,
                    Shelve = institutionArchiveNo.Shelve,
                    Row = institutionArchiveNo.Row,
                    Folder = institutionArchiveNo.Folder,
                    BranchId = institutionArchiveNo.BranchId,
                    RoomNo = institutionArchiveNo.RoomNo,
                    Remark = institutionArchiveNo.Remark
                };

                db.InstitutionArchiveNo.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                
                    UserHelper.OperationLog(Request.UserHostAddress, "Institution Folder", "Success", User.Identity.Name, User.Identity.Name + " updated institution folder of " + entity.InstitutionName + " at date " + DateTime.Today,Modules.ArchiveAndLetters);
                
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
                    InstitutionName = institutionArchiveNo.InstitutionName,
                    InstitutionTypeId = institutionArchiveNo.InstitutionTypeId,
                    ZoneId = institutionArchiveNo.ZoneId,
                    Shelve = institutionArchiveNo.Shelve,
                    Row = institutionArchiveNo.Row,
                    Folder = institutionArchiveNo.Folder,
                    BranchId = institutionArchiveNo.BranchId,
                    RoomNo = institutionArchiveNo.RoomNo,
                    Remark = institutionArchiveNo.Remark
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted institution archive of name" + institutionArchiveNo.InstitutionName);
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
