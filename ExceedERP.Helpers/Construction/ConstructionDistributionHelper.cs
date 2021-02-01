using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;

namespace ExceedERP.Helpers.Construction
{
    public class ConstructionDistributionHelper
    {
        private ExceedDbContext db = new ExceedDbContext();

        public void UpdateWorkInProgressDistribution(WorkInProgress workInProgress)
        {
            var existingDistribution = db.WorkInProgressDistribution
                .Where(wipd => wipd.WorkInProgressId == workInProgress.WorkInProgressId).ToList();
            if (existingDistribution.Any())
            {
                db.WorkInProgressDistribution.RemoveRange(existingDistribution);
                db.SaveChanges();
            }
            if (workInProgress.WorkInProgressItems.Any())
            {

                #region debit wip

                Dictionary<string, int> checkAccount = new Dictionary<string, int>();
                List<WorkInProgressDistribution> distributions = new List<WorkInProgressDistribution>();
                var wipCustomerAccount = db.ConstructionCustomerAccounts.FirstOrDefault(a => a.ProjectInformationId == workInProgress.ProjectInformationId && a.AccountType == ConstructionCustomerAccountTypes.WorkInProgress);
                if (wipCustomerAccount != null)
                {

                    AccountHelper accountHelper = new AccountHelper();
                    string[] account = wipCustomerAccount.AccountCode.Split('-');
                    string acccountSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Account)];
                    string locationSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location)];
                    //string costCenterSeG = HrHelper.GetWorkUnitCostCenter(record.OrgStructureId);
                    //string subAccountSeG = HrHelper.GetBranchSubAccount(branch);
                    string costCenterSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.CostCenter)];
                    string subAccountSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.SubAccount)];
                    //string costCenterSeG = "";
                    //string subAccountSeG = "empty";

                    string newacc =
                        accountHelper.RearrangeAccountSequence(acccountSeG, locationSeG, costCenterSeG, subAccountSeG);

                    foreach (WorkInProgressDetail lineItem in workInProgress.WorkInProgressItems)
                    {

                        if (lineItem.TotalAmount != 0 && checkAccount.ContainsKey(newacc))
                        {
                            var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == newacc);
                            if (existingDstn != null)
                            {
                                existingDstn.AccountCode = newacc;
                                existingDstn.AccountDesc = accountHelper.GetAccountDesc(newacc);
                                existingDstn.Debit += lineItem.TotalAmount;
                                existingDstn.Credit = 0;
                                existingDstn.WorkInProgressId = workInProgress.WorkInProgressId;
                                existingDstn.WorkInProgressWorkItemId = lineItem.WorkItemPerProjectId;

                            }

                        }
                        else if (lineItem.TotalAmount != 0 && !checkAccount.ContainsKey(newacc))
                        {
                            WorkInProgressDistribution distribution = new WorkInProgressDistribution
                            {
                                AccountCode = newacc,
                                AccountDesc = accountHelper.GetAccountDesc(newacc),
                                Debit = lineItem.TotalAmount,
                                Credit = 0,
                                WorkInProgressId = lineItem.WorkInProgressId,
                                WorkInProgressWorkItemId = lineItem.WorkItemPerProjectId
                            };
                            distributions.Add(distribution);
                            checkAccount.Add(newacc, lineItem.WorkItemPerProjectId);
                        }
                    }
                }


                foreach (WorkInProgressDistribution progressDistribution in distributions)
                {
                    db.WorkInProgressDistribution.Add(progressDistribution);
                    db.SaveChanges();
                }
                distributions.Clear();
                checkAccount.Clear();

                #endregion

                #region credit revenue

                foreach (WorkInProgressDetail lineItem in workInProgress.WorkInProgressItems)
                {
                    AccountHelper accountHelper = new AccountHelper();
                    string acc = lineItem.AccountCode;
                    string[] account = acc.Split('-');
                    string acccountSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Account)];
                    string locationSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location)];
                    //string costCenterSeG = HrHelper.GetWorkUnitCostCenter(record.OrgStructureId);
                    //string subAccountSeG = HrHelper.GetBranchSubAccount(branch);
                    string costCenterSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.CostCenter)];
                    string subAccountSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.SubAccount)];

                    string newacc =
                        accountHelper.RearrangeAccountSequence(acccountSeG, locationSeG, costCenterSeG, subAccountSeG);

                    if (lineItem.SubTotal != 0 && checkAccount.ContainsKey(newacc))
                    {
                        var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == newacc);
                        if (existingDstn != null)
                        {
                            existingDstn.AccountCode = newacc;
                            existingDstn.AccountDesc = accountHelper.GetAccountDesc(newacc);
                            existingDstn.Debit = 0;
                            existingDstn.Credit += lineItem.SubTotal;
                            existingDstn.WorkInProgressId = workInProgress.WorkInProgressId;
                            existingDstn.WorkInProgressWorkItemId = lineItem.WorkItemPerProjectId;
                        }
                    }
                    else if (lineItem.SubTotal != 0 && !checkAccount.ContainsKey(newacc))
                    {
                        WorkInProgressDistribution distribution = new WorkInProgressDistribution
                        {
                            AccountCode = newacc,
                            AccountDesc = accountHelper.GetAccountDesc(newacc),
                            Debit = 0,
                            Credit = lineItem.SubTotal,
                            WorkInProgressId = lineItem.WorkInProgressId,
                            WorkInProgressWorkItemId = lineItem.WorkItemPerProjectId
                        };
                        distributions.Add(distribution);
                        checkAccount.Add(newacc, lineItem.WorkItemPerProjectId);
                    }
                }
                foreach (WorkInProgressDistribution progressDistribution in distributions)
                {
                    db.WorkInProgressDistribution.Add(progressDistribution);
                    db.SaveChanges();
                }
                distributions.Clear();
                checkAccount.Clear();

                #endregion

                #region credit VAT


                foreach (WorkInProgressDetail lineItem in workInProgress.WorkInProgressItems)
                {
                    AccountHelper accountHelper = new AccountHelper();
                    if (lineItem.TaxAmount != 0)
                    {
                        var tax = db.GLTaxRates.Find(lineItem.GLTaxRateID);
                        if (tax != null)
                        {
                            string acc = tax.ArAccountCode;
                            if (checkAccount.ContainsKey(acc))
                            {
                                var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == acc);
                                if (existingDstn != null)
                                {
                                    existingDstn.Credit += lineItem.TaxAmount;
                                    existingDstn.Debit = 0;
                                    existingDstn.AccountCode = acc;
                                    existingDstn.AccountDesc = accountHelper.GetAccountDesc(acc);
                                    existingDstn.WorkInProgressId = lineItem.WorkInProgressId;
                                }
                            }
                            else if (!checkAccount.ContainsKey(acc))
                            {
                                var distribution = new WorkInProgressDistribution
                                {
                                    AccountCode = acc,
                                    AccountDesc = accountHelper.GetAccountDesc(acc),
                                    Debit = 0,
                                    Credit = lineItem.TaxAmount,
                                    WorkInProgressId = lineItem.WorkInProgressId,
                                    WorkInProgressWorkItemId = lineItem.WorkItemPerProjectId
                                };
                                distributions.Add(distribution);
                                checkAccount.Add(acc, lineItem.WorkItemPerProjectId);
                            }
                        }
                    }
                }
                foreach (WorkInProgressDistribution progressDistribution in distributions)
                {
                    db.WorkInProgressDistribution.Add(progressDistribution);
                    db.SaveChanges();
                }
                distributions.Clear();
                checkAccount.Clear();

                #endregion
            }
        }

        public void UpdateReceivableInvoice(ConsReceivableInvoice invoice)
        {
            var existingDst = db.ConsReceivableInvoiceDistribution
                .Where(ds => ds.ConsReceivableInvoiceId == invoice.ConsReceivableInvoiceId).ToList();
            if (existingDst.Any())
            {
                db.ConsReceivableInvoiceDistribution.RemoveRange(existingDst);
                db.SaveChanges();
            }
            var receivableAccountEntity = db.ConstructionCustomerAccounts.FirstOrDefault(a => a.ProjectInformationId == invoice.ProjectInformationId && a.AccountType == ConstructionCustomerAccountTypes.ReceivableAccount);
            var retentionAccountEntity = db.ConstructionCustomerAccounts.FirstOrDefault(a => a.ProjectInformationId == invoice.ProjectInformationId && a.AccountType == ConstructionCustomerAccountTypes.Retention);
            var advanceRepaymentAccountEntity = db.ConstructionCustomerAccounts.FirstOrDefault(a => a.ProjectInformationId == invoice.ProjectInformationId && a.AccountType == ConstructionCustomerAccountTypes.AdvancePayment);
            var penalityAccountEntity = db.ConstructionCustomerAccounts.FirstOrDefault(a => a.ProjectInformationId == invoice.ProjectInformationId && a.AccountType == ConstructionCustomerAccountTypes.Penality);
            var otherAccountEntity = db.ConstructionCustomerAccounts.FirstOrDefault(a => a.ProjectInformationId == invoice.ProjectInformationId && a.AccountType == ConstructionCustomerAccountTypes.Others);
        
            string receivableAccount = "", retentionAccount = "", advanceRepaymentAccount = "", penalityAccount = "", otherAccount = "";
            if (receivableAccountEntity != null && retentionAccountEntity != null && advanceRepaymentAccountEntity != null && penalityAccountEntity != null && otherAccountEntity != null)
            {
                receivableAccount = receivableAccountEntity.AccountCode;
                retentionAccount = retentionAccountEntity.AccountCode;
                advanceRepaymentAccount = advanceRepaymentAccountEntity.AccountCode;
                penalityAccount = penalityAccountEntity.AccountCode;
                otherAccount = otherAccountEntity.AccountCode;
            }
            if (invoice.InvoiceItems.Any() && !string.IsNullOrEmpty(receivableAccount) && !string.IsNullOrEmpty(retentionAccount) && !string.IsNullOrEmpty(advanceRepaymentAccount) && !string.IsNullOrEmpty(penalityAccount) && !string.IsNullOrEmpty(otherAccount))
            {
                Dictionary<string, int> checkAccount = new Dictionary<string, int>();
                List<ConsReceivableInvoiceDistribution> distributions = new List<ConsReceivableInvoiceDistribution>();

                #region debit customer receivable (net amount)
                foreach (var invoiceItem in invoice.InvoiceItems)
                {
                    AccountHelper accountHelper = new AccountHelper();
                    //string acc = invoiceItem.AccountCode;
                    string[] account = receivableAccount.Split('-');
                    string acccountSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Account)];
                    string locationSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location)];
                    //string costCenterSeG = HrHelper.GetWorkUnitCostCenter(record.OrgStructureId);
                    //string subAccountSeG = HrHelper.GetBranchSubAccount(branch);
                    string costCenterSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.CostCenter)];
                    string subAccountSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.SubAccount)];

                    string newacc =
                        accountHelper.RearrangeAccountSequence(acccountSeG, locationSeG, costCenterSeG, subAccountSeG);

                    if (invoiceItem.NetAmount != 0 && checkAccount.ContainsKey(newacc))
                    {
                        var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == newacc);
                        if (existingDstn != null)
                        {
                            existingDstn.AccountCode = newacc;
                            existingDstn.AccountDesc = accountHelper.GetAccountDesc(newacc);
                            existingDstn.Debit += invoiceItem.NetAmount;
                            existingDstn.Credit = 0;
                            existingDstn.ConsReceivableInvoiceId = invoice.ConsReceivableInvoiceId;
                        }
                    }
                    else if (invoiceItem.NetAmount != 0 && !checkAccount.ContainsKey(newacc))
                    {
                        ConsReceivableInvoiceDistribution distribution = new ConsReceivableInvoiceDistribution
                        {
                            AccountCode = newacc,
                            AccountDesc = accountHelper.GetAccountDesc(newacc),
                            Debit = invoiceItem.NetAmount,
                            Credit = 0,
                            ConsReceivableInvoiceId = invoiceItem.ConsReceivableInvoiceId,
                        };
                        distributions.Add(distribution);
                        checkAccount.Add(newacc, invoiceItem.WorkItemPerProjectId);
                    }
                }
                foreach (ConsReceivableInvoiceDistribution invoiceDistribution in distributions)
                {
                    db.ConsReceivableInvoiceDistribution.Add(invoiceDistribution);
                    db.SaveChanges();
                }
                distributions.Clear();
                checkAccount.Clear();

                #endregion

                #region debit Retention
                foreach (var invoiceItem in invoice.InvoiceItems)
                {
                    AccountHelper accountHelper = new AccountHelper();
                    //string acc = invoiceItem.AccountCode;
                    string[] account = retentionAccount.Split('-');
                    string acccountSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Account)];
                    string locationSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location)];
                    //string costCenterSeG = HrHelper.GetWorkUnitCostCenter(record.OrgStructureId);
                    //string subAccountSeG = HrHelper.GetBranchSubAccount(branch);
                    string costCenterSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.CostCenter)];
                    string subAccountSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.SubAccount)];

                    string newacc =
                        accountHelper.RearrangeAccountSequence(acccountSeG, locationSeG, costCenterSeG, subAccountSeG);

                    if (invoiceItem.Retention != 0 && checkAccount.ContainsKey(newacc))
                    {
                        var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == newacc);
                        if (existingDstn != null)
                        {
                            existingDstn.AccountCode = newacc;
                            existingDstn.AccountDesc = accountHelper.GetAccountDesc(newacc);
                            existingDstn.Debit += invoiceItem.Retention;
                            existingDstn.Credit = 0;
                            existingDstn.ConsReceivableInvoiceId = invoice.ConsReceivableInvoiceId;
                        }
                    }
                    else if (invoiceItem.Retention != 0 && !checkAccount.ContainsKey(newacc))
                    {
                        ConsReceivableInvoiceDistribution distribution = new ConsReceivableInvoiceDistribution
                        {
                            AccountCode = newacc,
                            AccountDesc = accountHelper.GetAccountDesc(newacc),
                            Debit = invoiceItem.Retention,
                            Credit = 0,
                            ConsReceivableInvoiceId = invoiceItem.ConsReceivableInvoiceId,
                        };
                        distributions.Add(distribution);
                        checkAccount.Add(newacc, invoiceItem.WorkItemPerProjectId);
                    }
                }
                foreach (ConsReceivableInvoiceDistribution invoiceDistribution in distributions)
                {
                    db.ConsReceivableInvoiceDistribution.Add(invoiceDistribution);
                    db.SaveChanges();
                }
                distributions.Clear();
                checkAccount.Clear();

                #endregion

                #region debit advance repayment

                foreach (var invoiceItem in invoice.InvoiceItems)
                {
                    AccountHelper accountHelper = new AccountHelper();
                    //string acc = invoiceItem.AccountCode;
                    string[] account = advanceRepaymentAccount.Split('-');
                    string acccountSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Account)];
                    string locationSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location)];
                    //string costCenterSeG = HrHelper.GetWorkUnitCostCenter(record.OrgStructureId);
                    //string subAccountSeG = HrHelper.GetBranchSubAccount(branch);
                    string costCenterSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.CostCenter)];
                    string subAccountSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.SubAccount)];

                    string newacc =
                        accountHelper.RearrangeAccountSequence(acccountSeG, locationSeG, costCenterSeG, subAccountSeG);

                    if (invoiceItem.AdvanceRepayment != 0 && checkAccount.ContainsKey(newacc))
                    {
                        var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == newacc);
                        if (existingDstn != null)
                        {
                            existingDstn.AccountCode = newacc;
                            existingDstn.AccountDesc = accountHelper.GetAccountDesc(newacc);
                            existingDstn.Debit += invoiceItem.AdvanceRepayment;
                            existingDstn.Credit = 0;
                            existingDstn.ConsReceivableInvoiceId = invoice.ConsReceivableInvoiceId;
                        }
                    }
                    else if (invoiceItem.AdvanceRepayment != 0 && !checkAccount.ContainsKey(newacc))
                    {
                        ConsReceivableInvoiceDistribution distribution = new ConsReceivableInvoiceDistribution
                        {
                            AccountCode = newacc,
                            AccountDesc = accountHelper.GetAccountDesc(newacc),
                            Debit = invoiceItem.AdvanceRepayment,
                            Credit = 0,
                            ConsReceivableInvoiceId = invoiceItem.ConsReceivableInvoiceId,
                        };
                        distributions.Add(distribution);
                        checkAccount.Add(newacc, invoiceItem.WorkItemPerProjectId);
                    }
                }
                foreach (ConsReceivableInvoiceDistribution invoiceDistribution in distributions)
                {
                    db.ConsReceivableInvoiceDistribution.Add(invoiceDistribution);
                    db.SaveChanges();
                }
                distributions.Clear();
                checkAccount.Clear();

                #endregion
                #region debit correction and other payments
                foreach (var invoiceItem in invoice.InvoiceItems)
                {
                    AccountHelper accountHelper = new AccountHelper();
                    //string acc = invoiceItem.AccountCode;
                    string[] account = otherAccount.Split('-');
                    string acccountSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Account)];
                    string locationSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location)];
                    //string costCenterSeG = HrHelper.GetWorkUnitCostCenter(record.OrgStructureId);
                    //string subAccountSeG = HrHelper.GetBranchSubAccount(branch);
                    string costCenterSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.CostCenter)];
                    string subAccountSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.SubAccount)];

                    string newacc =
                        accountHelper.RearrangeAccountSequence(acccountSeG, locationSeG, costCenterSeG, subAccountSeG);

                    if ((invoiceItem.Correction != 0 || invoiceItem.Others != 0 )&& checkAccount.ContainsKey(newacc))
                    {
                        var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == newacc);
                        if (existingDstn != null)
                        {
                            existingDstn.AccountCode = newacc;
                            existingDstn.AccountDesc = accountHelper.GetAccountDesc(newacc);
                            existingDstn.Debit += (invoiceItem.Correction + invoiceItem.Others);
                            existingDstn.Credit = 0;
                            existingDstn.ConsReceivableInvoiceId = invoice.ConsReceivableInvoiceId;
                        }
                    }
                    else if ((invoiceItem.Correction != 0 || invoiceItem.Others != 0) && !checkAccount.ContainsKey(newacc))
                    {
                        ConsReceivableInvoiceDistribution distribution = new ConsReceivableInvoiceDistribution
                        {
                            AccountCode = newacc,
                            AccountDesc = accountHelper.GetAccountDesc(newacc),
                            Debit = (invoiceItem.Correction + invoiceItem.Others),
                            Credit = 0,
                            ConsReceivableInvoiceId = invoiceItem.ConsReceivableInvoiceId,
                        };
                        distributions.Add(distribution);
                        checkAccount.Add(newacc, invoiceItem.WorkItemPerProjectId);
                    }
                }
                foreach (ConsReceivableInvoiceDistribution invoiceDistribution in distributions)
                {
                    db.ConsReceivableInvoiceDistribution.Add(invoiceDistribution);
                    db.SaveChanges();
                }
                distributions.Clear();
                checkAccount.Clear();

                #endregion
                #region debit Penality

                foreach (var invoiceItem in invoice.InvoiceItems)
                {
                    AccountHelper accountHelper = new AccountHelper();
                    //string acc = invoiceItem.AccountCode;
                    string[] account = penalityAccount.Split('-');
                    string acccountSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Account)];
                    string locationSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location)];
                    //string costCenterSeG = HrHelper.GetWorkUnitCostCenter(record.OrgStructureId);
                    //string subAccountSeG = HrHelper.GetBranchSubAccount(branch);
                    string costCenterSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.CostCenter)];
                    string subAccountSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.SubAccount)];

                    string newacc =
                        accountHelper.RearrangeAccountSequence(acccountSeG, locationSeG, costCenterSeG, subAccountSeG);

                    if (invoiceItem.Penality != 0 && checkAccount.ContainsKey(newacc))
                    {
                        var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == newacc);
                        if (existingDstn != null)
                        {
                            existingDstn.AccountCode = newacc;
                            existingDstn.AccountDesc = accountHelper.GetAccountDesc(newacc);
                            existingDstn.Debit += invoiceItem.Penality;
                            existingDstn.Credit = 0;
                            existingDstn.ConsReceivableInvoiceId = invoice.ConsReceivableInvoiceId;
                        }
                    }
                    else if (invoiceItem.Penality != 0 && !checkAccount.ContainsKey(newacc))
                    {
                        ConsReceivableInvoiceDistribution distribution = new ConsReceivableInvoiceDistribution
                        {
                            AccountCode = newacc,
                            AccountDesc = accountHelper.GetAccountDesc(newacc),
                            Debit = invoiceItem.Penality,
                            Credit = 0,
                            ConsReceivableInvoiceId = invoiceItem.ConsReceivableInvoiceId,
                        };
                        distributions.Add(distribution);
                        checkAccount.Add(newacc, invoiceItem.WorkItemPerProjectId);
                    }
                }
                foreach (ConsReceivableInvoiceDistribution invoiceDistribution in distributions)
                {
                    db.ConsReceivableInvoiceDistribution.Add(invoiceDistribution);
                    db.SaveChanges();
                }
                distributions.Clear();
                checkAccount.Clear();

                #endregion

                #region credit progress in billing (subtotal)

                foreach (var invoiceItem in invoice.InvoiceItems)
                {
                    AccountHelper accountHelper = new AccountHelper();
                    string acc = invoiceItem.AccountCode;
                    string[] account = acc.Split('-');
                    string acccountSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Account)];
                    string locationSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location)];
                    //string costCenterSeG = HrHelper.GetWorkUnitCostCenter(record.OrgStructureId);
                    //string subAccountSeG = HrHelper.GetBranchSubAccount(branch);
                    string costCenterSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.CostCenter)];
                    string subAccountSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.SubAccount)];

                    string newacc =
                        accountHelper.RearrangeAccountSequence(acccountSeG, locationSeG, costCenterSeG, subAccountSeG);

                    if (invoiceItem.SubTotal != 0 && checkAccount.ContainsKey(newacc))
                    {
                        var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == newacc);
                        if (existingDstn != null)
                        {
                            existingDstn.AccountCode = newacc;
                            existingDstn.AccountDesc = accountHelper.GetAccountDesc(newacc);
                            existingDstn.Debit = 0;
                            existingDstn.Credit += invoiceItem.SubTotal;
                            existingDstn.ConsReceivableInvoiceId = invoice.ConsReceivableInvoiceId;
                        }
                    }
                    else if (invoiceItem.NetAmount != 0 && !checkAccount.ContainsKey(newacc))
                    {
                        ConsReceivableInvoiceDistribution distribution = new ConsReceivableInvoiceDistribution
                        {
                            AccountCode = newacc,
                            AccountDesc = accountHelper.GetAccountDesc(newacc),
                            Debit = 0,
                            Credit = invoiceItem.SubTotal,
                            ConsReceivableInvoiceId = invoiceItem.ConsReceivableInvoiceId,
                        };
                        distributions.Add(distribution);
                        checkAccount.Add(newacc, invoiceItem.WorkItemPerProjectId);
                    }
                }
                foreach (ConsReceivableInvoiceDistribution invoiceDistribution in distributions)
                {
                    db.ConsReceivableInvoiceDistribution.Add(invoiceDistribution);
                    db.SaveChanges();
                }
                distributions.Clear();
                checkAccount.Clear();

                #endregion

                #region credit VAT

                foreach (ConsReceivableInvoiceDetail lineItem in invoice.InvoiceItems)
                {
                    AccountHelper accountHelper = new AccountHelper();
                    if (lineItem.TaxAmount != 0)
                    {
                        var tax = db.GLTaxRates.Find(lineItem.GLTaxRateID);
                        if (tax != null)
                        {
                            string acc = tax.ArAccountCode;
                            if (checkAccount.ContainsKey(acc))
                            {
                                var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == acc);
                                if (existingDstn != null)
                                {
                                    existingDstn.Credit += lineItem.TaxAmount;
                                    existingDstn.Debit = 0;
                                    existingDstn.AccountCode = acc;
                                    existingDstn.AccountDesc = accountHelper.GetAccountDesc(acc);
                                    existingDstn.ConsReceivableInvoiceId = lineItem.ConsReceivableInvoiceId;
                                }
                            }
                            else if (!checkAccount.ContainsKey(acc))
                            {
                                var distribution = new ConsReceivableInvoiceDistribution
                                {
                                    AccountCode = acc,
                                    AccountDesc = accountHelper.GetAccountDesc(acc),
                                    Debit = 0,
                                    Credit = lineItem.TaxAmount,
                                    ConsReceivableInvoiceId = lineItem.ConsReceivableInvoiceId,
                                };
                                distributions.Add(distribution);
                                checkAccount.Add(acc, lineItem.WorkItemPerProjectId);
                            }
                        }
                    }
                }
                foreach (ConsReceivableInvoiceDistribution invoiceDistribution in distributions)
                {
                    db.ConsReceivableInvoiceDistribution.Add(invoiceDistribution);
                    db.SaveChanges();
                }
                distributions.Clear();
                checkAccount.Clear();
                #endregion
            }
        }

        public void UpdateStandardCollectionDistribution(ConsARStandardCollection collection)
        {
            var existingDistributions = db.ConsStandardCollectionDistribution
                .Where(cd => cd.ConsARStandardCollectionId == collection.ConsARStandardCollectionId).ToList();
            if (existingDistributions.Any())
            {
                db.ConsStandardCollectionDistribution.RemoveRange(existingDistributions);
                db.SaveChanges();
            }
            var receivableAccountEntity = db.ConstructionCustomerAccounts.FirstOrDefault(a => a.ProjectInformationId == collection.ProjectInformationId && a.AccountType == ConstructionCustomerAccountTypes.ReceivableAccount);
            var retentionAccountEntity = db.ConstructionCustomerAccounts.FirstOrDefault(a => a.ProjectInformationId == collection.ProjectInformationId && a.AccountType == ConstructionCustomerAccountTypes.Retention);
            var advancePaymentAccountEntity = db.ConstructionCustomerAccounts.FirstOrDefault(a => a.ProjectInformationId == collection.ProjectInformationId && a.AccountType == ConstructionCustomerAccountTypes.AdvancePayment);
            var penalityAccountEntity = db.ConstructionCustomerAccounts.FirstOrDefault(a => a.ProjectInformationId == collection.ProjectInformationId && a.AccountType == ConstructionCustomerAccountTypes.Penality);

            string receivableAccount = " ", retentionAccount = " ", advancePaymentAccount = " ", penalityAccount = " ";
            if (receivableAccountEntity != null && retentionAccountEntity != null && advancePaymentAccountEntity != null && penalityAccountEntity != null)
            {
                receivableAccount = receivableAccountEntity.AccountCode;
                retentionAccount = retentionAccountEntity.AccountCode;
                advancePaymentAccount = advancePaymentAccountEntity.AccountCode;
                penalityAccount = penalityAccountEntity.AccountCode;
            }
            AccountHelper accountHelper = new AccountHelper();
            if (collection.StandardCollectionItems.Any() && receivableAccount != " " && retentionAccount != " " && advancePaymentAccount != " " && penalityAccount != " ")
            {
                var vatWitholdingAccount = db.GLVatWithholdings.FirstOrDefault();
                var paymentMethod = db.PaymentMethods.Find(collection.PaymentMethodID);
                if (paymentMethod != null)
                {
                    var targetAccount = paymentMethod.ArAccountCode;


                    #region collectin against invoice 

                    if (collection.ReceiptType == StandardCollectionReceiptType.Invoice)
                    {
                        Dictionary<string, int> checkAccount = new Dictionary<string, int>();
                        List<ConsStandardCollectionDistribution> distributions = new List<ConsStandardCollectionDistribution>();

                        //foreach (var lineItem in collection.StandardCollectionItems)
                        //{
                        #region no retention and no advance repayment

                        //if (lineItem.AdvanceAmount == 0 && lineItem.Retention == 0)
                        //{
                        #region debit to bank/cash account
                        foreach (var lineItem in collection.StandardCollectionItems)
                        {
                            if (checkAccount.ContainsKey(targetAccount))
                            {
                                var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == targetAccount);
                                if (existingDstn != null)
                                {
                                    existingDstn.Debit += lineItem.VatWithholding ? lineItem.ApplyAmount - (lineItem.TaxAmount + lineItem.WithholdingTaxAmount) :
                                                                 lineItem.ApplyAmount - lineItem.WithholdingTaxAmount;

                                }
                            }
                            else if (!checkAccount.ContainsKey(targetAccount))
                            {
                                var dstn = new ConsStandardCollectionDistribution
                                {
                                    AccountCode = targetAccount,
                                    AccountDesc = accountHelper.GetAccountDesc(targetAccount),

                                    Debit = lineItem.VatWithholding ? lineItem.ApplyAmount - (lineItem.TaxAmount + lineItem.WithholdingTaxAmount) :
                                                                 lineItem.ApplyAmount - lineItem.WithholdingTaxAmount,
                                    Credit = 0,
                                    ConsARStandardCollectionId = lineItem.ConsARStandardCollectionId
                                };
                                distributions.Add(dstn);
                                checkAccount.Add(targetAccount, 0);
                            }
                        }
                        foreach (ConsStandardCollectionDistribution collectionDistribution in distributions)
                        {
                            db.ConsStandardCollectionDistribution.Add(collectionDistribution);
                            db.SaveChanges();
                        }
                        distributions.Clear();
                        checkAccount.Clear();
                        #endregion

                        #region debit withholding tax
                        foreach (var lineItem in collection.StandardCollectionItems)
                        {
                            var withholding = db.GLTaxRates.Find(lineItem.WithholdingTaxRateId);
                            if (withholding != null)
                            {
                                string acc = withholding.ArAccountCode;

                                if (checkAccount.ContainsKey(acc))
                                {
                                    var existingWitholdingDistribution = distributions.FirstOrDefault(d => d.AccountCode == acc);
                                    if (existingWitholdingDistribution != null)
                                    {
                                        existingWitholdingDistribution.Debit += lineItem.WithholdingTaxAmount;
                                    }
                                }
                                else if (!checkAccount.ContainsKey(acc))
                                {
                                    var distribution = new ConsStandardCollectionDistribution
                                    {
                                        AccountCode = acc,
                                        AccountDesc = accountHelper.GetAccountDesc(acc),
                                        Debit = lineItem.WithholdingTaxAmount,
                                        Credit = 0,
                                        ConsARStandardCollectionId = lineItem.ConsARStandardCollectionId
                                    };
                                    distributions.Add(distribution);
                                    checkAccount.Add(acc, 0);
                                }
                            }
                        }
                        foreach (ConsStandardCollectionDistribution collectionDistribution in distributions)
                        {
                            db.ConsStandardCollectionDistribution.Add(collectionDistribution);
                            db.SaveChanges();
                        }
                        distributions.Clear();
                        checkAccount.Clear();
                        #endregion

                        #region debit VAT withholding
                        foreach (var lineItem in collection.StandardCollectionItems)
                        {
                            if (vatWitholdingAccount != null && lineItem.VatWithholding &&
                                lineItem.TaxAmount != 0)
                            {
                                string acc = vatWitholdingAccount.WithholdingAccountCode;
                                if (checkAccount.ContainsKey(acc))
                                {
                                    var existingVatWithholdingDistribution = distributions.FirstOrDefault(d => d.AccountCode == acc);
                                    if (existingVatWithholdingDistribution != null)
                                    {
                                        existingVatWithholdingDistribution.Debit += lineItem.TaxAmount;
                                    }
                                }
                                else if (!checkAccount.ContainsKey(acc))
                                {
                                    var distribution = new ConsStandardCollectionDistribution
                                    {
                                        AccountCode = acc,
                                        AccountDesc = accountHelper.GetAccountDesc(acc),
                                        Debit = lineItem.TaxAmount,
                                        Credit = 0,
                                        ConsARStandardCollectionId = lineItem.ConsARStandardCollectionId
                                    };
                                    distributions.Add(distribution);
                                    checkAccount.Add(acc, 0);
                                }
                            }
                        }
                        foreach (ConsStandardCollectionDistribution collectionDistribution in distributions)
                        {
                            db.ConsStandardCollectionDistribution.Add(collectionDistribution);
                            db.SaveChanges();
                        }
                        distributions.Clear();
                        checkAccount.Clear();
                        #endregion

                        #region credit customer receivable
                        foreach (var lineItem in collection.StandardCollectionItems)
                        {
                            if (checkAccount.ContainsKey(receivableAccount))
                            {
                                var existingReceivableDistribution = distributions.FirstOrDefault(d => d.AccountCode == receivableAccount);
                                if (existingReceivableDistribution != null)
                                {
                                    existingReceivableDistribution.Credit += lineItem.ApplyAmount;
                                }
                            }
                            else if (!checkAccount.ContainsKey(receivableAccount))
                            {
                                var distr = new ConsStandardCollectionDistribution
                                {
                                    AccountCode = receivableAccount,
                                    AccountDesc = accountHelper.GetAccountDesc(receivableAccount),
                                    Debit = 0,
                                    Credit =  lineItem.ApplyAmount,
                                    ConsARStandardCollectionId = lineItem.ConsARStandardCollectionId
                                };
                                distributions.Add(distr);
                                checkAccount.Add(receivableAccount, 0);
                            }
                        }
                        foreach (ConsStandardCollectionDistribution collectionDistribution in distributions)
                        {
                            db.ConsStandardCollectionDistribution.Add(collectionDistribution);
                            db.SaveChanges();
                        }
                        distributions.Clear();
                        checkAccount.Clear();
                        #endregion

                        #region credit VAT payable
                        foreach (var lineItem in collection.StandardCollectionItems)
                        {
                            //if (!lineItem.VatWithholding && lineItem.TaxAmount != 0)
                            //{
                            //    var tax = db.GLTaxRates.Find(lineItem.GLTaxRateID);
                            //    if (tax != null)
                            //    {
                            //        string acc = tax.ArAccountCode;

                            //        var distribution = new ConsStandardCollectionDistribution
                            //        {
                            //            AccountCode = acc,
                            //            AccountDesc = AccountHelper.GetAccountDesc(acc),
                            //            Debit = 0,
                            //            Credit = lineItem.TaxAmount,
                            //            ConsARStandardCollectionId = lineItem.ConsARStandardCollectionId,
                            //        };
                            //        db.ConsStandardCollectionDistribution.Add(distribution);
                            //        db.SaveChanges();
                            //    }
                            //}
                        }
                        foreach (ConsStandardCollectionDistribution collectionDistribution in distributions)
                        {
                            db.ConsStandardCollectionDistribution.Add(collectionDistribution);
                            db.SaveChanges();
                        }
                        distributions.Clear();
                        checkAccount.Clear();
                        #endregion

                        //}

                        #endregion

                        #region with advance repayment but no retention

                        //else if (lineItem.AdvanceAmount != 0 && lineItem.Retention == 0)
                        //{
                        //    #region debit Cash/Bank

                        //    var dstn = new ConsStandardCollectionDistribution
                        //    {
                        //        AccountCode = targetAccount,
                        //        AccountDesc = AccountHelper.GetAccountDesc(targetAccount),
                        //        Debit = (lineItem.ApplyAmount + lineItem.TaxAmount) -
                        //                (lineItem.Retention + lineItem.AdvanceAmount +
                        //                 lineItem.WithholdingTaxAmount),
                        //        Credit = 0,
                        //        ConsARStandardCollectionId = lineItem.ConsARStandardCollectionId
                        //    };
                        //    db.ConsStandardCollectionDistribution.Add(dstn);
                        //    db.SaveChanges();

                        //    #endregion

                        //    #region debit withholding tax

                        //    var withholding = db.GLTaxRates.Find(lineItem.WithholdingTaxRateId);
                        //    if (withholding != null)
                        //    {
                        //        string acc = withholding.ArAccountCode;

                        //        var distribution = new ConsStandardCollectionDistribution
                        //        {
                        //            AccountCode = acc,
                        //            AccountDesc = AccountHelper.GetAccountDesc(acc),
                        //            Debit = lineItem.WithholdingTaxAmount,
                        //            Credit = 0,
                        //            ConsARStandardCollectionId = lineItem.ConsARStandardCollectionId
                        //        };
                        //        db.ConsStandardCollectionDistribution.Add(distribution);
                        //        db.SaveChanges();
                        //    }

                        //    #endregion

                        //    #region debit VAT withholding

                        //    if (vatWitholdingAccount != null && lineItem.VatWithholding &&
                        //        lineItem.WithholdingTaxAmount != 0)
                        //    {
                        //        string acc = vatWitholdingAccount.WithholdingAccountCode;
                        //        var distribution = new ConsStandardCollectionDistribution
                        //        {
                        //            AccountCode = acc,
                        //            AccountDesc = AccountHelper.GetAccountDesc(acc),
                        //            Debit = lineItem.TaxAmount,
                        //            Credit = 0,
                        //            ConsARStandardCollectionId = lineItem.ConsARStandardCollectionId
                        //        };
                        //        db.ConsStandardCollectionDistribution.Add(distribution);
                        //        db.SaveChanges();
                        //    }

                        //    #endregion

                        //    #region debit Advance Repayment

                        //    var dstnRep = new ConsStandardCollectionDistribution
                        //    {
                        //        AccountCode = targetAccount,
                        //        AccountDesc = AccountHelper.GetAccountDesc(targetAccount),
                        //        Debit = lineItem.AdvanceAmount,
                        //        Credit = 0,
                        //        ConsARStandardCollectionId = lineItem.ConsARStandardCollectionId
                        //    };
                        //    db.ConsStandardCollectionDistribution.Add(dstnRep);
                        //    db.SaveChanges();

                        //    #endregion

                        //    #region credit customer receivable

                        //    var distr = new ConsStandardCollectionDistribution
                        //    {
                        //        AccountCode = targetAccount,
                        //        AccountDesc = AccountHelper.GetAccountDesc(targetAccount),
                        //        Debit = 0,
                        //        Credit = lineItem.ApplyAmount,
                        //        ConsARStandardCollectionId = lineItem.ConsARStandardCollectionId
                        //    };
                        //    db.ConsStandardCollectionDistribution.Add(distr);
                        //    db.SaveChanges();

                        //    #endregion

                        //    #region credit VAT payable

                        //    if (!lineItem.VatWithholding && lineItem.TaxAmount != 0)
                        //    {
                        //        var tax = db.GLTaxRates.Find(lineItem.GLTaxRateID);
                        //        if (tax != null)
                        //        {
                        //            string acc = tax.ArAccountCode;

                        //            var distribution = new ConsStandardCollectionDistribution
                        //            {
                        //                AccountCode = acc,
                        //                AccountDesc = AccountHelper.GetAccountDesc(acc),
                        //                Debit = 0,
                        //                Credit = lineItem.TaxAmount,
                        //                ConsARStandardCollectionId = lineItem.ConsARStandardCollectionId,
                        //            };
                        //            db.ConsStandardCollectionDistribution.Add(distribution);
                        //            db.SaveChanges();
                        //        }

                        //    }

                        //    #endregion
                        //}

                        #endregion

                        #region with advance repayment and retention

                        //else if (lineItem.AdvanceAmount != 0 && lineItem.Retention != 0)
                        //{
                        //    #region debit Cash/Bank

                        //    var dstn = new ConsStandardCollectionDistribution
                        //    {
                        //        AccountCode = targetAccount,
                        //        AccountDesc = AccountHelper.GetAccountDesc(targetAccount),
                        //        Debit = (lineItem.ApplyAmount + lineItem.TaxAmount) -
                        //                (lineItem.Retention + lineItem.AdvanceAmount +
                        //                 lineItem.WithholdingTaxAmount),
                        //        Credit = 0,
                        //        ConsARStandardCollectionId = lineItem.ConsARStandardCollectionId
                        //    };
                        //    db.ConsStandardCollectionDistribution.Add(dstn);
                        //    db.SaveChanges();

                        //    #endregion

                        //    #region debit withholding tax

                        //    var withholding = db.GLTaxRates.Find(lineItem.WithholdingTaxRateId);
                        //    if (withholding != null)
                        //    {
                        //        string acc = withholding.ArAccountCode;

                        //        var distribution = new ConsStandardCollectionDistribution
                        //        {
                        //            AccountCode = acc,
                        //            AccountDesc = AccountHelper.GetAccountDesc(acc),
                        //            Debit = lineItem.WithholdingTaxAmount,
                        //            Credit = 0,
                        //            ConsARStandardCollectionId = lineItem.ConsARStandardCollectionId
                        //        };
                        //        db.ConsStandardCollectionDistribution.Add(distribution);
                        //        db.SaveChanges();
                        //    }

                        //    #endregion

                        //    #region debit VAT withholding

                        //    if (vatWitholdingAccount != null && lineItem.VatWithholding &&
                        //        lineItem.WithholdingTaxAmount != 0)
                        //    {
                        //        string acc = vatWitholdingAccount.WithholdingAccountCode;
                        //        var distribution = new ConsStandardCollectionDistribution
                        //        {
                        //            AccountCode = acc,
                        //            AccountDesc = AccountHelper.GetAccountDesc(acc),
                        //            Debit = lineItem.TaxAmount,
                        //            Credit = 0,
                        //            ConsARStandardCollectionId = lineItem.ConsARStandardCollectionId
                        //        };
                        //        db.ConsStandardCollectionDistribution.Add(distribution);
                        //        db.SaveChanges();
                        //    }

                        //    #endregion

                        //    #region debit advance payable by customer

                        //    var dstnRep = new ConsStandardCollectionDistribution
                        //    {
                        //        AccountCode = targetAccount,
                        //        AccountDesc = AccountHelper.GetAccountDesc(targetAccount),
                        //        Debit = lineItem.AdvanceAmount,
                        //        Credit = 0,
                        //        ConsARStandardCollectionId = lineItem.ConsARStandardCollectionId
                        //    };
                        //    db.ConsStandardCollectionDistribution.Add(dstnRep);
                        //    db.SaveChanges();

                        //    #endregion

                        //    #region debit retention receivable

                        //    var dstnRetn = new ConsStandardCollectionDistribution
                        //    {
                        //        AccountCode = targetAccount,
                        //        AccountDesc = AccountHelper.GetAccountDesc(targetAccount),
                        //        Debit = lineItem.Retention,
                        //        Credit = 0,
                        //        ConsARStandardCollectionId = lineItem.ConsARStandardCollectionId
                        //    };
                        //    db.ConsStandardCollectionDistribution.Add(dstnRetn);
                        //    db.SaveChanges();

                        //    #endregion

                        //    #region credit Customer receivable

                        //    var distr = new ConsStandardCollectionDistribution
                        //    {
                        //        AccountCode = targetAccount,
                        //        AccountDesc = AccountHelper.GetAccountDesc(targetAccount),
                        //        Debit = 0,
                        //        Credit = lineItem.ApplyAmount,
                        //        ConsARStandardCollectionId = lineItem.ConsARStandardCollectionId
                        //    };
                        //    db.ConsStandardCollectionDistribution.Add(distr);
                        //    db.SaveChanges();

                        //    #endregion

                        //    #region credit VAT payable

                        //    if (!lineItem.VatWithholding && lineItem.TaxAmount != 0)
                        //    {
                        //        var tax = db.GLTaxRates.Find(lineItem.GLTaxRateID);
                        //        if (tax != null)
                        //        {
                        //            string acc = tax.ArAccountCode;

                        //            var distribution = new ConsStandardCollectionDistribution
                        //            {
                        //                AccountCode = acc,
                        //                AccountDesc = AccountHelper.GetAccountDesc(acc),
                        //                Debit = 0,
                        //                Credit = lineItem.TaxAmount,
                        //                ConsARStandardCollectionId = lineItem.ConsARStandardCollectionId,
                        //            };
                        //            db.ConsStandardCollectionDistribution.Add(distribution);
                        //            db.SaveChanges();
                        //        }

                        //    }

                        //    #endregion
                        //}

                        #endregion


                        //}
                    }

                    #endregion

                    #region advance payment collection
                    else
                    {
                        Dictionary<string, int> checkAccount = new Dictionary<string, int>();
                        List<ConsStandardCollectionDistribution> distributions = new List<ConsStandardCollectionDistribution>();
                        #region debit Cash/Bank
                        foreach (var lineItem in collection.StandardCollectionItems)
                        {
                            if (checkAccount.ContainsKey(targetAccount))
                            {
                                var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == targetAccount);
                                if (existingDstn != null)
                                {
                                    existingDstn.Debit += lineItem.VatWithholding ? lineItem.AdvanceAmount - lineItem.Retention - lineItem.WithholdingTaxAmount :
                                        (lineItem.AdvanceAmount + lineItem.TaxAmount) - (lineItem.Retention + lineItem.WithholdingTaxAmount);
                                }
                            }
                            else if (!checkAccount.ContainsKey(targetAccount))
                            {
                                var dstn = new ConsStandardCollectionDistribution
                                {
                                    AccountCode = targetAccount,
                                    AccountDesc = accountHelper.GetAccountDesc(targetAccount),
                                    Debit = lineItem.VatWithholding ? lineItem.AdvanceAmount - lineItem.Retention - lineItem.WithholdingTaxAmount :
                                        (lineItem.AdvanceAmount + lineItem.TaxAmount) - (lineItem.Retention + lineItem.WithholdingTaxAmount),
                                    Credit = 0,
                                    ConsARStandardCollectionId = lineItem.ConsARStandardCollectionId
                                };
                                distributions.Add(dstn);
                                checkAccount.Add(targetAccount, 0);
                            }
                        }
                        foreach (ConsStandardCollectionDistribution collectionDistribution in distributions)
                        {
                            db.ConsStandardCollectionDistribution.Add(collectionDistribution);
                            db.SaveChanges();
                        }
                        distributions.Clear();
                        checkAccount.Clear();
                        #endregion

                        #region debit withholding tax
                        foreach (var lineItem in collection.StandardCollectionItems)
                        {
                            var withholding = db.GLTaxRates.Find(lineItem.WithholdingTaxRateId);
                            if (withholding != null && lineItem.WithholdingTaxAmount != 0)
                            {
                                string acc = withholding.ArAccountCode;

                                if (checkAccount.ContainsKey(acc))
                                {
                                    var existingWitholdingDistribution = distributions.FirstOrDefault(d => d.AccountCode == acc);
                                    if (existingWitholdingDistribution != null)
                                    {
                                        existingWitholdingDistribution.Debit += lineItem.WithholdingTaxAmount;
                                    }
                                }
                                else if (!checkAccount.ContainsKey(acc))
                                {
                                    var distribution = new ConsStandardCollectionDistribution
                                    {
                                        AccountCode = acc,
                                        AccountDesc = accountHelper.GetAccountDesc(acc),
                                        Debit = lineItem.WithholdingTaxAmount,
                                        Credit = 0,
                                        ConsARStandardCollectionId = lineItem.ConsARStandardCollectionId
                                    };
                                    distributions.Add(distribution);
                                    checkAccount.Add(acc, 0);
                                }
                            }
                        }
                        foreach (ConsStandardCollectionDistribution collectionDistribution in distributions)
                        {
                            db.ConsStandardCollectionDistribution.Add(collectionDistribution);
                            db.SaveChanges();
                        }
                        distributions.Clear();
                        checkAccount.Clear();
                        #endregion

                        #region debit VAT withholding
                        foreach (var lineItem in collection.StandardCollectionItems)
                        {
                            if (vatWitholdingAccount != null && lineItem.VatWithholding &&
                                lineItem.TaxAmount != 0)
                            {
                                string acc = vatWitholdingAccount.WithholdingAccountCode;
                                if (checkAccount.ContainsKey(acc))
                                {
                                    var existingVatWithholdingDistribution = distributions.FirstOrDefault(d => d.AccountCode == acc);
                                    if (existingVatWithholdingDistribution != null)
                                    {
                                        existingVatWithholdingDistribution.Debit += lineItem.TaxAmount;
                                    }
                                }
                                else if (!checkAccount.ContainsKey(acc))
                                {
                                    var distribution = new ConsStandardCollectionDistribution
                                    {
                                        AccountCode = acc,
                                        AccountDesc = accountHelper.GetAccountDesc(acc),
                                        Debit = lineItem.TaxAmount,
                                        Credit = 0,
                                        ConsARStandardCollectionId = lineItem.ConsARStandardCollectionId
                                    };
                                    distributions.Add(distribution);
                                    checkAccount.Add(acc, 0);
                                }
                            }
                        }
                        foreach (ConsStandardCollectionDistribution collectionDistribution in distributions)
                        {
                            db.ConsStandardCollectionDistribution.Add(collectionDistribution);
                            db.SaveChanges();
                        }
                        distributions.Clear();
                        checkAccount.Clear();
                        #endregion

                        #region debit Retention
                        foreach (var lineItem in collection.StandardCollectionItems)
                        {
                            if (lineItem.Retention != 0)
                            {
                                if (checkAccount.ContainsKey(retentionAccount))
                                {
                                    var existingRetentionDistribution = distributions.FirstOrDefault(d => d.AccountCode == retentionAccount);
                                    if (existingRetentionDistribution != null)
                                    {
                                        existingRetentionDistribution.Debit += lineItem.Retention;
                                    }
                                }
                                else if (!checkAccount.ContainsKey(retentionAccount))
                                {
                                    var dstnRetn = new ConsStandardCollectionDistribution
                                    {
                                        AccountCode = retentionAccount,
                                        AccountDesc = accountHelper.GetAccountDesc(targetAccount),
                                        Debit = lineItem.Retention,
                                        Credit = 0,
                                        ConsARStandardCollectionId = lineItem.ConsARStandardCollectionId
                                    };
                                    distributions.Add(dstnRetn);
                                    checkAccount.Add(retentionAccount, 0);
                                }

                            }
                        }
                        foreach (ConsStandardCollectionDistribution collectionDistribution in distributions)
                        {
                            db.ConsStandardCollectionDistribution.Add(collectionDistribution);
                            db.SaveChanges();
                        }
                        distributions.Clear();
                        checkAccount.Clear();
                        #endregion

                        #region credit advance receivable by customer
                        foreach (var lineItem in collection.StandardCollectionItems)
                        {
                            if (checkAccount.ContainsKey(advancePaymentAccount))
                            {
                                var existingAdvancePaymentDistribution = distributions.FirstOrDefault(d => d.AccountCode == advancePaymentAccount);
                                if (existingAdvancePaymentDistribution != null)
                                {
                                    existingAdvancePaymentDistribution.Credit += lineItem.AdvanceAmount;
                                }
                            }
                            else if (!checkAccount.ContainsKey(advancePaymentAccount))
                            {
                                var dstnAdvance = new ConsStandardCollectionDistribution
                                {
                                    AccountCode = advancePaymentAccount,
                                    AccountDesc = accountHelper.GetAccountDesc(targetAccount),
                                    Debit = 0,
                                    Credit = lineItem.AdvanceAmount,
                                    ConsARStandardCollectionId = lineItem.ConsARStandardCollectionId
                                };
                                distributions.Add(dstnAdvance);
                                checkAccount.Add(advancePaymentAccount, 0);
                            }
                        }
                        foreach (ConsStandardCollectionDistribution collectionDistribution in distributions)
                        {
                            db.ConsStandardCollectionDistribution.Add(collectionDistribution);
                            db.SaveChanges();
                        }
                        distributions.Clear();
                        checkAccount.Clear();
                        #endregion

                        #region credit VAT payable
                        foreach (var lineItem in collection.StandardCollectionItems)
                        {
                            if (lineItem.TaxAmount != 0)
                            {
                                var tax = db.GLTaxRates.Find(lineItem.GLTaxRateID);
                                if (tax != null)
                                {
                                    string acc = tax.ArAccountCode;
                                    if (checkAccount.ContainsKey(acc))
                                    {
                                        var existingVatDistribution = distributions.FirstOrDefault(d => d.AccountCode == acc);
                                        if (existingVatDistribution != null)
                                        {
                                            existingVatDistribution.Credit += lineItem.TaxAmount;
                                        }
                                    }
                                    else if (!checkAccount.ContainsKey(acc))
                                    {
                                        var distribution = new ConsStandardCollectionDistribution
                                        {
                                            AccountCode = acc,
                                            AccountDesc = accountHelper.GetAccountDesc(acc),
                                            Debit = 0,
                                            Credit = lineItem.TaxAmount,
                                            ConsARStandardCollectionId = lineItem.ConsARStandardCollectionId,
                                        };
                                        distributions.Add(distribution);
                                        checkAccount.Add(acc, 0);
                                    }

                                }

                            }
                        }
                        foreach (ConsStandardCollectionDistribution collectionDistribution in distributions)
                        {
                            db.ConsStandardCollectionDistribution.Add(collectionDistribution);
                            db.SaveChanges();
                        }
                        distributions.Clear();
                        checkAccount.Clear();
                        #endregion

                    }
                }

                #endregion
            }
        }
    }
}


