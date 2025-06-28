using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.CRM.Tender.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ExceedERP.Reporting.CRM.Tender.DataSource
{
    [DataObject]

    public class TenderWinnerCustomerNameODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public WinnerCustomerNameVM GetSaleTenderWinnerName(string id,string Tin)
        {
            WinnerCustomerNameVM  Item = new WinnerCustomerNameVM();
            var FetchWinnerItems = db.Winners.FirstOrDefault(x=>x.TinNumber==Tin&&x.SalesLotId.ToString()==id);
            if(FetchWinnerItems != null)
            {
                    Item.CustomerName = FetchWinnerItems.CustomerName;
                
            }
           
            return Item;
        }
    }
    
}