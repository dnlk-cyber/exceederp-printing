using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using ExceedERP.Reporting.Construction.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Construction.DataSource
{
    [DataObject]
    public class PaymentCertificateOds
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        //decimal currentVatRate = ConsSettingHelper.GetCurrentVatRate();
        decimal currentVatRate = new decimal();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<CertificateViewModel> GetProjectPaymentCertificate(string projectId, string paymentId)
        {
            List<CertificateViewModel> list = new List<CertificateViewModel>();

            int id = Convert.ToInt32(projectId);
            var project = db.ProjectInformation.Where(p => p.ProjectInformationId == id).FirstOrDefault();

            if (project != null)
            {
                int currentPaymentId = Convert.ToInt32(paymentId);
                // project information
                var currentProjectLocation = ProjectInformationHelper.GetProjectLocation(project.BranchId);
                // main contract information
                var mainContract = ContractMgmtHelper.GetProjectMainContract(id);
                var variationOrders = db.ContractVariationOrder.Where(vo => vo.ProjectContractId == mainContract.ProjectContractId).Distinct().ToList();
                List<MainContractVariationOrderVM> variationOrderList = new List<MainContractVariationOrderVM>();
                if (variationOrders.Any())
                {
                    foreach (ContractVariationOrder vOrder in variationOrders)
                    {
                        var variationOrderRecord = new MainContractVariationOrderVM
                        {
                            VariationOrderNo = vOrder.VariationOrderNumber,
                            Amount = (decimal)vOrder.ChangeCost
                        };
                        variationOrderList.Add(variationOrderRecord);
                    }
                }
                var projectAdvancePayment = db.ProjectAdvancePayments.Where(pap => pap.ProjectInformationId == id).FirstOrDefault();
                var availablePaymentCertificates = db.PaymentCertificates.Where(pc => pc.ProjectInformationId == id).ToList();
                var paymentOfCurrentNumber = db.PaymentCertificates.Where(p => p.ProjectInformationId == project.ProjectInformationId && p.ProjectPaymentCertificateId == currentPaymentId).ToList();
                if (paymentOfCurrentNumber.Any())
                {
                    availablePaymentCertificates = paymentOfCurrentNumber;
                }
                
                if (availablePaymentCertificates.Any())
                {
                    
                    foreach (ProjectPaymentCertificate certificate in availablePaymentCertificates)
                    {
                        var priorPayments = db.PaymentCertificates.Where(payment => payment.ProjectInformationId == id &&
                                payment.PaymentNumber < certificate.PaymentNumber).ToList();

                        var certificateRecord = new CertificateViewModel
                        {
                            ProjectName = project.Name,
                            Location = currentProjectLocation,
                            Consultant = project.Consultant,
                            Client = project.Client,

                            MainContract = mainContract.ContractAmount,
                            VariationOrderList = variationOrderList,
                            Sum = variationOrderList.Sum(v => v.Amount) + mainContract.ContractAmount,

                            PaymentNumber = certificate.PaymentNumber,
                            ExecutedAmount = certificate.ValueOfWorkExecutedToDate,
                            VatOfCurrentAmount = certificate.TaxAmount,
                            TotalWithVatofCurrentAmount = certificate.TotalWithVat,
                            PreviousPayment = certificate.PreviousPayment,
                            Retention = certificate.Retention,
                            Penality = certificate.Penality,
                            AdvanceRepayment = certificate.AdvanceRepayment,
                            Correction = certificate.Correction,
                            OtherDeductiveCosts = certificate.OtherDeductions,
                            MaterialOnSite = certificate.MaterialOnSite,

                            SumOfDeductiveCosts = certificate.PreviousPayment + certificate.Retention + 
                                        certificate.Penality + certificate.AdvanceRepayment + 
                                        certificate.Correction + certificate.OtherDeductions + certificate.MaterialOnSite,
                            NetAmount = certificate.NetPayableAmount,
                        };
                        certificateRecord.VatOfContractAmount = BillingHelper.CalculateTaxAmount(certificate.GLTaxRateID, certificateRecord.Sum);
                        certificateRecord.TotalWithVatOfContractAmount = certificateRecord.Sum + certificateRecord.VatOfContractAmount;
                        if (projectAdvancePayment != null)
                        {
                            var adPaymentToAdd = new PreviosPaymentsViewModel
                            {
                                PaymentNumber = "Advance",
                                Amount = projectAdvancePayment.ApprovedAmount
                            };
                            certificateRecord.PreviosPaymentList.Add(adPaymentToAdd);
                        }
                        if (priorPayments.Any())
                        {
                            foreach (ProjectPaymentCertificate prevPayment in priorPayments)
                            {
                                var previosPaymentRecord = new PreviosPaymentsViewModel
                                {
                                    PaymentNumber = prevPayment.PaymentNumber + GenericHelper.ToOrdinal(prevPayment.PaymentNumber) + " pay",
                                    Amount = prevPayment.NetPayableAmount
                                };
                                certificateRecord.PreviosPaymentList.Add(previosPaymentRecord);
                            }
                        }
                        list.Add(certificateRecord);
                    }
                }
            }
            return list;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public ProjectHeaderVM GetProjectHeader(string projectId)
        {
            int id = Convert.ToInt32(projectId);
            var item = db.ProjectInformation.Find(id);
            ProjectHeaderVM headerDs = new ProjectHeaderVM();
            var organization = db.Companies.FirstOrDefault();
            if (item != null)
            {
                headerDs.ProjectName = item.Name;

            }

            if (organization != null)
            {
                headerDs.Name = organization.TradeName;
                headerDs.AmharicName = organization.AmharicName;
                var address =
                    db.Addresses.Where(
                        a =>
                            a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType.ToLower() == "company").ToList();
                if (address.Count > 0)
                {
                    foreach (var a in address)
                    {

                        if (headerDs.Address == null)
                        {
                            headerDs.Address = a.Category + ":- " + a.Value;
                        }
                        else
                        {
                            headerDs.Address += ", " + a.Category + "-" + a.Value;
                        }

                    }

                }
                var logo =
                    db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType == AttachmentType.LOGO);
                if (logo != null)
                {
                    headerDs.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }

            return headerDs;
        }
    }
    [DataObject]
    public class PaymentCertificateNumberOds : List<PaymentNumberViewModel>
    {
        private ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PaymentNumberViewModel> GetAllPaymentNumbers(string projectId)
        {
            var list = new List<PaymentNumberViewModel>();
            int id = Convert.ToInt32(projectId);

            var project = db.ProjectInformation.Where(p => p.ProjectInformationId == id).FirstOrDefault();

            if (project != null)
            {
                var payments = db.PaymentCertificates.Where(p => p.ProjectInformationId == project.ProjectInformationId).ToList();
                if (payments.Any())
                {
                    foreach (ProjectPaymentCertificate  certificate in payments)
                    {
                        var record = new PaymentNumberViewModel
                        {
                            ConcatnatedNumber  = "Payment No. " + certificate.PaymentNumber,
                            ProjectPaymentCertificateId = certificate.ProjectPaymentCertificateId
                        };
                        list.Add(record);
                    }
                }
            }
            return list;
        }
    }
}
