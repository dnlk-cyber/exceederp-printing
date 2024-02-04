using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core.Domain.Printing.ProductionFollowUp;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExceedERP.Web.Areas.Printing.Models
{
    public class AddNewCustomer
    {
        private ExceedDbContext db = new ExceedDbContext();

        internal static OrganizationCustomer Save(Proforma proforma)
        {
                var customer = new OrganizationCustomer
            {
                TradeName = proforma.CustomerName,
                    BrandName = proforma.BrandName,
                    TinNo = proforma.TinNo,
                IsActive = true,
                IsCredit = false,
                IsTemporary = true,
                BusinessType="Private",
                Category="category",

                };
            return customer;
        }
        internal static OrganizationCustomer Add(Job job)
        {
            var customer = new OrganizationCustomer
            {
                TradeName = job.CustomerName,
                BrandName = job.BrandName,
                TinNo = job.TinNo,
                IsActive = true,
                IsCredit = false,
                IsTemporary = true,
                BusinessType = "Private",
                Category = "category",

            };
            return customer;
        }

        
    }
    }