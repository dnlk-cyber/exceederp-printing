using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using System.Data.Entity;
using ExceedERP.Helpers.Common;  
using ExceedERP.Web.Areas.CRM.Models;
using ExceedERP.Helpers;

namespace ExceedERP.Web.Areas.CRM.Controllers
{

    [AuthorizeRoles(CRMRoles.CRMAdmin, CRMRoles.ServiceSupportUser)] 
    public class ServiceUsageController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        //
        // GET: /CRM/ServiceUsage/
        public ActionResult Index()
        {
            ViewData["CustomerServices"] = db.CustomerServices;
            ViewData["Tax"] = db.Taxs;
            return View();
        }
        public ActionResult ServiceUsages_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<ServiceLineItem> LineItems = db.ServiceLineItems.ToList();
            List<ServiceUsageApproval> Approvals = db.ServiceUsageApprovals.ToList();
            List<ServiceUsage> ServiceUsages = db.ServiceUsages.ToList();
            foreach( var usage in ServiceUsages)
            {
                var approval =Approvals.Where(x=>x.ServiceUsageId == usage.ServiceUsageId).LastOrDefault();
                  if(approval != null)
                  {
                      usage.Status = approval.Status;
                  }
                else
                  {
                      usage.Status = CRMConstants.Pending;
                  }
                  usage.GrandTotal = LineItems.Where(x => x.ServiceUsageId == usage.ServiceUsageId).Sum(x => x.TotalAmount);

  
            }

            return Json(ServiceUsages.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceUsages_Create([DataSourceRequest]DataSourceRequest request, ServiceUsage ServiceUsage)
        {
         
                if (ModelState.IsValid)
                {
                    var entity = new ServiceUsage
                    {
                        Customer = ServiceUsage.Customer,
                        Date=ServiceUsage.Date,
                        ReceiveMethod = ServiceUsage.ReceiveMethod,
                        Remark = ServiceUsage.Remark,
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        
                        

                    };

                    db.ServiceUsages.Add(entity);
                    db.SaveChanges();
                    ServiceUsage.ServiceUsageId = entity.ServiceUsageId;
                }

            
            return Json(new[] { ServiceUsage }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceUsages_Update([DataSourceRequest]DataSourceRequest request, ServiceUsage ServiceUsage)
        {
      
                if (ModelState.IsValid)
                {
                    var entity = new ServiceUsage
                    {
                        ServiceUsageId = ServiceUsage.ServiceUsageId,
                        Customer = ServiceUsage.Customer,
                        Date = ServiceUsage.Date,
                        ReceiveMethod = ServiceUsage.ReceiveMethod,
                        Remark = ServiceUsage.Remark,
                        LastModified = DateTime.Now,
                        ModifiedBy = User.Identity.Name
                    };

                    db.ServiceUsages.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                }
            
            return Json(new[] { ServiceUsage }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceUsages_Destroy([DataSourceRequest]DataSourceRequest request, ServiceUsage ServiceUsage)
        {
            if (ModelState.IsValid)
            {
                var entity = new ServiceUsage
                {
                    ServiceUsageId = ServiceUsage.ServiceUsageId,
            
                    Remark = ServiceUsage.Remark,

                };

                db.ServiceUsages.Attach(entity);
                db.ServiceUsages.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Service Usage number " + ServiceUsage.ServiceUsageId);
     
            }

            return Json(new[] { ServiceUsage }.ToDataSourceResult(request, ModelState));
        }




        public string GetStatus(int Id)
        {

            ServiceUsage Usage = db.ServiceUsages.Where(j => j.ServiceUsageId == Id).FirstOrDefault();
            if (Usage.IsOnlineApproved == true)
            {
                return CRMConstants.Approved;
            }
           
                return CRMConstants.Pending;
           

        }

        public ActionResult ApprovalDDBind()
            {
            var approvals = new List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject>();
           var approval =new  ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject()
            {
                Name = CRMConstants.Approved,
                Id = 1,
            };
                   approvals.Add(approval);

           var approval2 = new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject()
           {
               Name = CRMConstants.Pending,
               Id = 2,
           };
           approvals.Add(approval2);
           var approval3 = new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject()
           {
               Name = CRMConstants.Rejected,
               Id = 3,
           };
           approvals.Add(approval3);
           return Json(approvals, JsonRequestBehavior.AllowGet);
        }

        public string GetApprovalStatus(int Id)
        {

            ServiceUsageApproval usage = db.ServiceUsageApprovals.Where(j => j.ServiceUsageId == Id).LastOrDefault();
            if (usage != null)
            {

                return usage.Status;
            }
            else
            {
                return CRMConstants.Pending;
            }

        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
