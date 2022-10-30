using System;
using ExceedERP.Web.Helpers;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.CRMAdmin)] 
    public class ObjectStateController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ObjectStates_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ObjectState> objectstates = db.ObjectStates.Where(os => os.Reference == id.ToString());
            DataSourceResult result = objectstates.ToDataSourceResult(request, objectState => new {
                ObjectStateID = objectState.ObjectStateID,
                Reference = objectState.Reference,
                ObjectStateDefinitionID = objectState.ObjectStateDefinitionID,
                CreatedBy = objectState.CreatedBy,
                DateCreated = objectState.DateCreated,
                LastModified = objectState.LastModified,
                ModifiedBy = objectState.ModifiedBy,
                OrgBranchName = objectState.OrgBranchName,
                Remark = objectState.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ObjectStates_Create([DataSourceRequest]DataSourceRequest request, ObjectState objectState, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new ObjectState
                {
                    Reference = id.ToString(),
                    ObjectStateDefinitionID = objectState.ObjectStateDefinitionID,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    Remark = objectState.Remark
                };

                db.ObjectStates.Add(entity);
                db.SaveChanges();
                objectState.ObjectStateID = entity.ObjectStateID;
            }

            return Json(new[] { objectState }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ObjectStates_Update([DataSourceRequest]DataSourceRequest request, ObjectState objectState)
        {
            if (ModelState.IsValid)
            {
                var entity = new ObjectState
                {
                    ObjectStateID = objectState.ObjectStateID,
                    Reference = objectState.Reference,
                    ObjectStateDefinitionID = objectState.ObjectStateDefinitionID,
                    Remark = objectState.Remark
                };

                db.ObjectStates.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { objectState }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ObjectStates_Destroy([DataSourceRequest]DataSourceRequest request, ObjectState objectState)
        {
            if (ModelState.IsValid)
            {
                var entity = new ObjectState
                {
                    ObjectStateID = objectState.ObjectStateID,
                    Reference = objectState.Reference,
                    ObjectStateDefinitionID = objectState.ObjectStateDefinitionID,
                    Remark = objectState.Remark
                };

                db.ObjectStates.Attach(entity);
                db.ObjectStates.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Object State number " + objectState.ObjectStateID);
      
            }

            return Json(new[] { objectState }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
