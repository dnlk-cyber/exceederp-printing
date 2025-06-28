using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Core.Domain.CRM.TenderManagement;

namespace ExceedERP.Web.Areas.CRM.Controllers.TenderManagement
{
	public class SalesLotParticipantsController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()

        {
           // IList<ProductCategory> productCategories = db.ProductCategories.ToList();
            ViewData["productCategories"] = db.SalesLotItems.ToList();
            ViewData["products"] = db.SalesLotItems.ToList();           
            ViewData["saleslotPurchase"] = db.SalesLotPurchases.ToList();
            ViewData["SalesLots"] = db.SalesLotPurchases.ToList();
            return View();
        }

        public ActionResult SalesLotParticipants_Read([DataSourceRequest]DataSourceRequest request,int id)
        {
            IQueryable<SalesLotParticipant> salesLots = db.SalesLotParticipants.Where(x=>x.SalesLotId==id);
            DataSourceResult result = salesLots.ToDataSourceResult(request, salesLot => new {
                SalesLotParticipantId = salesLot.SalesLotParticipantId,
                SalesLotId = salesLot.SalesLotId,
                PurchaseDate = salesLot.PurchaseDate,
                ReceiptNo = salesLot.ReceiptNo,
                ReceiptNotRequired = salesLot.ReceiptNotRequired,
                CustomerName = salesLot.CustomerName,
                TinNumber = salesLot.TinNumber,
                PurchaseId = salesLot.PurchaseId,
                Representative = salesLot.Representative,
                Telephone = salesLot.Telephone,
                Address = salesLot.Address,
                Remark = salesLot.Remark,
                OrgBranchName = salesLot.OrgBranchName,
                DateCreated = salesLot.DateCreated,
                CreatedBy = salesLot.CreatedBy,
                ModifiedBy = salesLot.ModifiedBy,
                LastModified = salesLot.LastModified,
                Count = GetItemCount(salesLot.SalesLotParticipantId)
            });

            return Json(result);
        }
        private int GetItemCount(int Id)
        {
            var fetchItems = db.SalesLotParticipantItems.Where(x => x.SalesLotParticipantId == Id).ToList();
            int TotalItems = 0;
            if (fetchItems.Count>0)
            { TotalItems = fetchItems.Count(); }
            return TotalItems;
        }
        public ActionResult SalesLotParticipants_Create([DataSourceRequest]DataSourceRequest request, SalesLotParticipant salesLot,int id)
        {
            var record = db.SalesLots.Find(id);
            if (record.IsOnlineApproved)
            {
                ModelState.AddModelError("Registered","You can not add Participant after Lot is Approved");
            }
            ModelState.Remove("TinNumber");
            var salesPurchaseCheck = db.SalesLotPurchases.SingleOrDefault(prop => prop.SalesLotPurchaseId == salesLot.PurchaseId);
            var salesParticipantCheck = db.SalesLotParticipants.SingleOrDefault(prop => prop.PurchaseId==salesLot.PurchaseId && prop.SalesLotId == id);
                if (salesPurchaseCheck!=null&&salesParticipantCheck==null) {
                    salesLot.Address = salesPurchaseCheck.Address;
                    salesLot.CustomerName = salesPurchaseCheck.CustomerName;
                    salesLot.PurchaseDate = salesPurchaseCheck.PurchaseDate;
                    salesLot.TinNumber = salesPurchaseCheck.TinNumber;
                    salesLot.ReceiptNo = salesPurchaseCheck.ReceiptNo;
                    salesLot.ReceiptNotRequired = salesPurchaseCheck.ReceiptNotRequired;
                    salesLot.Remark = salesPurchaseCheck.Remark;
                    salesLot.Representative = salesPurchaseCheck.Representative;
                    salesLot.SalesLotId = salesPurchaseCheck.SalesLotId;
                    salesLot.Telephone = salesPurchaseCheck.Telephone;
                    if (ModelState.IsValid)
                    {
                        salesLot.DateCreated = DateTime.Now;
                        salesLot.CreatedBy = User.Identity.Name;
                        var entity = GetMappedObject(salesLot,id);
                        db.SalesLotParticipants.Add(entity);
                        db.SaveChanges();
                        salesLot.SalesLotParticipantId = entity.SalesLotParticipantId;
                    
                }
            
            }
            return Json(new[] { salesLot }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SalesLotPurchases_Update([DataSourceRequest]DataSourceRequest request, SalesLotParticipant salesLot,int id)
        {
            var record = db.SalesLots.Find(id);
            if (record.IsOnlineApproved)
                {
                    ModelState.AddModelError("Approved", "You can not Update Participant after Lot is Approved");
                }
                if (ModelState.IsValid)
                {
                    salesLot.LastModified = DateTime.Now;
                    salesLot.ModifiedBy = User.Identity.Name;
                    var entity = GetMappedObject(salesLot,id);
                    db.SalesLotParticipants.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
            
            return Json(new[] { salesLot }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SalesLotParticipants_Destroy([DataSourceRequest]DataSourceRequest request, SalesLotParticipant salesLot,int id)
        {
            var record = db.SalesLots.Find(id);

            if (record.IsOnlineApproved)
                if (record.IsOnlineApproved)
                {
                    ModelState.AddModelError("Approved", "You can not Delete Participant after Lot is Approved");
                }
            var salesPurchaseCheck = db.SalesLotPurchases.SingleOrDefault(prop => prop.TinNumber == salesLot.TinNumber && prop.SalesLotId == id);

            if (salesPurchaseCheck == null)
            {
                
                    ModelState.AddModelError(" Not Registered", "Fist buy Bid Document");
                }

            var xyz = db.SalesLotParticipants.SingleOrDefault(xyzz => xyzz.SalesLotParticipantId == salesLot.SalesLotParticipantId);     
                    if (ModelState.IsValid)
                    {
                                        
                        db.SalesLotParticipants.Remove(xyz);
                        db.SaveChanges();
                    }
                
            
            return Json(new[] { salesLot }.ToDataSourceResult(request, ModelState));
        }
        private SalesLotParticipant GetMappedObject(SalesLotParticipant salesLot,int id)
        {
            var entity = new SalesLotParticipant
            {
                SalesLotParticipantId = salesLot.SalesLotParticipantId,
                SalesLotId = id,
                PurchaseDate = salesLot.PurchaseDate,
                ReceiptNo = salesLot.ReceiptNo,
                ReceiptNotRequired = salesLot.ReceiptNotRequired,
                Representative = salesLot.Representative,
                CustomerName = salesLot.CustomerName,
                TinNumber = salesLot.TinNumber,
                Telephone = salesLot.Telephone,
                PurchaseId = salesLot.PurchaseId,
                Address = salesLot.Address,
                Remark = salesLot.Remark,
                OrgBranchName = salesLot.OrgBranchName,
                DateCreated = salesLot.DateCreated,
                CreatedBy = salesLot.CreatedBy,
                ModifiedBy = salesLot.ModifiedBy,
                LastModified = salesLot.LastModified,

            };
            return entity;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
