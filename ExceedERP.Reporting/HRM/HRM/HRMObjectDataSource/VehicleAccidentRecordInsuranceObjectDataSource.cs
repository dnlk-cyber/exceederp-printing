using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class VehicleAccidentRecordInsuranceObjectDataSource : List<VehicleAccidentRecordInsurace>
    {
        //private ExceedDbContext db = new ExceedDbContext();

        //[DataObjectMethod(DataObjectMethodType.Select)]
        //public List<VehicleAccidentRecordInsurace> GetVehicleAccidentRecordInsurance(string period)
        //{
        //    List<VehicleAccidentRecordInsurace> InfoList = new List<VehicleAccidentRecordInsurace>();
        //    VehicleAccidentRecordInsurace Info = new VehicleAccidentRecordInsurace();


        //    if (period == null)
        //    {
        //        var asstes = db.FixedAssetAccidentRecords.ToList();
        //        foreach (var a in asstes)
        //        {
        //            Info = new VehicleAccidentRecordInsurace();
        //            Info.Amount = a.Amount;
        //            Info.PlateNo = a.PlateNumber;
        //            Info.Remark = a.Remark;
        //            Info.Vehicle = a.Fleets.EquipmentName;
        //            Info.ProjectName = a.Branch.AmharicName;
        //            InfoList.Add(Info);
        //        }
        //    }
        //    else
        //    {
        //        var asstes = db.FixedAssetAccidentRecords.Where(x => x.GlFiscalYear.Name == period).ToList();
        //        foreach (var a in asstes)
        //        {
        //            Info = new VehicleAccidentRecordInsurace();
        //            Info.Amount = a.Amount;
        //            Info.PlateNo = a.PlateNumber;
        //            Info.Remark = a.Remark;
        //            Info.Vehicle = a.Fleets.EquipmentName;
        //            InfoList.Add(Info);
        //        }

        //    }
        //    return InfoList;
        //}
    }
}