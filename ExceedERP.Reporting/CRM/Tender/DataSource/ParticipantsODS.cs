using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.CRM.Tender.DataSource
{
    [DataObject]
    public class ParticipantsODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ParticipantsVM> GetParticipants(string id)
        {
            List<ParticipantsVM> list = new List<ParticipantsVM>();
            List<LinedCustomers> customersList = new List<LinedCustomers>();
            var SelectParticipantItems = db.SalesLotParticipantItems.ToList();
            var SelectSalesLotItems = db.SalesLotItems.Where(x => x.SalesLotId.ToString() == id).ToList();
            List<int?> ItemIds = new List<int?>();
            ItemIds = SelectSalesLotItems.Select(x => x.ProductID).ToList();
            SelectParticipantItems = SelectParticipantItems.Where(x => ItemIds.Contains(x.ProductID)).ToList();
            var GroupedData = SelectParticipantItems.GroupBy(x => x.ProductID);
            foreach (var group in GroupedData)
            {
                foreach(var g in group)
                {

                    var LinedCustomer = new LinedCustomers();
                    var CheckIfSameLot = db.SalesLotParticipants.FirstOrDefault(x => x.SalesLotParticipantId == g.SalesLotParticipantId);
                    if(CheckIfSameLot!=null)
                    {
                        var CheckLot = db.SalesLots.FirstOrDefault(x => x.SalesLotId == CheckIfSameLot.SalesLotId);
                        if(CheckLot!=null)
                        {
                            LinedCustomer.CustomerNamee = CheckIfSameLot.CustomerName;
                            LinedCustomer.UnitPricee = g.UnitPrice;
                            customersList.Add(LinedCustomer);
                        }
                    }
                   
                }
                var FirstElement = group.First();
                var ItemCategory = new ParticipantsVM();
                ItemCategory.ItemName = db.Products.FirstOrDefault(x => x.ProductID == FirstElement.ProductID)?.Name??" ";
                ItemCategory.Unit= db.Products.FirstOrDefault(x => x.ProductID == FirstElement.ProductID)?.UOM??" ";
                ItemCategory.Quantity = FirstElement.Quantity;
               foreach(var cusName in customersList)
               {
                    ItemCategory.CustormerName = cusName.CustomerNamee;
                    ItemCategory.UnitPrice = cusName.UnitPricee;
                    list.Add(ItemCategory);

                }
            }
            return list;
        }
    }

    public class ParticipantsVM
    {
        public string ItemName { get; set; }
        public string Unit { get; set; }
        public decimal Quantity { get; set; }
        public string CustormerName { get; set; }
        public decimal UnitPrice { get; set; }
    }
    public class LinedCustomers
    {
        public string CustomerNamee { get; set; }
        public decimal UnitPricee { get; set; }
    }
}
