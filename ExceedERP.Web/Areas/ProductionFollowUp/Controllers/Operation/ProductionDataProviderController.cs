using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.ProductionFollowUp.Controllers.Operation
{

    public class ProductionDataProviderController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        // GET: ProductionFollowUp/ProductionDataProvider
        public ActionResult Index()
        {
            return View();
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