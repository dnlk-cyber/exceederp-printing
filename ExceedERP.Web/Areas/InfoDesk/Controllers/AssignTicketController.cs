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
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Web.Areas.InfoDesk.Controllers
{

    public class AssignTicketController : Controller
    {

        private ExceedDbContext db = new ExceedDbContext();
        ApplicationDbContext udb = new ApplicationDbContext();
        public ActionResult Handler()
        {

            var organizationCustomer =
                db.OrganizationCustomers.Select(
                    o =>
                        new
                        {
                            CustomerID = o.OrganizationCustomerID,
                            CustomerType = "Organization",
                            CustomerName = o.TradeName
                        });

            var combinedCustomer = organizationCustomer.ToList();
            ViewData["CustomerList"] = combinedCustomer;
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
        public string GetAge(DateTime AssignedDate, bool Status, bool IsActive, DateTime ClosedDate)
        {
            string Duration = string.Empty;
            if (Status)
            {

                TimeSpan Span = ClosedDate - AssignedDate;
                Duration = Span.Days.ToString() + " Day " + Span.Hours.ToString() + " Hours";
            }
            else
            {

                TimeSpan Span = DateTime.Now - AssignedDate;
                Duration = Span.Days.ToString() + " Day " + Span.Hours.ToString() + " Hours";


            }

            return Duration;
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
        [AuthorizeRoles(InfoDeskConstants.AssignHandler)]
        public ActionResult AssignTickets_Read([DataSourceRequest]DataSourceRequest request, int Id)
        {
            var Tickets = db.AssignTicket_HelpDesks.Where(x => x.CustomerTicketId == Id).ToList();
            //foreach (var t in Tickets)
            //{
            //    //      var CustomerTickets = db.CustomerTickets.Where(x => x.CustomerTicketID == t.CustomerTicketId).FirstOrDefault();

            //    t.Age = GetAge(DateTime.Parse(t.AssignedDate.ToString()), t.IsClosed, true, t.ClosedDate);
            //}
            //var AssignTickets = Tickets.Select(s => new AssignTicket
            //{
            //    Age = s.Age,
            //    // IsTransfer = s.IsTransfer,
            //    //    TransferTo = s.TransferTo,
            //    AssignedDate = s.AssignedDate,
            //    IsClosed = s.IsClosed,
            //    AssignTicketId = s.AssignTicketId,
            //    AssinedTo = s.AssinedTo,
            //    Remarked = s.Remarked
            //    //  IsActive = s.IsActive,
            //    //  TicketDefintion = s.TicketDefintion,
            //    //   TicketId = s.CustomerTicketId.ToString(),

            //});

            //DataSourceResult result = AssignTickets.ToDataSourceResult(request);

            DataSourceResult result = Tickets.ToDataSourceResult(request, s => new
            {
                CustomerTicketId = s.CustomerTicketId,
                AssignTicketId = s.AssignTicket_HelpDeskId,
                Age = s.Age,
                AssignedDate = s.AssignedDate,
                IsClosed = s.IsClosed,
                AssinedTo = s.AssinedTo,
                IsAssigned = s.IsAssigned,
                Role = s.Role,
                Solution = s.Solution,
                Remarked = s.Remarked,
                IsSendForClosing = s.IsSendForClosing,
                IsSolved = s.IsSolved,

            });

            return Json(result);
        }
        [AuthorizeRoles(InfoDeskConstants.InfoDeskAssignTicket)]
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
                    Remarked = assignTicket.Remarked
                };

                db.AssignTicket_HelpDesks.Add(entity);
                db.SaveChanges();

                assignTicket.AssignTicket_HelpDeskId = entity.AssignTicket_HelpDeskId;

            }

            return Json(new[] { assignTicket }.ToDataSourceResult(request));
        }
        [AuthorizeRoles(InfoDeskConstants.AssignHandler)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssignTickets_Update([DataSourceRequest]DataSourceRequest request, AssignTicket_HelpDesk assignTicket)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {

                var entity = new AssignTicket_HelpDesk
                {
                    AssignTicket_HelpDeskId = assignTicket.AssignTicket_HelpDeskId,
                    CustomerTicketId = assignTicket.CustomerTicketId,
                    Age = assignTicket.Age,
                    AssignedDate = assignTicket.AssignedDate,
                    AssinedTo = assignTicket.AssinedTo,
                    ClosedDate = assignTicket.ClosedDate,
                    IsAssigned = false,
                    CreatedBy = User.Identity.Name,
                    DateCreated = assignTicket.DateCreated,
                    Role = assignTicket.Role,
                    Solution = assignTicket.Solution,
                    Remarked = assignTicket.Remarked
                };


                db.AssignTicket_HelpDesks.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { assignTicket }.ToDataSourceResult(request));
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
        public ActionResult UpdateStatus(BindingModel model)
        {
            int statusInt = int.Parse(model.Name);
            FinalizeType status = (FinalizeType)(statusInt);
            int customerTicketId = int.Parse(model.Id);
            var customerTicket = db.CustomerTicket_HelpDesks.FirstOrDefault(x => x.CustomerTicket_HelpDeskId == customerTicketId);
            customerTicket.Status = status;
            db.Entry(customerTicket).State = EntityState.Modified;
            db.SaveChanges();
            //
            return RedirectToAction("Index");
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
                customerTicket.Status = FinalizeType.ASSIGNED;

                db.Entry(customerTicket).State = EntityState.Modified;
                db.SaveChanges();
            }

        }

        public string CheckStatus(int masterId)
        {
            var customerTicket = db.CustomerTicket_HelpDesks.FirstOrDefault(x => x.CustomerTicket_HelpDeskId == masterId);
            if (customerTicket != null)
            {
                if (customerTicket.Status == FinalizeType.ASSIGNED || customerTicket.Status == FinalizeType.CLOSED || customerTicket.Status == FinalizeType.RESOLVED)
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

                    var assignes = db.AssignTicket_HelpDesks.Where(x=>x.CustomerTicketId == ticket.CustomerTicketId && !x.IsSolved).ToList();
                    if(!assignes.Any())
                    {
                        var customer = db.CustomerTicket_HelpDesks.FirstOrDefault(x=>x.CustomerTicket_HelpDeskId == ticket.CustomerTicketId);

                        if(customer != null)
                        {
                            customer.Status = FinalizeType.RESOLVED;

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
                    ticket.Status = FinalizeType.RESOLVED;
                    ticket.Status = FinalizeType.CLOSED;

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
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}