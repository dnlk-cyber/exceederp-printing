using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;  
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;

using System.Data.Entity;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class JobCausesController : Controller
    {


        ExceedDbContext db = new ExceedDbContext();
 
   

        public ActionResult JobCauses_Read([DataSourceRequest] DataSourceRequest request , Guid Id)
        {
            var JobCauses = db.JobCauses.Where(x=>x.OrderId == Id).ToList();

            return Json(JobCauses.ToDataSourceResult(request));
        }

        public ActionResult JobCausesOrder_Read([DataSourceRequest] DataSourceRequest request, int Id)
        {
            var Request = db.JobRequest.Where(x => x.Num == Id).FirstOrDefault();
            if (Request != null)
            {
                var JobCauses = db.JobCauses.Where(x => x.OrderId == Request.JobRequestId).ToList();
                return Json(JobCauses.ToDataSourceResult(request));
            }
            return Json("");
         
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobCauses_Create([DataSourceRequest] DataSourceRequest request, JobCauses model , Guid Id)
        {
            ModelState.Remove("JobCausesId");
            ModelState.Remove("OrderId");

            if (model.Description == "" || string.IsNullOrEmpty(model.Description))
            {
                ModelState.Clear();
                ModelState.AddModelError("EmptyValue"," Reason Can't be Empty");
                return Json(new[] { model }.ToDataSourceResult(request, ModelState));
    
            }
            else
            {
                var Level = new JobCauses
                {
                    JobCausesId = Guid.NewGuid(),
                    Name = model.Name,
                    Description = model.Description,
                    OrderId = Id,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name
                };
                db.JobCauses.Add(Level);
                db.SaveChanges();
                model.JobCausesId = Level.JobCausesId;
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobCauses_Update([DataSourceRequest] DataSourceRequest request, JobCauses JobCauses)
        {
            if (JobCauses != null)
            {
                JobCauses.ModifiedBy = User.Identity.Name;
                JobCauses.LastModified = DateTime.Now;

                db.JobCauses.Attach(JobCauses);
                db.Entry(JobCauses).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { JobCauses }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobCauses_Destroy([DataSourceRequest] DataSourceRequest request, JobCauses JobCauses)
        {
            var JobCausess = db.JobCauses.Where(x => x.JobCausesId == JobCauses.JobCausesId).FirstOrDefault();
            if (JobCausess != null)
            {

                db.JobCauses.Attach(JobCausess);
                db.JobCauses.Remove(JobCausess);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted JobCauses number " + JobCauses.JobCausesId);

            }

            return Json(new[] { JobCauses }.ToDataSourceResult(request));
        }

        [AllowAnonymous]
        public ActionResult JobCauseDDBind(string Id)
        {
            var Request = db.JobRequest.Where(x => x.Num.ToString() == Id).FirstOrDefault();
            if (Request != null)
            {
                var JobCauses = db.JobCauses.Where(x => x.OrderId == Request.JobRequestId).Select(s => new SelectListItem
                {
                    Text = s.Name,
                    Value = s.JobCausesId.ToString()
                });
                return Json(JobCauses, JsonRequestBehavior.AllowGet);
            }
            return Json("");
           
        }

    }
}
