using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.CRM.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;

namespace ExceedERP.Reporting.CRM.Printing.DataSource
{
    [DataObject]
    class ShipmentIssueDataSource
    {
        private readonly ExceedDbContext db = new ExceedDbContext();



        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ShipmentReportViewModel> GetShipmentData(string shipmentId)
        {
            try
            {
                List<ShipmentReportViewModel> shipments = new List<ShipmentReportViewModel>();

                //List<SalesShipment> existingShipmentData = new List<SalesShipment>();
                var OrgCustomers = db.OrganizationCustomers.ToList();
                var products = db.Products.ToList();

                string tradeName = "";
                string CompanyAddress = "";

                string customerAddress = null;

                string AmharicName = "";
                string Name = "";
                //string Address = "";

                HashSet<string> storeNames = new HashSet<string>();
                HashSet<string> shopNames = new HashSet<string>();
                HashSet<string> storeKeepers = new HashSet<string>();
                HashSet<int> shopAndStoreIds = new HashSet<int>();

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
                    //if (address.Any())
                    //{
                    //    foreach (var a in address)
                    //    {

                    //        if (Address == null)
                    //        {
                    //            Address = a.Category + ":- " + a.Value;
                    //        }
                    //        else
                    //        {
                    //            Address += ", " + a.Category + "-" + a.Value;
                    //        }

                    //    }

                    //}
                    //var logo =
                    //    db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                    //            a.ReferenceType == ExceedERP.Core.Domain.Common.AttachmentType.LOGO);
                    //if (logo != null)
                    //{
                    //    LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                    //}
                }




                if (!string.IsNullOrEmpty(shipmentId))
                {

                    var currentShipment = db.SalesShipments.FirstOrDefault(so => so.SalesShipmentId.ToString() == shipmentId);

                    if (currentShipment != null)
                    {
                        List<Address> customerAddresses = db.Addresses.Where(x => x.Reference == currentShipment.OrganizationCustomerId.ToString() && x.ReferenceType == "Organization_Customer").ToList();
                        if (customerAddresses.Any())
                        {
                            foreach (var a in customerAddresses)
                            {
                                if (customerAddress == null)
                                {
                                    customerAddress = a.Category + ":- " + a.Value;
                                }
                                else
                                {
                                    customerAddress += ", " + a.Category + "-" + a.Value;
                                }
                            }
                        }
                        var branch = db.Branch.Find(currentShipment.BranchId);
                        var customer = db.OrganizationCustomers.Find(currentShipment.OrganizationCustomerId);
                        //var assignmentRecord = db.SalesPersonAssignments
                        //                .FirstOrDefault(sa => sa.SalesStoreId == currentShipment.StoreID &&
                        //                sa.UserName == currentShipment.OnlineApprovedBy);
                        var shipmentRecordToAdd = new ShipmentReportViewModel
                        {
                            Id = currentShipment.SalesShipmentId,
                            Branch = branch?.Name,
                            Customer = customer?.TradeName,
                            CustomerAddress = customerAddress,
                            CustomerType = GetCustomerCategory(customer?.Category),
                            MethodOfShipment = currentShipment.MethodOfShipment,
                            TruckPlateNumber = currentShipment.PlateNo,
                            ShipmentDate = currentShipment.ShipmentDate.ToString("dd/MM/yyyy"),
                            ShipmentNumber = currentShipment.SalesShipmentId,
                            Source = currentShipment.Source.ToString(),
                            SalesOrderOrInvoiceNumber = currentShipment.Source == ShipmentSource.SalesInvoice ? currentShipment.CashSalesInvoiceID : currentShipment.SalesOrderVoucherID,
                            ShipmentStatus = currentShipment.Status,
                            TrailerPlateNumber = currentShipment.TrailerPlateNo,

                            RequestedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(currentShipment.CreatedBy)),
                            DateOfRequest = currentShipment.DateCreated?.ToString("dd/MM/yyyy"),
                            ApprovedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(currentShipment.OnlineApprovedBy)),
                            IssuedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(currentShipment.OnlineApprovedBy)),
                            DateOfApproval = currentShipment.OnlineApprovedTime != null ? currentShipment.OnlineApprovedTime.Value.ToString("dd/MM/yyyy") : "",
                            DateOfReceive = currentShipment.OnlineApprovedTime != null ? currentShipment.OnlineApprovedTime.Value.ToString("dd/MM/yyyy") : ""
                        };
                        if(currentShipment.Source == ShipmentSource.MaintenanceStoreRequisition)
                        {
                            var sReq = db.StoreRequisitions.FirstOrDefault(r => r.StoreRequisitionID.ToString() == currentShipment.StoreRequisitionNumber);
                            if(sReq != null)
                            {
                                shipmentRecordToAdd.DateOfRequest = sReq.RequestDate.ToString("dd/MM/yyyy");
                                var employee = db.Person_Employee.Find(sReq.EmployeeId);
                                if(employee != null)
                                {
                                    shipmentRecordToAdd.RequestedBy = $"{employee.FirstNameEnglish} {employee.MiddleNameEnglish} {employee.LastNameEnglish}";
                                }
                            }
                        }
                        

                        var lineItems = db.SalesShipmentLineItem.Where(li => li.SalesShipmentId == currentShipment.SalesShipmentId).ToList();
                        if (lineItems.Any())
                        {
                            foreach (SalesShipmentLineItem shipmentLineItem in lineItems)
                            {
                                var lineItemShopOrStore = db.SalesStores.Find(shipmentLineItem.SalesStoreId);
                                if (lineItemShopOrStore != null)
                                {
                                    var storeAssigments = db.SalesPersonAssignments
                                        .Where(sa => sa.SalesStoreId == lineItemShopOrStore.SalesStoreId &&
                                        sa.UserName == currentShipment.CreatedBy).ToList();
                                    if (storeAssigments.Any())
                                    {
                                        foreach (var asg in storeAssigments)
                                        {
                                            storeKeepers.Add(asg.FullName);
                                        }
                                    }
                                    var product = db.Products.Find(shipmentLineItem.ProductId);
                                    if (product != null && lineItemShopOrStore != null)
                                    {
                                        var productCategory = db.ProductCategories.FirstOrDefault(c => c.ProductCategoryId.ToString() == product.CategoryID);
                                        if (lineItemShopOrStore.Type == SalesStoreType.Shop)
                                        {
                                            shopNames.Add(lineItemShopOrStore.Description);
                                        }
                                        else
                                        {
                                            storeNames.Add(lineItemShopOrStore.Description);
                                        }
                                        var lineItemRecord = new ShipmentLineItemReportViewModel
                                        {
                                            ProductName = product.Name,
                                            UoM = product.UOM,
                                            InvoiceQuantity = shipmentLineItem.TotalQuantity,
                                            IssuedQuantity = shipmentLineItem.ShipedQuantity,
                                            UnitPrice = shipmentLineItem.Price,
                                            RemainingQuantity = shipmentLineItem.RemainingQuantity,
                                            Category = productCategory?.Description,
                                            ItemCode = product.ItemCode,
                                            Variety = shipmentLineItem.Variety,
                                            Class = shipmentLineItem.Class

                                        };
                                        lineItemRecord.TotalInvoiceAmount = lineItemRecord.InvoiceQuantity * lineItemRecord.UnitPrice;
                                        lineItemRecord.TotalIssuedAmount = lineItemRecord.IssuedQuantity * lineItemRecord.UnitPrice;
                                        lineItemRecord.TotalRemainingAmount = lineItemRecord.RemainingQuantity * lineItemRecord.UnitPrice;

                                        shipmentRecordToAdd.LineItem.Add(lineItemRecord);
                                    }

                                }
                            }
                            var concatnatedShop = "";
                            foreach (var n in shopNames)
                            {
                                concatnatedShop += " " + n;
                            }

                            var concatnatedStore = "";
                            foreach (var n in storeNames)
                            {
                                concatnatedStore += " " + n;
                            }

                            shipmentRecordToAdd.Store = concatnatedStore;
                            shipmentRecordToAdd.Shop = concatnatedShop;

                            var concatnatedKeepersName = "";
                            foreach(var n in storeKeepers)
                            {
                                if (string.IsNullOrEmpty(concatnatedKeepersName))
                                {
                                    concatnatedKeepersName = n;
                                }
                                else
                                {
                                    concatnatedKeepersName += ", " + n;
                                }
                            }
                            shipmentRecordToAdd.StoreKeeperName = concatnatedKeepersName;
                            shipments.Add(shipmentRecordToAdd);

                        }

                    }
                    return shipments;
                }
            }
            catch (Exception e)
            {
            }

            return null;

        }


        //[DataObjectMethod(DataObjectMethodType.Select)]
        //public List<SalesInventory> GetSalesinvoiceHeader(string invoiceNumber)
        //{
        //    try
        //    {
        //        List<CashSales> CashSalesInvoice = new List<CashSales>();
        //        SalesInventory SalesInvoiceViewModel;
        //        List<SalesInventory> StoreIssues = new List<SalesInventory>();

        //        List<CashSalesInvoice> CashSalesInvoices = new List<CashSalesInvoice>();
        //        var OrgCustomers = db.OrganizationCustomers.ToList();
        //        var product = db.Products.ToList();





        //        string tradeName = "";
        //        string CompanyAddress = "";

        //        string AmharicName = "";
        //        string Name = "";
        //        string Address = "";
        //        Image LogoFileName = null;
        //        var organization = db.Companies.FirstOrDefault(x => x.Category == "0");

        //        if (organization != null)
        //        {
        //            Name = organization.TradeName;
        //            AmharicName = organization.AmharicName;
        //            var address =
        //                db.Addresses.Where(
        //                    a =>
        //                        a.Reference == organization.MainCompanyId.ToString() &&
        //                        a.ReferenceType.ToLower() == "company").ToList();
        //            if (address.Any())
        //            {
        //                foreach (var a in address)
        //                {

        //                    if (Address == null)
        //                    {
        //                        Address = a.Category + ":- " + a.Value;
        //                    }
        //                    else
        //                    {
        //                        Address += ", " + a.Category + "-" + a.Value;
        //                    }

        //                }

        //            }
        //            var logo =
        //                db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
        //                        a.ReferenceType == ExceedERP.Core.Domain.Common.AttachmentType.LOGO);
        //            if (logo != null)
        //            {
        //                LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
        //            }
        //        }




        //        if (!string.IsNullOrEmpty(invoiceNumber))
        //        {

        //            CashSalesInvoices = db.CashSalesInvoices.Where(so => so.CashSalesInvoiceID.ToString() == invoiceNumber).ToList();

        //        }

        //        List<CashSalesInvoice> SalesInvoices = new List<CashSalesInvoice>();

        //        if (CashSalesInvoices != null)
        //        {
        //            SalesInvoices = CashSalesInvoices;


        //            string address = "";

        //            if (SalesInvoices.Any())
        //            {
        //                tradeName = SalesInvoices.FirstOrDefault().OrganizationCustomer.TradeName;

        //                var OrgCustomer = OrgCustomers.Where(x => x.TradeName == tradeName).FirstOrDefault();
        //                if (OrgCustomer != null)
        //                {
        //                    var OrgCustomerID = OrgCustomer.OrganizationCustomerID;
        //                    var addresss =
        //                        db.Addresses.ToList()
        //                            .Where(
        //                                x =>
        //                                    x.Reference == OrgCustomerID.ToString() &&
        //                                    x.ReferenceType == "Organization_Customer")
        //                            .SingleOrDefault();
        //                    if (addresss != null)
        //                    {

        //                        address = addresss.Value;
        //                    }
        //                }
        //                CashSalesInvoice sov = SalesInvoices.FirstOrDefault();

        //                string CustomerType = "CASH";
        //                int Index = 0;

        //                List<LineItem> lineItems = db.LineItems.Where(li => li.Voucher.ToString() == sov.CashSalesInvoiceID.ToString() && li.VoucherDefinitionID == "2").ToList();
        //                foreach (LineItem l in lineItems)
        //                {
        //                    int uOMID = 0;
        //                    string uOM = "";

        //                    try
        //                    {
        //                        uOMID = int.Parse(l.Product.UOM);
        //                        uOM = db.Lookups.Where(x => x.LookupId == uOMID).FirstOrDefault().Description;
        //                    }
        //                    catch (Exception e)
        //                    {
        //                        uOM = string.Empty;
        //                    }

        //                    var history = db.IssueHistorys.Where(x => x.InvoiceId == sov.CashSalesInvoiceID && x.Remark == l.ThingID.ToString()).ToList();
        //                    if (OrgCustomer.IsCredit)
        //                    {
        //                        CustomerType = "CREDIT";
        //                        Address adr = db.Addresses.Where(x => x.Reference == OrgCustomer.OrganizationCustomerID.ToString() && x.ReferenceType == "Organization_Customer").FirstOrDefault();

        //                        if (adr != null)
        //                        {
        //                            CompanyAddress = adr.Value;
        //                        }
        //                    }



        //                    SalesInvoiceViewModel = new SalesInventory();
        //                    SalesInvoiceViewModel.Consignee = sov.OrganizationCustomer.TradeName;
        //                    SalesInvoiceViewModel.CustomerAddress = address;
        //                    SalesInvoiceViewModel.OnlineApprovedBy = sov.CreatedBy;

        //                    SalesInvoiceViewModel.AmharicName = AmharicName;
        //                    SalesInvoiceViewModel.Name = Name;
        //                    SalesInvoiceViewModel.LogoFileName = LogoFileName;
        //                    SalesInvoiceViewModel.Address = Address;

        //                    StoreIssues.Add(SalesInvoiceViewModel);

        //                }
        //            }





        //            return StoreIssues.ToList();
        //        }
        //        else
        //        {
        //            return new List<SalesInventory>();
        //        }
        //    }
        //    catch (Exception e)
        //    {


        //    }

        //    return null;

        //}


            private string GetCustomerCategory(string catId)
        {
            return db.OrganizationCategories.FirstOrDefault(c => c.OrganizationCategoryID.ToString() == catId)?.Description;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public ShipmentReportHeaderViewModel GetReportHeaderData()
        {
            var db = new ExceedDbContext();

            var headerVM = new ShipmentReportHeaderViewModel();
            string tradeName = "";

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

            headerVM.AmharicName = AmharicName;
            headerVM.Name = Name;

            headerVM.LogoFileName = LogoFileName;
            headerVM.Address = Address;
            return headerVM;
        }
    }
}
