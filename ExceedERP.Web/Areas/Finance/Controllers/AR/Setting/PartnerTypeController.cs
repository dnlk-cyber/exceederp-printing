using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.AR.SalesOrder;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AR.Setting
{
    public class PartnerTypeController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PartnerTypes_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PartnerType> partnertypes = _db.PartnerTypes;
            DataSourceResult result = partnertypes.ToDataSourceResult(request, partnerType => new {
                PartnerTypeId = partnerType.PartnerTypeId,
                TypeName = partnerType.TypeName,
                Description = partnerType.Description,
                Enable = partnerType.Enable,
                DateCreated = partnerType.DateCreated,
                LastModified = partnerType.LastModified,
                CreatedBy = partnerType.CreatedBy,
                ModifiedBy = partnerType.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PartnerTypes_Create([DataSourceRequest]DataSourceRequest request, PartnerType partnerType)
        {
            if (ModelState.IsValid)
            {
                var entity = new PartnerType
                {
                    TypeName = partnerType.TypeName,
                    Description = partnerType.Description,
                    Enable = partnerType.Enable,
                    DateCreated = partnerType.DateCreated,
                    LastModified = partnerType.LastModified,
                    CreatedBy = partnerType.CreatedBy,
                    ModifiedBy = partnerType.ModifiedBy
                };

                _db.PartnerTypes.Add(entity);
                _db.SaveChanges();
                partnerType.PartnerTypeId = entity.PartnerTypeId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR partner type", "Success", User.Identity.Name, $"created  {partnerType.TypeName}", Modules.Finance);
                #endregion
            }

            return Json(new[] { partnerType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PartnerTypes_Update([DataSourceRequest]DataSourceRequest request, PartnerType partnerType)
        {
            if (ModelState.IsValid)
            {
                var entity = new PartnerType
                {
                    PartnerTypeId = partnerType.PartnerTypeId,
                    TypeName = partnerType.TypeName,
                    Description = partnerType.Description,
                    Enable = partnerType.Enable,
                    DateCreated = partnerType.DateCreated,
                    LastModified = partnerType.LastModified,
                    CreatedBy = partnerType.CreatedBy,
                    ModifiedBy = partnerType.ModifiedBy
                };

                _db.PartnerTypes.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR partner type", "Success", User.Identity.Name, $"updated {partnerType.TypeName}", Modules.Finance);
                #endregion
            }

            return Json(new[] { partnerType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PartnerTypes_Destroy([DataSourceRequest]DataSourceRequest request, PartnerType partnerType)
        {
            if (ModelState.IsValid)
            {
                var entity = new PartnerType
                {
                    PartnerTypeId = partnerType.PartnerTypeId,
                    TypeName = partnerType.TypeName,
                    Description = partnerType.Description,
                    Enable = partnerType.Enable,
                    DateCreated = partnerType.DateCreated,
                    LastModified = partnerType.LastModified,
                    CreatedBy = partnerType.CreatedBy,
                    ModifiedBy = partnerType.ModifiedBy
                };

                _db.PartnerTypes.Attach(entity);
                _db.PartnerTypes.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { partnerType }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Enable_Disable(string id)
        {
            if (id != null)
            {
                var partnerType = _db.PartnerTypes.Find(id);
                if (partnerType != null && partnerType.Enable)
                {
                    partnerType.Enable = false;
                }
                else
                {
                    if (partnerType != null) partnerType.Enable = true;
                }
                if (partnerType != null)
                {
                    _db.PartnerTypes.Attach(partnerType);
                    _db.Entry(partnerType).State = EntityState.Modified;
                    _db.SaveChanges();
                }
            }

            return RedirectToAction("Index");

        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
