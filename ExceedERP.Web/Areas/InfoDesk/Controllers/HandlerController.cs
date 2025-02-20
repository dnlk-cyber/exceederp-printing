using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Models;
using ExceedERP.Core.Domain.InfoDesk.Support;
using System.Collections.Generic;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using System.Data.Entity.Infrastructure;

namespace ExceedERP.Web.Areas.InfoDesk.Controllers
{
    [Authorize]
    public class HandlerController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private ApplicationDbContext adb = new ApplicationDbContext();
        // GET: InfoDesk/Handler
        public ActionResult Index()
        {
            var employees = db.Person_Employee.Select(x => new { x.EmployeeId, AssinedTo = x.EmployeeId, AssignedTo = x.EmployeeId, FullNameaEnglish = x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish, FullNameAmharic = x.FirstName + " " + x.MiddleName + " " + x.LastName }).ToList();
            ViewData["AssignTo"] = employees;
            ViewData["Employees"] = employees;
            ViewData["Materials"] = db.MaterialItems.ToList();
            ViewData["Group"] = db.SupportGroup_HelpDesks.ToList();
            ViewData["branchs"] = db.Branch.ToList();
            ViewData["bUnit"] = db.BusinessUnits.ToList();
            ViewData["orgStructure"] = db.MainCompanyStructures.ToList();
            return View();
        }
        public ActionResult Handler_Read([DataSourceRequest]DataSourceRequest request)
        {
            var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var assinedToMeTicketIds = db.AssignTicket_HelpDesks.Where(x => x.AssinedTo == employeeId).Select(x => x.CustomerTicketId);
            var customertickets = db.CustomerTicket_HelpDesks.Where(x => assinedToMeTicketIds.Contains(x.CustomerTicket_HelpDeskId) && x.IsSendForApproval).AsQueryable();
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
        public string GetAge(DateTime? CreatedDate, DateTime? ClosedDate, bool status)
        {
            string age = "";
            if (status&&CreatedDate.HasValue&&ClosedDate.HasValue)
            {
                var day = ClosedDate - CreatedDate;

                age = day.Value.Days.ToString() + " Day " + day.Value.Hours.ToString() + " Hours";

            }
           
            return age;
        }

        public ActionResult AssignedHandler_Read([DataSourceRequest]DataSourceRequest request, int Id)
        {
            var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var tickets = db.AssignTicket_HelpDesks.Where(x => x.CustomerTicketId == Id
                                                                && x.AssinedTo == employeeId).ToList();
           
            DataSourceResult result = tickets.ToDataSourceResult(request, assignTicket_HelpDesk => new
            {
                AssignTicket_HelpDeskId = assignTicket_HelpDesk.AssignTicket_HelpDeskId,
                CustomerTicketId = assignTicket_HelpDesk.CustomerTicketId,
                Age = GetAge(assignTicket_HelpDesk?.DateCreated, assignTicket_HelpDesk?.ClosedDate, assignTicket_HelpDesk.IsClosed),
              
                AssignedDate = assignTicket_HelpDesk.AssignedDate,
                AssinedTo = assignTicket_HelpDesk.AssinedTo,
                ClosedDate = assignTicket_HelpDesk.ClosedDate,
                IsAssigned = assignTicket_HelpDesk.IsAssigned,
                Role = assignTicket_HelpDesk.Role,
                Status = assignTicket_HelpDesk.Status,
                RootCause = assignTicket_HelpDesk.RootCause,
                Solution = assignTicket_HelpDesk.Solution,
                Address = assignTicket_HelpDesk.Address,
                PhoneNumber = assignTicket_HelpDesk.PhoneNumber,
                MaterialItemsId = assignTicket_HelpDesk.MaterialItemsId,
                Remarked = assignTicket_HelpDesk.Remarked,
                TicketPriority = assignTicket_HelpDesk.TicketPriority,
                IsClosed = assignTicket_HelpDesk.IsClosed,
                IsOnlineTransferred = assignTicket_HelpDesk.IsOnlineTransferred,
                IsSendForApproval = assignTicket_HelpDesk.IsSendForApproval,
                IsSendForClosing = assignTicket_HelpDesk.IsSendForClosing,
                IsSolved = assignTicket_HelpDesk.IsSolved,
                IsVoid = assignTicket_HelpDesk.IsVoid,
                OnlineApprovedBy = assignTicket_HelpDesk.OnlineApprovedBy,
                OnlineApprovedTime = assignTicket_HelpDesk.OnlineApprovedTime,
                OnlineTransferredTime = assignTicket_HelpDesk.OnlineTransferredTime,
                OrgBranchName = assignTicket_HelpDesk.OrgBranchName,
                Remark = assignTicket_HelpDesk.Remark,
                SendForApprovalBy = assignTicket_HelpDesk.SendForApprovalBy,
                SendForApprovalTime = assignTicket_HelpDesk.SendForApprovalTime,
                VoidBy = assignTicket_HelpDesk.VoidBy,
                VoidTime = assignTicket_HelpDesk.VoidTime,
                IsOnlineApproved = assignTicket_HelpDesk.IsOnlineApproved,
                IsOnlineTransferredBy = assignTicket_HelpDesk.IsOnlineTransferredBy,
                CreatedBy = assignTicket_HelpDesk.CreatedBy,
                DateCreated = assignTicket_HelpDesk.DateCreated,
                ModifiedBy = assignTicket_HelpDesk.ModifiedBy,
                LastModified = assignTicket_HelpDesk.LastModified


            });

            return Json(result);
        }

        public ActionResult AssignTickets_Read([DataSourceRequest]DataSourceRequest request, int Id)
        {
           
            var tickests = db.AssignTicket_HelpDesks.Where(x => x.CustomerTicketId == Id);
            DataSourceResult result = tickests.ToDataSourceResult(request, assignTicket_HelpDesk => new
            {
                AssignTicket_HelpDeskId = assignTicket_HelpDesk.AssignTicket_HelpDeskId,
                CustomerTicketId = assignTicket_HelpDesk.CustomerTicketId,
                Age = GetAge(assignTicket_HelpDesk?.DateCreated, assignTicket_HelpDesk?.ClosedDate, assignTicket_HelpDesk.IsClosed),

                AssignedDate = assignTicket_HelpDesk.AssignedDate,
                AssinedTo = assignTicket_HelpDesk.AssinedTo,
                ClosedDate = assignTicket_HelpDesk.ClosedDate,
                IsAssigned = assignTicket_HelpDesk.IsAssigned,
                Role = assignTicket_HelpDesk.Role,
                Status = assignTicket_HelpDesk.Status,
                RootCause = assignTicket_HelpDesk.RootCause,
                Solution = assignTicket_HelpDesk.Solution,
                Address = assignTicket_HelpDesk.Address,
                PhoneNumber = assignTicket_HelpDesk.PhoneNumber,
                MaterialItemsId = assignTicket_HelpDesk.MaterialItemsId,
                Remarked = assignTicket_HelpDesk.Remarked,
                TicketPriority = assignTicket_HelpDesk.TicketPriority,
                IsClosed = assignTicket_HelpDesk.IsClosed,
                IsOnlineTransferred = assignTicket_HelpDesk.IsOnlineTransferred,
                IsSendForApproval = assignTicket_HelpDesk.IsSendForApproval,
                IsSendForClosing = assignTicket_HelpDesk.IsSendForClosing,
                IsSolved = assignTicket_HelpDesk.IsSolved,
                IsVoid = assignTicket_HelpDesk.IsVoid,
                OnlineApprovedBy = assignTicket_HelpDesk.OnlineApprovedBy,
                OnlineApprovedTime = assignTicket_HelpDesk.OnlineApprovedTime,
                OnlineTransferredTime = assignTicket_HelpDesk.OnlineTransferredTime,
                OrgBranchName = assignTicket_HelpDesk.OrgBranchName,
                Remark = assignTicket_HelpDesk.Remark,
                SendForApprovalBy = assignTicket_HelpDesk.SendForApprovalBy,
                SendForApprovalTime = assignTicket_HelpDesk.SendForApprovalTime,
                VoidBy = assignTicket_HelpDesk.VoidBy,
                VoidTime = assignTicket_HelpDesk.VoidTime,
                IsOnlineApproved = assignTicket_HelpDesk.IsOnlineApproved,
                IsOnlineTransferredBy = assignTicket_HelpDesk.IsOnlineTransferredBy,
                CreatedBy = assignTicket_HelpDesk.CreatedBy,
                DateCreated = assignTicket_HelpDesk.DateCreated,
                ModifiedBy = assignTicket_HelpDesk.ModifiedBy,
                LastModified = assignTicket_HelpDesk.LastModified


            });
            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssignTickets_Create([DataSourceRequest]DataSourceRequest request, AssignTicket_HelpDesk assignTicket, int Id)
        {
           
            assignTicket.Age = GetAge(assignTicket?.DateCreated, assignTicket?.ClosedDate, assignTicket.IsClosed);
            assignTicket.CustomerTicketId = Id;
            assignTicket.IsAssigned = true;
            assignTicket.IsSendForApproval = true;
            assignTicket.CreatedBy = User.Identity.Name;
            assignTicket.DateCreated = DateTime.Now;
            var person = db.Person_Employee.Find(assignTicket.AssinedTo);
            var Address = db.Addresses.Where(x => x.ReferenceType == "Organization_Customer").FirstOrDefault();
            assignTicket.Address = Address?.Value;
            assignTicket.PhoneNumber = person?.PhoneNo;
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(assignTicket);
                db.AssignTicket_HelpDesks.Add(entity);
                db.SaveChanges();
                assignTicket.AssignTicket_HelpDeskId = entity.AssignTicket_HelpDeskId;

            }


            return Json(new[] { assignTicket }.ToDataSourceResult(request));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssignTickets_Update([DataSourceRequest]DataSourceRequest request, AssignTicket_HelpDesk assignTicket)
        {
            assignTicket.Age = GetAge(assignTicket?.DateCreated, assignTicket?.ClosedDate, assignTicket.IsClosed);
            assignTicket.ModifiedBy = User.Identity.Name;
            assignTicket.LastModified = DateTime.Now;
            var person = db.Person_Employee.Find(assignTicket.AssinedTo);
            var Address = db.Addresses.Where(x => x.ReferenceType == "Organization_Customer").FirstOrDefault();
            assignTicket.Address = Address?.Value;
            assignTicket.PhoneNumber = person?.PhoneNo;
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(assignTicket); db.AssignTicket_HelpDesks.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new[] { assignTicket }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CustomerTickets_Update([DataSourceRequest]DataSourceRequest request, CustomerTicket_HelpDesk customerTicket_HelpDesk, int Id)
        {
            var ticket = db.CustomerTicket_HelpDesks.FirstOrDefault(x => x.CustomerTicket_HelpDeskId == Id);

            if (ticket == null)
            {
                return HttpNotFound();
            }

            // Set required fields with values from the existing ticket or the incoming model
            ticket.TicketType_HelpDeskId = customerTicket_HelpDesk.TicketType_HelpDeskId != 0 ? customerTicket_HelpDesk.TicketType_HelpDeskId : ticket.TicketType_HelpDeskId;
            ticket.TicketPriority = customerTicket_HelpDesk.TicketPriority != 0 ? customerTicket_HelpDesk.TicketPriority : ticket.TicketPriority;
            ticket.TicketStatus = customerTicket_HelpDesk.TicketStatus != 0 ? customerTicket_HelpDesk.TicketStatus : ticket.TicketStatus;
            ticket.SupportGroup_HelpDeskId = customerTicket_HelpDesk.SupportGroup_HelpDeskId != 0 ? customerTicket_HelpDesk.SupportGroup_HelpDeskId : ticket.SupportGroup_HelpDeskId;

            // Optional fields, assign if not null
            ticket.AssignedTo = customerTicket_HelpDesk.AssignedTo ?? ticket.AssignedTo;
            ticket.AssignedDate = DateTime.Now;  // Update to the current date for assignment
            ticket.SolutionGiven = !string.IsNullOrEmpty(customerTicket_HelpDesk.SolutionGiven) ? customerTicket_HelpDesk.SolutionGiven : ticket.SolutionGiven;

            // Set Rate to a default value of 1
            ticket.Rate = customerTicket_HelpDesk.Rate != 0 ? customerTicket_HelpDesk.Rate : 1;

            // Retrieve and update placement details if available
            var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == ticket.EmployeeId && x.isCurrent);
            if (placement != null)
            {
                ticket.BranchId = placement.BranchId;
                ticket.BusinessUnitId = placement.BusinessUnitId;
                ticket.OrgStructureId = placement.OrgStructureId;
            }

            // Validate the model state and save changes
            if (ModelState.IsValid)
            {
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { ticket }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Handler_Update([DataSourceRequest] DataSourceRequest request, CustomerTicket_HelpDesk assignTicket_HelpDesk)
        {

            var entity = db.CustomerTicket_HelpDesks.Find(assignTicket_HelpDesk.CustomerTicket_HelpDeskId);

            assignTicket_HelpDesk.TicketPriority = TicketPriority.High;
            if (ModelState.IsValid)
            {
                try
                {
                    // Retrieve the entity from the database to ensure it exists and hasn't been modified

                    if (entity == null)
                    {
                        // Return an error if the entity no longer exists
                        return Json(new { success = false, message = "Entity not found or has been deleted." });
                    }

                    // Update the entity's properties with the new values
                    db.Entry(entity).CurrentValues.SetValues(assignTicket_HelpDesk);
                    db.Entry(entity).State = EntityState.Modified;

                    // Save changes to the database
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    // Handle concurrency exception
                    return Json(new { success = false, message = "Concurrency error: Entity has been modified by another process.", exception = ex.Message });
                }
                catch (Exception ex)
                {
                    // Handle other exceptions
                    return Json(new { success = false, message = ex.Message });
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return Json(new { success = false, message = "Invalid data.", errors });
            }

            return Json(new[] { assignTicket_HelpDesk }.ToDataSourceResult(request));
        }


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

        public JsonResult GetMaterials()
        {
            var materials = db.MaterialItems.ToList();

            return Json(materials.Select(p => new
            {
                MaterialItemsId = p.MaterialItemsId,
                ItemName = p.ItemName
            }), JsonRequestBehavior.AllowGet);

        }
        public string SendForClosing(string childId)
        {
            int assignId = int.Parse(childId);
            var ticket = db.AssignTicket_HelpDesks.Find(assignId);
            if (ticket != null)
            {
                if (!ticket.IsSendForClosing)
                {
                    var customer = db.CustomerTicket_HelpDesks.FirstOrDefault(x => x.CustomerTicket_HelpDeskId == ticket.CustomerTicketId);
                    if (customer != null)
                    {
                        customer.IsSendForClosing = true;
                        customer.TicketStatus = TicketStatus.Resolved;
                        db.Entry(customer).State = EntityState.Modified;
                        db.SaveChanges();

                    }

                    ticket.IsSendForClosing = true;

                    db.Entry(ticket).State = EntityState.Modified;
                    db.SaveChanges();

                    return "Successfulley sent for closing";
                }
                else
                {
                    return "false";
                }

            }
            return "faild";
        }
        public string CheckStatus(int masterId)
        {
            var customer = db.CustomerTicket_HelpDesks.FirstOrDefault(x => x.CustomerTicket_HelpDeskId == masterId);
            if (customer != null)
            {
                if (customer.TicketStatus == TicketStatus.Resolved || customer.TicketStatus == TicketStatus.Closed)
                {
                    return "true";
                }
            }
            return "false";
        }
        private AssignTicket_HelpDesk GetMappedObject(AssignTicket_HelpDesk assignTicket_HelpDesk)
        {
            var entity = new AssignTicket_HelpDesk
            {
                AssignTicket_HelpDeskId = assignTicket_HelpDesk.AssignTicket_HelpDeskId,
                CustomerTicketId = assignTicket_HelpDesk.CustomerTicketId,
                Age = assignTicket_HelpDesk.Age,
                AssignedDate = assignTicket_HelpDesk.AssignedDate,
                AssinedTo = assignTicket_HelpDesk.AssinedTo,
                ClosedDate = assignTicket_HelpDesk.ClosedDate,
                IsAssigned = assignTicket_HelpDesk.IsAssigned,
                Role = assignTicket_HelpDesk.Role,
                Status = assignTicket_HelpDesk.Status,
                RootCause = assignTicket_HelpDesk.RootCause,
                Solution = assignTicket_HelpDesk.Solution,
                Address = assignTicket_HelpDesk.Address,
                PhoneNumber = assignTicket_HelpDesk.PhoneNumber,
                MaterialItemsId = assignTicket_HelpDesk.MaterialItemsId,
                Remarked = assignTicket_HelpDesk.Remarked,
                TicketPriority = assignTicket_HelpDesk.TicketPriority,
                IsClosed = assignTicket_HelpDesk.IsClosed,
                IsOnlineTransferred = assignTicket_HelpDesk.IsOnlineTransferred,
                IsSendForApproval = assignTicket_HelpDesk.IsSendForApproval,
                IsSendForClosing = assignTicket_HelpDesk.IsSendForClosing,
                IsSolved = assignTicket_HelpDesk.IsSolved,
                IsVoid = assignTicket_HelpDesk.IsVoid,
                OnlineApprovedBy = assignTicket_HelpDesk.OnlineApprovedBy,
                OnlineApprovedTime = assignTicket_HelpDesk.OnlineApprovedTime,
                OnlineTransferredTime = assignTicket_HelpDesk.OnlineTransferredTime,
                OrgBranchName = assignTicket_HelpDesk.OrgBranchName,
                Remark = assignTicket_HelpDesk.Remark,
                SendForApprovalBy = assignTicket_HelpDesk.SendForApprovalBy,
                SendForApprovalTime = assignTicket_HelpDesk.SendForApprovalTime,
                VoidBy = assignTicket_HelpDesk.VoidBy,
                VoidTime = assignTicket_HelpDesk.VoidTime,
                IsOnlineApproved = assignTicket_HelpDesk.IsOnlineApproved,
                IsOnlineTransferredBy = assignTicket_HelpDesk.IsOnlineTransferredBy,
                CreatedBy = assignTicket_HelpDesk.CreatedBy,
                DateCreated = assignTicket_HelpDesk.DateCreated,
                ModifiedBy = assignTicket_HelpDesk.ModifiedBy,
                LastModified = assignTicket_HelpDesk.LastModified

            };
            return entity;
        }

    }

}