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
    public class BlockController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            var types = db.Configurations.Where(x => x.Reference == "STRUCTURE_TYPE").ToList();
            ViewData["Types"] = types;
            //var projectTypes = db.ProjectTypes.ToList();
            //ViewData["Project"] = projectTypes;
            return View();
        }

        public ActionResult Blocks_Read([DataSourceRequest]DataSourceRequest request, int idd)
        {
            IQueryable<Block> blocks = db.Blocks.Where( x=> x.ProjectInformationId ==idd);
            DataSourceResult result = blocks.ToDataSourceResult(request, block => new {
                BlockId = block.BlockId,
                Name = block.Name,
                NumberOfBlocks = block.NumberOfBlocks,
                ProjectInformationId = block.ProjectInformationId,
                ProjectTypeId = block.ProjectTypeId,
                SubProjectTypeId = block.SubProjectTypeId,
                Remark = block.Remark
            });

            return Json(result);
        }
        public ActionResult BlocksOfSpecificType_Read([DataSourceRequest]DataSourceRequest request, int projectId, int subTypeId)
        {
            IQueryable<Block> blocks = db.Blocks.Where(x => x.ProjectInformationId == projectId && x.SubProjectTypeId == subTypeId);
            DataSourceResult result = blocks.ToDataSourceResult(request, block => new {
                BlockId = block.BlockId,
                Name = block.Name,
                NumberOfBlocks = block.NumberOfBlocks,
                ProjectInformationId = block.ProjectInformationId,
                ProjectTypeId = block.ProjectTypeId,
                SubProjectTypeId = block.SubProjectTypeId,
                Remark = block.Remark
            });

            return Json(result);
        }

        private bool CheckExistance(string name, int projectId)
        {
            var Block = db.Blocks.Where(b => b.Name == name && b.ProjectInformationId == projectId).FirstOrDefault();
            if (Block != null)
            {
                return true;
            }
            return false;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Blocks_Create([DataSourceRequest]DataSourceRequest request, Block block, int iddTwo)
        {
            if (ModelState.IsValid)
            {
                if (CheckExistance(block.Name, iddTwo))
                {
                    ModelState.AddModelError("", "Duplicate names are not allowed!!");
                }
                else
                {
                    var entity = new Block
                    {
                        BlockId = block.BlockId,
                        Name = block.Name,
                        NumberOfBlocks = block.NumberOfBlocks,
                        ProjectInformationId = iddTwo,
                        SubProjectTypeId = block.SubProjectTypeId,
                        Remark = block.Remark
                    };

                    db.Blocks.Add(entity);
                    db.SaveChanges();
                    block.BlockId = entity.BlockId;
                    
                }
            }

            return Json(new[] { block }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Blocks_Update([DataSourceRequest]DataSourceRequest request, Block block)
        {
            if (ModelState.IsValid)
            {
                var entity = new Block
                {
                    BlockId = block.BlockId,
                    Name = block.Name,
                    NumberOfBlocks = block.NumberOfBlocks,
                    ProjectInformationId = block.ProjectInformationId,
                    SubProjectTypeId = block.SubProjectTypeId,
                    Remark = block.Remark
                };

                db.Blocks.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { block }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Blocks_Destroy([DataSourceRequest]DataSourceRequest request, Block block)
        {
            if (ModelState.IsValid)
            {
                var entity = new Block
                {
                    BlockId = block.BlockId,
                    Name = block.Name,
                    NumberOfBlocks = block.NumberOfBlocks,
                    ProjectInformationId = block.ProjectInformationId,
                    ProjectTypeId = block.ProjectTypeId,
                    SubProjectTypeId = block.SubProjectTypeId,
                    Remark = block.Remark
                };

                db.Blocks.Attach(entity);
                db.Blocks.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { block }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
        private ProjectInformation GetProjecInformation(int id)
        {
            ProjectInformation project = new ProjectInformation();
            var temp = db.ProjectInformation.Find(id);
            if(temp != null)
            {
                project = temp;
            }
            return project;
        }
        public ActionResult GetProjectBlocks(int projectId)
        {

            db = new ExceedDbContext();
            var blocks = db.Blocks.Where(x => x.ProjectInformationId == projectId).OrderBy(x => x.Name);


            return Json(blocks.Select(x => new { Name = x.Name, BlockId = x.BlockId, ProjectInformationId = x.ProjectInformationId }), JsonRequestBehavior.AllowGet);
        }
    }
}
