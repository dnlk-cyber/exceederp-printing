using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Printing.Controllers.ProductionFollowUp.Operation
{

    public class ProductionDataProviderController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        // GET: ProductionFollowUp/ProductionDataProvider


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAllFixedAssets()
        {
            var data = db.FixedAssetSubCategoryNames.ToList();
            return Json(data.Select(s => new { Code = s.FixedAssetSubCategoryNameId, Name = s.Name }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAllProductionCustomerTypes()
        {
            var data = db.ProductionCustomerTypes;
            return Json(data.Select(s => new { Code = s.ProductionCustomerTypeId, Name = s.Name }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetBeforeVatAndTotalCost(int jobId, int quant=0)
        {
            var job = db.Jobs.Find(jobId);
            var jobType = db.JobCategories.FirstOrDefault(x => x.JobCategoryId.ToString() == job.JobTypeId.ToString());
            if (jobType != null)
            {
                var tax = db.GLTaxRates.FirstOrDefault(x => x.GLTaxRateID == jobType.GLTaxRateID);
                if (tax != null)
                {
                    double beforeTax = quant * job.UnitPrice;
                    double withTax = (quant * job.UnitPrice) + (quant * job.UnitPrice * Convert.ToDouble(tax.CalculatedRate));
                    double vat = withTax - beforeTax;
                    var calculatedField = Json(new
                    {
                        Status = "OK",
                        Amount = Math.Round(beforeTax, 3),
                        VAT = Math.Round(vat, 3),
                        Total = Math.Round(withTax, 3),
                        Unit = Math.Round(job.UnitPrice, 3)
                    });

                    return calculatedField;
                }
            }

            var nodata = Json(new
            {
                Status = "OK",
                Amount = 0,
                VAT = 0,
                Total = 0,
                Unit = 0
            });

            return nodata;

        }

        public JsonResult GetJobOrders(int customerId)
        {
            var jobOrders = db.Jobs.Where(q=>q.CustomerId==customerId).ToList();

            return Json(jobOrders.Select(p => new
            {
                JobId = p.JobId,
                JobNo = p.JobNo,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetItemCategory()
        {
            var itemCategory = db.ItemCategorys
                .Where(x => x.HaveParent == false)
                .ToList();//exclude sub categories
            return Json(itemCategory.Select(p => new
            {
                Name = p.ItemCategoryCode + "-" + p.Name,
                ItemCategoryCode = p.ItemCategoryCode
            }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPeriodDateRangeById(int id)
        {

            var glPeriod = db.InventoryPeriods.Find(id);
            if (glPeriod != null)
            {
                var formattedData = new
                {
                    Min = glPeriod.DateFrom,
                    Max = glPeriod.DateTo
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);

            }

            return null;
        }
        public JsonResult GetAllCustomers()
        {
            var customers = db.OrganizationCustomers.Select(s => new {
                Code = s.OrganizationCustomerID,
                Name = s.TradeName
            }).ToList();
            return Json(customers,JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllUnits()
        {
            var units = db.UoMs.Select(s => new {
                Code = s.UoMName,
                Name = s.UoMName
            }).ToList();
            return Json(units, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetWorkUnitCostCenters()
        {
            var costCenter = db.MainCompanyStructures;
            return
                Json(
                    costCenter.Select(
                        i => new
                        {
                            Name = i.Name,
                            Code = i.OrgStructureId
                        }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStoreByKeeper()
        {
            //List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            //var store = db.InventoryStores.Where(x => mybraches.Contains(x.BranchId.ToString())).Where(x => x.Enable).AsQueryable();

            // List<string> userStores = UserInformationHelper.GetUserStores(User.Identity.Name);
            var store = db.InventoryStores.Where(x => x.Enable).AsQueryable();

            return Json(store.Select(p => new
            {
                StoreCode = p.StoreCode,
                Name = p.StoreCode + "-" + p.Name
            }), JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetItemsUnderCategory(string ItemCategoryCode)
        {

            var inventoryItems = db.InventoryItems.Where(q => q.ItemCategoryCode == ItemCategoryCode).ToList().Select(s => new
            {

                s.ItemCode,
                s.Name
            });
            return Json(inventoryItems, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllStoresByBranch(string BranchId)
        {
            var store = db.InventoryStores.Where(x => x.Enable && x.BranchId.ToString() == BranchId).AsQueryable();
            return Json(store.Select(p => new
            {
                StoreCode = p.StoreCode,
                Name = p.StoreCode + "-" + p.Name
            }), JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetUnitPrice(string itemCode)
        {
            var item = db.StoreItemAssignments.Where(q=>q.ItemCode== itemCode).FirstOrDefault();

            return Json(new { item .ItemCode, item.AvgPrice } , JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllBranches()
        {
            var branches = db.Branch.Select(s => new
            {
                s.Code,
                s.BranchId,
                s.Name
            }).ToList();
            return Json(branches, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllStores()
        {
            var stores = db.InventoryStores.Select(s => new
            {
                s.StoreCode,
                s.Name
            }).ToList();
            return Json(stores, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCustomers()
        {
            var stores = db.OrganizationCustomers.Select(s => new
            {
                Code = s.OrganizationCustomerID,
                Name = s.TradeName
            }).ToList();
            return Json(stores, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetOpenInventoryPeriodsByStore(string store)
        {
            var periodStatuses = db.InventoryStorePeriodStatuses
                                .Where(x => x.StoreCode == store)
                                .Where(x => x.Status == FiscalYearStatus.Open)
                                .AsQueryable();

            List<SelectorVm> list = new List<SelectorVm>();

            foreach (var period in periodStatuses)
            {
                var p = new SelectorVm
                {
                    Code = period.InventoryPeriodId.ToString(),
                    Name = db.InventoryPeriods.Find(period.InventoryPeriodId)?.Name
                };
                list.Add(p);
            }
            return Json(list.Select(p => new
            {
                GLPeriodId = p.Code,
                Name = p.Name
            }), JsonRequestBehavior.AllowGet);

        }
        public class SelectorVm
        {
            public string Name { get; set; }
            public string Code { get; set; }
        }
        public JsonResult GetEmployes()
        {
            var employees = db.Person_Employee.Select(
                i => new
                {
                    Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                    Code = i.EmployeeId
                }).AsQueryable();
            return Json(employees, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetOrgnizationalCustomers()
        {
            var customers = db.OrganizationCustomers.ToList().Select(s => new
            {

                s.OrganizationCustomerID,
                s.TradeName
            });
            return Json(customers, JsonRequestBehavior.AllowGet);
        }
    }
}