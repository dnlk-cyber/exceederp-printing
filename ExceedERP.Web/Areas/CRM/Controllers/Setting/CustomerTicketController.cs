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
using ExceedERP.Web.Models;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.MarketingUser)]
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
        public ActionResult CustomerPendingTickets_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<CustomerTicket> customertickets = db.CustomerTickets.Where(x=>x.Status != FinalizeType.CLOSED);
            foreach(var ticket in customertickets)
            {
                ticket.CustomerAddress = GetCustomerAddress(ticket.CustomerID);
            }
            DataSourceResult result = customertickets.ToDataSourceResult(request, customerTicket => new
            {
                CustomerTicketID = customerTicket.CustomerTicketID,
                CustomerID = customerTicket.CustomerID,
                CustomerAddress = customerTicket.CustomerAddress,
               IsSendForClosing =    customerTicket.IsSendForClosing,
                Description = customerTicket.Description,
                Priority = customerTicket.Priority,
                EscalationLevel = customerTicket.OrganizationCustomer.EsclationLevel,
                TicketGroup = customerTicket.TicketGroup,
                Source = customerTicket.Source,
                CreatedOn = customerTicket.CreatedOn,
                DueDate = customerTicket.DueDate,
                CreatedBy = customerTicket.CreatedBy,
                DateCreated = customerTicket.DateCreated,
                LastModified = customerTicket.LastModified,
                ModifiedBy = customerTicket.ModifiedBy,
                OrgBranchName = customerTicket.OrgBranchName,
                StatusType = customerTicket.StatusType,
                Status = customerTicket.Status,
                PriorityColor = customerTicket.PriorityColor,
                StatusColor = customerTicket.StatusColor,
                Remark = customerTicket.Remark
            });

            return Json(result);
        }
       
        public ActionResult CustomerTickets_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<CustomerTicket> customertickets = db.CustomerTickets;
            foreach (var ticket in customertickets)
            {
                ticket.CustomerAddress = GetCustomerAddress(ticket.CustomerID);
            }
            DataSourceResult result = customertickets.ToDataSourceResult(request, customerTicket => new
            {
                CustomerTicketID = customerTicket.CustomerTicketID,
                CustomerID = customerTicket.CustomerID,
                EscalationLevel = customerTicket.OrganizationCustomer.EsclationLevel,
                CustomerAddress = customerTicket.CustomerAddress,
             
                Description = customerTicket.Description,
                Priority = customerTicket.Priority,
                TicketGroup = customerTicket.TicketGroup,
               Source = customerTicket.Source,
                CreatedOn = customerTicket.CreatedOn,
                DueDate = customerTicket.DueDate,
                CreatedBy = customerTicket.CreatedBy,
                DateCreated = customerTicket.DateCreated,
                LastModified = customerTicket.LastModified,
                ModifiedBy = customerTicket.ModifiedBy,
                OrgBranchName = customerTicket.OrgBranchName,
                StatusType = customerTicket.StatusType,
                Status = customerTicket.Status,
                PriorityColor = customerTicket.PriorityColor,
                StatusColor = customerTicket.StatusColor,
                Remark = customerTicket.Remark
            });

            return Json(result);
        }
        public ActionResult CustomerAwaitingTickets_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<CustomerTicket> customertickets = db.CustomerTickets.Where(x => x.Status == FinalizeType.WAITING);
            DataSourceResult result = customertickets.ToDataSourceResult(request, customerTicket => new
            {
                CustomerTicketID = customerTicket.CustomerTicketID,
                CustomerID = customerTicket.CustomerID,
                Description = customerTicket.Description,
                Priority = customerTicket.Priority,
                TicketGroup = customerTicket.TicketGroup,
                EscalationLevel = customerTicket.EscalationLevel,
                Source = customerTicket.Source,
                CreatedOn = customerTicket.CreatedOn,
                DueDate = customerTicket.DueDate,
                CreatedBy = customerTicket.CreatedBy,
                DateCreated = customerTicket.DateCreated,
                LastModified = customerTicket.LastModified,
                ModifiedBy = customerTicket.ModifiedBy,
                OrgBranchName = customerTicket.OrgBranchName,
                StatusType = customerTicket.StatusType,
                PriorityColor = customerTicket.PriorityColor,
                StatusColor = customerTicket.StatusColor,
                Remark = customerTicket.Remark
            });

            return Json(result);
        }
        public ActionResult CustomerResolvedTickets_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<CustomerTicket> customertickets = db.CustomerTickets.Where(x => x.Status == FinalizeType.RESOLVED);
            DataSourceResult result = customertickets.ToDataSourceResult(request, customerTicket => new
            {
                CustomerTicketID = customerTicket.CustomerTicketID,
                CustomerID = customerTicket.CustomerID,
                Description = customerTicket.Description,
                Priority = customerTicket.Priority,
                TicketGroup = customerTicket.TicketGroup,
                EscalationLevel = customerTicket.EscalationLevel,
                Source = customerTicket.Source,
                CreatedOn = customerTicket.CreatedOn,
                DueDate = customerTicket.DueDate,
                CreatedBy = customerTicket.CreatedBy,
                DateCreated = customerTicket.DateCreated,
                LastModified = customerTicket.LastModified,
                ModifiedBy = customerTicket.ModifiedBy,
                OrgBranchName = customerTicket.OrgBranchName,
                StatusType = customerTicket.StatusType,
                PriorityColor = customerTicket.PriorityColor,
                StatusColor = customerTicket.StatusColor,
                Remark = customerTicket.Remark
            });

            return Json(result);
        }
        public ActionResult CustomerClosedTickets_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<CustomerTicket> customertickets = db.CustomerTickets.Where(x => x.Status == FinalizeType.CLOSED);
            DataSourceResult result = customertickets.ToDataSourceResult(request, customerTicket => new
            {
                CustomerTicketID = customerTicket.CustomerTicketID,
                CustomerID = customerTicket.CustomerID,
                Description = customerTicket.Description,
                Priority = customerTicket.Priority,
                TicketGroup = customerTicket.TicketGroup,
                EscalationLevel = customerTicket.EscalationLevel,
                Source = customerTicket.Source,
                CreatedOn = customerTicket.CreatedOn,
                DueDate = customerTicket.DueDate,
                CreatedBy = customerTicket.CreatedBy,
                DateCreated = customerTicket.DateCreated,
                LastModified = customerTicket.LastModified,
                ModifiedBy = customerTicket.ModifiedBy,
                OrgBranchName = customerTicket.OrgBranchName,
                StatusType = customerTicket.StatusType,
                PriorityColor = customerTicket.PriorityColor,
                StatusColor = customerTicket.StatusColor,
                Remark = customerTicket.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CustomerTickets_Create([DataSourceRequest]DataSourceRequest request, CustomerTicket customerTicket)
        {
            if (ModelState.IsValid)
            {
                var entity = new CustomerTicket
                {
                    CustomerID = customerTicket.CustomerID,
                    Description = customerTicket.Description,
                    Priority = customerTicket.Priority,
                    TicketGroup = customerTicket.TicketGroup,
                    EscalationLevel = customerTicket.EscalationLevel,
                    Source = customerTicket.Source,
                    ClosedDate= DateTime.Now,
                    CreatedOn = customerTicket.CreatedOn,
                    DueDate = customerTicket.DueDate,
                    StatusType = customerTicket.StatusType,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    Remark = customerTicket.Remark
                };
                var Status = db.StatusTypes.Where(x => x.Name == customerTicket.StatusType).FirstOrDefault();
                var Priority = db.PriorityTypes.Where(x => x.Name == customerTicket.Priority).FirstOrDefault();
                if (Status != null)
                {
                    entity.StatusColor = Status.Color;
                }
                if (Priority != null)
                {
                    entity.PriorityColor = Priority.Color;
                }
                db.CustomerTickets.Add(entity);
                db.SaveChanges();
                customerTicket.CustomerTicketID = entity.CustomerTicketID;


                var Group = db.SupportGroups.Where(x => x.Name == customerTicket.TicketGroup).FirstOrDefault();
                if(Group != null)
                {
                    var Members = db.SupportGroupDetails.Where(x => x.SupportGroupId == Group.SupportGroupId).ToList();
                    foreach (var Member in Members)
                    {

                        var Assign = new AssignTicket
                        {
                            Age = string.Empty,
                            AssignedDate = DateTime.Now,
                            AssinedTo = Member.Name,
                            ClosedDate = DateTime.Now,
                            TicketDefintion = customerTicket.TicketDefintion,
                            CustomerTicketId = customerTicket.CustomerTicketID,
                            CreatedBy = User.Identity.Name,
                            DateCreated = DateTime.Now,
                            Remark = string.Empty,
                        };

                        db.AssignTickets.Add(Assign);
                        db.SaveChanges();
                    }
                }
            }

            return Json(new[] { customerTicket }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CustomerTickets_Update([DataSourceRequest]DataSourceRequest request, CustomerTicket customerTicket)
        {

            var Ticket = db.CustomerTickets.Where(x => x.CustomerTicketID == customerTicket.CustomerTicketID).FirstOrDefault();
              
            if (ModelState.IsValid)
            {
                   Ticket.CustomerID = customerTicket.CustomerID;
                      Ticket.Description = customerTicket.Description;
                      Ticket.Priority = customerTicket.Priority;
                      Ticket.TicketGroup = customerTicket.TicketGroup;
                     Ticket.EscalationLevel = customerTicket.EscalationLevel;
                      Ticket.Source = customerTicket.Source;
                      Ticket.CreatedOn = DateTime.Now;
                      Ticket.DueDate = customerTicket.DueDate;
                      Ticket.CreatedBy = customerTicket.CreatedBy;
                      Ticket.DateCreated = customerTicket.DateCreated;
                      Ticket.LastModified = DateTime.Now;
                      Ticket.ModifiedBy = User.Identity.Name;
                      Ticket.Remark = customerTicket.Remark;
               
                var Status = db.StatusTypes.Where(x => x.Name == customerTicket.StatusType).FirstOrDefault();
                var Priority = db.PriorityTypes.Where(x => x.Name == customerTicket.Priority).FirstOrDefault();
                if (Status != null)
                {
                    Ticket.StatusColor = Status.Color;
                }
                if (Priority != null)
                {
                    Ticket.PriorityColor = Priority.Color;
                }


                var Group = db.SupportGroups.Where(x => x.Name == Ticket.TicketGroup).FirstOrDefault();
                if(Group != null)
                {

                    var Memebers = db.SupportGroupDetails.Where(x => x.SupportGroupId == Group.SupportGroupId).ToList();

                }
                db.CustomerTickets.Attach(Ticket);
                db.Entry(Ticket).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { customerTicket }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CustomerTickets_Destroy([DataSourceRequest]DataSourceRequest request, CustomerTicket customerTicket)
        {
            var Ticket = db.CustomerTickets.Where(x => x.CustomerTicketID == customerTicket.CustomerTicketID).FirstOrDefault();
            if (Ticket != null)
            {

                db.CustomerTickets.Attach(Ticket);
                db.CustomerTickets.Remove(Ticket);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Customer Ticket number " + customerTicket.CustomerTicketID);

                var Tickets = db.AssignTickets.Where(x => x.CustomerTicketId == Ticket.CustomerTicketID).ToList();
                     foreach (var t in Tickets)
                     {
                         db.AssignTickets.Attach(t);
                         db.AssignTickets.Remove(t);
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
