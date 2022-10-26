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
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.CRMAdmin)]
    public class PriceController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Prices_Read([DataSourceRequest]DataSourceRequest request, int thingID)
        {
            IQueryable<Price> prices = db.Prices.Where(p => p.Thing == thingID.ToString());
            DataSourceResult result = prices.ToDataSourceResult(request, price => new
            {
                PriceID = price.PriceID,
                Thing = thingID,
                ReferenceType = price.ReferenceType,
                Reference = price.Reference,
                Value = price.Value,
                IsDefault = price.IsDefault,
                Priority = price.Priority,
                Variety = price.Variety,
                Class = price.Class,
                CreatedBy = price.CreatedBy,
                DateCreated = price.DateCreated,
                LastModified = price.LastModified,
                ModifiedBy = price.ModifiedBy,
                OrgBranchName = price.OrgBranchName,
                Remark = price.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Prices_Create([DataSourceRequest]DataSourceRequest request, Price price, int thingID, string priceThingType)
        {
            List<Price> priceList = db.Prices.Where(p => p.Thing.ToString() == thingID.ToString() && p.Priority == price.Priority).ToList();
            List<Price> RefExist = db.Prices.Where(p => p.Thing.ToString() == thingID.ToString() && p.Reference == price.Reference && p.Reference == price.ReferenceType).ToList();

            if (priceList.Count > 0)
            {
                ModelState.AddModelError("Invalid-Index", "Invalid index entry, please readjust index value");
            }
            else if (RefExist.Any())
            {
                ModelState.AddModelError("DuplicateName", "Reference Already Registered ");
            }
            else
            {
                if (ModelState.IsValid)
                {

                    //     var productName = db.Products.Where(p => p.ProductID == thingID).SingleOrDefault().Name;
                    // price enum get by index??     var refName =ExceedERP.Core.Domain.CRM.Price_Types.get()

                    //future implement
                    // if (price.ReferenceType == "7")
                    // {
                    //var  priceDetailList = db.SubCountries.Select(sc => new Reference() { Code = sc.SubCountryID.ToString(), Description = sc.Name }).ToList();


                    // }
                    // else if (price.ReferenceType == "4")
                    // {
                    //  var   priceDetailList = db.Holidays.Select(hd => new Reference() { Code = hd.HolidayID.ToString(), Description = hd.HolidayDefinition.Description }).ToList();

                    // }
                    var entity = new Price
                    {
                        Thing = thingID.ToString(),
                        Reference = price.Reference,
                        ReferenceType = price.ReferenceType,
                        Value = price.Value,
                        IsDefault = price.IsDefault,
                        Priority = price.Priority,
                        Variety = price.Variety,
                        Class = price.Class,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now,
                        Remark = price.Remark
                    };

                    db.Prices.Add(entity);
                    db.SaveChanges();
                    price.PriceID = entity.PriceID;
                }
            }

            return Json(new[] { price }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Prices_Update([DataSourceRequest]DataSourceRequest request, Price price)
        {

            List<Price> priceList = db.Prices.ToList();
            Price selectedPrice = priceList.Where(p => p.PriceID == price.PriceID).FirstOrDefault();
            priceList.Remove(selectedPrice);
            priceList = priceList.Where(p => p.Thing.ToString() == price.Thing && p.Priority == price.Priority).ToList();
            if (priceList.Any())
            {
                ModelState.AddModelError("Invalid-Index", "Invalid index entry, please readjust index value");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    db = new ExceedDbContext();
                    var entity = new Price
                    {
                        PriceID = price.PriceID,
                        Thing = price.Thing,
                        Reference = price.Reference,
                        ReferenceType = price.ReferenceType,
                        Value = price.Value,
                        IsDefault = price.IsDefault,
                        Priority = price.Priority,
                        Variety = price.Variety,
                        Class = price.Class,
                        Remark = price.Remark
                    };

                    db.Prices.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }


            return Json(new[] { price }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Prices_Destroy([DataSourceRequest]DataSourceRequest request, Price price)
        {
            if (ModelState.IsValid)
            {
                var entity = new Price
                {
                    PriceID = price.PriceID,
                    Thing = price.Thing,
                    Reference = price.Reference,
                    Value = price.Value,
                    IsDefault = price.IsDefault,
                    Priority = price.Priority,
                    Variety = price.Variety,
                    Class = price.Class,
                    Remark = price.Remark
                };

                db.Prices.Attach(entity);
                db.Prices.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted  Price number " + price.PriceID);

            }

            return Json(new[] { price }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
