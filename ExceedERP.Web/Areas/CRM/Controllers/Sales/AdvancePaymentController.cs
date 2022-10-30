using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Models;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.AdvancePaymentUser, CRMRoles.AdvancePaymentApprover)]
    public class AdvancePaymentController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["OrganizationCustomer"] = db.OrganizationCustomers.ToList();
            ViewData["PaymentMethod"] = db.PaymentMethods.ToList();
            ViewData["Branches"] = db.Branch.Select(b => new { b.BranchId, b.Name }).ToList();
            return View();
        }

        public decimal GetTotalPayment(int Id)
        {
            decimal total = decimal.Zero;
            var lines = db.AdvancePaymentLines.Where(x => x.AdvancePaymentId == Id).ToList();
            if (lines.Any())
            {
                total = lines.Sum(l => l.Value);
            }

            //foreach(var Line in Lines)
            //{
            //    PayedAmount =     PayedAmount + Line.AdvancePaymentLog.Sum(x => x.Amount);
            //}

            return total;// - PayedAmount;
        }
        public ActionResult AdvancePayments_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<AdvancePayment> AdvancePayments = db.AdvancePayments.ToList();

            var userBranches = new ApplicationBranchManager().GetUsersBranchByName(User.Identity.Name);
            var bIds = new HashSet<int>(userBranches.Select(b => b.BranchId));

            DataSourceResult result = AdvancePayments.Where(s => bIds.Contains(s.BranchId)).Select(s => new AdvancePayment
            {

                AdvancePaymentId = s.AdvancePaymentId,
                CreatedBy = s.CreatedBy,
                Customer = s.Customer,
                GrandTotal = GetTotalPayment(s.AdvancePaymentId),
                BranchId = s.BranchId,

                DateCreated = s.DateCreated,
                LastModified = s.LastModified,
                ModifiedBy = s.ModifiedBy

            }).ToDataSourceResult(request);

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AdvancePayments_Create([DataSourceRequest]DataSourceRequest request, AdvancePayment AdvancePayment)
        {
            List<AdvancePayment> AdvancePaymentList = db.AdvancePayments.Where(ocl => ocl.Customer == AdvancePayment.Customer).ToList();

            if (AdvancePaymentList.Any())
            {
                ModelState.AddModelError("InvalidIndexValue", "Customer Already Exists!");
                return Json(new[] { AdvancePayment }.ToDataSourceResult(request, ModelState));
            }
            else
            {

                if (ModelState.IsValid)
                {
                    var entity = new AdvancePayment
                    {
                        BranchId = AdvancePayment.BranchId,

                        Customer = AdvancePayment.Customer,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now
                    };

                    db.AdvancePayments.Add(entity);
                    db.SaveChanges();
                    AdvancePayment = entity;
                }

            }
            return Json(new[] { AdvancePayment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AdvancePayments_Update([DataSourceRequest]DataSourceRequest request, AdvancePayment AdvancePayment)
        {
            List<AdvancePayment> AdvancePaymentList = db.AdvancePayments.Where(ocl => ocl.Customer == AdvancePayment.Customer && ocl.AdvancePaymentId != AdvancePayment.AdvancePaymentId).ToList();

            if (AdvancePaymentList.Any())
            {
                ModelState.AddModelError("Invalid Index Value", "Customer Already Exists!");
            }
            else
            {

                if (ModelState.IsValid)
                {
                    var entity = new AdvancePayment
                    {
                        BranchId = AdvancePayment.BranchId,

                        Customer = AdvancePayment.Customer,
                        ModifiedBy = User.Identity.Name,
                        LastModified = DateTime.Now
                    };

                    db.AdvancePayments.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }

            }
            return Json(new[] { AdvancePayment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AdvancePayments_Destroy([DataSourceRequest]DataSourceRequest request, AdvancePayment AdvancePayment)
        {
            if (ModelState.IsValid)
            {
                var entity = new AdvancePayment
                {
                    BranchId = AdvancePayment.BranchId,
                    AdvancePaymentId = AdvancePayment.AdvancePaymentId,
                    Customer = AdvancePayment.Customer
                };

                db.AdvancePayments.Attach(entity);
                db.AdvancePayments.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted   Advance Payment number " + AdvancePayment.AdvancePaymentId);

            }

            return Json(new[] { AdvancePayment }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}