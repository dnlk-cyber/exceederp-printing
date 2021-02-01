using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ExceedERP.Web.Areas.Construction.Controllers.TaskSchedule
{
    public class AssignmentService
    {
        private ExceedDbContext db;

        public AssignmentService(ExceedDbContext context)
        {
            db = context;
        }

        public AssignmentService()
            : this(new ExceedDbContext())
        {
        }

        public virtual IQueryable<ResourceAssignmentViewModel> GetAll()
        {
            return db.ResourceAssignments.ToList().Select(assignment => new ResourceAssignmentViewModel
            {
                ID = assignment.ID,
                TaskID = assignment.TaskID,
                ResourceID = assignment.ResourceID,
                Units = assignment.Units
            }).AsQueryable();
        }

        public virtual void Insert(ResourceAssignmentViewModel assignment)
        {
            var entity = assignment.ToEntity();

            db.ResourceAssignments.Add(entity);
            db.SaveChanges();

            assignment.ID = entity.ID;
        }

        public virtual void Update(ResourceAssignmentViewModel assignment)
        {
            var entity = assignment.ToEntity();

            db.ResourceAssignments.Attach(entity);
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }

        public virtual void Delete(ResourceAssignmentViewModel assignment)
        {
            var entity = assignment.ToEntity();

            db.ResourceAssignments.Attach(entity);
            db.ResourceAssignments.Remove(entity);
            db.SaveChanges();
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}