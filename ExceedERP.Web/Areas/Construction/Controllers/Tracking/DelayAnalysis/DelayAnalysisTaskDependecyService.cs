using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Tracking.DelayAnalysis
{
    public class DelayAnalysisTaskDependecyService
    {
        private ExceedDbContext db;

        public DelayAnalysisTaskDependecyService(ExceedDbContext context)
        {
            db = context;
        }

        public DelayAnalysisTaskDependecyService()
            : this(new ExceedDbContext())
        {
        }

        public virtual IQueryable<DelayTaskDependencyViewModel> GetAll()
        {
            return db.DelayTaskDependencies.ToList().Select(dependency => new DelayTaskDependencyViewModel
            {
                DependencyID = dependency.DelayTaskDependencyId,
                PredecessorID = dependency.PredecessorID,
                SuccessorID = dependency.SuccessorID,
                Type = (DependencyType)dependency.Type,
            }).AsQueryable();
        }
       
        public virtual void Insert(DelayTaskDependencyViewModel dependency, ModelStateDictionary modelState)
        {
            var entity = new DelayTaskDependency();
            
                entity = dependency.ToEntity();
            
            db.DelayTaskDependencies.Add(entity);
            db.SaveChanges();

            dependency.DependencyID = entity.DelayTaskDependencyId;
        }

        
        public virtual void Update(DelayTaskDependencyViewModel dependency, ModelStateDictionary modelState)
        {
            var entity = dependency.ToEntity();
            db.DelayTaskDependencies.Attach(entity);
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }

        public virtual void Delete(DelayTaskDependencyViewModel dependency, ModelStateDictionary modelState)
        {
            var entity = dependency.ToEntity();
            db.DelayTaskDependencies.Attach(entity);
            db.DelayTaskDependencies.Remove(entity);
            db.SaveChanges();
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}