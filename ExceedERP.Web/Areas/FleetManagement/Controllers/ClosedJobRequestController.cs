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
        private readonly CacheData CacheData = new CacheData();
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
            var structure = db.MainCompanyStructures;


            ViewData["Structure"] = structure;

            ViewData["branch"] = CacheData.GetBranchCaches().Select(
             i => new
             {
                 Name = i.Name,
                 BranchId = i.Id
             }).AsQueryable();

            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                var businessUnits = CacheData.GetBusinessUnitCaches().Select(
         i => new
         {
             Name = i.Name,
             Id = i.Id
         }).AsQueryable();
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

        public ActionResult Read([DataSourceRequest] DataSourceRequest request, int parentNum)
        {
            //var JobRequest = db.JobRequest.FirstOrDefault(x => x.JobRequestId == Num);
            var FetchJobOrder = db.JobOrder.Where(x => x.Referance == parentNum && x.RemarkAfterDelivery!=null).ToList();
            DataSourceResult Result = FetchJobOrder.ToDataSourceResult(request, j => new
            {
           RemarkAfterDelivery = j.RemarkAfterDelivery,
           JobOrderID=j.JobOrderID,
           Referance=j.Referance,
           Num=j.Num,
           ModifiedBy=j.ModifiedBy,
          LastModified = j.LastModified
            });
            return Json(Result);
        }
        private void RemoveNonNullablePropertiesFromModelState<T>()
        {
            var type = typeof(T);
            var nonNullableProps = type.GetProperties()
                .Where(p =>
                    p.CanRead &&
                    p.CanWrite &&
                    p.PropertyType.IsValueType &&           // value type
                    Nullable.GetUnderlyingType(p.PropertyType) == null // not Nullable<>
                );

            foreach (var prop in nonNullableProps)
            {
                ModelState.Remove(prop.Name);
            }
        }
        public ActionResult Create([DataSourceRequest] DataSourceRequest request, JobOrder jobOrder, int parentNum)
        {
            // find joborder that belongs to this JobRequest.Num
            RemoveNonNullablePropertiesFromModelState<JobOrder>();
            var fetchJobOrder = db.JobOrder.FirstOrDefault(
                x => x.Referance == parentNum && x.ServiceCenterJobId == null);

            if (fetchJobOrder == null)
            {
                ModelState.AddModelError("", "No matching JobOrder found.");
                return Json(new[] { jobOrder }.ToDataSourceResult(request, ModelState));
            }

            fetchJobOrder.RemarkAfterDelivery = jobOrder.RemarkAfterDelivery;
            //ModelState.Remove("JobOrderID");
           
            if (ModelState.IsValid)
            {
                db.Entry(fetchJobOrder).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { fetchJobOrder }.ToDataSourceResult(request, ModelState));
        }
        //public ActionResult Update([DataSourceRequest] DataSourceRequest request,JobOrder jobOrder,int Num)
        //{
        //    var FetchJobOrder = db.JobOrder.FirstOrDefault(x => x.Referance == Num&&x.ServiceCenterJobId==null);
        //    FetchJobOrder.RemarkAfterDelivery = jobOrder.RemarkAfterDelivery;
        //    if(ModelState.IsValid)
        //    {
        //        db.Entry(FetchJobOrder).State = System.Data.Entity.EntityState.Modified;
        //        db.SaveChanges();
        //    }

        //    return Json(new[] { jobOrder }.ToDataSourceResult(request, ModelState));
        //}
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
    






