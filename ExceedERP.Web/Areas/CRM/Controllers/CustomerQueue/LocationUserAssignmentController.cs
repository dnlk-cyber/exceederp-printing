using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.CRMQueueSetting)]
    public class LocationUserAssignmentController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();


        public ActionResult Assignments_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<LocationUserAssignment> assignments = db.LocationUserAssignment.Where(s => s.LocationMapId ==id);

            var listToReturn = new List<LocationUserAssignment>();
            foreach (var assignment in assignments)
            {
                var vm = new LocationUserAssignment
                {
                    LocationUserAssignmentId = assignment.LocationUserAssignmentId,
                    LocationMapId = assignment.LocationMapId,
                    UserName = assignment.UserName,
                    FullName = GetUserFullName(assignment.UserName),
                    
                    CreatedBy = assignment.CreatedBy,
                    DateCreated = assignment.DateCreated,
                    ModifiedBy = assignment.ModifiedBy,
                    LastModified = assignment.LastModified
                };
                listToReturn.Add(vm);
            }
            //DataSourceResult result = assignments.ToDataSourceResult(request, assignment => new
            //{
            //    LocationUserAssignmentId = assignment.LocationUserAssignmentId,
            //    ProjectInformationId = assignment.ProjectInformationId,
            //    UserName = assignment.UserName,

            //    DateCreated = assignment.DateCreated,
            //    LastModified = assignment.LastModified,
            //    CreatedBy = assignment.CreatedBy,
            //    ModifiedBy = assignment.ModifiedBy

            //});

            return Json(listToReturn.AsQueryable().ToDataSourceResult(request));
        }
        private bool AssignmentExists(LocationUserAssignment record)
        {
            var valueExists = false;
            var temp = db.LocationUserAssignment
                .FirstOrDefault(c => c.LocationMapId == record.LocationMapId &&
                                c.UserName == record.UserName &&
                                c.LocationUserAssignmentId != record.LocationUserAssignmentId);
            if (temp != null)
            {
                valueExists = true;
            }
            return valueExists;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Assignment_Create([DataSourceRequest]DataSourceRequest request, LocationUserAssignment assignment, int id)
        {
            if (ModelState.IsValid)
            {
                if (AssignmentExists(assignment))
                {
                    ModelState.AddModelError("", "Assignment  Exists");
                }
                else
                {
                    var entity = new LocationUserAssignment
                    {
                        LocationUserAssignmentId = assignment.LocationUserAssignmentId,
                        LocationMapId = id,
                        UserName = assignment.UserName,

                        DateCreated = DateTime.Now,
                        LastModified = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = User.Identity.Name
                    };

                    db.LocationUserAssignment.Add(entity);
                    db.SaveChanges();
                    assignment.LocationUserAssignmentId = entity.LocationUserAssignmentId;
                }

            }

            return Json(new[] { assignment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Assignment_Update([DataSourceRequest]DataSourceRequest request, LocationUserAssignment assignment)
        {
            if (ModelState.IsValid)
            {
                if (AssignmentExists(assignment))
                {
                    ModelState.AddModelError("", "Assignment  Exists");
                }
                else
                {
                    var entity = new LocationUserAssignment
                    {
                        LocationUserAssignmentId = assignment.LocationUserAssignmentId,
                        LocationMapId = assignment.LocationMapId,
                        UserName = assignment.UserName,
                        FullName = assignment.FullName,

                        DateCreated = assignment.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = assignment.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };
                    db.LocationUserAssignment.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Json(new[] { assignment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Assignment_Destroy([DataSourceRequest]DataSourceRequest request, LocationUserAssignment assignment)
        {
            if (ModelState.IsValid)
            {
                var entity = new LocationUserAssignment
                {
                    LocationUserAssignmentId = assignment.LocationUserAssignmentId,
                    LocationMapId = assignment.LocationMapId,
                    UserName = assignment.UserName,
                    FullName = assignment.FullName,

                    DateCreated = assignment.DateCreated,
                    LastModified = assignment.LastModified,
                    CreatedBy = assignment.CreatedBy,
                    ModifiedBy = assignment.ModifiedBy
                };

                db.LocationUserAssignment.Attach(entity);
                db.LocationUserAssignment.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { assignment }.ToDataSourceResult(request, ModelState));
        }
        private string GetUserFullName(string userName)
        {
            var identityDb = new ApplicationDbContext();
            string name = userName;
            var currentUser = identityDb.Users.FirstOrDefault(u => u.UserName == userName);
            if (currentUser != null)
            {
                name = $"{currentUser.FirstName} {currentUser.MiddleName} {currentUser.LastName}";
            }
            return name;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}