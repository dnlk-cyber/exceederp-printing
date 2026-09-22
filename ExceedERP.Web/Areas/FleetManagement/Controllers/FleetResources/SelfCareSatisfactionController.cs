using Kendo.Mvc.UI;
using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using ExceedERP.ApplicationServices.FleetManagement.FleetResource;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Maintenance;
namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    //[AuthorizeRoles(FleetManagementRoles.ResourceReturnUser)] 
    public class SelfCareSatisfactionController : Controller
    {
        IVehicleReturnService _VehicleReturnService;

        private readonly CacheData CacheData = new CacheData();
        ExceedDbContext db = new ExceedDbContext();


       
        //
        // GET: /FleetManagement/VehicleReturn/
        public ActionResult Index()
        {  
            return View();
        }


        public ActionResult SelfCareSatisfactionRead([DataSourceRequest] DataSourceRequest request,Guid Id)
        {
            var satisfactionNote = db.SelfCareSatisfaction.Where(x=>x.RequestedEquipmentId==Id).ToList();
            DataSourceResult Result = satisfactionNote.ToDataSourceResult(request, Note => new
            {
                RequestedEquipmentId = Note.RequestedEquipmentId,
                SelfCareSatisfactionId = Note.SelfCareSatisfactionId,
                DateCreated = Note.DateCreated,
                CreatedBy = Note.CreatedBy,
                SatisfactionNote = Note.SatisfactionNote
            });
            return Json(Result);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SelfCareSatisfactionCreate([DataSourceRequest] DataSourceRequest request, SelfCareSatisfaction SNote,Guid Id)
        {
            if (ModelState.IsValid)
            {
                var entity = new SelfCareSatisfaction
                {
                    RequestedEquipmentId = Id,
                   // SelfCareSatisfactionId = SNote.SelfCareSatisfactionId,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    SatisfactionNote = SNote.SatisfactionNote
                };
                db.SelfCareSatisfaction.Add(entity);
                db.SaveChanges();
            }

            return Json(new[] { SNote }.ToDataSourceResult(request,ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SelfCareSatisfactionUpdate([DataSourceRequest] DataSourceRequest request, SelfCareSatisfaction SNote)
        {
            var fetchNote = db.SelfCareSatisfaction.FirstOrDefault(x => x.SelfCareSatisfactionId == SNote.SelfCareSatisfactionId);
            fetchNote.SatisfactionNote = SNote.SatisfactionNote;
             if(ModelState.IsValid)
            {
                db.Entry(fetchNote).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
               
            }

            return Json(new[] { SNote }.ToDataSourceResult(request,ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SelfCareSatisfactionDestroy([DataSourceRequest] DataSourceRequest request, SelfCareSatisfaction SNote)
        {
            var fetchNote = db.SelfCareSatisfaction.FirstOrDefault(x => x.SelfCareSatisfactionId == SNote.SelfCareSatisfactionId);
            //fetchNote.SatisfactionNote = SNote.SatisfactionNote;
            if (ModelState.IsValid)
            {
                db.SelfCareSatisfaction.Remove(fetchNote);
                db.SaveChanges();

            }

            return Json(new[] { SNote }.ToDataSourceResult(request, ModelState));
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

