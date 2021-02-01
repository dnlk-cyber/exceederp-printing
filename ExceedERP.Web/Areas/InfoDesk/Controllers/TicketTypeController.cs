﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.InfoDesk;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.InfoDesk.Controllers
{
    public class TicketTypeController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }
  
        public ActionResult TicketTypes_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<TicketType_HelpDesk> tickettypes = db.TicketType_HelpDesks;
            DataSourceResult result = tickettypes.ToDataSourceResult(request, ticketType => new {
                TicketTypeId = ticketType.TicketType_HelpDeskId,
                Type = ticketType.Type,
                Description = ticketType.Description,
                DateCreated = ticketType.DateCreated,
                LastModified = ticketType.LastModified,
                CreatedBy = ticketType.CreatedBy,
                ModifiedBy = ticketType.ModifiedBy
            });

            return Json(result);
        }
     
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TicketTypes_Create([DataSourceRequest]DataSourceRequest request, TicketType_HelpDesk ticketType)
        {
            if (ModelState.IsValid)
            {
                var entity = new TicketType_HelpDesk
                {
                    Type = ticketType.Type,
                    Description = ticketType.Description,
                    DateCreated = ticketType.DateCreated,
                    LastModified = ticketType.LastModified,
                    CreatedBy = ticketType.CreatedBy,
                    ModifiedBy = ticketType.ModifiedBy
                };

                db.TicketType_HelpDesks.Add(entity);
                db.SaveChanges();
                ticketType.TicketType_HelpDeskId = entity.TicketType_HelpDeskId;
            }

            return Json(new[] { ticketType }.ToDataSourceResult(request, ModelState));
        }
      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TicketTypes_Update([DataSourceRequest]DataSourceRequest request, TicketType_HelpDesk ticketType)
        {
            if (ModelState.IsValid)
            {
                var entity = new TicketType_HelpDesk
                {
                    TicketType_HelpDeskId = ticketType.TicketType_HelpDeskId,
                    Type = ticketType.Type,
                    Description = ticketType.Description,
                    DateCreated = ticketType.DateCreated,
                    LastModified = ticketType.LastModified,
                    CreatedBy = ticketType.CreatedBy,
                    ModifiedBy = ticketType.ModifiedBy
                };

                db.TicketType_HelpDesks.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { ticketType }.ToDataSourceResult(request, ModelState));
        }
      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TicketTypes_Destroy([DataSourceRequest]DataSourceRequest request, TicketType_HelpDesk ticketType)
        {
            if (ModelState.IsValid)
            {
                var entity = new TicketType_HelpDesk
                {
                    TicketType_HelpDeskId = ticketType.TicketType_HelpDeskId,
                    Type = ticketType.Type,
                    Description = ticketType.Description,
                    DateCreated = ticketType.DateCreated,
                    LastModified = ticketType.LastModified,
                    CreatedBy = ticketType.CreatedBy,
                    ModifiedBy = ticketType.ModifiedBy
                };

                db.TicketType_HelpDesks.Attach(entity);
                db.TicketType_HelpDesks.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { ticketType }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
