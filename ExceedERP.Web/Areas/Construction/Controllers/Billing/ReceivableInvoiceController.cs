using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Helpers.Finance;

namespace ExceedERP.Web.Areas.Construction.Controllers.Billing
{
    public class ReceivableInvoiceController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public static decimal repayableAmnt { get; set; }
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
            ViewData["Branchs"] = db.Branch.Select(b => new { b.Name, b.BranchId }).ToList();
            #region concatnated work items
            var workItems = db.WorkItemPerProject.OrderBy(x => x.ItemName).ToList();
            //var workItemsWithConcatnatedValue = new List<WorkItemPerProject>();
            //var workItem = new WorkItemPerProject();
            //foreach (var item in workItems)
            //{
            //    var tempWorkItem = new WorkItemPerProject();
            //    if (item.BlockId != null)
            //    {
            //        tempWorkItem = item;
            //        var workItemBlock = db.Blocks.Find(item.BlockId);
            //        if (workItemBlock != null)
            //        {
            //            tempWorkItem.ItemName = item.ItemName + " of " + workItemBlock.Name;
            //        }
            //    }
            //    workItemsWithConcatnatedValue.Add(tempWorkItem);
            //}
            List<PaymentCertificateViewModel> list = new List<PaymentCertificateViewModel>();
            var paymentCeritificates = db.PaymentCertificates.ToList();
            if (paymentCeritificates.Any())
            {
                foreach (var certificate in paymentCeritificates)
                {
                    var record = new PaymentCertificateViewModel
                    {
                        ProjectPaymentCertificateId = certificate.ProjectPaymentCertificateId,
                        PaymentNumber = GenericHelper.ToOrdinal(certificate.PaymentNumber) + " pay"
                    };
                    list.Add(record);
                }
            }
            ViewData["PaymentCertificates"] = list;
            #endregion
            ViewData["WorkItems"] = workItems;

            ViewData["PaymentTerms"] = db.PaymentTerms.ToList();

            //this.UpdateInvoices();
            return View();
        }
        public ActionResult ProjectInformation_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProjectInformation> projectinformation = db.ProjectInformation.Where(x => x.ProjectStatus == ProjectStatus.STARTED || x.ProjectStatus == ProjectStatus.APPROVED).AsQueryable();
            DataSourceResult result = projectinformation.ToDataSourceResult(request, projectInformation => new {
                projectInformation.ProjectInformationId,
                projectInformation.Name,
                projectInformation.Code,
                projectInformation.Category,
                projectInformation.ProjectType,
                projectInformation.BranchId,
                projectInformation.ContractAmount,
                projectInformation.ProjectScope,
                projectInformation.EmployeeId,
                projectInformation.Client,
                projectInformation.Consultant,
                projectInformation.ProjectStatus,

                projectInformation.OrgBranchName,
                projectInformation.DateCreated,
                projectInformation.LastModified,
                projectInformation.CreatedBy,
                projectInformation.ModifiedBy
            });

            return Json(result);
        }
        public ActionResult Invoices_Read([DataSourceRequest]DataSourceRequest request, int projectId)
        {
            IQueryable<ConsReceivableInvoice> invoices = db.ConsReceivableInvoices.Where(iv => iv.ProjectInformationId == projectId);
            DataSourceResult result = invoices.ToDataSourceResult(request, invoice => new
            {
                invoice.ConsReceivableInvoiceId,
                invoice.ProjectInformationId,
                invoice.Source,
                invoice.InvoiceNumber,
                invoice.Client,
                invoice.ConstructionTransactionClass,
                invoice.Date,
                invoice.PaymentTerm,
                invoice.DueDate,
                invoice.Reference,
                invoice.ConstructionInvoiceStatus,
                invoice.OperationStatus,
                invoice.WorkInProgressId,
                invoice.IsInvoiceFromWorkInProgress,
                invoice.TotalAmount,
                invoice.CollectedAmount,

                invoice.OrgBranchName,
                invoice.DateCreated,
                invoice.LastModified,
                invoice.CreatedBy,
                invoice.ModifiedBy,

                Status = invoice.Status,
                IsSendForApproval = invoice.IsSendForApproval,
                SendForApprovalBy = invoice.SendForApprovalBy,
                SendForApprovalTime = invoice.SendForApprovalTime,
                IsOnlineApproved = invoice.IsOnlineApproved,
                OnlineApprovedBy = invoice.OnlineApprovedBy,
                OnlineApprovedTime = invoice.OnlineApprovedTime,
                IsOnlineTransferred = invoice.IsOnlineTransferred,
                IsOnlineTransferredBy = invoice.IsOnlineTransferredBy,
                OnlineTransferredTime = invoice.OnlineTransferredTime,
                IsVoid = invoice.IsVoid,
                VoidBy = invoice.VoidBy,
                VoidTime = invoice.VoidTime,
            });

            return Json(result);
        }

        
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Invoices_Create([DataSourceRequest]DataSourceRequest request, ConsReceivableInvoice invoice, int projectIdCreate)
        {
            if (ModelState.IsValid)
            {
                
                var entity = new ConsReceivableInvoice
                {
                    //ReceivableInvoiceId = invoice.ReceivableInvoiceId,
                    ProjectInformationId = projectIdCreate,
                    Source = invoice.Source,
                    InvoiceNumber = invoice.InvoiceNumber,
                    Client = ProjectInformationHelper.GetProjectClient(projectIdCreate),
                    ConstructionTransactionClass = invoice.ConstructionTransactionClass,
                    Date = invoice.Date,
                    PaymentTerm = invoice.PaymentTerm,
                    DueDate = invoice.DueDate,
                    Reference = invoice.Reference,
                    ConstructionInvoiceStatus = invoice.ConstructionInvoiceStatus,
                    OperationStatus = invoice.OperationStatus,
                    WorkInProgressId = invoice.WorkInProgressId,
                    IsInvoiceFromWorkInProgress = invoice.IsInvoiceFromWorkInProgress,
                    TotalAmount = invoice.TotalAmount,
                    CollectedAmount = invoice.CollectedAmount,

                    OrgBranchName = invoice.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = invoice.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = invoice.ModifiedBy,

                    Status = OperationStatuses.New.ToString(),
                    IsSendForApproval = invoice.IsSendForApproval,
                    SendForApprovalBy = invoice.SendForApprovalBy,
                    SendForApprovalTime = invoice.SendForApprovalTime,
                    IsOnlineApproved = invoice.IsOnlineApproved,
                    OnlineApprovedBy = invoice.OnlineApprovedBy,
                    OnlineApprovedTime = invoice.OnlineApprovedTime,
                    IsOnlineTransferred = invoice.IsOnlineTransferred,
                    IsOnlineTransferredBy = invoice.IsOnlineTransferredBy,
                    OnlineTransferredTime = invoice.OnlineTransferredTime,
                    IsVoid = invoice.IsVoid,
                    VoidBy = invoice.VoidBy,
                    VoidTime = invoice.VoidTime
                };
                entity.DueDate = this.CalculateInvoiceDueDate(entity);
                if (!string.IsNullOrEmpty(entity.Reference))
                {
                    entity.IsInvoiceFromWorkInProgress = true;
                }
                db.ConsReceivableInvoices.Add(entity);
                db.SaveChanges();

                invoice.ConsReceivableInvoiceId = entity.ConsReceivableInvoiceId;
                invoice.InvoiceNumber = entity.InvoiceNumber;

                UpdateInvoiceNumber(entity);

                if (entity.IsInvoiceFromWorkInProgress)
                {
                    //this.CopyInvoiceLineFromWorkInProgress(entity.Reference, entity.ConsReceivableInvoiceId);
                }
                //}
            }

            return Json(new[] { invoice }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Invoices_Update([DataSourceRequest]DataSourceRequest request, ConsReceivableInvoice invoice)
        {
            if (ModelState.IsValid)
            {
                var entity = new ConsReceivableInvoice
                {

                    ConsReceivableInvoiceId = invoice.ConsReceivableInvoiceId,
                    ProjectInformationId = invoice.ProjectInformationId,
                    Source = invoice.Source,
                    InvoiceNumber = invoice.InvoiceNumber,
                    Client = ProjectInformationHelper.GetProjectClient(invoice.ProjectInformationId),
                    ConstructionTransactionClass = invoice.ConstructionTransactionClass,
                    Date = invoice.Date,
                    PaymentTerm = invoice.PaymentTerm,
                    DueDate = invoice.DueDate,
                    Reference = invoice.Reference,
                    ConstructionInvoiceStatus = invoice.ConstructionInvoiceStatus,
                    OperationStatus = invoice.OperationStatus,
                    WorkInProgressId = invoice.WorkInProgressId,
                    IsInvoiceFromWorkInProgress = invoice.IsInvoiceFromWorkInProgress,
                    TotalAmount = invoice.TotalAmount,
                    CollectedAmount = invoice.CollectedAmount,

                    OrgBranchName = invoice.OrgBranchName,
                    DateCreated = invoice.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = invoice.CreatedBy,
                    ModifiedBy = User.Identity.Name,

                    Status = invoice.Status,
                    IsSendForApproval = invoice.IsSendForApproval,
                    SendForApprovalBy = invoice.SendForApprovalBy,
                    SendForApprovalTime = invoice.SendForApprovalTime,
                    IsOnlineApproved = invoice.IsOnlineApproved,
                    OnlineApprovedBy = invoice.OnlineApprovedBy,
                    OnlineApprovedTime = invoice.OnlineApprovedTime,
                    IsOnlineTransferred = invoice.IsOnlineTransferred,
                    IsOnlineTransferredBy = invoice.IsOnlineTransferredBy,
                    OnlineTransferredTime = invoice.OnlineTransferredTime,
                    IsVoid = invoice.IsVoid,
                    VoidBy = invoice.VoidBy,
                    VoidTime = invoice.VoidTime,
                };
                entity.DueDate = this.CalculateInvoiceDueDate(entity);

                if (!string.IsNullOrEmpty(entity.Reference))
                {
                    entity.IsInvoiceFromWorkInProgress = true;
                }

                db.ConsReceivableInvoices.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

               
            }

            return Json(new[] { invoice }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Invoices_Destroy([DataSourceRequest]DataSourceRequest request, ConsReceivableInvoice invoice)
        {
            if (ModelState.IsValid)
            {
                var entity = new ConsReceivableInvoice
                {
                    ConsReceivableInvoiceId = invoice.ConsReceivableInvoiceId,
                    ProjectInformationId = invoice.ProjectInformationId,
                    Source = invoice.Source,
                    InvoiceNumber = invoice.InvoiceNumber,
                    Client = invoice.Client,
                    ConstructionTransactionClass = invoice.ConstructionTransactionClass,
                    Date = invoice.Date,
                    PaymentTerm = invoice.PaymentTerm,
                    DueDate = invoice.DueDate,
                    Reference = invoice.Reference,
                    ConstructionInvoiceStatus = invoice.ConstructionInvoiceStatus,
                    OperationStatus = invoice.OperationStatus,
                    WorkInProgressId = invoice.WorkInProgressId,
                    IsInvoiceFromWorkInProgress = invoice.IsInvoiceFromWorkInProgress,
                    TotalAmount = invoice.TotalAmount,
                    CollectedAmount = invoice.CollectedAmount,

                    OrgBranchName = invoice.OrgBranchName,
                    DateCreated = invoice.DateCreated,
                    LastModified = invoice.LastModified,
                    CreatedBy = invoice.CreatedBy,
                    ModifiedBy = invoice.ModifiedBy,

                    Status = invoice.Status,
                    IsSendForApproval = invoice.IsSendForApproval,
                    SendForApprovalBy = invoice.SendForApprovalBy,
                    SendForApprovalTime = invoice.SendForApprovalTime,
                    IsOnlineApproved = invoice.IsOnlineApproved,
                    OnlineApprovedBy = invoice.OnlineApprovedBy,
                    OnlineApprovedTime = invoice.OnlineApprovedTime,
                    IsOnlineTransferred = invoice.IsOnlineTransferred,
                    IsOnlineTransferredBy = invoice.IsOnlineTransferredBy,
                    OnlineTransferredTime = invoice.OnlineTransferredTime,
                    IsVoid = invoice.IsVoid,
                    VoidBy = invoice.VoidBy,
                    VoidTime = invoice.VoidTime,
                };

                db.ConsReceivableInvoices.Attach(entity);
                db.ConsReceivableInvoices.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { invoice }.ToDataSourceResult(request, ModelState));
        }

        private void UpdateInvoiceNumber(ConsReceivableInvoice invoice)
        {
            invoice.InvoiceNumber = invoice.ConsReceivableInvoiceId.ToString("D10");

            db.ConsReceivableInvoices.Attach(invoice);
            db.Entry(invoice).State = EntityState.Modified;
            db.SaveChanges();
        }

        // 
        //  copy the work in progress detail to invoice detail
        private void CopyInvoiceLineFromWorkInProgress(string reference, int invoiceId)
        {
            var currentWIP = db.WorkInProgress.FirstOrDefault(wip => wip.WorkInProgressNumber == reference);
            if (currentWIP != null)
            {
                var wipLines = db.WorkInProgressDetails.Where(d => d.WorkInProgressId == currentWIP.WorkInProgressId)
                    .ToList();
                if (wipLines.Any())
                {
                    foreach (WorkInProgressDetail wipLine in wipLines)
                    {
                        var invoiceDetail = new ConsReceivableInvoiceDetail
                        {
                            ConsReceivableInvoiceId = invoiceId,
                            Service = wipLine.Service,
                            WorkItemPerProjectId = wipLine.WorkItemPerProjectId,
                            Description = wipLine.Description,
                            AccountCode = wipLine.AccountCode,
                            Quantity = wipLine.Quantity,
                            UnitPrice = wipLine.UnitPrice,
                            SubTotal = wipLine.SubTotal,
                            GLTaxRateID = wipLine.GLTaxRateID,
                            TaxAmount = wipLine.TaxAmount,
                            TotalAmount = wipLine.TotalAmount,

                            OrgBranchName = wipLine.OrgBranchName,
                            DateCreated = wipLine.DateCreated,
                            LastModified = wipLine.LastModified,
                            CreatedBy = wipLine.CreatedBy,
                            ModifiedBy = wipLine.ModifiedBy
                        };
                        invoiceDetail.NetAmount = wipLine.TotalAmount;

                        db.ConsReceivableInvoiceDetails.Add(invoiceDetail);
                        db.SaveChanges();
                    }

                    //update the invoice header containg these items
                    var currentInvoice = db.ConsReceivableInvoices.Find(invoiceId);
                    if(currentInvoice != null)
                    {
                        currentInvoice.TotalAmount = currentInvoice.InvoiceItems.Sum(itm => itm.NetAmount);

                        db.ConsReceivableInvoices.Attach(currentInvoice);
                        db.Entry(currentInvoice).State = EntityState.Modified;
                        db.SaveChanges();

                    }
                }
            }

        }
        //[AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        public JsonResult SendForApproval(int id)
        {
            var record = db.ConsReceivableInvoices.Find(id);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved && record.InvoiceItems.Count > 0)
            {
                #region validate price and tax

                foreach (var item in record.InvoiceItems)
                {
                    if (item.UnitPrice <= 0 || item.GLTaxRateID == 0)
                    {
                        var formattedData3 = new
                        {
                            Message = "Please fill unit price and tax information for all items.",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData3, JsonRequestBehavior.AllowGet);
                    }
                }
                #endregion

                record.OperationStatus = OperationStatuses.WaitingApproval;
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                db.ConsReceivableInvoices.Attach(record);
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

            if (record.InvoiceItems.Count == 0)
            {
                var formattedData = new
                {
                    Message = "Please add line items first.",
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

        private DateTime CalculateInvoiceDueDate(ConsReceivableInvoice model)
        {
            return model.Date.AddDays(BillingHelper.GetPaymentTermDays(model.PaymentTerm));
        }

        private void UpdateInvoices()
        {
            var invoices = db.ConsReceivableInvoices.ToList();
            if (invoices.Any())
            {
                foreach (ConsReceivableInvoice invoice in invoices)
                {
                    if (invoice.InvoiceItems.Any())
                    {
                        invoice.TotalAmount = invoice.InvoiceItems.Sum(i => i.NetAmount);

                        db.ConsReceivableInvoices.Attach(invoice);
                        db.Entry(invoice).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
        }

        public decimal GetAdvanceAmountToBeRepaid(int workItemId)
        {
            //var repayableAmnt = new decimal();

            var currentWorkItem = db.WorkItemPerProject.Find(workItemId);
            if (currentWorkItem != null)
            {
                var currentProject = db.ProjectInformation.Find(currentWorkItem.ProjectInformationId);
                if (currentProject != null)
                {
                    repayableAmnt = BillingHelper.GetAmountOfUnPaidAdvancePayment(currentProject.ProjectInformationId);
                }
            }

            return repayableAmnt;
        }
        public JsonResult TransferDistribution(string id, string description)
        {
            AccountHelper accountHelper = new AccountHelper();
            int myid = int.Parse(id);
            ConsReceivableInvoice record = db.ConsReceivableInvoices.Find(myid);

            if (record != null && record.IsOnlineApproved && !record.IsOnlineTransferred)
            {
                #region check segment length validation

                foreach (var dist in record.InvoiceItems)
                {
                    string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
                    if (!String.IsNullOrEmpty(message))
                    {
                        var formattedData = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                }


                #endregion
                #region check if debit and credit are equal
                decimal debit = 0;
                decimal credit = 0;
                var invoiceDistributions = db.ConsReceivableInvoiceDistribution.Where(d => d.ConsReceivableInvoiceId == record.ConsReceivableInvoiceId).ToList();
                //var distribution = record.IssueDistribution.ToList();
                if (invoiceDistributions.Any())
                {
                    debit = Math.Round(invoiceDistributions.Sum(x => x.Debit), 2);
                    credit = Math.Round(invoiceDistributions.Sum(x => x.Credit), 2);
                }
                if (debit != credit)
                {
                    var formattedData = new
                    {
                        Message = "Total debit and total credit is not balanced.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                if (debit == 0)
                {
                    var formattedData = new
                    {
                        Message = "Total debit and total credit is zero.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                #endregion
                #region send to GL
                string glperiod = "";
                string glyear = "";
                #region get period

                var period = db.ConstructionPeriod.FirstOrDefault(p => p.DateFrom < record.Date && p.DateTo > record.Date);
                //var period = db.InventoryPeriods.Find(record.GLPeriodId);


                if (period != null)
                {
                    var glperiodObj = db.GLPeriods.FirstOrDefault(x => x.Name == period.Name);

                    if (glperiodObj != null)
                    {
                        glperiod = glperiodObj.Name;
                        glyear = glperiodObj.GlFiscalYear.Name;
                        if (glperiodObj.Status == FiscalYearStatus.Close)
                        {
                            var formattedData = new
                            {
                                Message = $"The period { glperiodObj.Name } is closed,the record cannot be transfered to closed period.",
                                Status = "NOT_OK"
                            };
                            return Json(formattedData, JsonRequestBehavior.AllowGet);
                        }
                    }

                    else
                    {
                        var formattedData = new
                        {
                            Message = $"The corresponding GL period {period.Name} is not found.",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);

                    }
                }
                else
                {
                    var formattedData = new
                    {
                        Message = $"The period is not found in construction period.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

                #endregion
                if (invoiceDistributions.Any())
                {
                    GlRecordJournal journal = new GlRecordJournal
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        JournalName = "Cons-Invoice",
                        Description = !String.IsNullOrEmpty(description) ? description : "Cons-Invoice",
                        FiscalYear = glyear,
                        PeriodName = glperiod,
                        CategoryName = "Cons-Invoice",
                        SourcesName = "Cons-Invoice",
                        JournalNumber = "Cons-Invoice." + record.ConsReceivableInvoiceId.ToString(),
                        EffectiveDate = record.Date,
                        Status = "Approved",
                        IsOnlineApproved = true,
                        OnlineApprovedBy = User.Identity.Name,
                        OnlineApprovedTime = DateTime.Now,
                        OrgBranchName = record.OrgBranchName
                    };

                    db.GLRecordJournals.Add(journal);
                    db.SaveChanges();
                    foreach (ConsReceivableInvoiceDistribution i in invoiceDistributions)
                    {
                        #region debit and credit

                        var journalDetail = new GLRecordJournalDetail
                        {
                            DateCreated = DateTime.Now,
                            CreatedBy = User.Identity.Name,
                            GLRecordJournal = journal,
                            AccountCode = i.AccountCode,
                            AccountDesc = i.AccountDesc,
                            AdditionalDescription = i.AdditionalDescription,
                            Debit = i.Debit,
                            Credit = i.Credit
                        };
                        db.GLRecordJournalDetails.Add(journalDetail);
                        db.SaveChanges();
                        #endregion
                    }
                    record.IsOnlineTransferred = true;
                    record.IsOnlineTransferredBy = User.Identity.Name;
                    record.OnlineTransferredTime = DateTime.Now;
                    db.Entry(record).State = EntityState.Modified;
                    db.SaveChanges();
                    var formattedData = new
                    {
                        Message = "Successfully transferred.",
                        Status = "OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var formattedData = new
                    {
                        Message = "There is no distribution to be transferred.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                #endregion

            }
            if (record != null && !record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record cannot be transfered before validation.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record != null && record.IsOnlineTransferred)
            {
                var formattedData = new
                {
                    Message = "You cannot transfer this transaction, it is already transfered.",
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