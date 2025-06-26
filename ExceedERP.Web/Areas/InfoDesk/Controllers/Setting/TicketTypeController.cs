using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.InfoDesk;
using ExceedERP.DataAccess.Context;
using System;

namespace ExceedERP.Web.Areas.InfoDesk.Controllers.Settings
{
    [Authorize]
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
                TicketType_HelpDeskId = ticketType.TicketType_HelpDeskId,
                Type = ticketType.Type,
                TicketCategory = ticketType.TicketCategory,
                TicketPriority = ticketType.TicketPriority,
                EstimatedCost = ticketType.EstimatedCost,
                TicketCode = ticketType.TicketCode,
                DueDays = ticketType.DueDays,
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
                ticketType.DateCreated = DateTime.Now;
                ticketType.CreatedBy = User.Identity.Name;
                var entity = GetMappedObject(ticketType);
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
                ticketType.LastModified = DateTime.Now;
                ticketType.ModifiedBy = User.Identity.Name;
                var entity = GetMappedObject(ticketType);
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
                var entity = GetMappedObject(ticketType);
                db.TicketType_HelpDesks.Attach(entity);
                db.TicketType_HelpDesks.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { ticketType }.ToDataSourceResult(request, ModelState));
        }

        private TicketType_HelpDesk GetMappedObject(TicketType_HelpDesk ticketType)
        {
            var entity = new TicketType_HelpDesk
            {
                TicketType_HelpDeskId = ticketType.TicketType_HelpDeskId,
                Type = ticketType.Type,
                TicketCategory = ticketType.TicketCategory,
                TicketPriority = ticketType.TicketPriority,
                EstimatedCost = ticketType.EstimatedCost,
                TicketCode = ticketType.TicketCode,
                DueDays = ticketType.DueDays,
                Description = ticketType.Description,
                DateCreated = ticketType.DateCreated,
                LastModified = ticketType.LastModified,
                CreatedBy = ticketType.CreatedBy,
                ModifiedBy = ticketType.ModifiedBy
            };
            return entity;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
