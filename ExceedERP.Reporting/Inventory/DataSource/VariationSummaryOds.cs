using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Reporting.Inventory.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ExceedERP.Reporting.Inventory.DataSource
{
    [DataObject]
    public class VariationSummaryOds
    {
        ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public InventoryCountVariationHeaderVm GetVariationHeader(string id)
        {

            InventoryCountVariationHeaderVm record = new InventoryCountVariationHeaderVm();
            var Organization = _db.Companies.FirstOrDefault(x => x.Category == "0");

            if (!String.IsNullOrEmpty(id))
            {
                int countId = int.Parse(id);
                var propertyCount = _db.PropertyCounts.Find(countId);
                if (propertyCount != null)
                {
                    string skeeper = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(UserHelper.GetUserName(propertyCount.StoreKeeperId)));

                    record.StoreKeeper = skeeper.Length == 0 ? "............................................. " : skeeper;
                    record.VoucherId = id;
                }
            }

            if (Organization != null)
            {
                record.Name = Organization.TradeName;
                record.AmharicName = Organization.AmharicName;
                record.Abbreviation = Organization.Abbreviation;

                var address =
                    _db.Addresses.Where(a =>
                        a.Reference == Organization.MainCompanyId.ToString() &&
                        a.ReferenceType.ToLower() == "company").ToList();
                if (address.Any())
                {
                    foreach (var a in address)
                    {

                        if (record.Address == null)
                        {
                            record.Address = a.Category + ":- " + a.Value;
                        }
                        else
                        {
                            record.Address += ", " + a.Category + "-" + a.Value;
                        }


                    }

                }
                var logo =
                    _db.Attachments.FirstOrDefault(a => a.Reference == Organization.MainCompanyId.ToString() &&
                                                       a.ReferenceType == AttachmentType.LOGO);
                if (logo != null)
                {
                    record.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }

                record.Date =
                    XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
            }
            return record;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public IEnumerable<VariationSummaryVm> GetVariationSummary(string id, string category,bool ignoreZeroValues)
        {
            List<VariationSummaryVm> list = new List<VariationSummaryVm>();

            if (!String.IsNullOrEmpty(id))
            {
                int countId = int.Parse(id);
                var propertyCount = _db.PropertyCounts.Find(countId);
                if (propertyCount != null)
                {
                    var countitems = _db.PropertyCountItems.Where(x=>x.PropertyCountId==countId).ToList();

                    if (!String.IsNullOrEmpty(category))
                    {
                        countitems = countitems.Where(x => x.ItemCategoryCode == category).ToList();
                    }
                    var categories = countitems.DistinctByField(x => x.ItemCategoryCode);

                    foreach (var cat in categories)
                    {
                        var variationSummaryVm = new VariationSummaryVm()
                        {
                            CategoryCode = cat.ItemCategoryCode,
                            CategoryName = cat.ItemCategory.Name,
                            VariationItemList = new List<VariationItemViewModel>()
                        };

                        foreach (var countItem in countitems.Where(x => x.ItemCategoryCode == cat.ItemCategoryCode))
                        {
                            if (ignoreZeroValues && countItem.PreviousBalance==0 && countItem.Count==0)
                                continue;
                            
                            VariationItemViewModel itemViewModel = new VariationItemViewModel();

                            if (countItem.PreviousBalance < countItem.Count)
                            {
                                itemViewModel.OveragePrice = (countItem.Count - countItem.PreviousBalance) * countItem.Price;
                                itemViewModel.OverageQty = countItem.Count - countItem.PreviousBalance;
                                itemViewModel.Amouont = countItem.Count * countItem.Price;
                                itemViewModel.PhysicalAmount = countItem.Count * countItem.Price;
                                itemViewModel.TotalAmount = countItem.PreviousBalance * countItem.Price;
                                itemViewModel.Price = countItem.Price;
                                itemViewModel.ShortageQty = 0;
                                itemViewModel.ShortagePrice = 0;
                                itemViewModel.PreviousBalance = countItem.PreviousBalance;
                                itemViewModel.ItemCode = countItem.ItemCode;
                                itemViewModel.ItemLocation = countItem.Location;
                                itemViewModel.ItemCategoryCode = countItem.ItemCategoryCode;
                                itemViewModel.Count = countItem.Count;
                                itemViewModel.UoM = countItem.InventoryItem.UoMName;
                                itemViewModel.ItemName = countItem.InventoryItem.Name;

                                variationSummaryVm.VariationItemList.Add(itemViewModel);
                            }
                            else if (countItem.PreviousBalance > countItem.Count)
                            {
                                itemViewModel.ShortagePrice = (countItem.PreviousBalance - countItem.Count) * countItem.Price;
                                itemViewModel.ShortageQty = countItem.PreviousBalance - countItem.Count;
                                itemViewModel.PhysicalAmount = countItem.Count * countItem.Price;
                                itemViewModel.TotalAmount = countItem.PreviousBalance * countItem.Price;
                                itemViewModel.OverageQty = 0;
                                itemViewModel.Price = countItem.Price;
                                itemViewModel.OveragePrice = 0;
                                itemViewModel.PreviousBalance = countItem.PreviousBalance;
                                itemViewModel.ItemCode = countItem.ItemCode;
                                itemViewModel.ItemLocation = countItem.Location;
                                itemViewModel.ItemCategoryCode = countItem.ItemCategoryCode;
                                itemViewModel.Count = countItem.Count;
                                itemViewModel.UoM = countItem.InventoryItem.UoMName;
                                itemViewModel.ItemName = countItem.InventoryItem.Name;

                                variationSummaryVm.VariationItemList.Add(itemViewModel);
                            }
                            
                            else
                            {
                                
                                itemViewModel.ShortagePrice = 0;
                                itemViewModel.ShortageQty = 0;
                                itemViewModel.Amouont = countItem.Count * countItem.Price;
                                itemViewModel.OverageQty = 0;
                                itemViewModel.PhysicalAmount = countItem.Count * countItem.Price;
                                itemViewModel.TotalAmount = countItem.PreviousBalance * countItem.Price;
                                itemViewModel.PreviousBalance = countItem.PreviousBalance;
                                itemViewModel.Price = countItem.Price;
                                itemViewModel.OveragePrice = 0;
                                itemViewModel.ItemCode = countItem.ItemCode;
                                itemViewModel.ItemLocation = countItem.Location;
                                itemViewModel.ItemCategoryCode = countItem.ItemCategoryCode;
                                itemViewModel.Count = countItem.Count;
                                itemViewModel.UoM = countItem.InventoryItem.UoMName;
                                itemViewModel.ItemName = countItem.InventoryItem.Name;

                                variationSummaryVm.VariationItemList.Add(itemViewModel);
                                
                            }
                        }
                        variationSummaryVm.RecordAmount = Math.Round(variationSummaryVm.VariationItemList.Sum(x => x.TotalAmount),4);
                        variationSummaryVm.PhysicalAmount = Math.Round(variationSummaryVm.VariationItemList.Sum(x => x.PhysicalAmount), 4);
                        variationSummaryVm.ShortageAmount = Math.Round(variationSummaryVm.VariationItemList.Sum(x => x.ShortagePrice),4);
                        variationSummaryVm.OverageAmount = Math.Round(variationSummaryVm.VariationItemList.Sum(x => x.OveragePrice), 4);
                       
                        list.Add(variationSummaryVm);
                    }

                }
            }
           
            return list.OrderBy(x => x.CategoryCode);
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PropertyCount> GetCountDates()
        {
            List<PropertyCount> countDates = _db.PropertyCounts.ToList();
            return countDates;
        }
    }
}
