﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
﻿using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Procurement;
 using ExceedERP.Core.Domain.Procurement.Settings;
 using ExceedERP.DataAccess.Context;
 using ExceedERP.Helpers.Procurement;
 using ExceedERP.Web.Helpers;


namespace ExceedERP.Web.Areas.Procurement.Controllers
{


    public class ItemPriceIndexController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementConstants.ProcurementUser, ProcurementConstants.ProcurementApprover)]
        public ActionResult Index()
        {
            PopulateCategories();
            return View();
        }

        private void PopulateCategories()
        {
            var categories = db.ItemCategorys;
            var items = db.InventoryItems;
            var branches = db.Branch;

            var inventoryPeriods = db.InventoryPeriods;
            var selectedinventoryPeriods = inventoryPeriods.Select(
                i => new
                {
                    Name = i.Name,
                    GLPeriodId = i.InventoryPeriodId
                }).AsQueryable();
            ViewData["inventoryPeriods"] = selectedinventoryPeriods;
            ViewData["Items"] = items;
            ViewData["Category"] = categories;
            ViewData["defaultCategory"] = categories.FirstOrDefault();
            ViewData["Branches"] = branches;

        }

    

        [AuthorizeRoles(ProcurementConstants.ProcurementUser, ProcurementConstants.ProcurementApprover)]
        public ActionResult ItemPriceIndexes_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ItemPriceIndex> itempriceindexes = db.ItemPriceIndexes;
            DataSourceResult result = itempriceindexes.ToDataSourceResult(request, itemPriceIndex => new {
                BranchId = itemPriceIndex.BranchId,
                ItemPriceIndexID = itemPriceIndex.ItemPriceIndexID,
                ItemCategoryCode = itemPriceIndex.ItemCategoryCode,
                ItemCode = itemPriceIndex.ItemCode,
                ItemName = itemPriceIndex.ItemName,
                PriceBeforeVAT = itemPriceIndex.PriceBeforeVAT,
                SourceOfData = itemPriceIndex.SourceOfData,
                Remark = itemPriceIndex.Remark,
                DateOfInformation = itemPriceIndex.DateOfInformation,
                SourceType = itemPriceIndex.SourceType,
                GLPeriodId = itemPriceIndex.GLPeriodId,
                SupplierName = itemPriceIndex.SupplierName,

                OrgBranchName = itemPriceIndex.OrgBranchName,
                DateCreated = itemPriceIndex.DateCreated,
                LastModified = itemPriceIndex.LastModified,
                CreatedBy = itemPriceIndex.CreatedBy,
                ModifiedBy = itemPriceIndex.ModifiedBy
            });

            return Json(result);
        }


        [AuthorizeRoles(ProcurementConstants.ProcurementUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ItemPriceIndexes_Create([DataSourceRequest]DataSourceRequest request, ItemPriceIndex itemPriceIndex)
        {
            if (ModelState.IsValid)
            {
                var priceIndex = db.ItemPriceIndexes.Where(x=>x.BranchId==itemPriceIndex.BranchId).FirstOrDefault(x => x.ItemCode == itemPriceIndex.ItemCode);
                if (priceIndex == null)
                {
                    #region new

                    var entity = new ItemPriceIndex
                    {
                        BranchId = itemPriceIndex.BranchId,
                        ItemPriceIndexID = itemPriceIndex.ItemPriceIndexID,
                        ItemCategoryCode = itemPriceIndex.ItemCategoryCode,
                        ItemCode = itemPriceIndex.ItemCode,
                        ItemName = db.InventoryItems.Find(itemPriceIndex.ItemCode)?.Name,
                        PriceBeforeVAT = itemPriceIndex.PriceBeforeVAT,
                        SourceOfData = itemPriceIndex.SourceOfData,
                        Remark = itemPriceIndex.Remark,
                        DateOfInformation = itemPriceIndex.DateOfInformation,
                        SourceType = SourceType.External,
                        GLPeriodId = itemPriceIndex.GLPeriodId,
                        SupplierName = itemPriceIndex.SupplierName,

                        OrgBranchName = itemPriceIndex.OrgBranchName,
                        DateCreated = itemPriceIndex.DateCreated,
                        LastModified = itemPriceIndex.LastModified,
                        CreatedBy = itemPriceIndex.CreatedBy,
                        ModifiedBy = itemPriceIndex.ModifiedBy
                    };

                    db.ItemPriceIndexes.Add(entity);
                    db.SaveChanges();
                    itemPriceIndex.ItemPriceIndexID = entity.ItemPriceIndexID;

                    #endregion
                }
                else
                {
                    #region update
                    priceIndex.PriceBeforeVAT = itemPriceIndex.PriceBeforeVAT;
                    priceIndex.DateOfInformation = itemPriceIndex.DateOfInformation;
                    priceIndex.SourceOfData = itemPriceIndex.SourceOfData;
                    priceIndex.SourceType = SourceType.External;
                    priceIndex.GLPeriodId = itemPriceIndex.GLPeriodId;
                    priceIndex.SupplierName = itemPriceIndex.SupplierName;
                    priceIndex.Remark = itemPriceIndex.Remark;
                    db.ItemPriceIndexes.Attach(priceIndex);
                    db.Entry(priceIndex).State = EntityState.Modified;
                    db.SaveChanges();
                    #endregion
                }
            }

            return Json(new[] { itemPriceIndex }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ItemPriceIndexes_Update([DataSourceRequest]DataSourceRequest request, ItemPriceIndex itemPriceIndex)
        {
            if (ModelState.IsValid)
            {
                var entity = new ItemPriceIndex
                {
                    BranchId = itemPriceIndex.BranchId,
                    ItemPriceIndexID = itemPriceIndex.ItemPriceIndexID,
                    ItemCategoryCode = itemPriceIndex.ItemCategoryCode,
                    ItemCode = itemPriceIndex.ItemCode,
                    ItemName = db.InventoryItems.Find(itemPriceIndex.ItemCode)?.Name,
                    PriceBeforeVAT = itemPriceIndex.PriceBeforeVAT,
                    SourceOfData = itemPriceIndex.SourceOfData,
                    Remark = itemPriceIndex.Remark,
                    DateOfInformation = itemPriceIndex.DateOfInformation,
                    SourceType = SourceType.External,
                    GLPeriodId = itemPriceIndex.GLPeriodId,
                    SupplierName = itemPriceIndex.SupplierName,

                    OrgBranchName = itemPriceIndex.OrgBranchName,
                    DateCreated = itemPriceIndex.DateCreated,
                    LastModified = itemPriceIndex.LastModified,
                    CreatedBy = itemPriceIndex.CreatedBy,
                    ModifiedBy = itemPriceIndex.ModifiedBy
                };

                db.ItemPriceIndexes.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { itemPriceIndex }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ItemPriceIndexes_Destroy([DataSourceRequest]DataSourceRequest request, ItemPriceIndex itemPriceIndex)
        {
            if (ModelState.IsValid)
            {
                var entity = new ItemPriceIndex
                {
                    BranchId = itemPriceIndex.BranchId,
                    ItemPriceIndexID = itemPriceIndex.ItemPriceIndexID,
                    ItemCategoryCode = itemPriceIndex.ItemCategoryCode,
                    ItemCode = itemPriceIndex.ItemCode,
                    PriceBeforeVAT = itemPriceIndex.PriceBeforeVAT,
                    SourceOfData = itemPriceIndex.SourceOfData,
                    Remark = itemPriceIndex.Remark,
                    DateOfInformation = itemPriceIndex.DateOfInformation,
                    SourceType = itemPriceIndex.SourceType,
                    GLPeriodId = itemPriceIndex.GLPeriodId,
                    SupplierName = itemPriceIndex.SupplierName,

                    OrgBranchName = itemPriceIndex.OrgBranchName,
                    DateCreated = itemPriceIndex.DateCreated,
                    LastModified = itemPriceIndex.LastModified,
                    CreatedBy = itemPriceIndex.CreatedBy,
                    ModifiedBy = itemPriceIndex.ModifiedBy
                };

                db.ItemPriceIndexes.Attach(entity);
                db.ItemPriceIndexes.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { itemPriceIndex }.ToDataSourceResult(request, ModelState));
        }
    
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }
    [AuthorizeRoles(ProcurementConstants.ProcurementAdmin, ProcurementConstants.ProcurementApprover, ProcurementConstants.ProcurementUser)]
    
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
