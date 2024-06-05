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
        public List<FleetBindingHelper.HelperObject> GetIdentities()
        {

            var result = db.EquipmentIdentity.Select(e => new FleetBindingHelper.HelperObject
            {

                Name = e.EquipmentCategoryName,
                Value = e.EquipmentIdentityID.ToString()

            }
            ).ToList();

            return result;

        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FleetBindingHelper.HelperObject> GetAssetCode()
        {

            var result = db.Fleets.Select(e => new FleetBindingHelper.HelperObject
            {

                Name = e.UnitNum,
                Value = e.UnitNum

            }
            ).ToList();

            return result;

        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FleetBindingHelper.HelperObject> GetEquipmentNames()
        {
            List<FleetBindingHelper.HelperObject> List = new List<FleetBindingHelper.HelperObject>();
            var EquipmentTypes = db.Fleets.DistinctByField(x => x.EquipmentName).ToList();


            return EquipmentTypes.Select(s => new FleetBindingHelper.HelperObject { ParentValue = s.EquipmentName, Name = s.EquipmentName }).ToList();

        }
        
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FleetBindingHelper.HelperObject> GetEquipmentName()
        {
            List<FleetBindingHelper.HelperObject> List = new List<FleetBindingHelper.HelperObject>();
            var equipmentTypes = db.EquipmentType.ToList();
            foreach (var equipmentType in equipmentTypes)
            {
              
                var FixedAssets = db.FixedAssetSubCategories.Where(x => x.FixedAssetSubCategoryID == equipmentType.FixedAssetSubCategoryId).ToList();
                foreach (var FixedAsset in FixedAssets)
                {
                    List.Add(new FleetBindingHelper.HelperObject { ParentValue = equipmentType.EquipmentTypeID.ToString(), Name = FixedAsset.SubCategoryName });
                }
            }

            return List;

        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FleetBindingHelper.HelperObject> GetEquipmentNamesFixed()
        {
            List<FleetBindingHelper.HelperObject> List = new List<FleetBindingHelper.HelperObject>();
            var result = db.EquipmentName.Select(s => new FleetBindingHelper.HelperObject
            {
                Name = s.Name,
                Value = s.EquipmentNameID.ToString()
            }).ToList();
            return result;

        }


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FleetBindingHelper.HelperObject> GetCategory()
        {

            var result = db.EquipmentType.Select(e => new FleetBindingHelper.HelperObject
            {
                ParentValue = e.EquipmentIdentityID.ToString(),
                Name = e.EquipmentTypeName,
                Value = e.EquipmentTypeID.ToString()

            }
            ).ToList();

            return result;

        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FleetBindingHelper.HelperObject> GetFleetStatus()
        {

            List<FleetBindingHelper.HelperObject> result = new List<FleetBindingHelper.HelperObject>
            {
                new FleetBindingHelper.HelperObject { Name = FleetViewModel.ACTIVE },
                new FleetBindingHelper.HelperObject { Name = FleetViewModel.ONDUTY },
                new FleetBindingHelper.HelperObject { Name = FleetViewModel.OnMaintenance }
            };

            return result;

        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FleetBindingHelper.HelperObject> GetPlateNumber()
        {

            var result = db.Fleets.DistinctByField(x => x.PlateNumber).Select(e => new FleetBindingHelper.HelperObject
            {

                Name = e.PlateNumber,
                Value = e.PlateNumber


            }
            ).ToList();

            return result;

        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FleetBindingHelper.HelperObject> GetSerialNo()
        {

            var result = db.Fleets.DistinctByField(x => x.SerialNo).Select(e => new FleetBindingHelper.HelperObject
            {

                Name = e.SerialNo,
                Value = e.SerialNo


            }
            ).ToList();

            return result;

        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FleetBindingHelper.HelperObject> GetChasisNumber()
        {

            var result = db.Fleets.DistinctByField(x => x.ChasisNumber).Select(e => new FleetBindingHelper.HelperObject
            {

                Name = e.ChasisNumber,
                Value = e.ChasisNumber


            }
            ).ToList();

            return result;

        }


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FleetBindingHelper.HelperObject> GetModelNo()
        {

            var result = db.Fleets.DistinctByField(x => x.Model).Select(e => new FleetBindingHelper.HelperObject
            {

                Name = e.Model,
                Value = e.Model


            }
            ).ToList();

            return result;

        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FleetBindingHelper.HelperObject> GetProductionYear()
        {

            var result = db.Fleets.Select(e => new FleetBindingHelper.HelperObject
            {

                Name = e.ManufacturerYear,
                Value = e.FleetsID.ToString()


            }
            ).ToList();

            return result;

        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FleetBindingHelper.HelperObject> GetCondition()
        {

            var result = db.FleetConditions.Select(e => new FleetBindingHelper.HelperObject
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
                    var equipmentName = db.EquipmentName.FirstOrDefault(x => x.EquipmentNameID == f.EquipmentNameID);
                    var equipmentIdentity = db.EquipmentIdentity.FirstOrDefault(x => x.EquipmentIdentityID == f.EquipmentIdentityID);
                    var equipmentType = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID == f.EquipmentTypeID);
                    FleetViewModel = new FleetViewModel
                    {
                        Id = RowCount,
                        Location = f.Location,
                        CurentOdoMeter = f.CurentOdoMeter,
                        PurchasingCost = f.BookValue,
                        AnnualDepreciationCost = f.AnnualDepreciationCost,
                        //ChasisNumber = f.ChasisNumber.IfNullDefaultValue() + " / " + f.SerialNo.IfNullDefaultValue(),
                        ChasisNumber = f?.ChasisNumber ?? "Not Assigned",
                        EngineNumber = f.EngineNumber,
                        EquipmentName = equipmentName?.Name ?? "Not Assigned",
                        Manufacturer = f.Manufacturer,
                        ManufacturerYear = f.ManufacturerYear,
                        PlateNumber = f.PlateNumber,
                        StateOfOdoMeter = f.StateOfOdoMeter,
                        TypeOfFuel = f.TypeOfFuel,
                        UnitNum = f.UnitNum,

                        Model = f.Model,
                        Status = f.Status,

                        OperatorName = f.OperatorName,
                        PurchasedDate = f.PurchasedDate,
                        RentalRate = f.RentalRate,
                        SupplierCompany = f.SupplierCompany,
                        SerialNo = f.SerialNo,
                        Type = f.Type,
                        Tyre = f.Tyre,
                        TyreSize = f.TyreSize,
                        WayOfSupply = f.WayOfSupply,
                        Battery = f.Battery,
                        Capacity = f.Capacity,
                        Category = f.Category,
                        Condition = f.Condition,
                        Color = f.Color,
                        DepreciationCost = f.DepreciationCost,
                        EngineManufacturer = f.EngineManufacturer,
                        EngineModel = f.EngineModel,
                        EquipmentIdentityName = equipmentIdentity?.EquipmentCategoryName ?? "",
                        EquipmentTypeName = equipmentType?.EquipmentTypeName ?? "",
                        MaxFuelStandard = f.MaxFuelStandard,
                        MinFuelStandard = f.MinFuelStandard,
                        IdealRentalRate = f.IdealRentalRate,
                        Libre = f.Libre,
                        


                    };


                    RowCount += 1;

                    FleetList.Add(FleetViewModel);
                }
            }


            return FleetList.OrderByDescending(x => x.UnitNum).ToList();
        }

    }
}
