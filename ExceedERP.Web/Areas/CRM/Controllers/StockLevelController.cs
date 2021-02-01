﻿using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;     
﻿using ExceedERP.Core;
﻿using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
﻿using ExceedERP.DataAccess;

﻿using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;  

namespace ExceedERP.Web.Areas.CRM.Controllers
{

    [AuthorizeRoles(CRMRoles.CRMAdmin )] 
    public class StockLevelController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult StockLevels_Read([DataSourceRequest]DataSourceRequest request, int thingID, string stockLevelThingType)
        {
            IQueryable<StockLevel> stocklevels = db.StockLevels.Where(sl => sl.Thing == thingID.ToString() && sl.ThingType == stockLevelThingType);
            DataSourceResult result = stocklevels.ToDataSourceResult(request, stockLevel => new
            {
                StockLevelID = stockLevel.StockLevelID,
                Thing = stockLevel.Thing,
                StoreID = stockLevel.StoreID,
                MinimumLevel = stockLevel.MinimumLevel,
                MaximumLevel = stockLevel.MaximumLevel,
                LeadTime = stockLevel.LeadTime,
                EconomicOrderQuantity = stockLevel.EconomicOrderQuantity,
                CreatedBy = stockLevel.CreatedBy,
                DateCreated = stockLevel.DateCreated,
                LastModified = stockLevel.LastModified,
                ModifiedBy = stockLevel.ModifiedBy,
                OrgBranchName = stockLevel.OrgBranchName,
                Remark = stockLevel.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StockLevels_Create([DataSourceRequest]DataSourceRequest request, StockLevel stockLevel, int thingID, string stockLevelThingType)
        {
            if (ModelState.IsValid)
            {
                var entity = new StockLevel
                {
                    Thing = thingID.ToString(),
                    ThingType = stockLevelThingType,
                    StoreID = stockLevel.StoreID,
                    MinimumLevel = stockLevel.MinimumLevel,
                    MaximumLevel = stockLevel.MaximumLevel,
                    LeadTime = stockLevel.LeadTime,
                    EconomicOrderQuantity = stockLevel.EconomicOrderQuantity,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    Remark = stockLevel.Remark
                };

                db.StockLevels.Add(entity);
                db.SaveChanges();
                stockLevel.StockLevelID = entity.StockLevelID;
            }

            return Json(new[] { stockLevel }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StockLevels_Update([DataSourceRequest]DataSourceRequest request, StockLevel stockLevel)
        {
            if (ModelState.IsValid)
            {
                var entity = new StockLevel
                {
                    StockLevelID = stockLevel.StockLevelID,
                    ThingType = stockLevel.ThingType,
                    Thing = stockLevel.Thing,
                    StoreID = stockLevel.StoreID,
                    MinimumLevel = stockLevel.MinimumLevel,
                    MaximumLevel = stockLevel.MaximumLevel,
                    LeadTime = stockLevel.LeadTime,
                    EconomicOrderQuantity = stockLevel.EconomicOrderQuantity,
                    Remark = stockLevel.Remark
                };

                db.StockLevels.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { stockLevel }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StockLevels_Destroy([DataSourceRequest]DataSourceRequest request, StockLevel stockLevel)
        {
            if (ModelState.IsValid)
            {
                var entity = new StockLevel
                {
                    StockLevelID = stockLevel.StockLevelID,
                    Thing = stockLevel.Thing,
                    StoreID = stockLevel.StoreID,
                    MinimumLevel = stockLevel.MinimumLevel,
                    MaximumLevel = stockLevel.MaximumLevel,
                    LeadTime = stockLevel.LeadTime,
                    EconomicOrderQuantity = stockLevel.EconomicOrderQuantity,
                    ThingType = stockLevel.ThingType,
                    Remark = stockLevel.Remark
                };

                db.StockLevels.Attach(entity);
                db.StockLevels.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted  Stock Level number " + stockLevel.StockLevelID);
            }

            return Json(new[] { stockLevel }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
