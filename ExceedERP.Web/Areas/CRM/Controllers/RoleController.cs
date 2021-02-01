﻿using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
﻿using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;  

namespace ExceedERP.Web.Areas.CRM.Controllers
{
       [AuthorizeRoles(CRMRoles.CRMAdmin)] 
    public class RoleController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            IQueryable<Role> roles = db.Roles;
            ViewData["RoleList"] = roles;
            return View();
        }

        public ActionResult Roles_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Role> roles = db.Roles;
            DataSourceResult result = roles.ToDataSourceResult(request, role => new {
                RoleID = role.RoleID,
                Description = role.Description,
                ParentID = role.ParentID,
                CreatedBy = role.CreatedBy,
                DateCreated = role.DateCreated,
                LastModified = role.LastModified,
                ModifiedBy = role.ModifiedBy,
                OrgBranchName = role.OrgBranchName,
                Remark = role.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Roles_Create([DataSourceRequest]DataSourceRequest request, Role role)
        {
            if (ModelState.IsValid)
            {
                var entity = new Role
                {
                    Description = role.Description,
                    ParentID = role.ParentID,
                    Remark = role.Remark
                };

                db.Roles.Add(entity);
                db.SaveChanges();
                role.RoleID = entity.RoleID;
                RedirectToAction("Index");

            }

            return Json(new[] { role }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Roles_Update([DataSourceRequest]DataSourceRequest request, Role role)
        {
            if (ModelState.IsValid)
            {
                var entity = new Role
                {
                    RoleID = role.RoleID,
                    Description = role.Description,
                    ParentID = role.ParentID,
                    Remark = role.Remark
                };

                db.Roles.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { role }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Roles_Destroy([DataSourceRequest]DataSourceRequest request, Role role)
        {
            if (ModelState.IsValid)
            {
                var entity = new Role
                {
                    RoleID = role.RoleID,
                    Description = role.Description,
                    ParentID = role.ParentID,
                    Remark = role.Remark
                };

                db.Roles.Attach(entity);
                db.Roles.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted  Role number " + role.RoleID);
      
            }

            return Json(new[] { role }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
