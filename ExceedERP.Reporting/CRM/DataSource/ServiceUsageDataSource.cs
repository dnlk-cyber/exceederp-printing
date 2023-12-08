using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.CRM.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using ExceedERP.Helpers.Common;

namespace ExceedERP.Reporting.CRM.DataSource
{
    [DataObject]
    public class ServiceUsageDataSource
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ServiceUsageViewModel> GetServiceUsageList(string ServiceName, string customer, string Type,
            DateTime startingDate, DateTime endingDate)
        {
            List<ServiceUsageViewModel> ServiceUsages = new List<ServiceUsageViewModel>();





            //string AmharicName = "";
            //string Name = "";
            //string Address = "";
            //Image LogoFileName = null;
            //var organization = db.Companies.FirstOrDefault(x => x.Category == "0");

            //if (organization != null)
            //{
            //    Name = organization.TradeName;
            //    AmharicName = organization.AmharicName;
            //    var address =
            //        db.Addresses.Where(
            //            a =>
            //                a.Reference == organization.MainCompanyId.ToString() &&
            //                a.ReferenceType.ToLower() == "company").ToList();
            //    if (address.Any())
            //    {
            //        foreach (var a in address)
            //        {

            //            if (Address == null)
            //            {
            //                Address = a.Category + ":- " + a.Value;
            //            }
            //            else
            //            {
            //                Address += ", " + a.Category + "-" + a.Value;
            //            }

            //        }

            //    }
            //    var logo =
            //        db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
            //                a.ReferenceType == ExceedERP.Core.Domain.Common.AttachmentType.LOGO);
            //    if (logo != null)
            //    {
            //        LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
            //    }
            //}




            //var UsageList = db.ServiceUsages.Where(x => x.DateCreated > startingDate && x.DateCreated < endingDate).ToList();

            //if (UsageList.Any())
            //{
            //    foreach (var usage in UsageList)
            //    {
                   
            //        var LineItem = db.ServiceLineItems.Where(x => x.ServiceUsageId == usage.ServiceUsageId).ToList();
            //        var taxs = db.Taxs.ToList();
            //        if (LineItem.Any())
            //        {
            //            foreach (var Item in LineItem)
            //            {
            //                decimal TotalAmount = 0;
            //                var taxValue = taxs.Where(t => t.TaxID.ToString() == Item.Tax).FirstOrDefault();
            //                if (taxValue != null)
            //                {
            //                    TotalAmount = (Convert.ToDecimal(taxValue.Amount) * (Item.TotalAmount)/100) + Item.TotalAmount;

            //                }
            //                ServiceUsages.Add(new ServiceUsageViewModel
            //                {
            //                    Customer = usage.Customer,
            //                    GrandTotal = TotalAmount,
            //                    Type = Enum.GetName(Item.CustomerService.Type.GetType(), Item.CustomerService.Type),
            //                    ServiceName = Item.CustomerService.Description,
            //                    Remark = usage.Remark,
            //                 AmharicName = AmharicName,   Name = Name,  LogoFileName = LogoFileName, Address = Address});

            //            }
            //        }

            //    }
            //   if (!string.IsNullOrEmpty(customer))
            //    {
            //        ServiceUsages = ServiceUsages.Where(x => x.Customer == customer).ToList();
            //    }

            //   if (!string.IsNullOrEmpty(ServiceName))
            //   {
            //       ServiceUsages = ServiceUsages.Where(x => x.ServiceName == ServiceName).ToList();
            //   }


            //   if (!string.IsNullOrEmpty(Type))
            //   {
            //       ServiceUsages = ServiceUsages.Where(x => x.Type == Type).ToList();
            //   }
   
            //}

            return ServiceUsages;
        }


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<CustomerService> GetCustomerServiceList()
        {

            var result = db.CustomerServices.ToList();

            return result;

        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<OrganizationCustomer> GetCustomerList()
        {

            var result = db.OrganizationCustomers.ToList();

            return result;

        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<System.Web.Mvc.SelectListItem> GetServiceType()
        {


            var result = EnumExtension.EnumToSelectList(typeof(CRMEnums.ServiceType));

            return result;

        }
    }
}
