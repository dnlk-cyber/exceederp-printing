using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Maintenance.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Maintenance.DataSource
{
    [DataObject]
    public class WorkOrderWorkSheetOds
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
 
            public WorkShopWorkSheetViewModel GetData(string id)
        { 
           WorkShopWorkSheetViewModel model = new WorkShopWorkSheetViewModel();
            int.TryParse(id, out int Num);

            var jobOrder = db.JobOrder.Where(q=>q.Num==Num).FirstOrDefault();
            if (jobOrder != null)
            {
                var organization = db.Companies.FirstOrDefault(x => x.Category == "0");
                Image LogoFileName = null;
                string AmharicName = "";
                string Name = "";
                if (organization != null)
                {
                    Name = organization.TradeName;
                    AmharicName = organization.AmharicName;
                    var logo =
                                       db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                                               a.ReferenceType == ExceedERP.Core.Domain.Common.AttachmentType.LOGO);
                    if (logo != null)
                    {
                        LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                    }
                }

                model.LogoFileName = LogoFileName;
                model.Name = Name;
                model.AmharicName = AmharicName;
                model.Date = DateTime.Now.ToString("yyyy-MM-dd");
                var jobRequest = db.JobRequest.Where(q => q.Num == jobOrder.Num).FirstOrDefault();
                if (jobRequest != null)
                {
                    model.TimeIn = jobRequest.DateCreated?.ToString("yyyy-MM-dd");
                    model.PlateNo = jobRequest.LicensePlate.ToString();

                    string customer;
                    if (jobRequest.CustomerId == null)
                    {
                        customer = "Internal";
                    }
                    else
                    {
                        customer = db.OrganizationCustomers.Find(jobRequest.CustomerId).TradeName;
                    }
                    model.Customer = customer;


                }
                var vehicle = db.EquipmentInformation.Where(q => q.LicensePlate == jobOrder.LicensePlate).FirstOrDefault();
                model.OrderNo = jobOrder.Num.ToString();
                model.PlateNo = jobOrder.LicensePlate.ToString();
                model.Date = DateTime.Now.ToString("yyyy-MM-dd");
                if (vehicle != null)
                {

                    model.vehicleModel = vehicle.DeviceNumber;
                    model.VehicleType = vehicle.GroupName;
                    model.Mileage = vehicle.InitialOdometer;

                }

                var workSheets = db.ServiceCrewWorkLogs.Where(q => q.JobId == jobOrder.JobOrderID).ToList();
                IList<WorkSheetViewModel> workSheetViewModels = new List<WorkSheetViewModel>();
                if (workSheets.Any())
                {
                    int i = 1;
                    foreach(var workSheet in workSheets)
                    {
                        WorkSheetViewModel workSheetViewModel = new WorkSheetViewModel();
                        workSheetViewModel.Description = workSheet.Remark;
                        workSheetViewModel.No = i;
                        workSheetViewModel.ServiceGroup = workSheet.ServiceGroup;
                        workSheetViewModel.SubCategoryCode = db.ItemCategorys.Find(workSheet.SubCategoryCode)?.Name;
                        workSheetViewModel.RegularTime = workSheet.TotalLabourHours;
                        workSheetViewModel.OverTime = workSheet.StandaredHours;
                        var service = db.JobOrderDetail.Find(workSheet.JobOrderDetailId);
                        if(service!=null)
                        {
                            var repairType = db.RepairType.Find(service.RepairTypeID);
                            if(repairType!=null)
                            {
                                workSheetViewModel.ActionCode = repairType.Name;
                                workSheetViewModel.LaborCost = workSheet.StandaredHours * repairType.PayableTime;
                            }
                           

                        }
                        var mechanic = db.MechanicCrew.Find(workSheet.MecanicId);
                        if (mechanic != null)
                        {
                            var employee = db.Person_Employee.Find(mechanic.EmployeeId);
                            if(employee!=null)
                            {
                                workSheetViewModel.Mechanic = employee.FirstName+" "+ employee.MiddleName + " "+ employee.LastName;

                            }

                        }
                        workSheetViewModels.Add(workSheetViewModel);
                        i++;
                    }
                }
                model.WorkSheetViewModels = workSheetViewModels;

            }

            return model;
        }

    }
}
