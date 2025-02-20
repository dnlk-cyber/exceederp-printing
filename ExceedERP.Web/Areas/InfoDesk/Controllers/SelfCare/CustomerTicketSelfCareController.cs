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
using ExceedERP.Web.Areas.InfoDesk.Models;
using ExceedERP.Web.Helpers;
using System.Web;
using System.IO;
using ExceedERP.Web.Controllers;

namespace ExceedERP.Web.Areas.InfoDesk.Controllers
{
    [Authorize]
    public class CustomerTicketSelfCareController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        ApplicationDbContext udb = new ApplicationDbContext();
        public ActionResult Index()
        {
            var employees = db.Person_Employee.Select(x => new { x.EmployeeId, AssinedTo = x.EmployeeId, AssignedTo = x.EmployeeId, FullNameaEnglish = x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish, FullNameAmharic = x.FirstName + " " + x.MiddleName + " " + x.LastName }).ToList();

            ViewData["Materials"] = db.MaterialItems.ToList();
            ViewData["bUnit"] = db.BusinessUnits.Select(x=>new { BusinessUnitId=x.BusinessUnitId, x.Name}).ToList();
            ViewData["orgStructure"] = db.MainCompanyStructures.ToList();
            ViewData["branchs"] = db.Branch.ToList();
            ViewData["AssignTo"] = employees;
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
            ViewData["Employees"] = employees;
            return View();
        }
        //public ActionResult TicketStatus()
        //{
        //    ViewData["TicketTypes"] = db.TicketType_HelpDesks.ToList();
        //    ViewData["TicketDefActivity"] = db.TicketActivityDefinition_HelpDesks.ToList();
        //    return View();
        //}
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

        public ActionResult ResolvedTicket()
        {
            return View();
        }
        public ActionResult AwaitingTicket()
        {
            return View();
        }      
        public double GetAge(DateTime? CreatedDate, DateTime? ClosedDate, bool status)
        {
            double age = 0;
            if (status && CreatedDate.HasValue && ClosedDate.HasValue)
            {
                age = ClosedDate.Value.Subtract(CreatedDate.Value).Days;
            }
            
            return age;
        }   
        public ActionResult CustomerTickets_Read([DataSourceRequest]DataSourceRequest request)
        {
            
            var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var masterIds = db.AssignTicket_HelpDesks.Where(x => x.AssinedTo == employeeId).Select(x => x.CustomerTicketId);
            var customertickets = db.CustomerTicket_HelpDesks
                                        .Where(x => x.EmployeeId == employeeId
                                            ||x.AssignedTo ==employeeId
                                            ||masterIds.Contains(x.CustomerTicket_HelpDeskId)).ToList();
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
                AgeOfTicket = GetAge(customerTicket.DateCreated, customerTicket.ClosedDate,customerTicket.IsClosed).ToString(),

            });

            return Json(result);
        }
     
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CustomerTickets_Create([DataSourceRequest]DataSourceRequest request, CustomerTicket_HelpDesk customerTicket_HelpDesk)
        {
            var userName = User.Identity.Name;
            var user = udb.Users.FirstOrDefault(x => x.UserName == userName);
            var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            customerTicket_HelpDesk.EmployeeId = employeeId;
            var empPlace = db.Placements.FirstOrDefault(x => x.isCurrent
                                        && x.EmployeeId == customerTicket_HelpDesk.EmployeeId);  


            if(empPlace != null)
            {
                customerTicket_HelpDesk.BranchId = empPlace.BranchId;
                customerTicket_HelpDesk.BusinessUnitId = empPlace.BusinessUnitId;
                customerTicket_HelpDesk.OrgStructureId = empPlace.OrgStructureId;
            }
            customerTicket_HelpDesk.EmployeeId = employeeId;
            customerTicket_HelpDesk.DateCreated = DateTime.Now;
            customerTicket_HelpDesk.CreatedBy = User.Identity.Name;
            customerTicket_HelpDesk.ClosedDate =null;
            customerTicket_HelpDesk.IsAssigned =false;
            var ticketType = db.TicketType_HelpDesks.Find(customerTicket_HelpDesk.TicketType_HelpDeskId);
            if(ticketType !=null && ticketType.DueDays > 0)
            {
                customerTicket_HelpDesk.DueDate = DateTime.Now.AddDays((double)ticketType.DueDays);
                customerTicket_HelpDesk.ClosedDate = DateTime.Now.AddDays((double)ticketType.DueDays);
                customerTicket_HelpDesk.OverDue = (decimal)(customerTicket_HelpDesk.ClosedDate.Value - customerTicket_HelpDesk.DueDate.Value).TotalDays;

            }
            if (ModelState.IsValid)
            {
                
                var entity = GetMappedObject(customerTicket_HelpDesk);
                db.CustomerTicket_HelpDesks.Add(entity);
                db.SaveChanges();
                customerTicket_HelpDesk.CustomerTicket_HelpDeskId = entity.CustomerTicket_HelpDeskId;
                var groupMembers = db.SupportGroupDetail_HelpDesks.Where(x => x.SupportGroupId == entity.SupportGroup_HelpDeskId).ToList();
                foreach (var groupMember in groupMembers)
                {
                    var entityAssign = new AssignTicket_HelpDesk
                    {
                        Age = null,
                        AssignedDate = DateTime.Now,
                        AssinedTo = groupMember.EmployeeId,
                        ClosedDate = DateTime.Now,
                        IsAssigned = true,
                        CustomerTicketId = entity.CustomerTicket_HelpDeskId,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now,
                        Role = "",
                        RootCause = "",
                        Solution = "",
                        MaterialItemsId = 0,
                        Remarked = "",
                        TicketPriority = TicketPriority.High
                    };
                    db.AssignTicket_HelpDesks.Add(entityAssign);


                }
                db.SaveChanges();
            }

            return Json(new[] { customerTicket_HelpDesk }.ToDataSourceResult(request));
        }
      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CustomerTickets_Update([DataSourceRequest] DataSourceRequest request, CustomerTicket_HelpDesk customerTicket_HelpDesk)
        {
            var ticket = db.CustomerTicket_HelpDesks.Find(customerTicket_HelpDesk.CustomerTicket_HelpDeskId);
            if (ticket == null)
            {
                return Json("Ticket not found", JsonRequestBehavior.AllowGet);
            }

            var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var empPlace = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == employeeId);

            if (empPlace != null)
            {
                ticket.BranchId = empPlace.BranchId;
                ticket.BusinessUnitId = empPlace.BusinessUnitId;
                ticket.OrgStructureId = empPlace.OrgStructureId;
            }

            ticket.EmployeeId = employeeId;
            ticket.SupportGroup_HelpDeskId = customerTicket_HelpDesk.SupportGroup_HelpDeskId;
            ticket.Description = customerTicket_HelpDesk.Description;
            ticket.TicketType_HelpDeskId = customerTicket_HelpDesk.TicketType_HelpDeskId;
            ticket.LastModified = DateTime.Now;
            ticket.ModifiedBy = User.Identity.Name;
            var ticketType = db.TicketType_HelpDesks.Find(customerTicket_HelpDesk.TicketType_HelpDeskId);
            if (ticketType != null && ticketType.DueDays > 0)
            {
                ticket.DueDate = DateTime.Now.AddDays((double)ticketType.DueDays);
                ticket.ClosedDate = DateTime.Now.AddDays((double)ticketType.DueDays);
                ticket.OverDue = (decimal)(ticket.ClosedDate.Value- ticket.DueDate.Value).TotalDays;

            }
            db.Entry(ticket).State = EntityState.Modified;
            db.SaveChanges();
            //if (ModelState.IsValid)
            //{
            //    //try
            //    //{
            //    //    var entity = GetMappedObject(customerTicket_HelpDesk);
            //    //    db.CustomerTicket_HelpDesks.Attach(entity);
            //    //    db.Entry(entity).State = EntityState.Modified;
            //    //    db.SaveChanges();
            //    ////}
            //    //catch (Exception ex)
            //    //{
            //    //    // Handle or log the exception as needed
            //    //    return Json($"Error: {ex.Message}", JsonRequestBehavior.AllowGet);
            //    //}
            //}

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

        private CustomerTicket_HelpDesk GetMappedObject(CustomerTicket_HelpDesk customerTicket_HelpDesk)
        {
            var entity = new CustomerTicket_HelpDesk
            {
                CustomerTicket_HelpDeskId = customerTicket_HelpDesk.CustomerTicket_HelpDeskId,
                IsClosed = customerTicket_HelpDesk.IsClosed,
                IsConfirmedByTheRequester = customerTicket_HelpDesk.IsConfirmedByTheRequester,
                IsSendForClosing = customerTicket_HelpDesk.IsSendForClosing,
                TicketStatus = customerTicket_HelpDesk.TicketStatus,
                MaterialItemsId = customerTicket_HelpDesk.MaterialItemsId,
                Quantity = customerTicket_HelpDesk.Quantity,
                Role = customerTicket_HelpDesk.Role,
                Rate = customerTicket_HelpDesk.Rate,
                LastModified = customerTicket_HelpDesk.LastModified,
                ModifiedBy = customerTicket_HelpDesk.ModifiedBy,
                PriorityColor = customerTicket_HelpDesk.PriorityColor,
                BranchId = customerTicket_HelpDesk.BranchId,
                BusinessUnitId = customerTicket_HelpDesk.BusinessUnitId,
                OrgStructureId = customerTicket_HelpDesk.OrgStructureId,
                AssignedDate = customerTicket_HelpDesk.AssignedDate,
                AssignedTo = customerTicket_HelpDesk.AssignedTo,
                AgeOfTicket = customerTicket_HelpDesk.AgeOfTicket,
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
                IsAssigned = false

            };
            return entity;
        }
        public ActionResult CustomerAwaitingTickets_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<CustomerTicket_HelpDesk> customertickets = db.CustomerTicket_HelpDesks.Where(x=>x.TicketStatus==global::TicketStatus.Waiting);
            DataSourceResult result = customertickets.ToDataSourceResult(request, customerTicket => new
            {
                CustomerTicketID = customerTicket.CustomerTicket_HelpDeskId,
                Description = customerTicket.Description,
                TicketPriority = customerTicket.TicketPriority,
                SupportGroup_HelpDeskId = customerTicket.SupportGroup_HelpDeskId,
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
                TicketPriority = customerTicket.TicketPriority,
                SupportGroup_HelpDeskId = customerTicket.SupportGroup_HelpDeskId,
                TicketType_HelpDeskId = customerTicket.TicketType_HelpDeskId,
                DueDate = customerTicket.DueDate,
                CreatedBy = customerTicket.CreatedBy,
                DateCreated = customerTicket.DateCreated,
                LastModified = customerTicket.LastModified,
                ModifiedBy = customerTicket.ModifiedBy,
                OrgBranchName = customerTicket.OrgBranchName,
                TicketStatus = customerTicket.TicketStatus,
                PriorityColor = customerTicket.PriorityColor,
                Remark = customerTicket.Remark,
                AgeOfTicket = customerTicket.AgeOfTicket,
                IsClosed = customerTicket.IsClosed,
                Rate = customerTicket.Rate,
                EmployeeId = customerTicket.EmployeeId,
                AssignedDate=customerTicket.AssignedDate,
                ClosedDate = customerTicket.ClosedDate
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
                customerTicket.IsClosed = true;
                customerTicket.Remark = model.Remark;
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
            return RedirectToAction("Index");
        }
        public JsonResult SendForApproval(int id)
        {
            var ticket = db.CustomerTicket_HelpDesks.Find(id);
            if(ticket != null)
            {
                ticket.IsSendForApproval = true;
                ticket.IsAssigned = true;
                ticket.TicketStatus = TicketStatus.Assigned;
                ticket.SendApprovalTime = DateTime.Now;
                ticket.SendApprovalBy = User.Identity.Name;
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new
                {
                    Message = "You have successfully sent for approval.",
                    Status = "OK"
                }, JsonRequestBehavior.AllowGet);

            }
            return Json(new
            {
                Message = "Record Not Found.",
                Status = "NOT-OK"
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UploadSingleFiles(int masterId, HttpPostedFileBase file, string Description)
        {
            if (file != null && file.ContentLength > 0)
            {
                string path = HttpContext.Server.MapPath("~/Content/InfoDesk/InfoDeskFiles/");
                string shortPath = "/Content/InfoDesk/InfoDeskFiles/";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string newFileName = Path.GetExtension(file.FileName);
                var fileExtension = Path.GetExtension(file.FileName);
                string name = "" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;
                newFileName = name + newFileName;
                string filePath = path + newFileName;

                file.SaveAs(filePath);

                var entity = new CustomerTicketAttachment
                {
                    CustomerTicket_HelpDeskId = masterId,
                    FilePath = filePath,
                    FileName = newFileName,
                    Description = Description,
                    ShortPath = shortPath,
                    FileExtension = fileExtension
                };

                db.CustomerTicketAttachments.Add(entity);
                db.SaveChanges();
            }

            return RedirectToAction("Index", "CustomerTicketSelfCare");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
