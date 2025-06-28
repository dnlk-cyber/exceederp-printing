using ExceedERP.DataAccess.Context;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Common.ViewModel;

namespace ExceedERP.Web.Areas.CRM.Controllers.TenderManagement
{
	public class TenderDataProviderController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: TenderManagement/TenderDataProvider
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetAllCategories()
        {

            var salesLots = db.ProductCategories.Select(x => new { x.ProductCategoryId, x.Description }).ToList();
            return Json(salesLots, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetStoreByBranch(string id, string text)
        {
            int did = int.Parse(id);
            var store = db.InventoryStores.Where(x => x.Enable).Where(x => x.BranchId == did).AsQueryable();
            if (!string.IsNullOrEmpty(text))
                store = store.Where(x => x.StoreCode.Contains(text) || x.Name.Contains(text));
            return Json(store.Select(p => new
            {
                StoreCode = p.StoreCode,
                Name = p.StoreCode + "-" + p.Name
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetBranches()
        {
            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            var branches = db.Branch.Where(x => mybraches.Contains(x.BranchId.ToString())).ToList();
            return Json(branches.Select(x => new
            {
                Name = x.Name,
                Code = x.BranchId
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllBusinessUnits()
        {
            List<SelectorVm> list = new List<SelectorVm>();
            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                var businessUnits = db.BusinessUnits;
                foreach (var bu in businessUnits)
                {
                    var record = new SelectorVm();
                    record.Name = bu.Name;
                    record.Code = bu.BusinessUnitId.ToString();
                    list.Add(record);
                }
            }
            else
            {
                var record = new SelectorVm();
                record.Name = "No Business Unit";
                record.Code = "0";
                list.Add(record);
            }
            return Json(list.Select(x => new
            {
                Name = x.Name,
                Code = x.Code
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetBusinessUnits()
        {

            var branches = db.BusinessUnits;
            return Json(branches.Select(x => new
            {
                Name = x.Name,
                Id = x.BusinessUnitId
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTenders()
        {

            var tendersList = db.SaleTenders;
            return Json(tendersList.Select(x => new
            {
                Name = x.TenderName,
                Id = x.SaleTenderId
            }), JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetBranches()
        //{
        //    var branch = db.Branch;
        //    var result = branch.Select(s => new
        //    {
        //        Text = s.Name,
        //        Value = s.BranchId
        //    }).ToList();
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
        public JsonResult GetAllProducts(string categoryId)
        {
          // int categorId = int.Parse(categoryId);
            var Products = db.Products
                .Where(p => p.CategoryID == categoryId)
                .Select(prop => new { prop.ProductID, prop.Name }).Distinct().ToList();

            return Json(Products, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetParticipants(int salesLotId)
        {
            var participants = db.SalesLotPurchases.Where(sli=>sli.SalesLotId==salesLotId).ToList();
            var selectedParticipants = participants.Select(p=>new { Value=p.SalesLotPurchaseId,
                                                                    Text=p.CustomerName});
            return Json(selectedParticipants, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSalesLot()
        {
            var salesLots = db.SalesLots.ToList();

            return Json(salesLots.Select(p => new
            {
                SalesLotId = p.SalesLotId,
                LotName = p.LotName,
            }), JsonRequestBehavior.AllowGet);
        }
 
        public JsonResult GetSalesLotsTinNumbers()
        {
            // Get the participant


            // Get the sales lot items for the purchase
            var salesLots = db.SalesLotPurchases.Select(x=>new {x.SalesLotId,x.LotName}).Distinct().ToList();
            return Json(salesLots, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPurchasedTinNumbers(int salesLotID)
        {
            
            var TinNumbers = db.SalesLotPurchases
                .Where(p => p.SalesLotId == salesLotID)
                .Select(prop=>new { Value=prop.SalesLotPurchaseId,
                 Text=prop.CustomerName}).Distinct().ToList();               
            return Json(TinNumbers, JsonRequestBehavior.AllowGet);
            
        }


        public JsonResult GetSelectedCategories(int salesLotParticipantId)
        {
            // Get the participant
            var participant = db.SalesLotParticipants
                .SingleOrDefault(p => p.SalesLotParticipantId == salesLotParticipantId);

            if (participant == null)
            {
                return Json(new List<object>(), JsonRequestBehavior.AllowGet); // No participant found
            }
          
            var categories = db.SalesLotItems
                .Where(p => p.SalesLotId == participant.SalesLotId)
                .Select(p => new { p.ProductCategoryId, p.ProductCategoryDesc })
                .Distinct()
                .ToList();

            return Json(categories, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSelectedProducts( int salesLotParticipantId, int categoryId)
        {
            

            // Get the participant
            var participant = db.SalesLotParticipants
                .SingleOrDefault(p => p.SalesLotParticipantId == salesLotParticipantId);

            if (participant == null)
            {
                return Json(new List<object>(), JsonRequestBehavior.AllowGet); // No participant found
            }
            var products = db.SalesLotItems
                .Where(p => p.SalesLotId == participant.SalesLotId && p.ProductCategoryId == categoryId)
                .Select(p => new { p.ProductID, p.ProductName }).Distinct()
                .ToList();

            return Json(products, JsonRequestBehavior.AllowGet);
        }

   
    }
}