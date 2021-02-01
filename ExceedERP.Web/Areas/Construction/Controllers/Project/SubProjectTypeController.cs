using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
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
    public class SubProjectTypeController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult SubProjectTypes_Read([DataSourceRequest]DataSourceRequest request, int idd)
        {
            IQueryable<SubProjectType> subProjectTypes = db.SubProjectType.Where( x => !x.IsDeleted && x.ProjectInformationId == idd);
            DataSourceResult result = subProjectTypes.ToDataSourceResult(request, subProject => new
            {
                SubProjectTypeId = subProject.SubProjectTypeId,
                Name = subProject.Name,
                NumberOfBlocks = subProject.NumberOfBlocks,
                ProjectInformationId = subProject.ProjectInformationId,
                Remark = subProject.Remark,
                OrderIndex = subProject.OrderIndex,
            });

            return Json(result);
        }
        public ActionResult SubProjectTypes_Read_Deleted([DataSourceRequest]DataSourceRequest request, int idd)
        {
            IQueryable<SubProjectType> subProjectTypes = db.SubProjectType.Where(x => x.IsDeleted && x.ProjectInformationId == idd);
            DataSourceResult result = subProjectTypes.ToDataSourceResult(request, subProject => new
            {
                SubProjectTypeId = subProject.SubProjectTypeId,
                Name = subProject.Name,
                NumberOfBlocks = subProject.NumberOfBlocks,
                ProjectInformationId = subProject.ProjectInformationId,
                Remark = subProject.Remark,
                OrderIndex = subProject.OrderIndex,
                IsDeleted = subProject.IsDeleted,
                DeletedBy = subProject.DeletedBy,
                DeletedOn = subProject.DeletedOn
            });

            return Json(result);
        }
        private bool SubProjectExists(string name, int projectId)
        {
            var Block = db.SubProjectType.Where(b => b.Name == name && b.ProjectInformationId == projectId).FirstOrDefault();
            if (Block != null)
            {
                return true;
            }
            return false;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SubProjectType_Create([DataSourceRequest]DataSourceRequest request, SubProjectType subProject, int iddTwo)
        {
            if (ModelState.IsValid)
            {
                if (SubProjectExists(subProject.Name, iddTwo))
                {
                    ModelState.AddModelError("", "Duplicate names are not allowed!!");
                }
                else
                {
                    var entity = new SubProjectType
                    {
                        SubProjectTypeId = subProject.SubProjectTypeId,
                        Name = subProject.Name,
                        NumberOfBlocks = subProject.NumberOfBlocks,
                        ProjectInformationId = iddTwo,
                        OrderIndex = this.GenerateNewOrderIndexOnEntityAdded(iddTwo),
                        Remark = subProject.Remark
                    };

                    db.SubProjectType.Add(entity);
                    db.SaveChanges();
                    subProject.SubProjectTypeId = entity.SubProjectTypeId;

                    this.CreateBlocksUnderSubProjectType(entity);

                }
            }

            return Json(new[] { subProject }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SubProjectType_Update([DataSourceRequest]DataSourceRequest request, SubProjectType subProject)
        {
            if (ModelState.IsValid)
            {
                var entity = new SubProjectType
                {
                    SubProjectTypeId = subProject.SubProjectTypeId,
                    Name = subProject.Name,
                    NumberOfBlocks = subProject.NumberOfBlocks,
                    ProjectInformationId = subProject.ProjectInformationId,
                    OrderIndex = subProject.OrderIndex,
                    Remark = subProject.Remark
                };

                db.SubProjectType.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { subProject }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SubProjectType_Destroy([DataSourceRequest]DataSourceRequest request, SubProjectType subProject)
        {
            if (ModelState.IsValid)
            {
                var entity = new SubProjectType
                {
                    SubProjectTypeId = subProject.SubProjectTypeId,
                    Name = subProject.Name,
                    NumberOfBlocks = subProject.NumberOfBlocks,
                    ProjectInformationId = subProject.ProjectInformationId,
                    OrderIndex = subProject.OrderIndex,
                    Remark = subProject.Remark,

                    IsDeleted = true,
                    DeletedOn = DateTime.Now,
                    DeletedBy = User.Identity.Name
                };

                db.SubProjectType.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                //this.RemoveBlocksUderSubProjectType(entity.SubProjectTypeId);
                //this.UpdateOrderingOnEntityDeleted(entity.ProjectInformationId);
            }

            return Json(new[] { subProject }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult SubProjectType_Destroy_Permanently([DataSourceRequest]DataSourceRequest request, SubProjectType subProject)
        {
            if (ModelState.IsValid)
            {
                var entity = new SubProjectType
                {
                    SubProjectTypeId = subProject.SubProjectTypeId,
                    Name = subProject.Name,
                    NumberOfBlocks = subProject.NumberOfBlocks,
                    ProjectInformationId = subProject.ProjectInformationId,
                    OrderIndex = subProject.OrderIndex,
                    Remark = subProject.Remark
                };

                db.SubProjectType.Attach(entity);
                db.SubProjectType.Remove(entity);
                db.SaveChanges();

                this.RemoveBlocksUderSubProjectType(entity.SubProjectTypeId);
                this.UpdateOrderingOnEntityDeleted(entity.ProjectInformationId);
            }

            return Json(new[] { subProject }.ToDataSourceResult(request, ModelState));
        }
        public void UpdateSubProjectTypeOrder(int id, int oldOrderIndex,int newOrderIndex)
        {
            var currentModel = db.SubProjectType.Find(id);
            if(currentModel != null)
            {
                // exchange order indexes
                var affectedModel =
                    db.SubProjectType.FirstOrDefault(sp => sp.ProjectInformationId == currentModel.ProjectInformationId && sp.OrderIndex == newOrderIndex);
                if (affectedModel != null)
                {
                    affectedModel.OrderIndex = oldOrderIndex;

                    db.SubProjectType.Attach(affectedModel);
                    db.Entry(affectedModel).State = EntityState.Modified;
                    db.SaveChanges();
                }

                currentModel.OrderIndex = newOrderIndex;

                db.SubProjectType.Attach(currentModel);
                db.Entry(currentModel).State = EntityState.Modified;
                db.SaveChanges();
               
            }
        }
        private int GenerateNewOrderIndexOnEntityAdded(int projectId) {
            var index = 1;
            var currentProject = db.ProjectInformation.Find(projectId);
            if(currentProject != null)
            {
                var subProjects = db.SubProjectType.Where(sb => sb.ProjectInformationId == projectId).ToList();
                if (subProjects.Any())
                {
                    index = subProjects.Count + 1;
                }
            }
            return index;
        }
        private void UpdateOrderingOnEntityDeleted(int projectId)
        {
            var subs = db.SubProjectType.Where(sp => sp.ProjectInformationId == projectId).OrderBy(sp => sp.OrderIndex).ToList();
            if (subs.Any())
            {
                var subsArray = subs.ToArray();
                for (int i = 1; i <= subsArray.Length; i++)
                {
                    var sb = subsArray[i - 1];
                    sb.OrderIndex = i;

                    db.SubProjectType.Attach(sb);
                    db.Entry(sb).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
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
            if (temp != null)
            {
                project = temp;
            }
            return project;
        }
        //public ActionResult GetProjectBlocks(int projectId)
        //{

        //    db = new ExceedDbContext();
        //    var blocks = db.Blocks.Where(x => x.ProjectInformationId == projectId).OrderBy(x => x.Name);


        //    return Json(blocks.Select(x => new { Name = x.Name, BlockId = x.BlockId, ProjectInformationId = x.ProjectInformationId }), JsonRequestBehavior.AllowGet);
        //}
        private void CreateBlocksUnderSubProjectType(SubProjectType model)
        {
            var currentProject = db.ProjectInformation.Find(model.ProjectInformationId);
            if (currentProject != null && model.NumberOfBlocks > 0)
            {
                if (currentProject.ProjectType == AvailableProjectTypes.Building)
                {
                    for (int n = 1; n <= model.NumberOfBlocks; n++)
                    {
                        var blockToAdd = new Block
                        {
                            Name = "B-" + n,
                        };
                        db.Blocks.Add(blockToAdd);
                        db.SaveChanges();
                    }
                }
                else if (currentProject.ProjectType == AvailableProjectTypes.Road)
                {
                    for (int n = 1; n <= model.NumberOfBlocks; n++)
                    {
                        var blockToAdd = new Block
                        {
                            Name = "R-" + n,
                        };
                        db.Blocks.Add(blockToAdd);
                        db.SaveChanges();
                    }
                }
            }
        }
        private void RemoveBlocksUderSubProjectType(int id)
        {
            var blocks = db.Blocks.Where(b => b.SubProjectTypeId == id).ToList();
            if (blocks.Any())
            {
                db.Blocks.RemoveRange(blocks);
                db.SaveChanges();
            }
            var genericWorkItems = db.GenericWorkItemPerProject.Where(w => w.SubProjectTypeId == id).ToList();
            var genericAreaWorkItems = db.GenericAreaWorkItem.Where(w => w.SubProjectTypeId == id).ToList();
            var genericSubContractWorkItems = db.GenericSubContractWorkItem.Where(w => w.SubProjectTypeId == id).ToList();
            var genericCustomWorkItem = db.GenericProjectCustomWorkItem.Where(w => w.SubProjectTypeId == id).ToList();
            if (genericWorkItems.Any())
            {
                var ids = new HashSet<int>(genericWorkItems.Select(w => w.GenericWorkItemPerProjectId));
                db.GenericWorkItemPerProject.RemoveRange(genericWorkItems);
                db.SaveChanges();

                var workItems = db.WorkItemPerProject.Where(w => ids.Contains(w.GenericWorkItemPerProjectId)).ToList();
                if (workItems.Any())
                {
                    var wItemIds = new HashSet<int>(workItems.Select(w => w.WorkItemPerProjectId));
                    var nullableIds = new HashSet<int?>();
                    foreach(var i in wItemIds)
                    {
                        nullableIds.Add(i);
                    }
                    var durationItems = db.WorkItemDuration.Where(wd => wItemIds.Contains(wd.WorkItemPerProjectId)).ToList();
                    if (durationItems.Any())
                    {
                        db.WorkItemDuration.RemoveRange(durationItems);
                        db.SaveChanges();
                    }
                    //var dirtyTaskData = db.Tasks.Where(t => nullableIds.Contains(t.WorkItemPerProjectId)).ToList();
                    //if (dirtyTaskData.Any())
                    //{
                    //    db.Tasks.RemoveRange(dirtyTaskData);
                    //    db.SaveChanges();
                    //}
                    db.WorkItemPerProject.RemoveRange(workItems);
                    db.SaveChanges();
                }
            }
            if (genericSubContractWorkItems.Any())
            {
                var ids = new HashSet<int>(genericSubContractWorkItems.Select(w => w.GenericSubContractWorkItemId));
                db.GenericSubContractWorkItem.RemoveRange(genericSubContractWorkItems);
                db.SaveChanges();

                var workItems = db.SubContractWorkItem.Where(w => ids.Contains(w.GenericSubContractWorkItemId)).ToList();
                if (workItems.Any())
                {
                    db.SubContractWorkItem.RemoveRange(workItems);
                    db.SaveChanges();
                }
            }
            if (genericAreaWorkItems.Any())
            {
                var ids = new HashSet<int>(genericAreaWorkItems.Select(w => w.GenericAreaWorkItemId));
                db.GenericAreaWorkItem.RemoveRange(genericAreaWorkItems);
                db.SaveChanges();

                var workItems = db.AreaWorkItemes.Where(w => ids.Contains(w.GenericAreaWorkItemId)).ToList();
                if (workItems.Any())
                {
                    db.AreaWorkItemes.RemoveRange(workItems);
                    db.SaveChanges();
                }
            }
            if (genericCustomWorkItem.Any())
            {
                var ids = new HashSet<int>(genericCustomWorkItem.Select(w => w.GenericProjectCustomWorkItemId));
                db.GenericProjectCustomWorkItem.RemoveRange(genericCustomWorkItem);
                db.SaveChanges();

                var workItems = db.ProjectCustomWorkItems.Where(w => ids.Contains(w.GenericProjectCustomWorkItemId)).ToList();
                if (workItems.Any())
                {
                    db.ProjectCustomWorkItems.RemoveRange(workItems);
                    db.SaveChanges();
                }
            }
        }
    }
}