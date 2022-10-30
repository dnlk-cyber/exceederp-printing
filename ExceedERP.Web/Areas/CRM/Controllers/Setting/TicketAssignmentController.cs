using System;
using ExceedERP.Web.Helpers;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.CRM.Controllers
{

    [AuthorizeRoles(CRMRoles.CRMAdmin,CRMRoles.MarketingUser)] 
    public class TicketAssignmentController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TicketAssignments_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<TicketAssignment> ticketassignments = db.TicketAssignments;
            DataSourceResult result = ticketassignments.ToDataSourceResult(request, ticketAssignment => new {
                TicketAssignmentID = ticketAssignment.TicketAssignmentID,
                TicketID = ticketAssignment.TicketID,
                ticketAssignmentID = ticketAssignment.TicketActivityDefinitionID,
                AssignedBy = ticketAssignment.AssignedBy,
                ProcessedBy = ticketAssignment.ProcessedBy,
                AssignmentDate = ticketAssignment.AssignmentDate,
                DueDate = ticketAssignment.DueDate,
                EndDate = ticketAssignment.EndDate,
                CreatedBy = ticketAssignment.CreatedBy,
                DateCreated = ticketAssignment.DateCreated,
                LastModified = ticketAssignment.LastModified,
                ModifiedBy = ticketAssignment.ModifiedBy,
                OrgBranchName = ticketAssignment.OrgBranchName,
                Remark = ticketAssignment.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TicketAssignments_Create([DataSourceRequest]DataSourceRequest request, TicketAssignment ticketAssignment)
        {
            if (ModelState.IsValid)
            {
                var entity = new TicketAssignment
                {
                    TicketID = ticketAssignment.TicketID,
                    TicketAssignmentID = ticketAssignment.TicketAssignmentID,
                    AssignedBy = ticketAssignment.AssignedBy,
                    ProcessedBy = ticketAssignment.ProcessedBy,
                    AssignmentDate = ticketAssignment.AssignmentDate,
                    DueDate = ticketAssignment.DueDate,
                    EndDate = ticketAssignment.EndDate,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    Remark = ticketAssignment.Remark
                };

                db.TicketAssignments.Add(entity);
                db.SaveChanges();
                ticketAssignment.TicketAssignmentID = entity.TicketAssignmentID;
            }

            return Json(new[] { ticketAssignment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TicketAssignments_Update([DataSourceRequest]DataSourceRequest request, TicketAssignment ticketAssignment)
        {
            if (ModelState.IsValid)
            {
                var entity = new TicketAssignment
                {
                    TicketAssignmentID = ticketAssignment.TicketAssignmentID,
                    TicketID = ticketAssignment.TicketID,
                   
                    AssignedBy = ticketAssignment.AssignedBy,
                    ProcessedBy = ticketAssignment.ProcessedBy,
                    AssignmentDate = ticketAssignment.AssignmentDate,
                    DueDate = ticketAssignment.DueDate,
                    EndDate = ticketAssignment.EndDate,
                    Remark = ticketAssignment.Remark
                };

                db.TicketAssignments.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { ticketAssignment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TicketAssignments_Destroy([DataSourceRequest]DataSourceRequest request, TicketAssignment ticketAssignment)
        {
            if (ModelState.IsValid)
            {
                var entity = new TicketAssignment
                {
                    TicketAssignmentID = ticketAssignment.TicketAssignmentID,
                    TicketID = ticketAssignment.TicketID,
                   
                    AssignedBy = ticketAssignment.AssignedBy,
                    ProcessedBy = ticketAssignment.ProcessedBy,
                    AssignmentDate = ticketAssignment.AssignmentDate,
                    DueDate = ticketAssignment.DueDate,
                    EndDate = ticketAssignment.EndDate,
                    Remark = ticketAssignment.Remark
                };

                db.TicketAssignments.Attach(entity);
                db.TicketAssignments.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted  Ticket Assignment number " + ticketAssignment.TicketAssignmentID);
      
            }

            return Json(new[] { ticketAssignment }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
