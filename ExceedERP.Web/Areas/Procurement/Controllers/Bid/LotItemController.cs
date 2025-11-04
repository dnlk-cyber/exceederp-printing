using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Core.Domain.Procurement.Tender.Lot;
using ExceedERP.Core.Domain.Procurement.Tender.Lots;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Bid
{


    public class LotItemController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult LotItems_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<LotItems> lotitems = _db.LotItems.Where(l => l.LotID == id);
            DataSourceResult result = lotitems.ToDataSourceResult(request, lotItems => new
            {
                LotItemsID = lotItems.LotItemsID,
                LotID = lotItems.LotID,
                CatCode = lotItems.CatCode,// + "-" + lotItems.CategoryName,
                CategoryName = lotItems.CategoryName,
                ItmCode = lotItems.ItmCode,// + "-" + lotItems.ItemName,
                ItemName = lotItems.ItemName,
                Name = lotItems.Name,
                ModelNo = lotItems.ModelNo,
                PartNo = lotItems.PartNo,
                Quantity = lotItems.Quantity,
                IsApproved = lotItems.IsApproved,
                ItemSpecification = lotItems.ItemSpecification,
                UoM = lotItems.UoM,
                ItemType = lotItems.ItemType,
                OrgBranchName = lotItems.OrgBranchName,
                DateCreated = lotItems.DateCreated,
                LastModified = lotItems.LastModified,
                CreatedBy = lotItems.CreatedBy,
                ModifiedBy = lotItems.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LotItems_Create([DataSourceRequest]DataSourceRequest request, LotItems lotItems, int id)
        {
           
            if (ModelState.IsValid)
            {
                var entity = new LotItems
                {
                    LotID = id,
                    CatCode = lotItems.CatCode,
                    CategoryName = lotItems.CategoryName,
                    ItmCode = lotItems.ItmCode,
                    ItemName = lotItems.ItemName,
                    Name = lotItems.Name,
                    ModelNo = lotItems.ModelNo,
                    PartNo = lotItems.PartNo,
                    Quantity = lotItems.Quantity,
                    IsApproved = lotItems.IsApproved,
                    ItemSpecification = lotItems.ItemSpecification,
                    UoM = lotItems.UoM,
                    ItemType = lotItems.ItemType,
                    OrgBranchName = lotItems.OrgBranchName,
                    LastModified = lotItems.LastModified,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    ModifiedBy = lotItems.ModifiedBy
                };

                _db.LotItems.Add(entity);
                _db.SaveChanges();
                lotItems.LotItemsID = entity.LotItemsID;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "lot item", "Success", User.Identity.Name, $"created item {entity.Name} under record {entity.LotID} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { lotItems }.ToDataSourceResult(request, ModelState));
        }


        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LotItems_Update([DataSourceRequest]DataSourceRequest request, LotItems lotItems)
        {
            if (ModelState.IsValid)
            {
                var entity = new LotItems
                {
                    LotItemsID = lotItems.LotItemsID,
                    LotID = lotItems.LotID,
                    CatCode = lotItems.CatCode,
                    CategoryName = lotItems.CategoryName,
                    ItmCode = lotItems.ItmCode,
                    ItemName = lotItems.ItemName,
                    Name = lotItems.Name,
                    ModelNo = lotItems.ModelNo,
                    PartNo = lotItems.PartNo,
                    Quantity = lotItems.Quantity,
                    IsApproved = lotItems.IsApproved,
                    ItemSpecification = lotItems.ItemSpecification,
                    UoM = lotItems.UoM,
                    ItemType = lotItems.ItemType,
                    OrgBranchName = lotItems.OrgBranchName,
                    DateCreated = lotItems.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = lotItems.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                _db.LotItems.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "lot item", "Success", User.Identity.Name, $"updated item {entity.Name} under record {entity.LotID} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { lotItems }.ToDataSourceResult(request, ModelState));
        }


        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LotItems_Destroy([DataSourceRequest]DataSourceRequest request, LotItems lotItems)
        {
            if (ModelState.IsValid)
            {
                var entity = new LotItems
                {
                    LotItemsID = lotItems.LotItemsID,
                    CatCode = lotItems.CatCode,
                    CategoryName = lotItems.CategoryName,
                    ItmCode = lotItems.ItmCode,
                    ItemName = lotItems.ItemName,
                    PartNo = lotItems.PartNo,
                    Quantity = lotItems.Quantity,
                    IsApproved = lotItems.IsApproved,
                    ItemSpecification = lotItems.ItemSpecification,
                    UoM = lotItems.UoM,
                    ItemType = lotItems.ItemType,
                    OrgBranchName = lotItems.OrgBranchName,
                    DateCreated = lotItems.DateCreated,
                    LastModified = lotItems.LastModified,
                    CreatedBy = lotItems.CreatedBy,
                    ModifiedBy = lotItems.ModifiedBy
                };

                _db.LotItems.Attach(entity);
                _db.LotItems.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { lotItems }.ToDataSourceResult(request, ModelState));
        }


        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }
        [AuthorizeRoles(ProcurementRoles.ProcurementAdmin)]

        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        public JsonResult GetLotItems(string id)
        {
            int lotid = int.Parse(id);
            var lotitems = _db.LotItems.Where(x=>x.LotID==lotid);

            return Json(lotitems.Select(i => new
            {
                Name = i.ItemName, 
                ItemCode = i.ItmCode
            }), JsonRequestBehavior.AllowGet);
        }

       

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
