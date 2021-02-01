﻿using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;  
using ExceedERP.Core.Domain.Inventory;

namespace ExceedERP.Web.Areas.CRM.Controllers
{

    [AuthorizeRoles(CRMRoles.CRMAdmin)] 
    public class StoreController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            IQueryable<InventoryStore> stores = db.InventoryStores;
            ViewData["StoreList"] = stores;
            return View();
        }

        public ActionResult Stores_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<InventoryStore> stores = db.InventoryStores;
            DataSourceResult result = stores.ToDataSourceResult(request, store => new InventoryStore
            {
                StoreCode = store.StoreCode,
                Name = store.Name,
         
                Remark = store.Remark,
               
                CreatedBy = store.CreatedBy,
                DateCreated = store.DateCreated,
                LastModified = store.LastModified,
                ModifiedBy = store.ModifiedBy,
      
            });

            return Json(result);
        }
 
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
