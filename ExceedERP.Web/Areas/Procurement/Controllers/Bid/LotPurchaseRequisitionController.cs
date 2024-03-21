using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Core.Domain.Procurement.Tender.Lot;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Bid
{
    public class LotPurchaseRequisitionController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult LotPurchaseRequisitions_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<LotPurchaseRequisition> lotpurchaserequisitions = _db.LotPurchaseRequisitions.Where(x=>x.LotID==id);
            DataSourceResult result = lotpurchaserequisitions.ToDataSourceResult(request, lotPurchaseRequisition => new {
                PurchaseRequisitionAssignmentId = lotPurchaseRequisition.PurchaseRequisitionAssignmentId,
                LotPurchaseRequisitionId = lotPurchaseRequisition.LotPurchaseRequisitionId,
                LotID = lotPurchaseRequisition.LotID,
                StoreRequisitionID = lotPurchaseRequisition.StoreRequisitionID,
                PurchaseRequisitionID = lotPurchaseRequisition.PurchaseRequisitionID,
                OrgBranchName = lotPurchaseRequisition.OrgBranchName,
                DateCreated = lotPurchaseRequisition.DateCreated,
                LastModified = lotPurchaseRequisition.LastModified,
                CreatedBy = lotPurchaseRequisition.CreatedBy,
                ModifiedBy = lotPurchaseRequisition.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LotPurchaseRequisitions_Create(int id,[DataSourceRequest]DataSourceRequest request, LotPurchaseRequisition lotPurchaseRequisition)
        {
            var exist = _db.LotPurchaseRequisitions
                .Where(x=>x.LotID==id) //added
                .Where(x => x.PurchaseRequisitionAssignmentId == lotPurchaseRequisition.PurchaseRequisitionAssignmentId).FirstOrDefault();
            if (exist!=null)
            {
                ModelState.AddModelError("exist", $" The assignement is already added in lot {exist.LotID} - lot number {exist.Lot.LotNumber} ");
            }
            //var exist = db.LotPurchaseRequisitions.Where(x => x.PurchaseRequisitionID == lotPurchaseRequisition.PurchaseRequisitionID).ToList();
            //if (exist.Any())
            //{
            //    string tendereref = "";
            //    var existlot = db.Lots.Find(exist.FirstOrDefault().LotID);
            //    if (existlot != null)
            //    {
            //        var existtender = db.Tenders.Find(existlot.TenderID);
            //        if (existtender != null && !existtender.IsCancelled)
            //            ModelState.AddModelError("exist", lotPurchaseRequisition.PurchaseRequisitionID + $" already exists in the system under tender { existtender.TenderReference }.");
            //    }


            //}
            var assignment =
                _db.PurchaseRequisitionAssignments.Find(lotPurchaseRequisition.PurchaseRequisitionAssignmentId);


            if (assignment != null)
            {
                lotPurchaseRequisition.PurchaseRequisitionID = assignment.PurchaseRequisitionID;

                var purchaseRequisition = _db.PurchaseRequisitions.Find(assignment.PurchaseRequisitionID);
                if (purchaseRequisition != null)
                    lotPurchaseRequisition.StoreRequisitionID = purchaseRequisition.StoreRequisitionID;
            }

            if (ModelState.IsValid)
            {
                var entity = new LotPurchaseRequisition
                {
                    LotID = id,
                    PurchaseRequisitionAssignmentId= lotPurchaseRequisition.PurchaseRequisitionAssignmentId,
                    StoreRequisitionID = lotPurchaseRequisition.StoreRequisitionID,
                    PurchaseRequisitionID = lotPurchaseRequisition.PurchaseRequisitionID,
                    OrgBranchName = lotPurchaseRequisition.OrgBranchName,
                    DateCreated = lotPurchaseRequisition.DateCreated,
                    LastModified = lotPurchaseRequisition.LastModified,
                    CreatedBy = lotPurchaseRequisition.CreatedBy,
                    ModifiedBy = lotPurchaseRequisition.ModifiedBy
                };

                _db.LotPurchaseRequisitions.Add(entity);
                _db.SaveChanges();
                lotPurchaseRequisition.LotPurchaseRequisitionId = entity.LotPurchaseRequisitionId;

                AddPrItems(lotPurchaseRequisition.PurchaseRequisitionAssignmentId, id);
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "lot required document", "Success", User.Identity.Name, $"created lot PR {entity.PurchaseRequisitionID} under record {entity.LotID} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { lotPurchaseRequisition }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LotPurchaseRequisitions_Update([DataSourceRequest]DataSourceRequest request, LotPurchaseRequisition lotPurchaseRequisition)
        {

            var exist = _db.LotPurchaseRequisitions
                 .Where(x => x.LotID == lotPurchaseRequisition.LotID) //added
                .Where(x => x.LotPurchaseRequisitionId != lotPurchaseRequisition.LotPurchaseRequisitionId)
                .Where(x => x.PurchaseRequisitionAssignmentId == lotPurchaseRequisition.PurchaseRequisitionAssignmentId).FirstOrDefault();
            if (exist != null)
            {
                ModelState.AddModelError("exist", $" The assignement is already added in lot {exist.LotID} - lot number {exist.Lot.LotNumber} ");
            }
            //var exist = db.LotPurchaseRequisitions.Where(x => x.PurchaseRequisitionID == lotPurchaseRequisition.PurchaseRequisitionID).ToList();
            //if (exist.Any())
            //{
            //    string tendereref = "";
            //    var existlot = db.Lots.Find(exist.FirstOrDefault()?.LotID);
            //    if (existlot != null)
            //    {
            //        var existtender = db.Tenders.Find(existlot.TenderID);
            //        if (existtender != null && !existtender.IsCancelled)
            //            ModelState.AddModelError("exist", lotPurchaseRequisition.PurchaseRequisitionID + $" already exists in the system under tender { existtender.TenderReference }.");
            //    }


            //}

            var assignment =
                _db.PurchaseRequisitionAssignments.Find(lotPurchaseRequisition.PurchaseRequisitionAssignmentId);


            if (assignment != null)
            {
                lotPurchaseRequisition.PurchaseRequisitionID = assignment.PurchaseRequisitionID;

                var purchaseRequisition = _db.PurchaseRequisitions.Find(assignment.PurchaseRequisitionID);
                if (purchaseRequisition != null)
                    lotPurchaseRequisition.StoreRequisitionID = purchaseRequisition.StoreRequisitionID;
            }

            if (ModelState.IsValid)
            {
                var entity = new LotPurchaseRequisition
                {
                    LotID = lotPurchaseRequisition.LotID,
                    StoreRequisitionID = lotPurchaseRequisition.StoreRequisitionID,
                    LotPurchaseRequisitionId = lotPurchaseRequisition.LotPurchaseRequisitionId,
                    PurchaseRequisitionAssignmentId = lotPurchaseRequisition.PurchaseRequisitionAssignmentId,
                    PurchaseRequisitionID = lotPurchaseRequisition.PurchaseRequisitionID,
                    OrgBranchName = lotPurchaseRequisition.OrgBranchName,
                    DateCreated = lotPurchaseRequisition.DateCreated,
                    LastModified = lotPurchaseRequisition.LastModified,
                    CreatedBy = lotPurchaseRequisition.CreatedBy,
                    ModifiedBy = lotPurchaseRequisition.ModifiedBy
                };

                _db.LotPurchaseRequisitions.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "lot required document", "Success", User.Identity.Name, $"updated lot PR {entity.PurchaseRequisitionID} under record {entity.LotID} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { lotPurchaseRequisition }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LotPurchaseRequisitions_Destroy([DataSourceRequest]DataSourceRequest request, LotPurchaseRequisition lotPurchaseRequisition)
        {
            if (ModelState.IsValid)
            {
                var entity = new LotPurchaseRequisition
                {
                    LotID = lotPurchaseRequisition.LotID,
                    StoreRequisitionID = lotPurchaseRequisition.StoreRequisitionID,
                    LotPurchaseRequisitionId = lotPurchaseRequisition.LotPurchaseRequisitionId,
                    PurchaseRequisitionAssignmentId = lotPurchaseRequisition.PurchaseRequisitionAssignmentId,
                    PurchaseRequisitionID = lotPurchaseRequisition.PurchaseRequisitionID,
                    OrgBranchName = lotPurchaseRequisition.OrgBranchName,
                    DateCreated = lotPurchaseRequisition.DateCreated,
                    LastModified = lotPurchaseRequisition.LastModified,
                    CreatedBy = lotPurchaseRequisition.CreatedBy,
                    ModifiedBy = lotPurchaseRequisition.ModifiedBy
                };

                _db.LotPurchaseRequisitions.Attach(entity);
                _db.LotPurchaseRequisitions.Remove(entity);
                _db.SaveChanges();

                RemovePrItems(lotPurchaseRequisition.PurchaseRequisitionAssignmentId, lotPurchaseRequisition.LotID);
            }

            return Json(new[] { lotPurchaseRequisition }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        private void AddPrItems(int assignmentid, int id)
        {
            var record = _db.PurchaseRequisitionAssignments.Find(assignmentid);
            if (record != null)
            {
                foreach (var item in record.PurchaseRequisitionReassignmentItems)
                {

                    if (item.ItemType == PurchaseItemType.FixedAsset)
                    {
                        #region fixed asset

                        var existingLotItem = _db.LotItems
                            .Where(x => x.LotID == id)
                            .Where(x=>x.Name==item.Name)
                            .Where(x => x.ItmCode == item.ItmCode)
                             .Where(x => x.ItemSpecification == item.ItemSpecification)//added
                            .FirstOrDefault(x => x.ModelNo == item.ModelNo);
                        if (existingLotItem == null)
                        {

                            LotItems lotItems = new LotItems
                            {
                                LotID = id,
                                CatCode = item.CatCode,
                                CategoryName = item.CategoryName,
                                ItmCode = item.ItmCode,
                                ItemName = item.ItemName,
                                Name = item.Name,
                                ModelNo = item.ModelNo,
                                PartNo = item.PartNo,
                                Quantity = item.Quantity,
                                ItemSpecification = item.ItemSpecification,
                                UoM = item.UoM,
                                ItemType = item.ItemType,
                            };


                            _db.LotItems.Add(lotItems);
                            _db.SaveChanges();
                        }
                        else
                        {
                            existingLotItem.Quantity = existingLotItem.Quantity + item.Quantity;
                            _db.Entry(existingLotItem).State = EntityState.Modified;
                            _db.SaveChanges();
                        }

                        #endregion
                    }

                    else
                    {
                        #region sock and service

                        var existingLotItem = _db.LotItems
                            .Where(x => x.LotID == id)
                            .Where(x => x.ItemSpecification == item.ItemSpecification)//added
                            .FirstOrDefault(x => x.ItmCode == item.ItmCode);
                        if (existingLotItem == null)
                        {

                            LotItems lotItems = new LotItems
                            {
                                LotID = id,
                                CatCode = item.CatCode,
                                CategoryName = item.CategoryName,
                                ItmCode = item.ItmCode,
                                ItemName = item.ItemName,
                                PartNo = item.PartNo,
                                Quantity = item.Quantity,
                                ItemSpecification = item.ItemSpecification,
                                UoM = item.UoM,
                                ItemType = item.ItemType,
                            };


                            _db.LotItems.Add(lotItems);
                            _db.SaveChanges();
                        }
                        else
                        {
                            existingLotItem.Quantity = existingLotItem.Quantity + item.Quantity;
                            _db.Entry(existingLotItem).State = EntityState.Modified;
                            _db.SaveChanges();
                        }
                        #endregion
                    }


                }
            }
        }
        private void RemovePrItems(int assignmentid, int id)
        {
            var record = _db.PurchaseRequisitionAssignments.Find(assignmentid);
            if (record != null)
            {
                foreach (var item in record.PurchaseRequisitionReassignmentItems)
                {

                    if (item.ItemType == PurchaseItemType.FixedAsset)
                    {
                        #region fixed asset from lot item

                        var existingLotItem = _db.LotItems
                            .Where(x => x.LotID == id)
                            .Where(x => x.Name == item.Name)
                            .Where(x => x.ItemSpecification == item.ItemSpecification)//added
                            .FirstOrDefault(x => x.ItmCode == item.ItmCode);
                        if (existingLotItem != null)
                        {
                            decimal qty = existingLotItem.Quantity - item.Quantity;
                            if (qty <= 0)
                            {
                                _db.LotItems.Remove(existingLotItem);
                                _db.SaveChanges();
                            }
                            else
                            {
                                existingLotItem.Quantity = qty;
                                _db.Entry(existingLotItem).State = EntityState.Modified;
                                _db.SaveChanges();
                            }

                        }

                        #endregion
                        #region fixed asset from participant item item

                        var existingPriceItem = _db.BidParticipatingSupplierItems
                            .Where(x => x.BidPParticipatingSupplier.LotID == id)
                            .Where(x => x.Name == item.Name)
                            .FirstOrDefault(x => x.ItmCode == item.ItmCode);
                        if (existingPriceItem != null)
                        {
                            decimal qty = existingPriceItem.Quantity - item.Quantity;
                            if (qty <= 0)
                            {
                                _db.BidParticipatingSupplierItems.Remove(existingPriceItem);
                                _db.SaveChanges();
                            }
                            else
                            {
                                existingPriceItem.Quantity = qty;
                                _db.Entry(existingPriceItem).State = EntityState.Modified;
                                _db.SaveChanges();
                            }

                        }

                        #endregion
                    }
                    else
                    {
                        #region stock and service from lot item

                        var existingLotItem = _db.LotItems.Where(x => x.LotID == id)
                            .Where(x => x.ItemSpecification == item.ItemSpecification)//added
                            .FirstOrDefault(x => x.ItmCode == item.ItmCode);
                        if (existingLotItem != null)
                        {
                            decimal qty = existingLotItem.Quantity - item.Quantity;
                            if (qty <= 0)
                            {
                                _db.LotItems.Remove(existingLotItem);
                                _db.SaveChanges();
                            }
                            else
                            {
                                existingLotItem.Quantity = qty;
                                _db.Entry(existingLotItem).State = EntityState.Modified;
                                _db.SaveChanges();
                            }

                        }

                        #endregion
                        #region stock and service from bid partifipant item

                        var existingPriceItem = _db.BidParticipatingSupplierItems.Where(x => x.BidPParticipatingSupplier.LotID == id)
                            .Where(x => x.ItemSpecification == item.ItemSpecification)//added
                            .FirstOrDefault(x => x.ItmCode == item.ItmCode);
                        if (existingPriceItem != null)
                        {
                            decimal qty = existingPriceItem.Quantity - item.Quantity;
                            if (qty <= 0)
                            {
                                _db.BidParticipatingSupplierItems.Remove(existingPriceItem);
                                _db.SaveChanges();
                            }
                            else
                            {
                                existingPriceItem.Quantity = qty;
                                _db.Entry(existingPriceItem).State = EntityState.Modified;
                                _db.SaveChanges();
                            }

                        }

                        #endregion
                    }
                }
              
            }
        }
        public JsonResult ReloadPRItems(string id)
        {
            if (id != null)
            {
                int did = int.Parse(id);
                var lotPR = _db.LotPurchaseRequisitions.Find(did);
                var prItems = _db.PurchaseRequisitionReassignmentItems
                    .Where(x=>x.PurchaseRequisitionAssignmentId== lotPR.PurchaseRequisitionAssignmentId).ToList();

                var participatingSuppliers = _db.BidPParticipatingSupplierses.Where(x => x.LotID == lotPR.LotID).ToList();
                foreach (var item in prItems)
                {
                    if (item.ItemType == PurchaseItemType.FixedAsset)
                    {
                        #region fixed asset

                        var existingLotItem = _db.LotItems
                            .Where(x => x.LotID == lotPR.LotID)
                            .Where(x => x.Name == item.Name)
                            .Where(x => x.ItmCode == item.ItmCode)
                            .FirstOrDefault(x => x.ModelNo == item.ModelNo);
                        if (existingLotItem == null)
                        {

                            var lotItem = new LotItems
                            {
                                LotID = lotPR.LotID,
                                CatCode = item.CatCode,
                                CategoryName = item.CategoryName,
                                ItmCode = item.ItmCode,
                                ItemName = item.ItemName,
                                Name = item.Name,
                                ModelNo = item.ModelNo,
                                PartNo = item.PartNo,
                                Quantity = item.Quantity,
                                ItemSpecification = item.ItemSpecification,
                                UoM = item.UoM,
                                ItemType = item.ItemType,
                            };

                            _db.LotItems.Add(lotItem);
                            _db.SaveChanges();

                            foreach (var supp in participatingSuppliers)
                            {
                                #region add participant item
                                var entity = new BidParticipatingSupplierItem
                                {
                                    BidPParticipatingSuppliersID = supp.BidPParticipatingSuppliersID,
                                    LotItemID = lotItem.LotItemsID,
                                    CatCode = lotItem.CatCode,
                                    CategoryName = lotItem.CategoryName,
                                    ItmCode = lotItem.ItmCode,
                                    ItemName = lotItem.ItemName,
                                    PartNo = lotItem.PartNo,
                                    Name = lotItem.Name,
                                    ItemSpecification = lotItem.ItemSpecification,
                                    UoM = lotItem.UoM,
                                    ItemType = lotItem.ItemType,
                                    GLTaxRateID = 0,
                                    TaxAmount = 0,
                                    Quantity = lotItem.Quantity,
                                    SubTotal = 0,
                                    UnitPrice = 0,
                                    Total = 0,

                                    IsOutOfStock = false
                                };

                                _db.BidParticipatingSupplierItems.Add(entity);
                                _db.SaveChanges();
                                #endregion
                            }

                           
                        }
                       

                        #endregion
                    }

                    else
                    {
                        #region sock and service

                        var existingLotItem = _db.LotItems
                            .Where(x => x.LotID == lotPR.LotID)
                            .Where(x => x.ItemSpecification == item.ItemSpecification)//added
                            .FirstOrDefault(x => x.ItmCode == item.ItmCode);
                        if (existingLotItem == null)
                        {

                            LotItems lotItem = new LotItems
                            {
                                LotID = lotPR.LotID,
                                CatCode = item.CatCode,
                                CategoryName = item.CategoryName,
                                ItmCode = item.ItmCode,
                                ItemName = item.ItemName,
                                PartNo = item.PartNo,
                                Quantity = item.Quantity,
                                ItemSpecification = item.ItemSpecification,
                                UoM = item.UoM,
                                ItemType = item.ItemType,
                            };


                            _db.LotItems.Add(lotItem);
                            _db.SaveChanges();

                            foreach (var supp in participatingSuppliers)
                            {
                                #region add participant item
                                var entity = new BidParticipatingSupplierItem
                                {
                                    BidPParticipatingSuppliersID = supp.BidPParticipatingSuppliersID,
                                    LotItemID = lotItem.LotItemsID,
                                    CatCode = lotItem.CatCode,
                                    CategoryName = lotItem.CategoryName,
                                    ItmCode = lotItem.ItmCode,
                                    ItemName = lotItem.ItemName,
                                    PartNo = lotItem.PartNo,
                                    Name = lotItem.Name,
                                    ItemSpecification = lotItem.ItemSpecification,
                                    UoM = lotItem.UoM,
                                    ItemType = lotItem.ItemType,
                                    GLTaxRateID = 0,
                                    TaxAmount = 0,
                                    Quantity = lotItem.Quantity,
                                    SubTotal = 0,
                                    UnitPrice = 0,
                                    Total = 0,

                                    IsOutOfStock = false
                                };

                                _db.BidParticipatingSupplierItems.Add(entity);
                                _db.SaveChanges();
                                #endregion
                            }
                        }
                       
                        #endregion
                    }
                }

                var formattedData1 = new
                {
                    Message = "Operation completed.",
                    Status = "OK"
                };
                return Json(formattedData1, JsonRequestBehavior.AllowGet);
            }

            var formattedData = new
            {
                Message = "Please select PR for reload.",
                Status = "NOT_OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);

        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
