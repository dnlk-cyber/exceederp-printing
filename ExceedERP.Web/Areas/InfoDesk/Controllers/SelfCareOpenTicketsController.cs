using System;
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
using ExceedERP.Web.Areas.InfoDesk.Models;


namespace ExceedERP.Web.Areas.InfoDesk.Controllers
{
    [Authorize]
    public class SelfCareOpenTicketsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        ApplicationDbContext udb = new ApplicationDbContext();             
        public ActionResult Index()
        {
            ViewData["branchs"] = db.Branch.ToList();
            ViewData["AssignTo"] = db.Person_Employee.ToList();
            ViewData["Materials"] = db.MaterialItems.ToList();
            ViewData["Group"] = db.SupportGroup_HelpDesks.ToList();
            var sysusers =
              udb.Users.Select(o =>
                      new
                      {
                          CustomerID = o.Id,
                          CustomerName = o.FirstName
                      });

            var users = sysusers.ToList();
            ViewData["SysUsers"] = users;
            ViewData["TicketTypes"] = db.TicketType_HelpDesks.ToList();
            ViewData["TicketDefActivity"] = db.TicketActivityDefinition_HelpDesks.ToList();
            ViewData["Employees"] = db.Person_Employee.ToList();

            return View();
        }      
        public double GetAge(DateTime? CreatedDate, DateTime? ClosedDate, TicketStatus status)
        {
            double age = 0;
            if (status == TicketStatus.Closed && CreatedDate != null && ClosedDate != null)
            {
                age = ClosedDate.Value.Subtract(CreatedDate.Value).Days;
            }
            else if (CreatedDate != null && ClosedDate != null)
            {
                age = DateTime.Now.Subtract(CreatedDate.Value).Days;
            }
            return age;
        }      
        public ActionResult SelfCareOpenTickets_Read([DataSourceRequest]DataSourceRequest request)
        {
            TimeSpan age = new TimeSpan();

            List<CustomerTicket_HelpDesk> customertickets = new List<CustomerTicket_HelpDesk>();

            var userName = User.Identity.Name;

            var user = udb.Users.FirstOrDefault(x => x.UserName == userName);

            if (user != null)
            {
                var empId = user.EmployeeId;
                //  var assinedToMe = db.AssignTicket_HelpDesks.Where(x => x.AssinedTo == empId).ToList()?.Select(x => x.CustomerTicketId);

                customertickets = db.CustomerTicket_HelpDesks.Where(x => x.EmployeeId == empId).ToList();
            }


            DataSourceResult result = customertickets.ToDataSourceResult(request, customerTicket => new
            {
                CustomerTicket_HelpDeskId = customerTicket.CustomerTicket_HelpDeskId,

                //  CustomerAddress = customerTicket.CustomerAddress,

                Description = customerTicket.Description,
                TicketPriority = customerTicket.TicketPriority,
                SupportGroup_HelpDeskId = customerTicket.SupportGroup_HelpDeskId,
                DueDate = customerTicket.DueDate,
                CreatedBy = customerTicket.CreatedBy,
                DateCreated = customerTicket.DateCreated,
                LastModified = customerTicket.LastModified,
                ModifiedBy = customerTicket.ModifiedBy,
                OrgBranchName = customerTicket.OrgBranchName,
                TicketStatus = customerTicket.TicketStatus,
                PriorityColor = customerTicket.PriorityColor,
                Remark = customerTicket.Remark,
                TicketType_HelpDeskId = customerTicket.TicketType_HelpDeskId,
                EmployeeId = customerTicket.EmployeeId,
                AgeOfTicket = GetAge(customerTicket.DateCreated, customerTicket.ClosedDate, customerTicket.TicketStatus),
                Rate = customerTicket.Rate,
                IsClosed = customerTicket.IsClosed

            });

            return Json(result);
        } 
    
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SelfCareOpenTickets_Create([DataSourceRequest]DataSourceRequest request, CustomerTicket_HelpDesk customerTicket_HelpDesk)
        {
            var userName = User.Identity.Name;

            customerTicket_HelpDesk.EmployeeId = 0;

           var user = udb.Users.FirstOrDefault(x => x.UserName == userName);
            if (user != null)
            {
                customerTicket_HelpDesk.EmployeeId = Convert.ToInt32(user.EmployeeId);
            }
            if (ModelState.IsValid)
            {
                var entity = new CustomerTicket_HelpDesk
                {
                    Description = customerTicket_HelpDesk.Description,
                    TicketPriority = customerTicket_HelpDesk.TicketPriority,
                    SupportGroup_HelpDeskId = customerTicket_HelpDesk.SupportGroup_HelpDeskId,
                    ClosedDate = null,
                    TicketType_HelpDeskId = customerTicket_HelpDesk.TicketType_HelpDeskId,
                    DueDate = customerTicket_HelpDesk.DueDate,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    Remark = customerTicket_HelpDesk.Remark,
                    SolutionGiven = customerTicket_HelpDesk.SolutionGiven,
                    EmployeeId = customerTicket_HelpDesk.EmployeeId,

                };


                db.CustomerTicket_HelpDesks.Add(entity);
                db.SaveChanges();
                customerTicket_HelpDesk.CustomerTicket_HelpDeskId = entity.CustomerTicket_HelpDeskId;

            }

            return Json(new[] { customerTicket_HelpDesk }.ToDataSourceResult(request));
        }
      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SelfCareOpenTickets_Update([DataSourceRequest]DataSourceRequest request, CustomerTicket_HelpDesk customerTicket_HelpDesk)
        {
            var ticket = db.CustomerTicket_HelpDesks.FirstOrDefault(x => x.CustomerTicket_HelpDeskId == customerTicket_HelpDesk.CustomerTicket_HelpDeskId);

            if (ModelState.IsValid)
            {

                ticket.Description = customerTicket_HelpDesk.Description;
                ticket.TicketPriority = customerTicket_HelpDesk.TicketPriority;
                ticket.SupportGroup_HelpDeskId = customerTicket_HelpDesk.SupportGroup_HelpDeskId;
                ticket.DueDate = customerTicket_HelpDesk.DueDate;
                ticket.CreatedBy = customerTicket_HelpDesk.CreatedBy;
                ticket.DateCreated = customerTicket_HelpDesk.DateCreated;
                ticket.LastModified = DateTime.Now;
                ticket.TicketType_HelpDeskId = customerTicket_HelpDesk.TicketType_HelpDeskId;
                ticket.ModifiedBy = User.Identity.Name;
                ticket.Remark = customerTicket_HelpDesk.Remark;
                ticket.SolutionGiven = customerTicket_HelpDesk.SolutionGiven;
                ticket.EmployeeId = customerTicket_HelpDesk.EmployeeId;

                //var Status = db.StatusTypes.Where(x => x.Name == customerTicket.StatusType).FirstOrDefault();
                //var Priority = db.PriorityType_HelpDesks.Where(x => x.Name == customerTicket_HelpDesk.Priority).FirstOrDefault();
                ////if (Status != null)
                ////{
                ////    Ticket.StatusColor = Status.Color;
                ////}
                //if (Priority != null)
                //{
                //    ticket.PriorityColor = Priority.Color;
                //}


                //var Group = db.SupportGroup_HelpDesks.FirstOrDefault(x => x.Name == ticket.TicketGroup);
                //if (Group != null)
                //{

                //    var Memebers = db.SupportGroupDetail_HelpDesks.Where(x => x.SupportGroupId == Group.SupportGroup_HelpDeskId).ToList();

                //}
                //db.CustomerTicket_HelpDesks.Attach(ticket);
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { customerTicket_HelpDesk }.ToDataSourceResult(request, ModelState));
        }
       
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SelfCareOpenTickets_Destroy([DataSourceRequest]DataSourceRequest request, CustomerTicket_HelpDesk customerTicket)
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
        public ActionResult Confirmation(BindingModel model)
        {
            int customerTicketId = int.Parse(model.Id);
            var customerTicket = db.CustomerTicket_HelpDesks.FirstOrDefault(x => x.CustomerTicket_HelpDeskId == customerTicketId);

            customerTicket.IsConfirmedByTheRequester = Convert.ToBoolean(model.IsConfirmedByTheRequester);
            
            if (customerTicket.IsConfirmedByTheRequester)
            {
                customerTicket.Rate = Convert.ToInt32(model.Rate);
                customerTicket.TicketStatus = TicketStatus.Resolved;
            }
            if (!customerTicket.IsConfirmedByTheRequester)
            {
                customerTicket.Rate = 0;
                customerTicket.TicketStatus = TicketStatus.Waiting;
            }

            db.Entry(customerTicket).State = EntityState.Modified;
            db.SaveChanges();
            //
            return RedirectToAction("Index");
        }
        public bool GetStatus(int mid)
        {
            var customerTicket = db.CustomerTicket_HelpDesks.FirstOrDefault(x=>x.CustomerTicket_HelpDeskId == mid);
            if (customerTicket != null)
            {
                var assign = db.AssignTicket_HelpDesks.Where(x=>x.CustomerTicketId == customerTicket.CustomerTicket_HelpDeskId).Where(x=>!x.IsSendForClosing).ToList();
                if (assign.Any())
                {
                    return false;
                }
            }
            return true;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
