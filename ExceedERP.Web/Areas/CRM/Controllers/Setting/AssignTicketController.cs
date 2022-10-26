using ExceedERP.DataAccess.Context;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.CRM;
using System.Data.Entity;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Areas.CRM.Models;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Models;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.CRMAdmin)]
    public class AssignTicketController : Controller
    {

        private ExceedDbContext db = new ExceedDbContext();
        ApplicationDbContext udb = new ApplicationDbContext();
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
        public string GetCustomerAddress(int Id)
        {
            
            var Address = db.Addresses.Where(x => x.Reference == Id.ToString()).FirstOrDefault();
            if(Address != null )
            {
                return Address.Value;
            }
            else
            {
                return string.Empty;
            }
        }
        public string GetAge(DateTime AssignedDate, bool Status, bool IsActive , DateTime ClosedDate)
        {
            string Duration = string.Empty;
            if (Status && IsActive)
            {
                
                TimeSpan Span = ClosedDate - AssignedDate;
                Duration = Span.Days.ToString() + " Day " + Span.Hours.ToString() + " Hours";
            }
            else
            {
                if (IsActive)
                {
                    TimeSpan Span = DateTime.Now - AssignedDate;
                    Duration = Span.Days.ToString() + " Day " + Span.Hours.ToString() + " Hours";
                }
                else
                {
                    TimeSpan Span = DateTime.Now - DateTime.Now;
                    Duration = Span.Days.ToString() + " Day " + Span.Hours.ToString() + " Hours";
                }
            }

            return Duration;
        }

        public ActionResult TicketStatus_Read([DataSourceRequest]DataSourceRequest request, int Id)
        {
            var Tickets = db.TicketStatusLogs.Where(x => x.TicketId == Id).ToList();

            var AssignTickets = Tickets.Select(s => new

            {
                User = s.User,
                Remark = s.Remark,

            });

            
            DataSourceResult result = AssignTickets.ToDataSourceResult(request);

            return Json(result);
        }

        public ActionResult AssignTickets_Read([DataSourceRequest]DataSourceRequest request, int Id)
        {
            var  Tickets = db.AssignTickets.Where(x=>x.CustomerTicketId == Id).ToList();
            foreach (var t in Tickets)
            {
          //      var CustomerTickets = db.CustomerTickets.Where(x => x.CustomerTicketID == t.CustomerTicketId).FirstOrDefault();

                t.Age = GetAge(DateTime.Parse(t.DateCreated.ToString()), t.IsClosed, t.IsActive, t.ClosedDate);
            }
            var AssignTickets = Tickets.Select(s => new AssignTicketViewModel
            { 
                Age = s.Age,
             IsTransfer = s.IsTransfer,
            TransferTo = s.TransferTo,
            AssignedDate = s.AssignedDate,
                     IsClosed = s.IsClosed,
            AssignTicketId = s.AssignTicketId,
            AssinedTo = s.AssinedTo,
            IsActive = s.IsActive,
            TicketDefintion = s.TicketDefintion,
            TicketId = s.CustomerTicketId.ToString(),
            
            });
          
             DataSourceResult result = AssignTickets.ToDataSourceResult(request);

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssignTickets_Create([DataSourceRequest]DataSourceRequest request, AssignTicketViewModel AssignTicket, int Id)
        {
            ModelState.Remove("CustomerTicketId");
            var Group = db.AssignTickets.Where(x => x.CustomerTicketId == Id && x.AssinedTo == AssignTicket.AssinedTo).ToList();
            if (Group.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("SameName", " The Ticket Already Assigned to " + AssignTicket.AssinedTo + " ! ");
                return Json(new[] { AssignTicket }.ToDataSourceResult(request, ModelState));
            }

            var entity = new AssignTicket
            {
                Age = AssignTicket.Age,
                IsTransfer = AssignTicket.IsTransfer,
                TransferTo = AssignTicket.TransferTo,
                AssignedDate = AssignTicket.AssignedDate,
                AssinedTo = AssignTicket.AssinedTo,
                ClosedDate = DateTime.Now,
                TicketDefintion = AssignTicket.TicketDefintion,
                CustomerTicketId = Id,
                CreatedBy = User.Identity.Name,
                DateCreated = DateTime.Now,
                Remark = string.Empty,
            };

            db.AssignTickets.Add(entity);
            db.SaveChanges();

            AssignTicket.AssignTicketId = entity.AssignTicketId;
            var Ticket = db.CustomerTickets.Where(x => x.CustomerTicketID == Id).FirstOrDefault();

            Ticket.Status = FinalizeType.ASSIGNED;

            db.CustomerTickets.Attach(Ticket);
            db.Entry(Ticket).State = EntityState.Modified;
            db.SaveChanges();

            return Json(new[] { entity }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssignTickets_Update([DataSourceRequest]DataSourceRequest request, AssignTicketViewModel AssignTicket)
        {
            var Assign = db.AssignTickets.Where(x=>x.AssignTicketId == AssignTicket.AssignTicketId).FirstOrDefault();
           

                Assign.IsTransfer = AssignTicket.IsTransfer;
                Assign.TransferTo = AssignTicket.TransferTo;
            
                if(Assign.IsActive == false && AssignTicket.IsActive == true)
                {
                    Assign.AssignedDate = DateTime.Now;
                    Assign.IsActive = true;
                    Assign.IsClosed = false;
                    Assign.ClosedDate = DateTime.Now;
                    var Ticket = db.CustomerTickets.Where(x => x.CustomerTicketID.ToString() == AssignTicket.TicketId).FirstOrDefault();

                    Ticket.Status = FinalizeType.ASSIGNED;
                    Assign.TicketDefintion = Ticket.TicketDefintion;
                    db.CustomerTickets.Attach(Ticket);
                    db.Entry(Ticket).State = EntityState.Modified;
                    db.SaveChanges();
                }

                if (Assign.IsActive == true && AssignTicket.IsActive == false)
                {
                      Assign.ClosedDate = DateTime.Now;
                      Assign.IsClosed =true;
                }
                Assign.IsActive = AssignTicket.IsActive; 

                db.AssignTickets.Attach(Assign);
                db.Entry(Assign).State = EntityState.Modified;
                db.SaveChanges();
           

            return Json(new[] { Assign }.ToDataSourceResult(request));
        }



        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssignTickets_Destroy([DataSourceRequest]DataSourceRequest request, AssignTicket AssignTicket)
        {

            var Ticket = db.AssignTickets.Where(x => x.AssignTicketId == AssignTicket.AssignTicketId).FirstOrDefault();
            if (Ticket != null)
            {
                db.AssignTickets.Attach(Ticket);
                db.AssignTickets.Remove(Ticket);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Priority Type number " + AssignTicket.AssignTicketId);

            }
            return Json(new[] { AssignTicket }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult UpdateStatus(BindingModel model)
        {
            try
            {
                var AssignedTicket = db.AssignTickets.Where(x => x.AssignTicketId.ToString() == model.Id).FirstOrDefault();
                var Status = db.StatusTypes.Where(x => x.StatusTypeId.ToString() == model.Name).FirstOrDefault();
                var CustomerTicket = db.CustomerTickets.Where(x => x.CustomerTicketID == AssignedTicket.CustomerTicketId).FirstOrDefault();
                var TicketDef = db.TicketActivityDefinitions.Where(x => x.Description == AssignedTicket.TicketDefintion).FirstOrDefault();
                if (Status.Status == FinalizeType.CLOSED)
                {
                    if (TicketDef.CanFinalize)
                    {
                        CustomerTicket.Status = Status.Status;
                        CustomerTicket.ClosedDate = DateTime.Now;
                        db.CustomerTickets.Attach(CustomerTicket);
                        db.Entry(CustomerTicket).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
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
                        ViewBag.StateMsg = "Ticket Definition doesn’t not Allow Finalization ! ";


                        return View("index");
                    }
                }
                else
                {
                    CustomerTicket.Status = Status.Status;
                    db.CustomerTickets.Attach(CustomerTicket);
                    db.Entry(CustomerTicket).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch (Exception e)
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
                return View("index");
            }
            return RedirectToAction("Index");
        }
        public ActionResult SendForClosing(BindingModel model)
        {
            try
            {
                var AssignedTicket = db.AssignTickets.Where(x => x.AssignTicketId.ToString() == model.Id).FirstOrDefault();
                var Status = db.StatusTypes.Where(x => x.StatusTypeId.ToString() == model.Name).FirstOrDefault();
                var CustomerTicket = db.CustomerTickets.Where(x => x.CustomerTicketID == AssignedTicket.CustomerTicketId).FirstOrDefault();
                var TicketDef = db.TicketActivityDefinitions.Where(x => x.Description == AssignedTicket.TicketDefintion).FirstOrDefault();

                CustomerTicket.IsSendForClosing = true;

                db.CustomerTickets.Attach(CustomerTicket);
                db.Entry(CustomerTicket).State = EntityState.Modified;
                db.SaveChanges();

                AssignedTicket.IsClosed = true;
                AssignedTicket.Remark = model.Name;
                AssignedTicket.ClosedDate = DateTime.Now;
                db.AssignTickets.Attach(AssignedTicket);
                db.Entry(AssignedTicket).State = EntityState.Modified;
                db.SaveChanges();

                TicketStatusLog Log = new TicketStatusLog
                {
                    User = AssignedTicket.AssinedTo,
                    Remark = model.Name,
                    TicketId = CustomerTicket.CustomerTicketID,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name
                };
                db.TicketStatusLogs.Add(Log);
                db.SaveChanges();


            }
            catch (Exception e)
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
                return View("index");
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}