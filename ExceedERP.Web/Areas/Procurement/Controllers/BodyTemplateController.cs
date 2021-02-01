using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.Tender;
using ExceedERP.Core.Domain.Procurement.Tender.Lot;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.Procurement.Models;

namespace ExceedERP.Web.Areas.Procurement.Controllers
{
    public class BodyTemplateController : Controller
    {
        ExceedDbContext db =  new ExceedDbContext();
        // GET: Procurement/BodyTemplate
        public ActionResult Index(string id)
        {
            int lotId = int.Parse(id);
            Lot lot = db.Lots.Find(lotId);

            var supplier = lot.BidAwardedSupplier;
            var participantItem = lot.ParticipantsItems;
            var viewModel = new List<AssignItemToParticipant>();


            foreach (var bidAwardedSupplier in participantItem)
            {
                AssignItemToParticipant items = new AssignItemToParticipant();
                InventoryItem item = db.InventoryItems.Find(bidAwardedSupplier.ItmCode);
                items.ItemCode = bidAwardedSupplier.ItmCode;
                if (item != null) items.ItemName = item.Name;

                items.Assigned = false;
                viewModel.Add(items);
            }

            ViewBag.SupplierItem = viewModel;
            return View();
        }

        //public ActionResult Index(string id ,string[] selectedItem, string[] technical)
        //{
               
        //    return RedirectToAction("Index", "Tender");
        //}
     }
}