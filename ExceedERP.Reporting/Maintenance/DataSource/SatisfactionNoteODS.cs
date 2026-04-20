using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Fleet.ViewModel;
using ExceedERP.Reporting.Maintenance.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Maintenance;
using System.Drawing;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Reporting.Maintenance.DataSource
{
    [DataObject]
    public class SatisfactionNoteODS
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
       public SatisfactionNoteVM GetSatisfactionNote(string id)
        {
            SatisfactionNoteVM Note = new SatisfactionNoteVM();
            var fetchService = db.ServiceCenterJob.FirstOrDefault(x => x.ServiceCenterJobId.ToString() == id);
            var fetchOrder = db.JobOrder.FirstOrDefault(x => x.JobOrderID==fetchService.JobOrderID);
            var jobRequest = db.JobRequest.FirstOrDefault(x => x.Num == fetchOrder.Referance);
            if(jobRequest != null&&jobRequest.OnlineApprovedTime!=null&&fetchOrder!=null&&fetchOrder.OnlineApprovedTime!=null&&fetchService!=null&&fetchService.SecondOnlineApprovedTime!=null&&fetchService.SecondOnlineApprovedBy!=null)
            {
                Note.InspectedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(jobRequest.OnlineApprovedBy));
                Note.InspectorSign = SignatureHelper.GetUserSignature(jobRequest.OnlineApprovedBy);
                Note.InspectedDate=jobRequest.OnlineApprovedTime?.ToString("yyyy-MM-dd") ?? "";
                Note.ApprovedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(fetchService.SecondOnlineApprovedBy));
                Note.ApproverSign = SignatureHelper.GetUserSignature(fetchService.SecondOnlineApprovedBy);
                Note.ApprovedDate=fetchService.SecondOnlineApprovedTime?.ToString("yyyy-MM-dd") ?? "";
                Note.CheckedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(fetchOrder.OnlineApprovedBy));
                Note.CheckerSign = SignatureHelper.GetUserSignature(fetchOrder.OnlineApprovedBy);
                Note.CheckedDate = fetchOrder.OnlineApprovedTime?.ToString("yyyy-MM-dd")??"";
            }
            Note.RefNo = fetchOrder.Num.ToString()??" ";
            Note.PlateNumber = fetchOrder.LicensePlate??" ";
            Note.ChasisNo = db.Fleets.FirstOrDefault(x => x.PlateNumber == fetchOrder.LicensePlate || x.SerialNo == fetchOrder.LicensePlate)?.ChasisNumber??" ";
            Note.VehicleType = fetchOrder.EquipmentType;
            Note.MaintenanceAffectedBy = fetchOrder.MaintenanceCenter;

            return Note;
        }
    }
}

public class jobCausesVM
{
    public string WorkDescription { get; set; }
    public string Quantity { get; set; }
    public string Unit { get; set; }
    public string Remark { get; set; }
}
public class SatisfactionNoteVM
{
    public string MaintenanceAffectedBy { get; set; }
    public string JobOrderDate { get; set; }
    public string RefNo { get; set; }
    public string Model { get; set; }
    public string ChasisNo { get; set; }
    public string VehicleType { get; set; }
    public string PlateNumber { get; set; }
    public string JobPerformed { get; set; }
    public string InspectedBy { get; set; }
    public string InspectedDate { get; set; }
    public Image InspectorSign { get; set; }
    public string CheckedBy { get; set; }
    public string CheckedDate { get; set; }
    public Image CheckerSign { get; set; }
    public string ApprovedBy { get; set; }
    public string ApprovedDate { get; set; }
    public Image ApproverSign { get; set; }
    public jobCausesVM JobCauses { get; set; }
}