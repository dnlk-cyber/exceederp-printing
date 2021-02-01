using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.UI;
using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using ExceedERP.Helpers.Common;  
using ExceedERP.Web.Areas.CRM.Models;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.CRMAdmin, CRMRoles.ServiceSupportUser)] 
    public class ServiceLineItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        //
        // GET: /CRM/ServiceLineItem/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ServiceLineItems_Read([DataSourceRequest]DataSourceRequest request, int Id)
        {
            IQueryable<ServiceLineItem> ServiceLineItems = db.ServiceLineItems.Where(x => x.ServiceUsageId == Id);
            DataSourceResult result = ServiceLineItems.ToDataSourceResult(request, ServiceLineItem => new
            {
                ServiceLineItemID = ServiceLineItem.ServiceLineItemId,
                CustomerServiceId = ServiceLineItem.CustomerServiceId,
                ServiceUsageId = ServiceLineItem.ServiceUsageId,
                UnitPrice = ServiceLineItem.UnitPrice,
                TotalAmount = ServiceLineItem.TotalAmount,
                Tax = ServiceLineItem.Tax,
                Remark = ServiceLineItem.Remark,
                CreatedBy = ServiceLineItem.CreatedBy,
                DateCreated = ServiceLineItem.DateCreated,
                LastModified = ServiceLineItem.LastModified,
                ModifiedBy = ServiceLineItem.ModifiedBy,
                IsOnlineApproved = ServiceLineItem.IsOnlineApproved

            });

            return Json(result);
        }

        public ActionResult ServiceLineItemApproval_Read([DataSourceRequest]DataSourceRequest request, int Id)
        {
            List<ServiceUsageApproval> usageApproval = db.ServiceUsageApprovals.Where(x => x.ServiceUsageId == Id).ToList();


            DataSourceResult result = usageApproval.ToDataSourceResult(request, approval => new ServiceUsageApproval()
            {
                ServiceUsageApprovalId = approval.ServiceUsageApprovalId,
                ServiceUsageId = approval.ServiceUsageId,
                Remark = approval.Remark,
                Status = approval.Status,



            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceLineItems_Create([DataSourceRequest]DataSourceRequest request, ServiceLineItem ServiceLineItem, int Id)
        {

            if (ServiceLineItem.TotalAmount == 0)
            {
                ModelState.AddModelError("InvalidQuantity", " Total Amount Can't Be 0 ! ");

            }
            else if (ServiceLineItem.UnitPrice == 0)
            {
                ModelState.AddModelError("InvalidQuantity", "Unit Price Value Can't Be 0 ! ");

            }
            else
            {
                if (ModelState.IsValid)
                {
                    var entity = new ServiceLineItem
                    {

                        CustomerServiceId = ServiceLineItem.CustomerServiceId,
                        UnitPrice = ServiceLineItem.UnitPrice,
                        TotalAmount = ServiceLineItem.TotalAmount,
                        Tax = ServiceLineItem.Tax,
                        ServiceUsageId = Id,

                        Remark = ServiceLineItem.Remark,
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name

                    };

                    db.ServiceLineItems.Add(entity);
                    db.SaveChanges();
                    ServiceLineItem.ServiceLineItemId = entity.ServiceLineItemId;
                }
            }


            return Json(new[] { ServiceLineItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceLineItems_Update([DataSourceRequest]DataSourceRequest request, ServiceLineItem ServiceLineItem)
        {
            if (ServiceLineItem.TotalAmount == 0)
            {
                ModelState.AddModelError("InvalidQuantity", " Total Amount Can't Be 0 ! ");

            }
            else if (ServiceLineItem.UnitPrice == 0)
            {
                ModelState.AddModelError("InvalidQuantity", "Unit Price Value Can't Be 0 ! ");

            }
            else
            {
                var entity = new ServiceLineItem
                {
                    ServiceLineItemId = ServiceLineItem.ServiceLineItemId,
                    CustomerServiceId = ServiceLineItem.CustomerServiceId,
                    UnitPrice = ServiceLineItem.UnitPrice,
                    TotalAmount = ServiceLineItem.TotalAmount,
                    Tax = ServiceLineItem.Tax,
                    ServiceUsageId = ServiceLineItem.ServiceUsageId,

                    Remark = ServiceLineItem.Remark,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name
                };

                db.ServiceLineItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { ServiceLineItem }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceLineItems_Destroy([DataSourceRequest]DataSourceRequest request, ServiceLineItem ServiceLineItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new ServiceLineItem
                {
                    ServiceLineItemId = ServiceLineItem.ServiceLineItemId,

                    Remark = ServiceLineItem.Remark,

                };

                db.ServiceLineItems.Attach(entity);
                db.ServiceLineItems.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted  Service LineItem number " + ServiceLineItem.ServiceLineItemId);

            }

            return Json(new[] { ServiceLineItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceLineItems_Approve([DataSourceRequest]DataSourceRequest request, ServiceUsageApproval ServiceUsageApproval, int Id)
        {
            if (ServiceUsageApproval.Status == CRMConstants.Approved)
            {
                var entity = db.ServiceUsages.Where(x => x.ServiceUsageId == Id).FirstOrDefault();

                entity.IsOnlineApproved = true;
                entity.OnlineApprovedBy = User.Identity.Name;
                entity.OnlineApprovedTime = DateTime.Now;

                db.ServiceUsages.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            var approval = new ServiceUsageApproval()
            {


                Status = ServiceUsageApproval.Status,
                ServiceUsageId = Id,
                Remark = ServiceUsageApproval.Remark,
                DateCreated = DateTime.Now,
                CreatedBy = User.Identity.Name

            };

            db.ServiceUsageApprovals.Add(approval);
            db.SaveChanges();
            ServiceUsageApproval.ServiceUsageApprovalId = approval.ServiceUsageApprovalId;



            return Json(new[] { approval }.ToDataSourceResult(request));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
