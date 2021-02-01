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
    public class CloneWorkItemPerProjectController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static string CurrentUSer { get; set; }
        public static int Category { get; set; }
        public static string OverHeadCost { get; set; }
        public static string ProfitMargin { get; set; }
        public static int ProjectFrom { get; set; }
        public static int ProjectTo { get; set; }

        public ActionResult Index()
        {
            db.Configuration.ProxyCreationEnabled = false;
            ConstructionInputs inputs = new ConstructionInputs();
            var lookups = db.Lookups.ToList();
            ViewData["Categories"] = db.WorkCategory.Select(wc => new { wc.Name, wc.ConfigurationId, wc.WorkCategoryId}).ToList();
            var units = lookups.Where(x => x.LookupType == LookUpTypes.UOM).ToList();
            ViewData["Unit"] = units;
            //var projects = lookups.Where(x => x.LookupType == LookUpTypes.PROJECT_TYPE).ToList();
            //ViewData["ProjectTypeList"] = projects;
            var items = db.InventoryItems.Select(i => new { i.Name, i.ItemCode}).ToList();
            ViewData["Items"] = items;
            var jobTitle = db.ProjectEstimationIndexLabour.ToList();
            ViewData["titles"] =
                jobTitle.Select(
                    x =>
                        new
                        {
                            ProjectEstimationIndexLabourId = x.ProjectEstimationIndexLabourId,
                            Title = GetTitle(x.ProjectJobTitleId)
                        }).ToList();
            var workItems = db.ProjectEstimationWorkItem.Select(i => new { i.ItemName, i.ProjectEstimationWorkItemId}).ToList();
            ViewData["WorkItems"] = workItems;
            SetDefaultValue();

            var internalEquipment = db.InternalMachinaryRent.ToList();
            var externalEquipment = db.MachinaryRent.ToList();
            var internalIds = new HashSet<Guid>(internalEquipment.Select(ic => ic.FleetsID));
            var externalIds = new HashSet<Guid>(externalEquipment.Select(ee => ee.FleetsID));
            var combinedEquipments = db.Fleets.Where(flt => (internalIds.Contains(flt.FleetsID)) || (externalIds.Contains(flt.FleetsID)));
            ViewData["Equipment"] = combinedEquipments.Select(eq => new { FleetsID = eq.FleetsID, EquipmentName = eq.EquipmentName });

            CurrentUSer = User.Identity.Name;
            ViewData["uom"] = db.UnitOfMeasurement.Select(x =>new{x.UnitOfMeasurementId,x.Abbreviation}).ToList();
            var types = db.EquipmentType.ToList();
            //ViewData["EquipmentType"] = types.Select(
            //    x =>
            //        new
            //        {
            //            EquipmentTypeName = x.EquipmentTypeName,
            //            EquipmentTypeID = x.EquipmentTypeID,
            //        }).ToList();

            //var categories = db.EquipmentIdentity.ToList();
            //ViewData["EquipmentIdentity"] = categories.Select(
            //    x =>
            //        new
            //        {
            //            EquipmentCategoryName = x.EquipmentCategoryName,
            //            EquipmentIdentityID = x.EquipmentIdentityID,
            //        }).ToList();
            var projectList =
                db.ProjectInformation.Where(x => x.ProjectStatus == ProjectStatus.ESTIMATION).OrderBy(x => x.Name);
            ViewData["ProjectList"] = projectList;
            var approvedProjectList =
                db.ProjectInformation.Where(
                    x =>
                        x.ProjectStatus != ProjectStatus.ESTIMATION && x.ProjectStatus != ProjectStatus.WAITING_APPROVAL)
                    .OrderBy(x => x.Name);
            ViewData["ApprovedProjectList"] = approvedProjectList;

            var itemCategories = db.ItemCategorys.Select(c => new { c.ItemCategoryCode, c.Name}).ToList();
            ViewData["ItemCategorys"] =
                itemCategories;
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

        public void SetProject(int to)
        {
            //ProjectFrom = 0;
            //ProjectFrom = from;
            ProjectTo = 0;
            ProjectTo = to;
            ProjectEstimationCopyController.ProjectId = to;

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

        public ActionResult WorkItemPerProject_Read([DataSourceRequest] DataSourceRequest request)
        {
            IQueryable<WorkItemPerProject> workitemperproject =
                db.WorkItemPerProject.Where(x => x.ProjectInformationId == ProjectTo && x.ParentId == 0);
            DataSourceResult result = workitemperproject.ToDataSourceResult(request, workItemPerProject => new
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
                WorkCategoryId = workItemPerProject.WorkCategoryId
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WorkItemPerProject_Create([DataSourceRequest] DataSourceRequest request,
            WorkItemPerProject workItemPerProject)
        {
            if (ModelState.IsValid)
            {
                var entity = new WorkItemPerProject
                {
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
                    DateCreated = DateTime.Now,
                    LastModified = workItemPerProject.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = workItemPerProject.ModifiedBy,
                    ProjectInformationId = ProjectTo,
                    WorkCategoryId = workItemPerProject.WorkCategoryId,

                };

                db.WorkItemPerProject.Add(entity);
                db.SaveChanges();
                workItemPerProject.WorkItemPerProjectId = entity.WorkItemPerProjectId;
            }

            return Json(new[] { workItemPerProject }.ToDataSourceResult(request, ModelState));
        }

        //public JsonResult GetApprovedProjects(int ProjectInformationIdTwo)
        //{
        //    var items = new List<ProjectInformation>();
        //    try
        //    {
        //        var types =
        //            db.ProjectInformation.Where(x => x.ProjectInformationId == ProjectInformationIdTwo).FirstOrDefault();

        //        if (types != null)
        //        {

        //            items =
        //                db.ProjectInformation.Where(
        //                    x => x.ProjectStatus == ProjectStatus.APPROVED && x.ProjectTypeId == types.ProjectTypeId)
        //                    .ToList();

        //        }

        //        return Json(items.Select(p => new { Name = p.Name, ProjectInformationIdTwo = p.ProjectInformationId }),
        //            JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception)
        //    {

        //        // throw;
        //    }
        //    return Json(items.Select(p => new { Name = p.Name, ProjectInformationId = p.ProjectInformationId }),
        //        JsonRequestBehavior.AllowGet);

        //}

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WorkItemPerProject_Update([DataSourceRequest] DataSourceRequest request,
            WorkItemPerProject workItemPerProject)
        {
            if (ModelState.IsValid)
            {
                var entity = new WorkItemPerProject
                {
                    ProjectEstimationWorkItemId = workItemPerProject.ProjectEstimationWorkItemId,
                    WorkItemPerProjectId = workItemPerProject.WorkItemPerProjectId,
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
                    LastModified = DateTime.Now,
                    CreatedBy = workItemPerProject.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    ProjectInformationId = workItemPerProject.ProjectInformationId,
                    WorkCategoryId = workItemPerProject.WorkCategoryId
                };

                db.WorkItemPerProject.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { workItemPerProject }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WorkItemPerProject_Destroy([DataSourceRequest] DataSourceRequest request,
            WorkItemPerProject workItemPerProject)
        {
            if (ModelState.IsValid)
            {
                var entity = new WorkItemPerProject
                {
                    ProjectEstimationWorkItemId = workItemPerProject.ProjectEstimationWorkItemId,
                    WorkItemPerProjectId = workItemPerProject.WorkItemPerProjectId,
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
                    WorkCategoryId = workItemPerProject.WorkCategoryId
                };

                db.WorkItemPerProject.Attach(entity);
                db.WorkItemPerProject.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { workItemPerProject }.ToDataSourceResult(request, ModelState));
        }

        public bool CheckCopied(int to)
        {
            var Check = db.GenericWorkItemPerProject.Where(x => x.ProjectInformationId == to).ToList();
            if (Check.Any())
            {
                return true;
            }
            return false;
        }
        public string GetTaregtProjectType(int targetProjectId)
        {
            var currentProject = db.ProjectInformation.Find(targetProjectId);
            var projectTypeName = "";
            if (currentProject != null)
            {
                //var currentProjectsType = db.ProjectTypes.Find(currentProject.ProjectTypeId);
                //if (currentProjectsType != null)
                //{
                projectTypeName = currentProject.ProjectType.ToString();
                //}
            }
            return projectTypeName.ToLower();
        }
        public bool WorkItemExistsInTargetBlock(GenericWorkItemPerProject model, SubProjectStructureTypeMapping bMapping)
        {
            var dbInstance = new ExceedDbContext();
            var workItemExists = false;
            var tempList = dbInstance.GenericWorkItemPerProject.Where(
                wi => wi.WorkCategory.ConfigurationId == bMapping.ConfigurationId &&
                        wi.ItemName == model.ItemName &&
                        wi.SubProjectTypeId == bMapping.SubProjectTypeId).ToList();
            if (tempList.Any())
            {
                workItemExists = true;
            }
            return workItemExists;
        }
        public bool PieceRateWorkItemExistsInTargetBlock(GenericAreaWorkItem model, SubProjectStructureTypeMapping bMapping)
        {
            var dbInstance = new ExceedDbContext();
            var pieceRateWorkItemExists = false;
            var tempList = dbInstance.GenericAreaWorkItem.Where(
                wi => wi.WorkCategory.ConfigurationId == bMapping.ConfigurationId &&
                        wi.WorkItemName == model.WorkItemName &&
                        wi.SubProjectTypeId == bMapping.SubProjectTypeId).ToList();
            if (tempList.Any())
            {
                pieceRateWorkItemExists = true;
            }
            return pieceRateWorkItemExists;
        }
        public bool SubContractWorkItemExistsInTargetBlock(GenericSubContractWorkItem model, SubProjectStructureTypeMapping bMapping)
        {
            var dbInstance = new ExceedDbContext();
            var subContractWorkItemExists = false;
            var tempList = dbInstance.GenericSubContractWorkItem.Where(
                wi => wi.WorkCategory.ConfigurationId == bMapping.ConfigurationId &&
                        wi.WorkItemName == model.WorkItemName &&
                        wi.SubProjectTypeId == bMapping.SubProjectTypeId).ToList();
            if (tempList.Any())
            {
                subContractWorkItemExists = true;
            }
            return subContractWorkItemExists;
        }
        public bool CustomWorkItemExistsInTargetBlock(GenericProjectCustomWorkItem model, SubProjectStructureTypeMapping bMapping)
        {
            var dbInstance = new ExceedDbContext();
            var customWorkItemExists = false;
            var tempList = dbInstance.GenericProjectCustomWorkItem.Where(
                wi =>
                        wi.WorkItemName == model.WorkItemName &&
                        wi.SubProjectTypeId == bMapping.SubProjectTypeId).ToList();
            if (tempList.Any())
            {
                foreach (var wi in tempList)
                {
                    var strType = ProjectEstimationHelper.GetCustomWorkItemCategory(wi.WorkCategoryId).ConfigurationId;
                    if (strType == bMapping.ConfigurationId)
                    {
                        customWorkItemExists = true;
                    }
                }

            }
            return customWorkItemExists;
        }
        /// <summary>
        /// clone estimation from a project into a project with 
        /// defined blocks
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public ActionResult CloneEstimation(int from, int to)
        {
            var estimationWorkItems = db.GenericWorkItemPerProject.Where(x => x.ProjectInformationId == from).ToList();
            var estimationPieceRateWorkItems = db.GenericAreaWorkItem.Where(x => x.ProjectInformationId == from).ToList();
            var estimationSubContractWorkItems =
                db.GenericSubContractWorkItem.Where(x => x.ProjectInformationId == from).ToList();
            //var customWorkItems = db.GenericProjectCustomWorkItem.Where(cw => cw.ProjectInformationId == from).ToList();

            GenericWorkItemPerProject workItemPerProject = new GenericWorkItemPerProject();
            GenericAreaWorkItem areaWorkItem = new GenericAreaWorkItem();
            GenericSubContractWorkItem subContractWorkItem = new GenericSubContractWorkItem();
            //GenericProjectCustomWorkItem customWorkItem = new GenericProjectCustomWorkItem();

            var subProjectTypes = db.SubProjectType.Where(x => x.ProjectInformationId == to).ToList();
            var Check = db.GenericWorkItemPerProject.Where(x => x.ProjectInformationId == to).ToList();
            var CheckPieceRate = db.GenericAreaWorkItem.Where(x => x.ProjectInformationId == to).ToList();
            var CheckSubContract = db.GenericSubContractWorkItem.Where(x => x.ProjectInformationId == to).ToList();
            //var existingCustomWorkItems = db.GenericProjectCustomWorkItem.Where(x => x.ProjectInformationId == to).ToList();

            #region remove existing data ...
            if (Check.Any())
            {
                db.GenericWorkItemPerProject.RemoveRange(Check);
                db.SaveChanges();
            }
            if (CheckPieceRate.Any())
            {
                db.GenericAreaWorkItem.RemoveRange(CheckPieceRate);
                db.SaveChanges();
            }

            if (CheckSubContract.Any())
            {
                db.GenericSubContractWorkItem.RemoveRange(CheckSubContract);
                db.SaveChanges();

            }
            //if (existingCustomWorkItems.Any())
            //{
            //    db.GenericProjectCustomWorkItem.RemoveRange(existingCustomWorkItems);
            //    db.SaveChanges();
            //}
            #endregion

            if (estimationWorkItems.Any() || estimationPieceRateWorkItems.Any() || estimationSubContractWorkItems.Any())
            {
                if (subProjectTypes.Any())
                {
                    #region With sub-project type

                    #region piece Work Item

                    foreach (var subProjectType in subProjectTypes)
                    {
                        var mappings = db.SubProjectStructureTypeMapping.Where(x => x.SubProjectTypeId == subProjectType.SubProjectTypeId);
                        foreach (var m in mappings)
                        {
                            foreach (var p in estimationPieceRateWorkItems)
                            {

                                if (m.ConfigurationId == p.WorkCategory.ConfigurationId)
                                {
                                    if (!PieceRateWorkItemExistsInTargetBlock(p, m))
                                    {
                                        areaWorkItem = new GenericAreaWorkItem();
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
                                        areaWorkItem.ProjectInformationId = to;
                                        areaWorkItem.SubProjectTypeId = subProjectType.SubProjectTypeId;
                                        ExceedDbContext WorkItemdb = new ExceedDbContext();
                                        WorkItemdb.GenericAreaWorkItem.Add(areaWorkItem);
                                        WorkItemdb.SaveChanges();
                                    }
                                }
                            }
                        }
                    }

                    #endregion

                    #region Sub contract Work Item

                    foreach (var subProjectType in subProjectTypes)
                    {
                        var mappings = db.SubProjectStructureTypeMapping.Where(x => x.SubProjectTypeId == subProjectType.SubProjectTypeId);
                        foreach (var m in mappings)
                        {
                            foreach (var s in estimationSubContractWorkItems)
                            {

                                if (m.ConfigurationId == s.WorkCategory.ConfigurationId)
                                {
                                    if (!SubContractWorkItemExistsInTargetBlock(s, m))
                                    {
                                        subContractWorkItem = new GenericSubContractWorkItem();
                                        subContractWorkItem.UnitOfMeasurementId = s.UnitOfMeasurementId;
                                        subContractWorkItem.OverHeadProfit = s.OverHeadProfit;
                                        subContractWorkItem.Quntity = s.Quntity;
                                        subContractWorkItem.SelectedCost = s.SelectedCost;
                                        subContractWorkItem.SubContractCostOne = s.SubContractCostOne;
                                        subContractWorkItem.SubContractCostTwo = s.SubContractCostTwo;
                                        subContractWorkItem.SubContractCostThree = s.SubContractCostThree;
                                        subContractWorkItem.TotalCost = s.TotalCost;
                                        subContractWorkItem.WorkCategoryId = s.WorkCategoryId;
                                        subContractWorkItem.WorkItemName = s.WorkItemName;
                                        subContractWorkItem.ItemCode = s.ItemCode;
                                        subContractWorkItem.ProjectInformationId = to;
                                        subContractWorkItem.SubProjectTypeId = subProjectType.SubProjectTypeId;
                                        subContractWorkItem.OverHeadProfit = s.OverHeadProfit;
                                        ExceedDbContext WorkItemdb = new ExceedDbContext();
                                        WorkItemdb.GenericSubContractWorkItem.Add(subContractWorkItem);
                                        WorkItemdb.SaveChanges();
                                    }
                                }
                            }
                        }
                    }

                    #endregion

                    #region custom work items
                    //foreach (var subProjectType in subProjectTypes)
                    //{
                    //    var mappings = db.SubProjectStructureTypeMapping.Where(x => x.SubProjectTypeId == subProjectType.SubProjectTypeId);
                    //    foreach (var m in mappings)
                    //    {
                    //        foreach (var cw in customWorkItems)
                    //        {

                    //            if (m.ConfigurationId == ProjectEstimationHelper.GetCustomWorkItemCategory(cw.WorkCategoryId).ConfigurationId)
                    //            {
                    //                if (!CustomWorkItemExistsInTargetBlock(cw, m))
                    //                {
                    //                    customWorkItem = new GenericProjectCustomWorkItem();
                    //                    customWorkItem.WorkCategoryId = cw.WorkCategoryId;
                    //                    customWorkItem.WorkItemName = cw.WorkItemName;
                    //                    customWorkItem.ItemCode = cw.ItemCode;
                    //                    customWorkItem.ProjectInformationId = to;
                    //                    customWorkItem.ProjectType = cw.ProjectType;
                    //                    customWorkItem.TotalUnitRate = cw.TotalUnitRate;
                    //                    customWorkItem.SubProjectTypeId = subProjectType.SubProjectTypeId;
                    //                    customWorkItem.TotalQuantityOfWork = cw.TotalQuantityOfWork;
                    //                    customWorkItem.TotalCost = cw.TotalUnitRate;
                    //                    customWorkItem.Remark = cw.Remark;

                    //                    customWorkItem.HourlyOutPut = cw.HourlyOutPut;
                    //                    customWorkItem.UnitOfMeasurementId = cw.UnitOfMeasurementId;
                    //                    customWorkItem.MaterialCostPerUnit = cw.MaterialCostPerUnit;
                    //                    customWorkItem.EquipmentCostPerUnit = cw.EquipmentCostPerUnit;
                    //                    customWorkItem.LaborCostPerUnit = cw.LaborCostPerUnit;
                    //                    customWorkItem.DirectCost = cw.DirectCost;
                    //                    customWorkItem.ProfitRate = cw.ProfitRate;
                    //                    customWorkItem.OverheadRate = cw.OverheadRate;
                    //                    customWorkItem.ProfitCost = cw.ProfitCost;
                    //                    customWorkItem.OverHeadCost = cw.OverHeadCost;

                    //                    ExceedDbContext WorkItemdb = new ExceedDbContext();
                    //                    WorkItemdb.GenericProjectCustomWorkItem.Add(customWorkItem);
                    //                    WorkItemdb.SaveChanges();

                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                    #endregion
                    #region Work item
                    foreach (var subProjectType in subProjectTypes)
                    {
                        var mappings = db.SubProjectStructureTypeMapping.Where(x => x.SubProjectTypeId == subProjectType.SubProjectTypeId);
                        foreach (var m in mappings)
                        {
                            foreach (var p in estimationWorkItems)
                            {

                                if (m.ConfigurationId == p.WorkCategory.ConfigurationId)
                                {
                                    // check if the work item is copied already ...
                                    //var tempWorkItem = new ExceedDbContext().WorkItemPerProject.Where(
                                    //    w => w.WorkCategory.ConfigurationId == p.WorkCategory.ConfigurationId && w.ItemName == p.ItemName);
                                    if (!WorkItemExistsInTargetBlock(p, m))
                                    {
                                        //continue;
                                        // work item exixts .. so jump this work item and go to the next one ..

                                        //foreach (var e in estimationWorkItems)
                                        //{
                                        workItemPerProject = new GenericWorkItemPerProject();
                                        workItemPerProject.ProjectInformationId = to;
                                        workItemPerProject.IsSubCategory = p.IsSubCategory;
                                        workItemPerProject.ItemCode = p.ItemCode;
                                        workItemPerProject.ContainsWorkItemAsInput = p.ContainsWorkItemAsInput;
                                        workItemPerProject.CopiedFromId = p.GenericWorkItemPerProjectId;
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
                                        workItemPerProject.SubProjectTypeId = subProjectType.SubProjectTypeId;
                                        workItemPerProject.LabourHourlyOutPut = p.LabourHourlyOutPut;
                                        workItemPerProject.UnitOfMeasurementId = p.UnitOfMeasurementId;
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
                                        WorkItemdb.GenericWorkItemPerProject.Add(workItemPerProject);
                                        WorkItemdb.SaveChanges();

                                        #region Copy custom content(if any)
                                        if (p.ContainsWorkItemAsInput)
                                        {
                                            var customWorkItemContents = db.GenericProjectCustomWorkItemDetails.Where(
                                            cwd => cwd.GenericWorkItemPerProjectId == p.GenericWorkItemPerProjectId).ToList();
                                            if (customWorkItemContents.Any())
                                            {
                                                foreach (GenericProjectCustomWorkItemDetail customMaterial in customWorkItemContents)
                                                {
                                                    var customMaterialToAdd = new GenericProjectCustomWorkItemDetail
                                                    {
                                                        GenericWorkItemPerProjectId = workItemPerProject.GenericWorkItemPerProjectId,
                                                        ProjectEstimationWorkItemId = customMaterial.ProjectEstimationWorkItemId,
                                                        QuantityPerUnit = customMaterial.QuantityPerUnit,
                                                        UnitCost = customMaterial.UnitCost,
                                                        TotalCost = customMaterial.TotalCost,

                                                        CreatedBy = User.Identity.Name,
                                                        DateCreated = DateTime.Now,
                                                        ModifiedBy = User.Identity.Name,
                                                        LastModified = DateTime.Now
                                                    };
                                                    WorkItemdb.GenericProjectCustomWorkItemDetails.Add(customMaterialToAdd);
                                                    WorkItemdb.SaveChanges();
                                                }
                                            }
                                        }
                                        #endregion

                                        #region Copy Material

                                        var etsmnMaterial =
                                            db.ProjectEstimationMaterial.Where(
                                                em =>
                                                    em.ProjectEstimationWorkItemId ==
                                                    p.ProjectEstimationWorkItemId).ToList();
                                        if (etsmnMaterial.Any())
                                        {
                                            foreach (var material in etsmnMaterial)
                                            {
                                                var entity = new GenericProjectMaterial
                                                {
                                                    Unit = material.Unit,
                                                    Quantity = material.Quantity,
                                                    ItemCategoryCode = material.ItemCategoryCode,
                                                    ItemCode = material.ItemCode,
                                                    UnitPrice = material.UnitPrice,
                                                    TotalCost = material.TotalCost,
                                                    MaterialUnitCost = material.MaterialUnitCost,
                                                    GenericWorkItemPerProjectId = workItemPerProject.GenericWorkItemPerProjectId,
                                                    OrgBranchName = material.OrgBranchName,
                                                    DateCreated = DateTime.Now,
                                                    LastModified = null,
                                                    CreatedBy = User.Identity.Name,
                                                    ModifiedBy = null,

                                                };
                                                WorkItemdb = new ExceedDbContext();
                                                WorkItemdb.GenericProjectMaterial.Add(entity);
                                                WorkItemdb.SaveChanges();

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
                                            foreach (var labor in etsmnLabour)
                                            {
                                                var entity = new GenericProjectDirectLabour
                                                {
                                                    Unit = labor.Unit,
                                                    Quantity = labor.Quantity,
                                                    UnitFactory = labor.UnitFactory,
                                                    IndexedHourlySalary = labor.IndexedHourlySalary,
                                                    DailyPay = labor.DailyPay,
                                                    TotalCost = labor.TotalCost,
                                                    ManpowerUnitCost = labor.ManpowerUnitCost,
                                                    OrgBranchName = labor.OrgBranchName,
                                                    DateCreated = DateTime.Now,
                                                    LastModified = null,
                                                    CreatedBy = User.Identity.Name,
                                                    ModifiedBy = null,
                                                    GenericWorkItemPerProjectId =
                                                        workItemPerProject.GenericWorkItemPerProjectId,
                                                    ProjectEstimationIndexLabourId =
                                                        labor.ProjectEstimationIndexLabourId
                                                };
                                                WorkItemdb = new ExceedDbContext();
                                                WorkItemdb.GenericProjectDirectLabour.Add(entity);
                                                WorkItemdb.SaveChanges();

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
                                            foreach (var equipment in etsmnEquipment)
                                            {
                                                var entity = new GenericProjectEquipment
                                                {
                                                    //EquipmentTypeID = m.EquipmentTypeID,
                                                    //EquipmentIdentityID = m.EquipmentIdentityID,
                                                    FleetsID = equipment.FleetsID,
                                                    Unit = equipment.Unit,
                                                    Quantity = equipment.Quantity,
                                                    UnitFactory = equipment.UnitFactory,
                                                    RentalCost = equipment.RentalCost,
                                                    TotalCost = equipment.TotalCost,
                                                    EquipmentUnitCost = equipment.EquipmentUnitCost,
                                                    EquipmentType = equipment.EquipmentType,
                                                    OrgBranchName = equipment.OrgBranchName,
                                                    DateCreated = DateTime.Now,
                                                    LastModified = null,
                                                    CreatedBy = User.Identity.Name,
                                                    ModifiedBy = null,
                                                    GenericWorkItemPerProjectId =
                                                        workItemPerProject.GenericWorkItemPerProjectId,

                                                };

                                                WorkItemdb = new ExceedDbContext();
                                                WorkItemdb.GenericProjectEquipment.Add(entity);
                                                WorkItemdb.SaveChanges();

                                            }
                                        }

                                        #endregion

                                        #region Copy Takeoff sheet

                                        //var etsmnTakeOfSheet =
                                        //    db.TakeOffPerProject.Where(
                                        //        x => x.WorkItemPerProjectId == p.WorkItemPerProjectId).ToList();
                                        //TakeOffPerProject takeOffPerProject = new TakeOffPerProject();
                                        //if (etsmnTakeOfSheet.Any())
                                        //{
                                        //    foreach (var m in etsmnTakeOfSheet)
                                        //    {
                                        //        takeOffPerProject = new TakeOffPerProject();
                                        //        takeOffPerProject = m;
                                        //        takeOffPerProject.OrgBranchName = m.OrgBranchName;
                                        //        takeOffPerProject.DateCreated = DateTime.Now;
                                        //        takeOffPerProject.LastModified = null;
                                        //        takeOffPerProject.CreatedBy = User.Identity.Name;
                                        //        takeOffPerProject.ModifiedBy = null;
                                        //        takeOffPerProject.WorkItemPerProjectId =
                                        //            workItemPerProject.WorkItemPerProjectId;
                                        //        WorkItemdb = new ExceedDbContext();
                                        //        WorkItemdb.TakeOffPerProject.Add(takeOffPerProject);
                                        //        WorkItemdb.SaveChanges();
                                        //        takeOffPerProject.TakeOffPerProjectId =
                                        //            takeOffPerProject.TakeOffPerProjectId;

                                        //        #region Takeoff Line

                                        //        var takeoffline =
                                        //            db.TakeOffLinePerProject.Where(
                                        //                x => x.TakeOffPerProjectId == m.TakeOffPerProjectId)
                                        //                .ToList();
                                        //        if (takeoffline.Any())
                                        //        {
                                        //            foreach (var t in takeoffline)
                                        //            {
                                        //                var entityi = new TakeOffLinePerProject();
                                        //                entityi = t;
                                        //                entityi.OrgBranchName = t.OrgBranchName;
                                        //                entityi.DateCreated = DateTime.Now;
                                        //                entityi.LastModified = null;
                                        //                entityi.CreatedBy = User.Identity.Name;
                                        //                entityi.ModifiedBy = null;
                                        //                entityi.TakeOffLinePerProjectId =
                                        //                    takeOffPerProject.TakeOffPerProjectId;
                                        //                WorkItemdb = new ExceedDbContext();
                                        //                WorkItemdb.TakeOffLinePerProject.Add(entityi);
                                        //                WorkItemdb.SaveChanges();

                                        //            }
                                        //        }

                                        //        #endregion

                                        //        #region Takeoff Line per Kg

                                        //        var takeofflineKg =
                                        //            db.TakeOffLineForKgPerProject.Where(
                                        //                x => x.TakeOffPerProjectId == m.TakeOffPerProjectId).ToList();
                                        //        if (takeofflineKg.Any())
                                        //        {
                                        //            foreach (var t in takeofflineKg)
                                        //            {
                                        //                var entityy = new TakeOffLineForKgPerProject();

                                        //                entityy = t;
                                        //                entityy.OrgBranchName = t.OrgBranchName;
                                        //                entityy.DateCreated = DateTime.Now;
                                        //                entityy.LastModified = null;
                                        //                entityy.CreatedBy = User.Identity.Name;
                                        //                entityy.ModifiedBy = null;
                                        //                entityy.TakeOffPerProjectId =
                                        //                    takeOffPerProject.TakeOffPerProjectId;

                                        //                WorkItemdb = new ExceedDbContext();
                                        //                WorkItemdb.TakeOffLineForKgPerProject.Add(entityy);
                                        //                WorkItemdb.SaveChanges();

                                        //            }
                                        //        }

                                        //        #endregion
                                        //    }
                                        //}

                                        #endregion

                                    }
                                }
                            }
                        }
                    }

                    #endregion

                    #endregion
                }


                else
                {
                    #region With out Block

                    //foreach (var p in estimationPieceRateWorkItems)
                    //{
                    //    areaWorkItem = new AreaWorkItem();
                    //    areaWorkItem.Width = p.Width;
                    //    areaWorkItem.Height = p.Height;
                    //    areaWorkItem.Area = p.Area;
                    //    areaWorkItem.CylinderLock = p.CylinderLock;
                    //    areaWorkItem.DirectCost = p.DirectCost;
                    //    areaWorkItem.MaterialCost = p.MaterialCost;
                    //    areaWorkItem.OverHeadProfitCost = p.OverHeadProfitCost;
                    //    areaWorkItem.Price = p.Price;
                    //    areaWorkItem.WorkCategoryId = p.WorkCategoryId;
                    //    areaWorkItem.ProjectInformationId = to;
                    //    ExceedDbContext WorkItemdb = new ExceedDbContext();
                    //    WorkItemdb.AreaWorkItemes.Add(areaWorkItem);
                    //    WorkItemdb.SaveChanges();
                    //    areaWorkItem.ProjectEstimationAreaWorkItemId = areaWorkItem.ProjectEstimationAreaWorkItemId;
                    //}

                    //foreach (var s in estimationSubContractWorkItems)
                    //{
                    //    subContractWorkItem = new SubContractWorkItem();
                    //    subContractWorkItem.UnitOfMeasurementId = s.UnitOfMeasurementId;
                    //    subContractWorkItem.OverHeadProfit = s.OverHeadProfit;
                    //    subContractWorkItem.Quntity = s.Quntity;
                    //    subContractWorkItem.SelectedCost = s.SelectedCost;
                    //    subContractWorkItem.SubContractCostOne = s.SubContractCostOne;
                    //    subContractWorkItem.SubContractCostTwo = s.SubContractCostTwo;
                    //    subContractWorkItem.SubContractCostThree = s.SubContractCostThree;
                    //    subContractWorkItem.EstimationSubContractWorkItemId = s.EstimationSubContractWorkItemId;
                    //    subContractWorkItem.TotalCost = s.TotalCost;
                    //    subContractWorkItem.WorkCategoryId = s.WorkCategoryId;
                    //    subContractWorkItem.WorkItemName = s.WorkItemName;
                    //    subContractWorkItem.ProjectInformationId = to;
                    //    ExceedDbContext WorkItemdb = new ExceedDbContext();
                    //    WorkItemdb.SubContractWorkItem.Add(subContractWorkItem);
                    //    WorkItemdb.SaveChanges();
                    //    subContractWorkItem.EstimationSubContractWorkItemId = subContractWorkItem.EstimationSubContractWorkItemId;
                    //}
                    //foreach (var e in estimationWorkItems)
                    //{

                    //    var entites = new WorkItemPerProject
                    //    {


                    //        WorkCategoryId = e.WorkCategoryId,
                    //        UnitOfMeasurementId = e.UnitOfMeasurementId,
                    //        ProjectEstimationWorkItemId = e.ProjectEstimationWorkItemId,
                    //        TotalQuantityOfWork = e.TotalQuantityOfWork,
                    //        LabourOutPut = e.LabourOutPut,
                    //        EquipmentOutput = e.EquipmentOutput,
                    //        MaterialCost = e.MaterialCost,
                    //        Remark = e.Remark,
                    //        OverheadRate = e.OverheadRate,
                    //        ProfitMargin = e.ProfitMargin,
                    //        ProjectTypeId = e.ProjectTypeId,
                    //        LabourHourlyOutPut = e.LabourHourlyOutPut,
                    //        DirectCost = e.DirectCost,
                    //        OverHeadCost = e.OverHeadCost,
                    //        ProfitCost = e.ProfitCost,
                    //        ProjectInformationId = to,
                    //        WorkItemPerProjectId = 0,
                    //        OrgBranchName = e.OrgBranchName,
                    //        LastModified = null,
                    //        ModifiedBy = null,
                    //        CreatedBy = User.Identity.Name,
                    //        DateCreated = DateTime.Now
                    //    };
                    //    ExceedDbContext WorkItemdb = new ExceedDbContext();
                    //    WorkItemdb.WorkItemPerProject.Add(entites);
                    //    WorkItemdb.SaveChanges();
                    //    workItemPerProject.WorkItemPerProjectId = entites.WorkItemPerProjectId;

                    //    #region Copy Material

                    //    var etsmnMaterial =
                    //        db.ProjectMaterial.Where(x => x.WorkItemPerProjectId == e.WorkItemPerProjectId).ToList();
                    //    if (etsmnMaterial.Any())
                    //    {
                    //        foreach (var m in etsmnMaterial)
                    //        {
                    //            var entity = new ProjectMaterial
                    //            {
                    //                ItemCode = m.ItemCode,
                    //                ItemCategoryCode = m.ItemCategoryCode,
                    //                MaterialUnitCost = m.MaterialUnitCost,
                    //                Quantity = m.Quantity,
                    //                TotalCost = m.TotalCost,
                    //                Unit = m.Unit,
                    //                UnitPrice = m.UnitPrice,
                    //                OrgBranchName = m.OrgBranchName,
                    //                DateCreated = DateTime.Now,
                    //                LastModified = null,
                    //                CreatedBy = User.Identity.Name,
                    //                ModifiedBy = null,
                    //                WorkItemPerProjectId = workItemPerProject.WorkItemPerProjectId
                    //            };

                    //            WorkItemdb = new ExceedDbContext();
                    //            WorkItemdb.ProjectMaterial.Add(entity);
                    //            WorkItemdb.SaveChanges();
                    //            //projectMaterial.ProjectMaterialId = entity.ProjectMaterialId;
                    //        }
                    //    }

                    //    #endregion

                    //    #region Copy Labour Cost

                    //    var etsmnLabour =
                    //        db.ProjectDirectLabour.Where(x => x.WorkItemPerProjectId == e.WorkItemPerProjectId).ToList();
                    //    if (etsmnLabour.Any())
                    //    {
                    //        foreach (var m in etsmnLabour)
                    //        {
                    //            var entity = new ProjectDirectLabour
                    //            {
                    //                IndexedHourlySalary = m.IndexedHourlySalary,
                    //                DailyPay = m.DailyPay,
                    //                ManpowerUnitCost = m.ManpowerUnitCost,
                    //                ProjectEstimationIndexLabourId = m.ProjectEstimationIndexLabourId,
                    //                Quantity = m.Quantity,
                    //                TotalCost = m.TotalCost,
                    //                Unit = m.Unit,
                    //                UnitFactory = m.UnitFactory,
                    //                OrgBranchName = m.OrgBranchName,
                    //                DateCreated = DateTime.Now,
                    //                LastModified = null,
                    //                CreatedBy = User.Identity.Name,
                    //                ModifiedBy = null,
                    //                WorkItemPerProjectId = workItemPerProject.WorkItemPerProjectId
                    //            };
                    //            WorkItemdb = new ExceedDbContext();
                    //            WorkItemdb.ProjectDirectLabour.Add(entity);
                    //            WorkItemdb.SaveChanges();
                    //            //projectDirectLabour.ProjectDirectLabourId = entity.ProjectDirectLabourId;
                    //        }
                    //    }

                    //    #endregion

                    //    #region Copy Equipment

                    //    var etsmnEquipment =
                    //        db.ProjectEquipment.Where(x => x.WorkItemPerProjectId == e.WorkItemPerProjectId).ToList();
                    //    if (etsmnEquipment.Any())
                    //    {
                    //        foreach (var m in etsmnEquipment)
                    //        {
                    //            var entity = new ProjectEquipment
                    //            {
                    //                EquipmentTypeID = m.EquipmentTypeID,
                    //                EquipmentIdentityID = m.EquipmentIdentityID,
                    //                AssetNumber = m.AssetNumber,
                    //                EquipmentType = m.EquipmentType,
                    //                EquipmentUnitCost = m.EquipmentUnitCost,
                    //                Quantity = m.Quantity,
                    //                RentalCost = m.RentalCost,
                    //                TotalCost = m.TotalCost,
                    //                Unit = m.Unit,
                    //                UnitFactory = m.UnitFactory,

                    //                OrgBranchName = m.OrgBranchName,
                    //                DateCreated = DateTime.Now,
                    //                LastModified = null,
                    //                CreatedBy = User.Identity.Name,
                    //                ModifiedBy = null,
                    //                WorkItemPerProjectId = workItemPerProject.WorkItemPerProjectId
                    //            };
                    //            db = new ExceedDbContext();
                    //            db.ProjectEquipment.Add(entity);
                    //            db.SaveChanges();
                    //            //projectEquipment.ProjectEquipmentId = entity.ProjectEquipmentId;
                    //        }
                    //    }


                    //    #endregion

                    //    #region Copy Takeoff sheet

                    //    var etsmnTakeOfSheet =
                    //        db.TakeOffPerProject.Where(x => x.WorkItemPerProjectId == e.WorkItemPerProjectId).ToList();
                    //    TakeOffPerProject takeOffPerProject = new TakeOffPerProject();
                    //    if (etsmnTakeOfSheet.Any())
                    //    {
                    //        foreach (var m in etsmnTakeOfSheet)
                    //        {
                    //            takeOffPerProject = new TakeOffPerProject();
                    //            takeOffPerProject = m;
                    //            takeOffPerProject.OrgBranchName = m.OrgBranchName;
                    //            takeOffPerProject.DateCreated = DateTime.Now;
                    //            takeOffPerProject.LastModified = null;
                    //            takeOffPerProject.CreatedBy = User.Identity.Name;
                    //            takeOffPerProject.ModifiedBy = null;
                    //            takeOffPerProject.WorkItemPerProjectId = workItemPerProject.WorkItemPerProjectId;
                    //            WorkItemdb = new ExceedDbContext();
                    //            WorkItemdb.TakeOffPerProject.Add(takeOffPerProject);
                    //            WorkItemdb.SaveChanges();
                    //            takeOffPerProject.TakeOffPerProjectId = takeOffPerProject.TakeOffPerProjectId;

                    //            #region Takeoff Line

                    //            var takeoffline =
                    //                db.TakeOffLinePerProject.Where(x => x.TakeOffPerProjectId == m.TakeOffPerProjectId)
                    //                    .ToList();
                    //            if (takeoffline.Any())
                    //            {
                    //                foreach (var t in takeoffline)
                    //                {
                    //                    var entityi = new TakeOffLinePerProject();
                    //                    entityi = t;
                    //                    entityi.OrgBranchName = t.OrgBranchName;
                    //                    entityi.DateCreated = DateTime.Now;
                    //                    entityi.LastModified = null;
                    //                    entityi.CreatedBy = User.Identity.Name;
                    //                    entityi.ModifiedBy = null;
                    //                    entityi.TakeOffLinePerProjectId = takeOffPerProject.TakeOffPerProjectId;
                    //                    WorkItemdb = new ExceedDbContext();
                    //                    WorkItemdb.TakeOffLinePerProject.Add(entityi);
                    //                    WorkItemdb.SaveChanges();

                    //                }
                    //            }

                    //            #endregion

                    //            #region Takeoff Line per Kg

                    //            var takeofflineKg =
                    //                db.TakeOffLineForKgPerProject.Where(
                    //                    x => x.TakeOffPerProjectId == m.TakeOffPerProjectId).ToList();
                    //            if (takeofflineKg.Any())
                    //            {
                    //                foreach (var t in takeofflineKg)
                    //                {
                    //                    var entityy = new TakeOffLineForKgPerProject();

                    //                    entityy = t;
                    //                    entityy.OrgBranchName = t.OrgBranchName;
                    //                    entityy.DateCreated = DateTime.Now;
                    //                    entityy.LastModified = null;
                    //                    entityy.CreatedBy = User.Identity.Name;
                    //                    entityy.ModifiedBy = null;
                    //                    entityy.TakeOffPerProjectId = takeOffPerProject.TakeOffPerProjectId;

                    //                    WorkItemdb = new ExceedDbContext();
                    //                    WorkItemdb.TakeOffLineForKgPerProject.Add(entityy);
                    //                    WorkItemdb.SaveChanges();

                    //                }
                    //            }

                    //            #endregion
                    //        }
                    //    }

                    //    #endregion
                    //}

                    #endregion
                }
                this.UpdateWorkItemsHeirarchy(from, to);
            }

            else

            {
                return new JavaScriptResult() { Script = "alert('" + "Please create estimation for this project type!!" + "');" };
            }
            ProjectTo = 0;
            return new JavaScriptResult() { Script = "alert('" + "Operation completed Successfully!!" + "');" };
        }

        /// <summary>
        /// check for existence of blocks in the destination project 
        /// </summary>
        /// <param name="to"></param>
        /// <returns></returns>
        public bool TargetProjectHasNoSubProjectTypes(int to)
        {
            var hasNoSubProjectTypes = false;
            var temp = db.SubProjectType.Where(b => b.ProjectInformationId == to).ToList();
            if (!temp.Any())
            {
                hasNoSubProjectTypes = true;
            }
            return hasNoSubProjectTypes;
        }

        /// <summary>
        /// copy the blocks and every thing within ... 
        /// from the source project to the destination projecty
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public JsonResult CloneEstimationWithSubProjectTypes(int from, int to)
        {
            #region Clean exixting data
            var estimationWorkItemsToClean = db.GenericWorkItemPerProject.Where(x => x.ProjectInformationId == to).ToList();
            var estimationPieceRateWorkItemsToClean = db.GenericAreaWorkItem.Where(x => x.ProjectInformationId == to).ToList();
            var estimationSubContractWorkItemsToClean =
                db.GenericSubContractWorkItem.Where(x => x.ProjectInformationId == to).ToList();

            if (estimationWorkItemsToClean.Any() || estimationPieceRateWorkItemsToClean.Any() || estimationSubContractWorkItemsToClean.Any())
            {
                db.GenericSubContractWorkItem.RemoveRange(estimationSubContractWorkItemsToClean);
                db.GenericAreaWorkItem.RemoveRange(estimationPieceRateWorkItemsToClean);
                db.GenericSubContractWorkItem.RemoveRange(estimationSubContractWorkItemsToClean);
                db.SaveChanges();
            }
            #endregion

            #region entire data to copy
            var estimationWorkItems = db.GenericWorkItemPerProject.Where(x => x.ProjectInformationId == from).ToList();
            var estimationPieceRateWorkItems = db.GenericAreaWorkItem.Where(x => x.ProjectInformationId == from).ToList();
            var estimationSubContractWorkItems =
                db.GenericSubContractWorkItem.Where(x => x.ProjectInformationId == from).ToList();
            #endregion
            // the list of blocks that need to be copied
            var listOfSubProjectTypesFromSourceProject = db.SubProjectType.Where(b => b.ProjectInformationId == from).ToList();

            if (listOfSubProjectTypesFromSourceProject.Any())
            {
                foreach (SubProjectType subProjectTypeToCopy in listOfSubProjectTypesFromSourceProject)
                {
                    #region data under this very sub-project type
                    var estimationWorkItemsInCurrentSubProjectType = estimationWorkItems.Where(w => w.SubProjectTypeId == subProjectTypeToCopy.SubProjectTypeId).ToList();
                    var estimationPieceRateWorkItemsInCurrentSubProjectType = estimationPieceRateWorkItems.Where(p => p.SubProjectTypeId == subProjectTypeToCopy.SubProjectTypeId).ToList();
                    var estimationSubContractWorkItemsInCurrentSubProjectType = estimationSubContractWorkItems.Where(sc => sc.SubProjectTypeId == subProjectTypeToCopy.SubProjectTypeId).ToList();
                    #endregion
                    var subTypeRecord = new SubProjectType
                    {
                        Name = subProjectTypeToCopy.Name,
                        NumberOfBlocks = subProjectTypeToCopy.NumberOfBlocks,
                        ProjectInformationId = to,
                        //ProjectTypeId = blockToCopy.ProjectTypeId,
                        Remark = subProjectTypeToCopy.Remark
                    };
                    //var dbInstance = new ExceedDbContext();
                    db.SubProjectType.Add(subTypeRecord);
                    db.SaveChanges();

                    // find the structure types under the block being copied
                    var mappings = db.SubProjectStructureTypeMapping.Where(m => m.SubProjectTypeId == subProjectTypeToCopy.SubProjectTypeId).ToList();
                    if (mappings.Any())
                    {
                        // copy the structure types from the soruce block to the destination block
                        // created above ..
                        foreach (SubProjectStructureTypeMapping mappingToCopy in mappings)
                        {
                            var mappingRecord = new SubProjectStructureTypeMapping
                            {
                                SubProjectTypeId = subTypeRecord.SubProjectTypeId,
                                ConfigurationId = mappingToCopy.ConfigurationId,
                                Remark = mappingToCopy.Remark
                            };
                            //var structureDbInstance = new ExceedDbContext();
                            db.SubProjectStructureTypeMapping.Add(mappingRecord);
                            db.SaveChanges();

                            var workItemsPerCurrentStructreType = estimationWorkItemsInCurrentSubProjectType.Where(w => w.WorkCategory.ConfigurationId == mappingToCopy.ConfigurationId).ToList();
                            var pieceRatesPerCurrentStructreType = estimationPieceRateWorkItemsInCurrentSubProjectType.Where(w => w.WorkCategory.ConfigurationId == mappingToCopy.ConfigurationId).ToList();
                            var subContractWorkItemsPerCurrentStructreType = estimationSubContractWorkItemsInCurrentSubProjectType.Where(w => w.WorkCategory.ConfigurationId == mappingToCopy.ConfigurationId).ToList();

                            #region Copy Work items
                            foreach (GenericWorkItemPerProject workItem in workItemsPerCurrentStructreType)
                            {
                                var workItemRecord = new GenericWorkItemPerProject
                                {
                                    ProjectInformationId = to,

                                    IsSubCategory = workItem.IsSubCategory,
                                    ItemCode = workItem.ItemCode,
                                    ContainsWorkItemAsInput = workItem.ContainsWorkItemAsInput,
                                    CopiedFromId = workItem.GenericWorkItemPerProjectId,
                                    ItemName = workItem.ItemName,
                                    ProjectEstimationWorkItemId = workItem.ProjectEstimationWorkItemId,
                                    TotalQuantityOfWork = workItem.TotalQuantityOfWork,
                                    DirectCost = workItem.DirectCost,
                                    MaterialCost = workItem.MaterialCost,
                                    LabourOutPut = workItem.LabourOutPut,
                                    EquipmentOutput = workItem.EquipmentOutput,
                                    MaterialCostPerUnit = workItem.MaterialCostPerUnit,
                                    LaborCostPerUnit = workItem.LaborCostPerUnit,
                                    EquipmentCostPerUnit = workItem.EquipmentCostPerUnit,
                                    TotalUnitCost = workItem.TotalUnitCost,
                                    OverHeadCost = workItem.OverHeadCost,
                                    ProfitCost = workItem.ProfitCost,
                                    ProjectType = workItem.ProjectType,
                                    Remark = workItem.Remark,

                                    SubProjectTypeId = subTypeRecord.SubProjectTypeId,

                                    LabourHourlyOutPut = workItem.LabourHourlyOutPut,
                                    UnitOfMeasurementId = workItem.UnitOfMeasurementId,
                                    WorkCategoryId = workItem.WorkCategoryId,
                                    OverheadRate = workItem.OverheadRate,
                                    ProfitMargin = workItem.ProfitMargin,
                                    ParentId = workItem.ParentId,
                                    OrgBranchName = workItem.OrgBranchName,
                                    LastModified = null,
                                    ModifiedBy = null,
                                    CreatedBy = User.Identity.Name,
                                    DateCreated = DateTime.Now,
                                };
                                db.GenericWorkItemPerProject.Add(workItemRecord);
                                db.SaveChanges();

                                #region copy work item resources

                                var materials = db.GenericProjectMaterial.Where(m => m.GenericWorkItemPerProjectId == workItem.GenericWorkItemPerProjectId).ToList();
                                var labors = db.GenericProjectDirectLabour.Where(m => m.GenericWorkItemPerProjectId == workItem.GenericWorkItemPerProjectId).ToList();
                                var equipments = db.GenericProjectEquipment.Where(m => m.GenericWorkItemPerProjectId == workItem.GenericWorkItemPerProjectId).ToList();
                                var customWorkItemContents = db.GenericProjectCustomWorkItemDetails.Where(
                                            cwd => cwd.GenericWorkItemPerProjectId == workItem.GenericWorkItemPerProjectId).ToList();


                                foreach (GenericProjectCustomWorkItemDetail customMaterial in customWorkItemContents)
                                {
                                    var customMaterialToAdd = new GenericProjectCustomWorkItemDetail
                                    {
                                        GenericWorkItemPerProjectId = workItemRecord.GenericWorkItemPerProjectId,
                                        ProjectEstimationWorkItemId = customMaterial.ProjectEstimationWorkItemId,
                                        QuantityPerUnit = customMaterial.QuantityPerUnit,
                                        UnitCost = customMaterial.UnitCost,
                                        TotalCost = customMaterial.TotalCost,

                                        CreatedBy = User.Identity.Name,
                                        DateCreated = DateTime.Now,
                                        ModifiedBy = User.Identity.Name,
                                        LastModified = DateTime.Now
                                    };
                                    db.GenericProjectCustomWorkItemDetails.Add(customMaterialToAdd);
                                    db.SaveChanges();
                                }
                                foreach (GenericProjectMaterial material in materials)
                                {
                                    var entity = new GenericProjectMaterial
                                    {
                                        Unit = material.Unit,
                                        Quantity = material.Quantity,
                                        ItemCategoryCode = material.ItemCategoryCode,
                                        ItemCode = material.ItemCode,
                                        UnitPrice = material.UnitPrice,
                                        TotalCost = material.TotalCost,
                                        MaterialUnitCost = material.MaterialUnitCost,
                                        GenericWorkItemPerProjectId = workItemRecord.GenericWorkItemPerProjectId,
                                        OrgBranchName = material.OrgBranchName,
                                        DateCreated = DateTime.Now,
                                        LastModified = null,
                                        CreatedBy = User.Identity.Name,
                                        ModifiedBy = null,

                                    };

                                    db.GenericProjectMaterial.Add(entity);
                                    db.SaveChanges();
                                }
                                foreach (GenericProjectDirectLabour labor in labors)
                                {
                                    var entity = new GenericProjectDirectLabour
                                    {
                                        Unit = labor.Unit,
                                        Quantity = labor.Quantity,
                                        UnitFactory = labor.UnitFactory,
                                        IndexedHourlySalary = labor.IndexedHourlySalary,
                                        DailyPay = labor.DailyPay,
                                        TotalCost = labor.TotalCost,
                                        ManpowerUnitCost = labor.ManpowerUnitCost,
                                        OrgBranchName = labor.OrgBranchName,
                                        DateCreated = DateTime.Now,
                                        LastModified = null,
                                        CreatedBy = User.Identity.Name,
                                        ModifiedBy = null,
                                        GenericWorkItemPerProjectId =
                                                        workItemRecord.GenericWorkItemPerProjectId,
                                        ProjectEstimationIndexLabourId =
                                                        labor.ProjectEstimationIndexLabourId
                                    };
                                    db.GenericProjectDirectLabour.Add(entity);
                                    db.SaveChanges();
                                }
                                foreach (GenericProjectEquipment equipment in equipments)
                                {
                                    var entity = new GenericProjectEquipment
                                    {

                                        FleetsID = equipment.FleetsID,
                                        Unit = equipment.Unit,
                                        Quantity = equipment.Quantity,
                                        UnitFactory = equipment.UnitFactory,
                                        RentalCost = equipment.RentalCost,
                                        TotalCost = equipment.TotalCost,
                                        EquipmentUnitCost = equipment.EquipmentUnitCost,
                                        EquipmentType = equipment.EquipmentType,
                                        OrgBranchName = equipment.OrgBranchName,
                                        DateCreated = DateTime.Now,
                                        LastModified = null,
                                        CreatedBy = User.Identity.Name,
                                        ModifiedBy = null,
                                        GenericWorkItemPerProjectId =
                                                        workItemRecord.GenericWorkItemPerProjectId,

                                    };

                                    db.GenericProjectEquipment.Add(entity);
                                    db.SaveChanges();
                                }
                                #endregion
                            }
                            #endregion

                            #region Copy piece rates
                            foreach (GenericAreaWorkItem pieceRate in pieceRatesPerCurrentStructreType)
                            {
                                var piecerAterecord = new GenericAreaWorkItem
                                {
                                    WorkItemName = pieceRate.WorkItemName,
                                    ItemCode = pieceRate.ItemCode,
                                    Width = pieceRate.Width,
                                    Height = pieceRate.Height,
                                    Area = pieceRate.Area,
                                    CylinderLock = pieceRate.CylinderLock,
                                    DirectCost = pieceRate.DirectCost,
                                    MaterialCost = pieceRate.MaterialCost,
                                    OverHeadProfitCost = pieceRate.OverHeadProfitCost,
                                    Price = pieceRate.Price,
                                    WorkCategoryId = pieceRate.WorkCategoryId,
                                    ProjectInformationId = to,
                                    SubProjectTypeId = subTypeRecord.SubProjectTypeId,
                                };
                                db.GenericAreaWorkItem.Add(piecerAterecord);
                                db.SaveChanges();
                            }
                            #endregion

                            #region copy sub-contract work items
                            foreach (GenericSubContractWorkItem subContractWorkItem in subContractWorkItemsPerCurrentStructreType)
                            {
                                var entity = new GenericSubContractWorkItem
                                {
                                    UnitOfMeasurementId = subContractWorkItem.UnitOfMeasurementId,
                                    OverHeadProfit = subContractWorkItem.OverHeadProfit,
                                    Quntity = subContractWorkItem.Quntity,
                                    SelectedCost = subContractWorkItem.SelectedCost,
                                    SubContractCostOne = subContractWorkItem.SubContractCostOne,
                                    SubContractCostTwo = subContractWorkItem.SubContractCostTwo,
                                    SubContractCostThree = subContractWorkItem.SubContractCostThree,
                                    TotalCost = subContractWorkItem.TotalCost,
                                    WorkCategoryId = subContractWorkItem.WorkCategoryId,
                                    WorkItemName = subContractWorkItem.WorkItemName,
                                    ItemCode = subContractWorkItem.ItemCode,
                                    ProjectInformationId = to,
                                    SubProjectTypeId = subTypeRecord.SubProjectTypeId,
                                };
                                db.GenericSubContractWorkItem.Add(entity);
                                db.SaveChanges();
                            }
                            #endregion

                        }
                    }
                }
            }
            this.UpdateWorkItemsHeirarchy(from, to);
            var data = new
            {
                Message = "Operation Completed Successfully!",
                Status = "Ok"
            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        private void UpdateWorkItemsHeirarchy(int sourceProjId, int destProjId)
        {
            var workItemsInProject = db.GenericWorkItemPerProject.Where(wi => wi.ProjectInformationId == destProjId && wi.ParentId != 0).ToList();
            if (workItemsInProject.Any())
            {
                foreach (var workItem in workItemsInProject)
                {
                    var orginalWorkItem = db.GenericWorkItemPerProject.FirstOrDefault(w => w.ProjectInformationId == sourceProjId && w.GenericWorkItemPerProjectId == workItem.CopiedFromId);
                    if (orginalWorkItem != null)
                    {
                        var targetParent = db.GenericWorkItemPerProject.FirstOrDefault(w => w.ProjectInformationId == destProjId && w.SubProjectTypeId == workItem.SubProjectTypeId && w.CopiedFromId == orginalWorkItem.ParentId);
                        if (targetParent != null)
                        {
                            workItem.ParentId = targetParent.GenericWorkItemPerProjectId;
                            db.GenericWorkItemPerProject.Attach(workItem);
                            db.Entry(workItem).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    //var workItemParent = db.WorkItemPerProject.FirstOrDefault(w => w.ProjectInformationId == projectId && w.ProjectEstimationWorkItemId == workItem.ParentId);


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
