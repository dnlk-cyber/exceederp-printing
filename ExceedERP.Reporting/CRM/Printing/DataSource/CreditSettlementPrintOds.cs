using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.CRM.Printing.ViewModel;
//using ExceedERP.Reporting.CRM.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using XAPI;

namespace ExceedERP.Reporting.CRM.Printing.DataSource
{
    [DataObject]
    class CreditSettlementPrintOds
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<CreditSettlementPrintViewModel> GetShipmentData(string creditSettlemetId)
        {
            try
            {
                List<CreditSettlementPrintViewModel> list = new List<CreditSettlementPrintViewModel>();

                
                if (!string.IsNullOrEmpty(creditSettlemetId))
                {

                    var currentRecord = db.CreditSettlement.FirstOrDefault(so => so.CreditSettlementId.ToString() == creditSettlemetId);

                    if (currentRecord != null)
                    {
                        var recordToAdd = new CreditSettlementPrintViewModel
                        {
                            VoucherNumber = currentRecord.CreditSettlementId,
                            ReceiptMethod = currentRecord.PaymentMethod.Method,
                            Date = currentRecord.Date,
                            Remark = currentRecord.Remark,
                            Customer = currentRecord.OrganizationCustomer.TradeName,
                            


                            PreparedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(currentRecord.CreatedBy)),
                            ApprovedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(currentRecord.OnlineApprovedBy)),
                            CheckedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(currentRecord.SendForApprovalBy)),
                        };

                        var lineItems = db.LineCreditSettlements.Where(l => l.CreditSettlementId == currentRecord.CreditSettlementId).ToList();
                        if (lineItems.Any())
                        {
                            recordToAdd.Amount = lineItems.Sum(l => l.SettelmentAmount);
                            recordToAdd.AmountInWords = NumberExtensions.toWords((int)recordToAdd.Amount);
                        }
                        foreach(var lineItem in lineItems)
                        {
                            var lineRecord = new CreditSettlementPrintLineItemViewModel
                            {
                                 Amount = lineItem.SettelmentAmount,
                                 InvoiceNumber = lineItem.InvoiceId,
                                 Witholding = lineItem.WithholdingAmount,
                                 NetAmount = lineItem.NetBalance
                            };
                            recordToAdd.LineItems.Add(lineRecord);
                        }
                        list.Add(recordToAdd);
                    }

                }
                return list;
            }
            catch (Exception e)
            {
            }
            return null;
        }
    }
}
