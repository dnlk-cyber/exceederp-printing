using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.AP.PaymentRequest;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Maintenance.Controllers.WorkFlows
{
    public class ExternalMaintenancePaymentRequestController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: Maintenance/ExternalMaintenancePaymentRequest
        public ActionResult Index()
        {
            ViewData["MaintenanceCenters"] = db.MaintenanceCenter.Select(x => new
            {
                Value = x.MaintenanceCenterID,
                Text = x.Name
            });
            ViewData["ServiceCenterJobs"] = db.ServiceCenterJob.Select(x => new
            {
                Value = x.ServiceCenterJobId,
                Text = x.Num
            }).ToList();
            ViewData["Name"] = db.EquipmentName.Select(s => new
            {
                Value = s.EquipmentNameID,
                Text = s.Name
            }).ToList();
            ViewData["Fleet"] = db.Fleets.Select(s => new
            {
                Value = s.FleetsID,
                Text = s.PlateNumber + " " + s.SerialNo
            }).ToList();
            ViewData["Branch"] = db.Branch.Select(s => new
            {
                Value = s.BranchId,
                Text = s.Name
            }).ToList();


            ViewData["GlTaxRate"] = db.GLTaxRates.Select(s => new
            {
                Value = s.GLTaxRateID,
                Text = s.TaxName
            }).ToList();
            ViewData["MaintenanceCenter"] = db.MaintenanceCenter.Select(s => new
            {
                Value = s.MaintenanceCenterID,
                Text = s.Name 
            }).ToList();


            ViewData["ServiceCenter"] = db.ServiceCenters.Select(s => new
            {
                Value = s.ServiceCenterId,
                Text = s.Name
            }).ToList();
            ViewData["BusinessUnit"] = db.BusinessUnits.Select(s => new
            {
                Value = s.BusinessUnitId,
                Text = s.Name
            }).ToList();
            return View();
        }

        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            var model = db.ExternalMaintenancePaymentRequests;

            return Json(model.OrderBy(x => x.ExternalMaintenancePaymenntrequestId).ToDataSourceResult(request, ModelState));
        }
        public ActionResult Read_Detail([DataSourceRequest] DataSourceRequest request, int id)
        {
            var model = db.ExternalMaintenancePaymentRequestDetails.Where(x => x.ExternalMaintenancePaymenntrequestId == id);

            return Json(model.ToDataSourceResult(request, ModelState));
        }
         public ActionResult Read_Details([DataSourceRequest] DataSourceRequest request, int id)
        {
            var model = db.ExternalMaintenancePaymentRequestDetails.Where(x => x.ExternalPaymentRequestJobId == id);

            return Json(model.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Create([DataSourceRequest] DataSourceRequest request, ExternalMaintenancePaymentRequest MaintenanceRequests)
        {
            var fetchMaintenanceCenter = db.MaintenanceCenter.FirstOrDefault(x => x.MaintenanceCenterID == MaintenanceRequests.MaintenanceCenterId);
            if(fetchMaintenanceCenter!=null)
            {
                MaintenanceRequests.BranchId = fetchMaintenanceCenter.BranchId;
                MaintenanceRequests.BusinessUnitId = fetchMaintenanceCenter.BusinessUnitId;
            }
            int AllCounts = 0;
            var fetchAllPaymentRequests = db.ExternalMaintenancePaymentRequests.ToList() ;
            if (fetchAllPaymentRequests.Any())
            {
                AllCounts = fetchAllPaymentRequests.Count();
            }
            MaintenanceRequests.JobNum = AllCounts + 1;
            if (ModelState.IsValid)
            {
                var MRequest = new ExternalMaintenancePaymentRequest
                {
                    TotalAmount = 0,
                    AmountDue = 0,
                    ForwardedTo = User.Identity.Name,
                    ForwardedFrom=User.Identity.Name,
                    MaintenanceCenterId = MaintenanceRequests.MaintenanceCenterId,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    JobNum = MaintenanceRequests.JobNum,
                    //Description = MaintenanceRequest.Description,
                };
                db.ExternalMaintenancePaymentRequests.Add(MRequest);
                db.SaveChanges();
            }

            return Json(new[] { MaintenanceRequests }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Update([DataSourceRequest] DataSourceRequest request, ExternalMaintenancePaymentRequest model)
        {
            if (ModelState.IsValid)
            {

                model.ModifiedBy = User.Identity.Name;
                model.LastModified = DateTime.Today;
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Delete([DataSourceRequest] DataSourceRequest request, ExternalMaintenancePaymentRequest model)
        {
            if (ModelState.IsValid)
            {

                var entity = db.ExternalMaintenancePaymentRequests.Find(model.ExternalMaintenancePaymenntrequestId);
                var detail = db.ExternalMaintenancePaymentRequestDetails.Where(x => x.ExternalMaintenancePaymenntrequestId == entity.ExternalMaintenancePaymenntrequestId).ToList();
                if (detail.Any())
                {
                    db.ExternalMaintenancePaymentRequestDetails.RemoveRange(detail);
                    db.SaveChanges();
                }
                db.ExternalMaintenancePaymentRequests.Remove(entity);
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        //public JsonResult SendForApproval(int id)
        //{
        //    var entity = db.ExternalMaintenancePaymentRequests.FirstOrDefault(x => x.ExternalMaintenancePaymenntrequestId == id);
        //    if (entity != null)
        //    {
        //        entity.IsSendForApproval = true;
        //        entity.IsOnlineTransferredBy = User.Identity.Name;
        //        entity.OnlineTransferredTime = DateTime.Today;
        //        db.Entry(entity).State = EntityState.Modified;
        //        db.SaveChanges();
        //        var returnMessage = new
        //        {
        //            Message = "Successfully Sent!",

        //        };
        //        return Json(returnMessage.Message, JsonRequestBehavior.AllowGet);

        //    }
        //    var jsonData = new
        //    {
        //        Message = "Error Sending!",

        //    };
        //    return Json(jsonData.Message, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult Approve(int id)
        //{
        //    object response = null;
        //    var entity = db.ExternalMaintenancePaymentRequests.Find(id);
        //    if (entity != null)
        //    {
               
        //        entity.IsOnlineApproved = true;
        //        entity.OnlineApprovedBy = User.Identity.Name;
        //        entity.OnlineApprovedTime = DateTime.Today;


        //        db.ExternalMaintenancePaymentRequests.Attach(entity);
        //        db.Entry(entity).State = EntityState.Modified;
        //        db.SaveChanges();
        //        GenerateExternalPaymentRequest(entity.JobNum);
        //        response = new { Success = true, Message = "Successfully Approved" };
        //    }
        //    else
        //    {
        //        response = new { Success = false, Message = "Something is wrong!" };
        //    }
        //    return Json(response);
        //}
        public JsonResult SendForApproval(int id)
        {
            var record = db.ExternalMaintenancePaymentRequests.Find(id);          

            if (record.TotalAmount <= 0)
            {
                var formattedData = new
                {
                    Message = "Net payable should be more than 0.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                db.ExternalMaintenancePaymentRequests.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();
                #region approval workflow
                bool isNewRecord = true;
                var validations = db.ExternalMaintenancePaymentRequestValidations.Where(x => x.Status == ApprovalStatuses.Approve).AsQueryable();
                if (validations.Any())
                    isNewRecord = false;
                decimal docamount = record.TotalAmount;
                int docid = record.ExternalMaintenancePaymenntrequestId;
                int step = record.CurrentApprovalStep != null ? (int)record.CurrentApprovalStep : 0;
                DocApprovalWorkflowHelper.ForwardToApprover(docid, docamount, "", record.ForwardedTo, Core.Domain.Common.DocumentApproval.ApprovalDocumentType.ExternalMaintenanceRequestPayment, isNewRecord, step);
                #endregion
                var formattedData = new
                {
                     Message = "Successfully Sent.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            else if (record == null)
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            else if (record.IsSendForApproval)
            {
                var formattedData = new
                {
                    Message = "The record is already sent for approval you cannot send again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            else if (record.IsOnlineApproved)
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


        #region ExternalMaintenancePaymentRequest
        public void GenerateExternalPaymentRequest(int num)
        {
            var jobOrder = db.JobOrder.FirstOrDefault(x => x.Num == num);
            if (jobOrder != null)
            {

                var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == jobOrder.RequesterId);
                if (placement != null)
                {
                    var serviceCenterJob = db.ServiceCenterJob.FirstOrDefault(x => x.Num == num);
                    if (serviceCenterJob != null)
                    {
                        var request = new PaymentRequest
                        {
                            SupplierId = "0",
                            IsNewSupplier = false,
                            Supplier = " ",
                            Purchaser = 0,
                            ReasonOfPayment = $"Vehicle Plate No {jobOrder.LicensePlate} Repair",
                            RequestDate = DateTime.Now.Date,
                            RequestDateAm = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year),
                            InvoiceNo = "0",
                            PurchaseRequestNo = "0",
                            PONo = "0",
                            GRN = "0",
                            BranchId = jobOrder.BranchId,
                            BusinessUnitId = jobOrder.BusinessUnitId,
                            OrgStuructureId = placement.OrgStructureId,
                            EmployeeId = jobOrder.RequesterId,
                            Paid = false,
                           FromBranchId = jobOrder.BranchId,
                           // BranchId = jobOrder.BranchId,
                            FromBusinessUnitId = jobOrder.BusinessUnitId,
                            //BusinessUnitId = jobOrder.BusinessUnitId,
                            CreatedAtBranchId = jobOrder.BranchId,
                            CreatedAtBusinessUnitId = jobOrder.BusinessUnitId,
                            CreatedAtOrgStructureId = placement.OrgStructureId,
                           // OrgStuructureId = placement.OrgStructureId,
                            CreatedBy = User.Identity.Name,
                            
                            DateCreated = DateTime.Now.Date,
                            ModifiedBy = User.Identity.Name,
                            LastModified = DateTime.Now.Date,
                            CurrentApprovalStep = 0,
                            IsDepartmentOnlineApproved = true,
                            DepartmentOnlineApprovedBy = User.Identity.Name,
                            DepartmentOnlineApprovedTime = DateTime.Now,
                        };
                        db.PaymentRequests.Add(request);
                        db.SaveChanges();
                        var payments = db.ExternalServiceCenterPayments.Where(x => x.ServiceCenterJobId == serviceCenterJob.ServiceCenterJobId);
                        if (payments.Any())
                        {
                            foreach (var payment in payments)
                            {
                                var requestDetail = new PaymentRequestLine
                                {
                                    PaymentRequestId = request.PaymentRequestId,
                                    Amount = payment.SubTotal,
                                    TaxAmount = 0,
                                    TotalAmount = payment.Total,
                                    ItemName = "0",
                                    GLTaxRateID = payment.GlTaxRateId,
                                    Description = $"External Maintenance: {payment.CostType}",
                                    Remark = "",
                                    CreatedBy = User.Identity.Name,
                                    DateCreated = DateTime.Now.Date,
                                    ModifiedBy = User.Identity.Name,
                                    LastModified = DateTime.Now.Date,
                                };
                                db.PaymentRequestLines.Add(requestDetail);
                            }

                            db.SaveChanges();
                        }
                    }
                }

            }

        }
        #endregion
    }
}