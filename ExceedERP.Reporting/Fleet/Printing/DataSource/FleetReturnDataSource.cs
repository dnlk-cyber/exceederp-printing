using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
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
using ExceedERP.Helpers.Fleet;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.ApplicationServices.FleetManagement.FleetResource;
using System.IO;
using System.Web;
using ExceedERP.Reporting.Fleet.Printing.ViewModel;
using ExceedERP.Core.Domain.FleetManagement.EquipmentInspection;

namespace ExceedERP.Reporting.Fleet.Printing.DataSource
{
    [DataObject]
    public class FleetReturnDataSource
    {
        ExceedDbContext db = new ExceedDbContext();





        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FleetReturnViewModel> GetFleetReturn(string Id)
        {
            IFleetService _FleetService = new FleetService();
            List<FleetReturnViewModel> FleetList = new List<FleetReturnViewModel>();
            FleetReturnViewModel FleetReturnViewModel;

            #region SharedResource




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


            #endregion

            var FleetReturn = db.FleetReturns.Where(x => x.FleetReturnId.ToString() == Id).FirstOrDefault();
            var FleetReturnParts = db.FleetReturnItems.Where(x => x.FleetReturnId.ToString() == Id).ToList();
             List<CheckList> list = db.CheckLists.ToList();
            Fleets fleet = db.Fleets.Where(x => x.PlateNumber == FleetReturn.PlateNo).FirstOrDefault();
           string CheckList = string.Empty;
           if (fleet != null)
           {
               var CheckListlog = db.CheckListLogs.Where(x => x.FleetId == fleet.FleetsID.ToString()).ToList();

               foreach (var check in CheckListlog)
               {
                   CheckList = CheckList + " - " + check.LineName + " __ ,";
               }
           }

            foreach (var FleetReturnPart in FleetReturnParts)
            {
                if (FleetReturn != null)
                {
                 
                    FleetReturnViewModel = new FleetReturnViewModel();
                    //   FleetReturnViewModel.Equipment = FleetReturn.Fleet.UnitNum + " " + FleetReturn.Fleet.;
                    FleetReturnViewModel.AssetNo = FleetReturn.AssetNo;
                    FleetReturnViewModel.Causeofreturn = FleetReturn.Causeofreturn;
                    FleetReturnViewModel.ChancesSerialNo = FleetReturn.ChancesSerialNo;
                    FleetReturnViewModel.Condition = FleetReturn.Condition;
                            FleetReturnViewModel.FleetReturnItemId = FleetReturnPart.FleetReturnItemId;
                    FleetReturnViewModel.CostCode = FleetReturnPart.CostCode;
                    FleetReturnViewModel.FleetReturnId = FleetReturn.FleetReturnId;
                    FleetReturnViewModel.CreditAccountNo = FleetReturn.CreditAccountNo;
                    FleetReturnViewModel.DebitAccountNo = FleetReturn.DebitAccountNo;
                    FleetReturnViewModel.DeliveredTo = FleetReturn.DeliveredTo;
                    FleetReturnViewModel.EquipmentName = FleetReturn.EquipmentName;
                    FleetReturnViewModel.From = FleetReturn.From;
                            FleetReturnViewModel.CheckList =  CheckList;
                    FleetReturnViewModel.IdentifcationNo = FleetReturnPart.IdentifcationNo;
                    FleetReturnViewModel.Location = FleetReturnPart.Location;
                    FleetReturnViewModel.Manufacturer = FleetReturn.Manufacturer;
                    FleetReturnViewModel.OwnerCertificateNo = FleetReturn.OwnerCertificateNo;
                    FleetReturnViewModel.PartNo = FleetReturnPart.PartNo;
                    FleetReturnViewModel.PlateNo = FleetReturnPart.PartNo;
                    FleetReturnViewModel.Recieved = FleetReturnPart.Recieved;
                    FleetReturnViewModel.Requested = FleetReturnPart.Requested;
                    FleetReturnViewModel.TotalPriceBirr = FleetReturnPart.TotalPriceBirr;
                    FleetReturnViewModel.TotalPriceCent = FleetReturnPart.TotalPriceCent;
                    FleetReturnViewModel.UnitPriceBirr = FleetReturnPart.UnitPriceBirr;
                    FleetReturnViewModel.UnitPriceCent = FleetReturnPart.UnitPriceCent;
                    FleetReturnViewModel.Unit = FleetReturnPart.Unit;
                    FleetReturnViewModel.AmharicName = AmharicName;
                    FleetReturnViewModel.Name = Name;
                    FleetReturnViewModel.Address = Address;
                    FleetReturnViewModel.LogoFileName = LogoFileName;

                    FleetReturnViewModel.Description = FleetReturnPart.Description;

                    FleetList.Add(FleetReturnViewModel);

                }
            }


            return FleetList;
        }

    }
}
