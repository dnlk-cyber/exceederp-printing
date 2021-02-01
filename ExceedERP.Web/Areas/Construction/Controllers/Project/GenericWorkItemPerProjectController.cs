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
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Project
{
    [AuthorizeRoles(ConstructionConstants.EstimationUser)]
    public class GenericWorkItemPerProjectController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        //public static string CurrentUSer { get; set; }
        //public static int Category { get; set; }
        public static string OverHeadCost { get; set; }
        public static string ProfitMargin { get; set; }

        private readonly ConstructionInputs inputs;
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

            //CurrentUSer = User.Identity.Name;
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
        public ActionResult GenericWorkItemPerProject_Read([DataSourceRequest]DataSourceRequest request)
        {
            db.Configuration.ProxyCreationEnabled = false;

            IQueryable<GenericWorkItemPerProject> workItems = db.GenericWorkItemPerProject.Where(x => x.ParentId == 0);

            DataSourceResult result = workItems.ToDataSourceResult(request, workItemPerProject => new
            {
                GenericWorkItemPerProjectId = workItemPerProject.GenericWorkItemPerProjectId,
                ProjectEstimationWorkItemId = workItemPerProject.ProjectEstimationWorkItemId,
                IsSubCategory = workItemPerProject.IsSubCategory,
                ItemCode = workItemPerProject.ItemCode,
                ContainsWorkItemAsInput = workItemPerProject.ContainsWorkItemAsInput,
                SubProjectTypeId = workItemPerProject.SubProjectTypeId,
                ProjectInformation = workItemPerProject.ProjectInformation,

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
                TransportCostRate = workItemPerProject.TransportCostRate,
                HasAdditionalTransportationCost = workItemPerProject.HasAdditionalTransportationCost,
                CalculatedTransportCost = workItemPerProject.CalculatedTransportCost,
                TotalUnitCostIncludingTransport = workItemPerProject.TotalUnitCostIncludingTransport,
                OverHeadCost = workItemPerProject.OverHeadCost,
                ProfitCost = workItemPerProject.ProfitCost,
                Remark = workItemPerProject.Remark,
                LabourHourlyOutPut = workItemPerProject.LabourHourlyOutPut,
                UnitOfMeasurementId = workItemPerProject.UnitOfMeasurementId,
                OverheadRate = workItemPerProject.OverheadRate,
                ProfitMargin = workItemPerProject.ProfitMargin,

                IsSupplementaryWorkItem = workItemPerProject.IsSupplementaryWorkItem,
                IsVariationWorkItem = workItemPerProject.IsVariationWorkItem,
                AddedQuantityOfWork = workItemPerProject.AddedQuantityOfWork,
                OmittedQuantityOfWork = workItemPerProject.OmittedQuantityOfWork,

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

            return Json(result);
        }

        public ActionResult GenericWorkItemUnderWorkItem_Read([DataSourceRequest]DataSourceRequest request, int parentWorkItemId)
        {
            //var currentWorkItem = db.GenericWorkItemPerProject.Find(parentWorkItemId);
            //if (currentWorkItem != null)
            //{

            //}
            IQueryable<GenericWorkItemPerProject> workItems = db.GenericWorkItemPerProject.Where(x => x.ParentId == parentWorkItemId);
            DataSourceResult result = workItems.ToDataSourceResult(request, workItemPerProject => new
            {
                GenericWorkItemPerProjectId = workItemPerProject.GenericWorkItemPerProjectId,
                ProjectEstimationWorkItemId = workItemPerProject.ProjectEstimationWorkItemId,
                IsSubCategory = workItemPerProject.IsSubCategory,
                ItemCode = workItemPerProject.ItemCode,
                ContainsWorkItemAsInput = workItemPerProject.ContainsWorkItemAsInput,
                SubProjectTypeId = workItemPerProject.SubProjectTypeId,
                //ProjectInformation = workItemPerProject.ProjectInformation,

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
                TransportCostRate = workItemPerProject.TransportCostRate,
                HasAdditionalTransportationCost = workItemPerProject.HasAdditionalTransportationCost,
                CalculatedTransportCost = workItemPerProject.CalculatedTransportCost,
                TotalUnitCostIncludingTransport = workItemPerProject.TotalUnitCostIncludingTransport,
                OverHeadCost = workItemPerProject.OverHeadCost,
                ProfitCost = workItemPerProject.ProfitCost,
                Remark = workItemPerProject.Remark,
                LabourHourlyOutPut = workItemPerProject.LabourHourlyOutPut,
                UnitOfMeasurementId = workItemPerProject.UnitOfMeasurementId,
                OverheadRate = workItemPerProject.OverheadRate,
                ProfitMargin = workItemPerProject.ProfitMargin,

                IsSupplementaryWorkItem = workItemPerProject.IsSupplementaryWorkItem,
                IsVariationWorkItem = workItemPerProject.IsVariationWorkItem,
                AddedQuantityOfWork = workItemPerProject.AddedQuantityOfWork,
                OmittedQuantityOfWork = workItemPerProject.OmittedQuantityOfWork,

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



            return Json(result);
        }
        // work items per a work category
        public ActionResult GenericWorkItemPerCategory_Read([DataSourceRequest]DataSourceRequest request, int categoryId)
        {
            IQueryable<GenericWorkItemPerProject> workitemperproject = db.GenericWorkItemPerProject.Where(x => x.WorkCategoryId == categoryId);
            DataSourceResult result = workitemperproject.ToDataSourceResult(request, workItemPerProject => new
            {
                GenericWorkItemPerProjectId = workItemPerProject.GenericWorkItemPerProjectId,
                ProjectEstimationWorkItemId = workItemPerProject.ProjectEstimationWorkItemId,
                IsSubCategory = workItemPerProject.IsSubCategory,
                ItemCode = workItemPerProject.ItemCode,
                ContainsWorkItemAsInput = workItemPerProject.ContainsWorkItemAsInput,
                SubProjectTypeId = workItemPerProject.SubProjectTypeId,
                //ProjectInformation = workItemPerProject.ProjectInformation,

                TotalQuantityOfWork = workItemPerProject.TotalQuantityOfWork,
                DirectCost = workItemPerProject.DirectCost,
                MaterialCost = workItemPerProject.MaterialCost,
                LabourOutPut = workItemPerProject.LabourOutPut,
                EquipmentOutput = workItemPerProject.EquipmentOutput,
                MaterialCostPerUnit = workItemPerProject.MaterialCostPerUnit,
                LaborCostPerUnit = workItemPerProject.LaborCostPerUnit,
                EquipmentCostPerUnit = workItemPerProject.EquipmentCostPerUnit,
                TotalUnitCost = workItemPerProject.TotalUnitCost,
                TransportCostRate = workItemPerProject.TransportCostRate,
                HasAdditionalTransportationCost = workItemPerProject.HasAdditionalTransportationCost,
                CalculatedTransportCost = workItemPerProject.CalculatedTransportCost,
                TotalUnitCostIncludingTransport = workItemPerProject.TotalUnitCostIncludingTransport,
                OverHeadCost = workItemPerProject.OverHeadCost,
                ProfitCost = workItemPerProject.ProfitCost,
                Remark = workItemPerProject.Remark,
                LabourHourlyOutPut = workItemPerProject.LabourHourlyOutPut,
                UnitOfMeasurementId = workItemPerProject.UnitOfMeasurementId,
                OverheadRate = workItemPerProject.OverheadRate,
                ProfitMargin = workItemPerProject.ProfitMargin,

                IsSupplementaryWorkItem = workItemPerProject.IsSupplementaryWorkItem,
                IsVariationWorkItem = workItemPerProject.IsVariationWorkItem,
                AddedQuantityOfWork = workItemPerProject.AddedQuantityOfWork,
                OmittedQuantityOfWork = workItemPerProject.OmittedQuantityOfWork,

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
                //ProjectListController.UpdateProjectTotalCost(
                //    workitemperproject.Select(x => x.WorkItemPerProjectId).ToList());
            }

            return Json(result);
        }

        /// <summary>
        /// returns the work items under a given Structure type
        /// </summary>
        /// <param name="request"></param>
        /// <param name="strId"></param>
        /// <returns></returns>
        public ActionResult GenericWorkItemPerStructure([DataSourceRequest]DataSourceRequest request, int strId, int subProjectMappingId)
        {
            IQueryable<WorkCategory> workcategorys = db.WorkCategory.Where(wc => wc.ConfigurationId == strId);
            List<GenericWorkItemPerProject> workItems = new List<GenericWorkItemPerProject>();
            SubProjectStructureTypeMapping currentMapping = new SubProjectStructureTypeMapping();
            //var tempBlock = db.BlockMappings.Where(bm => bm.ConfigurationId == strId).FirstOrDefault();
            //if(tempBlock != null)
            //{
            //    currentBlock = tempBlock;
            //}

            SubProjectStructureTypeMapping otherOne = new SubProjectStructureTypeMapping();
            var temp2 = db.SubProjectStructureTypeMapping.Find(subProjectMappingId);
            if (temp2 != null)
            {
                otherOne = temp2;
            }

            foreach (WorkCategory category in workcategorys)
            {
                var workItemList = db.GenericWorkItemPerProject
                    .Where(wi => wi.WorkCategoryId == category.WorkCategoryId &&
                    wi.SubProjectTypeId == temp2.SubProjectTypeId && wi.ParentId == 0);
                if (workItemList != null && workItemList.Count() > 0)
                {
                    foreach (GenericWorkItemPerProject item in workItemList)
                    {
                        workItems.Add(item);
                    }
                }
            }
            IQueryable<GenericWorkItemPerProject> workItemsToReturn = workItems.AsQueryable();

            DataSourceResult result = workItemsToReturn.ToDataSourceResult(request, workItemPerProject => new
            {
                GenericWorkItemPerProjectId = workItemPerProject.GenericWorkItemPerProjectId,
                ProjectEstimationWorkItemId = workItemPerProject.ProjectEstimationWorkItemId,
                IsSubCategory = workItemPerProject.IsSubCategory,
                ItemCode = workItemPerProject.ItemCode,
                ContainsWorkItemAsInput = workItemPerProject.ContainsWorkItemAsInput,
                SubProjectTypeId = workItemPerProject.SubProjectTypeId,
                //ProjectInformation = workItemPerProject.ProjectInformation,

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
                TransportCostRate = workItemPerProject.TransportCostRate,
                HasAdditionalTransportationCost = workItemPerProject.HasAdditionalTransportationCost,
                CalculatedTransportCost = workItemPerProject.CalculatedTransportCost,
                TotalUnitCostIncludingTransport = workItemPerProject.TotalUnitCostIncludingTransport,
                OverHeadCost = workItemPerProject.OverHeadCost,
                ProfitCost = workItemPerProject.ProfitCost,
                Remark = workItemPerProject.Remark,
                LabourHourlyOutPut = workItemPerProject.LabourHourlyOutPut,
                UnitOfMeasurementId = workItemPerProject.UnitOfMeasurementId,
                OverheadRate = workItemPerProject.OverheadRate,
                ProfitMargin = workItemPerProject.ProfitMargin,

                IsSupplementaryWorkItem = workItemPerProject.IsSupplementaryWorkItem,
                IsVariationWorkItem = workItemPerProject.IsVariationWorkItem,
                AddedQuantityOfWork = workItemPerProject.AddedQuantityOfWork,
                OmittedQuantityOfWork = workItemPerProject.OmittedQuantityOfWork,

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

            return Json(result);
        }
        public ActionResult GenericWorkItemWithoutVariationPerStructure([DataSourceRequest]DataSourceRequest request, int strId, int subProjectMappingId)
        {
            IQueryable<WorkCategory> workcategorys = db.WorkCategory.Where(wc => wc.ConfigurationId == strId);
            List<GenericWorkItemPerProject> workItems = new List<GenericWorkItemPerProject>();
            SubProjectStructureTypeMapping currentMapping = new SubProjectStructureTypeMapping();
            //var tempBlock = db.BlockMappings.Where(bm => bm.ConfigurationId == strId).FirstOrDefault();
            //if(tempBlock != null)
            //{
            //    currentBlock = tempBlock;
            //}

            SubProjectStructureTypeMapping otherOne = new SubProjectStructureTypeMapping();
            var temp2 = db.SubProjectStructureTypeMapping.Find(subProjectMappingId);
            if (temp2 != null)
            {
                otherOne = temp2;
            }

            foreach (WorkCategory category in workcategorys)
            {
                var workItemList = db.GenericWorkItemPerProject
                    .Where(wi => wi.WorkCategoryId == category.WorkCategoryId &&
                    wi.SubProjectTypeId == temp2.SubProjectTypeId && wi.ParentId == 0 && !wi.IsVariationWorkItem && !wi.IsSupplementaryWorkItem);
                if (workItemList != null && workItemList.Count() > 0)
                {
                    foreach (GenericWorkItemPerProject item in workItemList)
                    {
                        workItems.Add(item);
                    }
                }
            }
            IQueryable<GenericWorkItemPerProject> workItemsToReturn = workItems.AsQueryable();

            DataSourceResult result = workItemsToReturn.ToDataSourceResult(request, workItemPerProject => new
            {
                GenericWorkItemPerProjectId = workItemPerProject.GenericWorkItemPerProjectId,
                ProjectEstimationWorkItemId = workItemPerProject.ProjectEstimationWorkItemId,
                IsSubCategory = workItemPerProject.IsSubCategory,
                ItemCode = workItemPerProject.ItemCode,
                ContainsWorkItemAsInput = workItemPerProject.ContainsWorkItemAsInput,
                SubProjectTypeId = workItemPerProject.SubProjectTypeId,
                //ProjectInformation = workItemPerProject.ProjectInformation,

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
                TransportCostRate = workItemPerProject.TransportCostRate,
                HasAdditionalTransportationCost = workItemPerProject.HasAdditionalTransportationCost,
                CalculatedTransportCost = workItemPerProject.CalculatedTransportCost,
                TotalUnitCostIncludingTransport = workItemPerProject.TotalUnitCostIncludingTransport,
                OverHeadCost = workItemPerProject.OverHeadCost,
                ProfitCost = workItemPerProject.ProfitCost,
                Remark = workItemPerProject.Remark,
                LabourHourlyOutPut = workItemPerProject.LabourHourlyOutPut,
                UnitOfMeasurementId = workItemPerProject.UnitOfMeasurementId,
                OverheadRate = workItemPerProject.OverheadRate,
                ProfitMargin = workItemPerProject.ProfitMargin,

                IsSupplementaryWorkItem = workItemPerProject.IsSupplementaryWorkItem,
                IsVariationWorkItem = workItemPerProject.IsVariationWorkItem,
                AddedQuantityOfWork = workItemPerProject.AddedQuantityOfWork,
                OmittedQuantityOfWork = workItemPerProject.OmittedQuantityOfWork,

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

            return Json(result);
        }

        public ActionResult VariationGenericWorkItemPerStructure([DataSourceRequest]DataSourceRequest request, int strId, int subProjectMappingId)
        {
            var workcategorys = db.WorkCategory.Where(wc => wc.ConfigurationId == strId).ToList();
            List<GenericWorkItemPerProject> workItems = new List<GenericWorkItemPerProject>();
            //SubProjectStructureTypeMapping currentMapping = new SubProjectStructureTypeMapping();
            //var tempBlock = db.BlockMappings.Where(bm => bm.ConfigurationId == strId).FirstOrDefault();
            //if(tempBlock != null)
            //{
            //    currentBlock = tempBlock;
            //}

            //SubProjectStructureTypeMapping otherOne = new SubProjectStructureTypeMapping();
            var subStructMapping = db.SubProjectStructureTypeMapping.Find(subProjectMappingId);

            if (subStructMapping != null)
            {
                foreach (WorkCategory category in workcategorys)
                {
                    var workItemListInCategory = db.GenericWorkItemPerProject
                        .Where(wi => wi.WorkCategoryId == category.WorkCategoryId &&
                        wi.SubProjectTypeId == subStructMapping.SubProjectTypeId && wi.ParentId == 0 &&
                        wi.IsVariationWorkItem);
                    if (workItemListInCategory.Any())
                    {

                        workItems.AddRange(workItemListInCategory);
                    }
                }
            }

            IQueryable<GenericWorkItemPerProject> workItemsToReturn = workItems.AsQueryable();

            DataSourceResult result = workItemsToReturn.ToDataSourceResult(request, workItemPerProject => new
            {
                GenericWorkItemPerProjectId = workItemPerProject.GenericWorkItemPerProjectId,
                ProjectEstimationWorkItemId = workItemPerProject.ProjectEstimationWorkItemId,
                IsSubCategory = workItemPerProject.IsSubCategory,
                ItemCode = workItemPerProject.ItemCode,
                ContainsWorkItemAsInputcn = workItemPerProject.ContainsWorkItemAsInput,
                SubProjectTypeId = workItemPerProject.SubProjectTypeId,
                //ProjectInformation = workItemPerProject.ProjectInformation,

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
                TransportCostRate = workItemPerProject.TransportCostRate,
                HasAdditionalTransportationCost = workItemPerProject.HasAdditionalTransportationCost,
                CalculatedTransportCost = workItemPerProject.CalculatedTransportCost,
                TotalUnitCostIncludingTransport = workItemPerProject.TotalUnitCostIncludingTransport,
                OverHeadCost = workItemPerProject.OverHeadCost,
                ProfitCost = workItemPerProject.ProfitCost,
                Remark = workItemPerProject.Remark,
                LabourHourlyOutPut = workItemPerProject.LabourHourlyOutPut,
                UnitOfMeasurementId = workItemPerProject.UnitOfMeasurementId,
                OverheadRate = workItemPerProject.OverheadRate,
                ProfitMargin = workItemPerProject.ProfitMargin,

                IsSupplementaryWorkItem = workItemPerProject.IsSupplementaryWorkItem,
                IsVariationWorkItem = workItemPerProject.IsVariationWorkItem,
                AddedQuantityOfWork = workItemPerProject.AddedQuantityOfWork,
                OmittedQuantityOfWork = workItemPerProject.OmittedQuantityOfWork,

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

            return Json(result);
        }
        public ActionResult SupplementaryGenericWorkItemPerStructure([DataSourceRequest]DataSourceRequest request, int strId, int subProjectMappingId)
        {
            var workcategorys = db.WorkCategory.Where(wc => wc.ConfigurationId == strId).ToList();
            List<GenericWorkItemPerProject> workItems = new List<GenericWorkItemPerProject>();

            var subStructMapping = db.SubProjectStructureTypeMapping.Find(subProjectMappingId);

            if (subStructMapping != null)
            {
                foreach (WorkCategory category in workcategorys)
                {
                    var workItemListInCategory = db.GenericWorkItemPerProject
                        .Where(wi => wi.WorkCategoryId == category.WorkCategoryId &&
                        wi.SubProjectTypeId == subStructMapping.SubProjectTypeId && wi.ParentId == 0 &&
                        wi.IsSupplementaryWorkItem);
                    if (workItemListInCategory.Any())
                    {

                        workItems.AddRange(workItemListInCategory);
                    }
                }
            }

            IQueryable<GenericWorkItemPerProject> workItemsToReturn = workItems.AsQueryable();

            DataSourceResult result = workItemsToReturn.ToDataSourceResult(request, workItemPerProject => new
            {
                GenericWorkItemPerProjectId = workItemPerProject.GenericWorkItemPerProjectId,
                ProjectEstimationWorkItemId = workItemPerProject.ProjectEstimationWorkItemId,
                IsSubCategory = workItemPerProject.IsSubCategory,
                ItemCode = workItemPerProject.ItemCode,
                ContainsWorkItemAsInput = workItemPerProject.ContainsWorkItemAsInput,
                SubProjectTypeId = workItemPerProject.SubProjectTypeId,
                //ProjectInformation = workItemPerProject.ProjectInformation,

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
                TransportCostRate = workItemPerProject.TransportCostRate,
                HasAdditionalTransportationCost = workItemPerProject.HasAdditionalTransportationCost,
                CalculatedTransportCost = workItemPerProject.CalculatedTransportCost,
                TotalUnitCostIncludingTransport = workItemPerProject.TotalUnitCostIncludingTransport,
                OverHeadCost = workItemPerProject.OverHeadCost,
                ProfitCost = workItemPerProject.ProfitCost,
                Remark = workItemPerProject.Remark,
                LabourHourlyOutPut = workItemPerProject.LabourHourlyOutPut,
                UnitOfMeasurementId = workItemPerProject.UnitOfMeasurementId,
                OverheadRate = workItemPerProject.OverheadRate,
                ProfitMargin = workItemPerProject.ProfitMargin,

                IsSupplementaryWorkItem = workItemPerProject.IsSupplementaryWorkItem,
                IsVariationWorkItem = workItemPerProject.IsVariationWorkItem,
                AddedQuantityOfWork = workItemPerProject.AddedQuantityOfWork,
                OmittedQuantityOfWork = workItemPerProject.OmittedQuantityOfWork,

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

            return Json(result);
        }

        public ActionResult WorkItemThicknessFactor_Read([DataSourceRequest]DataSourceRequest request, int workItemId)
        {
            List<GenericWorkItemThicknessFactorEditViewModel> list = new List<GenericWorkItemThicknessFactorEditViewModel>();

            var currentWorkItem = db.GenericWorkItemPerProject.Find(workItemId);
            if (currentWorkItem != null)
            {
                var record = new GenericWorkItemThicknessFactorEditViewModel
                {
                    ItemId = currentWorkItem.GenericWorkItemPerProjectId,
                    ThicknessFactor = currentWorkItem.ThicknessFactor
                };
                list.Add(record);
            }

            return Json(list.AsQueryable().ToDataSourceResult(request));
        }
        public ActionResult WorkItemThicknessFactor_Update([DataSourceRequest]DataSourceRequest request, GenericWorkItemThicknessFactorEditViewModel item, int workItemId)
        {
            if (ModelState.IsValid)
            {
                var currentWorkItem = db.GenericWorkItemPerProject.Find(workItemId);
                if (currentWorkItem != null && item.ThicknessFactor > 0)
                {
                    currentWorkItem.ThicknessFactor = item.ThicknessFactor;

                    db.GenericWorkItemPerProject.Attach(currentWorkItem);
                    db.Entry(currentWorkItem).State = EntityState.Modified;
                    db.SaveChanges();

                    ProjectEstimationHelper.UpdateGenericWorkItem(workItemId);

                }
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult SpecificWorkItemDetail_Read([DataSourceRequest]DataSourceRequest request, int workItemId)
        {
            var currentWorkItem = db.GenericWorkItemPerProject.Find(workItemId);
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
                    Value = currentWorkItem.TotalQuantityOfWork.ToString("N2")
                };
                var materialCostRecord = new WorkItemDetailViewModel
                {
                    Attribute = "Material Unit Cost",
                    Value = currentWorkItem.MaterialCostPerUnit.ToString("N2")
                };
                var equipmentCostRecord = new WorkItemDetailViewModel
                {
                    Attribute = "Equipment Unit Cost",
                    Value = currentWorkItem.EquipmentCostPerUnit.ToString("N2")
                };
                var laborCostRecord = new WorkItemDetailViewModel
                {
                    Attribute = "Manpower Unit Cost",
                    Value = currentWorkItem.LaborCostPerUnit.ToString("N2")
                };
                var directCostRecord = new WorkItemDetailViewModel
                {
                    Attribute = "Direct Cost of Work Item",
                    Value = currentWorkItem.DirectCost.ToString("N2")
                };
                var profitCostRecord = new WorkItemDetailViewModel
                {
                    Attribute = "Profit Cost (" + currentWorkItem.ProfitMargin + "%)",
                    Value = currentWorkItem.ProfitCost.ToString("N2")
                };
                var overheadCostRecord = new WorkItemDetailViewModel
                {
                    Attribute = "Overhead Cost (" + currentWorkItem.OverheadRate + "%)",
                    Value = currentWorkItem.OverHeadCost.ToString("N2")
                };
                var totalCostRecord = new WorkItemDetailViewModel
                {
                    Attribute = "Total Unit Cost",
                    Value = currentWorkItem.TotalUnitCost.ToString("N2")
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

                if (currentWorkItem.HasAdditionalTransportationCost)
                {
                    var totalcostIncludingTransportRecord = new WorkItemDetailViewModel
                    {
                        Attribute = "Total including transportation cost (" + currentWorkItem.TransportCostRate.ToString("N2") + "%)",
                        Value = currentWorkItem.TotalUnitCostIncludingTransport.ToString("N2")
                    };
                    list.Add(totalcostIncludingTransportRecord);
                }

            }
            //IQueryable<TemplateWorkItemDetailViewModel> result = list.AsQueryable();
            return Json(list.AsQueryable().ToDataSourceResult(request));
        }

        public ActionResult WorkItemToCopyToStructre_Read([DataSourceRequest]DataSourceRequest request, int destinationSubProjectTypeMappingId, int sourceSubProjTypeMappingId, int catId = 0)
        {
            List<CopyGenericTemplateWorkItemViewModel> listToReturn = new List<CopyGenericTemplateWorkItemViewModel>();

            var sourceMapping = db.SubProjectStructureTypeMapping.Find(sourceSubProjTypeMappingId);
            var destinationMapping = db.SubProjectStructureTypeMapping.Find(destinationSubProjectTypeMappingId);
            if (sourceMapping != null && destinationMapping != null)
            {
                //var workCategories 
                var sourceSubProjectType = db.SubProjectType.Find(sourceMapping.SubProjectTypeId);
                var destinationSubProjectType = db.SubProjectType.Find(destinationMapping.SubProjectTypeId);
                if (sourceSubProjectType != null && destinationSubProjectType != null)
                {
                    var workItemsToCopy = db.GenericWorkItemPerProject
                        .Where(w => w.SubProjectTypeId == sourceMapping.SubProjectTypeId &&
                        w.WorkCategory.ConfigurationId == sourceMapping.ConfigurationId &&
                        w.ParentId == 0).ToList();
                    if(catId > 0)
                    {
                        workItemsToCopy = workItemsToCopy.Where(w => w.WorkCategoryId == catId).ToList();
                    }
                    foreach (GenericWorkItemPerProject workItemToCopy in workItemsToCopy)
                    {
                        var existingWorkItem = db.GenericWorkItemPerProject.FirstOrDefault(w => w.ProjectEstimationWorkItemId == workItemToCopy.ProjectEstimationWorkItemId &&
                                                                                w.SubProjectTypeId == destinationSubProjectType.SubProjectTypeId && w.WorkCategory.ConfigurationId == destinationMapping.ConfigurationId);

                        var item = new CopyGenericTemplateWorkItemViewModel
                        {
                            Id = workItemToCopy.GenericWorkItemPerProjectId,
                            Category = $"{ workItemToCopy.WorkCategory.Index } {workItemToCopy.WorkCategory.Name}",
                            WorkCategoryId = workItemToCopy.WorkCategoryId,
                            IsSubCategory = workItemToCopy.IsSubCategory,
                            ItemName = workItemToCopy.ItemName,
                            TotalQuantityOfWork = workItemToCopy.TotalQuantityOfWork,
                            TotalUnitRate = workItemToCopy.TotalUnitCost,
                            UnitOfMeasurement = ConsSettingHelper.GetUnitOfMearurnment(workItemToCopy.UnitOfMeasurementId ?? 0),
                        };
                        if (existingWorkItem != null)
                        {
                            item.IsCopied = true;
                        }
                        listToReturn.Add(item);
                    }
                    //var project = db.ProjectInformation.Find(currentSubProjectType.ProjectInformationId);
                    //if (project != null)
                    //{
                    //    var templateWorkItems = db.ProjectEstimationWorkItem
                    //                    .Where(tw => tw.ProjectType == project.ProjectType &&
                    //                    tw.WorkCategory.ConfigurationId == strId &&
                    //                    tw.ParentId == 0).ToList();

                    //    if (templateWorkItems.Any())
                    //    {
                    //        foreach (ProjectEstimationWorkItem workItem in templateWorkItems)
                    //        {
                    //            // check if the workItem is copied already 
                    //            var workItemInProject =
                    //                db.GenericWorkItemPerProject.FirstOrDefault(
                    //                    wpp => wpp.ProjectEstimationWorkItemId == workItem.ProjectEstimationWorkItemId &&
                    //                    wpp.WorkCategory.ConfigurationId == strId &&
                    //                    wpp.SubProjectTypeId == mapping.SubProjectTypeId);

                    //            var templateToReturn = new CopyTemplateWorkItemViewModel
                    //            {
                    //                Id = workItem.ProjectEstimationWorkItemId,
                    //                ItemName = workItem.ItemName,
                    //                Category = workItem.WorkCategory.Name,
                    //                TotalQuantityOfWork = workItem.TotalQuantityOfWork,
                    //                UnitOfMeasurement = ConsSettingHelper.GetUnitOfMearurnment(workItem.UnitOfMeasurementId ?? 0),
                    //                TotalUnitRate = workItem.TotalUnitCost,
                    //                IsSubCategory = workItem.IsSubCategory,
                    //            };
                    //            if (workItemInProject != null)
                    //            {
                    //                templateToReturn.IsCopied = true;
                    //            }
                    //            listToReturn.Add(templateToReturn);
                    //        }
                    //    }
                    //}
                }
            }

            return Json(listToReturn.AsQueryable().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult GenericWorkItemPerProject_Create([DataSourceRequest]DataSourceRequest request, WorkItemPerProject workItemPerProject)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var entity = new WorkItemPerProject
        //        {
        //            ItemName = "name from other",
        //            //Name = "name",
        //            TotalQuantityOfWork = workItemPerProject.TotalQuantityOfWork,
        //            ProjectEstimationWorkItemId = workItemPerProject.ProjectEstimationWorkItemId,
        //            IsSubCategory = workItemPerProject.IsSubCategory,
        //            ItemCode = workItemPerProject.ItemCode,

        //            DirectCost = workItemPerProject.DirectCost,
        //            MaterialCost = workItemPerProject.MaterialCost,
        //            LabourOutPut = workItemPerProject.LabourOutPut,
        //            EquipmentOutput = workItemPerProject.EquipmentOutput,
        //MaterialCostPerUnit = workItemPerProject.MaterialCostPerUnit,
        //        LaborCostPerUnit = workItemPerProject.LaborCostPerUnit,
        //        EquipmentCostPerUnit = workItemPerProject.EquipmentCostPerUnit,
        //            TotalUnitCost = workItemPerProject.TotalUnitCost,
        //            OverHeadCost = workItemPerProject.OverHeadCost,
        //            ProfitCost = workItemPerProject.ProfitCost,
        //            ProjectType = (AvailableProjectTypes)ProjectTye,
        //            Remark = workItemPerProject.Remark,
        //            LabourHourlyOutPut = workItemPerProject.LabourHourlyOutPut,
        //            UnitOfMeasurementId = workItemPerProject.UnitOfMeasurementId,
        //            OverheadRate = workItemPerProject.OverheadRate,
        //            ProfitMargin = workItemPerProject.ProfitMargin,
        //            ParentId = workItemPerProject.ParentId,
        //            OrgBranchName = workItemPerProject.OrgBranchName,
        //            DateCreated = DateTime.Now,
        //            LastModified = workItemPerProject.LastModified,
        //            CreatedBy = User.Identity.Name,
        //            ModifiedBy = workItemPerProject.ModifiedBy,
        //            ProjectInformationId = ProjectId,
        //            WorkCategoryId = workItemPerProject.WorkCategoryId
        //        };
        //        entity.TotalUnitCost = entity.LabourHourlyOutPut > 0 ?
        //                                        (entity.DirectCost + entity.OverHeadCost + entity.ProfitCost) / entity.LabourHourlyOutPut :
        //                                        entity.DirectCost + entity.OverHeadCost + entity.ProfitCost;
        //        db.WorkItemPerProject.Add(entity);
        //        db.SaveChanges();
        //        workItemPerProject.WorkItemPerProjectId = entity.WorkItemPerProjectId;
        //        ProjectListController.UpdateProjectTotalCost(workItemPerProject.WorkItemPerProjectId);
        //    }

        //    return Json(new[] { workItemPerProject }.ToDataSourceResult(request, ModelState));
        //}

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GenericWorkItemPerProject_Update([DataSourceRequest]DataSourceRequest request, GenericWorkItemPerProject workItemPerProject)
        {
            if (ModelState.IsValid)
            {
                var entity = new GenericWorkItemPerProject
                {
                    GenericWorkItemPerProjectId = workItemPerProject.GenericWorkItemPerProjectId,
                    ProjectEstimationWorkItemId = workItemPerProject.ProjectEstimationWorkItemId,
                    IsSubCategory = workItemPerProject.IsSubCategory,
                    ItemCode = workItemPerProject.ItemCode,
                    ContainsWorkItemAsInput = workItemPerProject.ContainsWorkItemAsInput,
                    ItemName = string.IsNullOrEmpty(workItemPerProject.ItemName) ?
                        GetEstimationWorkItem(workItemPerProject.ProjectEstimationWorkItemId) :
                        workItemPerProject.ItemName,
                    SubProjectTypeId = workItemPerProject.SubProjectTypeId,
                    ProjectInformation = workItemPerProject.ProjectInformation,
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

                    IsSupplementaryWorkItem = workItemPerProject.IsSupplementaryWorkItem,
                    IsVariationWorkItem = workItemPerProject.IsVariationWorkItem,
                    AddedQuantityOfWork = workItemPerProject.AddedQuantityOfWork,
                    OmittedQuantityOfWork = workItemPerProject.OmittedQuantityOfWork,

                    OrgBranchName = workItemPerProject.OrgBranchName,
                    DateCreated = workItemPerProject.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = workItemPerProject.CreatedBy,
                    ModifiedBy = User.Identity.Name,

                };

                db.GenericWorkItemPerProject.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                // consider the scenario where the item has been defined as subcategory item and had child items ... and 
                // on demand if that item is changed to regular item, meaning the child items will remain within the project
                // but with false reference to their previous parent which currently couldnt have any child.
                if (!entity.IsSubCategory)
                {
                    ProjectEstimationHelper.CleanFalseChildItems(entity.GenericWorkItemPerProjectId);
                }
            }

            return Json(new[] { workItemPerProject }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GenericWorkItemPerProject_Destroy([DataSourceRequest]DataSourceRequest request, GenericWorkItemPerProject workItemPerProject)
        {
            if (ModelState.IsValid)
            {
                var entity = new GenericWorkItemPerProject
                {
                    GenericWorkItemPerProjectId = workItemPerProject.GenericWorkItemPerProjectId,
                    ProjectEstimationWorkItemId = workItemPerProject.ProjectEstimationWorkItemId,
                    IsSubCategory = workItemPerProject.IsSubCategory,
                    ItemCode = workItemPerProject.ItemCode,
                    ContainsWorkItemAsInput = workItemPerProject.ContainsWorkItemAsInput,

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
                    TransportCostRate = workItemPerProject.TransportCostRate,
                    HasAdditionalTransportationCost = workItemPerProject.HasAdditionalTransportationCost,
                    CalculatedTransportCost = workItemPerProject.CalculatedTransportCost,
                    TotalUnitCostIncludingTransport = workItemPerProject.TotalUnitCostIncludingTransport,
                    OverHeadCost = workItemPerProject.OverHeadCost,
                    ProfitCost = workItemPerProject.ProfitCost,
                    ProjectType = workItemPerProject.ProjectType,
                    Remark = workItemPerProject.Remark,
                    LabourHourlyOutPut = workItemPerProject.LabourHourlyOutPut,
                    UnitOfMeasurementId = workItemPerProject.UnitOfMeasurementId,
                    OverheadRate = workItemPerProject.OverheadRate,
                    ProfitMargin = workItemPerProject.ProfitMargin,

                    IsSupplementaryWorkItem = workItemPerProject.IsSupplementaryWorkItem,
                    IsVariationWorkItem = workItemPerProject.IsVariationWorkItem,
                    AddedQuantityOfWork = workItemPerProject.AddedQuantityOfWork,
                    OmittedQuantityOfWork = workItemPerProject.OmittedQuantityOfWork,

                    ParentId = workItemPerProject.ParentId,
                    OrgBranchName = workItemPerProject.OrgBranchName,
                    DateCreated = workItemPerProject.DateCreated,
                    LastModified = workItemPerProject.LastModified,
                    CreatedBy = workItemPerProject.CreatedBy,
                    ModifiedBy = workItemPerProject.ModifiedBy,
                    ProjectInformationId = workItemPerProject.ProjectInformationId,
                    WorkCategoryId = workItemPerProject.WorkCategoryId,



                };

                db.GenericWorkItemPerProject.Attach(entity);
                db.GenericWorkItemPerProject.Remove(entity);
                db.SaveChanges();

                if (entity.IsSubCategory || entity.ContainsWorkItemAsInput)
                    ProjectEstimationHelper.CleanReferencesFromProjectOnGenericWorkItemDelete(entity);
            }

            return Json(new[] { workItemPerProject }.ToDataSourceResult(request, ModelState));
        }
        #region Methods

        // 'string withTake' was the third parameter here .....
        //public ActionResult CopyEstimation(int projectId, int type)
        //{
        //    var project = db.ProjectInformation.Where(x => x.ProjectInformationId == project).FirstOrDefault();
        //    if (project != null)
        //    {
        //        type = (int)project.ProjectType;
        //    }
        //    var estimationWorkItems = db.ProjectEstimationWorkItem.Where(x => x.ProjectType == (AvailableProjectTypes)type).ToList();
        //    var estimationPieceRateWorkItems =
        //        db.ProjectEstimationAreaWorkItem.Where(x => x.ProjectType == (AvailableProjectTypes)type).ToList();
        //    var estimationSubContractWorkItems =
        //        db.EstimationSubContractWorkItem.Where(x => x.ProjectType == (AvailableProjectTypes)type).ToList();
        //    var estimationCustomWorkItems = db.ProjectEstimationCustomWorkItems.Where(cw => cw.ProjectType == (AvailableProjectTypes)type)
        //        .ToList();
        //    WorkItemPerProject workItemPerProject = new WorkItemPerProject();
        //    AreaWorkItem areaWorkItem = new AreaWorkItem();
        //    SubContractWorkItem subContractWorkItem = new SubContractWorkItem();
        //    ProjectCustomWorkItem customWorkItem = new ProjectCustomWorkItem();
        //    var blocks = db.Blocks.Where(x => x.ProjectInformationId == project).ToList();


        //    var existingWorkItems =
        //        db.WorkItemPerProject.Where(x => x.ProjectInformationId == project && x.ProjectType == (AvailableProjectTypes)type).ToList();
        //    var existingAreaWorkItems = db.AreaWorkItemes.Where(x => x.ProjectInformationId == project).ToList();
        //    var existingSubContractWorkItems = db.SubContractWorkItem.Where(x => x.ProjectInformationId == project).ToList();
        //    var existingCustomWorkItems = db.ProjectCustomWorkItems.Where(cw => cw.ProjectInformationId == project)
        //        .ToList();
        //    #region Remove Copied data
        //    if (existingWorkItems.Any())
        //    {
        //        db.WorkItemPerProject.RemoveRange(existingWorkItems);
        //        db.SaveChanges();
        //    }
        //    if (existingAreaWorkItems.Any())
        //    {
        //        db.AreaWorkItemes.RemoveRange(existingAreaWorkItems);
        //        db.SaveChanges();
        //    }

        //    if (existingSubContractWorkItems.Any())
        //    {
        //        db.SubContractWorkItem.RemoveRange(existingSubContractWorkItems);
        //        db.SaveChanges();

        //    }
        //    if (existingCustomWorkItems.Any())
        //    {
        //        db.ProjectCustomWorkItems.RemoveRange(existingCustomWorkItems);
        //        db.SaveChanges();
        //    }
        //    #endregion
        //    if (estimationWorkItems.Any() || estimationPieceRateWorkItems.Any() || estimationSubContractWorkItems.Any() || estimationCustomWorkItems.Any())
        //    {
        //        if (blocks.Any())
        //        {
        //            #region With block

        //            #region piece Work Item

        //            foreach (var b in blocks)
        //            {
        //                var blockMapping = db.BlockMappings.Where(x => x.BlockId == b.BlockId);
        //                foreach (var bm in blockMapping)
        //                {
        //                    foreach (var p in estimationPieceRateWorkItems)
        //                    {

        //                        if (bm.ConfigurationId == p.WorkCategory.ConfigurationId)
        //                        {
        //                            areaWorkItem = new AreaWorkItem();
        //                            areaWorkItem.WorkItemName = p.WorkItemName;
        //                            areaWorkItem.ItemCode = p.ItemCode;
        //                            areaWorkItem.Width = p.Width;
        //                            areaWorkItem.Height = p.Height;
        //                            areaWorkItem.Area = p.Area;
        //                            areaWorkItem.CylinderLock = p.CylinderLock;
        //                            areaWorkItem.DirectCost = p.DirectCost;
        //                            areaWorkItem.MaterialCost = p.MaterialCost;
        //                            areaWorkItem.OverHeadProfitCost = p.OverHeadProfitCost;
        //                            areaWorkItem.Price = p.Price;
        //                            areaWorkItem.WorkCategoryId = p.WorkCategoryId;
        //                            areaWorkItem.ProjectInformationId = project;
        //                            areaWorkItem.BlockId = b.BlockId;
        //                            ExceedDbContext WorkItemdb = new ExceedDbContext();
        //                            WorkItemdb.AreaWorkItemes.Add(areaWorkItem);
        //                            WorkItemdb.SaveChanges();
        //                            areaWorkItem.AreaWorkItemId = areaWorkItem.AreaWorkItemId;
        //                        }
        //                    }
        //                }
        //            }

        //            #endregion


        //            #region Custom Work Items

        //            foreach (var b in blocks)
        //            {
        //                var blockMapping = db.BlockMappings.Where(x => x.BlockId == b.BlockId);
        //                foreach (var bm in blockMapping)
        //                {
        //                    foreach (var cw in estimationCustomWorkItems)
        //                    {
        //                        if (bm.ConfigurationId == ProjectEstimationHelper.GetCustomWorkItemCategory(cw.WorkCategoryId).ConfigurationId)
        //                        {
        //                            customWorkItem = new ProjectCustomWorkItem();
        //                            customWorkItem.WorkCategoryId = cw.WorkCategoryId;
        //                            customWorkItem.WorkItemName = cw.WorkItemName;
        //                            customWorkItem.ItemCode = cw.ItemCode;
        //                            customWorkItem.ProjectInformationId = project;
        //                            customWorkItem.ProjectType = cw.ProjectType;
        //                            customWorkItem.TotalUnitRate = cw.TotalUnitRate;
        //                            customWorkItem.BlockId = b.BlockId;
        //                            customWorkItem.Quantity = 1;
        //                            customWorkItem.TotalCost = cw.TotalUnitRate;
        //                            customWorkItem.Remark = cw.Remark;

        //                            ExceedDbContext customWorkItemDb = new ExceedDbContext();
        //                            customWorkItemDb.ProjectCustomWorkItems.Add(customWorkItem);
        //                            customWorkItemDb.SaveChanges();
        //                        }
        //                    }
        //                }
        //            }

        //            #endregion


        //            #region Sub contract Work Item

        //            foreach (var b in blocks)
        //            {
        //                var blockMapping = db.BlockMappings.Where(x => x.BlockId == b.BlockId);
        //                foreach (var bm in blockMapping)
        //                {
        //                    //foreach (var p in estimationPieceRateWorkItems)
        //                    //{
        //                    foreach (var s in estimationSubContractWorkItems)
        //                    {
        //                        if (bm.ConfigurationId == s.WorkCategory.ConfigurationId)
        //                        {

        //                            subContractWorkItem = new SubContractWorkItem();
        //                            subContractWorkItem.UnitOfMeasurementId = s.UnitOfMeasurementId;
        //                            subContractWorkItem.OverHeadProfit = s.OverHeadProfit;
        //                            subContractWorkItem.Quntity = s.Quntity;
        //                            subContractWorkItem.SelectedCost = s.SelectedCost;
        //                            subContractWorkItem.SubContractCostOne = s.SubContractCostOne;
        //                            subContractWorkItem.SubContractCostTwo = s.SubContractCostTwo;
        //                            subContractWorkItem.SubContractCostThree = s.SubContractCostThree;
        //                            subContractWorkItem.SubContractWorkItemId = s.EstimationSubContractWorkItemId;
        //                            subContractWorkItem.TotalCost = s.TotalCost;
        //                            subContractWorkItem.WorkCategoryId = s.WorkCategoryId;
        //                            subContractWorkItem.WorkItemName = s.WorkItemName;
        //                            subContractWorkItem.ProjectInformationId = project;
        //                            subContractWorkItem.BlockId = b.BlockId;
        //                            subContractWorkItem.ItemCode = s.ItemCode;
        //                            ExceedDbContext WorkItemdb = new ExceedDbContext();
        //                            WorkItemdb.SubContractWorkItem.Add(subContractWorkItem);
        //                            WorkItemdb.SaveChanges();
        //                            subContractWorkItem.SubContractWorkItemId =
        //                                subContractWorkItem.SubContractWorkItemId;
        //                        }
        //                    }
        //                    //}
        //                }
        //            }

        //            #endregion

        //            #region Work item

        //            foreach (var b in blocks)
        //            {
        //                var blockMapping = db.BlockMappings.Where(x => x.BlockId == b.BlockId);
        //                foreach (var bm in blockMapping)
        //                {
        //                    //foreach (var p in estimationPieceRateWorkItems)
        //                    //{
        //                    foreach (var p in estimationWorkItems)
        //                    {

        //                        if (bm.ConfigurationId == p.WorkCategory.ConfigurationId)
        //                        {
        //                            //foreach (var e in estimationWorkItems)
        //                            //{
        //                            workItemPerProject = new WorkItemPerProject();
        //                            workItemPerProject.ProjectInformationId = project;

        //                            workItemPerProject.IsSubCategory = p.IsSubCategory;
        //                            workItemPerProject.ItemCode = p.ItemCode;
        //                            workItemPerProject.ItemName = p.ItemName;

        //                            workItemPerProject.ProjectEstimationWorkItemId = p.ProjectEstimationWorkItemId;
        //                            workItemPerProject.TotalQuantityOfWork = p.TotalQuantityOfWork;
        //                            workItemPerProject.DirectCost = p.DirectCost;
        //                            workItemPerProject.MaterialCost = p.MaterialCost;
        //                            workItemPerProject.LabourOutPut = p.LabourOutPut;
        //                            workItemPerProject.EquipmentOutput = p.EquipmentOutput;
        //                            workItemPerProject.TotalUnitCost = p.TotalUnitCost;
        //                            workItemPerProject.OverHeadCost = p.OverHeadCost;
        //                            workItemPerProject.ProfitCost = p.ProfitCost;
        //                            workItemPerProject.ProjectType = p.ProjectType;
        //                            workItemPerProject.Remark = p.Remark;
        //                            workItemPerProject.BlockId = b.BlockId;
        //                            workItemPerProject.LabourHourlyOutPut = p.LabourHourlyOutPut;
        //                            workItemPerProject.UnitOfMeasurementId = p.UnitOfMeasurementId ?? 0;
        //                            workItemPerProject.WorkCategoryId = p.WorkCategoryId;
        //                            workItemPerProject.OverheadRate = p.OverheadRate;
        //                            workItemPerProject.ProfitMargin = p.ProfitMargin;
        //                            workItemPerProject.ParentId = p.ParentId;
        //                            workItemPerProject.OrgBranchName = p.OrgBranchName;
        //                            workItemPerProject.LastModified = null;
        //                            workItemPerProject.ModifiedBy = null;
        //                            workItemPerProject.CreatedBy = User.Identity.Name;
        //                            workItemPerProject.DateCreated = DateTime.Now;
        //                            ExceedDbContext WorkItemdb = new ExceedDbContext();
        //                            WorkItemdb.WorkItemPerProject.Add(workItemPerProject);
        //                            WorkItemdb.SaveChanges();
        //                            workItemPerProject.WorkItemPerProjectId =
        //                                workItemPerProject.WorkItemPerProjectId;



        //                            #region Copy Material

        //                            var etsmnMaterial =
        //                                db.ProjectEstimationMaterial.Where(
        //                                    x =>
        //                                        x.ProjectEstimationWorkItemId ==
        //                                        p.ProjectEstimationWorkItemId).ToList();
        //                            if (etsmnMaterial.Any())
        //                            {
        //                                foreach (var m in etsmnMaterial)
        //                                {
        //                                    var entity = new ProjectMaterial
        //                                    {
        //                                        Unit = m.Unit,
        //                                        Quantity = m.Quantity,
        //                                        ItemCategoryCode = m.ItemCategoryCode,
        //                                        ItemCode = m.ItemCode,
        //                                        UnitPrice = m.UnitPrice,
        //                                        TotalCost = m.TotalCost,
        //                                        MaterialUnitCost = m.MaterialUnitCost,
        //                                        OrgBranchName = m.OrgBranchName,
        //                                        DateCreated = DateTime.Now,
        //                                        LastModified = null,
        //                                        CreatedBy = User.Identity.Name,
        //                                        ModifiedBy = null,
        //                                        WorkItemPerProjectId =
        //                                            workItemPerProject.WorkItemPerProjectId,

        //                                    };
        //                                    WorkItemdb = new ExceedDbContext();
        //                                    WorkItemdb.ProjectMaterial.Add(entity);
        //                                    WorkItemdb.SaveChanges();
        //                                    //projectMaterial.ProjectMaterialId = entity.ProjectMaterialId;
        //                                }
        //                            }

        //                            #endregion

        //                            #region Copy Labour Cost

        //                            var etsmnLabour =
        //                                db.ProjectEstimationDirectLabour.Where(
        //                                    x =>
        //                                        x.ProjectEstimationWorkItemId ==
        //                                        p.ProjectEstimationWorkItemId).ToList();
        //                            if (etsmnLabour.Any())
        //                            {
        //                                foreach (var m in etsmnLabour)
        //                                {
        //                                    var entity = new ProjectDirectLabour
        //                                    {
        //                                        Unit = m.Unit,
        //                                        Quantity = m.Quantity,
        //                                        UnitFactory = m.UnitFactory,
        //                                        IndexedHourlySalary = m.IndexedHourlySalary,
        //                                        DailyPay = m.DailyPay,
        //                                        TotalCost = m.TotalCost,
        //                                        ManpowerUnitCost = m.ManpowerUnitCost,
        //                                        OrgBranchName = m.OrgBranchName,
        //                                        DateCreated = DateTime.Now,
        //                                        LastModified = null,
        //                                        CreatedBy = User.Identity.Name,
        //                                        ModifiedBy = null,
        //                                        WorkItemPerProjectId =
        //                                            workItemPerProject.WorkItemPerProjectId,
        //                                        ProjectEstimationIndexLabourId =
        //                                            m.ProjectEstimationIndexLabourId
        //                                    };
        //                                    WorkItemdb = new ExceedDbContext();
        //                                    WorkItemdb.ProjectDirectLabour.Add(entity);
        //                                    WorkItemdb.SaveChanges();
        //                                    //projectDirectLabour.ProjectDirectLabourId = entity.ProjectDirectLabourId;
        //                                }
        //                            }

        //                            #endregion

        //                            #region Copy Equipment

        //                            var etsmnEquipment =
        //                                db.ProjectEstimationEquipment.Where(
        //                                    x =>
        //                                        x.ProjectEstimationWorkItemId ==
        //                                        p.ProjectEstimationWorkItemId).ToList();
        //                            if (etsmnEquipment.Any())
        //                            {
        //                                foreach (var m in etsmnEquipment)
        //                                {
        //                                    var entity = new ProjectEquipment
        //                                    {
        //                                        //EquipmentTypeID = m.EquipmentTypeID,
        //                                        //EquipmentIdentityID = m.EquipmentIdentityID,
        //                                        FleetsID = m.FleetsID,
        //                                        Unit = m.Unit,
        //                                        Quantity = m.Quantity,
        //                                        UnitFactory = m.UnitFactory,
        //                                        RentalCost = m.RentalCost,
        //                                        TotalCost = m.TotalCost,
        //                                        EquipmentUnitCost = m.EquipmentUnitCost,
        //                                        // EquipmentType = m.EquipmentType,
        //                                        OrgBranchName = m.OrgBranchName,
        //                                        DateCreated = DateTime.Now,
        //                                        LastModified = null,
        //                                        CreatedBy = User.Identity.Name,
        //                                        ModifiedBy = null,
        //                                        WorkItemPerProjectId =
        //                                            workItemPerProject.WorkItemPerProjectId,

        //                                    };
        //                                    WorkItemdb = new ExceedDbContext();
        //                                    WorkItemdb.ProjectEquipment.Add(entity);
        //                                    WorkItemdb.SaveChanges();
        //                                    //projectEquipment.ProjectEquipmentId = entity.ProjectEquipmentId;
        //                                }
        //                            }

        //                            #endregion

        //                            #region Copy Takeoff sheet

        //                            //if (withTake == "on")
        //                            //{

        //                            //    // the e was replaced by p starting from this current foreach starting poit to
        //                            //    // the the one in  line 784
        //                            //    var etsmnTakeOfSheet =
        //                            //        db.ProjectTakeOff.Where(
        //                            //            x =>
        //                            //                x.ProjectEstimationWorkItemId ==
        //                            //                p.ProjectEstimationWorkItemId &&
        //                            //                x.ProjectId == type).ToList();
        //                            //    TakeOffPerProject takeOffPerProject = new TakeOffPerProject();
        //                            //    if (etsmnTakeOfSheet.Any())
        //                            //    {
        //                            //        foreach (var m in etsmnTakeOfSheet)
        //                            //        {
        //                            //            takeOffPerProject = new TakeOffPerProject();
        //                            //            var entity = new TakeOffPerProject
        //                            //            {
        //                            //                TakeOffNo = m.TakeOffNo,

        //                            //                WorkItemPerProjectId =
        //                            //                    workItemPerProject.WorkItemPerProjectId,
        //                            //                ProjectInformationId = type,
        //                            //                Remark = m.Remark,
        //                            //                OrgBranchName = m.OrgBranchName,
        //                            //                DateCreated = DateTime.Now,
        //                            //                LastModified = null,
        //                            //                CreatedBy = User.Identity.Name,
        //                            //                ModifiedBy = null
        //                            //            };
        //                            //            WorkItemdb = new ExceedDbContext();
        //                            //            WorkItemdb.TakeOffPerProject.Add(entity);
        //                            //            WorkItemdb.SaveChanges();
        //                            //            takeOffPerProject.TakeOffPerProjectId =
        //                            //                entity.TakeOffPerProjectId;

        //                            //            #region Takeoff Line

        //                            //            var takeoffline =
        //                            //                db.ProjectTakeOffLine.Where(
        //                            //                    x => x.ProjectTakeOffId == m.ProjectTakeOffId)
        //                            //                    .ToList();
        //                            //            if (takeoffline.Any())
        //                            //            {
        //                            //                foreach (var t in takeoffline)
        //                            //                {
        //                            //                    var entityi = new TakeOffLinePerProject
        //                            //                    {
        //                            //                        No = t.T,
        //                            //                        D = t.D,
        //                            //                        Quantity = t.S,
        //                            //                        Description = t.Description,
        //                            //                        UnitOfMeasurementId = t.UnitOfMeasurementId,
        //                            //                        Length = t.Length,
        //                            //                        Width = t.Width,
        //                            //                        Depth = t.Height,
        //                            //                        OrgBranchName = t.OrgBranchName,
        //                            //                        DateCreated = DateTime.Now,
        //                            //                        LastModified = null,
        //                            //                        CreatedBy = User.Identity.Name,
        //                            //                        ModifiedBy = null,
        //                            //                        TakeOffLinePerProjectId =
        //                            //                            takeOffPerProject.TakeOffPerProjectId
        //                            //                    };
        //                            //                    WorkItemdb = new ExceedDbContext();

        //                            //                    WorkItemdb.TakeOffLinePerProject.Add(entityi);
        //                            //                    WorkItemdb.SaveChanges();

        //                            //                }
        //                            //            }

        //                            //            #endregion

        //                            //            #region Takeoff Line per Kg

        //                            //            var takeofflineKg =
        //                            //                db.ProjectTakeOffLineForKg.Where(
        //                            //                    x => x.ProjectTakeOffId == m.ProjectTakeOffId)
        //                            //                    .ToList();
        //                            //            if (takeofflineKg.Any())
        //                            //            {
        //                            //                foreach (var t in takeofflineKg)
        //                            //                {
        //                            //                    var entityy = new TakeOffLineForKgPerProject
        //                            //                    {
        //                            //                        Description = t.Description,
        //                            //                        //UnitRateId = t.UnitRateId,
        //                            //                        Type = t.Type,
        //                            //                        Shape = t.Shape,
        //                            //                        Length = t.Length,
        //                            //                        NoOfBars = t.NoOfBars,
        //                            //                        NoOfElement = t.NoOfElement,
        //                            //                        TotalBar = t.TotalBar,
        //                            //                        TotalLength = t.TotalLength,
        //                            //                        GrandTotal = t.GrandTotal,
        //                            //                        OrgBranchName = t.OrgBranchName,
        //                            //                        DateCreated = DateTime.Now,
        //                            //                        LastModified = null,
        //                            //                        CreatedBy = User.Identity.Name,
        //                            //                        ModifiedBy = null,
        //                            //                        TakeOffPerProjectId =
        //                            //                            takeOffPerProject.TakeOffPerProjectId
        //                            //                    };
        //                            //                    WorkItemdb = new ExceedDbContext();
        //                            //                    WorkItemdb.TakeOffLineForKgPerProject.Add(entityy);
        //                            //                    WorkItemdb.SaveChanges();

        //                            //                }
        //                            //            }

        //                            //            #endregion
        //                            //        }
        //                            //    }



        //                            //}
        //                            #endregion
        //                            //}
        //                        }
        //                    }
        //                }

        //            }
        //            #endregion
        //            #endregion
        //        }
        //        #region copying work items when no block is defined
        //        //else
        //        //{
        //        //    #region WithOut block
        //        //    foreach (var p in estimationPieceRateWorkItems)
        //        //    {
        //        //        areaWorkItem = new AreaWorkItem();
        //        //        areaWorkItem.Width = p.Width;
        //        //        areaWorkItem.Height = p.Height;
        //        //        areaWorkItem.Area = p.Area;
        //        //        areaWorkItem.CylinderLock = p.CylinderLock;
        //        //        areaWorkItem.DirectCost = p.DirectCost;
        //        //        areaWorkItem.MaterialCost = p.MaterialCost;
        //        //        areaWorkItem.OverHeadProfitCost = p.OverHeadProfitCost;
        //        //        areaWorkItem.Price = p.Price;
        //        //        areaWorkItem.WorkCategoryId = p.WorkCategoryId;
        //        //        areaWorkItem.ProjectInformationId = project;
        //        //        ExceedDbContext WorkItemdb = new ExceedDbContext();
        //        //        WorkItemdb.AreaWorkItemes.Add(areaWorkItem);
        //        //        WorkItemdb.SaveChanges();
        //        //        areaWorkItem.AreaWorkItemId = areaWorkItem.AreaWorkItemId;
        //        //    }
        //        //    foreach(var cw in estimationCustomWorkItems)
        //        //    {
        //        //        customWorkItem = new ProjectCustomWorkItem();
        //        //        customWorkItem.WorkCategoryId = cw.WorkCategoryId;
        //        //        customWorkItem.WorkItemName = cw.WorkItemName;
        //        //        customWorkItem.ItemCode = cw.ItemCode;
        //        //        customWorkItem.ProjectInformationId = project;
        //        //        customWorkItem.ProjectType = cw.ProjectType;
        //        //        customWorkItem.TotalUnitRate = cw.TotalUnitRate;
        //        //        //customWorkItem.BlockId = b.BlockId;
        //        //        customWorkItem.Quantity = 1;
        //        //        customWorkItem.TotalCost = cw.TotalUnitRate;
        //        //        customWorkItem.Remark = cw.Remark;

        //        //        ExceedDbContext customWorkItemDb = new ExceedDbContext();
        //        //        customWorkItemDb.ProjectCustomWorkItems.Add(customWorkItem);
        //        //        customWorkItemDb.SaveChanges();
        //        //    }
        //        //    foreach (var s in estimationSubContractWorkItems)
        //        //    {
        //        //        subContractWorkItem = new SubContractWorkItem();
        //        //        subContractWorkItem.UnitOfMeasurementId = s.UnitOfMeasurementId;
        //        //        subContractWorkItem.OverHeadProfit = s.OverHeadProfit;
        //        //        subContractWorkItem.Quntity = s.Quntity;
        //        //        subContractWorkItem.SelectedCost = s.SelectedCost;
        //        //        subContractWorkItem.SubContractCostOne = s.SubContractCostOne;
        //        //        subContractWorkItem.SubContractCostTwo = s.SubContractCostTwo;
        //        //        subContractWorkItem.SubContractCostThree = s.SubContractCostThree;
        //        //        subContractWorkItem.SubContractWorkItemId = s.EstimationSubContractWorkItemId;
        //        //        subContractWorkItem.TotalCost = s.TotalCost;
        //        //        subContractWorkItem.WorkCategoryId = s.WorkCategoryId;
        //        //        subContractWorkItem.WorkItemName = s.WorkItemName;
        //        //        subContractWorkItem.ProjectInformationId = project;
        //        //        ExceedDbContext WorkItemdb = new ExceedDbContext();
        //        //        WorkItemdb.SubContractWorkItem.Add(subContractWorkItem);
        //        //        WorkItemdb.SaveChanges();
        //        //        subContractWorkItem.SubContractWorkItemId = subContractWorkItem.SubContractWorkItemId;
        //        //    }
        //        //    foreach (var e in estimationWorkItems)
        //        //    {
        //        //        workItemPerProject = new WorkItemPerProject();
        //        //        workItemPerProject.ProjectInformationId = project;
        //        //        workItemPerProject.ProjectEstimationWorkItemId = e.ProjectEstimationWorkItemId;
        //        //        workItemPerProject.IsSubCategory = e.IsSubCategory;
        //        //        workItemPerProject.ItemCode = e.ItemCode;
        //        //        workItemPerProject.TotalQuantityOfWork = e.TotalQuantityOfWork;
        //        //        workItemPerProject.DirectCost = e.DirectCost;
        //        //        workItemPerProject.MaterialCost = e.MaterialCost;
        //        //        workItemPerProject.LabourOutPut = e.LabourOutPut;
        //        //        workItemPerProject.EquipmentOutput = e.EquipmentOutput;
        //        //        workItemPerProject.TotalUnitCost = e.TotalUnitCost;
        //        //        workItemPerProject.OverHeadCost = e.OverHeadCost;
        //        //        workItemPerProject.ProfitCost = e.ProfitCost;
        //        //        workItemPerProject.ProjectType = e.ProjectType;
        //        //        workItemPerProject.Remark = e.Remark;

        //        //        workItemPerProject.LabourHourlyOutPut = e.LabourHourlyOutPut;
        //        //        workItemPerProject.UnitOfMeasurementId = e.UnitOfMeasurementId ?? 0;
        //        //        workItemPerProject.WorkCategoryId = e.WorkCategoryId;
        //        //        workItemPerProject.OverheadRate = e.OverheadRate;
        //        //        workItemPerProject.ProfitMargin = e.ProfitMargin;
        //        //        workItemPerProject.ParentId = e.ParentId;
        //        //        workItemPerProject.OrgBranchName = e.OrgBranchName;
        //        //        workItemPerProject.LastModified = null;
        //        //        workItemPerProject.ModifiedBy = null;
        //        //        workItemPerProject.CreatedBy = User.Identity.Name;
        //        //        workItemPerProject.DateCreated = DateTime.Now;
        //        //        ExceedDbContext WorkItemdb = new ExceedDbContext();
        //        //        WorkItemdb.WorkItemPerProject.Add(workItemPerProject);
        //        //        WorkItemdb.SaveChanges();
        //        //        workItemPerProject.WorkItemPerProjectId = workItemPerProject.WorkItemPerProjectId;

        //        //        #region Copy Material

        //        //        var etsmnMaterial =
        //        //            db.ProjectEstimationMaterial.Where(
        //        //                x => x.ProjectEstimationWorkItemId == e.ProjectEstimationWorkItemId).ToList();
        //        //        if (etsmnMaterial.Any())
        //        //        {
        //        //            foreach (var m in etsmnMaterial)
        //        //            {
        //        //                var entity = new ProjectMaterial
        //        //                {
        //        //                    Unit = m.Unit,
        //        //                    Quantity = m.Quantity,
        //        //                    ItemCategoryCode = m.ItemCategoryCode,
        //        //                    ItemCode = m.ItemCode,
        //        //                    UnitPrice = m.UnitPrice,
        //        //                    TotalCost = m.TotalCost,
        //        //                    MaterialUnitCost = m.MaterialUnitCost,
        //        //                    OrgBranchName = m.OrgBranchName,
        //        //                    DateCreated = DateTime.Now,
        //        //                    LastModified = null,
        //        //                    CreatedBy = User.Identity.Name,
        //        //                    ModifiedBy = null,
        //        //                    WorkItemPerProjectId = workItemPerProject.WorkItemPerProjectId,

        //        //                };
        //        //                WorkItemdb = new ExceedDbContext();
        //        //                WorkItemdb.ProjectMaterial.Add(entity);
        //        //                WorkItemdb.SaveChanges();
        //        //                //projectMaterial.ProjectMaterialId = entity.ProjectMaterialId;
        //        //            }
        //        //        }

        //        //        #endregion

        //        //        #region Copy Labour Cost

        //        //        var etsmnLabour =
        //        //            db.ProjectEstimationDirectLabour.Where(
        //        //                x => x.ProjectEstimationWorkItemId == e.ProjectEstimationWorkItemId).ToList();
        //        //        if (etsmnLabour.Any())
        //        //        {
        //        //            foreach (var m in etsmnLabour)
        //        //            {
        //        //                var entity = new ProjectDirectLabour
        //        //                {
        //        //                    Unit = m.Unit,
        //        //                    Quantity = m.Quantity,
        //        //                    UnitFactory = m.UnitFactory,
        //        //                    IndexedHourlySalary = m.IndexedHourlySalary,
        //        //                    DailyPay = m.DailyPay,
        //        //                    TotalCost = m.TotalCost,
        //        //                    ManpowerUnitCost = m.ManpowerUnitCost,
        //        //                    OrgBranchName = m.OrgBranchName,
        //        //                    DateCreated = DateTime.Now,
        //        //                    LastModified = null,
        //        //                    CreatedBy = User.Identity.Name,
        //        //                    ModifiedBy = null,
        //        //                    WorkItemPerProjectId = workItemPerProject.WorkItemPerProjectId,
        //        //                    ProjectEstimationIndexLabourId = m.ProjectEstimationIndexLabourId
        //        //                };
        //        //                WorkItemdb = new ExceedDbContext();
        //        //                WorkItemdb.ProjectDirectLabour.Add(entity);
        //        //                WorkItemdb.SaveChanges();
        //        //                //projectDirectLabour.ProjectDirectLabourId = entity.ProjectDirectLabourId;
        //        //            }
        //        //        }

        //        //        #endregion

        //        //        #region Copy Equipment

        //        //        var etsmnEquipment =
        //        //            db.ProjectEstimationEquipment.Where(
        //        //                x => x.ProjectEstimationWorkItemId == e.ProjectEstimationWorkItemId).ToList();
        //        //        if (etsmnEquipment.Any())
        //        //        {
        //        //            foreach (var m in etsmnEquipment)
        //        //            {
        //        //                var entity = new ProjectEquipment
        //        //                {
        //        //                    //EquipmentTypeID = m.EquipmentTypeID,
        //        //                    //EquipmentIdentityID = m.EquipmentIdentityID,
        //        //                    FleetsID = m.FleetsID,
        //        //                    Unit = m.Unit,
        //        //                    Quantity = m.Quantity,
        //        //                    UnitFactory = m.UnitFactory,
        //        //                    RentalCost = m.RentalCost,
        //        //                    TotalCost = m.TotalCost,
        //        //                    EquipmentUnitCost = m.EquipmentUnitCost,
        //        //                    EquipmentType = m.EquipmentType,
        //        //                    OrgBranchName = m.OrgBranchName,
        //        //                    DateCreated = DateTime.Now,
        //        //                    LastModified = null,
        //        //                    CreatedBy = User.Identity.Name,
        //        //                    ModifiedBy = null,
        //        //                    WorkItemPerProjectId = workItemPerProject.WorkItemPerProjectId,

        //        //                };

        //        //                db.ProjectEquipment.Add(entity);
        //        //                db.SaveChanges();
        //        //                //projectEquipment.ProjectEquipmentId = entity.ProjectEquipmentId;
        //        //            }
        //        //        }

        //        //        #endregion

        //        //        #region target for removal
        //        //        //if (withTake == "on")
        //        //        //{
        //        //        //    #region Copy Takeoff sheet

        //        //        //    var etsmnTakeOfSheet =
        //        //        //        db.ProjectTakeOff.Where(
        //        //        //            x =>
        //        //        //                x.ProjectEstimationWorkItemId == e.ProjectEstimationWorkItemId &&
        //        //        //                x.ProjectId == type).ToList();
        //        //        //    TakeOffPerProject takeOffPerProject = new TakeOffPerProject();
        //        //        //    if (etsmnTakeOfSheet.Any())
        //        //        //    {
        //        //        //        foreach (var m in etsmnTakeOfSheet)
        //        //        //        {
        //        //        //            takeOffPerProject = new TakeOffPerProject();
        //        //        //            var entity = new TakeOffPerProject
        //        //        //            {
        //        //        //                TakeOffNo = m.TakeOffNo,

        //        //        //                WorkItemPerProjectId = workItemPerProject.WorkItemPerProjectId,
        //        //        //                ProjectInformationId = type,
        //        //        //                Remark = m.Remark,
        //        //        //                OrgBranchName = m.OrgBranchName,
        //        //        //                DateCreated = DateTime.Now,
        //        //        //                LastModified = null,
        //        //        //                CreatedBy = User.Identity.Name,
        //        //        //                ModifiedBy = null
        //        //        //            };
        //        //        //            WorkItemdb = new ExceedDbContext();
        //        //        //            WorkItemdb.TakeOffPerProject.Add(entity);
        //        //        //            WorkItemdb.SaveChanges();
        //        //        //            takeOffPerProject.TakeOffPerProjectId = entity.TakeOffPerProjectId;

        //        //        //            #region Takeoff Line

        //        //        //            var takeoffline =
        //        //        //                db.ProjectTakeOffLine.Where(x => x.ProjectTakeOffId == m.ProjectTakeOffId)
        //        //        //                    .ToList();
        //        //        //            if (takeoffline.Any())
        //        //        //            {
        //        //        //                foreach (var t in takeoffline)
        //        //        //                {
        //        //        //                    var entityi = new TakeOffLinePerProject
        //        //        //                    {
        //        //        //                        No = t.T,
        //        //        //                        D = t.D,
        //        //        //                        Quantity = t.S,
        //        //        //                        Description = t.Description,
        //        //        //                        UnitOfMeasurementId = t.UnitOfMeasurementId,
        //        //        //                        Length = t.Length,
        //        //        //                        Width = t.Width,
        //        //        //                        Depth = t.Height,
        //        //        //                        OrgBranchName = t.OrgBranchName,
        //        //        //                        DateCreated = DateTime.Now,
        //        //        //                        LastModified = null,
        //        //        //                        CreatedBy = User.Identity.Name,
        //        //        //                        ModifiedBy = null,
        //        //        //                        TakeOffLinePerProjectId = takeOffPerProject.TakeOffPerProjectId
        //        //        //                    };
        //        //        //                    WorkItemdb = new ExceedDbContext();

        //        //        //                    WorkItemdb.TakeOffLinePerProject.Add(entityi);
        //        //        //                    WorkItemdb.SaveChanges();

        //        //        //                }
        //        //        //            }

        //        //        //            #endregion

        //        //        //            #region Takeoff Line per Kg

        //        //        //            var takeofflineKg =
        //        //        //                db.ProjectTakeOffLineForKg.Where(x => x.ProjectTakeOffId == m.ProjectTakeOffId)
        //        //        //                    .ToList();
        //        //        //            if (takeofflineKg.Any())
        //        //        //            {
        //        //        //                foreach (var t in takeofflineKg)
        //        //        //                {
        //        //        //                    var entityy = new TakeOffLineForKgPerProject
        //        //        //                    {
        //        //        //                        Description = t.Description,
        //        //        //                        //UnitRateId = t.UnitRateId,
        //        //        //                        Type = t.Type,
        //        //        //                        Shape = t.Shape,
        //        //        //                        Length = t.Length,
        //        //        //                        NoOfBars = t.NoOfBars,
        //        //        //                        NoOfElement = t.NoOfElement,
        //        //        //                        TotalBar = t.TotalBar,
        //        //        //                        TotalLength = t.TotalLength,
        //        //        //                        GrandTotal = t.GrandTotal,
        //        //        //                        OrgBranchName = t.OrgBranchName,
        //        //        //                        DateCreated = DateTime.Now,
        //        //        //                        LastModified = null,
        //        //        //                        CreatedBy = User.Identity.Name,
        //        //        //                        ModifiedBy = null,
        //        //        //                        TakeOffPerProjectId = takeOffPerProject.TakeOffPerProjectId
        //        //        //                    };
        //        //        //                    WorkItemdb = new ExceedDbContext();
        //        //        //                    WorkItemdb.TakeOffLineForKgPerProject.Add(entityy);
        //        //        //                    WorkItemdb.SaveChanges();

        //        //        //                }
        //        //        //            }

        //        //        //            #endregion
        //        //        //        }
        //        //        //    }

        //        //        //    #endregion
        //        //        //} 
        //        //        #endregion

        //        //    }
        //        //    #endregion
        //        //} 
        //        #endregion
        //        // 
        //        //  summary:
        //        //      after copying the work items from the template,
        //        //      update work items that are sub-work items 
        //        this.UpdateWorkItemsHeirarchy(project);
        //    }
        //    else
        //    {
        //        return new JavaScriptResult() { Script = "alert('" + "No existing estimated data was found for this type of project!!" + "');" };
        //    }
        //    project = 0;
        //    ProjectId = 0;
        //    return new JavaScriptResult() { Script = "alert('" + "Data Copied Successfully!!" + "');" };
        //}

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

        public static void MigrateWorkItemsToNewStructure()
        {
            var db = new ExceedDbContext();
            //var projectUnderEstimation = db.ProjectInformation.Where(p => p.ProjectStatus == ProjectStatus.ESTIMATION).ToList();
            var projectUnderEstimation = db.ProjectInformation.ToList();

            if (projectUnderEstimation.Any())
            {
                foreach (ProjectInformation project in projectUnderEstimation)
                {
                    var existingBlocks = db.Blocks.Where(b => b.ProjectInformationId == project.ProjectInformationId).ToList();
                    var workItems = db.WorkItemPerProject.Where(wi => wi.ProjectInformationId == project.ProjectInformationId).ToList();
                    var areaWorkItems = db.AreaWorkItemes.Where(wi => wi.ProjectInformationId == project.ProjectInformationId).ToList();
                    var subContractWorkItems = db.SubContractWorkItem.Where(wi => wi.ProjectInformationId == project.ProjectInformationId).ToList();
                    var customWorkItems = db.ProjectCustomWorkItems.Where(wi => wi.ProjectInformationId == project.ProjectInformationId).ToList();

                    var subProjectTypes = db.SubProjectType.Where(sb => sb.ProjectInformationId == project.ProjectInformationId).ToList();
                    var genericWorkItems = db.GenericWorkItemPerProject.Where(wi => wi.ProjectInformationId == project.ProjectInformationId).ToList();
                    var genericAreaWorkItems = db.GenericAreaWorkItem.Where(wi => wi.ProjectInformationId == project.ProjectInformationId).ToList();
                    var genericSubContractWorkItems = db.GenericSubContractWorkItem.Where(wi => wi.ProjectInformationId == project.ProjectInformationId).ToList();
                    var genericCustomWorkItems = db.GenericProjectCustomWorkItem.Where(wi => wi.ProjectInformationId == project.ProjectInformationId).ToList();
                    var genericStructureMapping = db.SubProjectStructureTypeMapping.Where(m => m.SubProject.ProjectInformationId == project.ProjectInformationId).ToList();
                    var genericMaterials = db.GenericProjectMaterial.Where(m => m.WorkItem.ProjectInformationId == project.ProjectInformationId).ToList();
                    var genericEquipments = db.GenericProjectEquipment.Where(e => e.WorkItem.ProjectInformationId == project.ProjectInformationId).ToList();
                    var genericLabors = db.GenericProjectDirectLabour.Where(l => l.WorkItem.ProjectInformationId == project.ProjectInformationId).ToList();

                    #region Remove existing data
                    if (genericMaterials.Any())
                    {
                        db.GenericProjectMaterial.RemoveRange(genericMaterials);
                        db.SaveChanges();
                    }
                    if (genericEquipments.Any())
                    {
                        db.GenericProjectEquipment.RemoveRange(genericEquipments);
                        db.SaveChanges();
                    }
                    if (genericLabors.Any())
                    {
                        db.GenericProjectDirectLabour.RemoveRange(genericLabors);
                        db.SaveChanges();
                    }

                    if (genericCustomWorkItems.Any())
                    {
                        db.GenericProjectCustomWorkItem.RemoveRange(genericCustomWorkItems);
                        db.SaveChanges();
                    }
                    if (genericAreaWorkItems.Any())
                    {
                        db.GenericAreaWorkItem.RemoveRange(genericAreaWorkItems);
                        db.SaveChanges();
                    }
                    if (genericSubContractWorkItems.Any())
                    {
                        db.GenericSubContractWorkItem.RemoveRange(genericSubContractWorkItems);
                        db.SaveChanges();
                    }

                    if (genericWorkItems.Any())
                    {
                        db.GenericWorkItemPerProject.RemoveRange(genericWorkItems);
                        db.SaveChanges();
                    }
                    if (genericStructureMapping.Any())
                    {
                        db.SubProjectStructureTypeMapping.RemoveRange(genericStructureMapping);
                        db.SaveChanges();
                    }
                    if (subProjectTypes.Any())
                    {
                        db.SubProjectType.RemoveRange(subProjectTypes);
                        db.SaveChanges();
                    }
                    #endregion

                    if (existingBlocks.Any())
                    {
                        foreach (Block block in existingBlocks)
                        {
                            #region add sub-project type
                            var subProjectTypeToAdd = new SubProjectType
                            {
                                Name = block.Name,
                                NumberOfBlocks = block.NumberOfBlocks,
                                ProjectInformationId = block.ProjectInformationId,
                                Remark = block.Remark,
                            };
                            db.SubProjectType.Add(subProjectTypeToAdd);
                            db.SaveChanges();
                            #endregion

                            var bMappings = db.BlockMappings.Where(bm => bm.BlockId == block.BlockId).ToList();
                            if (bMappings.Any())
                            {
                                foreach (BlockMapping mapping in bMappings)
                                {
                                    #region add subproject and structure mapping
                                    var mappingToAdd = new SubProjectStructureTypeMapping
                                    {
                                        SubProjectTypeId = subProjectTypeToAdd.SubProjectTypeId,
                                        ConfigurationId = mapping.ConfigurationId,
                                        Remark = mapping.Remark,
                                    };
                                    db.SubProjectStructureTypeMapping.Add(mappingToAdd);
                                    db.SaveChanges();
                                    #endregion
                                }
                            }

                            var existingWorkItemsUnderBlock = db.WorkItemPerProject.Where(w => w.BlockId == block.BlockId).ToList();
                            if (existingWorkItemsUnderBlock.Any())
                            {
                                foreach (WorkItemPerProject workItemPerProject in existingWorkItemsUnderBlock)
                                {
                                    #region add work items
                                    var genericWorkItemToAdd = new GenericWorkItemPerProject
                                    {
                                        ProjectEstimationWorkItemId = workItemPerProject.ProjectEstimationWorkItemId,
                                        IsSubCategory = workItemPerProject.IsSubCategory,
                                        ItemName = workItemPerProject.ItemName,
                                        ItemCode = workItemPerProject.ItemCode,
                                        ContainsWorkItemAsInput = workItemPerProject.ContainsWorkItemAsInput,
                                        SubProjectTypeId = subProjectTypeToAdd.SubProjectTypeId,

                                        TotalQuantityOfWork = workItemPerProject.TotalQuantityOfWork,
                                        DirectCost = workItemPerProject.DirectCost,
                                        MaterialCost = workItemPerProject.MaterialCost,
                                        LabourOutPut = workItemPerProject.LabourOutPut,
                                        EquipmentOutput = workItemPerProject.EquipmentOutput,
                                        MaterialCostPerUnit = workItemPerProject.MaterialCostPerUnit,
                                        LaborCostPerUnit = workItemPerProject.LaborCostPerUnit,
                                        EquipmentCostPerUnit = workItemPerProject.EquipmentCostPerUnit,
                                        TotalUnitCost = workItemPerProject.TotalUnitCost,
                                        TransportCostRate = workItemPerProject.TransportCostRate,
                                        HasAdditionalTransportationCost = workItemPerProject.HasAdditionalTransportationCost,
                                        CalculatedTransportCost = workItemPerProject.CalculatedTransportCost,
                                        TotalUnitCostIncludingTransport = workItemPerProject.TotalUnitCostIncludingTransport,
                                        OverHeadCost = workItemPerProject.OverHeadCost,
                                        ProfitCost = workItemPerProject.ProfitCost,
                                        ProjectType = workItemPerProject.ProjectType,
                                        Remark = workItemPerProject.Remark,
                                        LabourHourlyOutPut = workItemPerProject.LabourHourlyOutPut,
                                        UnitOfMeasurementId = workItemPerProject.UnitOfMeasurementId,
                                        OverheadRate = workItemPerProject.OverheadRate,
                                        ProfitMargin = workItemPerProject.ProfitMargin,
                                        CopiedFromId = workItemPerProject.WorkItemPerProjectId,

                                        ParentId = workItemPerProject.ParentId,
                                        OrgBranchName = workItemPerProject.OrgBranchName,
                                        DateCreated = workItemPerProject.DateCreated,
                                        LastModified = workItemPerProject.LastModified,
                                        CreatedBy = workItemPerProject.CreatedBy,
                                        ModifiedBy = workItemPerProject.ModifiedBy,
                                        ProjectInformationId = workItemPerProject.ProjectInformationId,
                                        WorkCategoryId = workItemPerProject.WorkCategoryId,


                                    };
                                    db.GenericWorkItemPerProject.Add(genericWorkItemToAdd);
                                    db.SaveChanges();

                                    #region add materials
                                    var materials = db.ProjectMaterial.Where(m => m.WorkItemPerProjectId == workItemPerProject.WorkItemPerProjectId).ToList();
                                    if (materials.Any())
                                    {
                                        foreach (ProjectMaterial material in materials)
                                        {
                                            var materialToAdd = new GenericProjectMaterial
                                            {
                                                ItemCode = material.ItemCode,
                                                ItemCategoryCode = material.ItemCategoryCode,
                                                Unit = material.Unit,
                                                Quantity = material.Quantity,
                                                UnitPrice = material.UnitPrice,
                                                TotalCost = material.TotalCost,
                                                MaterialUnitCost = material.MaterialUnitCost,
                                                OrgBranchName = material.OrgBranchName,
                                                DateCreated = material.DateCreated,
                                                LastModified = material.LastModified,
                                                CreatedBy = material.CreatedBy,
                                                ModifiedBy = material.ModifiedBy,
                                                GenericWorkItemPerProjectId = genericWorkItemToAdd.GenericWorkItemPerProjectId
                                            };
                                            db.GenericProjectMaterial.Add(materialToAdd);
                                            db.SaveChanges();
                                        }
                                    }
                                    #endregion
                                    #region add direct labor
                                    var directLabors = db.ProjectDirectLabour.Where(l => l.WorkItemPerProjectId == workItemPerProject.WorkItemPerProjectId).ToList();
                                    if (directLabors.Any())
                                    {
                                        foreach (ProjectDirectLabour labor in directLabors)
                                        {
                                            var laborToAdd = new GenericProjectDirectLabour
                                            {
                                                Unit = labor.Unit,
                                                Quantity = labor.Quantity,
                                                UnitFactory = labor.UnitFactory,
                                                IndexedHourlySalary = labor.IndexedHourlySalary,
                                                DailyPay = labor.DailyPay,
                                                TotalCost = labor.TotalCost,
                                                ManpowerUnitCost = labor.ManpowerUnitCost,
                                                OrgBranchName = labor.OrgBranchName,
                                                DateCreated = labor.DateCreated,
                                                LastModified = labor.LastModified,
                                                CreatedBy = labor.CreatedBy,
                                                ModifiedBy = labor.ModifiedBy,
                                                ProjectEstimationIndexLabourId = labor.ProjectEstimationIndexLabourId,
                                                GenericWorkItemPerProjectId = genericWorkItemToAdd.GenericWorkItemPerProjectId
                                            };
                                            db.GenericProjectDirectLabour.Add(laborToAdd);
                                            db.SaveChanges();
                                        }
                                    }
                                    #endregion
                                    #region add equipment
                                    var equipments = db.ProjectEquipment.Where(e => e.WorkItemPerProjectId == workItemPerProject.WorkItemPerProjectId).ToList();
                                    if (equipments.Any())
                                    {
                                        foreach (ProjectEquipment projectEquipment in equipments)
                                        {

                                            var equipmentToAdd = new GenericProjectEquipment
                                            {
                                                GenericWorkItemPerProjectId = genericWorkItemToAdd.GenericWorkItemPerProjectId,
                                                FleetsID = projectEquipment.FleetsID,
                                                Unit = projectEquipment.Unit,
                                                Quantity = projectEquipment.Quantity,
                                                UnitFactory = projectEquipment.UnitFactory,
                                                RentalCost = projectEquipment.RentalCost,
                                                TotalCost = projectEquipment.TotalCost,
                                                EquipmentUnitCost = projectEquipment.EquipmentUnitCost,
                                                EquipmentType = projectEquipment.EquipmentType,

                                                OrgBranchName = projectEquipment.OrgBranchName,
                                                DateCreated = projectEquipment.DateCreated,
                                                LastModified = projectEquipment.LastModified,
                                                CreatedBy = projectEquipment.CreatedBy,
                                                ModifiedBy = projectEquipment.ModifiedBy,
                                            };
                                            db.GenericProjectEquipment.Add(equipmentToAdd);
                                            db.SaveChanges();

                                        }
                                    }
                                    #endregion
                                    #endregion
                                }
                            }
                            var existingPieceRateUnderBlock = db.AreaWorkItemes.Where(aw => aw.BlockId == block.BlockId).ToList();
                            if (existingPieceRateUnderBlock.Any())
                            {
                                foreach (AreaWorkItem pieceRate in existingPieceRateUnderBlock)
                                {
                                    #region add piece rate
                                    var pieceRateToAdd = new GenericAreaWorkItem
                                    {
                                        WorkCategoryId = pieceRate.WorkCategoryId,
                                        Price = pieceRate.Price,
                                        WorkItemName = pieceRate.WorkItemName,
                                        ItemCode = pieceRate.ItemCode,
                                        DirectCost = pieceRate.DirectCost,
                                        Width = pieceRate.Width,
                                        Height = pieceRate.Height,
                                        Area = pieceRate.Area,
                                        CylinderLock = pieceRate.CylinderLock,
                                        OverHeadProfitCost = pieceRate.OverHeadProfitCost,
                                        ProjectInformationId = pieceRate.ProjectInformationId,
                                        MaterialCost = pieceRate.MaterialCost,
                                        TransportCost = pieceRate.TransportCost,
                                        SubProjectTypeId = subProjectTypeToAdd.SubProjectTypeId,

                                        OrgBranchName = pieceRate.OrgBranchName,
                                        DateCreated = pieceRate.DateCreated,
                                        LastModified = pieceRate.LastModified,
                                        CreatedBy = pieceRate.CreatedBy,
                                        ModifiedBy = pieceRate.ModifiedBy,
                                    };
                                    db.GenericAreaWorkItem.Add(pieceRateToAdd);
                                    db.SaveChanges();
                                    #endregion
                                }
                            }

                            var existingSubContractWorkItems = db.SubContractWorkItem.Where(scw => scw.BlockId == block.BlockId).ToList();
                            if (existingSubContractWorkItems.Any())
                            {
                                foreach (SubContractWorkItem subContractWorkItem in existingSubContractWorkItems)
                                {
                                    #region add sub-contract work item
                                    var subContractWorkItemToAdd = new GenericSubContractWorkItem
                                    {
                                        WorkCategoryId = subContractWorkItem.WorkCategoryId,
                                        UnitOfMeasurementId = subContractWorkItem.UnitOfMeasurementId,
                                        WorkItemName = subContractWorkItem.WorkItemName,
                                        ItemCode = subContractWorkItem.ItemCode,
                                        Quntity = subContractWorkItem.Quntity,
                                        SubContractCostOne = subContractWorkItem.SubContractCostOne,
                                        SubContractCostTwo = subContractWorkItem.SubContractCostTwo,
                                        SubContractCostThree = subContractWorkItem.SubContractCostThree,
                                        SelectedCost = subContractWorkItem.SelectedCost,
                                        TotalCost = subContractWorkItem.TotalCost,
                                        ProjectInformationId = subContractWorkItem.ProjectInformationId,
                                        SubProjectTypeId = subProjectTypeToAdd.SubProjectTypeId,

                                        OrgBranchName = subContractWorkItem.OrgBranchName,
                                        DateCreated = subContractWorkItem.DateCreated,
                                        LastModified = subContractWorkItem.LastModified,
                                        CreatedBy = subContractWorkItem.CreatedBy,
                                        ModifiedBy = subContractWorkItem.ModifiedBy
                                    };
                                    db.GenericSubContractWorkItem.Add(subContractWorkItemToAdd);
                                    db.SaveChanges();
                                    #endregion
                                }
                            }
                            var existingCustomWorkIems = db.ProjectCustomWorkItems.Where(cw => cw.BlockId == block.BlockId).ToList();
                            if (existingCustomWorkIems.Any())
                            {
                                foreach (ProjectCustomWorkItem customWorkItem in existingCustomWorkIems)
                                {
                                    #region add custom work items
                                    var customWorkItemToAdd = new GenericProjectCustomWorkItem
                                    {
                                        ProjectInformationId = project.ProjectInformationId,
                                        SubProjectTypeId = subProjectTypeToAdd.SubProjectTypeId,
                                        WorkCategoryId = customWorkItem.WorkCategoryId,
                                        ProjectType = customWorkItem.ProjectType,
                                        WorkItemName = customWorkItem.WorkItemName,
                                        TotalUnitRate = customWorkItem.TotalUnitRate,
                                        TotalQuantityOfWork = customWorkItem.Quantity,
                                        TotalCost = customWorkItem.TotalCost,
                                        Remark = customWorkItem.Remark,
                                        ItemCode = customWorkItem.ItemCode,

                                        OrgBranchName = customWorkItem.OrgBranchName,
                                        DateCreated = customWorkItem.DateCreated,
                                        LastModified = customWorkItem.LastModified,
                                        CreatedBy = customWorkItem.CreatedBy,
                                        ModifiedBy = customWorkItem.ModifiedBy,
                                    };
                                    db.GenericProjectCustomWorkItem.Add(customWorkItemToAdd);
                                    db.SaveChanges();
                                    #endregion
                                }
                            }
                        }
                    }

                    UpdateWorkItemsHeirarchyAfterMigration(project.ProjectInformationId);
                }
            }
        }
        private static void UpdateWorkItemsHeirarchyAfterMigration(int projectId)
        {
            ExceedDbContext db = new ExceedDbContext();
            var genericWorkItems = db.GenericWorkItemPerProject.Where(wi => wi.ProjectInformationId == projectId && wi.ParentId != 0).ToList();
            if (genericWorkItems.Any())
            {
                foreach (var workItem in genericWorkItems)
                {
                    var rootWorkItem = db.WorkItemPerProject.Find(workItem.CopiedFromId);
                    if (rootWorkItem != null)
                    {
                        var genericParent = db.GenericWorkItemPerProject
                            .FirstOrDefault(gw => gw.ProjectInformationId == projectId &&
                                            gw.SubProjectTypeId == workItem.SubProjectTypeId &&
                                            rootWorkItem.ParentId == gw.CopiedFromId);
                        if (genericParent != null)
                        {
                            //}
                            //var workItemParent = db.GenericWorkItemPerProject.FirstOrDefault(w => w.ProjectInformationId == projectId && w.SubProjectTypeId == workItem.SubProjectTypeId && w.ProjectEstimationWorkItemId == workItem.ParentId);

                            //if (workItemParent != null)
                            //{
                            workItem.ParentId = genericParent.GenericWorkItemPerProjectId;

                            db.GenericWorkItemPerProject.Attach(workItem);
                            db.Entry(workItem).State = EntityState.Modified;
                            db.SaveChanges();
                            //}
                        }
                    }
                }
            }
        }

        private void DeleteSubWorkItemsWhenParentWorkItemIsDeleted(int id)
        {
            var itemsToDelete = db.GenericWorkItemPerProject.Where(w => w.ParentId == id).ToList();
            if (itemsToDelete.Any())
            {
                db.GenericWorkItemPerProject.RemoveRange(itemsToDelete);
            }
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}