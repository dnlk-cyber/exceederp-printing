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
using ExceedERP.Core.Domain.Common;
using ExceedERP.Helpers.Construction;

namespace ExceedERP.Web.Areas.Construction.Controllers.Project
{
    public class ProjectListController : Controller
    {
        private ExceedDbContext db;
        public static string CurrentUSer { get; set; }
        //public static int ProjectId { get; set; }
        //public static int Category { get; set; }
        private static decimal OverHeadCostRate { get; set; }
        private static decimal ProfitMarginRate { get; set; }
        ConstructionInputs inputs;

        public ProjectListController()
        {
            db = new ExceedDbContext();
            OverHeadCostRate = ConsSettingHelper.GetCurrentOverHeadRate();
            ProfitMarginRate = ConsSettingHelper.GetCurrentProfitRate();
        }
        public ActionResult Index(string name)
        {
            db.Configuration.ProxyCreationEnabled = false;

            //ViewBag.Title = "Project Detail";
            //if (name == "Building")
            //{
            //    ProjectId = 1;
            //    ViewBag.Title = "Project Detail For Building";
            //}
            //else if (name == "Road")
            //{
            //    ProjectId = 2;
            //    ViewBag.Title = "Project Detail For Road";
            //}
            //else if (name == "Dam")
            //{
            //    ProjectId = 3;
            //    ViewBag.Title = "Project Detail For Dam";
            //}
            //else if (name == "Irrigation")
            //{
            //    ProjectId = 4;
            //    ViewBag.Title = "Project Detail For Irrigation";
            //}
            //else
            //{
            //    ProjectId = 0;
            //}
            ConstructionInputs inputs = new ConstructionInputs();
            var lookups = db.Lookups.ToList();
            var workItemCategories = from w in db.WorkCategory
                                     join p in db.ProjectTypeMappings
                                         on w.ConfigurationId equals p.ConfigurationId
                                     //where p.ProjectTypeId == ProjectId
                                     select (new { w.WorkCategoryId, w.ConfigurationId, w.Description, w.Name, w.Index });
            ViewData["Categories"] = workItemCategories.ToList();
            var units = lookups.Where(x => x.LookupType == LookUpTypes.UOM).ToList();
            ViewData["Unit"] = units;
            var projects = lookups.Where(x => x.LookupType == LookUpTypes.PROJECT_TYPE).ToList();
            ViewData["ProjectTypeList"] = projects;
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
            //SetDefaultValue();
            ViewData["Equipment"] = inputs.GetInternalExternalEquipmentViewModel().Select(p => new { AssetNumber = p.AssetNumber, Name = p.Name }).ToList(); ;
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
            var projectList = db.ProjectInformation.ToList();
            foreach (var p in projectList)
            {
                var workitemperproject =
                    db.WorkItemPerProject.Where(x => x.ProjectInformationId == p.ProjectInformationId).ToList();
                UpdateProjectTotalCost(
                       workitemperproject.Select(x => x.WorkItemPerProjectId).ToList());
            }
            var branches = db.Branch.ToList();
            ViewData["Branch"] = branches;
            return View();
        }


        //public ActionResult ProjectInformation_Read([DataSourceRequest]DataSourceRequest request)
        //{
        //    IQueryable<ProjectInformation> projectinformation = db.ProjectInformation.Where(x => x.ProjectType == ProjectId);
        //    DataSourceResult result = projectinformation.ToDataSourceResult(request, projectInformation => new
        //    {
        //        ProjectInformationId = projectInformation.ProjectInformationId,
        //        Name = projectInformation.Name,
        //        Code = projectInformation.Code,
        //        Category = projectInformation.Category,
        //        BranchId = projectInformation.BranchId,
        //        ContractAmount = projectInformation.ContractAmount,
        //        ProjectScope = projectInformation.ProjectScope,
        //        EmployeeId = projectInformation.EmployeeId,
        //        Client = projectInformation.Client,
        //        Consultant = projectInformation.Consultant,
        //        OrgBranchName = projectInformation.OrgBranchName,
        //        DateCreated = projectInformation.DateCreated,
        //        LastModified = projectInformation.LastModified,
        //        CreatedBy = projectInformation.CreatedBy,
        //        ModifiedBy = projectInformation.ModifiedBy,
        //        ProjectStatus = projectInformation.ProjectStatus
        //    });

        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
        #region Work Item
        public ActionResult WorkItemPerProject_Read([DataSourceRequest]DataSourceRequest request, int id)
        {


            IQueryable<WorkItemPerProject> workItems = db.WorkItemPerProject.Where(x => x.ProjectInformationId == id);
            DataSourceResult result = workItems.ToDataSourceResult(request, workItemPerProject => new
            {
                ProjectEstimationWorkItemId = workItemPerProject.ProjectEstimationWorkItemId,
                WorkItemPerProjectId = workItemPerProject.WorkItemPerProjectId,
                ItemName = workItemPerProject.ItemName,
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
                //MaterialCostPerUnit = workItemPerProject.MaterialCostPerUnit,
                //LaborCostPerUnit = workItemPerProject.LaborCostPerUnit,
                //EquipmentCostPerUnit = workItemPerProject.EquipmentCostPerUnit,
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


            UpdateProjectTotalCost(
                workItems.Select(x => x.WorkItemPerProjectId).ToList());

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WorkItemPerProject_Create([DataSourceRequest]DataSourceRequest request, WorkItemPerProject workItemPerProject, int idTwo2)
        {
            if (ModelState.IsValid)
            {
                var entity = new WorkItemPerProject
                {
                    WorkItemPerProjectId = workItemPerProject.WorkItemPerProjectId,
                    ProjectEstimationWorkItemId = workItemPerProject.ProjectEstimationWorkItemId,
                    ItemName = workItemPerProject.ItemName,
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
                    ProjectInformationId = idTwo2,
                    WorkCategoryId = workItemPerProject.WorkCategoryId
                };
                entity.TotalUnitCost = entity.LabourHourlyOutPut > 0 ? (entity.DirectCost + entity.OverHeadCost + entity.ProfitCost) / entity.LabourHourlyOutPut :
                    entity.DirectCost + entity.OverHeadCost + entity.ProfitCost;

                db.WorkItemPerProject.Add(entity);
                db.SaveChanges();
                workItemPerProject.WorkItemPerProjectId = entity.WorkItemPerProjectId;
                //CopyEstimation(workItemPerProject.ProjectEstimationWorkItemId, idTwo, workItemPerProject.WorkItemPerProjectId);
                //UpdateProjectTotalCost(workItemPerProject.WorkItemPerProjectId);
            }

            return Json(new[] { workItemPerProject }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WorkItemPerProject_Update([DataSourceRequest]DataSourceRequest request, WorkItemPerProject workItemPerProject, int blId)
        {
            if (ModelState.IsValid)
            {
                //BlockMapping blockMapping = new BlockMapping();
                //var temp2 = db.BlockMappings.Find(blId);
                //if (temp2 != null)
                //{
                //    blockMapping = temp2;
                //}


                var entityFromContext = db.WorkItemPerProject.Find(workItemPerProject.WorkItemPerProjectId);
                if (entityFromContext != null)
                {
                    entityFromContext.IsSubCategory = workItemPerProject.IsSubCategory;
                    entityFromContext.ItemCode = workItemPerProject.ItemCode;
                    entityFromContext.WorkItemPerProjectId = workItemPerProject.WorkItemPerProjectId;
                    entityFromContext.ItemName = string.IsNullOrEmpty(entityFromContext.ItemName) ?
                        GetEstimationWorkItem(workItemPerProject.ProjectEstimationWorkItemId) :
                        workItemPerProject.ItemName;
                    entityFromContext.BlockId = GetBlock(blId).BlockId;

                    entityFromContext.ProjectEstimationWorkItemId = workItemPerProject.ProjectEstimationWorkItemId;
                    entityFromContext.TotalQuantityOfWork = workItemPerProject.TotalQuantityOfWork;
                    entityFromContext.DirectCost = workItemPerProject.DirectCost;
                    entityFromContext.MaterialCost = workItemPerProject.MaterialCost;
                    entityFromContext.LabourOutPut = workItemPerProject.LabourOutPut;
                    entityFromContext.EquipmentOutput = workItemPerProject.EquipmentOutput;
                    entityFromContext.MaterialCostPerUnit = workItemPerProject.MaterialCostPerUnit;
                    entityFromContext.LaborCostPerUnit = workItemPerProject.LaborCostPerUnit;
                    entityFromContext.EquipmentCostPerUnit = workItemPerProject.EquipmentCostPerUnit;
                    entityFromContext.TotalUnitCost = workItemPerProject.TotalUnitCost;
                    entityFromContext.OverHeadCost = workItemPerProject.OverHeadCost;
                    entityFromContext.ProfitCost = workItemPerProject.ProfitCost;
                    entityFromContext.ProjectType = workItemPerProject.ProjectType;
                    entityFromContext.Remark = workItemPerProject.Remark;
                    entityFromContext.LabourHourlyOutPut = workItemPerProject.LabourHourlyOutPut;
                    entityFromContext.UnitOfMeasurementId = workItemPerProject.UnitOfMeasurementId;
                    entityFromContext.OverheadRate = workItemPerProject.OverheadRate;
                    entityFromContext.ProfitMargin = workItemPerProject.ProfitMargin;
                    ProfitMarginRate = workItemPerProject.ProfitMargin;
                    entityFromContext.ParentId = workItemPerProject.ParentId;
                    entityFromContext.OrgBranchName = workItemPerProject.OrgBranchName;
                    entityFromContext.DateCreated = workItemPerProject.DateCreated;
                    entityFromContext.LastModified = DateTime.Now;
                    entityFromContext.CreatedBy = workItemPerProject.CreatedBy;
                    entityFromContext.ModifiedBy = User.Identity.Name;
                    entityFromContext.ProjectInformationId = workItemPerProject.ProjectInformationId;
                    entityFromContext.WorkCategoryId = workItemPerProject.WorkCategoryId;
                }


                #region changed code
                //var entity = new WorkItemPerProject
                //{

                //    WorkItemPerProjectId = workItemPerProject.WorkItemPerProjectId,
                //    ItemName = GetEstimationWorkItem(workItemPerProject.ProjectEstimationWorkItemId),
                //    BlockId = GetBlock(blId).BlockId,

                //    ProjectEstimationWorkItemId = workItemPerProject.ProjectEstimationWorkItemId,
                //    TotalQuantityOfWork = workItemPerProject.TotalQuantityOfWork,
                //    DirectCost = workItemPerProject.DirectCost,
                //    MaterialCost = workItemPerProject.MaterialCost,
                //    LabourOutPut = workItemPerProject.LabourOutPut,
                //    EquipmentOutput = workItemPerProject.EquipmentOutput,
                //    TotalUnitCost = workItemPerProject.TotalUnitCost,
                //    OverHeadCost = workItemPerProject.OverHeadCost,
                //    ProfitCost = workItemPerProject.ProfitCost,
                //    ProjectTypeId = workItemPerProject.ProjectTypeId,
                //    Remark = workItemPerProject.Remark,
                //    LabourHourlyOutPut = workItemPerProject.LabourHourlyOutPut,
                //    UnitOfMeasurementId = workItemPerProject.UnitOfMeasurementId,
                //    OverheadRate = workItemPerProject.OverheadRate,
                //    ProfitMargin = workItemPerProject.ProfitMargin,
                //    ParentId = workItemPerProject.ParentId,
                //    OrgBranchName = workItemPerProject.OrgBranchName,
                //    DateCreated = workItemPerProject.DateCreated,
                //    LastModified = DateTime.Now,
                //    CreatedBy = workItemPerProject.CreatedBy,
                //    ModifiedBy = User.Identity.Name,
                //    ProjectInformationId = workItemPerProject.ProjectInformationId,
                //    WorkCategoryId = workItemPerProject.WorkCategoryId,


                //}; 
                #endregion

                var prevQuantity =
                    db.WorkItemPerProject.Where(x => x.WorkItemPerProjectId == workItemPerProject.WorkItemPerProjectId).FirstOrDefault();
                if (prevQuantity != null)
                {
                    entityFromContext.TotalUnitCost = entityFromContext.LabourHourlyOutPut > 0 ? (entityFromContext.DirectCost + entityFromContext.OverHeadCost + entityFromContext.ProfitCost) / prevQuantity.LabourHourlyOutPut :
                        entityFromContext.DirectCost + entityFromContext.OverHeadCost + entityFromContext.ProfitCost;
                }
                else
                {
                    entityFromContext.TotalUnitCost = entityFromContext.LabourHourlyOutPut > 0 ? (entityFromContext.DirectCost + entityFromContext.OverHeadCost + entityFromContext.ProfitCost) / entityFromContext.LabourHourlyOutPut :
                        entityFromContext.DirectCost + entityFromContext.OverHeadCost + entityFromContext.ProfitCost;
                }

                #region snipet for detaching an entity
                //var local = db.Set<WorkItemPerProject>()
                //         .Local
                //         .FirstOrDefault(f => f.WorkItemPerProjectId == workItemPerProject.WorkItemPerProjectId);
                //if (local != null)
                //{
                //    db.Entry(local).State = EntityState.Detached;
                //}
                //db.Entry(entity).State = EntityState.Modified; 
                #endregion

                db.WorkItemPerProject.Attach(entityFromContext);
                db.Entry(entityFromContext).State = EntityState.Modified;
                db.SaveChanges();
                //UpdateProjectTotalCost(workItemPerProject.WorkItemPerProjectId);
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
                    ProjectEstimationWorkItemId = workItemPerProject.ProjectEstimationWorkItemId,
                    WorkItemPerProjectId = workItemPerProject.WorkItemPerProjectId,
                    ItemName = workItemPerProject.ItemName,
                    BlockId = workItemPerProject.BlockId,
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
                UpdateProjectTotalCost(workItemPerProject.WorkItemPerProjectId);
            }

            return Json(new[] { workItemPerProject }.ToDataSourceResult(request, ModelState));
        }
        #endregion
        #region PieceRate
        public ActionResult AreaWorkItem_Read([DataSourceRequest]DataSourceRequest request, int ida)
        {
            IQueryable<AreaWorkItem> AreaWorkItems = db.AreaWorkItemes.AsQueryable();
            //if (ProjectId > 0)
            //{
            AreaWorkItems =
                    db.AreaWorkItemes.Where(x => x.ProjectInformationId == ida);
            DataSourceResult result = AreaWorkItems.ToDataSourceResult(request,
                    AreaWorkItem => new
                    {
                        AreaWorkItemId = AreaWorkItem.AreaWorkItemId,
                        WorkCategoryId = AreaWorkItem.WorkCategoryId,
                        Price = AreaWorkItem.Price,
                        WorkItemName = AreaWorkItem.WorkItemName,
                        ItemCode = AreaWorkItem.ItemCode,
                        DirectCost = AreaWorkItem.DirectCost,
                        Width = AreaWorkItem.Width,
                        Height = AreaWorkItem.Height,
                        Area = AreaWorkItem.Area,
                        CylinderLock = AreaWorkItem.CylinderLock,
                        OverHeadProfitCost = AreaWorkItem.OverHeadProfitCost,
                        ProjectInformationId = AreaWorkItem.ProjectInformationId,
                        MaterialCost = AreaWorkItem.MaterialCost,
                        OrgBranchName = AreaWorkItem.OrgBranchName,
                        DateCreated = AreaWorkItem.DateCreated,
                        LastModified = AreaWorkItem.LastModified,
                        CreatedBy = AreaWorkItem.CreatedBy,
                        ModifiedBy = AreaWorkItem.ModifiedBy,
                        TransportCost = AreaWorkItem.TransportCost,
                    });

            return Json(result);
            //}

        }

        public ActionResult AreaWorkItemPerStructure([DataSourceRequest]DataSourceRequest request, int sId, int blokkId)
        {
            IQueryable<WorkCategory> workcategorys = db.WorkCategory.Where(wc => wc.ConfigurationId == sId);
            List<AreaWorkItem> workItems = new List<AreaWorkItem>();
            BlockMapping currentBlock = new BlockMapping();

            BlockMapping otherOne = new BlockMapping();
            var temp2 = db.BlockMappings.Find(blokkId);
            if (temp2 != null)
            {
                otherOne = temp2;
            }

            foreach (WorkCategory category in workcategorys)
            {
                var workItemList = db.AreaWorkItemes.Where(wi => wi.WorkCategoryId == category.WorkCategoryId && wi.BlockId == temp2.BlockId);
                if (workItemList != null && workItemList.Count() > 0)
                {
                    foreach (AreaWorkItem item in workItemList)
                    {
                        workItems.Add(item);
                    }
                }
            }
            IQueryable<AreaWorkItem> workItemsToReturn = workItems.AsQueryable();
            DataSourceResult result = workItemsToReturn.ToDataSourceResult(request,
                    AreaWorkItem => new
                    {
                        AreaWorkItemId = AreaWorkItem.AreaWorkItemId,
                        WorkCategoryId = AreaWorkItem.WorkCategoryId,
                        Price = AreaWorkItem.Price,
                        WorkItemName = AreaWorkItem.WorkItemName,
                        ItemCode = AreaWorkItem.ItemCode,
                        DirectCost = AreaWorkItem.DirectCost,
                        Width = AreaWorkItem.Width,
                        Height = AreaWorkItem.Height,
                        Area = AreaWorkItem.Area,
                        CylinderLock = AreaWorkItem.CylinderLock,
                        OverHeadProfitCost = AreaWorkItem.OverHeadProfitCost,
                        ProjectInformationId = AreaWorkItem.ProjectInformationId,
                        MaterialCost = AreaWorkItem.MaterialCost,
                        OrgBranchName = AreaWorkItem.OrgBranchName,
                        DateCreated = AreaWorkItem.DateCreated,
                        LastModified = AreaWorkItem.LastModified,
                        CreatedBy = AreaWorkItem.CreatedBy,
                        ModifiedBy = AreaWorkItem.ModifiedBy,
                        TransportCost = AreaWorkItem.TransportCost,
                        BlockId = AreaWorkItem.BlockId
                    });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AreaWorkItem_Create([DataSourceRequest]DataSourceRequest request, AreaWorkItem AreaWorkItem, int idaTwo)
        {
            if (ModelState.IsValid)
            {


                var entity = new AreaWorkItem
                {
                    AreaWorkItemId = AreaWorkItem.AreaWorkItemId,
                    Price = AreaWorkItem.Price,
                    WorkCategoryId = AreaWorkItem.WorkCategoryId,
                    WorkItemName = AreaWorkItem.WorkItemName,
                    ItemCode = AreaWorkItem.ItemCode,
                    DirectCost = AreaWorkItem.DirectCost,
                    Width = AreaWorkItem.Width,
                    Height = AreaWorkItem.Height,
                    Area = AreaWorkItem.Area,
                    CylinderLock = AreaWorkItem.CylinderLock,
                    OverHeadProfitCost = AreaWorkItem.OverHeadProfitCost,
                    TransportCost = AreaWorkItem.TransportCost,
                    ProjectInformationId = idaTwo,
                    MaterialCost = AreaWorkItem.MaterialCost,

                    OrgBranchName = AreaWorkItem.OrgBranchName,
                    DateCreated = DateTime.Now.Date,
                    LastModified = AreaWorkItem.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = AreaWorkItem.ModifiedBy
                };
                entity.Area = entity.Width * entity.Height;
                entity.MaterialCost = entity.Area * entity.Price;
                entity.DirectCost = entity.MaterialCost + entity.TransportCost + entity.CylinderLock;
                entity.OverHeadProfitCost = entity.DirectCost + (entity.DirectCost * (OverHeadCostRate / 100)) +
                                         (entity.DirectCost * (ProfitMarginRate / 100));
                db.AreaWorkItemes.Add(entity);
                db.SaveChanges();
                //ProjectEstimationWorkItemController.ProjectId = 0;
                AreaWorkItem.AreaWorkItemId = entity.AreaWorkItemId;

            }

            return Json(new[] { AreaWorkItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AreaWorkItem_Update([DataSourceRequest]DataSourceRequest request, AreaWorkItem AreaWorkItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new AreaWorkItem
                {
                    AreaWorkItemId = AreaWorkItem.AreaWorkItemId,
                    WorkCategoryId = AreaWorkItem.WorkCategoryId,
                    Price = AreaWorkItem.Price,
                    WorkItemName = AreaWorkItem.WorkItemName,
                    ItemCode = AreaWorkItem.ItemCode,
                    DirectCost = AreaWorkItem.DirectCost,
                    Width = AreaWorkItem.Width,
                    Height = AreaWorkItem.Height,
                    Area = AreaWorkItem.Area,
                    TransportCost = AreaWorkItem.TransportCost,
                    CylinderLock = AreaWorkItem.CylinderLock,
                    OverHeadProfitCost = AreaWorkItem.OverHeadProfitCost,

                    ProjectInformationId = AreaWorkItem.ProjectInformationId,
                    MaterialCost = AreaWorkItem.MaterialCost,

                    OrgBranchName = AreaWorkItem.OrgBranchName,
                    DateCreated = AreaWorkItem.DateCreated,
                    LastModified = DateTime.Now.Date,
                    CreatedBy = AreaWorkItem.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                entity.Area = entity.Width * entity.Height;
                entity.MaterialCost = entity.Area * entity.Price;
                entity.DirectCost = entity.MaterialCost + entity.TransportCost + entity.CylinderLock;
                entity.OverHeadProfitCost = entity.DirectCost + (entity.DirectCost * (OverHeadCostRate / 100)) +
                                            (entity.DirectCost * (ProfitMarginRate / 100));
                db = new ExceedDbContext();
                db.AreaWorkItemes.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { AreaWorkItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AreaWorkItem_Destroy([DataSourceRequest]DataSourceRequest request, AreaWorkItem AreaWorkItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new AreaWorkItem
                {
                    AreaWorkItemId = AreaWorkItem.AreaWorkItemId,
                    WorkCategoryId = AreaWorkItem.WorkCategoryId,
                    WorkItemName = AreaWorkItem.WorkItemName,
                    ItemCode = AreaWorkItem.ItemCode,
                    DirectCost = AreaWorkItem.DirectCost,
                    Width = AreaWorkItem.Width,
                    Height = AreaWorkItem.Height,
                    Price = AreaWorkItem.Price,
                    Area = AreaWorkItem.Area,
                    CylinderLock = AreaWorkItem.CylinderLock,
                    OverHeadProfitCost = AreaWorkItem.OverHeadProfitCost,
                    TransportCost = AreaWorkItem.TransportCost,
                    ProjectInformationId = AreaWorkItem.ProjectInformationId,
                    MaterialCost = AreaWorkItem.MaterialCost,
                    OrgBranchName = AreaWorkItem.OrgBranchName,
                    DateCreated = AreaWorkItem.DateCreated,
                    LastModified = AreaWorkItem.LastModified,
                    CreatedBy = AreaWorkItem.CreatedBy,
                    ModifiedBy = AreaWorkItem.ModifiedBy
                };

                db.AreaWorkItemes.Attach(entity);
                db.AreaWorkItemes.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { AreaWorkItem }.ToDataSourceResult(request, ModelState));
        }

        #endregion

        #region Sub Contract work Item
        public ActionResult SubContractWorkItem_Read([DataSourceRequest]DataSourceRequest request, int ids)
        {
            IQueryable<SubContractWorkItem> SubContractWorkItems = db.SubContractWorkItem.AsQueryable();
            //if (ProjectId > 0)
            //{
            SubContractWorkItems =
                    db.SubContractWorkItem.Where(x => x.ProjectInformationId == ids);
            DataSourceResult result = SubContractWorkItems.ToDataSourceResult(request,
                    SubContractWorkItem => new
                    {
                        SubContractWorkItemId = SubContractWorkItem.SubContractWorkItemId,
                        WorkCategoryId = SubContractWorkItem.WorkCategoryId,
                        UnitOfMeasurementId = SubContractWorkItem.UnitOfMeasurementId,
                        WorkItemName = SubContractWorkItem.WorkItemName,
                        ItemCode = SubContractWorkItem.ItemCode,
                        Quntity = SubContractWorkItem.Quntity,
                        SubContractCostOne = SubContractWorkItem.SubContractCostOne,
                        SubContractCostTwo = SubContractWorkItem.SubContractCostTwo,
                        SubContractCostThree = SubContractWorkItem.SubContractCostThree,
                        SelectedCost = SubContractWorkItem.SelectedCost,
                        TotalCost = SubContractWorkItem.TotalCost,
                        ProjectInformationId = SubContractWorkItem.ProjectInformationId,

                        OrgBranchName = SubContractWorkItem.OrgBranchName,
                        DateCreated = SubContractWorkItem.DateCreated,
                        LastModified = SubContractWorkItem.LastModified,
                        CreatedBy = SubContractWorkItem.CreatedBy,
                        ModifiedBy = SubContractWorkItem.ModifiedBy,

                    });

            return Json(result);
            //}

        }

        public ActionResult SubContractWorkItemPerStructure([DataSourceRequest]DataSourceRequest request, int strIdFRead, int blckIdFRead)
        {
            IQueryable<WorkCategory> workcategorys = db.WorkCategory.Where(wc => wc.ConfigurationId == strIdFRead);
            List<SubContractWorkItem> workItems = new List<SubContractWorkItem>();
            BlockMapping currentBlock = new BlockMapping();

            BlockMapping otherOne = new BlockMapping();
            var temp2 = db.BlockMappings.Find(blckIdFRead);
            if (temp2 != null)
            {
                otherOne = temp2;
            }

            foreach (WorkCategory category in workcategorys)
            {
                var workItemList = db.SubContractWorkItem.Where(wi => wi.WorkCategoryId == category.WorkCategoryId && wi.BlockId == temp2.BlockId);
                if (workItemList != null && workItemList.Count() > 0)
                {
                    foreach (SubContractWorkItem item in workItemList)
                    {
                        workItems.Add(item);
                    }
                }
            }
            IQueryable<SubContractWorkItem> workItemsToReturn = workItems.AsQueryable();

            DataSourceResult result = workItemsToReturn.ToDataSourceResult(request,
                    SubContractWorkItem => new
                    {
                        SubContractWorkItemId = SubContractWorkItem.SubContractWorkItemId,
                        WorkCategoryId = SubContractWorkItem.WorkCategoryId,
                        UnitOfMeasurementId = SubContractWorkItem.UnitOfMeasurementId,
                        WorkItemName = SubContractWorkItem.WorkItemName,
                        ItemCode = SubContractWorkItem.ItemCode,
                        Quntity = SubContractWorkItem.Quntity,
                        SubContractCostOne = SubContractWorkItem.SubContractCostOne,
                        SubContractCostTwo = SubContractWorkItem.SubContractCostTwo,
                        SubContractCostThree = SubContractWorkItem.SubContractCostThree,
                        SelectedCost = SubContractWorkItem.SelectedCost,
                        TotalCost = SubContractWorkItem.TotalCost,
                        ProjectInformationId = SubContractWorkItem.ProjectInformationId,

                        OrgBranchName = SubContractWorkItem.OrgBranchName,
                        DateCreated = SubContractWorkItem.DateCreated,
                        LastModified = SubContractWorkItem.LastModified,
                        CreatedBy = SubContractWorkItem.CreatedBy,
                        ModifiedBy = SubContractWorkItem.ModifiedBy,

                    });

            return Json(result);

        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SubContractWorkItem_Create([DataSourceRequest]DataSourceRequest request, SubContractWorkItem SubContractWorkItem, int idsTwo)
        {
            if (ModelState.IsValid)
            {

                var entity = new SubContractWorkItem
                {
                    SubContractWorkItemId = SubContractWorkItem.SubContractWorkItemId,
                    WorkCategoryId = SubContractWorkItem.WorkCategoryId,
                    UnitOfMeasurementId = SubContractWorkItem.UnitOfMeasurementId,
                    WorkItemName = SubContractWorkItem.WorkItemName,
                    ItemCode = SubContractWorkItem.ItemCode,
                    Quntity = SubContractWorkItem.Quntity,
                    SubContractCostOne = SubContractWorkItem.SubContractCostOne,
                    SubContractCostTwo = SubContractWorkItem.SubContractCostTwo,
                    SubContractCostThree = SubContractWorkItem.SubContractCostThree,
                    SelectedCost = SubContractWorkItem.SelectedCost,
                    TotalCost = SubContractWorkItem.TotalCost,
                    ProjectInformationId = idsTwo,

                    OrgBranchName = SubContractWorkItem.OrgBranchName,
                    DateCreated = DateTime.Now.Date,
                    LastModified = SubContractWorkItem.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = SubContractWorkItem.ModifiedBy,
                };
                entity.SelectedCost = (entity.SubContractCostOne + entity.SubContractCostTwo + entity.SubContractCostThree) / 3;
                entity.OverHeadProfit = (entity.SelectedCost * (OverHeadCostRate / 100)) +
                                          (entity.SelectedCost * (ProfitMarginRate / 100));
                entity.TotalCost = entity.SelectedCost + entity.OverHeadProfit;

                db.SubContractWorkItem.Add(entity);
                db.SaveChanges();
                //ProjectEstimationWorkItemController.ProjectId = 0;
                SubContractWorkItem.SubContractWorkItemId = entity.SubContractWorkItemId;

            }

            return Json(new[] { SubContractWorkItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SubContractWorkItem_Update([DataSourceRequest]DataSourceRequest request, SubContractWorkItem SubContractWorkItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new SubContractWorkItem
                {
                    SubContractWorkItemId = SubContractWorkItem.SubContractWorkItemId,
                    WorkCategoryId = SubContractWorkItem.WorkCategoryId,
                    UnitOfMeasurementId = SubContractWorkItem.UnitOfMeasurementId,
                    WorkItemName = SubContractWorkItem.WorkItemName,
                    ItemCode = SubContractWorkItem.ItemCode,
                    Quntity = SubContractWorkItem.Quntity,
                    SubContractCostOne = SubContractWorkItem.SubContractCostOne,
                    SubContractCostTwo = SubContractWorkItem.SubContractCostTwo,
                    SubContractCostThree = SubContractWorkItem.SubContractCostThree,
                    SelectedCost = SubContractWorkItem.SelectedCost,
                    TotalCost = SubContractWorkItem.TotalCost,
                    ProjectInformationId = SubContractWorkItem.ProjectInformationId,

                    OrgBranchName = SubContractWorkItem.OrgBranchName,
                    DateCreated = SubContractWorkItem.DateCreated,
                    LastModified = DateTime.Now.Date,
                    CreatedBy = SubContractWorkItem.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                entity.SelectedCost = (entity.SubContractCostOne + entity.SubContractCostTwo + entity.SubContractCostThree) / 3;
                entity.OverHeadProfit = (entity.SelectedCost * (OverHeadCostRate / 100)) +
                                          (entity.SelectedCost * (ProfitMarginRate / 100));
                entity.TotalCost = entity.SelectedCost + entity.OverHeadProfit;
                db = new ExceedDbContext();
                db.SubContractWorkItem.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { SubContractWorkItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SubContractWorkItem_Destroy([DataSourceRequest]DataSourceRequest request, SubContractWorkItem SubContractWorkItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new SubContractWorkItem
                {
                    SubContractWorkItemId = SubContractWorkItem.SubContractWorkItemId,
                    WorkCategoryId = SubContractWorkItem.WorkCategoryId,
                    UnitOfMeasurementId = SubContractWorkItem.UnitOfMeasurementId,
                    WorkItemName = SubContractWorkItem.WorkItemName,
                    ItemCode = SubContractWorkItem.ItemCode,
                    Quntity = SubContractWorkItem.Quntity,
                    SubContractCostOne = SubContractWorkItem.SubContractCostOne,
                    SubContractCostTwo = SubContractWorkItem.SubContractCostTwo,
                    SubContractCostThree = SubContractWorkItem.SubContractCostThree,
                    SelectedCost = SubContractWorkItem.SelectedCost,
                    TotalCost = SubContractWorkItem.TotalCost,
                    ProjectInformationId = SubContractWorkItem.ProjectInformationId,
                    OrgBranchName = SubContractWorkItem.OrgBranchName,
                    DateCreated = SubContractWorkItem.DateCreated,
                    LastModified = SubContractWorkItem.LastModified,
                    CreatedBy = SubContractWorkItem.CreatedBy,
                    ModifiedBy = SubContractWorkItem.ModifiedBy
                };

                db.SubContractWorkItem.Attach(entity);
                db.SubContractWorkItem.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { SubContractWorkItem }.ToDataSourceResult(request, ModelState));
        }

        #endregion
        #region Methods
        //private void GetWorkCategories(int id)
        //{
        //    var project = db.ProjectInformation.Where(x => x.ProjectInformationId == id).FirstOrDefault();
        //    if (project != null)
        //    {
        //        ProjectId = project.ProjectType;
        //    }
        //    var workItemCategories = from w in db.WorkCategory
        //                             join p in db.ProjectTypeMappings
        //                                 on w.ConfigurationId equals p.ConfigurationId
        //                             //where p.ProjectTypeId == ProjectId
        //                             select (new { w.WorkCategoryId, w.ConfigurationId, w.Description, w.Name, w.Index });
        //    ViewData["Categories"] = workItemCategories.ToList();
        //}

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

        private Block GetBlock(int blockMappingId)
        {
            BlockMapping blockMapping = new BlockMapping();
            Block block = new Block();
            var temp = db.BlockMappings.Find(blockMappingId);
            if (temp != null)
            {
                var tempBlock = db.Blocks.Find(temp.BlockId);
                if (tempBlock != null)
                {
                    block = tempBlock;
                }
            }
            return block;
        }

        //private void SetDefaultValue()
        //{
        //    var configartion = db.Configurations.ToList();
        //    if (configartion.Any())
        //    {
        //        var ovH = configartion.Where(x => x.Attribute == "OVER_HEAD_COST_RATE").FirstOrDefault();
        //        if (ovH != null)
        //        {
        //            OverHeadCost = Convert.ToDecimal(ovH.CurrentValue);
        //        }
        //        var profMa = configartion.Where(x => x.Attribute == "PROFIT_COST_RATE").FirstOrDefault();
        //        if (profMa != null)
        //        {
        //            ProfitMargin = Convert.ToDecimal(profMa.CurrentValue);
        //        }
        //    }
        //}
        public string ReturnValues()
        {
            return OverHeadCostRate + "-" + ProfitMarginRate;
        }
        public static void UpdateProjectTotalCost(int id)
        {
            ExceedDbContext db = new ExceedDbContext();

            decimal materialCost = 0;
            decimal labourCost = 0;
            decimal equipmentCost = 0;
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
            var workItem = db.WorkItemPerProject.FirstOrDefault(x => x.WorkItemPerProjectId == id);

            if (workItem != null)
            {
                var projectStatus =
                    db.ProjectInformation
                        .FirstOrDefault(x => x.ProjectInformationId == workItem.ProjectInformationId);
                if (projectStatus != null)
                {
                    if (projectStatus.ProjectStatus == ProjectStatus.ESTIMATION)
                    {
                        UpdateProjectEsTotalCost(id);
                    }
                    else
                    {
                        workItem.DirectCost = 0;
                        //UpdateMaterial(workItem.WorkItemPerProjectId);
                        var materials = db.ProjectMaterial.Where(x => x.WorkItemPerProjectId == workItem.WorkItemPerProjectId).ToList();
                        if (materials.Any())
                        {
                            materialCost = materials.Sum(x => x.TotalCost);
                        }
                        //UpdateEquimpment(workItem.WorkItemPerProjectId);
                        var equipments = db.ProjectEquipment.Where(x => x.WorkItemPerProjectId == workItem.WorkItemPerProjectId).ToList();
                        if (equipments.Any())
                        {
                            equipmentCost = equipments.Sum(x => x.TotalCost);
                        }
                        //UpdateLabour(workItem.WorkItemPerProjectId);
                        var labouCosts = db.ProjectDirectLabour.Where(x => x.WorkItemPerProjectId == workItem.WorkItemPerProjectId).ToList();
                        if (labouCosts.Any())
                        {
                            labourCost = labouCosts.Sum(x => x.TotalCost);

                        }
                        //if (workItem.OverheadRate != 0)
                        //{
                        //    overHeadPercent = workItem.OverheadRate;
                        //}
                        //if (workItem.ProfitMargin != 0)
                        //{
                        //    profitMarginPercent = workItem.ProfitMargin;
                        //}
                        workItem.MaterialCost = materialCost;
                        workItem.LabourOutPut = labourCost;
                        workItem.EquipmentOutput = equipmentCost;
                        workItem.DirectCost = materialCost + equipmentCost + labourCost;
                        workItem.OverHeadCost = workItem.DirectCost * (OverHeadCostRate / 100);
                        workItem.ProfitCost = workItem.DirectCost * (ProfitMarginRate / 100);
                        workItem.TotalUnitCost = workItem.LabourHourlyOutPut > 0 ? (workItem.DirectCost + workItem.OverHeadCost + workItem.ProfitCost) / workItem.LabourHourlyOutPut :
                            workItem.DirectCost + workItem.OverHeadCost + workItem.ProfitCost;
                        db = new ExceedDbContext();
                        var entity = new WorkItemPerProject
                        {
                            ItemName = workItem.ItemName,
                            BlockId = workItem.BlockId,
                            IsSubCategory = workItem.IsSubCategory,
                            ItemCode = workItem.ItemCode,
                            WorkItemPerProjectId = workItem.WorkItemPerProjectId,
                            WorkCategoryId = workItem.WorkCategoryId,
                            UnitOfMeasurementId = workItem.UnitOfMeasurementId,
                            ProjectEstimationWorkItemId = workItem.ProjectEstimationWorkItemId,
                            DirectCost = workItem.DirectCost,
                            TotalQuantityOfWork = workItem.TotalQuantityOfWork,
                            LabourOutPut = workItem.LabourOutPut,
                            EquipmentOutput = workItem.EquipmentOutput,
                            MaterialCostPerUnit = workItem.MaterialCostPerUnit,
                            LaborCostPerUnit = workItem.LaborCostPerUnit,
                            EquipmentCostPerUnit = workItem.EquipmentCostPerUnit,
                            TotalUnitCost = workItem.TotalUnitCost,
                            OverHeadCost = workItem.OverHeadCost,
                            ProfitCost = workItem.ProfitCost,
                            MaterialCost = workItem.MaterialCost,
                            Remark = workItem.Remark,
                            OverheadRate = OverHeadCostRate,
                            ProfitMargin = ProfitMarginRate,
                            ProjectType = workItem.ProjectType,
                            LabourHourlyOutPut = workItem.LabourHourlyOutPut,
                            ParentId = workItem.ParentId,
                            OrgBranchName = workItem.OrgBranchName,
                            DateCreated = workItem.DateCreated,
                            LastModified = DateTime.Now,
                            CreatedBy = workItem.CreatedBy,
                            ModifiedBy = CurrentUSer,
                            ProjectInformationId = workItem.ProjectInformationId,
                        };

                        db.WorkItemPerProject.Attach(entity);
                        db.Entry(entity).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }


            }

        }
        public static void UpdateProjectEsTotalCost(int id)
        {
            ExceedDbContext db = new ExceedDbContext();
            decimal materialCost = 0;
            decimal labourCost = 0; decimal equipmentCost = 0;
            decimal overHeadPercent = 0;
            decimal profitMarginPercent = 0;
            var configartions = db.Configurations.Where(x => x.Category == "Construction").ToList();
            if (configartions.Any())
            {
                var overheadRate = configartions.Where(x => x.Reference == "OVER_HEAD_COST_RATE").FirstOrDefault();
                var profiRate = configartions.Where(x => x.Reference == "PROFIT_COST_RATE").FirstOrDefault();
                if (overheadRate != null)
                {
                    overHeadPercent = Convert.ToDecimal(overheadRate.CurrentValue);
                }
                if (profiRate != null)
                {
                    profitMarginPercent = Convert.ToDecimal(profiRate.CurrentValue);
                }
            }
            var workItem = db.WorkItemPerProject.Where(x => x.WorkItemPerProjectId == id).FirstOrDefault();

            if (workItem != null)
            {
                var hourlyOutPut = workItem.LabourHourlyOutPut > 0 ? workItem.LabourHourlyOutPut : 1;

                workItem.DirectCost = 0;
                //UpdateEsMaterial(workItem.WorkItemPerProjectId);
                var materials = db.ProjectMaterial.Where(x => x.WorkItemPerProjectId == workItem.WorkItemPerProjectId).ToList();
                if (materials.Any())
                {
                    materialCost = materials.Sum(x => x.TotalCost);
                }
                //UpdateEsEquimpment(workItem.WorkItemPerProjectId);
                var equipments = db.ProjectEquipment.Where(x => x.WorkItemPerProjectId == workItem.WorkItemPerProjectId).ToList();
                if (equipments.Any())
                {
                    equipmentCost = equipments.Sum(x => x.TotalCost);
                }
                //UpdateESLabour(workItem.WorkItemPerProjectId);
                var labouCosts = db.ProjectDirectLabour.Where(x => x.WorkItemPerProjectId == workItem.WorkItemPerProjectId).ToList();
                if (labouCosts.Any())
                {
                    labourCost = labouCosts.Sum(x => x.TotalCost);
                }

                workItem.MaterialCost = materialCost;
                workItem.LabourOutPut = labourCost;
                workItem.EquipmentOutput = equipmentCost;
                workItem.MaterialCostPerUnit = materialCost;
                workItem.LaborCostPerUnit = labourCost / hourlyOutPut;
                workItem.EquipmentCostPerUnit = equipmentCost / hourlyOutPut;
                workItem.DirectCost = workItem.MaterialCostPerUnit + workItem.LaborCostPerUnit + workItem.EquipmentCostPerUnit;
                workItem.OverHeadCost = workItem.DirectCost * (overHeadPercent / 100);
                workItem.ProfitCost = workItem.DirectCost * (profitMarginPercent / 100);
                workItem.TotalUnitCost = workItem.DirectCost + workItem.OverHeadCost + workItem.ProfitCost;
                

                db.WorkItemPerProject.Attach(workItem);
                db.Entry(workItem).State = EntityState.Modified;
                db.SaveChanges();
            }

        }
        //public void CopyEstimation(int workItemId, int project, int id)
        //{
        //    var estimationWorkItems = db.ProjectEstimationWorkItem.Where(x => x.ProjectEstimationWorkItemId == workItemId).FirstOrDefault();
        //    WorkItemPerProject workItemPerProject = new WorkItemPerProject();
        //    workItemPerProject = db.WorkItemPerProject.Where(x => x.WorkItemPerProjectId == id).FirstOrDefault();

        //    if (estimationWorkItems != null)
        //    {
        //        #region Update WorkItem
        //        workItemPerProject.ItemName = estimationWorkItems.ItemName;
        //        //workItemPerProject.Name = estimationWorkItems.ItemName;

        //        workItemPerProject.UnitOfMeasurementId = estimationWorkItems.UnitOfMeasurementId ?? 0;
        //        workItemPerProject.TotalQuantityOfWork = estimationWorkItems.TotalQuantityOfWork;
        //        workItemPerProject.DirectCost = estimationWorkItems.DirectCost;
        //        workItemPerProject.EquipmentOutput = estimationWorkItems.EquipmentOutput;
        //        workItemPerProject.LabourHourlyOutPut = estimationWorkItems.LabourHourlyOutPut;
        //        workItemPerProject.MaterialCost = estimationWorkItems.MaterialCost;
        //        workItemPerProject.OverHeadCost = estimationWorkItems.OverHeadCost;
        //        workItemPerProject.LabourOutPut = estimationWorkItems.LabourHourlyOutPut;
        //        workItemPerProject.OverheadRate = estimationWorkItems.OverheadRate;
        //        workItemPerProject.ProfitCost = estimationWorkItems.ProfitCost;
        //        workItemPerProject.DirectCost = estimationWorkItems.DirectCost;
        //        workItemPerProject.ProfitMargin = estimationWorkItems.ProfitMargin;
        //        workItemPerProject.TotalUnitCost = estimationWorkItems.TotalUnitCost;
        //        db = new ExceedDbContext();
        //        db.WorkItemPerProject.Attach(workItemPerProject);
        //        db.Entry(workItemPerProject).State = EntityState.Modified;
        //        db.SaveChanges();
        //        #endregion

        //        #region Copy Material
        //        var etsmnMaterial = db.ProjectEstimationMaterial.Where(x => x.ProjectEstimationWorkItemId == estimationWorkItems.ProjectEstimationWorkItemId).ToList();
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
        //                    WorkItemPerProjectId = id,

        //                };
        //                db = new ExceedDbContext();
        //                db.ProjectMaterial.Add(entity);
        //                db.SaveChanges();
        //                //projectMaterial.ProjectMaterialId = entity.ProjectMaterialId;
        //            }
        //        }
        //        #endregion

        //        #region Copy Labour Cost
        //        var etsmnLabour = db.ProjectEstimationDirectLabour.Where(x => x.ProjectEstimationWorkItemId == estimationWorkItems.ProjectEstimationWorkItemId).ToList();
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
        //                    WorkItemPerProjectId = id,
        //                    ProjectEstimationIndexLabourId = m.ProjectEstimationIndexLabourId
        //                };
        //                db = new ExceedDbContext();
        //                db.ProjectDirectLabour.Add(entity);
        //                db.SaveChanges();
        //                //projectDirectLabour.ProjectDirectLabourId = entity.ProjectDirectLabourId;
        //            }
        //        }
        //        #endregion

        //        #region Copy Equipment
        //        var etsmnEquipment = db.ProjectEstimationEquipment.Where(x => x.ProjectEstimationWorkItemId == estimationWorkItems.ProjectEstimationWorkItemId).ToList();
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
        //                    WorkItemPerProjectId = id,

        //                };

        //                db.ProjectEquipment.Add(entity);
        //                db.SaveChanges();
        //                //projectEquipment.ProjectEquipmentId = entity.ProjectEquipmentId;
        //            }
        //        }
        //        #endregion

        //        #region Copy Takeoff sheet

        //        var etsmnTakeOfSheet =
        //            db.ProjectTakeOff.Where(
        //                x =>
        //                    x.ProjectEstimationWorkItemId == estimationWorkItems.ProjectEstimationWorkItemId).ToList();
        //        TakeOffPerProject takeOffPerProject = new TakeOffPerProject();
        //        if (etsmnTakeOfSheet.Any())
        //        {
        //            foreach (var m in etsmnTakeOfSheet)
        //            {
        //                takeOffPerProject = new TakeOffPerProject();
        //                var entity = new TakeOffPerProject
        //                {
        //                    TakeOffNo = m.TakeOffNo,
        //                    WorkItemPerProjectId = id,
        //                    ProjectInformationId = project,
        //                    Remark = m.Remark,
        //                    OrgBranchName = m.OrgBranchName,
        //                    DateCreated = DateTime.Now,
        //                    LastModified = null,
        //                    CreatedBy = User.Identity.Name,
        //                    ModifiedBy = null,

        //                };
        //                db = new ExceedDbContext();
        //                db.TakeOffPerProject.Add(entity);
        //                db.SaveChanges();
        //                takeOffPerProject.TakeOffPerProjectId = entity.TakeOffPerProjectId;

        //                #region Takeoff Line

        //                var takeoffline =
        //                    db.ProjectTakeOffLine.Where(x => x.ProjectTakeOffId == m.ProjectTakeOffId).ToList();
        //                if (takeoffline.Any())
        //                {
        //                    foreach (var t in takeoffline)
        //                    {
        //                        var entityi = new TakeOffLinePerProject
        //                        {
        //                            No = t.T,
        //                            D = t.D,
        //                            Quantity = t.S,
        //                            Description = t.Description,
        //                            UnitOfMeasurementId = t.UnitOfMeasurementId,
        //                            Length = t.Length,
        //                            Width = t.Width,
        //                            Depth = t.Height,
        //                            OrgBranchName = t.OrgBranchName,
        //                            DateCreated = DateTime.Now,
        //                            LastModified = null,
        //                            CreatedBy = User.Identity.Name,
        //                            ModifiedBy = null,
        //                            TakeOffLinePerProjectId = takeOffPerProject.TakeOffPerProjectId,

        //                        };
        //                        db = new ExceedDbContext();

        //                        db.TakeOffLinePerProject.Add(entityi);
        //                        db.SaveChanges();

        //                    }
        //                }

        //                #endregion

        //                #region Takeoff Line per Kg

        //                var takeofflineKg =
        //                    db.ProjectTakeOffLineForKg.Where(x => x.ProjectTakeOffId == m.ProjectTakeOffId)
        //                        .ToList();
        //                if (takeofflineKg.Any())
        //                {
        //                    foreach (var t in takeofflineKg)
        //                    {
        //                        var entityy = new TakeOffLineForKgPerProject
        //                        {
        //                            Description = t.Description,
        //                            //UnitRateId = t.UnitRateId,
        //                            Type = t.Type,
        //                            Shape = t.Shape,
        //                            Length = t.Length,
        //                            NoOfBars = t.NoOfBars,
        //                            NoOfElement = t.NoOfElement,
        //                            TotalBar = t.TotalBar,
        //                            TotalLength = t.TotalLength,
        //                            GrandTotal = t.GrandTotal,
        //                            OrgBranchName = t.OrgBranchName,
        //                            DateCreated = DateTime.Now,
        //                            LastModified = null,
        //                            CreatedBy = User.Identity.Name,
        //                            ModifiedBy = null,
        //                            TakeOffPerProjectId = takeOffPerProject.TakeOffPerProjectId
        //                        };
        //                        db = new ExceedDbContext();
        //                        db.TakeOffLineForKgPerProject.Add(entityy);
        //                        db.SaveChanges();

        //                    }
        //                }

        //                #endregion
        //            }
        //        }

        //        #endregion


        //    }




        //}

        private static decimal GetEquipmentUnitCost(int id)
        {
            decimal labourCost = 0;
            ExceedDbContext db = new ExceedDbContext();
            var workItem = db.WorkItemPerProject.Where(x => x.WorkItemPerProjectId == id).FirstOrDefault();

            if (workItem != null)
            {
                var labouCosts =
                    db.ProjectEquipment.Where(x => x.WorkItemPerProjectId == id).ToList();
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

        //private static decimal GetRentalRate(Guid id)
        //{
        //    ExceedDbContext db = new ExceedDbContext();
        //    decimal hourlyOutputRate = 0;
        //    var fleet = db.Fleets.Where(x => x.FleetsID == id).FirstOrDefault();
        //    if (fleet != null)
        //    {
        //        if (fleet.IsExternal)
        //        {
        //            var equipment = db.MachinaryRent.Where(em => em.FleetsID == id).FirstOrDefault();
        //            if (equipment != null)
        //            {
        //                hourlyOutputRate = equipment.RentalWithFuel;
        //            }
        //        }
        //        else
        //        {
        //            var equipment = db.InternalMachinaryRent.Where(im => im.FleetsID == id).FirstOrDefault();
        //            if (equipment != null)
        //            {
        //                hourlyOutputRate = equipment.HourlyRate;
        //            }
        //        }
        //    }

        //    return hourlyOutputRate;
        //}
        //public static void UpdateEquimpment(int id)
        //{
        //    ExceedDbContext dbContext = new ExceedDbContext();
        //    var equipments = dbContext.ProjectEquipment.Where(x => x.WorkItemPerProjectId == id).ToList();

        //    if (equipments.Any())
        //    {
        //        foreach (var e in equipments)
        //        {
        //            dbContext = new ExceedDbContext();
        //            var entity = new ProjectEquipment
        //            {
        //                ProjectEquipmentId = e.ProjectEquipmentId,
        //                FleetsID = e.FleetsID,
        //                //EquipmentIdentityID = e.EquipmentIdentityID,
        //                //EquipmentTypeID = e.EquipmentTypeID,
        //                Unit = e.Unit,
        //                Quantity = e.Quantity,
        //                UnitFactory = e.UnitFactory,
        //                RentalCost = e.RentalCost,
        //                TotalCost = (e.Quantity * e.RentalCost * e.UnitFactory),
        //                // ProjectEstimationWorkItem = e.ProjectEstimationWorkItem,
        //                WorkItemPerProjectId = e.WorkItemPerProjectId,
        //                EquipmentUnitCost = e.EquipmentUnitCost,
        //                OrgBranchName = e.OrgBranchName,
        //                DateCreated = e.DateCreated,
        //                LastModified = DateTime.Now,
        //                CreatedBy = e.CreatedBy,
        //                ModifiedBy = e.ModifiedBy
        //            };
        //            dbContext.ProjectEquipment.Attach(entity);
        //            dbContext.Entry(entity).State = EntityState.Modified;
        //            dbContext.SaveChanges();
        //        }
        //    }

        //}

        //public static void UpdateEsEquimpment(int id)
        //{
        //    ExceedDbContext dbContext = new ExceedDbContext();
        //    var equipments = dbContext.ProjectEquipment.Where(x => x.WorkItemPerProjectId == id).ToList();

        //    if (equipments.Any())
        //    {
        //        foreach (var e in equipments)
        //        {
        //            dbContext = new ExceedDbContext();
        //            var entity = new ProjectEquipment
        //            {
        //                ProjectEquipmentId = e.ProjectEquipmentId,
        //                FleetsID = e.FleetsID,
        //                //EquipmentIdentityID = e.EquipmentIdentityID,
        //                //EquipmentTypeID = e.EquipmentTypeID,
        //                Unit = e.Unit,
        //                Quantity = e.Quantity,
        //                UnitFactory = e.UnitFactory,
        //                RentalCost = ConstructionInputsHelper.GetRentalRate(e.FleetsID),
        //                TotalCost = (e.Quantity * e.RentalCost * e.UnitFactory),

        //                WorkItemPerProjectId = e.WorkItemPerProjectId,
        //                EquipmentUnitCost = GetEquipmentUnitCost(e.WorkItemPerProjectId),
        //                OrgBranchName = e.OrgBranchName,
        //                DateCreated = e.DateCreated,
        //                LastModified = DateTime.Now,
        //                CreatedBy = e.CreatedBy,
        //                ModifiedBy = e.ModifiedBy
        //            };
        //            entity.TotalCost = entity.RentalCost * entity.Quantity * e.UnitFactory;
        //            dbContext.ProjectEquipment.Attach(entity);
        //            dbContext.Entry(entity).State = EntityState.Modified;
        //            dbContext.SaveChanges();
        //        }
        //    }

        //}
        private static decimal GetMaterialUnitCost(int id)
        {
            decimal labourCost = 0;
            ExceedDbContext db = new ExceedDbContext();
            var workItem = db.WorkItemPerProject.Where(x => x.WorkItemPerProjectId == id).FirstOrDefault();

            if (workItem != null)
            {
                var labouCosts =
                    db.ProjectMaterial.Where(x => x.WorkItemPerProjectId == id).ToList();
                if (labouCosts.Any())
                {

                    foreach (var m in labouCosts)
                    {
                        labourCost += m.TotalCost;
                    }
                }
                labourCost = labourCost / workItem.TotalQuantityOfWork;
            }

            return labourCost;
        }
        //public static void UpdateMaterial(int id)
        //{
        //    ExceedDbContext dbContext = new ExceedDbContext();
        //    var equipments = dbContext.ProjectMaterial.Where(x => x.WorkItemPerProjectId == id).ToList();
        //    if (equipments.Any())
        //    {
        //        foreach (var e in equipments)
        //        {
        //            dbContext = new ExceedDbContext();
        //            var entity = new ProjectMaterial
        //            {
        //                ProjectMaterialId = e.ProjectMaterialId,
        //                ItemCode = e.ItemCode,
        //                ItemCategoryCode = e.ItemCategoryCode,
        //                Unit = e.Unit,
        //                Quantity = e.Quantity,
        //                UnitPrice = e.UnitPrice,
        //                TotalCost = e.Quantity * e.UnitPrice,
        //                WorkItemPerProjectId = e.WorkItemPerProjectId,
        //                MaterialUnitCost = GetMaterialUnitCost(e.WorkItemPerProjectId),
        //                OrgBranchName = e.OrgBranchName,
        //                DateCreated = e.DateCreated,
        //                LastModified = DateTime.Now,
        //                CreatedBy = e.CreatedBy,
        //                ModifiedBy = e.ModifiedBy
        //            };
        //            dbContext.ProjectMaterial.Attach(entity);
        //            dbContext.Entry(entity).State = EntityState.Modified;
        //            dbContext.SaveChanges();
        //        }
        //    }


        //}
        //public static void UpdateEsMaterial(int id)
        //{
        //    ExceedDbContext dbContext = new ExceedDbContext();
        //    var equipments = dbContext.ProjectMaterial.Where(x => x.WorkItemPerProjectId == id).ToList();
        //    if (equipments.Any())
        //    {
        //        foreach (var e in equipments)
        //        {
        //            dbContext = new ExceedDbContext();
        //            var entity = new ProjectMaterial
        //            {
        //                ProjectMaterialId = e.ProjectMaterialId,
        //                ItemCode = e.ItemCode,
        //                ItemCategoryCode = e.ItemCategoryCode,
        //                Unit = e.Unit,
        //                Quantity = e.Quantity,
        //                UnitPrice = e.UnitPrice,
        //                TotalCost = e.Quantity * e.UnitPrice,
        //                WorkItemPerProjectId = e.WorkItemPerProjectId,
        //                MaterialUnitCost = e.MaterialUnitCost,
        //                OrgBranchName = e.OrgBranchName,
        //                DateCreated = e.DateCreated,
        //                LastModified = DateTime.Now,
        //                CreatedBy = e.CreatedBy,
        //                ModifiedBy = e.ModifiedBy
        //            };
        //            dbContext.ProjectMaterial.Attach(entity);
        //            dbContext.Entry(entity).State = EntityState.Modified;
        //            dbContext.SaveChanges();
        //        }
        //    }


        //}

        //public static decimal GetIndexedHourlyPayPerJobTitle(int id)
        //{
        //    var db = new ExceedDbContext();
        //    var jobTitles = db.ProjectEstimationIndexLabour.Where(x => x.ProjectEstimationIndexLabourId == id).FirstOrDefault();
        //    if (jobTitles != null)
        //    {
        //        return jobTitles.ApprovedIndexedHourlySalary;
        //    }
        //    return 0;
        //}
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
            var workItem = db.WorkItemPerProject.Where(x => x.WorkItemPerProjectId == id).FirstOrDefault();

            if (workItem != null)
            {
                var labouCosts =
                    db.ProjectDirectLabour.Where(x => x.WorkItemPerProjectId == id).ToList();
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
        //public static void UpdateLabour(int id)
        //{
        //    ExceedDbContext dbContext = new ExceedDbContext();
        //    var labours = dbContext.ProjectDirectLabour.Where(x => x.WorkItemPerProjectId == id).ToList();


        //    if (labours.Any())
        //    {
        //        foreach (var e in labours)
        //        {
        //            dbContext = new ExceedDbContext();
        //            var entity = new ProjectDirectLabour
        //            {
        //                ProjectDirectLabourId = e.ProjectDirectLabourId,

        //                ProjectEstimationIndexLabourId = e.ProjectEstimationIndexLabourId,
        //                Quantity = e.Quantity,
        //                UnitFactory = e.UnitFactory,
        //                IndexedHourlySalary = e.IndexedHourlySalary,
        //                DailyPay = e.DailyPay,
        //                TotalCost = e.Quantity * e.UnitFactory *
        //                         e.IndexedHourlySalary,
        //                WorkItemPerProjectId = e.WorkItemPerProjectId,
        //                ManpowerUnitCost = e.ManpowerUnitCost,
        //                OrgBranchName = e.OrgBranchName,
        //                CreatedBy = e.CreatedBy,
        //                ModifiedBy = e.ModifiedBy,
        //                DateCreated = e.DateCreated,
        //                LastModified = DateTime.Now
        //            };
        //            dbContext.ProjectDirectLabour.Attach(entity);
        //            dbContext.Entry(entity).State = EntityState.Modified;
        //            dbContext.SaveChanges();
        //        }
        //    }

        //}

        //public static void UpdateESLabour(int id)
        //{
        //    ExceedDbContext dbContext = new ExceedDbContext();
        //    var labours = dbContext.ProjectDirectLabour.Where(x => x.WorkItemPerProjectId == id).ToList();


        //    if (labours.Any())
        //    {
        //        foreach (var e in labours)
        //        {
        //            dbContext = new ExceedDbContext();
        //            var entity = new ProjectDirectLabour
        //            {
        //                ProjectDirectLabourId = e.ProjectDirectLabourId,

        //                ProjectEstimationIndexLabourId = e.ProjectEstimationIndexLabourId,
        //                Quantity = e.Quantity,
        //                UnitFactory = e.UnitFactory,
        //                IndexedHourlySalary = GetIndexedHourlyPayPerJobTitle(e.ProjectEstimationIndexLabourId),
        //                DailyPay = GetDailyPayPerJobTitle(e.ProjectEstimationIndexLabourId),
        //                TotalCost = e.Quantity * e.UnitFactory *
        //                         e.IndexedHourlySalary,
        //                WorkItemPerProjectId = e.WorkItemPerProjectId,
        //                ManpowerUnitCost = GetManPowerUnitCost(e.WorkItemPerProjectId),
        //                OrgBranchName = e.OrgBranchName,
        //                CreatedBy = e.CreatedBy,
        //                ModifiedBy = e.ModifiedBy,
        //                DateCreated = e.DateCreated,
        //                LastModified = DateTime.Now
        //            };
        //            entity.TotalCost = e.Quantity * e.UnitFactory *
        //                               entity.IndexedHourlySalary;
        //            dbContext.ProjectDirectLabour.Attach(entity);
        //            dbContext.Entry(entity).State = EntityState.Modified;
        //            dbContext.SaveChanges();
        //        }
        //    }

        //}

        public static void UpdateProjectTotalCost(List<int> workItemPerProjects)
        {

            foreach (var id in workItemPerProjects)
            {
                ExceedDbContext db = new ExceedDbContext();


                var workItem = db.WorkItemPerProject.Where(x => x.WorkItemPerProjectId == id).FirstOrDefault();

                if (workItem != null)
                {

                    var projectStatus = db.ProjectInformation.Where(x => x.ProjectInformationId == workItem.ProjectInformationId).FirstOrDefault();
                    if (projectStatus != null)
                    {
                        if (projectStatus.ProjectStatus == ProjectStatus.ESTIMATION)
                        {
                            UpdateProjectEsTotalCost(workItem.WorkItemPerProjectId);
                        }
                        else
                        {
                            UpdateProjectTotalCost(workItem.WorkItemPerProjectId);
                        }
                    }
                }
            }
        }


        #endregion




        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
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
    }
}
