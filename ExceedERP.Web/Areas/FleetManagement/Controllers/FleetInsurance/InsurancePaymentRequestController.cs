using System.Data.Entity;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Helpers.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.FleetManagement.FleetTimeSheet;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.FleetManagement.FleetInsurances;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.Global;
using System.IO;
using ExceedERP.Core.Domain.Procurement.PurchaseOrder;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.Setting
{
    //[AuthorizeRoles(FleetManagementRoles.FleetTimeSheet)]
    public class InsurancePaymentRequestController : Controller
    {
        private readonly CacheData CacheData = new CacheData();
        ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            PopulateData();
            return View();
        }
        public ActionResult Budget_Index()
        {
            PopulateData();
            return View();
        }
        public ActionResult Finance_Index()
        {
            PopulateData();
            return View();
        }
       public void PopulateData()
        { 
            ViewData["Category"] = CacheData.GetEquipmentIdentityCaches().Select(
      i => new
      {
          Text = i.Name,
          Value = i.Id
      }).AsQueryable();
            ViewData["Type"] = CacheData.GetEquipmentTypeCaches().Select(
        i => new
        {
            Text = i.Name,
            Value = i.Id
        }).AsQueryable();

            var setting = db.RequestToInspections.FirstOrDefault();
            ViewBag.setting = setting != null ? setting.Remark : " ";
            ViewData["Name"] = CacheData.GetEquipmentNameCaches().Select(
        i => new
        {
            Text = i.Name,
            Value = i.Id
        }).AsQueryable();
            ViewData["InsuranceCover"] = db.InsuranceCoverTypes.Select(p => new
            {
                Id = p.InsuranceCoverTypeId,
                Name = p.Code + "-" + p.Name + " " + p.Description
            }

                ).AsQueryable();
            ViewData["Fleet"] = db.Fleets.Select(s => new
            {
                Value = s.FleetsID,
                Text = s.PlateNumber + " " + s.SerialNo
            });
            ViewData["fleet"] = CacheData.GetFleetsSecondCache().Select(x => new { Id = x.FleetsID, Name = x.PlateNumber }).ToList();
            ViewData["InsuranceCompany"] = db.InsuranceCompany.Select(x => new { x.InsuranceCompanyId, x.InsuranceCompanyName });
        }

        public JsonResult Seed() 
        {
            var fleetInsurances = db.FleetInsurance.ToList();
            foreach (var item in fleetInsurances)
            {
                var fleet = db.Fleets.Find(item.FleetsID);
                if(fleet != null)
                {
                    item.EquipmentCategoryId = fleet?.EquipmentIdentityID ?? Guid.Empty;
                    item.EquipmentTypeId = fleet?.EquipmentTypeID ?? Guid.Empty;
                    item.EquipmentNameId = fleet?.EquipmentNameID ?? Guid.Empty;
                    db.Entry(item).State = EntityState.Modified;
                }
                
            }
            db.SaveChanges();
            return Json("Succcessfully Assigned Data", JsonRequestBehavior.AllowGet);
        }
        private bool IsExpired(DateTime? expireDate)
        {
            DateTime currentDate = DateTime.Now;
            TimeSpan timeDifference = (DateTime)expireDate - currentDate;
            int daysRemaining = timeDifference.Days;
            if(daysRemaining <= 15)
            {
                return true;
            }
            return false;
        }
        private StatusEnum IsExpire(DateTime? expireDate)
        {
            DateTime currentDate = DateTime.Now;
            TimeSpan timeDifference = (DateTime)expireDate - currentDate;
            int daysRemaining = timeDifference.Days;
            if(daysRemaining <= 15)
            {
               return StatusEnum.Expired;
            }
            return StatusEnum.Active;
        }


        //public ActionResult FleetInsurances_Read([DataSourceRequest] DataSourceRequest request)
        //{
        //    var fleetInsurances = db.FleetInsurance;

        //    DataSourceResult result = fleetInsurances.ToDataSourceResult(request, fleetsInsurance => new FleetInsurance
        //    {
        //        FleetInsuranceId = fleetsInsurance.FleetInsuranceId,
        //        FleetsID = fleetsInsurance.FleetsID,
        //        InsuranceCompanyId = fleetsInsurance.InsuranceCompanyId,
        //        PremiumCost = fleetsInsurance.PremiumCost,
        //        DueDate = fleetsInsurance.DueDate,
        //        ExpireDate = fleetsInsurance.ExpireDate,
        //        AddToHistory=fleetsInsurance.AddToHistory,
        //        HasExpired = IsExpired(fleetsInsurance.DueDate),
        //        Attachment = fleetsInsurance.Attachment,
        //        Description = fleetsInsurance.Description,
        //        PolicyNo = fleetsInsurance.PolicyNo,
        //        ThirdPartyInsurance = fleetsInsurance.ThirdPartyInsurance,
        //        ComprehensiveInsurance = fleetsInsurance.ComprehensiveInsurance,
        //        InsuranceType = fleetsInsurance.InsuranceType,
        //        CreatedBy = fleetsInsurance.CreatedBy,
        //        DateCreated = fleetsInsurance.DateCreated,
        //        EquipmentCategoryId = fleetsInsurance.EquipmentCategoryId,
        //        EquipmentTypeId = fleetsInsurance.EquipmentTypeId,
        //        EquipmentNameId = fleetsInsurance.EquipmentNameId

        //    });

        //    return Json(result);
        //}

        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            // Execute query in memory
            var fleetInsurancePR = db.FleetInsurancePaymentRequest.AsQueryable();
              DataSourceResult result=fleetInsurancePR.ToDataSourceResult(request,f => new
                                  {
                                      f.FleetInsurancePaymentRequestId,
                                      f.FleetInsuranceId,
                                      f.FleetsID,
                                      f.InsuranceCompanyId,
                                      f.InsuranceCoverTypeId,
                  f.InsuranceType,
                  f.CreatedBy,
                                      f.DateCreated,
                                      f.EquipmentCategoryId,
                                      f.EquipmentTypeId,
                                      f.EquipmentNameId,
                                      f.IsOnlineApproved,
                                      f.IsSendForApproval,
                  f.EmployeeId,
                  f.ReasonofPayment,
                  f.DurationFrom,
                  f.DurationTo,
                  f.Status,
                  f.IsBudgetApproved,
                  f.BudgetApprovedBy,
                  f.BudgetApprovedTime,
                  f.OnlineApprovedBy,
                  f.OnlineApprovedTime,
                  f.IsAuthorized,
                  f.AuthorizedBy,
                  f.AuthorizedTime,
                  f.SendForApprovalBy,
                  f.SendForApprovalTime,
                  f.IsOnlineTransferred,
                  f.IsOnlineTransferredBy,
                  f.LastModified,
                  f.ModifiedBy,
                  f.IsFinanceSendForApproval,
                  f.FinanceSendForApprovalBy,
                  f.FinanceSendForApprovalTime,
                  f.IsFinanceApproved,
                  f.FinanceApprovedBy,
                  f.FinanceApprovedTime,
                  f.AmountDue,
                  f.TotalAmount,
                  f.IsVoid,
                  f.Paid,
                  f.VoidBy,
                  f.VoidTime,

              });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
         public ActionResult Budget_Read([DataSourceRequest] DataSourceRequest request)
        {
            // Execute query in memory
            var fleetInsurancePR = db.FleetInsurancePaymentRequest.Where(x=>x.IsOnlineApproved).AsQueryable();
              DataSourceResult result=fleetInsurancePR.ToDataSourceResult(request,f => new
                                  {
                                      f.FleetInsurancePaymentRequestId,
                                      f.FleetInsuranceId,
                                      f.FleetsID,
                                      f.InsuranceCompanyId,
                                      f.InsuranceCoverTypeId,
                  f.InsuranceType,
                  f.CreatedBy,
                                      f.DateCreated,
                                      f.EquipmentCategoryId,
                                      f.EquipmentTypeId,
                                      f.EquipmentNameId,
                                      f.IsOnlineApproved,
                                      f.IsSendForApproval,
                  f.EmployeeId,
                  f.ReasonofPayment,
                  f.DurationFrom,
                  f.DurationTo,
                  f.Status,
                  f.IsBudgetApproved,
                  f.BudgetApprovedBy,
                  f.BudgetApprovedTime,
                  f.OnlineApprovedBy,
                  f.OnlineApprovedTime,
                  f.IsAuthorized,
                  f.AuthorizedBy,
                  f.AuthorizedTime,
                  f.SendForApprovalBy,
                  f.SendForApprovalTime,
                  f.IsOnlineTransferred,
                  f.IsOnlineTransferredBy,
                  f.LastModified,
                  f.ModifiedBy,
                  f.IsFinanceSendForApproval,
                  f.FinanceSendForApprovalBy,
                  f.FinanceSendForApprovalTime,
                  f.IsFinanceApproved,
                  f.FinanceApprovedBy,
                  f.FinanceApprovedTime,
                  f.AmountDue,
                  f.TotalAmount,
                  f.IsVoid,
                  f.Paid,
                  f.VoidBy,
                  f.VoidTime,

              });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Finance_Read([DataSourceRequest] DataSourceRequest request)
        {
            // Execute query in memory
            var fleetInsurancePR = db.FleetInsurancePaymentRequest.Where(x=>x.IsBudgetApproved).AsQueryable();
              DataSourceResult result=fleetInsurancePR.ToDataSourceResult(request,f => new
                                  {
                                      f.FleetInsurancePaymentRequestId,
                                      f.FleetInsuranceId,
                                      f.FleetsID,
                                      f.InsuranceCompanyId,
                                      f.InsuranceCoverTypeId,
                  f.InsuranceType,
                  f.CreatedBy,
                                      f.DateCreated,
                                      f.EquipmentCategoryId,
                                      f.EquipmentTypeId,
                                      f.EquipmentNameId,
                                      f.IsOnlineApproved,
                                      f.IsSendForApproval,
                  f.EmployeeId,
                  f.ReasonofPayment,
                  f.DurationFrom,
                  f.DurationTo,
                  f.Status,
                  f.IsBudgetApproved,
                  f.BudgetApprovedBy,
                  f.BudgetApprovedTime,
                  f.OnlineApprovedBy,
                  f.OnlineApprovedTime,
                  f.IsAuthorized,
                  f.AuthorizedBy,
                  f.AuthorizedTime,
                  f.SendForApprovalBy,
                  f.SendForApprovalTime,
                  f.IsOnlineTransferred,
                  f.IsOnlineTransferredBy,
                  f.LastModified,
                  f.ModifiedBy,
                  f.IsFinanceSendForApproval,
                  f.FinanceSendForApprovalBy,
                  f.FinanceSendForApprovalTime,
                  f.IsFinanceApproved,
                  f.FinanceApprovedBy,
                  f.FinanceApprovedTime,
                  f.AmountDue,
                  f.TotalAmount,
                  f.IsVoid,
                  f.Paid,
                  f.VoidBy,
                  f.VoidTime,

              });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create([DataSourceRequest] DataSourceRequest request, FleetInsurancePaymentRequest fleetsInsurance)
        {
            var existingInsurnace = db.FleetInsurance.FirstOrDefault(x => x.FleetsID == fleetsInsurance.FleetsID && DbFunctions.TruncateTime(x.DueDate) == DbFunctions.TruncateTime(fleetsInsurance.DurationFrom) && DbFunctions.TruncateTime(x.ExpireDate) == DbFunctions.TruncateTime(fleetsInsurance.DurationTo) && x.InsuranceCompanyId == fleetsInsurance.InsuranceCompanyId && x.InsuranceCoverTypeId == fleetsInsurance.InsuranceCoverTypeId && x.InsuranceType == fleetsInsurance.InsuranceType && !x.IsPaymentRequestGenerated);
             if(existingInsurnace==null)
            {
                ModelState.AddModelError("No Isurance", "No registered insurance with this information");
            }
             var existingPR=db.FleetInsurancePaymentRequest.FirstOrDefault(x => x.FleetsID == fleetsInsurance.FleetsID && DbFunctions.TruncateTime(x.DurationFrom) == DbFunctions.TruncateTime(fleetsInsurance.DurationFrom) && DbFunctions.TruncateTime(x.DurationTo) == DbFunctions.TruncateTime(fleetsInsurance.DurationTo) && x.InsuranceCompanyId == fleetsInsurance.InsuranceCompanyId && x.InsuranceCoverTypeId == fleetsInsurance.InsuranceCoverTypeId && x.InsuranceType == fleetsInsurance.InsuranceType && !x.IsVoid);
            if(existingPR!=null)
            {
                ModelState.AddModelError("exists", "Payment Request already exists");
            }
            fleetsInsurance.AmountDue = existingInsurnace.ThirdPartyInsurance + existingInsurnace.PremiumCost + existingInsurnace.ComprehensiveInsurance+existingInsurnace.Payment;
            fleetsInsurance.TotalAmount = existingInsurnace.ThirdPartyInsurance + existingInsurnace.PremiumCost + existingInsurnace.ComprehensiveInsurance+existingInsurnace.Payment;

            if (ModelState.IsValid)
            {
                var entity = new FleetInsurancePaymentRequest
                {

                    FleetsID = fleetsInsurance.FleetsID,
                    InsuranceCompanyId = fleetsInsurance.InsuranceCompanyId,
                    DurationFrom = fleetsInsurance.DurationFrom,
                    DurationTo = fleetsInsurance.DurationTo,
                    InsuranceCoverTypeId = fleetsInsurance.InsuranceCoverTypeId,
                    AmountDue = fleetsInsurance.AmountDue,
                    InsuranceType = fleetsInsurance.InsuranceType,
                    Status = "Pending",
                TotalAmount = fleetsInsurance.TotalAmount,
                    EquipmentCategoryId = fleetsInsurance.EquipmentCategoryId,
                    EquipmentTypeId = fleetsInsurance.EquipmentTypeId,
                    EquipmentNameId = fleetsInsurance.EquipmentNameId
                };
                entity.RequestDate = DateTime.Now;

             
                try
                {
                    db.FleetInsurancePaymentRequest.Add(entity);
                    db.SaveChanges();
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
                {
                    var sqlEx = ex.InnerException;

                    while (sqlEx.InnerException != null)
                    {
                        sqlEx = sqlEx.InnerException;
                    }

                    throw new Exception(sqlEx.Message);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                    throw;
                }
                fleetsInsurance.FleetInsuranceId = entity.FleetInsuranceId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Fleet insurance payment request", "Success", User.Identity.Name, $"created insurance PR {entity.FleetInsurancePaymentRequestId} ", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { fleetsInsurance }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Update([DataSourceRequest] DataSourceRequest request, FleetInsurancePaymentRequest fleetsInsurance)
        {
            var existingInsurnace = db.FleetInsurance.FirstOrDefault(x => x.FleetsID == fleetsInsurance.FleetsID && DbFunctions.TruncateTime(x.DueDate) == DbFunctions.TruncateTime(fleetsInsurance.DurationFrom) && DbFunctions.TruncateTime(x.ExpireDate) == DbFunctions.TruncateTime(fleetsInsurance.DurationTo) && x.InsuranceCompanyId == fleetsInsurance.InsuranceCompanyId && x.InsuranceCoverTypeId == fleetsInsurance.InsuranceCoverTypeId && x.InsuranceType == fleetsInsurance.InsuranceType && !x.IsPaymentRequestGenerated);
            if (existingInsurnace == null)
            {
                ModelState.AddModelError("No Isurance", "No registered insurance with this information");
            }

            fleetsInsurance.AmountDue = existingInsurnace.ThirdPartyInsurance + existingInsurnace.PremiumCost + existingInsurnace.ComprehensiveInsurance + existingInsurnace.Payment;
            fleetsInsurance.TotalAmount = existingInsurnace.ThirdPartyInsurance + existingInsurnace.PremiumCost + existingInsurnace.ComprehensiveInsurance + existingInsurnace.Payment;

            if (ModelState.IsValid)
            {
                var existingPR = db.FleetInsurancePaymentRequest.FirstOrDefault(x => x.FleetInsurancePaymentRequestId == fleetsInsurance.FleetInsurancePaymentRequestId);
                existingPR.FleetsID = fleetsInsurance.FleetsID;
                existingPR.InsuranceCompanyId = fleetsInsurance.InsuranceCompanyId;
                existingPR.DurationFrom = fleetsInsurance.DurationFrom;
                existingPR.DurationTo = fleetsInsurance.DurationTo;
                existingPR.InsuranceCoverTypeId = fleetsInsurance.InsuranceCoverTypeId;
                existingPR.AmountDue = fleetsInsurance.AmountDue;
                existingPR.InsuranceType = fleetsInsurance.InsuranceType;
                existingPR.TotalAmount = fleetsInsurance.TotalAmount;
                existingPR.EquipmentCategoryId = fleetsInsurance.EquipmentCategoryId;
                existingPR.EquipmentTypeId = fleetsInsurance.EquipmentTypeId;
                existingPR.EquipmentNameId = fleetsInsurance.EquipmentNameId;


                db.Entry(existingPR).State = EntityState.Modified;
                db.SaveChanges();
                
            }

            return Json(new[] { fleetsInsurance }.ToDataSourceResult(request, ModelState));
        }


        public ActionResult Destroy([DataSourceRequest] DataSourceRequest request, FleetInsurancePaymentRequest fleetsInsurance)
        {
            if (ModelState.IsValid)
            {
                var existingPR = db.FleetInsurancePaymentRequest.FirstOrDefault(x => x.FleetInsurancePaymentRequestId == fleetsInsurance.FleetInsurancePaymentRequestId);

                
                db.FleetInsurancePaymentRequest.Remove(existingPR);
                db.SaveChanges();
               
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted insurance PR number {existingPR.FleetInsurancePaymentRequestId }");
                #endregion
            }

            return Json(new[] { fleetsInsurance }.ToDataSourceResult(request, ModelState));
        }
       
        public JsonResult GetCoverType()
        {

            return Json(db.InsuranceCoverTypes.Where(x => x.IsActive).Select(p => new
            {
                Id = p.InsuranceCoverTypeId,
                Name = p.Code + "-" + p.Name + " " + p.Description
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult SendForApproval(int id)
        {
            var record = db.FleetInsurancePaymentRequest.FirstOrDefault(x => x.FleetInsurancePaymentRequestId == id);
            record.IsSendForApproval = true;
            record.SendForApprovalBy = User.Identity.Name;
            record.SendForApprovalTime = DateTime.Now;
            record.Status = "Waiting Approval";
            db.Entry(record).State = EntityState.Modified;
            db.SaveChanges();
            var formattedData = new
            {
                Message = "Send For Approval Success",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Admin_Read([DataSourceRequest] DataSourceRequest request, int Id)
        {
            var Admin = new List<FleetInsurancePaymentRequestValidation>();
            var Request = db.FleetInsurancePaymentRequestValidation.Where(x => x.FleetInsurancePaymentRequestId == Id).FirstOrDefault();
            if (Request != null)
            {

                Admin =
                    db.FleetInsurancePaymentRequestValidation
                        .Where(x => x.FleetInsurancePaymentRequestId == Id)
                        .ToList();
            }
            return Json(Admin.ToDataSourceResult(request));
        }

        public ActionResult Admin_Create(int id, [DataSourceRequest]DataSourceRequest request, FleetInsurancePaymentRequestValidation Validation)
        {
            var record = db.FleetInsurancePaymentRequest.FirstOrDefault(x => x.FleetInsurancePaymentRequestId == id);
            
            var hasApproved = db.FleetInsurancePaymentRequestValidation
                    .Where(x => x.FleetInsurancePaymentRequestId == id)
                .Where(x => x.Status == ApprovalStatuses.Approve).AsQueryable();
            if (hasApproved.Any())
                ModelState.AddModelError("hasapproved", "You can't approve again.You have already approved.");
            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    if (Validation.Status == ApprovalStatuses.Approve)
                    {
                        record.IsOnlineApproved = true;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime= DateTime.Now;
                        record.Status = "Approved";
                        //var subCompartments = db.HarvestedSubCompartment.Where(x => x.PlantationHarvestingYearId == inventoryYear.PlantationHarvestingYearId).ToList();

                        db.Entry(record).State = EntityState.Modified;
                        db.SaveChanges();
                        
                    }

                    if (Validation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.SendForApprovalTime = DateTime.Now;
                        record.Status = "Rejected";
                        record.SendForApprovalBy = " ";
                        db.Entry(record).State = EntityState.Modified;
                        db.SaveChanges();

                    }

                    var entity = new FleetInsurancePaymentRequestValidation
                    {
                        FleetInsurancePaymentRequestId = id,
                        Status = Validation.Status,
                        Remark = Validation.Remark,                       
                        DateCreated = DateTime.Now,
                        LastModified = Validation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = Validation.ModifiedBy
                    };

                    db.FleetInsurancePaymentRequestValidation.Add(entity);
                    db.SaveChanges();
                }
            }

            return Json(new[] { Validation }.ToDataSourceResult(request, ModelState));
        }
        public JsonResult BudgetApprove(int id)
        {
            var record = db.FleetInsurancePaymentRequest.FirstOrDefault(x => x.FleetInsurancePaymentRequestId == id);
            record.IsBudgetApproved = true;
            record.BudgetApprovedBy = User.Identity.Name;
            record.BudgetApprovedTime = DateTime.Now;
            record.Status = "Budget Approved";
            db.Entry(record).State = EntityState.Modified;
            db.SaveChanges();
            var formattedData = new
            {
                Message = "Approval Success",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult BudgetReject(int id)
        {
            var record = db.FleetInsurancePaymentRequest.FirstOrDefault(x => x.FleetInsurancePaymentRequestId == id);
            record.IsOnlineApproved = false;
            record.OnlineApprovedBy = "";
            record.BudgetApprovedTime = DateTime.Now;
            record.Status = "Budget Rejected";
            db.Entry(record).State = EntityState.Modified;
            db.SaveChanges();
            var formattedData = new
            {
                Message = "Rejection Success",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FinanceSendForApproval(int id)
        {
            var record = db.FleetInsurancePaymentRequest.FirstOrDefault(x => x.FleetInsurancePaymentRequestId == id);
            record.IsFinanceSendForApproval = true;
            record.FinanceSendForApprovalBy = "";
            record.FinanceSendForApprovalTime = DateTime.Now;
            record.Status = "Waiting Finance Approval";
            db.Entry(record).State = EntityState.Modified;
            db.SaveChanges();
            var formattedData = new
            {
                Message = "Send For Finance Approal",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FinanceApprove(int id)
        {
            var record = db.FleetInsurancePaymentRequest.FirstOrDefault(x => x.FleetInsurancePaymentRequestId == id);
            record.IsFinanceApproved = true;
            record.FinanceApprovedBy = User.Identity.Name;
            record.FinanceApprovedTime = DateTime.Now;
            record.Status = "Finance Approved";
            db.Entry(record).State = EntityState.Modified;
            db.SaveChanges();
            var formattedData = new
            {
                Message = "Approval Success",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FinanceReject(int id)
        {
            var record = db.FleetInsurancePaymentRequest.FirstOrDefault(x => x.FleetInsurancePaymentRequestId == id);
            record.IsBudgetApproved = false;
            record.BudgetApprovedBy = "";
            record.BudgetApprovedTime = DateTime.Now;
            record.Status = "Finance Rejected";
            db.Entry(record).State = EntityState.Modified;
            db.SaveChanges();
            var formattedData = new
            {
                Message = "Rejecte Success",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}