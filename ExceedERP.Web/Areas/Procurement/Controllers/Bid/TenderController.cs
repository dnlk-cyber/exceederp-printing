using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.AP;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.Contract;
using ExceedERP.Core.Domain.Procurement.Proforma;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Core.Domain.Procurement.Tender;
using ExceedERP.Core.Domain.Procurement.Tender.Lot;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Areas.Finance.Models;
using ExceedERP.Web.Areas.Procurement.Models;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.PurchaseOrder;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Bid
{
    

    public class TenderController : Controller
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

            var stores = _db.InventoryStores;
            var receivetype = _db.ReceiveTypes;
            var items = _db.LotItems;
            var tax = _db.GLTaxRates;
            var branches = _db.Branch;

            ViewData["Branches"] = branches;

            ViewData["tax"] = tax;


            var selecteditems = items.DistinctByField(f => f.ItmCode).Select(
                i => new
                {
                    Name = i.ItmCode + "_" + i.ItemName,
                    ItmCode = i.ItmCode
                }).AsQueryable();

            ViewData["Items"] = selecteditems;

            var lotitems = items.Select(
                i => new
                {
                    Name =  i.ItmCode + "_" + i.ItemName + "_" + i.Name,
                    LotItemID = i.LotItemsID
                }).AsQueryable();

            ViewData["LotItems"] = lotitems;

            var selectedcatedories = items.DistinctByField(c => c.CatCode).Select(
                i => new
                {
                    Name = i.CatCode + "_" + i.CategoryName,
                    CatCode = i.CatCode
                }).AsQueryable();
            ViewData["Category"] = selectedcatedories;
            ViewData["defaultCategory"] = selectedcatedories.FirstOrDefault();

            var bidPurchasingSupplier = _db.BidPurchasingSupplierses.AsQueryable();

            ViewData["suppliers"] = bidPurchasingSupplier;

            var employee = _db.Person_Employee;
            var selectedEmployee = employee.Select(
                i => new
                {
                    Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                    Code = i.EmployeeId
                }).AsQueryable();
            ViewData["personEmployees"] = selectedEmployee;

            ViewData["stores"] = stores;
            ViewData["ReeceiveType"] = receivetype;

            var inventoryPeriods = _db.InventoryPeriods;
            var selectedinventoryPeriods = inventoryPeriods.Select(
                i => new
                {
                    Name = i.Name,
                    GLPeriodId = i.InventoryPeriodId
                }).AsQueryable();
            ViewData["inventoryPeriods"] = selectedinventoryPeriods;


        }

        private bool CheckStatus(DateTime date)
        {
            int days = (date - DateTime.Now).Days;
            if (days > 0 && days <= 3)
                return true;
            else
                return false;
        }
        public JsonResult GetDepartment()
        {
            var company = _db.MainCompanyStructures.FirstOrDefault(x => x.Parent == 0);
            var companies = _db.MainCompanyStructures.Where(x => x.Parent == company.StructureId);
            return Json(companies.Select(x => new { Department = x.Name }));
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

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        public ActionResult Tenders_Read([DataSourceRequest]DataSourceRequest request)
        {
            int empid = UserInformationHelper.GetUserEmployeeId(User.Identity.Name);
            IQueryable<Tender> tenders = _db.Tenders;//.Where(x => x.ProcurementOfficer == empid && empid != 0);
            DataSourceResult result = tenders.ToDataSourceResult(request, tender => new
            {
                TenderID = tender.TenderID,
                ProcurementOfficer=tender.ProcurementOfficer,
                BranchId = tender.BranchId,
                BusinessUnitId = tender.BusinessUnitId,
                TenderReference = tender.TenderReference,
                Description = tender.Description,
                FloatDate = tender.FloatDate,
                ClosingDate = tender.ClosingDate,
                OpenigDate = tender.OpenigDate,
                TechnicalOpenigDate = tender.TechnicalOpenigDate,
                FinancialOpenigDate = tender.FinancialOpenigDate,
                ProcurementCategory = tender.ProcurementCategory,
                TenderType = tender.TenderType,
                PurchaseCategory = tender.PurchaseCategory,
                IsCancelled = tender.IsCancelled,
                PurchaseStatus = tender.PurchaseStatus,

                Status = tender.Status,
                IsVoid = tender.IsVoid,
                VoidBy = tender.VoidBy,
                VoidTime = tender.VoidTime,
                IsOnlineApproved = tender.IsOnlineApproved,
                OnlineApprovedBy = tender.OnlineApprovedBy,
                OnlineApprovedTime = tender.OnlineApprovedTime,
                IsSendForApproval = tender.IsSendForApproval,
                SendForApprovalBy = tender.SendForApprovalBy,
                SendForApprovalTime = tender.SendForApprovalTime,
                IsOnlineTransferred = tender.IsOnlineTransferred,
                IsOnlineTransferredBy = tender.IsOnlineTransferredBy,
                OnlineTransferredTime = tender.OnlineTransferredTime,
                CloseStatus = CheckStatus(tender.ClosingDate),
                OpenStatus = CheckStatus(tender.OpenigDate),


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
            #region validation
            if(tender.FloatDate>tender.ClosingDate)
                ModelState.AddModelError("float","Tender Float date should be before closing date");

            if (tender.ClosingDate >= tender.OpenigDate)
                ModelState.AddModelError("float", "Tender closing date should be before opening date");

            //if (tender.TechnicalOpenigDate!=null && tender.TechnicalOpenigDate <= tender.OpenigDate)
            //    ModelState.AddModelError("tech", "Technical Opening Date should be after opening date");

            //if (tender.FinancialOpenigDate != null && tender.FinancialOpenigDate < tender.OpenigDate)
            //    ModelState.AddModelError("tech", "Financial Opening Date should be after opening date");

            var exists = _db.Tenders.Where(x => x.TenderReference == tender.TenderReference).ToList();

            if (exists.Any())
            ModelState.AddModelError("", tender.TenderReference + " already exists in the system.");
            #endregion

            if (ModelState.IsValid)
            {
                 var entity = new Tender
                    {
                     ProcurementOfficer = UserInformationHelper.GetUserEmployeeId(User.Identity.Name),
                     BranchId = UserInformationHelper.GetUserDefaultBranchId(User.Identity.Name),
                     BusinessUnitId = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name),
                     TenderReference = tender.TenderReference,
                     Description = tender.Description,
                        FloatDate = tender.FloatDate,
                        ClosingDate = tender.ClosingDate,
                        OpenigDate = tender.OpenigDate,
                        TechnicalOpenigDate = tender.TechnicalOpenigDate,
                        FinancialOpenigDate = tender.FinancialOpenigDate,
                        Status = Enum.GetName(typeof(OperationStatuses), 5),
                        PurchaseStatus = tender.PurchaseStatus,

                       IsOnlineApproved = tender.IsOnlineApproved,
                        OnlineApprovedBy = tender.OnlineApprovedBy,
                        OnlineApprovedTime = tender.OnlineApprovedTime,
                        IsVoid = tender.IsVoid,
                        VoidBy = tender.VoidBy,
                        VoidTime = tender.VoidTime,

                        ProcurementCategory = tender.ProcurementCategory,
                        PurchaseCategory=tender.PurchaseCategory,
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
                UserHelper.OperationLog(Request.UserHostAddress, "Tender", "Success", User.Identity.Name, $"created tender {entity.TenderID} ", Modules.Procurement);
                #endregion

            }

            return Json(new[] { tender }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Tenders_Update([DataSourceRequest]DataSourceRequest request, Tender tender)
        {
            #region validation

            if (tender.FloatDate > tender.ClosingDate)
                ModelState.AddModelError("float", "Tender Float date should be before closing date");
            if (tender.ClosingDate >= tender.OpenigDate)
                ModelState.AddModelError("float", "Tender closing date should be before opening date");

            //if (tender.TechnicalOpenigDate != null && tender.TechnicalOpenigDate <= tender.OpenigDate)
            //    ModelState.AddModelError("tech", "Technical Opening Date should be after opening date");

            //if (tender.FinancialOpenigDate != null && tender.FinancialOpenigDate < tender.OpenigDate)
            //    ModelState.AddModelError("tech", "Financial Opening Date should be after opening date");

            var exists = _db.Tenders.Where(x => x.TenderID != tender.TenderID)
                .Where(x => x.TenderReference == tender.TenderReference).ToList();

            if (exists.Any())
                ModelState.AddModelError("", tender.TenderReference + " already exists in the system.");

            #endregion
            if (ModelState.IsValid)
            {
               
                var entity = new Tender
                {
                    TenderID = tender.TenderID,
                    ProcurementOfficer = tender.ProcurementOfficer,
                    BranchId =tender.BranchId,
                    BusinessUnitId = tender.BusinessUnitId,
                    TenderReference = tender.TenderReference,
                    Description = tender.Description,
                    FloatDate = tender.FloatDate,
                    ClosingDate = tender.ClosingDate,
                    OpenigDate = tender.OpenigDate,
                    TechnicalOpenigDate = tender.TechnicalOpenigDate,
                    FinancialOpenigDate = tender.FinancialOpenigDate,
                    Status = tender.Status,
                    ProcurementCategory = tender.ProcurementCategory,
                    PurchaseCategory = tender.PurchaseCategory,
                    TenderType = tender.TenderType,
                    PurchaseStatus = tender.PurchaseStatus,

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
                UserHelper.OperationLog(Request.UserHostAddress, "Tender", "Success", User.Identity.Name, $"updated tender {entity.TenderID} ", Modules.Procurement);
                #endregion

            }

            return Json(new[] { tender }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Tenders_Destroy([DataSourceRequest]DataSourceRequest request, Tender tender)
        {
            #region validation
            var exists = _db.Lots.Where(x => x.TenderID == tender.TenderID).ToList();
            if (exists.Any())
            {
                ModelState.AddModelError("", $" Tender {tender.TenderReference } can not be deleted ,it has child items.");
            }

            

            #endregion
            if (ModelState.IsValid)
            {
                var entity = new Tender
                {
                    TenderID = tender.TenderID,
                    ProcurementOfficer = tender.ProcurementOfficer,
                    BranchId = tender.BranchId,
                    BusinessUnitId = tender.BusinessUnitId,
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

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        public JsonResult SendForApproval(int id)
        {
            var record = _db.Tenders.Find(id);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved && record.Lots.Count > 0)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                _db.Tenders.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();
                var formattedData = new
                {
                    Message = "Successfully Sent.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record == null)
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.Lots.Count == 0)
            {
                var formattedData = new
                {
                    Message = "Please add lot first.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IsSendForApproval)
            {
                var formattedData = new
                {
                    Message = "The record is already sent for approval you cannot send again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is already approved.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        public JsonResult SendToContract(int id)
        {
            var record = _db.Tenders.Find(id);
            if (record != null &&record.IsOnlineApproved && record.Lots.Count > 0)
            {
                TransferToContract(id);
                var formattedData = new
                {
                    Message = "Successfully Sent.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record == null)
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.Lots.Count == 0)
            {
                var formattedData = new
                {
                    Message = "Please add lot first.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }


            if (!record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is not approved.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementVoid)]
        public JsonResult Void(int id)
        {
            var record = _db.Tenders.Find(id);
            if (record != null && record.IsOnlineApproved && !record.IsVoid)
            {
                #region get advance status

                var lots = _db.Lots.Where(x => x.TenderID == record.TenderID).ToList();
                foreach (var lot in lots)
                {
                    var award = _db.BidAwardedSuppliers.Where(x => x.LotID == lot.LotID).ToList();
                    foreach (var awardedSupplier in award)
                    {
                        var advance = _db.APPayments
                            .Where(x => x.PoTypeEnum == PurchaseType.TenderAward)
                            .Where(x => x.IsVoid == false)
                            .Where(x => x.IsOnlineApproved)
                            .FirstOrDefault(x => x.PaymentReference == awardedSupplier.BidAwardedSupplierID.ToString());
                        if (advance!=null)
                        {
                            var formattedData1 = new
                            {
                                Message = "You can't void this record since a related advance payment is already applied.",
                                Status = "NOT_OK"
                            };
                            return Json(formattedData1, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                #endregion 

                record.Status = Enum.GetName(typeof(OperationStatuses), 10);
                record.IsOnlineApproved = false;
                record.IsSendForApproval = false;
                record.IsVoid = true;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                _db.Tenders.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();

                var formattedData = new
                {
                    Message = "Void Successful.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record == null)
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

           
            if (!record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is not approved hence you can modify by editing.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }


            return null;
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

        //public JsonResult GetParticipantItem(string id)
        //{
        //    int lotId = int.Parse(id);
        //    var lot = _db.Lots.Find(lotId);
        //    if (lot != null)
        //    {
        //        //not to lose data on refresh

        //        //_db.ParticipantsItems.RemoveRange(lot.ParticipantsItems);
        //        //_db.SaveChanges();

        //        #region items
        //        var suppliers = _db.BidPParticipatingSupplierses.Where(x => x.LotID == lotId).ToList();

        //        var evalbydoc = _db.LotEvaluationByDocuments
        //            .Where(x => x.LotID == lotId)
        //            .Where(x => x.IsAvailable).ToList();

        //        foreach (var supplier in suppliers)
        //        {
        //            //filter eval by doc
        //            var exist = evalbydoc.Where(x => x.SupplierName == supplier.SupplierName).ToList();
        //            if (exist.Any())
        //                continue;

        //            var lotitems = _db.LotItems.Where(x => x.LotID == lotId).ToList();

        //            var evaluations = _db.TenderEvaluationCriterias.Where(x => x.LotID == lotId).ToList();

        //            foreach (var lotitem in lotitems)
        //            {
        //                #region jump failed ones

        //                var passfailCriteria = evaluations.FirstOrDefault(x => x.Type == CriteriaType.Pass_Fail);
        //                if (passfailCriteria != null)
        //                {
        //                    if (lotitem.ItemType == PurchaseItemType.FixedAsset)
        //                    {
        //                        var proformPassFail = _db.TenderPassFailResult.Where(
        //                            x => x.LotID == lotId && x.ItmCode == lotitem.ItmCode && 
        //                            x.Name == lotitem.Name &&
        //                             x.ModelNo == lotitem.ModelNo &&
        //                                 x.SupplierName == supplier.SupplierName).FirstOrDefault();

        //                        if (proformPassFail?.Result == PassFailStatus.Fail)
        //                            continue;

        //                    }
        //                    else
        //                    {
        //                        var proformPassFail = _db.TenderPassFailResult
        //                            .Where(x=>x.SupplierName == supplier.SupplierName)
        //                             .Where(x => x.LotID == lotId)
        //                              .Where(x => x.ItmCode == lotitem.ItmCode)
        //                              .Where(x => x.ItemSpecification == lotitem.ItemSpecification)//added
        //                            .FirstOrDefault();

        //                        if (proformPassFail?.Result == PassFailStatus.Fail)
        //                            continue;
        //                    }
                            
        //                }

        //                #endregion

        //                if (lotitem.ItemType == PurchaseItemType.FixedAsset)
        //                {
        //                    //if exists jump
        //                    var exists = _db.BidParticipatingSupplierItems.Where(
        //                        x => x.BidPParticipatingSupplier.LotID == lotId 
        //                        && x.ItmCode == lotitem.ItmCode 
        //                         && x.Name == lotitem.Name
        //                          && x.ModelNo == lotitem.ModelNo
        //                         && x.SupplierName == supplier.SupplierName).FirstOrDefault();

        //                    if (exists == null)
        //                    {
        //                        var item = new ParticipantsItem
        //                        {
        //                            CatCode = lotitem.CatCode,
        //                            CategoryName = lotitem.CategoryName,
        //                            ItmCode = lotitem.ItmCode,
        //                            ItemName = lotitem.ItemName,
        //                            Name = lotitem.Name,
        //                            ModelNo = lotitem.ModelNo,
        //                            ItemSpecification = lotitem.ItemSpecification,
        //                            UoM = lotitem.UoM,
        //                            SupplierId = supplier.SupplierId,
        //                            IsNewSupplier = supplier.IsNewSupplier,
        //                            SupplierName = supplier.SupplierName,
        //                            LotID = lotId,
        //                            GLTaxRateID = 0,
        //                            Quantity = lotitem.Quantity,
        //                            LotItemID=lotitem.LotItemsID
        //                        };
        //                        _db.ParticipantsItems.Add(item);
        //                        _db.SaveChanges();
        //                    }
        //                }
        //                else
        //                {
        //                    //if exists jump
        //                    var exists = _db.ParticipantsItems
        //                                .Where(x => x.SupplierName == supplier.SupplierName)
        //                               .Where(x => x.LotID == lotId)
        //                              .Where(x => x.ItmCode == lotitem.ItmCode)
        //                              .Where(x => x.ItemSpecification == lotitem.ItemSpecification)//added
        //                            .FirstOrDefault();

        //                    if (exists == null)
        //                    {
        //                        var item = new ParticipantsItem
        //                        {
        //                            CatCode = lotitem.CatCode,
        //                            CategoryName = lotitem.CategoryName,
        //                            ItmCode = lotitem.ItmCode,
        //                            ItemName = lotitem.ItemName,
        //                            ItemSpecification = lotitem.ItemSpecification,
        //                            Name = lotitem.Name,
        //                            ModelNo = lotitem.ModelNo,
        //                            UoM =lotitem.UoM,
        //                            SupplierId = supplier.SupplierId,
        //                            IsNewSupplier = supplier.IsNewSupplier,
        //                            SupplierName = supplier.SupplierName,
        //                            LotID = lotId,
        //                            GLTaxRateID = 0,
        //                            Quantity = lotitem.Quantity,
        //                            LotItemID = lotitem.LotItemsID
        //                        };
        //                        _db.ParticipantsItems.Add(item);
        //                        _db.SaveChanges();
        //                    }
        //                }
        //            }
        //        }

                

        //        #endregion

        //        var formattedData = new
        //        {
        //            Message = "Complete",
        //            Status = "OK"
        //        };
        //        return Json(formattedData, JsonRequestBehavior.AllowGet);
        //    }
        //    var formattedData1 = new
        //    {
        //        Message = "The record is not found.",
        //        Status = "NOT_OK"
        //    };
        //    return Json(formattedData1, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult GenerateResult(string id)
        {
            int lotid = int.Parse(id);
            var lot = _db.Lots.Find(lotid);
           
            if (lot != null)
            {
                _db.TenderEvaluationResults.RemoveRange(lot.EvaluationResults);
                _db.SaveChanges();

                var evaluations = lot.EvaluationCriterias;

                #region validation 

                var allNoPriceParticipantItems = _db.BidParticipatingSupplierItems
                    .Where(x => x.BidPParticipatingSupplier.LotID == lotid)
                    .Where(x => x.IsOutOfStock == false)
                    .Where(x => x.Total == 0);

                var lotCriteria = _db.TenderEvaluationCriterias.Where(x => x.LotID == lotid).ToList();

                if (lotCriteria.Any())
                {
                    if (lotCriteria.Count == 1 &&lotCriteria.FirstOrDefault().Type== CriteriaType.Pass_Fail)
                    {
                        //if pass fail only, chekc passed only 
                        var passedItems = _db.TenderPassFailResult.Where(x => x.LotID == lotid).Where(x => x.Result == PassFailStatus.Pass).ToList();
                        foreach(var i in passedItems)
                        {
                            var supp= _db.BidPParticipatingSupplierses
                                 .Where(x => x.LotID == lotid)
                                 .Where(x => x.SupplierName == i.SupplierName).FirstOrDefault();

                            var noPricePassItems= allNoPriceParticipantItems
                                 .Where(x=>x.BidPParticipatingSuppliersID== supp.BidPParticipatingSuppliersID)
                                 .Where(x=>x.LotItemID==i.LotItemID)
                                 .ToArray();

                            if (noPricePassItems.Any())
                            {
                                var formattedData3 = new
                                {
                                    Message = "Please fill price for all passed items.",
                                    Status = "NOT_OK"
                                };
                                return Json(formattedData3, JsonRequestBehavior.AllowGet);
                            }

                        }

                    }
                    else
                    {
                        if (allNoPriceParticipantItems.Any())
                        {
                            var formattedData3 = new
                            {
                                Message = "Please fill price for all items.",
                                Status = "NOT_OK"
                            };
                            return Json(formattedData3, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else
                {

                    if (allNoPriceParticipantItems.Any())
                    {
                        var formattedData3 = new
                        {
                            Message = "Please fill price for all items.",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData3, JsonRequestBehavior.AllowGet);
                    }
                }


                #endregion
                var participants = _db.BidPParticipatingSupplierses
                    .Where(x => x.LotID == lotid).ToList();
                foreach (var supplier in participants)
                {
                    var participantsItems = _db.BidParticipatingSupplierItems
                        .Where(x => x.BidPParticipatingSuppliersID == supplier.BidPParticipatingSuppliersID)
                        .Where(x => x.IsOutOfStock == false).ToList();

                    var result = _db.TenderEvaluationResults
                        .Where(x => x.LotID == lotid)
                        .FirstOrDefault(x => x.SupplierName == supplier.SupplierName);
                    if (result == null)
                    {
                        #region header
                        var evaluationResult = new TenderEvaluationResult
                        {
                            LotID = lotid,
                            Financial =0,
                            Technical = 0,
                            Pass = PassFailStatus.NA,
                            Total = 0,
                            SupplierId = supplier.SupplierId,
                            IsNewSupplier = supplier.IsNewSupplier,
                            SupplierName = supplier.SupplierName,
                        };
                        _db.TenderEvaluationResults.Add(evaluationResult);
                        _db.SaveChanges();
                        #endregion
                        foreach (var participantsItem in participantsItems)
                        {
                            #region line

                            var evaluationResultItem = new TenderEvaluationItemResult
                            {
                                TenderEvaluationResult = evaluationResult,
                                ItmCode = participantsItem.ItmCode,                              
                                ItemName = participantsItem.ItemName,
                                Name = participantsItem.Name,
                                ItemSpecification = participantsItem.ItemSpecification,
                                ModelNo = participantsItem.ModelNo,
                                CatCode = participantsItem.CatCode,
                                CategoryName = participantsItem.CategoryName,
                                LotItemID = participantsItem.LotItemID
                            };

                            #region pass fail

                            bool isPassFail = false;
                            var passfailCriteria = evaluations.FirstOrDefault(x => x.Type == CriteriaType.Pass_Fail);
                            if (passfailCriteria != null)
                            {
                                isPassFail = true;

                                //    var passFailResult = _db.TenderPassFailResult.FirstOrDefault(
                                //    x =>
                                //        x.LotID == lotid && x.ItmCode == participantsItem.ItmCode &&
                                //        x.SupplierName == participantsItem.SupplierName);

                                //if (passFailResult != null)
                                //{
                                //    //added to exclude failed ones
                                //    if (passFailResult.Result == PassFailStatus.Fail)
                                //        continue;
                                //    evaluationResultItem.Pass = passFailResult.Result;
                                //}

                                if (participantsItem.ItemType == PurchaseItemType.FixedAsset)
                                {
                                    var passFailResult = _db.TenderPassFailResult
                                        .Where(x => x.LotID == lotid)
                                        .Where(x => x.ItmCode == participantsItem.ItmCode)
                                        .Where(x => x.Name == participantsItem.Name)
                                        .Where(x => x.ModelNo == participantsItem.ModelNo)
                                        .FirstOrDefault(x => x.SupplierName == supplier.SupplierName);

                                    if (passFailResult != null)
                                    {
                                        if (passFailResult.Result == PassFailStatus.Fail)
                                            continue;
                                        evaluationResultItem.Pass = passFailResult.Result;
                                    }

                                }
                                else
                                {
                                    var passFailResult = _db.TenderPassFailResult
                                       .Where(x => x.LotID == lotid)
                                        .Where(x => x.ItmCode == participantsItem.ItmCode)
                                        .Where(x => x.ItemSpecification == participantsItem.ItemSpecification)//added
                                        .FirstOrDefault(x => x.SupplierName == supplier.SupplierName);

                                    if (passFailResult != null)
                                    {
                                        if (passFailResult.Result == PassFailStatus.Fail)
                                            continue;
                                        evaluationResultItem.Pass = passFailResult.Result;
                                    }

                                }
                            }
                            #endregion
                            #region financial

                            var financialCriteria = evaluations.FirstOrDefault(x => x.Type == CriteriaType.Financial);
                            int criteriaid = 0;
                            if (financialCriteria != null)
                                criteriaid = financialCriteria.TenderEvaluationCriteriaID;
                            //new block
                            List<BidParticipatingSupplierItem> allparticipantsItems = new List<BidParticipatingSupplierItem>();
                            var bidparticipants = _db.BidPParticipatingSupplierses.Where(x => x.LotID == lotid).ToList();
                            foreach(var supp in bidparticipants)
                            {
                                var participantItems = _db.BidParticipatingSupplierItems
                                    .Where(x => x.BidPParticipatingSuppliersID == supp.BidPParticipatingSuppliersID).ToList();

                                allparticipantsItems.AddRange(participantItems);
                            }

                            evaluationResultItem.Financial =
                                ComputeFinancial(participantsItem.ItemType, supplier.BidPParticipatingSuppliersID, participantsItem.ItmCode, participantsItem.ItemSpecification, participantsItem.ItemSpecification, participantsItem.ModelNo,
                                criteriaid, allparticipantsItems, isPassFail);
                            #endregion
                            #region technical

                            var technicalCriteria = evaluations.FirstOrDefault(x => x.Type == CriteriaType.Technical);
                            if (technicalCriteria != null)
                            {
                                //var proformaTechnical = _db.TenderTechnicals.FirstOrDefault(x =>
                                //    x.LotID == lotid && x.ItmCode == participantsItem.ItmCode &&
                                //    x.SupplierName == participantsItem.SupplierName);

                                //if (proformaTechnical != null) evaluationResultItem.Technical = proformaTechnical.Result;

                                if (participantsItem.ItemType == PurchaseItemType.FixedAsset)
                                {
                                    var proformaTechnical = _db.TenderTechnicals
                                        .Where(x => x.LotID == lotid)
                                        .Where(x => x.ItmCode == participantsItem.ItmCode)
                                        .Where(x => x.Name == participantsItem.Name)
                                        .Where(x => x.ModelNo == participantsItem.ModelNo)
                                        .FirstOrDefault(x => x.SupplierName == supplier.SupplierName);

                                    if (proformaTechnical != null) evaluationResultItem.Technical = proformaTechnical.Result;
                                }
                                else
                                {
                                    var proformaTechnical = _db.TenderTechnicals
                                        .Where(x => x.LotID == lotid)
                                        .Where(x => x.ItmCode == participantsItem.ItmCode)
                                        .Where(x => x.ItemSpecification == participantsItem.ItemSpecification)//added
                                        .FirstOrDefault(x => x.SupplierName == supplier.SupplierName);

                                    if (proformaTechnical != null) evaluationResultItem.Technical = proformaTechnical.Result;
                                }
                            }
                            #endregion

                            evaluationResultItem.Total = evaluationResultItem.Financial + evaluationResultItem.Technical;
                            _db.TenderEvaluationItemResults.Add(evaluationResultItem);
                            _db.SaveChanges();

                            #endregion
                        }
                    }
                    else
                    {
                        foreach (var participantsItem in participantsItems)
                        {
                            #region line

                            var evaluationResultItem = new TenderEvaluationItemResult
                            {
                                TenderEvaluationResult = result,
                                ItmCode = participantsItem.ItmCode,
                                ItemName = participantsItem.ItemName,
                                Name = participantsItem.Name,
                                ItemSpecification = participantsItem.ItemSpecification,
                                ModelNo = participantsItem.ModelNo,
                                CatCode = participantsItem.CatCode,
                                CategoryName = participantsItem.CategoryName,
                                LotItemID = participantsItem.LotItemID
                            };

                            #region pass fail

                            bool isPassFail = false;
                            var passfailCriteria = evaluations.FirstOrDefault(x => x.Type == CriteriaType.Pass_Fail);
                            if (passfailCriteria != null)
                            {
                                isPassFail = true;

                                //    var passFailResult = _db.TenderPassFailResult.FirstOrDefault(
                                //    x =>
                                //        x.LotID == lotid && x.ItmCode == participantsItem.ItmCode &&
                                //        x.SupplierName == participantsItem.SupplierName);

                                //if (passFailResult != null)
                                //{
                                //    //added to exclude failed ones
                                //    if (passFailResult.Result == PassFailStatus.Fail)
                                //        continue;
                                //    evaluationResultItem.Pass = passFailResult.Result;
                                //}

                                if (participantsItem.ItemType == PurchaseItemType.FixedAsset)
                                {
                                    var passFailResult = _db.TenderPassFailResult
                                        .Where(x => x.LotID == lotid)
                                        .Where(x => x.ItmCode == participantsItem.ItmCode)
                                        .Where(x => x.Name == participantsItem.Name)
                                        .Where(x => x.ModelNo == participantsItem.ModelNo)
                                        .FirstOrDefault(x => x.SupplierName == supplier.SupplierName);

                                    if (passFailResult != null)
                                    {
                                        if (passFailResult.Result == PassFailStatus.Fail)
                                            continue;
                                        evaluationResultItem.Pass = passFailResult.Result;
                                    }

                                }
                                else
                                {
                                    var passFailResult = _db.TenderPassFailResult
                                       .Where(x => x.LotID == lotid)
                                        .Where(x => x.ItmCode == participantsItem.ItmCode)
                                        .Where(x => x.ItemSpecification == participantsItem.ItemSpecification)//added
                                        .FirstOrDefault(x => x.SupplierName == supplier.SupplierName);

                                    if (passFailResult != null)
                                    {
                                        if (passFailResult.Result == PassFailStatus.Fail)
                                            continue;
                                        evaluationResultItem.Pass = passFailResult.Result;
                                    }

                                }
                            }
                            #endregion
                            #region financial

                            var financialCriteria = evaluations.FirstOrDefault(x => x.Type == CriteriaType.Financial);
                            int criteriaid = 0;
                            if (financialCriteria != null)
                                criteriaid = financialCriteria.TenderEvaluationCriteriaID;

                            //new block
                            List<BidParticipatingSupplierItem> allparticipantsItems = new List<BidParticipatingSupplierItem>();
                            var bidparticipants = _db.BidPParticipatingSupplierses.Where(x => x.LotID == lotid).ToList();
                            foreach (var supp in bidparticipants)
                            {
                                var participantItems = _db.BidParticipatingSupplierItems
                                    .Where(x => x.BidPParticipatingSuppliersID == supp.BidPParticipatingSuppliersID).ToList();

                                allparticipantsItems.AddRange(participantItems);
                            }

                            evaluationResultItem.Financial =
                                ComputeFinancial(participantsItem.ItemType, supplier.BidPParticipatingSuppliersID, participantsItem.ItmCode, participantsItem.ItemSpecification, participantsItem.ItemSpecification, participantsItem.ModelNo,
                                criteriaid, allparticipantsItems, isPassFail);
                            #endregion
                            #region technical

                            var technicalCriteria = evaluations.FirstOrDefault(x => x.Type == CriteriaType.Technical);
                            if (technicalCriteria != null)
                            {
                                //var proformaTechnical = _db.TenderTechnicals.FirstOrDefault(x =>
                                //    x.LotID == lotid && x.ItmCode == participantsItem.ItmCode &&
                                //    x.SupplierName == participantsItem.SupplierName);

                                //if (proformaTechnical != null) evaluationResultItem.Technical = proformaTechnical.Result;

                                if (participantsItem.ItemType == PurchaseItemType.FixedAsset)
                                {
                                    var proformaTechnical = _db.TenderTechnicals
                                        .Where(x => x.LotID == lotid)
                                        .Where(x => x.ItmCode == participantsItem.ItmCode)
                                        .Where(x => x.Name == participantsItem.Name)
                                        .Where(x => x.ModelNo == participantsItem.ModelNo)
                                        .FirstOrDefault(x => x.SupplierName == supplier.SupplierName);

                                    if (proformaTechnical != null) evaluationResultItem.Technical = proformaTechnical.Result;
                                }
                                else
                                {
                                    var proformaTechnical = _db.TenderTechnicals
                                        .Where(x => x.LotID == lotid)
                                        .Where(x => x.ItmCode == participantsItem.ItmCode)
                                        .Where(x => x.ItemSpecification == participantsItem.ItemSpecification)//added
                                        .FirstOrDefault(x => x.SupplierName == supplier.SupplierName);

                                    if (proformaTechnical != null) evaluationResultItem.Technical = proformaTechnical.Result;
                                }
                            }
                            #endregion

                            evaluationResultItem.Total = evaluationResultItem.Financial + evaluationResultItem.Technical;
                            _db.TenderEvaluationItemResults.Add(evaluationResultItem);
                            _db.SaveChanges();

                            #endregion
                        }
                    }

                }

                GenerateSummaryResult(lotid);

                var formattedData = new
                {
                    Message = "Complete",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            var formattedData1 = new
            {
                Message = "The record is not found.",
                Status = "NOT_OK"
            };
            return Json(formattedData1, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GenerateResult_1(string id)
        //{
        //    int lotid = int.Parse(id);
        //    var lot = _db.Lots.Find(lotid);

        //    if (lot != null)
        //    {
        //        _db.TenderEvaluationResults.RemoveRange(lot.EvaluationResults);
        //        _db.SaveChanges();

        //        var evaluations = lot.EvaluationCriterias;

        //        #region validation 

        //        var allParticipantItems = _db.ParticipantsItems
        //            .Where(x => x.LotID == lotid).Where(x => x.Total == 0);

        //        if (allParticipantItems.Any())
        //        {
        //            var formattedData3 = new
        //            {
        //                Message = "Please fill price for all items.",
        //                Status = "NOT_OK"
        //            };
        //            return Json(formattedData3, JsonRequestBehavior.AllowGet);
        //        }


        //        #endregion
        //        foreach (var participantsItem in lot.ParticipantsItems)
        //        {

        //            var result = _db.TenderEvaluationResults
        //                .Where(x => x.LotID == lotid)
        //                .FirstOrDefault(x => x.SupplierName == participantsItem.SupplierName);
        //            if (result == null)
        //            {
        //                #region header
        //                var evaluationResult = new TenderEvaluationResult
        //                {
        //                    LotID = lot.LotID,
        //                    Financial = 0,
        //                    Technical = 0,
        //                    Pass = PassFailStatus.NA,
        //                    Total = 0,
        //                    SupplierId = participantsItem.SupplierId,
        //                    IsNewSupplier = participantsItem.IsNewSupplier,
        //                    SupplierName = participantsItem.SupplierName,
        //                };
        //                _db.TenderEvaluationResults.Add(evaluationResult);
        //                _db.SaveChanges();
        //                #endregion
        //                #region line

        //                var evaluationResultItem = new TenderEvaluationItemResult
        //                {
        //                    TenderEvaluationResult = evaluationResult,
        //                    ItmCode = participantsItem.ItmCode,
        //                    ItemName = participantsItem.ItemName,
        //                    Name = participantsItem.Name,
        //                    ItemSpecification = participantsItem.ItemSpecification,
        //                    ModelNo = participantsItem.ModelNo,
        //                    CatCode = participantsItem.CatCode,
        //                    CategoryName = participantsItem.CategoryName,
        //                    LotItemID = participantsItem.LotItemID
        //                };

        //                #region pass fail

        //                bool isPassFail = false;
        //                var passfailCriteria = evaluations.FirstOrDefault(x => x.Type == CriteriaType.Pass_Fail);
        //                if (passfailCriteria != null)
        //                {
        //                    isPassFail = true;

        //                    //    var passFailResult = _db.TenderPassFailResult.FirstOrDefault(
        //                    //    x =>
        //                    //        x.LotID == lotid && x.ItmCode == participantsItem.ItmCode &&
        //                    //        x.SupplierName == participantsItem.SupplierName);

        //                    //if (passFailResult != null)
        //                    //{
        //                    //    //added to exclude failed ones
        //                    //    if (passFailResult.Result == PassFailStatus.Fail)
        //                    //        continue;
        //                    //    evaluationResultItem.Pass = passFailResult.Result;
        //                    //}

        //                    if (participantsItem.ItemType == PurchaseItemType.FixedAsset)
        //                    {
        //                        var passFailResult = _db.TenderPassFailResult
        //                            .Where(x => x.LotID == lotid)
        //                            .Where(x => x.ItmCode == participantsItem.ItmCode)
        //                            .Where(x => x.Name == participantsItem.Name)
        //                            .Where(x => x.ModelNo == participantsItem.ModelNo)
        //                            .FirstOrDefault(x => x.SupplierName == participantsItem.SupplierName);

        //                        if (passFailResult != null)
        //                        {
        //                            if (passFailResult.Result == PassFailStatus.Fail)
        //                                continue;
        //                            evaluationResultItem.Pass = passFailResult.Result;
        //                        }

        //                    }
        //                    else
        //                    {
        //                        var passFailResult = _db.TenderPassFailResult
        //                           .Where(x => x.LotID == lotid)
        //                            .Where(x => x.ItmCode == participantsItem.ItmCode)
        //                            .Where(x => x.ItemSpecification == participantsItem.ItemSpecification)//added
        //                            .FirstOrDefault(x => x.SupplierName == participantsItem.SupplierName);

        //                        if (passFailResult != null)
        //                        {
        //                            if (passFailResult.Result == PassFailStatus.Fail)
        //                                continue;
        //                            evaluationResultItem.Pass = passFailResult.Result;
        //                        }

        //                    }
        //                }
        //                #endregion
        //                #region financial

        //                var financialCriteria = evaluations.FirstOrDefault(x => x.Type == CriteriaType.Financial);
        //                int criteriaid = 0;
        //                if (financialCriteria != null)
        //                    criteriaid = financialCriteria.TenderEvaluationCriteriaID;

        //                evaluationResultItem.Financial =
        //                    ComputeFinancial(participantsItem.ItemType, participantsItem.SupplierName, participantsItem.ItmCode, participantsItem.ItemSpecification, participantsItem.ItemSpecification, participantsItem.ModelNo,
        //                    criteriaid, lot.ParticipantsItems.ToList(), isPassFail);
        //                #endregion
        //                #region technical

        //                var technicalCriteria = evaluations.FirstOrDefault(x => x.Type == CriteriaType.Technical);
        //                if (technicalCriteria != null)
        //                {
        //                    //var proformaTechnical = _db.TenderTechnicals.FirstOrDefault(x =>
        //                    //    x.LotID == lotid && x.ItmCode == participantsItem.ItmCode &&
        //                    //    x.SupplierName == participantsItem.SupplierName);

        //                    //if (proformaTechnical != null) evaluationResultItem.Technical = proformaTechnical.Result;

        //                    if (participantsItem.ItemType == PurchaseItemType.FixedAsset)
        //                    {
        //                        var proformaTechnical = _db.TenderTechnicals
        //                            .Where(x => x.LotID == lotid)
        //                            .Where(x => x.ItmCode == participantsItem.ItmCode)
        //                            .Where(x => x.Name == participantsItem.Name)
        //                            .Where(x => x.ModelNo == participantsItem.ModelNo)
        //                            .FirstOrDefault(x => x.SupplierName == participantsItem.SupplierName);

        //                        if (proformaTechnical != null) evaluationResultItem.Technical = proformaTechnical.Result;
        //                    }
        //                    else
        //                    {
        //                        var proformaTechnical = _db.TenderTechnicals
        //                            .Where(x => x.LotID == lotid)
        //                            .Where(x => x.ItmCode == participantsItem.ItmCode)
        //                            .Where(x => x.ItemSpecification == participantsItem.ItemSpecification)//added
        //                            .FirstOrDefault(x => x.SupplierName == participantsItem.SupplierName);

        //                        if (proformaTechnical != null) evaluationResultItem.Technical = proformaTechnical.Result;
        //                    }
        //                }
        //                #endregion

        //                evaluationResultItem.Total = evaluationResultItem.Financial + evaluationResultItem.Technical;
        //                _db.TenderEvaluationItemResults.Add(evaluationResultItem);
        //                _db.SaveChanges();

        //                #endregion
        //            }
        //            else
        //            {
        //                #region line

        //                var evaluationResultItem = new TenderEvaluationItemResult
        //                {
        //                    TenderEvaluationResult = result,
        //                    ItmCode = participantsItem.ItmCode,
        //                    ItemName = participantsItem.ItemName,
        //                    Name = participantsItem.Name,
        //                    ItemSpecification = participantsItem.ItemSpecification,
        //                    ModelNo = participantsItem.ModelNo,
        //                    CatCode = participantsItem.CatCode,
        //                    CategoryName = participantsItem.CategoryName,
        //                    LotItemID = participantsItem.LotItemID
        //                };

        //                #region pass fail

        //                bool isPassFail = false;
        //                var passfailCriteria = evaluations.FirstOrDefault(x => x.Type == CriteriaType.Pass_Fail);
        //                if (passfailCriteria != null)
        //                {
        //                    isPassFail = true;

        //                    if (participantsItem.ItemType == PurchaseItemType.FixedAsset)
        //                    {
        //                        var passFailResult = _db.TenderPassFailResult
        //                            .Where(x => x.LotID == lotid)
        //                            .Where(x => x.ItmCode == participantsItem.ItmCode)
        //                            .Where(x => x.Name == participantsItem.Name)
        //                            .Where(x => x.ModelNo == participantsItem.ModelNo)
        //                            .FirstOrDefault(x => x.SupplierName == participantsItem.SupplierName);

        //                        if (passFailResult != null)
        //                        {
        //                            if (passFailResult.Result == PassFailStatus.Fail)
        //                                continue;
        //                            evaluationResultItem.Pass = passFailResult.Result;
        //                        }

        //                    }
        //                    else
        //                    {
        //                        var passFailResult = _db.TenderPassFailResult
        //                           .Where(x => x.LotID == lotid)
        //                            .Where(x => x.ItmCode == participantsItem.ItmCode)
        //                            .Where(x => x.ItemSpecification == participantsItem.ItemSpecification)//added
        //                            .FirstOrDefault(x => x.SupplierName == participantsItem.SupplierName);

        //                        if (passFailResult != null)
        //                        {
        //                            if (passFailResult.Result == PassFailStatus.Fail)
        //                                continue;
        //                            evaluationResultItem.Pass = passFailResult.Result;
        //                        }

        //                    }
        //                }
        //                #endregion
        //                #region financial

        //                var financialCriteria = evaluations.FirstOrDefault(x => x.Type == CriteriaType.Financial);
        //                int criteriaid = 0;
        //                if (financialCriteria != null)
        //                    criteriaid = financialCriteria.TenderEvaluationCriteriaID;

        //                evaluationResultItem.Financial = ComputeFinancial(participantsItem.ItemType, participantsItem.SupplierName, participantsItem.ItmCode, participantsItem.ItemSpecification, participantsItem.Name, participantsItem.ModelNo,
        //                    criteriaid, lot.ParticipantsItems.ToList(), isPassFail);
        //                #endregion
        //                #region technical

        //                var technicalCriteria = evaluations.FirstOrDefault(x => x.Type == CriteriaType.Technical);
        //                if (technicalCriteria != null)
        //                {
        //                    if (participantsItem.ItemType == PurchaseItemType.FixedAsset)
        //                    {
        //                        var tenderTechnical = _db.TenderTechnicals
        //                            .Where(x => x.LotID == lotid)
        //                            .Where(x => x.ItmCode == participantsItem.ItmCode)
        //                            .Where(x => x.Name == participantsItem.Name)
        //                            .Where(x => x.ModelNo == participantsItem.ModelNo)
        //                            .FirstOrDefault(x => x.SupplierName == participantsItem.SupplierName);

        //                        if (tenderTechnical != null) evaluationResultItem.Technical = tenderTechnical.Result;
        //                    }
        //                    else
        //                    {
        //                        var tenderTechnical = _db.TenderTechnicals
        //                            .Where(x => x.LotID == lotid)
        //                            .Where(x => x.ItmCode == participantsItem.ItmCode)
        //                            .Where(x => x.ItemSpecification == participantsItem.ItemSpecification)//added
        //                            .FirstOrDefault(x => x.SupplierName == participantsItem.SupplierName);

        //                        if (tenderTechnical != null) evaluationResultItem.Technical = tenderTechnical.Result;
        //                    }

        //                }
        //                #endregion

        //                evaluationResultItem.Total = evaluationResultItem.Financial + evaluationResultItem.Technical;
        //                _db.TenderEvaluationItemResults.Add(evaluationResultItem);
        //                _db.SaveChanges();

        //                #endregion
        //            }

        //        }

        //        GenerateSummaryResult(lotid);

        //        var formattedData = new
        //        {
        //            Message = "Complete",
        //            Status = "OK"
        //        };
        //        return Json(formattedData, JsonRequestBehavior.AllowGet);
        //    }

        //    var formattedData1 = new
        //    {
        //        Message = "The record is not found.",
        //        Status = "NOT_OK"
        //    };
        //    return Json(formattedData1, JsonRequestBehavior.AllowGet);
        //}
        public JsonResult GenerateWinner(string id)
        {
            int lotid = int.Parse(id);
            var lot = _db.Lots.Find(lotid);
            if (lot != null)
            {
                if (lot.EvaluateTotalFinance)
                {
                    #region evaluate by total financial result
                    List<TenderEvaluationResult> canProvideAllItems = new List<TenderEvaluationResult>();
                    var lotItems = _db.LotItems.Where(x => x.LotID == lotid).ToList();

                    var awards = _db.BidAwardedSuppliers.Where(x => x.LotID == lotid).ToList();
                    var winnerCanProvide = awards.Where(x => x.AwardGiven).ToList();
                    var winnerCantProvide =
                        awards.Where(x => x.AwardGiven == false).Select(x => x.SupplierName).ToList();
                    _db.BidAwardedSuppliers.RemoveRange(winnerCanProvide);
                    _db.SaveChanges();

                    var tenderEvaluationResults = _db.TenderEvaluationResults
                        .Where(x => x.Pass != PassFailStatus.Fail)
                        .Where(x => x.LotID == lotid).ToList();
                    if (winnerCantProvide.Any())
                        tenderEvaluationResults = tenderEvaluationResults
                            .Where(x => winnerCantProvide.Contains(x.SupplierName) == false).ToList();

                    #region remove suppliers that don't provide all items
                    foreach (var s in tenderEvaluationResults)
                    {
                        bool canProvide = true;
                        var supplierItems = _db.TenderEvaluationItemResults.Where(x =>
                          x.TenderEvaluationResultID == s.TenderEvaluationResultID).ToList();

                        foreach (var i in lotItems)
                        {
                            var exist = supplierItems.Where(x => x.ItmCode == i.ItmCode).FirstOrDefault();
                            if (exist == null)
                            {
                                canProvide = false;
                                break;
                            }
                        }

                        if (canProvide)
                            canProvideAllItems.Add(s);

                    }
                    #endregion

                    var topResult = canProvideAllItems//tenderEvaluationResults
                        .OrderByDescending(x => x.Total).FirstOrDefault();

                    if (topResult != null)
                    {
                        var supp = _db.BidPParticipatingSupplierses
                           .Where(x => x.SupplierName == topResult.SupplierName)
                           .Where(x => x.LotID == lotid).FirstOrDefault();

                        var winner = _db.BidAwardedSuppliers.Where(x => x.LotID == lotid).FirstOrDefault(x => x.SupplierName == topResult.SupplierName);
                        var resultItems = _db.TenderEvaluationItemResults.Where(x =>
                            x.TenderEvaluationResultID == topResult.TenderEvaluationResultID).ToList();
                        if (winner == null)
                        {
                            #region header

                            var awardedSupplier = new BidAwardedSupplier
                            {
                                LotID = lot.LotID,
                                TenderReference = lot.Tender.TenderReference,
                                AwardDate = DateTime.Today,
                                SupplierId = topResult.SupplierId,
                                IsNewSupplier = topResult.IsNewSupplier,
                                SupplierName = topResult.SupplierName,
                                DeliveryDate = DateTime.Today,
                                AwardGiven = true
                            };
                            _db.BidAwardedSuppliers.Add(awardedSupplier);
                            _db.SaveChanges();

                            #endregion

                            foreach (var item in resultItems)
                            {
                                #region line


                                var participantitem = _db.BidParticipatingSupplierItems
                                    .Where(x => x.BidPParticipatingSuppliersID==supp.BidPParticipatingSuppliersID)
                                    .Where(x => x.ItmCode == item.ItmCode)
                                    .Where(x => x.ItemSpecification == item.ItemSpecification)//added
                                    .FirstOrDefault();

                                if (participantitem != null)
                                {
                                    var supplierItem = new BidAwardedSupplierItem
                                    {
                                        BidAwardedSupplier = awardedSupplier,
                                        ItmCode = item.ItmCode,
                                        ItemName = item.ItemName,
                                        ModelNo = item.ModelNo,
                                        CatCode = item.CatCode,
                                        CategoryName = item.CategoryName,
                                        ItemType = participantitem.ItemType,
                                        PartNo = participantitem.PartNo,
                                        ItemSpecification = participantitem.ItemSpecification,
                                        UoM = participantitem.UoM,
                                        Quanity = participantitem.Quantity,
                                        UPrice = participantitem.UnitPrice,
                                        Discount = 0,
                                        FinalUPrice = participantitem.UnitPrice,
                                        GLTaxRateID = participantitem.GLTaxRateID,
                                        TaxAmount = participantitem.TaxAmount,
                                        TPrice = participantitem.Total,
                                        LotItemID = participantitem.LotItemID
                                    };
                                    _db.BidAwardedSupplierItems.Add(supplierItem);
                                }

                                _db.SaveChanges();

                                #endregion
                            }

                        }
                        else
                        {
                            foreach (var item in resultItems)
                            {
                                #region line
                                var participantitem = _db.BidParticipatingSupplierItems
                                    .Where(x => x.BidPParticipatingSuppliersID == supp.BidPParticipatingSuppliersID)
                                    .Where(x => x.ItmCode == item.ItmCode)
                                    .Where(x => x.ItemSpecification == item.ItemSpecification)//added
                                    .FirstOrDefault();
                                if (participantitem != null)
                                {
                                    var supplierItem = new BidAwardedSupplierItem
                                    {
                                        BidAwardedSupplier = winner,
                                        ItmCode = item.ItmCode,
                                        ItemName = item.ItemName,
                                        ModelNo = item.ModelNo,
                                        CatCode = item.CatCode,
                                        CategoryName = item.CategoryName,
                                        ItemType = participantitem.ItemType,
                                        PartNo = participantitem.PartNo,
                                        ItemSpecification = participantitem.ItemSpecification,
                                        UoM = participantitem.UoM,
                                        Quanity = participantitem.Quantity,
                                        UPrice = participantitem.UnitPrice,
                                        Discount = 0,
                                        FinalUPrice = participantitem.UnitPrice,
                                        GLTaxRateID = participantitem.GLTaxRateID,
                                        TaxAmount = participantitem.TaxAmount,
                                        TPrice = participantitem.Total,
                                        LotItemID = participantitem.LotItemID
                                    };
                                    _db.BidAwardedSupplierItems.Add(supplierItem);
                                }

                                _db.SaveChanges();

                                #endregion
                            }
                        }
                    }



                    #endregion
                }
                else
                {
                    #region evaluate by item financial result
                    var awards = _db.BidAwardedSuppliers.Where(x => x.LotID == lotid).ToList();
                    var winnerCanProvide = awards.Where(x => x.AwardGiven).ToList();
                    var winnerCantProvide =
                        awards.Where(x => x.AwardGiven == false).Select(x => x.SupplierName).ToList();
                    _db.BidAwardedSuppliers.RemoveRange(winnerCanProvide);
                    _db.SaveChanges();
                    foreach (var lotItems in lot.LotItems)
                    {
                        var item = lotItems;

                        var tenderEvaluationResults = _db.TenderEvaluationResults
                            .Where(x => x.Pass != PassFailStatus.Fail)
                            .Where(x => x.LotID == lotid).ToList();
                        if (winnerCantProvide.Any())
                            tenderEvaluationResults = tenderEvaluationResults
                                .Where(x => winnerCantProvide.Contains(x.SupplierName) == false).ToList();

                        var resultIds = tenderEvaluationResults.Select(x => x.TenderEvaluationResultID).Distinct();

                        var tenderEvaluationItemResults = _db.TenderEvaluationItemResults
                            .Where(x => resultIds.Contains(x.TenderEvaluationResultID)).ToList();

                        var topResult = tenderEvaluationItemResults
                            .Where(x => x.ItmCode == item.ItmCode)
                            .Where(x => x.Name == item.Name)//added
                            .Where(x => x.ModelNo == item.ModelNo)//added
                            .Where(x => x.LotItemID == item.LotItemsID)//new added
                            .OrderByDescending(x => x.Total).FirstOrDefault();

                        if (topResult != null)
                        {
                            var supp = _db.BidPParticipatingSupplierses
                            .Where(x => x.SupplierName == topResult.TenderEvaluationResult.SupplierName)
                            .Where(x => x.LotID == lotid).FirstOrDefault();

                            var participantitem = _db.BidParticipatingSupplierItems
                                .Where(x => x.BidPParticipatingSuppliersID == supp.BidPParticipatingSuppliersID)
                                .Where(x => x.ItmCode == item.ItmCode)
                                .Where(x => x.Name == item.Name)//added
                                .Where(x => x.ModelNo == item.ModelNo)//added
                                .Where(x => x.ItemSpecification == item.ItemSpecification)//new  added
                                 .Where(x => x.LotItemID == item.LotItemsID)//new added
                                                                          .FirstOrDefault();
                            if (participantitem != null)
                            {
                                var winner = _db.BidAwardedSuppliers.Where(x => x.LotID == lotid)
                                    .FirstOrDefault(x => x.SupplierName == topResult.TenderEvaluationResult.SupplierName);
                                if (winner == null)
                                {
                                    #region header

                                    var awardedSupplier = new BidAwardedSupplier
                                    {
                                        LotID = lot.LotID,
                                        TenderReference = lot.Tender.TenderReference,
                                        AwardDate = DateTime.Today,
                                        SupplierId = topResult.TenderEvaluationResult.SupplierId,
                                        IsNewSupplier = topResult.TenderEvaluationResult.IsNewSupplier,
                                        SupplierName = topResult.TenderEvaluationResult.SupplierName,
                                        DeliveryDate = DateTime.Today,
                                        AwardGiven = true,
                                    };
                                    _db.BidAwardedSuppliers.Add(awardedSupplier);
                                    _db.SaveChanges();

                                    #endregion

                                    #region line

                                    var supplierItem = new BidAwardedSupplierItem
                                    {
                                        BidAwardedSupplier = awardedSupplier,
                                        ItmCode = lotItems.ItmCode,
                                        ItemName = lotItems.ItemName,
                                        Name = lotItems.Name,
                                        ModelNo = lotItems.ModelNo,
                                        CatCode = lotItems.CatCode,
                                        CategoryName = lotItems.CategoryName,
                                        ItemType = lotItems.ItemType,
                                        PartNo = lotItems.PartNo,
                                        IsApproved = lotItems.IsApproved,
                                        ItemSpecification = lotItems.ItemSpecification,
                                        UoM = lotItems.UoM,
                                        Quanity = lotItems.Quantity,
                                        UPrice = participantitem.UnitPrice,
                                        Discount = 0,
                                        FinalUPrice = participantitem.UnitPrice,
                                        GLTaxRateID = participantitem.GLTaxRateID,
                                        TaxAmount = participantitem.TaxAmount,
                                        TPrice = participantitem.Total,
                                        LotItemID = participantitem.LotItemID
                                    };
                                    _db.BidAwardedSupplierItems.Add(supplierItem);
                                    _db.SaveChanges();

                                    #endregion
                                }
                                else
                                {
                                    #region line

                                    var supplierItem = new BidAwardedSupplierItem
                                    {
                                        BidAwardedSupplier = winner,
                                        ItmCode = lotItems.ItmCode,
                                        ItemName = lotItems.ItemName,
                                        Name = lotItems.Name,
                                        ModelNo = lotItems.ModelNo,
                                        CatCode = lotItems.CatCode,
                                        CategoryName = lotItems.CategoryName,
                                        ItemType = lotItems.ItemType,
                                        PartNo = lotItems.PartNo,
                                        IsApproved = lotItems.IsApproved,
                                        ItemSpecification = lotItems.ItemSpecification,
                                        UoM = lotItems.UoM,
                                        Quanity = lotItems.Quantity,
                                        UPrice = participantitem.UnitPrice,
                                        Discount = 0,
                                        FinalUPrice = participantitem.UnitPrice,
                                        GLTaxRateID = participantitem.GLTaxRateID,
                                        TaxAmount = participantitem.TaxAmount,
                                        TPrice = participantitem.Total,
                                        LotItemID = participantitem.LotItemID
                                    };
                                    _db.BidAwardedSupplierItems.Add(supplierItem);
                                    _db.SaveChanges();

                                    #endregion
                                }


                            }
                        }
                    }
                    #endregion
                }

                var formattedData = new
                {
                    Message = "Complete",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            var formattedData1 = new
            {
                Message = "The record is not found.",
                Status = "NOT_OK"
            };
            return Json(formattedData1, JsonRequestBehavior.AllowGet);
        }

        private float ComputeFinancial(PurchaseItemType type,int participantId, string itemcode,string spec, string name,string model, int criteriaId, List<BidParticipatingSupplierItem> participantsItems,bool passfail)
        {
            float result = 0;

            if (type == PurchaseItemType.FixedAsset)
            {
                var participantsItem = participantsItems
                    .Where(x => x.ItmCode == itemcode)
                    .Where(x => x.Name == name)
                    .Where(x => x.ModelNo == model)
                    .FirstOrDefault(x => x.BidPParticipatingSuppliersID == participantId);
                var specificItems = participantsItems.Where(x => x.ItmCode == itemcode).Where(x => x.Name == name).Where(x => x.ModelNo == model).ToList();
                float lowestPrice = specificItems.Any()? (float)specificItems.Min(x => x.Total):0;
                if (participantsItem != null)
                {
                    float price = (float)participantsItem.Total;
                    if (passfail)
                    {
                        result = lowestPrice * 100 / price;
                    }
                    else
                    {
                        var criteria = _db.TenderEvaluationCriterias.Find(criteriaId);
                        if (criteria != null) result = lowestPrice * criteria.Weight / price;
                    }

                }

            }
            else
            {
                var participantsItem = participantsItems
                    .Where(x => x.ItmCode == itemcode)
                    .Where(x => x.ItemSpecification ==spec)//added
                    .FirstOrDefault(x => x.BidPParticipatingSuppliersID == participantId);

                var specificItems = participantsItems.Where(x => x.ItmCode == itemcode).ToList();
                float lowestPrice = specificItems.Any() ? (float)specificItems.Min(x => x.Total) : 0;
                if (participantsItem != null)
                {
                    float price = (float)participantsItem.Total;
                    if (passfail)
                    {
                        result = lowestPrice * 100 / price;
                    }
                    else
                    {
                        var criteria = _db.TenderEvaluationCriterias.Find(criteriaId);
                        if (criteria != null) result = lowestPrice * criteria.Weight / price;
                    }

                }

            }
           
            return result;
        }
        
        private void GenerateSummaryResult(int lotid)
        {
            var tenderEvaluationResults = _db.TenderEvaluationResults.Where(x => x.LotID == lotid).ToList();

            decimal minFinancial = 0;

            foreach (var result in tenderEvaluationResults)
            {

                var supp = _db.BidPParticipatingSupplierses
                    .Where(x => x.SupplierName == result.SupplierName)
                    .Where(x => x.LotID == lotid).FirstOrDefault();

                decimal sum = 0;

                var participantItems = _db.BidParticipatingSupplierItems
                    //.Where(x => x.LotID == lotid)
                    .Where(x => x.BidPParticipatingSuppliersID == supp.BidPParticipatingSuppliersID).ToList();

               if(participantItems.Any())
                    sum=participantItems.Sum(x => x.Total);

                if ((minFinancial > 0 && sum < minFinancial) || minFinancial == 0)
                    minFinancial = sum;
            }

            #region pass fail and financial

            bool isPassFail = false;
            var passfailCriteria = _db.TenderEvaluationCriterias.Where(x => x.LotID == lotid).FirstOrDefault(x => x.Type == CriteriaType.Pass_Fail);
            if (passfailCriteria != null)
                isPassFail = true;

            var financialCriteria = _db.TenderEvaluationCriterias.Where(x => x.LotID == lotid).FirstOrDefault(x => x.Type == CriteriaType.Financial);
            

            #endregion

            foreach (var result in tenderEvaluationResults)
            {
                var supp = _db.BidPParticipatingSupplierses
                   .Where(x => x.SupplierName == result.SupplierName)
                   .Where(x => x.LotID == lotid).FirstOrDefault();

                var itemResult = _db.TenderEvaluationItemResults
                    .Where(x => x.TenderEvaluationResultID == result.TenderEvaluationResultID).ToList();

                result.Technical = itemResult.Average(x => x.Technical);

                decimal totalprice = _db.BidParticipatingSupplierItems
                     .Where(x => x.BidPParticipatingSuppliersID == supp.BidPParticipatingSuppliersID)
                    .Sum(x => x.Total);

                if (isPassFail)
                {
                    result.Financial = (float) (minFinancial * 100 / totalprice);
                }
                else
                {
                    if (financialCriteria != null)
                        result.Financial = (float) (minFinancial * (decimal) financialCriteria.Weight / totalprice);
                }

                var passfail = itemResult.FirstOrDefault(x => x.Pass == PassFailStatus.Fail);

                if (passfail != null)
                    result.Pass = passfail.Pass;

                var passfailNa= itemResult.FirstOrDefault(x => x.Pass == PassFailStatus.NA);
                if(passfailNa!=null)
                    result.Pass = passfailNa.Pass;
                if (passfail == null&& passfailNa == null)
                    result.Pass = PassFailStatus.Pass;

                result.Total = result.Financial + result.Technical;

                _db.TenderEvaluationResults.Attach(result);
                _db.Entry(result).State = EntityState.Modified;
                _db.SaveChanges();
            }
        }

        //private void GenerateSummaryResult_1(int lotid)
        //{
        //    var tenderEvaluationResults = _db.TenderEvaluationResults.Where(x => x.LotID == lotid).ToList();

        //    decimal minFinancial = 0;

        //    foreach (var result in tenderEvaluationResults)
        //    {
        //        decimal sum = _db.ParticipantsItems.Where(x => x.LotID == lotid)
        //            .Where(x => x.SupplierName == result.SupplierName).Sum(x => x.Total);
        //        if ((minFinancial > 0 && sum < minFinancial) || minFinancial == 0)
        //            minFinancial = sum;
        //    }

        //    #region pass fail and financial

        //    bool isPassFail = false;
        //    var passfailCriteria = _db.TenderEvaluationCriterias.Where(x => x.LotID == lotid).FirstOrDefault(x => x.Type == CriteriaType.Pass_Fail);
        //    if (passfailCriteria != null)
        //        isPassFail = true;

        //    var financialCriteria = _db.TenderEvaluationCriterias.Where(x => x.LotID == lotid).FirstOrDefault(x => x.Type == CriteriaType.Financial);


        //    #endregion

        //    foreach (var result in tenderEvaluationResults)
        //    {
        //        var itemResult = _db.TenderEvaluationItemResults
        //            .Where(x => x.TenderEvaluationResultID == result.TenderEvaluationResultID).ToList();

        //        result.Technical = itemResult.Average(x => x.Technical);

        //        decimal totalprice = _db.ParticipantsItems.Where(x => x.LotID == lotid)
        //            .Where(x => x.SupplierName == result.SupplierName).Sum(x => x.Total);

        //        if (isPassFail)
        //        {
        //            result.Financial = (float)(minFinancial * 100 / totalprice);
        //        }
        //        else
        //        {
        //            if (financialCriteria != null)
        //                result.Financial = (float)(minFinancial * (decimal)financialCriteria.Weight / totalprice);
        //        }

        //        var passfail = itemResult.FirstOrDefault(x => x.Pass == PassFailStatus.Fail);

        //        if (passfail != null)
        //            result.Pass = passfail.Pass;

        //        var passfailNa = itemResult.FirstOrDefault(x => x.Pass == PassFailStatus.NA);
        //        if (passfailNa != null)
        //            result.Pass = passfailNa.Pass;
        //        if (passfail == null && passfailNa == null)
        //            result.Pass = PassFailStatus.Pass;

        //        result.Total = result.Financial + result.Technical;

        //        _db.TenderEvaluationResults.Attach(result);
        //        _db.Entry(result).State = EntityState.Modified;
        //        _db.SaveChanges();
        //    }
        //}
        private void TransferToContract(int id)
        {
            var lots = _db.Lots.Where(x => x.TenderID == id).ToList();
            foreach (var lot in lots)
            {
                var awards = _db.BidAwardedSuppliers.Where(x=>x.AwardGiven).Where(x => x.LotID == lot.LotID).ToList();
                foreach (var award in awards)
                {
                    SendForContract(award.BidAwardedSupplierID);
                }
            }

        }

        private void SendForContract(int id)
        {
            var record = _db.BidAwardedSuppliers.Find(id);

            if (record != null && !record.IsTransferredToContract && record.BidWinningItems.Count > 0)
            {
                decimal contractamount = record.BidWinningItems.Sum(x => x.TPrice);
                var lot = _db.Lots.Find(record.LotID);
                if (lot != null)
                {
                    var tender = _db.Tenders.Find(lot.TenderID);
                    #region send to contract
                    if (tender != null)
                    {
                        #region create contract header
                        var contractManagement = new ContractManagement
                        {

                            BranchId = tender.BranchId,
                            SupplierName = record.SupplierName,
                            IsNewSupplier = record.IsNewSupplier,
                            SupplierId = record.SupplierId,
                            ContractRefNo = "NA",
                            TenderRefNo = tender.TenderReference,
                            ProcurementRefNo = record.BidAwardedSupplierID,
                            SigningDate = DateTime.Today,
                            ContractType = tender.ProcurementCategory,
                            PurchaseCategory = tender.PurchaseCategory,
                            DeliveryPlace = "NA",
                            DeliveryDate = record.DeliveryDate,
                            ContractAmount = contractamount,
                            RetentionPercentage = 0,
                            ProcurementOfficer = tender.ProcurementOfficer,

                            ForwardedTo = UserInformationHelper.GetEmployeeUserId(tender.ProcurementOfficer),
                            ForwardedFrom = User.Identity.Name,


                            Status = "Editing",
                            Remark = record.Remark,

                            DateCreated = DateTime.Now,
                            CreatedBy = User.Identity.Name
                        };
                        _db.ContractManagements.Add(contractManagement);
                        _db.SaveChanges();


                        #endregion

                        var awardedSupplierItems = _db.BidAwardedSupplierItems.Where(x => x.BidAwardedSupplierID == record.BidAwardedSupplierID).ToList();
                        foreach (var item in awardedSupplierItems)
                        {

                            if (item.Quanity > 0)
                            {
                                #region item
                                var contractManagementItem = new ContractManagementItem
                                {
                                    ContractManagement = contractManagement,
                                    CatCode = item.CatCode,
                                    CategoryName = item.CategoryName,
                                    ItmCode = item.ItmCode,
                                    ItemName = item.ItemName,
                                    Name = item.Name,
                                    ModelNo=item.ModelNo,
                                    PartNo = item.PartNo,
                                    Quantity = item.Quanity,
                                    Omission = 0,
                                    Addition = 0,
                                    ItemSpecification = item.ItemSpecification,
                                    UoM = item.UoM,
                                    ItemType = item.ItemType,
                                    Type = item.ItemType.ToString(),
                                    UnitPrice = item.FinalUPrice,//updated
                                    Total = item.TPrice,
                                    GLTaxRateID = item.GLTaxRateID,
                                    TaxAmount = item.TaxAmount,

                                    CreatedBy = User.Identity.Name,
                                    DateCreated = DateTime.Today,
                                    ModifiedBy = User.Identity.Name,
                                    LastModified = DateTime.Today
                                };
                                _db.ContractManagementItems.Add(contractManagementItem);


                                _db.SaveChanges();
                                #endregion
                            }

                        }
                    }
                    #endregion
                }

                record.IsTransferredToContract = true;
                _db.BidAwardedSuppliers.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();

            }

        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        public JsonResult TransferAwardToPO(int id)
        {
            var record = _db.BidAwardedSuppliers.Find(id);

            if (record == null)
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            var exisit = _db.PurchaseOrders.Where(x => x.BidAwardedSupplierID == id).FirstOrDefault();
            if(exisit!=null)
            {
                var formattedData = new
                {
                    Message = $"The award number {id} is already available in purchase order.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);

            }

            if (record.BidWinningItems.Count == 0)
            {
                var formattedData = new
                {
                    Message = "Please add award items first.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record != null && !record.IsTransferredToPo && record.BidWinningItems.Count > 0)
            {
                decimal amount = record.BidWinningItems.Sum(x => x.TPrice);
                var lot = _db.Lots.Find(record.LotID);
                if (lot != null)
                {
                    var tender = _db.Tenders.Find(lot.TenderID);
                    #region send to po
                    if (tender != null)
                    {                        
                        #region create po
                        var purchaseOrder = new PurchaseOrder
                        {

                            TenderID = tender.TenderID,
                            BidAwardedSupplierID=id,
                            ProcurementOfficer = tender.ProcurementOfficer,
                            BranchId = tender.BranchId, 
                            BusinessUnitId = tender.BusinessUnitId,
                            SroreRequisition = "",
                            SupplierName = record.SupplierName,
                            SupplierId = record.SupplierId,
                            POType = POType.CashBase,
                            ProcurementMethod = ProcurementMethod.Tender,
                            OrderDate = DateTime.Today,
                            DeliveryPlace = "",
                            DeliveryDate = DateTime.Today,
                            PurchaseRequisitionID=0,
                            OrgStructureId = 0,

                            ForwardedFrom = UserInformationHelper.GetEmployeeUserId(tender.ProcurementOfficer),
                            ForwardedTo = UserInformationHelper.GetEmployeeUserId(tender.ProcurementOfficer),


                            Status = "Editing",
                            Remark = tender.TenderReference+"_"+ record.Remark,

                            DateCreated = DateTime.Now,
                            CreatedBy = User.Identity.Name
                        };
                        _db.PurchaseOrders.Add(purchaseOrder);
                        _db.SaveChanges();


                        #endregion

                        #region items

                        var winnerItems = _db.BidAwardedSupplierItems.Where(x => x.BidAwardedSupplierID == record.BidAwardedSupplierID).ToList();
                        foreach (var item in winnerItems)
                        {
                            var tax = _db.GLTaxRates.Find(item.GLTaxRateID);
                            if (item.Quanity > 0)
                            {
                                #region item

                                if (tax != null)
                                {
                                    var poitem = new PurchaseOrderItem
                                    {
                                        PurchaseOrder = purchaseOrder,
                                        CatCode = item.CatCode,
                                        CategoryName = item.CategoryName,
                                        ItmCode = item.ItmCode,
                                        ItemName = item.ItemName,
                                        Name = item.Name,
                                        ModelNo = item.ModelNo,
                                        PartNo = item.PartNo,
                                        Quantity = item.Quanity,
                                        UnitPrice = item.UPrice,
                                        GLTaxRateID = item.GLTaxRateID,
                                        TaxAmount = (tax.CalculatedRate * item.Quanity * item.UPrice),
                                        SubTotal = item.UPrice * item.Quanity,
                                        Total = item.TPrice,
                                        IsApproved = item.IsApproved,
                                        ItemSpecification = item.ItemSpecification,
                                        UoM = item.UoM,
                                        ItemType = item.ItemType,

                                        CreatedBy = User.Identity.Name,
                                        DateCreated = DateTime.Today
                                    };
                                    _db.PurchaseOrderItems.Add(poitem);
                                }

                                _db.SaveChanges();
                                #endregion
                            }

                        }

                        #endregion

                        #region prs

                        var purchaseRequisitions = _db.LotPurchaseRequisitions.Where(x => x.LotID == lot.LotID).ToList();
                        foreach (var purchaseRequisition in purchaseRequisitions)
                        {

                            var poPurchaseRequisition = new PoPurchaseRequisition
                            {
                                PurchaseOrder = purchaseOrder,
                                PurchaseRequisitionAssignmentId = purchaseRequisition.PurchaseRequisitionAssignmentId,
                                PurchaseRequisitionID = purchaseRequisition.PurchaseRequisitionID,
                                StoreRequisitionID = purchaseRequisition.StoreRequisitionID,
                                CreatedBy = User.Identity.Name,
                                DateCreated = DateTime.Now
                            };
                            _db.PoPurchaseRequisitions.Add(poPurchaseRequisition);
                           _db.SaveChanges();

                        }

                        #endregion
                    }
                    #endregion

                }

                record.IsTransferredToPo = true;
                _db.BidAwardedSuppliers.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();
                var formattedData = new
                {
                    Message = "Transfer to PO succesful.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);

            }

            var formattedData1 = new
            {
                Message = "Transfer to PO not successful.",
                Status = "NOT_OK"
            };
            return Json(formattedData1, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GrnHeader_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            List<GrnViewModel> grnVms = new List<GrnViewModel>();

            var receiveType = _db.ReceiveTypes.FirstOrDefault(x => x.Type == GoodReceiveType.Tender);

            var lots = _db.Lots.Where(x => x.TenderID == id).ToList();
            foreach (var lot in lots)
            {
                var awards = _db.BidAwardedSuppliers.Where(x => x.LotID == lot.LotID).ToList();
                foreach (var bidAwardedSupplier in awards)
                {
                    #region grn stock header
                    var stockReceives = _db.GoodReceives.Where(x => x.ReceiveTypeId == receiveType.ReceiveTypeId).Where(x => x.ReceiveRef == bidAwardedSupplier.BidAwardedSupplierID.ToString());

                    foreach (var goodReceive in stockReceives)
                    {
                        #region grn header
                        GrnViewModel record = new GrnViewModel
                        {
                            GrnId = goodReceive.GoodReceiveId,
                            GrnType = GrnType.Stock,
                            IsManual = false,
                            ReceiveRef = goodReceive.ReceiveRef,
                            ReceiveTypeId = goodReceive.ReceiveTypeId,
                            StoreCode = goodReceive.StoreCode,
                            SupplierName = goodReceive.SupplierName,
                            SupplierId = goodReceive.SupplierId,
                            SupplierInvoiceNo = goodReceive.SupplierInvoiceNo,
                            TenderLot = goodReceive.TenderLot,
                            Reference = goodReceive.Reference,
                            ReceiveDate = goodReceive.ReceiveDate,
                            Status = goodReceive.Status,
                            Remark = goodReceive.Remark,
                            GLPeriodId = goodReceive.GLPeriodId,

                            IsSendForApproval = goodReceive.IsSendForApproval,
                            SendForApprovalBy = goodReceive.SendForApprovalBy,
                            SendForApprovalTime = goodReceive.SendForApprovalTime,

                            IsOnlineApproved = goodReceive.IsOnlineApproved,
                            OnlineApprovedBy = goodReceive.OnlineApprovedBy,
                            OnlineApprovedTime = goodReceive.OnlineApprovedTime,

                            IsVoid = goodReceive.IsVoid,
                            VoidBy = goodReceive.VoidBy,
                            VoidTime = goodReceive.VoidTime,

                            DateCreated = goodReceive.DateCreated,
                            LastModified = goodReceive.LastModified,
                            CreatedBy = goodReceive.CreatedBy,
                            ModifiedBy = goodReceive.ModifiedBy
                        };
                        grnVms.Add(record);
                        #endregion
                    }
                    #endregion
                    #region grn service header
                    var serviceReceives = _db.ServiceReceives.Where(x => x.ReceiveTypeId == receiveType.ReceiveTypeId).Where(x => x.ReceiveRef == bidAwardedSupplier.BidAwardedSupplierID.ToString());

                    foreach (var serviceReceive in serviceReceives)
                    {
                        #region grn service header
                        GrnViewModel record = new GrnViewModel
                        {
                            GrnId = serviceReceive.ServiceReceiveId,
                            GrnType = GrnType.Service,
                            IsManual = false,
                            ReceiveRef = serviceReceive.ReceiveRef,
                            ReceiveTypeId = serviceReceive.ReceiveTypeId,
                            StoreCode = "NA",
                            SupplierName = serviceReceive.SupplierName,
                            SupplierId = serviceReceive.SupplierId,
                            SupplierInvoiceNo = serviceReceive.SupplierInvoiceNo,
                            TenderLot = serviceReceive.TenderLot,
                            Reference = serviceReceive.Reference,
                            ReceiveDate = serviceReceive.ReceiveDate,
                            Status = serviceReceive.Status,
                            Remark = serviceReceive.Remark,
                            GLPeriodId = serviceReceive.GLPeriodId,

                            IsSendForApproval = serviceReceive.IsSendForApproval,
                            SendForApprovalBy = serviceReceive.SendForApprovalBy,
                            SendForApprovalTime = serviceReceive.SendForApprovalTime,

                            IsOnlineApproved = serviceReceive.IsOnlineApproved,
                            OnlineApprovedBy = serviceReceive.OnlineApprovedBy,
                            OnlineApprovedTime = serviceReceive.OnlineApprovedTime,

                            IsVoid = serviceReceive.IsVoid,
                            VoidBy = serviceReceive.VoidBy,
                            VoidTime = serviceReceive.VoidTime,

                            DateCreated = serviceReceive.DateCreated,
                            LastModified = serviceReceive.LastModified,
                            CreatedBy = serviceReceive.CreatedBy,
                            ModifiedBy = serviceReceive.ModifiedBy
                        };
                        grnVms.Add(record);
                        #endregion
                    }
                    #endregion
                    #region grn fixed asset header

                    var fixedAssetReceives = _db.FixedAssetReceives.Where(x => x.ReceiveTypeId == receiveType.ReceiveTypeId).Where(x => x.ReceiveRef == bidAwardedSupplier.BidAwardedSupplierID.ToString());

                    foreach (var fixedAssetReceive in fixedAssetReceives)
                    {
                        #region grn fixed asset header
                        GrnViewModel record = new GrnViewModel
                        {
                            GrnId = fixedAssetReceive.FixedAssetReceiveId,
                            GrnType = GrnType.FixedAsset,
                            IsManual = false,
                            ReceiveRef = fixedAssetReceive.ReceiveRef,
                            ReceiveTypeId = fixedAssetReceive.ReceiveTypeId,
                            StoreCode = fixedAssetReceive.StoreCode,
                            SupplierName = fixedAssetReceive.SupplierName,
                            SupplierId = fixedAssetReceive.SupplierId,
                            SupplierInvoiceNo = fixedAssetReceive.SupplierInvoiceNo,
                            TenderLot = fixedAssetReceive.TenderLot,
                            Reference = fixedAssetReceive.Reference,
                            ReceiveDate = fixedAssetReceive.ReceiveDate,
                            Status = fixedAssetReceive.Status,
                            Remark = fixedAssetReceive.Remark,
                            GLPeriodId = fixedAssetReceive.GLPeriodId,

                            IsSendForApproval = fixedAssetReceive.IsSendForApproval,
                            SendForApprovalBy = fixedAssetReceive.SendForApprovalBy,
                            SendForApprovalTime = fixedAssetReceive.SendForApprovalTime,

                            IsOnlineApproved = fixedAssetReceive.IsOnlineApproved,
                            OnlineApprovedBy = fixedAssetReceive.OnlineApprovedBy,
                            OnlineApprovedTime = fixedAssetReceive.OnlineApprovedTime,

                            IsVoid = fixedAssetReceive.IsVoid,
                            VoidBy = fixedAssetReceive.VoidBy,
                            VoidTime = fixedAssetReceive.VoidTime,

                            DateCreated = fixedAssetReceive.DateCreated,
                            LastModified = fixedAssetReceive.LastModified,
                            CreatedBy = fixedAssetReceive.CreatedBy,
                            ModifiedBy = fixedAssetReceive.ModifiedBy
                        };
                        grnVms.Add(record);
                        #endregion
                    }
                    #endregion
                }
            }
          
          

            IQueryable<GrnViewModel> grnViewModels = grnVms.AsQueryable();
            DataSourceResult result = grnViewModels.ToDataSourceResult(request, grnViewModel => new
            {
                ReceiveRef = grnViewModel.ReceiveRef,
                GrnType = grnViewModel.GrnType,
                IsManual = grnViewModel.IsManual,
                BranchId = grnViewModel.BranchId,
                ReceiveTypeId = grnViewModel.ReceiveTypeId,
                GrnId = grnViewModel.GrnId,
                TenderLot = grnViewModel.TenderLot,
                StoreCode = grnViewModel.StoreCode,
                SupplierName = grnViewModel.SupplierName,
                SupplierInvoiceNo = grnViewModel.SupplierInvoiceNo,
                Reference = grnViewModel.Reference,
                ReceiveDate = grnViewModel.ReceiveDate,
                Status = grnViewModel.Status,
                Remark = grnViewModel.Remark,

                IsSendForApproval = grnViewModel.IsSendForApproval,
                SendForApprovalBy = grnViewModel.SendForApprovalBy,
                SendForApprovalTime = grnViewModel.SendForApprovalTime,

                IsOnlineApproved = grnViewModel.IsOnlineApproved,
                OnlineApprovedBy = grnViewModel.OnlineApprovedBy,
                OnlineApprovedTime = grnViewModel.OnlineApprovedTime,

                IsVoid = grnViewModel.IsVoid,
                VoidBy = grnViewModel.VoidBy,
                VoidTime = grnViewModel.VoidTime,

                DateCreated = grnViewModel.DateCreated,
                LastModified = grnViewModel.LastModified,
                CreatedBy = grnViewModel.CreatedBy,
                ModifiedBy = grnViewModel.ModifiedBy
            });

            return Json(result);
        }
        public ActionResult GrnItem_Read([DataSourceRequest]DataSourceRequest request, int id, GrnType type)
        {
            List<GrnViewModelItem> viewModelItems = new List<GrnViewModelItem>();

            if (type == GrnType.Stock)
            {
                #region stock

                var goodReceiveItems = _db.GoodReceiveItems.Where(x => x.GoodReceiveId == id).ToList();
                foreach (var goodReceiveItem in goodReceiveItems)
                {

                    GrnViewModelItem itemvm = new GrnViewModelItem
                    {
                        GrnViewModelItemID = goodReceiveItem.GoodReceiveItemID,
                        Category = goodReceiveItem.ItemCategory.Name,
                        SubCategory = "",
                        Item = goodReceiveItem.InventoryItem.Name,
                        Quantity = goodReceiveItem.Quantity,
                        UnitPrice = goodReceiveItem.UnitPrice,
                        SubTotal = goodReceiveItem.SubTotal,
                        Total = goodReceiveItem.Total,
                        GLTaxRateID = goodReceiveItem.GLTaxRateID,
                        TaxAmount = goodReceiveItem.TaxAmount,
                        IsExtra = goodReceiveItem.IsExtra,
                        ExtraQuantity = goodReceiveItem.ExtraQuantity,
                        ExtraAmount = goodReceiveItem.ExtraAmount,
                        WarrantyInformation = goodReceiveItem.WarrantyInformation,
                        Specification = goodReceiveItem.Specification,
                        Expire = goodReceiveItem.Expire,
                        ExpireDate = goodReceiveItem.ExpireDate,
                        Balance = goodReceiveItem.Balance,
                        OrgBranchName = goodReceiveItem.OrgBranchName,
                    };
                    viewModelItems.Add(itemvm);
                }

                #endregion

            }
            else if (type == GrnType.Service)
            {
                #region service

                var serviceReceiveItems = _db.ServiceReceiveItems.Where(x => x.ServiceReceiveId == id).ToList();
                foreach (var item in serviceReceiveItems)
                {
                    var serviceCategory = _db.ServiceCategories.FirstOrDefault(x => x.ServiceCategoryCode == item.ServiceCategoryCode);
                    GrnViewModelItem itemvm = new GrnViewModelItem
                    {
                        GrnViewModelItemID = item.ServiceReceiveItemId,
                        Category = serviceCategory?.Name,
                        SubCategory = "",
                        Item = item.ServiceItemName,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        SubTotal = item.SubTotal,
                        Total = item.Total,
                        GLTaxRateID = item.GLTaxRateID,
                        TaxAmount = item.TaxAmount,

                    };
                    viewModelItems.Add(itemvm);
                }

                #endregion

            }

            else if (type == GrnType.FixedAsset)
            {
                #region stock

                var fixedAssetReceiveItems = _db.FixedAssetReceiveItems.Where(x => x.FixedAssetReceiveId == id).ToList();
                foreach (var fixedAssetReceiveItem in fixedAssetReceiveItems)
                {

                    GrnViewModelItem itemvm = new GrnViewModelItem
                    {
                        GrnViewModelItemID = fixedAssetReceiveItem.FixedAssetReceiveItemID,
                        Category = fixedAssetReceiveItem.FixedAssetCategory.CategoryName,
                        SubCategory = fixedAssetReceiveItem.FixedAssetSubCategory.SubCategoryName,
                        Item = fixedAssetReceiveItem.Name,
                        Quantity = fixedAssetReceiveItem.Quantity,
                        UnitPrice = fixedAssetReceiveItem.UnitPrice,
                        SubTotal = fixedAssetReceiveItem.SubTotal,
                        Total = fixedAssetReceiveItem.Total,
                        GLTaxRateID = fixedAssetReceiveItem.GLTaxRateID,
                        TaxAmount = fixedAssetReceiveItem.TaxAmount,
                        IsExtra = fixedAssetReceiveItem.IsExtra,
                        ExtraQuantity = fixedAssetReceiveItem.ExtraQuantity,
                        ExtraAmount = fixedAssetReceiveItem.ExtraAmount,
                        WarrantyInformation = fixedAssetReceiveItem.WarrantyInformation,
                        Specification = fixedAssetReceiveItem.Specification,
                        Expire = fixedAssetReceiveItem.Expire,
                        ExpireDate = fixedAssetReceiveItem.ExpireDate,
                        Balance = fixedAssetReceiveItem.Balance,
                        OrgBranchName = fixedAssetReceiveItem.OrgBranchName,
                    };
                    viewModelItems.Add(itemvm);
                }

                #endregion

            }

            IQueryable<GrnViewModelItem> grnViewModelItems = viewModelItems.AsQueryable();
            DataSourceResult result = grnViewModelItems.ToDataSourceResult(request, fixedAssetReceiveItem => new
            {
                GrnViewModelItemID = fixedAssetReceiveItem.GrnViewModelItemID,
                Category = fixedAssetReceiveItem.Category,
                SubCategory = fixedAssetReceiveItem.SubCategory,
                Item = fixedAssetReceiveItem.Item,
                Quantity = fixedAssetReceiveItem.Quantity,
                UnitPrice = fixedAssetReceiveItem.UnitPrice,
                SubTotal = fixedAssetReceiveItem.SubTotal,
                Total = fixedAssetReceiveItem.Total,
                GLTaxRateID = fixedAssetReceiveItem.GLTaxRateID,
                TaxAmount = fixedAssetReceiveItem.TaxAmount,
                IsExtra = fixedAssetReceiveItem.IsExtra,
                ExtraQuantity = fixedAssetReceiveItem.ExtraQuantity,
                ExtraAmount = fixedAssetReceiveItem.ExtraAmount,
                WarrantyInformation = fixedAssetReceiveItem.WarrantyInformation,
                Specification = fixedAssetReceiveItem.Specification,
                Expire = fixedAssetReceiveItem.Expire,
                ExpireDate = fixedAssetReceiveItem.ExpireDate,
                Balance = fixedAssetReceiveItem.Balance,
                OrgBranchName = fixedAssetReceiveItem.OrgBranchName,
            });

            return Json(result);
        }
        public ActionResult GrnValidation_Read([DataSourceRequest]DataSourceRequest request, int id, GrnType type)
        {
            List<GrnViewModelValidation> grnViewModelValidations = new List<GrnViewModelValidation>();

            if (type == GrnType.Stock)
            {
                #region stock

                var goodReceiveValidations = _db.GoodReceiveValidations.Where(x => x.GoodReceiveId == id).ToList();
                foreach (var validation in goodReceiveValidations)
                {

                    GrnViewModelValidation record = new GrnViewModelValidation
                    {
                        Status = validation.Status,
                        Remark = validation.Remark,
                        OrgBranchName = validation.OrgBranchName,
                        DateCreated = validation.DateCreated,
                        LastModified = validation.LastModified,
                        CreatedBy = validation.CreatedBy,
                        ModifiedBy = validation.ModifiedBy
                    };
                    grnViewModelValidations.Add(record);
                }

                #endregion
            }
            else if (type == GrnType.Service)
            {
                #region service

                var serviceReceiveValidations = _db.ServiceReceiveValidations.Where(x => x.ServiceReceiveId == id).ToList();
                foreach (var validation in serviceReceiveValidations)
                {

                    GrnViewModelValidation record = new GrnViewModelValidation
                    {
                        Status = validation.Status,
                        Remark = validation.Remark,
                        DateCreated = validation.DateCreated,
                        LastModified = validation.LastModified,
                        CreatedBy = validation.CreatedBy,
                        ModifiedBy = validation.ModifiedBy
                    };
                    grnViewModelValidations.Add(record);
                }

                #endregion
            }

            else if (type == GrnType.FixedAsset)
            {
                #region service

                var fixedAssetReceiveValidations = _db.FixedAssetReceiveValidations.Where(x => x.FixedAssetReceiveValidationID == id).ToList();
                foreach (var validation in fixedAssetReceiveValidations)
                {

                    GrnViewModelValidation record = new GrnViewModelValidation
                    {
                        Status = validation.Status,
                        Remark = validation.Remark,
                        DateCreated = validation.DateCreated,
                        LastModified = validation.LastModified,
                        CreatedBy = validation.CreatedBy,
                        ModifiedBy = validation.ModifiedBy
                    };
                    grnViewModelValidations.Add(record);
                }

                #endregion
            }

            IQueryable<GrnViewModelValidation> paymentScheduleViewModels = grnViewModelValidations.AsQueryable();
            DataSourceResult result = paymentScheduleViewModels.ToDataSourceResult(request, paymentScheduleViewModel => new
            {
                Status = paymentScheduleViewModel.Status,
                Remark = paymentScheduleViewModel.Remark,
                OrgBranchName = paymentScheduleViewModel.OrgBranchName,
                DateCreated = paymentScheduleViewModel.DateCreated,
                LastModified = paymentScheduleViewModel.LastModified,
                CreatedBy = paymentScheduleViewModel.CreatedBy,
                ModifiedBy = paymentScheduleViewModel.ModifiedBy
            });

            return Json(result);
        }

        //selectors
        public JsonResult GetLotItemsByLotId(string id)
        {
            int lotid = int.Parse(id);
            var lotitems = _db.LotItems.Where(x => x.LotID == lotid);

            return Json(lotitems.Select(i => new
            {
                Name = /*i.CatCode + "_" + i.CategoryName + "_" +*/ i.ItmCode + "_" + i.ItemName + "_" + i.Name,
                Code = i.LotItemsID
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLotParticipantItemsByLotId(string id)
        {
            int lotid = int.Parse(id);
            var lotitems = _db.BidParticipatingSupplierItems
                .Where(x => x.BidPParticipatingSupplier.LotID == lotid);

            return Json(lotitems.Select(i => new
            {
                Name = /*i.CatCode + "_" + i.CategoryName + "_" +*/ i.ItmCode + "_" + i.ItemName + "_" + i.Name,
                Code = i.LotItemID
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLotResultItemsByLotId(string lot,string award)
        {
            int lotid = int.Parse(lot);
            int awardid = int.Parse(award);
            var awardObj = _db.BidAwardedSuppliers.Find(awardid);
            var lotitems = _db.TenderEvaluationItemResults.Where(x => x.TenderEvaluationResult.LotID == lotid)
                .Where(x => x.TenderEvaluationResult.SupplierName == awardObj.SupplierName);

            return Json(lotitems.Select(i => new
            {
                Name = /*i.CatCode + "_" + i.CategoryName + "_" +*/ i.ItmCode + "_" + i.ItemName + "_" + i.Name,
                Code = i.LotItemID
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Getlots(string id, string lotid)
        {
            int tenderid = int.Parse(id);
            int did = int.Parse(lotid);//self
            var lots = _db.Lots
                .Where(x=>x.TenderID== tenderid)
                .Where(x => x.LotID != did).AsQueryable();
            return Json(lots.Select(i => new
            {
                LotID = i.LotID,
                LotNum =i.LotNumber +"-"+i.Description
            }), JsonRequestBehavior.AllowGet);

        }

        public JsonResult LoadBidPurchaseSuppliers(string id, string lotid)
        {
            int sourceLotId = int.Parse(id);
            int destLotId = int.Parse(lotid);

            if (sourceLotId != destLotId)
            {
                var suppliers = _db.BidPurchasingSupplierses.Where(x => x.LotID == sourceLotId).ToList();

                foreach (var s in suppliers)
                {
                    var exist = _db.BidPurchasingSupplierses
                        .Where(x => x.SupplierName == s.SupplierName)
                        .Where(x => x.LotID == destLotId).FirstOrDefault();
                    if (exist == null)
                    {
                        var entity = new BidPurchasingSuppliers
                        {
                            LotID = destLotId,
                            SupplierId = s.SupplierId,
                            IsNewSupplier = s.IsNewSupplier,
                            SupplierName = s.SupplierName,
                            Representative = s.Representative,
                            PurchaseDate = s.PurchaseDate,
                            ReceiptNo = s.ReceiptNo,
                            ReceiptNotRequired = s.ReceiptNotRequired,
                            Address = s.Address,
                            Telephone = s.Telephone,
                            Remark = s.Remark,
                            OrgBranchName = s.OrgBranchName,
                            LastModified = s.LastModified,
                            CreatedBy = User.Identity.Name,
                            DateCreated = DateTime.Now,
                        };

                        _db.BidPurchasingSupplierses.Add(entity);
                        _db.SaveChanges();

                        #region operation log
                        UserHelper.OperationLog(Request.UserHostAddress, $"lot bid document purchase copied from lot {sourceLotId}", "Success", User.Identity.Name, $"created bid purchase by {entity.SupplierName} under record {entity.LotID} ", Modules.Procurement);
                        #endregion

                    }
                }
                var formattedData1 = new
                {
                    Message = "Operation completed.",
                    Status = "OK"
                };
                return Json(formattedData1, JsonRequestBehavior.AllowGet);
            }

            else
            {
                var formattedData = new
                {
                    Message = "Update completed.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}

