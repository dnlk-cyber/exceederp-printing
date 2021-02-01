using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.AR;
using ExceedERP.Core.Domain.Finance.AR.SalesOrder;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.Finance.ViewModel.Print;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.Finance.DataSource.Print
{
    [DataObject]
    public class ApSalesOrderOds
    {
        readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public ArSalesOrderHeaderVm ArSalesOrderHeader(int id)
        {
            var salesOrder = _db.ArSalesOrders.Find(id);
            var organization = _db.Companies.FirstOrDefault(x => x.Category == "0");

            ArSalesOrderHeaderVm headerDs = new ArSalesOrderHeaderVm();

            if (salesOrder != null)
            {
                headerDs.VoucherId = salesOrder.ArSalesOrderId.ToString();
                headerDs.Customer =salesOrder.CustomerName;
                headerDs.OrderDate = salesOrder.OrderDate;
                headerDs.Station = salesOrder.Station;
                var emp = _db.Person_Employee.FirstOrDefault(x => x.EmployeeId == salesOrder.SalesPersonId);
                if (emp != null)
                    headerDs.SalesPerson =
                        emp.FirstNameEnglish + " " + emp.MiddleNameEnglish + " " + emp.LastNameEnglish;
                var partner = _db.Partners.FirstOrDefault(x => x.PartnerId == salesOrder.PartnerId);
                if (partner != null)
                    headerDs.Partner = partner.Name;
                headerDs.DocTitile = salesOrder.OrderType==OrderType.Cash?"CASH SALES ORDER": "CREDIT SALES ORDER";
                headerDs.PrintStatus = salesOrder.IsReceiptPrinted ? "PRINTED" : "NOT PRINTED";
                headerDs.PaymentTerm = salesOrder.PaymentTerm;
                headerDs.OrderReference = salesOrder.OrderReference;
                headerDs.Product = salesOrder.Product;
                headerDs.Length = salesOrder.Length;
                headerDs.Repeatition = salesOrder.Repeatition;

                headerDs.OrderStatus = salesOrder.OrderStatus;
                headerDs.PreparedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(salesOrder.CreatedBy)); 
                headerDs.ApprovedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(salesOrder.OnlineApprovedBy));
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
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ArSalesOrderLineVm> ArSalesOrderLines(int id)
        {
            List<ArSalesOrderLineVm> list = new List<ArSalesOrderLineVm>();
            var invoiceLines = _db.ArSalesOrderLines.Where(x => x.ArSalesOrderId == id).ToList();
            foreach (var line in invoiceLines)
            {
                var entity = new ArSalesOrderLineVm
                {
                    ItemName =_db.ARSetupProductServiceAccounts.Find(int.Parse(line.ItemName))?.Product,
                    SubItem = _db.ArProductService.Find(int.Parse(line.SubItem))?.ItemName,
                    AccountCode = line.AccountCode,
                    AccountDesc = line.AccountDesc,
                    UoM = line.UoM,
                    Quantity = line.Quantity,
                    UnitPrice = line.UnitPrice,
                    SalesPrice = line.SalesPrice,
                    Discount = line.Discount,
                    Commission = line.Commission,
                    SubTotal = line.SubTotal,
                    TaxName = _db.GLTaxRates.Find(line.GLTaxRateID)?.TaxName,
                    TaxAmount = line.TaxAmount,
                    TotalAmount = line.TotalAmount,
                    Remark = line.Remark
                };
                list.Add(entity);
            }
            return list;
        }
    }
}
