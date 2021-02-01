using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Project
{
    [AuthorizeRoles(ConstructionConstants.EstimationUser)]
    public class CopyEstimationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static string CurrentUSer { get; set; }
        public static int Category { get; set; }
        public static string OverHeadCost { get; set; }
        public static string ProfitMargin { get; set; }
        ConstructionInputs inputs;
        public ActionResult Index()
        {
            db.Configuration.ProxyCreationEnabled = false;
            ConstructionInputs inputs = new ConstructionInputs();
            var lookups = db.Lookups.ToList();
            ViewData["Categories"] = db.WorkCategory.ToList();
            var units = lookups.Where(x => x.LookupType == LookUpTypes.UOM).ToList();
            ViewData["Unit"] = units;
            //var projects = db.ProjectTypes.ToList();
            //ViewData["ProjectTypeList"] = projects;
            var items = db.InventoryItems.ToList();
            ViewData["Items"] = items.ToList();
            var itemCategories = db.ConstructionItemCategories.ToList();
            ViewData["ItemCategorys"] = itemCategories.Select(p => new { ItemCategoryCode = p.ItemCategoryCode, Name = p.Name }); ; ;
            var jobTitle = db.ProjectEstimationIndexLabour.ToList();

            ViewData["titles"] =
                jobTitle.Select(
                    x =>
                        new
                        {
                            ProjectEstimationIndexLabourId = x.ProjectEstimationIndexLabourId,
                            Title = GetTitle(x.ProjectJobTitleId)
                        }).ToList();



            var workItems = db.ProjectEstimationWorkItem.ToList();
            ViewData["WorkItems"] = workItems;
            SetDefaultValue();

            var internalEquipment = db.InternalMachinaryRent.ToList();
            var externalEquipment = db.MachinaryRent.ToList();
            var internalIds = new HashSet<Guid>(internalEquipment.Select(ic => ic.FleetsID));
            var externalIds = new HashSet<Guid>(externalEquipment.Select(ee => ee.FleetsID));
            var combinedEquipments = db.Fleets.Where(flt => (internalIds.Contains(flt.FleetsID)) || (externalIds.Contains(flt.FleetsID)));
            ViewData["Equipment"] = combinedEquipments.Select(eq => new { FleetsID = eq.FleetsID, EquipmentName = eq.EquipmentName });

            CurrentUSer = User.Identity.Name;
            var uom = db.UnitOfMeasurement.ToList();
            ViewData["uom"] = uom.Select(
                        x =>
                            new
                            {
                                UnitOfMeasurementId = x.UnitOfMeasurementId,
                                Abbreviation = x.Abbreviation
                            }).ToList(); ;
            var types = db.EquipmentType.ToList();
            ViewData["EquipmentType"] = types.Select(
                        x =>
                            new
                            {
                                EquipmentTypeName = x.EquipmentTypeName,
                                EquipmentTypeID = x.EquipmentTypeID,
                            }).ToList();

            var categories = db.EquipmentIdentity.ToList();
            ViewData["EquipmentIdentity"] = categories.Select(
                      x =>
                          new
                          {
                              EquipmentCategoryName = x.EquipmentCategoryName,
                              EquipmentIdentityID = x.EquipmentIdentityID,
                          }).ToList();
            var projectList = db.ProjectInformation.Where(x => x.ProjectStatus == ProjectStatus.ESTIMATION).OrderBy(x => x.Name);
            ViewData["ProjectList"] = projectList.ToList();

            return View();
        }
        public JsonResult GetItems(string ItemCategoryCode)
        {
            var items = db.InventoryItems.Where(x => x.ItemCategoryCode == ItemCategoryCode).ToList();

            ViewData["Items"] = items;
            return Json(items.Select(p => new { ItemCode = p.ItemCode, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetItemCategory()
        {
            var items = db.ConstructionItemCategories.ToList();
            return Json(items.Select(p => new { ItemCategoryCode = p.ItemCategoryCode, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEquipmentType(Guid EquipmentIdentityID)
        {
            var items = db.EquipmentType.Where(x => x.EquipmentIdentityID == EquipmentIdentityID).ToList();

            //ViewData["EquipmentType"] = items.Select(
            //       x =>
            //           new
            //           {
            //               EquipmentTypeName = x.EquipmentTypeName,
            //               EquipmentTypeID = x.EquipmentTypeID,
            //           }).ToList();
            return Json(items.Select(p => new { EquipmentTypeName = p.EquipmentTypeName, EquipmentTypeID = p.EquipmentTypeID }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEquipmentCategory()
        {
            var items = db.EquipmentIdentity.ToList();
            ViewData["EquipmentIdentity"] = items.Select(
                     x =>
                         new
                         {
                             EquipmentCategoryName = x.EquipmentCategoryName,
                             EquipmentIdentityID = x.EquipmentIdentityID,
                         }).ToList();
            return Json(items.Select(p => new { EquipmentCategoryName = p.EquipmentCategoryName, EquipmentIdentityID = p.EquipmentIdentityID }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEquipment(Guid EquipmentTypeID)
        {
            inputs = new ConstructionInputs();
            var items = inputs.GetInternalExternalEquipmentViewModel().Where(x => x.EquipmentTypeID == EquipmentTypeID);
            ViewData["Equipment"] = items.Select(p => new { AssetNumber = p.AssetNumber, Name = p.Name });
            return Json(items.Select(p => new { AssetNumber = p.AssetNumber, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }
        private string GetTitle(int id)
        {
            var pJTitle = db.ProjectJobTitles.Where(x => x.ProjectJobTitleId == id).FirstOrDefault();
            if (pJTitle != null)
            {
                return pJTitle.JobTitle;
            }
            return "";
        }
        public ActionResult GetProject()
        {
            db = new ExceedDbContext();
            var projects = db.ProjectInformation.Where(x => x.ProjectStatus == ProjectStatus.ESTIMATION).OrderBy(x => x.Name);

            return Json(projects.Select(x => new { Name = x.Name, ProjectInformationId = x.ProjectInformationId }), JsonRequestBehavior.AllowGet);
        }
        public static int ProjectId { get; set; }
        public static int ProjectTye { get; set; }
        public void SetProjectId(int value)
        {
            ProjectId = 0;
            ProjectId = value;
            // Category = category;
        }
        public void SetProjectTypeId(int value)
        {
            ProjectTye = 0;
            ProjectTye = value;
            // Category = category;
        }

        private void SetDefaultValue()
        {
            var configartion = db.Configurations.ToList();
            if (configartion.Any())
            {
                var ovH = configartion.Where(x => x.Attribute == "OVER_HEAD_COST_RATE").FirstOrDefault();
                if (ovH != null)
                {
                    OverHeadCost = ovH.CurrentValue;
                }
                var profMa = configartion.Where(x => x.Attribute == "PROFIT_COST_RATE").FirstOrDefault();
                if (profMa != null)
                {
                    ProfitMargin = profMa.CurrentValue;
                }
            }
        }
        public string ReturnValues()
        {
            return OverHeadCost + "-" + ProfitMargin;
        }
        public ActionResult WorkItemPerProject_Read([DataSourceRequest]DataSourceRequest request)
        {
            //IQueryable<WorkItemPerProject> workItems = db.WorkItemPerProject.Where(x => x.ProjectInformationId == ProjectId && x.ParentId == 0);
            IQueryable<WorkItemPerProject> workItems = db.WorkItemPerProject.Where(x => x.ParentId == 0);

            DataSourceResult result = workItems.ToDataSourceResult(request, workItemPerProject => new
            {
                WorkItemPerProjectId = workItemPerProject.WorkItemPerProjectId,
                ProjectEstimationWorkItemId = workItemPerProject.ProjectEstimationWorkItemId,
                IsSubCategory = workItemPerProject.IsSubCategory,
                ItemCode = workItemPerProject.ItemCode,
                ItemName = workItemPerProject.ItemName,
                TotalQuantityOfWork = workItemPerProject.TotalQuantityOfWork,
                DirectCost = workItemPerProject.DirectCost,
                MaterialCost = workItemPerProject.MaterialCost,
                LabourOutPut = workItemPerProject.LabourOutPut,
                EquipmentOutput = workItemPerProject.EquipmentOutput,
                MaterialCostPerUnit = workItemPerProject.MaterialCostPerUnit,
                LaborCostPerUnit = workItemPerProject.LaborCostPerUnit,
                EquipmentCostPerUnit = workItemPerProject.EquipmentCostPerUnit,
                TotalUnitCost = workItemPerProject.TotalUnitCost,
                OverHeadCost = workItemPerProject.OverHeadCost,
                ProfitCost = workItemPerProject.ProfitCost,
                Remark = workItemPerProject.Remark,
                LabourHourlyOutPut = workItemPerProject.LabourHourlyOutPut,
                UnitOfMeasurementId = workItemPerProject.UnitOfMeasurementId,
                OverheadRate = workItemPerProject.OverheadRate,
                ProfitMargin = workItemPerProject.ProfitMargin,

                OrgBranchName = workItemPerProject.OrgBranchName,
                DateCreated = workItemPerProject.DateCreated,
                LastModified = workItemPerProject.LastModified,
                CreatedBy = workItemPerProject.CreatedBy,
                ModifiedBy = workItemPerProject.ModifiedBy,

                ParentId = workItemPerProject.ParentId,
                ProjectType = workItemPerProject.ProjectType,
                ProjectInformationId = workItemPerProject.ProjectInformationId,
                WorkCategoryId = workItemPerProject.WorkCategoryId
            });

            //var workItemPerProjectt = workItems.FirstOrDefault();
            //if (workItemPerProjectt != null && workItemPerProjectt.ProjectInformation.ProjectStatus == ProjectStatus.ESTIMATION)
            //{
            //    ProjectListController.UpdateProjectTotalCost(
            //        workItems.Select(x => x.WorkItemPerProjectId).ToList());
            //}

            return Json(result);
        }

        public ActionResult WorkItemUnderWorkItem_Read([DataSourceRequest]DataSourceRequest request, int parentWorkItemId)
        {
            var currentWorkItem = db.WorkItemPerProject.Find(parentWorkItemId);
            if (currentWorkItem != null)
            {

            }
            IQueryable<WorkItemPerProject> workItems = db.WorkItemPerProject.Where(x => x.ParentId == parentWorkItemId);
            DataSourceResult result = workItems.ToDataSourceResult(request, workItemPerProject => new
            {
                WorkItemPerProjectId = workItemPerProject.WorkItemPerProjectId,
                ProjectEstimationWorkItemId = workItemPerProject.ProjectEstimationWorkItemId,
                IsSubCategory = workItemPerProject.IsSubCategory,
                ItemCode = workItemPerProject.ItemCode,
                ItemName = workItemPerProject.ItemName,
                TotalQuantityOfWork = workItemPerProject.TotalQuantityOfWork,
                DirectCost = workItemPerProject.DirectCost,
                MaterialCost = workItemPerProject.MaterialCost,
                LabourOutPut = workItemPerProject.LabourOutPut,
                EquipmentOutput = workItemPerProject.EquipmentOutput,
                MaterialCostPerUnit = workItemPerProject.MaterialCostPerUnit,
                LaborCostPerUnit = workItemPerProject.LaborCostPerUnit,
                EquipmentCostPerUnit = workItemPerProject.EquipmentCostPerUnit,
                TotalUnitCost = workItemPerProject.TotalUnitCost,
                OverHeadCost = workItemPerProject.OverHeadCost,
                ProfitCost = workItemPerProject.ProfitCost,
                Remark = workItemPerProject.Remark,
                LabourHourlyOutPut = workItemPerProject.LabourHourlyOutPut,
                UnitOfMeasurementId = workItemPerProject.UnitOfMeasurementId,
                OverheadRate = workItemPerProject.OverheadRate,
                ProfitMargin = workItemPerProject.ProfitMargin,

                OrgBranchName = workItemPerProject.OrgBranchName,
                DateCreated = workItemPerProject.DateCreated,
                LastModified = workItemPerProject.LastModified,
                CreatedBy = workItemPerProject.CreatedBy,
                ModifiedBy = workItemPerProject.ModifiedBy,

                ActualQuantityOfWork = workItemPerProject.ActualQuantityOfWork,
                ParentId = workItemPerProject.ParentId,
                ProjectType = workItemPerProject.ProjectType,
                ProjectInformationId = workItemPerProject.ProjectInformationId,
                WorkCategoryId = workItemPerProject.WorkCategoryId
            });



            return Json(result);
        }
        // work items per a work category
        public ActionResult WorkItemPerCategory_Read([DataSourceRequest]DataSourceRequest request, int categoryId)
        {
            IQueryable<WorkItemPerProject> workitemperproject = db.WorkItemPerProject.Where(x => x.WorkCategoryId == categoryId);
            DataSourceResult result = workitemperproject.ToDataSourceResult(request, workItemPerProject => new
            {
                WorkItemPerProjectId = workItemPerProject.WorkItemPerProjectId,
                ProjectEstimationWorkItemId = workItemPerProject.ProjectEstimationWorkItemId,
                IsSubCategory = workItemPerProject.IsSubCategory,
                ItemCode = workItemPerProject.ItemCode,

                TotalQuantityOfWork = workItemPerProject.TotalQuantityOfWork,
                DirectCost = workItemPerProject.DirectCost,
                MaterialCost = workItemPerProject.MaterialCost,
                LabourOutPut = workItemPerProject.LabourOutPut,
                EquipmentOutput = workItemPerProject.EquipmentOutput,
                MaterialCostPerUnit = workItemPerProject.MaterialCostPerUnit,
                LaborCostPerUnit = workItemPerProject.LaborCostPerUnit,
                EquipmentCostPerUnit = workItemPerProject.EquipmentCostPerUnit,
                TotalUnitCost = workItemPerProject.TotalUnitCost,
                OverHeadCost = workItemPerProject.OverHeadCost,
                ProfitCost = workItemPerProject.ProfitCost,
                Remark = workItemPerProject.Remark,
                LabourHourlyOutPut = workItemPerProject.LabourHourlyOutPut,
                UnitOfMeasurementId = workItemPerProject.UnitOfMeasurementId,
                OverheadRate = workItemPerProject.OverheadRate,
                ProfitMargin = workItemPerProject.ProfitMargin,

                OrgBranchName = workItemPerProject.OrgBranchName,
                DateCreated = workItemPerProject.DateCreated,
                LastModified = workItemPerProject.LastModified,
                CreatedBy = workItemPerProject.CreatedBy,
                ModifiedBy = workItemPerProject.ModifiedBy,

                ParentId = workItemPerProject.ParentId,
                ProjectType = workItemPerProject.ProjectType,
                ProjectInformationId = workItemPerProject.ProjectInformationId,
                WorkCategoryId = workItemPerProject.WorkCategoryId
            });

            var workItemPerProjectt = workitemperproject.FirstOrDefault();
            if (workItemPerProjectt != null && workItemPerProjectt.ProjectInformation.ProjectStatus == ProjectStatus.ESTIMATION)
            {
                ProjectListController.UpdateProjectTotalCost(
                    workitemperproject.Select(x => x.WorkItemPerProjectId).ToList());
            }

            return Json(result);
        }

        /// <summary>
        /// returns the work items under a given Structure type
        /// </summary>
        /// <param name="request"></param>
        /// <param name="strId"></param>
        /// <returns></returns>
        public ActionResult WorkItemPerStructure([DataSourceRequest]DataSourceRequest request, int strId, int blkId)
        {
            IQueryable<WorkCategory> workcategorys = db.WorkCategory.Where(wc => wc.ConfigurationId == strId);
            List<WorkItemPerProject> workItems = new List<WorkItemPerProject>();
            BlockMapping currentBlock = new BlockMapping();
            //var tempBlock = db.BlockMappings.Where(bm => bm.ConfigurationId == strId).FirstOrDefault();
            //if(tempBlock != null)
            //{
            //    currentBlock = tempBlock;
            //}

            BlockMapping otherOne = new BlockMapping();
            var temp2 = db.BlockMappings.Find(blkId);
            if (temp2 != null)
            {
                otherOne = temp2;
            }

            foreach (WorkCategory category in workcategorys)
            {
                var workItemList = db.WorkItemPerProject.Where(wi => wi.WorkCategoryId == category.WorkCategoryId && wi.BlockId == temp2.BlockId && wi.ParentId == 0);
                if (workItemList != null && workItemList.Count() > 0)
                {
                    foreach (WorkItemPerProject item in workItemList)
                    {
                        workItems.Add(item);
                    }
                }
            }
            IQueryable<WorkItemPerProject> workItemsToReturn = workItems.AsQueryable();

            DataSourceResult result = workItemsToReturn.ToDataSourceResult(request, workItemPerProject => new
            {
                WorkItemPerProjectId = workItemPerProject.WorkItemPerProjectId,
                ProjectEstimationWorkItemId = workItemPerProject.ProjectEstimationWorkItemId,
                IsSubCategory = workItemPerProject.IsSubCategory,
                ItemCode = workItemPerProject.ItemCode,

                ItemName = workItemPerProject.ItemName,
                TotalQuantityOfWork = workItemPerProject.TotalQuantityOfWork,
                DirectCost = workItemPerProject.DirectCost,
                MaterialCost = workItemPerProject.MaterialCost,
                LabourOutPut = workItemPerProject.LabourOutPut,
                EquipmentOutput = workItemPerProject.EquipmentOutput,
                MaterialCostPerUnit = workItemPerProject.MaterialCostPerUnit,
                LaborCostPerUnit = workItemPerProject.LaborCostPerUnit,
                EquipmentCostPerUnit = workItemPerProject.EquipmentCostPerUnit,
                TotalUnitCost = workItemPerProject.TotalUnitCost,
                OverHeadCost = workItemPerProject.OverHeadCost,
                ProfitCost = workItemPerProject.ProfitCost,
                Remark = workItemPerProject.Remark,
                LabourHourlyOutPut = workItemPerProject.LabourHourlyOutPut,
                UnitOfMeasurementId = workItemPerProject.UnitOfMeasurementId,
                OverheadRate = workItemPerProject.OverheadRate,
                ProfitMargin = workItemPerProject.ProfitMargin,

                OrgBranchName = workItemPerProject.OrgBranchName,
                DateCreated = workItemPerProject.DateCreated,
                LastModified = workItemPerProject.LastModified,
                CreatedBy = workItemPerProject.CreatedBy,
                ModifiedBy = workItemPerProject.ModifiedBy,

                ParentId = workItemPerProject.ParentId,
                ProjectType = workItemPerProject.ProjectType,
                ProjectInformationId = workItemPerProject.ProjectInformationId,
                WorkCategoryId = workItemPerProject.WorkCategoryId
            });

            //var workItemPerProjectt = workitemperproject.FirstOrDefault();
            //if (workItemPerProjectt != null && workItemPerProjectt.ProjectInformation.ProjectStatus == ProjectStatus.ESTIMATION)
            //{
            //    ProjectListController.UpdateProjectTotalCost(
            //        workitemperproject.Select(x => x.WorkItemPerProjectId).ToList());
            //}

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WorkItemPerProject_Create([DataSourceRequest]DataSourceRequest request, WorkItemPerProject workItemPerProject)
        {
            if (ModelState.IsValid)
            {
                var entity = new WorkItemPerProject
                {
                    ItemName = "name from other",
                    //Name = "name",
                    TotalQuantityOfWork = workItemPerProject.TotalQuantityOfWork,
                    ProjectEstimationWorkItemId = workItemPerProject.ProjectEstimationWorkItemId,
                    IsSubCategory = workItemPerProject.IsSubCategory,
                    ItemCode = workItemPerProject.ItemCode,

                    DirectCost = workItemPerProject.DirectCost,
                    MaterialCost = workItemPerProject.MaterialCost,
                    LabourOutPut = workItemPerProject.LabourOutPut,
                    EquipmentOutput = workItemPerProject.EquipmentOutput,
                    MaterialCostPerUnit = workItemPerProject.MaterialCostPerUnit,
                    LaborCostPerUnit = workItemPerProject.LaborCostPerUnit,
                    EquipmentCostPerUnit = workItemPerProject.EquipmentCostPerUnit,
                    TotalUnitCost = workItemPerProject.TotalUnitCost,
                    OverHeadCost = workItemPerProject.OverHeadCost,
                    ProfitCost = workItemPerProject.ProfitCost,
                    ProjectType = (AvailableProjectTypes)ProjectTye,
                    Remark = workItemPerProject.Remark,
                    LabourHourlyOutPut = workItemPerProject.LabourHourlyOutPut,
                    UnitOfMeasurementId = workItemPerProject.UnitOfMeasurementId,
                    OverheadRate = workItemPerProject.OverheadRate,
                    ProfitMargin = workItemPerProject.ProfitMargin,
                    ParentId = workItemPerProject.ParentId,
                    OrgBranchName = workItemPerProject.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = workItemPerProject.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = workItemPerProject.ModifiedBy,
                    ProjectInformationId = ProjectId,
                    WorkCategoryId = workItemPerProject.WorkCategoryId
                };
                entity.TotalUnitCost = entity.LabourHourlyOutPut > 0 ?
                                                (entity.DirectCost + entity.OverHeadCost + entity.ProfitCost) / entity.LabourHourlyOutPut :
                                                entity.DirectCost + entity.OverHeadCost + entity.ProfitCost;
                db.WorkItemPerProject.Add(entity);
                db.SaveChanges();
                workItemPerProject.WorkItemPerProjectId = entity.WorkItemPerProjectId;
                ProjectListController.UpdateProjectTotalCost(workItemPerProject.WorkItemPerProjectId);
            }

            return Json(new[] { workItemPerProject }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WorkItemPerProject_Update([DataSourceRequest]DataSourceRequest request, WorkItemPerProject workItemPerProject)
        {
            if (ModelState.IsValid)
            {
                var entity = new WorkItemPerProject
                {
                    WorkItemPerProjectId = workItemPerProject.WorkItemPerProjectId,
                    ProjectEstimationWorkItemId = workItemPerProject.ProjectEstimationWorkItemId,
                    IsSubCategory = workItemPerProject.IsSubCategory,
                    ItemCode = workItemPerProject.ItemCode,
                    ItemName = string.IsNullOrEmpty(workItemPerProject.ItemName) ?
                        GetEstimationWorkItem(workItemPerProject.ProjectEstimationWorkItemId) :
                        workItemPerProject.ItemName,
                    BlockId = workItemPerProject.BlockId,

                    TotalQuantityOfWork = workItemPerProject.TotalQuantityOfWork,
                    DirectCost = workItemPerProject.DirectCost,
                    MaterialCost = workItemPerProject.MaterialCost,
                    LabourOutPut = workItemPerProject.LabourOutPut,
                    EquipmentOutput = workItemPerProject.EquipmentOutput,
                    MaterialCostPerUnit = workItemPerProject.MaterialCostPerUnit,
                    LaborCostPerUnit = workItemPerProject.LaborCostPerUnit,
                    EquipmentCostPerUnit = workItemPerProject.EquipmentCostPerUnit,
                    TotalUnitCost = workItemPerProject.TotalUnitCost,
                    OverHeadCost = workItemPerProject.OverHeadCost,
                    ProfitCost = workItemPerProject.ProfitCost,
                    ProjectType = workItemPerProject.ProjectType,
                    Remark = workItemPerProject.Remark,
                    LabourHourlyOutPut = workItemPerProject.LabourHourlyOutPut,
                    UnitOfMeasurementId = workItemPerProject.UnitOfMeasurementId,
                    OverheadRate = workItemPerProject.OverheadRate,
                    ProfitMargin = workItemPerProject.ProfitMargin,
                    ParentId = workItemPerProject.ParentId,
                    ProjectInformationId = workItemPerProject.ProjectInformationId,
                    WorkCategoryId = workItemPerProject.WorkCategoryId,

                    OrgBranchName = workItemPerProject.OrgBranchName,
                    DateCreated = workItemPerProject.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = workItemPerProject.CreatedBy,
                    ModifiedBy = User.Identity.Name,

                };

                entity.TotalUnitCost = entity.LabourHourlyOutPut > 0 ? (entity.DirectCost + entity.OverHeadCost + entity.ProfitCost) / entity.LabourHourlyOutPut :
                                             (entity.DirectCost + entity.OverHeadCost + entity.ProfitCost);
                db.WorkItemPerProject.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                ProjectListController.UpdateProjectTotalCost(workItemPerProject.WorkItemPerProjectId);
            }

            return Json(new[] { workItemPerProject }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WorkItemPerProject_Destroy([DataSourceRequest]DataSourceRequest request, WorkItemPerProject workItemPerProject)
        {
            if (ModelState.IsValid)
            {
                var entity = new WorkItemPerProject
                {
                    WorkItemPerProjectId = workItemPerProject.WorkItemPerProjectId,
                    ProjectEstimationWorkItemId = workItemPerProject.ProjectEstimationWorkItemId,
                    IsSubCategory = workItemPerProject.IsSubCategory,
                    ItemCode = workItemPerProject.ItemCode,

                    TotalQuantityOfWork = workItemPerProject.TotalQuantityOfWork,
                    DirectCost = workItemPerProject.DirectCost,
                    MaterialCost = workItemPerProject.MaterialCost,
                    LabourOutPut = workItemPerProject.LabourOutPut,
                    EquipmentOutput = workItemPerProject.EquipmentOutput,
                    MaterialCostPerUnit = workItemPerProject.MaterialCostPerUnit,
                    LaborCostPerUnit = workItemPerProject.LaborCostPerUnit,
                    EquipmentCostPerUnit = workItemPerProject.EquipmentCostPerUnit,
                    TotalUnitCost = workItemPerProject.TotalUnitCost,
                    OverHeadCost = workItemPerProject.OverHeadCost,
                    ProfitCost = workItemPerProject.ProfitCost,
                    ProjectType = workItemPerProject.ProjectType,
                    Remark = workItemPerProject.Remark,
                    LabourHourlyOutPut = workItemPerProject.LabourHourlyOutPut,
                    UnitOfMeasurementId = workItemPerProject.UnitOfMeasurementId,
                    OverheadRate = workItemPerProject.OverheadRate,
                    ProfitMargin = workItemPerProject.ProfitMargin,

                    ParentId = workItemPerProject.ParentId,
                    OrgBranchName = workItemPerProject.OrgBranchName,
                    DateCreated = workItemPerProject.DateCreated,
                    LastModified = workItemPerProject.LastModified,
                    CreatedBy = workItemPerProject.CreatedBy,
                    ModifiedBy = workItemPerProject.ModifiedBy,
                    ProjectInformationId = workItemPerProject.ProjectInformationId,
                    WorkCategoryId = workItemPerProject.WorkCategoryId,



                };

                db.WorkItemPerProject.Attach(entity);
                db.WorkItemPerProject.Remove(entity);
                db.SaveChanges();
                ProjectListController.UpdateProjectTotalCost(workItemPerProject.WorkItemPerProjectId);
            }

            return Json(new[] { workItemPerProject }.ToDataSourceResult(request, ModelState));
        }
        #region Methods

        // 'string withTake' was the third parameter here .....
        public ActionResult CopyEstimation(int project, int type)
        {
            var projectType = db.ProjectInformation.Where(x => x.ProjectInformationId == project).FirstOrDefault();
            if (projectType != null)
            {
                type = (int)projectType.ProjectType;
            }
            var estimationWorkItems = db.ProjectEstimationWorkItem.Where(x => x.ProjectType == (AvailableProjectTypes)type).ToList();
            var estimationPieceRateWorkItems =
                db.ProjectEstimationAreaWorkItem.Where(x => x.ProjectType == (AvailableProjectTypes)type).ToList();
            var estimationSubContractWorkItems =
                db.EstimationSubContractWorkItem.Where(x => x.ProjectType == (AvailableProjectTypes)type).ToList();
            var estimationCustomWorkItems = db.ProjectEstimationCustomWorkItems.Where(cw => cw.ProjectType == (AvailableProjectTypes)type)
                .ToList();
            WorkItemPerProject workItemPerProject = new WorkItemPerProject();
            AreaWorkItem areaWorkItem = new AreaWorkItem();
            SubContractWorkItem subContractWorkItem = new SubContractWorkItem();
            ProjectCustomWorkItem customWorkItem = new ProjectCustomWorkItem();
            var blocks = db.Blocks.Where(x => x.ProjectInformationId == project).ToList();


            var existingWorkItems =
                db.WorkItemPerProject.Where(x => x.ProjectInformationId == project && x.ProjectType == (AvailableProjectTypes)type).ToList();
            var existingAreaWorkItems = db.AreaWorkItemes.Where(x => x.ProjectInformationId == project).ToList();
            var existingSubContractWorkItems = db.SubContractWorkItem.Where(x => x.ProjectInformationId == project).ToList();
            var existingCustomWorkItems = db.ProjectCustomWorkItems.Where(cw => cw.ProjectInformationId == project)
                .ToList();
            #region Remove Copied data
            if (existingWorkItems.Any())
            {
                db.WorkItemPerProject.RemoveRange(existingWorkItems);
                db.SaveChanges();
            }
            if (existingAreaWorkItems.Any())
            {
                db.AreaWorkItemes.RemoveRange(existingAreaWorkItems);
                db.SaveChanges();
            }

            if (existingSubContractWorkItems.Any())
            {
                db.SubContractWorkItem.RemoveRange(existingSubContractWorkItems);
                db.SaveChanges();

            }
            if (existingCustomWorkItems.Any())
            {
                db.ProjectCustomWorkItems.RemoveRange(existingCustomWorkItems);
                db.SaveChanges();
            }
            #endregion
            if (estimationWorkItems.Any() || estimationPieceRateWorkItems.Any() || estimationSubContractWorkItems.Any() || estimationCustomWorkItems.Any())
            {
                if (blocks.Any())
                {
                    #region With block

                    #region piece Work Item

                    foreach (var b in blocks)
                    {
                        var blockMapping = db.BlockMappings.Where(x => x.BlockId == b.BlockId);
                        foreach (var bm in blockMapping)
                        {
                            foreach (var p in estimationPieceRateWorkItems)
                            {

                                if (bm.ConfigurationId == p.WorkCategory.ConfigurationId)
                                {
                                    areaWorkItem = new AreaWorkItem();
                                    areaWorkItem.WorkItemName = p.WorkItemName;
                                    areaWorkItem.ItemCode = p.ItemCode;
                                    areaWorkItem.Width = p.Width;
                                    areaWorkItem.Height = p.Height;
                                    areaWorkItem.Area = p.Area;
                                    areaWorkItem.CylinderLock = p.CylinderLock;
                                    areaWorkItem.DirectCost = p.DirectCost;
                                    areaWorkItem.MaterialCost = p.MaterialCost;
                                    areaWorkItem.OverHeadProfitCost = p.OverHeadProfitCost;
                                    areaWorkItem.Price = p.Price;
                                    areaWorkItem.WorkCategoryId = p.WorkCategoryId;
                                    areaWorkItem.ProjectInformationId = project;
                                    areaWorkItem.BlockId = b.BlockId;
                                    ExceedDbContext WorkItemdb = new ExceedDbContext();
                                    WorkItemdb.AreaWorkItemes.Add(areaWorkItem);
                                    WorkItemdb.SaveChanges();
                                    areaWorkItem.AreaWorkItemId = areaWorkItem.AreaWorkItemId;
                                }
                            }
                        }
                    }

                    #endregion


                    #region Custom Work Items

                    foreach (var b in blocks)
                    {
                        var blockMapping = db.BlockMappings.Where(x => x.BlockId == b.BlockId);
                        foreach (var bm in blockMapping)
                        {
                            foreach (var cw in estimationCustomWorkItems)
                            {
                                if (bm.ConfigurationId == ProjectEstimationHelper.GetCustomWorkItemCategory(cw.WorkCategoryId).ConfigurationId)
                                {
                                    customWorkItem = new ProjectCustomWorkItem();
                                    customWorkItem.WorkCategoryId = cw.WorkCategoryId;
                                    customWorkItem.WorkItemName = cw.WorkItemName;
                                    customWorkItem.ItemCode = cw.ItemCode;

                                    customWorkItem.TotalQuantityOfWork = cw.TotalQuantityOfWork;
                                    customWorkItem.HourlyOutPut = cw.HourlyOutPut;
                                    customWorkItem.UnitOfMeasurementId = cw.UnitOfMeasurementId;
                                    customWorkItem.ProfitRate = cw.ProfitRate;
                                    customWorkItem.OverheadRate = cw.OverheadRate;
                                    customWorkItem.ProfitCost = cw.ProfitCost;
                                    customWorkItem.OverHeadCost = cw.OverHeadCost;
                                    customWorkItem.MaterialCostPerUnit = cw.MaterialCostPerUnit;
                                    customWorkItem.LaborCostPerUnit = cw.LaborCostPerUnit;
                                    customWorkItem.EquipmentCostPerUnit = cw.EquipmentCostPerUnit;
                                    customWorkItem.DirectCost = cw.DirectCost;

                                    customWorkItem.ProjectInformationId = project;
                                    customWorkItem.ProjectType = cw.ProjectType;
                                    customWorkItem.TotalUnitRate = cw.TotalUnitRate;
                                    customWorkItem.BlockId = b.BlockId;
                                    customWorkItem.Quantity = 1;
                                    customWorkItem.TotalCost = cw.TotalUnitRate;
                                    customWorkItem.Remark = cw.Remark;

                                    ExceedDbContext customWorkItemDb = new ExceedDbContext();
                                    customWorkItemDb.ProjectCustomWorkItems.Add(customWorkItem);
                                    customWorkItemDb.SaveChanges();
                                }
                            }
                        }
                    }

                    #endregion


                    #region Sub contract Work Item

                    foreach (var b in blocks)
                    {
                        var blockMapping = db.BlockMappings.Where(x => x.BlockId == b.BlockId);
                        foreach (var bm in blockMapping)
                        {
                            //foreach (var p in estimationPieceRateWorkItems)
                            //{
                            foreach (var s in estimationSubContractWorkItems)
                            {
                                if (bm.ConfigurationId == s.WorkCategory.ConfigurationId)
                                {

                                    subContractWorkItem = new SubContractWorkItem();
                                    subContractWorkItem.UnitOfMeasurementId = s.UnitOfMeasurementId;
                                    subContractWorkItem.OverHeadProfit = s.OverHeadProfit;
                                    subContractWorkItem.Quntity = s.Quntity;
                                    subContractWorkItem.SelectedCost = s.SelectedCost;
                                    subContractWorkItem.SubContractCostOne = s.SubContractCostOne;
                                    subContractWorkItem.SubContractCostTwo = s.SubContractCostTwo;
                                    subContractWorkItem.SubContractCostThree = s.SubContractCostThree;
                                    subContractWorkItem.SubContractWorkItemId = s.EstimationSubContractWorkItemId;
                                    subContractWorkItem.TotalCost = s.TotalCost;
                                    subContractWorkItem.WorkCategoryId = s.WorkCategoryId;
                                    subContractWorkItem.WorkItemName = s.WorkItemName;
                                    subContractWorkItem.ProjectInformationId = project;
                                    subContractWorkItem.BlockId = b.BlockId;
                                    subContractWorkItem.ItemCode = s.ItemCode;
                                    ExceedDbContext WorkItemdb = new ExceedDbContext();
                                    WorkItemdb.SubContractWorkItem.Add(subContractWorkItem);
                                    WorkItemdb.SaveChanges();
                                    subContractWorkItem.SubContractWorkItemId =
                                        subContractWorkItem.SubContractWorkItemId;
                                }
                            }
                            //}
                        }
                    }

                    #endregion

                    #region Work item

                    foreach (var b in blocks)
                    {
                        var blockMapping = db.BlockMappings.Where(x => x.BlockId == b.BlockId);
                        foreach (var bm in blockMapping)
                        {
                            //foreach (var p in estimationPieceRateWorkItems)
                            //{
                            foreach (var p in estimationWorkItems)
                            {

                                if (bm.ConfigurationId == p.WorkCategory.ConfigurationId)
                                {
                                    //foreach (var e in estimationWorkItems)
                                    //{
                                    workItemPerProject = new WorkItemPerProject();
                                    workItemPerProject.ProjectInformationId = project;

                                    workItemPerProject.IsSubCategory = p.IsSubCategory;
                                    workItemPerProject.ItemCode = p.ItemCode;
                                    workItemPerProject.ItemName = p.ItemName;

                                    workItemPerProject.ProjectEstimationWorkItemId = p.ProjectEstimationWorkItemId;
                                    workItemPerProject.TotalQuantityOfWork = p.TotalQuantityOfWork;
                                    workItemPerProject.DirectCost = p.DirectCost;
                                    workItemPerProject.MaterialCost = p.MaterialCost;
                                    workItemPerProject.LabourOutPut = p.LabourOutPut;
                                    workItemPerProject.EquipmentOutput = p.EquipmentOutput;
                                    workItemPerProject.MaterialCostPerUnit = p.MaterialCostPerUnit;
                                    workItemPerProject.LaborCostPerUnit = p.LaborCostPerUnit;
                                    workItemPerProject.EquipmentCostPerUnit = p.EquipmentCostPerUnit;
                                    workItemPerProject.TotalUnitCost = p.TotalUnitCost;
                                    workItemPerProject.OverHeadCost = p.OverHeadCost;
                                    workItemPerProject.ProfitCost = p.ProfitCost;
                                    workItemPerProject.ProjectType = p.ProjectType;
                                    workItemPerProject.Remark = p.Remark;
                                    workItemPerProject.BlockId = b.BlockId;
                                    workItemPerProject.LabourHourlyOutPut = p.LabourHourlyOutPut;
                                    workItemPerProject.UnitOfMeasurementId = p.UnitOfMeasurementId ?? 0;
                                    workItemPerProject.WorkCategoryId = p.WorkCategoryId;
                                    workItemPerProject.OverheadRate = p.OverheadRate;
                                    workItemPerProject.ProfitMargin = p.ProfitMargin;
                                    workItemPerProject.ParentId = p.ParentId;
                                    workItemPerProject.OrgBranchName = p.OrgBranchName;
                                    workItemPerProject.LastModified = null;
                                    workItemPerProject.ModifiedBy = null;
                                    workItemPerProject.CreatedBy = User.Identity.Name;
                                    workItemPerProject.DateCreated = DateTime.Now;
                                    ExceedDbContext WorkItemdb = new ExceedDbContext();
                                    WorkItemdb.WorkItemPerProject.Add(workItemPerProject);
                                    WorkItemdb.SaveChanges();
                                    workItemPerProject.WorkItemPerProjectId =
                                        workItemPerProject.WorkItemPerProjectId;



                                    #region Copy Material

                                    var etsmnMaterial =
                                        db.ProjectEstimationMaterial.Where(
                                            x =>
                                                x.ProjectEstimationWorkItemId ==
                                                p.ProjectEstimationWorkItemId).ToList();
                                    if (etsmnMaterial.Any())
                                    {
                                        foreach (var m in etsmnMaterial)
                                        {
                                            var entity = new ProjectMaterial
                                            {
                                                Unit = m.Unit,
                                                Quantity = m.Quantity,
                                                ItemCategoryCode = m.ItemCategoryCode,
                                                ItemCode = m.ItemCode,
                                                UnitPrice = m.UnitPrice,
                                                TotalCost = m.TotalCost,
                                                MaterialUnitCost = m.MaterialUnitCost,
                                                OrgBranchName = m.OrgBranchName,
                                                DateCreated = DateTime.Now,
                                                LastModified = null,
                                                CreatedBy = User.Identity.Name,
                                                ModifiedBy = null,
                                                WorkItemPerProjectId =
                                                    workItemPerProject.WorkItemPerProjectId,

                                            };
                                            WorkItemdb = new ExceedDbContext();
                                            WorkItemdb.ProjectMaterial.Add(entity);
                                            WorkItemdb.SaveChanges();
                                            //projectMaterial.ProjectMaterialId = entity.ProjectMaterialId;
                                        }
                                    }

                                    #endregion

                                    #region Copy Labour Cost

                                    var etsmnLabour =
                                        db.ProjectEstimationDirectLabour.Where(
                                            x =>
                                                x.ProjectEstimationWorkItemId ==
                                                p.ProjectEstimationWorkItemId).ToList();
                                    if (etsmnLabour.Any())
                                    {
                                        foreach (var m in etsmnLabour)
                                        {
                                            var entity = new ProjectDirectLabour
                                            {
                                                Unit = m.Unit,
                                                Quantity = m.Quantity,
                                                UnitFactory = m.UnitFactory,
                                                IndexedHourlySalary = m.IndexedHourlySalary,
                                                DailyPay = m.DailyPay,
                                                TotalCost = m.TotalCost,
                                                ManpowerUnitCost = m.ManpowerUnitCost,
                                                OrgBranchName = m.OrgBranchName,
                                                DateCreated = DateTime.Now,
                                                LastModified = null,
                                                CreatedBy = User.Identity.Name,
                                                ModifiedBy = null,
                                                WorkItemPerProjectId =
                                                    workItemPerProject.WorkItemPerProjectId,
                                                ProjectEstimationIndexLabourId =
                                                    m.ProjectEstimationIndexLabourId
                                            };
                                            WorkItemdb = new ExceedDbContext();
                                            WorkItemdb.ProjectDirectLabour.Add(entity);
                                            WorkItemdb.SaveChanges();
                                            //projectDirectLabour.ProjectDirectLabourId = entity.ProjectDirectLabourId;
                                        }
                                    }

                                    #endregion

                                    #region Copy Equipment

                                    var etsmnEquipment =
                                        db.ProjectEstimationEquipment.Where(
                                            x =>
                                                x.ProjectEstimationWorkItemId ==
                                                p.ProjectEstimationWorkItemId).ToList();
                                    if (etsmnEquipment.Any())
                                    {
                                        foreach (var m in etsmnEquipment)
                                        {
                                            var entity = new ProjectEquipment
                                            {
                                                //EquipmentTypeID = m.EquipmentTypeID,
                                                //EquipmentIdentityID = m.EquipmentIdentityID,
                                                FleetsID = m.FleetsID,
                                                Unit = m.Unit,
                                                Quantity = m.Quantity,
                                                UnitFactory = m.UnitFactory,
                                                RentalCost = m.RentalCost,
                                                TotalCost = m.TotalCost,
                                                EquipmentUnitCost = m.EquipmentUnitCost,
                                                // EquipmentType = m.EquipmentType,
                                                OrgBranchName = m.OrgBranchName,
                                                DateCreated = DateTime.Now,
                                                LastModified = null,
                                                CreatedBy = User.Identity.Name,
                                                ModifiedBy = null,
                                                WorkItemPerProjectId =
                                                    workItemPerProject.WorkItemPerProjectId,

                                            };
                                            WorkItemdb = new ExceedDbContext();
                                            WorkItemdb.ProjectEquipment.Add(entity);
                                            WorkItemdb.SaveChanges();
                                            //projectEquipment.ProjectEquipmentId = entity.ProjectEquipmentId;
                                        }
                                    }

                                    #endregion

                                    #region Copy Takeoff sheet

                                    //if (withTake == "on")
                                    //{

                                    //    // the e was replaced by p starting from this current foreach starting poit to
                                    //    // the the one in  line 784
                                    //    var etsmnTakeOfSheet =
                                    //        db.ProjectTakeOff.Where(
                                    //            x =>
                                    //                x.ProjectEstimationWorkItemId ==
                                    //                p.ProjectEstimationWorkItemId &&
                                    //                x.ProjectId == type).ToList();
                                    //    TakeOffPerProject takeOffPerProject = new TakeOffPerProject();
                                    //    if (etsmnTakeOfSheet.Any())
                                    //    {
                                    //        foreach (var m in etsmnTakeOfSheet)
                                    //        {
                                    //            takeOffPerProject = new TakeOffPerProject();
                                    //            var entity = new TakeOffPerProject
                                    //            {
                                    //                TakeOffNo = m.TakeOffNo,

                                    //                WorkItemPerProjectId =
                                    //                    workItemPerProject.WorkItemPerProjectId,
                                    //                ProjectInformationId = type,
                                    //                Remark = m.Remark,
                                    //                OrgBranchName = m.OrgBranchName,
                                    //                DateCreated = DateTime.Now,
                                    //                LastModified = null,
                                    //                CreatedBy = User.Identity.Name,
                                    //                ModifiedBy = null
                                    //            };
                                    //            WorkItemdb = new ExceedDbContext();
                                    //            WorkItemdb.TakeOffPerProject.Add(entity);
                                    //            WorkItemdb.SaveChanges();
                                    //            takeOffPerProject.TakeOffPerProjectId =
                                    //                entity.TakeOffPerProjectId;

                                    //            #region Takeoff Line

                                    //            var takeoffline =
                                    //                db.ProjectTakeOffLine.Where(
                                    //                    x => x.ProjectTakeOffId == m.ProjectTakeOffId)
                                    //                    .ToList();
                                    //            if (takeoffline.Any())
                                    //            {
                                    //                foreach (var t in takeoffline)
                                    //                {
                                    //                    var entityi = new TakeOffLinePerProject
                                    //                    {
                                    //                        No = t.T,
                                    //                        D = t.D,
                                    //                        Quantity = t.S,
                                    //                        Description = t.Description,
                                    //                        UnitOfMeasurementId = t.UnitOfMeasurementId,
                                    //                        Length = t.Length,
                                    //                        Width = t.Width,
                                    //                        Depth = t.Height,
                                    //                        OrgBranchName = t.OrgBranchName,
                                    //                        DateCreated = DateTime.Now,
                                    //                        LastModified = null,
                                    //                        CreatedBy = User.Identity.Name,
                                    //                        ModifiedBy = null,
                                    //                        TakeOffLinePerProjectId =
                                    //                            takeOffPerProject.TakeOffPerProjectId
                                    //                    };
                                    //                    WorkItemdb = new ExceedDbContext();

                                    //                    WorkItemdb.TakeOffLinePerProject.Add(entityi);
                                    //                    WorkItemdb.SaveChanges();

                                    //                }
                                    //            }

                                    //            #endregion

                                    //            #region Takeoff Line per Kg

                                    //            var takeofflineKg =
                                    //                db.ProjectTakeOffLineForKg.Where(
                                    //                    x => x.ProjectTakeOffId == m.ProjectTakeOffId)
                                    //                    .ToList();
                                    //            if (takeofflineKg.Any())
                                    //            {
                                    //                foreach (var t in takeofflineKg)
                                    //                {
                                    //                    var entityy = new TakeOffLineForKgPerProject
                                    //                    {
                                    //                        Description = t.Description,
                                    //                        //UnitRateId = t.UnitRateId,
                                    //                        Type = t.Type,
                                    //                        Shape = t.Shape,
                                    //                        Length = t.Length,
                                    //                        NoOfBars = t.NoOfBars,
                                    //                        NoOfElement = t.NoOfElement,
                                    //                        TotalBar = t.TotalBar,
                                    //                        TotalLength = t.TotalLength,
                                    //                        GrandTotal = t.GrandTotal,
                                    //                        OrgBranchName = t.OrgBranchName,
                                    //                        DateCreated = DateTime.Now,
                                    //                        LastModified = null,
                                    //                        CreatedBy = User.Identity.Name,
                                    //                        ModifiedBy = null,
                                    //                        TakeOffPerProjectId =
                                    //                            takeOffPerProject.TakeOffPerProjectId
                                    //                    };
                                    //                    WorkItemdb = new ExceedDbContext();
                                    //                    WorkItemdb.TakeOffLineForKgPerProject.Add(entityy);
                                    //                    WorkItemdb.SaveChanges();

                                    //                }
                                    //            }

                                    //            #endregion
                                    //        }
                                    //    }



                                    //}
                                    #endregion
                                    //}
                                }
                            }
                        }

                    }
                    #endregion
                    #endregion
                }
                #region copying work items when no block is defined
                //else
                //{
                //    #region WithOut block
                //    foreach (var p in estimationPieceRateWorkItems)
                //    {
                //        areaWorkItem = new AreaWorkItem();
                //        areaWorkItem.Width = p.Width;
                //        areaWorkItem.Height = p.Height;
                //        areaWorkItem.Area = p.Area;
                //        areaWorkItem.CylinderLock = p.CylinderLock;
                //        areaWorkItem.DirectCost = p.DirectCost;
                //        areaWorkItem.MaterialCost = p.MaterialCost;
                //        areaWorkItem.OverHeadProfitCost = p.OverHeadProfitCost;
                //        areaWorkItem.Price = p.Price;
                //        areaWorkItem.WorkCategoryId = p.WorkCategoryId;
                //        areaWorkItem.ProjectInformationId = project;
                //        ExceedDbContext WorkItemdb = new ExceedDbContext();
                //        WorkItemdb.AreaWorkItemes.Add(areaWorkItem);
                //        WorkItemdb.SaveChanges();
                //        areaWorkItem.AreaWorkItemId = areaWorkItem.AreaWorkItemId;
                //    }
                //    foreach(var cw in estimationCustomWorkItems)
                //    {
                //        customWorkItem = new ProjectCustomWorkItem();
                //        customWorkItem.WorkCategoryId = cw.WorkCategoryId;
                //        customWorkItem.WorkItemName = cw.WorkItemName;
                //        customWorkItem.ItemCode = cw.ItemCode;
                //        customWorkItem.ProjectInformationId = project;
                //        customWorkItem.ProjectType = cw.ProjectType;
                //        customWorkItem.TotalUnitRate = cw.TotalUnitRate;
                //        //customWorkItem.BlockId = b.BlockId;
                //        customWorkItem.Quantity = 1;
                //        customWorkItem.TotalCost = cw.TotalUnitRate;
                //        customWorkItem.Remark = cw.Remark;

                //        ExceedDbContext customWorkItemDb = new ExceedDbContext();
                //        customWorkItemDb.ProjectCustomWorkItems.Add(customWorkItem);
                //        customWorkItemDb.SaveChanges();
                //    }
                //    foreach (var s in estimationSubContractWorkItems)
                //    {
                //        subContractWorkItem = new SubContractWorkItem();
                //        subContractWorkItem.UnitOfMeasurementId = s.UnitOfMeasurementId;
                //        subContractWorkItem.OverHeadProfit = s.OverHeadProfit;
                //        subContractWorkItem.Quntity = s.Quntity;
                //        subContractWorkItem.SelectedCost = s.SelectedCost;
                //        subContractWorkItem.SubContractCostOne = s.SubContractCostOne;
                //        subContractWorkItem.SubContractCostTwo = s.SubContractCostTwo;
                //        subContractWorkItem.SubContractCostThree = s.SubContractCostThree;
                //        subContractWorkItem.SubContractWorkItemId = s.EstimationSubContractWorkItemId;
                //        subContractWorkItem.TotalCost = s.TotalCost;
                //        subContractWorkItem.WorkCategoryId = s.WorkCategoryId;
                //        subContractWorkItem.WorkItemName = s.WorkItemName;
                //        subContractWorkItem.ProjectInformationId = project;
                //        ExceedDbContext WorkItemdb = new ExceedDbContext();
                //        WorkItemdb.SubContractWorkItem.Add(subContractWorkItem);
                //        WorkItemdb.SaveChanges();
                //        subContractWorkItem.SubContractWorkItemId = subContractWorkItem.SubContractWorkItemId;
                //    }
                //    foreach (var e in estimationWorkItems)
                //    {
                //        workItemPerProject = new WorkItemPerProject();
                //        workItemPerProject.ProjectInformationId = project;
                //        workItemPerProject.ProjectEstimationWorkItemId = e.ProjectEstimationWorkItemId;
                //        workItemPerProject.IsSubCategory = e.IsSubCategory;
                //        workItemPerProject.ItemCode = e.ItemCode;
                //        workItemPerProject.TotalQuantityOfWork = e.TotalQuantityOfWork;
                //        workItemPerProject.DirectCost = e.DirectCost;
                //        workItemPerProject.MaterialCost = e.MaterialCost;
                //        workItemPerProject.LabourOutPut = e.LabourOutPut;
                //        workItemPerProject.EquipmentOutput = e.EquipmentOutput;
                //        workItemPerProject.TotalUnitCost = e.TotalUnitCost;
                //        workItemPerProject.OverHeadCost = e.OverHeadCost;
                //        workItemPerProject.ProfitCost = e.ProfitCost;
                //        workItemPerProject.ProjectType = e.ProjectType;
                //        workItemPerProject.Remark = e.Remark;

                //        workItemPerProject.LabourHourlyOutPut = e.LabourHourlyOutPut;
                //        workItemPerProject.UnitOfMeasurementId = e.UnitOfMeasurementId ?? 0;
                //        workItemPerProject.WorkCategoryId = e.WorkCategoryId;
                //        workItemPerProject.OverheadRate = e.OverheadRate;
                //        workItemPerProject.ProfitMargin = e.ProfitMargin;
                //        workItemPerProject.ParentId = e.ParentId;
                //        workItemPerProject.OrgBranchName = e.OrgBranchName;
                //        workItemPerProject.LastModified = null;
                //        workItemPerProject.ModifiedBy = null;
                //        workItemPerProject.CreatedBy = User.Identity.Name;
                //        workItemPerProject.DateCreated = DateTime.Now;
                //        ExceedDbContext WorkItemdb = new ExceedDbContext();
                //        WorkItemdb.WorkItemPerProject.Add(workItemPerProject);
                //        WorkItemdb.SaveChanges();
                //        workItemPerProject.WorkItemPerProjectId = workItemPerProject.WorkItemPerProjectId;

                //        #region Copy Material

                //        var etsmnMaterial =
                //            db.ProjectEstimationMaterial.Where(
                //                x => x.ProjectEstimationWorkItemId == e.ProjectEstimationWorkItemId).ToList();
                //        if (etsmnMaterial.Any())
                //        {
                //            foreach (var m in etsmnMaterial)
                //            {
                //                var entity = new ProjectMaterial
                //                {
                //                    Unit = m.Unit,
                //                    Quantity = m.Quantity,
                //                    ItemCategoryCode = m.ItemCategoryCode,
                //                    ItemCode = m.ItemCode,
                //                    UnitPrice = m.UnitPrice,
                //                    TotalCost = m.TotalCost,
                //                    MaterialUnitCost = m.MaterialUnitCost,
                //                    OrgBranchName = m.OrgBranchName,
                //                    DateCreated = DateTime.Now,
                //                    LastModified = null,
                //                    CreatedBy = User.Identity.Name,
                //                    ModifiedBy = null,
                //                    WorkItemPerProjectId = workItemPerProject.WorkItemPerProjectId,

                //                };
                //                WorkItemdb = new ExceedDbContext();
                //                WorkItemdb.ProjectMaterial.Add(entity);
                //                WorkItemdb.SaveChanges();
                //                //projectMaterial.ProjectMaterialId = entity.ProjectMaterialId;
                //            }
                //        }

                //        #endregion

                //        #region Copy Labour Cost

                //        var etsmnLabour =
                //            db.ProjectEstimationDirectLabour.Where(
                //                x => x.ProjectEstimationWorkItemId == e.ProjectEstimationWorkItemId).ToList();
                //        if (etsmnLabour.Any())
                //        {
                //            foreach (var m in etsmnLabour)
                //            {
                //                var entity = new ProjectDirectLabour
                //                {
                //                    Unit = m.Unit,
                //                    Quantity = m.Quantity,
                //                    UnitFactory = m.UnitFactory,
                //                    IndexedHourlySalary = m.IndexedHourlySalary,
                //                    DailyPay = m.DailyPay,
                //                    TotalCost = m.TotalCost,
                //                    ManpowerUnitCost = m.ManpowerUnitCost,
                //                    OrgBranchName = m.OrgBranchName,
                //                    DateCreated = DateTime.Now,
                //                    LastModified = null,
                //                    CreatedBy = User.Identity.Name,
                //                    ModifiedBy = null,
                //                    WorkItemPerProjectId = workItemPerProject.WorkItemPerProjectId,
                //                    ProjectEstimationIndexLabourId = m.ProjectEstimationIndexLabourId
                //                };
                //                WorkItemdb = new ExceedDbContext();
                //                WorkItemdb.ProjectDirectLabour.Add(entity);
                //                WorkItemdb.SaveChanges();
                //                //projectDirectLabour.ProjectDirectLabourId = entity.ProjectDirectLabourId;
                //            }
                //        }

                //        #endregion

                //        #region Copy Equipment

                //        var etsmnEquipment =
                //            db.ProjectEstimationEquipment.Where(
                //                x => x.ProjectEstimationWorkItemId == e.ProjectEstimationWorkItemId).ToList();
                //        if (etsmnEquipment.Any())
                //        {
                //            foreach (var m in etsmnEquipment)
                //            {
                //                var entity = new ProjectEquipment
                //                {
                //                    //EquipmentTypeID = m.EquipmentTypeID,
                //                    //EquipmentIdentityID = m.EquipmentIdentityID,
                //                    FleetsID = m.FleetsID,
                //                    Unit = m.Unit,
                //                    Quantity = m.Quantity,
                //                    UnitFactory = m.UnitFactory,
                //                    RentalCost = m.RentalCost,
                //                    TotalCost = m.TotalCost,
                //                    EquipmentUnitCost = m.EquipmentUnitCost,
                //                    EquipmentType = m.EquipmentType,
                //                    OrgBranchName = m.OrgBranchName,
                //                    DateCreated = DateTime.Now,
                //                    LastModified = null,
                //                    CreatedBy = User.Identity.Name,
                //                    ModifiedBy = null,
                //                    WorkItemPerProjectId = workItemPerProject.WorkItemPerProjectId,

                //                };

                //                db.ProjectEquipment.Add(entity);
                //                db.SaveChanges();
                //                //projectEquipment.ProjectEquipmentId = entity.ProjectEquipmentId;
                //            }
                //        }

                //        #endregion

                //        #region target for removal
                //        //if (withTake == "on")
                //        //{
                //        //    #region Copy Takeoff sheet

                //        //    var etsmnTakeOfSheet =
                //        //        db.ProjectTakeOff.Where(
                //        //            x =>
                //        //                x.ProjectEstimationWorkItemId == e.ProjectEstimationWorkItemId &&
                //        //                x.ProjectId == type).ToList();
                //        //    TakeOffPerProject takeOffPerProject = new TakeOffPerProject();
                //        //    if (etsmnTakeOfSheet.Any())
                //        //    {
                //        //        foreach (var m in etsmnTakeOfSheet)
                //        //        {
                //        //            takeOffPerProject = new TakeOffPerProject();
                //        //            var entity = new TakeOffPerProject
                //        //            {
                //        //                TakeOffNo = m.TakeOffNo,

                //        //                WorkItemPerProjectId = workItemPerProject.WorkItemPerProjectId,
                //        //                ProjectInformationId = type,
                //        //                Remark = m.Remark,
                //        //                OrgBranchName = m.OrgBranchName,
                //        //                DateCreated = DateTime.Now,
                //        //                LastModified = null,
                //        //                CreatedBy = User.Identity.Name,
                //        //                ModifiedBy = null
                //        //            };
                //        //            WorkItemdb = new ExceedDbContext();
                //        //            WorkItemdb.TakeOffPerProject.Add(entity);
                //        //            WorkItemdb.SaveChanges();
                //        //            takeOffPerProject.TakeOffPerProjectId = entity.TakeOffPerProjectId;

                //        //            #region Takeoff Line

                //        //            var takeoffline =
                //        //                db.ProjectTakeOffLine.Where(x => x.ProjectTakeOffId == m.ProjectTakeOffId)
                //        //                    .ToList();
                //        //            if (takeoffline.Any())
                //        //            {
                //        //                foreach (var t in takeoffline)
                //        //                {
                //        //                    var entityi = new TakeOffLinePerProject
                //        //                    {
                //        //                        No = t.T,
                //        //                        D = t.D,
                //        //                        Quantity = t.S,
                //        //                        Description = t.Description,
                //        //                        UnitOfMeasurementId = t.UnitOfMeasurementId,
                //        //                        Length = t.Length,
                //        //                        Width = t.Width,
                //        //                        Depth = t.Height,
                //        //                        OrgBranchName = t.OrgBranchName,
                //        //                        DateCreated = DateTime.Now,
                //        //                        LastModified = null,
                //        //                        CreatedBy = User.Identity.Name,
                //        //                        ModifiedBy = null,
                //        //                        TakeOffLinePerProjectId = takeOffPerProject.TakeOffPerProjectId
                //        //                    };
                //        //                    WorkItemdb = new ExceedDbContext();

                //        //                    WorkItemdb.TakeOffLinePerProject.Add(entityi);
                //        //                    WorkItemdb.SaveChanges();

                //        //                }
                //        //            }

                //        //            #endregion

                //        //            #region Takeoff Line per Kg

                //        //            var takeofflineKg =
                //        //                db.ProjectTakeOffLineForKg.Where(x => x.ProjectTakeOffId == m.ProjectTakeOffId)
                //        //                    .ToList();
                //        //            if (takeofflineKg.Any())
                //        //            {
                //        //                foreach (var t in takeofflineKg)
                //        //                {
                //        //                    var entityy = new TakeOffLineForKgPerProject
                //        //                    {
                //        //                        Description = t.Description,
                //        //                        //UnitRateId = t.UnitRateId,
                //        //                        Type = t.Type,
                //        //                        Shape = t.Shape,
                //        //                        Length = t.Length,
                //        //                        NoOfBars = t.NoOfBars,
                //        //                        NoOfElement = t.NoOfElement,
                //        //                        TotalBar = t.TotalBar,
                //        //                        TotalLength = t.TotalLength,
                //        //                        GrandTotal = t.GrandTotal,
                //        //                        OrgBranchName = t.OrgBranchName,
                //        //                        DateCreated = DateTime.Now,
                //        //                        LastModified = null,
                //        //                        CreatedBy = User.Identity.Name,
                //        //                        ModifiedBy = null,
                //        //                        TakeOffPerProjectId = takeOffPerProject.TakeOffPerProjectId
                //        //                    };
                //        //                    WorkItemdb = new ExceedDbContext();
                //        //                    WorkItemdb.TakeOffLineForKgPerProject.Add(entityy);
                //        //                    WorkItemdb.SaveChanges();

                //        //                }
                //        //            }

                //        //            #endregion
                //        //        }
                //        //    }

                //        //    #endregion
                //        //} 
                //        #endregion

                //    }
                //    #endregion
                //} 
                #endregion
                // 
                //  summary:
                //      after copying the work items from the template,
                //      update work items that are sub-work items 
                this.UpdateWorkItemsHeirarchy(project);
            }
            else
            {
                return new JavaScriptResult() { Script = "alert('" + "No existing estimated data was found for this type of project!!" + "');" };
            }
            project = 0;
            ProjectId = 0;
            return new JavaScriptResult() { Script = "alert('" + "Data Copied Successfully!!" + "');" };
        }

        #endregion

        private void UpdateWorkItemsHeirarchy(int projectId)
        {
            var workItemsInProject = db.WorkItemPerProject.Where(wi => wi.ProjectInformationId == projectId && wi.ParentId != 0).ToList();
            if (workItemsInProject.Any())
            {
                foreach (var workItem in workItemsInProject)
                {
                    var workItemParent = db.WorkItemPerProject.FirstOrDefault(w => w.ProjectInformationId == projectId && w.BlockId == workItem.BlockId && w.ProjectEstimationWorkItemId == workItem.ParentId);

                    if (workItemParent != null)
                    {
                        workItem.ParentId = workItemParent.WorkItemPerProjectId;
                        db.WorkItemPerProject.Attach(workItem);
                        db.Entry(workItem).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
        }
        private string GetEstimationWorkItem(int id)
        {
            ProjectEstimationWorkItem workItem = new ProjectEstimationWorkItem();
            var model = db.ProjectEstimationWorkItem.Find(id);
            if (model != null)
            {
                workItem = model;
            }
            return workItem.ItemName;
            //return "temporary name";
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}