using ExceedERP.DataAccess.Context;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Linq;
using System.Web.Mvc;

using System.Data.Entity;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Models;
using ExceedERP.Core.Domain.InfoDesk.Support;
using ExceedERP.Core.Domain.InfoDesk;
using ExceedERP.Web.Areas.InfoDesk.Models;
using System.Collections.Generic;

namespace ExceedERP.Web.Areas.InfoDesk.Controllers
{

    public class AssignTicketController : Controller
    {

        private ExceedDbContext db = new ExceedDbContext();
        ApplicationDbContext udb = new ApplicationDbContext();
        public ActionResult Handler()
        {

            var sysusers =
              udb.Users.Select(
                  o =>
                      new
                      {
                          CustomerID = o.Id,
                          CustomerName = o.FirstName
                      });

            var users = sysusers.ToList();
            ViewData["SysUsers"] = users;
            ViewData["TicketTypes"] = db.TicketType_HelpDesks.ToList();
            ViewData["TicketDefActivity"] = db.TicketActivityDefinition_HelpDesks.ToList();
            return View();
        }
        public ActionResult Index()
        {
            ViewData["AssignTo"] = db.Person_Employee.ToList();
            ViewData["Employees"] = db.Person_Employee.ToList();

            ViewData["Materials"] = db.MaterialItems.ToList();
            ViewData["TicketTypes"] = db.TicketType_HelpDesks.ToList();
            ViewData["TicketDefActivity"] = db.TicketActivityDefinition_HelpDesks.ToList();
            return View();
        }
        public string GetCustomerAddress(int Id)
        {

            var Address = db.Addresses.Where(x => x.Reference == Id.ToString()).FirstOrDefault();
            if (Address != null)
            {
                return Address.Value;
            }
            else
            {
                return string.Empty;
            }
        }
        public string GetAge(DateTime? CreatedDate, DateTime? ClosedDate, bool status)
        {
            string age = "";
            if (status)
            {
                var day = ClosedDate - CreatedDate;

                age = day.Value.Days.ToString() + " Day " + day.Value.Hours.ToString() + " Hours";

            }
            else
            {
                var day = DateTime.Now - CreatedDate;
                age = day.Value.Days.ToString() + " Day " + day.Value.Hours.ToString() + " Hours";
            }
            return age;
        }
        [AuthorizeRoles(InfoDeskConstants.AssignHandler)]
        public ActionResult AssignedHandler_Read([DataSourceRequest]DataSourceRequest request, int Id)
        {
            var tickets = db.AssignTicket_HelpDesks.Where(x => x.CustomerTicketId == Id).ToList();
            var userName = User.Identity.Name;

            var user = udb.Users.FirstOrDefault(x => x.UserName == userName);
            if (user != null)
            {
                var empId = int.Parse(user.EmployeeId);
                tickets = tickets.Where(x => x.AssinedTo == empId).ToList();
            }
            else
            {
                tickets = new List<AssignTicket_HelpDesk>();
            }


            DataSourceResult result = tickets.ToDataSourceResult(request, s => new
            {
                CustomerTicketId = s.CustomerTicketId,
                AssignTicket_HelpDeskId = s.AssignTicket_HelpDeskId,
                Age = 0,//GetAge(s?.DateCreated, s?.ClosedDate, s.IsClosed),
                AssignedDate = s.AssignedDate,
                IsClosed = s.IsClosed,
                AssinedTo = s.AssinedTo,
                IsAssigned = s.IsAssigned,
                Role = s.Role,
                Solution = s.Solution,
                Remarked = s.Remarked,
                IsSendForClosing = s.IsSendForClosing,
                IsSolved = s.IsSolved,
                MaterialItemsId = s.MaterialItemsId

            });

            return Json(result);
        }
        [AuthorizeRoles(InfoDeskConstants.AssignHandler)]
        public ActionResult AssignTickets_Read([DataSourceRequest]DataSourceRequest request, int Id)
        {


            var Tickets = db.AssignTicket_HelpDesks.Where(x => x.CustomerTicketId == Id).ToList();


            DataSourceResult result = Tickets.ToDataSourceResult(request, s => new
            {
                CustomerTicketId = s.CustomerTicketId,
                AssignTicket_HelpDeskId = s.AssignTicket_HelpDeskId,
                Age = 0,//GetAge(s?.DateCreated, s?.ClosedDate, s.IsClosed),
                AssignedDate = s.AssignedDate,
                IsClosed = s.IsClosed,
                AssinedTo = s.AssinedTo,
                IsAssigned = s.IsAssigned,
                Role = s.Role,
                Solution = s.Solution,
                Remarked = s.Remarked,
                IsSendForClosing = s.IsSendForClosing,
                IsSolved = s.IsSolved,
                MaterialItemsId = s.MaterialItemsId

            });

            return Json(result);
        }
        //[AuthorizeRoles(InfoDeskConstants.InfoDeskAssignTicket)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssignTickets_Create([DataSourceRequest]DataSourceRequest request, AssignTicket_HelpDesk assignTicket, int Id)
        {

            if (ModelState.IsValid)
            {
                var entity = new AssignTicket_HelpDesk
                {
                    Age = assignTicket.Age,
                    AssignedDate = assignTicket.AssignedDate,
                    AssinedTo = assignTicket.AssinedTo,
                    ClosedDate = DateTime.Now,
                    IsAssigned = false,
                    CustomerTicketId = Id,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    Role = assignTicket.Role,
                    Solution = assignTicket.Solution,
                    MaterialItemsId = assignTicket.MaterialItemsId,
                    Remarked = assignTicket.Remarked
                };

                db.AssignTicket_HelpDesks.Add(entity);
                db.SaveChanges();

                assignTicket.AssignTicket_HelpDeskId = entity.AssignTicket_HelpDeskId;

            }

            return Json(new[] { assignTicket }.ToDataSourceResult(request));
        }
        // [AuthorizeRoles(InfoDeskConstants.AssignHandler)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssignTickets_Update([DataSourceRequest]DataSourceRequest request, AssignTicket_HelpDesk assignTicket_HelpDesk)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {

                var entity = new AssignTicket_HelpDesk
                {
                    AssignTicket_HelpDeskId = assignTicket_HelpDesk.AssignTicket_HelpDeskId,
                    CustomerTicketId = assignTicket_HelpDesk.CustomerTicketId,
                    Age = assignTicket_HelpDesk.Age,
                    AssignedDate = assignTicket_HelpDesk.AssignedDate,
                    AssinedTo = assignTicket_HelpDesk.AssinedTo,
                    ClosedDate = assignTicket_HelpDesk.ClosedDate,
                    IsAssigned = false,
                    CreatedBy = User.Identity.Name,
                    DateCreated = assignTicket_HelpDesk.DateCreated,
                    Role = assignTicket_HelpDesk.Role,
                    Solution = assignTicket_HelpDesk.Solution,
                    MaterialItemsId = assignTicket_HelpDesk.MaterialItemsId,
                    Remarked = assignTicket_HelpDesk.Remarked
                };


                db.AssignTicket_HelpDesks.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { assignTicket_HelpDesk }.ToDataSourceResult(request));
        }

        [AuthorizeRoles(InfoDeskConstants.AssignHandler)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssignTickets_Destroy([DataSourceRequest]DataSourceRequest request, AssignTicket_HelpDesk AssignTicket)
        {

            var ticket = db.AssignTicket_HelpDesks.FirstOrDefault(x => x.AssignTicket_HelpDeskId == AssignTicket.AssignTicket_HelpDeskId);
            if (ticket != null)
            {
                db.AssignTicket_HelpDesks.Attach(ticket);
                db.AssignTicket_HelpDesks.Remove(ticket);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Priority Type number " + AssignTicket.AssignTicket_HelpDeskId);

            }
            return Json(new[] { AssignTicket }.ToDataSourceResult(request, ModelState));
        }

        public void Assign(string id)
        {
            var customerTicketId = int.Parse(id);
            var assignedTickets = db.AssignTicket_HelpDesks.Where(x => x.CustomerTicketId == customerTicketId).ToList();
            foreach (var assigned in assignedTickets)
            {
                // var assign = new AssignTicket();

                assigned.IsAssigned = true;

                db.Entry(assigned).State = EntityState.Modified;
                db.SaveChanges();
            }
            var customerTicket = db.CustomerTicket_HelpDesks.FirstOrDefault(x => x.CustomerTicket_HelpDeskId == customerTicketId);
            if (customerTicket != null)
            {
                customerTicket.Status = InfoDeskFinalizeType.ASSIGNED;

                db.Entry(customerTicket).State = EntityState.Modified;
                db.SaveChanges();
            }

        }

        public string CheckStatus(int masterId)
        {
            var customerTicket = db.CustomerTicket_HelpDesks.FirstOrDefault(x => x.CustomerTicket_HelpDeskId == masterId);
            if (customerTicket != null)
            {
                if (customerTicket.Status == InfoDeskFinalizeType.ASSIGNED || customerTicket.Status == InfoDeskFinalizeType.CLOSED || customerTicket.Status == InfoDeskFinalizeType.RESOLVED)
                {
                    return "true";
                }
                else
                {
                    return "false";
                }

            }
            return "false";
        }

        public string ApproveSolved(string childId)
        {
            int assignId = int.Parse(childId);
            var ticket = db.AssignTicket_HelpDesks.Find(assignId);
            if (ticket != null)
            {
                if (!ticket.IsSolved)
                {
                    ticket.IsSolved = true;

                    db.Entry(ticket).State = EntityState.Modified;
                    db.SaveChanges();

                    var assignes = db.AssignTicket_HelpDesks.Where(x => x.CustomerTicketId == ticket.CustomerTicketId && !x.IsSolved).ToList();
                    if (!assignes.Any())
                    {
                        var customer = db.CustomerTicket_HelpDesks.FirstOrDefault(x => x.CustomerTicket_HelpDeskId == ticket.CustomerTicketId);

                        if (customer != null)
                        {
                            customer.Status = InfoDeskFinalizeType.RESOLVED;

                            db.Entry(customer).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }

                    return "true";
                }
                else
                {
                    return "false";
                }
            }
            return "faild";
        }
        public string CloseTicket(int mid)
        {
            var ticket = db.CustomerTicket_HelpDesks.Find(mid);
            if (ticket != null)
            {
                if (!ticket.IsClosed)
                {
                    ticket.IsClosed = true;
                    ticket.Status = InfoDeskFinalizeType.CLOSED;

                    db.Entry(ticket).State = EntityState.Modified;
                    db.SaveChanges();

                    return "Successfully Closed";
                }
                else
                {
                    return "Closed Before";
                }

            }
            return "Please select the row ";
        }

        public ActionResult TicketStatus_Read([DataSourceRequest]DataSourceRequest request, int Id)
        {
            var Tickets = db.TicketStatusLog_HelpDesks.Where(x => x.TicketId == Id).ToList();

            var AssignTickets = Tickets.Select(s => new

            {
                User = s.User,
                Remark = s.Remark,

            });


            DataSourceResult result = AssignTickets.ToDataSourceResult(request);

            return Json(result);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}