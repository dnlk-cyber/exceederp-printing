using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Procurements.ViewModel;

namespace ExceedERP.Reporting.Procurements.DataSource
{
    

    [DataObject]
    public class SuppliersObjectDataSource
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SupplierListViewModel> GetSupplier()
        {
            List<SupplierListViewModel> supplierList = new List<SupplierListViewModel>();
            SupplierListViewModel supplier = new SupplierListViewModel();

            var proformas = db.ParticipatingSuppliers.ToList();
            foreach(var proforma in proformas)
            {
                supplier = new SupplierListViewModel();
                supplier.SupplierName = proforma.SupplierName;
                supplier.ID = proforma.SupplierId;
                supplier.Address = proforma.Address;

                supplierList.Add(supplier);
            }
         
            return supplierList;
        }
    }
    [DataObject]
    public class BidParticipantSupplierObjectDataSource
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SupplierListViewModel> GetTenderSupplier(string id)
        {
            List<SupplierListViewModel> supplierList = new List<SupplierListViewModel>();
            SupplierListViewModel supplier = new SupplierListViewModel();

            var tender = db.Tenders.FirstOrDefault(x=>x.TenderID.ToString()==id);
            if (tender != null)
            {
                var lot = db.Lots.FirstOrDefault(x => x.TenderID == tender.TenderID);

                var participantSuppliers = db.BidPParticipatingSupplierses.Where(x => x.LotID==lot.LotID).ToList();
                foreach (var participantSupplier in participantSuppliers)
                {
                    supplier = new SupplierListViewModel();

                    supplier.SupplierName = participantSupplier.SupplierName;
                    supplier.ID = participantSupplier.SupplierId;
                    supplier.Address = participantSupplier.Address;

                    supplierList.Add(supplier);
                }
            }
            return supplierList;
        }
    }
}
