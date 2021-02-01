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
    public class TicketActivityDefinitionController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TicketActivityDefinitions_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<TicketActivityDefinition> ticketactivitydefinitions = db.TicketActivityDefinitions;
            DataSourceResult result = ticketactivitydefinitions.ToDataSourceResult(request, ticketActivityDefinition => new {
                TicketActivityDefinitionID = ticketActivityDefinition.TicketActivityDefinitionID,
                Description = ticketActivityDefinition.Description,
                Index = ticketActivityDefinition.Index,
                CanFinalize = ticketActivityDefinition.CanFinalize,
                CreatedBy = ticketActivityDefinition.CreatedBy,
                DateCreated = ticketActivityDefinition.DateCreated,
                LastModified = ticketActivityDefinition.LastModified,
                ModifiedBy = ticketActivityDefinition.ModifiedBy,
                OrgBranchName = ticketActivityDefinition.OrgBranchName,
                Remark = ticketActivityDefinition.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TicketActivityDefinitions_Create([DataSourceRequest]DataSourceRequest request, TicketActivityDefinition ticketActivityDefinition)
        {

            List<TicketActivityDefinition> ticketActivityDefinitionList = db.TicketActivityDefinitions.Where(tad=>tad.Index== ticketActivityDefinition.Index).ToList();

            var TicketActivityDefinition = db.TicketActivityDefinitions.Where(x => x.Description == ticketActivityDefinition.Description).ToList();
            
            if (ticketActivityDefinitionList.Any())
            {
                ModelState.AddModelError("InvalidIndexValue","Please readjust index value!");
            }
            else if (TicketActivityDefinition.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("SameName", " Name Already Registered ! ");
                return Json(new[] { ticketActivityDefinition }.ToDataSourceResult(request, ModelState));
            }
            else
            {
                if (ModelState.IsValid)
                {
                    ticketActivityDefinitionList = db.TicketActivityDefinitions.ToList();
                    if (ticketActivityDefinition.CanFinalize == true)
                    {
                        foreach (TicketActivityDefinition ticket in ticketActivityDefinitionList)
                        {
                            ticket.CanFinalize = false;
                            db.TicketActivityDefinitions.Attach(ticket);
                            db.Entry(ticket).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    var entity = new TicketActivityDefinition
                    {
                        Description = ticketActivityDefinition.Description,
                        Index = ticketActivityDefinition.Index,
                        CanFinalize = ticketActivityDefinition.CanFinalize,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now,
                        Remark = ticketActivityDefinition.Remark
                    };
                    db.TicketActivityDefinitions.Add(entity);
                    db.SaveChanges();
                    ticketActivityDefinition.TicketActivityDefinitionID = entity.TicketActivityDefinitionID;
                }
            }
            

            return Json(new[] { ticketActivityDefinition }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TicketActivityDefinitions_Update([DataSourceRequest]DataSourceRequest request, TicketActivityDefinition ticketActivityDefinition)
        {
            if (ModelState.IsValid)
            {
                List<TicketActivityDefinition> ticketActivityDefinitionList = ticketActivityDefinitionList = db.TicketActivityDefinitions.ToList();
                if (ticketActivityDefinition.CanFinalize == true)
                {
                    foreach (TicketActivityDefinition ticket in ticketActivityDefinitionList)
                    {
                        ticket.CanFinalize = false;
                        db.TicketActivityDefinitions.Attach(ticket);
                        db.Entry(ticket).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                db = new ExceedDbContext();
                var entity = new TicketActivityDefinition
                {
                    TicketActivityDefinitionID = ticketActivityDefinition.TicketActivityDefinitionID,
                    Description = ticketActivityDefinition.Description,
                    Index = ticketActivityDefinition.Index,
                    CanFinalize = ticketActivityDefinition.CanFinalize,
                    Remark = ticketActivityDefinition.Remark
                };

                db.TicketActivityDefinitions.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { ticketActivityDefinition }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TicketActivityDefinitions_Destroy([DataSourceRequest]DataSourceRequest request, TicketActivityDefinition ticketActivityDefinition)
        {
            if (ModelState.IsValid)
            {
                var entity = new TicketActivityDefinition
                {
                    TicketActivityDefinitionID = ticketActivityDefinition.TicketActivityDefinitionID,
                    Description = ticketActivityDefinition.Description,
                    Index = ticketActivityDefinition.Index,
                    CanFinalize = ticketActivityDefinition.CanFinalize,
                    Remark = ticketActivityDefinition.Remark
                };

                db.TicketActivityDefinitions.Attach(entity);
                db.TicketActivityDefinitions.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted  Ticket Activity number " + ticketActivityDefinition.TicketActivityDefinitionID);
      
            }

            return Json(new[] { ticketActivityDefinition }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
