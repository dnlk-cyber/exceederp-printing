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
    public class SaleTenderWinnerODS
    {
        ExceedDbContext db = new ExceedDbContext();


        [DataObjectMethod(DataObjectMethodType.Select)]
        public SaleTenderVM GetSaleTenderWinner(int id)
        {
            
            var model = new SaleTenderVM();
            var SaleTenderReciept = db.SalesLots.Find(id);
            int salesLot = id;
           // int salesLot = int.Parse(id);
            if (SaleTenderReciept != null)
            {
                model.PlaceForTheProduct = null;
                model.BidsParticipatePlace = SaleTenderReciept.BidParticpatePlace;

                model.SpecificNameOFtheProductPlace = null;
                var TenderRef = db.SaleTenders.SingleOrDefault(st => st.SaleTenderId == SaleTenderReciept.SaleTenderId);
                model.TenderRference = TenderRef.TenderReference;
                model.EvaluationDate = TenderRef.EvaluationDate.Year.ToString()+"/"+TenderRef.EvaluationDate.Month.ToString()+"/"+TenderRef.EvaluationDate.Day.ToString();
                model.Preparedby = SaleTenderReciept.CreatedBy;
                model.approvedBy = SaleTenderReciept.OnlineApprovedBy;
                model.PreparedDate = SaleTenderReciept.DateCreated.HasValue
      ? SaleTenderReciept.DateCreated.Value.Year.ToString() + "/" +
        SaleTenderReciept.DateCreated.Value.Month.ToString() + "/" +
        SaleTenderReciept.DateCreated.Value.Day.ToString()
      : string.Empty; // Handle null case if needed
                model.ApprovedDate = SaleTenderReciept.OnlineApprovedTime.HasValue
                    ? SaleTenderReciept.OnlineApprovedTime.Value.Year.ToString() + "/" +
                      SaleTenderReciept.OnlineApprovedTime.Value.Month.ToString() + "/" +
                      SaleTenderReciept.OnlineApprovedTime.Value.Day.ToString()
                    : string.Empty; // Handle null case if needed
            }
            var winnerDetail = db.Winners.Where(w => w.SalesLotId == salesLot).ToList();
            model.winners = new List<winnerVM>();
            foreach (var winner in winnerDetail)
            {
                var winnervm = new winnerVM();
                var productItem = db.SalesLotItems.SingleOrDefault(sli => sli.SalesLotItemId == winner.SalesLotItemId);
                winnervm.CustomerName = winner.CustomerName;
                winnervm.TinNumber = winner.TinNumber;
                winnervm.SubTotal = winner.WinningPrice;
                winnervm.Vat = (winner.WinningPrice * (decimal)(15))/100;
                winnervm.TotalIncludingVat = winnervm.Vat + winner.WinningPrice; ;
                winnervm.ProductName = winner.ProductName;
                winnervm.UOM = productItem.UOM;
                winnervm.ProductArea = productItem.ProductArea;
                winnervm.PorductAreaNumber = productItem.ProductAreaNumber ?? 0;
                model.winners.Add(winnervm);

            }
            return model;
        }
    }
}