using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.Construction.Controllers.Estimation
{
    [AuthorizeRoles(ConstructionConstants.EstimationUser)]
    public class BoqSummaryController : Controller
    {
        //public static int projectId;
        private ExceedDbContext db;

        public BoqSummaryController()
        {
            db = new ExceedDbContext();
            //projectId = 0;
        }
        public ActionResult Index()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var list = db.ProjectInformation.AsQueryable();
            ViewData["Projects"] = list;
            var blocks = db.Blocks.AsQueryable();
            ViewData["Blocks"] = blocks;
            return View();
        }

        //public JsonResult GetBlocks(int projectId)
        //{
        //    var blocks = db.Blocks.Where(x => x.ProjectInformationId == projectId).ToList();
        //    return Json(blocks.Select(p => new { Name = p.Name, BlockId = p.BlockId }),
        //      JsonRequestBehavior.AllowGet);
        //}
        public ActionResult CleanDirtyData() {
            var existingData = db.ProjectEstmnBoqSummary.ToList();
            if (existingData.Any())
            {
                db.ProjectEstmnBoqSummary.RemoveRange(existingData);
                db.SaveChanges();
            }
            return Redirect("Index");
        }

        public ActionResult ProjectEstmnBoqSummary_Read([DataSourceRequest]DataSourceRequest request)
        {
            //var summary = db.ProjectEstmnBoqSummary.ToList().OrderByDescending(p => p.ProjectInformationId);
            //var projectWithBoQSummayIds = new HashSet<int>(summary.Select(s => s.ProjectInformationId));

            //if (projectId == 0)
            //{
            //    var defaultProject = db.ProjectInformation.FirstOrDefault( p => (projectWithBoQSummayIds.Contains(p.ProjectInformationId)));
            //    if (defaultProject != null)
            //    {
            //        projectId = defaultProject.ProjectInformationId;
            //    }
            //}
             
            IQueryable<ProjectEstmnBoqSummary> projectestmnboqsummarys = db.ProjectEstmnBoqSummary;

            DataSourceResult result = projectestmnboqsummarys.ToDataSourceResult(request, projectEstmnBoqSummary => new
            {
                ProjectEstmnBoqSummaryId = projectEstmnBoqSummary.ProjectEstmnBoqSummaryId,
                SubProjectTypeId = projectEstmnBoqSummary.SubProjectTypeId,
                StructureyType = projectEstmnBoqSummary.StructureyType,
                WorkCategory = projectEstmnBoqSummary.WorkCategory,
                Unit = projectEstmnBoqSummary.Unit,
                TotalPrice = projectEstmnBoqSummary.TotalPrice,
                Remark = projectEstmnBoqSummary.Remark,
                ProjectInformationId = projectEstmnBoqSummary.ProjectInformationId,
                SubProjectTypeName = projectEstmnBoqSummary.SubProjectTypeName,

                OrgBranchName = projectEstmnBoqSummary.OrgBranchName,
                DateCreated = projectEstmnBoqSummary.DateCreated,
                LastModified = projectEstmnBoqSummary.LastModified,
                CreatedBy = projectEstmnBoqSummary.CreatedBy,
                ModifiedBy = projectEstmnBoqSummary.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstmnBoqSummary_Create([DataSourceRequest]DataSourceRequest request, ProjectEstmnBoqSummary projectEstmnBoqSummary)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEstmnBoqSummary
                {
                    SubProjectTypeId = projectEstmnBoqSummary.SubProjectTypeId,
                    StructureyType = projectEstmnBoqSummary.StructureyType,
                    WorkCategory = projectEstmnBoqSummary.WorkCategory,
                    Unit = projectEstmnBoqSummary.Unit,
                    TotalPrice = projectEstmnBoqSummary.TotalPrice,
                    Remark = projectEstmnBoqSummary.Remark,
                    ProjectInformationId = projectEstmnBoqSummary.ProjectInformationId,
                    SubProjectTypeName = projectEstmnBoqSummary.SubProjectTypeName,
                    
                    OrgBranchName = projectEstmnBoqSummary.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = projectEstmnBoqSummary.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = projectEstmnBoqSummary.ModifiedBy
                };

                db.ProjectEstmnBoqSummary.Add(entity);
                db.SaveChanges();
                projectEstmnBoqSummary.ProjectEstmnBoqSummaryId = entity.ProjectEstmnBoqSummaryId;
            }

            return Json(new[] { projectEstmnBoqSummary }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstmnBoqSummary_Update([DataSourceRequest]DataSourceRequest request, ProjectEstmnBoqSummary projectEstmnBoqSummary)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEstmnBoqSummary
                {
                    ProjectEstmnBoqSummaryId = projectEstmnBoqSummary.ProjectEstmnBoqSummaryId,
                    SubProjectTypeId = projectEstmnBoqSummary.SubProjectTypeId,
                    StructureyType = projectEstmnBoqSummary.StructureyType,
                    WorkCategory = projectEstmnBoqSummary.WorkCategory,
                    Unit = projectEstmnBoqSummary.Unit,
                    TotalPrice = projectEstmnBoqSummary.TotalPrice,
                    Remark = projectEstmnBoqSummary.Remark,
                    ProjectInformationId = projectEstmnBoqSummary.ProjectInformationId,
                    SubProjectTypeName = projectEstmnBoqSummary.SubProjectTypeName,

                    OrgBranchName = projectEstmnBoqSummary.OrgBranchName,
                    DateCreated = projectEstmnBoqSummary.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = projectEstmnBoqSummary.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                };

                db.ProjectEstmnBoqSummary.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { projectEstmnBoqSummary }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstmnBoqSummary_Destroy([DataSourceRequest]DataSourceRequest request, ProjectEstmnBoqSummary projectEstmnBoqSummary)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEstmnBoqSummary
                {
                    SubProjectTypeId = projectEstmnBoqSummary.SubProjectTypeId,
                    StructureyType = projectEstmnBoqSummary.StructureyType,
                    WorkCategory = projectEstmnBoqSummary.WorkCategory,
                    Unit = projectEstmnBoqSummary.Unit,
                    TotalPrice = projectEstmnBoqSummary.TotalPrice,
                    Remark = projectEstmnBoqSummary.Remark,
                    ProjectInformationId = projectEstmnBoqSummary.ProjectInformationId,
                    SubProjectTypeName = projectEstmnBoqSummary.SubProjectTypeName,

                    OrgBranchName = projectEstmnBoqSummary.OrgBranchName,
                    DateCreated = projectEstmnBoqSummary.DateCreated,
                    LastModified = projectEstmnBoqSummary.LastModified,
                    CreatedBy = projectEstmnBoqSummary.CreatedBy,
                    ModifiedBy = projectEstmnBoqSummary.ModifiedBy
                };

                db.ProjectEstmnBoqSummary.Attach(entity);
                db.ProjectEstmnBoqSummary.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { projectEstmnBoqSummary }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult MakeSummary(int projectId, int subTypeId)
        {
            var pricedBoq = db.ProjectEstimationBoqBreakdown.Where(x => x.ProjectInformationId == projectId && x.SubProjectTypeId == subTypeId).ToList();
            List<ProjectEstmnBoqSummary> boqList = new List<ProjectEstmnBoqSummary>();
            var pricedBoqSummaryList = db.ProjectEstmnBoqSummary.Where(x => x.ProjectInformationId == projectId && x.SubProjectTypeId == subTypeId).ToList();
            ProjectEstmnBoqSummary boq;
            List<string> cheklist = new List<string>();
            if (pricedBoqSummaryList.Any())
            {
                db.ProjectEstmnBoqSummary.RemoveRange(pricedBoqSummaryList);
                db.SaveChanges();
            }
            foreach (var p in pricedBoq)
            {
                if (cheklist.Contains(p.WorkCategoryId.ToString()) && cheklist.Any())
                {
                    continue;
                }
                boq = new ProjectEstmnBoqSummary();
                //db = new ExceedDbContext();
                boq.ProjectInformationId = p.ProjectInformationId;
                decimal total = Convert.ToDecimal(pricedBoq.Where(x => x.WorkCategoryId == p.WorkCategoryId).Sum(x => x.TotalCost));

                // add the total of the sub-cmponents of each boq member
                var subComponents = db.ProjectEstimationBoqDetail.Where(d => d.ProjectEstimationBoqBreakdown.WorkCategoryId == p.WorkCategoryId && 
                                                                    d.ProjectEstimationBoqBreakdown.SubProjectTypeId == subTypeId && 
                                                                    d.ProjectEstimationBoqBreakdown.ProjectInformationId == projectId
                                                                    ).ToList();
                var subTotal = new decimal();
                if (subComponents.Any())
                {
                  subTotal = subComponents.Sum(s => s.TotalCost);
                }
                boq.WorkCategory = p.WorkCategory;
                boq.StructureyType = p.StructureyType;
                boq.TotalPrice = total + subTotal;
                boq.SubProjectTypeId = subTypeId;
                boq.SubProjectTypeName = p.SubProjectTypeName;
                cheklist.Add(p.WorkCategoryId.ToString());
                var entity = new ProjectEstmnBoqSummary
                {
                    StructureyType = boq.StructureyType,
                    WorkCategory = boq.WorkCategory,
                    Unit = "Birr",
                    SubProjectTypeId = subTypeId,
                    SubProjectTypeName = boq.SubProjectTypeName,
                    TotalPrice = boq.TotalPrice,
                    Remark = boq.Remark,
                    ProjectInformationId = boq.ProjectInformationId,
                };
                db.ProjectEstmnBoqSummary.Add(entity);
                db.SaveChanges();
                boq.ProjectEstmnBoqSummaryId = entity.ProjectEstmnBoqSummaryId;
            }
            //projectId = id;
            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        //private Block GetBlock(int id)
        //{
        //    Block block = new Block();
        //    var temp = db.Blocks.Find(id);
        //    if (temp != null)
        //    {
        //        block = temp;
        //    }
        //    return block;
        //}
    }
}
