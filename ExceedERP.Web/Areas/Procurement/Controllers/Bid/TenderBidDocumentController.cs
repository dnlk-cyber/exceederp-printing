using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Core.Domain.Procurement.Tender;
using ExceedERP.Core.Domain.Procurement.Tender.Lot;
using ExceedERP.Core.Domain.Procurement.Tender.Lots;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Bid
{
     

    public class TenderBidDocumentController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            PopulateCategories();
            return View();
        }

        private void PopulateCategories()
        {

            var categories = _db.ItemCategorys;
            var items = _db.InventoryItems;
            var tax = _db.GLTaxRates;

            ViewData["tax"] = tax;
            ViewData["Items"] = items;
            ViewData["Category"] = categories;
            ViewData["defaultCategory"] = categories.FirstOrDefault();

            var bidPurchasingSupplier = _db.BidPurchasingSupplierses.AsQueryable();

            ViewData["suppliers"] = bidPurchasingSupplier;
            ViewData["defaultsuppliers"] = bidPurchasingSupplier.FirstOrDefault();
            ViewData["defaultItems"] = items.FirstOrDefault();


        }

        public JsonResult GetDepartment()
        {
            var company = _db.MainCompanyStructures.FirstOrDefault(x => x.Parent == 0);
            var companies = _db.MainCompanyStructures.Where(x => x.Parent == company.StructureId);
            return Json(companies.Select(x => new { Department = x.Name }));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        public ActionResult TenderSendForApproval(int id)
        {

            Tender record = _db.Tenders.Find(id);

            // var record = masterDataItem;
            //  var entityType = ObjectContext.GetObjectType(record.GetType()); 

            if (!record.IsOnlineApproved)
            {


                if (ModelState.IsValid)
                {
                    record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                    _db.Tenders.Attach(record);
                    _db.Entry(record).State = EntityState.Modified;
                    _db.SaveChanges();
                }

            }

            return RedirectToAction("Index");
        }
        public JsonResult GetDepartments()
        {
            var persons = _db.Placements.AsQueryable();
            return Json(persons.Select(i => new { Supplier = i.PlacementId }), JsonRequestBehavior.AllowGet);

        }
        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        public ActionResult Send_For_Evaluation()
        {
            return RedirectToAction("Index");
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult Tenders_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Tender> tenders = _db.Tenders;
            DataSourceResult result = tenders.ToDataSourceResult(request, tender => new
            {
                TenderID = tender.TenderID,
                TenderReference = tender.TenderReference,
                Description = tender.Description,
                FloatDate = tender.FloatDate,
                ClosingDate = tender.ClosingDate,
                OpenigDate = tender.OpenigDate,
                ProcurementCategory = tender.ProcurementCategory,
                TenderType = tender.TenderType,
                PurchaseCategory = tender.PurchaseCategory,

                Status = Enum.GetName(typeof(OperationStatuses), 5),
                IsOnlineApproved = tender.IsOnlineApproved,
                OnlineApprovedBy = tender.OnlineApprovedBy,
                OnlineApprovedTime = tender.OnlineApprovedTime,
                IsVoid = tender.IsVoid,
                VoidBy = tender.VoidBy,
                VoidTime = tender.VoidTime,



                DateCreated = tender.DateCreated,
                LastModified = tender.LastModified,
                CreatedBy = tender.CreatedBy,
                ModifiedBy = tender.ModifiedBy


            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Tenders_Create([DataSourceRequest]DataSourceRequest request, Tender tender)
        {
            if (ModelState.IsValid)
            {
                #region validation

                bool valid = true;
                var exists = _db.Tenders.Where(x => x.TenderID != tender.TenderID)
                    .Where(x => x.TenderReference == tender.TenderReference).ToList();

                if (exists.Any())
                {
                    ModelState.AddModelError("", tender.TenderReference + " already exists in the system.");
                    valid = false;
                }

                #endregion

                if (valid)
                {
                    var entity = new Tender
                    {
                        TenderReference = tender.TenderReference,
                        Description = tender.Description,
                        FloatDate = tender.FloatDate,
                        ClosingDate = tender.ClosingDate,
                        OpenigDate = tender.OpenigDate,
                        Status = Enum.GetName(typeof(OperationStatuses), 5),
                        IsOnlineApproved = tender.IsOnlineApproved,
                        OnlineApprovedBy = tender.OnlineApprovedBy,
                        OnlineApprovedTime = tender.OnlineApprovedTime,
                        IsVoid = tender.IsVoid,
                        VoidBy = tender.VoidBy,
                        VoidTime = tender.VoidTime,

                        ProcurementCategory = tender.ProcurementCategory,
                        TenderType = tender.TenderType,

                        DateCreated = DateTime.Now.Date,
                        LastModified = tender.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = tender.ModifiedBy

                    };

                    _db.Tenders.Add(entity);
                    _db.SaveChanges();
                    tender.TenderID = entity.TenderID;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "tender bid document", "Success", User.Identity.Name, $"created document under {entity.TenderID} under record {entity.TenderID} ", Modules.Procurement);
                    #endregion
                }
            }

            return Json(new[] { tender }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Tenders_Update([DataSourceRequest]DataSourceRequest request, Tender tender)
        {
            if (ModelState.IsValid)
            {
                #region validation

                bool valid = true;
                var exists = _db.Tenders.Where(x => x.TenderID != tender.TenderID)
                    .Where(x => x.TenderReference == tender.TenderReference).ToList();

                if (exists.Any())
                {
                    ModelState.AddModelError("", tender.TenderReference + " already exists in the system.");
                    valid = false;
                }

                #endregion

                if (valid)
                {
                    var entity = new Tender
                    {
                        TenderID = tender.TenderID,
                        TenderReference = tender.TenderReference,
                        TenderType = tender.TenderType,
                        Description = tender.Description,
                        FloatDate = tender.FloatDate,
                        ClosingDate = tender.ClosingDate,
                        OpenigDate = tender.OpenigDate,
                        Status = tender.Status,
                        ProcurementCategory = tender.ProcurementCategory,
                        IsOnlineApproved = tender.IsOnlineApproved,
                        OnlineApprovedBy = tender.OnlineApprovedBy,
                        OnlineApprovedTime = tender.OnlineApprovedTime,
                        IsVoid = tender.IsVoid,

                        VoidBy = tender.VoidBy,
                        VoidTime = tender.VoidTime,
                        OrgBranchName = tender.OrgBranchName,
                        DateCreated = tender.DateCreated,
                        LastModified = DateTime.Now.Date,
                        CreatedBy = tender.CreatedBy,
                        ModifiedBy = User.Identity.Name

                    };

                    _db.Tenders.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "tender bid document", "Success", User.Identity.Name, $"updated document under {entity.TenderID} under record {entity.TenderID} ", Modules.Procurement);
                    #endregion
                }
            }

            return Json(new[] { tender }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Tenders_Destroy([DataSourceRequest]DataSourceRequest request, Tender tender)
        {
            if (ModelState.IsValid)
            {
                var entity = new Tender
                {
                    TenderID = tender.TenderID,
                    TenderReference = tender.TenderReference,
                    Description = tender.Description,
                    FloatDate = tender.FloatDate,
                    ClosingDate = tender.ClosingDate,
                    OpenigDate = tender.OpenigDate,
                    Status = tender.Status,
                    TenderType = tender.TenderType,
                    ProcurementCategory = tender.ProcurementCategory,
                    IsOnlineApproved = tender.IsOnlineApproved,
                    OnlineApprovedBy = tender.OnlineApprovedBy,
                    OnlineApprovedTime = tender.OnlineApprovedTime,
                    IsVoid = tender.IsVoid,
                    VoidBy = tender.VoidBy,

                    VoidTime = tender.VoidTime,
                    OrgBranchName = tender.OrgBranchName,
                    DateCreated = tender.DateCreated,
                    LastModified = tender.LastModified,
                    CreatedBy = tender.CreatedBy,
                    ModifiedBy = tender.ModifiedBy
                };

                _db.Tenders.Attach(entity);
                _db.Tenders.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { tender }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }
        

        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }


        public JsonResult GetCriteriaType(string id)
        {
            int lotId = int.Parse(id);
            Lot lot = _db.Lots.Find(lotId);
            string result = "None";
            if (lot != null)
            {
                if (lot.EvaluationCriterias != null)
                {
                    if (lot.EvaluationCriterias.Any(x => x.Type == CriteriaType.Technical))
                    {
                        result = "Technical";
                    }
                    else if (lot.EvaluationCriterias.Any(x => x.Type == CriteriaType.Pass_Fail))
                    {
                        result = "PassFail";
                    }
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
