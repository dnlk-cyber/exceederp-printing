using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class VehicleAccRecordInsuraceByTypeObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<VehicleAccRecordInsuraceByType> GetVehicleAccRecordInsuraceByType(string period)
        {
            List<VehicleAccRecordInsuraceByType> InfoList = new List<VehicleAccRecordInsuraceByType>();
            VehicleAccRecordInsuraceByType Info = new VehicleAccRecordInsuraceByType();
            //var categories = db.EquipmentType.OrderBy(x => x.CategoryName).ToList();
            //HighLevelAccident highLevelAccident = new HighLevelAccident();
            //MediumLevelAccident mediumLevelAccident = new MediumLevelAccident();
            //LowLevelAccident lowLevelAccident = new LowLevelAccident();
            //if (period == null)
            //{
            //    foreach (var c in categories)
            //    {
            //        Info = new VehicleAccRecordInsuraceByType();
            //        Info.HighLevelAccidents = new HighLevelAccident();
            //        Info.LowLevelAccidents = new LowLevelAccident();
            //        Info.MediumLevelAccidents = new MediumLevelAccident();
            //        Info.Type = c.CategoryName;
            //        var allVechiel =
            //            db.Fleets.Where(
            //                x => x.EquipmentTypeID == c.EquipmentTypeID).ToList();
            //        foreach (var al in allVechiel)
            //        {
            //            var asstes = db.FixedAssetAccidentRecords.Where(x => x.FleetsID == al.FleetsID).ToList();

            //            foreach (var a in asstes)
            //            {
            //                var hlvels =
            //                    db.VehicleAccidentDetails.Where(x => x.AccidentLevel == AccidentLevel.HIGH).ToList();
            //                Info.HighLevelAccidents.HighCrash += hlvels.Where(x => x.AccidentType == AccidentType.CRASH).Count();
            //                Info.HighLevelAccidents.HighRevrese += hlvels.Where(x => x.AccidentType == AccidentType.REVERSE).Count();
            //                Info.HighLevelAccidents.HighThirdParty += hlvels.Where(x => x.IsThirdPartAccident).Count();


            //                var mlvels =
            //                    db.VehicleAccidentDetails.Where(x => x.AccidentLevel == AccidentLevel.MEDIUM).ToList();
            //                Info.MediumLevelAccidents.MediumCrash += mlvels.Where(x => x.AccidentType == AccidentType.CRASH).Count();
            //                Info.MediumLevelAccidents.MediumRevrese += mlvels.Where(x => x.AccidentType == AccidentType.REVERSE).Count();
            //                Info.MediumLevelAccidents.MediumThirdParty += mlvels.Where(x => x.IsThirdPartAccident).Count();


            //                var llvels =
            //                    db.VehicleAccidentDetails.Where(x => x.AccidentLevel == AccidentLevel.LOW).ToList();
            //                Info.LowLevelAccidents.LowCrash += llvels.Where(x => x.AccidentType == AccidentType.CRASH).Count();
            //                Info.LowLevelAccidents.LowRevrese += llvels.Where(x => x.AccidentType == AccidentType.REVERSE).Count();
            //                Info.LowLevelAccidents.LowThirdParty += llvels.Where(x => x.IsThirdPartAccident).Count();

            //            }
            //        }
            //        InfoList.Add(Info);
            //    }
            //}
            //else
            //{
            //    foreach (var c in categories)
            //    {
            //        Info = new VehicleAccRecordInsuraceByType();
            //        Info.HighLevelAccidents = new HighLevelAccident();
            //        Info.LowLevelAccidents = new LowLevelAccident();
            //        Info.MediumLevelAccidents = new MediumLevelAccident();
            //        Info.Type = c.CategoryName;
            //        var allVechiel =
            //            db.Fleets.Where(
            //                x => x.EquipmentTypeID == c.EquipmentTypeID).ToList();
            //        foreach (var al in allVechiel)
            //        {
            //            var asstes = db.FixedAssetAccidentRecords.Where(x => x.FleetsID == al.FleetsID && x.GlFiscalYear.Name == period).ToList();

            //            foreach (var a in asstes)
            //            {
            //                var hlvels =
            //                    db.VehicleAccidentDetails.Where(x => x.AccidentLevel == AccidentLevel.HIGH).ToList();
            //                Info.HighLevelAccidents.HighCrash += hlvels.Where(x => x.AccidentType == AccidentType.CRASH).Count();
            //                Info.HighLevelAccidents.HighRevrese += hlvels.Where(x => x.AccidentType == AccidentType.REVERSE).Count();
            //                Info.HighLevelAccidents.HighThirdParty += hlvels.Where(x => x.IsThirdPartAccident).Count();


            //                var mlvels =
            //                    db.VehicleAccidentDetails.Where(x => x.AccidentLevel == AccidentLevel.MEDIUM).ToList();
            //                Info.MediumLevelAccidents.MediumCrash += mlvels.Where(x => x.AccidentType == AccidentType.CRASH).Count();
            //                Info.MediumLevelAccidents.MediumRevrese += mlvels.Where(x => x.AccidentType == AccidentType.REVERSE).Count();
            //                Info.MediumLevelAccidents.MediumThirdParty += mlvels.Where(x => x.IsThirdPartAccident).Count();


            //                var llvels =
            //                    db.VehicleAccidentDetails.Where(x => x.AccidentLevel == AccidentLevel.LOW).ToList();
            //                Info.LowLevelAccidents.LowCrash += llvels.Where(x => x.AccidentType == AccidentType.CRASH).Count();
            //                Info.LowLevelAccidents.LowRevrese += llvels.Where(x => x.AccidentType == AccidentType.REVERSE).Count();
            //                Info.LowLevelAccidents.LowThirdParty += llvels.Where(x => x.IsThirdPartAccident).Count();

            //            }
            //        }
            //        InfoList.Add(Info);
            //    }

            //}
            return InfoList;
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}