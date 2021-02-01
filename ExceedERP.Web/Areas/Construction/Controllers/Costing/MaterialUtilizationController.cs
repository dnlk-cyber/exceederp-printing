using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Construction.Controllers.Costing
{
    public class MaterialUtilizationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        ConstructionInputs inputs = new ConstructionInputs();
        public ActionResult Index()
        {

            var materials = inputs.GetMaterialViewModel(0);

            ViewData["Materials"] = materials;
            return View();
        }

        public ActionResult MaterialUtilization_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var materials = inputs.GetMaterialViewModel(id);

            ViewData["Materials"] = materials;
            IQueryable<MaterialUtilization> materialutilization = db.MaterialUtilization.Where( x=> x.ProjectInformationId == id);
            DataSourceResult result = materialutilization.ToDataSourceResult(request, materialUtilization => new {
                MaterialUtilizationId = materialUtilization.MaterialUtilizationId,
                ProjectMaterialId = materialUtilization.ProjectMaterialId,
                RequestedQuantity = materialUtilization.RequestedQuantity,
                DeliveredQuantity = materialUtilization.DeliveredQuantity,
                RemainingQuantity = materialUtilization.RemainingQuantity,                
                MaterialIssuedToSite = materialUtilization.MaterialIssuedToSite,               
                Remark = materialUtilization.Remark,
                ProjectInformationId = materialUtilization.ProjectInformationId ,
                OrgBranchName = materialUtilization.OrgBranchName,
                DateCreated = materialUtilization.DateCreated,
                LastModified = materialUtilization.LastModified,
                CreatedBy = materialUtilization.CreatedBy,
                ModifiedBy = materialUtilization.ModifiedBy
                
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MaterialUtilization_Create([DataSourceRequest]DataSourceRequest request, MaterialUtilization materialUtilization, int idTwo)
        {
            if (ModelState.IsValid)
            {
                var entity = new MaterialUtilization
                {
                    ProjectMaterialId = materialUtilization.ProjectMaterialId,
                    RequestedQuantity = materialUtilization.RequestedQuantity,
                    DeliveredQuantity = materialUtilization.DeliveredQuantity,
                    RemainingQuantity = materialUtilization.RemainingQuantity,                    
                    MaterialIssuedToSite = materialUtilization.MaterialIssuedToSite,              
                    Remark = materialUtilization.Remark,
                    ProjectInformationId = idTwo ,                   
                    OrgBranchName = materialUtilization.OrgBranchName,
                    DateCreated = materialUtilization.DateCreated,
                    LastModified = materialUtilization.LastModified,
                    CreatedBy = materialUtilization.CreatedBy,
                    ModifiedBy = materialUtilization.ModifiedBy
                };

                db.MaterialUtilization.Add(entity);
                db.SaveChanges();
                materialUtilization.MaterialUtilizationId = entity.MaterialUtilizationId;
            }

            return Json(new[] { materialUtilization }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MaterialUtilization_Update([DataSourceRequest]DataSourceRequest request, MaterialUtilization materialUtilization)
        {
            if (ModelState.IsValid)
            {
                var entity = new MaterialUtilization
                {
                    MaterialUtilizationId = materialUtilization.MaterialUtilizationId,
                    ProjectMaterialId = materialUtilization.ProjectMaterialId,
                    RequestedQuantity = materialUtilization.RequestedQuantity,
                    DeliveredQuantity = materialUtilization.DeliveredQuantity,
                    RemainingQuantity = materialUtilization.RemainingQuantity,                    
                    MaterialIssuedToSite = materialUtilization.MaterialIssuedToSite,
                   Remark = materialUtilization.Remark,
                    ProjectInformationId = materialUtilization.ProjectInformationId ,
                    OrgBranchName = materialUtilization.OrgBranchName,
                    DateCreated = materialUtilization.DateCreated,
                    LastModified = materialUtilization.LastModified,
                    CreatedBy = materialUtilization.CreatedBy,
                    ModifiedBy = materialUtilization.ModifiedBy
                };

                db.MaterialUtilization.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { materialUtilization }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MaterialUtilization_Destroy([DataSourceRequest]DataSourceRequest request, MaterialUtilization materialUtilization)
        {
            if (ModelState.IsValid)
            {
                var entity = new MaterialUtilization
                {
                    MaterialUtilizationId = materialUtilization.MaterialUtilizationId,
                    ProjectMaterialId = materialUtilization.ProjectMaterialId,
                    RequestedQuantity = materialUtilization.RequestedQuantity,
                    DeliveredQuantity = materialUtilization.DeliveredQuantity,
                    RemainingQuantity = materialUtilization.RemainingQuantity,                    
                    MaterialIssuedToSite = materialUtilization.MaterialIssuedToSite,                   
                    Remark = materialUtilization.Remark,
                    ProjectInformationId = materialUtilization.ProjectInformationId ,
                    OrgBranchName = materialUtilization.OrgBranchName,
                    DateCreated = materialUtilization.DateCreated,
                    LastModified = materialUtilization.LastModified,
                    CreatedBy = materialUtilization.CreatedBy,
                    ModifiedBy = materialUtilization.ModifiedBy
                };

                db.MaterialUtilization.Attach(entity);
                db.MaterialUtilization.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { materialUtilization }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
