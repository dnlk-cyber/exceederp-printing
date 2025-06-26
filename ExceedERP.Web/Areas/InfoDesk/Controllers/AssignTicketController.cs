using ExceedERP.DataAccess.Context;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Models;
using ExceedERP.Core.Domain.InfoDesk.Support;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.InfoDesk;

namespace ExceedERP.Web.Areas.InfoDesk.Controllers
{
    [Authorize]
    public class AssignTicketController : Controller
    {

        private ExceedDbContext db = new ExceedDbContext();
        ApplicationDbContext udb = new ApplicationDbContext();
        public ActionResult Index()
        {
            var employees = db.Person_Employee.Select(x => new { x.EmployeeId, AssinedTo = x.EmployeeId, AssignedTo = x.EmployeeId, FullNameaEnglish = x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish, FullNameAmharic = x.FirstName + " " + x.MiddleName + " " + x.LastName }).ToList();
            ViewData["AssignTo"] = employees;
            ViewData["Employees"] = employees;
            ViewData["Group"] = db.SupportGroup_HelpDesks.ToList();
            ViewData["Materials"] = db.MaterialItems.ToList();
            ViewData["TicketTypes"] = db.TicketType_HelpDesks.ToList();
            ViewData["TicketDefActivity"] = db.TicketActivityDefinition_HelpDesks.ToList();
            var branches = db.Branch.Select(x => new BranchVM { BranchId = x.BranchId, Name = x.Name.Replace("\n", "").Replace("\r", "").Replace("#", "").Replace("+", " ") });
            ViewData["branches"] = branches;
            var businessUnits = db.BusinessUnits.Select(x => new BussinesUnitVM { BusinessUnitId = x.BusinessUnitId, Name = x.Name.Replace("\n", "").Replace("\r", "").Replace("#", "").Replace("+", " ") });
            ViewData["businessUnits"] = businessUnits;
            var workunits = db.MainCompanyStructures.Select(x => new OrgStructureVM { OrgStructureId = x.OrgStructureId, Name = x.Name.Replace("\n", "").Replace("\r", "").Replace("#", "").Replace("+", " ") }).ToList();
            ViewData["workunits"] = workunits;
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

                age= day.Value.Days.ToString() + " Day " + day.Value.Hours.ToString() + " Hours";

            }
            
            return age;
        }
        public ActionResult AssignedHandler_Read([DataSourceRequest]DataSourceRequest request, int Id)
        {
            var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var tickets = db.CustomerTicket_HelpDesks.Where(x => x.CustomerTicket_HelpDeskId == Id
                                                                && x.AssignedTo==employeeId).ToList();
           


            DataSourceResult result = tickets.ToDataSourceResult(request, customerTicket => new
            {
                CustomerTicket_HelpDeskId = customerTicket.CustomerTicket_HelpDeskId,
                EmployeeId = customerTicket.EmployeeId,
                BranchId = customerTicket.BranchId,
                BusinessUnitId = customerTicket.BusinessUnitId,
                OrgStructureId = customerTicket.OrgStructureId,
                RequesterIPAddress = customerTicket.RequesterIPAddress,
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
                Handler = customerTicket.Handler,
                FixedAssetCategoryId = customerTicket.FixedAssetCategoryId,
                FixedAssetSubCategoryId = customerTicket.FixedAssetSubCategoryId,
                AssetRegistrationId = customerTicket.AssetRegistrationId,
                ItemName = customerTicket.ItemName,
                TagNo = customerTicket.TagNo,
                SendApprovalBy = customerTicket.SendApprovalBy,
                SendApprovalTime = customerTicket.SendApprovalTime,
                PendingReason = customerTicket.PendingReason,
                OrgBranchName = customerTicket.OrgBranchName,
                TicketType_HelpDeskId = customerTicket.TicketType_HelpDeskId,
                TicketStatus = customerTicket.TicketStatus,
                CreatedBy = customerTicket.CreatedBy,
                DateCreated = customerTicket.DateCreated,
                LastModified = customerTicket.LastModified,
                ModifiedBy = customerTicket.ModifiedBy,
                PriorityColor = customerTicket.PriorityColor,
                Remark = customerTicket.Remark,

            });

            return Json(result);
        }
        public ActionResult AssignedHandlerForSelfCare_Read([DataSourceRequest]DataSourceRequest request, int Id)
        {
            var tickets = db.CustomerTicket_HelpDesks.Where(x => x.CustomerTicket_HelpDeskId == Id).ToList();
            var userName = User.Identity.Name;

            DataSourceResult result = tickets.ToDataSourceResult(request, customerTicket => new
            {
                CustomerTicket_HelpDeskId = customerTicket.CustomerTicket_HelpDeskId,
                EmployeeId = customerTicket.EmployeeId,
                BranchId = customerTicket.BranchId,
                BusinessUnitId = customerTicket.BusinessUnitId,
                OrgStructureId = customerTicket.OrgStructureId,
                RequesterIPAddress = customerTicket.RequesterIPAddress,
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
                Handler = customerTicket.Handler,
                FixedAssetCategoryId = customerTicket.FixedAssetCategoryId,
                FixedAssetSubCategoryId = customerTicket.FixedAssetSubCategoryId,
                AssetRegistrationId = customerTicket.AssetRegistrationId,
                ItemName = customerTicket.ItemName,
                TagNo = customerTicket.TagNo,
                SendApprovalBy = customerTicket.SendApprovalBy,
                SendApprovalTime = customerTicket.SendApprovalTime,
                PendingReason = customerTicket.PendingReason,
                OrgBranchName = customerTicket.OrgBranchName,
                TicketType_HelpDeskId = customerTicket.TicketType_HelpDeskId,
                TicketStatus = customerTicket.TicketStatus,
                CreatedBy = customerTicket.CreatedBy,
                DateCreated = customerTicket.DateCreated,
                LastModified = customerTicket.LastModified,
                ModifiedBy = customerTicket.ModifiedBy,
                PriorityColor = customerTicket.PriorityColor,
                Remark = customerTicket.Remark,
            });
            return Json(result);
        }
        public ActionResult CustomerTickets_Update([DataSourceRequest]DataSourceRequest request, CustomerTicket_HelpDesk customerTicket_HelpDesk, int Id)
        {
            var ticket = db.CustomerTicket_HelpDesks.FirstOrDefault(x => x.CustomerTicket_HelpDeskId == Id);
            var isAssigned = ticket.IsAssigned;
            if(!isAssigned)
            {
                if (ModelState.IsValid)
                {
                    ticket.AssignedTo = customerTicket_HelpDesk.AssignedTo;
                    ticket.TicketPriority = customerTicket_HelpDesk.TicketPriority;
                    ticket.Role = customerTicket_HelpDesk.Role;
                    ticket.AssignedDate = DateTime.Now;
                    ticket.SolutionGiven = customerTicket_HelpDesk.SolutionGiven;
                    ticket.IsAssigned = true;
                    var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == customerTicket_HelpDesk.EmployeeId && x.isCurrent);
                    if(placement != null)
                    {
                        ticket.BranchId = placement.BranchId;
                        ticket.BusinessUnitId = placement.BusinessUnitId;
                        ticket.OrgStructureId = placement.OrgStructureId;
                    }
                    
                    ticket.TicketStatus = TicketStatus.Assigned;
                    db.Entry(ticket).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return Json(new[] { customerTicket_HelpDesk }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult AssignTickets_Read([DataSourceRequest]DataSourceRequest request, int Id)
        {
            //var Tickets = db.CustomerTicket_HelpDesks
            //    .Where(x => x.CustomerTicket_HelpDeskId == Id)
            //    .Where(x=>x.IsAssigned)
            //    .ToList();
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
                Handler = assignTicket_HelpDesk.Handler,
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
                NotifModels notif = new NotifModels
                {
                    UserId = UserInformationHelper.GetEmployeeUserId(assignTicket.AssinedTo),
                    Title = $"Ticke is Assigned to : {assignTicket.AssinedTo} Name: {assignTicket.PhoneNumber} ",
                    Message = $"Ticke is Assigned: {assignTicket.CustomerTicketId} Id:{assignTicket.CustomerTicketId}",
                    URL = "/InfoDesk/AssignTicket"
                };
                Hubs.Notify.NotifyUser(notif);
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
                var entity = GetMappedObject(assignTicket);
                db.AssignTicket_HelpDesks.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                NotifModels notif = new NotifModels
                {
                    UserId = UserInformationHelper.GetEmployeeUserId(assignTicket.AssinedTo),
                    Title = $"Ticke is Assigned to : {assignTicket.AssinedTo} Name: {assignTicket.PhoneNumber} ",
                    Message = $"Ticke is Assigned: {assignTicket.CustomerTicketId} Id:{assignTicket.CustomerTicketId}",
                    URL = "/InfoDesk/AssignTicket"
                };
                Hubs.Notify.NotifyUser(notif);
            }
            return Json(new[] { assignTicket }.ToDataSourceResult(request));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssignTickets_Destroy([DataSourceRequest]DataSourceRequest request, AssignTicket_HelpDesk assignTicket)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(assignTicket);
                db.AssignTicket_HelpDesks.Attach(entity);
                db.AssignTicket_HelpDesks.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Priority Type number " + assignTicket.AssignTicket_HelpDeskId);
            }
            return Json(new[] { assignTicket }.ToDataSourceResult(request, ModelState));
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
                Handler = assignTicket_HelpDesk.Handler,
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
        public JsonResult Assign(int id)
        {
            var customerTicket = db.CustomerTicket_HelpDesks.Find(id);
            if(customerTicket == null)
            {
                return Json(new { Message = "Record not found.", Status = "Not-Ok" }, JsonRequestBehavior.AllowGet);

            }
            customerTicket.IsAssigned = true;
            customerTicket.TicketStatus = TicketStatus.Assigned;
            var assignedTickets = db.AssignTicket_HelpDesks.Where(x => x.CustomerTicketId == customerTicket.CustomerTicket_HelpDeskId).ToList();
            foreach (var assigned in assignedTickets)
            {
                // var assign = new AssignTicket();
                assigned.IsAssigned = true;
                assigned.ModifiedBy = User.Identity.Name;
                assigned.LastModified = DateTime.Now;
                db.Entry(assigned).State = EntityState.Modified;
                NotifModels notif = new NotifModels
                {
                    UserId = UserInformationHelper.GetEmployeeUserId(assigned.AssinedTo),
                    Title = $"Ticke is Assigned to : {assigned.AssinedTo} Name: {customerTicket.RootCuase} ",
                    Message = $"Ticke is Assigned: {customerTicket.CustomerTicket_HelpDeskId} Id:{customerTicket.CustomerTicket_HelpDeskId}",
                    URL = "/InfoDesk/AssignTicket"
                };
                Hubs.Notify.NotifyUser(notif);
            }
            db.Entry(customerTicket).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { Message = "Assign successfully.", Status = "Ok" }, JsonRequestBehavior.AllowGet);

        }
        public string CheckStatus(int masterId)
        {
            var customerTicket = db.CustomerTicket_HelpDesks.FirstOrDefault(x => x.CustomerTicket_HelpDeskId == masterId);
            if (customerTicket != null)
            {
                if (customerTicket.TicketStatus == TicketStatus.Closed 
                    //|| customerTicket.TicketStatus == TicketStatus.Assigned 
                    //|| customerTicket.TicketStatus == TicketStatus.Resolved
                    )
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
            if (!int.TryParse(childId, out int assignId))
            {
                return "failed"; // Handle invalid input
            }

            var ticket = db.AssignTicket_HelpDesks.Find(assignId);
            if (ticket == null)
            {
                return "failed"; // Ticket not found
            }

            if (ticket.IsSolved)
            {
                return "false"; // Already solved
            }

            ticket.IsSolved = true;
            db.Entry(ticket).State = EntityState.Modified;

            var remainingAssignes = db.AssignTicket_HelpDesks
                                      .Any(x => x.CustomerTicketId == ticket.CustomerTicketId && !x.IsSolved);

            if (!remainingAssignes)
            {
                var customer = db.CustomerTicket_HelpDesks
                                 .FirstOrDefault(x => x.CustomerTicket_HelpDeskId == ticket.CustomerTicketId);
                if (customer != null)
                {
                    customer.TicketStatus = TicketStatus.Resolved;

                    db.Entry(customer).State = EntityState.Modified;
                    var entity = new InfoDeskKnowledgeBase
                    {
                        Problem = customer.RootCuase,
                        Solution = customer.SolutionGiven,
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy =null,
                        LastModified = null

                    };
                    db.InfoDeskKnowledgeBases.Add(entity);
                    db.SaveChanges();
                    NotifModels notif = new NotifModels
                    {
                        UserId = UserInformationHelper.GetEmployeeUserId((int)customer.EmployeeId),
                        Title = $"Ticke is Assigned to : {customer.AssignedTo} Name: {customer.RootCuase} ",
                        Message = $"Ticke is Assigned: {customer.CustomerTicket_HelpDeskId} Id:{customer.CustomerTicket_HelpDeskId}",
                        URL = "/InfoDesk/AssignTicket"
                    };
                    Hubs.Notify.NotifyUser(notif);
                }
            }

           // Save all changes once

            return "true";
        }
        public JsonResult CloseTicket(int mid)
        {
            var ticket = db.CustomerTicket_HelpDesks.Find(mid);
            if(ticket == null)
            {
                return Json(new { Message = "Record not found.", Status = "Not-Ok" }, JsonRequestBehavior.AllowGet);

            }
            if (ticket.IsClosed)
            {
                return Json(new { Message = "already Closed.", Status = "Not-Ok" }, JsonRequestBehavior.AllowGet);

            }
            if (!ticket.IsAssigned)
            {
                return Json(new { Message = "Assign First.", Status = "Not-Ok" }, JsonRequestBehavior.AllowGet);

            }
            if (ticket.TicketStatus != TicketStatus.Resolved)
            {
                return Json(new { Message = "Solve it firs First.", Status = "Not-Ok" }, JsonRequestBehavior.AllowGet);

            }
            ticket.IsClosed = true;
            ticket.TicketStatus = TicketStatus.Closed;
            ticket.ClosedDate = DateTime.Now;

            db.Entry(ticket).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { Message = "Successfully Closed.", Status = "Ok" }, JsonRequestBehavior.AllowGet);
            
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