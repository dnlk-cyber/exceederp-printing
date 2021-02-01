using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Finance.Receivable.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Finance.Receivable.DataSource
{
   
    [DataObject]
    public class ReceiptsJournalOds : List<ReceiptsJournalViewModel>
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ReceiptsJournalViewModel> GetReceiptJournal(string periodfrom, string periodto, string customer,string branch,string paymentmethod)
        {
            List<ReceiptsJournalViewModel> receiptJournalLists = new List<ReceiptsJournalViewModel>();

            if (!String.IsNullOrEmpty(periodfrom) && !String.IsNullOrEmpty(periodto))
            {
                int from = int.Parse(periodfrom);
                int to = int.Parse(periodto);
                DateTime dt11 = _db.GLPeriods.Find(from).DateFrom.Date;
                DateTime dt22 = _db.GLPeriods.Find(to).DateTo.Date;
                
                List<string> accountList = new List<string>();

                if(!string.IsNullOrEmpty(paymentmethod))
                {
                   // string naturalSegment = paymentmethod.Split('-')[0];
                    accountList.Add(paymentmethod);
                }
                else
                {
                    var paymentMethods = _db.PaymentMethods.Where(x => x.ApplyToAr).Select(x => x.ApAccountCode).Distinct().ToList();
                    foreach (var account in paymentMethods)
                    {
                        if (!string.IsNullOrEmpty(account))
                        {
                            //string naturalSegment = account.ToString().Split('-')[0];
                            var e = accountList.FirstOrDefault(x => x == account);
                            if (e == null)
                                accountList.Add(account);
                        }
                    }
                }
                
                #region standard colletion

                var arStandardCollectionDistributions = _db.ARStandardCollectionDistributions
                    .Where(x => x.ArStandardCollection.ReceiptDate >= dt11 && x.ArStandardCollection.ReceiptDate <= dt22)
                    .Where(x => x.ArStandardCollection.IsVoid == false)
                    .Where(x => x.ArStandardCollection.IsOnlineApproved)
                    .Where(x => accountList.Any(f => x.AccountCode==f) ||x.AccountCode.StartsWith("13101") || x.AccountCode.StartsWith("11104")) 
                    //.Where(x => accountList.Any(f => x.Description.StartsWith(f)))
                    .ToList();

               
                if (!String.IsNullOrEmpty(customer))
                {
                    arStandardCollectionDistributions = arStandardCollectionDistributions.Where(x => x.ArStandardCollection.CustomerName.ToUpper() == customer.ToUpper()).ToList();
                }

                foreach (var distribution in arStandardCollectionDistributions)
                {
                    if (!String.IsNullOrEmpty(branch))
                    {
                        //string str = "-" + branch + "-";
                        int did = int.Parse(branch);
                        var arstandardlines = _db.ARStandardCollectionLines
                            .Where(x => x.ArStandardCollectionId == distribution.ArStandardCollectionId)
                            .Where(x=>x.BranchId==did).ToList();
                        if (!arstandardlines.Any())
                            continue;
                    }


                    var receiptsJournalViewModel = new ReceiptsJournalViewModel
                    {
                        Date = distribution.ArStandardCollection.ReceiptDate,
                        CustomerName = distribution.ArStandardCollection.CustomerName,
                        TransactionNo = distribution.ArStandardCollection.ArStandardCollectionId.ToString(),
                        OrderNo = "",
                        ReferenceNo = distribution.ArStandardCollection.Reference,
                        PaymentMethod = distribution.ArStandardCollection.ReceiptMethod,
                        AccountCode = distribution.AccountCode,
                        AccountDescription = distribution.AccountDesc,
                        DebitAmount = distribution.Debit,
                        CreditAmount = distribution.Credit
                    };


                    receiptJournalLists.Add(receiptsJournalViewModel);

                }

                #endregion
                #region misc colletion

                var miscelaneousCollectionDistributions = _db.ARMiscelaneousCollectionDistributions.Where(x => x.ARMiscelaneousCollection.ReceiptDate >= dt11 && x.ARMiscelaneousCollection.ReceiptDate <= dt22)
                    .Where(x => x.ARMiscelaneousCollection.IsVoid == false)
                    .Where(x => x.ARMiscelaneousCollection.IsOnlineApproved)
                    .Where(x => accountList.Any(f => x.AccountCode == f) || x.AccountCode.StartsWith("13101") || x.AccountCode.StartsWith("11104"))
                    //.Where(x => accountList.Any(f => x.Description.StartsWith(f)))
                    .ToList();

                if (!String.IsNullOrEmpty(branch))
                {
                    int did = int.Parse(branch);
                   // string str = "-" + branch + "-";
                    miscelaneousCollectionDistributions = miscelaneousCollectionDistributions
                                                         .Where(x => x.ARMiscelaneousCollection.BranchId==did).ToList();
                }

                if (!String.IsNullOrEmpty(customer))
                {
                    miscelaneousCollectionDistributions = miscelaneousCollectionDistributions.Where(x => x.ARMiscelaneousCollection.CustomerName.ToUpper() == customer.ToUpper()).ToList();
                }

                foreach (var distribution in miscelaneousCollectionDistributions)
                {
                    var receiptsJournalViewModel = new ReceiptsJournalViewModel
                    {
                        Date = distribution.ARMiscelaneousCollection.ReceiptDate,
                        CustomerName = distribution.ARMiscelaneousCollection.CustomerName,
                        TransactionNo = distribution.ARMiscelaneousCollection.ARMiscelaneousCollectionID.ToString(),
                        OrderNo = "",
                        ReferenceNo = distribution.ARMiscelaneousCollection.Remark,
                        PaymentMethod = distribution.ARMiscelaneousCollection.ReceiptMethod,
                        AccountCode = distribution.AccountCode,
                        AccountDescription = distribution.AccountDesc,
                        DebitAmount = distribution.Debit,
                        CreditAmount = distribution.Credit
                    };

                    receiptJournalLists.Add(receiptsJournalViewModel);

                }

                #endregion
            }

            return receiptJournalLists;
        }
    }
}

