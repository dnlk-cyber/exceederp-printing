using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.AR;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AR.Setting
{
    public class ArProductServiceController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ArProductService_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ArProductService> arproductservice = _db.ArProductService.Where(x=>x.ARSetupProductServiceAccountID==id);
            DataSourceResult result = arproductservice.ToDataSourceResult(request, arProductService => new {
                ArProductServiceId = arProductService.ArProductServiceId,
                Code = arProductService.Code,
                ItemName = arProductService.ItemName,
                Description = arProductService.Description,
                UoM = arProductService.UoM,
                SalesPrice = arProductService.SalesPrice,
                Remark = arProductService.Remark,
                OrgBranchName = arProductService.OrgBranchName,
                DateCreated = arProductService.DateCreated,
                LastModified = arProductService.LastModified,
                CreatedBy = arProductService.CreatedBy,
                ModifiedBy = arProductService.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ArProductService_Create(int id,[DataSourceRequest]DataSourceRequest request, ArProductService arProductService)
        {
            if (ModelState.IsValid)
            {
                var entity = new ArProductService
                {
                    ARSetupProductServiceAccountID=id,
                    Code = arProductService.Code,
                    ItemName = arProductService.ItemName,
                    Description = arProductService.Description,
                    UoM = arProductService.UoM,
                    SalesPrice = arProductService.SalesPrice,
                    Remark = arProductService.Remark,
                    OrgBranchName = arProductService.OrgBranchName,
                    DateCreated = arProductService.DateCreated,
                    LastModified = arProductService.LastModified,
                    CreatedBy = arProductService.CreatedBy,
                    ModifiedBy = arProductService.ModifiedBy
                };

                _db.ArProductService.Add(entity);
                _db.SaveChanges();
                arProductService.ArProductServiceId = entity.ArProductServiceId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR product type", "Success", User.Identity.Name, $"created  {arProductService.Description}", Modules.Finance);
                #endregion
            }

            return Json(new[] { arProductService }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ArProductService_Update([DataSourceRequest]DataSourceRequest request, ArProductService arProductService)
        {
            if (ModelState.IsValid)
            {
                var entity = new ArProductService
                {
                    ArProductServiceId = arProductService.ArProductServiceId,
                    ARSetupProductServiceAccountID = arProductService.ARSetupProductServiceAccountID,
                    Code = arProductService.Code,
                    ItemName = arProductService.ItemName,
                    Description = arProductService.Description,
                    UoM = arProductService.UoM,
                    SalesPrice = arProductService.SalesPrice,
                    Remark = arProductService.Remark,
                    OrgBranchName = arProductService.OrgBranchName,
                    DateCreated = arProductService.DateCreated,
                    LastModified = arProductService.LastModified,
                    CreatedBy = arProductService.CreatedBy,
                    ModifiedBy = arProductService.ModifiedBy
                };

                _db.ArProductService.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR product type", "Success", User.Identity.Name, $"updated  {arProductService.Description}", Modules.Finance);
                #endregion
            }

            return Json(new[] { arProductService }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ArProductService_Destroy([DataSourceRequest]DataSourceRequest request, ArProductService arProductService)
        {
            if (ModelState.IsValid)
            {
                var entity = new ArProductService
                {
                    ArProductServiceId = arProductService.ArProductServiceId,
                    Code = arProductService.Code,
                    ItemName = arProductService.ItemName,
                    Description = arProductService.Description,
                    UoM = arProductService.UoM,
                    SalesPrice = arProductService.SalesPrice,
                    OrgBranchName = arProductService.OrgBranchName,
                    DateCreated = arProductService.DateCreated,
                    LastModified = arProductService.LastModified,
                    CreatedBy = arProductService.CreatedBy,
                    ModifiedBy = arProductService.ModifiedBy
                };

                _db.ArProductService.Attach(entity);
                _db.ArProductService.Remove(entity);
                _db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted product type " + arProductService.ArProductServiceId + " " + arProductService.Description, Modules.Finance);
                #endregion
            }

            return Json(new[] { arProductService }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
