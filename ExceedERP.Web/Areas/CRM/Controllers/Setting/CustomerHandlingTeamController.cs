using System;
using ExceedERP.Web.Helpers;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles( CRMRoles.CRMAdmin)] 
    public class CustomerHandlingTeamController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CustomerHandlingTeams_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<CustomerHandlingTeam> customerhandlingteams = db.CustomerHandlingTeams;
            DataSourceResult result = customerhandlingteams.ToDataSourceResult(request, customerHandlingTeam => new {
                CustomerHandlingTeamID = customerHandlingTeam.CustomerHandlingTeamID,
                Name = customerHandlingTeam.Name,
                Category = customerHandlingTeam.Category,
                IsActive = customerHandlingTeam.IsActive,
                ParentID = customerHandlingTeam.ParentID,
                Remark = customerHandlingTeam.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CustomerHandlingTeams_Create([DataSourceRequest]DataSourceRequest request, CustomerHandlingTeam customerHandlingTeam)
        {
            if (ModelState.IsValid)
            {
                var entity = new CustomerHandlingTeam
                {
                    Name = customerHandlingTeam.Name,
                    Category = customerHandlingTeam.Category,
                    IsActive = customerHandlingTeam.IsActive,
                    ParentID = customerHandlingTeam.ParentID,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    Remark = customerHandlingTeam.Remark
                };

                db.CustomerHandlingTeams.Add(entity);
                db.SaveChanges();
                customerHandlingTeam.CustomerHandlingTeamID = entity.CustomerHandlingTeamID;
            }

            return Json(new[] { customerHandlingTeam }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CustomerHandlingTeams_Update([DataSourceRequest]DataSourceRequest request, CustomerHandlingTeam customerHandlingTeam)
        {
            if (ModelState.IsValid)
            {
                var entity = new CustomerHandlingTeam
                {
                    CustomerHandlingTeamID = customerHandlingTeam.CustomerHandlingTeamID,
                    Name = customerHandlingTeam.Name,
                    Category = customerHandlingTeam.Category,
                    IsActive = customerHandlingTeam.IsActive,
                    ParentID = customerHandlingTeam.ParentID,
                    Remark = customerHandlingTeam.Remark
                };

                db.CustomerHandlingTeams.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { customerHandlingTeam }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CustomerHandlingTeams_Destroy([DataSourceRequest]DataSourceRequest request, CustomerHandlingTeam customerHandlingTeam)
        {
            if (ModelState.IsValid)
            {
                var entity = new CustomerHandlingTeam
                {
                    CustomerHandlingTeamID = customerHandlingTeam.CustomerHandlingTeamID,
                    Name = customerHandlingTeam.Name,
                    Category = customerHandlingTeam.Category,
                    IsActive = customerHandlingTeam.IsActive,
                    ParentID = customerHandlingTeam.ParentID,
                    Remark = customerHandlingTeam.Remark
                };

                db.CustomerHandlingTeams.Attach(entity);
                db.CustomerHandlingTeams.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { customerHandlingTeam }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
