using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Inventory;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.CRMAdmin)]
    public class ProductPriceLogController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            //var uom = db.Lookups.Where(u => u.LookupType == LookUpTypes.UOM).AsQueryable();
            //ViewData["ProductCategories"] = db.ItemCategorys.Select(c => new { c.ProductCategoryId, c.Description }).ToList();
            ViewData["ItemCategorys"] = db.ItemCategorys.Where(c => !c.HaveParent).Select(c => new { c.ItemCategoryCode, c.Name }).ToList();
            ViewData["ItemSubCategorys"] = db.ItemCategorys.Where(c => c.HaveParent).Select(c => new { c.ItemCategoryCode, c.Name }).ToList();

            ViewData["InventoryStores"] = db.InventoryStores.Select(p => new {p.Name, p.StoreCode });

            //ViewData["UoM"] = uom.AsQueryable();

            return View();
        }

        public ActionResult ProductPriceLog_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<ProductPriceLog> products = db.ProductPriceLog.ToList();

            DataSourceResult result = products.AsQueryable().ToDataSourceResult(request, productPriceLog => new ProductPriceLog
            {
                ProductPriceLogId = productPriceLog.ProductPriceLogId,
                StoreItemAssignmentID = productPriceLog.StoreItemAssignmentID,
                ItemCategoryCode = productPriceLog.StoreItemAssignment.ItemCategory.ItemCategoryCode,
                //SubCategory = productPriceLog.StoreItemAssignment.ItemCategory.,
                UoM = productPriceLog.StoreItemAssignment.InventoryItem.UoMName,
                ItemCode = productPriceLog.StoreItemAssignment.ItemCode,
                ItemName = productPriceLog.StoreItemAssignment.InventoryItem.Name,
                StoreCode = productPriceLog.StoreItemAssignment.StoreCode,

                PriceType = productPriceLog.PriceType,

                NewDate = productPriceLog.NewDate,
                InitialDate = productPriceLog.InitialDate,
                InitialPrice = productPriceLog.InitialPrice,
                InitialExchangeRate = productPriceLog.InitialExchangeRate,
                NewExchangeRate = productPriceLog.NewExchangeRate,
                ProposedNewPrice = productPriceLog.ProposedNewPrice,
                ApprovedNewPrice = productPriceLog.ApprovedNewPrice,

                Difference = productPriceLog.ProposedNewPrice - productPriceLog.ApprovedNewPrice,

                CreatedBy = productPriceLog.CreatedBy,
                DateCreated = productPriceLog.DateCreated,
                LastModified = productPriceLog.LastModified,
                ModifiedBy = productPriceLog.ModifiedBy

            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductPriceLog_Create([DataSourceRequest]DataSourceRequest request, ProductPriceLog productPriceLog)
        {

            if (ModelState.IsValid)
            {

                var entity = new ProductPriceLog
                {
                    ProductPriceLogId = productPriceLog.ProductPriceLogId,
                    StoreItemAssignmentID = productPriceLog.StoreItemAssignmentID,
                    StoreCode = productPriceLog.StoreCode,
                    PriceType = productPriceLog.PriceType,
                    ItemCategoryCode = productPriceLog.ItemCode,
                    
                    NewDate = productPriceLog.NewDate,
                    InitialDate = productPriceLog.InitialDate,
                    InitialPrice = productPriceLog.InitialPrice,
                    InitialExchangeRate = productPriceLog.InitialExchangeRate,
                    NewExchangeRate = productPriceLog.NewExchangeRate,
                    ProposedNewPrice = productPriceLog.ProposedNewPrice,
                    ApprovedNewPrice = productPriceLog.ApprovedNewPrice,

                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name

                };
                if (entity.NewExchangeRate > 0 && entity.InitialExchangeRate > 0)
                {
                    decimal newPrice = (entity.NewExchangeRate / entity.InitialExchangeRate) * entity.InitialPrice;
                    entity.ProposedNewPrice = newPrice;
                }

                db.ProductPriceLog.Add(entity);
                db.SaveChanges();
                productPriceLog.ProductPriceLogId = entity.ProductPriceLogId;

            }

            return Json(new[] { productPriceLog }.ToDataSourceResult(request, ModelState));

        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductPriceLog_Update([DataSourceRequest]DataSourceRequest request, ProductPriceLog productPriceLog)
        {

            if (ModelState.IsValid)
            {
                var entity = new ProductPriceLog
                {
                    ProductPriceLogId = productPriceLog.ProductPriceLogId,
                    StoreItemAssignmentID = productPriceLog.StoreItemAssignmentID,
                    PriceType = productPriceLog.PriceType,

                    NewDate = productPriceLog.NewDate,
                    InitialDate = productPriceLog.InitialDate,
                    InitialPrice = productPriceLog.InitialPrice,
                    InitialExchangeRate = productPriceLog.InitialExchangeRate,
                    NewExchangeRate = productPriceLog.NewExchangeRate,
                    ProposedNewPrice = productPriceLog.ProposedNewPrice,
                    ApprovedNewPrice = productPriceLog.ApprovedNewPrice,

                    CreatedBy = productPriceLog.CreatedBy,
                    DateCreated = productPriceLog.DateCreated,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name
                };
                if (entity.NewExchangeRate > 0 && entity.InitialExchangeRate > 0)
                {
                    decimal newPrice = (entity.NewExchangeRate / entity.InitialExchangeRate) * entity.InitialPrice;
                    entity.ProposedNewPrice = newPrice;
                }

                db.ProductPriceLog.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();


            }
            return Json(new[] { productPriceLog }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductPriceLog_Destroy([DataSourceRequest]DataSourceRequest request, ProductPriceLog productPriceLog)
        {
            ProductPriceLog entity = db.ProductPriceLog.FirstOrDefault(x => x.ProductPriceLogId == productPriceLog.ProductPriceLogId);


            if (entity != null)
            {
                db.ProductPriceLog.Attach(entity);
                db.ProductPriceLog.Remove(entity);
                db.SaveChanges();

                return Json(new[] { productPriceLog }.ToDataSourceResult(request, ModelState));
            }


            return Json(new[] { productPriceLog }.ToDataSourceResult(request, ModelState));
        }

        public JsonResult CalculatePrice(DateTime currentDate, decimal exchangeRate)
        {
            var storeAssignments = db.StoreItemAssignments.ToList();
            foreach (StoreItemAssignment assignment in storeAssignments)
            {
                var existingRecord = db.ProductPriceLog.FirstOrDefault(p => p.StoreItemAssignmentID == assignment.StoreItemAssignmentID);
                if (existingRecord != null)
                {
                    // check if the selected date is not older than the previously used date
                    if (existingRecord.InitialDate < currentDate)
                    {
                        #region update existing price
                        if (exchangeRate != 0 && existingRecord.InitialExchangeRate != 0)
                        {
                            decimal newPrice = (exchangeRate / existingRecord.InitialExchangeRate) * existingRecord.InitialPrice;

                            existingRecord.ProposedNewPrice = newPrice;
                            if (existingRecord.ApprovedNewPrice == 0)
                            {
                                existingRecord.ApprovedNewPrice = newPrice;
                            }
                            existingRecord.NewExchangeRate = exchangeRate;
                            existingRecord.NewDate = currentDate;

                            db.ProductPriceLog.Attach(existingRecord);
                            db.Entry(existingRecord).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        #endregion
                    }
                }
                else
                {
                    //if (int.TryParse(price.Thing, out int pId))
                    //{
                    #region create new price
                    var newPrice = new ProductPriceLog
                    {
                        StoreItemAssignmentID = assignment.StoreItemAssignmentID,


                        InitialExchangeRate = exchangeRate,
                        InitialPrice = assignment.AvgPrice,

                        NewDate = currentDate,
                        NewExchangeRate = exchangeRate,
                        ApprovedNewPrice = assignment.AvgPrice,
                        ProposedNewPrice = assignment.AvgPrice,
                        //PriceType = assignment.ReferenceType,

                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        LastModified = null,
                        ModifiedBy = null,
                        OrgBranchName = null,
                    };
                    if (assignment.LastModified != null)
                    {
                        newPrice.InitialDate = assignment.LastModified.Value;
                    }
                    else if (assignment.DateCreated != null)
                    {
                        newPrice.InitialDate = assignment.DateCreated.Value;
                    }
                    else
                    {
                        //var product = db.Products.FirstOrDefault(p => p.ProductID.ToString() == price.Thing);
                        //if (product != null && product.LastModified.HasValue)
                        //{
                        //    newPrice.InitialDate = product.LastModified.Value;
                        //}
                        //else if (product != null && product.DateCreated.HasValue)
                        //{
                        //    newPrice.InitialDate = product.DateCreated.Value;
                        //}
                        //else
                        //{
                        newPrice.InitialDate = currentDate;
                        //}

                    }
                    db.ProductPriceLog.Add(newPrice);
                    db.SaveChanges();
                    #endregion
                    //}
                }

            }

            return Json(new
            {
                Message = "Successful",
                Status = "Ok"
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AcceptProposedPrice(int[] priceLogIds)
        {
            var existingPrices = db.ProductPriceLog.Where(p => priceLogIds.Contains(p.ProductPriceLogId) && p.ApprovedNewPrice != p.ProposedNewPrice).ToList();
            foreach (var price in existingPrices)
            {
                price.ApprovedNewPrice = price.ProposedNewPrice;

                db.ProductPriceLog.Attach(price);
                db.Entry(price).State = EntityState.Modified;
                db.SaveChanges();
            }
            db.SaveChanges();
            return Json(new
            {
                Message = "Price updated successfully",
                Status = "Ok"
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdateProductPrice(int[] priceLogIds)
        {
            var existingPrices = db.ProductPriceLog.Where(p => priceLogIds.Contains(p.ProductPriceLogId)).ToList();
            foreach (var price in existingPrices)
            {
                //var refType = $"{price.PriceType }[{price.NewDate.ToString("yyyy-MM-dd")}]";
                //var existingPriceWithCurrentRef = db.Prices.FirstOrDefault(p => p.Thing == price.ProductId.ToString() && p.ReferenceType == refType);
                //if (existingPriceWithCurrentRef != null)
                //{
                //    existingPriceWithCurrentRef.Value = price.ApprovedNewPrice;
                //    db.Prices.Attach(existingPriceWithCurrentRef);
                //    db.Entry(existingPriceWithCurrentRef).State = EntityState.Modified;
                //}
                //else
                //{
                //    Price productPrice = new Price
                //    {
                //        ReferenceType = $"{price.PriceType }[{price.NewDate.ToString("yyyy-MM-dd")}]",
                //        Thing = price.ProductId.ToString(),
                //        Value = price.ApprovedNewPrice,

                //        DateCreated = DateTime.Now,
                //        CreatedBy = User.Identity.Name,
                //        LastModified = null,
                //        ModifiedBy = null
                //    };
                //    db.Prices.Add(productPrice);
                //}

                var storeItem = db.StoreItemAssignments.Find(price.StoreItemAssignmentID);
                if(storeItem != null)
                {
                    storeItem.AvgPrice = price.ApprovedNewPrice;

                    db.StoreItemAssignments.Attach(storeItem);
                    db.Entry(storeItem).State = EntityState.Modified;
                }
            }
            db.SaveChanges();
            return Json(new
            {
                Message = "Item price updated successfully",
                Status = "Ok"
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

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
