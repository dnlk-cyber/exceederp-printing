using ExceedERP.Core.Domain.CRM;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

using Kendo.Mvc.Extensions;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Models;


namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.CRMAdmin)] 
    public class SupportGroupController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        ApplicationDbContext udb = new ApplicationDbContext();
        public ActionResult Index()
        {
            var organizationCustomer =
                db.OrganizationCustomers.Select(
                    o =>
                        new
                        {
                            CustomerID = o.OrganizationCustomerID,
                            CustomerType = "Organization",
                            CustomerName = o.TradeName
                        });

            var combinedCustomer = organizationCustomer.ToList();
            ViewData["CustomerList"] = combinedCustomer;
            var sysusers =
              udb.Users.Select(
                  o =>
                      new
                      {
                          CustomerID = o.Id,
                          CustomerName = o.FirstName
                      });

            var users = sysusers.ToList();
            ViewData["SysUsers"] = users;
            return View();
        }

        public ActionResult Groups_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<SupportGroup> Groups = db.SupportGroups;
            DataSourceResult result = Groups.ToDataSourceResult(request);

            return Json(result);
        }

      [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Groups_Create([DataSourceRequest]DataSourceRequest request, SupportGroup model )
        {
            var Group = db.SupportGroups.Where(x => x.Name == model.Name).ToList();
          if(Group.Any())
          {
              ModelState.Clear();
              ModelState.AddModelError("SameName", " Name Already Registered ! ");
              return Json(new[] { model }.ToDataSourceResult(request, ModelState));
          }
          if (ModelState.IsValid)
            {
                var entity = new SupportGroup
                {
                    Description = model.Description,
                    Name = model.Name,
                    Remark = model.Remark,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now 
                    
                };

                db.SupportGroups.Add(entity);
                db.SaveChanges();
                model.SupportGroupId = entity.SupportGroupId;
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
      public ActionResult Groups_Update([DataSourceRequest]DataSourceRequest request, SupportGroup model)
        {
            var Groups = db.SupportGroups.Where(x => x.SupportGroupId == model.SupportGroupId).FirstOrDefault();


            Groups.Description = model.Description;
            Groups.Name = model.Name;
            Groups.Remark = model.Remark;
           Groups.LastModified = DateTime.Now;
            Groups.ModifiedBy = User.Identity.Name;
               

                db.SupportGroups.Attach(Groups);
                db.Entry(Groups).State = EntityState.Modified;
                db.SaveChanges();
           

            return Json(new[] { Groups }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Groups_Destroy([DataSourceRequest]DataSourceRequest request, SupportGroup model)
        {
            var Memeber = db.SupportGroups.Where(x => x.SupportGroupId == model.SupportGroupId).FirstOrDefault();
            db.SupportGroups.Attach(Memeber);
            db.SupportGroups.Remove(Memeber);
            db.SaveChanges();
            UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Support Groups number " + model.SupportGroupId);



            return Json(new[] { model }.ToDataSourceResult(request));

         
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
