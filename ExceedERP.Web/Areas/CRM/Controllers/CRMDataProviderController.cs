using ExceedERP.Core;
using ExceedERP.DataAccess;
using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;     
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using System.Data.Entity.Core.Objects;
 using ExceedERP.Core.Domain.Common;
using Microsoft.Ajax.Utilities;
using ExceedERP.Web.Areas.CRM.Models;
using Price_Types = ExceedERP.Core.Domain.CRM.Price_Types;
using ExceedERP.Helpers.Common;  
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Web.Models; 

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
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult UserDDBind()
        {

            var Parts = UserDb.Users.Select(s => new
            {
                Name = s.FirstName +" "+ s.LastName,
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

            var Tickets = db.CustomerTickets.Where(x=>x.Status != FinalizeType.CLOSED).Select(s => new
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
            return Json(uom.Select(s => new { Text = s.UoMName , Value= s.UoM  }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLineItemByInvoice( string invoiceId)
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
  
        public JsonResult GetStoreList()
        {
            var storeList = db.SalesStores.Select(p => new  { StoreCode = p.SalesStoreId, Name = p.Description } ).ToList();
                                 
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

            return Json(StatusTypes,JsonRequestBehavior.AllowGet);
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

        public JsonResult GetItemCategory()
        {
            var CategoryList = db.ItemCategorys.ToList();
            List<ItemCategory> referenceList = CategoryList.Select(tcategory => new ItemCategory() { ItemCategoryCode = tcategory.ItemCategoryCode.ToString(), Description = tcategory.Name }).ToList();
            return Json(referenceList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetItemByCategory(int Category)
        {
            var CRMCategory = db.ProductCategories.Where(x=>x.ProductCategoryId == Category).FirstOrDefault();
            var InventoryMaps = db.InventoryCateoryMaps.Where(x=>x.Category == CRMCategory.Description).ToList();
            foreach (var InventoryMap in InventoryMaps)
            {
                var InventoryItems = db.InventoryItems.Where(x => x.ItemCategoryCode == InventoryMap.InventoryCategoryId).ToList();
                List<InventoryItem> InventoryItem = InventoryItems.Select(tcategory => new InventoryItem() { Name = tcategory.Name.ToString(), ItemCode = tcategory.ItemCode }).ToList();
                return Json(InventoryItem, JsonRequestBehavior.AllowGet);
            }
            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetThingList()
        {
            var thingList = db.InventoryItems;
            return Json(thingList.Select(thing => new { ProductID = thing.ItemCode, Name = thing.Name }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetThingListByStore(string StoreID)
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
            }
            return Json(thingList.DistinctByField(x => x.ItemCode).Select(thing => new { ProductID = thing.ProductID, Name = thing.Name }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStockBalance(string storeID, string productID, string VarietyID,string Class)
        {
            try
            {
                string SaleQuoteDefination = "0";
                string SalesOrderVoucherDefination = "1";
                string CashInvoiceDefination = "2";
                string productName = productID ?? string.Empty;
                var Store = db.SalesStores.Where(x => x.SalesStoreId.ToString() == storeID).FirstOrDefault();
                var Product = db.Products.Where(p => p.ProductID.ToString() == productID).FirstOrDefault();
                 
                List<StoreItemAssignment> stockLevel = Store.InventoryStore.StoreItemAssignments.Where(x => x.ItemCode == Product.ItemCode && x.StoreCode == Store.InventoryStore.StoreCode).ToList();

                List<LineItem> SalesOrderItems = db.LineItems.Where(l => (l.ThingID == Product.ProductID  && l.VoucherDefinitionID == SalesOrderVoucherDefination && l.StoreID == Store.SalesStoreId  && l.Status != CRMConstants.ProductIssued)).ToList();
                List<LineItem> SalesQuoteItems = db.LineItems.Where(l => (l.ThingID == Product.ProductID  && l.VoucherDefinitionID == SaleQuoteDefination && l.StoreID == Store.SalesStoreId  && l.Status != CRMConstants.ProductIssued)).ToList();

                List<LineItem> CashInvoiceItems = db.LineItems.Where(l => (l.ThingID == Product.ProductID   && l.VoucherDefinitionID == CashInvoiceDefination && l.StoreID == Store.SalesStoreId  && l.Status != CRMConstants.ProductIssued)).ToList();


                if (!string.IsNullOrEmpty(VarietyID))
                {
                    SalesQuoteItems = SalesQuoteItems.Where(x => x.Variety == VarietyID).ToList();
                    SalesOrderItems = SalesOrderItems.Where(x => x.Variety == VarietyID).ToList();
                    CashInvoiceItems = CashInvoiceItems.Where(x => x.Variety == VarietyID).ToList();
                }

                if (!string.IsNullOrEmpty(Class))
                {
                    SalesQuoteItems = SalesQuoteItems.Where(x => x.Class == Class).ToList();
                    SalesOrderItems = SalesOrderItems.Where(x => x.Class == Class).ToList();
                    CashInvoiceItems = CashInvoiceItems.Where(x => x.Class == Class).ToList();
                }

                List<LineItem> saleItem = new List<LineItem>();
                List<LineItem> cashinvoiceItem = new List<LineItem>();
                List<LineItem> SalequoteItem = new List<LineItem>();

                foreach (var SalesQuoteItem in SalesQuoteItems)
                {
                    string OrderID = SalesQuoteItem.Voucher;
                    SalesQuoteVoucher sov =
                        db.SalesQuoteVouchers.Where(s => s.SalesQuoteVoucherID.ToString() == OrderID).SingleOrDefault();

                    if (sov != null)
                    {
                        if (sov.LastObjectState == CRMConstants.Forwarded)
                        {

                            SalequoteItem.Add(SalesQuoteItem);
                        }
                    }
                }
                if (SalequoteItem != null)
                {
                    foreach (var sale in SalequoteItem)
                    {
                        SalesQuoteItems.Remove(sale);

                    }
                }



                foreach (var SalesOrderItem in SalesOrderItems)
                {
                    string OrderID = SalesOrderItem.Voucher;
                    SalesOrderVoucher sov =
                        db.SalesOrderVouchers.Where(s => s.SalesOrderVoucherID.ToString() == OrderID).SingleOrDefault();

                    if (sov != null)
                    {
                        if (sov.LastObjectState == CRMConstants.Forwarded)
                        {

                            saleItem.Add(SalesOrderItem);
                        }
                    }
                }
                if (saleItem != null)
                {
                    foreach (var sale in saleItem)
                    {
                        SalesOrderItems.Remove(sale);

                    }
                }

                foreach (var CashInvoiceItem in CashInvoiceItems)
                {
                    string InvoiceID = CashInvoiceItem.Voucher;
                    CashSalesInvoice cInvoice =
                        db.CashSalesInvoices.Where(
                            c =>
                                c.CashSalesInvoiceID.ToString() == InvoiceID).SingleOrDefault();
                    if (cInvoice != null)
                    {
                        if (cInvoice.LastObjectState == CRMConstants.Forwarded)
                        {

                            cashinvoiceItem.Add(CashInvoiceItem);
                        }
                    }

                }
                if (cashinvoiceItem != null)
                {
                    foreach (var invoice in cashinvoiceItem)
                    {

                        CashInvoiceItems.Remove(invoice);
                    }
                }

                decimal QuoteQuantity = 0;
                if (SalesQuoteItems != null)
                {
                    QuoteQuantity = SalesQuoteItems.Sum(s => s.Quantity);
                }
                else
                {
                    QuoteQuantity = 0;
                }

                decimal OrderedQuantity = 0;
                if (SalesOrderItems != null)
                {
                    OrderedQuantity = SalesOrderItems.Sum(s => s.Quantity);
                }
                else
                {
                    OrderedQuantity = 0;
                }
                decimal InvoiceChecKQuantity = 0;
                if (SalesOrderItems != null)
                {
                    InvoiceChecKQuantity = CashInvoiceItems.Sum(s => s.Quantity);
                }
                else
                {
                    InvoiceChecKQuantity = 0;
                }
                if (stockLevel != null)
                {
                    List<StoreItemAssignment> stockBalance = new List<StoreItemAssignment>();
                    if (!string.IsNullOrEmpty(Class))
                    {
                        stockBalance = stockLevel.Where(x => x.ItemCategoryCode == Class.ToString()).ToList();
                        if (stockBalance == null)
                        {
                            stockBalance = stockLevel;
                        }
                    }
                    else
                    {

                        stockBalance = stockLevel;

                    }

                    decimal stocktotal = stockBalance.Sum(s => s.Balance) - OrderedQuantity - InvoiceChecKQuantity - QuoteQuantity;
                    if(stocktotal <0)
                    {
                        stocktotal = 0;
                    }
                    decimal reserved = OrderedQuantity + InvoiceChecKQuantity + QuoteQuantity;
                    return Json(stocktotal + "_0.0_" + reserved, JsonRequestBehavior.AllowGet);

                }
            }
            catch( Exception e)
            { return Json(0.0, JsonRequestBehavior.AllowGet); }
            return Json(0.0, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetTaxList()
        {
            var taxList = db.GLTaxRates.Where(x => x.ApplyToAr == true && x.Enable == true).ToList();
            return Json(taxList, JsonRequestBehavior.AllowGet);
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
            var AdditionalCost = db.AdditionalCostLogs.Where(x => x.LineItemId == Id).Select(s => new   { AdditionalCostId = s.AdditionalCostId , Name = s.AdditionalCost.Name, Value = s.Value }).ToList();
            return Json(AdditionalCost, JsonRequestBehavior.AllowGet);
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

        public JsonResult GetPriceValue(string priceTagID, string priceTagDetailID, string productID, string VarietyID ,string ClassID)
        {

            var productnumId = db.Products.Where(x => x.ProductID.ToString() == productID).FirstOrDefault().ProductID.ToString();
            var priceValue = db.Prices.Where(p => (p.ReferenceType == priceTagID && p.Reference == priceTagDetailID && p.Thing == productnumId) || (p.ReferenceType == priceTagID && p.Reference == priceTagDetailID && p.Thing == productnumId && p.Variety == VarietyID)).ToList();
            var DefaultpriceValue = db.Prices.Where(p =>( p.IsDefault == true && p.Thing == productnumId && p.Variety == VarietyID) || ( p.IsDefault == true && p.Thing == productnumId) ).FirstOrDefault();

            decimal price = 0;
            if (priceValue != null)
            {
               
                var classPrice = priceValue.Where(x => x.Class == ClassID).FirstOrDefault();
                if(classPrice != null)
                {
                    price = classPrice.Value;
                }
                else
                {
                    price = priceValue.FirstOrDefault().Value;

                }
                return Json(price, JsonRequestBehavior.AllowGet);
            }
                  else if(DefaultpriceValue != null)
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

        public JsonResult GetPriceTagDetailList(string priceTag)
        {
            List<Reference> priceDetailList = null;

            if (priceTag =="ON_HOLIDAY")
            {
                priceDetailList = db.CRMHolidays.Select(hd => new Reference() { Code = hd.CRMHolidayID.ToString(), Description = hd.HolidayDefinition.Description }).ToList();
             
            }
            else if (priceTag == "LOCATION_PRICE")
            {
                priceDetailList = db.SubCountries.Select(sc => new Reference() { Code = sc.SubCountryID.ToString(), Description = sc.Name }).ToList();


            }
            else
            {
                priceDetailList = new List<Reference>();
                priceDetailList.Add(new Reference() { Code = "1001", Description = priceTag });

            }
            return Json(priceDetailList, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetPriceTagDetailListVoucher(string priceTag)
        {
            List<Reference> priceDetailList = null;
            DateTime dt = DateTime.Today.Date;

           if (priceTag == "LOCATION_PRICE")
            {
                priceDetailList = db.SubCountries.Select(sc => new Reference() { Code = sc.SubCountryID.ToString(), Description = sc.Name }).ToList();


            }
          else if (priceTag =="ON_HOLIDAY")
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




    }


}