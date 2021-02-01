using ExceedERP.Core.Domain.CRM;
using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.CRM.Models;
using ExceedERP.Helpers.Common;  
namespace ExceedERP.Web.Areas.CRM.Controllers
{
           [AuthorizeRoles( CRMRoles.CRMAdmin)] 
    public class CustomerServiceController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        //
        // GET: /CRM/CustomerService/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult CustomerServices_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<CustomerService> CustomerServices = db.CustomerServices;
            DataSourceResult result = CustomerServices.ToDataSourceResult(request, CustomerService => new
            {
                CustomerServiceID = CustomerService.CustomerServiceId,
                Description = CustomerService.Description,
                Type = CustomerService.Type,
                Remark = CustomerService.Remark,
                CreatedBy = CustomerService.CreatedBy,
                DateCreated = CustomerService.DateCreated,
                LastModified = CustomerService.LastModified,
                ModifiedBy = CustomerService.ModifiedBy,

            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CustomerServices_Create([DataSourceRequest]DataSourceRequest request, CustomerService CustomerService)
        {
            List<CustomerService> CustomerServices = db.CustomerServices.Where(p => p.Description == CustomerService.Description).ToList();
            if (CustomerServices.Any())
            {
                ModelState.AddModelError("DuplicateEntrie", "Customer Service Already exist");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var entity = new CustomerService
                    {
                        Description = CustomerService.Description,
                        Type = CustomerService.Type,
                        Remark = CustomerService.Remark,
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name

                    };

                    db.CustomerServices.Add(entity);
                    db.SaveChanges();
                    CustomerService.CustomerServiceId = entity.CustomerServiceId;
                }

            }
            return Json(new[] { CustomerService }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CustomerServices_Update([DataSourceRequest]DataSourceRequest request, CustomerService CustomerService)
        {
            List<CustomerService> CustomerServices = db.CustomerServices.Where(p => p.Description == CustomerService.Description && p.CustomerServiceId != CustomerService.CustomerServiceId).ToList();
            if (CustomerServices.Any())
            {
                ModelState.AddModelError("DuplicateEntire", "Customer Service Already exist");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var entity = new CustomerService
                    {
                        CustomerServiceId = CustomerService.CustomerServiceId,
                        Description = CustomerService.Description,
                        Type = CustomerService.Type,
                        Remark = CustomerService.Remark,
                        LastModified = DateTime.Now,
                        ModifiedBy = User.Identity.Name
                    };

                    db.CustomerServices.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                }
            }
            return Json(new[] { CustomerService }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CustomerServices_Destroy([DataSourceRequest]DataSourceRequest request, CustomerService CustomerService)
        {
            if (ModelState.IsValid)
            {
                var entity = new CustomerService
                {
                    CustomerServiceId = CustomerService.CustomerServiceId,
                    Description = CustomerService.Description,
                    Type = CustomerService.Type,
                    Remark = CustomerService.Remark,

                };

                db.CustomerServices.Attach(entity);
                db.CustomerServices.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Customer Service number " + CustomerService.CustomerServiceId);
		
            }

            return Json(new[] { CustomerService }.ToDataSourceResult(request, ModelState));
        }


        public ActionResult CustomerServiceDDBind()
        {

            var customers = db.CustomerServices.Select(s => new BindingModel
            {
                Name = s.Description,
                Id = s.CustomerServiceId.ToString()
            });

            return Json(customers, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
