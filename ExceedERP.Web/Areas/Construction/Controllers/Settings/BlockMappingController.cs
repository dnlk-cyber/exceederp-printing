using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Construction.Controllers.Settings
{
    public class BlockMappingController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            //var types = db.Configurations.Where(x => x.Reference == "STRUCTURE_TYPE").ToList();
            //ViewData["Types"] = types;
            return View();
        }

        public ActionResult BlockMappings_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<BlockMapping> blockMappings = db.BlockMappings.Where(x => x.BlockId == id);
            DataSourceResult result = blockMappings.ToDataSourceResult(request, blockMapping => new
            {
                BlockMappingId = blockMapping.BlockMappingId,
                ConfigurationId = blockMapping.ConfigurationId,
                BlockId = blockMapping.BlockId,
                Remark = blockMapping.Remark
            });

            return Json(result);
        }

        private bool ValueExists(BlockMapping model, int blockId)
        {
            bool valueExists = false;
            var temp = db.BlockMappings.Where(bm => blockId == bm.BlockId && bm.ConfigurationId == model.ConfigurationId).FirstOrDefault();
            if(temp != null)
            {
                valueExists = true;
            }
            return valueExists;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BlockMappings_Create([DataSourceRequest]DataSourceRequest request, BlockMapping blockMapping, int idTwo)
        {
            if (ModelState.IsValid)
            {
                if (ValueExists(blockMapping, idTwo)) {
                    ModelState.AddModelError("", "Duplication of structure types in a single Block is not allowed!!");
                }
                else
                {
                    var entity = new BlockMapping
                    {
                        //BlockMappingId = BlockMapping.BlockMappingId,
                        ConfigurationId = blockMapping.ConfigurationId,
                        BlockId = idTwo,
                        Remark = blockMapping.Remark
                    };

                    db.BlockMappings.Add(entity);
                    db.SaveChanges();
                    blockMapping.BlockMappingId = entity.BlockMappingId;
                }

            }

            return Json(new[] { blockMapping }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BlockMappings_Update([DataSourceRequest]DataSourceRequest request, BlockMapping blockMapping)
        {
            if (ModelState.IsValid)
            {
                var entity = new BlockMapping
                {
                    BlockMappingId = blockMapping.BlockMappingId,
                    ConfigurationId = blockMapping.ConfigurationId,
                    BlockId = blockMapping.BlockId,
                    Remark = blockMapping.Remark
                };

                db.BlockMappings.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { blockMapping }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BlockMappings_Destroy([DataSourceRequest]DataSourceRequest request, BlockMapping blockMapping)
        {
            if (ModelState.IsValid)
            {
                var entity = new BlockMapping
                {
                    BlockMappingId = blockMapping.BlockMappingId,
                    ConfigurationId = blockMapping.ConfigurationId,
                    BlockId = blockMapping.BlockId,
                    Remark = blockMapping.Remark
                };

                db.BlockMappings.Attach(entity);
                db.BlockMappings.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { blockMapping }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
