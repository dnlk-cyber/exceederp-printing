using ExceedERP.Core.Domain.PrintingEstimation;
using ExceedERP.Core.Domain.ProductionFollowUp;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExceedERP.Web.Areas.ProductionFollowUp.Models
{
    public class AddNewCustomer
    {
        private ExceedDbContext db = new ExceedDbContext();

        internal static Customer Save(Proforma proforma)
        {
            var customer = new Customer
            {
                Name = proforma.CustomerName,
                ContactName = proforma.ContactName,
                Phone = proforma.ContactPhone,
                City = proforma.City,
                SubCity = proforma.SubCity,
                Woreda = proforma.Woreda,
                Pobox = proforma.POBox,
                VAT = proforma.CustomerVAT,
                Margin = proforma.CustomerVAT,
                OverHead = proforma.CustomerOverHead,
                Description = proforma.CustomerDescription


            };
            return customer;
        }
        internal static Customer Add(Job job)
        {
            var customer = new Customer
            {
                Name = job.CustomerName,
                ContactName = job.ContactName,
                Phone = job.ContactPhone,
                City = job.City,
                SubCity = job.SubCity,
                Woreda = job.Woreda,
                Pobox = job.POBox,
                VAT = job.CustomerVAT,
                Margin = job.CustomerVAT,
                OverHead = job.CustomerOverHead,
                Description = job.CustomerDescription


            };
            return customer;
        }

        
    }
    }