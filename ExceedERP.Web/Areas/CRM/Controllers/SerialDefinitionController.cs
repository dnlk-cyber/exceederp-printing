using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;  

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.CRMAdmin)] 
    public class SerialDefinitionController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SerialDefinitions_Read([DataSourceRequest]DataSourceRequest request, string serialDefinitionThingID)
        {
            IQueryable<SerialDefinition> serialdefinitions = db.SerialDefinitions.Where(sd=>sd.Thing==serialDefinitionThingID);
            DataSourceResult result = serialdefinitions.ToDataSourceResult(request, serialDefinition => new {
                SerialDefinitionID = serialDefinition.SerialDefinitionID,
                Thing = serialDefinition.Thing,
                Description = serialDefinition.Description,
                IsMandatory = serialDefinition.IsMandatory,
                CreatedBy = serialDefinition.CreatedBy,
                DateCreated = serialDefinition.DateCreated,
                LastModified = serialDefinition.LastModified,
                ModifiedBy = serialDefinition.ModifiedBy,
                OrgBranchName = serialDefinition.OrgBranchName,
                Remark = serialDefinition.Remark
            });
            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SerialDefinitions_Create([DataSourceRequest]DataSourceRequest request, SerialDefinition serialDefinition,string serialDefinitionThingID)
        {
                 List<SerialDefinition> serialDefinitions = db.SerialDefinitions.Where(s => s.Description == serialDefinition.Description && s.Thing==serialDefinitionThingID).ToList();
                 if (serialDefinitions.Any())
                 {
                     ModelState.AddModelError("DuplicateEntire", "Serial Definition Already Exist");
                 }
                 else
                 {
                     if (ModelState.IsValid)
                     {
                         var entity = new SerialDefinition
                         {
                             Thing = serialDefinitionThingID,
                             Description = serialDefinition.Description,
                             IsMandatory = serialDefinition.IsMandatory,
                             CreatedBy = User.Identity.Name,
                             DateCreated = DateTime.Now,
                             Remark = serialDefinition.Remark
                         };

                         db.SerialDefinitions.Add(entity);
                         db.SaveChanges();
                         serialDefinition.SerialDefinitionID = entity.SerialDefinitionID;
                     }
                 }
            return Json(new[] { serialDefinition }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SerialDefinitions_Update([DataSourceRequest]DataSourceRequest request, SerialDefinition serialDefinition, string serialDefinitionThingID)
        {
               List<SerialDefinition> serialDefinitions = db.SerialDefinitions.Where(s => s.Description == serialDefinition.Description && s.Thing==serialDefinitionThingID).ToList();
               if (serialDefinitions.Any())
               {
                   ModelState.AddModelError("DuplicateEntire", "Serial Definition Already Exist");
               }
               else
               {
                   if (ModelState.IsValid)
                   {
                       var entity = new SerialDefinition
                       {
                           SerialDefinitionID = serialDefinition.SerialDefinitionID,
                           Thing = serialDefinition.Thing,
                           Description = serialDefinition.Description,
                           IsMandatory = serialDefinition.IsMandatory,
                           Remark = serialDefinition.Remark
                       };

                       db.SerialDefinitions.Attach(entity);
                       db.Entry(entity).State = EntityState.Modified;
                       db.SaveChanges();
                   }
               }

            return Json(new[] { serialDefinition }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SerialDefinitions_Destroy([DataSourceRequest]DataSourceRequest request, SerialDefinition serialDefinition)
        {
            if (ModelState.IsValid)
            {
                var entity = new SerialDefinition
                {
                    SerialDefinitionID = serialDefinition.SerialDefinitionID,
                    Thing = serialDefinition.Thing,
                    Description = serialDefinition.Description,
                    IsMandatory = serialDefinition.IsMandatory,
                    Remark = serialDefinition.Remark
                };

                db.SerialDefinitions.Attach(entity);
                db.SerialDefinitions.Remove(entity);
                db.SaveChanges();

                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted serial Definition number " + serialDefinition.SerialDefinitionID);
     
            }

            return Json(new[] { serialDefinition }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
