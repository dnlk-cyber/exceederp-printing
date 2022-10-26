using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.CRM.ViewModel;

namespace ExceedERP.Reporting.CRM.DataSource
{
    [DataObject]
    public class ParametersOds
    {
        private readonly ExceedDbContext db = new  ExceedDbContext();
        //private readonly ApplicationDbContext
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<object> GetBranches()
        {
            List<object> list = new List<object>();
            var currentUser = HttpContext.Current.User;
            //var userBranches = new ApplicationBranchManager().GetUsersBranchByName(currentUser.Identity.Name);
            //var userBranch = db.UserBranches.Where(x => x.ApplicationUserId == user.Id);
            var allBranches = db.Branch.ToList();
            foreach (Branch branch in allBranches)
            {
                var record = new 
                {
                    Id = branch.BranchId,
                    Name = branch.Name
                };
                list.Add(record);
            }
            return list;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<object> GetStores(string branchId)
        {
            List<object> list = new List<object>();
            var currentUser = HttpContext.Current.User;
            var stores = db.SalesStores.Where(s => s.InventoryStore.BranchId.ToString() == branchId).ToList();
            foreach (SalesStore store in stores)
            {
                var record = new
                {
                    Id = store.SalesStoreId,
                    Name = store.Description
                };
                list.Add(record);
            }
            return list;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<object> GetCustomers()
        {
            List<object> list = new List<object>();
            //var currentUser = HttpContext.Current.User;
            var customers = db.OrganizationCustomers.Where(s => !s.IsTemporary).ToList();
            foreach (OrganizationCustomer customer in customers)
            {
                var record = new
                {
                    Id = customer.OrganizationCustomerID,
                    Name = customer.TradeName
                };
                list.Add(record);
            }
            return list;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<object> GetCustomersFromTenderFollowup()
        {
            List<object> list = new List<object>();
            var ids = new HashSet<int>(db.TenderFollowup.Select(t => t.CustomerId));
            var customers = db.OrganizationCustomers.Where(s => !s.IsTemporary && ids.Contains(s.OrganizationCustomerID)).ToList();
            foreach (OrganizationCustomer customer in customers)
            {
                var record = new
                {
                    Id = customer.OrganizationCustomerID,
                    Name = customer.TradeName
                };
                list.Add(record);
            }
            return list;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<object> GetSalesPersonInSaless()
        {
            List<object> list = new List<object>();
            //var currentUser = HttpContext.Current.User;
            var users = new HashSet<string>(db.CashSalesInvoices.Where(c => !string.IsNullOrEmpty(c.InvoicePrintedBy)).Select(c => c.InvoicePrintedBy));
            //var customers = db.OrganizationCustomers.Where(s => !s.IsTemporary).ToList();
            foreach (var u in users)
            {
                var record = new
                {
                    Id = u,
                    Name = u
                };
                list.Add(record);
            }
            return list;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<object> GetSalesInvoiceStatus()
        {
            List<object> list = new List<object>();
            var printed = new { Name = "Printed" };
            var hold = new { Name = "Hold" };
            var forwarded = new { Name = "Forwarded" };
            list.Add(forwarded);
            list.Add(hold);
            list.Add(printed);
            return list;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<object> GetFiscalYears()
        {
            List<object> list = new List<object>();
            var yearsList = db.GLFiscalYears.ToList();
            foreach (var year in yearsList)
            {
                list.Add(new { Id = year.GlFiscalYearId, Name = year.Name });
            }
            return list;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<object> GetCRMPeriodsInYear(string yearId)
        {
            List<object> list = new List<object>();
            var currentYear = db.GLFiscalYears.FirstOrDefault(y => y.GlFiscalYearId.ToString() == yearId);
            if(currentYear != null)
            {
                var periods = db.CRMPeriods.Where(p => p.DateTo <= currentYear.DateTo && p.DateFrom >= currentYear.DateFrom).ToList();
                foreach (var period in periods)
                {
                    list.Add(new { Id = period.CRMPeriodId, Name = period.Name });
                }
            }
            return list;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<object> GetProductCategories()
        {
            List<object> list = new List<object>();
            var categories = db.ProductCategories.ToList();
            foreach (var category in categories)
            {
                list.Add(new { Id = category.ProductCategoryId, Name = category.Description });
            }
            return list;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<object> GetProducts(string productCategoryId)
        {
            List<object> list = new List<object>();
            var products = db.Products.Where(p => p.CategoryID == productCategoryId).ToList();
            foreach (var product in products)
            {
                list.Add(new { Id = product.ProductID, Name = product.Name });
            }
            return list;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<object> GetReceiptCategories()
        {
            List<object> list = new List<object>();
            var fiscalCash = new
            {
                Id = ReceiptCategory.CashSalesFiscalReceipt,
                Name = ReceiptCategory.CashSalesFiscalReceipt
            };
            list.Add(fiscalCash);
            var manualCash = new
            {
                Id = ReceiptCategory.CashSalesManualReceipt,
                Name = ReceiptCategory.CashSalesManualReceipt
            };
            list.Add(manualCash);
            var fiscalCredit = new
            {
                Id = ReceiptCategory.CreditSalesFiscalReceipt,
                Name = ReceiptCategory.CreditSalesFiscalReceipt
            };
            list.Add(fiscalCredit);
            var manualCredit = new
            {
                Id = ReceiptCategory.CreditSalesManualReceipt,
                Name = ReceiptCategory.CreditSalesManualReceipt
            };
            list.Add(manualCredit);
            return list;
        }

        /// <summary>
        /// Job order numbers from maintenance 
        /// referenced in shipment (indirect reference from Store requision no.)
        /// </summary>
        /// <returns></returns>
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<object> GetJobOrdersNumbers()
        {
            List<object> list = new List<object>();
            //var numbers = new HashSet<string>(db.StoreRequisitions.Where(sr => sr.IsForMaintenance && sr.IsOnlineApproved).Select(sr => sr.JobOrderNo));
            //foreach (var num in numbers)
            //{
            //    list.Add(new
            //    {
            //        Id = num,
            //        Name = num
            //    });
            //}
            return list;
        }
    }
}
