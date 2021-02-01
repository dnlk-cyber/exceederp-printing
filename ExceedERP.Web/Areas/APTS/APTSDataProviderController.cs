using ExceedERP.Core.Domain.APTS;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.APTS
{
    public class APTSDataProviderController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
    
        public ActionResult GetDrugLists()
        {
            List<DrugLists> drugLists = new List<DrugLists>();
            var drugs = db.DrugLists.Where(x => x.IsActive && x.IsOur);
            foreach(var drug in drugs)
            {
                var drugList = new DrugLists();
                var item = db.InventoryItems.FirstOrDefault(x=>x.ItemCode == drug.ItemCode);
                if(item != null)
                {
                    drugList.DrugListsId = drug.DrugListsId;
                    drugList.Description = item.Name;

                }
                drugLists.Add(drugList);
            }
            return Json(drugLists.Select(p => new
            {
                DrugListsId = p.DrugListsId,
                Description = p.Description
            }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetUnits()
        {
            List<DrugUnit> drugUnits = new List<DrugUnit>();
            var units = db.DrugUnits.ToList();
            foreach(var unit in units)
            {
                var drugUnit = new DrugUnit();
                var uom = db.UoMs.FirstOrDefault(x=>x.UoMName == unit.UoMName);
                if(uom != null)
                {
                    drugUnit.DrugUnitId = unit.DrugUnitId;
                    drugUnit.UnitName = uom.UoMName;
                }

                drugUnits.Add(drugUnit);
            }
           
            return Json(drugUnits.Select(p => new
            {
                DrugUnitId = p.DrugUnitId,
                UnitName = p.UnitName
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFisicalYear()
        {

            var periodList = db.GLFiscalYears;

            return Json(periodList.Select(p => new
            {
                GlFiscalYearId = p.GlFiscalYearId,
                Name = p.Name
            }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetGlPeriod()
        {

            var periodList = db.GLPeriods;

            return Json(periodList.Select(p => new
            {
                GLPeriodId = p.GLPeriodId,
                Name = p.Name
            }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetBranchs()
        {

            var periodList = db.Branch.ToList();

            return Json(periodList.Select(p => new
            {
                BranchId = p.BranchId,
                Name = p.Name
            }), JsonRequestBehavior.AllowGet);

        }
        public ActionResult GetDrugStoreLists()
        {
            List<DrugStore> storeLists = new List<DrugStore>();
            var stores = db.DrugStores.ToList();
            
                storeLists.AddRange(stores);
            
            return Json(storeLists.Select(p => new
            {
                DrugStoreId = p.DrugStoreId,
                NameWithCode = p.NameWithCode
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllStoresFromInventory ()
        {
            var store = db.InventoryStores.Where(x => x.Enable).AsQueryable();
            return Json(store.Select(p => new
            {
                StoreCode = p.StoreCode,
                Name = p.StoreCode + "-" + p.Name
            }), JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetDrugStores()
        {
            var drugStores = db.DrugStores.ToList();
            return Json(drugStores.Select(p => new
            {
                DrugStoreId = p.DrugStoreId,
                NameWithCode = p.NameWithCode
            }), JsonRequestBehavior.AllowGet);
        }

    }
}