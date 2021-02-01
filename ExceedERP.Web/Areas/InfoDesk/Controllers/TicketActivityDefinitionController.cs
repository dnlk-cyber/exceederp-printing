using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.InfoDesk.Support;
using ExceedERP.Core.Domain.InfoDesk;

namespace ExceedERP.Web.Areas.InfoDesk.Controllers
{
   

    public class TicketActivityDefinitionController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TicketActivityDefinitions_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<TicketActivityDefinition_HelpDesk> ticketactivitydefinitions = db.TicketActivityDefinition_HelpDesks;
            DataSourceResult result = ticketactivitydefinitions.ToDataSourceResult(request, ticketActivityDefinition => new {
                TicketActivityDefinitionID = ticketActivityDefinition.TicketActivityDefinition_HelpDeskId,
                Description = ticketActivityDefinition.Description,
                Index = ticketActivityDefinition.Index,
                CanFinalize = ticketActivityDefinition.CanFinalize,
                TicketActivityStatus=ticketActivityDefinition.TicketActivityStatus,
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
        public ActionResult TicketActivityDefinitions_Create([DataSourceRequest]DataSourceRequest request, TicketActivityDefinition_HelpDesk ticketActivityDefinition)
        {

            List<TicketActivityDefinition_HelpDesk> ticketActivityDefinitionList = db.TicketActivityDefinition_HelpDesks.Where(tad=>tad.Index== ticketActivityDefinition.Index).ToList();

            var TicketActivityDefinition = db.TicketActivityDefinition_HelpDesks.Where(x => x.Description == ticketActivityDefinition.Description).ToList();
            
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
                    ticketActivityDefinitionList = db.TicketActivityDefinition_HelpDesks.ToList();
                    if (ticketActivityDefinition.CanFinalize == true)
                    {
                        foreach (TicketActivityDefinition_HelpDesk ticket in ticketActivityDefinitionList)
                        {
                            ticket.CanFinalize = false;
                            db.TicketActivityDefinition_HelpDesks.Attach(ticket);
                            db.Entry(ticket).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    var entity = new TicketActivityDefinition_HelpDesk
                    {
                        TicketActivityStatus = ticketActivityDefinition.TicketActivityStatus,
                        Description = ticketActivityDefinition.Description,
                        Index = ticketActivityDefinition.Index,
                        CanFinalize = ticketActivityDefinition.CanFinalize,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now,
                        Remark = ticketActivityDefinition.Remark
                    };
                    db.TicketActivityDefinition_HelpDesks.Add(entity);
                    db.SaveChanges();
                    ticketActivityDefinition.TicketActivityDefinition_HelpDeskId = entity.TicketActivityDefinition_HelpDeskId;
                }
            }
            

            return Json(new[] { ticketActivityDefinition }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TicketActivityDefinitions_Update([DataSourceRequest]DataSourceRequest request, TicketActivityDefinition_HelpDesk ticketActivityDefinition)
        {
            if (ModelState.IsValid)
            {
                List<TicketActivityDefinition_HelpDesk> ticketActivityDefinitionList = ticketActivityDefinitionList = db.TicketActivityDefinition_HelpDesks.ToList();
                if (ticketActivityDefinition.CanFinalize == true)
                {
                    foreach (TicketActivityDefinition_HelpDesk ticket in ticketActivityDefinitionList)
                    {
                        ticket.CanFinalize = false;
                        db.TicketActivityDefinition_HelpDesks.Attach(ticket);
                        db.Entry(ticket).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                db = new ExceedDbContext();
                var entity = new TicketActivityDefinition_HelpDesk
                {
                    TicketActivityDefinition_HelpDeskId = ticketActivityDefinition.TicketActivityDefinition_HelpDeskId,
                    Description = ticketActivityDefinition.Description,
                    Index = ticketActivityDefinition.Index,
                    CanFinalize = ticketActivityDefinition.CanFinalize,
                    Remark = ticketActivityDefinition.Remark,
                    TicketActivityStatus = ticketActivityDefinition.TicketActivityStatus,
                };

                db.TicketActivityDefinition_HelpDesks.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { ticketActivityDefinition }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TicketActivityDefinitions_Destroy([DataSourceRequest]DataSourceRequest request, TicketActivityDefinition_HelpDesk ticketActivityDefinition)
        {
            if (ModelState.IsValid)
            {
                var entity = new TicketActivityDefinition_HelpDesk
                {
                    TicketActivityDefinition_HelpDeskId = ticketActivityDefinition.TicketActivityDefinition_HelpDeskId,
                    Description = ticketActivityDefinition.Description,
                    Index = ticketActivityDefinition.Index,
                    CanFinalize = ticketActivityDefinition.CanFinalize,
                    Remark = ticketActivityDefinition.Remark,
                    TicketActivityStatus = ticketActivityDefinition.TicketActivityStatus,
                };

                db.TicketActivityDefinition_HelpDesks.Attach(entity);
                db.TicketActivityDefinition_HelpDesks.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted  Ticket Activity number " + ticketActivityDefinition.TicketActivityDefinition_HelpDeskId);
      
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
