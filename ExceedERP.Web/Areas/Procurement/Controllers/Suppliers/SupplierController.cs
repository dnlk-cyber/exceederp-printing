using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Core.Domain.Procurement.Supplier;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Suppliers
{
    

    public class SupplierController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
      
        private static int Count = 0;

        [AuthorizeRoles(ProcurementRoles.ProcurementSupplierUser, ProcurementRoles.ProcurementSupplierApprover)]
        public ActionResult Index(string id)
        {
            return View();
        }

        public JsonResult GetSupplierCategories()
        {
            var categories = db.SupplierCategories;

            return Json(categories.Select(c => new { Category = c.Category }), JsonRequestBehavior.AllowGet);
        }


        [AuthorizeRoles(ProcurementRoles.ProcurementSupplierUser, ProcurementRoles.ProcurementSupplierApprover)]
        public ActionResult SupplierRankUpdate(string idtt, string name)
        {
            ProcurementSupplier procurement = db.ProcurementSuppliers.Find(int.Parse(idtt));
            procurement.Rank = name;
            procurement.ModifiedBy = User.Identity.Name;
            procurement.LastModified = DateTime.Today;
            db.ProcurementSuppliers.Attach(procurement);
            db.Entry(procurement).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");

        }

        [AuthorizeRoles(ProcurementRoles.ProcurementSupplierUser, ProcurementRoles.ProcurementSupplierApprover)]
        public ActionResult ProcurementSuppliers_Read([DataSourceRequest]DataSourceRequest request, string id)
        {
            IQueryable<ProcurementSupplier> procurementsuppliers = db.ProcurementSuppliers;

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
                Remark = procurementSupplier.Remark,
                TCode = procurementSupplier.TCode,
                Rank = procurementSupplier.Rank,
                OrgBranchName = procurementSupplier.OrgBranchName,
                DateCreated = procurementSupplier.DateCreated,
                LastModified = procurementSupplier.LastModified,
                CreatedBy = procurementSupplier.CreatedBy,
                ModifiedBy = procurementSupplier.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementSupplierUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProcurementSuppliers_Create([DataSourceRequest]DataSourceRequest request, ProcurementSupplier procurementSupplier)
        {
            #region validation
            var exists = db.ProcurementSuppliers.Where(x => x.Category.ToLower() == procurementSupplier.Category.ToLower())
                .Where(x => x.Name.ToLower() == procurementSupplier.Name.ToLower()).ToList();

            if (exists.Any())
            {
                ModelState.AddModelError("Name", procurementSupplier.Name + " already exists in the system.");
            }

            var supplierid = db.ProcurementSuppliers.Where(x => x.Category.ToLower() == procurementSupplier.Category.ToLower())
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
                    TCode = procurementSupplier.TCode,
                    Rank = procurementSupplier.Rank,
                    OrgBranchName = procurementSupplier.OrgBranchName,
                    DateCreated = procurementSupplier.DateCreated,
                    LastModified = procurementSupplier.LastModified,
                    CreatedBy = procurementSupplier.CreatedBy,
                    ModifiedBy = procurementSupplier.ModifiedBy
                };

                db.ProcurementSuppliers.Add(entity);
                db.SaveChanges();
                procurementSupplier.ProcurementSupplierID = entity.ProcurementSupplierID;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Supplier ", "Success", User.Identity.Name, $"created supplier {entity.Name}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { procurementSupplier }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementSupplierUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProcurementSuppliers_Update([DataSourceRequest]DataSourceRequest request, ProcurementSupplier procurementSupplier)
        {
            #region validation
            var exists = db.ProcurementSuppliers.Where(x => x.ProcurementSupplierID != procurementSupplier.ProcurementSupplierID)
                .Where(x => x.Category.ToLower() == procurementSupplier.Category.ToLower())
                .Where(x => x.Name.ToLower() == procurementSupplier.Name.ToLower()).ToList();

            if (exists.Any())
            {
                ModelState.AddModelError("Name", procurementSupplier.Name + " already exists in the system.");
            }

            var supplierid = db.ProcurementSuppliers.Where(x => x.ProcurementSupplierID != procurementSupplier.ProcurementSupplierID)
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
                    Remark = procurementSupplier.Remark,
                    TCode = procurementSupplier.TCode,
                    Rank = procurementSupplier.Rank,
                    OrgBranchName = procurementSupplier.OrgBranchName,
                    MinuteNo = procurementSupplier.MinuteNo,
                    DateCreated = procurementSupplier.DateCreated,
                    LastModified = procurementSupplier.LastModified,
                    CreatedBy = procurementSupplier.CreatedBy,
                    ModifiedBy = procurementSupplier.ModifiedBy
                };

                db.ProcurementSuppliers.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Supplier ", "Success", User.Identity.Name, $"updated supplier {entity.Name}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { procurementSupplier }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementSupplierUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProcurementSuppliers_Destroy([DataSourceRequest]DataSourceRequest request, ProcurementSupplier procurementSupplier)
        {
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
                    TCode = procurementSupplier.TCode,
                    Rank = procurementSupplier.Rank,
                    OrgBranchName = procurementSupplier.OrgBranchName,
                    DateCreated = procurementSupplier.DateCreated,
                    LastModified = procurementSupplier.LastModified,
                    CreatedBy = procurementSupplier.CreatedBy,
                    ModifiedBy = procurementSupplier.ModifiedBy
                };

                db.ProcurementSuppliers.Attach(entity);
                db.ProcurementSuppliers.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { procurementSupplier }.ToDataSourceResult(request, ModelState));
        }

        public JsonResult GetSuppliers()
        {
            var supplierlist = db.ProcurementSuppliers.Where(x => x.IsActive);
            return Json(supplierlist.Select(i => new { Name = i.Name, ProcurementSupplierID = i.ProcurementSupplierID }), JsonRequestBehavior.AllowGet);

        }

        public ActionResult Shortlist(string id)
        {
            if (id != null)
            {
                int sid = int.Parse(id);
                var supplier = db.ProcurementSuppliers.FirstOrDefault(p => p.ProcurementSupplierID == sid);
                if (supplier != null && supplier.IsForShortList)
                {
                    supplier.IsForShortList = false;
                    #region disable log
                    UserHelper.DeleteLog(Request.UserHostAddress, "Disable", "Success", User.Identity.Name, "Removed from short list  " + supplier.Name);
                    #endregion
                }
                else
                {
                    if (supplier != null)
                    {
                        supplier.IsForShortList = true;
                        #region enable log
                        UserHelper.DeleteLog(Request.UserHostAddress, "Enable", "Success", User.Identity.Name, "Added to short list " + supplier.Name);
                        #endregion
                    }
                }
                if (supplier != null)
                {
                    db.ProcurementSuppliers.Attach(supplier);
                    db.Entry(supplier).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Index");

        }

        [AuthorizeRoles(ProcurementRoles.ProcurementSupplierUser)]
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementSupplierUser)]
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
