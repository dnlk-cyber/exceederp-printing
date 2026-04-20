using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.Procurements.ViewModel;

namespace ExceedERP.Reporting.Procurements.DataSource
{

    [DataObject]
    public class ProformaSummaryObjectDataSource
    {
        ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ProformaAndLotSummaryViewModel> GetProformaSummary(string id)
        {
            List<ProformaAndLotSummaryViewModel> proformaSummaryList = new List<ProformaAndLotSummaryViewModel>();

            var supplierData = db.ParticipatingSupplierItems.Where(x => x.ParticipatingSupplier.ProformaPurchaseID.ToString() == id);

            foreach (var supplierItem in supplierData)
            {
                var proformaSummary = new ProformaAndLotSummaryViewModel
                {
                    Description = supplierItem.ItemType == PurchaseItemType.FixedAsset ? supplierItem.ItemName + "-" + supplierItem.Name + "-" + supplierItem.ModelNo : supplierItem.ItemName + "-" + supplierItem.ItemSpecification,
                    SupplierName = supplierItem.ParticipatingSupplier.SupplierName,
                    Quantity = supplierItem.Quantity,
                    Unit = supplierItem.UoM,
                    UPrice = supplierItem.IsOutOfStock ? "Out of Stock" : (supplierItem.UPrice).ToString(),
                    //TPrice = supplier.IsOutOfStock ? "Out of Stock" : (supplier.Quantity * supplier.UPrice).ToString()
                };
                //sub total

                proformaSummaryList.Add(proformaSummary);
            }

            return proformaSummaryList;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SupplierSummaryViewModel> GetSupplierSummary(string id)
        {
            List<SupplierSummaryViewModel> supplierSummaryList = new List<SupplierSummaryViewModel>();


            var winningSuppliers = db.WinningSuppliers.Where(x => x.AwardGiven).Where(x => x.ProformaPurchaseID.ToString() == id).ToList();

            foreach (var winner in winningSuppliers)
            {

                var supplierSummary = new SupplierSummaryViewModel
                {
                    WinningSupplier = new List<WinningSupplierDetailVM>(),
                    WinnerSupplierName = winner.SupplierName
                };



                var winnerSuppliers = db.WinningSupplierItems.Where(x => x.WinningSupplierID == winner.WinningSupplierID).ToList();
                foreach (var winnerSupplier in winnerSuppliers)
                {
                    WinningSupplierDetailVM winnervm =
                        new WinningSupplierDetailVM
                        {
                            Description = winnerSupplier.ItemType == PurchaseItemType.FixedAsset ? winnerSupplier.ItemName + "-" + winnerSupplier.Name + "-" + winnerSupplier.ModelNo : winnerSupplier.ItemName + "-" + winnerSupplier.ItemSpecification,
                            Unit = winnerSupplier.UoM,
                            Quantity = winnerSupplier.Quanity,
                            UnitPrice = winnerSupplier.UPrice,
                            TotalPrice = winnerSupplier.TPrice,
                            SubTotal = winnerSupplier.UPrice * winnerSupplier.Quanity,
                            Vat = winnerSupplier.TaxAmount
                        };

                    supplierSummary.WinningSupplier.Add(winnervm);
                }

                supplierSummaryList.Add(supplierSummary);


            }

            return supplierSummaryList;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ProformaTeamVm> GetProformaTeam(string id)
        {
            List<ProformaTeamVm> members = new List<ProformaTeamVm>();
            if (!string.IsNullOrEmpty(id))
            {
                int did = int.Parse(id);
                var proforma = db.ProformaPurchases.Find(did);
                var team = db.ProformaTeams.Where(x => x.ProformaPurchaseID == proforma.ProformaPurchaseID).FirstOrDefault();
                if (team != null)
                {
                    var proformaTeams = db.ProformaTeamMembers.Where(x => x.ProformaTeamId == team.ProformaTeamId).ToList();
                    foreach (var m in proformaTeams)
                    {
                        string user = UserHelper.GetEmployeeUserName(m.EmployeeId);
                        var member = new ProformaTeamVm
                        {
                            FullName = EmployeeHelper.GetEmployeeEnglishName(m.EmployeeId),
                            Title = m.Title,
                            Signature = m.SignedBy == user ? SignatureHelper.GetUserSignature(user) : null

                        };
                        members.Add(member);
                    }
                }
            }

            return members;
        }

    }
}
