using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.Construction.Controllers.Billing
{
    [AuthorizeRoles(ConstructionConstants.CostingUser)]
    public class ProjectPaymentCertificateController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            var taxRates = db.GLTaxRates;
            var taxWithConcatnatedValue = new List<GLTaxRate>();
            foreach (GLTaxRate taxRate in taxRates)
            {
                var taxName = new GLTaxRate
                {
                    GLTaxRateID = taxRate.GLTaxRateID,
                    TaxName = taxRate.TaxName + "( " + Math.Round(taxRate.Rate, 0) + "% )"
                };
                taxWithConcatnatedValue.Add(taxName);
            }
            ViewData["TaxTypes"] = taxWithConcatnatedValue;

            //var boqs = db.BoQOfExcutedWork.ToList();
            //var listOfBoQNumbers = new List<BoQOfExecutedWork>();
            //foreach(BoQOfExecutedWork boq in boqs)
            //{
            //    var billNumber = new BoQOfExecutedWork
            //    {
            //        BoQOfExecutedWorkId = boq.BoQOfExecutedWorkId,

            //    };
            //}
           
            return View();
        }
        public ActionResult ProjectInformation_Read([DataSourceRequest] DataSourceRequest request)
        {
            var currentUser = User.Identity.Name;
            var assignments = db.ConstructionProjectUserAssignments.Where(assgn => assgn.UserName == currentUser).ToList();
            var ids = new HashSet<int>(assignments.Select(a => a.ProjectInformationId));

            IQueryable<ProjectInformation> projectinformation = 
                db.ProjectInformation.Where(x => x.ProjectStatus != ProjectStatus.ESTIMATION && !x.IsDeleted && ids.Contains(x.ProjectInformationId));
            //IQueryable<ProjectInformation> projectinformation = db.ProjectInformation;
            DataSourceResult result = projectinformation.ToDataSourceResult(request, projectInformation => new
            {
                ProjectInformationId = projectInformation.ProjectInformationId,
                ProjectType = projectInformation.ProjectType,
                Name = projectInformation.Name,
                Code = projectInformation.Code,
                Category = projectInformation.Category,
                BranchId = projectInformation.BranchId,
                ContractAmount = projectInformation.ContractAmount,
                ProjectScope = projectInformation.ProjectScope,
                EmployeeId = projectInformation.EmployeeId,
                Client = projectInformation.Client,
                Consultant = projectInformation.Consultant,
                OrgBranchName = projectInformation.OrgBranchName,
                DateCreated = projectInformation.DateCreated,
                LastModified = projectInformation.LastModified,
                CreatedBy = projectInformation.CreatedBy,
                ModifiedBy = projectInformation.ModifiedBy,
                ProjectStatus = projectInformation.ProjectStatus
            });

            return Json(result);
        }
        public ActionResult Paymentcertificate_Read([DataSourceRequest]DataSourceRequest request, int projectIdRead)
        {
            IQueryable<ProjectPaymentCertificate> certificates = db.PaymentCertificates.Where(c => c.ProjectInformationId == projectIdRead);
            DataSourceResult result = certificates.ToDataSourceResult(request, certificate => new
            {
                ProjectPaymentCertificateId = certificate.ProjectPaymentCertificateId,
                ProjectInformationId = certificate.ProjectInformationId,
                PaymentNumber = certificate.PaymentNumber,
                BillReference = certificate.BillReference,
                GLTaxRateID = certificate.GLTaxRateID,
                ValueOfWorkExecutedToDate = certificate.ValueOfWorkExecutedToDate,
                TaxAmount = certificate.TaxAmount,
                TotalWithVat = certificate.TotalWithVat,
                PreviousPayment = certificate.PreviousPayment,
                Retention = certificate.Retention,
                Penality = certificate.Penality,
                Correction = certificate.Correction,
                AdvanceRepayment = certificate.AdvanceRepayment,
                OtherDeductions = certificate.OtherDeductions,
                MaterialOnSite = certificate.MaterialOnSite,

                NetPayableAmount = certificate.NetPayableAmount,

                RetentionRte = ConsSettingHelper.GetCurrentRetensionRate(certificate.ProjectInformationId),


                OrgBranchName = certificate.OrgBranchName,
                DateCreated = certificate.DateCreated,
                LastModified = certificate.LastModified,
                CreatedBy = certificate.CreatedBy?? " ",
                ModifiedBy = certificate.ModifiedBy?? " ",

                Status = certificate.Status?? " ",
                IsSendForApproval = certificate.IsSendForApproval,
                SendForApprovalBy = certificate.SendForApprovalBy,
                SendForApprovalTime = certificate.SendForApprovalTime,
                IsOnlineApproved = certificate.IsOnlineApproved,
                OnlineApprovedBy = certificate.OnlineApprovedBy,
                OnlineApprovedTime = certificate.OnlineApprovedTime,
                IsOnlineTransferred = certificate.IsOnlineTransferred,
                IsOnlineTransferredBy = certificate.IsOnlineTransferredBy,
                OnlineTransferredTime = certificate.OnlineTransferredTime,
                IsVoid = certificate.IsVoid,
                VoidBy = certificate.VoidBy,
                VoidTime = certificate.VoidTime,
            });

            return Json(result);
        }

        //private bool CheckExistance(ProjectPaymentcertificate model)
        //{
        //    bool certificateExists = false;
        //    var certificate = db.ProjectPaymentcertificates.Where(at => at.certificateName == model.certificateName).FirstOrDefault();
        //    if (certificate != null)
        //    {
        //        certificateExists = true;
        //    }
        //    return certificateExists;
        //}
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PaymentCertificate_Create([DataSourceRequest]DataSourceRequest request, ProjectPaymentCertificate certificate, int projectIdCreate)
        {
            if (ModelState.IsValid)
            {
                #region MyRegion
                //if (CheckExistance(certificate))
                //{
                //    ModelState.AddModelError("", "Value Exists!!");
                //}
                //else
                //{ 
                #endregion
                var entity = new ProjectPaymentCertificate
                {
                    //ProjectPaymentCertificateId = certificate.ProjectPaymentCertificateId,
                    ProjectInformationId = projectIdCreate,
                    PaymentNumber = this.GetPaymentCounter(projectIdCreate),
                    BillReference = certificate.BillReference,
                    GLTaxRateID = certificate.GLTaxRateID,
                    ValueOfWorkExecutedToDate = certificate.ValueOfWorkExecutedToDate,
                    //TaxAmount = (ConsSettingHelper.GetCurrentVatRate() / 100) * (certificate.ValueOfWorkExecutedToDate),
                    TotalWithVat = certificate.TotalWithVat,
                    PreviousPayment = certificate.PreviousPayment,
                    Retention = (ConsSettingHelper.GetCurrentRetensionRate(certificate.ProjectInformationId) / 100) * (certificate.ValueOfWorkExecutedToDate),
                    Penality = certificate.Penality,
                    Correction = certificate.Correction,
                    AdvanceRepayment = certificate.AdvanceRepayment,
                    OtherDeductions = certificate.OtherDeductions,
                    MaterialOnSite = certificate.MaterialOnSite,

                    OrgBranchName = certificate.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = certificate.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = certificate.ModifiedBy,


                    Status = OperationStatuses.New.ToString(),
                    IsSendForApproval = certificate.IsSendForApproval,
                    SendForApprovalBy = certificate.SendForApprovalBy,
                    SendForApprovalTime = certificate.SendForApprovalTime,
                    IsOnlineApproved = certificate.IsOnlineApproved,
                    OnlineApprovedBy = certificate.OnlineApprovedBy,
                    OnlineApprovedTime = certificate.OnlineApprovedTime,
                    IsOnlineTransferred = certificate.IsOnlineTransferred,
                    IsOnlineTransferredBy = certificate.IsOnlineTransferredBy,
                    OnlineTransferredTime = certificate.OnlineTransferredTime,
                    IsVoid = certificate.IsVoid,
                    VoidBy = certificate.VoidBy,
                    VoidTime = certificate.VoidTime,
                };
                entity.TaxAmount = BillingHelper.CalculateTaxAmount(entity.GLTaxRateID, entity.ValueOfWorkExecutedToDate);
                entity.PreviousPayment = this.GetPreviousPayments(entity.ProjectInformationId, entity.PaymentNumber);
                entity.TotalWithVat = entity.ValueOfWorkExecutedToDate + entity.TaxAmount;
                entity.AdvanceRepayment = (ConsSettingHelper.GetCalculatedAdvanceRepaymentRate(projectIdCreate) / 100) * (entity.TotalWithVat);

                var deductiveSum = entity.PreviousPayment + 
                                    entity.Retention + entity.Penality + entity.AdvanceRepayment + 
                                    entity.Correction  + entity.OtherDeductions + entity.MaterialOnSite;
                entity.NetPayableAmount = entity.TotalWithVat - deductiveSum;

                db.PaymentCertificates.Add(entity);
                db.SaveChanges();
                certificate.ProjectPaymentCertificateId = entity.ProjectPaymentCertificateId;
                certificate.PaymentNumber = entity.PaymentNumber;
                certificate.TaxAmount = entity.TaxAmount;
                certificate.TotalWithVat = entity.TotalWithVat;
                certificate.PreviousPayment = entity.PreviousPayment;
                certificate.Retention = entity.Retention;
                certificate.AdvanceRepayment = entity.AdvanceRepayment;
                certificate.NetPayableAmount = entity.NetPayableAmount;
                //}
            }

            return Json(new[] { certificate }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Paymentcertificate_Update([DataSourceRequest]DataSourceRequest request, ProjectPaymentCertificate certificate)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectPaymentCertificate
                {
                    ProjectPaymentCertificateId = certificate.ProjectPaymentCertificateId,
                    ProjectInformationId = certificate.ProjectInformationId,
                    BillReference = certificate.BillReference,
                    PaymentNumber = certificate.PaymentNumber,
                    GLTaxRateID = certificate.GLTaxRateID,
                    ValueOfWorkExecutedToDate = certificate.ValueOfWorkExecutedToDate,
                    //TaxAmount = (ConsSettingHelper.GetCurrentVatRate() /100) *(certificate.ValueOfWorkExecutedToDate),
                    TotalWithVat = certificate.TotalWithVat,
                    PreviousPayment = certificate.PreviousPayment,
                    Retention = (ConsSettingHelper.GetCurrentRetensionRate(certificate.ProjectInformationId) /100) * (certificate.ValueOfWorkExecutedToDate),
                    Penality = certificate.Penality,
                    Correction = certificate.Correction,
                    AdvanceRepayment = certificate.AdvanceRepayment,
                    OtherDeductions = certificate.OtherDeductions,
                    MaterialOnSite = certificate.MaterialOnSite,

                    OrgBranchName = certificate.OrgBranchName,
                    DateCreated = certificate.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = certificate.CreatedBy,
                    ModifiedBy = User.Identity.Name,

                    Status = certificate.Status,
                    IsSendForApproval = certificate.IsSendForApproval,
                    SendForApprovalBy = certificate.SendForApprovalBy,
                    SendForApprovalTime = certificate.SendForApprovalTime,
                    IsOnlineApproved = certificate.IsOnlineApproved,
                    OnlineApprovedBy = certificate.OnlineApprovedBy,
                    OnlineApprovedTime = certificate.OnlineApprovedTime,
                    IsOnlineTransferred = certificate.IsOnlineTransferred,
                    IsOnlineTransferredBy = certificate.IsOnlineTransferredBy,
                    OnlineTransferredTime = certificate.OnlineTransferredTime,
                    IsVoid = certificate.IsVoid,
                    VoidBy = certificate.VoidBy,
                    VoidTime = certificate.VoidTime,
                };
                entity.TaxAmount = BillingHelper.CalculateTaxAmount(entity.GLTaxRateID, entity.ValueOfWorkExecutedToDate);
                entity.PreviousPayment = this.GetPreviousPayments(entity.ProjectInformationId, entity.PaymentNumber);
                entity.TotalWithVat = entity.ValueOfWorkExecutedToDate + entity.TaxAmount;
                
                entity.AdvanceRepayment = (ConsSettingHelper.GetCalculatedAdvanceRepaymentRate(entity.ProjectInformationId) / 100) * (entity.TotalWithVat);

                var deductiveSum = entity.PreviousPayment +
                                                    entity.Retention + entity.Penality + entity.AdvanceRepayment +
                                                    entity.Correction + entity.OtherDeductions + entity.MaterialOnSite;
                entity.NetPayableAmount = entity.TotalWithVat - deductiveSum;

                db.PaymentCertificates.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                certificate.TaxAmount = entity.TaxAmount;
                certificate.TotalWithVat = entity.TotalWithVat;
                certificate.PreviousPayment = entity.PreviousPayment;
                certificate.Retention = entity.Retention;
                certificate.AdvanceRepayment = entity.AdvanceRepayment;
                certificate.NetPayableAmount = entity.NetPayableAmount;
            }

            return Json(new[] { certificate }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Paymentcertificate_Destroy([DataSourceRequest]DataSourceRequest request, ProjectPaymentCertificate certificate)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectPaymentCertificate
                {
                    ProjectPaymentCertificateId = certificate.ProjectPaymentCertificateId,
                    ProjectInformationId = certificate.ProjectInformationId,
                    BillReference = certificate.BillReference,
                    PaymentNumber = certificate.PaymentNumber,
                    GLTaxRateID = certificate.GLTaxRateID,
                    ValueOfWorkExecutedToDate = certificate.ValueOfWorkExecutedToDate,
                    TaxAmount = certificate.TaxAmount,
                    TotalWithVat = certificate.TotalWithVat,
                    PreviousPayment = certificate.PreviousPayment,
                    Retention = certificate.Retention,
                    Penality = certificate.Penality,
                    Correction = certificate.Correction,
                    AdvanceRepayment = certificate.AdvanceRepayment,
                    OtherDeductions = certificate.OtherDeductions,
                    MaterialOnSite = certificate.MaterialOnSite,

                    NetPayableAmount = certificate.NetPayableAmount,

                    OrgBranchName = certificate.OrgBranchName,
                    DateCreated = certificate.DateCreated,
                    LastModified = certificate.LastModified,
                    CreatedBy = certificate.CreatedBy,
                    ModifiedBy = certificate.ModifiedBy,

                    Status = certificate.Status,
                    IsSendForApproval = certificate.IsSendForApproval,
                    SendForApprovalBy = certificate.SendForApprovalBy,
                    SendForApprovalTime = certificate.SendForApprovalTime,
                    IsOnlineApproved = certificate.IsOnlineApproved,
                    OnlineApprovedBy = certificate.OnlineApprovedBy,
                    OnlineApprovedTime = certificate.OnlineApprovedTime,
                    IsOnlineTransferred = certificate.IsOnlineTransferred,
                    IsOnlineTransferredBy = certificate.IsOnlineTransferredBy,
                    OnlineTransferredTime = certificate.OnlineTransferredTime,
                    IsVoid = certificate.IsVoid,
                    VoidBy = certificate.VoidBy,
                    VoidTime = certificate.VoidTime,
                };

                db.PaymentCertificates.Attach(entity);
                db.PaymentCertificates.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { certificate }.ToDataSourceResult(request, ModelState));
        }
        private int GetPaymentCounter(int projectId) {
            int counter = 0;
            var availablePayments = db.PaymentCertificates.Where(pc => pc.ProjectInformationId == projectId).ToList();
            // this is the first payment of the project - excluding advance payment
            if (!availablePayments.Any())
            {
                counter = 1;
            }
            else
            {
                counter = availablePayments.Max(p => p.PaymentNumber) + 1 ;
            }
            return counter;
        }
        private decimal GetPreviousPayments(int projectId, int paymentNumber) {
            var totalPreviosPayment = new decimal();

            //var advancedPayment = db.ProjectAdvancePayments.Where(adp => adp.ProjectInformationId == projectId).FirstOrDefault();
            var priorPayments = db.PaymentCertificates.Where(payment => payment.ProjectInformationId == projectId &&
                                payment.PaymentNumber < paymentNumber).ToList();
            if (priorPayments.Any())
            {
                totalPreviosPayment = priorPayments.Sum(p => p.NetPayableAmount);
            }
            return totalPreviosPayment;
        }
        public decimal GetAmountOfExcutedWorkToDate(string reference) {
            var amnt = new decimal();
            var currentBoQ = db.BoQOfExcutedWork.FirstOrDefault(b => b.BillReference == reference);
            if(currentBoQ != null)
            {
                var contents = db.BoQOfExcutedWorkContent.Where(bc => bc.BoQOfExecutedWorkId == currentBoQ.BoQOfExecutedWorkId).ToList();
                if (contents.Any())
                {
                    amnt = contents.Sum(c => c.AmountToDatePayment);
                }
            }
            return amnt;
        }
        public JsonResult SendForApproval(int id)
        {
            var record = db.PaymentCertificates.Find(id);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved)
            {
                #region validate price and tax

                //foreach (var item in record.WorkInProgressItems)
                //{
                if (record.ValueOfWorkExecutedToDate <= 0 || record.NetPayableAmount <= 0)
                {
                    var formattedData3 = new
                    {
                        Message = "Please fill Correct Amounts in the Payment Cerificate",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData3, JsonRequestBehavior.AllowGet);
                }
                //}
                #endregion

                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;

                db.PaymentCertificates.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();
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

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}