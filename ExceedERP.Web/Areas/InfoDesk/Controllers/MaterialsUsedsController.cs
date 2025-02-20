using ExceedERP.DataAccess.Context;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Linq;
using System.Web.Mvc;

using System.Data.Entity;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.InfoDesk.Support;
namespace ExceedERP.Web.Areas.InfoDesk.Controllers
{
    [Authorize]
    public class MaterialsUsedsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        // GET: InfoDesk/MaterialsUseds
        public ActionResult Index()
        {
            return View(db.MaterialsUseds.ToList());
        }
        
        public ActionResult MaterialsUseds_Read([DataSourceRequest]DataSourceRequest request, int id)
        {           
            var materials = db.MaterialsUseds.Where(x => x.AssignedTicketId == id).ToList();

            DataSourceResult result = materials.ToDataSourceResult(request, s => new
            {
                AssignedTicketId = s.AssignedTicketId,
                CustomerTicketId = s.CustomerTicketId,
                MaterialItemsId = s.MaterialItemsId,
                MaterialsUsedId = s.MaterialsUsedId,
                Quantity = s.Quantity,
                Remark = s.Remark

            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MaterialsUseds_Create([DataSourceRequest]DataSourceRequest request, MaterialsUsed material, int id)
        {

            if (ModelState.IsValid)
            {
                var assign = db.AssignTicket_HelpDesks.Find(id);
                if (assign != null)
                {
                    material.AssignedTicketId = id;
                    material.CustomerTicketId = assign.CustomerTicketId;
                }
                var entity = new MaterialsUsed
                {
                    AssignedTicketId = id,
                    CustomerTicketId = material.CustomerTicketId,
                    MaterialItemsId = material.MaterialItemsId,
                    Quantity = material.Quantity,
                    Remark = material.Remark
                };

                db.MaterialsUseds.Add(entity);
                db.SaveChanges();

                material.MaterialsUsedId = entity.MaterialsUsedId;

            }

            return Json(new[] { material }.ToDataSourceResult(request));
        }
       
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MaterialsUseds_Update([DataSourceRequest]DataSourceRequest request, MaterialsUsed material)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {

                var entity = new MaterialsUsed
                {
                    AssignedTicketId = material.AssignedTicketId,
                    CustomerTicketId = material.CustomerTicketId,
                    MaterialItemsId = material.MaterialItemsId,
                    MaterialsUsedId = material.MaterialsUsedId,
                    Quantity = material.Quantity,
                    Remark = material.Remark
                };

             
                db.MaterialsUseds.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { material }.ToDataSourceResult(request));
        }
        
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MaterialsUseds_Destroy([DataSourceRequest]DataSourceRequest request, MaterialsUsed material)
        {

            var mater = db.MaterialsUseds.FirstOrDefault(x => x.MaterialsUsedId == material.MaterialsUsedId);
            if (mater != null)
            {
                db.MaterialsUseds.Attach(mater);
                db.MaterialsUseds.Remove(mater);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted material number " + material.MaterialsUsedId);

            }
            return Json(new[] { material }.ToDataSourceResult(request, ModelState));
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
