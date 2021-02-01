using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Helpers.Construction
{
    public static class BillingHelper
    {
        private static ExceedDbContext db = new ExceedDbContext();

        public static decimal CalculateTaxAmount(int glTaxId, decimal subTotal)
        {
            var amnt = new decimal();
            var taxType = db.GLTaxRates.Find(glTaxId);
            if (taxType != null)
            {
                var calculatedRate = taxType.CalculatedRate;
                amnt = calculatedRate * subTotal;
            }
            return amnt;
        }

        public static int GetPaymentTermDays(int id)
        {
            var termDays = 0;
            var paymentTerm = db.PaymentTerms.Find(id);
            if (paymentTerm != null)
            {
                termDays = paymentTerm.PaymentTermsDays;
            }
            return termDays;
        }
        public static decimal GetApplyAmountFromInvoice(string invoiceNumber)
        {
            var currentInvoice = db.ConsReceivableInvoices.FirstOrDefault(inv => inv.InvoiceNumber == invoiceNumber);
            var amnt = new decimal();
            if (currentInvoice != null)
            {
                var invoiceDetails = db.ConsReceivableInvoiceDetails
                    .Where(d => d.ConsReceivableInvoiceId == currentInvoice.ConsReceivableInvoiceId).ToList();
                if (invoiceDetails.Any())
                {
                    amnt = invoiceDetails.Sum(d => d.NetAmount);
                }
            }
            return amnt;
        }

        // calculate taxes for collections with receipt type of Advance Prepayment or Invoice
        public static decimal CalculateTaxAmount(int glTaxId, int collectionId, ConsARStandardCollectionDetail record)
        {
            var taxAmnt = new decimal();
            var taxType = db.GLTaxRates.Find(glTaxId);
            var currentCollection = db.ConsARStandardCollections.Find(collectionId);
            if (currentCollection != null && taxType != null)
            {
                if (currentCollection.ReceiptType == StandardCollectionReceiptType.Prepayment)
                {
                    taxAmnt = record.AdvanceAmount * taxType.CalculatedRate;
                }
                else if (currentCollection.ReceiptType == StandardCollectionReceiptType.Invoice)
                {
                    taxAmnt = record.TaxAmount;
                }
            }
            return taxAmnt;
        }
       // public static decimal Get

        // calculating withholding taxes for Standar Collections
        public static decimal CalculateWithholdingTaxAmount(int glTaxId, int collectionId,
            ConsARStandardCollectionDetail record)
        {
            var taxAmnt = new decimal();
            var taxType = db.GLTaxRates.Find(glTaxId);
            var currentCollection = db.ConsARStandardCollections.Find(collectionId);
            if (currentCollection != null && taxType != null)
            {
                if (currentCollection.ReceiptType == StandardCollectionReceiptType.Prepayment)
                {
                    taxAmnt = record.AdvanceAmount * taxType.CalculatedRate;
                }
                else if (currentCollection.ReceiptType == StandardCollectionReceiptType.Invoice)
                {
                    taxAmnt = (record.ApplyAmount - record.Retention) * taxType.CalculatedRate;
                }
            }
            return taxAmnt;
        }
        //
        //  Summary:
        //      Gets the total collected amount of advance payment (or prepayment) for a given
        //      project or customer
        public static decimal GetAmountOfCollectedAdvancePaymentPerProject(int projectId)
        {
            var amnt = new decimal();
            var collections = db.ConsARStandardCollections
                .Where(c => c.ReceiptType == StandardCollectionReceiptType.Prepayment &&
                            c.ProjectInformationId == projectId).ToList();

            if (collections.Any())
            {
                amnt = collections.Sum(c => c.PrepaymentBalance);
            }
            return amnt;
        }

        public static decimal GetAmountOfUnPaidAdvancePayment(int projectId)
        {
            var amnt = new decimal();
            var totalAdvanceAmount = GetAmountOfCollectedAdvancePaymentPerProject(projectId);
            var repayedAmount = new decimal();
            var invoices = db.ConsReceivableInvoices.Where(inv => inv.ProjectInformationId == projectId).ToList();
            if (invoices.Any())
            {

                foreach (var inv in invoices)
                {
                    var invoiceItems = db.ConsReceivableInvoiceDetails.Where(invd => invd.ConsReceivableInvoiceId == inv.ConsReceivableInvoiceId).ToList();
                    if (invoiceItems.Any())
                    {
                        repayedAmount += invoiceItems.Sum(itm => itm.AdvanceRepayment);
                    }
                }
            }
            amnt = totalAdvanceAmount - repayedAmount;
            if (amnt < 0)
            {
                amnt = 0;
            }
            return amnt;
        }
        /// <summary>
        /// used to update -
        /// work item .... actual excuted todate
        /// BoQ Content .... excuted todate and amount
        /// Task .... Excuted qty and amnt todate
        /// </summary>
        /// <param name="takeOffSheetId"></param>
        public static void UpdateWorkItemBoQAndScheduleOnTakeOffSheetContentChange(int takeOffSheetId)
        {

            var currentTakeOfSheet = db.ProjectTakeOff.Find(takeOffSheetId);
            if (currentTakeOfSheet != null)
            {
                var currentWorkItem = db.WorkItemPerProject.Find(currentTakeOfSheet.WorkItemPerProjectId);
                
                if (currentWorkItem != null)
                {
                    var workItemUnitRate = currentWorkItem.HasAdditionalTransportationCost ? currentWorkItem.TotalUnitCostIncludingTransport: currentWorkItem.TotalUnitCost;
                    var executedQuantityOfWorkTodate = new decimal();
                    if (currentTakeOfSheet.IsReinforcementbarTakeOff)
                    {
                        var content = db.ProjectTakeOffLineForKg
                        .Where(tc => tc.ProjectTakeOffId == currentTakeOfSheet.ProjectTakeOffId).ToList();
                        if (content.Any())
                        {
                            executedQuantityOfWorkTodate = content.Sum(c => c.TotalWeight);
                        }
                    }
                    else
                    {
                        var takeOffContents = db.ProjectTakeOffLine
                        .Where(tc => tc.ProjectTakeOffId == currentTakeOfSheet.ProjectTakeOffId).ToList();
                        if (takeOffContents.Any())
                        {
                            executedQuantityOfWorkTodate = takeOffContents.Sum(c => c.Quantity);
                        }
                    }
                    

                    if (executedQuantityOfWorkTodate > 0)
                    {
                        #region update work item


                        var totalQuantity = currentWorkItem.TotalQuantityOfWork + currentWorkItem.AddedQuantityOfWork - currentWorkItem.OmittedQuantityOfWork;

                        //currentWorkItem.ActualQuantityOfWork  = totalQuantity <  executedQuantityOfWorkTodate ? totalQuantity : executedQuantityOfWorkTodate;
                        currentWorkItem.ActualQuantityOfWork  = executedQuantityOfWorkTodate;

                        db.WorkItemPerProject.Attach(currentWorkItem);
                        db.Entry(currentWorkItem).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        #endregion

                        #region update BoQ
                        var latestBoQ = GetProjectActiveLatestBoQ(currentWorkItem.ProjectInformationId);
                        if(latestBoQ != null)
                        {
                            var boqContentToModify =
                                db.BoQOfExcutedWorkContent
                                .FirstOrDefault(bqc => bqc.BoQOfExecutedWorkId == latestBoQ.BoQOfExecutedWorkId && bqc.WorkItemPerProjectId == currentWorkItem.WorkItemPerProjectId);
                            if(boqContentToModify != null)
                            {
                                boqContentToModify.QuantityToDate = currentWorkItem.ActualQuantityOfWork;
                                boqContentToModify.QuantityThisPayment = boqContentToModify.QuantityToDate - boqContentToModify.QuantityTillPrevPayment;

                                boqContentToModify.AmountThisPayment = boqContentToModify.QuantityThisPayment * boqContentToModify.UnitRate;
                                boqContentToModify.AmountToDatePayment = boqContentToModify.QuantityToDate * boqContentToModify.UnitRate;

                                db.BoQOfExcutedWorkContent.Attach(boqContentToModify);
                                db.Entry(boqContentToModify).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        #endregion

                        #region update schedule
                        var latestVersion = GetProjectLatestVersion(currentWorkItem.ProjectInformationId);
                        if (latestVersion == 1)
                        {
                            var tasksToModify = db.Tasks.Where(t => t.WorkItemDuration.WorkItemPerProjectId == currentWorkItem.WorkItemPerProjectId &&
                            t.Version == latestVersion && !(t.IsSubActivityCreatedInSchedule)).ToList();

                            if (tasksToModify.Any())
                            {
                                foreach (Core.Domain.Construction.Task task in tasksToModify)
                                {
                                    task.ExecutedQuantityToDate = executedQuantityOfWorkTodate > task.TotalQuantityOfWork ? task.TotalQuantityOfWork : executedQuantityOfWorkTodate;
                                    task.ExecutedQuantityThisVersion = executedQuantityOfWorkTodate > task.TotalQuantityOfWork ? task.TotalQuantityOfWork : executedQuantityOfWorkTodate;
                                    task.ExecutedQuantityTillPrevVersion = 0;
                                    task.ExecutedAmountToDate = task.ExecutedQuantityToDate * workItemUnitRate;
                                    task.ExecutedAmountThisVersion = task.ExecutedQuantityThisVersion * workItemUnitRate;
                                    task.ExecutedAmountTillPrevVersion = 0;

                                    if(task.ExecutedQuantityToDate > 0)
                                    {
                                        task.PercentComplete = task.ExecutedQuantityToDate / task.TotalQuantityOfWork;
                                    }

                                    db.Tasks.Attach(task);
                                    db.Entry(task).State = System.Data.Entity.EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                        }
                        else if (latestVersion > 1)
                        {
                            var tasksToModify = db.Tasks.Where(t => t.WorkItemDuration.WorkItemPerProjectId == currentWorkItem.WorkItemPerProjectId &&
                            t.Version == latestVersion && !(t.IsSubActivityCreatedInSchedule)).ToList();
                            if (tasksToModify.Any())
                            {
                                foreach (Core.Domain.Construction.Task task in tasksToModify)
                                {
                                    task.ExecutedQuantityToDate = executedQuantityOfWorkTodate > task.TotalQuantityOfWork ? task.TotalQuantityOfWork : executedQuantityOfWorkTodate;
                                    task.ExecutedQuantityTillPrevVersion = GetPreviousVersionTask(task).ExecutedQuantityToDate;
                                    task.ExecutedQuantityThisVersion = task.ExecutedQuantityToDate - task.ExecutedQuantityTillPrevVersion;

                                    task.ExecutedAmountThisVersion = task.ExecutedQuantityThisVersion * workItemUnitRate;
                                    task.ExecutedAmountTillPrevVersion = task.ExecutedQuantityTillPrevVersion * workItemUnitRate;
                                    task.ExecutedAmountToDate = task.ExecutedQuantityToDate * workItemUnitRate;

                                    db.Tasks.Attach(task);
                                    db.Entry(task).State = System.Data.Entity.EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                        } 
                        #endregion
                    }
                    // there is no executed work 
                    else
                    {
                        currentWorkItem.ActualQuantityOfWork = 0;

                        db.WorkItemPerProject.Attach(currentWorkItem);
                        db.Entry(currentWorkItem).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
        }
        private static Core.Domain.Construction.Task GetPreviousVersionTask(Core.Domain.Construction.Task record)
        {
            var taskToReturn = new Core.Domain.Construction.Task();
            var task = db.Tasks.FirstOrDefault(t => t.ID == record.PreviousVersionTaskId);
            if (task != null)
            {
                taskToReturn = task;
            }
            return taskToReturn;
        }
        private static int GetProjectLatestVersion(int projectId)
        {
            var version = 0;
            var revisions = db.ScheduleRevisions.Where(rev => rev.ProjectInformationId == projectId).ToList();
            if (revisions.Any())
            {
                version = revisions.Max(rv => rv.Version);
            }
            return version;
        }
        /// <summary>
        /// Returns the active BOQ (one that is latest and not used in payment yet) of a given project
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        private static BoQOfExecutedWork GetProjectActiveLatestBoQ(int projectId)
        {
            var boqs = db.BoQOfExcutedWork.Where(b => b.ProjectInformationId == projectId).OrderByDescending(b => b.BillNumber).ToList();
            if (boqs.Any())
            {
               var boqsArray = boqs.ToArray();
                var latestBoQ = boqsArray[0];
                #region check is the boq can be modified 
                var payment = db.PaymentCertificates.FirstOrDefault(pc => pc.BillReference == latestBoQ.BillReference);
                // ... boq not used in Payment Certificates, its free for modification.
                if (payment == null)
                {
                    return boqsArray[0];
                }
                // ...its used in a payment certificate, cannot be modified.
                else
                {
                    return null;
                }
                #endregion
            }
            else
            {
                return null;
            }
        }
    }
}
