using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Fleet;
using ExceedERP.Reporting.Fleet.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;

namespace ExceedERP.Reporting.Fleet.DataSource
{
    [DataObject]
    public class EquipmentMaintenanceMechanicDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<MaintenaceMechanicViewModel> GetMaintenanceMechanic(DateTime StartingDate, DateTime EndingDate, string EquipmentType, string EquipmentName, string Project, string EquipmentCategory, string status, string PlateNo, string Model, string ChasisNo, string serialNo)
        {
            List<MaintenaceMechanicViewModel> MaintenaceMechanicList = new List<MaintenaceMechanicViewModel>();


            GLPeriod Periods = new GLPeriod();
            EquipmentIdentity Type = new EquipmentIdentity();
            List<Fleets> Equipments = db.Fleets.ToList();
            FleetMaintenaceHelper fleetMaintenaceHelper = new FleetMaintenaceHelper();



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

            Equipments = Equipments.FilterFleet(EquipmentCategory, EquipmentType, EquipmentName, PlateNo, ChasisNo, serialNo, string.Empty, Project, Model);


            var JobOrders = db.JobOrder.ToList();



            foreach (var Equipment in Equipments)
            {

                //var Orders = JobOrders.Where(x => x.FleetsID == Equipment.FleetsID).ToList();

                //foreach (var Order in Orders)
                //{
                //    var ServiceCrewWorkLogs = db.ServiceCrewWorkLogs.Where(x => x.DateCreated >= StartingDate && x.DateCreated <= EndingDate && x.JobId == Order.JobOrderID).ToList();

                //    foreach (var ServiceCrewWorkLog in ServiceCrewWorkLogs)
                //    {
                //        var MaintenaceMechanicViewModel = new MaintenaceMechanicViewModel();


                //        try
                //        {
                //            if (ServiceCrewWorkLog.DateCreated != null)
                //            {
                //                MaintenaceMechanicViewModel.Date = DateTime.Parse(ServiceCrewWorkLog.DateCreated.ToString());
                //            }

                //            MaintenaceMechanicViewModel.Mechanic = ServiceCrewWorkLog.MechanicCrew.FullName;
                //            MaintenaceMechanicViewModel.ServiceName = ServiceCrewWorkLog.JobOrderDetail.Service.ServiceName;
                //          MaintenaceMechanicViewModel.TotalWorked = fleetMaintenaceHelper.GetMechanicSingleDuration(ServiceCrewWorkLog);
                //            MaintenaceMechanicViewModel.JobOrderNo = Order.Num;

                //            MaintenaceMechanicViewModel.EquipmentCategory = Equipment.EquipmentIdentity.EquipmentCategoryName;
                //            MaintenaceMechanicViewModel.EquipmentType = Equipment.EquipmentType.EquipmentTypeName;
                //            MaintenaceMechanicViewModel.AssetCode = Equipment.UnitNum;

                //            MaintenaceMechanicViewModel.AmharicName = AmharicName;
                //            MaintenaceMechanicViewModel.Name = Name;
                //            MaintenaceMechanicViewModel.LogoFileName = LogoFileName;
                //            MaintenaceMechanicViewModel.Address = Address;

                //            MaintenaceMechanicList.Add(MaintenaceMechanicViewModel);
                //        }
                //        catch (Exception)
                //        {

                //        }
                //    }
                //}
            }





            if (!MaintenaceMechanicList.Any())
            {
                var MaintenaceMechanicViewModel = new MaintenaceMechanicViewModel
                {
                    AmharicName = AmharicName,
                    Name = Name,
                    LogoFileName = LogoFileName,
                    Address = Address
                };

                MaintenaceMechanicList.Add(MaintenaceMechanicViewModel);
            }

            return MaintenaceMechanicList;

        }

    }
}
