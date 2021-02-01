using ExceedERP.Core.Domain.Common;

using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Project
{
    [AuthorizeRoles(ConstructionConstants.EstimationUser)]
    public class TakeOffPerProjectController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var lookups = db.Lookups.ToList();

            ViewData["Categories"] = db.WorkCategory.Select(c => new { c.Name, c.ConfigurationId, c.WorkCategoryId}).ToList();
            var units = lookups.Where(x => x.LookupType == LookUpTypes.UOM).ToList();
            ViewData["Unit"] = units;

            //var unitWegight = db.UnitRate.ToList();
            //ViewData["Diameters"] = unitWegight;

            var deformedBars = db.DeformedBars.ToList();
            ViewData["bars"] = deformedBars;

            //var workItems = db.WorkItemPerProject.Select(w => new { w.WorkItemPerProjectId, w.ItemName}).ToList();
            //ViewData["WorkItems"] = workItems;
            //var projectTypes = db.ProjectInformation.ToList();
            //ViewData["ProjectList"] = projectTypes;
            var uom = db.UnitOfMeasurement.ToList();
            ViewData["uom"] = uom.Select(
                        x =>
                            new
                            {
                                UnitOfMeasurementId = x.UnitOfMeasurementId,
                                Abbreviation = x.Abbreviation
                            }).ToList(); ;
            //
            //this.CleanTakeOffSheet();
            //
            return View();
        }
        public ActionResult GetProject()
        {

            db = new ExceedDbContext();
            var projects = db.ProjectInformation
                        .Select(c => new ProjectInformation
                        {
                            ProjectInformationId = c.ProjectInformationId,
                            Name = c.Name
                        })
                        .OrderBy(e => e.Name);

            return Json(projects, JsonRequestBehavior.AllowGet);
        }
        public ActionResult WorkItemPerProject_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<WorkItemPerProject> workitemperproject = db.WorkItemPerProject;
            DataSourceResult result = workitemperproject.ToDataSourceResult(request, workItemPerProject => new
            {
                WorkItemPerProjectId = workItemPerProject.WorkItemPerProjectId,
                IsSubCategory = workItemPerProject.IsSubCategory,
                ItemCode = workItemPerProject.ItemCode,
                ProjectEstimationWorkItemId = workItemPerProject.ProjectEstimationWorkItemId,
                ItemName = string.IsNullOrEmpty(workItemPerProject.ItemName) ? this.GetCategory(workItemPerProject.WorkCategoryId).Name + "_" + workItemPerProject.WorkItemPerProjectId : workItemPerProject.ItemName,
                TotalQuantityOfWork = workItemPerProject.TotalQuantityOfWork,
                DirectCost = workItemPerProject.DirectCost,
                MaterialCost = workItemPerProject.MaterialCost,
                LabourOutPut = workItemPerProject.LabourOutPut,
                EquipmentOutput = workItemPerProject.EquipmentOutput,
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

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult TakeOffPerProject_Read([DataSourceRequest]DataSourceRequest request, int idta)
        {
            IQueryable<TakeOffPerProject> takeoffperprojectList = db.TakeOffPerProject.Where(x => x.WorkItemPerProjectId == idta);
            DataSourceResult result = takeoffperprojectList.ToDataSourceResult(request, takeOffPerProject => new
            {
                TakeOffPerProjectId = takeOffPerProject.TakeOffPerProjectId,
                TakeOffNo = takeOffPerProject.TakeOffNo,
                IsReinforcementbarTakeOff = takeOffPerProject.IsReinforcementbarTakeOff,
                WorkItemPerProjectId = takeOffPerProject.WorkItemPerProjectId,
                ProjectInformationId = takeOffPerProject.ProjectInformationId,
                Remark = takeOffPerProject.Remark,
                OrgBranchName = takeOffPerProject.OrgBranchName,
                DateCreated = takeOffPerProject.DateCreated,
                LastModified = takeOffPerProject.LastModified,
                CreatedBy = takeOffPerProject.CreatedBy,
                ModifiedBy = takeOffPerProject.ModifiedBy
            });

            return Json(result);
        }
        private bool WorkItemHasTakeOffSheet(int workItemId)
        {
            bool hasTakeOffSheet = false;
            var takeOff = db.TakeOffPerProject.Where(t => t.WorkItemPerProjectId == workItemId).ToList();
            if (takeOff.Any())
            {
                hasTakeOffSheet = true;
            }
            return hasTakeOffSheet;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TakeOffPerProject_Create([DataSourceRequest]DataSourceRequest request, TakeOffPerProject takeOffPerProject, int idtaTwo)
        {
            if (ModelState.IsValid)
            {
                if (WorkItemHasTakeOffSheet(idtaTwo))
                {
                    ModelState.AddModelError("", "Take off sheet exists for this work item");
                }
                else
                {
                    var entity = new TakeOffPerProject
                    {
                        //TakeOffNo = this.CreateUniqueTakeOffNumber(idtaTwo),
                        WorkItemPerProjectId = idtaTwo,
                        ProjectInformationId = this.GetProjectInformationId(idtaTwo),
                        IsReinforcementbarTakeOff = takeOffPerProject.IsReinforcementbarTakeOff,
                        Remark = takeOffPerProject.Remark,
                        OrgBranchName = takeOffPerProject.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = takeOffPerProject.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = takeOffPerProject.ModifiedBy
                    };

                    try
                    {
                        db.TakeOffPerProject.Add(entity);
                        db.SaveChanges();
                        takeOffPerProject.TakeOffPerProjectId = entity.TakeOffPerProjectId;
                        this.UpdateTakeOffNumber(entity);
                    }
                    catch (DbEntityValidationException e)
                    {
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                eve.Entry.Entity.GetType().Name, eve.Entry.State);
                            foreach (var ve in eve.ValidationErrors)
                            {
                                Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                    ve.PropertyName, ve.ErrorMessage);
                            }
                        }
                        throw;
                    }
                    
                }

            }

            return Json(new[] { takeOffPerProject }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TakeOffPerProject_Update([DataSourceRequest]DataSourceRequest request, TakeOffPerProject takeOffPerProject)
        {
            if (ModelState.IsValid)
            {
                var entity = new TakeOffPerProject
                {
                    TakeOffPerProjectId = takeOffPerProject.TakeOffPerProjectId,
                    //TakeOffNo = this.CreateUniqueTakeOffNumber(takeOffPerProject.WorkItemPerProjectId),
                    WorkItemPerProjectId = takeOffPerProject.WorkItemPerProjectId,
                    ProjectInformationId = takeOffPerProject.ProjectInformationId,
                    IsReinforcementbarTakeOff = takeOffPerProject.IsReinforcementbarTakeOff,
                    Remark = takeOffPerProject.Remark,
                    OrgBranchName = takeOffPerProject.OrgBranchName,
                    DateCreated = takeOffPerProject.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = takeOffPerProject.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                var local = db.Set<TakeOffPerProject>()
                         .Local
                         .FirstOrDefault(f => f.TakeOffPerProjectId == entity.TakeOffPerProjectId);
                if (local != null)
                {
                    db.Entry(local).State = EntityState.Detached;
                }
                db.Entry(entity).State = EntityState.Modified;

                //db.TakeOffPerProject.Attach(entity);
                //db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { takeOffPerProject }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TakeOffPerProject_Destroy([DataSourceRequest]DataSourceRequest request, TakeOffPerProject takeOffPerProject)
        {
            if (ModelState.IsValid)
            {
                var entity = new TakeOffPerProject
                {
                    TakeOffPerProjectId = takeOffPerProject.TakeOffPerProjectId,
                    TakeOffNo = takeOffPerProject.TakeOffNo,
                    WorkItemPerProjectId = takeOffPerProject.WorkItemPerProjectId,
                    ProjectInformationId = takeOffPerProject.ProjectInformationId,
                    IsReinforcementbarTakeOff = takeOffPerProject.IsReinforcementbarTakeOff,
                    Remark = takeOffPerProject.Remark,
                    OrgBranchName = takeOffPerProject.OrgBranchName,
                    DateCreated = takeOffPerProject.DateCreated,
                    LastModified = takeOffPerProject.LastModified,
                    CreatedBy = takeOffPerProject.CreatedBy,
                    ModifiedBy = takeOffPerProject.ModifiedBy
                };

                db.TakeOffPerProject.Attach(entity);
                db.TakeOffPerProject.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { takeOffPerProject }.ToDataSourceResult(request, ModelState));
        }
        public static int ProjectId { get; set; }

        public void SetProjectId(int value)
        {
            ProjectId = 0;
            ProjectId = value;
            // Category = category;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        private ProjectEstimationWorkItem GetWorkItemDetail(int id)
        {
            ProjectEstimationWorkItem item = new ProjectEstimationWorkItem();
            var wi = db.ProjectEstimationWorkItem.Find(id);
            if (wi != null)
            {
                item = wi;
            }
            return item;
        }
        private WorkCategory GetCategory(int id)
        {
            WorkCategory cat = new WorkCategory();
            var category = db.WorkCategory.Find(id);
            if (category != null)
            {
                cat = category;
            }
            return cat;
        }
        private int GetProjectInformationId(int workItemId)
        {
            var currentWorkItem = db.WorkItemPerProject.Find(workItemId);
            var project = new ProjectInformation();
            if (currentWorkItem != null)
            {
                var temp = db.ProjectInformation.Find(currentWorkItem.ProjectInformationId);
                if (temp != null)
                {
                    project = temp;
                }
            }
            return project.ProjectInformationId;
        }

        // 
        //  Summary:
        //      check if there are take off sheets that have no matching work items and 
        //      remove them (if any)
        //private void CleanTakeOffSheet() {
        //    var allWorkItems = db.WorkItemPerProject.ToList();
        //    var workItemIds = new HashSet<int>(allWorkItems.Select(w => w.WorkItemPerProjectId));
        //    var dirtyData = db.TakeOffPerProject.Where(takeOff => (!(workItemIds.Contains(takeOff.WorkItemPerProjectId)))).ToList();
        //    if (dirtyData.Any())
        //    {
        //        var ids = new HashSet<int>(dirtyData.Select(d => d.TakeOffPerProjectId));
        //        var cascadedWorkItemDurationItems = db.WorkItemDuration.Where(w => (ids.Contains(w.TakeOffPerProjectId))).ToList();
        //        if (cascadedWorkItemDurationItems.Any())
        //        {
        //            var cascadeIds = new HashSet<int>(cascadedWorkItemDurationItems.Select(wid => wid.WorkItemDurationId));
        //            var cascadedScheduleWorkItems = db.Tasks.Where(tsk => (cascadeIds.Contains(tsk.WorkItemDurationId ?? 0))).ToList();
        //            if (cascadedScheduleWorkItems.Any())
        //            {
        //                db.Tasks.RemoveRange(cascadedScheduleWorkItems);
        //                db.SaveChanges();
        //            }

        //            db.WorkItemDuration.RemoveRange(cascadedWorkItemDurationItems);
        //            db.SaveChanges();
        //        }

        //        db.TakeOffPerProject.RemoveRange(dirtyData);
        //        db.SaveChanges();
        //    }
        //}
        private string CreateUniqueTakeOffNumber(int workItemId)
        {
            var takeOffCounter = 0;
            var concatnatedString = "takeoff ";
            var currentWorkItem = db.WorkItemPerProject.Find(workItemId);
            if (currentWorkItem != null)
            {
                var takeOffList = db.TakeOffPerProject.Where(t => t.ProjectInformationId == currentWorkItem.ProjectInformationId).ToList();
                if (!takeOffList.Any())
                {
                    takeOffCounter = 1;
                }
                else
                {
                    var count = takeOffList.Count();
                    takeOffCounter = count + 1;
                }
                var strType = currentWorkItem.WorkCategory.Configuration.CurrentValue.ToLower().Replace('_', ' ');
                concatnatedString = concatnatedString + "00" + takeOffCounter + " [" + currentWorkItem.ItemName + "(" + strType + ")] ";

                if (currentWorkItem.BlockId != null)
                {
                    var block = db.Blocks.Find(currentWorkItem.BlockId);
                    if (block != null)
                    {
                        concatnatedString = concatnatedString + "[" + block.Name + "] ";
                        var project = db.ProjectInformation.Find(block.ProjectInformationId);
                        if (project != null)
                        {
                            concatnatedString = concatnatedString + " [" + project.Name + "]";
                        }
                    }
                }
            }
            return concatnatedString;
        }

        private void UpdateTakeOffNumber(TakeOffPerProject takeOff)
        {
            takeOff.TakeOffNo = takeOff.TakeOffPerProjectId.ToString("D12");

            db.TakeOffPerProject.Attach(takeOff);
            db.Entry(takeOff).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}
