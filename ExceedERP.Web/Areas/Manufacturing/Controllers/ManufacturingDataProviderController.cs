using ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation;
using ExceedERP.Core.Domain.Manufacturing.Setting;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers
{
    public class ManufacturingDataProviderController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: Manufacturing/ManufacturingDataProvider
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetInventoryItemCategory(string text)
        {
            // only Category
            var itemCategory = db.ItemCategorys.Where(x => !x.HaveParent);
            if (!string.IsNullOrWhiteSpace(text))
            {
                itemCategory = itemCategory.Where(x => x.Name.Contains(text));
            }
            var result = itemCategory.Select(s => new
            {
                Value = s.ItemCategoryCode,
                Text = s.Name + "-" + s.ItemCategoryCode
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetInventoryItemSubCategory(string text, string categoryCode)
        {
            // Only sub-Category
            var itemCategory = db.ItemCategorys.Where(x => x.HaveParent && x.ParentName == categoryCode);

            if (!string.IsNullOrWhiteSpace(text))
            {
                itemCategory = itemCategory.Where(x => x.Name.Contains(text));
            }
            var result = itemCategory.Select(s => new
            {
                Value = s.ItemCategoryCode,
                Text = s.Name + "-" + s.ItemCategoryCode
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetUoM()
        {
            var unitOfm = db.UoMs.AsQueryable();
            return Json(unitOfm.Select(i => new
            {
                UoMName = i.UoMName,
                Value = i.UoMName
            })
            , JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTaskCategory(WorkShop taskType)
        {
            var taskCategory = db.ManifucturingTaskCategories.Where(x => x.Type == taskType).AsQueryable();
            var result = taskCategory.Select(s => new
            {
                Text = s.Name,
                Value = s.ManifucturingTaskCategoryId
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFurnitureJobCategory(string text)
        {
            var furnitureJobCategories = db.FurnitureJobCategories.AsQueryable();
            if (!string.IsNullOrWhiteSpace(text))
            {
                furnitureJobCategories = furnitureJobCategories.Where(x => x.JobName.Contains(text)).AsQueryable();
            }
            var result = furnitureJobCategories.Select(s => new
            {
                Value = s.FurnitureJobCategoryId,
                Text = s.JobName
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetACustomers(string text)
        {
            var customers = db.OrganizationCustomers.AsQueryable();
            if (!string.IsNullOrWhiteSpace(text))
            {
                customers = customers.Where(x => x.TradeName.Contains(text)).AsQueryable();
            }
            var result = customers.Select(s => new
            {
                Value = s.OrganizationCustomerID,
                Text = s.TradeName
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetInventoryItems(string text)
        {
            var inventoryItems = db.InventoryItems.AsQueryable();
            if (!string.IsNullOrWhiteSpace(text))
            {
                inventoryItems = inventoryItems.Where(x => x.Name.Contains(text)).AsQueryable();
            }
            var result = inventoryItems.Select(s => new
            {
                Value = s.ItemCode,
                Text = s.Name
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetManufacturingMaterialCategory(string text)
        {
            var materialCategory = db.ManufacturingMaterialCategories.AsQueryable();
            if (!string.IsNullOrWhiteSpace(text))
            {
                materialCategory = materialCategory.Where(x => x.ManufacturingCategory.Contains(text)).AsQueryable();
            }
            var result = materialCategory.Select(s => new
            {
                Value = s.ManufacturingMaterialCategoryId,
                Text = s.ManufacturingCategory
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetManufacturingMaterialCategoryItem(int materialCategoryId, string text)
        {
            var materialCategoryItems = db.ManufacturingMaterialCategoryItems.Where(x => x.ManufacturingMaterialCategoryId == materialCategoryId).AsQueryable();
            if (!string.IsNullOrWhiteSpace(text))
            {
                materialCategoryItems = materialCategoryItems.Where(x => x.ItemName.Contains(text)).AsQueryable();
            }
            var result = materialCategoryItems.Select(s => new
            {
                Value = s.ManufacturingMaterialCategoryItemId,
                Text = s.ItemName
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStandardJobType(string text)
        {
            var jobType = db.FurnitureStandardJobTypes.AsQueryable();
            if (!string.IsNullOrWhiteSpace(text))
            {
                jobType = jobType.Where(x => x.JobTypeName.Contains(text) || x.JobCode.Contains(text)).AsQueryable();
            }
            var result = jobType.Select(s => new
            {
                Value = s.FurnitureStandardJobTypeId,
                Text = s.JobCode + "/" + s.JobTypeName
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // Get Approved Estimations
        public JsonResult GetFurnitureEstimation(string text)
        {
            var estimation = db.FurnitureEstimationForms.Where(x => x.IsOnlineApproved).AsQueryable();
            if (!string.IsNullOrEmpty(text))
            {
                estimation = estimation.Where(x => x.FurnitureEstimationId == int.Parse(text)).AsQueryable();
            }
            var result = estimation.Select(s => new
            {
                Value = s.FurnitureEstimationId,
                Text = s.FurnitureEstimationId + "/" + db.FurnitureStandardJobTypes.FirstOrDefault(x => x.FurnitureStandardJobTypeId == s.JobTypeId).JobTypeName
            }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEmployees(string text)
        {
            var employees = db.Person_Employee.AsQueryable();
            if (!string.IsNullOrWhiteSpace(text))
            {
                employees = employees.Where(x => x.FirstName.Contains(text) || x.MiddleName.Contains(text) || x.LastName.Contains(text)).AsQueryable();
            }
            var result = employees.Select(s => new
            {
                Value = s.EmployeeId,
                Text = s.FirstName + "-" + s.MiddleName + "-" + s.LastName
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProformaInvoiceNo(string text)
        {
           
            var jobOrder = db.FurnitureJobOrderForms.ToList();
            var proformaInvoice = db.FurnitureProformaInvoices.Where(x => x.IsOnlineApproved).ToList();
            
            foreach (var item in jobOrder)
            {

                proformaInvoice.RemoveAll(x => x.FurnitureProformaInvoiceId == item.FurnitureProformaInvoiceId);
            }
            
            if (!string.IsNullOrWhiteSpace(text))
            {
                proformaInvoice = proformaInvoice.Where(x => x.FurnitureProformaInvoiceId == int.Parse(text)).ToList();
            }
            var result = proformaInvoice.Select(s => new
            {
                Value = s.FurnitureProformaInvoiceId,
                Text = s.FurnitureProformaInvoiceId + "-" + db.OrganizationCustomers.FirstOrDefault(x => x.OrganizationCustomerID == s.CustomerId).TradeName
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}