using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.Procurements.Print.VM;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.Procurements.Print.Print
{
    [DataObject]
    public class ForeignPurchasePaymentOds
    {
        ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public ForeignPurchaseCostVm GetForeignPurchasePayment(int id)
        {
            var foreignPurchaseCost = _db.ForeignPurchaseCosts.Find(id);
            var organization = _db.Companies.FirstOrDefault(x => x.Category == "0");

            ForeignPurchaseCostVm headerDs = new ForeignPurchaseCostVm();
            if (foreignPurchaseCost != null)
            {
                headerDs.VoucherId = foreignPurchaseCost.ForeignPurchaseCostId.ToString();
                headerDs.From = "";
                headerDs.To = "";
                headerDs.Payee = foreignPurchaseCost.Payee;
                headerDs.CostType = foreignPurchaseCost.CostType;
                headerDs.Amount = foreignPurchaseCost.Amount;
                headerDs.Currency = foreignPurchaseCost.Currency;
                headerDs.ExchangeRate = foreignPurchaseCost.ExchangeRate;
                headerDs.ForexAmount = foreignPurchaseCost.ForexAmount;

                headerDs.CreatedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(foreignPurchaseCost.CreatedBy));
                if (foreignPurchaseCost.DateCreated != null)
                    headerDs.CreateDate = foreignPurchaseCost.DateCreated != null ? foreignPurchaseCost.DateCreated.ToString() : "";
                headerDs.Approver =
                    EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(foreignPurchaseCost.OnlineApprovedBy));
                headerDs.ApprovedDate = foreignPurchaseCost.OnlineApprovedTime != null ? foreignPurchaseCost.OnlineApprovedTime.ToString() : "";




            }


            if (organization != null)
            {
                headerDs.Name = organization.TradeName;
                headerDs.AmharicName = organization.AmharicName;
                var address =
                    _db.Addresses.Where(
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
                    _db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                                                              a.ReferenceType == AttachmentType.LOGO);
                if (logo != null)
                {
                    headerDs.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }
            return headerDs;
        }
       
       
    }
}