using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Proforma;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Proforma
{
   

    public class ProformaWinningSuppliersController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser, ProcurementConstants.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser, ProcurementConstants.ProcurementTenderApprover)]
        public ActionResult WinningSuppliers_Read([DataSourceRequest]DataSourceRequest request , int id )
        {
            IQueryable<WinningSupplier> winningsuppliers = _db.WinningSuppliers.Where(p => p.ProformaPurchaseID == id);
            DataSourceResult result = winningsuppliers.ToDataSourceResult(request, winningSupplier => new {
                WinningSupplierID = winningSupplier.WinningSupplierID,
                ProformaPurchaseID = winningSupplier.ProformaPurchaseID,
                SupplierId = winningSupplier.SupplierId,
                IsNewSupplier = winningSupplier.IsNewSupplier,
                SupplierName = winningSupplier.SupplierName,
                DeliveryDate = winningSupplier.DeliveryDate,
                AwardDate = winningSupplier.AwardDate,
                AwardGiven = winningSupplier.AwardGiven,
                GRNNo = winningSupplier.GRNNo,
                Remark = winningSupplier.Remark,
                OrgBranchName = winningSupplier.OrgBranchName,
                DateCreated = winningSupplier.DateCreated,
                LastModified = winningSupplier.LastModified,
                CreatedBy = winningSupplier.CreatedBy,
                ModifiedBy = winningSupplier.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WinningSuppliers_Create([DataSourceRequest]DataSourceRequest request, WinningSupplier winningSupplier , int id )
            {
            if (ModelState.IsValid)
            {
                    var entity = new WinningSupplier
                    {
                        ProformaPurchaseID =id,
                        SupplierId = winningSupplier.SupplierId,
                        IsNewSupplier = winningSupplier.IsNewSupplier,
                        SupplierName = winningSupplier.SupplierName,
                        DeliveryDate = winningSupplier.DeliveryDate,
                        AwardDate = winningSupplier.AwardDate,
                        AwardGiven = winningSupplier.AwardGiven,
                        GRNNo = winningSupplier.GRNNo,
                        Remark = winningSupplier.Remark,
                        OrgBranchName = winningSupplier.OrgBranchName,
                        DateCreated = DateTime.Today,
                        LastModified = winningSupplier.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = winningSupplier.ModifiedBy
                    };

                    _db.WinningSuppliers.Add(entity);
                    _db.SaveChanges();
                    winningSupplier.WinningSupplierID = entity.WinningSupplierID;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "proforma purchase winners", "Success", User.Identity.Name, $"created winner {entity.SupplierName} under record {entity.ProformaPurchaseID}", Modules.Procurement);
                #endregion

            }


            return Json(new[] { winningSupplier }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WinningSuppliers_Update([DataSourceRequest]DataSourceRequest request, WinningSupplier winningSupplier)
        {
                if (ModelState.IsValid)
                {
                        var entity = new WinningSupplier
                        {
                            ProformaPurchaseID= winningSupplier.ProformaPurchaseID,
                            WinningSupplierID = winningSupplier.WinningSupplierID,
                            SupplierId = winningSupplier.SupplierId,
                            IsNewSupplier = winningSupplier.IsNewSupplier,
                            SupplierName = winningSupplier.SupplierName,
                            DeliveryDate = winningSupplier.DeliveryDate,
                            AwardDate = winningSupplier.AwardDate,
                            AwardGiven = winningSupplier.AwardGiven,
                            GRNNo = winningSupplier.GRNNo,
                            Remark = winningSupplier.Remark,
                            OrgBranchName = winningSupplier.OrgBranchName,
                            DateCreated = winningSupplier.DateCreated,
                            LastModified = DateTime.Today,
                            CreatedBy = winningSupplier.CreatedBy,
                            ModifiedBy = User.Identity.Name
                        };

                        _db.WinningSuppliers.Attach(entity);
                        _db.Entry(entity).State = EntityState.Modified;
                    
                    _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "proforma purchase winners", "Success", User.Identity.Name, $"updated winner {entity.SupplierName} under record {entity.ProformaPurchaseID}", Modules.Procurement);
                #endregion
            }


            return Json(new[] { winningSupplier }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WinningSuppliers_Destroy([DataSourceRequest]DataSourceRequest request, WinningSupplier winningSupplier)
        {
            if (ModelState.IsValid)
            {
                var entity = new WinningSupplier
                {
                    WinningSupplierID = winningSupplier.WinningSupplierID,
                    SupplierId = winningSupplier.SupplierId,
                    IsNewSupplier = winningSupplier.IsNewSupplier,
                    SupplierName = winningSupplier.SupplierName,
                    DeliveryDate = winningSupplier.DeliveryDate,
                    GRNNo = winningSupplier.GRNNo,
                    Remark = winningSupplier.Remark,
                    OrgBranchName = winningSupplier.OrgBranchName,
                    DateCreated = winningSupplier.DateCreated,
                    LastModified = winningSupplier.LastModified,
                    CreatedBy = winningSupplier.CreatedBy,
                    ModifiedBy = winningSupplier.ModifiedBy
                };

                _db.WinningSuppliers.Attach(entity);
                _db.WinningSuppliers.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { winningSupplier }.ToDataSourceResult(request, ModelState));
        }

        
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
