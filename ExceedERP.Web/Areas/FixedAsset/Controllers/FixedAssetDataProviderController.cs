using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.FixedAsset.Registrations;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers
{
    public class FixedAssetDataProviderController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();

        

        public ActionResult SupplierShortListSelector()
        {
            return View();
        }

        public JsonResult GetPeriodDateRangeById(int id)
        {

            var glPeriod = db.FixedAssetPeriods.Find(id);
            if (glPeriod != null)
            {
                var formattedData = new
                {
                    Min = glPeriod.DateFrom,
                    Max = glPeriod.DateTo
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);

            }

            return null;
        }

        public JsonResult GetFixedAssetConditions()
        {
            var store = db.FixedAssetConditions.AsQueryable();
            return Json(store.Select(p => new
            {
                FixedAssetConditionId = p.FixedAssetConditionId,
                Name = p.Name
            }), JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetOpenFixedAssetPeriods()
        {
            var store = db.FixedAssetPeriods.Where(x => x.Status == FiscalYearStatus.Open).AsQueryable();
            return Json(store.Select(p => new
            {
                GLPeriodId = p.FixedAssetPeriodId,
                Name = p.Name
            }), JsonRequestBehavior.AllowGet);

        }

        
        public JsonResult GetStoreCodeFromSrNo(string id = "")
        {
            int did = int.Parse(id);
            var data = from a in db.FixedAssetRequisitions
                       where (a.FixedAssetRequisitionId.Equals(did))
                       select a;
            var formattedData = data.Select(m => new
            {
                StoreCode = m.StoreCode
            });
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFixedAssetRequisitionId()
        {
            var fixedAssetRequisitions = db.FixedAssetRequisitions
                .Where(sto => sto.IsMiddleOnlineApproved)
                .Where(sto => sto.IsVoid==false)
                .OrderByDescending(x=>x.FixedAssetRequisitionId).AsQueryable();
            return Json(fixedAssetRequisitions.Select(i => new
            {
                FixedAssetRequisitionId = i.FixedAssetRequisitionId,
                Remark = i.FixedAssetRequisitionId + "-" + i.Reference + "-" + i.Remark,
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFixedAssetForTransfer_unused(string subCategoryCode)
        {
            int id = int.Parse(subCategoryCode);

            var assetRegistrations = db.AssetRegistrations.AsNoTracking().Where(i => i.FixedAssetSubCategoryID == id).AsQueryable();
            return Json(assetRegistrations.Select(i => new
            {
                Name = i.Tag + "-" + i.Name,
                AssetRegistrationId = i.AssetRegistrationId

            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFixedAssetDataForUserCard(string id)
        {
            int did = int.Parse(id);
            var assetRegistrations = db.AssetRegistrations.Where(x=>x.AssetRegistrationId==did).AsQueryable();
            return Json(assetRegistrations.Select(i => new
            {
                Quanity = i.Quanity,
                UnitPrice = i.PurchasePrice,
                ItemName = i.Name,
                Unit = i.Unit,
                Tag = i.Tag,
                SerialNo = i.SerialNo,
                GRNDate = i.GRNDate,
                GRN = i.GRN
            }), JsonRequestBehavior.AllowGet);
         
        }
        public JsonResult GetFixedAssetForUserCard(string text)
        {
            var assetRegistrations = db.AssetRegistrations.AsNoTracking().AsQueryable();
            if (!String.IsNullOrEmpty(text))
                assetRegistrations = assetRegistrations.Where(x => x.Name.ToUpper().Contains(text.ToUpper())
                                                                   || x.Tag.ToUpper().Contains(text.ToUpper()));
            return Json(assetRegistrations.Select(i => new
            {
                Name = i.Tag + "-" + i.Name,
                AssetRegistrationId = i.AssetRegistrationId.ToString()

            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFixedAssetForTransfer(string subCategoryCode, string text)
        {
            int id = int.Parse(subCategoryCode);
            var assetRegistrations = db.AssetRegistrations.AsNoTracking().Where(i => i.FixedAssetSubCategoryID == id).AsQueryable();

            if (!String.IsNullOrEmpty(text))
                assetRegistrations = assetRegistrations.Where(x => x.Tag.ToUpper().Contains(text.ToUpper())
                                                                   || x.Name.ToUpper().Contains(text.ToUpper()))
                    .AsQueryable();

           
            return Json(assetRegistrations.Select(i => new
            {
                Name = i.Tag + "-" + i.Name,
                AssetRegistrationId = i.AssetRegistrationId

            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFixedAssetForTransferNew(string subCategoryCode, string custodianBeforeTransfer, string fromStoreCode, string text)
        {
           // List<AssetRegistration> assetsToTransfer = new List<AssetRegistration>();
            var assetRegistrations = db.AssetRegistrations.AsQueryable();
            int id = 0;
            int custodianBeforeTransferId = 0;

            if (subCategoryCode != null)
            {
                id = int.Parse(subCategoryCode);
                 assetRegistrations = assetRegistrations.Where(i => i.FixedAssetSubCategoryID == id).AsQueryable();
            }

            //if (custodianBeforeTransfer != null)
            //{
            //    custodianBeforeTransferId = int.Parse(custodianBeforeTransfer);
            //    assetRegistrations = assetRegistrations.Where(i => i.EmployeeId == custodianBeforeTransferId.ToString()).AsQueryable();
            //}

            //commented for time being

            //if (string.IsNullOrWhiteSpace(fromStoreCode))
            //{
            //    assetsToTransfer = assetRegistrations.Where(x => x.StoreCode == fromStoreCode).ToList();
            //}

            //if (!string.IsNullOrWhiteSpace(custodianBeforeTransfer))
            //{
            //    var userCards = db.UserCardItems
            //        .Where(x => x.EmployeeId == custodianBeforeTransferId && x.Active == true).ToList();
            //    foreach (var userCard in userCards)
            //    {
            //        foreach (var asset in assetRegistrations)
            //        {
            //            if (userCard.AssetRegistrationId == asset.AssetRegistrationId)
            //            {
            //                assetsToTransfer.Add(asset);
            //            }
            //        }
            //    }
            //}

            

            if (!String.IsNullOrEmpty(text))
                assetRegistrations = assetRegistrations.Where(x => x.Tag.ToUpper().Contains(text.ToUpper())
                                                               || x.Name.ToUpper().Contains(text.ToUpper()))
                    .AsQueryable() ;


            return Json(assetRegistrations.Select(i => new
            {
                Name = i.Tag + "-" + i.Name,
                AssetRegistrationId = i.AssetRegistrationId

            }), JsonRequestBehavior.AllowGet);
        }

        // added new
        /// <summary>
        /// filters and gets FixedAsset from LocationTracking based on FixedAssetTransactionType for IssueItem 
        /// </summary>
        /// <param name="subCategoryCode"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public JsonResult GetFixedAssetForIssueItem(string subCategoryCode, string text)
        {
            var assetsBySubCategories = db.AssetRegistrations
                .Where(x => x.FixedAssetSubCategoryID.ToString() == subCategoryCode).ToList();

            // filters and returns if the 'TransactionType' is 'Store' or 'Return' and 'IsCurrent' is 'true' 
            var locationTrackingList = db.LocationTrackings
                .Where(x => x.LocationType == LocationType.Store && x.IsCurrent).ToList();

            List<AssetRegistration> assetList = new List<AssetRegistration>();

            if (locationTrackingList.Any() && assetsBySubCategories.Any())
            {

                foreach (var locationTracking in locationTrackingList)
                {
                    foreach (var assetByCategory in assetsBySubCategories)
                    {
                        if (locationTracking.AssetRegistrationId == assetByCategory.AssetRegistrationId)
                        {
                            assetList.Add(assetByCategory);
                        }
                    }
                }

                // if filter is applied in the search box filter and return matching a
                if (!string.IsNullOrEmpty(text))
                {
                    assetList = assetList.Where(x => x.Tag.ToLower().Contains(text.ToLower())).ToList();
                }
            }

            return Json(assetList.Select(s => new
            {
                Name = s.Name + "-" + s.Tag,
                AssetRegistrationId = s.AssetRegistrationId
            }), JsonRequestBehavior.AllowGet);


        }
        public JsonResult GetFixedAssetForIssue(string subCategoryCode, string storeCode, string text)
        {
            int id = int.Parse(subCategoryCode);

            var assets = db.LocationTrackings
                    .Where(x => x.TransactionType == FixedAssetTransactionType.UserCard|| x.TransactionType == FixedAssetTransactionType.Issue /*|| x.TransactionType == "User Card"*/)
                    .ToList();

            var assetIds = assets.Select(s => s.AssetRegistrationId);

            var assetRegistrations = db.AssetRegistrations.AsNoTracking().Where(i => i.FixedAssetSubCategoryID == id).AsQueryable();

            assetRegistrations = assetRegistrations.Where(q => !assetIds.Contains(q.AssetRegistrationId));

            if (!String.IsNullOrEmpty(text))
                assetRegistrations = assetRegistrations.Where(x => x.Tag.ToUpper().Contains(text.ToUpper())
                                                                   ||x.Name.ToUpper().Contains(text.ToUpper()))
                    .AsQueryable();

            //foreach (var registration in assetRegistrations)
            //{
            //    var record = db.LocationTrackings
            //        .Where(x => x.AssetRegistrationId == registration.AssetRegistrationId).ToList();

            //    if (record.Any())
            //    {
            //        var maxid = record.Max(x => x.LocationTrackingId);
            //        var locationTracking = db.LocationTrackings.Find(maxid);
            //        if (locationTracking != null && locationTracking.StoreCode != storeCode)
            //            continue;
            //    }
            //}
            return Json(assetRegistrations.Select(i => new
            {
                Name = i.Tag + "-" + i.Name,
                AssetRegistrationId = i.AssetRegistrationId

            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFixedAssetForReturn(string subCategoryCode, string empId, string text)
        {

            int id = int.Parse(subCategoryCode);
            int empid = int.Parse(empId);
            var assetRegistrations = db.AssetRegistrations.Where(i => i.FixedAssetSubCategoryID == id).AsQueryable();
            var assets = db.UserCardItems
                .Where(x=>x.EmployeeId==empid&&x.Active)
                   // .Where(x => x.TransactionType == FixedAssetTransactionType.UserCard || x.TransactionType == FixedAssetTransactionType.Issue /*|| x.TransactionType == "User Card"*/)
                    .ToList();

            var assetIds = assets.Select(s => s.AssetRegistrationId);
            assetRegistrations = assetRegistrations.Where(q => assetIds.Contains(q.AssetRegistrationId));

            //added to filter by employees id
            assetRegistrations = assetRegistrations.Where(x => x.EmployeeId == empId);


            if (!String.IsNullOrEmpty(text))
                assetRegistrations = assetRegistrations.Where(x => x.Tag.ToUpper().Contains(text.ToUpper())
                    || x.Name.ToUpper().Contains(text.ToUpper()))
                    .AsQueryable();

           
            return Json(assetRegistrations.Select(i => new
            {
                Name = i.Tag + "-" + i.Name,
                AssetRegistrationId = i.AssetRegistrationId

            }), JsonRequestBehavior.AllowGet);
        }
        

        public JsonResult GetAssetDetails(string id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                int did = int.Parse(id);
                var assetRegistration = db.AssetRegistrations.Find(did);
                
                var formattedData = new
                {
                    Tag = assetRegistration?.Tag,
                    Size = assetRegistration?.Size,
                    SerialNo = assetRegistration?.SerialNo,
                    Name = assetRegistration?.Name,
                    Quantity = assetRegistration?.Quanity,
                    ItemName = assetRegistration?.Name,//added
                    UnitPrice = assetRegistration?.BookValue,//added
                    Total = assetRegistration?.BookValue,//added
                    
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            else
            {


                var formattedData = new
                {
                    Tag = "",
                    Size = "",
                    SerialNo = "",
                    Name = ""
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetFixedAssetForDisposal(string subCategoryCode)
        {
            int id = int.Parse(subCategoryCode);
            var assetRegistrations = db.AssetRegistrations.Where(i => i.FixedAssetSubCategoryID == id).AsQueryable();
            return Json(assetRegistrations.Select(i => new
            {
                Name = i.Tag + "-" + i.Name,
                AssetRegistrationId = i.AssetRegistrationId

            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFixedAsset(string subCategoryCode)
        {
            int id = int.Parse(subCategoryCode);

            var assetRegistrations = db.AssetRegistrations.Where(i => i.FixedAssetSubCategoryID == id).AsQueryable();
            return Json(assetRegistrations.Select(i => new
            {
                Name = i.Tag + "-" + i.Name,
                AssetRegistrationId = i.AssetRegistrationId

            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCategoryForRul(string groupid)
        {
            int id = int.Parse(groupid);
            var items = db.FixedAssetCategories.Where(i => i.FixedAssetGroupID == id).AsQueryable();
            return Json(items.Select(i => new
            {
                Name = i.CategoryCode + "-" + i.CategoryName,
                CategoryCode = i.CategoryCode

            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSubCategory(string categoryCode,string text)
        {
            int id = int.Parse(categoryCode);
            var category = db.FixedAssetCategories.Find(id);
            var items = db.FixedAssetSubCategories.Where(i => i.CategoryCode == category.CategoryCode).AsQueryable();
            if (!String.IsNullOrWhiteSpace(text))
                items = items.Where(x => x.SubCategoryName.Contains(text));
            return Json(items.Select(i => new
            {
                Name = i.SubCategoryCode + "-" + i.SubCategoryName,
                FixedAssetSubCategoryID = i.FixedAssetSubCategoryID

            }), JsonRequestBehavior.AllowGet);
        }

        //new Added as a replacacement for above "GetSubCategory" for __
        public JsonResult GetSUbCategories(string categoryCode, string text)
        {
            var subSategories = db.FixedAssetSubCategories.Where(x => x.CategoryCode.Equals(categoryCode)).AsQueryable();
            var projectSubCategoryValue = subSategories.Select(i => new
            {
                Name = i.SubCategoryCode + "-" + i.SubCategoryName,
                FixedAssetSubCategoryID = i.FixedAssetSubCategoryID
            });
            return Json(projectSubCategoryValue, JsonRequestBehavior.AllowGet);
        }

        //If subCategoryCode has a none numerical value int.Parse() will throw an execption 
        public JsonResult GetSubCategoryName(string subCategoryCode, string text)
        {
            int id = int.Parse(subCategoryCode);
            var category = db.FixedAssetSubCategories.Find(id);
            var items = db.FixedAssetSubCategoryNames.Where(i => i.FixedAssetSubCategoryID == category.FixedAssetSubCategoryID).AsQueryable();
            if (!String.IsNullOrWhiteSpace(text))
                items = items.Where(x => x.Name.Contains(text));
            return Json(items.Select(i => new
            {
                Name = i.Name,
                FixedAssetSubCategoryNameId = i.FixedAssetSubCategoryNameId

            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCategory()
        {
            var itemCategory = db.FixedAssetCategories.AsQueryable();
            return Json(itemCategory.Select(p => new
            {
                Name = p.CategoryCode + "-" + p.CategoryName,
                FixedAssetCategoryID = p.FixedAssetCategoryID
            }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGroup()
        {
            var itemCategory = db.FixedAssetGroups.AsQueryable();
            return Json(itemCategory.Select(p => new
            {
                Name = p.Name,
                FixedAssetGroupID = p.FixedAssetGroupID
            }),
                JsonRequestBehavior.AllowGet);
        }

       

        public JsonResult GetProformaPurchaseId()
        {
            var proformaPurchases = db.ProformaPurchases.Where(sto => sto.IsOnlineApproved).AsQueryable();
            return Json(proformaPurchases.Select(i => new
            {
                ProformaPurchaseID = i.ProformaPurchaseID,
                Remark = i.ProformaPurchaseID + "-" + i.ProformaReference,
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPurchaseRequisitionId()
        {
            var purchaseRequisitions = db.PurchaseRequisitions.Where(sto => sto.IsOnlineApproved).AsQueryable();
            return Json(purchaseRequisitions.Select(i => new
            {
                PurchaseRequisitionID = i.PurchaseRequisitionID,
                Remark = i.PurchaseRequisitionID + "-" + i.ReferenceNo + "-" + i.Remark,
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSuppliers()
        {

            var suppliers = db.ProcurementSuppliers.Where(x => x.IsForShortList).ToList();


            return Json(suppliers.Select(c => new { value = c.SupplierId, desc = c.SupplierId + "_" + c.Name }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAssetTransferSendIds()
        {

            var suppliers = db.AssetTransfers
                .Where(x => x.IsOnlineApproved)
                .Where(x => x.IsVoid==false)
                .Where(x => x.ConfirmReceive == false)
                .Select(x=> new
                {
                    x.AssetTransferId,
                    x.RequestNumber
                }).ToList();


            return Json(suppliers.Select(c => new
            {
                Code = c.AssetTransferId,
                Name = c.AssetTransferId + "_" + c.RequestNumber
            }), JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetMachineTransferSendIds()
        {

            var list = db.MachineryAndVehicleTransfers
                .Where(x => x.IsOnlineApproved)
                .Where(x => x.IsVoid == false)
                .Select(x => new { x.MachineryAndVehicleTransferId, x.RequestNumber }).ToList();


            return Json(list.Select(c => new
            {
                Code = c.MachineryAndVehicleTransferId,
                Name = c.MachineryAndVehicleTransferId + "_" + c.RequestNumber
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAssetForStockIssue(string text)
        {

            var list = db.AssetRegistrations
               .AsQueryable();

            if (!String.IsNullOrEmpty(text))
                list = list.Where(x => x.PlateNo.Contains(text) || x.Tag.Contains(text)).AsQueryable();

            return Json(list.Select(c => new
            {
                Code = c.AssetRegistrationId,
                Name = c.Tag + "_" + c.PlateNo
            }), JsonRequestBehavior.AllowGet);
        }

        //
        public JsonResult GetAssetData(string id)
        {
            int did = int.Parse(id);
            var asset = db.AssetRegistrations.Find(did);
            if (asset != null)
            {
                var data = new
                {
                    Name = asset.Name,
                    Unit = asset.Unit,
                    Tag = asset.Tag,
                    SerialNo = asset.SerialNo,
                    SIV = asset.SIV,
                    GRN = asset.GRN,
                    GRNDate = asset.GRNDate,
                    BookValue = asset.BookValue,
                    Quanity = asset.Quanity,
                };
                
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

    }
}