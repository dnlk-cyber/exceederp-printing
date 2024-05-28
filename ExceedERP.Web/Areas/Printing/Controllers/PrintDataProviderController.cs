using ExceedERP.Core.Domain.Printing;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Printing.Controllers
{
    public class PrintDataProviderController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        

        public JsonResult PrintMachineType()
        {
            var printMachine = db.PrintingMachineTypes;
            var result = printMachine.Select(s => new
            {
                Text = s.MachineTypeName,
                Value = s.PrintingMachineTypeId
            }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult PrintPaperSize()
        {
            var paperSize = db.PrintingPaperSizes;
            var result = paperSize.Select(s => new
            {
                Text = s.PaperSizeName,
                Value = s.PrintingPaperSizeId
            }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult JobType()
        {
            var jobType = db.PrintingJobTypes;
            var result = jobType.Select(s => new
            {
                Text = s.JobTypeName,
                Value = s.PrintingJobTypeId
            }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult JobTypeFilterable(string text)
        {
            var jobType = db.PrintingJobTypes.AsQueryable();
            if (!string.IsNullOrWhiteSpace(text))
            {
                jobType = jobType.Where(x => x.JobTypeName.ToLower().Contains(text.ToLower())).AsQueryable();
            }
            var result = jobType.Select(s => new
            {
                Text = s.JobTypeName,
                Value = s.PrintingJobTypeId
            }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PrintingProcess(PrintingProcessCategory processCategory)
        {
            var printingProcess = db.PrintingProcesses.Where(x => x.PrintingCategory == processCategory);
            var result = printingProcess.Select(s => new
            {
                Text = s.ProcessName,
                Value = s.PrintingProcessId
            }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
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
        
        public JsonResult PaperSizeFilterable(string text)
        {
            var paper = db.PrintingPaperSizes.AsQueryable();
            if (!string.IsNullOrWhiteSpace(text))
            {
                paper = paper.Where(x => x.PaperSizeName.ToLower().Contains(text.ToLower())).AsQueryable();
            }
            var result = paper.Select(s => new
            {
                Value = s.PrintingPaperSizeId,
                Text = s.PaperSizeName
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult BindingStyleFilterable(string text)
        {
            var bindingStyle = db.PrintingBindingStyles.AsQueryable();
            if (!string.IsNullOrWhiteSpace(text))
            {
                bindingStyle = bindingStyle.Where(x => x.BindingStyleName.ToLower().Contains(text.ToLower())).AsQueryable();
            }
            var result = bindingStyle.Select(s => new
            {
                Value = s.PrintingBindingStyleId,
                Text = s.BindingStyleName
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetACustomers(string text)
        {
            var customers = db.OrganizationCustomers.AsQueryable();
            if (!string.IsNullOrWhiteSpace(text))
            {
                customers = customers.Where(x => x.TradeName.ToLower().Contains(text.ToLower())).AsQueryable();
            }
            var result = customers.Select(s => new
            {
                Value = s.OrganizationCustomerID,
                Text = s.TradeName
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPrintingMaterialCategory(string text)
        {
            var materialCategory = db.PrintingMaterialCategories.AsQueryable();
            if (!string.IsNullOrWhiteSpace(text))
            {
                materialCategory = materialCategory.Where(x => x.CategoryName.Contains(text)).AsQueryable();
            }
            var result = materialCategory.Select(s => new
            {
                Value = s.PrintingMaterialCategoryId,
                Text = s.CategoryName
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPrintingMaterialCategoryItem(int materialCategoryId, string text)
        {
            var materialCategoryItems = db.PrintingMaterialCategoryItems.Where(x => x.PrintingMaterialCategoryId == materialCategoryId).AsQueryable();
            if (!string.IsNullOrWhiteSpace(text))
            {
                materialCategoryItems = materialCategoryItems.Where(x => x.ItemName.Contains(text)).AsQueryable();
            }
            var result = materialCategoryItems.Select(s => new
            {
                Value = s.PrintingMaterialCategoryItemId,
                Text = s.ItemName
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet); 
        }
        public JsonResult GetPrintingProcess(string text, PrintingProcessCategory printingProcessCategory)
        {
            var printingProcess = db.PrintingProcesses.Where(x => x.PrintingCategory == printingProcessCategory).AsQueryable();
            if (!string.IsNullOrWhiteSpace(text))
            {
                printingProcess = printingProcess.Where(x => x.ProcessName.ToLower().Contains(text.ToLower())).AsQueryable();
            }
            var result = printingProcess.Select(s => new
            {
                Value = s.PrintingProcessId,
                Text = s.ProcessName
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}