using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Fleet;
using ExceedERP.Reporting.Fleet.FleetMaintenance.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  ExceedERP.Reporting.Fleet.FleetMaintenance.DataSource
{

    [DataObject]
    public class ServiceTypeDataSource
    {

        ExceedDbContext db = new ExceedDbContext();
        IJobOrderService _JobOrderService = new JobOrderService();
        IServiceService _ServiceService = new ServiceService();


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<WorkUnitSection> GetWorkUnitSection()
        {


            db = new ExceedDbContext();
            var result = db.WorkUnitSection.ToList();

            return result;

        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ServiceTypeViewModel> GetServiceType(DateTime StartingDate, DateTime EndingDate, string WorkUnitSection, string Category, string Type, string EquipmentName, string PlateNo, string Model, string ChasisNo, string Serial, string Location)
        {
            List<ServiceTypeViewModel> ServiceList = new List<ServiceTypeViewModel>();
            ServiceTypeViewModel JobOrderModel;
            GLPeriod Periods = new GLPeriod();



            #region SharedResource
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





 
            Guid RequestId = Guid.Empty;
            List<JobOrder> jobs = db.JobOrder.Where(x => x.DateIssued >= StartingDate && x.DateIssued <= EndingDate && x.IsOnlineTransferred == true).ToList();

            foreach (var job in jobs)
            {
                if (FleetGenericHelper.FilterSingleFleet(job.FleetsID, Category, Type, PlateNo, ChasisNo, Serial, string.Empty, Location, Model))
                {
                var Request = db.JobRequest.Where(x => x.Num == job.Referance).FirstOrDefault();
                if (Request != null)
                {
                    RequestId = Request.JobRequestId;
                }

                List<JobOrderDetail> Services = db.JobOrderDetail.Where(x => x.JobOrderID == job.JobOrderID || x.JobOrderID == RequestId).ToList();

             
               
                foreach (var Service in Services)
                {

                    decimal STD = 0;
                    decimal Durations = 0;
                    string Section = string.Empty;

                    if (Service.IsOnlineApproved)
                    {
                        TimeSpan Duration = DateTime.Parse(Service.OnlineApprovedTime.ToString()) - DateTime.Parse(job.OnlineApprovedTime.ToString());
                        Durations = (decimal)Duration.TotalHours;
                    }
                    var Inspection = _ServiceService.GetById(Service.ServiceID);
                    if (Inspection != null)
                    {
                        var Sections = db.WorkUnitSection.Where(x => x.WorkUnitSectionId == Inspection.SectionId).FirstOrDefault();
                        if (Sections != null)
                        {
                            Section = Sections.Name;
                        }
                        if (Inspection.STDTime != null)
                        {
                            STD = (decimal)Inspection.STDTime;
                        }
                    }
          
                    JobOrderModel = new ServiceTypeViewModel();

                    JobOrderModel.ActualTime = Durations;

                    if (Durations == 0)
                    {
                        Durations = 1;
                    }

                    JobOrderModel.Efficency = (STD / Durations) * 100;
                    JobOrderModel.STD = STD;
                    JobOrderModel.Equipment = job.EquipmentName;
                    JobOrderModel.ServiceName = Service.ServiceName;
                    JobOrderModel.Section = Section;


                    JobOrderModel.AmharicName = AmharicName;
                    JobOrderModel.Name = Name;
                    JobOrderModel.Address = Address;
                    JobOrderModel.LogoFileName = LogoFileName;



                         ServiceList.Add(JobOrderModel);

                }
            }


            }




            
               if(!string.IsNullOrEmpty( WorkUnitSection))
                {
                    return ServiceList.Where(x => x.Section == WorkUnitSection).ToList();
                }

            if(!ServiceList.Any())
            {
                  JobOrderModel = new ServiceTypeViewModel();
                JobOrderModel.AmharicName = AmharicName;
                JobOrderModel.Name = Name;
                JobOrderModel.Address = Address;
                JobOrderModel.LogoFileName = LogoFileName;



                ServiceList.Add(JobOrderModel);
            }
               return ServiceList;
            
        }





    }
}

