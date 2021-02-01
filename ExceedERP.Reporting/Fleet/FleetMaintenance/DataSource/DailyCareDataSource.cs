using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Fleet.FleetMaintenance.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.FleetMaintenance.DataSource
{
    [DataObject]
    public class DailyCareDataSource
    {
        ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<DailyCareViewModel> GetDailyCare(string Id)
        {

            List<DailyCareViewModel> FleetList = new List<DailyCareViewModel>();
            DailyCareViewModel CareModel;


            #region for ShardedDataSource
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




            var Cares = db.DailyCareLogs.Where(x => x.DailyCareLogId.ToString() == Id).FirstOrDefault();
            if (Cares != null)
            {
                var LineCares = db.DailyCareDetailLogs.Where(x => x.DailyCareLogId == Cares.DailyCareLogId).ToList();
                foreach (var Line in LineCares)
                {
                    var Items = db.DailyCareDetailItems.Where(x => x.DailyCareDetailLogId == Line.DailyCareDetailLogId).ToList();
                    CareModel = new DailyCareViewModel();

                    CareModel.AssetNo = Cares.Fleets.UnitNum;
                    CareModel.Care = Line.DailyCare.Name;
                    CareModel.Category = Line.DailyCare.DailyCareCategory.Name;
                    CareModel.EquipmentType = Cares.Fleets.EquipmentIdentity.EquipmentCategoryName;
                    CareModel.Location = Cares.Fleets.Location;
                    CareModel.OperatorName = Cares.Fleets.OperatorName;
                    var Item = Items.Where(x => x.Day == Core.Domain.FleetManagement.WeekDays.Monday).FirstOrDefault();
                    if (Item != null)
                    {
                        CareModel.Mon =  "✓";
                        CareModel.MonRemark = Item.Description;

                    }

                      Item = Items.Where(x => x.Day == Core.Domain.FleetManagement.WeekDays.Tuesday).FirstOrDefault();
                    if (Item != null)
                    {
                        CareModel.Tus =  "✓";
                        CareModel.TusRemark = Item.Description;

                    }
                      Item = Items.Where(x => x.Day == Core.Domain.FleetManagement.WeekDays.Wednesday).FirstOrDefault();
                    if (Item != null)
                    {
                        CareModel.Wed = "✓" ;
                        CareModel.WedRemark = Item.Description;

                    }
                      Item = Items.Where(x => x.Day == Core.Domain.FleetManagement.WeekDays.Thursday).FirstOrDefault();
                    if (Item != null)
                    {
                        CareModel.Thu =  "✓";
                        CareModel.ThuRemark = Item.Description;

                    }
                      Item = Items.Where(x => x.Day == Core.Domain.FleetManagement.WeekDays.Friday).FirstOrDefault();
                    if (Item != null)
                    {
                        CareModel.Fri =  "✓";
                        CareModel.FriRemark = Item.Description;

                    }

                    CareModel.AmharicName = AmharicName;
                    CareModel.Name = Name;
                    CareModel.Address = Address;
                    CareModel.LogoFileName = LogoFileName;


                    FleetList.Add(CareModel);

                }
          
            }


            return FleetList;
        }
    }
}
