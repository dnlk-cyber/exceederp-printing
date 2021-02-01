using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Supplier;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AP
{
    public class ApProcurementSupplierController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
       

       [AuthorizeRoles(FinanceConstants.AccountPayableView, FinanceConstants.AccountPayableApprover,FinanceConstants.AccountPayablePayment, FinanceConstants.AccountPayableInvoice)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableView, FinanceConstants.AccountPayableApprover, FinanceConstants.AccountPayablePayment, FinanceConstants.AccountPayableInvoice)]
        public ActionResult ProcurementSuppliers_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProcurementSupplier> procurementsuppliers = _db.ProcurementSuppliers;
            DataSourceResult result = procurementsuppliers.ToDataSourceResult(request, procurementSupplier => new
            {
                ProcurementSupplierID = procurementSupplier.ProcurementSupplierID,
                Category = procurementSupplier.Category,
                Name = procurementSupplier.Name,
                SupplierId = procurementSupplier.SupplierId,
                AccountCode = procurementSupplier.AccountCode,
                TradeLicense = procurementSupplier.TradeLicense,
                TIN = procurementSupplier.TIN,
                Address = procurementSupplier.Address,
                Telephone = procurementSupplier.Telephone,
                Fax = procurementSupplier.Fax,
                Email = procurementSupplier.Email,
                IsCredit = procurementSupplier.IsCredit,
                IsActive = procurementSupplier.IsActive,
                IsBlackListed = procurementSupplier.IsBlackListed,
                IsForShortList = procurementSupplier.IsForShortList,
                MinuteNo = procurementSupplier.MinuteNo,
                Rank = procurementSupplier.Rank,
                Remark = procurementSupplier.Remark,
                OrgBranchName = procurementSupplier.OrgBranchName,
                DateCreated = procurementSupplier.DateCreated,
                LastModified = procurementSupplier.LastModified,
                CreatedBy = procurementSupplier.CreatedBy,
                ModifiedBy = procurementSupplier.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FinanceConstants.AccountPayablePayment, FinanceConstants.AccountPayableInvoice)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProcurementSuppliers_Update([DataSourceRequest]DataSourceRequest request, ProcurementSupplier procurementSupplier)
        {
            #region validation
            var exists = _db.ProcurementSuppliers.Where(x => x.ProcurementSupplierID != procurementSupplier.ProcurementSupplierID)
                .Where(x => x.Category.ToLower() == procurementSupplier.Category.ToLower())
                .Where(x => x.Name.ToLower() == procurementSupplier.Name.ToLower()).ToList();

            if (exists.Any())
            {
                ModelState.AddModelError("Name", procurementSupplier.Name + " already exists in the system.");
            }

            var supplierid = _db.ProcurementSuppliers.Where(x => x.ProcurementSupplierID != procurementSupplier.ProcurementSupplierID)
                .Where(x => x.Category.ToLower() == procurementSupplier.Category.ToLower())
                .Where(x => x.SupplierId.ToLower() == procurementSupplier.SupplierId.ToLower()).ToList();

            if (supplierid.Any())
            {
                ModelState.AddModelError("ID", procurementSupplier.SupplierId + " already exists in the system.");
            }


            #endregion
            if (ModelState.IsValid)
            {
                var entity = new ProcurementSupplier
                {
                    ProcurementSupplierID = procurementSupplier.ProcurementSupplierID,
                    Category = procurementSupplier.Category,
                    Name = procurementSupplier.Name,
                    SupplierId = procurementSupplier.SupplierId,
                    AccountCode = procurementSupplier.AccountCode,
                    TradeLicense = procurementSupplier.TradeLicense,
                    TIN = procurementSupplier.TIN,
                    Address = procurementSupplier.Address,
                    Telephone = procurementSupplier.Telephone,
                    Fax = procurementSupplier.Fax,
                    Email = procurementSupplier.Email,
                    IsCredit = procurementSupplier.IsCredit,
                    IsActive = procurementSupplier.IsActive,
                    IsBlackListed = procurementSupplier.IsBlackListed,
                    IsForShortList = procurementSupplier.IsForShortList,
                    MinuteNo = procurementSupplier.MinuteNo,
                    Remark = procurementSupplier.Remark,
                    Rank = procurementSupplier.Rank,

                    OrgBranchName = procurementSupplier.OrgBranchName,
                    DateCreated = procurementSupplier.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = procurementSupplier.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                _db.ProcurementSuppliers.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "procumrement supplier registration", "Success", User.Identity.Name, $"updated supplier {entity.SupplierId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { procurementSupplier }.ToDataSourceResult(request, ModelState));
        }


        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
