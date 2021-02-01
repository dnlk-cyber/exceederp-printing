using ExceedERP.Core.Domain.FixedAsset;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Fleet;
using ExceedERP.Reporting.Fleet.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Reporting.Fleet.DataSource
{
    [DataObject]
    public class FleetDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();



        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject> GetIdentities()
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
        public List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject> GetAssetCode()
        {

            var result = db.Fleets.Select(e => new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject
            {

                Name = e.UnitNum,
                Value = e.UnitNum

            }
            ).ToList();

            return result;

        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject> GetEquipmentNames()
        {
            List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject> List = new List<FleetBindingHelper.HelperObject>();
            var EquipmentTypes = db.Fleets.DistinctByField(x => x.EquipmentName).ToList();


            return EquipmentTypes.Select(s => new FleetBindingHelper.HelperObject { ParentValue = s.EquipmentName, Name = s.EquipmentName }).ToList();

        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject> GetEquipmentName()
        {
            List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject> List = new List<FleetBindingHelper.HelperObject>();
            var EquipmentTypes = db.EquipmentType.ToList();
            foreach (var EquipmentType in EquipmentTypes)
            {
                var FixedAssetCategory = db.FixedAssetCategories.Where(x => x.FixedAssetCategoryID == EquipmentType.FixedAssetCategoryID).FirstOrDefault();
                if (FixedAssetCategory != null)
                {
                    var FixedAssets = db.FixedAssetSubCategories.Where(x => x.CategoryCode == FixedAssetCategory.CategoryCode).ToList();
                    foreach (var FixedAsset in FixedAssets)
                    {
                        List.Add(new FleetBindingHelper.HelperObject { ParentValue = EquipmentType.EquipmentTypeID.ToString(), Name = FixedAsset.SubCategoryName });
                    }
                }
            }

            return List;

        }



        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject> GetCategory()
        {

            var result = db.EquipmentType.Select(e => new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject
            {
                ParentValue = e.EquipmentIdentityID.ToString(),
                Name = e.EquipmentTypeName,
                Value = e.EquipmentTypeID.ToString()

            }
            ).ToList();

            return result;

        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject> GetFleetStatus()
        {

            List<Helpers.Fleet.FleetBindingHelper.HelperObject> result = new List<Helpers.Fleet.FleetBindingHelper.HelperObject>
            {
                new Helpers.Fleet.FleetBindingHelper.HelperObject { Name = FleetViewModel.ACTIVE },
                new Helpers.Fleet.FleetBindingHelper.HelperObject { Name = FleetViewModel.ONDUTY },
                new Helpers.Fleet.FleetBindingHelper.HelperObject { Name = FleetViewModel.OnMaintenance }
            };

            return result;

        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject> GetPlateNumber()
        {

            var result = db.Fleets.DistinctByField(x => x.PlateNumber).Select(e => new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject
            {

                Name = e.PlateNumber,
                Value = e.PlateNumber


            }
            ).ToList();

            return result;

        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject> GetSerialNo()
        {

            var result = db.Fleets.DistinctByField(x => x.SerialNo).Select(e => new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject
            {

                Name = e.SerialNo,
                Value = e.SerialNo


            }
            ).ToList();

            return result;

        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject> GetChasisNumber()
        {

            var result = db.Fleets.DistinctByField(x => x.ChasisNumber).Select(e => new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject
            {

                Name = e.ChasisNumber,
                Value = e.ChasisNumber


            }
            ).ToList();

            return result;

        }


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject> GetModelNo()
        {

            var result = db.Fleets.DistinctByField(x => x.Model).Select(e => new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject
            {

                Name = e.Model,
                Value = e.Model


            }
            ).ToList();

            return result;

        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject> GetProductionYear()
        {

            var result = db.Fleets.Select(e => new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject
            {

                Name = e.ManufacturerYear,
                Value = e.FleetsID.ToString()


            }
            ).ToList();

            return result;

        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject> GetCondition()
        {

            var result = db.FleetConditions.Select(e => new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject
            {

                Name = e.Name,
                Value = e.Name


            }
            ).ToList();

            return result;

        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<Manufacturer> GetManufacturer()
        {

            var result = db.Manufacturer.ToList();

            return result;

        }
      
        public static string DateFormat(string Date)
        {
            DateTime Parsedate = DateTime.Now;
            if (!string.IsNullOrWhiteSpace(Date))
            {
                try
                {
                    Parsedate = DateTime.Parse(Date);
                    return Parsedate.Day + "/" + Parsedate.Month + "/" + Parsedate.Year;
                }
                catch (Exception)
                {
                    Parsedate = DateTime.Now;
                    return Parsedate.Day + "/" + Parsedate.Month + "/" + Parsedate.Year;
                }
            }
            return Parsedate.Day + "/" + Parsedate.Month + "/" + Parsedate.Year;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FleetViewModel> GetFleetList(string Identity, string category, string EquipmentName, string ProductionYear, string status, string PlateNo, string Model, string ChasisNo, string Location, string Condition)
        {
            List<FleetViewModel> FleetList = new List<FleetViewModel>();
            FleetViewModel FleetViewModel;
            List<Fleets> Fleets;

            var Assets = db.AssetRegistrations.ToList();
            var Branches = db.Branch.ToList();


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

                Fleets = db.Fleets.ToList();

            }
            if (Fleets.Any())
            {

                if (!string.IsNullOrEmpty(EquipmentName))
                {


                    Fleets = Fleets.Where(x => x.EquipmentName == EquipmentName).ToList();


                }
                if (!string.IsNullOrEmpty(ProductionYear))
                {
                    Fleets = Fleets.Where(x => x.ManufacturerYear == ProductionYear).ToList();
                }

                if (!string.IsNullOrEmpty(Location))
                {
                    Fleets = Fleets.Where(x => x.Location == Location).ToList();
                }
                if (!string.IsNullOrEmpty(ChasisNo))
                {
                    Fleets = Fleets.Where(x => x.ChasisNumber == ChasisNo).ToList();
                }
                if (!string.IsNullOrEmpty(PlateNo))
                {
                    Fleets = Fleets.Where(x => x.PlateNumber == PlateNo).ToList();
                }
                if (!string.IsNullOrEmpty(Model))
                {
                    Fleets = Fleets.Where(x => x.Model == Model).ToList();
                }
                if (!string.IsNullOrEmpty(status))
                {
                    Fleets = Fleets.Where(x => x.Status == status).ToList();
                }

                if (!string.IsNullOrEmpty(Condition))
                {
                    Fleets = Fleets.Where(x => x.Condition == Condition).ToList();
                }
                int RowCount = 1;

                foreach (var f in Fleets.Where(x => x.IsExternal == false && x.IsReturned == false))
                {


                    FleetViewModel = new FleetViewModel
                    {
                        Id = RowCount,
                        Location = f.Location,
                        CurentOdoMeter = f.CurentOdoMeter,
                        PurchasingCost = f.BookValue,
                        AnnualDepreciationCost = f.AnnualDepreciationCost,
                        ChasisNumber = f.ChasisNumber.IfNullDefaultValue() + " / " + f.SerialNo.IfNullDefaultValue(),
                        EngineNumber = f.EngineNumber,
                        EquipmentName = f.EquipmentName,
                        Manufacturer = f.Manufacturer,
                        ManufacturerYear = f.ManufacturerYear,
                        PlateNumber = f.PlateNumber,
                        StateOfOdoMeter = f.StateOfOdoMeter,
                        TypeOfFuel = f.TypeOfFuel,
                        UnitNum = f.UnitNum,

                        Model = f.Model,
                        Status = f.Status
                    };


                    RowCount += 1;

                    FleetList.Add(FleetViewModel);
                }
            }


            return FleetList.OrderByDescending(x => x.UnitNum).ToList();
        }

    }
}
