using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using System.Data.Entity.Core.Objects;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Web.Areas.CRM.Models;
using Price_Types = ExceedERP.Core.Domain.CRM.Price_Types;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Web.Models;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Helpers.CRM;
using System.Threading.Tasks;
using ExceedERP.Reporting.Fleet.ViewModel;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AllowAnonymous]
    public class CRMDataProviderController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        ApplicationDbContext UserDb = new ApplicationDbContext();
        public static string EmployeeId
        {
            get;
            set;
        }
        public static decimal CalculatedCost
        {
            get;
            set;
        }
        public static decimal SelectedPrice
        {
            get;
            set;
        }
        public static decimal SelectedTaxRate
        {
            get;
            set;
        }
        // GET: Common/DataProvider
        //public ActionResult Index()
        //{
        //    return View();
        //}
        public JsonResult UserDDBind()
        {

            var Parts = UserDb.Users.Select(s => new
            {
                Name = s.FirstName + " " + s.LastName,
                Value = s.Id,
            }).ToList();
            return Json(Parts, JsonRequestBehavior.AllowGet);

        }
        public JsonResult UserNameDDBind()
        {

            var Parts = UserDb.Users.Select(s => new
            {
                Name = s.FirstName + " " + s.LastName,
                Value = s.UserName,
            }).ToList();
            return Json(Parts, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetMembersByGroup(string Group)
        {
            var SupportGroup = db.SupportGroups.Where(x => x.Name == Group).FirstOrDefault();
            if (SupportGroup != null)
            {
                var Memebers = db.SupportGroupDetails.Where(x => x.SupportGroupId == SupportGroup.SupportGroupId).Select(s => new
                {
                    Name = s.Name,
                    Value = s.Name,
                }).ToList();

                return Json(Memebers, JsonRequestBehavior.AllowGet);
            }
            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CustomerTicketDDBind()
        {

            var Tickets = db.CustomerTickets.Select(s => new
            {
                Name = s.Description,
                Value = s.CustomerTicketID,
            }).ToList();
            return Json(Tickets, JsonRequestBehavior.AllowGet);

        }
        public JsonResult PendingCustomerTicketDDBind()
        {

            var Tickets = db.CustomerTickets.Where(x => x.Status != FinalizeType.CLOSED).Select(s => new
            {
                Name = s.Description,
                Value = s.CustomerTicketID,
            }).ToList();
            return Json(Tickets, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GroupDDBind()
        {

            var Parts = db.SupportGroups.Select(s => new
            {
                Name = s.Name,
                Value = s.Name,
            }).ToList();
            return Json(Parts, JsonRequestBehavior.AllowGet);

        }

        public JsonResult PriorityDDBind()
        {

            var Parts = db.PriorityTypes.Select(s => new
            {
                Name = s.Name,
                Value = s.Name,
            }).ToList();
            return Json(Parts, JsonRequestBehavior.AllowGet);

        }
        public JsonResult StatusDDBind()
        {

            var Parts = db.StatusTypes.Select(s => new
            {
                Name = s.Name,
                Value = s.Name,
            }).ToList();
            return Json(Parts, JsonRequestBehavior.AllowGet);

        }
        public JsonResult StatusAwaitingDDBind()
        {

            var StatusAwaiting = db.StatusTypes.Where(x => x.Status == FinalizeType.WAITING).ToList();
            return Json(StatusAwaiting, JsonRequestBehavior.AllowGet);

        }
        public ActionResult ItemCategoryDDBind()
        {

            var ItemCategory = db.ProductCategories.Select(s => new
            {
                Text = s.Description,
                Value = s.ProductCategoryId
            });

            return Json(ItemCategory, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetNationality()
        {
            var countries = db.Countries.AsQueryable();
            return Json(countries.Select(p => new { CountryID = p.CountryId, Nationality = p.Nationality }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPersonCategory()
        {
            var categories = db.PersonCategories.AsQueryable();
            return Json(categories.Select(p => new { PersonCategoryID = p.PersonCategoryID, Description = p.Description }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUOM(string Code)
        {
            var uom = db.InventoryItems.Where(u => u.ItemCode == Code).AsQueryable();
            return Json(uom.Select(s => new { Text = s.UoMName, Value = s.UoM }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLineItemByInvoice(string invoiceId)
        {
            var item = db.LineItems.Where(u => u.VoucherDefinitionID == "2" && u.Voucher == invoiceId).Select(lineItem => new
            {

                LineItemID = lineItem.LineItemID,
                Voucher = lineItem.Voucher,
                VoucherDefinitionID = lineItem.VoucherDefinitionID,
                ThingID = lineItem.ThingID,
                Product = lineItem.Product.Name,
                UnitAmount = lineItem.UnitAmount,
                Quantity = lineItem.Quantity,
                StoreID = lineItem.StoreID,

            }).ToList();
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCustomer()
        {
            var organizationCustomers = db.OrganizationCustomers.AsQueryable();

            return Json(organizationCustomers, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetOrganizationCategory()
        {
            var organizationCategoryList = db.OrganizationCategories.AsQueryable();
            return Json(organizationCategoryList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSpecificationCategory()
        {
            var specificationCategoryList = db.Lookups.Where(sc => sc.LookupType == LookUpTypes.ADDITION_TYPE).AsQueryable();

            return Json(specificationCategoryList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStoreList(int storeId)
        {
            //List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            //var store = db.InventoryStores.Where(x => mybraches.Contains(x.BranchId.ToString())).Where(x => x.Enable).AsQueryable();
            List<int> userStores = db.SalesPersonAssignments.Where(ass => ass.UserName == User.Identity.Name).ToList().Select(u => u.SalesStoreId).ToList();
            //var store = db.InventoryStores.Where(x => userStores.Contains(x.StoreCode.ToString())).Where(x => x.Enable).AsQueryable();

            var storeList = db.SalesStores.Where(s => userStores.Contains(s.SalesStoreId) && s.SalesStoreId == storeId).Select(p => new { StoreCode = p.SalesStoreId, Name = p.Description }).ToList();

            return Json(storeList, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetInventoryStoreList()
        {
            var storeList = db.InventoryStores.Select(p => new { StoreCode = p.StoreCode, Name = p.Name }).ToList();

            return Json(storeList, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetObjectStateDefinitionList()
        {
            var objectStateDefinitionList = db.ObjectStateDefinitions.AsQueryable();

            return Json(objectStateDefinitionList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetOrganizationCustomer()
        {
            var organizationCustomers = db.OrganizationCustomers.AsQueryable();

            return Json(organizationCustomers);
        }
        public JsonResult GetStatusType()
        {
            var StatusTypes = db.StatusTypes.ToList();

            return Json(StatusTypes, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetGroup()
        {
            var Groups = db.SupportGroups.ToList();

            return Json(Groups, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTicketDefintion()
        {
            var TicketActivityDefinition = db.TicketActivityDefinitions.ToList();

            return Json(TicketActivityDefinition, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPriorityType()
        {
            var PriorityTypes = db.PriorityTypes.ToList();

            return Json(PriorityTypes, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBankList()
        {
            var bankList = db.Banks.AsQueryable();

            return Json(bankList.Select(b => new { BankID = b.BankId, TradeName = b.Name }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSerialDefinitionList(string thingId)
        {
            var serialDefinitionList = db.SerialDefinitions.Where(sd => sd.Thing == thingId).AsQueryable();
            return Json(serialDefinitionList.Select(p => new { Thing = p.Thing, Description = p.Description }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLanguage()
        {
            var languages = db.Languages.AsQueryable();
            return Json(languages.Select(p => new { LanguageID = p.LanguageId, Description = p.Description }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCountry()
        {
            var countries = db.Countries.AsQueryable();
            return Json(countries.Select(p => new { CountryID = p.CountryId, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetNation()
        {
            var lookups = db.Lookups.Where(l => l.LookupType == LookUpTypes.NATION).AsQueryable();
            return Json(lookups.Select(p => new { LookupID = p.LookupId, Description = p.Description }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEmployeeList()
        {
            var employeeList = db.Person_Employee.Where(e => e.IsActive == true).AsQueryable();

            return Json(employeeList.Select(p => new { PersonID = p.EmployeeId, Name = p.FirstName + " " + p.MiddleName + " " + p.LastName }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOrgCampaignType()
        {

            var Campaigns = EnumExtension.EnumToSelectList(typeof(CampaignType));

            return Json(Campaigns, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetTeamList()
        {
            var organizationStructure = db.MainCompanyStructures;
            return Json(organizationStructure.Select(os => new { OrgStructureID = os.OrgStructureId, Name = os.Name }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCartList()
        {
            var cartList = db.Carts;
            return Json(cartList.Select(c => new { CartID = c.CartID, Description = c.Description }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCustomerList()
        {

            var organizationCustomer =
                db.OrganizationCustomers.Select(
                    o =>
                        new
                        {
                            CustomerID = o.OrganizationCustomerID,
                            CustomerType = "Organization",
                            CustomerName = o.TradeName
                        });

            var combinedCustomer = organizationCustomer.ToList();
            return Json(combinedCustomer, JsonRequestBehavior.AllowGet);

        }
        public ActionResult GetReference(string referenceType)
        {
            return View();
        }

        public JsonResult GetReferenceList(string referenceType)
        {
            //*****Value 3 refers 
            if (referenceType == "3")
            {
                List<OrganizationCustomer> organizationList = db.OrganizationCustomers.ToList();
                List<Reference> referenceList = organizationList.Select(o => new Reference
                {

                    Code = o.OrganizationCustomerID.ToString(),
                    Description = o.TradeName
                }).ToList();

                return Json(referenceList, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return null;
            }
        }
        public JsonResult GetReferenceType()
        {
            var globalTypes = db.GlobalTypeList.ToList();
            return Json(globalTypes.Select(gt => new { Code = gt.GlobalTypeListID, Description = gt.Description }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetThingCategory()
        {
            var thingCategoryList = db.ProductCategories.ToList();
            List<Reference> referenceList = thingCategoryList.Select(tcategory => new Reference() { Code = tcategory.ProductCategoryId.ToString(), Description = tcategory.Description }).ToList();
            return Json(referenceList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductCategory()
        {
            var categories = db.ProductCategories.Where(c => c.Parent == null).ToList();
            return Json(categories.Select(c => new { c.ProductCategoryId, c.Description }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProductSubCategory(int categoryId)
        {
            var categories = db.ProductCategories.Where(c => c.Parent == categoryId.ToString()).ToList();
            return Json(categories.Select(c => new { c.ProductCategoryId, c.Description }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetItemCategory()
        {
            var CategoryList = db.ItemCategorys.ToList();
            List<ItemCategory> referenceList = CategoryList.Select(tcategory => new ItemCategory() { ItemCategoryCode = tcategory.ItemCategoryCode.ToString(), Description = tcategory.Name }).ToList();
            return Json(referenceList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetItemByCategory(int Category)
        {
            var CRMCategory = db.ProductCategories.Where(x => x.ProductCategoryId == Category).FirstOrDefault();
            var InventoryMaps = db.InventoryCateoryMaps.Where(x => x.Category == CRMCategory.Description).ToList();
            foreach (var InventoryMap in InventoryMaps)
            {
                var InventoryItems = db.InventoryItems.Where(x => x.ItemCategoryCode == InventoryMap.InventoryCategoryId).ToList();
                List<InventoryItem> InventoryItem = InventoryItems.Select(tcategory => new InventoryItem() { Name = tcategory.Name.ToString(), ItemCode = tcategory.ItemCode }).ToList();
                return Json(InventoryItem, JsonRequestBehavior.AllowGet);
            }
            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProductByCategory(int categoryId = 0, int subcategoryId = 0, string filterText = null)
        {
            var subCategory = db.ProductCategories.FirstOrDefault(c => c.ProductCategoryId == subcategoryId);
            var CRMCategory = db.ProductCategories.Where(x => x.ProductCategoryId == categoryId).FirstOrDefault();
            if (CRMCategory == null)
            {
                return null;
            }
            else
            {
                var items = db.Products.Where(x => x.CategoryID == CRMCategory.ProductCategoryId.ToString()).ToList();
                if (subCategory != null)
                {
                    items = items.Where(i => i.SubCategoryId == subCategory.ProductCategoryId.ToString()).ToList();
                }
                if (string.IsNullOrEmpty(filterText) == false)
                {
                    items = items
                        .Where(i => i.Name.ToLower().Contains(filterText.ToLower()) || 
                            i.ItemCode.Contains(filterText)
                        ).ToList();
                }
                return Json(items.Select(i => new { ProductId = i.ProductID, Name= i.Name + " " + i.ItemCode }), JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult GetItemByCRMProductCategory(int categoryId = 0, int subcategoryId = 0, string filterText = null)
        {
            var subCategory = db.ProductCategories.FirstOrDefault(c => c.ProductCategoryId == subcategoryId);
            //var id = subcategoryId == 0 ? categoryId : subcategoryId;
            var CRMCategory = db.ProductCategories.Where(x => x.ProductCategoryId == categoryId).FirstOrDefault();
            //var InventoryMaps = db.InventoryCateoryMaps.Where(x => x.Category == CRMCategory.Description).ToList();
            if (CRMCategory == null)
            {
                return null;
            }
            else
            {
                var inventoryItems = db.InventoryItems.Where(x => x.ItemCategoryCode == CRMCategory.InventoryCategoryCode).ToList();
                if (subCategory != null)
                {
                    inventoryItems = inventoryItems.Where(i => i.SubCategoryCode == subCategory.InventoryCategoryCode).ToList();
                }
                if (string.IsNullOrEmpty(filterText) == false)
                {
                    inventoryItems = inventoryItems.Where(i => i.Name.ToLower().Contains(filterText.ToLower())).ToList();
                }
                return Json(inventoryItems.Select(i => new { i.ItemCode, Name = i.Name + " " +i.ItemCode }), JsonRequestBehavior.AllowGet);
            }

        }
        public JsonResult GetThingList()
        {
            var thingList = db.InventoryItems;
            return Json(thingList.Select(thing => new { ProductID = thing.ItemCode, Name = thing.Name }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetThingListByStore(string StoreID, string filterString = null)
        {
            var SaleStores = db.SalesStores.Where(x => x.SalesStoreId.ToString() == StoreID).ToList();
            var thingList = new List<Product>();
            foreach (var SaleStore in SaleStores)
            {
                string SId = db.InventoryStores.Where(x => x.StoreCode == SaleStore.StoreCode).FirstOrDefault().StoreCode.ToString();
                var stockBalance = db.StoreItemAssignments.Where(p => p.InventoryStore.StoreCode == SId).ToList();


                var products = db.Products.ToList();
                foreach (var sb in stockBalance)
                {

                    var product = products.Where(x => x.ItemCode == sb.ItemCode).FirstOrDefault();
                    if (product != null && product.IsActive)
                    {
                        thingList.Add(product);
                    }

                }
                if (!string.IsNullOrEmpty(filterString))
                {
                    thingList = thingList.Where(l => l.Name.ToLower().Contains(filterString.ToLower()) || l.ItemCode.Contains(filterString)).ToList();
                }
            }
            return Json(thingList.DistinctByField(x => x.ItemCode).Select(thing => new { ProductID = thing.ProductID, Name = thing.Name + "_" + thing.ItemCode }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStockBalance(string storeID, string productID, string VarietyID, string Class)
        {
            try
            {
                string SaleQuoteDefination = "0";
                //string SalesOrderVoucherDefination = "1";
                //string CashInvoiceDefination = "2";
                string productName = productID ?? string.Empty;
                var Store = db.SalesStores.Where(x => x.SalesStoreId.ToString() == storeID).FirstOrDefault();
                var Product = db.Products.Where(p => p.ProductID.ToString() == productID).FirstOrDefault();

                //List<StoreItemAssignment> stockLevel = Store.InventoryStore.StoreItemAssignments.Where(x => x.ItemCode == Product.ItemCode && x.StoreCode == Store.InventoryStore.StoreCode).ToList();
                var storeItemsList = db.StoreItemAssignments.Where(x => x.ItemCode == Product.ItemCode && x.StoreCode == Store.InventoryStore.StoreCode).ToList();

                var notApprovedQuotes = db.SalesQuoteVouchers.Where(s => s.IsOnlineApproved == false).ToList();
                var notApprovedQuoteIds = new HashSet<string>(notApprovedQuotes.Select(q => q.SalesQuoteVoucherID.ToString()));

                //List<LineItem> SalesQuoteItems = db.LineItems.Where(l => (l.ThingID == Product.ProductID && l.VoucherDefinitionID == SaleQuoteDefination && l.StoreID == Store.SalesStoreId && (l.Status == CRMConstants.Pending || l.Status == CRMConstants.Rejected || l.Status == CRMConstants.UnForwarded))).ToList();
                List<LineItem> SalesQuoteItems = db.LineItems.Where(l => l.ThingID == Product.ProductID && l.VoucherDefinitionID == SaleQuoteDefination && l.StoreID == Store.SalesStoreId && notApprovedQuoteIds.Contains(l.Voucher)).ToList();

                var approvedNotShippedInvoices = db.CashSalesInvoices.Where(c => c.IsOnlineApproved == true && c.IsShipped == false).ToList();
                var approvedNotShippedInvoiceIds = new HashSet<string>(approvedNotShippedInvoices.Select(c => c.CashSalesInvoiceID.ToString()));

                List<LineItem> approvedNotShippedItems = db.LineItems.Where(l => l.ThingID == Product.ProductID && l.VoucherDefinitionID == "2" && l.StoreID == Store.SalesStoreId && approvedNotShippedInvoiceIds.Contains(l.Voucher)).ToList();
                //List<LineItem> SalesOrderItems = db.LineItems.Where(l => (l.ThingID == Product.ProductID && l.VoucherDefinitionID == SalesOrderVoucherDefination && l.StoreID == Store.SalesStoreId && l.Status != CRMConstants.ProductIssued)).ToList();
                //List<LineItem> CashInvoiceItems = db.LineItems.Where(l => (l.ThingID == Product.ProductID && l.VoucherDefinitionID == CashInvoiceDefination && l.StoreID == Store.SalesStoreId && l.Status != CRMConstants.ProductIssued)).ToList();


                if (!string.IsNullOrEmpty(VarietyID))
                {
                    SalesQuoteItems = SalesQuoteItems.Where(x => x.Variety == VarietyID).ToList();
                    //SalesOrderItems = SalesOrderItems.Where(x => x.Variety == VarietyID).ToList();
                    //CashInvoiceItems = CashInvoiceItems.Where(x => x.Variety == VarietyID).ToList();
                    approvedNotShippedItems = approvedNotShippedItems.Where(l => l.Variety == VarietyID).ToList();
                }

                if (!string.IsNullOrEmpty(Class))
                {
                    SalesQuoteItems = SalesQuoteItems.Where(x => x.Class == Class).ToList();
                    //SalesOrderItems = SalesOrderItems.Where(x => x.Class == Class).ToList();
                    //CashInvoiceItems = CashInvoiceItems.Where(x => x.Class == Class).ToList();
                    approvedNotShippedItems = approvedNotShippedItems.Where(l => l.Class == Class).ToList();
                }


                decimal quoteQty = SalesQuoteItems.Any() ? SalesQuoteItems.Sum(s => s.Quantity) : 0;
                decimal notShippedInvoiceQty = approvedNotShippedItems.Any() ? approvedNotShippedItems.Sum(l => l.Quantity) : 0;

                if (storeItemsList != null)
                {

                    decimal totalActualBalance = storeItemsList.Sum(s => s.Balance);

                    decimal reserved = quoteQty + notShippedInvoiceQty;

                    return Json(totalActualBalance + "_0.0_" + reserved, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception e)
            { return Json(0.0, JsonRequestBehavior.AllowGet); }
            return Json(0.0, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetTaxList()
        {
            var taxList = db.GLTaxRates.Where(x => x.ApplyToAr == true && x.Enable == true).ToList();
            return Json(taxList.Select(t => new { t.GLTaxRateID, t.TaxName }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetWithholdingTaxList()
        {
            var taxList = db.GLTaxRates.Where(x => x.ApplyToWithholding == true && x.Enable == true).ToList();
            return Json(taxList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAdditionalCost()
        {
            var AdditionalCost = db.AdditionalCosts.ToList();
            return Json(AdditionalCost, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLineAdditionalCost(int Id)
        {
            //var AdditionalCost = db.AdditionalCostLogs.Where(x => x.LineItemId == Id).Select(s => new { AdditionalCostId = s.AdditionalCostId, Name = s.AdditionalCost.Name, Value = s.Value }).ToList();
            return Json(null, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetInvoiceDetail(int InvoiceID)
        {
            var invoice = db.CashSalesInvoices.Where(x => x.CashSalesInvoiceID == InvoiceID).FirstOrDefault();
            if (invoice != null)
            {
                return Json(invoice.OrganizationCustomerId + "_" + invoice.RecieveMethod, JsonRequestBehavior.AllowGet);
            }
            return Json("o_o", JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPriceList(string productID)
        {
            var priceList = db.Prices.Where(p => p.Thing == productID).ToList();
            return Json(priceList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPriceValue(string priceTagID, string productID, string VarietyID, string ClassID)
        //public JsonResult GetPriceValue(string priceTagID, string priceTagDetailID, string productID, string VarietyID, string ClassID)
        {

            var productnumId = db.Products.Where(x => x.ProductID.ToString() == productID).FirstOrDefault().ProductID.ToString();
            var priceValue = db.Prices.Where(p => (p.ReferenceType == priceTagID && p.Thing == productnumId) || (p.ReferenceType == priceTagID && p.Thing == productnumId && p.Variety == VarietyID)).ToList();
            var DefaultpriceValue = db.Prices.Where(p => (p.IsDefault == true && p.Thing == productnumId && p.Variety == VarietyID) || (p.IsDefault == true && p.Thing == productnumId)).FirstOrDefault();

            decimal price = 0;
            if (priceValue.Any())
            {

                var classPrice = priceValue.Where(x => x.Class == ClassID).FirstOrDefault();
                if (classPrice != null)
                {
                    price = classPrice.Value;
                }
                else
                {
                    price = priceValue.FirstOrDefault().Value;

                }
                return Json(price, JsonRequestBehavior.AllowGet);
            }
            else if (DefaultpriceValue != null)
            {

                return Json(DefaultpriceValue.Value, JsonRequestBehavior.AllowGet);
            }
            else
            {

                return Json(0.0, JsonRequestBehavior.AllowGet);
            }


        }


        public JsonResult GetTaxValue(string taxID)
        {
            var taxValue = db.GLTaxRates.Where(t => t.GLTaxRateID.ToString() == taxID.ToString()).FirstOrDefault();
            if (taxValue != null)
            {
                return Json(taxValue.Rate, JsonRequestBehavior.AllowGet);
            }
            else
            {
                SelectedTaxRate = 0;
                return Json(0.0, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetVoucherTermDefinition()
        {
            return null;
        }

        public JsonResult GetPriceReference(string referenceTypeCode)
        {
            List<Reference> referenceList = new List<Reference>();
            if (referenceTypeCode == Price_Types.ON_HOLIDAY.ToString())
            {
                List<HolidayDefinition> holidayDefinitionList = db.HolidayDefinitions.ToList();
                referenceList = holidayDefinitionList.Select(hd => new Reference() { Code = hd.HolidayDefinitionID.ToString(), Description = hd.Description }).ToList();
            }
            else if (referenceTypeCode == Price_Types.LOCATION_PRICE.ToString())
            {
                List<SubCountry> subCountryList = db.SubCountries.ToList();
                referenceList = subCountryList.Select(location => new Reference() { Code = location.SubCountryID.ToString(), Description = location.Name }).ToList();
            }

            return Json(referenceList.Select(rl => new { Reference = rl.Code, Description = rl.Description }), JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetAccountList()
        {
            var accountList = db.Accounts.ToList();
            return Json(accountList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetControlAccountList()
        {
            var controlAccountList = db.ControlAccounts.ToList();
            return Json(controlAccountList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSubCountry()
        {
            var subCountry = db.SubCountries.ToList();
            return Json(subCountry, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetHolidayDefinition()
        {
            var holidayDefinition = db.HolidayDefinitions.ToList();
            return Json(holidayDefinition, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetVoucherExtension()
        {
            var voucherExtensiontList = db.VoucherExtensions.ToList();
            return Json(voucherExtensiontList, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetPriceTagDetailList(string priceTag)
        //{
        //    List<Reference> priceDetailList = null;

        //    if (priceTag == "ON_HOLIDAY")
        //    {
        //        priceDetailList = db.CRMHolidays.Select(hd => new Reference() { Code = hd.CRMHolidayID.ToString(), Description = hd.HolidayDefinition.Description }).ToList();

        //    }
        //    else if (priceTag == "LOCATION_PRICE")
        //    {
        //        priceDetailList = db.SubCountries.Select(sc => new Reference() { Code = sc.SubCountryID.ToString(), Description = sc.Name }).ToList();


        //    }
        //    else
        //    {
        //        priceDetailList = new List<Reference>();
        //        priceDetailList.Add(new Reference() { Code = "1001", Description = priceTag });

        //    }
        //    return Json(priceDetailList, JsonRequestBehavior.AllowGet);

        //}
        public JsonResult GetPriceTagDetailListVoucher(string priceTag)
        {
            List<Reference> priceDetailList = null;
            DateTime dt = DateTime.Today.Date;

            if (priceTag == "LOCATION_PRICE")
            {
                priceDetailList = db.SubCountries.Select(sc => new Reference() { Code = sc.SubCountryID.ToString(), Description = sc.Name }).ToList();


            }
            else if (priceTag == "ON_HOLIDAY")
            {

                priceDetailList = db.CRMHolidays.Where(hd => dt >= EntityFunctions.TruncateTime(hd.Period.StartDate) && dt <= EntityFunctions.TruncateTime(hd.Period.EndDate)).Select(hd => new Reference() { Code = hd.CRMHolidayID.ToString(), Description = hd.HolidayDefinition.Description }).ToList();

            }
            else
            {
                priceDetailList = new List<Reference>();
                priceDetailList.Add(new Reference() { Code = "1001", Description = priceTag });

            }
            return Json(priceDetailList, JsonRequestBehavior.AllowGet);

        }


        public JsonResult GetOpenCRMPeriods()
        {
            var inventoryPeriods = db.CRMPeriods.Where(x => x.Status == FiscalYearStatus.Open).AsQueryable();
            return Json(inventoryPeriods.Select(p => new
            {
                CRMPeriodId = p.CRMPeriodId,
                Name = p.Name
            }), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Gets Open periods that are valid for creating a sales order from  given quote.
        /// the period of the referenced sales quote is considered
        /// </summary>
        /// <param name="quoteId"></param>
        /// <returns></returns>
        public JsonResult GetOpenCRMPeriodsWithinSalesQuoteRange(int quoteId = 0)
        {
            //List<CRMPeriod> listToReturn = new List<CRMPeriod>();
            var periods = db.CRMPeriods.Where(x => x.Status == FiscalYearStatus.Open).ToList();

            var currentQuote = db.SalesQuoteVouchers.Find(quoteId);
            if (currentQuote != null)
            {
                var currentPeriod = db.CRMPeriods.Find(currentQuote.CRMPeriodId);
                if (currentPeriod != null)
                {
                    periods = periods.Where(x => x.DateFrom >= currentPeriod.DateFrom).ToList();
                }
            }
            //listToReturn.AddRange(periods);
            return Json(periods.Select(p => new
            {
                CRMPeriodId = p.CRMPeriodId,
                Name = p.Name
            }), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Comma delimted ids are passed from the multi-selector widget
        /// </summary>
        /// <param name="concatnatedOrderIds"></param>
        /// <returns></returns>
        public JsonResult GetOpenCRMPeriodsWithinSalesOrderRange(string concatnatedOrderIds)
        {
            //List<CRMPeriod> listToReturn = new List<CRMPeriod>();

            var periods = db.CRMPeriods.Where(x => x.Status == FiscalYearStatus.Open).ToList();
            List<SalesOrderVoucher> referencedOrderVouchers = new List<SalesOrderVoucher>();
            if (!string.IsNullOrEmpty(concatnatedOrderIds))
            {
                var idsArray = concatnatedOrderIds.Split(',');
                if (idsArray.Length > 0)
                {
                    for (int i = 0; i < idsArray.Length; i++)
                    {
                        int.TryParse(idsArray[i], out int targetId);
                        var orderRecord = db.SalesOrderVouchers.Find(targetId);
                        if (orderRecord != null)
                        {
                            referencedOrderVouchers.Add(orderRecord);
                        }
                    }
                }
            }
            if (referencedOrderVouchers.Any())
            {
                var arrangedOrders = referencedOrderVouchers.OrderBy(o => o.Date).ToArray();

                var leastPeriod = db.CRMPeriods.Find(arrangedOrders[0].CRMPeriodId);
                if (leastPeriod != null)
                {
                    periods = periods.Where(x => x.DateFrom >= leastPeriod.DateFrom).ToList();
                }
            }
            //listToReturn.AddRange(periods);
            return Json(periods.Select(p => new
            {
                CRMPeriodId = p.CRMPeriodId,
                Name = p.Name
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetOpenCRMPeriodsWithinSingleSalesOrderRange(int orderId = 0)
        {
            //List<CRMPeriod> listToReturn = new List<CRMPeriod>();
            var periods = db.CRMPeriods.Where(x => x.Status == FiscalYearStatus.Open).ToList();

            var currentOrder = db.SalesOrderVouchers.Find(orderId);
            if (currentOrder != null)
            {
                var currentPeriod = db.CRMPeriods.Find(currentOrder.CRMPeriodId);
                if (currentPeriod != null)
                {
                    periods = periods.Where(x => x.DateFrom >= currentPeriod.DateFrom).ToList();
                }
            }
            //listToReturn.AddRange(periods);
            return Json(periods.Select(p => new
            {
                CRMPeriodId = p.CRMPeriodId,
                Name = p.Name
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetOpenCRMPeriodsWithinVouchersReferencedByShipment(int shipmentSource = -1, int invoiceId = 0, int salesOrderId = 0)
        {
            //List<CRMPeriod> listToReturn = new List<CRMPeriod>();
            var periods = db.CRMPeriods.Where(x => x.Status == FiscalYearStatus.Open).ToList();

            ShipmentSource source = (ShipmentSource)shipmentSource;
            if (source == ShipmentSource.SalesInvoice)
            {
                var currentInvoice = db.CashSalesInvoices.Find(invoiceId);
                if (currentInvoice != null)
                {
                    var currentPeriod = db.CRMPeriods.Find(currentInvoice.CRMPeriodId);
                    if (currentPeriod != null)
                    {
                        periods = periods.Where(x => x.DateFrom >= currentPeriod.DateFrom).ToList();
                    }
                }
            }
            else if (source == ShipmentSource.SalesOrder)
            {
                var currentSalesOrder = db.SalesOrderVouchers.Find(salesOrderId);

                var currentPeriod = db.CRMPeriods.Find(currentSalesOrder.CRMPeriodId);
                if (currentPeriod != null)
                {
                    periods = periods.Where(x => x.DateFrom >= currentPeriod.DateFrom).ToList();
                }
            }
            //var currentQuote = db.SalesQuoteVouchers.Find(quoteId);
            //if (currentQuote != null)
            //{

            //}
            //listToReturn.AddRange(periods);
            return Json(periods.Select(p => new
            {
                CRMPeriodId = p.CRMPeriodId,
                Name = p.Name
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllBranches()
        {
            var branches = db.Branch.AsQueryable();
            return Json(branches.Select(b => new
            {
                b.BranchId,
                b.Name,
                b.Abbreviation
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetInventoryStoresInBranch(int branchId)
        {
            var stores = db.InventoryStores.Where(s => s.BranchId == branchId);
            return Json(stores.Select(s => new { s.StoreCode, s.Name }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetBranches()
        {
            var userBranches = new ApplicationBranchManager().GetUsersBranchByName(User.Identity.Name);

            //var branches = db.Branch.AsQueryable();
            return Json(userBranches.Select(b => new
            {
                b.BranchId,
                b.Name,
                b.Abbreviation
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetStoreInBranch(int branchId)
        {
            //var userBranches = new ApplicationBranchManager().GetUsersBranchByName(User.Identity.Name);

            var stores = db.SalesStores.Where(s => s.InventoryStore.BranchId == branchId);
            return Json(stores.Select(b => new
            {
                b.Description,
                b.SalesStoreId
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPaymentTerms()
        {
            var terms = db.PaymentTerms.AsQueryable();
            return Json(terms.Select(b => new
            {
                b.PaymentTermsID,
                PaymentTermsDays = b.PaymentTermsDays + " Days"
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSalesInvoiceNumbers()
        {
            var helper = new CRMSalesHelper();
            var invoices = db.CashSalesInvoices.AsQueryable();
            List<CashSalesInvoice> listToReturn = new List<CashSalesInvoice>();
            if (invoices.Any())
            {
                foreach (CashSalesInvoice invoice in invoices)
                {
                    if (helper.SalesInvoiceHasUnShipedLineLitems(invoice, "2"))
                    {
                        listToReturn.Add(invoice);
                    }
                }
            }
            return Json(listToReturn.Select(b => new
            {
                b.CashSalesInvoiceID,
                InvoiceNumberDisplay = b.CashSalesInvoiceID + " - " + b.OrganizationCustomer.TradeName
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSalesQuoteNumber(int source, string filterString, int customerId = 0)
        {
            if ((SalesOrderSource)source == SalesOrderSource.Manual)
                //return Json(new { SalesQuoteVoucherID = CRMConstants.NoReference});
                return null;
            else
            {
                //var quotes = db.SalesQuoteVouchers.Where(sqv => sqv.IsVoid == false && sqv.LastObjectState != CRMConstants.Forwarded).ToList();
                var quotes = db.SalesQuoteVouchers
                    .Where(sqv => /*sqv.IsVoid == false &&*/
                        sqv.OrganizationCustomerId == customerId &&
                        sqv.IsOnlineApproved)
                        .ToList();
                List<SalesQuoteVoucher> tempList = new List<SalesQuoteVoucher>();
                tempList.AddRange(quotes);
                foreach (var q in tempList)
                {
                    var ordersOfCurrentQuote =
                        db.SalesOrderVouchers.Where
                        (o => o.Source == SalesOrderSource.SalesQuote &&
                                    o.References == q.SalesQuoteVoucherID.ToString()).ToList();
                    //(o.LastObjectState == CRMConstants.Forwarded || o.LastObjectState == CRMConstants.Approved)).ToList();

                    if (ordersOfCurrentQuote.Any())
                    {
                        quotes.Remove(q);
                    }
                }
                if (!string.IsNullOrEmpty(filterString))
                {
                    quotes = quotes.Where(q => q.SalesQuoteVoucherID.ToString().Contains(filterString)).ToList();
                }
                return Json(quotes.Select(q => new { q.SalesQuoteVoucherID }), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetSalesOrderNumber(int source, string filterString, int customerId = 0)
        {
            if ((SalesInvoiceSource)source == SalesInvoiceSource.Manual)
                //return Json(new { SalesQuoteVoucherID = CRMConstants.NoReference});
                return null;
            else
            {
                //var quotes = db.SalesOrderVouchers.Where(sqv => sqv.IsVoid == false && sqv.LastObjectState != CRMConstants.Forwarded).ToList();
                var orders = db.SalesOrderVouchers
                    .Where(sqv =>
                        sqv.PaymentMethod == CRMConstants.CashPaymentMethod &&
                        //sqv.IsVoid == false &&
                        sqv.OrganizationCustomerId == customerId &&
                        sqv.IsOnlineApproved
                        ).ToList();
                List<SalesOrderVoucher> tempList = new List<SalesOrderVoucher>();
                tempList.AddRange(orders);
                foreach (var q in tempList)
                {
                    var invoiceOfCurrentOrder =
                        db.CashSalesInvoices.Where
                        (o => o.Source == SalesInvoiceSource.SalesOrder &&
                                    o.References == q.SalesOrderVoucherID.ToString()).ToList();

                    if (invoiceOfCurrentOrder.Any())
                    {
                        orders.Remove(q);
                    }
                }
                if (!string.IsNullOrEmpty(filterString))
                {
                    orders = orders.Where(q => q.SalesOrderVoucherID.ToString().Contains(filterString)).ToList();
                }
                return Json(orders.Select(q => new { q.SalesOrderVoucherID }), JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// Gets Invoice Numbers for shipment reference
        /// </summary>
        /// <param name="shipmentSource"></param>
        /// <param name="filterString"></param>
        /// <returns></returns>
        public JsonResult GetShipmentInvoiceNumbers(int shipmentSource, string filterString, int customerId = 0)
        {
            if ((ShipmentSource)shipmentSource == ShipmentSource.SalesInvoice)
            {
                var helper = new CRMSalesHelper();
                var invoices =
                    db.CashSalesInvoices.Where(s => /*s.OrganizationCustomerId == customerId &&*/
                    //s.IsVoid == false &&
                    s.IsShipped == false &&
                    s.ShipmentGenerationMethod == ShipmentGenerationMethod.Manual &&
                    s.PaymentMethod == CRMConstants.CashPaymentMethod &&
                    s.IsOnlineApproved
                    //(s.LastObjectState == CRMConstants.Forwarded || s.LastObjectState == CRMConstants.PaymentReceived || s.LastObjectState == CRMConstants.HoldSalesInvoiceStatus || s.LastObjectState == CRMConstants.PrintedSalesInvoiceStatus)
                    );
                if (customerId != 0)
                {
                    invoices = invoices.Where(i => i.OrganizationCustomerId == customerId);
                }
                if (!string.IsNullOrEmpty(filterString))
                {
                    invoices = invoices.Where(q => q.CashSalesInvoiceID.ToString().Contains(filterString));
                }
                //List<CashSalesInvoice> listToReturn = new List<CashSalesInvoice>();
                //if (invoices.Any())
                //{
                //    foreach (CashSalesInvoice invoice in invoices)
                //    {
                //        if (helper.SalesInvoiceHasUnShipedLineLitems(invoice, "2"))
                //        {
                //            listToReturn.Add(invoice);
                //        }
                //    }
                //}
                return Json(invoices.Select(b => new
                {
                    b.CashSalesInvoiceID,
                    InvoiceNumberDisplay = b.CashSalesInvoiceID
                }), JsonRequestBehavior.AllowGet);
            }
            else return null;
        }
        /// <summary>
        /// Gets sales order voucher numbers for shipment reference
        /// </summary>
        /// <param name="shipmentSource"></param>
        /// <param name="filterString"></param>
        /// <returns></returns>
        public JsonResult GetShipmentSalesOrderNumbers(int shipmentSource, string filterString, int customerId = 0)
        {
            var helper = new CRMSalesHelper();

            if ((ShipmentSource)shipmentSource == ShipmentSource.SalesInvoice)
            {
                return null;
            }
            var orders = db.SalesOrderVouchers.Where(s =>
                //s.OrganizationCustomerId == customerId && 
                //s.IsVoid == false &&
                s.IsShipped == false &&
                //s.LastObjectState == CRMConstants.Forwarded &&
                s.IsOnlineApproved &&
                s.PaymentMethod == CRMConstants.CreditPaymentMethod).ToList();
            if (customerId != 0)
            {
                orders = orders.Where(o => o.OrganizationCustomerId == customerId).ToList();
            }
            if (!string.IsNullOrEmpty(filterString))
            {
                orders = orders.Where(q => q.SalesOrderVoucherID.ToString().Contains(filterString)).ToList();
            }
            //List<SalesOrderVoucher> listToReturn = new List<SalesOrderVoucher>();
            //if (orders.Any())
            //{
            //    foreach (SalesOrderVoucher order in orders)
            //    {
            //        if (helper.SalesOrderVoucherHasUnShipedLineLitems(order, "1"))
            //        {
            //            listToReturn.Add(order);
            //        }
            //    }
            //}
            return Json(orders.Select(q => new { q.SalesOrderVoucherID }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetSalesOrderNumberForMultiSelect(int source = -1, int customerId = 0, bool includeNotShipedOrders = false)
        {
            var helper = new CRMSalesHelper();
            //var quotes = db.SalesOrderVouchers.Where(sqv => sqv.IsVoid == false && sqv.LastObjectState != CRMConstants.Forwarded).ToList();
            if ((SalesInvoiceSource)source == SalesInvoiceSource.Manual) { return null; }
            var orders =
                db.SalesOrderVouchers
                .Where(sqv => sqv.PaymentMethod == CRMConstants.CreditPaymentMethod &&
                                //sqv.IsVoid == false &&
                                sqv.IsOnlineApproved &&
                                sqv.OrganizationCustomerId == customerId
                                ).ToList();
            List<SalesOrderVoucher> tempList = new List<SalesOrderVoucher>();
            tempList.AddRange(orders);
            foreach (var q in tempList)
            {
                var invoicesOfCurrentOrder =
                    db.CashSalesInvoices.Where
                    (o => o.Source == SalesInvoiceSource.SalesOrder &&
                                o.References == q.SalesOrderVoucherID.ToString()
                                //&&
                                //(o.LastObjectState == CRMConstants.Forwarded || o.LastObjectState == CRMConstants.Approved)
                                ).ToList();
                if (!includeNotShipedOrders)
                {
                    if (invoicesOfCurrentOrder.Any() || helper.SalesOrderVoucherHasUnShipedLineLitems(q, "1"))
                    {
                        orders.Remove(q);
                    }
                }
                else
                {
                    if (invoicesOfCurrentOrder.Any())
                    {
                        orders.Remove(q);
                    }
                }

            }
            return Json(orders.Select(q => new InvoiceReferenceSelectViewModel
            {
                Value = q.SalesOrderVoucherID,
                Display = q.SalesOrderVoucherID.ToString()
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllUsers()
        {
            var userDb = new ApplicationDbContext();
            var users = userDb.Users;

            return Json(users.Select(u => new { UserName = u.UserName, FullName = u.FirstName + " " + u.MiddleName + " " + u.LastName + " | @" + u.UserName }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLoggedinUser()
        {
            var userDb = new ApplicationDbContext();
            var users = userDb.Users.Where(u => u.UserName == User.Identity.Name);

            return Json(users.Select(u => new { UserName = u.UserName, FullName = u.FirstName + " " + u.MiddleName + " " + u.LastName + " | @" + u.UserName }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSalesShops()
        {
            var shops = db.SalesStores.Where(s => s.Type == SalesStoreType.Shop);
            return Json(shops.Select(s => new { s.SalesStoreId, s.Description }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSalesStoresAndShops()
        {
            var shops = db.SalesStores;
            return Json(shops.Select(s => new { s.SalesStoreId, s.Description }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAssiggnedPriceTags(int productId)
        {
            List<SelectListItem> listToReturn = new List<SelectListItem>();
            var allValues =
              Enum
             .GetValues(typeof(Price_Types))
             .Cast<int>()
             .Select(i => new SelectListItem
             {
                 Value = i.ToString(),
                 Text = Enum.GetName(typeof(Price_Types), i),
             }
             )
             .ToList();
            foreach (var item in allValues)
            {
                var mappedPriceTag = db.Prices.FirstOrDefault(p => p.Thing == productId.ToString() && p.ReferenceType == item.Text);
                if (mappedPriceTag != null)
                {
                    listToReturn.Add(item);
                }
            }

            return Json(listToReturn, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGlTaxRates()
        {
            var taxRates = db.GLTaxRates;
            return Json(taxRates.Select(s => new { s.GLTaxRateID, s.TaxName }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAddtionalCostTypes()
        {
            var types = db.AdditionalCosts;
            return Json(types.Select(a => new { a.AdditionalCostId, a.Name }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetQuotationTerms()
        {
            var terms = db.QuotationTerms;
            return Json(terms.Select(t => new { t.QuotationTermId, t.QuotationInDays, t.Desription }), JsonRequestBehavior.AllowGet);
        }
        public string GetPeriodRanges(int periodId)
        {
            string range = null;
            var currentPeriod = db.CRMPeriods.Find(periodId);
            if (currentPeriod != null)
            {
                var start = currentPeriod.DateFrom.ToString("MMMM dd, yyyy");
                var end = currentPeriod.DateTo.ToString("MMMM dd, yyyy");
                if (!string.IsNullOrEmpty(start) && !string.IsNullOrEmpty(end))
                {
                    range = start + "_" + end;
                }
            }

            return range;
        }
        public string GetPeriodRangesWithinSalesQuoteDates(int periodId, int quoteId = 0)
        {
            string range = null;
            var currentQuote = db.SalesQuoteVouchers.Find(quoteId);
            var currentPeriod = db.CRMPeriods.Find(periodId);
            if (currentPeriod != null)
            {

                var start = currentPeriod.DateFrom.ToString("MMMM dd, yyyy");
                var end = currentPeriod.DateTo.ToString("MMMM dd, yyyy");
                if (currentQuote != null &&
                    currentQuote.Date.Date >= currentPeriod.DateFrom.Date &&
                    currentQuote.Date.Date <= currentPeriod.DateTo.Date
                )
                {
                    start = currentQuote.Date.ToString("MMMM dd, yyyy");
                }
                if (!string.IsNullOrEmpty(start) && !string.IsNullOrEmpty(end))
                {
                    range = start + "_" + end;
                }
            }

            return range;
        }
        /// <summary>
        /// Returns time range of a given crm period. the date of the earliest sales order being referenced by the
        /// given sales invoice is taken as the intial date
        /// </summary>
        /// <param name="periodId"> crm period id</param>
        /// <param name="concatnatedOrderIds"> comma delimted order ids</param>
        /// <returns></returns>
        public string GetPeriodRangesWithinSalesOrderDates(int periodId, string concatnatedOrderIds)
        {
            string range = null;
            List<SalesOrderVoucher> referencedOrderVouchers = new List<SalesOrderVoucher>();
            if (!string.IsNullOrEmpty(concatnatedOrderIds))
            {
                var idsArray = concatnatedOrderIds.Split(',');
                if (idsArray.Length > 0)
                {
                    for (int i = 0; i < idsArray.Length; i++)
                    {
                        int.TryParse(idsArray[i], out int targetId);
                        var orderRecord = db.SalesOrderVouchers.Find(targetId);
                        if (orderRecord != null)
                        {
                            referencedOrderVouchers.Add(orderRecord);
                        }
                    }
                }
            }

            var currentPeriod = db.CRMPeriods.Find(periodId);
            if (currentPeriod != null)
            {

                var start = currentPeriod.DateFrom.ToString("MMMM dd, yyyy");
                var end = currentPeriod.DateTo.ToString("MMMM dd, yyyy");

                if (referencedOrderVouchers.Any())
                {
                    var arrangedOrders = referencedOrderVouchers.OrderBy(o => o.Date).ToArray();
                    var earliesttOrder = arrangedOrders[0];

                    if (earliesttOrder != null &&
                        earliesttOrder.Date.Date >= currentPeriod.DateFrom.Date &&
                        earliesttOrder.Date.Date <= currentPeriod.DateTo.Date)
                    {
                        start = earliesttOrder.Date.ToString("MMMM dd, yyyy");
                    }
                }
                if (!string.IsNullOrEmpty(start) && !string.IsNullOrEmpty(end))
                {
                    range = start + "_" + end;
                }
            }

            return range;
        }
        public string GetPeriodRangesWithinSalesSingleSalesOrderDates(int periodId, int orderId = 0)
        {
            string range = null;
            var currentOrder = db.SalesOrderVouchers.Find(orderId);
            var currentPeriod = db.CRMPeriods.Find(periodId);
            if (currentPeriod != null)
            {

                var start = currentPeriod.DateFrom.ToString("MMMM dd, yyyy");
                var end = currentPeriod.DateTo.ToString("MMMM dd, yyyy");
                if (currentOrder != null &&
                    currentOrder.Date.Date >= currentPeriod.DateFrom.Date &&
                    currentOrder.Date.Date <= currentPeriod.DateTo.Date
                )
                {
                    start = currentOrder.Date.ToString("MMMM dd, yyyy");
                }
                if (!string.IsNullOrEmpty(start) && !string.IsNullOrEmpty(end))
                {
                    range = start + "_" + end;
                }
            }

            return range;
        }
        public string GetPeriodRangesWithinShipmentReferencesDates(int periodId, int source = -1, int invoiceId = 0, int orderId = 0)
        {
            ShipmentSource src = (ShipmentSource)source;

            string range = null;
            //var currentOrder = db.SalesOrderVouchers.Find(orderId);
            var currentPeriod = db.CRMPeriods.Find(periodId);
            if (currentPeriod != null)
            {
                var start = currentPeriod.DateFrom.ToString("MMMM dd, yyyy");
                var end = currentPeriod.DateTo.ToString("MMMM dd, yyyy");

                if (src == ShipmentSource.SalesInvoice)
                {
                    var currentInvoice = db.CashSalesInvoices.Find(invoiceId);
                    if (currentInvoice != null &&
                        currentInvoice.Date.Date >= currentPeriod.DateFrom.Date &&
                        currentInvoice.Date.Date <= currentPeriod.DateTo.Date)
                    {
                        start = currentInvoice.Date.ToString("MMMM dd, yyyy");
                    }
                }
                else if (src == ShipmentSource.SalesOrder)
                {
                    var currentOrder = db.SalesOrderVouchers.Find(orderId);
                    if (currentOrder != null &&
                    currentOrder.Date.Date >= currentPeriod.DateFrom.Date &&
                    currentOrder.Date.Date <= currentPeriod.DateTo.Date)
                    {
                        start = currentOrder.Date.ToString("MMMM dd, yyyy");
                    }
                }


                if (!string.IsNullOrEmpty(start) && !string.IsNullOrEmpty(end))
                {
                    range = start + "_" + end;
                }
            }

            return range;
        }

        public JsonResult GetTemporaryCustomersForAutocomplete(string text)
        {
            //var northwind = new SampleEntities();


            var customers = db.OrganizationCustomers.Where(c => c.IsTemporary).Select(customer => new
            {
                TradeName = customer.TradeName,

            });

            if (!string.IsNullOrEmpty(text))
            {
                customers = customers.Where(p => p.TradeName.ToLower().Contains(text.ToLower()));
            }

            return Json(customers, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllCustomersForAutoComplete()
        {

            var customers = db.OrganizationCustomers.Select(customer => new
            {
                TradeName = customer.TradeName,

            });


            return Json(customers, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetInvestmentTypes()
        {
            var cs = db.InvestmentTypes.Select(v => new
            {
                InvestmentName = v.InvestmentName,
                InvestmentTypeId = v.InvestmentTypeId,

            });
            return Json(cs, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetQueueTypes()
        {
            var cs = db.QueueTypes.Select(v => new
            {
                v.QueueName,
                v.QueueTypeId,

            });
            return Json(cs, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetQueueCustomers(string filterString)
        {
            filterString = filterString.Trim().ToLower();
            var data = db.QueueCustomer.Select(v => new
            {
                v.CustomerName,
                v.QueueCustomerId,
                v.Phone,
                v.TinNumber
            }).ToList();
            if (!string.IsNullOrEmpty(filterString))
            {
                data = data.Where(d =>
                                d.CustomerName.ToLower().Contains(filterString.ToLower()) ||
                                d.Phone.Contains(filterString) ||
                                d.TinNumber.Contains(filterString)
                                ).ToList();
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLocationMaps(int branchId, string filterString)
        {
            var locIds = new HashSet<int>(db.LocationUserAssignment.Where(uas => uas.UserName == User.Identity.Name).ToList().Select(m => m.LocationMapId));

            var data = db.LocationMap.Where(m => m.BranchId == branchId && locIds.Contains(m.LocationMapId)).ToList();
            if (!string.IsNullOrEmpty(filterString))
            {
                data = data.Where(d => d.LocationName.ToLower().Contains(filterString.ToLower())).ToList();
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCustomerQueue(int customerId, string filterString)
        {
            var locIds = new HashSet<int>(db.LocationUserAssignment.Where(uas => uas.UserName == User.Identity.Name).ToList().Select(m => m.LocationMapId));

            var data =
                db.Queue.Where(m => m.QueueCustomerId == customerId &&
                                    m.QueueStatus != CustomerQueueStatus.Closed &&
                                    m.IsOnlineApproved &&
                                    locIds.Contains(m.LocationMapId)
                                    ).ToList();
            if (!string.IsNullOrEmpty(filterString))
            {
                data = data.Where(d => d.QueueNumber.ToString().Contains(filterString.ToLower())).ToList();
            }
            return Json(data.Select(d => new { d.QueueId, d.QueueNumber }), JsonRequestBehavior.AllowGet);
        }
        public string GetConcatnatedQueueCustomerData(int id)
        {
            string data = null;
            var customer = db.QueueCustomer.Find(id);
            if (customer != null)
            {
                var tin = customer.TinNumber;
                var phone = customer.Phone;
                var investmentType = customer.InvestmentType.InvestmentName;

                if (!string.IsNullOrEmpty(tin) && !string.IsNullOrEmpty(phone) && !string.IsNullOrEmpty(investmentType))
                {
                    data = tin + "_" + phone + "_" + investmentType;
                }
            }

            return data;
        }
        public string GetConcatnatedQueueData(int id)
        {
            string data = null;
            var q = db.Queue.Find(id);
            if (q != null)
            {
                decimal previouslyShipped = 0;
                var previousQues = db.QueueStatus.Where(s => s.QueueId == id).ToList();

                if (previousQues.Any())
                {
                    previouslyShipped = previousQues.Sum(ss => ss.ShippedQuantity);
                }

                var ap = q.ApprovedQuantity;
                var rem = ap - previouslyShipped;
                var br = q.BranchId;
                var l = q.LocationMapId;
                var store = q.Store;

                //if (ap> 0 && br>0 && l>0)
                //{
                data = ap + "_" + rem + "_" + br + "_" + l;
                //}
                if (!string.IsNullOrEmpty(store))
                {
                    data += "_" + store;
                }
            }

            return data;
        }
        public decimal GetPreviouslyShippedQueue(int id)
        {
            decimal shipped = 0;
            var queue = db.QueueStatus.Where(s => s.QueueId == id).ToList();

            if (queue.Any())
            {
                shipped = queue.Sum(q => q.ShippedQuantity);
            }
            return shipped;
        }

        public int? GetReferedSalesQuoteBranchId(int quoteId)
        {
            return db.SalesQuoteVouchers.Find(quoteId)?.BranchId;

        }
        public int? GetReferedSalesOrderBranchId(string orderIds)
        {
            int? bId = 0;
            if (!string.IsNullOrEmpty(orderIds))
            {
                var ids = orderIds.Split(',');
                int.TryParse(ids[0], out int firstId);
                bId = db.SalesOrderVouchers.Find(firstId)?.BranchId;
            }
            return bId;
        }
        public int? GetReferedSingleSalesOrderBranchId(int orderId)
        {
            return db.SalesOrderVouchers.Find(orderId)?.BranchId;

        }
        public int? GetBranchOfShipmentReference(int shipmentSource = -1, int invoiceId = 0, int salesOrderId = 0)
        {
            int? branchId = 0;
            if ((ShipmentSource)shipmentSource == ShipmentSource.SalesInvoice)
            {
                branchId = db.CashSalesInvoices.Find(invoiceId)?.BranchId;
            }
            else if ((ShipmentSource)shipmentSource == ShipmentSource.SalesOrder)
            {
                branchId = db.SalesOrderVouchers.Find(salesOrderId)?.BranchId;

            }
            return branchId;

        }
        public int? GetStoreOfShipmentReference(int shipmentSource = -1, int invoiceId = 0, int salesOrderId = 0)
        {
            int? storeId = 0;
            if ((ShipmentSource)shipmentSource == ShipmentSource.SalesInvoice)
            {
                storeId = db.CashSalesInvoices.Find(invoiceId)?.StoreID;
            }
            else if ((ShipmentSource)shipmentSource == ShipmentSource.SalesOrder)
            {
                storeId = db.SalesOrderVouchers.Find(salesOrderId)?.StoreID;

            }
            return storeId;

        }
        public string GetBaseUrlAndPosIdSetting(int invoiceId)
        {
            var name = "";
            var posId = "";
            var baseUrl = "";
            var currentInvoice = db.CashSalesInvoices.Find(invoiceId);
            //if(currentInvoice != null)
            var lineItem = db.LineItems.FirstOrDefault(l => l.Voucher == invoiceId.ToString() && l.VoucherDefinitionID == "2");
            if (lineItem != null)
            {
                var currentStore = db.SalesStores.FirstOrDefault(s => s.SalesStoreId == lineItem.StoreID);
                if (currentStore != null)
                {
                    posId = currentStore.PosId;

                    baseUrl = currentStore.PosServiceBaseUrl;
                }
            }
            //var baseUrlSetting = db.PosSetting.FirstOrDefault(s => s.SettingType == PosSettingType.BaseUrl);
            ////var posIdSetting = db.PosSetting.FirstOrDefault(s => s.SettingType == PosSettingType.PosId);
            //if (baseUrlSetting != null)
            //{
            //    name = baseUrlSetting.Value + "_" + posId;
            //}
            name = baseUrl + "_" + posId;
            return name;
        }
        public async Task<bool> IsDuplicateLineItem(string voucherId, string voucherDefId, int productId, string variety, string className)
        {
            bool itemExists = false;
            if (string.IsNullOrEmpty(variety))
            {
                variety = null;
            }
            if (string.IsNullOrEmpty(className))
            {
                className = null;
            }
            var existingItem = await db.LineItems.Where(l => l.Voucher == voucherId &&
                                           l.VoucherDefinitionID == voucherDefId &&
                                           l.ThingID == productId &&
                                           l.Variety == variety &&
                                           l.Class == className).ToListAsync();
            if (existingItem.Any())
            {
                itemExists = true;
            }
            return itemExists;
        }
        public JsonResult GetInventoryCategories()
        {
            var db = new ExceedDbContext();
            var cats = db.ItemCategorys.Where(c => !c.HaveParent);
            return Json(cats.Select(c => new { c.ItemCategoryCode, c.Name }).ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllProductCategories()
        {
            var db = new ExceedDbContext();
            var cats = db.ProductCategories.Where(c => c.Parent == null);
            return Json(cats.Select(c => new { c.ProductCategoryId, c.Description }).ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSubCategories(string categoryId)
        {
            var db = new ExceedDbContext();
            //var parentCategory = db.ProductCategories.FirstOrDefault(c => c.ProductCategoryId == categoryId);
            //if(parentCategory != null)
            //{
            var cats = db.ProductCategories.Where(c => c.Parent == categoryId);
            return Json(cats.Select(c => new { c.ProductCategoryId, c.Description }).ToList(), JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    return null;
            //}
        }
        public JsonResult GetFiscalYears()
        {
            var db = new ExceedDbContext();
            var years = db.GLFiscalYears.Where(f => f.Status == FiscalYearStatus.Open || f.Status == FiscalYearStatus.FuturePeriod);
            return Json(years.Select(y => new { y.GlFiscalYearId, y.Name }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReceiveMethodDDBind()
        {
            var db = new ExceedDbContext();
            var PaymentMethods = db.PaymentMethods.Where(x => x.Enable == true).Select(s => new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject
            {
                Name = s.Method,
                Id = s.PaymentMethodID
            });

            return Json(PaymentMethods, JsonRequestBehavior.AllowGet);
        }
        public string GetUoMOfSelectedProduct(int productId)
        {
            return db.Products.Find(productId)?.UOM;
        }

        public JsonResult GetAllProducts()
        {
            return Json(db.Products.Select(p => new { Name = p.Name + " [" + p.ItemCode + "]", ProductId = p.ProductID }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllProductsByCategory(int categoryId, string filterText)
        {
            var products = db.Products.Where(p => p.CategoryID == categoryId.ToString()).ToList();
            if (!string.IsNullOrEmpty(filterText))
            {
                products = products.Where(l => l.Name.ToLower().Contains(filterText.ToLower()) || l.ItemCode.Contains(filterText)).ToList();
            }
            return Json(products.Select(p => new { Name = p.Name + " [" + p.ItemCode + "]", ProductId = p.ProductID }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllUoM()
        {
            return Json(db.UoMs.Select(u => new { u.UoMName, u.Description }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPossibleConvertableUoMOfProduct(int productId = 0)
        {
            //List<UoM> units = new List<UoM>();
            var assignedUom = db.ProductUnitConvertorSetup.Where(p => p.ProductId == productId).ToList();
            //if (assignedUom.Any())
            //{
            //    units = db.UoMs.Where(u => assignedUom.Contains())
            //}

            return Json(assignedUom.Select(u => new { UoMName = u.FromUoM }), JsonRequestBehavior.AllowGet);
        }
        public decimal GetUnitConversionMultiplicationFactor(string uom, int productId)
        {
            decimal val = 0;
            var conversionSetup = db.ProductUnitConvertorSetup.FirstOrDefault(p => p.ProductId == productId && p.FromUoM == uom);
            if (conversionSetup != null)
            {
                val = conversionSetup.MultiplicationFactor;
            }
            return val;
        }

        public JsonResult GetCRMPeriodsForStorePlan(int annualStorePlanId)
        {
            List<CRMPeriod> periods = new List<CRMPeriod>();
            var headerRecord = db.StoreAnnualSalesPlans.Find(annualStorePlanId);
            if (headerRecord != null)
            {
                var year = db.GLFiscalYears.Find(headerRecord.BranchAnnualSalesPlan.AnnualSalesPlan.GlFiscalYearId);
                if (year != null)
                {
                    periods = db.CRMPeriods.Where(s => s.DateFrom >= year.DateFrom && s.DateFrom <= year.DateTo).ToList();
                }
            }

            return Json(periods.Select(p => new { p.CRMPeriodId, p.Name }), JsonRequestBehavior.AllowGet);
        }
        public string GetInventoryItemName(string code)
        {
            return db.InventoryItems.Find(code)?.Name;
        }
        public string GetInventoryItemUOM(string code)
        {
            return db.InventoryItems.Find(code)?.UoMName;
        }
        public decimal? GetInventoryItemPrice(string code)
        {
            return db.InventoryItems.Find(code)?.AvgPrice;
        }
        /// <summary>
        /// service types (repair types)
        /// </summary>
        /// <returns></returns>
        public JsonResult GetCRMServiceTypes()
        {
            return Json(db.RepairType.Select(r => new { RepairTypeId = r.RepairTypeID.ToString(), r.Name }), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// category of maintenance repair types
        /// </summary>
        /// <returns></returns>
        //public JsonResult GetServiceCategories()
        //{
        //    var catIdInRep = new HashSet<string>(db.RepairType.Select(r => r.CategoryCode));
        //    var cats = db.ItemCategorys.Where(c => catIdInRep.Contains(c.ItemCategoryCode));
        //    return Json(cats.Select(c => new { c.Name, c.ItemCategoryCode }), JsonRequestBehavior.AllowGet);
        //}
        /// <summary>
        /// sub category of maintenance repair types
        /// </summary>
        /// <param name="categoryCode"></param>
        /// <returns></returns>
        //public JsonResult GetServiceSubCategories(string categoryCode)
        //{
        //    var subCatIds = new HashSet<string>(db.RepairType.Where(c => c.CategoryCode == categoryCode).Select(r => r.SubCategoryCode));
        //    var cats = db.ItemCategorys.Where(c => subCatIds.Contains(c.ItemCategoryCode));
        //    return Json(cats.Select(c => new { c.Name, c.ItemCategoryCode }), JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult GetMaintenanceServices(string categoryCode, string subCategoryCode, ServiceGroup serviceGroup)
        //{
        //    var cats = db.RepairType.Where(r => r.CategoryCode == categoryCode && r.SubCategoryCode == subCategoryCode && r.ServiceGroup == serviceGroup);
        //    return Json(cats.Select(c => new { Name = c.Name, RepairTypeId = c.RepairTypeID.ToString() }), JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult GetServices(string subCategoryCode, ServiceGroup ServiceGroup)
        //{
        //    var enumCategories = db.RepairType.Where(q => q.SubCategoryCode == subCategoryCode && q.ServiceGroup == ServiceGroup)
        //        .Select(s => new
        //        {
        //            Code = s.RepairTypeID.ToString(),
        //            Name = s.Code + "-" + s.Name
        //        })
        //        .ToList();
        //    return Json(enumCategories, JsonRequestBehavior.AllowGet);
        //}
        public JsonResult GetAllInventoryCategoryies()
        {
            var cats = db.ItemCategorys.Where(c => !c.HaveParent);
            return Json(cats.Select(c => new { c.Name, c.ItemCategoryCode }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetInventoryItems(string categoryCode, string filterText)
        {
            var data = db.InventoryItems.Where(c => c.ItemCategoryCode == categoryCode);
            if (!string.IsNullOrEmpty(categoryCode))
            {
                data = data.Where(d => d.Name.ToLower().Contains(filterText.ToLower()) || d.ItemCode.Contains(filterText));
            }
            return Json(data.Select(c => new { c.Name, c.ItemCode }), JsonRequestBehavior.AllowGet);
        }

        #region constant related data sources

        #endregion
        public JsonResult GetShipmentSourcesExcludingMaintenance()
        {
            var data = Enum.GetValues(typeof(ShipmentSource))
               .Cast<ShipmentSource>()
               .Select(t => new CRMEnumDisplayViewModel
               {
                   Value = ((int)t),
                   Text = t.ToString()
               })
               .Where(m => m.Value != 2); // exclude Maintenance sources
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //public JsonResult GetEquipments(int customerId, string filterText)
        //{
        //    var equipments = db.EquipmentInformation.Where(e => e.CustomerId == customerId).ToList();
        //    if (!string.IsNullOrEmpty(filterText))
        //    {
        //        equipments = equipments.Where(e => e.LicensePlate.Contains(filterText)).ToList();
        //    }
        //    return Json(equipments.Select(e => new { e.LicensePlate }), JsonRequestBehavior.AllowGet);
        //}
        public JsonResult GetStoreItemassignmentByCategoryAndStore(string categoryCode, string storeCode, string filterText)
        {
            var items = db.StoreItemAssignments
                .Where(i => i.InventoryItem.ItemCategoryCode == categoryCode &&
                    i.InventoryStore.StoreCode == storeCode)
                    .Select(i => new { i.StoreItemAssignmentID, Item = i.InventoryItem.Name + " " + i.InventoryItem.ItemCode })
                    .ToList();
            if (!string.IsNullOrEmpty(filterText))
            {
                items = items.Where(i => i.Item.ToLower().Contains(filterText.ToLower())).ToList();
            }
            return Json(items, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCompanyBankAccounts()
        {
            var data = db.BankAccounts.Select(p => new { AccountName = p.AccountNumber + " " + p.AccountName + " " + p.BankBranch + " Branch", BankAccountId = p.BankAccountId });

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMaintenanceJobOrdersToGenerateSQ()
        {
            var existingNumbers = new HashSet<string>(db.SalesQuoteVouchers.Where(sq => sq.Source == SalesQuoteSource.Maintenance && !string.IsNullOrEmpty(sq.JobOrderNumber)).Select(sq => sq.JobOrderNumber));
            var data = db.JobOrder
                .Where(j => !existingNumbers.Contains(j.Num.ToString()) 
                && j.IsSecondOnlineApproved 
                && j.Status == MaintenanceConst.Closed).OrderByDescending(j => j.Num);

            return Json(data.Select(d => new { JobOrderNumber = d.Num}), JsonRequestBehavior.AllowGet);
        }
        #region enum related 
        public JsonResult GetTenderFollowupStatus()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem
            {
                Value = TenderFollowupStatus.TechnicalEvaluation,
                Text = TenderFollowupStatus.TechnicalEvaluation
            });
            list.Add(new SelectListItem
            {
                Value = TenderFollowupStatus.FinancialEvaluation,
                Text = TenderFollowupStatus.FinancialEvaluation
            });
            list.Add(new SelectListItem
            {
                Value = TenderFollowupStatus.LostFinancialEvaluation,
                Text = TenderFollowupStatus.LostFinancialEvaluation
            });
            list.Add(new SelectListItem
            {
                Value = TenderFollowupStatus.LostTechnicalEvaluation,
                Text = TenderFollowupStatus.LostTechnicalEvaluation
            });
            list.Add(new SelectListItem
            {
                Value = TenderFollowupStatus.LostNonTechnicalEvaluation,
                Text = TenderFollowupStatus.LostNonTechnicalEvaluation
            });
            list.Add(new SelectListItem
            {
                Value = TenderFollowupStatus.NonParticipating,
                Text = TenderFollowupStatus.NonParticipating
            });
            list.Add(new SelectListItem
            {
                Value = TenderFollowupStatus.Won,
                Text = TenderFollowupStatus.Won
            });
            list.Add(new SelectListItem
            {
                Value = TenderFollowupStatus.AdvancePayment,
                Text = TenderFollowupStatus.AdvancePayment
            });
            list.Add(new SelectListItem
            {
                Value = TenderFollowupStatus.OpenLC,
                Text = TenderFollowupStatus.OpenLC
            });
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}