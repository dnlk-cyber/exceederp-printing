using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.Proforma;
using ExceedERP.Core.Domain.Procurement.Supplier;
using ExceedERP.Core.Domain.Procurement.Tender;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common.ViewModel;
using ExceedERP.Core.Domain.FixedAsset.Registrations;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Core.Domain.Procurement.Tender.Lot;
using ExceedERP.Helpers.Common;
using Microsoft.AspNet.Identity;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;

namespace ExceedERP.Web.Areas.Inventory.Controllers
{
    public class DataProviderController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();


        public JsonResult GetAllEmployees(string text)
        {
            List<EmployeeVm> list = new List<EmployeeVm>();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var employees = db.Person_Employee
                    .Where(x=>x.IsActive)
                    .Where(x => x.EmployeeCode.ToUpper().Contains(text.ToUpper()) ||
                        x.FirstNameEnglish.ToUpper().Contains(text.ToUpper()) ||
                          x.MiddleNameEnglish.ToUpper().Contains(text.ToUpper()) ||
                            x.LastNameEnglish.ToUpper().Contains(text.ToUpper()) ||
                             (x.FirstName + " " + x.MiddleName + " " + x.LastName).ToUpper().Contains(text.ToUpper()) ||
                              (x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish).ToUpper().Contains(text.ToUpper())
                    )
                    .Select(x => new EmployeeVm
                    {
                        EmployeeId = x.EmployeeId,
                        EmployeeCode = x.EmployeeCode,
                        FirstName = x.FirstName,
                        MiddleName = x.MiddleName,
                        LastName = x.LastName,
                        FirstNameEnglish = x.FirstNameEnglish,
                        MiddleNameEnglish = x.MiddleNameEnglish,
                        LastNameEnglish = x.LastNameEnglish
                    }).ToList();

                list = employees;
            }

            return Json(list.Select(p => new
            {
                EmployeeId = p.EmployeeId,
                FullNameaEnglish = p.EmployeeCode + " - " + p.FirstNameEnglish + " " + p.MiddleNameEnglish + " " + p.LastNameEnglish,
                FullNameAmharic = p.EmployeeCode + " - " + p.FirstName + " " + p.MiddleName + " " + p.LastName,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult getAllInventories()
        {
            var data = db.ItemCategorys.Select(s => new { s.ItemCategoryCode, s.Name }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllStoresByBranch(string BranchId)
        {
            var store = db.InventoryStores.Where(x => x.Enable && x.BranchId.ToString() == BranchId).AsQueryable();
            return Json(store.Select(p => new
            {
                StoreCode = p.StoreCode,
                Name = p.StoreCode + "-" + p.Name
            }), JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetPeriodDateRangeById(int id)
        {

            var glPeriod = db.InventoryPeriods.Find(id);
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
        public JsonResult GetStoreCodeFromGrNo(string id = "")
        {
            int did = int.Parse(id);
            var data = from a in db.GoodReceives
                       where (a.GoodReceiveId.Equals(did))
                       select a;
            var formattedData = data.Select(m => new
            {
                StoreCode = m.StoreCode
            });
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetStoreCodeFromSrNo(string id = "")
        {
            int did = int.Parse(id);
            var data = from a in db.StoreRequisitions
                       where (a.StoreRequisitionID.Equals(did))
                       select a;
            var formattedData = data.Select(m => new
            {
                StoreCode = m.StoreCode
            });
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetItemPartNo(string id = "")
        {
            var data = from a in db.InventoryItems
                       where (a.ItemCode.Equals(id))
                       select a;
            var formattedData = data.Select(m => new
            {
                PartNo = m.PartNo,
                UoM = m.UoM.UoMName
            });
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTendersLot(string tenderId)
        {
            Tender tender = new Tender();
            if (tenderId != null)
            {
                int tenderid = int.Parse(tenderId);
                tender = db.Tenders.Find(tenderid);
            }
            if (tender != null && tender.Lots != null)
            {
                return Json(tender.Lots.Select(x => new { Name = "Lot_" + x.LotID, LotId = x.LotID }), JsonRequestBehavior.AllowGet);

            }
            return null;
        }

        public JsonResult GetSupplierPerLot(string LotId)
        {
            int lotId = int.Parse(LotId);
            Lot lot = db.Lots.Find(lotId);
            var awarded = lot.BidAwardedSupplier.GroupBy(x => x.SupplierName).Select(x => x.FirstOrDefault());
            List<BidPurchasingSuppliers> bidPurchasingSupplierses = new List<BidPurchasingSuppliers>();
            foreach (var par in awarded)
            {
                BidPurchasingSuppliers bidPurchasing = db.BidPurchasingSupplierses.Find(par.SupplierName);
                bidPurchasingSupplierses.Add(bidPurchasing);
            }
            return Json(bidPurchasingSupplierses.Select(x => new { Name = x.SupplierName, Id = x.BidPurchasingSuppliersID }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTenderSuppliers(string refernceCode)
        {
            var suppliers = db.ProcurementSuppliers.Where(x => x.SupplierReference == refernceCode);
            return Json(suppliers.Select(i => new { Name = i.Name, ProcurementSupplierID = i.ProcurementSupplierID }), JsonRequestBehavior.AllowGet);

        }

       

        public JsonResult GetAllStores()
        {
            var store = db.InventoryStores.Where(x => x.Enable).AsQueryable();
            return Json(store.Select(p => new
            {
                StoreCode = p.StoreCode,
                Name = p.StoreCode + "-" + p.Name
            }), JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetStoreByKeeper()
        {
            //List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            //var store = db.InventoryStores.Where(x => mybraches.Contains(x.BranchId.ToString())).Where(x => x.Enable).AsQueryable();

            List<string> userStores = UserInformationHelper.GetUserStores(User.Identity.Name);
            var store = db.InventoryStores.Where(x => userStores.Contains(x.StoreCode.ToString())).Where(x => x.Enable).AsQueryable();

            return Json(store.Select(p => new
            {
                StoreCode = p.StoreCode,
                Name = p.StoreCode + "-" + p.Name
            }), JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetStoreByUserBranchForSelfCare()
        {
            var mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
            var store = db.InventoryStores.Where(x => mybraches.Contains(x.BranchId)).Where(x => x.Enable).AsQueryable();

            return Json(store.Select(p => new
            {
                StoreCode = p.StoreCode,
                Name = p.StoreCode + "-" + p.Name
            }), JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetAllStoreByKeepers()
        {
            ApplicationDbContext app = new ApplicationDbContext();
            var keepers = db.StoreKeeperAssignments.DistinctByField(x => x.StoreKeeper).Select(x=>x.StoreKeeper).ToList();

            var users = app.Users.Where(x => keepers.Contains(x.Id)).ToList();
            return Json(users.Select(p => new
            {
                Code = p.Id,
                Name = p.UserName + "(" + p.FirstName +" "+p.MiddleName + " " +p.LastName + ")"
            }), JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetStoreByBranchForCount(string id)
        {
            int did = int.Parse(id);
            var store = db.InventoryStores.Where(x => x.BranchId == did).AsQueryable();
            return Json(store.Select(p => new
            {
                StoreCode = p.StoreCode,
                Name = p.StoreCode + "-" + p.Name
            }), JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetStoreByBranch(string id,string text)
        {
            int did = int.Parse(id);
            var store = db.InventoryStores.Where(x => x.Enable).Where(x => x.BranchId == did).AsQueryable();

            if (!string.IsNullOrEmpty(text))
                store = store.Where(x => x.StoreCode.Contains(text) || x.Name.Contains(text));

            return Json(store.Select(p => new
            {
                StoreCode = p.StoreCode,
                Name = p.StoreCode + "-" + p.Name
            }), JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetStoreByBranchForSR(string id, string text)
        {
            int did = int.Parse(id);
            var store = db.InventoryStores
                .Where(x => x.Enable) //they shall ask SR but no other transaction
                .Where(x => x.BranchId == did).AsQueryable();

            if (!string.IsNullOrEmpty(text))
                store = store.Where(x => x.StoreCode.Contains(text) || x.Name.Contains(text));

            return Json(store.Select(p => new
            {
                StoreCode = p.StoreCode,
                Name = p.StoreCode + "-" + p.Name
            }), JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetStore()
        {
            var store = db.InventoryStores.Where(x => x.Enable).AsQueryable();
            return Json(store.Select(p => new
            {
                StoreCode = p.StoreCode,
                Name = p.StoreCode + "-" + p.Name
            }), JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetOpenInventoryPeriods()
        {
            var inventoryPeriods = db.InventoryPeriods.Where(x => x.Status == FiscalYearStatus.Open).AsQueryable();
            return Json(inventoryPeriods.Select(p => new
            {
                GLPeriodId = p.InventoryPeriodId,
                Name = p.Name
            }), JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetOpenInventoryPeriodsByStore(string store,string text)
        {
            var periodStatuses = db.InventoryStorePeriodStatuses
                                .Where(x=>x.StoreCode==store)
                                .Where(x => x.Status == FiscalYearStatus.Open)
                                .AsQueryable();

            List<SelectorVm> list = new List<SelectorVm>();

            foreach (var period in periodStatuses)
            {
                var p = new SelectorVm
                {
                    Code = period.InventoryPeriodId.ToString(),
                    Name = GetPeriodName(period)
                };
                list.Add(p);
            }
            return Json(list.Select(p => new
            {
                GLPeriodId =p.Code,
                Name = p.Name
            }), JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetOpenInventoryPeriodsByYear(string year, string text)
        {
           

            List<SelectorVm> list = new List<SelectorVm>();

            if (!string.IsNullOrEmpty(year))
            {
                int did = int.Parse(year);
                List<String> yearPeriods = db.GLPeriods.Where(x => x.GlFiscalYearId == did).Select(x => x.Name).ToList();

                var periodStatuses = db.InventoryStorePeriodStatuses
               .Where(x => x.Status == FiscalYearStatus.Open)
               .DistinctBy(x => x.InventoryPeriodId).AsQueryable();

              
                foreach (var period in periodStatuses)
                {
                    string name = GetPeriodName(period);
                    if (yearPeriods.Contains(name))
                    {
                        var p = new SelectorVm
                        {
                            Code = period.InventoryPeriodId.ToString(),
                            Name = name
                        };
                        list.Add(p);
                    }
                }
            }
            return Json(list.Select(p => new
            {
                GLPeriodId = p.Code,
                Name = p.Name
            }), JsonRequestBehavior.AllowGet);

        }

        private string GetPeriodName(InventoryStorePeriodStatus p)
        {
            return db.InventoryPeriods.Find(p.InventoryPeriodId)?.Name;
        }

        public JsonResult GetFixedAsset(string itemCode, string from)
        {

            var goodReciveItem = db.AssetRegistrations.Where(x => x.Name == itemCode && x.Status == RegistrationStatus.Active);


            return
                Json(
                    goodReciveItem.Select(
                        p =>
                            new
                            {
                                Name = p.Tag,
                                receivedId = p.AssetRegistrationId.ToString()
                            }), JsonRequestBehavior.AllowGet);



        }

        public JsonResult GetGlPeriods()
        {
            var glperiods = db.GLPeriods.AsQueryable();
            return Json(glperiods.Select(e => new
                {
                    GLPeriodId = e.GLPeriodId,
                    Name = e.Name
                }),
                JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetGlYears()
        {
            var glperiods = db.GLFiscalYears.Where(x=>x.Status==FiscalYearStatus.Open).AsQueryable();
            return Json(glperiods.Select(e => new
                {
                    GlFiscalYearId = e.GlFiscalYearId,
                    Name = e.Name
                }),
                JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTax()
        {
            var tax = db.GLTaxRates.Where(x => x.Enable).AsQueryable();
            return Json(tax.Select(p => new
            {
                Name = p.TaxName,
                GLTaxRateID = p.GLTaxRateID
            }),
                JsonRequestBehavior.AllowGet);
        }
       

        public JsonResult GetProformaItemCategory(string id)
        {
            int did = int.Parse(id);
            var itemCategory = db.ProformaItems.Where(x => x.ProformaPurchaseID == did).AsQueryable();
            return Json(itemCategory.Select(p => new
            {
                Name = p.CatCode + "-" + p.CategoryName,
                ItemCategoryCode = p.CatCode
            }),
                JsonRequestBehavior.AllowGet);
        }

     
        public JsonResult GetProformaSuppliers(string id)
        {
            int did = int.Parse(id);
            var items = db.WinningSuppliers.Where(x => x.ProformaPurchaseID == did).DistinctBy(x => x.SupplierName).AsQueryable();
            return Json(items.Select(p => new
            {
                WinningSupplierID = p.WinningSupplierID,
                Supplier = p.SupplierName
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProformaItems(string id, string ItemCategoryCode)
        {
            int did = int.Parse(id);
            var items = db.ProformaItems.Where(x => x.ProformaPurchaseID == did)
                .Where(x => x.CatCode == ItemCategoryCode).AsQueryable();
            return Json(items.Select(p => new
            {
                Name = p.ItmCode + "-" + p.ItemName + "-" + p.Name,
                ItemCode = p.ItmCode + "_" + p.Name
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGenericItemName(string itemtype, string subcatcode, string text)
        {

            //only fixed asset has name
            string code = subcatcode.Split('_')[0];
           
            #region FixedAsset
            if (itemtype == "FixedAsset")
            {
                int did = int.Parse(code);
                var items = db.FixedAssetSubCategoryNames.Where(x => x.FixedAssetSubCategoryID == did).AsQueryable();
                if (!string.IsNullOrEmpty(text))
                {
                    items = items.Where(p => p.Name.ToUpper().Contains(text.ToUpper()));
                }
                return Json(items.Select(i => new
                {
                    Name = i.Name,
                    Code=i.Name

                }), JsonRequestBehavior.AllowGet);
            }
            #endregion
          

            return null;
        }
        public JsonResult GetGenericItem(string itemtype,string catcode, string text)
        {
            string code = catcode.Split('_')[0];
            #region stock
            if (itemtype == "Stock")
            {
                var items = db.InventoryItems.Where(x => x.Enable).Where(x => x.ItemCategoryCode == code).AsQueryable();
                if (!string.IsNullOrEmpty(text))
                {
                    items = items.Where(p =>
                        p.Name.ToUpper().Contains(text.ToUpper()) || p.ItemCode.Contains(text.ToUpper()));
                }
                return Json(items.Select(i => new
                {
                    Name = i.ItemCode + "_" + i.Name,
                    CatCode = i.ItemCode + "_" + i.Name

                }), JsonRequestBehavior.AllowGet);
            }
            #endregion
            #region FixedAsset
            if (itemtype == "FixedAsset")
            {
                var items = db.FixedAssetSubCategories.Where(x => x.CategoryCode == code).AsQueryable();
                if (!string.IsNullOrEmpty(text))
                {
                    items = items.Where(p =>p.SubCategoryName.ToUpper().Contains(text.ToUpper()) || p.SubCategoryCode.Contains(text.ToUpper()));
                }
                return Json(items.Select(i => new
                {
                    Name = i.SubCategoryCode + "_" + i.SubCategoryName,
                    CatCode = i.FixedAssetSubCategoryID + "_" + i.SubCategoryName

                }), JsonRequestBehavior.AllowGet);
            }
            #endregion
            #region Service
            if (itemtype == "Service")
            {
                var items = db.ServiceItems.Where(x => x.Enable).Where(x => x.ServiceCategoryCode == code).AsQueryable();
                if (!string.IsNullOrEmpty(text))
                {
                    items = items.Where(p =>p.Name.ToUpper().Contains(text.ToUpper()) || p.ServiceItemCode.Contains(text.ToUpper()));
                }
                return Json(items.Select(i => new
                {
                    Name = i.ServiceItemCode + "_" + i.Name,
                    CatCode = i.ServiceItemCode + "_" + i.Name

                }), JsonRequestBehavior.AllowGet);
            }
            #endregion

            return null;
        }
        public JsonResult GetGenericItemCategory(string itemtype, string text)
        {
            #region stock
            if (itemtype == "Stock")
            {
                var items = db.ItemCategorys.AsQueryable();
                if (!string.IsNullOrEmpty(text))
                {
                    items = items.Where(p =>
                        p.Name.ToUpper().Contains(text.ToUpper()) || p.ItemCategoryCode.Contains(text.ToUpper()));
                }
                return Json(items.Select(i => new
                {
                    Name = i.ItemCategoryCode + "_" + i.Name,
                    CatCode = i.ItemCategoryCode + "_" + i.Name

                }), JsonRequestBehavior.AllowGet);
            }
            #endregion
            #region FixedAsset
            if (itemtype == "FixedAsset")
            {
                var items = db.FixedAssetCategories.AsQueryable();
                if (!string.IsNullOrEmpty(text))
                {
                    items = items.Where(p =>
                        p.CategoryName.ToUpper().Contains(text.ToUpper()) || p.CategoryCode.Contains(text.ToUpper()));
                }
                return Json(items.Select(i => new
                {
                    Name = i.CategoryCode + "_" + i.CategoryName,
                    CatCode = i.CategoryCode + "_" + i.CategoryName

                }), JsonRequestBehavior.AllowGet);
            }
            #endregion
            #region Service
            if (itemtype == "Service")
            {
                var items = db.ServiceCategories.AsQueryable();
                if (!string.IsNullOrEmpty(text))
                {
                    items = items.Where(p =>
                        p.Name.ToUpper().Contains(text.ToUpper()) || p.ServiceCategoryCode.Contains(text.ToUpper()));
                }
                return Json(items.Select(i => new
                {
                    Name = i.ServiceCategoryCode + "_" + i.Name,
                    CatCode = i.ServiceCategoryCode + "_" + i.Name

                }), JsonRequestBehavior.AllowGet);
            }
            #endregion

            return null;
        }

        class ItemType
        {
            public string Type { get; set; }
        }
        
        public JsonResult GetItemTypes()
        {
            List<ItemType> list=new List<ItemType>();
            list.Add( new ItemType {Type = "Stock"});
            list.Add( new ItemType {Type = "FixedAsset"});
            list.Add( new ItemType {Type = "Service"});

            return Json(list.Select(p => new
                {
                    Name = p.Type
                }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetItemCategoryForItemDef()
        {
            var itemCategory = db.ItemCategorys
                .Where(x => x.HaveParent == false)
                .ToList();
            List<ItemCategory> list = new List<ItemCategory>();

            foreach (var category in itemCategory)
            {
                List<string> roles = db.ItemCategoryUserRoles.Where(x => x.ItemCategoryCode == category.ItemCategoryCode).Select(x => x.RoleName).ToList();
                if (roles.Contains(User.Identity.GetUserId()))
                    list.Add(category);
            }
            return Json(list.Select(p => new
                {
                    Name = p.ItemCategoryCode + "-" + p.Name,
                    ItemCategoryCode = p.ItemCategoryCode
                }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetItemSubCategoryForItemDef(string catcode,string text)
        {
            var itemSubCategory = db.ItemCategorys.Where(x => x.ParentName == catcode).AsQueryable();
            List<ItemCategory> list = new List<ItemCategory>();


            if (!String.IsNullOrEmpty(text))
                itemSubCategory = itemSubCategory.Where(x => x.Name.ToUpper().Contains(text.ToUpper())
                 || x.ItemCategoryCode.ToUpper().Contains(text.ToUpper()));


            foreach (var category in itemSubCategory)
            {
                //List<string> roles = db.ItemCategoryUserRoles.Where(x => x.ItemCategoryCode == category.ItemCategoryCode).Select(x => x.RoleName).ToList();
                //if (roles.Contains(User.Identity.GetUserId()))
                    list.Add(category);
            }
            return Json(list.Select(p => new
                {
                    Name = p.ItemCategoryCode + "-" + p.Name,
                    SubCategoryCode = p.ItemCategoryCode
                }),
                JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetItemCategory()
        {
            var itemCategory = db.ItemCategorys
                //.Where(x => x.HaveParent == false)
                .ToList();//exclude sub categories
            return Json(itemCategory.Select(p => new
                {
                    Name = p.ItemCategoryCode + "-" + p.Name,
                    ItemCategoryCode = p.ItemCategoryCode
                }),
                JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetItemCategoryExcludingFuel()
        {
            var fuelCategoryCodes = new HashSet<string>( db.FuelItemSettings.Select(x => x.ItemCategoryCode));
            var itemCategory = db.ItemCategorys
                .Where(x => fuelCategoryCodes.Contains(x.ItemCategoryCode) == false)
                .ToList();
            return Json(itemCategory.Select(p => new
            {
                Name = p.ItemCategoryCode + "-" + p.Name,
                ItemCategoryCode = p.ItemCategoryCode
            }),
                JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFuelItemCategory()
        {
            var itemCategory = db.FuelItemSettings.DistinctBy(x => x.ItemCategoryCode).AsQueryable();
            return Json(itemCategory.Select(p => new
            {
                Name = p.ItemCategoryCode + "-" + p.ItemCategory.Name,
                ItemCategoryCode = p.ItemCategoryCode
            }),
                JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetManualReceiveTypes()
        {
            var receiveTypes = db.ReceiveTypes.Where(x=>x.Type==GoodReceiveType.Other).AsQueryable();
            return Json(receiveTypes.Select(i => new
            {
                Name = i.Name,
                ReceiveTypeId = i.ReceiveTypeId

            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAutoReceiveTypes()
        {
            var receiveTypes = db.ReceiveTypes.Where(x => x.Type == GoodReceiveType.PurchaseOrder).AsQueryable();
            return Json(receiveTypes.Select(i => new
            {
                Name = i.Name,
                ReceiveTypeId = i.ReceiveTypeId

            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGrnReferenceType(string id)
        {
            int did = int.Parse(id);
           
            var type = db.ReceiveTypes.Find(did);
            if (type != null)
            {
                var receiveType = type.Type;

                switch (receiveType)
                {
                    case GoodReceiveType.PurchaseOrder:

                        var purchaseOrderItems = db.PurchaseOrderItems
                            .Where(x => x.ItemType == PurchaseItemType.Stock)
                            .Where(x => x.PurchaseOrder.IsOnlineApproved)
                            .Where(x => x.PurchaseOrder.IsVoid == false)
                            .Where(x => x.PurchaseOrder.IsFullfilled == false)
                            .ToList()
                            .DistinctByField(x => x.PurchaseOrderID);


                        return Json(purchaseOrderItems.Select(i => new
                        {
                            Name = "PO_" + i.PurchaseOrderID + " SR_" + i.PurchaseOrder.SroreRequisition,
                            referenceId = i.PurchaseOrderID
                        }), JsonRequestBehavior.AllowGet);


                    case GoodReceiveType.Tender:

                        List<BidAwardedSupplierItem> awards = new List<BidAwardedSupplierItem>();
                        var tenders = db.Tenders
                            .Where(x => x.IsOnlineApproved)
                            .Where(x => x.IsVoid).ToList();
                        foreach (var tender in tenders)
                        {
                            var lots = db.Lots.Where(x => x.TenderID == tender.TenderID).ToList();

                            foreach (var lot in lots)
                            {
                                var award = db.BidAwardedSupplierItems
                                    .Where(x => x.BidAwardedSupplier.LotID == lot.LotID)
                                    .Where(x => x.ItemType == PurchaseItemType.Stock)
                                    .ToList()
                                    .DistinctByField(x => x.BidAwardedSupplierID); ;

                                awards.AddRange(award);
                            }
                        }

                        return Json(awards.Select(i => new
                        {
                            Name = "Tender_" + i.BidAwardedSupplier.Lot.Tender.TenderReference + " Lot_" + i.BidAwardedSupplier.Lot.LotNumber + " Award_" + i.BidAwardedSupplier.SupplierName,
                            referenceId = i.BidAwardedSupplierID
                        }), JsonRequestBehavior.AllowGet);

                }

            }
            return null;

        }
        public JsonResult GetReceiveStockRefernences(string id,string text)
        {
            int did = int.Parse(id);

            var type = db.ReceiveTypes.Find(did);
            if (type != null)
            {
                var receiveType = type.Type;

                switch (receiveType)
                {
                    case GoodReceiveType.PurchaseOrder:
                        var purchaseOrderItems = db.PurchaseOrderItems
                            .Where(x => x.ItemType == PurchaseItemType.Stock)
                            .Where(x => x.PurchaseOrder.IsOnlineApproved)
                            .Where(x => x.PurchaseOrder.IsVoid == false)
                            .Where(x => x.PurchaseOrder.IsFullfilled == false)
                            .ToList();

                        if(!String.IsNullOrWhiteSpace(text))
                            purchaseOrderItems= purchaseOrderItems.Where(x=>x.PurchaseOrderID.ToString().Contains(text) 
                                                                            || x.PurchaseOrder.ProformaPurchaseID.ToString().Contains(text) 
                                                                            || x.PurchaseOrder.PurchaseRequisitionID.ToString().Contains(text)
                                                                            || x.PurchaseOrder.SupplierName.ToUpper().Contains(text.ToUpper())).ToList();

                        var listpurchaseOrderItems = purchaseOrderItems
                            .DistinctByField(x => x.PurchaseOrderID);
                           

                        return Json(listpurchaseOrderItems.Select(i => new
                        {
                            Name = "PO_" + i.PurchaseOrderID,//+" SR_" +i.PurchaseOrder.SroreRequisition,
                            referenceId = i.PurchaseOrderID
                        }), JsonRequestBehavior.AllowGet);


                    case GoodReceiveType.Tender:

                        List<BidAwardedSupplierItem> awardItems = new List<BidAwardedSupplierItem>();
                        var tenders = db.Tenders
                            .Where(x => x.IsOnlineApproved)
                            .Where(x=>x.IsVoid==false).ToList();

                        foreach (var tender in tenders)
                        {
                            var lots = db.Lots.Where(x => x.TenderID == tender.TenderID).ToList();
                            
                            foreach (var lot in lots)
                            {
                                var awardItem = db.BidAwardedSupplierItems
                                    .Where(x => x.BidAwardedSupplier.LotID == lot.LotID)
                                    .Where(x => x.ItemType == PurchaseItemType.Stock)
                                    .ToList();

                                if (!String.IsNullOrWhiteSpace(text))
                                    awardItem = awardItem.Where(x => x.BidAwardedSupplierID.ToString().Contains(text)
                                                                     || x.BidAwardedSupplier.SupplierName.Contains(text)).ToList();
                                var listaward = awardItem
                                        .DistinctByField(x => x.BidAwardedSupplierID);

                                //check if lc cost is concluded
                                if (tender.PurchaseCategory == PurchaseCategory.ForeignPurchase)
                                {
                                    foreach (var awarditem in listaward)
                                    {
                                        var foreignpurchase = db.ForeignPurchases.FirstOrDefault(x =>x.PurchaseRef == awarditem.BidAwardedSupplierID);
                                        if(foreignpurchase!=null&&foreignpurchase.IsOnlineApproved&&!foreignpurchase.IsVoid)
                                            awardItems.Add(awarditem);
                                    }
                                    
                                }
                                else 
                                {
                                    awardItems.AddRange(awardItem);
                                }
                            }
                        }

                        return Json(awardItems.Select(i => new
                            {
                                Name = "Tender_" + i.BidAwardedSupplier.Lot.Tender.TenderReference+" Lot_" + i.BidAwardedSupplier.Lot.LotNumber+" Award_" +i.BidAwardedSupplier.SupplierName,
                                referenceId = i.BidAwardedSupplierID
                            }), JsonRequestBehavior.AllowGet);
                        
                }

            }
            return null;

        }
        public JsonResult GetReceiveFixedAssetRefernences(string id, string text)
        {
            int did = int.Parse(id);

            var type = db.ReceiveTypes.Find(did);
            if (type != null)
            {
                var receiveType = type.Type;

                switch (receiveType)
                {
                    case GoodReceiveType.PurchaseOrder:
                        var purchaseOrderItems = db.PurchaseOrderItems
                            .Where(x => x.ItemType == PurchaseItemType.FixedAsset)
                            .Where(x => x.PurchaseOrder.IsOnlineApproved)
                            .Where(x => x.PurchaseOrder.IsVoid == false)
                            .ToList();

                        if (!String.IsNullOrWhiteSpace(text))
                            purchaseOrderItems = purchaseOrderItems.Where(x => x.PurchaseOrderID.ToString().Contains(text)
                                                                             || x.PurchaseOrder.ProformaPurchaseID.ToString().Contains(text)
                                                                             || x.PurchaseOrder.PurchaseRequisitionID.ToString().Contains(text)
                                                                             || x.PurchaseOrder.SupplierName.ToUpper().Contains(text.ToUpper())).ToList();

                        var listpurchaseOrderItems = purchaseOrderItems
                            .DistinctByField(x => x.PurchaseOrderID);


                        return Json(listpurchaseOrderItems.Select(i => new
                        {
                            Name = "PO_" + i.PurchaseOrderID + " SR_" + i.PurchaseOrder.SroreRequisition,
                            referenceId = i.PurchaseOrderID
                        }), JsonRequestBehavior.AllowGet);


                    case GoodReceiveType.Tender:

                        List<BidAwardedSupplierItem> awardItems = new List<BidAwardedSupplierItem>();
                        var tenders = db.Tenders
                            .Where(x => x.IsOnlineApproved)
                            .Where(x => x.IsVoid == false).ToList();

                        foreach (var tender in tenders)
                        {
                            var lots = db.Lots.Where(x => x.TenderID == tender.TenderID).ToList();

                            foreach (var lot in lots)
                            {
                                var awardItem = db.BidAwardedSupplierItems
                                    .Where(x => x.BidAwardedSupplier.LotID == lot.LotID)
                                    .Where(x => x.ItemType == PurchaseItemType.FixedAsset)
                                    .ToList();

                                if (!String.IsNullOrWhiteSpace(text))
                                    awardItem = awardItem.Where(x => x.BidAwardedSupplierID.ToString().Contains(text)
                                                                     || x.BidAwardedSupplier.SupplierName.ToUpper().Contains(text.ToUpper())
                                                                     || x.BidAwardedSupplier.Lot.Tender.TenderReference.ToUpper().Contains(text.ToUpper())).ToList();
                                var listaward = awardItem
                                        .DistinctByField(x => x.BidAwardedSupplierID);

                                //check if lc cost is concluded
                                if (tender.PurchaseCategory == PurchaseCategory.ForeignPurchase)
                                {
                                    foreach (var awarditem in listaward)
                                    {
                                        var foreignpurchase = db.ForeignPurchases.FirstOrDefault(x => x.PurchaseRef == awarditem.BidAwardedSupplierID);
                                        if (foreignpurchase != null && foreignpurchase.IsOnlineApproved && !foreignpurchase.IsVoid)
                                            awardItems.Add(awarditem);
                                    }

                                }
                                else
                                {
                                    awardItems.AddRange(awardItem);
                                }
                            }
                        }

                        return Json(awardItems.Select(i => new
                        {
                            Name = "Tender_" + i.BidAwardedSupplier.Lot.Tender.TenderReference + " Lot_" + i.BidAwardedSupplier.Lot.LotNumber + " Award_" + i.BidAwardedSupplier.SupplierName,
                            referenceId = i.BidAwardedSupplierID
                        }), JsonRequestBehavior.AllowGet);

                }

            }
            return null;

        }
        public JsonResult GetReceiveFixedAssetRefernences1(string id)
        {
            int did = int.Parse(id);

            var type = db.ReceiveTypes.Find(did);
            if (type != null)
            {
                var receiveType = type.Type;

                switch (receiveType)
                {
                    case GoodReceiveType.PurchaseOrder:

                        var purchaseOrderItems = db.PurchaseOrderItems
                            .Where(x => x.ItemType == PurchaseItemType.FixedAsset)
                            .Where(x => x.PurchaseOrder.IsOnlineApproved)
                            .Where(x => x.PurchaseOrder.IsVoid == false)
                            .ToList()
                            .DistinctByField(x => x.PurchaseOrderID);


                        return Json(purchaseOrderItems.Select(i => new
                        {
                            Name = "PO_" + i.PurchaseOrderID,
                            referenceId = i.PurchaseOrderID
                        }), JsonRequestBehavior.AllowGet);


                    case GoodReceiveType.Tender:

                        List<BidAwardedSupplierItem> awardItems = new List<BidAwardedSupplierItem>();
                        var tenders = db.Tenders
                            .Where(x => x.IsOnlineApproved)
                            .Where(x => x.IsVoid == false).ToList();
                        foreach (var tender in tenders)
                        {
                            var lots = db.Lots.Where(x => x.TenderID == tender.TenderID).ToList();

                            foreach (var lot in lots)
                            {
                                var awardItem = db.BidAwardedSupplierItems
                                    .Where(x => x.BidAwardedSupplier.LotID == lot.LotID)
                                    .Where(x => x.ItemType == PurchaseItemType.FixedAsset)
                                    .ToList()
                                    .DistinctByField(x => x.BidAwardedSupplierID); ;

                                //check if lc cost is concluded
                                if (tender.PurchaseCategory == PurchaseCategory.ForeignPurchase)
                                {
                                    foreach (var awarditem in awardItem)
                                    {
                                        var foreignpurchase = db.ForeignPurchases.FirstOrDefault(x => x.PurchaseRef == awarditem.BidAwardedSupplierID);
                                        if (foreignpurchase != null && foreignpurchase.IsOnlineApproved && !foreignpurchase.IsVoid)
                                            awardItems.Add(awarditem);
                                    }

                                }
                                else
                                {
                                    awardItems.AddRange(awardItem);
                                }
                            }
                        }

                        return Json(awardItems.Select(i => new
                        {
                            Name = "Tender_" + i.BidAwardedSupplier.Lot.Tender.TenderReference + " Lot_" + i.BidAwardedSupplier.Lot.LotNumber + " Award_" + i.BidAwardedSupplier.SupplierName,
                            referenceId = i.BidAwardedSupplierID
                        }), JsonRequestBehavior.AllowGet);

                }

            }
            return null;

        }
        public JsonResult GetReceiveServiceRefernences(string id)
        {
            int did = int.Parse(id);

            var type = db.ReceiveTypes.Find(did);
            if (type != null)
            {
                var receiveType = type.Type;

                switch (receiveType)
                {
                    case GoodReceiveType.PurchaseOrder:

                        var purchaseOrderItems = db.PurchaseOrderItems
                            .Where(x => x.ItemType == PurchaseItemType.Service)
                            .Where(x => x.PurchaseOrder.IsOnlineApproved)
                            .Where(x => x.PurchaseOrder.IsVoid == false)
                            .ToList()
                            .DistinctByField(x => x.PurchaseOrderID);


                        return Json(purchaseOrderItems.Select(i => new
                        {
                            Name = "PO_" + i.PurchaseOrderID,
                            referenceId = i.PurchaseOrderID
                        }), JsonRequestBehavior.AllowGet);


                    case GoodReceiveType.Tender:

                        List<BidAwardedSupplierItem> awardItems = new List<BidAwardedSupplierItem>();
                        var tenders = db.Tenders
                            .Where(x => x.IsOnlineApproved)
                            .Where(x => x.IsVoid==false).ToList();
                        foreach (var tender in tenders)
                        {
                            var lots = db.Lots.Where(x => x.TenderID == tender.TenderID).ToList();

                            foreach (var lot in lots)
                            {
                                var awardItem = db.BidAwardedSupplierItems
                                    .Where(x => x.BidAwardedSupplier.LotID == lot.LotID)
                                    .Where(x => x.ItemType == PurchaseItemType.Service)
                                    .ToList()
                                    .DistinctByField(x => x.BidAwardedSupplierID); ;

                                //check if lc cost is concluded
                                if (tender.PurchaseCategory == PurchaseCategory.ForeignPurchase)
                                {
                                    foreach (var awarditem in awardItem)
                                    {
                                        var foreignpurchase = db.ForeignPurchases.FirstOrDefault(x => x.PurchaseRef == awarditem.BidAwardedSupplierID);
                                        if (foreignpurchase != null && foreignpurchase.IsOnlineApproved && !foreignpurchase.IsVoid)
                                            awardItems.Add(awarditem);
                                    }

                                }
                                else
                                {
                                    awardItems.AddRange(awardItem);
                                }
                            }
                        }

                        return Json(awardItems.Select(i => new
                        {
                            Name = "Tender_" + i.BidAwardedSupplier.Lot.Tender.TenderReference + " Lot_" + i.BidAwardedSupplier.Lot.LotNumber + " Award_" + i.BidAwardedSupplier.SupplierName,
                            referenceId = i.BidAwardedSupplierID
                        }), JsonRequestBehavior.AllowGet);

                }

            }
            return null;

        }
        
        public JsonResult GetReceiveSuppliers(string id, string type)
        {
            //int did = int.Parse(id);
            int.TryParse(id, out int did);
            int typed = int.Parse(type);
            var receiveTypes = db.ReceiveTypes.Find(typed);
            if (receiveTypes != null)
            {
                var receiveType = receiveTypes.Type;

                if (receiveType == GoodReceiveType.PurchaseOrder)
                {
                    var orders = db.PurchaseOrders.Where(x => x.PurchaseOrderID == did).AsQueryable();
                    return Json(orders.Select(i => new
                    {
                        Type = "PO",
                        Name = i.SupplierName,
                        SupplierId = i.SupplierId
                    }), JsonRequestBehavior.AllowGet);
                }

                if (receiveType == GoodReceiveType.Tender)
                {
                    var award = db.BidAwardedSuppliers.Where(x => x.Lot.TenderID == did).AsQueryable();
                    return Json(award.Select(i => new
                    {
                        Type = "Tender",
                        Name = i.SupplierName,
                        SupplierId = i.SupplierId
                    }), JsonRequestBehavior.AllowGet);
                }


            }
            return null;
        }

        public JsonResult GetStoreRequisitionData(string id)
        {
            int did = int.Parse(id);
            var storeRequisition = db.StoreRequisitions.Where(x=>x.StoreRequisitionID==did).AsQueryable();

            return Json(storeRequisition.Select(i => new
            {
                
                PlateNo = i.PlateNo,
                StoreCode=i.StoreCode
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTransferStoreCodes(string id)
        {
            int did = int.Parse(id);
           
            var storeRequisition = db.StoreRequisitions.Where(x=>x.StoreRequisitionID==did).ToList();
            return Json(storeRequisition.Select(i => new
            {
                FromStoreCode = i.StoreCode,
                ToStoreCode = i.ToStoreCode
            }), JsonRequestBehavior.AllowGet);
        }


        public JsonResult CheckReceiveType(string type)
        {
            int typed = int.Parse(type);
            var receiveTypes = db.ReceiveTypes.Find(typed);
            if (receiveTypes != null)
            {
                var receiveType = receiveTypes.Type;
               if(receiveTypes.Type==GoodReceiveType.PurchaseOrder||receiveTypes.Type==GoodReceiveType.Tender)
               {
                   var formattedData = new
                    {
                        Type = "Auto"
                    };
                 return Json(formattedData, JsonRequestBehavior.AllowGet);
               }
               else
               {
                   var formattedData2 = new
                   {
                       Type = "Manual"
                   };
                   return Json(formattedData2, JsonRequestBehavior.AllowGet);
               }

            }

            var formattedData1 = new
            {
                Type = "Manual"
            };
            return Json(formattedData1, JsonRequestBehavior.AllowGet);
        }
        public ActionResult VirtualItems_Read([DataSourceRequest] DataSourceRequest request, string ItemCategoryCode, string text)
        {
            var items = GetItemList();
            if (!string.IsNullOrEmpty(ItemCategoryCode))
            {
                List<string> categorylist = new List<string> {ItemCategoryCode};
                var cat = db.ItemCategorys.Where(x => x.ParentName == ItemCategoryCode).Select(x => x.ItemCategoryCode)
                    .ToList();

                if(cat.Any())
                    categorylist.AddRange(cat);

                    items = items.Where(p =>categorylist.Contains(p.ItemCategoryCode)).ToList();
            }
            if (!string.IsNullOrEmpty(text))
            {
                items = items.Where(p => p.Name.ToUpper().Contains(text.ToUpper()) || p.ItemCode.Contains(text.ToUpper())).ToList();
            }

            items = items.Take(100);
            return Json(items.ToDataSourceResult(request));
        }
        public ActionResult Items_ValueMapper(string value)
        {
            var dataItemIndex = -1;
            var data = GetItemList();
            if (value != "")
            {
                var index = 0;
                foreach (var vm in data)
                {
                    if (vm.ItemCode == value)
                    {
                        dataItemIndex = index;
                        break;
                    }

                    index += 1;
                }
            }
            return Json(dataItemIndex, JsonRequestBehavior.AllowGet);
        }


        public class ItemViewModel
        {
            public string ItemCategoryCode { get; set; }
            public string Name { get; set; }
            public string ItemCode { get; set; }
            //public bool Enable { get; set; }
        }

        public IEnumerable<ItemViewModel> GetItemList()
        {

            var items = (List<ItemViewModel>)System.Runtime.Caching.MemoryCache.Default["AllItems"];
            if (items == null)
            {
                items = db.InventoryItems                   
                     .Where(x => x.Enable)
                     .OrderBy(x => x.ItemCode)
                     .Select(x => new ItemViewModel {
                         ItemCode=x.ItemCode,
                         Name=x.Name,
                         ItemCategoryCode=x.ItemCategoryCode
                     })
                     .ToList();
                System.Runtime.Caching.MemoryCache.Default["AllItems"] = items;
            }

            return items.Select(item => new ItemViewModel
            {
                ItemCategoryCode = item.ItemCategoryCode,
                ItemCode = item.ItemCode,
                Name = item.ItemCode + "-" + item.Name,
            });
        }
        public JsonResult GetItems2(string ItemCategoryCode, string text)
        {

            var items = (List<InventoryItem>)System.Runtime.Caching.MemoryCache.Default[ItemCategoryCode];
            if (items == null)
            {
                items = db.InventoryItems.Where(x => x.Enable).Where(i => i.ItemCategoryCode == ItemCategoryCode).OrderBy(x => x.Name).ToList();
                System.Runtime.Caching.MemoryCache.Default[ItemCategoryCode] = items;
            }

            //var items =
            if (!string.IsNullOrEmpty(text))
            {
                items = items.Where(p => p.Name.ToUpper().Contains(text.ToUpper()) || p.ItemCode.Contains(text.ToUpper())).ToList();
            }
            return Json(items.Select(i => new
            {
                Name = i.ItemCode + "-" + i.Name,
                ItemCode = i.ItemCode

            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetItems1(string ItemCategoryCode, string text)
        {
            var items = db.InventoryItems.Where(x => x.Enable).Where(i => i.ItemCategoryCode == ItemCategoryCode).OrderBy(x => x.Name).AsQueryable();
            if (!string.IsNullOrEmpty(text))
            {
                items = items.Where(p => p.Name.ToUpper().Contains(text.ToUpper()) || p.ItemCode.Contains(text.ToUpper()));
            }
            return Json(items.Select(i => new
            {
                Name = i.ItemCode + "-" + i.Name,
                ItemCode = i.ItemCode

            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFuelItems(string ItemCategoryCode, string text)
        {
            var items = db.FuelItemSettings.Where(i => i.ItemCategoryCode == ItemCategoryCode).AsQueryable();
            if (!string.IsNullOrEmpty(text))
            {
                items = items.Where(p => p.InventoryItem.Name.ToUpper().Contains(text.ToUpper()) || p.ItemCode.Contains(text.ToUpper()));
            }
            return Json(items.Select(i => new
            {
                Name = i.ItemCode + "-" + i.InventoryItem.Name,
                ItemCode = i.ItemCode

            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGlAccountCategory(int accountType)
        {
            GlAccountTypes type = (GlAccountTypes)accountType;
            var categories = db.GlChartOfAccountCategories.Where(i => i.AccountType == type).AsQueryable();
            return Json(categories.Select(i => new
            {
                AccountSubType = i.AccountSubType

            }), JsonRequestBehavior.AllowGet);
        }



        public JsonResult GetStoreItems(string storeCode)
        {
            var items = db.StoreItemAssignments.Where(s => s.StoreCode == storeCode).ToList();
            List<InventoryItem> itemsInStore = new List<InventoryItem>();
            var filteredItem = itemsInStore.AsQueryable();
            foreach (var item in items)
            {
                var itemin = db.InventoryItems.Where(x => x.Enable).FirstOrDefault(it => it.ItemCode == item.ItemCode);
                if (itemin != null)
                {
                    itemsInStore.Add(itemin);

                }
            }

            return Json(filteredItem.Select(i => new { ItemCode = i.ItemCode, Name = i.ItemCode + "-" + i.Name }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetStoreItemsByCategory(string storeCode, string ItemCategoryCode)
        {
            var items = db.StoreItemAssignments.Where(s => s.ItemCategoryCode == ItemCategoryCode).Where(s => s.StoreCode == storeCode).ToList();

            return Json(items.Select(i => new
            {
                ItemCode = i.ItemCode,
                Name = i.ItemCode + "-" + i.InventoryItem.Name
            }), JsonRequestBehavior.AllowGet);

        }


        public JsonResult GetVoucherByReference(string type)
        {

            if (type != null)
            {
                GoodReceiveType ReceiveType = (GoodReceiveType)Enum.Parse(typeof(GoodReceiveType), type);

                switch (ReceiveType)
                {
                    case GoodReceiveType.PurchaseOrder:
                        var orders = db.PurchaseOrders.AsQueryable();
                        return Json(orders.Select(i => new { Name = "PO_" + i.PurchaseOrderID, referenceId = i.PurchaseOrderID }), JsonRequestBehavior.AllowGet);
                    case GoodReceiveType.Tender:
                        var tender = db.Tenders.AsQueryable();
                        return Json(tender.Select(i => new { Name = "Tender_" + i.TenderID, referenceId = i.TenderID }), JsonRequestBehavior.AllowGet);
                }
            }



            return null;
        }
        public JsonResult GetSuppliers(string reciveRef)
        {

            if (string.IsNullOrEmpty(reciveRef) && reciveRef != "")
            {
                int proformaId = int.Parse(reciveRef);
                List<ProcurementSupplier> procurementSuppliers = new List<ProcurementSupplier>();
                var supplierlist = db.ParticipatingSuppliers.Where(x => x.ProformaPurchaseID == proformaId);
                foreach (ParticipatingSupplier participatingSupplier in supplierlist)
                {
                    int id = int.Parse(participatingSupplier.SupplierId);//supplier
                    ProcurementSupplier supplier = db.ProcurementSuppliers.Find(id);
                    procurementSuppliers.Add(supplier);
                }
                return Json(procurementSuppliers.Select(i => new { Name = i.Name, ProcurementSupplierID = i.ProcurementSupplierID }), JsonRequestBehavior.AllowGet);

            }
            return null;


        }
        public JsonResult GetGroodReceiveType(string id)
        {
            int typeId = int.Parse(id);
            ReceiveType receiveType = db.ReceiveTypes.Find(typeId);
            if (receiveType != null)
                switch (receiveType.Type)
                {
                    case GoodReceiveType.PurchaseOrder:

                        return Json("Purchase", JsonRequestBehavior.AllowGet);

                    case GoodReceiveType.Tender:
                        return Json("Tender", JsonRequestBehavior.AllowGet);
                    case GoodReceiveType.Other:
                        return Json("Other", JsonRequestBehavior.AllowGet);

                }
            return null;
        }
        public JsonResult GetPurchaseOrder()
        {
            var orders = db.PurchaseOrders.AsQueryable();
            return Json(orders.Select(i => new { PurchaseOrderID = i.PurchaseOrderID }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetReceiveType()
        {
            var orders = db.ReceiveTypes.AsQueryable();
            return Json(orders.Select(i => new { ReceiveTypeId = i.ReceiveTypeId, Name = i.Name }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetItemTransferSent()
        {
            var orders = db.ItemTransfers.Where(x => x.IsVoid == false).AsQueryable();
            return Json(orders.Select(i => new { ItemTransferID = i.ItemTransferID }), JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetPurchaseRequisitionId()
        {
            var purchaseRequisitions = db.PurchaseRequisitions.Where(x => x.IsVoid == false).Where(sto => sto.IsOnlineApproved).AsQueryable();
            return Json(purchaseRequisitions.Select(i => new
            {
                PurchaseRequisitionID = i.PurchaseRequisitionID,
                Remark = i.PurchaseRequisitionID + "-" + i.ReferenceNo + "-" + i.Remark,
            }), JsonRequestBehavior.AllowGet);
        }
        #region store requisition related

        //old not fast
        public JsonResult GetStoreRequisitionId()
        {
            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            var storeReq = db.StoreRequisitions.AsQueryable().Where(x => x.IsVoid == false).Where(x => mybraches.Contains(x.OrgBranchName)).Where(sto => sto.IsFuel == false).Where(sto => sto.IsSecondOnlineApproved).AsQueryable();
            return Json(storeReq.Select(i => new
            {
                StoreRequisitionID = i.StoreRequisitionID,
                Remark = i.StoreRequisitionID + "-" + i.Reference + "-" + i.Remark,
            }), JsonRequestBehavior.AllowGet);
        }
        //old not fast -fuels
        public JsonResult GetfuelRequisitionId()
        {
            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            var storeReq = db.StoreRequisitions.AsQueryable().Where(x => x.IsVoid == false).Where(x => mybraches.Contains(x.OrgBranchName)).Where(sto => sto.IsFuel).Where(x => x.IsTransfer == false).Where(sto => sto.IsSecondOnlineApproved).AsQueryable();
            return Json(storeReq.Select(i => new
                {
                    StoreRequisitionID = i.StoreRequisitionID,
                    Remark = i.StoreRequisitionID + "-" + i.Reference + "-" + i.Remark,
                }),
                JsonRequestBehavior.AllowGet);
        }

        public class StoreRequisitionViewModel
        {
            public int StoreRequisitionID { get; set; }
            public string Remark { get; set; }
            public string Reference { get; set; }
        }

        public IEnumerable<StoreRequisitionViewModel> GetStoreRequisitionListForIssue(bool isfuel)
        {

            List<string> userStores = UserInformationHelper.GetUserStores(User.Identity.Name);

            var items = db.StoreRequisitions
                .Where(x => userStores.Contains(x.StoreCode))
                .Where(sto => sto.IsFuel == isfuel)
                .Where(x => x.IsVoid == false)
                .Where(x => x.IsTransfer == false)
                .Where(sto => sto.IsSecondOnlineApproved).ToList();


            return items.Select(item => new StoreRequisitionViewModel
            {
                StoreRequisitionID = item.StoreRequisitionID,
                Reference = item.Reference,
                Remark = item.StoreRequisitionID + "-" + item.Reference + "-" + item.Remark,
            });
        }
        //non fuel
        public ActionResult VirtualStoreRequisitionId_Read([DataSourceRequest] DataSourceRequest request, string text)
        {

            var items = GetStoreRequisitionListForIssue(false);

            if (!string.IsNullOrEmpty(text))
            {
                items = items.Where(p =>
                    p.StoreRequisitionID.ToString().ToUpper().Contains(text.ToUpper()) ||
                    p.Reference.Contains(text.ToUpper())).AsQueryable();
            }
            return Json(items.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        //non fuel
        public ActionResult StoreRequisitionId_ValueMapper(string value)
        {
            var dataItemIndex = -1;
            var data = GetStoreRequisitionListForIssue(false);

            if (value != "")
            {
                var index = 0;
                foreach (var vm in data)
                {
                    if (vm.StoreRequisitionID.ToString() == value)
                    {
                        dataItemIndex = index;
                        break;
                    }

                    index += 1;
                }
            }
            return Json(dataItemIndex, JsonRequestBehavior.AllowGet);
        }

        // fuel
        public ActionResult VirtualFuelRequisitionId_Read([DataSourceRequest] DataSourceRequest request, string text)
        {

            var items = GetStoreRequisitionListForIssue(true);

            if (!string.IsNullOrEmpty(text))
            {
                items = items.Where(p =>
                    p.StoreRequisitionID.ToString().ToUpper().Contains(text.ToUpper()) ||
                    p.Reference.Contains(text.ToUpper())).AsQueryable();
            }
            return Json(items.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        // fuel
        public ActionResult FuelRequisitionId_ValueMapper(string value)
        {
            var dataItemIndex = -1;
            var data = GetStoreRequisitionListForIssue(true);

            if (value != "")
            {
                var index = 0;
                foreach (var vm in data)
                {
                    if (vm.StoreRequisitionID.ToString() == value)
                    {
                        dataItemIndex = index;
                        break;
                    }

                    index += 1;
                }
            }
            return Json(dataItemIndex, JsonRequestBehavior.AllowGet);
        }
        #endregion



        public JsonResult GetStoreRequisitionIdTransfer()
        {
            var storeReq = db.StoreRequisitions.Where(x => x.IsVoid == false).Where(sto => sto.IsSecondOnlineApproved && sto.IsTransfer).OrderByDescending(x => x.StoreRequisitionID).AsQueryable();
            return Json(storeReq.Select(i => new
                {
                    StoreRequisitionID = i.StoreRequisitionID,
                    Remark = i.StoreRequisitionID + "-" + i.Reference + "-" + i.Remark
                }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTransferSendNumber()
        {
            var itemTransfers = db.ItemTransfers.Where(sto => sto.IsOnlineApproved && !sto.IsVoid && !sto.IsFullfilled).OrderByDescending(x=>x.ItemTransferID).AsQueryable();
            return Json(itemTransfers.Select(i => new
                {
                    ItemTransferID = i.ItemTransferID,
                    Remark = i.ItemTransferID + "-" + i.DocumentRef + "-" + i.Remark
                }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTransferStore(string id)
        {
            int did = int.Parse(id);
            var data = from a in db.ItemTransfers
                where (a.ItemTransferID.Equals(did))
                select a;
            var formattedData = data.Select(m => new
            {
                FromStore = m.FromStore,
                ToStore = m.ToStore
            });
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFuelStoreRequisitionId()
        {
            var storeReq = db.StoreRequisitions.Where(x => x.IsVoid == false).Where(x => x.IsFuel).Where(x => x.IsTransfer == false && !x.IsFullfilled).AsQueryable();
            return Json(storeReq.Select(i => new { StoreRequisitionID = i.StoreRequisitionID }),
                JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetGoodReceiveId()
        {
            var goodReceives = db.GoodReceives.Where(x => x.IsVoid == false).Where(x => x.IsOnlineApproved).AsQueryable();
            return Json(goodReceives.Select(i => new
                {
                    GoodReceiveId = i.GoodReceiveId
                }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAccountCode()
        {
            var glchartofaccountaccount = db.GLChartOfAccountAccounts.AsQueryable();
            return Json(glchartofaccountaccount.Select(i => new { GlchartAccunt = i.Values + "_" + i.Description, GlValue = i.Values }),
                 JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSubAccountCode()
        {
            var subAccountSeg = db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.SubAccount);
            var subAccounts = db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == subAccountSeg.GLSegmentSetupId);
            return Json(subAccounts.Select(i => new { GlchartSubAccunt = i.Values + "_" + i.Description, GlValue = i.Values }),
                 JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCompany()
        {
            var locationSeg = db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.Location);
            var locations =db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == locationSeg.GLSegmentSetupId);
           
            return Json(locations.Select(i => new { Name = i.Values + "_" + i.Description, Value = i.Values }),
                 JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetParents()
        {
            var Itemcat = db.ItemCategorys.AsQueryable();
            return Json(Itemcat.Select(i => new { Name = i.ItemCategoryCode + "_" + i.Name, ItemCategoryCode = i.ItemCategoryCode }),
                JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetCostCenter()
        {
            var costCenter = db.DepartmentCostCenters;
            return Json(costCenter.Select(i => new { Name = i.LocationCode + "-" + i.DepartmentName, code = i.DepartmentCostCenterID }),
             JsonRequestBehavior.AllowGet);
        }

        public JsonResult MapCostCenter()
        {
            var costCenterSeg = db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.CostCenter);
            var costCenters =
                db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == costCenterSeg.GLSegmentSetupId);
            
            return Json(costCenters.Select(i => new { Name = i.Values + "-" + i.Description, code = i.Values }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWorkUnitCostCenters()
        {
            var costCenter = db.MainCompanyStructures;
            return
                Json(
                    costCenter.Select(
                        i => new
                        {
                            Name = i.Name,
                            Code = i.OrgStructureId
                        }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEmployees(string text)
        {
            List<SelectorVm> list = new List<SelectorVm>();

            if (!String.IsNullOrWhiteSpace(text))
            {
               
                var employees = db.Person_Employee
                    .Where(x=>x.IsActive)
                   .Where(x => x.EmployeeCode.ToUpper().Contains(text.ToUpper()) ||
                       x.FirstNameEnglish.ToUpper().Contains(text.ToUpper()) ||
                         x.MiddleNameEnglish.ToUpper().Contains(text.ToUpper()) ||
                           x.LastNameEnglish.ToUpper().Contains(text.ToUpper()) ||
                            (x.FirstName + " " + x.MiddleName + " " + x.LastName).ToUpper().Contains(text.ToUpper()) ||
                             (x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish).ToUpper().Contains(text.ToUpper())
                   ).Select(p => new SelectorVm
                   {
                       Code = p.EmployeeId.ToString(),
                       Name = p.EmployeeCode + " - " + p.FirstNameEnglish + " " + p.MiddleNameEnglish + " " + p.LastNameEnglish
                   }).ToList();

                list = employees;
            }

            return
          Json(
          list.Select(
              i => new
              {
                  Name = i.Name,
                  Code = i.Code
              }), JsonRequestBehavior.AllowGet);


        }

        public JsonResult GetSelfEmployee()
        {
            //int empid = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var employee = db.Person_Employee;//.Where(x => x.EmployeeId == empid);
            return
                Json(
                    employee.Select(
                        i => new { Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish, Code = i.EmployeeId }), JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetStoreKeeperUsers(string storeCode)
        {
            ApplicationDbContext appdb = new ApplicationDbContext();
            var unitOfm = appdb.Users.AsQueryable();
            return Json(unitOfm.Select(i => new
                {
                    Name = i.UserName +"_"+i.FirstName+ " " + i.MiddleName +" " + i.LastName,
                    Code = i.Id
                }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEmployeesByStore(string storeCode)
        {
            //var storebranch=db.InventoryStores.
            var employee = db.Person_Employee;//;.Where(x=>x);
            return
                Json(
                    employee.Select(
                        i => new { Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish, Code = i.EmployeeId }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetUoM()
        {
            var unitOfm = db.UoMs.AsQueryable();
            return Json(unitOfm.Select(i => new
                {
                    UoMName = i.UoMName, Value = i.UoMName 
                })
            , JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPrForProforma()
        {
            var empid = UserInformationHelper.GetUserEmployeeId(User.Identity.Name);
            var purcahseRequisition = db.PurchaseRequisitionAssignments
                .Where(x => x.ProcurementOfficer == empid)
                .Where(x => x.PurchaseRequisition.ProcurementMethod == ProcurementMethod.Proforma)
                .Where(x => x.PurchaseRequisition.IsSecondOnlineApproved)
                .Where(x => x.PurchaseRequisition.IsVoid == false);

           
            return Json(purcahseRequisition.Select(i => new
                {
                    PurchaseRequisitionID = i.PurchaseRequisitionID
                }),
                JsonRequestBehavior.AllowGet);
        }
        

        public JsonResult GetPurchaseRequisiton()
        {
            var purcahseRequisition = db.PurchaseRequisitions.Where(x => x.IsSecondOnlineApproved);
            return Json(purcahseRequisition.Select(i => new { PurchaseRequisitionID = i.PurchaseRequisitionID }),
                JsonRequestBehavior.AllowGet);
        }
        public ActionResult GLAccount()
        {
            return View();
        }
        public ActionResult ClAccount()
        {
            return View();
        }
        public ActionResult ACAccount()
        {
            return View();
        }
        public ActionResult ItemFiltering()
        {
            return View();
        }
        public ActionResult ItemFilterCat()
        {
            return View();
        }

        public JsonResult GetPlateNumbersForFuelRequisition()
        {
            var plateNumbers = db.AssetRegistrations
                .Where(a => string.IsNullOrEmpty(a.PlateNo) == false).Select(p => new { p.PlateNo }).Distinct().ToList();
            return Json(plateNumbers.Select(p => new { p.PlateNo }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPlateNumbersFromFleetForFuelRequisition()
        {
            List<string> plateNums=new List<string>();

            var fixedPlates=db.AssetRegistrations
                .Where(a => string.IsNullOrEmpty(a.PlateNo) == false).Select(a => a.PlateNo).ToList();

            if (fixedPlates.Any())
                plateNums.AddRange(fixedPlates);

            var fleetPlates = db.Fleets
                .Where(a => string.IsNullOrEmpty(a.PlateNumber) == false).Select(a => a.PlateNumber).ToList();
            
            if (fleetPlates.Any())
                plateNums.AddRange(fleetPlates);

            var fleetAssetCode = db.Fleets
                .Where(a => string.IsNullOrEmpty(a.PlateNumber) == true).Select(a => a.UnitNum).ToList();

            if (fleetAssetCode.Any())
                plateNums.AddRange(fleetAssetCode);

            //var plateNumbers = db.Fleets
            //    .Where(a => string.IsNullOrEmpty(a.PlateNumber) == false)
            //    .Select(p => new
            //    { 
            //        p.PlateNumber
            //    })
            //    .Distinct().ToList();
            return Json(plateNums.Distinct().Select(p => new { PlateNo=p }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStoreCounts(string store)
        {
            int maxCount = 0;
            var storeCounts=db.PropertyCounts
                .Where(a => a.StoreCode == store && a.IsClosed).ToList();
            if (storeCounts.Any())
                maxCount = storeCounts.Max(x => x.PropertyCountId);

            var counts = db.PropertyCounts
                .Where(a => a.StoreCode == store && a.PropertyCountId==maxCount)
                .Select(p => new { p.PropertyCountId, p.CountDate })
                .OrderByDescending(p => p.CountDate).ToList();

            return Json(counts.Select(p => new
            { Code = p.PropertyCountId,
                Name = p.CountDate.ToShortDateString()
            }),
            JsonRequestBehavior.AllowGet);
        }
    }
}