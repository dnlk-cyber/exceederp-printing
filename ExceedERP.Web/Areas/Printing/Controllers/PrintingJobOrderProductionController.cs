using ExceedERP.Core.Domain.Printing;
using ExceedERP.Core.Domain.Printing.PrintingProduction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Printing.Controllers
{
    public class PrintingJobOrderProductionController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: Printing/PrintingJobOrderProduction
        public ActionResult Index()
        {
            ViewData["Customer"] = db.OrganizationCustomers.Select(s => new
            {
                Value = s.OrganizationCustomerID,
                Text = s.TradeName
            }).ToList();


            ViewData["PaperSize"] = db.PrintingPaperSizes.Select(s => new
            {
                Value = s.PrintingPaperSizeId,
                Text = s.PaperSizeName
            }).ToList();
            ViewData["BindingStyle"] = db.PrintingBindingStyles.Select(s => new
            {
                Value = s.PrintingBindingStyleId,
                Text = s.BindingStyleName
            }).ToList();

            ViewData["PrintingMaterialCategory"] = db.PrintingMaterialCategories.Select(s => new
            {
                Value = s.PrintingMaterialCategoryId,
                Text = s.CategoryName
            }).ToList();
            ViewData["PItem"] = db.PrintingMaterialCategoryItems.Select(s => new
            {
                Value = s.PrintingMaterialCategoryItemId,
                Text = s.ItemName
            }).ToList();
            ViewData["PrintingProcess"] = db.PrintingProcesses.Select(s => new
            {
                Value = s.PrintingProcessId,
                Text = s.LabourActivity
            }).ToList();
            ViewData["PrintingMachineType"] = db.PrintingMachineTypes.Select(s => new
            {
                Value = s.PrintingMachineTypeId,
                Text = s.MachineTypeName
            }).ToList();

            //
            var categories = db.ItemCategorys;
            var items = db.InventoryItems;
            var costCenter = db.BranchCostCenter;
            var branches = db.Branch;

            var inventoryPeriods = db.InventoryPeriods;
            var selectedinventoryPeriods = inventoryPeriods.Select(
             i => new
             {
                 Name = i.Name,
                 GLPeriodId = i.InventoryPeriodId
             }).AsQueryable();
            ViewData["inventoryPeriods"] = selectedinventoryPeriods;

            ViewData["Branches"] = branches;

            var selecteditems = items.Select(
                i => new
                {
                    Name = i.ItemCode + "_" + i.Name,
                    ItemCode = i.ItemCode
                }).ToList();
            ViewData["Items"] = selecteditems;
            ViewData["Category"] = categories;
            ViewData["defaultCategory"] = categories.FirstOrDefault();



            var employee = db.Person_Employee;
            var selectedEmployee = employee.Select(
                i => new
                {
                    Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                    Code = i.EmployeeId
                }).AsQueryable();
            ViewData["personEmployees"] = selectedEmployee;

            var selectedcostCenter = costCenter.Select(
                i => new
                {
                    Name = i.MainCompanyStructure.Name,
                    Code = i.OrgStructureId
                }).AsQueryable();

            ViewData["costCenters"] = selectedcostCenter;
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            var categories = db.ItemCategorys;
            var items = db.InventoryItems;
            var stores = db.InventoryStores;
            var branches = db.Branch;
            var costCenter = db.BranchCostCenter;
            var selectedcostCenter = costCenter.Select(
              i => new
              {
                  Name = i.MainCompanyStructure.Name,
                  Code = i.OrgStructureId
              }).AsQueryable();

            var inventoryPeriods = db.InventoryPeriods;


            ViewData["costCenters"] = selectedcostCenter;
            ViewData["stores"] = stores;
            ViewData["Branches"] = branches;
            var selecteditems = items.Select(
                i => new
                {
                    Name = i.ItemCode + "_" + i.Name,
                    ItemCode = i.ItemCode
                }).ToList();
            ViewData["Items"] = selecteditems;
            ViewData["Category"] = categories;

            ViewData["defaultCategory"] = categories.FirstOrDefault();
        }
        public ActionResult PrintingJobOrderProduction_Read([DataSourceRequest]DataSourceRequest request)
        {
            var entity = db.PrintingJobOrderProductions;
            return Json(entity.ToDataSourceResult(request));
        }

        public ActionResult PrintingOrderProduction_Update([DataSourceRequest]DataSourceRequest request, PrintingJobOrderProduction model)
        {
            if (ModelState.IsValid)
            {
                db.PrintingJobOrderProductions.Attach(model);
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        #region Check&Approve
        public JsonResult CheckProduction(int id)
        {
            object response = null;
            var production = db.PrintingJobOrderProductions.FirstOrDefault(x => x.PrintingJobOrderProductionId == id);
            if (production != null)
            {
                var validity = CheckValidity(production.PrintingJobOrderProductionId);
                if (validity)
                {
                    production.IsSendForApproval = true;
                    production.SendForApprovalBy = User.Identity.Name;
                    production.SendForApprovalTime = DateTime.Today;

                    db.PrintingJobOrderProductions.Attach(production);
                    db.Entry(production).State = EntityState.Modified;
                    db.SaveChanges();
                    response = new { Success = true, Message = "Checked Successfully!" };
                }
                else
                {
                    response = new { Success = false, Message = "Something is wrong! Not Valid" };
                }
            }
            else
            {
                response = new { Success = false, Message = "Something is wrong!" };
            }
            return Json(response);
        }
        //If Validity needs to be checked before approval eg. proper store requisition etc
        private bool CheckValidity(int id)
        {
           // To-Do
            return true;
        }
        public JsonResult ApproveProduction(int id)
        {
            object response = null;
            var production = db.PrintingJobOrderProductions.Find(id);
            if (production != null)
            {

                production.IsOnlineApproved = true;
                production.OnlineApprovedBy = User.Identity.Name;
                production.OnlineApprovedTime = DateTime.Today;


                db.PrintingJobOrderProductions.Attach(production);
                db.Entry(production).State = EntityState.Modified;
                db.SaveChanges();
                //--Generate Productions--//
                GenerateProductionFollowUp(production);

                response = new { Success = true, Message = "Successfully Approved and Created Production!" };
            }
            else
            {
                response = new { Success = false, Message = "Something is wrong!" };
            }
            return Json(response);
        }

        private void GenerateProductionFollowUp(PrintingJobOrderProduction production)
        {
            if (production != null)
            {
                var jobFollowUp = new PrintingProductionFollowUp
                {
                    BindingStyle = production.BindingStyle,
                    CustomerId = production.CustomerId,
                    JobDescription = production.JobDescription,
                    PrintJobType = PrintJobType.Book,
                    PrintingJobOrderItemId = production.PrintingJobOrderItemId,
                    Pages = production.TotalTextNoPages,
                    TextPaperSizeId = production.TextPaperSizeId,
                    Quantity = production.Quantity,
                    PrintingOperationStatus = PrintingOperationStatus.Started,
                    CoverNoOfColor = production.CoverNoOfColor,
                    CoverPaperSizeId = production.CoverPaperSizeId,
                    TextNoOfColor = production.TextNoOfColors,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Today,
                    
                };
                db.PrintingProductionFollowUps.Add(jobFollowUp);
                db.SaveChanges();

                var materials = db.PrintingProductionMaterialCosts
                    .Where(x => x.PrintingJobOrderProductionId == production.PrintingJobOrderProductionId).ToList();
                foreach (var item in materials)
                {
                    var entity = new PrintingProductionAllottedRawMaterial
                    {
                        EstimationMaterialType = item.EstimationMaterialType,
                        PrintingMaterialCategoryId = item.PrintingMaterialCategoryId,
                        PrintingMaterialCategoryItemId = item.PrintingMaterialCategoryItemId,
                        UnitOfMeasurment = item.UnitOfMeasurment,
                        Quantity = item.Quantity,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Today,
                        PrintingProductionFollowUpId = jobFollowUp.PrintingProductionFollowUpId
                    };
                    db.PrintingProductionAllottedRawMaterials.Add(entity);
                }
                db.SaveChanges();
            }
        }
        #endregion
    }
}