using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using ExceedERP.Helpers.Construction.Extensions;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Data.OleDb;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Estimation
{
    [AuthorizeRoles(ConstructionConstants.EstimationUser)]
    public class ProjectEstimationWorkItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static int Category { get; set; }
        public static decimal OverHeadRate { get; set; }
        public static decimal ProfitRate { get; set; }
        public static string CurrentUSer { get; set; }

        private ConstructionInputs inputs;
        //public static AvailableProjectTypes ProjectType { get; set; }
        public static int? BlockId { get; set; }
        public ActionResult Index(int type)
        {
            #region Server side project type checking
            //AvailableProjectTypes currentType = (AvailableProjectTypes)type;
            //ViewBag.Title = "Estimation Template ";
            //ViewBag.Project = "";


            //if (currentType == AvailableProjectTypes.Building)
            //{
            //    ProjectType = AvailableProjectTypes.Building;
            //    ViewBag.Title = "Estimation Template | Temlate for Building Projects";
            //    ViewBag.Project = "Building";


            //}
            //else if (currentType == AvailableProjectTypes.Road)
            //{
            //    ProjectType = AvailableProjectTypes.Road;
            //    ViewBag.Title = "Estimation Template | Temlate for Road Projects";
            //    BlockId = null;
            //}
            //else if (currentType == AvailableProjectTypes.Dam)
            //{
            //    ProjectType = AvailableProjectTypes.Dam;
            //    ViewBag.Title = "Estimation Template | Temlate for Dam Projects";
            //    BlockId = null;
            //}
            //else if (currentType == AvailableProjectTypes.Irrigation)
            //{
            //    ProjectType = AvailableProjectTypes.Irrigation;
            //    ViewBag.Title = "Estimation Template | Temlate for Irrigation Projects";
            //    BlockId = null;
            //}
            //else
            //{
            //    ProjectId = 0;
            //    BlockId = null;
            //} 
            #endregion


            var workItemCategories = db.WorkCategory.Select(wc => new { wc.Name, wc.ConfigurationId, wc.WorkCategoryId });
            ViewData["Categories"] = workItemCategories.Distinct().ToList();

            inputs = new ConstructionInputs();
            db.Configuration.ProxyCreationEnabled = false;
            var lookups = db.Lookups.ToList();

            var units = lookups.Where(x => x.LookupType == LookUpTypes.UOM).ToList();
            ViewData["Unit"] = units;
            //var projects = lookups.Where(x => x.LookupType == LookUpTypes.PROJECT_TYPE).ToList();
            //ViewData["ProjectTypeList"] = projects.ToList();

            //var joinedItems = (from ii in db.InventoryItems
            //                   join mPI in db.MaterialPriceIndexIndices
            //                   on ii.ItemCode equals mPI.ItemCode
            //                   select new
            //                   {
            //                       itemCode = mPI.ItemCode
            //                   }).ToList();
            //var itemCodes = new HashSet<string>(joinedItems.Select(ic => ic.itemCode));

            //var items = db.InventoryItems.Where(item => (itemCodes.Contains(item.ItemCode)));
            //ViewData["Items"] = items.Select(p => new { ItemCode = p.ItemCode, Name = p.Name }).ToList(); ;
            var items = db.InventoryItems.Select(p => new { p.ItemCode, p.Name }).ToList();
            ViewData["Items"] = items;
            //
            // find the construction item categories which are also available in the material price index 
            //var joinedCategories = (from cIC in db.ConstructionItemCategories
            //                        join mPI in db.MaterialPriceIndexIndices
            //                        on cIC.ItemCategoryCode equals mPI.ItemCategoryCode
            //                        select new
            //                        {
            //                            categoryCode = mPI.ItemCategoryCode,
            //                            name = cIC.Name
            //                        }).ToList();
            //var codes = new HashSet<string>(joinedCategories.Select(ic => ic.categoryCode));

            //var itemCategories = db.ConstructionItemCategories.Where(ic => (codes.Contains(ic.ItemCategoryCode)));
            //ViewData["ItemCategorys"] = itemCategories.Select(p => new { ItemCategoryCode = p.ItemCategoryCode, Name = p.Name }).ToList(); 
            var itemCategories = db.ConstructionItemCategories.Select(p => new { p.ItemCategoryCode, p.Name }).ToList();
            ViewData["ItemCategorys"] = itemCategories;
            //

            var jobTitle = db.ProjectEstimationIndexLabour.ToList();

            ViewData["titles"] =
                jobTitle.Select(
                    x =>
                        new
                        {
                            ProjectEstimationIndexLabourId = x.ProjectEstimationIndexLabourId,
                            Title = GetTitle(x.ProjectJobTitleId)
                        }).ToList();

            var workItems = db.ProjectEstimationWorkItem.Select(w => new { w.ProjectEstimationWorkItemId, w.WorkCategoryId, w.ItemName, w.ProjectType }).ToList();
            ViewData["WorkItems"] = workItems;

            SetDefaultValue();


            var internalEquipment = db.InternalMachinaryRent.ToList();
            var externalEquipment = db.MachinaryRent.ToList();
            var internalIds = new HashSet<Guid>(internalEquipment.Select(ic => ic.FleetsID));
            var externalIds = new HashSet<Guid>(externalEquipment.Select(ee => ee.FleetsID));
            var combinedEquipments = db.Fleets.Where(flt => (internalIds.Contains(flt.FleetsID)) || (externalIds.Contains(flt.FleetsID)));
            ViewData["Equipment"] = combinedEquipments.Select(eq => new { FleetsID = eq.FleetsID, EquipmentName = eq.EquipmentName });
            //ViewData["Equipment"] = db.Fleets.Select(p => new { FleetsID = p.FleetsID, EquipmentName = p.EquipmentName }).ToList();

            CurrentUSer = User.Identity.Name;
            var uom = db.UnitOfMeasurement.ToList();
            ViewData["uom"] = uom.Select(x => new { x.UnitOfMeasurementId, x.Abbreviation }).ToList(); ;
            var types = db.EquipmentType.ToList();
            //ViewData["EquipmentType"] = types.Select(
            //            x =>
            //                new
            //                {
            //                    EquipmentTypeName = x.EquipmentTypeName,
            //                    EquipmentTypeID = x.EquipmentTypeID,
            //                }).ToList();

            //var categories = db.EquipmentIdentity.ToList();
            //ViewData["EquipmentIdentity"] = categories.Select(
            //          x =>
            //              new
            //              {
            //                  EquipmentCategoryName = x.EquipmentCategoryName,
            //                  EquipmentIdentityID = x.EquipmentIdentityID,
            //              }).ToList();

            //var workitemperproject = db.ProjectEstimationWorkItem.ToList();
            //UpdateProjectTotalCost(
            //       workitemperproject.Select(x => x.ProjectEstimationWorkItemId).ToList());
            ViewData["Blocks"] = db.Blocks.Select(b => new { b.Name, b.BlockId }).ToList();

            var availableWorkItems = db.ProjectEstimationWorkItem.Where(wi => wi.ProjectType == (AvailableProjectTypes)type).Select(i => new { i.ItemName, i.ProjectEstimationWorkItemId });
            ViewData["TemplateWorkItems"] = availableWorkItems.ToList();
            return View();
        }
        public ActionResult UpdateLatestChanges()
        {
            this.UpdateChangedInformation();
            return RedirectToAction("Index", "ProjectEstimationWorkItem", new { type = 0 });
        }
        //public JsonResult GetWorkCategories()
        //{
        //    //var allCategories = db.WorkCategory.ToList();
        //    //List<WorkCategory> categoriesOfCurrentProjectType = new List<WorkCategory>();
        //    //var typeMappings = db.ProjectTypeMappings.Where(tm => tm.ProjectTypeId == ProjectId).ToList();
        //    //if (typeMappings.Any())
        //    //{
        //    //    foreach (ProjectTypeMapping mapping in typeMappings)
        //    //    {
        //    //        var categoriesOfCurrentStructureType = allCategories.Where(c => c.ConfigurationId == mapping.ConfigurationId).ToList();
        //    //        categoriesOfCurrentProjectType.AddRange(categoriesOfCurrentStructureType);
        //    //    }
        //    //}
        //    #region using joined tables
        //    //var workItemCategories = db.WorkCategory;
        //    var workItemCategories = (from w in db.WorkCategory
        //                              join p in db.ProjectTypeMappings
        //                                  on w.ConfigurationId equals p.ConfigurationId
        //                              where p.ProjectType == (AvailableProjectTypes)type
        //                              select (new { w.WorkCategoryId, w.ConfigurationId, w.Description, w.Name, w.Index })).ToList();
        //    #endregion
        //    var listToReturn = new List<WorkCategory>();
        //    var category = new WorkCategory();
        //    if (ProjectType == AvailableProjectTypes.Road)
        //    {
        //        foreach (var c in workItemCategories)
        //        {
        //            category = new WorkCategory
        //            {
        //                WorkCategoryId = c.WorkCategoryId,
        //                Name = c.Name + " ( Ser. " + c.Index + " )"
        //            };
        //            listToReturn.Add(category);
        //        }
        //    }
        //    else
        //    {
        //        foreach (var c in workItemCategories)
        //        {
        //            category = new WorkCategory
        //            {
        //                WorkCategoryId = c.WorkCategoryId,
        //                Name = c.Name
        //            };
        //            listToReturn.Add(category);
        //        }
        //    }

        //    return Json(listToReturn.Select(c => new { Name = c.Name, WorkCategoryId = c.WorkCategoryId }), JsonRequestBehavior.AllowGet);
        //}
        private string GetTitle(int id)
        {
            var pJTitle = db.ProjectJobTitles.Where(x => x.ProjectJobTitleId == id).FirstOrDefault();
            if (pJTitle != null)
            {
                return pJTitle.JobTitle;
            }
            return "";
        }

        //public void SetProjectId(int value)
        //{
        //    ProjectId = 0;
        //    ProjectId = value;
        //    // Category = category;
        //}

        //public void SetBlock(int? blockId)
        //{
        //    BlockId = blockId;
        //}

        public JsonResult GetWorkItemsItems(int WorkCategoryId)
        {
            var items = db.ProjectEstimationWorkItem.ToList().Where(x => x.WorkCategoryId == WorkCategoryId).ToList();

            ViewData["WorkItems"] = items.Select(p => new { ItemName = p.ItemName, ProjectEstimationWorkItemId = p.ProjectEstimationWorkItemId });
            return Json(items.Select(p => new { ItemName = p.ItemName, ProjectEstimationWorkItemId = p.ProjectEstimationWorkItemId }), JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetEquipmentType(Guid EquipmentIdentityID)
        //{
        //    var items = db.EquipmentType.Where(x => x.EquipmentIdentityID == EquipmentIdentityID).ToList();

        //    //ViewData["EquipmentType"] = items.Select(
        //    //       x =>
        //    //           new
        //    //           {
        //    //               EquipmentTypeName = x.EquipmentTypeName,
        //    //               EquipmentTypeID = x.EquipmentTypeID,
        //    //           }).ToList();
        //    return Json(items.Select(p => new { EquipmentTypeName = p.EquipmentTypeName, EquipmentTypeID = p.EquipmentTypeID }), JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult GetEquipmentCategory()
        //{
        //    var items = db.EquipmentIdentity.ToList();
        //    ViewData["EquipmentIdentity"] = items.Select(
        //             x =>
        //                 new
        //                 {
        //                     EquipmentCategoryName = x.EquipmentCategoryName,
        //                     EquipmentIdentityID = x.EquipmentIdentityID,
        //                 }).ToList();
        //    return Json(items.Select(p => new { EquipmentCategoryName = p.EquipmentCategoryName, EquipmentIdentityID = p.EquipmentIdentityID }), JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult GetEquipment(Guid EquipmentTypeID)
        //{
        //    inputs = new ConstructionInputs();
        //    var items = inputs.GetInternalExternalEquipmentViewModel().Where(x => x.EquipmentTypeID == EquipmentTypeID);
        //    ViewData["Equipment"] = items.Select(p => new { AssetNumber = p.AssetNumber, Name = p.Name });
        //    return Json(items.Select(p => new { AssetNumber = p.AssetNumber, Name = p.Name }), JsonRequestBehavior.AllowGet);
        //}

        //public string ReturnValues()
        //{
        //    return OverHeadRate + "-" + ProfitRate;
        //}
        public ActionResult ProjectEstimationWorkItem_Read([DataSourceRequest]DataSourceRequest request, int type)
        {
            var projectType = (AvailableProjectTypes)type;

            IQueryable<ProjectEstimationWorkItem> projectEstimationWorkItems =
                db.ProjectEstimationWorkItem.Where(x => x.ProjectType == projectType && x.ParentId == 0 && !x.ContainsWorkItemAsInput);
            DataSourceResult result = projectEstimationWorkItems.ToDataSourceResult(request,
                projectEstimationWorkItem => new
                {
                    ProjectEstimationWorkItemId = projectEstimationWorkItem.ProjectEstimationWorkItemId,
                    WorkCategoryId = projectEstimationWorkItem.WorkCategoryId,
                    ItemName = projectEstimationWorkItem.ItemName,
                    TotalQuantityOfWork = projectEstimationWorkItem.TotalQuantityOfWork,
                    MaterialCost = projectEstimationWorkItem.MaterialCost,
                    LabourOutPut = projectEstimationWorkItem.LabourOutPut,
                    EquipmentOutput = projectEstimationWorkItem.EquipmentOutput,

                    MaterialCostPerUnit = projectEstimationWorkItem.MaterialCostPerUnit,
                    LaborCostPerUnit = projectEstimationWorkItem.LaborCostPerUnit,
                    EquipmentCostPerUnit = projectEstimationWorkItem.EquipmentCostPerUnit,
                    DirectCost = projectEstimationWorkItem.DirectCost,
                    OverHeadCost = projectEstimationWorkItem.OverHeadCost,
                    ProfitCost = projectEstimationWorkItem.ProfitCost,
                    TotalUnitCost = projectEstimationWorkItem.TotalUnitCost,

                    Remark = projectEstimationWorkItem.Remark,
                    ProjectType = projectEstimationWorkItem.ProjectType,
                    LabourHourlyOutPut = projectEstimationWorkItem.LabourHourlyOutPut,
                    UnitOfMeasurementId = projectEstimationWorkItem.UnitOfMeasurementId,
                    OverheadRate = projectEstimationWorkItem.OverheadRate,
                    ProfitMargin = projectEstimationWorkItem.ProfitMargin,
                    ParentId = projectEstimationWorkItem.ParentId,
                    IsSubCategory = projectEstimationWorkItem.IsSubCategory,
                    ItemCode = projectEstimationWorkItem.ItemCode,
                    ContainsWorkItemAsInput = projectEstimationWorkItem.ContainsWorkItemAsInput,

                    OrgBranchName = projectEstimationWorkItem.OrgBranchName,
                    DateCreated = projectEstimationWorkItem.DateCreated,
                    LastModified = projectEstimationWorkItem.LastModified,
                    CreatedBy = projectEstimationWorkItem.CreatedBy,
                    ModifiedBy = projectEstimationWorkItem.ModifiedBy,
                    BlockId = projectEstimationWorkItem.BlockId
                });
            //UpdateProjectTotalCost(projectEstimationWorkItems.Select(x => x.ProjectEstimationWorkItemId).ToList());
            return Json(result);
            //}

        }

        // for template work items that contain other work items as input, just like normal work items contain material, labor and equipment
        public ActionResult CustomWorkItems_Read([DataSourceRequest]DataSourceRequest request, int type)
        {
            var projectType = (AvailableProjectTypes)type;

            var existingCustomWorkItemToCoy = db.ProjectEstimationCustomWorkItems.Where(w => w.ProjectType == projectType).ToList();

            IQueryable<ProjectEstimationWorkItem> estimationCustomWorkItems =
                db.ProjectEstimationWorkItem.Where(x => x.ProjectType == projectType && x.ParentId == 0 && x.ContainsWorkItemAsInput);

            #region add the existing custom work item to the new design
            if (!estimationCustomWorkItems.Any())
            {
                foreach (ProjectEstimationCustomWorkItem customToCopy in existingCustomWorkItemToCoy)
                {
                    var items = db.EstimationCustomWorkItemContents.Where(w => w.ProjectEstimationCustomWorkItemId == customToCopy.ProjectEstimationCustomWorkItemId).ToList();
                    var materials = db.EstimationCustomWorkItemMaterial.Where(m => m.ProjectEstimationCustomWorkItemId == customToCopy.ProjectEstimationCustomWorkItemId).ToList();
                    var labors = db.EstimationCustomWorkItemDirectLabor.Where(l => l.ProjectEstimationCustomWorkItemId == customToCopy.ProjectEstimationCustomWorkItemId).ToList();
                    var equipments = db.EstimationCustomWorkItemEquipment.Where(e => e.ProjectEstimationCustomWorkItemId == customToCopy.ProjectEstimationCustomWorkItemId).ToList();
                    var entity = new ProjectEstimationWorkItem
                    {
                        //ProjectEstimationWorkItemId = customToCopy.ProjectEstimationWorkItemId,
                        WorkCategoryId = customToCopy.WorkCategoryId,
                        ItemName = customToCopy.WorkItemName,
                        DirectCost = customToCopy.DirectCost,
                        TotalQuantityOfWork = customToCopy.TotalQuantityOfWork,
                        LabourOutPut = customToCopy.LaborCost,
                        EquipmentOutput = customToCopy.EquipmentCost,
                        MaterialCostPerUnit = customToCopy.MaterialCostPerUnit,
                        LaborCostPerUnit = customToCopy.LaborCostPerUnit,
                        EquipmentCostPerUnit = customToCopy.EquipmentCostPerUnit,
                        TotalUnitCost = customToCopy.TotalUnitRate,
                        OverHeadCost = customToCopy.OverHeadCost,
                        ProfitCost = customToCopy.ProfitCost,
                        Remark = customToCopy.Remark,
                        BlockId = BlockId,
                        ProjectType = projectType,
                        MaterialCost = customToCopy.MaterialCost,
                        LabourHourlyOutPut = customToCopy.HourlyOutPut,
                        UnitOfMeasurementId = customToCopy.UnitOfMeasurementId,
                        OverheadRate = OverHeadRate,
                        ProfitMargin = ProfitRate,
                        ParentId = 0,
                        IsSubCategory = false,
                        ItemCode = customToCopy.ItemCode,
                        ContainsWorkItemAsInput = true,

                        OrgBranchName = customToCopy.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = customToCopy.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = customToCopy.ModifiedBy
                    };
                    //entity.DirectCost = entity.MaterialCostPerUnit + entity.EquipmentCostPerUnit + entity.LaborCostPerUnit;
                    //entity.TotalUnitCost = entity.LabourHourlyOutPut > 0 ? (entity.DirectCost + entity.OverHeadCost + entity.ProfitCost) / entity.LabourHourlyOutPut :
                    //     (entity.DirectCost + entity.OverHeadCost + entity.ProfitCost);
                    db.ProjectEstimationWorkItem.Add(entity);
                    db.SaveChanges();

                    #region copy resources
                    foreach (EstimationCustomWorkItemContent item in items)
                    {
                        var contentEntity = new EstimationTemplateCustomWorkItemDetail
                        {
                            ParentId = entity.ProjectEstimationWorkItemId,
                            ProjectEstimationWorkItemId = item.ProjectEstimationWorkItemId,
                            QuantityPerUnit = item.QuantityPerUnit,
                            UnitCost = item.UnitCost,
                            TotalCost = item.UnitPrice,
                            CreatedBy = item.CreatedBy,
                            DateCreated = item.DateCreated,
                            LastModified = item.LastModified,
                            ModifiedBy = item.ModifiedBy,
                            OrgBranchName = item.OrgBranchName,

                        };
                        db.EstimationTemplateCustomWorkItemDetails.Add(contentEntity);
                        db.SaveChanges();
                    }
                    foreach (EstimationCustomWorkItemMaterial material in materials)
                    {
                        var materialEntity = new ProjectEstimationMaterial
                        {
                            ItemCategoryCode = material.ItemCategoryCode,
                            ItemCode = material.ItemCode,
                            ProjectEstimationWorkItemId = entity.ProjectEstimationWorkItemId,
                            Quantity = material.Quantity,
                            Unit = material.Unit,
                            UnitPrice = material.UnitPrice,
                            TotalCost = material.TotalCost,

                            CreatedBy = material.CreatedBy,
                            DateCreated = material.DateCreated,
                            LastModified = material.LastModified,
                            ModifiedBy = material.ModifiedBy,
                            OrgBranchName = material.OrgBranchName,
                        };
                        db.ProjectEstimationMaterial.Add(materialEntity);
                        db.SaveChanges();
                    }
                    foreach (EstimationCustomWorkItemDirectLabor labor in labors)
                    {
                        var laborEtntity = new ProjectEstimationDirectLabour
                        {
                            ProjectEstimationIndexLabourId = labor.ProjectEstimationIndexLabourId,
                            IndexedHourlySalary = labor.IndexedHourlySalary,
                            ProjectEstimationWorkItemId = entity.ProjectEstimationWorkItemId,
                            Quantity = labor.Quantity,
                            Unit = labor.Unit,
                            UnitFactory = labor.UnitFactory,
                            TotalCost = labor.TotalCost,

                            CreatedBy = labor.CreatedBy,
                            DateCreated = labor.DateCreated,
                            LastModified = labor.LastModified,
                            ModifiedBy = labor.ModifiedBy,
                            OrgBranchName = labor.OrgBranchName,
                        };
                        db.ProjectEstimationDirectLabour.Add(laborEtntity);
                        db.SaveChanges();
                    }
                    foreach (EstimationCustomWorkItemEquipment equipment in equipments)
                    {
                        var equipmentEntity = new ProjectEstimationEquipment
                        {
                            FleetsID = equipment.FleetsID,
                            ProjectEstimationWorkItemId = entity.ProjectEstimationWorkItemId,
                            Quantity = equipment.Quantity,
                            RentalCost = equipment.RentalCost,
                            UnitFactory = equipment.UnitFactory,
                            Unit = equipment.Unit,
                            TotalCost = equipment.TotalCost,

                            CreatedBy = equipment.CreatedBy,
                            DateCreated = equipment.DateCreated,
                            LastModified = equipment.LastModified,
                            ModifiedBy = equipment.ModifiedBy,
                            OrgBranchName = equipment.OrgBranchName,
                        };
                        db.ProjectEstimationEquipment.Add(equipmentEntity);
                        db.SaveChanges();
                    }
                    #endregion
                }
            }
            #endregion

            DataSourceResult result = estimationCustomWorkItems.ToDataSourceResult(request,
                projectEstimationWorkItem => new
                {
                    ProjectEstimationWorkItemId = projectEstimationWorkItem.ProjectEstimationWorkItemId,
                    WorkCategoryId = projectEstimationWorkItem.WorkCategoryId,
                    ItemName = projectEstimationWorkItem.ItemName,
                    TotalQuantityOfWork = projectEstimationWorkItem.TotalQuantityOfWork,
                    MaterialCost = projectEstimationWorkItem.MaterialCost,
                    LabourOutPut = projectEstimationWorkItem.LabourOutPut,
                    EquipmentOutput = projectEstimationWorkItem.EquipmentOutput,

                    MaterialCostPerUnit = projectEstimationWorkItem.MaterialCostPerUnit,
                    LaborCostPerUnit = projectEstimationWorkItem.LaborCostPerUnit,
                    EquipmentCostPerUnit = projectEstimationWorkItem.EquipmentCostPerUnit,
                    DirectCost = projectEstimationWorkItem.DirectCost,
                    OverHeadCost = projectEstimationWorkItem.OverHeadCost,
                    ProfitCost = projectEstimationWorkItem.ProfitCost,
                    TotalUnitCost = projectEstimationWorkItem.TotalUnitCost,

                    Remark = projectEstimationWorkItem.Remark,
                    ProjectType = projectEstimationWorkItem.ProjectType,
                    LabourHourlyOutPut = projectEstimationWorkItem.LabourHourlyOutPut,
                    UnitOfMeasurementId = projectEstimationWorkItem.UnitOfMeasurementId,
                    OverheadRate = projectEstimationWorkItem.OverheadRate,
                    ProfitMargin = projectEstimationWorkItem.ProfitMargin,
                    ParentId = projectEstimationWorkItem.ParentId,
                    IsSubCategory = projectEstimationWorkItem.IsSubCategory,
                    ItemCode = projectEstimationWorkItem.ItemCode,
                    ContainsWorkItemAsInput = projectEstimationWorkItem.ContainsWorkItemAsInput,

                    OrgBranchName = projectEstimationWorkItem.OrgBranchName,
                    DateCreated = projectEstimationWorkItem.DateCreated,
                    LastModified = projectEstimationWorkItem.LastModified,
                    CreatedBy = projectEstimationWorkItem.CreatedBy,
                    ModifiedBy = projectEstimationWorkItem.ModifiedBy,
                    BlockId = projectEstimationWorkItem.BlockId
                });
            //UpdateProjectTotalCost(projectEstimationWorkItems.Select(x => x.ProjectEstimationWorkItemId).ToList());
            return Json(result);
            //}

        }
        public ActionResult SpecificWorkItemDetail_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var currentWorkItem = db.ProjectEstimationWorkItem.Find(id);
            List<WorkItemDetailViewModel> list = new List<WorkItemDetailViewModel>();
            if (currentWorkItem != null)
            {
                #region add components
                var nameRecord = new WorkItemDetailViewModel
                {
                    Attribute = "Item Name",
                    Value = currentWorkItem.ItemName
                };
                var qtyRecord = new WorkItemDetailViewModel
                {
                    Attribute = "Total Qty of work",
                    Value = currentWorkItem.TotalQuantityOfWork.ToString("N4")
                };
                var materialCostRecord = new WorkItemDetailViewModel
                {
                    Attribute = "Material Unit Cost",
                    Value = currentWorkItem.MaterialCostPerUnit.ToString("N4")
                };
                var equipmentCostRecord = new WorkItemDetailViewModel
                {
                    Attribute = "Equipment Unit Cost",
                    Value = currentWorkItem.EquipmentCostPerUnit.ToString("N4")
                };
                var laborCostRecord = new WorkItemDetailViewModel
                {
                    Attribute = "Manpower Unit Cost",
                    Value = currentWorkItem.LaborCostPerUnit.ToString("N4")
                };
                var directCostRecord = new WorkItemDetailViewModel
                {
                    Attribute = "Direct Cost of Work Item",
                    Value = currentWorkItem.DirectCost.ToString("N4")
                };
                var profitCostRecord = new WorkItemDetailViewModel
                {
                    Attribute = "Profit Cost (" + currentWorkItem.ProfitMargin + "%)",
                    Value = currentWorkItem.ProfitCost.ToString("N4")
                };
                var overheadCostRecord = new WorkItemDetailViewModel
                {
                    Attribute = "Overhead Cost (" + currentWorkItem.OverheadRate + "%)",
                    Value = currentWorkItem.OverHeadCost.ToString("N4")
                };
                var totalCostRecord = new WorkItemDetailViewModel
                {
                    Attribute = "Total Unit Cost",
                    Value = currentWorkItem.TotalUnitCost.ToString("N4")
                };
                #endregion
                //list.Add(nameRecord);
                //list.Add(qtyRecord);

                list.Add(materialCostRecord);
                list.Add(equipmentCostRecord);
                list.Add(laborCostRecord);
                list.Add(directCostRecord);
                list.Add(profitCostRecord);
                list.Add(overheadCostRecord);
                list.Add(totalCostRecord);

            }
            //IQueryable<TemplateWorkItemDetailViewModel> result = list.AsQueryable();
            return Json(list.AsQueryable().ToDataSourceResult(request));
        }
        public ActionResult EstimationWorkItemUnderWorkItem_Read([DataSourceRequest]DataSourceRequest request, int parentId, int type)
        {
            var projectType = (AvailableProjectTypes)type;

            IQueryable<ProjectEstimationWorkItem> projectEstimationWorkItems =
                db.ProjectEstimationWorkItem.Where(x => x.ProjectType == projectType && x.ParentId == parentId);
            DataSourceResult result = projectEstimationWorkItems.ToDataSourceResult(request,
                projectEstimationWorkItem => new
                {
                    ProjectEstimationWorkItemId = projectEstimationWorkItem.ProjectEstimationWorkItemId,
                    WorkCategoryId = projectEstimationWorkItem.WorkCategoryId,
                    ItemName = projectEstimationWorkItem.ItemName,
                    TotalQuantityOfWork = projectEstimationWorkItem.TotalQuantityOfWork,
                    MaterialCost = projectEstimationWorkItem.MaterialCost,
                    LabourOutPut = projectEstimationWorkItem.LabourOutPut,
                    EquipmentOutput = projectEstimationWorkItem.EquipmentOutput,

                    MaterialCostPerUnit = projectEstimationWorkItem.MaterialCostPerUnit,
                    LaborCostPerUnit = projectEstimationWorkItem.LaborCostPerUnit,
                    EquipmentCostPerUnit = projectEstimationWorkItem.EquipmentCostPerUnit,
                    DirectCost = projectEstimationWorkItem.DirectCost,
                    OverHeadCost = projectEstimationWorkItem.OverHeadCost,
                    ProfitCost = projectEstimationWorkItem.ProfitCost,
                    TotalUnitCost = projectEstimationWorkItem.TotalUnitCost,

                    Remark = projectEstimationWorkItem.Remark,
                    ProjectType = projectEstimationWorkItem.ProjectType,
                    LabourHourlyOutPut = projectEstimationWorkItem.LabourHourlyOutPut,
                    UnitOfMeasurementId = projectEstimationWorkItem.UnitOfMeasurementId,
                    OverheadRate = projectEstimationWorkItem.OverheadRate,
                    ProfitMargin = projectEstimationWorkItem.ProfitMargin,
                    ParentId = projectEstimationWorkItem.ParentId,
                    IsSubCategory = projectEstimationWorkItem.IsSubCategory,
                    ItemCode = projectEstimationWorkItem.ItemCode,
                    ContainsWorkItemAsInput = projectEstimationWorkItem.ContainsWorkItemAsInput,

                    OrgBranchName = projectEstimationWorkItem.OrgBranchName,
                    DateCreated = projectEstimationWorkItem.DateCreated,
                    LastModified = projectEstimationWorkItem.LastModified,
                    CreatedBy = projectEstimationWorkItem.CreatedBy,
                    ModifiedBy = projectEstimationWorkItem.ModifiedBy,
                    BlockId = projectEstimationWorkItem.BlockId
                });
            return Json(result);
            //}

        }
        private void SetDefaultValue()
        {
            var configartion = db.Configurations.ToList();
            if (configartion.Any())
            {
                var ovH = configartion.Where(x => x.Attribute == "OVER_HEAD_COST_RATE").FirstOrDefault();
                if (ovH != null)
                {
                    OverHeadRate = Convert.ToDecimal(ovH.CurrentValue);
                }
                var profMa = configartion.Where(x => x.Attribute == "PROFIT_COST_RATE").FirstOrDefault();
                if (profMa != null)
                {
                    ProfitRate = Convert.ToDecimal(profMa.CurrentValue);
                }
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstimationWorkItem_Create([DataSourceRequest]DataSourceRequest request, ProjectEstimationWorkItem projectEstimationWorkItem, int? parentIdW, int type)
        {
            var projectType = (AvailableProjectTypes)type;

            var errorExists = false;
            if (ModelState.IsValid)
            {
                if (ItemCodeExists(projectEstimationWorkItem, projectType))
                {
                    ModelState.AddModelError("", "Duplicate codes are not allowed!");
                    errorExists = true;
                }
                else if (IsInvalidWorkItemCode(projectEstimationWorkItem, projectType))
                {
                    ModelState.AddModelError("", "Code/series is not in the Work Category's range!");
                    errorExists = true;
                }
                if (!errorExists)
                {
                    var entity = new ProjectEstimationWorkItem
                    {
                        ProjectEstimationWorkItemId = projectEstimationWorkItem.ProjectEstimationWorkItemId,
                        WorkCategoryId = projectEstimationWorkItem.WorkCategoryId,
                        ItemName = !string.IsNullOrEmpty(projectEstimationWorkItem.ItemName) ? projectEstimationWorkItem.ItemName.RemoveUnnecessaryCharactors() : "",
                        DirectCost = projectEstimationWorkItem.DirectCost,
                        TotalQuantityOfWork = projectEstimationWorkItem.IsSubCategory ? 0 : projectEstimationWorkItem.TotalQuantityOfWork,
                        LabourOutPut = projectEstimationWorkItem.LabourOutPut,
                        EquipmentOutput = projectEstimationWorkItem.EquipmentOutput,
                        MaterialCostPerUnit = projectEstimationWorkItem.MaterialCostPerUnit,
                        LaborCostPerUnit = projectEstimationWorkItem.LaborCostPerUnit,
                        EquipmentCostPerUnit = projectEstimationWorkItem.EquipmentCostPerUnit,
                        TotalUnitCost = projectEstimationWorkItem.TotalUnitCost,
                        OverHeadCost = projectEstimationWorkItem.OverHeadCost,
                        ProfitCost = projectEstimationWorkItem.ProfitCost,
                        Remark = projectEstimationWorkItem.Remark,
                        BlockId = BlockId,
                        ProjectType = projectType,
                        MaterialCost = projectEstimationWorkItem.MaterialCost,
                        LabourHourlyOutPut = projectEstimationWorkItem.IsSubCategory ? 0 : projectEstimationWorkItem.LabourHourlyOutPut,
                        UnitOfMeasurementId = projectEstimationWorkItem.IsSubCategory ? null : projectEstimationWorkItem.UnitOfMeasurementId,
                        OverheadRate = projectEstimationWorkItem.IsSubCategory ? 0 : OverHeadRate,
                        ProfitMargin = projectEstimationWorkItem.IsSubCategory ? 0 : ProfitRate,
                        ParentId = parentIdW ?? 0,
                        IsSubCategory = projectEstimationWorkItem.IsSubCategory,
                        ItemCode = projectEstimationWorkItem.ItemCode,
                        ContainsWorkItemAsInput = projectEstimationWorkItem.ContainsWorkItemAsInput,

                        OrgBranchName = projectEstimationWorkItem.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = projectEstimationWorkItem.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = projectEstimationWorkItem.ModifiedBy
                    };
                    entity.DirectCost = entity.MaterialCostPerUnit + entity.EquipmentCostPerUnit + entity.LaborCostPerUnit;
                    entity.TotalUnitCost = entity.LabourHourlyOutPut > 0 ? (entity.DirectCost + entity.OverHeadCost + entity.ProfitCost) / entity.LabourHourlyOutPut :
                         (entity.DirectCost + entity.OverHeadCost + entity.ProfitCost);
                    db.ProjectEstimationWorkItem.Add(entity);
                    db.SaveChanges();
                    //ProjectId = 0;
                    projectEstimationWorkItem.ProjectEstimationWorkItemId = entity.ProjectEstimationWorkItemId;
                    //UpdateProjectTotalCost(projectEstimationWorkItem.ProjectEstimationWorkItemId);
                }

            }

            return Json(new[] { projectEstimationWorkItem }.ToDataSourceResult(request, ModelState));
        }
        // create custom work item
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CustomWorkItem_Create([DataSourceRequest]DataSourceRequest request, ProjectEstimationWorkItem projectEstimationWorkItem, int? parentIdW, int type)
        {
            var projectType = (AvailableProjectTypes)type;

            var errorExists = false;
            if (ModelState.IsValid)
            {
                if (ItemCodeExists(projectEstimationWorkItem, projectType))
                {
                    ModelState.AddModelError("", "Duplicate codes are not allowed!");
                    errorExists = true;
                }
                else if (IsInvalidWorkItemCode(projectEstimationWorkItem, projectType))
                {
                    ModelState.AddModelError("", "Code/series is not in the Work Category's range!");
                    errorExists = true;
                }
                if (!errorExists)
                {
                    var entity = new ProjectEstimationWorkItem
                    {
                        ProjectEstimationWorkItemId = projectEstimationWorkItem.ProjectEstimationWorkItemId,
                        WorkCategoryId = projectEstimationWorkItem.WorkCategoryId,
                        ItemName = !string.IsNullOrEmpty(projectEstimationWorkItem.ItemName) ? projectEstimationWorkItem.ItemName.RemoveUnnecessaryCharactors() : "",
                        DirectCost = projectEstimationWorkItem.DirectCost,
                        TotalQuantityOfWork = projectEstimationWorkItem.IsSubCategory ? 0 : projectEstimationWorkItem.TotalQuantityOfWork,
                        LabourOutPut = projectEstimationWorkItem.LabourOutPut,
                        EquipmentOutput = projectEstimationWorkItem.EquipmentOutput,
                        MaterialCostPerUnit = projectEstimationWorkItem.MaterialCostPerUnit,
                        LaborCostPerUnit = projectEstimationWorkItem.LaborCostPerUnit,
                        EquipmentCostPerUnit = projectEstimationWorkItem.EquipmentCostPerUnit,
                        TotalUnitCost = projectEstimationWorkItem.TotalUnitCost,
                        OverHeadCost = projectEstimationWorkItem.OverHeadCost,
                        ProfitCost = projectEstimationWorkItem.ProfitCost,
                        Remark = projectEstimationWorkItem.Remark,
                        BlockId = BlockId,
                        ProjectType = projectType,
                        MaterialCost = projectEstimationWorkItem.MaterialCost,
                        LabourHourlyOutPut = projectEstimationWorkItem.IsSubCategory ? 0 : projectEstimationWorkItem.LabourHourlyOutPut,
                        UnitOfMeasurementId = projectEstimationWorkItem.IsSubCategory ? null : projectEstimationWorkItem.UnitOfMeasurementId,
                        OverheadRate = projectEstimationWorkItem.IsSubCategory ? 0 : OverHeadRate,
                        ProfitMargin = projectEstimationWorkItem.IsSubCategory ? 0 : ProfitRate,
                        ParentId = parentIdW ?? 0,
                        IsSubCategory = projectEstimationWorkItem.IsSubCategory,
                        ItemCode = projectEstimationWorkItem.ItemCode,
                        ContainsWorkItemAsInput = true,

                        OrgBranchName = projectEstimationWorkItem.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = projectEstimationWorkItem.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = projectEstimationWorkItem.ModifiedBy
                    };
                    entity.DirectCost = entity.MaterialCostPerUnit + entity.EquipmentCostPerUnit + entity.LaborCostPerUnit;
                    entity.TotalUnitCost = entity.LabourHourlyOutPut > 0 ? (entity.DirectCost + entity.OverHeadCost + entity.ProfitCost) / entity.LabourHourlyOutPut :
                         (entity.DirectCost + entity.OverHeadCost + entity.ProfitCost);
                    db.ProjectEstimationWorkItem.Add(entity);
                    db.SaveChanges();
                    //ProjectId = 0;
                    projectEstimationWorkItem.ProjectEstimationWorkItemId = entity.ProjectEstimationWorkItemId;
                    //UpdateProjectTotalCost(projectEstimationWorkItem.ProjectEstimationWorkItemId);
                }

            }

            return Json(new[] { projectEstimationWorkItem }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EstimationWorkItemUnderWorkItem_Create([DataSourceRequest]DataSourceRequest request, ProjectEstimationWorkItem projectEstimationWorkItem, int parentIdW, int type)
        {
            var projectType = (AvailableProjectTypes)type;

            var errorExists = false;
            if (ModelState.IsValid)
            {
                if (ItemCodeExists(projectEstimationWorkItem, projectType))
                {
                    ModelState.AddModelError("", "Duplicate codes are not allowed!");
                    errorExists = true;
                }
                else if (IsInvalidWorkItemCode(projectEstimationWorkItem, projectType))
                {
                    ModelState.AddModelError("", "Code/series is not in the Work Category's range!");
                    errorExists = true;
                }
                if (!errorExists)
                {
                    var entity = new ProjectEstimationWorkItem
                    {
                        ProjectEstimationWorkItemId = projectEstimationWorkItem.ProjectEstimationWorkItemId,
                        WorkCategoryId = this.GetParentCategoryId(parentIdW),
                        ItemName = !string.IsNullOrEmpty(projectEstimationWorkItem.ItemName) ? projectEstimationWorkItem.ItemName.RemoveUnnecessaryCharactors() : "",
                        DirectCost = projectEstimationWorkItem.DirectCost,
                        TotalQuantityOfWork = projectEstimationWorkItem.IsSubCategory ? 0 : projectEstimationWorkItem.TotalQuantityOfWork,
                        LabourOutPut = projectEstimationWorkItem.LabourOutPut,
                        EquipmentOutput = projectEstimationWorkItem.EquipmentOutput,
                        MaterialCostPerUnit = projectEstimationWorkItem.MaterialCostPerUnit,
                        LaborCostPerUnit = projectEstimationWorkItem.LaborCostPerUnit,
                        EquipmentCostPerUnit = projectEstimationWorkItem.EquipmentCostPerUnit,
                        TotalUnitCost = projectEstimationWorkItem.TotalUnitCost,
                        OverHeadCost = projectEstimationWorkItem.OverHeadCost,
                        ProfitCost = projectEstimationWorkItem.ProfitCost,
                        Remark = projectEstimationWorkItem.Remark,
                        BlockId = BlockId,
                        ProjectType = projectType,
                        MaterialCost = projectEstimationWorkItem.MaterialCost,
                        LabourHourlyOutPut = projectEstimationWorkItem.IsSubCategory ? 0 : projectEstimationWorkItem.LabourHourlyOutPut,
                        UnitOfMeasurementId = projectEstimationWorkItem.IsSubCategory ? null : projectEstimationWorkItem.UnitOfMeasurementId,
                        OverheadRate = projectEstimationWorkItem.IsSubCategory ? 0 : OverHeadRate,
                        ProfitMargin = projectEstimationWorkItem.IsSubCategory ? 0 : ProfitRate,
                        ParentId = parentIdW,
                        IsSubCategory = projectEstimationWorkItem.IsSubCategory,
                        ItemCode = projectEstimationWorkItem.ItemCode,
                        ContainsWorkItemAsInput = projectEstimationWorkItem.ContainsWorkItemAsInput,


                        OrgBranchName = projectEstimationWorkItem.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = projectEstimationWorkItem.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = projectEstimationWorkItem.ModifiedBy
                    };
                    entity.DirectCost = entity.LaborCostPerUnit + entity.EquipmentCostPerUnit + entity.MaterialCostPerUnit;
                    entity.TotalUnitCost = entity.LabourHourlyOutPut > 0 ? (entity.DirectCost + entity.OverHeadCost + entity.ProfitCost) / entity.LabourHourlyOutPut :
                         (entity.DirectCost + entity.OverHeadCost + entity.ProfitCost);
                    db.ProjectEstimationWorkItem.Add(entity);
                    db.SaveChanges();
                    projectEstimationWorkItem.ProjectEstimationWorkItemId = entity.ProjectEstimationWorkItemId;
                    //UpdateProjectTotalCost(projectEstimationWorkItem.ProjectEstimationWorkItemId);
                }

            }

            return Json(new[] { projectEstimationWorkItem }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CustomSubWorkItem_Create([DataSourceRequest]DataSourceRequest request, ProjectEstimationWorkItem projectEstimationWorkItem, int parentIdW, int type)
        {
            var projectType = (AvailableProjectTypes)type;

            var errorExists = false;
            if (ModelState.IsValid)
            {
                if (ItemCodeExists(projectEstimationWorkItem, projectType))
                {
                    ModelState.AddModelError("", "Duplicate codes are not allowed!");
                    errorExists = true;
                }
                else if (IsInvalidWorkItemCode(projectEstimationWorkItem, projectType))
                {
                    ModelState.AddModelError("", "Code/series is not in the Work Category's range!");
                    errorExists = true;
                }
                if (!errorExists)
                {
                    var entity = new ProjectEstimationWorkItem
                    {
                        ProjectEstimationWorkItemId = projectEstimationWorkItem.ProjectEstimationWorkItemId,
                        WorkCategoryId = this.GetParentCategoryId(parentIdW),
                        ItemName = !string.IsNullOrEmpty(projectEstimationWorkItem.ItemName) ? projectEstimationWorkItem.ItemName.RemoveUnnecessaryCharactors() : "",
                        DirectCost = projectEstimationWorkItem.DirectCost,
                        TotalQuantityOfWork = projectEstimationWorkItem.IsSubCategory ? 0 : projectEstimationWorkItem.TotalQuantityOfWork,
                        LabourOutPut = projectEstimationWorkItem.LabourOutPut,
                        EquipmentOutput = projectEstimationWorkItem.EquipmentOutput,
                        MaterialCostPerUnit = projectEstimationWorkItem.MaterialCostPerUnit,
                        LaborCostPerUnit = projectEstimationWorkItem.LaborCostPerUnit,
                        EquipmentCostPerUnit = projectEstimationWorkItem.EquipmentCostPerUnit,
                        TotalUnitCost = projectEstimationWorkItem.TotalUnitCost,
                        OverHeadCost = projectEstimationWorkItem.OverHeadCost,
                        ProfitCost = projectEstimationWorkItem.ProfitCost,
                        Remark = projectEstimationWorkItem.Remark,
                        BlockId = BlockId,
                        ProjectType = projectType,
                        MaterialCost = projectEstimationWorkItem.MaterialCost,
                        LabourHourlyOutPut = projectEstimationWorkItem.IsSubCategory ? 0 : projectEstimationWorkItem.LabourHourlyOutPut,
                        UnitOfMeasurementId = projectEstimationWorkItem.IsSubCategory ? null : projectEstimationWorkItem.UnitOfMeasurementId,
                        OverheadRate = projectEstimationWorkItem.IsSubCategory ? 0 : OverHeadRate,
                        ProfitMargin = projectEstimationWorkItem.IsSubCategory ? 0 : ProfitRate,
                        ParentId = parentIdW,
                        IsSubCategory = projectEstimationWorkItem.IsSubCategory,
                        ItemCode = projectEstimationWorkItem.ItemCode,
                        ContainsWorkItemAsInput = true,

                        OrgBranchName = projectEstimationWorkItem.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = projectEstimationWorkItem.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = projectEstimationWorkItem.ModifiedBy
                    };
                    entity.DirectCost = entity.MaterialCostPerUnit + entity.EquipmentCostPerUnit + entity.LaborCostPerUnit;
                    entity.TotalUnitCost = entity.LabourHourlyOutPut > 0 ? (entity.DirectCost + entity.OverHeadCost + entity.ProfitCost) / entity.LabourHourlyOutPut :
                         (entity.DirectCost + entity.OverHeadCost + entity.ProfitCost);
                    db.ProjectEstimationWorkItem.Add(entity);
                    db.SaveChanges();
                    projectEstimationWorkItem.ProjectEstimationWorkItemId = entity.ProjectEstimationWorkItemId;
                }

            }

            return Json(new[] { projectEstimationWorkItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstimationWorkItem_Update([DataSourceRequest]DataSourceRequest request, ProjectEstimationWorkItem projectEstimationWorkItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEstimationWorkItem
                {
                    ProjectEstimationWorkItemId = projectEstimationWorkItem.ProjectEstimationWorkItemId,
                    WorkCategoryId = projectEstimationWorkItem.WorkCategoryId,
                    ItemName = !string.IsNullOrEmpty(projectEstimationWorkItem.ItemName) ? projectEstimationWorkItem.ItemName.RemoveUnnecessaryCharactors() : "",
                    DirectCost = projectEstimationWorkItem.DirectCost,
                    TotalQuantityOfWork = projectEstimationWorkItem.IsSubCategory ? 0 : projectEstimationWorkItem.TotalQuantityOfWork,
                    LabourOutPut = projectEstimationWorkItem.LabourOutPut,
                    EquipmentOutput = projectEstimationWorkItem.EquipmentOutput,
                    MaterialCostPerUnit = projectEstimationWorkItem.MaterialCostPerUnit,
                    LaborCostPerUnit = projectEstimationWorkItem.LaborCostPerUnit,
                    EquipmentCostPerUnit = projectEstimationWorkItem.EquipmentCostPerUnit,
                    TotalUnitCost = projectEstimationWorkItem.TotalUnitCost,
                    OverHeadCost = projectEstimationWorkItem.OverHeadCost,
                    ProfitCost = projectEstimationWorkItem.ProfitCost,
                    Remark = projectEstimationWorkItem.Remark,
                    BlockId = projectEstimationWorkItem.BlockId,
                    ProjectType = projectEstimationWorkItem.ProjectType,
                    MaterialCost = projectEstimationWorkItem.MaterialCost,
                    LabourHourlyOutPut = projectEstimationWorkItem.IsSubCategory ? 0 : projectEstimationWorkItem.LabourHourlyOutPut,
                    UnitOfMeasurementId = projectEstimationWorkItem.IsSubCategory ? null : projectEstimationWorkItem.UnitOfMeasurementId,
                    OverheadRate = projectEstimationWorkItem.IsSubCategory ? 0 : OverHeadRate,
                    ProfitMargin = projectEstimationWorkItem.IsSubCategory ? 0 : ProfitRate,
                    ParentId = projectEstimationWorkItem.ParentId,
                    IsSubCategory = projectEstimationWorkItem.IsSubCategory,
                    ItemCode = projectEstimationWorkItem.ItemCode,
                    ContainsWorkItemAsInput = projectEstimationWorkItem.ContainsWorkItemAsInput,


                    OrgBranchName = projectEstimationWorkItem.OrgBranchName,
                    DateCreated = projectEstimationWorkItem.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = projectEstimationWorkItem.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                entity.DirectCost = entity.MaterialCostPerUnit + entity.EquipmentCostPerUnit + entity.LaborCostPerUnit;
                entity.TotalUnitCost = entity.LabourHourlyOutPut > 0 ? (entity.DirectCost + entity.OverHeadCost + entity.ProfitCost) / entity.LabourHourlyOutPut :
                    entity.DirectCost + entity.OverHeadCost + entity.ProfitCost;
                db = new ExceedDbContext();
                db.ProjectEstimationWorkItem.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                UpdateWorkItemCost(projectEstimationWorkItem.ProjectEstimationWorkItemId);
            }

            return Json(new[] { projectEstimationWorkItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstimationWorkItem_Destroy([DataSourceRequest]DataSourceRequest request, ProjectEstimationWorkItem projectEstimationWorkItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEstimationWorkItem
                {
                    ProjectEstimationWorkItemId = projectEstimationWorkItem.ProjectEstimationWorkItemId,
                    WorkCategoryId = projectEstimationWorkItem.WorkCategoryId,
                    ItemName = projectEstimationWorkItem.ItemName,
                    DirectCost = projectEstimationWorkItem.DirectCost,
                    TotalQuantityOfWork = projectEstimationWorkItem.TotalQuantityOfWork,
                    LabourOutPut = projectEstimationWorkItem.LabourOutPut,
                    EquipmentOutput = projectEstimationWorkItem.EquipmentOutput,
                    MaterialCostPerUnit = projectEstimationWorkItem.MaterialCostPerUnit,
                    LaborCostPerUnit = projectEstimationWorkItem.LaborCostPerUnit,
                    EquipmentCostPerUnit = projectEstimationWorkItem.EquipmentCostPerUnit,
                    TotalUnitCost = projectEstimationWorkItem.TotalUnitCost,
                    OverHeadCost = projectEstimationWorkItem.OverHeadCost,
                    ProfitCost = projectEstimationWorkItem.ProfitCost,
                    Remark = projectEstimationWorkItem.Remark,
                    BlockId = projectEstimationWorkItem.BlockId,
                    ProjectType = projectEstimationWorkItem.ProjectType,
                    MaterialCost = projectEstimationWorkItem.MaterialCost,
                    LabourHourlyOutPut = projectEstimationWorkItem.LabourHourlyOutPut,
                    UnitOfMeasurementId = projectEstimationWorkItem.UnitOfMeasurementId,
                    OverheadRate = projectEstimationWorkItem.OverheadRate,
                    ProfitMargin = projectEstimationWorkItem.ProfitMargin,
                    ParentId = projectEstimationWorkItem.ParentId,
                    IsSubCategory = projectEstimationWorkItem.IsSubCategory,
                    ItemCode = projectEstimationWorkItem.ItemCode,
                    ContainsWorkItemAsInput = projectEstimationWorkItem.ContainsWorkItemAsInput,

                    OrgBranchName = projectEstimationWorkItem.OrgBranchName,
                    DateCreated = projectEstimationWorkItem.DateCreated,
                    LastModified = projectEstimationWorkItem.LastModified,
                    CreatedBy = projectEstimationWorkItem.CreatedBy,
                    ModifiedBy = projectEstimationWorkItem.ModifiedBy
                };

                db.ProjectEstimationWorkItem.Attach(entity);
                db.ProjectEstimationWorkItem.Remove(entity);
                db.SaveChanges();

                ProjectEstimationHelper.CleanReferenceAndOrphanedItemsOnTemplateWorkItemDeleted(entity.ProjectEstimationWorkItemId);
            }

            return Json(new[] { projectEstimationWorkItem }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult TemplateWorkItemToCopyToStructre_Read([DataSourceRequest]DataSourceRequest request, int strId, int subProjTypeMappingId)
        {
            List<CopyTemplateWorkItemViewModel> listToReturn = new List<CopyTemplateWorkItemViewModel>();

            var mapping = db.SubProjectStructureTypeMapping.Find(subProjTypeMappingId);
            if (mapping != null)
            {
                var currentSubProjectType = db.SubProjectType.Find(mapping.SubProjectTypeId);
                if (currentSubProjectType != null)
                {
                    var project = db.ProjectInformation.Find(currentSubProjectType.ProjectInformationId);
                    if (project != null)
                    {
                        var templateWorkItems = db.ProjectEstimationWorkItem
                                        .Where(tw => tw.ProjectType == project.ProjectType &&
                                        tw.WorkCategory.ConfigurationId == strId &&
                                        tw.ParentId == 0).ToList();

                        if (templateWorkItems.Any())
                        {
                            foreach (ProjectEstimationWorkItem workItem in templateWorkItems)
                            {
                                // check if the workItem is copied already 
                                var workItemInProject =
                                    db.GenericWorkItemPerProject.FirstOrDefault(
                                        wpp => wpp.ProjectEstimationWorkItemId == workItem.ProjectEstimationWorkItemId &&
                                        wpp.WorkCategory.ConfigurationId == strId &&
                                        wpp.SubProjectTypeId == mapping.SubProjectTypeId);

                                var templateToReturn = new CopyTemplateWorkItemViewModel
                                {
                                    Id = workItem.ProjectEstimationWorkItemId,
                                    ItemName = workItem.ItemName,
                                    Category = workItem.WorkCategory.Name,
                                    TotalQuantityOfWork = workItem.TotalQuantityOfWork,
                                    UnitOfMeasurement = ConsSettingHelper.GetUnitOfMearurnment(workItem.UnitOfMeasurementId ?? 0),
                                    TotalUnitRate = workItem.TotalUnitCost,
                                    IsSubCategory = workItem.IsSubCategory,
                                };
                                if (workItemInProject != null)
                                {
                                    templateToReturn.IsCopied = true;
                                }
                                listToReturn.Add(templateToReturn);
                            }
                        }
                    }
                }
            }

            return Json(listToReturn.AsQueryable().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult TemplateWorkItemToCopyToVariation_Read([DataSourceRequest]DataSourceRequest request, int strId, int subProjTypeMappingId)
        {
            List<AddVariationAndSupplementaryWorkItemViewModel> listToReturn = new List<AddVariationAndSupplementaryWorkItemViewModel>();

            var mapping = db.SubProjectStructureTypeMapping.Find(subProjTypeMappingId);
            if (mapping != null)
            {
                var currentSubProjectType = db.SubProjectType.Find(mapping.SubProjectTypeId);
                if (currentSubProjectType != null)
                {
                    var project = db.ProjectInformation.Find(currentSubProjectType.ProjectInformationId);
                    if (project != null)
                    {
                        var templateWorkItems = db.ProjectEstimationWorkItem
                                        .Where(tw => tw.ProjectType == project.ProjectType &&
                                        tw.WorkCategory.ConfigurationId == strId &&
                                        tw.ParentId == 0).ToList();

                        if (templateWorkItems.Any())
                        {
                            foreach (ProjectEstimationWorkItem workItem in templateWorkItems)
                            {
                                // check if the workItem is copied already 
                                var workItemInProject =
                                    db.GenericWorkItemPerProject.FirstOrDefault(
                                        wpp => wpp.ProjectEstimationWorkItemId == workItem.ProjectEstimationWorkItemId &&
                                        wpp.WorkCategory.ConfigurationId == strId &&
                                        wpp.SubProjectTypeId == mapping.SubProjectTypeId);

                                var templateToReturn = new AddVariationAndSupplementaryWorkItemViewModel
                                {
                                    Id = workItem.ProjectEstimationWorkItemId,
                                    ItemName = workItem.ItemName,
                                    Category = workItem.WorkCategory.Name,
                                    TotalQuantityOfWork = workItem.TotalQuantityOfWork,
                                    UnitOfMeasurement = ConsSettingHelper.GetUnitOfMearurnment(workItem.UnitOfMeasurementId ?? 0),
                                    TotalUnitRate = workItem.TotalUnitCost,
                                };
                                if (workItemInProject != null)
                                {
                                    //templateToReturn.IsCopied = true;
                                    if (workItemInProject.IsVariationWorkItem)
                                    {
                                        templateToReturn.IsCopiedAsVariationWork = true;
                                    }
                                    else if (workItemInProject.IsSupplementaryWorkItem)
                                    {
                                        templateToReturn.IsCopiedAsSupplementaryWork = true;
                                    }
                                    else
                                    {
                                        templateToReturn.IsCopied = true;
                                    }
                                }
                                listToReturn.Add(templateToReturn);
                            }
                        }
                    }
                }
            }

            return Json(listToReturn.AsQueryable().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public ActionResult TemplateWorkItemToCopyToSupplementary_Read([DataSourceRequest]DataSourceRequest request, int strId, int subProjTypeMappingId)
        {
            List<AddVariationAndSupplementaryWorkItemViewModel> listToReturn = new List<AddVariationAndSupplementaryWorkItemViewModel>();

            var mapping = db.SubProjectStructureTypeMapping.Find(subProjTypeMappingId);
            if (mapping != null)
            {
                var currentSubProjectType = db.SubProjectType.Find(mapping.SubProjectTypeId);
                if (currentSubProjectType != null)
                {
                    var project = db.ProjectInformation.Find(currentSubProjectType.ProjectInformationId);
                    if (project != null)
                    {
                        var templateWorkItems = db.ProjectEstimationWorkItem
                                        .Where(tw => tw.ProjectType == project.ProjectType &&
                                        tw.WorkCategory.ConfigurationId == strId &&
                                        tw.ParentId == 0).ToList();

                        if (templateWorkItems.Any())
                        {
                            foreach (ProjectEstimationWorkItem workItem in templateWorkItems)
                            {
                                // check if the workItem is copied already 
                                var workItemInProject =
                                    db.GenericWorkItemPerProject.FirstOrDefault(
                                        wpp => wpp.ProjectEstimationWorkItemId == workItem.ProjectEstimationWorkItemId &&
                                        wpp.WorkCategory.ConfigurationId == strId &&
                                        wpp.SubProjectTypeId == mapping.SubProjectTypeId);

                                var templateToReturn = new AddVariationAndSupplementaryWorkItemViewModel
                                {
                                    Id = workItem.ProjectEstimationWorkItemId,
                                    ItemName = workItem.ItemName,
                                    Category = workItem.WorkCategory.Name,
                                    TotalQuantityOfWork = workItem.TotalQuantityOfWork,
                                    UnitOfMeasurement = ConsSettingHelper.GetUnitOfMearurnment(workItem.UnitOfMeasurementId ?? 0),
                                    TotalUnitRate = workItem.TotalUnitCost,
                                };
                                if (workItemInProject != null)
                                {
                                    //templateToReturn.IsCopied = true;
                                    if (workItemInProject.IsVariationWorkItem)
                                    {
                                        templateToReturn.IsCopiedAsVariationWork = true;
                                    }
                                    else if (workItemInProject.IsSupplementaryWorkItem)
                                    {
                                        templateToReturn.IsCopiedAsSupplementaryWork = true;
                                    }
                                    else
                                    {
                                        templateToReturn.IsCopied = true;
                                    }
                                }
                                listToReturn.Add(templateToReturn);
                            }
                        }
                    }
                }
            }

            return Json(listToReturn.AsQueryable().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        private int GetParentCategoryId(int parentWorkItemId)
        {
            var id = 0;
            var parentWorkItem = db.ProjectEstimationWorkItem.Find(parentWorkItemId);
            if (parentWorkItem != null)
            {
                id = parentWorkItem.WorkCategoryId;
            }
            return id;
        }
        private bool IsInvalidWorkItemCode(ProjectEstimationWorkItem model, AvailableProjectTypes type)
        {
            bool isInvalid = false;
            var currentCategory = db.WorkCategory.Find(model.WorkCategoryId);
            if (currentCategory != null && type == AvailableProjectTypes.Road)
            {
                var categoryCode = currentCategory.Index.ToString();
                var codeInitial = categoryCode.Substring(0, 2) + ".";
                if (model.ItemCode != null)
                {
                    if (!(model.ItemCode.StartsWith(codeInitial)) && type == AvailableProjectTypes.Road)
                    {
                        isInvalid = true;
                    }
                }

            }
            return isInvalid;
        }
        private bool IsInvalidSubWorkItemCode(ProjectEstimationWorkItem model, int parentId, AvailableProjectTypes type)
        {
            bool isInvalid = false;
            var parentworkItem = db.ProjectEstimationWorkItem.Find(parentId);
            if (parentworkItem != null)
            {
                var parentCode = parentworkItem.ItemCode;
                if (parentCode != null)
                {
                    //var codeInitial = categoryCode.Substring(0, 2) + ".";
                    if (!(model.ItemCode.StartsWith(parentCode)) && type == AvailableProjectTypes.Road)
                    {
                        isInvalid = true;
                    }
                }

            }
            return isInvalid;
        }
        private bool ItemCodeExists(ProjectEstimationWorkItem model, AvailableProjectTypes type)
        {
            var codeExists = false;
            if (type == AvailableProjectTypes.Road)
            {
                var temp = db.ProjectEstimationWorkItem.FirstOrDefault(w => w.ItemCode == model.ItemCode && w.ProjectType == AvailableProjectTypes.Road);
                if (temp != null)
                {
                    codeExists = true;
                }
            }
            return codeExists;
        }
        private static decimal GetEquipmentUnitCost(int id)
        {
            decimal labourCost = 0;
            ExceedDbContext db = new ExceedDbContext();
            var workItem = db.ProjectEstimationWorkItem.Where(x => x.ProjectEstimationWorkItemId == id).FirstOrDefault();

            if (workItem != null)
            {
                var labouCosts =
                    db.ProjectEstimationEquipment.Where(x => x.ProjectEstimationWorkItemId == id).ToList();
                if (labouCosts.Any())
                {

                    foreach (var m in labouCosts)
                    {
                        labourCost += m.TotalCost;
                    }
                }
                if (workItem.LabourHourlyOutPut > 0)
                {
                    labourCost = labourCost / workItem.LabourHourlyOutPut;
                }
                else
                {
                    labourCost = labourCost / 1;
                }
            }
            //totalMaterialCost = labourCost;
            return labourCost;
        }

        private static decimal GetMaterialUnitCost(int id)
        {
            decimal labourCost = 0;
            ExceedDbContext db = new ExceedDbContext();
            var workItem = db.ProjectEstimationWorkItem.Where(x => x.ProjectEstimationWorkItemId == id).FirstOrDefault();

            if (workItem != null)
            {
                var labouCosts =
                    db.ProjectEstimationMaterial.Where(x => x.ProjectEstimationWorkItemId == id).ToList();
                if (labouCosts.Any())
                {

                    foreach (var m in labouCosts)
                    {
                        labourCost += m.TotalCost;
                    }
                }
                labourCost = workItem.TotalQuantityOfWork > 0 ? labourCost / workItem.TotalQuantityOfWork : labourCost;
            }

            return labourCost;
        }
        private static decimal GetIndexedHourlyPayPerJobTitle(int id)
        {
            var dbs = new ExceedDbContext();
            var jobTitles = dbs.ProjectEstimationIndexLabour.Where(x => x.ProjectEstimationIndexLabourId == id).FirstOrDefault();
            if (jobTitles != null)
            {
                return jobTitles.ApprovedIndexedHourlySalary;
            }
            return 0;
        }
        private static decimal GetDailyPayPerJobTitle(int id)

        {
            ExceedDbContext db = new ExceedDbContext();
            var jobTitles = db.ProjectEstimationIndexLabour.Where(x => x.ProjectEstimationIndexLabourId == id).FirstOrDefault();
            if (jobTitles != null)
            {
                return jobTitles.NetBirrHour;
            }
            return 0;
        }
        private static decimal GetManPowerUnitCost(int id)
        {
            ExceedDbContext db = new ExceedDbContext();
            decimal labourCost = 0;
            var workItem = db.ProjectEstimationWorkItem.Where(x => x.ProjectEstimationWorkItemId == id).FirstOrDefault();

            if (workItem != null)
            {
                var labouCosts =
                    db.ProjectEstimationDirectLabour.Where(x => x.ProjectEstimationWorkItemId == id).ToList();
                if (labouCosts.Any())
                {

                    foreach (var m in labouCosts)
                    {
                        labourCost += m.TotalCost;
                    }
                }
                if (workItem.LabourHourlyOutPut > 0)
                {
                    labourCost = labourCost / workItem.LabourHourlyOutPut;
                }
                else
                {
                    labourCost = labourCost / 1;
                }
            }
            //totalMaterialCost = labourCost;
            return labourCost;
        }

        /// <summary>
        /// update template work item when any change in material, labor and equipment
        /// resources
        /// </summary>
        /// <param name="id"></param>
        public static void UpdateWorkItemCost(int id)
        {
            ExceedDbContext db = new ExceedDbContext();
            decimal hourlyOutPut = 1;
            decimal totalMaterialCost = 0;
            decimal totalLabourCost = 0;
            decimal totalEquipmentCost = 0;
            //decimal overHeadPercent = 0;
            //decimal profitMarginPercent = 0;
            //var configartions = db.Configurations.Where(x => x.Category == "Construction").ToList();
            //if (configartions.Any())
            //{
            //    var overheadRate = configartions.Where(x => x.Reference == "OVER_HEAD_COST_RATE").FirstOrDefault();
            //    var profiRate = configartions.Where(x => x.Reference == "PROFIT_COST_RATE").FirstOrDefault();
            //    if (overheadRate != null)
            //    {
            //        overHeadPercent = Convert.ToDecimal(overheadRate.CurrentValue);
            //    }
            //    if (profiRate != null)
            //    {
            //        profitMarginPercent = Convert.ToDecimal(profiRate.CurrentValue);
            //    }
            //}
            var workItem = db.ProjectEstimationWorkItem.Where(x => x.ProjectEstimationWorkItemId == id).FirstOrDefault();

            if (workItem != null && !workItem.IsSubCategory)
            {
                workItem.DirectCost = 0;
                hourlyOutPut = workItem.LabourHourlyOutPut > 0 ? workItem.LabourHourlyOutPut : 1;
                var materials = db.ProjectEstimationMaterial.Where(x => x.ProjectEstimationWorkItemId == workItem.ProjectEstimationWorkItemId).ToList();
                if (materials.Any())
                {
                    totalMaterialCost = materials.Sum(x => x.TotalCost);
                }
                var equipments = db.ProjectEstimationEquipment.Where(x => x.ProjectEstimationWorkItemId == workItem.ProjectEstimationWorkItemId).ToList();
                if (equipments.Any())
                {
                    totalEquipmentCost = equipments.Sum(x => x.TotalCost);
                }
                var labouCosts = db.ProjectEstimationDirectLabour.Where(x => x.ProjectEstimationWorkItemId == workItem.ProjectEstimationWorkItemId).ToList();
                if (labouCosts.Any())
                {
                    totalLabourCost = labouCosts.Sum(x => x.TotalCost);
                }
                // check if its a custom work item - meaning, if it has other work item under it.
                if (workItem.ContainsWorkItemAsInput)
                {
                    var items = db.EstimationTemplateCustomWorkItemDetails.Where(t => t.ParentId == workItem.ProjectEstimationWorkItemId).ToList();
                    if (items.Any())
                    {
                        totalMaterialCost = totalMaterialCost + items.Sum(itm => itm.TotalCost);
                    }
                }
                //
                workItem.MaterialCost = totalMaterialCost;
                workItem.LabourOutPut = totalLabourCost;
                workItem.EquipmentOutput = totalEquipmentCost;
                //workItem.MaterialCostPerUnit = totalMaterialCost / hourlyOutPut;
                workItem.MaterialCostPerUnit = totalMaterialCost;
                workItem.LaborCostPerUnit = totalLabourCost / hourlyOutPut;
                workItem.EquipmentCostPerUnit = totalEquipmentCost / hourlyOutPut;
                workItem.DirectCost = workItem.MaterialCostPerUnit + workItem.EquipmentCostPerUnit + workItem.LaborCostPerUnit;
                workItem.OverHeadCost = workItem.DirectCost * (workItem.OverheadRate / 100);
                workItem.ProfitCost = workItem.DirectCost * (workItem.ProfitMargin / 100);

                workItem.TotalUnitCost = workItem.DirectCost + workItem.ProfitCost + workItem.OverHeadCost;

                //db = new ExceedDbContext();
                db.ProjectEstimationWorkItem.Attach(workItem);
                db.Entry(workItem).State = EntityState.Modified;
                db.SaveChanges();
            }

        }
        private void UpdateChangedInformation()
        {
            var workItems = db.ProjectEstimationWorkItem.Where(w => !w.IsSubCategory).ToList();
            if (workItems.Any())
            {
                foreach (ProjectEstimationWorkItem workItem in workItems)
                {
                    var hourlyOutPut = workItem.LabourHourlyOutPut > 0 ? workItem.LabourHourlyOutPut : 1;
                    var totalMaterialCost = new decimal();
                    var totalEquipmentCost = new decimal();
                    var totalLaborCost = new decimal();
                    var materials = db.ProjectEstimationMaterial.Where(m => m.ProjectEstimationWorkItemId == workItem.ProjectEstimationWorkItemId).ToList();
                    var equipments = db.ProjectEstimationEquipment.Where(e => e.ProjectEstimationWorkItemId == workItem.ProjectEstimationWorkItemId).ToList();
                    var labors = db.ProjectEstimationDirectLabour.Where(l => l.ProjectEstimationWorkItemId == workItem.ProjectEstimationWorkItemId).ToList();
                    if (materials.Any())
                    {
                        totalMaterialCost = materials.Sum(m => m.TotalCost);
                    }
                    if (labors.Any())
                    {
                        totalLaborCost = labors.Sum(l => l.TotalCost);
                    }
                    if (equipments.Any())
                    {
                        totalEquipmentCost = equipments.Sum(e => e.TotalCost);
                    }

                    //workItem.MaterialCostPerUnit = totalMaterialCost / hourlyOutPut;
                    workItem.MaterialCostPerUnit = totalMaterialCost;
                    workItem.EquipmentCostPerUnit = totalEquipmentCost / hourlyOutPut;
                    workItem.LaborCostPerUnit = totalLaborCost / hourlyOutPut;

                    workItem.DirectCost = workItem.MaterialCostPerUnit + workItem.LaborCostPerUnit + workItem.EquipmentCostPerUnit;
                    workItem.OverHeadCost = workItem.OverheadRate / 100 * workItem.DirectCost;
                    workItem.ProfitCost = workItem.ProfitMargin / 100 * workItem.DirectCost;

                    workItem.TotalUnitCost = workItem.DirectCost + workItem.ProfitCost + workItem.OverHeadCost;

                    db.ProjectEstimationWorkItem.Attach(workItem);
                    db.Entry(workItem).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }
        public static void UpdateCustomWorkItemAfterUpdatingTemplateWorkItem(int workItemId)
        {
            ExceedDbContext db = new ExceedDbContext();

            var customWorkItemContents = db.EstimationCustomWorkItemContents
                                                .Where(c => c.ProjectEstimationWorkItemId == workItemId)
                                                .ToList();
            if (customWorkItemContents.Any())
            {
                var customWorkItemIds = new HashSet<int>(customWorkItemContents.Select(c => c.ProjectEstimationCustomWorkItemId));
                var customWorkItemsToEdit = db.ProjectEstimationCustomWorkItems.Where(cwi => (customWorkItemIds.Contains(cwi.ProjectEstimationCustomWorkItemId))).ToList();

                if (customWorkItemsToEdit.Any())
                {
                    foreach (ProjectEstimationCustomWorkItem affectedWorkItem in customWorkItemsToEdit)
                    {
                        var totalCostOfCustomWorkItem = new decimal();
                        var contents = db.EstimationCustomWorkItemContents.Where(c => c.ProjectEstimationCustomWorkItemId == affectedWorkItem.ProjectEstimationCustomWorkItemId).ToList();
                        if (contents.Any())
                        {
                            foreach (EstimationCustomWorkItemContent content in contents)
                            {
                                var templateWorkItem = db.ProjectEstimationWorkItem.Find(content.ProjectEstimationWorkItemId);
                                if (templateWorkItem != null)
                                {
                                    // update content's unit const and the total cost
                                    content.UnitCost = templateWorkItem.TotalUnitCost;
                                    content.UnitPrice = content.QuantityPerUnit * templateWorkItem.TotalUnitCost;

                                    db.EstimationCustomWorkItemContents.Attach(content);
                                    db.Entry(content).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                                totalCostOfCustomWorkItem = totalCostOfCustomWorkItem + content.UnitPrice;
                            }
                            // finally update the unit rate of the custom work item
                            affectedWorkItem.TotalUnitRate = totalCostOfCustomWorkItem;

                            db.ProjectEstimationCustomWorkItems.Attach(affectedWorkItem);
                            db.Entry(affectedWorkItem).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }
            }
        }


        public ActionResult ImportTemplateWorkItemsFromExcel(IEnumerable<HttpPostedFileBase> file, int type = -1)
        {
            //HttpPostedFileBase file = Request.Files[i]; //Uploaded file
            if (file != null && type != -1)
            {
                WorkCategory selectedWorkCategory = null;
                ProjectTypeMapping selectedStrAndProjectTypeMapping = null;

                var unitsOfMeasurnment = db.UnitOfMeasurement.Select(u => new { u.Name, u.Abbreviation, u.UnitOfMeasurementId }).ToList();

                string fileNamePrefix = DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day.ToString() + DateTime.Now.Hour + DateTime.Now.Minute;

                System.Data.DataSet ds = new System.Data.DataSet();
                #region import

                foreach (HttpPostedFileBase postedFileBase in file)
                //for (int i =0; i< Request.Files.Count; i++)
                {
                    // postedFileBase = Request.Files[i];
                    //var postedFileBase = Request.Files["file"];
                    if (postedFileBase != null)
                    {
                        string fileExtension = System.IO.Path.GetExtension(postedFileBase.FileName);
                        if (fileExtension == ".xls" || fileExtension == ".xlsx" || fileExtension == ".xlsm")
                        {
                            string fileLocation = Server.MapPath("~/App_Data/Import/Construction/Estimation/") + fileNamePrefix +
                                                  DateTime.Now.Second.ToString() + postedFileBase.FileName;
                            if (System.IO.File.Exists(fileLocation))
                            {
                                System.IO.File.Delete(fileLocation);
                            }
                            postedFileBase.SaveAs(fileLocation);
                            var excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation +
                                                        ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";

                            if (fileExtension == ".xls")
                            {
                                excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation +
                                                        ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                            }
                            //connection String for xlsx file format.
                            else if (fileExtension == ".xlsx")
                            {

                                excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation +
                                                        ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                            }
                            else if (fileExtension == ".xlsm")
                            {

                                excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation +
                                                        ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                            }
                            //Create Connection to Excel work book and add oledb namespace
                            OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                            excelConnection.Open();
                            System.Data.DataTable dt = new System.Data.DataTable();

                            dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            if (dt == null)
                            {
                                return null;
                            }

                            String[] excelSheets = new String[dt.Rows.Count];
                            int t = 0;
                            //excel data saves in temp file here.
                            foreach (System.Data.DataRow row in dt.Rows)
                            {
                                // hanle the case when any filter is applied to the excell workbook
                                // excell adds other additional datatables with the name something like '_xlnm#_FilterDatabase'
                                // So, in such cases, some mechanism of removing the default _FilterDataBase have to be excluded from the count
                                if (row["TABLE_NAME"].ToString().Contains("_FilterDatabase")) { }
                                else
                                {
                                    excelSheets[t] = row["TABLE_NAME"].ToString();
                                    t++;
                                }
                            }
                            OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);


                            string query = string.Format("Select * from [{0}]", excelSheets[0]);
                            using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                            {
                                dataAdapter.Fill(ds);
                            }

                        }
                    }
                }
                #endregion

                string errors = "";

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int j = 0; j <= ds.Tables[0].Rows.Count - 1; j++)
                    {
                        string itemCode = ds.Tables[0].Rows[j].ItemArray[0].ToString();
                        var uom = ds.Tables[0].Rows[j].ItemArray[2].ToString();
                        var itemUnitOfMeasurenment = unitsOfMeasurnment.FirstOrDefault(u => u.Abbreviation == uom);

                        // the selected row in the excel file is work item (with codes like 14.01, 10.5(a), 10.5(b))
                        if (!string.IsNullOrEmpty(itemCode) && itemCode.Contains("."))
                        {
                            // 
                            if (selectedWorkCategory != null && selectedStrAndProjectTypeMapping != null) {  
                            var existingWorkItemWithSameCode =
                               (AvailableProjectTypes)type == AvailableProjectTypes.Road ?
                               db.ProjectEstimationWorkItem
                                .FirstOrDefault(w => w.ProjectType == (AvailableProjectTypes)type && w.ItemCode == itemCode) :
                                db.ProjectEstimationWorkItem
                                .FirstOrDefault(w => w.ProjectType == (AvailableProjectTypes)type &&
                                                w.ItemCode == itemCode &&
                                                w.WorkCategoryId == selectedWorkCategory.WorkCategoryId &&
                                                w.WorkCategory.ConfigurationId == selectedStrAndProjectTypeMapping.ConfigurationId);
                            if (existingWorkItemWithSameCode == null)
                            {
                                // check for parent-child relationship
                                var registeredParent =
                                    db.ProjectEstimationWorkItem
                                    .FirstOrDefault(w => itemCode.StartsWith(w.ItemCode) 
                                                    && w.ProjectType == (AvailableProjectTypes)type &&
                                                    w.WorkCategoryId == selectedWorkCategory.WorkCategoryId);
                                if (registeredParent != null && !registeredParent.IsSubCategory)
                                {
                                    registeredParent.IsSubCategory = true;

                                    db.ProjectEstimationWorkItem.Attach(registeredParent);
                                    db.Entry(registeredParent).State = EntityState.Modified;
                                    db.SaveChanges();
                                }

                                var workItemToInsert = new ProjectEstimationWorkItem
                                {
                                    ItemCode = itemCode,
                                    WorkCategoryId = selectedWorkCategory.WorkCategoryId,
                                    ItemName = ds.Tables[0].Rows[j].ItemArray[1].ToString(),
                                    ProjectType = (AvailableProjectTypes)type,
                                    ParentId = registeredParent != null ? registeredParent.ProjectEstimationWorkItemId : 0,
                                    UnitOfMeasurementId = itemUnitOfMeasurenment != null ? itemUnitOfMeasurenment.UnitOfMeasurementId : 0,

                                    TotalQuantityOfWork = 1,
                                    LabourHourlyOutPut = 1,
                                    OverheadRate = OverHeadRate,
                                    ProfitMargin = ProfitRate,
                                    IsSubCategory = false,
                                    DateCreated = DateTime.Now,
                                    CreatedBy = User.Identity.Name,
                                };
                                db.ProjectEstimationWorkItem.Add(workItemToInsert);
                                db.SaveChanges();
                            }
                        } }
                        // 
                        // the selected row in the excel file is either structure type or work category
                        // structure and wrok category coding varies according to project type
                        else
                        {
                            if ((AvailableProjectTypes)type == AvailableProjectTypes.Road)
                            {

                                if (!string.IsNullOrEmpty(itemCode) && Regex.IsMatch(itemCode, @"^[0-9]+$"))
                                {
                                    var parsedCode = int.Parse(itemCode);
                                    var rem = parsedCode % 1000;
                                    // this row is Structure type (series no. like 1000, 2000 ... 
                                    if (rem == 0)
                                    {
                                        var mapping = db.ProjectTypeMappings.FirstOrDefault(m => m.ProjectType == AvailableProjectTypes.Road && m.CodeOrSeries == parsedCode);
                                        if (mapping == null)
                                        {
                                            errors += "the code/series " + parsedCode + " wasn't found in the project & structure type mapping \n";
                                        }
                                    }
                                    // this is category (codes like 1100, 1200 ...
                                    else
                                    {
                                        var category = db.WorkCategory.FirstOrDefault(c => c.Index == parsedCode);
                                        if (category != null)
                                        {
                                            selectedWorkCategory = category;
                                        }
                                        else
                                        {
                                            errors += "Work Category with the code/series " + parsedCode + " is not registered.";
                                        }
                                    }
                                }
                            }
                            else if((AvailableProjectTypes)type == AvailableProjectTypes.Building)
                            {
                                // work Category ... e.g. 1 Excavation and Earth Work
                                if (!string.IsNullOrEmpty(itemCode) && Regex.IsMatch(itemCode, @"^[0-9]+$") && selectedStrAndProjectTypeMapping != null)
                                {
                                    var parsedCode = int.Parse(itemCode);
                                    var cat = db.WorkCategory.FirstOrDefault(wc => wc.Index == parsedCode && wc.ConfigurationId == selectedStrAndProjectTypeMapping.ConfigurationId);
                                    if(cat == null)
                                    {
                                        errors += "Work Category with index +" + parsedCode + " was not found \n";
                                    }
                                    else
                                    {
                                        selectedWorkCategory = cat;
                                    }
                                }
                                // structure type ... like str (1) Substructure
                                else if(!string.IsNullOrEmpty(itemCode) && Regex.IsMatch(itemCode, @"^[(0-9)]+$"))
                                {
                                    var temp = itemCode.Replace('(', ' ').Replace(')', ' ').Trim();
                                    var parsingSucceeded = int.TryParse(temp, out int result);
                                    if (parsingSucceeded)
                                    {
                                        var currentMapping = 
                                            db.ProjectTypeMappings
                                            .FirstOrDefault(p => p.ProjectType == AvailableProjectTypes.Building && p.CodeOrSeries == result);
                                        if (currentMapping == null)
                                        {
                                            errors += "Project type and structure type mapping with the code/series " + result + " was not found \n";
                                        }
                                        else
                                        {
                                            selectedStrAndProjectTypeMapping = currentMapping;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return RedirectToAction("Index", "ProjectEstimationWorkItem", new { type = type });
        }
        //private bool StartsWithStringExtenssion(string longString, string shortString)
        //{
        //   return longString.StartsWith(shortString, StringComparison.OrdinalIgnoreCase);
        //}
        // private ProjectEstimationWorkItem GetWorkItemParentIfExists(string currentItemCode)
        //{
        //    var normalizedString = currentItemCode.to
        //   return db.ProjectEstimationWorkItem.FirstOrDefault(w => StartsWithStringExtenssion(currentItemCode, w.ItemCode));
        //}

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
