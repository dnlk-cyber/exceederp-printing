using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Fleet.FleetMaintenance.Printing.ViewModel;
using ExceedERP.Reporting.Fleet.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JobRequestViewModel = ExceedERP.Reporting.Fleet.FleetMaintenance.Printing.ViewModel.JobRequestViewModel;


namespace ExceedERP.Reporting.Fleet.FleetMaintenance.Printing.DataSource
{


    [DataObject]
    public class JobRequestDataSource : List<JobRequestViewModel>
    {

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<JobRequestViewModel> GetJobRequest(string Id)
        {
            ExceedDbContext db = new ExceedDbContext();
            JobRequest JobRequest = new JobRequest();
            IFleetService _FleetSerivce = new FleetService();
            List<JobRequestViewModel> JobRequestList = new List<JobRequestViewModel>();


            #region Shared Company DS
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


            if (!string.IsNullOrEmpty(Id))
            {
                JobRequest = db.JobRequest.Where(x => x.JobRequestId.ToString() == Id).FirstOrDefault();


            }




            var JobRequestDetails = db.JobCauses.Where(x => x.OrderId == JobRequest.JobRequestId).ToList();




            foreach (var JobRequestDetail in JobRequestDetails)
            {

                var model = new JobRequestViewModel();


                if (JobRequest.FleetsID != null)
                {
                    var Fleet = _FleetSerivce.GetById(JobRequest.FleetsID);
                    if (Fleet != null)
                    {

                        var Asset = Fleet;// db.AssetRegistrations.Where(x => x.Tag == JobRequest.AssetNum).FirstOrDefault();
                        if (Asset != null)
                        {
                            model.ProjectName = Fleet.Location;
                            model.Manufacture = Asset.Manufacturer;
                            model.Model = Asset.Model;
                            model.AssetNum = Fleet.UnitNum;
                            model.OperatorName = Fleet.OperatorName;
                            model.Odometer = Fleet.CurentOdoMeter;
                            model.OperatorName = Fleet.OperatorName;
                            model.EquipmentName = Asset.SerialNo;
                            model.FuelAmount = JobRequest.FuelAmount;
                            model.PlateNumber = Asset.PlateNumber;
                            model.EquipmentName = Asset.EquipmentName;
                            model.LastKMHR = JobRequest.Odometer;
                            model.EquipmentType = Fleet.EquipmentType.EquipmentTypeName;
                        }
                    }
                }
                model.Num = JobRequest.Num;
                model.Requester = JobRequest.Requester;
                model.LastKMHR = JobRequest.Odometer;
                model.RequestedBy = JobRequest.CreatedBy;
                model.ApprovedBy = JobRequest.OnlineApprovedBy;
                model.IsM = !JobRequest.IsPM;
                model.IsPM = JobRequest.IsPM;
                model.Cause = JobRequestDetail.Name;
                model.CauseDescription = JobRequestDetail.Description;

                model.Status = JobRequest.Status;

                model.AmharicName = AmharicName;
                model.Name = Name;
                model.Address = Address;
                model.LogoFileName = LogoFileName;

                JobRequestList.Add(model);
            }

            return JobRequestList;
        }



    }

}








