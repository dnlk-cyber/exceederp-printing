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
    public class JobOrderOds
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public JobOrderViewModel GetData(string id)
        {
            JobOrderViewModel model = new JobOrderViewModel();
            int.TryParse(id, out int Num);
            var jobOrder = db.JobOrder.Where(q => q.Num == Num).FirstOrDefault();
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
                model.JobOrderNo = jobOrder.JobOrderNum.ToString();
                if (jobOrder.IsRework)
                {
                    model.IsRework = "Yes";
                }
                else
                {
                    model.IsRework = "No";
                }
                
                model.ProjectName = jobOrder.ProjectName;
                model.Date = DateTime.Now.ToString("yyyy-MM-dd");

                var jobRequest = db.JobRequest.Where(q => q.Num == jobOrder.Num).FirstOrDefault();
                if(jobRequest!=null)
                {

                    model.DateCreated = jobRequest.DateCreated?.ToString("yyyy-MM-dd");
                    model.EquipmentName = jobRequest.EquipmentName;
                    model.LicensePlate = jobRequest.LicensePlate;
                    model.Odometer = jobRequest.Odometer;
                    model.EquipmentType = jobRequest.EquipmentType;
                    //model.Mileage=jobRequest.VeheicleReceptionFormId
                    var vehicleReceptionForm = db.VehicleReceptionForm.Find(jobRequest.VeheicleReceptionFormId);
                    if (vehicleReceptionForm != null)
                    {
                        model.Mileage = vehicleReceptionForm.Mileage;
                        model.Model = vehicleReceptionForm.VehicleModel;
                        model.EquipmentType = vehicleReceptionForm.VehicleType;
                        var equipmentCategory = db.EquipmentIdentity.Find(vehicleReceptionForm.VehicleCategoryId);
                        if (equipmentCategory != null)
                        {
                            model.EquipmentCategory = equipmentCategory.EquipmentCategoryName;
                        }

                    }
                    var branch = db.Branch.Find(jobRequest.BranchId);
                    if (branch != null)
                    {
                        model.Branch = branch.Name;
                    }
                    var bussinesUnit = db.BusinessUnits.Find(jobRequest.BusinessUnitId);
                    if (bussinesUnit != null)
                    {
                        model.BusinessUnit = bussinesUnit.Name;
                    }
                    var organizationStructure = db.MainCompanyStructures.Find(jobRequest.OrgStructureId);
                    if (organizationStructure != null)
                    {
                        model.OrgStructure = organizationStructure.OrgBranchName;
                    }
                    var maintenanceCenter = db.MaintenanceCenter.Find(jobRequest.MaintenanceCenterId);
                    if (maintenanceCenter != null)
                    {
                        model.MaintenanceCenter = maintenanceCenter.Name;
                    }
                    var requester = db.Person_Employee.Find(jobRequest.RequesterId);
                    if (requester != null)
                    {
                        model.Requester = requester.FirstNameEnglish + " " + requester.MiddleNameEnglish + " " + requester.LastNameEnglish;
                    }
                    string customer;
                    if(jobRequest.CustomerId==null)
                    {
                        customer = "Internal";
                    }
                    else
                    {
                        customer = db.OrganizationCustomers.Find(jobRequest.CustomerId).TradeName;
                    }
                    model.Customer = customer;


                }
                //var vehicle = db.EquipmentInformation.Where(q => q.LicensePlate == jobOrder.LicensePlate).FirstOrDefault();                
                //if (vehicle != null)
                //{
                //    model.AssetNum = vehicle.DeviceNumber;
                //    model.EquipmentType = vehicle.GroupName;
                //    model.Mileage = vehicle.InitialOdometer;
                //}

                var jobServices = db.JobOrderDetail.Where(q => q.JobOrderID == jobOrder.JobOrderID).ToList();
                IList<JobCauseViewModel> jobCauseViewModels = new List<JobCauseViewModel>();
                if (jobServices.Any())
                {
                    int i = 1;
                    foreach (var jobService in jobServices)
                    {
                        JobCauseViewModel jobCauseViewModel = new JobCauseViewModel();
                        jobCauseViewModel.Description = jobService.Remark;
                        jobCauseViewModel.JobCause = db.JobCauses.Find(jobService.JobCausesId)?.Description;
                        jobCauseViewModel.No = i;
                        jobCauseViewModel.ServiceGroup = jobService.ServiceGroup;
                        jobCauseViewModel.SubCategoryCode = db.ItemCategorys.Find(jobService.SubCategoryCode)?.Name;
                        var service = db.JobOrderDetail.Find(jobService.JobOrderDetailID);
                        if (service != null)
                        {
                            var repairType = db.RepairType.Find(service.RepairTypeID);
                            if (repairType != null)
                            {
                                jobCauseViewModel.Service =repairType.Code+"-"+ repairType.Name;
                            }
                        }
                        jobCauseViewModels.Add(jobCauseViewModel);
                        i++;
                    }
                }
                model.JobCauseViewModels = jobCauseViewModels;
            }
            return model;
        }
    }
}

