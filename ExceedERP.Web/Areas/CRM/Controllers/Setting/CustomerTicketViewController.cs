using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles( CRMRoles.MarketingUser)] 
    public class CustomerTicketViewController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CustomerTicketViews_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<CustomerTicket> customertickets = db.CustomerTickets;
            List<CustomerTicketView> customerTiketViews = new List<CustomerTicketView>();
            CustomerTicketView ctv;
            foreach (CustomerTicket customerTicket in customertickets)
            {
                ctv = new CustomerTicketView();
                ctv.CreatedOn = customerTicket.CreatedOn;
                ctv.CustomerID = customerTicket.CustomerID;
                ctv.CustomerTicketID = customerTicket.CustomerTicketID;
                ctv.Description = customerTicket.Description;
                ctv.DueDate = customerTicket.DueDate;
                ctv.EscalationLevel = customerTicket.EscalationLevel;
                ctv.Priority = customerTicket.Priority;
                ctv.Remark = customerTicket.Remark;
                ctv.Source = customerTicket.Source;
                ctv.TicketGroup = customerTicket.TicketGroup;

                customerTiketViews.Add(ctv);
            }
            DataSourceResult result = customerTiketViews.AsQueryable().ToDataSourceResult(request, customerTicketView => new
            {
                CustomerTicketID = customerTicketView.CustomerTicketID,
                CustomerID = customerTicketView.CustomerID,
                Description = customerTicketView.Description,
                Priority = customerTicketView.Priority,
                TicketGroup = customerTicketView.TicketGroup,
                EscalationLevel = customerTicketView.EscalationLevel,
                Source = customerTicketView.Source,
                CreatedOn = customerTicketView.CreatedOn,
                DueDate = customerTicketView.DueDate,
                Remark = customerTicketView.Remark
            });

            return Json(result);
        }

        public ActionResult CustomerTicketViews_TakeAction([DataSourceRequest] DataSourceRequest request, string customerTicketID)
        {
            
            return Json(CustomerTicketViews_Read(request));
        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }
    
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
