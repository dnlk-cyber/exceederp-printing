using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.Bank
{
    [AuthorizeRoles(FinanceConstants.BankReconciliationAdmin)]
    public class BankUserAssignmentController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private ApplicationDbContext appdb = new ApplicationDbContext();

        public ActionResult Index()
        {
            PopulateData();
            return View();
        }
        private void PopulateData()
        {
            var acounts = db.BankAccounts.ToList();

            ViewData["accounts"] = acounts
                 .Select(e => new 
                 {
                     Code = e.AccountNumber,
                     Name = e.AccountNumber + "-" + e.AccountName
                 })
                 .OrderBy(e => e.Name);          
          

            var users = appdb.Users.AsQueryable();
            var selectedUser = users.Select(i => new {
                Name = i.UserName + "_" + i.FirstName + " " + i.MiddleName + " " + i.LastName,
                Code = i.Id
            })
                .AsQueryable();

            ViewData["users"] = selectedUser;

        }
        public ActionResult BankUserAssignments_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<BankUserAssignment> BankUserAssignments = db.BankUserAssignments;
            DataSourceResult result = BankUserAssignments.ToDataSourceResult(request, BankUserAssignment => new
            {
                BankUserAssignmentId = BankUserAssignment.BankUserAssignmentId,
                BankAccount = BankUserAssignment.BankAccount,
                User = BankUserAssignment.User
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BankUserAssignments_Create([DataSourceRequest]DataSourceRequest request, BankUserAssignment BankUserAssignment)
        {

            #region validation

            bool valid = true;
            var exists = db.BankUserAssignments.Where(x => x.User == BankUserAssignment.User)
                                                  .Where(x => x.BankAccount == BankUserAssignment.BankAccount).ToList();

            if (exists.Any())
            {
                ModelState.AddModelError("Store Keeper","The selected store keeper is already assigned in store "+BankUserAssignment.User);
                valid = false;
            }

            #endregion
            if (ModelState.IsValid)
            {
                

                if (valid)
                {

                    var entity = new BankUserAssignment
                    {
                        User = BankUserAssignment.User,
                        BankAccount = BankUserAssignment.BankAccount,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now
                    };

                    db.BankUserAssignments.Add(entity);
                    db.SaveChanges();
                    BankUserAssignment.BankUserAssignmentId = entity.BankUserAssignmentId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Bank user assignment", "Success", User.Identity.Name, $"created bank account {entity.BankAccount} and user {entity.User}  ", Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { BankUserAssignment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BankUserAssignments_Update([DataSourceRequest]DataSourceRequest request, BankUserAssignment BankUserAssignment)
        {
            #region validation

            bool valid = true;
            var exists = db.BankUserAssignments.Where(x => x.BankUserAssignmentId != BankUserAssignment.BankUserAssignmentId)
                                    .Where(x => x.User == BankUserAssignment.User)
                                                  .Where(x => x.BankAccount == BankUserAssignment.BankAccount).ToList();

            if (exists.Any())
            {
                ModelState.AddModelError("Store Keeper", "The selected store keeper is already assigned in store " + BankUserAssignment.User);
                valid = false;
            }

            #endregion
            if (ModelState.IsValid)
            {
                if (valid)
                {
                    var entity = new BankUserAssignment
                    {
                        BankUserAssignmentId = BankUserAssignment.BankUserAssignmentId,
                        BankAccount = BankUserAssignment.BankAccount,
                        User = BankUserAssignment.User,
                        CreatedBy =BankUserAssignment.CreatedBy,
                        DateCreated=BankUserAssignment.DateCreated,
                        ModifiedBy = User.Identity.Name,
                        LastModified = DateTime.Now
                    };

                    db.BankUserAssignments.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Bank user assignment", "Success", User.Identity.Name, $"updated bank user account  {entity.BankAccount} and user {entity.User}  ", Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { BankUserAssignment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BankUserAssignments_Destroy([DataSourceRequest]DataSourceRequest request, BankUserAssignment BankUserAssignment)
        {
            if (ModelState.IsValid)
            {
                var entity = new BankUserAssignment
                {
                    BankUserAssignmentId = BankUserAssignment.BankUserAssignmentId,
                    BankAccount = BankUserAssignment.BankAccount
                };

                db.BankUserAssignments.Attach(entity);
                db.BankUserAssignments.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted bank user " + BankUserAssignment.User + " and account" + BankUserAssignment.BankAccount);
                #endregion
            }

            return Json(new[] { BankUserAssignment }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
