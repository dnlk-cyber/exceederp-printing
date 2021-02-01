using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.APTS;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.APTS.Controllers
{
    public class MedicineListsController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: APTS/MedicineLists
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        public void PopulateViewData()
        {
           var itemCategories = db.ItemCategorys.ToList();
           var inventoryItems = db.InventoryItems.ToList();
            List<ItemVM> items = new List<ItemVM>();
            List<ItemCatagoryVM> itemCatagoryList = new List<ItemCatagoryVM>();
            foreach (var it in inventoryItems)
            {
                var item = new ItemVM();

                item.ItemCode = it.ItemCode;
                item.Name = it.Name;
                item.ItemCodeWithName = it.ItemCode + "-" + it.Name;

                items.Add(item);
            }
            foreach (var it in itemCategories)
            {
                var itemCat = new ItemCatagoryVM();

                itemCat.ItemCategoryCode = it.ItemCategoryCode;
                itemCat.Name = it.Name;
                itemCat.ItemCategoryCodeWithName = it.ItemCategoryCode + "-" + it.Name;

                itemCatagoryList.Add(itemCat);
            }
            ViewData["Items"] = items;
            ViewData["Catagory"] = itemCatagoryList;
        }
        public ActionResult MedicineLists_Read([DataSourceRequest]DataSourceRequest request)
        {
            var medisince = db.DrugLists.Where(x=>x.IsActive).ToList();
            DataSourceResult result = medisince.ToDataSourceResult(request, medice => new
            {
                DrugListsId = medice.DrugListsId,
                InternalDrugCode=medice.InternalDrugCode,
                DrugCode = medice.DrugCode,
                Description = medice.Description,
                IsActive =medice.IsActive,
                ItemCode = medice.ItemCode,
                ItemCategoryCode = medice.ItemCategoryCode,
                IsOur = medice.IsOur

            });

            return Json(result);
        }
        public ActionResult MedicineLists_Create([DataSourceRequest]DataSourceRequest request, DrugLists medicineLists)
        {
           if(ModelState.IsValid)
            {
                var description = "";
                var item = db.InventoryItems.FirstOrDefault(x=>x.ItemCode == medicineLists.ItemCode);
                if(item != null)
                {
                    description = item.Name;
                }
                var entity = new DrugLists
                {
                    DrugCode = medicineLists.DrugCode,
                    InternalDrugCode =medicineLists.InternalDrugCode,
                    Description = description,
                    IsActive = medicineLists.IsActive,
                    ItemCode = medicineLists.ItemCode,
                    ItemCategoryCode = medicineLists.ItemCategoryCode,
                    IsOur = medicineLists.IsOur
                };
                db.DrugLists.Add(entity);
                db.SaveChanges();
                medicineLists.DrugListsId = entity.DrugListsId;
            }

            return Json(new[] { medicineLists }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult MedicineLists_Update([DataSourceRequest]DataSourceRequest request, DrugLists medicineLists)
        {
            if (ModelState.IsValid)
            {
                var entity = new DrugLists
                {
                    DrugListsId = medicineLists.DrugListsId,
                    InternalDrugCode = medicineLists.InternalDrugCode,
                    DrugCode = medicineLists.DrugCode,
                    Description = medicineLists.Description,
                    ItemCode = medicineLists.ItemCode,
                    ItemCategoryCode = medicineLists.ItemCategoryCode,
                    IsActive = medicineLists.IsActive,
                    IsOur = medicineLists.IsOur

                };
       
                db.DrugLists.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                var requisition = db.RequisionModel20s.FirstOrDefault(x=>x.DrugListsId == medicineLists.DrugListsId);
                if(requisition != null)
                {
                    requisition.Description = medicineLists.Description;
                    requisition.InternalCode = medicineLists.InternalDrugCode;

                    db.Entry(requisition).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Json(new[] { medicineLists }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult MedicineLists_Destroy([DataSourceRequest]DataSourceRequest request, DrugLists medicineLists)
        {
            if (ModelState.IsValid)
            {
                
                var deletedList = db.DrugLists.Find(medicineLists.DrugListsId);
                deletedList.IsActive = false;
                deletedList.IsOur = false;
                db.Entry(deletedList).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { medicineLists }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }

    public class ItemVM
    {
        public string ItemCode { get; set; }
        public string Name { get; set; }
        public string ItemCodeWithName { get; set; }
    }
    public class ItemCatagoryVM
    {
        public string ItemCategoryCode { get; set; }
        public string Name { get; set; }
        public string ItemCategoryCodeWithName { get; set; }
    }
}