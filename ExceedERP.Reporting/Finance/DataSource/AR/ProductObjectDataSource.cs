using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Finance.ViewModel;

namespace ExceedERP.Reporting.Finance.DataSource.AR
{

    [DataObject]
    public class ProductObjectDataSource
    {
        readonly ExceedDbContext _db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ProductSoldToCustomerVm> GetProductSoldCustomer()
        {
            List<ProductSoldToCustomerVm> productList = new List<ProductSoldToCustomerVm>();
        
            var products = _db.ARSetupProductServiceAccounts.Select(x=>new {x.Product,x.ARSetupProductServiceAccountID}).ToList();
            foreach (var pro in products)
            {
                ProductSoldToCustomerVm product = new ProductSoldToCustomerVm
                {
                    ProductName = pro.Product,
                    ProductId = pro.ARSetupProductServiceAccountID
                };

                productList.Add(product);
            }

            return productList;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ProductSoldToCustomerVm> GetProductItem(string product)
        {
            List<ProductSoldToCustomerVm> productItemList = new List<ProductSoldToCustomerVm>();
            if(product!=null)
            {
                var productsItems = _db.ArProductService
                    .Where(x => x.ARSetupProductServiceAccountID.ToString() == product).Select(x=> new {x.ItemName,x.ArProductServiceId}).ToList();
                foreach (var items in productsItems)
                {
                    ProductSoldToCustomerVm productvm = new ProductSoldToCustomerVm
                    {
                        ProductName = items.ItemName,
                        ProductId = items.ArProductServiceId
                    };

                    productItemList.Add(productvm);
                }
            }
           

            return productItemList;
        }
    }
}
