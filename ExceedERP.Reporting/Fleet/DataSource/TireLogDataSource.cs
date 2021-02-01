using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetTireManagement;
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
        public  class TireLogDataSource
    {
        ExceedDbContext db = new ExceedDbContext();
           [DataObjectMethod(DataObjectMethodType.Select)]
            public List<TireViewModel> GetireIssueLog(string Identity, string category, string Manufacturer, string tyretube , string tyretype,
        DateTime startingDate, DateTime endingDate)
            {
                List<TireViewModel> TireList = new List<TireViewModel>();
                TireViewModel FleetViewModel;
                List<Fleets> Fleets;
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
                        Fleets = db.Fleets.Where(so => so.Manufacturer == Manufacturer).ToList();
                    }
                    else
                    {
                        Fleets = db.Fleets.ToList();
                    }
                }
                if (Fleets.Any())
                {
                    List<Fleets> List = Fleets;
                    foreach (Fleets sov in List)
                    {
                        string OperatorName = string.Empty;
                        var operators = db.Operator.Where(x => x.FleetsID == sov.FleetsID).FirstOrDefault();
                        if (operators != null)
                        {
                            OperatorName = operators.FullName;
                        }
                        List<TireIssue> Issues = db.TireIssue.Where(x => x.PlateNumber == sov.PlateNumber && x.DateCreated > startingDate && x.DateCreated < endingDate).ToList();


                        if (!string.IsNullOrEmpty(tyretype))
                        {

                            Issues = Issues.Where(x => x.TireType == tyretype).ToList();
                        }


                        if (!string.IsNullOrEmpty(tyretube))
                        {
                            Issues = Issues.Where(x => x.TireTube == tyretube).ToList();
                        }
                        foreach(var f in Issues)
                        {
                            FleetViewModel = new TireViewModel();
                            FleetViewModel.PlateNumber = sov.PlateNumber + " - "+sov.EquipmentName;
                            FleetViewModel.OperatorName = f.OperatorName;
                            FleetViewModel.UnitPrice = decimal.Parse(f.UnitPrice.ToString());
                            FleetViewModel.Quantity =  0;
                            FleetViewModel.TotalPrice = decimal.Parse(( f.UnitPrice * 0).ToString());
                            FleetViewModel.SerialNo = f.SerialNo;
                            FleetViewModel.Brand = f.Brand;
                            FleetViewModel.InstalledDate = f.InstalledDate;
                            FleetViewModel.TireSize = f.TireSize;
                            FleetViewModel.TireType = f.TireType;
                            FleetViewModel.TireTube = f.TireTube;
                            FleetViewModel.AmharicName = AmharicName;
                            FleetViewModel.Name = Name;
                            FleetViewModel.LogoFileName = LogoFileName;
                            FleetViewModel.Address = Address;

                            TireList.Add(FleetViewModel);
                        }

                    }
                }

                return TireList;
            }

    }
}
