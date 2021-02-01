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
            ViewData["branchs"] = db.Branch.ToList();


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
            ViewData["Customers"] = db.OrganizationCustomers.ToList();
            ViewData["TicketTypes"] = db.TicketType_HelpDesks.ToList();
            ViewData["TicketDefActivity"] = db.TicketActivityDefinition_HelpDesks.ToList();

            return View();
        }
        public ActionResult TicketStatus()
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
            ViewData["TicketTypes"] = db.TicketType_HelpDesks.ToList();
            ViewData["TicketDefActivity"] = db.TicketActivityDefinition_HelpDesks.ToList();

            return View();
        }
        public ActionResult ResolvedTicket()
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
            return View();
        }
        public ActionResult AwaitingTicket()
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
            return View();
        }
        public ActionResult CustomerPendingTickets_Read([DataSourceRequest]DataSourceRequest request, string val)
        {
            IQueryable<CustomerTicket_HelpDesk> customertickets = db.CustomerTicket_HelpDesks.Where(x => x.Status != FinalizeType.CLOSED);
            //if (val.HasValue())
            //{
            //    List<CustomerTicket> customerTickets = new List<CustomerTicket>();
            //    var user = User.Identity.Name;
            //    //  var assignedTickets = db.AssignTickets.Where(x => x.AssinedTo == user).Where(x => x.IsActive == true).ToList();
            //    var assignedTickets = db.AssignTickets.Where(x => x.AssinedTo == user).ToList();

            //    HashSet<int> ids = new HashSet<int>(assignedTickets.Select(x => x.CustomerTicketId));

            //    customertickets = customertickets.Where(x => ids.Contains(x.CustomerTicketID));
            //    //foreach (var at in assignedTickets)
            //    //{
            //    //    if (at.IsActive == true)
            //    //        customerTickets.Add(db.CustomerTickets.Find(at.CustomerTicketId));
            //    //}
            //}
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

            });
            return Json(result);


        }
        [AuthorizeRoles(InfoDeskConstants.OpenTicket)]
        public ActionResult CustomerTickets_Read([DataSourceRequest]DataSourceRequest request)
        {
            TimeSpan age = new TimeSpan();
            IQueryable<CustomerTicket_HelpDesk> customertickets = db.CustomerTicket_HelpDesks;
            foreach (var ticket in customertickets)
            {
                if (ticket.Status == FinalizeType.CLOSED)
                {
                    age = (TimeSpan)(ticket.ClosedDate - ticket.DateCreated);
                    ticket.AgeOfTicket = age.Days.ToString() + "days" + " " + age.Hours.ToString() + "hours";

                }
                else
                {
                    age = (TimeSpan)(DateTime.Now - ticket.DateCreated);
                    ticket.AgeOfTicket = age.Days.ToString() + "days" + " " + age.Hours.ToString() + "hours";


                }
                //   ticket.CustomerAddress = GetCustomerAddress(ticket.CustomerID);
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
                AgeOfTicket = customerTicket.AgeOfTicket,

            });

            return Json(result);
        }
        [AuthorizeRoles(InfoDeskConstants.OpenTicket)]
        public ActionResult CustomerAwaitingTickets_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<CustomerTicket_HelpDesk> customertickets = db.CustomerTicket_HelpDesks.Where(x => x.Status == FinalizeType.WAITING);
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
        [AuthorizeRoles(InfoDeskConstants.OpenTicket)]
        public ActionResult CustomerResolvedTickets_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<CustomerTicket_HelpDesk> customertickets = db.CustomerTicket_HelpDesks.Where(x => x.Status == FinalizeType.RESOLVED);
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
                Remark = customerTicket.Remark
            });

            return Json(result);
        }
        public ActionResult CustomerClosedTickets_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<CustomerTicket_HelpDesk> customertickets = db.CustomerTicket_HelpDesks.Where(x => x.Status == FinalizeType.CLOSED);
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
                Remark = customerTicket.Remark
            });

            return Json(result);
        }
        [AuthorizeRoles(InfoDeskConstants.OpenTicket)]
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
                //var Status = db.StatusTypes.Where(x => x.Name == customerTicket.StatusType).FirstOrDefault();
                //var Priority = db.PriorityTypes.Where(x => x.Name == customerTicket.Priority).FirstOrDefault();
                //if (Status != null)
                //{
                //    entity.StatusColor = Status.Color;
                //}
                //if (Priority != null)
                //{
                //    entity.PriorityColor = Priority.Color;
                //}

                db.CustomerTicket_HelpDesks.Add(entity);
                db.SaveChanges();
                customerTicket.CustomerTicket_HelpDeskId = entity.CustomerTicket_HelpDeskId;


                //var Group = db.SupportGroups.Where(x => x.Name == customerTicket.TicketGroup).FirstOrDefault();
                //if(Group != null)
                //{
                //    var Members = db.SupportGroupDetails.Where(x => x.SupportGroupId == Group.SupportGroupId).ToList();
                //    foreach (var Member in Members)
                //    {

                //        var Assign = new AssignTicket
                //        {
                //            Age = string.Empty,
                //            AssignedDate = DateTime.Now,
                //            AssinedTo = Member.Name,
                //            ClosedDate = DateTime.Now,
                //             CustomerTicketId = customerTicket.CustomerTicketID,
                //            CreatedBy = User.Identity.Name,
                //            DateCreated = DateTime.Now,
                //            Remark = string.Empty,
                //        };

                //        db.AssignTickets.Add(Assign);
                //        db.SaveChanges();
                //    }
                //}
            }

            return Json(new[] { customerTicket }.ToDataSourceResult(request));
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

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
