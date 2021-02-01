using System;
using ExceedERP.Web.Helpers;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Models;
using ExceedERP.Core.Domain.InfoDesk.Support;
using System.Collections.Generic;
using ExceedERP.Core.Domain.InfoDesk;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Web.Areas.InfoDesk.Controllers
{
    public class HandlerController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private ApplicationDbContext adb = new ApplicationDbContext();
        // GET: InfoDesk/Handler
        public ActionResult Index()
        {
            ViewData["AssignTo"] = db.Person_Employee.ToList();
            return View();
        }
        public ActionResult CustomerPendingTickets_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<CustomerTicket_HelpDesk> customertickets =new List<CustomerTicket_HelpDesk>(); //db.CustomerTickets.Where(x => x.Status != FinalizeType.CLOSED);

            var userName = User.Identity.Name;

            var user = adb.Users.FirstOrDefault(x=>x.UserName == userName);
            if (user != null)
            {
                var empId = int.Parse(user.EmployeeId);
                var assinedToMe = db.AssignTicket_HelpDesks.Where(x=>x.AssinedTo == empId).ToList()?.Select(x=>x.CustomerTicketId);

                customertickets = db.CustomerTicket_HelpDesks.Where(x=>assinedToMe.Contains(x.CustomerTicket_HelpDeskId) && x.Status != FinalizeType.WAITING).ToList();
            }            
          
            DataSourceResult result = customertickets.ToDataSourceResult(request, customerTicket => new
            {

                CustomerTicket_HelpDeskId = customerTicket.CustomerTicket_HelpDeskId,
                Description = customerTicket.Description,
                Priority = customerTicket.Priority,
                TicketGroup = customerTicket.TicketGroup,
                SpecificTicketTypeId = customerTicket.SpecificTicketTypeId,
                DueDate = customerTicket.DueDate,
                CreatedBy = customerTicket.CreatedBy,
                DateCreated = customerTicket.DateCreated,
                LastModified = customerTicket.LastModified,
                ModifiedBy = customerTicket.ModifiedBy,
                OrgBranchName = customerTicket.OrgBranchName,
                Status = customerTicket.Status,
                PriorityColor = customerTicket.PriorityColor,
                Remark = customerTicket.Remark,
                AgeOfTicket = customerTicket.AgeOfTicket,


            });
            return Json(result);


        }
        [AuthorizeRoles(InfoDeskConstants.Handler)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CustomerTickets_Create([DataSourceRequest]DataSourceRequest request, CustomerTicket_HelpDesk customerTicket)
        {
            if (ModelState.IsValid)
            {
                var entity = new CustomerTicket_HelpDesk
                {
                    Description = customerTicket.Description,
                    Priority = customerTicket.Priority,
                    TicketGroup = customerTicket.TicketGroup,
                    ClosedDate = DateTime.Now,
                    SpecificTicketTypeId = customerTicket.SpecificTicketTypeId,
                    DueDate = customerTicket.DueDate,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    Remark = customerTicket.Remark,
                    SolutionGiven = customerTicket.SolutionGiven,
                    EmployeeId = customerTicket.EmployeeId,

                };
               // var Status = db.StatusTypes.Where(x => x.Name == customerTicket.StatusType).FirstOrDefault();
                var Priority = db.PriorityType_HelpDesks.Where(x => x.Name == customerTicket.Priority).FirstOrDefault();
                //if (Status != null)
                //{
                //    entity.StatusColor = Status.Color;
                //}
                if (Priority != null)
                {
                    entity.PriorityColor = Priority.Color;
                }
                db.CustomerTicket_HelpDesks.Add(entity);
                db.SaveChanges();
                customerTicket.CustomerTicket_HelpDeskId = entity.CustomerTicket_HelpDeskId;


            }

            return Json(new[] { customerTicket }.ToDataSourceResult(request));
        }
        [AuthorizeRoles(InfoDeskConstants.Handler)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CustomerTickets_Update([DataSourceRequest]DataSourceRequest request, CustomerTicket_HelpDesk customerTicket)
        {

            var ticket = db.CustomerTicket_HelpDesks.Where(x => x.CustomerTicket_HelpDeskId == customerTicket.CustomerTicket_HelpDeskId).FirstOrDefault();

            if (ModelState.IsValid)
            {

                ticket.Description = customerTicket.Description;
                ticket.Priority = customerTicket.Priority;
                ticket.TicketGroup = customerTicket.TicketGroup;
                ticket.DueDate = customerTicket.DueDate;
                ticket.CreatedBy = customerTicket.CreatedBy;
                ticket.DateCreated = customerTicket.DateCreated;
                ticket.LastModified = DateTime.Now;
                ticket.SpecificTicketTypeId = customerTicket.SpecificTicketTypeId;
                ticket.ModifiedBy = User.Identity.Name;
                ticket.Remark = customerTicket.Remark;
                ticket.SolutionGiven = customerTicket.SolutionGiven;
                ticket.EmployeeId = customerTicket.EmployeeId;

                //var Status = db.StatusTypes.Where(x => x.Name == customerTicket.StatusType).FirstOrDefault();
                //var Priority = db.PriorityTypes.Where(x => x.Name == customerTicket.Priority).FirstOrDefault();
                //if (Status != null)
                //{
                //    ticket.StatusColor = Status.Color;
                //}
                //if (Priority != null)
                //{
                //    ticket.PriorityColor = Priority.Color;
                //}


                //var Group = db.SupportGroups.Where(x => x.Name == ticket.TicketGroup).FirstOrDefault();
                //if (Group != null)
                //{

                //    var Memebers = db.SupportGroupDetails.Where(x => x.SupportGroupId == Group.SupportGroupId).ToList();

                //}
               // db.CustomerTickets.Attach(ticket);
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { customerTicket }.ToDataSourceResult(request, ModelState));
        }
        [AuthorizeRoles(InfoDeskConstants.Handler)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CustomerTickets_Destroy([DataSourceRequest]DataSourceRequest request, CustomerTicket_HelpDesk customerTicket)
        {
            var Ticket = db.CustomerTicket_HelpDesks.Where(x => x.CustomerTicket_HelpDeskId == customerTicket.CustomerTicket_HelpDeskId).FirstOrDefault();
            if (Ticket != null)
            {

                db.CustomerTicket_HelpDesks.Attach(Ticket);
                db.CustomerTicket_HelpDesks.Remove(Ticket);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Customer Ticket number " + customerTicket.CustomerTicket_HelpDeskId);

                var Tickets = db.AssignTicket_HelpDesks.Where(x => x.CustomerTicketId == Ticket.CustomerTicket_HelpDeskId).ToList();
                foreach (var t in Tickets)
                {
                    db.AssignTicket_HelpDesks.Attach(t);
                    db.AssignTicket_HelpDesks.Remove(t);
                    db.SaveChanges();
                }
            }

            return Json(new[] { customerTicket }.ToDataSourceResult(request));
        }

        public string SendForClosing(string childId)
        {
            int assignId = int.Parse(childId);
            var ticket = db.AssignTicket_HelpDesks.Find(assignId);
            if (ticket != null)
            {
                if (!ticket.IsSendForClosing)
                {
                    ticket.IsSendForClosing = true;

                    db.Entry(ticket).State = EntityState.Modified;
                    db.SaveChanges();

                    return "true";
                }
                else
                {
                    return "false";
                }
            }
            return "faild";
        }
    }
}