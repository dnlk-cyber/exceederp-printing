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
using ExceedERP.Core.Domain.InfoDesk;
using ExceedERP.Web.Areas.InfoDesk.Models;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.InfoDesk.Controllers
{
    [Authorize]
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
            var employees = db.Person_Employee.Select(x => new { x.EmployeeId, AssinedTo = x.EmployeeId, AssignedTo = x.EmployeeId, FullNameaEnglish = x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish, FullNameAmharic = x.FirstName + " " + x.MiddleName + " " + x.LastName }).ToList();
            ViewData["Materials"] = db.MaterialItems.ToList();
            ViewData["branchs"] = db.Branch.ToList();
            ViewData["bUnit"] = db.BusinessUnits.ToList();
            ViewData["orgStructure"] = db.MainCompanyStructures.ToList();
            ViewData["AssignTo"] = employees;
            ViewData["Group"] = db.SupportGroup_HelpDesks.ToList();
            var sysusers =
              udb.Users.Select(o => new
                      {
                          CustomerID = o.Id,
                          CustomerName = o.FirstName
                      });
            var users = sysusers.ToList();
            ViewData["SysUsers"] = users;
            ViewData["TicketTypes"] = db.TicketType_HelpDesks.ToList();
            ViewData["TicketDefActivity"] = db.TicketActivityDefinition_HelpDesks.ToList();
            ViewData["Employees"] = employees;
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
        private double GetAge(DateTime? CreatedDate, DateTime? ClosedDate)
        {
            double age = 0;
            if (CreatedDate.HasValue&&ClosedDate.HasValue)
            {
                age = ClosedDate.Value.Subtract(CreatedDate.Value).Days;
            }
            else
            {
                age = DateTime.Now.Subtract(CreatedDate.Value).Days;
            }
            return age;
        }   
        public ActionResult CustomerTickets_Read([DataSourceRequest]DataSourceRequest request)
        {
           
            var customertickets = db.CustomerTicket_HelpDesks.ToList();
            DataSourceResult result = customertickets.ToDataSourceResult(request, customerTicket => new CustomerTicket_HelpDesk
            {
                CustomerTicket_HelpDeskId = customerTicket.CustomerTicket_HelpDeskId,
                EmployeeId = customerTicket.EmployeeId,
                BranchId = customerTicket.BranchId,
                BusinessUnitId = customerTicket.BusinessUnitId,
                OrgStructureId = customerTicket.OrgStructureId,
                Description = customerTicket.Description,
                TicketPriority = customerTicket.TicketPriority,
                SupportGroup_HelpDeskId = customerTicket.SupportGroup_HelpDeskId,
                DueDate = customerTicket.DueDate,
                OverDue = customerTicket.OverDue,
                ClosedDate = customerTicket.ClosedDate,
                IsClosed = customerTicket.IsClosed,
                IsConfirmedByTheRequester = customerTicket.IsConfirmedByTheRequester,
                AssignedDate = customerTicket.AssignedDate,
                AssignedTo = customerTicket.AssignedTo,
                IsAssigned = customerTicket.IsAssigned,
                IsSendForApproval = customerTicket.IsSendForApproval,
                IsMultipleAssign = customerTicket.IsMultipleAssign,
                IsSendForClosing = customerTicket.IsSendForClosing,
                Quantity = customerTicket.Quantity,
                MaterialItemsId = customerTicket.MaterialItemsId,
                Rate = customerTicket.Rate,
                Role = customerTicket.Role,
                RootCuase = customerTicket.RootCuase,
                SolutionGiven = customerTicket.SolutionGiven,
                TicketType_HelpDeskId = customerTicket.TicketType_HelpDeskId,
                TicketStatus = customerTicket.TicketStatus,
                CreatedBy = customerTicket.CreatedBy,
                DateCreated = customerTicket.DateCreated,
                LastModified = customerTicket.LastModified,
                ModifiedBy = customerTicket.ModifiedBy,
                OrgBranchName = customerTicket.OrgBranchName,
                PriorityColor = customerTicket.PriorityColor,
                Remark = customerTicket.Remark,
                AgeOfTicket = GetAge(customerTicket.DateCreated, customerTicket.ClosedDate).ToString(),


            });

            return Json(result);
        }
     
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CustomerTickets_Create([DataSourceRequest]DataSourceRequest request, CustomerTicket_HelpDesk customerTicket_HelpDesk)
        {
            var ticketType = db.TicketType_HelpDesks.Find(customerTicket_HelpDesk.TicketType_HelpDeskId);
            if (ticketType != null && ticketType.DueDays > 0)
            {
                customerTicket_HelpDesk.OverDue = ticketType.DueDays;
                customerTicket_HelpDesk.DueDate = DateTime.Now.AddDays((double)ticketType.DueDays);
                customerTicket_HelpDesk.ClosedDate = DateTime.Now.AddDays((double)ticketType.DueDays);
            }
            var empPlace = db.Placements
                                  .FirstOrDefault(x => x.isCurrent
                                       && x.EmployeeId == customerTicket_HelpDesk.EmployeeId);
            if (empPlace != null)
            {
                customerTicket_HelpDesk.BranchId = empPlace.BranchId;
                customerTicket_HelpDesk.BusinessUnitId = empPlace.BusinessUnitId;
                customerTicket_HelpDesk.OrgStructureId = empPlace.OrgStructureId;
            }
            customerTicket_HelpDesk.DateCreated = DateTime.Now;
            customerTicket_HelpDesk.CreatedBy = User.Identity.Name;
           
            if (ModelState.IsValid)
            {
                
                var entity = GetMappedObject(customerTicket_HelpDesk);
                db.CustomerTicket_HelpDesks.Add(entity);
                db.SaveChanges();

                customerTicket_HelpDesk.CustomerTicket_HelpDeskId = entity.CustomerTicket_HelpDeskId;

                var groupMembers = db.SupportGroupDetail_HelpDesks.Where(x => x.SupportGroupId == customerTicket_HelpDesk.SupportGroup_HelpDeskId).ToList();
                foreach(var groupMember in groupMembers)
                {
                    var person = db.Person_Employee.Find(groupMember.EmployeeId);
                    var Address = db.Addresses.Where(x => x.ReferenceType == "Organization_Customer").FirstOrDefault();
                   
                    var entityAssign = new AssignTicket_HelpDesk
                    {
                        Age = GetAge(customerTicket_HelpDesk?.DateCreated, customerTicket_HelpDesk?.ClosedDate).ToString(),
                        AssignedDate = DateTime.Now,
                        AssinedTo = groupMember.EmployeeId,
                        PhoneNumber = person?.PhoneNo,
                        ClosedDate = DateTime.Now,
                        IsAssigned = true,
                        CustomerTicketId = entity.CustomerTicket_HelpDeskId,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now,
                        Role = "",
                        Solution = "",
                        MaterialItemsId = 0,
                        Remarked = "",
                        Address = Address?.Value,
                        TicketPriority = TicketPriority.High
                    };
                    db.AssignTicket_HelpDesks.Add(entityAssign);
                  

                }
                db.SaveChanges();
            }

            return Json(new[] { customerTicket_HelpDesk }.ToDataSourceResult(request));
        }
      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CustomerTickets_Update([DataSourceRequest]DataSourceRequest request, CustomerTicket_HelpDesk customerTicket_HelpDesk)
        {
            var ticket = db.CustomerTicket_HelpDesks.FirstOrDefault(x => x.CustomerTicket_HelpDeskId == customerTicket_HelpDesk.CustomerTicket_HelpDeskId);
            var empPlace = db.Placements.FirstOrDefault(x => x.isCurrent
                                        && x.EmployeeId == customerTicket_HelpDesk.EmployeeId);


            if (empPlace != null)
            {
                ticket.BranchId = empPlace.BranchId;
                ticket.BusinessUnitId = empPlace.BusinessUnitId;
                ticket.OrgStructureId = empPlace.OrgStructureId;
            }
           var ticketType = db.TicketType_HelpDesks.Find(customerTicket_HelpDesk.TicketType_HelpDeskId);
            if (ticketType != null && ticketType.DueDays > 0)
            {
                customerTicket_HelpDesk.OverDue = ticketType.DueDays;
                customerTicket_HelpDesk.DueDate = DateTime.Now.AddDays((double)ticketType.DueDays);
                customerTicket_HelpDesk.ClosedDate = DateTime.Now.AddDays((double)ticketType.DueDays);
            }
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

                
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { customerTicket_HelpDesk }.ToDataSourceResult(request, ModelState));
        }
       
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

        public ActionResult CustomerAwaitingTickets_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<CustomerTicket_HelpDesk> customertickets = db.CustomerTicket_HelpDesks.Where(x=>x.TicketStatus==global::TicketStatus.Waiting);
            DataSourceResult result = customertickets.ToDataSourceResult(request, customerTicket => new CustomerTicket_HelpDesk
            {
                CustomerTicket_HelpDeskId = customerTicket.CustomerTicket_HelpDeskId,
                EmployeeId = customerTicket.EmployeeId,
                BranchId = customerTicket.BranchId,
                BusinessUnitId = customerTicket.BusinessUnitId,
                OrgStructureId = customerTicket.OrgStructureId,
                Description = customerTicket.Description,
                TicketPriority = customerTicket.TicketPriority,
                SupportGroup_HelpDeskId = customerTicket.SupportGroup_HelpDeskId,
                DueDate = customerTicket.DueDate,
                OverDue = customerTicket.OverDue,
                AgeOfTicket = customerTicket.AgeOfTicket,
                ClosedDate = customerTicket.ClosedDate,
                IsClosed = customerTicket.IsClosed,
                IsConfirmedByTheRequester = customerTicket.IsConfirmedByTheRequester,
                AssignedDate = customerTicket.AssignedDate,
                AssignedTo = customerTicket.AssignedTo,
                IsAssigned = customerTicket.IsAssigned,
                IsSendForApproval = customerTicket.IsSendForApproval,
                IsMultipleAssign = customerTicket.IsMultipleAssign,
                IsSendForClosing = customerTicket.IsSendForClosing,
                Quantity = customerTicket.Quantity,
                MaterialItemsId = customerTicket.MaterialItemsId,
                Rate = customerTicket.Rate,
                Role = customerTicket.Role,
                RootCuase = customerTicket.RootCuase,
                SolutionGiven = customerTicket.SolutionGiven,
                TicketType_HelpDeskId = customerTicket.TicketType_HelpDeskId,
                TicketStatus = customerTicket.TicketStatus,
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
                EmployeeId = customerTicket.EmployeeId,
                BranchId = customerTicket.BranchId,
                BusinessUnitId = customerTicket.BusinessUnitId,
                OrgStructureId = customerTicket.OrgStructureId,
                Description = customerTicket.Description,
                TicketPriority = customerTicket.TicketPriority,
                SupportGroup_HelpDeskId = customerTicket.SupportGroup_HelpDeskId,
                DueDate = customerTicket.DueDate,
                OverDue = customerTicket.OverDue,
                AgeOfTicket = customerTicket.AgeOfTicket,
                ClosedDate = customerTicket.ClosedDate,
                IsClosed = customerTicket.IsClosed,
                IsConfirmedByTheRequester = customerTicket.IsConfirmedByTheRequester,
                AssignedDate = customerTicket.AssignedDate,
                AssignedTo = customerTicket.AssignedTo,
                IsAssigned = customerTicket.IsAssigned,
                IsMultipleAssign = customerTicket.IsMultipleAssign,
                IsSendForClosing = customerTicket.IsSendForClosing,
                Quantity = customerTicket.Quantity,
                MaterialItemsId = customerTicket.MaterialItemsId,
                Rate = customerTicket.Rate,
                Role = customerTicket.Role,
                RootCuase = customerTicket.RootCuase,
                SolutionGiven = customerTicket.SolutionGiven,
                TicketType_HelpDeskId = customerTicket.TicketType_HelpDeskId,
                TicketStatus = customerTicket.TicketStatus,
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


        public ActionResult Confirmation(BindingModel model)
        {

            int customerTicketId = int.Parse(model.Id);
            var customerTicket = db.CustomerTicket_HelpDesks.FirstOrDefault(x => x.CustomerTicket_HelpDeskId == customerTicketId);

            customerTicket.IsConfirmedByTheRequester = Convert.ToBoolean(model.IsConfirmedByTheRequester);

            if (customerTicket.IsConfirmedByTheRequester)
            {
                customerTicket.Rate = Convert.ToInt32(model.Rate);
                customerTicket.Remark = model.Remark;
                customerTicket.IsClosed = true;
                customerTicket.TicketStatus = global::TicketStatus.Closed;
            }
            if (!customerTicket.IsConfirmedByTheRequester)
            {
                customerTicket.Rate = 0;
                customerTicket.Remark = model.Remark;
                customerTicket.IsClosed = false;
                customerTicket.TicketStatus = global::TicketStatus.NOT_SOLVED;
            }

            db.Entry(customerTicket).State = EntityState.Modified;
            db.SaveChanges();
            //
            return RedirectToAction("Index", "CustomerTicket");
        }

        private CustomerTicket_HelpDesk GetMappedObject(CustomerTicket_HelpDesk customerTicket)
        {
            var entity = new CustomerTicket_HelpDesk
            {
                CustomerTicket_HelpDeskId = customerTicket.CustomerTicket_HelpDeskId,
                EmployeeId = customerTicket.EmployeeId,
                BranchId = customerTicket.BranchId,
                BusinessUnitId = customerTicket.BusinessUnitId,
                OrgStructureId = customerTicket.OrgStructureId,
                Description = customerTicket.Description,
                TicketPriority = customerTicket.TicketPriority,
                SupportGroup_HelpDeskId = customerTicket.SupportGroup_HelpDeskId,
                DueDate = customerTicket.DueDate,
                OverDue = customerTicket.OverDue,
                AgeOfTicket = customerTicket.AgeOfTicket,
                ClosedDate = customerTicket.ClosedDate,
                IsClosed = customerTicket.IsClosed,
                IsConfirmedByTheRequester = customerTicket.IsConfirmedByTheRequester,
                AssignedDate = customerTicket.AssignedDate,
                AssignedTo = customerTicket.AssignedTo,
                IsAssigned = customerTicket.IsAssigned,
                IsMultipleAssign = customerTicket.IsMultipleAssign,
                IsSendForClosing = customerTicket.IsSendForClosing,
                Quantity = customerTicket.Quantity,
                MaterialItemsId = customerTicket.MaterialItemsId,
                Rate = customerTicket.Rate,
                Role = customerTicket.Role,
                RootCuase = customerTicket.RootCuase,
                SolutionGiven = customerTicket.SolutionGiven,
                TicketType_HelpDeskId = customerTicket.TicketType_HelpDeskId,
                TicketStatus = customerTicket.TicketStatus,
                CreatedBy = customerTicket.CreatedBy,
                DateCreated = customerTicket.DateCreated,
                LastModified = customerTicket.LastModified,
                ModifiedBy = customerTicket.ModifiedBy,
                OrgBranchName = customerTicket.OrgBranchName,
                PriorityColor = customerTicket.PriorityColor,
                Remark = customerTicket.Remark,
            };
            return entity;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
