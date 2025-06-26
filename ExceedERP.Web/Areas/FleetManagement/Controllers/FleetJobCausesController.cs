using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;  
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using System.Data.Entity;
using ExceedERP.Core.Domain.Maintenance;
using XAPI;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class FleetJobCausesController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult FleetJobCauses_Read([DataSourceRequest] DataSourceRequest request , Guid jobRequstId)
        {
            var JobCauses = db.JobCauses.Where(x=>x.OrderId == jobRequstId).ToList();

            return Json(JobCauses.ToDataSourceResult(request));
        }

        public ActionResult FleetJobCauses_Read([DataSourceRequest] DataSourceRequest request, int Id)
        {
            var Request = db.JobRequest.Where(x => x.Num == Id).FirstOrDefault();
            if (Request != null)
            {
                var JobCauses = db.JobCauses.Where(x => x.OrderId == Request.JobRequestId).ToList();
                var result = JobCauses.ToDataSourceResult(request, model => new JobCauses
                {
                    Name = XAPI.Common.RemoveSpecialChars(model.Name),
                    Description = XAPI.Common.RemoveSpecialChars(model.Description),
                });
                return Json(result);
                
            }
            return Json("");
         
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FleetJobCauses_Create([DataSourceRequest] DataSourceRequest request, JobCauses model , Guid jobRequstId)
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
                    Name = XAPI.Common.RemoveSpecialChars(model.Name),
                    Description = XAPI.Common.RemoveSpecialChars(model.Description),
                    OrderId = jobRequstId,
                    DateCreated = DateTime.Now,
                    LastModified = null,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy =""
                };
                db.JobCauses.Add(Level);
                db.SaveChanges();
                model.JobCausesId = Level.JobCausesId;
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FleetJobCauses_Update([DataSourceRequest] DataSourceRequest request, JobCauses JobCauses)
        {
            if (JobCauses != null)
            {
                JobCauses.ModifiedBy = User.Identity.Name;
                JobCauses.LastModified = DateTime.Now;
                JobCauses.Name = XAPI.Common.RemoveSpecialChars(JobCauses.Name);
                JobCauses.Description = XAPI.Common.RemoveSpecialChars(JobCauses.Description);
                db.JobCauses.Attach(JobCauses);
                db.Entry(JobCauses).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { JobCauses }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FleetJobCauses_Destroy([DataSourceRequest] DataSourceRequest request, JobCauses JobCauses)
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

        
        public ActionResult JobCauseDDBind(string Id)
        {
            var Request = db.JobRequest.Where(x => x.Num.ToString() == Id).FirstOrDefault();
            if (Request != null)
            {
                var JobCauses = db.JobCauses.Where(x => x.OrderId == Request.JobRequestId).Select(s => new SelectListItem
                {
                    Text = s.Description,
                    Value = s.JobCausesId.ToString()
                });
                return Json(JobCauses, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
           
        }

    }
}
