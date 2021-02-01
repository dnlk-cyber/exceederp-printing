using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Helpers.Finance;

namespace ExceedERP.Web.Areas.Construction.Controllers.Billing
{
    [AuthorizeRoles(ConstructionConstants.CostingUser)]
    public class StandardCollectionController : Controller
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

            ViewData["Branchs"] = db.Branch.Select(b => new { b.Name, b.BranchId }).ToList();

            var paymentMethods = db.PaymentMethods;
            ViewData["PaymentMethods"] = paymentMethods;

            return View();
        }
        public ActionResult ProjectInformation_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProjectInformation> projectinformation = db.ProjectInformation.Where(x => x.ProjectStatus == ProjectStatus.STARTED || x.ProjectStatus == ProjectStatus.APPROVED).AsQueryable();
            DataSourceResult result = projectinformation.ToDataSourceResult(request, projectInformation => new
            {
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
        public ActionResult StandardCollections_Read([DataSourceRequest]DataSourceRequest request, int projectId)
        {
            IQueryable<ConsARStandardCollection> collections = db.ConsARStandardCollections.Where(c => c.ProjectInformationId == projectId).OrderByDescending(c => c.ConsARStandardCollectionId);
            DataSourceResult result = collections.ToDataSourceResult(request, collection => new
            {
                collection.ConsARStandardCollectionId,
                collection.ProjectInformationId,
                collection.ReceiptID,
                collection.ReceiptType,
                collection.ClientName,
                collection.ReceiptNumber,
                collection.ReceiptDate,
                collection.PaymentMethodID,
                collection.Reference,
                collection.PrepaymentBalance,
                collection.OutstandingBalance,


                collection.OrgBranchName,
                collection.DateCreated,
                collection.LastModified,
                collection.CreatedBy,
                collection.ModifiedBy,

                Status = collection.Status,
                IsSendForApproval = collection.IsSendForApproval,
                SendForApprovalBy = collection.SendForApprovalBy,
                SendForApprovalTime = collection.SendForApprovalTime,
                IsOnlineApproved = collection.IsOnlineApproved,
                OnlineApprovedBy = collection.OnlineApprovedBy,
                OnlineApprovedTime = collection.OnlineApprovedTime,
                IsOnlineTransferred = collection.IsOnlineTransferred,
                IsOnlineTransferredBy = collection.IsOnlineTransferredBy,
                OnlineTransferredTime = collection.OnlineTransferredTime,
                IsVoid = collection.IsVoid,
                VoidBy = collection.VoidBy,
                VoidTime = collection.VoidTime,
            });

            return Json(result);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StandardCollection_Create([DataSourceRequest]DataSourceRequest request, ConsARStandardCollection collection, int projectIdCreate)
        {
            if (ModelState.IsValid)
            {

                var entity = new ConsARStandardCollection
                {
                    //ReceivableInvoiceId = collection.ReceivableInvoiceId,
                    ProjectInformationId = projectIdCreate,
                    ReceiptID = collection.ReceiptID,
                    ReceiptType = collection.ReceiptType,
                    ClientName = ProjectInformationHelper.GetProjectClient(projectIdCreate),
                    ReceiptNumber = collection.ReceiptNumber,
                    ReceiptDate = collection.ReceiptDate,
                    PaymentMethodID = collection.PaymentMethodID,
                    Reference = collection.Reference,
                    PrepaymentBalance = collection.PrepaymentBalance,
                    OutstandingBalance = collection.OutstandingBalance,

                    OrgBranchName = collection.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = collection.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = collection.ModifiedBy,

                    Status = OperationStatuses.New.ToString(),
                    IsSendForApproval = collection.IsSendForApproval,
                    SendForApprovalBy = collection.SendForApprovalBy,
                    SendForApprovalTime = collection.SendForApprovalTime,
                    IsOnlineApproved = collection.IsOnlineApproved,
                    OnlineApprovedBy = collection.OnlineApprovedBy,
                    OnlineApprovedTime = collection.OnlineApprovedTime,
                    IsOnlineTransferred = collection.IsOnlineTransferred,
                    IsOnlineTransferredBy = collection.IsOnlineTransferredBy,
                    OnlineTransferredTime = collection.OnlineTransferredTime,
                    IsVoid = collection.IsVoid,
                    VoidBy = collection.VoidBy,
                    VoidTime = collection.VoidTime
                };
                //entity.DueDate = this.CalculateInvoiceDueDate(entity);
                //if (!string.IsNullOrEmpty(entity.Reference))
                //{
                //    entity.IsInvoiceFromWorkInProgress = true;
                //}
                db.ConsARStandardCollections.Add(entity);
                db.SaveChanges();

                collection.ConsARStandardCollectionId = entity.ConsARStandardCollectionId;
                //collection.InvoiceNumber = entity.InvoiceNumber;

                //UpdateInvoiceNumber(entity);

                //if (entity.IsInvoiceFromWorkInProgress)
                //{
                //    this.CopyInvoiceLineFromWorkInProgress(entity.Reference, entity.ReceivableInvoiceId);
                //}
                //}
            }

            return Json(new[] { collection }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StandardCollection_Update([DataSourceRequest]DataSourceRequest request, ConsARStandardCollection collection)
        {
            if (ModelState.IsValid)
            {
                var entity = new ConsARStandardCollection
                {
                    ConsARStandardCollectionId = collection.ConsARStandardCollectionId,
                    ProjectInformationId = collection.ProjectInformationId,
                    ReceiptID = collection.ReceiptID,
                    ReceiptType = collection.ReceiptType,
                    ClientName = ProjectInformationHelper.GetProjectClient(collection.ProjectInformationId),
                    ReceiptNumber = collection.ReceiptNumber,
                    ReceiptDate = collection.ReceiptDate,
                    PaymentMethodID = collection.PaymentMethodID,
                    Reference = collection.Reference,
                    PrepaymentBalance = collection.PrepaymentBalance,
                    OutstandingBalance = collection.OutstandingBalance,

                    OrgBranchName = collection.OrgBranchName,
                    DateCreated = collection.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = collection.CreatedBy,
                    ModifiedBy = User.Identity.Name,

                    Status = collection.Status,
                    IsSendForApproval = collection.IsSendForApproval,
                    SendForApprovalBy = collection.SendForApprovalBy,
                    SendForApprovalTime = collection.SendForApprovalTime,
                    IsOnlineApproved = collection.IsOnlineApproved,
                    OnlineApprovedBy = collection.OnlineApprovedBy,
                    OnlineApprovedTime = collection.OnlineApprovedTime,
                    IsOnlineTransferred = collection.IsOnlineTransferred,
                    IsOnlineTransferredBy = collection.IsOnlineTransferredBy,
                    OnlineTransferredTime = collection.OnlineTransferredTime,
                    IsVoid = collection.IsVoid,
                    VoidBy = collection.VoidBy,
                    VoidTime = collection.VoidTime,
                };
                //entity.DueDate = this.CalculateInvoiceDueDate(entity);

                //if (!string.IsNullOrEmpty(entity.Reference))
                //{
                //    entity.IsInvoiceFromWorkInProgress = true;
                //}

                db.ConsARStandardCollections.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();


            }

            return Json(new[] { collection }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StandardCollection_Destroy([DataSourceRequest]DataSourceRequest request, ConsARStandardCollection collection)
        {
            if (ModelState.IsValid)
            {
                var entity = new ConsARStandardCollection
                {
                    ConsARStandardCollectionId = collection.ConsARStandardCollectionId,
                    ProjectInformationId = collection.ProjectInformationId,
                    ReceiptID = collection.ReceiptID,
                    ReceiptType = collection.ReceiptType,
                    ClientName = ProjectInformationHelper.GetProjectClient(collection.ProjectInformationId),
                    ReceiptNumber = collection.ReceiptNumber,
                    ReceiptDate = collection.ReceiptDate,
                    PaymentMethodID = collection.PaymentMethodID,
                    Reference = collection.Reference,
                    PrepaymentBalance = collection.PrepaymentBalance,
                    OutstandingBalance = collection.OutstandingBalance,

                    OrgBranchName = collection.OrgBranchName,
                    DateCreated = collection.DateCreated,
                    LastModified = collection.LastModified,
                    CreatedBy = collection.CreatedBy,
                    ModifiedBy = collection.ModifiedBy,

                    Status = collection.Status,
                    IsSendForApproval = collection.IsSendForApproval,
                    SendForApprovalBy = collection.SendForApprovalBy,
                    SendForApprovalTime = collection.SendForApprovalTime,
                    IsOnlineApproved = collection.IsOnlineApproved,
                    OnlineApprovedBy = collection.OnlineApprovedBy,
                    OnlineApprovedTime = collection.OnlineApprovedTime,
                    IsOnlineTransferred = collection.IsOnlineTransferred,
                    IsOnlineTransferredBy = collection.IsOnlineTransferredBy,
                    OnlineTransferredTime = collection.OnlineTransferredTime,
                    IsVoid = collection.IsVoid,
                    VoidBy = collection.VoidBy,
                    VoidTime = collection.VoidTime,
                };

                db.ConsARStandardCollections.Attach(entity);
                db.ConsARStandardCollections.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { collection }.ToDataSourceResult(request, ModelState));
        }


        //[AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        public JsonResult SendForApproval(int id)
        {
            var record = db.ConsARStandardCollections.Find(id);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved && record.StandardCollectionItems.Count > 0)
            {
                #region validate price and tax

                //foreach (var item in record.StandardCollectionItems)
                //{
                //    if (item.UnitPrice <= 0 || item.GLTaxRateID == 0)
                //    {
                //        var formattedData3 = new
                //        {
                //            Message = "Please fill unit price and tax information for all items.",
                //            Status = "NOT_OK"
                //        };
                //        return Json(formattedData3, JsonRequestBehavior.AllowGet);
                //    }
                //}
                #endregion

                //record.OperationStatus = OperationStatuses.WaitingApproval;
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                db.ConsARStandardCollections.Attach(record);
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

            if (record.StandardCollectionItems.Count == 0)
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

        public JsonResult TransferDistribution(string id, string description)
        {
            int myid = int.Parse(id);
            ConsARStandardCollection record = db.ConsARStandardCollections.Find(myid);

            if (record != null && record.IsOnlineApproved && !record.IsOnlineTransferred)
            {
                #region check segment length validation

                //foreach (var dist in record.StandardCollectionItems)
                //{
                //    string message = AccountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
                //    if (!String.IsNullOrEmpty(message))
                //    {
                //        var formattedData = new
                //        {
                //            Message = message,
                //            Status = "NOT_OK"
                //        };
                //        return Json(formattedData, JsonRequestBehavior.AllowGet);
                //    }
                //}


                #endregion
                #region check if debit and credit are equal
                decimal debit = 0;
                decimal credit = 0;
                var collectionDistributions = db.ConsStandardCollectionDistribution.Where(d => d.ConsARStandardCollectionId == record.ConsARStandardCollectionId).ToList();
                //var distribution = record.IssueDistribution.ToList();
                if (collectionDistributions.Any())
                {
                    debit = Math.Round(collectionDistributions.Sum(x => x.Debit), 2);
                    credit = Math.Round(collectionDistributions.Sum(x => x.Credit), 2);
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

                var period = db.ConstructionPeriod.FirstOrDefault(p => p.DateFrom < record.ReceiptDate && p.DateTo > record.ReceiptDate);
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
                if (collectionDistributions.Any())
                {
                    GlRecordJournal journal = new GlRecordJournal
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        JournalName = "Cons-Collection",
                        Description = !String.IsNullOrEmpty(description) ? description : "Cons-Invoice",
                        FiscalYear = glyear,
                        PeriodName = glperiod,
                        CategoryName = "Cons-Collection",
                        SourcesName = "Cons-Invoice",
                        JournalNumber = "Cons-Collection." + record.ConsARStandardCollectionId.ToString(),
                        EffectiveDate = record.ReceiptDate,
                        Status = "Approved",
                        IsOnlineApproved = true,
                        OnlineApprovedBy = User.Identity.Name,
                        OnlineApprovedTime = DateTime.Now,
                        OrgBranchName = record.OrgBranchName
                    };

                    db.GLRecordJournals.Add(journal);
                    db.SaveChanges();
                    foreach (ConsStandardCollectionDistribution i in collectionDistributions)
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