﻿using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core;
﻿using ExceedERP.DataAccess;
﻿using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.CRM.Models;
using ExceedERP.Helpers.Common;  

namespace ExceedERP.Web.Areas.CRM.Controllers
{
       [AuthorizeRoles(CRMRoles.CRMAdmin)] 
    public class OrganizationCustomerController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            var periodList = db.Periods;
            var accountList = db.Accounts;
            var bankList = db.Banks;
            var organizationCategory = db.OrganizationCategories;
            ViewData["PeriodList"] = periodList.AsQueryable();
            ViewData["AccountList"] = accountList.AsQueryable();
            ViewData["BankList"] = bankList.AsQueryable();
            ViewData["OrganizationCategory"] = organizationCategory.AsQueryable();
            return View();
        }

        public ActionResult OrganizationCustomers_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<OrganizationCustomer> organizationcustomers = db.OrganizationCustomers.Where(x=> x.IsTemporary == false);
            DataSourceResult result = organizationcustomers.ToDataSourceResult(request, organizationCustomer => new
            {
                OrganizationCustomerID = organizationCustomer.OrganizationCustomerID,
                TradeName = organizationCustomer.TradeName,
                BrandName = organizationCustomer.BrandName,
                EsclationLevel = organizationCustomer.EsclationLevel,
                BusinessType = organizationCustomer.BusinessType,
                Category = organizationCustomer.Category,
                IsActive = organizationCustomer.IsActive,
                CustomerType = organizationCustomer.CustomerType,
                CreatedBy = organizationCustomer.CreatedBy,
                DateCreated = organizationCustomer.DateCreated,
                LastModified = organizationCustomer.LastModified,
                ModifiedBy = organizationCustomer.ModifiedBy,
                OrgBranchName = organizationCustomer.OrgBranchName,
                IsCredit = organizationCustomer.IsCredit
            });

            return Json(result);
        }
        public ActionResult OrganizationTinNo_Read([DataSourceRequest]DataSourceRequest request ,int Id)
        {
            IQueryable<OrganizationCustomer> organizationcustomers = db.OrganizationCustomers.Where(x=>x.OrganizationCustomerID == Id);
            DataSourceResult result = organizationcustomers.ToDataSourceResult(request, organizationCustomer => new
            {
                Id = organizationCustomer.OrganizationCustomerID,
                Name = organizationCustomer.TinNo,
                
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OrganizationTinNo_Create([DataSourceRequest]DataSourceRequest request, BindingModel organizationCustomer, int Id)
        {
           var  Customer = db.OrganizationCustomers.Where(tc => tc.OrganizationCustomerID ==Id ).FirstOrDefault();

           Customer.TinNo = organizationCustomer.Name;

            db.OrganizationCustomers.Attach(Customer);
            db.Entry(Customer).State = EntityState.Modified;
            db.SaveChanges();
                 
            return Json(new[] { organizationCustomer }.ToDataSourceResult(request, ModelState));

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OrganizationCustomers_Create([DataSourceRequest]DataSourceRequest request, OrganizationCustomer organizationCustomer)
        {
            List<OrganizationCustomer> organizationCustomerList = db.OrganizationCustomers.Where(tc => tc.TradeName == organizationCustomer.TradeName).ToList();
            List<OrganizationCustomer> organizationCustomerLists = db.OrganizationCustomers.Where(tc => tc.BrandName == organizationCustomer.BrandName).ToList();
            if (organizationCustomerList.Any())
            {
                ModelState.AddModelError("DuplicateIndexEntry", " Trade Name Already exist !");
            }
            else if (organizationCustomerLists.Any())
            {
                ModelState.AddModelError("DuplicateIndexEntry", " Brand Name Already exist !");
            }

            else
            {
                if (ModelState.IsValid)
                {
                    var entity = new OrganizationCustomer
                    {
                        TradeName = organizationCustomer.TradeName,
                        BrandName = organizationCustomer.BrandName,
                        BusinessType = organizationCustomer.BusinessType,
                        Category = organizationCustomer.Category,
                        IsActive = organizationCustomer.IsActive,
                        EsclationLevel = organizationCustomer.EsclationLevel,
                        CustomerType = organizationCustomer.CustomerType,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now,
                        IsCredit = organizationCustomer.IsCredit

                    };

                    db.OrganizationCustomers.Add(entity);
                    db.SaveChanges();
                    organizationCustomer.OrganizationCustomerID = entity.OrganizationCustomerID;
                }
            }
            return Json(new[] { organizationCustomer }.ToDataSourceResult(request, ModelState));

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OrganizationCustomers_Update([DataSourceRequest]DataSourceRequest request, OrganizationCustomer organizationCustomer)
        {
            if (ModelState.IsValid)
            {
                var entity = new OrganizationCustomer
                {
                    OrganizationCustomerID = organizationCustomer.OrganizationCustomerID,
                    TradeName = organizationCustomer.TradeName,
                    BrandName = organizationCustomer.BrandName,
                    BusinessType = organizationCustomer.BusinessType,
                    CustomerType = organizationCustomer.CustomerType,
                    EsclationLevel = organizationCustomer.EsclationLevel,
                    Category = organizationCustomer.Category,
                    IsActive = organizationCustomer.IsActive,
                    IsCredit = organizationCustomer.IsCredit

                };

                db.OrganizationCustomers.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { organizationCustomer }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OrganizationCustomers_Destroy([DataSourceRequest]DataSourceRequest request, OrganizationCustomer organizationCustomer)
        {
            if (ModelState.IsValid)
            {
                var entity = new OrganizationCustomer
                {
                    OrganizationCustomerID = organizationCustomer.OrganizationCustomerID,
                    TradeName = organizationCustomer.TradeName,
                    BrandName = organizationCustomer.BrandName,
                    BusinessType = organizationCustomer.BusinessType,
                    Category = organizationCustomer.Category,
                    EsclationLevel = organizationCustomer.EsclationLevel,
                    IsActive = organizationCustomer.IsActive,
                    CustomerType = organizationCustomer.CustomerType,
                    IsCredit = organizationCustomer.IsCredit
                };

                db.OrganizationCustomers.Attach(entity);
                db.OrganizationCustomers.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Organization Customer number " + organizationCustomer.OrganizationCustomerID);
      
            }

            return Json(new[] { organizationCustomer }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult OrgCustomerDDBind()
        {

            var customers = db.OrganizationCustomers.Where(o=>o.IsActive==true).Select(s => new OrganizationCustomerViewModel
            {
                TradeName = s.TradeName,
                OrganizationCustomerID = s.OrganizationCustomerID
            });

            return Json(customers, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CashOrgCustomerDDBind()
        {

            var customers = db.OrganizationCustomers.Where(o => o.IsActive == true && o.IsTemporary == false).Select(s => new OrganizationCustomerViewModel
            {
                TradeName = s.TradeName,
                OrganizationCustomerID = s.OrganizationCustomerID
            }).ToList();

            customers.Insert(0,new OrganizationCustomerViewModel { TradeName = "New Customer", OrganizationCustomerID = CRMConstants.NewCustomerId });

            return Json(customers, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CreditOrgCustomerDDBind()
        {

            var customers = db.OrganizationCustomers.Where(o => o.IsActive == true && o.IsCredit == true && o.IsTemporary == false).Select(s => new OrganizationCustomerViewModel
            {
                TradeName = s.TradeName,
                OrganizationCustomerID = s.OrganizationCustomerID
            });

            return Json(customers, JsonRequestBehavior.AllowGet);
        }

        public ActionResult OrgCustomerCreditDDBind()
        {
            var customers = db.CashSalesInvoices.Where(o => o.PaymentMethod == "Credit").ToList();
            List<OrganizationCustomerViewModel> OrgCust = new List<OrganizationCustomerViewModel>();
            foreach (var customer in customers)
            {
                if (OrgCust.Where(x => x.TradeName == customer.OrganizationCustomer.TradeName).FirstOrDefault() == null)
                {
                    OrgCust.Add(new OrganizationCustomerViewModel
                       {
                           TradeName = customer.OrganizationCustomer.TradeName,
                           OrganizationCustomerID = customer.CashSalesInvoiceID
                       });

                }
            }
            return Json(OrgCust, JsonRequestBehavior.AllowGet);
        }

        public ActionResult OrgCustomerCreditSettleDDBind()
        {
            var customers = db.CashSalesInvoices.Where(o => o.PaymentMethod == "Credit").ToList();
            List<OrganizationCustomerViewModel> OrgCust = new List<OrganizationCustomerViewModel>();
            foreach (var customer in customers)
            {
                if (OrgCust.Where(x => x.TradeName == customer.OrganizationCustomer.TradeName).FirstOrDefault() == null)
                {
                    OrgCust.Add(new OrganizationCustomerViewModel
                    {
                        TradeName = customer.OrganizationCustomer.TradeName,
                        OrganizationCustomerID = customer.OrganizationCustomerId
                    });

                }
            }
            return Json(OrgCust, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PaymentMethodByCustomerDDBind(string customerId)
        {

            var customer = db.OrganizationCustomers.Where(x => x.OrganizationCustomerID.ToString() == customerId).FirstOrDefault();
            List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject> paymentMethod = new List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject>();

            if (customer.IsCredit)
            {
                paymentMethod.Add(new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject { Name = "CREDIT", Id = 1 });
                paymentMethod.Add(new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject { Name = "CASH", Id = 2 });
               
            }
            else
            {
                paymentMethod.Add(new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject { Name = "CASH", Id = 1 });
            }

            return Json(paymentMethod, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PaymentMethodDDBind()
        {

            List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject > paymentMethod = new List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject >();
            paymentMethod.Add(new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject  { Name = "CASH", Id = 1 });
            paymentMethod.Add(new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject  { Name = "CREDIT", Id = 2 });
            return Json(paymentMethod, JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
