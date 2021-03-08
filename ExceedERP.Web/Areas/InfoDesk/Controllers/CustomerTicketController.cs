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
using ExceedERP.Web.Areas.InfoDesk.Models;

namespace ExceedERP.Web.Areas.InfoDesk.Controllers
{

    public class CustomerTicketController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        ApplicationDbContext udb = new ApplicationDbContext();

        public string GetCustomerAddress(int Id)
        {

            var Address = db.Addresses.Where(x => x.Reference == Id.ToString() && x.ReferenceType == "Organization_Customer").FirstOrDefault();
            if (Address != null)
            {
                return Address.Value;
            }
            else
            {
                return string.Empty;
            }
        }


        public ActionResult Index()
        {
            ViewData["Materials"] = db.MaterialItems.ToList();
            ViewData["branchs"] = db.Branch.ToList();
            ViewData["AssignTo"] = db.Person_Employee.ToList();

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
        public ActionResult TicketStatus()
        {

            ViewData["TicketTypes"] = db.TicketType_HelpDesks.ToList();
            ViewData["TicketDefActivity"] = db.TicketActivityDefinition_HelpDesks.ToList();

            return View();
        }
        public ActionResult ResolvedTicket()
        {

            return View();
        }
        public ActionResult AwaitingTicket()
        {
            
            return View();
        }
      
        public double GetAge(DateTime? CreatedDate, DateTime? ClosedDate, InfoDeskFinalizeType status)
        {
            double age = 0;
            if (status == InfoDeskFinalizeType.CLOSED)
            {
                age = ClosedDate.Value.Subtract(CreatedDate.Value).Days;
            }
            else
            {
                age = DateTime.Now.Subtract(CreatedDate.Value).Days;
            }
            return age;
        }
        [AuthorizeRoles(InfoDeskConstants.OpenTicket)]
        public ActionResult CustomerTickets_Read([DataSourceRequest]DataSourceRequest request)
        {
            TimeSpan age = new TimeSpan();

            List<CustomerTicket_HelpDesk> customertickets = new List<CustomerTicket_HelpDesk>();

            var userName = User.Identity.Name;

            var user = udb.Users.FirstOrDefault(x => x.UserName == userName);

            if (user != null)
            {
                var empId = int.Parse(user.EmployeeId);
              //  var assinedToMe = db.AssignTicket_HelpDesks.Where(x => x.AssinedTo == empId).ToList()?.Select(x => x.CustomerTicketId);

                customertickets = db.CustomerTicket_HelpDesks.Where(x => x.EmployeeId == empId).ToList();
            }

          
            DataSourceResult result = customertickets.ToDataSourceResult(request, customerTicket => new
            {
                CustomerTicket_HelpDeskId = customerTicket.CustomerTicket_HelpDeskId,

                //  CustomerAddress = customerTicket.CustomerAddress,

                Description = customerTicket.Description,
                Priority = customerTicket.Priority,
                TicketGroup = customerTicket.TicketGroup,
                DueDate = customerTicket.DueDate,
                CreatedBy = customerTicket.CreatedBy,
                DateCreated = customerTicket.DateCreated,
                LastModified = customerTicket.LastModified,
                ModifiedBy = customerTicket.ModifiedBy,
                OrgBranchName = customerTicket.OrgBranchName,
                Status = customerTicket.Status,
                PriorityColor = customerTicket.PriorityColor,
                Remark = customerTicket.Remark,
                SpecificTicketTypeId = customerTicket.SpecificTicketTypeId,
                EmployeeId = customerTicket.EmployeeId,
                Rate = customerTicket.Rate,
                AgeOfTicket = GetAge(customerTicket.DateCreated, customerTicket.ClosedDate, customerTicket.Status),

            });

            return Json(result);
        }
     
        [AuthorizeRoles(InfoDeskConstants.OpenTicket)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CustomerTickets_Create([DataSourceRequest]DataSourceRequest request, CustomerTicket_HelpDesk customerTicket_HelpDesk)
        {
            if (ModelState.IsValid)
            {
                var entity = new CustomerTicket_HelpDesk
                {
                    Description = customerTicket_HelpDesk.Description,
                    Priority = customerTicket_HelpDesk.Priority,
                    TicketGroup = customerTicket_HelpDesk.TicketGroup,
                    ClosedDate = null,
                    SpecificTicketTypeId = customerTicket_HelpDesk.SpecificTicketTypeId,
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
        [AuthorizeRoles(InfoDeskConstants.OpenTicket)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CustomerTickets_Update([DataSourceRequest]DataSourceRequest request, CustomerTicket_HelpDesk customerTicket_HelpDesk)
        {
            var ticket = db.CustomerTicket_HelpDesks.FirstOrDefault(x => x.CustomerTicket_HelpDeskId == customerTicket_HelpDesk.CustomerTicket_HelpDeskId);

            if (ModelState.IsValid)
            {

                ticket.Description = customerTicket_HelpDesk.Description;
                ticket.Priority = customerTicket_HelpDesk.Priority;
                ticket.TicketGroup = customerTicket_HelpDesk.TicketGroup;
                ticket.DueDate = customerTicket_HelpDesk.DueDate;
                ticket.CreatedBy = customerTicket_HelpDesk.CreatedBy;
                ticket.DateCreated = customerTicket_HelpDesk.DateCreated;
                ticket.LastModified = DateTime.Now;
                ticket.SpecificTicketTypeId = customerTicket_HelpDesk.SpecificTicketTypeId;
                ticket.ModifiedBy = User.Identity.Name;
                ticket.Remark = customerTicket_HelpDesk.Remark;
                ticket.SolutionGiven = customerTicket_HelpDesk.SolutionGiven;
                ticket.EmployeeId = customerTicket_HelpDesk.EmployeeId;

                //var Status = db.StatusTypes.Where(x => x.Name == customerTicket.StatusType).FirstOrDefault();
                var Priority = db.PriorityType_HelpDesks.Where(x => x.Name == customerTicket_HelpDesk.Priority).FirstOrDefault();
                //if (Status != null)
                //{
                //    Ticket.StatusColor = Status.Color;
                //}
                if (Priority != null)
                {
                    ticket.PriorityColor = Priority.Color;
                }


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
        [AuthorizeRoles(InfoDeskConstants.OpenTicket)]
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

        [AuthorizeRoles(InfoDeskConstants.OpenTicket)]
        public ActionResult CustomerAwaitingTickets_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<CustomerTicket_HelpDesk> customertickets = db.CustomerTicket_HelpDesks.Where(x => x.Status == InfoDeskFinalizeType.WAITING);
            DataSourceResult result = customertickets.ToDataSourceResult(request, customerTicket => new
            {
                CustomerTicketID = customerTicket.CustomerTicket_HelpDeskId,
                Description = customerTicket.Description,
                Priority = customerTicket.Priority,
                TicketGroup = customerTicket.TicketGroup,
                DueDate = customerTicket.DueDate,
                CreatedBy = customerTicket.CreatedBy,
                DateCreated = customerTicket.DateCreated,
                LastModified = customerTicket.LastModified,
                ModifiedBy = customerTicket.ModifiedBy,
                OrgBranchName = customerTicket.OrgBranchName,
                PriorityColor = customerTicket.PriorityColor,
                Remark = customerTicket.Remark,
            });

            return Json(result);
        }
        public ActionResult CustomerPendingTickets_Read([DataSourceRequest]DataSourceRequest request, string val)
        {
            IQueryable<CustomerTicket_HelpDesk> customertickets = db.CustomerTicket_HelpDesks;//.Where(x => x.Status != FinalizeType.CLOSED);

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
                IsClosed = customerTicket.IsClosed,
                Rate = customerTicket.Rate,
                EmployeeId = customerTicket.EmployeeId
            });
            return Json(result);


        }

        //[AuthorizeRoles(InfoDeskConstants.OpenTicket)]
        //public ActionResult CustomerResolvedTickets_Read([DataSourceRequest]DataSourceRequest request)
        //{
        //    IQueryable<CustomerTicket_HelpDesk> customertickets = db.CustomerTicket_HelpDesks.Where(x => x.Status == FinalizeType.RESOLVED);
        //    DataSourceResult result = customertickets.ToDataSourceResult(request, customerTicket => new
        //    {
        //        CustomerTicketID = customerTicket.CustomerTicket_HelpDeskId,
        //        Description = customerTicket.Description,
        //        Priority = customerTicket.Priority,
        //        TicketGroup = customerTicket.TicketGroup,
        //        DueDate = customerTicket.DueDate,
        //        CreatedBy = customerTicket.CreatedBy,
        //        DateCreated = customerTicket.DateCreated,
        //        LastModified = customerTicket.LastModified,
        //        ModifiedBy = customerTicket.ModifiedBy,
        //        OrgBranchName = customerTicket.OrgBranchName,
        //        PriorityColor = customerTicket.PriorityColor,
        //        Remark = customerTicket.Remark
        //    });

        //    return Json(result);
        //}

        //public ActionResult CustomerClosedTickets_Read([DataSourceRequest]DataSourceRequest request)
        //{
        //    IQueryable<CustomerTicket_HelpDesk> customertickets = db.CustomerTicket_HelpDesks.Where(x => x.Status == FinalizeType.CLOSED);
        //    DataSourceResult result = customertickets.ToDataSourceResult(request, customerTicket => new
        //    {
        //        CustomerTicketID = customerTicket.CustomerTicket_HelpDeskId,
        //        Description = customerTicket.Description,
        //        Priority = customerTicket.Priority,
        //        TicketGroup = customerTicket.TicketGroup,
        //        DueDate = customerTicket.DueDate,
        //        CreatedBy = customerTicket.CreatedBy,
        //        DateCreated = customerTicket.DateCreated,
        //        LastModified = customerTicket.LastModified,
        //        ModifiedBy = customerTicket.ModifiedBy,
        //        OrgBranchName = customerTicket.OrgBranchName,
        //        PriorityColor = customerTicket.PriorityColor,
        //        Remark = customerTicket.Remark
        //    });

        //    return Json(result);
        //}
        public ActionResult Confirmation(BindingModel model)
        {

            int customerTicketId = int.Parse(model.Id);
            var customerTicket = db.CustomerTicket_HelpDesks.FirstOrDefault(x => x.CustomerTicket_HelpDeskId == customerTicketId);

            customerTicket.IsConfirmedByTheRequester = Convert.ToBoolean(model.IsConfirmedByTheRequester);

            if (customerTicket.IsConfirmedByTheRequester)
            {
                customerTicket.Rate = Convert.ToInt32(model.Rate);
                customerTicket.IsClosed = true;
                customerTicket.Status = InfoDeskFinalizeType.CLOSED;
            }
            if (!customerTicket.IsConfirmedByTheRequester)
            {
                customerTicket.Rate = 0;
                customerTicket.IsClosed = false;
                customerTicket.Status = InfoDeskFinalizeType.NOT_SOLVED;
            }

            db.Entry(customerTicket).State = EntityState.Modified;
            db.SaveChanges();
            //
            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
