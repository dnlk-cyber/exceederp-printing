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
    [AuthorizeRoles(CRMRoles.CRMAdmin)]
    public class SalesPersonAssignmentController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();

      
        public ActionResult Assignments_Read([DataSourceRequest]DataSourceRequest request)
        {
            var unMappedUserFullNames = db.SalesPersonAssignments.Where(u => u.FullName == null).ToList();
            if (unMappedUserFullNames.Any())
            {
                foreach(var asg in unMappedUserFullNames)
                {
                    asg.FullName = GetUserFullName(asg.UserName);

                    db.SalesPersonAssignments.Attach(asg);
                    db.Entry(asg).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            IQueryable<SalesPersonAssignment> assignments = db.SalesPersonAssignments;

            var listToReturn = new List<SalesPersonAssignment>();
            //var provider = new DataProviderController();
            foreach (var assignment in assignments)
            {
                var vm = new SalesPersonAssignment
                {
                    SalesPersonAssignmentId = assignment.SalesPersonAssignmentId,
                    SalesStoreId = assignment.SalesStoreId,
                    UserName = assignment.UserName,
                    //FullName = GetUserFullName(assignment.UserName),
                    FullName = assignment.FullName,
                    PaymentMethodId = assignment.PaymentMethodId,
                    CashOnHandPaymentMethodId = assignment.CashOnHandPaymentMethodId,

                    CreatedBy = assignment.CreatedBy,
                    DateCreated = assignment.DateCreated,
                    ModifiedBy = assignment.ModifiedBy,
                    LastModified = assignment.LastModified
                };
                listToReturn.Add(vm);
            }
            //DataSourceResult result = assignments.ToDataSourceResult(request, assignment => new
            //{
            //    SalesPersonAssignmentId = assignment.SalesPersonAssignmentId,
            //    ProjectInformationId = assignment.ProjectInformationId,
            //    UserName = assignment.UserName,

            //    DateCreated = assignment.DateCreated,
            //    LastModified = assignment.LastModified,
            //    CreatedBy = assignment.CreatedBy,
            //    ModifiedBy = assignment.ModifiedBy

            //});

            return Json(listToReturn.AsQueryable().ToDataSourceResult(request));
        }
        private bool AssignmentExists(SalesPersonAssignment record)
        {
            var valueExists = false;
            var temp = db.SalesPersonAssignments
                .FirstOrDefault(c => c.SalesStoreId == record.SalesStoreId &&
                                c.UserName == record.UserName &&
                                c.SalesPersonAssignmentId != record.SalesPersonAssignmentId);
            if (temp != null)
            {
                valueExists = true;
            }
            return valueExists;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Assignment_Create([DataSourceRequest]DataSourceRequest request, SalesPersonAssignment assignment)
        {
            if (ModelState.IsValid)
            {
                if (AssignmentExists(assignment))
                {
                    ModelState.AddModelError("", "Assignment  Exists");
                }
                else
                {
                    var entity = new SalesPersonAssignment
                    {
                        SalesPersonAssignmentId = assignment.SalesPersonAssignmentId,
                        SalesStoreId = assignment.SalesStoreId,
                        UserName = assignment.UserName,
                        FullName = GetUserFullName(assignment.UserName),
                        PaymentMethodId = assignment.PaymentMethodId,
                        CashOnHandPaymentMethodId = assignment.CashOnHandPaymentMethodId,

                        DateCreated = DateTime.Now,
                        LastModified = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = User.Identity.Name
                    };

                    db.SalesPersonAssignments.Add(entity);
                    db.SaveChanges();
                    assignment.SalesPersonAssignmentId = entity.SalesPersonAssignmentId;
                }

            }

            return Json(new[] { assignment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Assignment_Update([DataSourceRequest]DataSourceRequest request, SalesPersonAssignment assignment)
        {
            if (ModelState.IsValid)
            {
                if (AssignmentExists(assignment))
                {
                    ModelState.AddModelError("", "Assignment  Exists");
                }
                else
                {
                    var entity = new SalesPersonAssignment
                    {
                        SalesPersonAssignmentId = assignment.SalesPersonAssignmentId,
                        SalesStoreId = assignment.SalesStoreId,
                        UserName = assignment.UserName,
                        FullName = GetUserFullName(assignment.UserName),
                        PaymentMethodId = assignment.PaymentMethodId,
                        CashOnHandPaymentMethodId = assignment.CashOnHandPaymentMethodId,

                        DateCreated = assignment.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = assignment.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };
                    db.SalesPersonAssignments.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Json(new[] { assignment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Assignment_Destroy([DataSourceRequest]DataSourceRequest request, SalesPersonAssignment assignment)
        {
            if (ModelState.IsValid)
            {
                var entity = new SalesPersonAssignment
                {
                    SalesPersonAssignmentId = assignment.SalesPersonAssignmentId,
                    SalesStoreId = assignment.SalesStoreId,
                    UserName = assignment.UserName,
                    FullName = assignment.FullName,
                    PaymentMethodId = assignment.PaymentMethodId,
                    CashOnHandPaymentMethodId = assignment.CashOnHandPaymentMethodId,

                    DateCreated = assignment.DateCreated,
                    LastModified = assignment.LastModified,
                    CreatedBy = assignment.CreatedBy,
                    ModifiedBy = assignment.ModifiedBy
                };

                db.SalesPersonAssignments.Attach(entity);
                db.SalesPersonAssignments.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { assignment }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        private string GetUserFullName(string userName)
        {
            var identityDb = new ApplicationDbContext();
            string name = userName;
            var currentUser = identityDb.Users.FirstOrDefault(u => u.UserName == userName);
            if(currentUser !=  null)
            {
                name = $"{currentUser.FirstName} {currentUser.MiddleName} {currentUser.LastName}";
            }
            return name;
        }
    }
}