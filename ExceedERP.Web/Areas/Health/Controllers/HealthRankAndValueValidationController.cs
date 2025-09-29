using DocumentFormat.OpenXml.Office2010.Excel;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.AP.PaymentRequest;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Health;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Health.Controllers
{
    public class HealthRankAndValueValidationController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
       
        public JsonResult HealthRankAndValueValidation_Read([DataSourceRequest] DataSourceRequest request, int vId)
        {
            var healths = db.HealthRankAndValueValidation.Where(x=>x.HealthRankAndValueId==vId).ToList();
            var healthValidations = healths.Select(health => new HealthRankAndValueValidation
            {
                HealthRankAndValueValidationId = health.HealthRankAndValueValidationId,
                HealthRankAndValueId = health.HealthRankAndValueId,
                Status = health.Status,
                Remark = health.Remark,
                CreatedBy = health.CreatedBy,
                DateCreated = health.DateCreated,
                ModifiedBy = health.ModifiedBy,
                LastModified = health.LastModified,
                OrgBranchName = health.OrgBranchName,

            }).ToList();

            return Json(healthValidations.ToDataSourceResult(request),JsonRequestBehavior.AllowGet);
        }
        public JsonResult HealthRankAndValueValidation_Create([DataSourceRequest] DataSourceRequest request, HealthRankAndValueValidation health,int vId)
        {
            ModelState.Clear();
            var healandValueList = db.HealthRankAndValueValidation.Where(x => x.Status == ApprovalStatuses.Approve && x.HealthRankAndValueId == vId).ToList();
            if (healandValueList.Any())
            {
                ModelState.AddModelError("exist", "it is approved once!");
            }
            health.HealthRankAndValueId = vId;
            var healthApprovalSetting = db.HealthApprovalSettings.FirstOrDefault();
            if (healthApprovalSetting == null)
            {
                ModelState.AddModelError("exist", "Set the health approval Setting first!");
                return Json(new[] { health }.ToDataSourceResult(request, ModelState));

            }
            if (ModelState.IsValid)
            {
                
                var entity = GetMappedObject(health);
                db.HealthRankAndValueValidation.Add(entity);
                db.SaveChanges();
                var healthHead = db.HealthRankAndValues.Find(vId);
                if (healthHead != null && entity.Status == ApprovalStatuses.Approve&& !healthApprovalSetting.RequiresHrApprovalForPaymentRequest)
                {
                    healthHead.IsSendForApproval = true;
                    healthHead.IsOnlineApproved = true;
                    db.Entry(healthHead).State = EntityState.Modified;
                    db.SaveChanges();
                    var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == healthHead.EmployeeId && x.isCurrent);
                    if (placement != null)
                    {
                        var requestEntity = new PaymentRequest
                        {
                            SupplierId = "0",
                            IsNewSupplier = false,
                            Supplier = " ",
                            Purchaser = 0,
                            ReasonOfPayment = "Health Value",
                            RequestDate = DateTime.Now.Date,
                            RequestDateAm = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year),
                            InvoiceNo = "0",
                            PurchaseRequestNo = "0",
                            PONo = "0",
                            GRN = "0",
                            BranchId = healthHead.BranchId,
                            BusinessUnitId = healthHead.BusinessUnitId,
                            OrgStuructureId = placement.OrgStructureId,
                            EmployeeId = healthHead.EmployeeId,
                            Paid = false,
                            FromBranchId = healthHead.BranchId,
                            FromBusinessUnitId = healthHead.BusinessUnitId,
                            CreatedAtBranchId = healthHead.BranchId,
                            CreatedAtBusinessUnitId = healthHead.BusinessUnitId,
                            CreatedAtOrgStructureId = placement.OrgStructureId,
                            CreatedBy = User.Identity.Name,
                            DateCreated = DateTime.Now.Date,
                            ModifiedBy = User.Identity.Name,
                            LastModified = DateTime.Now.Date,
                            CurrentApprovalStep = 0,
                            IsDepartmentOnlineApproved = true,
                            DepartmentOnlineApprovedBy= User.Identity.Name,
                            DepartmentOnlineApprovedTime = DateTime.Now,



                        };
                        db.PaymentRequests.Add(requestEntity);
                        db.SaveChanges();
                        var glTaxRate = db.HealthExpenseAndTaxSettings.FirstOrDefault();
                        if(glTaxRate != null)
                        {
                            var RequestEntityDetail = new PaymentRequestLine
                            {
                                PaymentRequestId = requestEntity.PaymentRequestId,
                                Amount = healthHead.UsedAmount,
                                TaxAmount = 0,
                                TotalAmount = healthHead.UsedAmount,
                                ItemName = glTaxRate.ApSetupItemId.ToString(),
                                GLTaxRateID = glTaxRate.GLTaxRateID,
                                Description = "Health Payment",
                                Remark = "Description",
                                CreatedBy = User.Identity.Name,
                                DateCreated = DateTime.Now.Date,
                                ModifiedBy = User.Identity.Name,
                                LastModified = DateTime.Now.Date,

                            };
                            db.PaymentRequestLines.Add(RequestEntityDetail);
                            db.SaveChanges();
                        }
                        else
                        {
                            var RequestEntityDetail = new PaymentRequestLine
                            {
                                PaymentRequestId = requestEntity.PaymentRequestId,
                                Amount = healthHead.UsedAmount,
                                TaxAmount = 0,
                                TotalAmount = healthHead.UsedAmount,
                                ItemName = "0",
                                GLTaxRateID = 0,
                                Description = "Health Payment",
                                Remark = "Description",
                                CreatedBy = User.Identity.Name,
                                DateCreated = DateTime.Now.Date,
                                ModifiedBy = User.Identity.Name,
                                LastModified = DateTime.Now.Date,

                            };
                            db.PaymentRequestLines.Add(RequestEntityDetail);
                            db.SaveChanges();
                        }
                        healthHead.PaymentRequestId = requestEntity.PaymentRequestId;
                        db.Entry(healthHead).State = EntityState.Modified;
                        db.SaveChanges();

                        //do attachments

                        var attachments=db.HealthAttachments.Where(x=>x.HealthRankAndValueId==vId).ToList();

                        foreach (var file in attachments)
                        {
                            db.PaymentRequestAttachments.Add(new PaymentRequestAttachment()
                            {
                                FileName = Path.GetFileName(file.FileName),
                                PaymentRequestId = requestEntity.PaymentRequestId,
                                FilePath = file.FilePath

                            });
                            db.SaveChanges();
                        }

                    }
                }
               
                #region operation log   
                UserHelper.OperationLog(Request.UserHostAddress, "Health Validation", "Success", User.Identity.Name, User.Identity.Name + " add new health payment Validation ", Modules.Health);
                #endregion
            }

            return Json(new[] { health }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult HealthRankAndValueValidation_Update([DataSourceRequest] DataSourceRequest request, HealthRankAndValueValidation health)
        {
           
            ModelState.Clear();
            var healandValueList = db.HealthRankAndValueValidation.Where(x => x.Status == ApprovalStatuses.Approve && x.HealthRankAndValueId == health.HealthRankAndValueId&&x.HealthRankAndValueValidationId !=health.HealthRankAndValueValidationId).ToList();
            if (healandValueList.Any())
            {
                ModelState.AddModelError("exist", "It is approved once!");
            }
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(health);
                db.HealthRankAndValueValidation.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { health }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult HealthRankAndValueValidation_Destroy([DataSourceRequest] DataSourceRequest request, HealthRankAndValueValidation health)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {

                var entity = GetMappedObject(health);
                db.HealthRankAndValueValidation.Attach(entity);
                db.HealthRankAndValueValidation.Remove(entity);
                db.SaveChanges();
                


                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Health payment history " + entity.HealthRankAndValueValidationId);
                #endregion
            }

            return Json(new[] { health }.ToDataSourceResult(request, ModelState));
        }

        private HealthRankAndValueValidation GetMappedObject(HealthRankAndValueValidation health)
        {
            var entity = new HealthRankAndValueValidation
            {
                HealthRankAndValueValidationId = health.HealthRankAndValueValidationId,
                HealthRankAndValueId = health.HealthRankAndValueId,
                Status = health.Status,
                Remark = health.Remark,
                CreatedBy = health.CreatedBy,
                DateCreated = health.DateCreated,
                ModifiedBy = health.ModifiedBy,
                LastModified = health.LastModified,
                OrgBranchName = health.OrgBranchName,
            };
            return entity;
        }
        public string CheckStatuses(int masterId)
        {
            var reques = db.HealthRankAndValueValidation.Where(x => x.HealthRankAndValueId == masterId ).FirstOrDefault();
            if (reques !=null &&reques.Status==ApprovalStatuses.Approve)
            {
                
                return "true";
            }
            return "false";
        }
        public decimal AllowedAmount(int id)
        {
            decimal allowedAmount = 0;
            decimal remainingAmount = 0;
            decimal prevUsed = 0;

            var placement = db.Placements.Where(x=>x.isCurrent).OrderByDescending(x => x.PlacementId).FirstOrDefault(x => x.EmployeeId == id);

            if (placement != null)
            {
                var rank = db.HealthRankAndValueSettings.FirstOrDefault(x => x.RankId == placement.RankId);
                if (rank != null && rank.CurrentValue.ToLower() != "unlimited")
                {
                    allowedAmount = Convert.ToDecimal(rank.CurrentValue);
                    // remainingAmount = Convert.ToDecimal(rank.CurrentValue) - Convert.ToDecimal(health.UsedAmount) - prevUsed;
                    //if(remainingAmount < 0)
                    //{
                    //    //ModelState.AddModelError("", "your are using more than your balance");
                    //    var error = "your are using more than your balance";
                    //}
                }
            }

            return allowedAmount;

        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }


    }
}