using ExceedERP.Core.Domain.Printing;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Printing.Controllers
{
    public class PrintDataProviderController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();

        public JsonResult GetEmployees(string text)
        {
            var employees = db.Person_Employee.AsQueryable();
            if (!string.IsNullOrWhiteSpace(text))
            {
                employees = employees.Where(x => x.FirstNameEnglish.Contains(text) || x.MiddleNameEnglish.Contains(text) || x.LastNameEnglish.Contains(text)).AsQueryable();
            }
            var result = employees.Select(s => new
            {
                Value = s.EmployeeId,
                Text = s.FirstNameEnglish + "-" + s.MiddleNameEnglish + "-" + s.LastNameEnglish
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTaxRate()
        {
            var tax = db.GLTaxRates;
            var result = tax.Select(s => new
            {
                Text = s.TaxName,
                Value = s.GLTaxRateID
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPrintingEstimation(string text)
        {
            var estimation = db.PrintingCostEstimations.Where(x => x.IsOnlineApproved)
                .OrderByDescending(x => x.PrintingCostEstimationId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(text))
            {
                estimation = estimation.Where(x => x.PrintingCostEstimationId == int.Parse(text) || text.Contains(x.PrintJobType.ToString()))
                    .OrderByDescending(x => x.PrintingCostEstimationId)
                    .AsQueryable();
            }
            var result = estimation.Select(s => new
            {
                Value = s.PrintingCostEstimationId,
                Text = s.PrintingCostEstimationId + "-" + s.PrintJobType.ToString()
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult InventoryItems(string text, string itemCode)
        {
            var inventoryItems = db.InventoryItems.Where(x => x.ItemCategoryCode == itemCode);
            if (!string.IsNullOrWhiteSpace(text))
            {
                inventoryItems = inventoryItems.Where(x => x.ItemCode.Contains(text) || x.Name.Contains(text));
            }
            var result = inventoryItems.Select(s => new
            {
                Text = s.Name + "-" + s.ItemCode,
                Value = s.ItemCode
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult PrintMachineType(string text)
        {
            var printMachine = db.PrintingMachineTypes.ToList();
            if (!string.IsNullOrEmpty(text))
            {
                printMachine = printMachine.Where(x => x.MachineTypeName.Contains(text)).ToList();
            }
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
      
     
        public JsonResult PrintingProcess(PressCategory processCategory)
        {
            var printingProcess = db.PrintingProcesses.Where(x => x.PressCategory == processCategory);
            var result = printingProcess.Select(s => new
            {
                Text = s.PressCategory,
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
                Text = s.PrintPaperSize.ToString()
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
        public JsonResult GetPrintingProcess(string text, PressCategory printingProcessCategory)
        {
            var printingProcess = db.PrintingProcesses.Where(x => x.PressCategory == printingProcessCategory).AsQueryable();
            //if (!string.IsNullOrWhiteSpace(text))
            //{
            //    printingProcess = printingProcess.Where(x => x.LabourActivity.ToLower().Contains(text.ToLower())).AsQueryable();
            //}
            var result = printingProcess.Select(s => new
            {
                Value = s.PrintingProcessId,
                Text = s.LabourActivity
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUoM(string text) 
        {
            var unitOfm = db.UoMs.AsQueryable();
            if (!string.IsNullOrEmpty(text))
            {
                unitOfm = unitOfm.Where(x => x.UoMName.Contains(text));
            }
            return Json(unitOfm.Select(i => new
            {
                Text = i.UoMName,
                Value = i.UoMName
            })
            , JsonRequestBehavior.AllowGet);
        }


    }
}