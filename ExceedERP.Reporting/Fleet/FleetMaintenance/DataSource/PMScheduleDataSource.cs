using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using ExceedERP.DataAccess.Context;
using  ExceedERP.Reporting.Fleet.FleetMaintenance.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Helpers.Fleet;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Reporting.Fleet.FleetMaintenance.DataSource
{
    [DataObject]
    public class PMScheduleDataSource
    {
        ExceedDbContext db = new ExceedDbContext();
        IJobOrderService _JobOrderService = new JobOrderService();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PMActualScheduleViewModel> GetActualPMSchedule(string Year, string Period, DateTime StartingDate, DateTime EndingDate, string Location )
        {

            List<PMActualScheduleViewModel> FleetList = new List<PMActualScheduleViewModel>();
            PMActualScheduleViewModel ScheduleModel;
            IJobOrderDetailService _JobOrderDetailService = new JobOrderDetailService();

            var GlPeriod = db.GLPeriods.Where(x => x.GLPeriodId.ToString() == Period).FirstOrDefault();


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
            if (GlPeriod != null)
            {
                var Shcedules = db.PMSchedules.ToList();
                var Fleets = db.Fleets.ToList();
                var Job = _JobOrderDetailService.GetAll().Where(x => x.DateCreated > StartingDate && x.DateCreated < EndingDate).ToList();


                if (!string.IsNullOrEmpty(Location))
                {
                    Shcedules = Shcedules.Where(x => x.Location == Location).ToList();
                }
                var DistictType = Shcedules.DistinctByField(x => x.EquipmentTypeId).ToList();
                foreach (var Shcedule in DistictType)
                {
                    var PMDetail = db.PMScheduleDetails.Where(x => x.PMScheduleId == Shcedule.PMScheduleId);
                    var LineShedules = PMDetail.DistinctByField(x => x.ServiceId).ToList();
                    var FleetType = Fleets.Where(x => x.EquipmentTypeID == Shcedule.EquipmentTypeId).Count();
                    foreach (var Line in LineShedules)
                    {
                        ScheduleModel = new PMActualScheduleViewModel();

                        ScheduleModel.EquipmentType = Shcedule.EquipmentType.CategoryName;
                        ScheduleModel.Service = Line.Service.ServiceName;
                        ScheduleModel.FleetCount = decimal.Parse((FleetType).ToString());
                        ScheduleModel.Actual = decimal.Parse((Job.Where(x => x.ServiceID == Line.ServiceId).Count()).ToString());
                        ScheduleModel.Plan = decimal.Parse((PMDetail.Where(x => x.ServiceId == Line.ServiceId).Count()).ToString());

                        ScheduleModel.AmharicName = AmharicName;
                        ScheduleModel.Name = Name;
                        ScheduleModel.LogoFileName = LogoFileName;
                        ScheduleModel.Address = Address;

                        FleetList.Add(ScheduleModel);

                    }
                }
            }
            return FleetList;
        }
   
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject> GetEquipmentType()
        {

            var result = db.EquipmentIdentity.Select(e => new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject
            {

                Name = e.EquipmentCategoryName,
                Value = e.EquipmentIdentityID.ToString()

            }
            ).ToList();

            return result;

        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PMScheduleViewModel> GetPMSchedule(DateTime StartingDate, DateTime EndingDate,  string Year, string Category, string Type, string EquipmentName, string PlateNo, string Model, string ChasisNo, string Serial, string Location)
        {

            List<PMScheduleViewModel> FleetList = new List<PMScheduleViewModel>();
            PMScheduleViewModel ScheduleModel;
            GLPeriod Periods = new GLPeriod();



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


            var Shcedules = db.PMSchedules.ToList();


            if (!string.IsNullOrEmpty(Type))
            {
                Shcedules = Shcedules.Where(x => x.EquipmentTypeId.ToString() == Type).ToList();
            }

            foreach (var Shcedule in Shcedules)
            {
                var type = db.EquipmentType.Where(x => x.EquipmentTypeID == Shcedule.EquipmentTypeId).FirstOrDefault();
                if (type != null)
                {
                var LineShedules = db.PMScheduleDetails.Where(x => x.PMScheduleId == Shcedule.PMScheduleId).ToList();
                foreach (var Line in LineShedules)
                {
                    if (FleetGenericHelper.FilterSingleFleet(Line.FleetsId, Category, Type, PlateNo, ChasisNo, Serial, string.Empty, Location, Model))
                    {
                        ScheduleModel = new PMScheduleViewModel();

                        ScheduleModel.EquipmentType = type.EquipmentTypeName;
                            ScheduleModel.SerialNo = Line.Fleets.SerialNo;
                            ScheduleModel.LastDate = DateTime.Parse(Line.Fleets.LastMaintained.ToString());
                            ScheduleModel.LastKMHR = Line.Fleets.LastMaintenanceOdometer.ToString();
                            ScheduleModel.Month = Line.GLPeriod.Name;
                            ScheduleModel.Type = Shcedule.Type;
                            ScheduleModel.Week = Line.Weeks.ToString();
                            ScheduleModel.Service = Line.Service.ServiceName;

                            ScheduleModel.AmharicName = AmharicName;
                            ScheduleModel.Name = Name;
                            ScheduleModel.LogoFileName = LogoFileName;
                            ScheduleModel.Address = Address;

                            FleetList.Add(ScheduleModel);
                        }
                    }
                }
             
            }
            return FleetList;
        }
    }
}
