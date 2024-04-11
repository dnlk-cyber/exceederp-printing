using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Manufacturing.Production;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers
{
    public class FurnitureJobOrderFormController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["Customer"] = db.OrganizationCustomers.Select(s => new
            {
                Value = s.OrganizationCustomerID,
                Text = s.TradeName
            }).ToList();

            ViewData["JobType"] = db.FurnitureStandardJobTypes.Select(s => new
            {
                Value = s.FurnitureStandardJobTypeId,
                Text = s.JobTypeName
            }).ToList();
            return View();
        }

        public ActionResult FurnitureJobOrderForms_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FurnitureJobOrderForm> furniturejoborderforms = db.FurnitureJobOrderForms;
            DataSourceResult result = furniturejoborderforms.ToDataSourceResult(request, furnitureJobOrderForm => new FurnitureJobOrderForm
            {
                FurnitureJobOrderFormId = furnitureJobOrderForm.FurnitureJobOrderFormId,
                CustomerId = furnitureJobOrderForm.CustomerId,
                DeliveryDate = furnitureJobOrderForm.DeliveryDate,
                CRVNo = furnitureJobOrderForm.CRVNo,
                AdvancePaymentInPercent = furnitureJobOrderForm.AdvancePaymentInPercent,
                FurnitureProformaInvoiceId = furnitureJobOrderForm.FurnitureProformaInvoiceId,
                Status = furnitureJobOrderForm.Status,
                Remark = furnitureJobOrderForm.Remark,
                IsVoid = furnitureJobOrderForm.IsVoid,
                VoidBy = furnitureJobOrderForm.VoidBy,
                VoidTime = furnitureJobOrderForm.VoidTime,
                IsOnlineApproved = furnitureJobOrderForm.IsOnlineApproved,
                OnlineApprovedBy = furnitureJobOrderForm.OnlineApprovedBy,
                OnlineApprovedTime = furnitureJobOrderForm.OnlineApprovedTime,
                IsOnlineTransferred = furnitureJobOrderForm.IsOnlineTransferred,
                IsOnlineTransferredBy = furnitureJobOrderForm.IsOnlineTransferredBy,
                OnlineTransferredTime = furnitureJobOrderForm.OnlineTransferredTime,
                IsSendForApproval = furnitureJobOrderForm.IsSendForApproval,
                OrgBranchName = furnitureJobOrderForm.OrgBranchName,
                SendForApprovalBy = furnitureJobOrderForm.SendForApprovalBy,
                SendForApprovalTime = furnitureJobOrderForm.SendForApprovalTime,
                DateCreated = furnitureJobOrderForm.DateCreated,
                LastModified = furnitureJobOrderForm.LastModified,
                CreatedBy = furnitureJobOrderForm.CreatedBy,
                ModifiedBy = furnitureJobOrderForm.ModifiedBy,
                JobClosed = furnitureJobOrderForm.JobClosed
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureJobOrderForms_Create([DataSourceRequest]DataSourceRequest request, FurnitureJobOrderForm furnitureJobOrderForm)
        {
            #region Validation
            var existingJob = db.FurnitureJobOrderForms.Where(x => x.FurnitureProformaInvoiceId == furnitureJobOrderForm.FurnitureProformaInvoiceId).ToList();
            if (existingJob.Any())
            {
                ModelState.AddModelError(string.Empty, "Proforma Invoice No Exists");
            }
            var exisitngCrvNo = db.FurnitureJobOrderForms.Where(x => x.CRVNo == furnitureJobOrderForm.CRVNo).ToList();
            if (exisitngCrvNo.Any())
            {
                ModelState.AddModelError(string.Empty, "CRv.No Exists");
            }
            #endregion
            if (ModelState.IsValid)
            {
                if (!GenerateJobOrder(furnitureJobOrderForm))
                {
                    ModelState.AddModelError("", "Something's wrong!");
                }
            }
            return Json(new[] { furnitureJobOrderForm }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureJobOrderForms_Update([DataSourceRequest]DataSourceRequest request, FurnitureJobOrderForm furnitureJobOrderForm)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureJobOrderForm
                {
                    FurnitureJobOrderFormId = furnitureJobOrderForm.FurnitureJobOrderFormId,
                    CustomerId = furnitureJobOrderForm.CustomerId,

                    DeliveryDate = furnitureJobOrderForm.DeliveryDate,
                    CRVNo = furnitureJobOrderForm.CRVNo,
                    AdvancePaymentInPercent = furnitureJobOrderForm.AdvancePaymentInPercent,
                    FurnitureProformaInvoiceId = furnitureJobOrderForm.FurnitureProformaInvoiceId,
                    Status = furnitureJobOrderForm.Status,
                    Remark = furnitureJobOrderForm.Remark,
                    IsVoid = furnitureJobOrderForm.IsVoid,
                    VoidBy = furnitureJobOrderForm.VoidBy,
                    VoidTime = furnitureJobOrderForm.VoidTime,
                    IsOnlineApproved = furnitureJobOrderForm.IsOnlineApproved,
                    OnlineApprovedBy = furnitureJobOrderForm.OnlineApprovedBy,
                    OnlineApprovedTime = furnitureJobOrderForm.OnlineApprovedTime,
                    IsOnlineTransferred = furnitureJobOrderForm.IsOnlineTransferred,
                    IsOnlineTransferredBy = furnitureJobOrderForm.IsOnlineTransferredBy,
                    OnlineTransferredTime = furnitureJobOrderForm.OnlineTransferredTime,
                    IsSendForApproval = furnitureJobOrderForm.IsSendForApproval,
                    OrgBranchName = furnitureJobOrderForm.OrgBranchName,
                    SendForApprovalBy = furnitureJobOrderForm.SendForApprovalBy,
                    SendForApprovalTime = furnitureJobOrderForm.SendForApprovalTime,
                    DateCreated = furnitureJobOrderForm.DateCreated,
                    LastModified = furnitureJobOrderForm.LastModified,
                    CreatedBy = furnitureJobOrderForm.CreatedBy,
                    ModifiedBy = furnitureJobOrderForm.ModifiedBy
                };

                db.FurnitureJobOrderForms.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { furnitureJobOrderForm }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureJobOrderForms_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureJobOrderForm furnitureJobOrderForm)
        {
            if (ModelState.IsValid)
            {
                var jobOrder = db.FurnitureJobOrderForms.FirstOrDefault(x => x.FurnitureJobOrderFormId == furnitureJobOrderForm.FurnitureJobOrderFormId);
                if (jobOrder != null)
                {
                    var jobItems = db.FurnitureJobOrderItems.Where(x => x.FurnitureJobOrderFormId == jobOrder.FurnitureJobOrderFormId);
                    if (jobItems.Any())
                    {
                        db.FurnitureJobOrderItems.RemoveRange(jobItems);
                        db.FurnitureJobOrderForms.Remove(jobOrder);
                        db.SaveChanges();
                    }
                }
            }

            return Json(new[] { furnitureJobOrderForm }.ToDataSourceResult(request, ModelState));
        }

        private bool GenerateJobOrder(FurnitureJobOrderForm furnitureJobOrderForm)
        {
            var proformaInvoice = db.FurnitureProformaInvoices.Find(furnitureJobOrderForm.FurnitureProformaInvoiceId);
            if (proformaInvoice != null)
            {
                var proformaItems = db.FurnitureProformaInvoiceItems.Where(x => x.FurnitureProformaInvoiceId == proformaInvoice.FurnitureProformaInvoiceId).ToList();
                if (proformaItems.Any())
                {
                    var jobOrder = new FurnitureJobOrderForm
                    {
                        CustomerId = proformaInvoice.CustomerId,                                               
                        FurnitureProformaInvoiceId = proformaInvoice.FurnitureProformaInvoiceId,
                        DateCreated = DateTime.Today,
                        CreatedBy = User.Identity.Name,
                        CRVNo = furnitureJobOrderForm.CRVNo,
                        PaymentType = furnitureJobOrderForm.PaymentType,
                        AdvancePaymentInPercent = furnitureJobOrderForm.AdvancePaymentInPercent,
                        Remark = furnitureJobOrderForm.Remark

                    };
                    jobOrder.DeliveryDate = DateTime.Today.AddDays(proformaInvoice.DeliveryPeriod);
                    db.FurnitureJobOrderForms.Add(jobOrder);
                    db.SaveChanges();
                    foreach (var proformaItem in proformaItems)
                    {
                        var jobOrderItem = new FurnitureJobOrderItem
                        {
                            CreatedBy = User.Identity.Name,
                            DateCreated = DateTime.Today,
                            Quantity = proformaItem.Quantity,
                            JobTypeId = proformaItem.JobTypeId,
                            UnitOfMeasurment = proformaItem.UnitOfMeasurment,
                            UnitPriceBeforeVAT = proformaItem.UnitPriceBeforeVAT,
                            VAT = proformaItem.VAT,
                            UnitPriceWithVAT = proformaItem.UnitPriceWithVAT,
                            FurnitureJobOrderFormId = jobOrder.FurnitureJobOrderFormId,

                        };
                        db.FurnitureJobOrderItems.Add(jobOrderItem);
                        db.SaveChanges();
                        furnitureJobOrderForm.FurnitureJobOrderFormId = jobOrder.FurnitureJobOrderFormId;
                        
                    }
                    return true;
                }
            }
            return false;

        }
        public JsonResult CheckJob(int id)
        {
            object response = null;
            var jobOrder = db.FurnitureJobOrderForms.Find(id);
            if (jobOrder != null)
            {
                var validity = CheckJobItemValidity(jobOrder.FurnitureJobOrderFormId);
                if (validity)
                {
                    jobOrder.IsSendForApproval = true;
                    jobOrder.SendForApprovalBy = User.Identity.Name;
                    jobOrder.SendForApprovalTime = DateTime.Today;

                    db.FurnitureJobOrderForms.Attach(jobOrder);
                    db.Entry(jobOrder).State = EntityState.Modified;
                    db.SaveChanges();
                    response = new { Success = true, Message = "Job Order Checked Successfully!" };
                }
                else
                {
                    response = new { Success = false, Message = "Something is wrong! Please assign Job.No under job items!" };
                }
            }
            else
            {
                response = new { Success = false, Message = "Something is wrong!" };
            }
            return Json(response);
        }
        private bool CheckJobItemValidity(int id)
        {
            var jobItems = db.FurnitureJobOrderItems.Where(x => x.FurnitureJobOrderFormId == id).ToList();
            if (!jobItems.Any())
            {
                return false;
            }
            foreach (var item in jobItems)
            {
                if (string.IsNullOrWhiteSpace(item.JobNo))
                    return false;
            }
            return true;
        }
        public JsonResult ApproveJobOrder(int id)
        {
            object response = null;
            var jobOrder = db.FurnitureJobOrderForms.Find(id);
            if (jobOrder != null)
            {



                jobOrder.IsOnlineApproved = true;
                jobOrder.OnlineApprovedBy = User.Identity.Name;
                jobOrder.OnlineApprovedTime = DateTime.Today;


                db.FurnitureJobOrderForms.Attach(jobOrder);
                db.Entry(jobOrder).State = EntityState.Modified;
                db.SaveChanges();
                //--Generate Productions--//
                GenerateProduction(jobOrder);

                response = new { Success = true, Message = "Successfully Approved and Created Production!" };
            }
            else
            {
                response = new { Success = false, Message = "Something is wrong!" };
            }
            return Json(response);
        }

        private void GenerateProduction(FurnitureJobOrderForm jobOrder)
        {
            var jobItems = db.FurnitureJobOrderItems.Where(x => x.FurnitureJobOrderFormId == jobOrder.FurnitureJobOrderFormId).ToList();
            if (jobItems.Any())
            {
                foreach (var item in jobItems)
                {
                    var billOfQuantity = db.FurnitureBillOfQuantities.FirstOrDefault(x => x.JobTypeId == item.JobTypeId);
                    if (billOfQuantity != null)
                    {
                        var billOfMaterial = db.FurnitureBOQMaterials.FirstOrDefault(x => x.FurnitureBillOfQuantityId == billOfQuantity.FurnitureBillOfQuantityId);
                        var billOfLabor = db.FurnitureBOQLabors.FirstOrDefault(x => x.FurnitureBillOfQuantityId == billOfQuantity.FurnitureBillOfQuantityId);

                        if (billOfMaterial != null && billOfLabor != null)
                        {
                            var jobOrderProduction = new FurnitureJobOrderProduction
                            {
                                CustomerId = jobOrder.CustomerId,
                                DeliveryDate = jobOrder.DeliveryDate,
                                JobNo = item.JobNo,
                                JobTypeId = item.JobTypeId,
                                Quantity = item.Quantity,
                                Unit = item.UnitOfMeasurment,
                                FurnitureJobOrderFormId = jobOrder.FurnitureJobOrderFormId
                            };
                            db.FurnitureJobOrderProductions.Add(jobOrderProduction);
                            db.SaveChanges();

                            var productionBoQMaterial = new FurnitureProductionBillOfMaterial
                            {
                                Quantity = billOfMaterial.Quantity,
                                ItemCode = billOfMaterial.ItemCode,
                                ManufacturingMaterialCategoryId = billOfMaterial.ManufacturingMaterialCategoryId,
                                ManufacturingMaterialCategoryItemId = billOfMaterial.ManufacturingMaterialCategoryItemId,
                                UnitOfMeasurement = billOfMaterial.UnitOfMeasurement,
                                WorkshopType = billOfMaterial.WorkshopType,
                                Remark = billOfMaterial.Remark,
                                FurnitureJobOrderProductionId = jobOrderProduction.FurnitureJobOrderProductionId
                            };
                            db.FurnitureProductionBillOfMaterials.Add(productionBoQMaterial);


                            var productionBoqLabor = new FurnitureProductionBillOfLabor
                            {
                                EstimatedMinute = billOfLabor.EstimatedMinute,
                                ManufacturingTaskCategoryId = billOfLabor.ManufacturingTaskCategoryId,
                                NumberOfEmloyees = billOfLabor.NumberOfEmloyees,
                                WorkShopType = billOfLabor.WorkShopType,
                                FurnitureJobOrderProductionId = jobOrderProduction.FurnitureJobOrderProductionId
                            };
                            db.FurnitureProductionBillOfLabors.Add(productionBoqLabor);

                            db.SaveChanges();
                        }
                    }
                }

            }
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
