using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Inventory;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Contract
{
    public class RepresentativeElementsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult RepresentativeElements_Read([DataSourceRequest]DataSourceRequest request, int projectIdReadElement, int type)
        {
            IQueryable<ProjectRepresentativeElement> elements = db.ProjectRepresentativeElements.Where(e => e.ProjectInformationId == projectIdReadElement);
            if (type != -1) { elements = elements.Where(e => e.IndexType == (RepresentativeElementIndexType)type); }
            DataSourceResult result = elements.ToDataSourceResult(request, element => new
            {
                ProjectRepresentativeElementId = element.ProjectRepresentativeElementId,
                ProjectInformationId = element.ProjectInformationId,
                FleetsID = element.FleetsID,
                ProjectEstimationIndexLabourId = element.ProjectEstimationIndexLabourId,
                ItemCode = element.ItemCode,
                IndexType = element.IndexType,
                IndexCode = element.IndexCode,
                Description = element.Description,
                Unit = element.Unit,
                BasicPrice = element.BasicPrice,
                RepresentedItems = element.RepresentedItems,
                Source = element.Source,

                OrgBranchName = element.OrgBranchName,
                DateCreated = element.DateCreated,
                LastModified = element.LastModified,
                CreatedBy = element.CreatedBy,
                ModifiedBy = element.ModifiedBy
            });

            return Json(result);
        }
        private bool NonAdjustableExists(int projectId)
        {
            var nonAdjustableExists = false;
            //var t = (RepresentativeElementIndexType)type;
            var element = db.ProjectRepresentativeElements.FirstOrDefault(rep => rep.IndexType == RepresentativeElementIndexType.NonAdjustable && rep.ProjectInformationId == projectId);
            if(element != null)
            {
                nonAdjustableExists = true;
            }
            return nonAdjustableExists;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RepresentativeElement_Create([DataSourceRequest]DataSourceRequest request, ProjectRepresentativeElement element, int projectIdCreateElement, int type)
        {
            if (ModelState.IsValid)
            {
                if((RepresentativeElementIndexType)type == RepresentativeElementIndexType.NonAdjustable && NonAdjustableExists(projectIdCreateElement))
                {
                    ModelState.AddModelError("non-adjustable", "Non-adjustable Index exists for this project");
                }
                else
                {
                    var entity = new ProjectRepresentativeElement
                    {
                        //ProjectRepresentativeElementId = element.ProjectRepresentativeElementId,
                        ProjectInformationId = projectIdCreateElement,
                        FleetsID = element.FleetsID,
                        ProjectEstimationIndexLabourId = element.ProjectEstimationIndexLabourId,
                        ItemCode = element.ItemCode,
                        IndexType = (RepresentativeElementIndexType)type,
                        IndexCode = element.IndexCode,
                        Description = element.Description,
                        Unit = InventoryHelper.GetUnitOfMeasurment(element.ItemCode),
                        //BasicPrice = this.GetItemUnitPrice(element.ItemCode),
                        BasicPrice = element.BasicPrice,
                        RepresentedItems = element.RepresentedItems,
                        Source = element.Source,

                        OrgBranchName = element.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = element.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = element.ModifiedBy
                    };

                    db.ProjectRepresentativeElements.Add(entity);
                    db.SaveChanges();

                    element.ProjectRepresentativeElementId = entity.ProjectRepresentativeElementId;
                    element.Unit = entity.Unit;
                    element.BasicPrice = entity.BasicPrice;

                    this.AddRepresentativeWeightRange(projectIdCreateElement, entity);
                }
                
            }

            return Json(new[] { element }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RepresentativeElement_Update([DataSourceRequest]DataSourceRequest request, ProjectRepresentativeElement element)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectRepresentativeElement
                {
                    ProjectRepresentativeElementId = element.ProjectRepresentativeElementId,
                    ProjectInformationId = element.ProjectInformationId,
                    FleetsID = element.FleetsID,
                    ProjectEstimationIndexLabourId = element.ProjectEstimationIndexLabourId,
                    ItemCode = element.ItemCode,
                    IndexType = element.IndexType,
                    IndexCode = element.IndexCode,
                    Description = element.Description,
                    Unit = InventoryHelper.GetUnitOfMeasurment(element.ItemCode),
                    //BasicPrice = this.GetItemUnitPrice(element.ItemCode),
                    BasicPrice = element.BasicPrice,
                    RepresentedItems = element.RepresentedItems,
                    Source = element.Source,

                    OrgBranchName = element.OrgBranchName,
                    DateCreated = element.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = element.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.ProjectRepresentativeElements.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { element }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RepresentativeElement_Destroy([DataSourceRequest]DataSourceRequest request, ProjectRepresentativeElement element)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectRepresentativeElement
                {
                    ProjectRepresentativeElementId = element.ProjectRepresentativeElementId,
                    ProjectInformationId = element.ProjectInformationId,
                    FleetsID = element.FleetsID,
                    ProjectEstimationIndexLabourId = element.ProjectEstimationIndexLabourId,
                    ItemCode = element.ItemCode,
                    IndexType = element.IndexType,
                    IndexCode = element.IndexCode,
                    Unit = element.Unit,
                    BasicPrice = element.BasicPrice,
                    RepresentedItems = element.RepresentedItems,
                    Source = element.Source,

                    OrgBranchName = element.OrgBranchName,
                    DateCreated = element.DateCreated,
                    LastModified = element.LastModified,
                    CreatedBy = element.CreatedBy,
                    ModifiedBy = element.ModifiedBy
                };

                db.ProjectRepresentativeElements.Attach(entity);
                db.ProjectRepresentativeElements.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { element }.ToDataSourceResult(request, ModelState));
        }

        private decimal GetItemUnitPrice(string itemCode)
        {
            var price = new decimal();
            var currentItem = db.MaterialPriceIndexIndices.FirstOrDefault(m => m.ItemCode == itemCode);
            if (currentItem != null)
            {
                price = currentItem.ApprovedCost;
            }
            return price;
        }
        private void AddRepresentativeWeightRange(int projectId, ProjectRepresentativeElement record) {
            var currentProject = db.ProjectInformation.Find(projectId);
            if(currentProject != null)
            {
                if(currentProject.ProjectType == AvailableProjectTypes.Road)
                {
                    var projectTypeMapping = db.ProjectTypeMappings.Where(p => p.ProjectType == AvailableProjectTypes.Road);
                    var ids = new HashSet<int>(projectTypeMapping.Select(p => p.ConfigurationId));
                    var list = db.Configurations.Where(x => (ids.Contains(x.ConfigurationId)) && x.Reference == ConstructionSettingCategories.STRUCTURE_TYPE.ToString()).ToList();
                    if (list.Any())
                    {
                        var num = list.Count;
                        foreach(Configuration config in list)
                        {
                            var weightRangeToAdd = new RepresentativeElementWeightRange
                            {
                                ConfigurationId = config.ConfigurationId,
                                ProjectRepresentativeElementId  = record.ProjectRepresentativeElementId,
                                PercentageMin = Math.Round((decimal)(100/num), 0),
                                PercentageMax = Math.Round((decimal)(100 / num), 0),
                                DateCreated = DateTime.Now,
                                CreatedBy = User.Identity.Name,
                            };
                            db.RepresentativeElementWeightRange.Add(weightRangeToAdd);
                            db.SaveChanges();
                        }
                    }
                }
            }
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}