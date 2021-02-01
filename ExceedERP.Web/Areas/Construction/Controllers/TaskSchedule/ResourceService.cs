using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace ExceedERP.Web.Areas.Construction.Controllers.TaskSchedule
{
    public class ResourceService
    {
        private ExceedDbContext db;

        public ResourceService(ExceedDbContext context)
        {
            db = context;
        }

        public ResourceService()
            : this(new ExceedDbContext())
        {
        }

        public virtual IQueryable<ResourceViewModel> GetAll()
        {
            return db.Resources.ToList().Select(resource => new ResourceViewModel
            {
                ID = resource.ID,
                ResourceTypeId = resource.ResourceTypeId,
                Name = resource.Name,
                Color = resource.Color,
                UnitofMeasure = resource.UnitofMeasure,
                PrimaryRole = resource.PrimaryRole,
                ResourceType = db.ResourceTypes.Find(resource.ResourceTypeId).Name
            }).AsQueryable();
        }
        public virtual IQueryable<ResourceViewModel> GetTaskResources(int taskId)
        {
            var res = from r in db.Resources
                      join a in db.ResourceAssignments
                      on r.ID equals a.ResourceID 
                      where a.TaskID == taskId
                      select new
                      { r.ID, r.Name, r.PrimaryRole, r.Type, r.ResourceTypeId, r.UnitofMeasure, r.Color};
            
            return res.ToList().Select(resource => new ResourceViewModel {

                ID = resource.ID,
                ResourceTypeId = resource.ResourceTypeId,
                Name = resource.Name,
                Color = resource.Color,
                UnitofMeasure = resource.UnitofMeasure,
                PrimaryRole = resource.PrimaryRole,
                ResourceType = db.ResourceTypes.Find(resource.ResourceTypeId).Name
            }).AsQueryable();
        }
        public virtual void Insert(ResourceViewModel resourceVM)
        {
            var entity = new Resource
            {
                ID = resourceVM.ID,
                ResourceTypeId = resourceVM.ResourceTypeId,
                Name = resourceVM.Name,
                Color = resourceVM.Color,
                UnitofMeasure = resourceVM.UnitofMeasure,
                PrimaryRole = resourceVM.PrimaryRole
            };
            db.Resources.Add(entity);
            db.SaveChanges();

            resourceVM.ID = entity.ID;
            resourceVM.ResourceType = db.ResourceTypes.Find(entity.ResourceTypeId).Name;
        }

        public virtual void Update(ResourceViewModel resourceVM)
        {
            
                var entity = resourceVM.ToEntity();
                db.Resources.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
        }

        public virtual void Delete(ResourceViewModel resourceVM)
        {
            var entity = resourceVM.ToEntity();
            db.Resources.Attach(entity);

            //Delete(entity);

            db.Resources.Remove(entity);
            db.SaveChanges();
        }

        //private void Delete(Task task)
        //{
        //    var childTasks = db.Tasks.Where(t => t.ParentID == task.ID);

        //    foreach (var childTask in childTasks)
        //    {
        //        Delete(childTask);
        //        db.Tasks.Remove(childTask);
        //    }
        //}

        public void Dispose()
        {
            db.Dispose();
        }
    }
}