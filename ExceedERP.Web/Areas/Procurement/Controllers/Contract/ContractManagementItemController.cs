using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Contract;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Contract
{
   
   
    public class ContractManagementItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementConstants.ProcurementContractUser, ProcurementConstants.ProcurementContractApprover)]
        public ActionResult Index()
        {
            return View();
        }


        [AuthorizeRoles(ProcurementConstants.ProcurementContractUser, FinanceConstants.AccountPayableView, FinanceConstants.AccountPayableView, ProcurementConstants.ProcurementContractApprover)]
        public ActionResult ContractManagementItems_Read(int id, [DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ContractManagementItem> contractmanagementitems = db.ContractManagementItems.Where(x => x.ContractManagementID == id);
            DataSourceResult result = contractmanagementitems.ToDataSourceResult(request, contractManagementItem => new {
                ContractManagementItemID = contractManagementItem.ContractManagementItemID,
                ContractManagementID = contractManagementItem.ContractManagementID,
                CatCode = contractManagementItem.CatCode,
                CategoryName = contractManagementItem.CategoryName,
                ItmCode = contractManagementItem.ItmCode,
                ItemName = contractManagementItem.ItemName,
                Name = contractManagementItem.Name,
                ModelNo = contractManagementItem.ModelNo,
                PartNo = contractManagementItem.PartNo,
                Quantity = contractManagementItem.Quantity,
                Omission = contractManagementItem.Omission,
                Addition = contractManagementItem.Addition,
                ItemSpecification = contractManagementItem.ItemSpecification,
                UoM = contractManagementItem.UoM,
                ItemType = contractManagementItem.ItemType,
                Type = contractManagementItem.Type,
                UnitPrice = contractManagementItem.UnitPrice,
                Total = contractManagementItem.Total,
                TaxAmount = contractManagementItem.TaxAmount,
                GLTaxRateID = contractManagementItem.GLTaxRateID,
                Remark = contractManagementItem.Remark,

                OrgBranchName = contractManagementItem.OrgBranchName,
                DateCreated = contractManagementItem.DateCreated,
                LastModified = contractManagementItem.LastModified,
                CreatedBy = contractManagementItem.CreatedBy,
                ModifiedBy = contractManagementItem.ModifiedBy
            });

            return Json(result);
        }

   
        [AuthorizeRoles(ProcurementConstants.ProcurementContractUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractManagementItems_Create(int id, [DataSourceRequest]DataSourceRequest request, ContractManagementItem contractManagementItem)
        {
            if (id == 0)
            {
                ModelState.AddModelError("id", "Add Contract Management First");
            }

            if((contractManagementItem.Quantity-contractManagementItem.Omission+contractManagementItem.Addition)<=0)
                ModelState.AddModelError("error","Total quantity will be zero or less so ,adjust the omission and addition.");

            var tax = db.GLTaxRates.Find(contractManagementItem.GLTaxRateID);

            decimal netqty = contractManagementItem.Quantity - contractManagementItem.Omission +
                             contractManagementItem.Addition;

            if (ModelState.IsValid)
            {
                if (tax != null)
                {
                    var entity = new ContractManagementItem
                    {
                        ContractManagementID = id,
                        CatCode = contractManagementItem.CatCode,
                        CategoryName = contractManagementItem.CategoryName,
                        ItmCode = contractManagementItem.ItmCode,
                        ItemName = contractManagementItem.ItemName,
                        Name = contractManagementItem.Name,
                        ModelNo = contractManagementItem.ModelNo,
                        PartNo = contractManagementItem.PartNo,
                        Quantity = contractManagementItem.Quantity,
                        Omission = contractManagementItem.Omission,
                        Addition = contractManagementItem.Addition,
                        ItemSpecification = contractManagementItem.ItemSpecification,
                        UoM = contractManagementItem.UoM,
                        ItemType = contractManagementItem.ItemType,
                        Type = contractManagementItem.Type,
                        UnitPrice = contractManagementItem.UnitPrice,
                        TaxAmount = contractManagementItem.UnitPrice * netqty * tax.CalculatedRate,
                        Total = contractManagementItem.UnitPrice * netqty + (contractManagementItem.UnitPrice * netqty * tax.CalculatedRate),
                        Remark = contractManagementItem.Remark,

                        OrgBranchName = contractManagementItem.OrgBranchName,
                        DateCreated = contractManagementItem.DateCreated,
                        LastModified = contractManagementItem.LastModified,
                        CreatedBy = contractManagementItem.CreatedBy,
                        ModifiedBy = contractManagementItem.ModifiedBy
                    };


                    db.ContractManagementItems.Add(entity);
                    db.SaveChanges();
                    contractManagementItem.ContractManagementItemID = entity.ContractManagementItemID;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "contract item", "Success", User.Identity.Name, $"created contract item {entity.ItemName} under record {entity.ContractManagementID} ", Modules.Procurement);
                    #endregion
                }
            }

            return Json(new[] { contractManagementItem }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementContractUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractManagementItems_Update([DataSourceRequest]DataSourceRequest request, ContractManagementItem contractManagementItem)
        {
            var tax = db.GLTaxRates.Find(contractManagementItem.GLTaxRateID);

            decimal netqty = contractManagementItem.Quantity - contractManagementItem.Omission +
                             contractManagementItem.Addition;

            if (ModelState.IsValid)
            {
                if (tax != null)
                {
                    var entity = new ContractManagementItem
                    {
                        ContractManagementID = contractManagementItem.ContractManagementID,
                        ContractManagementItemID = contractManagementItem.ContractManagementItemID,
                        CatCode = contractManagementItem.CatCode,
                        CategoryName = contractManagementItem.CategoryName,
                        ItmCode = contractManagementItem.ItmCode,
                        ItemName = contractManagementItem.ItemName,
                        Name = contractManagementItem.Name,
                        ModelNo = contractManagementItem.ModelNo,
                        PartNo = contractManagementItem.PartNo,
                        Quantity = contractManagementItem.Quantity,
                        Omission = contractManagementItem.Omission,
                        Addition = contractManagementItem.Addition,
                        ItemSpecification = contractManagementItem.ItemSpecification,
                        UoM = contractManagementItem.UoM,
                        ItemType = contractManagementItem.ItemType,
                        Type = contractManagementItem.Type,
                        UnitPrice = contractManagementItem.UnitPrice,
                        TaxAmount = contractManagementItem.UnitPrice * netqty * tax.CalculatedRate,
                        Total = contractManagementItem.UnitPrice * netqty + (contractManagementItem.UnitPrice * netqty * tax.CalculatedRate),
                        Remark = contractManagementItem.Remark,

                        OrgBranchName = contractManagementItem.OrgBranchName,
                        DateCreated = contractManagementItem.DateCreated,
                        LastModified = contractManagementItem.LastModified,
                        CreatedBy = contractManagementItem.CreatedBy,
                        ModifiedBy = contractManagementItem.ModifiedBy
                    };

                    db.ContractManagementItems.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "contract item", "Success", User.Identity.Name, $"updated contract item {entity.ItemName} under record {entity.ContractManagementID} ", Modules.Procurement);
                    #endregion
                }

               
            }

            return Json(new[] { contractManagementItem }.ToDataSourceResult(request, ModelState));
        }

         [AuthorizeRoles(ProcurementConstants.ProcurementContractUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractManagementItems_Destroy([DataSourceRequest]DataSourceRequest request, ContractManagementItem contractManagementItem)
         {
             var contract = db.ContractManagements.Find(contractManagementItem.ContractManagementID);
             if(contract != null && contract.IsOnlineApproved)
                 ModelState.AddModelError("approved","You can't delete the record ,since it is approved.");
            if (ModelState.IsValid)
            {
                var entity = new ContractManagementItem
                {
                    ContractManagementItemID = contractManagementItem.ContractManagementItemID,
                    CatCode = contractManagementItem.CatCode,
                    CategoryName = contractManagementItem.CategoryName,
                    ItmCode = contractManagementItem.ItmCode,
                    ItemName = contractManagementItem.ItemName,
                    PartNo = contractManagementItem.PartNo,
                    Quantity = contractManagementItem.Quantity,
                    Omission = contractManagementItem.Omission,
                    Addition = contractManagementItem.Addition,
                    ItemSpecification = contractManagementItem.ItemSpecification,
                    UoM = contractManagementItem.UoM,
                    ItemType = contractManagementItem.ItemType,
                    Type = contractManagementItem.Type,
                    UnitPrice = contractManagementItem.UnitPrice,
                    Total = contractManagementItem.Total,
                    Remark = contractManagementItem.Remark,

                    OrgBranchName = contractManagementItem.OrgBranchName,
                    DateCreated = contractManagementItem.DateCreated,
                    LastModified = contractManagementItem.LastModified,
                    CreatedBy = contractManagementItem.CreatedBy,
                    ModifiedBy = contractManagementItem.ModifiedBy
                };

                db.ContractManagementItems.Attach(entity);
                db.ContractManagementItems.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { contractManagementItem }.ToDataSourceResult(request, ModelState));
        }

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

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
