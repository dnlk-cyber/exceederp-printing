using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Settings
{
    public class ConstructionProjectUserAssignmentController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: Construction/ConstructionProjectUserAssignment
        
        public ActionResult Assignments_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ConstructionProjectUserAssignment> assignments = db.ConstructionProjectUserAssignments;

            var listToReturn = new List<ConstructionProjectUserAssignmentViewModel>();
            var provider = new DataProviderController();
            foreach(var assignment in assignments)
            {
                var vm = new ConstructionProjectUserAssignmentViewModel
                {
                    ConstructionProjectUserAssignmentId = assignment.ConstructionProjectUserAssignmentId,
                    ProjectInformationId = assignment.ProjectInformationId,
                    UserName = assignment.UserName,
                    FullName = provider != null ? provider.GetUserFullName(assignment.UserName) : "",

                    CreatedBy = assignment.CreatedBy,
                    DateCreated = assignment.DateCreated,
                    ModifiedBy = assignment.ModifiedBy,
                    LastModified = assignment.LastModified
                };
                listToReturn.Add(vm);
            }
            //DataSourceResult result = assignments.ToDataSourceResult(request, assignment => new
            //{
            //    ConstructionProjectUserAssignmentId = assignment.ConstructionProjectUserAssignmentId,
            //    ProjectInformationId = assignment.ProjectInformationId,
            //    UserName = assignment.UserName,
                
            //    DateCreated = assignment.DateCreated,
            //    LastModified = assignment.LastModified,
            //    CreatedBy = assignment.CreatedBy,
            //    ModifiedBy = assignment.ModifiedBy

            //});

            return Json(listToReturn.AsQueryable().ToDataSourceResult(request));
        }
        private bool AssignmentExists(ConstructionProjectUserAssignmentViewModel record)
        {
            var valueExists = false;
            var temp = db.ConstructionProjectUserAssignments
                .FirstOrDefault(c => c.ProjectInformationId == record.ProjectInformationId && 
                                c.UserName == record.UserName && 
                                c.ConstructionProjectUserAssignmentId != record.ConstructionProjectUserAssignmentId);
            if (temp != null)
            {
                valueExists = true;
            }
            return valueExists;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Assignment_Create([DataSourceRequest]DataSourceRequest request, ConstructionProjectUserAssignmentViewModel assignment)
        {
            if (ModelState.IsValid)
            {
                if (AssignmentExists(assignment))
                {
                    ModelState.AddModelError("", "Assignment  Exists");
                }
                else
                {
                    var entity = new ConstructionProjectUserAssignment
                    {
                        //ConstructionProjectUserAssignmentId = assignment.ConstructionProjectUserAssignmentId,
                        ProjectInformationId = assignment.ProjectInformationId,
                        UserName = assignment.UserName,

                        DateCreated = DateTime.Now,
                        LastModified = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = User.Identity.Name
                    };

                    db.ConstructionProjectUserAssignments.Add(entity);
                    db.SaveChanges();
                    assignment.ConstructionProjectUserAssignmentId = entity.ConstructionProjectUserAssignmentId;
                }

            }

            return Json(new[] { assignment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Assignment_Update([DataSourceRequest]DataSourceRequest request, ConstructionProjectUserAssignmentViewModel assignment)
        {
            if (ModelState.IsValid)
            {
                if (AssignmentExists(assignment))
                {
                    ModelState.AddModelError("", "Assignment  Exists");
                }
                else
                {
                    var entity = new ConstructionProjectUserAssignment
                    {
                        ConstructionProjectUserAssignmentId = assignment.ConstructionProjectUserAssignmentId,
                        ProjectInformationId = assignment.ProjectInformationId,
                        UserName = assignment.UserName,

                        DateCreated = assignment.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = assignment.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };
                    db.ConstructionProjectUserAssignments.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Json(new[] { assignment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Assignment_Destroy([DataSourceRequest]DataSourceRequest request, ConstructionProjectUserAssignmentViewModel assignment)
        {
            if (ModelState.IsValid)
            {
                var entity = new ConstructionProjectUserAssignment
                {
                    ConstructionProjectUserAssignmentId = assignment.ConstructionProjectUserAssignmentId,
                    ProjectInformationId = assignment.ProjectInformationId,
                    UserName = assignment.UserName,

                    DateCreated = assignment.DateCreated,
                    LastModified = assignment.LastModified,
                    CreatedBy = assignment.CreatedBy,
                    ModifiedBy = assignment.ModifiedBy
                };

                db.ConstructionProjectUserAssignments.Attach(entity);
                db.ConstructionProjectUserAssignments.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { assignment }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}