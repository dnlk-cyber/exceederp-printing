using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.UI;
using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class FollowUpController : Controller
    {
        //
        // GET: /FleetManagement/FollowUp/
        ExceedDbContext db = new ExceedDbContext();
     public FollowUpController()
		{
			
		}
	 
		//
		// GET: /FleetManagement/FollowUp/
		public ActionResult Index()
		{
			return View();
		}


        public ActionResult FollowUp_Read([DataSourceRequest] DataSourceRequest request, Guid Id)
        {
            var FollowUp = db.FollowUp.Where(x => x.JobOrderId == Id).ToList();

            return Json(FollowUp.ToDataSourceResult(request));
        }

		  [AcceptVerbs(HttpVerbs.Post)]
		public ActionResult FollowUp_Create([DataSourceRequest] DataSourceRequest request, FollowUp FollowUp , Guid Id)
		{
            ModelState.Remove("FollowUpId");
            ModelState.Remove("JobOrderId");

			if (FollowUp != null )
			{
				FollowUp.FollowUpId = Guid.NewGuid();
                FollowUp.JobOrderId = Id;
                FollowUp.DurationHr = FollowUp.DurationHr;
				FollowUp.DateCreated = DateTime.Now;
				FollowUp.CreatedBy = User.Identity.Name;
		 
	            db.FollowUp.Add(FollowUp);
                db.SaveChanges();
			}

			return Json(new[] { FollowUp }.ToDataSourceResult(request));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult FollowUp_Update([DataSourceRequest] DataSourceRequest request, FollowUp FollowUp)
		{
			if (FollowUp != null )
			{
				FollowUp.LastModified = DateTime.Now;
				FollowUp.ModifiedBy = User.Identity.Name;
		 
		//		db.FollowUp.(FollowUp);
			}

			return Json(new[] {FollowUp}.ToDataSourceResult(request,ModelState));
		}



        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FollowUp_Destory([DataSourceRequest] DataSourceRequest request, FollowUp FollowUp)
        {
            
                db.FollowUp.Attach(FollowUp);
                db.FollowUp.Remove(FollowUp);
                db.SaveChanges();
            
            return Json(new[] { FollowUp }.ToDataSourceResult(request, ModelState));
        }

	}
}

