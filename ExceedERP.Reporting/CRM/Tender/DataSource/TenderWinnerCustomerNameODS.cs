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

    public class TenderReferenceODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public TenderReferenceVM GetSaleTenderReference(string id)
        {
            TenderReferenceVM Item = new TenderReferenceVM();
            var FetchLot = db.SalesLots.FirstOrDefault(x => x.SalesLotId.ToString() == id);
            if(FetchLot!=null)
            {
                var FetchReference = db.SaleTenders.FirstOrDefault(x => x.SaleTenderId== FetchLot.SaleTenderId);
                if(FetchReference!=null)
                {
                    Item.TenderReference = FetchReference.TenderReference;
                }

            }
            
            

            return Item;
        }
    }
    public class TenderReferenceVM
    {
        public string TenderReference { get; set; }
    }
}