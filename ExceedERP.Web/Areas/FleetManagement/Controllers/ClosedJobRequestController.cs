using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Hubs;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [AuthorizeRoles(MaintenanceRoles.MaintenanceRequestor, MaintenanceRoles.MaintenanceRequestApprover, MaintenanceRoles.MaintenanceServiceApprover)]
    public class ClosedJobRequestController : Controller
    {
        private readonly IFleetService _FleetService;
        private ExceedDbContext db = new ExceedDbContext();
        public ClosedJobRequestController()
        {
            _FleetService = new FleetService();
        }
        // GET: FleetManagement/ClosedJobRequest
        public ActionResult Index()
        {
            ViewData["Customer"] = db.OrganizationCustomers.ToList();
            ViewData["MaintenanceCenter"] = db.MaintenanceCenter.Select(s => new
            {
                Id = s.MaintenanceCenterID,
                Name = s.Name
            }).ToList();

            PopulateViewData();
            return View();
        }
        
        private void PopulateViewData()
        {
            var employee = db.Person_Employee;
            var selectedEmployee = employee.Select(
                i => new
                {
                    Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                    Code = i.EmployeeId
                }).AsQueryable();
            ViewData["personEmployees"] = selectedEmployee;
            var branches = db.Branch;
            var structure = db.MainCompanyStructures;


            ViewData["Structure"] = structure;

            ViewData["Branches"] = branches;

            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                var businessUnits = db.BusinessUnits.ToList();
                ViewData["BusinessUnits"] = businessUnits;
            }

            var items = db.ApSetupItems.ToList();
            ViewData["Items"] = items;
        }
        public ActionResult ClosedJobRequest_Read([DataSourceRequest] DataSourceRequest request, string JobID)
        {

            var JobRequest = db.JobRequest.Where(x => x.JobOrderStatus == JobOrderStatus.Delivered).ToList();
          
           
            return Json(JobRequest.ToDataSourceResult(request));
        }

    }
}
    






