using System;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Procurement;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Helpers.Common;


namespace ExceedERP.Web.Areas.Procurement.Controllers
{


    public class ItemPriceIndexController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementUser, ProcurementRoles.ProcurementApprover)]
        public ActionResult Index()
        {
            PopulateCategories();
            return View();
        }

        private void PopulateCategories()
        {
            var categories = db.ItemCategorys;
            var items = db.InventoryItems.Select(
                i => new
                {
                    ItemCode = i.ItemCode,
                    Name = i.ItemCode+"-"+i.Name
                }).AsQueryable();
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

    

        [AuthorizeRoles(ProcurementRoles.ProcurementUser, ProcurementRoles.ProcurementApprover)]
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

                IsVoid = itemPriceIndex.IsVoid,
                VoidBy = itemPriceIndex.VoidBy,
                VoidTime = itemPriceIndex.VoidTime,
                IsOnlineApproved = itemPriceIndex.IsOnlineApproved,
                OnlineApprovedBy = itemPriceIndex.OnlineApprovedBy,
                OnlineApprovedTime = itemPriceIndex.OnlineApprovedTime,
                IsSendForApproval = itemPriceIndex.IsSendForApproval,
                SendForApprovalBy = itemPriceIndex.SendForApprovalBy,
                SendForApprovalTime = itemPriceIndex.SendForApprovalTime,
                IsOnlineTransferred = itemPriceIndex.IsOnlineTransferred,
                IsOnlineTransferredBy = itemPriceIndex.IsOnlineTransferredBy,
                OnlineTransferredTime = itemPriceIndex.OnlineTransferredTime,

                OrgBranchName = itemPriceIndex.OrgBranchName,
                DateCreated = itemPriceIndex.DateCreated,
                LastModified = itemPriceIndex.LastModified,
                CreatedBy = itemPriceIndex.CreatedBy,
                ModifiedBy = itemPriceIndex.ModifiedBy
            });

            return Json(result);
        }


        [AuthorizeRoles(ProcurementRoles.ProcurementUser)]
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

        [AuthorizeRoles(ProcurementRoles.ProcurementUser)]
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

                    IsVoid = itemPriceIndex.IsVoid,
                    VoidBy = itemPriceIndex.VoidBy,
                    VoidTime = itemPriceIndex.VoidTime,
                    IsOnlineApproved = itemPriceIndex.IsOnlineApproved,
                    OnlineApprovedBy = itemPriceIndex.OnlineApprovedBy,
                    OnlineApprovedTime = itemPriceIndex.OnlineApprovedTime,
                    IsSendForApproval = itemPriceIndex.IsSendForApproval,
                    SendForApprovalBy = itemPriceIndex.SendForApprovalBy,
                    SendForApprovalTime = itemPriceIndex.SendForApprovalTime,
                    IsOnlineTransferred = itemPriceIndex.IsOnlineTransferred,
                    IsOnlineTransferredBy = itemPriceIndex.IsOnlineTransferredBy,
                    OnlineTransferredTime = itemPriceIndex.OnlineTransferredTime,

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

        [AuthorizeRoles(ProcurementRoles.ProcurementUser)]
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

                    IsVoid = itemPriceIndex.IsVoid,
                    VoidBy = itemPriceIndex.VoidBy,
                    VoidTime = itemPriceIndex.VoidTime,
                    IsOnlineApproved = itemPriceIndex.IsOnlineApproved,
                    OnlineApprovedBy = itemPriceIndex.OnlineApprovedBy,
                    OnlineApprovedTime = itemPriceIndex.OnlineApprovedTime,
                    IsSendForApproval = itemPriceIndex.IsSendForApproval,
                    SendForApprovalBy = itemPriceIndex.SendForApprovalBy,
                    SendForApprovalTime = itemPriceIndex.SendForApprovalTime,
                    IsOnlineTransferred = itemPriceIndex.IsOnlineTransferred,
                    IsOnlineTransferredBy = itemPriceIndex.IsOnlineTransferredBy,
                    OnlineTransferredTime = itemPriceIndex.OnlineTransferredTime,

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

        [AuthorizeRoles(ProcurementRoles.ProcurementUser)]
        public JsonResult SendForApproval(int id)
        {
            var record = db.ItemPriceIndexes.Find(id);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                db.ItemPriceIndexes.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "item price index", "Success", User.Identity.Name, $"send for approval item price index id {id}  ", Modules.Procurement);
                #endregion
                var formattedData = new
                {
                    Message = "Successfully Sent.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record == null)
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

           

            if (record.IsSendForApproval)
            {
                var formattedData = new
                {
                    Message = "The record is already sent for approval you cannot send again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is already approved.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }
        [AuthorizeRoles(ProcurementRoles.ProcurementAdmin, ProcurementRoles.ProcurementApprover, ProcurementRoles.ProcurementUser)]
    
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
