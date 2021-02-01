using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetTireManagement;
using ExceedERP.Core.Domain.FleetManagement.FuelManagement;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Fleet.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.DataSource
{
    [DataObject]
   public class FulelogDataSource
    {


        ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FuelViewModel> GetFuelIssuelogs(string Identity, string category, string FuelType, string Manufacturer,
           DateTime startingDate, DateTime endingDate)
        {
            List<FuelViewModel> FleetList = new List<FuelViewModel>();
            FleetViewModel FleetViewModel;
            List<Fleets> Fleets;
            List<TireIssue> Issues;
            var product = db.Products.ToList();





            string AmharicName = "";
            string Name = "";
            string Address = "";
            Image LogoFileName = null;
            var organization = db.Companies.FirstOrDefault(x => x.Category == "0");

            if (organization != null)
            {
                Name = organization.TradeName;
                AmharicName = organization.AmharicName;
                var address =
                    db.Addresses.Where(
                        a =>
                            a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType.ToLower() == "company").ToList();
                if (address.Any())
                {
                    foreach (var a in address)
                    {

                        if (Address == null)
                        {
                            Address = a.Category + ":- " + a.Value;
                        }
                        else
                        {
                            Address += ", " + a.Category + "-" + a.Value;
                        }

                    }

                }
                var logo =
                    db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType == ExceedERP.Core.Domain.Common.AttachmentType.LOGO);
                if (logo != null)
                {
                    LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }



            if (!string.IsNullOrEmpty(Identity))
            {
                if (!string.IsNullOrEmpty(category))
                {
                    Fleets =
                        db.Fleets.Where(
                            so =>
                                so.EquipmentIdentityID.ToString() == Identity &&
                                so.EquipmentTypeID.ToString() == category)
                            .ToList();
                }
                else
                {

                    Fleets =
                                          db.Fleets.Where(
                                              so =>

                                                  so.EquipmentIdentityID.ToString() == Identity)
                                              .ToList();
                }
           
            }
            else
            {
                if (!string.IsNullOrEmpty(Manufacturer))
                {
                    Fleets = db.Fleets.Where(so => so.ManufacturerYear == Manufacturer).ToList();
                }
                else
                {
                    Fleets = db.Fleets.ToList();
                }
            }
            if (Fleets.Any())
            {
                List<Fleets> List = Fleets;
                if (!string.IsNullOrEmpty(FuelType))
                {

               List = List.Where(x => x.TypeOfFuel == FuelType).ToList();
                }
                foreach (Fleets sov in List)
                {

                    string OperatorName = string.Empty;
                    var operators = db.Operator.Where(x=>x.FleetsID == sov.FleetsID).FirstOrDefault();
                    if(operators!=null)
                    {
                        OperatorName = operators.FullName;
                    }
                    List<Issue> fuels = db.Issues.Where(x => x.PlateNo == sov.PlateNumber && x.IsFuel == true && x.DateCreated > startingDate && x.DateCreated < endingDate).ToList();

                
                    foreach(var fuel in fuels)
                    {
                           foreach(var Items in fuel.IssueItems)
                    {
                        var FuelViewModel = new FuelViewModel();

                         
                        //FuelViewModel.UnitPrice = decimal.Parse(Items.UnitPrice.ToString());
                        //FuelViewModel.FinalKmHr = decimal.Parse(Items..ToString());
                        //FuelViewModel.PlanKmHr = int.Parse( fuel.PlanKmHr.ToString());
                        //FuelViewModel.FuelFilledAmount = decimal.Parse(fuel.FuelFilledAmount.ToString());
                        //FuelViewModel.FuelType = sov.TypeOfFuel;
                        //FuelViewModel.FuelSource = fuel.FuelSource;
                        //FuelViewModel.FuelType = fuel.FuelType;
                        //FuelViewModel.OperatorName = fuel.OperatorName;
                        //FuelViewModel.FuelSourceDetail = fuel.FuelSourceDetail;
                        //FuelViewModel.Date = fuel.DateCreated;
                        //FuelViewModel.Description = sov.PlateNumber + " - " + sov.EquipmentName;
                        //FuelViewModel.TotalFuelcost = decimal.Parse((fuels.Sum(x => x.FuelFilledAmount) * fuels.Sum(x => x.UnitPrice)).ToString());

                        FuelViewModel.AmharicName = AmharicName;
                        FuelViewModel.Name = Name;
                        FuelViewModel.LogoFileName = LogoFileName;
                        FuelViewModel.Address = Address;

                        FleetList.Add(FuelViewModel);
                    }}
                }
            }

            return FleetList;
        }

    }
}
