using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Helpers.Construction;
using System.Data.Entity.Validation;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.Construction.Controllers.Estimation
{
    [AuthorizeRoles(ConstructionConstants.EstimationUser)]
    public class ProjectEstimationBoqBreakdownController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {

            db.Configuration.ProxyCreationEnabled = false;
            var lookups = db.Lookups.ToList();
            //var projects = db.ProjectInformation.ToList();
            //ViewData["ProjectList"] = projects;
            var categories = db.WorkCategory;
            ViewData["Categories"] = categories;
            var projectTypes = lookups.Where(x => x.LookupType == LookUpTypes.PROJECT_TYPE).ToList();
            ViewData["ProjectTypeList"] = projectTypes;
            var uom = db.UnitOfMeasurement.ToList();
            ViewData["uom"] = uom.Select(
                        x =>
                            new
                            {
                                UnitOfMeasurementId = x.UnitOfMeasurementId,
                                Abbreviation = x.Abbreviation
                            }).ToList();
            //var 
            return View();
        }
        //[HttpGet]

        public ActionResult ProjectEstimationBoqBreakdown_Read([DataSourceRequest]DataSourceRequest request)
        {

            IQueryable<ProjectEstimationBoqBreakdown> projectestimationboqbreakdown = db.ProjectEstimationBoqBreakdown.OrderBy(x => x.BillNumber).AsQueryable();
            //if (ProjectId != 0)
            //{
            var project =
                projectestimationboqbreakdown.Where(x => x.ProjectInformationId == ProjectId).OrderBy(x => x.BillNumber).AsQueryable();

            DataSourceResult result = project.ToDataSourceResult(request,
                projectEstimationBoqBreakdown => new
                {
                    ProjectEstimationBoqBreakdownId = projectEstimationBoqBreakdown.ProjectEstimationBoqBreakdownId,
                    ProjectInformationId = projectEstimationBoqBreakdown.ProjectInformationId,
                    WorkItemPerProjectId = projectEstimationBoqBreakdown.GenericWorkItemPerProjectId,
                    UnitOfMeasurementId = projectEstimationBoqBreakdown.UnitOfMeasurementId,
                    BillNumber = projectEstimationBoqBreakdown.BillNumber,
                    Description = projectEstimationBoqBreakdown.Description,
                    Quantity = projectEstimationBoqBreakdown.Quantity,
                    UnitRate = projectEstimationBoqBreakdown.UnitRate,
                    TotalCost = projectEstimationBoqBreakdown.TotalCost,
                    ConfigurationId = projectEstimationBoqBreakdown.ConfigurationId,
                    WorkCategoryId = projectEstimationBoqBreakdown.WorkCategoryId,
                    //Category = GetName(projectEstimationBoqBreakdown.WorkCategoryId).Name,
                   
                    Index = projectEstimationBoqBreakdown.Index,

                    WorkCategory =projectEstimationBoqBreakdown.WorkCategory,
                    StructureyType = projectEstimationBoqBreakdown.StructureyType,
                    SubProjectTypeName = projectEstimationBoqBreakdown.SubProjectTypeName,

                    OrgBranchName = projectEstimationBoqBreakdown.OrgBranchName,
                    DateCreated = projectEstimationBoqBreakdown.DateCreated,
                    LastModified = projectEstimationBoqBreakdown.LastModified,
                    CreatedBy = projectEstimationBoqBreakdown.CreatedBy,
                    ModifiedBy = projectEstimationBoqBreakdown.ModifiedBy,
                    SubProjectTypeId = projectEstimationBoqBreakdown.SubProjectTypeId,
                });
            return Json(result, JsonRequestBehavior.AllowGet);

            //}

            //else
            //{

            //    DataSourceResult result = projectestimationboqbreakdown.ToDataSourceResult(request,
            //        projectEstimationBoqBreakdown => new
            //        {
            //            ProjectEstimationBoqBreakdownId = projectEstimationBoqBreakdown.ProjectEstimationBoqBreakdownId,
            //            ProjectInformationId = projectEstimationBoqBreakdown.ProjectInformationId,
            //            WorkItemPerProjectId = projectEstimationBoqBreakdown.WorkItemPerProjectId,
            //            UnitOfMeasurementId = projectEstimationBoqBreakdown.UnitOfMeasurementId,
            //            BillNumber = projectEstimationBoqBreakdown.BillNumber,
            //            Description = projectEstimationBoqBreakdown.Description,
            //            Quantity = projectEstimationBoqBreakdown.Quantity,
            //            UnitRate = projectEstimationBoqBreakdown.UnitRate,
            //            TotalCost = projectEstimationBoqBreakdown.TotalCost,
            //            ConfigurationId = projectEstimationBoqBreakdown.ConfigurationId,
            //            WorkCategoryId = projectEstimationBoqBreakdown.WorkCategoryId,
            //            Category = GetName(projectEstimationBoqBreakdown.WorkCategoryId).Name,
            //            Type =
            //                GetName(projectEstimationBoqBreakdown.WorkCategoryId).Configuration.CurrentValue.ToString(),
            //            Index = GetName(projectEstimationBoqBreakdown.WorkCategoryId).Index,
            //            WorkCategory = GetName(projectEstimationBoqBreakdown.WorkCategoryId),
            //            OrgBranchName = projectEstimationBoqBreakdown.OrgBranchName,
            //            DateCreated = projectEstimationBoqBreakdown.DateCreated,
            //            LastModified = projectEstimationBoqBreakdown.LastModified,
            //            CreatedBy = projectEstimationBoqBreakdown.CreatedBy,
            //            ModifiedBy = projectEstimationBoqBreakdown.ModifiedBy

            //        });
            //    return Json(result);
            //}


        }

        private decimal GetQuntity(int WorkItemId, decimal workItemQuantity)
        {
            decimal quntity = workItemQuantity;
            var takeOffSheet =
                db.TakeOffPerProject.Where(x => x.WorkItemPerProjectId == WorkItemId).FirstOrDefault();
            if (takeOffSheet != null)
            {
                var lines =
                    db.TakeOffLinePerProject.Where(x => x.TakeOffPerProjectId == takeOffSheet.TakeOffPerProjectId).ToList();
                if (lines.Any())
                {
                    quntity = lines.Sum(x => x.Quantity);
                    return quntity;
                }
                //var kgtakeof = db.TakeOffLineForKgPerProject.Where(x => x.TakeOffPerProjectId == takeOffSheet.TakeOffPerProjectId).ToList();
                //if (kgtakeof.Any())
                //{
                //    quntity = kgtakeof.Sum(x => x.GrandTotal);
                //    return quntity;
                //}
            }

            return quntity;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstimationBoqBreakdown_Create([DataSourceRequest]DataSourceRequest request, ProjectEstimationBoqBreakdown projectEstimationBoqBreakdown)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEstimationBoqBreakdown
                {
                    ProjectInformationId = ProjectId,
                    UnitOfMeasurementId = projectEstimationBoqBreakdown.UnitOfMeasurementId,
                    BillNumber = projectEstimationBoqBreakdown.BillNumber,
                    Description = projectEstimationBoqBreakdown.Description,
                    Quantity = projectEstimationBoqBreakdown.Quantity,
                    UnitRate = projectEstimationBoqBreakdown.UnitRate,
                    TotalCost = projectEstimationBoqBreakdown.TotalCost,
                    ConfigurationId = projectEstimationBoqBreakdown.ConfigurationId,
                    WorkCategoryId = projectEstimationBoqBreakdown.WorkCategoryId,
                    SubProjectTypeId = projectEstimationBoqBreakdown.SubProjectTypeId,
                    GenericWorkItemPerProjectId =
                        projectEstimationBoqBreakdown.GenericWorkItemPerProjectId,
                    OrgBranchName = projectEstimationBoqBreakdown.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = projectEstimationBoqBreakdown.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = projectEstimationBoqBreakdown.ModifiedBy
                };
                var category = GetName(entity.WorkCategoryId);
                if (category != null)
                {
                    entity.WorkCategory = category.Name;
                    entity.StructureyType = category.Configuration.CurrentValue.ToString();
                    entity.Index = category.Index;
                    entity.WorkCategoryId = entity.WorkCategoryId;
                }
                entity.TotalCost = entity.Quantity * entity.UnitRate;
                db.ProjectEstimationBoqBreakdown.Add(entity);
                db.SaveChanges();
                projectEstimationBoqBreakdown.ProjectEstimationBoqBreakdownId = entity.ProjectEstimationBoqBreakdownId;
            }

            return Json(new[] { projectEstimationBoqBreakdown }.ToDataSourceResult(request, ModelState));
        }
        private WorkCategory GetName(int id)
        {
            var name = db.WorkCategory.Where(x => x.WorkCategoryId == id).FirstOrDefault();

            return name;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstimationBoqBreakdown_Update([DataSourceRequest]DataSourceRequest request, ProjectEstimationBoqBreakdown projectEstimationBoqBreakdown)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEstimationBoqBreakdown
                {
                    ProjectEstimationBoqBreakdownId = projectEstimationBoqBreakdown.ProjectEstimationBoqBreakdownId,
                    ProjectInformationId = projectEstimationBoqBreakdown.ProjectInformationId,
                    UnitOfMeasurementId = projectEstimationBoqBreakdown.UnitOfMeasurementId,
                    BillNumber = projectEstimationBoqBreakdown.BillNumber,
                    Description = projectEstimationBoqBreakdown.Description,
                    Quantity = projectEstimationBoqBreakdown.Quantity,
                    UnitRate = projectEstimationBoqBreakdown.UnitRate,
                    TotalCost = projectEstimationBoqBreakdown.TotalCost,
                    ConfigurationId = projectEstimationBoqBreakdown.ConfigurationId,
                    WorkCategoryId = projectEstimationBoqBreakdown.WorkCategoryId,
                    WorkCategory = GetWorkItemName(projectEstimationBoqBreakdown.WorkCategoryId),
                    StructureyType = GetType(projectEstimationBoqBreakdown.WorkCategoryId),
                    Index = GetIndex(projectEstimationBoqBreakdown.WorkCategoryId),
                    SubProjectTypeId = projectEstimationBoqBreakdown.SubProjectTypeId,
                    GenericWorkItemPerProjectId =
                              projectEstimationBoqBreakdown.GenericWorkItemPerProjectId,
                    OrgBranchName = projectEstimationBoqBreakdown.OrgBranchName,
                    DateCreated = projectEstimationBoqBreakdown.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = projectEstimationBoqBreakdown.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };
                var category = GetName(entity.WorkCategoryId);
                if (category != null)
                {
                    entity.WorkCategory = category.Name;
                    entity.StructureyType = category.Configuration.CurrentValue.ToString();
                    entity.Index = category.Index;
                    entity.WorkCategoryId = entity.WorkCategoryId;
                }
                entity.TotalCost = entity.Quantity * entity.UnitRate;
                db.ProjectEstimationBoqBreakdown.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { projectEstimationBoqBreakdown }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstimationBoqBreakdown_Destroy([DataSourceRequest]DataSourceRequest request, ProjectEstimationBoqBreakdown projectEstimationBoqBreakdown)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEstimationBoqBreakdown
                {
                    ProjectEstimationBoqBreakdownId = projectEstimationBoqBreakdown.ProjectEstimationBoqBreakdownId,
                    ProjectInformationId = projectEstimationBoqBreakdown.ProjectInformationId,
                    UnitOfMeasurementId = projectEstimationBoqBreakdown.UnitOfMeasurementId,
                    BillNumber = projectEstimationBoqBreakdown.BillNumber,
                    Description = projectEstimationBoqBreakdown.Description,
                    Quantity = projectEstimationBoqBreakdown.Quantity,
                    UnitRate = projectEstimationBoqBreakdown.UnitRate,
                    TotalCost = projectEstimationBoqBreakdown.TotalCost,
                    ConfigurationId = projectEstimationBoqBreakdown.ConfigurationId,
                    WorkCategoryId = projectEstimationBoqBreakdown.WorkCategoryId,
                    WorkCategory = projectEstimationBoqBreakdown.WorkCategory,
                    StructureyType = projectEstimationBoqBreakdown.StructureyType,
                    Index = projectEstimationBoqBreakdown.Index,
                    SubProjectTypeId = projectEstimationBoqBreakdown.SubProjectTypeId,
                    GenericWorkItemPerProjectId =
                              projectEstimationBoqBreakdown.GenericWorkItemPerProjectId,
                    OrgBranchName = projectEstimationBoqBreakdown.OrgBranchName,
                    DateCreated = projectEstimationBoqBreakdown.DateCreated,
                    LastModified = projectEstimationBoqBreakdown.LastModified,
                    CreatedBy = projectEstimationBoqBreakdown.CreatedBy,
                    ModifiedBy = projectEstimationBoqBreakdown.ModifiedBy
                };

                db.ProjectEstimationBoqBreakdown.Attach(entity);
                db.ProjectEstimationBoqBreakdown.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { projectEstimationBoqBreakdown }.ToDataSourceResult(request, ModelState));
        }
        public static int ProjectId { get; set; }
        public static int ProjectType { get; set; }
        //[HttpGet]
        public ActionResult SetProjectId(int value)
        {
            var projectestimationboqbreakdown = db.ProjectEstimationBoqBreakdown.AsQueryable();
            ProjectId = 0;
            ProjectId = value;
            ProjectEstimationBoqDetailController.ProjectId = value;
            int index = 1;
            var list = new List<KeyValuePair<int, string>>();

            if (ProjectId > 0)
            {

                projectestimationboqbreakdown =
                    projectestimationboqbreakdown.Where(x => x.ProjectInformationId == ProjectId).AsQueryable();
            }

            if (projectestimationboqbreakdown.Any())
            {
                db.ProjectEstimationBoqBreakdown.RemoveRange(projectestimationboqbreakdown);
                db.SaveChanges();
            }

            var workItems = db.GenericWorkItemPerProject.Where(x => x.ProjectInformationId == ProjectId && x.ParentId == 0)
                        .OrderBy(x => x.WorkCategory.Index)
                        .ToList();
            //var areaWorkItems = db.GenericAreaWorkItem.Where(x => x.ProjectInformationId == ProjectId)
            //         .OrderBy(x => x.WorkCategory.Index)
            //         .ToList();
            //var subContractWorkItems = db.GenericSubContractWorkItem.Where(x => x.ProjectInformationId == ProjectId)
            //        .OrderBy(x => x.WorkCategory.Index)
            //        .ToList();
            var customWorkItemds = db.GenericProjectCustomWorkItem.Where(cw => cw.ProjectInformationId == ProjectId).ToList();

            if (workItems.Any())
            {
                var boqEntity = new ProjectEstimationBoqBreakdown();
                #region WorkItem
                foreach (var workItem in workItems)
                {
                    boqEntity = new ProjectEstimationBoqBreakdown();
                    db = new ExceedDbContext();
                    boqEntity.Description = GetWorkItemName(workItem.GenericWorkItemPerProjectId);
                    //project.TotalCost =workItems[w].TotalUnitCost; //input from take off sheet
                    boqEntity.UnitRate = workItem.TotalUnitCost;
                    //boqEntity.Quantity = GetQuntity(workItem.WorkItemPerProjectId,
                    //    workItem.TotalQuantityOfWork);
                    boqEntity.Quantity = workItem.TotalQuantityOfWork;
                    boqEntity.TotalCost = workItem.TotalUnitCost * workItem.TotalQuantityOfWork;
                    boqEntity.UnitOfMeasurementId = workItem.UnitOfMeasurementId ?? 0;
                    boqEntity.WorkCategoryId = workItem.WorkCategoryId;
                    boqEntity.ConfigurationId = workItem.WorkCategory.ConfigurationId;
                    boqEntity.WorkCategory = workItem.WorkCategory.Name;
                    boqEntity.StructureyType = workItem.WorkCategory.Configuration.CurrentValue.ToString();
                    boqEntity.SubProjectTypeName = ProjectEstimationHelper.GetSubProjectTypeName(workItem.SubProjectTypeId);

                    boqEntity.Index = GetBoQSubIndex(workItem.WorkCategoryId, workItem.WorkCategory.ConfigurationId, ProjectId);
                    //boqEntity.SubProjectTypeId = GetWorkItemBlockId(workItem.BlockId);
                    boqEntity.SubProjectTypeId = workItem.SubProjectTypeId ?? 0;
                    boqEntity.GenericWorkItemPerProjectId = workItem.GenericWorkItemPerProjectId;

                    boqEntity.ProjectInformationId = ProjectId;
                    //if (list.Contains(new KeyValuePair<int, string>(project.Index, project.Type)))
                    //{
                    //    project.BillNumber = project.Index + "." + index;
                    //    index = +1;
                    //    list.Add(new KeyValuePair<int, string>(project.Index, project.Type));
                    //}
                    //else
                    //{
                    //    list = new List<KeyValuePair<int, string>>();
                    //    index = 1;
                    //    project.BillNumber = project.Index + "." + index;
                    //    list.Add(new KeyValuePair<int, string>(project.Index, project.Type));
                    //}
                    if (workItem.ProjectInformation.ProjectType == AvailableProjectTypes.Road && workItem.ItemCode != null)
                    {
                        boqEntity.BillNumber = workItem.ItemCode;
                    }
                    else
                    {
                        boqEntity.BillNumber = workItem.WorkCategory.Index + "." + boqEntity.Index;
                    }
                    db = new ExceedDbContext();
                    db.ProjectEstimationBoqBreakdown.Add(boqEntity);
                    db.SaveChanges();
                    boqEntity.ProjectEstimationBoqBreakdownId = boqEntity.ProjectEstimationBoqBreakdownId;


                    var subWorkItems = db.GenericWorkItemPerProject.Where(w => w.ParentId == workItem.GenericWorkItemPerProjectId).ToList();
                    if (subWorkItems.Any())
                    {
                        foreach (var subworkItem in subWorkItems)
                        {
                            var detailBoQ = new ProjectEstimationBoqDetail
                            {
                                ProjectEstimationBoqBreakdownId = boqEntity.ProjectEstimationBoqBreakdownId,
                                Description = GetWorkItemName(subworkItem.GenericWorkItemPerProjectId),
                                UnitRate = subworkItem.TotalUnitCost,
                                Quntity = subworkItem.TotalQuantityOfWork,
                                UnitOfMeasurementId = subworkItem.UnitOfMeasurementId ?? 0,
                                Index = this.GetBoQSubIndexForSubWorkItem(boqEntity),
                            };
                            detailBoQ.TotalCost = detailBoQ.Quntity * detailBoQ.UnitRate;
                            if (subworkItem.ProjectInformation.ProjectType == AvailableProjectTypes.Road)
                            {
                                detailBoQ.BillNumber = subworkItem.ItemCode;
                            }
                            else
                            {
                                detailBoQ.BillNumber = boqEntity.BillNumber + "." + detailBoQ.Index;
                            }

                            db = new ExceedDbContext();
                            db.ProjectEstimationBoqDetail.Add(detailBoQ);
                            db.SaveChanges();
                        }
                    }

                }
                #endregion
                #region PieceRate WorkItem
                //foreach (var areaWorkItem in areaWorkItems)
                //{
                //    boqEntity = new ProjectEstimationBoqBreakdown();
                //    db = new ExceedDbContext();
                //    boqEntity.Description = GetAreaWorkItemName(areaWorkItem.GenericAreaWorkItemId);
                //    //project.TotalCost =workItems[w].TotalUnitCost; //input from take off sheet
                //    boqEntity.UnitRate = areaWorkItem.OverHeadProfitCost;
                //    boqEntity.Quantity = 1;
                //    boqEntity.TotalCost = areaWorkItem.OverHeadProfitCost * boqEntity.Quantity;
                //    boqEntity.UnitOfMeasurementId = GetUnitOfMeasuremnt("M2");
                //    boqEntity.WorkCategoryId = areaWorkItem.WorkCategoryId;
                //    boqEntity.ConfigurationId = areaWorkItem.WorkCategory.ConfigurationId;
                //    boqEntity.Category = areaWorkItem.WorkCategory.Name;
                //    boqEntity.Type = areaWorkItem.WorkCategory.Configuration.CurrentValue.ToString();
                //    boqEntity.Index = GetBoQSubIndex(areaWorkItem.WorkCategoryId, areaWorkItem.WorkCategory.ConfigurationId, ProjectId);
                //    boqEntity.SubProjectTypeId = areaWorkItem.SubProjectTypeId;
                //    boqEntity.GenericWorkItemPerProjectId = areaWorkItem.GenericAreaWorkItemId;

                //    boqEntity.ProjectInformationId = ProjectId;
                //    #region MyRegion
                //    //if (list.Contains(new KeyValuePair<int, string>(project.Index, project.Type)))
                //    //{
                //    //    project.BillNumber = project.Index + "." + index;
                //    //    index = +1;
                //    //    list.Add(new KeyValuePair<int, string>(project.Index, project.Type));
                //    //}
                //    //else
                //    //{
                //    //    list = new List<KeyValuePair<int, string>>();
                //    //    index = 1;
                //    //    project.BillNumber = project.Index + "." + index;
                //    //    list.Add(new KeyValuePair<int, string>(project.Index, project.Type));
                //    //} 
                //    #endregion
                //    if (areaWorkItem.ProjectInformation.ProjectType == AvailableProjectTypes.Road && areaWorkItem.ItemCode != null)
                //    {
                //        boqEntity.BillNumber = areaWorkItem.ItemCode;
                //    }
                //    else
                //    {
                //        boqEntity.BillNumber = areaWorkItem.WorkCategory.Index + "." + boqEntity.Index;
                //    }
                //    db = new ExceedDbContext();
                //    db.ProjectEstimationBoqBreakdown.Add(boqEntity);
                //    db.SaveChanges();
                //    boqEntity.ProjectEstimationBoqBreakdownId = boqEntity.ProjectEstimationBoqBreakdownId;
                //}
                #endregion

                #region Sub contarct WorkItem
                //foreach (var workItem in subContractWorkItems)
                //{
                //    boqEntity = new ProjectEstimationBoqBreakdown();
                //    db = new ExceedDbContext();
                //    boqEntity.Description = GetSubContractWorkItemName(workItem.GenericSubContractWorkItemId);
                //    //project.TotalCost =workItems[w].TotalUnitCost; //input from take off sheet
                //    boqEntity.UnitRate = workItem.TotalCost;
                //    boqEntity.Quantity = workItem.Quntity;
                //    boqEntity.TotalCost = workItem.TotalCost;
                //    boqEntity.UnitOfMeasurementId = workItem.UnitOfMeasurementId;
                //    boqEntity.WorkCategoryId = workItem.WorkCategoryId;
                //    boqEntity.ConfigurationId = workItem.WorkCategory.ConfigurationId;
                //    boqEntity.Category = workItem.WorkCategory.Name;
                //    boqEntity.Type = workItem.WorkCategory.Configuration.CurrentValue.ToString();
                //    boqEntity.Index = GetBoQSubIndex(workItem.WorkCategoryId, workItem.WorkCategory.ConfigurationId, ProjectId);
                //    boqEntity.SubProjectTypeId = workItem.SubProjectTypeId;
                //    boqEntity.GenericWorkItemPerProjectId = workItem.GenericSubContractWorkItemId;

                //    boqEntity.ProjectInformationId = ProjectId;
                //    #region MyRegion
                //    //if (list.Contains(new KeyValuePair<int, string>(project.Index, project.Type)))
                //    //{
                //    //    project.BillNumber = project.Index + "." + index;
                //    //    index = +1;
                //    //    list.Add(new KeyValuePair<int, string>(project.Index, project.Type));
                //    //}
                //    //else
                //    //{
                //    //    list = new List<KeyValuePair<int, string>>();
                //    //    index = 1;
                //    //    project.BillNumber = project.Index + "." + index;
                //    //    list.Add(new KeyValuePair<int, string>(project.Index, project.Type));
                //    //} 
                //    #endregion
                //    if (workItem.ProjectInformation.ProjectType == AvailableProjectTypes.Road && workItem.ItemCode != null)
                //    {
                //        boqEntity.BillNumber = workItem.ItemCode;
                //    }
                //    else
                //    {
                //        boqEntity.BillNumber = workItem.WorkCategory.Index + "." + boqEntity.Index;
                //    }

                //    db = new ExceedDbContext();
                //    //db.ProjectEstimationBoqBreakdown.Add(boqEntity);
                //    //db.SaveChanges();
                //    try
                //    {
                //        db.ProjectEstimationBoqBreakdown.Add(boqEntity);
                //        db.SaveChanges();
                //    }
                //    catch (DbEntityValidationException ex)
                //    {
                //        foreach (var entityValidationErrors in ex.EntityValidationErrors)
                //        {
                //            foreach (var validationError in entityValidationErrors.ValidationErrors)
                //            {
                //                Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                //            }
                //        }
                //    }
                //    //boqEntity.ProjectEstimationBoqBreakdownId = boqEntity.ProjectEstimationBoqBreakdownId;

                //}
                #endregion

                #region custom work items
                foreach (var workItem in customWorkItemds)
                {
                    boqEntity = new ProjectEstimationBoqBreakdown();
                    db = new ExceedDbContext();
                    boqEntity.Description = GetCustomWorkItemName(workItem.GenericProjectCustomWorkItemId);
                    //project.TotalCost =workItems[w].TotalUnitCost; //input from take off sheet
                    boqEntity.UnitRate = workItem.TotalUnitRate;
                    boqEntity.Quantity = workItem.TotalQuantityOfWork;
                    boqEntity.TotalCost = workItem.TotalCost;
                    boqEntity.WorkCategoryId = workItem.WorkCategoryId;
                    boqEntity.ConfigurationId = ProjectEstimationHelper.GetCustomWorkItemCategory(workItem.WorkCategoryId).ConfigurationId;
                    boqEntity.WorkCategory = ProjectEstimationHelper.GetCustomWorkItemCategory(workItem.WorkCategoryId).Name;
                    boqEntity.StructureyType = ProjectEstimationHelper.GetWorkCategoryStructureType(workItem.WorkCategoryId).CurrentValue.ToString();
                    boqEntity.SubProjectTypeName = ProjectEstimationHelper.GetSubProjectTypeName(workItem.SubProjectTypeId);

                    boqEntity.Index = GetBoQSubIndex(workItem.WorkCategoryId, ProjectEstimationHelper.GetCustomWorkItemCategory(workItem.WorkCategoryId).ConfigurationId, ProjectId);
                    boqEntity.SubProjectTypeId = workItem.SubProjectTypeId ?? 0;

                    boqEntity.ProjectInformationId = ProjectId;

                    if (workItem.ProjectType == AvailableProjectTypes.Road && workItem.ItemCode != null)
                    {
                        boqEntity.BillNumber = workItem.ItemCode;
                    }
                    else
                    {
                        boqEntity.BillNumber = ProjectEstimationHelper.GetCustomWorkItemCategory(workItem.WorkCategoryId).Index + "." + boqEntity.Index;
                    }

                    db = new ExceedDbContext();
                    db.ProjectEstimationBoqBreakdown.Add(boqEntity);
                    db.SaveChanges();
                    boqEntity.ProjectEstimationBoqBreakdownId = boqEntity.ProjectEstimationBoqBreakdownId;

                }
                #endregion
            }

            return RedirectToAction("ProjectEstimationBoqBreakdown_Read");
        }

        private int GetWorkItemBlockId(int? id)
        {
            int idToReturn = 0;
            if (id != null)
            {
                idToReturn = (int)id;
            }
            return idToReturn;
        }
        public int GetUnitOfMeasuremnt(string unit)
        {
            var unitOfMeasurement = db.UnitOfMeasurement.Where(x => x.Abbreviation.ToLower() == "m2").FirstOrDefault();
            if (unitOfMeasurement != null)
            {
                return unitOfMeasurement.UnitOfMeasurementId;
            }
            return 0;
        }
        public string GetWorkItemName(int id)
        {
            var workitem =
                db.GenericWorkItemPerProject.Find(id);
            if (workitem != null)
            {
                //return workitems.WorkCategory.Name;
                return string.IsNullOrEmpty(workitem.ItemName) ? workitem.WorkCategory.Name + "_" + workitem.GenericWorkItemPerProjectId : workitem.ItemName;
            }
            return "";
        }
        public string GetAreaWorkItemName(int id)
        {
            var workitem =
                db.GenericAreaWorkItem.FirstOrDefault(x => x.GenericAreaWorkItemId == id);
            if (workitem != null)
            {
                return workitem.WorkItemName + "[ P.R ]";
            }
            return "";
        }
        public string GetSubContractWorkItemName(int id)
        {
            var workitem =
                db.GenericSubContractWorkItem.FirstOrDefault(x => x.GenericSubContractWorkItemId == id);
            if (workitem != null)
            {
                return workitem.WorkItemName + "[ S.C ]";
            }
            return "";
        }
        public string GetCustomWorkItemName(int id)
        {
            var workItem = db.GenericProjectCustomWorkItem.FirstOrDefault(c => c.GenericProjectCustomWorkItemId == id);
            if (workItem != null)
            {
                return workItem.WorkItemName + " [ C.W ]";
            }
            return "";
        }
        public string GetType(int id)
        {
            var workitems =
                db.WorkItemPerProject.Where(x => x.WorkItemPerProjectId == id).FirstOrDefault();
            if (workitems != null)
            {
                return workitems.WorkCategory.Configuration.CurrentValue;
            }
            return "";
        }
        public int GetIndex(int id)
        {
            var workitems =
                db.WorkItemPerProject.Where(x => x.WorkItemPerProjectId == id).FirstOrDefault();
            if (workitems != null)
            {
                return workitems.WorkCategory.Index;
            }
            return 0;
        }

        public string GetSelectedProjectName()
        {
            var name = "";
            var selectedProject = db.ProjectInformation.FirstOrDefault(p => p.ProjectInformationId == ProjectId);
            if (selectedProject != null)
            {
                name = selectedProject.Name;
            }
            return name;
        }
        public void SetProjectType(int value)
        {
            ProjectType = 0;
            ProjectType = value;
        }
        private int GetBoQSubIndex(int workCategoryId, int configurationId, int projectId)
        {
            int counter = 1;
            var currentWorkCategory = db.WorkCategory.Find(workCategoryId);
            var existingBoQComponents = db.ProjectEstimationBoqBreakdown
                .Where(boq => boq.WorkCategoryId == workCategoryId && boq.ProjectInformationId == projectId && boq.ConfigurationId == configurationId);
            if (existingBoQComponents.Any())
            {
                counter = existingBoQComponents.Max(c => c.Index) + 1;
            }
            return counter;
        }
        private int GetBoQSubIndexForSubWorkItem(ProjectEstimationBoqBreakdown model)
        {
            var counter = 1;
            var existingSubComponents = db.ProjectEstimationBoqDetail.Where(bc => bc.ProjectEstimationBoqBreakdownId == model.ProjectEstimationBoqBreakdownId).ToList();
            if (existingSubComponents.Any())
            {
                counter = existingSubComponents.Max(s => s.Index) + 1;
            }
            return counter;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
