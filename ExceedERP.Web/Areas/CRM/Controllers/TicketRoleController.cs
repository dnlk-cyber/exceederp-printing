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
    public class TicketRoleController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TicketRoles_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<TicketRole> ticketroles = db.TicketRoles;
            DataSourceResult result = ticketroles.ToDataSourceResult(request, ticketRole => new {
                TicketRoleID = ticketRole.TicketRoleID,
                TicketActivityDefinitionID = ticketRole.TicketActivityDefinitionID,
                RoleID = ticketRole.RoleID,
                TicketGroup = ticketRole.TicketGroup,
                CreatedBy = ticketRole.CreatedBy,
                DateCreated = ticketRole.DateCreated,
                LastModified = ticketRole.LastModified,
                ModifiedBy = ticketRole.ModifiedBy,
                OrgBranchName = ticketRole.OrgBranchName,
                NeedsPassKey = ticketRole.NeedsPassKey

            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TicketRoles_Create([DataSourceRequest]DataSourceRequest request, TicketRole ticketRole)
        {
            if (ModelState.IsValid)
            {
                var entity = new TicketRole
                {
                    TicketActivityDefinitionID = ticketRole.TicketActivityDefinitionID,
                    RoleID = ticketRole.RoleID,
                    TicketGroup = ticketRole.TicketGroup,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    NeedsPassKey = ticketRole.NeedsPassKey
                };

                db.TicketRoles.Add(entity);
                db.SaveChanges();
                ticketRole.TicketRoleID = entity.TicketRoleID;
            }

            return Json(new[] { ticketRole }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TicketRoles_Update([DataSourceRequest]DataSourceRequest request, TicketRole ticketRole)
        {
            if (ModelState.IsValid)
            {
                var entity = new TicketRole
                {
                    TicketRoleID = ticketRole.TicketRoleID,
                    TicketActivityDefinitionID = ticketRole.TicketActivityDefinitionID,
                    RoleID = ticketRole.RoleID,
                    TicketGroup = ticketRole.TicketGroup,
                    NeedsPassKey = ticketRole.NeedsPassKey
                };

                db.TicketRoles.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { ticketRole }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TicketRoles_Destroy([DataSourceRequest]DataSourceRequest request, TicketRole ticketRole)
        {
            if (ModelState.IsValid)
            {
                var entity = new TicketRole
                {
                    TicketRoleID = ticketRole.TicketRoleID,
                    TicketActivityDefinitionID = ticketRole.TicketActivityDefinitionID,
                    RoleID = ticketRole.RoleID,
                    TicketGroup = ticketRole.TicketGroup,
                    NeedsPassKey = ticketRole.NeedsPassKey
                };

                db.TicketRoles.Attach(entity);
                db.TicketRoles.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted  Ticket Role number " + ticketRole.TicketRoleID);
      
            }

            return Json(new[] { ticketRole }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
