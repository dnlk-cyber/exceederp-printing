using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Manufacturing.Production;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation;
using ExceedERP.Helpers.Furniture;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers
{
    public class FurnitureFinishingController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["JobType"] = db.FurnitureStandardJobTypes.Select(s => new
            {
                Value = s.FurnitureStandardJobTypeId,
                Text = s.JobTypeName
            }).ToList();
            ViewData["TaskCategory"] = db.ManifucturingTaskCategories.Select(s => new
            {
                Value = s.ManifucturingTaskCategoryId,
                Text = s.Name
            });
            ViewData["Employee"] = db.Person_Employee.Select(s => new
            {
                Text = s.FirstName + "-" + s.MiddleName + "-" + s.LastName,
                Value = s.EmployeeId
            });
            ViewData["Customer"] = db.OrganizationCustomers.Select(s => new
            {
                Value = s.OrganizationCustomerID,
                Text = s.TradeName
            }).ToList();
            return View();
        }

        public ActionResult FurnitureJobOrderProductionsFinishing_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FurnitureJobOrderProduction> furniturejoborderproductions = db.FurnitureJobOrderProductions.Where(x => x.IsSendForFinishing);
            DataSourceResult result = furniturejoborderproductions.ToDataSourceResult(request, furnitureJobOrderProduction => new
            {
                FurnitureJobOrderProductionId = furnitureJobOrderProduction.FurnitureJobOrderProductionId,
                CustomerId = furnitureJobOrderProduction.CustomerId,
                JobTypeId = furnitureJobOrderProduction.JobTypeId,
                JobNo = furnitureJobOrderProduction.JobNo,
                Unit = furnitureJobOrderProduction.Unit,
                Quantity = furnitureJobOrderProduction.Quantity,
                DeliveryDate = furnitureJobOrderProduction.DeliveryDate,
                FurnitureJobOrderFormId = furnitureJobOrderProduction.FurnitureJobOrderFormId,
                IsSendForFinishing = furnitureJobOrderProduction.IsSendForFinishing,
                SendForFinishingBy = furnitureJobOrderProduction.SendForFinishingBy,
                SendForFinishingDate = furnitureJobOrderProduction.SendForFinishingDate,
                IsClosed = furnitureJobOrderProduction.IsClosed,
                ClosedBy = furnitureJobOrderProduction.ClosedBy,
                DateClosed = furnitureJobOrderProduction.DateClosed,
                Status = furnitureJobOrderProduction.Status,
                Remark = furnitureJobOrderProduction.Remark,
                IsVoid = furnitureJobOrderProduction.IsVoid,
                VoidBy = furnitureJobOrderProduction.VoidBy,
                VoidTime = furnitureJobOrderProduction.VoidTime,
                IsOnlineApproved = furnitureJobOrderProduction.IsOnlineApproved,
                OnlineApprovedBy = furnitureJobOrderProduction.OnlineApprovedBy,
                OnlineApprovedTime = furnitureJobOrderProduction.OnlineApprovedTime,
                IsOnlineTransferred = furnitureJobOrderProduction.IsOnlineTransferred,
                IsOnlineTransferredBy = furnitureJobOrderProduction.IsOnlineTransferredBy,
                OnlineTransferredTime = furnitureJobOrderProduction.OnlineTransferredTime,
                IsSendForApproval = furnitureJobOrderProduction.IsSendForApproval,
                OrgBranchName = furnitureJobOrderProduction.OrgBranchName,
                SendForApprovalBy = furnitureJobOrderProduction.SendForApprovalBy,
                SendForApprovalTime = furnitureJobOrderProduction.SendForApprovalTime,
                DateCreated = furnitureJobOrderProduction.DateCreated,
                LastModified = furnitureJobOrderProduction.LastModified,
                CreatedBy = furnitureJobOrderProduction.CreatedBy,
                ModifiedBy = furnitureJobOrderProduction.ModifiedBy
            });

            return Json(result);
        }

        #region Approval
        /// <summary>
        /// This method first iterates through all the jobItems in the jobOrder and if a
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult CloseJob(int id)
        {
            object response = null;
            var production = db.FurnitureJobOrderProductions.Find(id);

            if (production != null)
            {
                var jobOrder = db.FurnitureJobOrderForms.FirstOrDefault(x => x.FurnitureJobOrderFormId == production.FurnitureJobOrderFormId);
                var jobItems = db.FurnitureJobOrderItems.Where(x => x.FurnitureJobOrderFormId == jobOrder.FurnitureJobOrderFormId).ToList();

                int i = 1;
                foreach (var item in jobItems)
                {
                    if (item.IsClosed)
                    {
                        i++;
                    }
                }
                if (i == jobItems.Count())
                {
                    jobOrder.JobClosed = true;
                    db.Entry(jobOrder).State = EntityState.Modified;
                }
                var jobItem = db.FurnitureJobOrderItems.FirstOrDefault(x => x.FurnitureJobOrderFormId == jobOrder.FurnitureJobOrderFormId && x.JobNo == production.JobNo);
                jobItem.IsClosed = true;
                db.Entry(jobItem).State = EntityState.Modified;

                production.IsClosed = true;
                production.ClosedBy = User.Identity.Name;
                production.DateClosed = DateTime.Today;

                db.FurnitureJobOrderProductions.Attach(production);
                db.Entry(production).State = EntityState.Modified;
                db.SaveChanges();

                var result = FurnitureProductionHelper.GenerateItem(production, User.Identity.Name);
                if (!result.Success)
                {
                    return Json(new { Success = false, Message = result.Message });
                }
               
                response = new { Success = true, Message = "Furniture production closed successfully!" };

            }
            else
            {
                response = new { Success = false, Message = "Something is wrong!" };
            }
            return Json(response);
        }
        #endregion

        private bool GenerateItems(FurnitureJobOrderProduction model)
        {
            var itemCategory = db.ItemCategorys.FirstOrDefault(x => x.Name.ToLower().Contains("finishedgoods"));
            if (itemCategory == null)
                return false;
            var jobType = db.FurnitureStandardJobTypes.FirstOrDefault(x => x.FurnitureStandardJobTypeId == model.JobTypeId);
            if (jobType == null)
                return false;
            var item = new InventoryItem
            {
                ItemCategoryCode = db.ItemCategorys.FirstOrDefault(x => x.Name.ToLower().Contains("finishedgoods"))?.ItemCategoryCode ?? "",
                SubCategoryCode = "",
                ItemCode = model.JobNo,
                Name = jobType?.JobTypeName ?? "",
                UoMName = model.Unit,

            };
            db.InventoryItems.Add(item);
            db.SaveChanges();
            if (!Generate_GoodReceive(model, item)) return false;

            return true;
        }

        private bool Generate_GoodReceive(FurnitureJobOrderProduction productionJob, InventoryItem item)
        {

            GoodReceive goodReceive = new GoodReceive()
            {
                StoreCode = db.InventoryStores.FirstOrDefault(x => x.Name.Contains("furniture"))?.StoreCode ?? "",
                ReceiveDate = DateTime.Today,
                GLPeriodId = db.GLPeriods.FirstOrDefault(x => x.DateFrom < DateTime.Today && x.DateTo > DateTime.Today)?.GLPeriodId ?? 0,
                ReceiveTypeId = db.ReceiveTypes.FirstOrDefault(x => x.Type != GoodReceiveType.Other)?.ReceiveTypeId ?? 0,
                ReceiveRef = "",
                Reference = "",
                SupplierId = "",
                SupplierName = "",
                SupplierInvoiceNo = "",
            };
            db.GoodReceives.Add(goodReceive);
            db.SaveChanges();
            if (!Generate_GoodReceiveItem(productionJob, item, goodReceive.GoodReceiveId)) return false;
            return true;
        }

        private bool Generate_GoodReceiveItem(FurnitureJobOrderProduction productionJob, InventoryItem item, int id)
        {
            var jobOrder = db.FurnitureJobOrderForms.FirstOrDefault(x => x.FurnitureJobOrderFormId == productionJob.FurnitureJobOrderFormId);
            if (jobOrder == null)
                return false;
            var JobOrderItem = db.FurnitureJobOrderItems.FirstOrDefault(x => x.JobNo == productionJob.JobNo);
            if (jobOrder == null)
                return false;
            var glTaxRateId = db.FurnitureProformaInvoices
                .FirstOrDefault(x => x.FurnitureProformaInvoiceId == jobOrder.FurnitureProformaInvoiceId)
                ?.GLTaxRateID ?? 0;
            GoodReceiveItem goodReceive = new GoodReceiveItem
            {
                ItemCategoryCode = "",
                ItemCode = "",
                UOM = JobOrderItem.UnitOfMeasurment,
                Quantity = JobOrderItem.Quantity,
                UnitPrice = JobOrderItem.UnitPriceBeforeVAT,
                GLTaxRateID = glTaxRateId,
                SubTotal = JobOrderItem.UnitPriceWithVAT,
                TaxAmount = JobOrderItem.VAT,
                Total = JobOrderItem.UnitPriceWithVAT * JobOrderItem.Quantity,
                Specification = "",
                CreatedBy = User.Identity.Name,
                DateCreated = DateTime.Now,
                GoodReceiveId = id
            };
            db.GoodReceiveItems.Add(goodReceive);
            db.SaveChanges();
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
