 using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Reporting.CRM.ViewModel;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.Reporting.SharedDataSources;
using System.Drawing;

namespace ExceedERP.Reporting.CRM.Printing.DataSource
{
    [DataObject]
    public class SalesOrderObjectDataSource : List<OrderStatus>
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<Printing.ViewModel.OrderStatus> GetSalesOrderList(string salesOrderNumber)
        {
            List<Printing.ViewModel.OrderStatus> salesOrderViewList = new List<Printing.ViewModel.OrderStatus>();
            Printing.ViewModel.OrderStatus SalesOrderViewModel;
            List<SalesOrderVoucher> SalesOrderVouchers = new List<SalesOrderVoucher>();
            var product = db.Products.ToList();
            var orgCustomer = db.OrganizationCustomers.ToList();




            string AmharicName = "";
            string Name = "";
            string Address = "";
            Image LogoFileName = null;

            var organization = db.Companies.FirstOrDefault(x => x.Category == "0");

            if (organization != null)
            {
                Name = organization.TradeName;
                AmharicName = organization.AmharicName;
                var address =
                    db.Addresses.Where(
                        a =>
                            a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType.ToLower() == "company").ToList();
                if (address.Any())
                {
                    foreach (var a in address)
                    {

                        if (Address == null)
                        {
                            Address = a.Category + ":- " + a.Value;
                        }
                        else
                        {
                            Address += ", " + a.Category + "-" + a.Value;
                        }

                    }

                }
                var logo =
                    db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType == ExceedERP.Core.Domain.Common.AttachmentType.LOGO);
                if (logo != null)
                {
                    LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }





            if (!string.IsNullOrEmpty(salesOrderNumber))
            {

                SalesOrderVouchers = db.SalesOrderVouchers.Where(so => so.SalesOrderVoucherID.ToString() == salesOrderNumber).ToList();

            }

            if (SalesOrderVouchers.Any())
            {
                string address = "";
                string tradeName = "";

                tradeName = SalesOrderVouchers.FirstOrDefault().OrganizationCustomer.TradeName;

                var OrgCustomer = orgCustomer.Where(x => x.TradeName == tradeName).FirstOrDefault();
                if (OrgCustomer != null)
                {
                    var OrgCustomerID = OrgCustomer.OrganizationCustomerID;
                    var addresss =
                        db.Addresses.ToList()
                            .Where(
                                x =>
                                    x.Reference == OrgCustomerID.ToString() &&
                                    x.ReferenceType == "Organization_Customer")
                            .SingleOrDefault();
                    if (addresss != null)
                    {

                        address = addresss.Value;
                    }
                }
                foreach (SalesOrderVoucher sov in SalesOrderVouchers)
                {
                    List<LineItem> lineItems = db.LineItems.Where(li => li.Voucher == sov.SalesOrderVoucherID.ToString() && li.VoucherDefinitionID == "1").ToList();
                    foreach (LineItem l in lineItems)
                    {
                        string CustomerType = "CASH";
                        int uOMID = 0;
                        string uOM = "";
                        try
                        {
                            uOMID = int.Parse(l.Product.UOM);
                            uOM = db.Lookups.Where(x => x.LookupId == uOMID).FirstOrDefault().Description;
                        }
                        catch (Exception e)
                        {
                            uOM = string.Empty;
                        }
                        if (OrgCustomer.IsCredit)
                        {
                            CustomerType = "CREDIT";
                        }

                        SalesOrderViewModel = new Printing.ViewModel.OrderStatus();
                        SalesOrderViewModel.CustomerType = CustomerType;
                        SalesOrderViewModel.OrderNumber = sov.SalesOrderVoucherID.ToString();
                        SalesOrderViewModel.GrandTotal = sov.GrandTotal;
                        SalesOrderViewModel.OrderDate = sov.IssuedDate.ToString("dd/MM/yyyy");
                        SalesOrderViewModel.UOM = uOM;
                        SalesOrderViewModel.CustomerAddress = address;
                        SalesOrderViewModel.QuoteNumber = sov.References;
                        SalesOrderViewModel.OrderQuantity = l.CalculatedCost / l.UnitAmount;
                        SalesOrderViewModel.Status = sov.LastObjectState;
                        SalesOrderViewModel.Consignee = sov.OrganizationCustomer.TradeName;
                        SalesOrderViewModel.ProductName = l.Product.Name;
                        SalesOrderViewModel.Class = l.Class;
                        SalesOrderViewModel.Variety = l.Product.ItemCode;
                        SalesOrderViewModel.SalesPerson = sov.CreatedBy;
                        SalesOrderViewModel.PaymentMethod = sov.PaymentMethod;
                          SalesOrderViewModel.Store = l.SalesStore.Description;
                        SalesOrderViewModel.SellingPrice = l.UnitAmount;
                        SalesOrderViewModel.Tax = l.TaxAmount;
                        SalesOrderViewModel.SubTotoal = l.CalculatedCost;
                        SalesOrderViewModel.GrandTotal = l.TotalAmount;
                        SalesOrderViewModel.OnlineApprovedBy = sov.OnlineApprovedBy;
                        SalesOrderViewModel.SendForApprovalBy = sov.SendForApprovalBy;
                        SalesOrderViewModel.SendForApprovalTime = sov.SendForApprovalTime;
                        SalesOrderViewModel.OnlineApprovedTime = sov.OnlineApprovedTime;


                        SalesOrderViewModel.AmharicName = AmharicName;
                        SalesOrderViewModel.Name = Name;
                        SalesOrderViewModel.Address = Address;
                        SalesOrderViewModel.LogoFileName = LogoFileName;


                        SalesOrderViewModel.RequestedBy = sov.CreatedBy;
                        SalesOrderViewModel.CheckedBy = sov.OnlineApprovedBy;
                        SalesOrderViewModel.ApprovedBy = sov.OnlineApprovedBy;

                        salesOrderViewList.Add(SalesOrderViewModel);

                    }
                }
            }
            return salesOrderViewList;
        }


    }
}







