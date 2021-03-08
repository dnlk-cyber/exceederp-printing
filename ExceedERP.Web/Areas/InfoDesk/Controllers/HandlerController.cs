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
    public class HandlerController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private ApplicationDbContext adb = new ApplicationDbContext();
        // GET: InfoDesk/Handler
        public ActionResult Index()
        {
            ViewData["AssignTo"] = db.Person_Employee.ToList();
            ViewData["Employees"] = db.Person_Employee.ToList();
            ViewData["Materials"] = db.MaterialItems.ToList();
            return View();
        }
        public ActionResult CustomerPendingTickets_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<CustomerTicket_HelpDesk> customertickets =new List<CustomerTicket_HelpDesk>(); //db.CustomerTickets.Where(x => x.Status != FinalizeType.CLOSED);

            var userName = User.Identity.Name;

            var user = adb.Users.FirstOrDefault(x=>x.UserName == userName);
            if (user != null)
            {
                var empId = int.Parse(user.EmployeeId);
                var assinedToMe = db.AssignTicket_HelpDesks.Where(x=>x.AssinedTo == empId).ToList()?.Select(x=>x.CustomerTicketId);

                customertickets = db.CustomerTicket_HelpDesks.Where(x=>assinedToMe.Contains(x.CustomerTicket_HelpDeskId) && x.Status != InfoDeskFinalizeType.WAITING).ToList();
            }            
          
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
                Rate = customerTicket.Rate,
                EmployeeId = customerTicket.EmployeeId,
                

            });
            return Json(result);


        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Handler_Update([DataSourceRequest]DataSourceRequest request, AssignTicket_HelpDesk assignTicket_HelpDesk)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {

                var entity = new AssignTicket_HelpDesk
                {
                    AssignTicket_HelpDeskId = assignTicket_HelpDesk.AssignTicket_HelpDeskId,
                    CustomerTicketId = assignTicket_HelpDesk.CustomerTicketId,
                    Age = assignTicket_HelpDesk.Age,
                    AssignedDate = assignTicket_HelpDesk.AssignedDate,
                    AssinedTo = assignTicket_HelpDesk.AssinedTo,
                    ClosedDate = assignTicket_HelpDesk.ClosedDate,
                    IsAssigned = false,
                    CreatedBy = User.Identity.Name,
                    DateCreated = assignTicket_HelpDesk.DateCreated,
                    Role = assignTicket_HelpDesk.Role,
                    Solution = assignTicket_HelpDesk.Solution,
                    MaterialItemsId = assignTicket_HelpDesk.MaterialItemsId,
                    Remarked = assignTicket_HelpDesk.Remarked
                };


                db.AssignTicket_HelpDesks.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { assignTicket_HelpDesk }.ToDataSourceResult(request));
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
                var tickets = db.AssignTicket_HelpDesks.Where(x=>x.CustomerTicketId == ticket.CustomerTicketId && x.AssignTicket_HelpDeskId != assignId && !x.IsSendForClosing).ToList();
                if(tickets.Any())
                {
                    ticket.IsSendForClosing = true;

                    db.Entry(ticket).State = EntityState.Modified;
                    db.SaveChanges();

                    return "true";
                }
                else
                {
                    if (!ticket.IsSendForClosing)
                    {
                        var customer = db.CustomerTicket_HelpDesks.FirstOrDefault(x=>x.CustomerTicket_HelpDeskId == ticket.CustomerTicketId);
                        if(customer != null)
                        {
                            customer.Status = InfoDeskFinalizeType.WAITING_FOR_CONFIRMATION;

                            db.Entry(customer).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        ticket.IsSendForClosing = true;

                        db.Entry(ticket).State = EntityState.Modified;
                        db.SaveChanges();

                        return "true";
                    }
                    else
                    {
                        return "false";
                    }
                }
                
            }
            return "faild";
        }
        public string CheckStatus(int masterId)
        {
            var customer = db.CustomerTicket_HelpDesks.FirstOrDefault(x=>x.CustomerTicket_HelpDeskId == masterId);
            if(customer != null)
            {
                if(customer.Status == InfoDeskFinalizeType.RESOLVED || customer.Status == InfoDeskFinalizeType.CLOSED)
                {
                    return "true";
                }                
            }
            return "false";
        }
    }
}