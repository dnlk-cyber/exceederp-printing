using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using ExceedERP.Helpers;
using System.Linq;

using System.Drawing;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Reporting.Maintenance.DataSource
{
    [DataObject]
    public class ServiceJobOrderPrintOds
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
      //  ApplicationDbContext context = new ApplicationDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FormanDto> GetFormans(string id)
        {

            List<FormanDto> list = new List<FormanDto>();
            if (Guid.TryParse(id, out Guid jobOrderId))
            {
                var fetchExternalJobService = db.ServiceCenterJob.FirstOrDefault(x => x.ServiceCenterJobId.ToString() == id);
                var jobOrder = db.JobOrder.FirstOrDefault(x => x.JobOrderID == fetchExternalJobService.JobOrderID);
                if (jobOrder != null)
                {
                    var FetchJobServices = db.ServiceCenterJob.Where(x => x.JobOrderID.ToString() == id).ToList();
                   foreach(var jobService in FetchJobServices)
                    {
                        var Forman = new FormanDto();
                        Forman.FormanName = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(jobService.SecondOnlineApprovedBy));
                        Forman.FormanDate = jobService.SecondOnlineApprovedTime?.ToString("yyyy-MM-dd")??"";
                        list.Add(Forman);
                    }

                }

            }

            return list;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public JobOrderVm GetJobOrderPrint(string id)
        {

            JobOrderVm jobOrderVm = new JobOrderVm();
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

            jobOrderVm.LogoFileName = LogoFileName;
            jobOrderVm.Name = Name;
            jobOrderVm.AmharicName = AmharicName;
            if (Guid.TryParse(id, out Guid jobOrderId))
            {
                var fetchExternalJobService = db.ServiceCenterJob.FirstOrDefault(x => x.ServiceCenterJobId.ToString() == id);
                var jobOrder = db.JobOrder.FirstOrDefault(x => x.JobOrderID == fetchExternalJobService.JobOrderID);
                if (jobOrder != null)
                {
                    var jobRequest = db.JobRequest.FirstOrDefault(x => x.Num == jobOrder.Referance);
                    var jobDetail = db.JobOrderServiceCenterDetails.Where(x => x.ServiceCenterJobId == fetchExternalJobService.ServiceCenterJobId).ToList();
                    Person_Employee employee = new Person_Employee();
                    if(jobRequest!=null&&jobRequest.RequesterId!=0)
                    {  employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == jobRequest.RequesterId);
                    }

                    jobOrderVm.JobNum = jobOrder.Num;
                    // jobOrderVm. = jobOrder.;
                    jobOrderVm.PlateNumber = jobOrder.LicensePlate;
                    jobOrderVm.EquipmentName = jobOrder.EquipmentName;//
                    jobOrderVm.CustomerName = db.OrganizationCustomers.FirstOrDefault(x => x.OrganizationCustomerID == jobOrder.CustomerId)?.TradeName ?? "";
                    jobOrderVm.Requester = employee?.FirstNameEnglish + "-" + employee?.MiddleNameEnglish;
                    if(jobRequest!=null)
                    {
                        jobOrderVm.Branch = db.Branch.FirstOrDefault(x => x.BranchId.ToString() == jobRequest.FromBranchId)?.Name ?? "";
                    }
                    jobOrderVm.FromBranchName = "";                    
                    if (jobRequest!=null&&jobRequest.FromBranchId!=null)
                    {
                        var getBranch = db.Branch.FirstOrDefault(x => x.BranchId.ToString() == jobRequest.FromBranchId);
                        jobOrderVm.FromBranchName =getBranch.Name;
                    }
                    jobOrderVm.BusinessUnit = "";
                    jobOrderVm.MaintenanceCenter = db.MaintenanceCenter.FirstOrDefault(x => x.MaintenanceCenterID == jobOrder.MaintenanceCenterId)?.Name ?? "";
                    jobOrderVm.DateCreated =null;
                    if (jobOrder.DateCreated!=null)
                    {
                        jobOrderVm.DateCreated = jobOrder.DateCreated;
                    }
                    //jobOrderVm.DateClosed = jobOrder.DateCreated;
                    jobOrderVm.childClassVms = new List<childClassVM>();
                    if (jobDetail.Any())
                    {
                        foreach (var childVM in jobDetail)
                        {
                            var chclass = new childClassVM();
                            chclass.VehicleCategory = db.ItemCategorys.FirstOrDefault(x => x.ItemCategoryCode == childVM.SubCategoryCode)?.Name;
                            chclass.Cause = db.JobCauses.FirstOrDefault(x => x.JobCausesId == childVM.JobCausesId)?.Name;
                            chclass.ServiceGroup = nameof(childVM.ServiceGroup);
                            chclass.Description = db.JobOrderDetail.FirstOrDefault(x=>x.RepairTypeID==childVM.RepairTypeID&&x.JobOrderID==jobOrder.JobOrderID)?.DescriptionOfJobs??"";
                            chclass.ServiceName = db.RepairType.FirstOrDefault(x => x.RepairTypeID == childVM.RepairTypeID)?.Name??"";
                            chclass.ServiceCenter = db.ServiceCenters.FirstOrDefault(x => x.ServiceCenterId == childVM.ServiceCenterId)?.Name;
                            jobOrderVm.childClassVms.Add(chclass);
                        }
                    }
                    if (jobRequest != null)
                    {
                        jobOrderVm.KMReading = jobRequest.KMReading;
                        jobOrderVm.GPSInfo = jobRequest.GPSInfo;
                    }
                    jobOrderVm.MaintenanceRequestApproval = "";
                    if (jobRequest != null && jobRequest.OnlineApprovedBy != null)
                    {
                        jobOrderVm.MaintenanceRequestApproval = jobRequest.OnlineApprovedBy;
                        jobOrderVm.RequestedBy = employee?.FirstNameEnglish + "-" + employee?.MiddleNameEnglish;
                        jobOrderVm.CreatedBySignature = SignatureHelper.GetUserSignature(jobRequest.OnlineApprovedBy);
                    }
                    //jobOrderVm.RequestedBy = employee?.FirstNameEnglish + "-" + employee?.MiddleNameEnglish;
                    //jobOrderVm.CreatedBySignature = SignatureHelper.GetUserSignature(jobRequest.OnlineApprovedBy);
                    jobOrderVm.ApproveDate = null;
                    if (jobOrder != null)
                    {
                        jobOrderVm.TechnicalApproval = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(jobOrder.OnlineApprovedBy));
                        jobOrderVm.ApprovedBySignature = SignatureHelper.GetUserSignature(jobOrder.OnlineApprovedBy);
                      
                    }

                 
                    if (jobOrder.SecondOnlineApprovedTime!=null)
                    {
                        jobOrderVm.ApproveDate = jobOrder.OnlineApprovedTime;
                    }
                    jobOrderVm.CheckedDate = null;
                    if (jobRequest != null)
                    {
                        jobOrderVm.CheckedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(jobRequest.OnlineApprovedBy));
                        jobOrderVm.CheckedBySignature = SignatureHelper.GetUserSignature(jobRequest.OnlineApprovedBy);
                    }
                    if (jobRequest.OnlineApprovedTime != null)
                    {
                        jobOrderVm.CheckedDate = jobOrder.OnlineApprovedTime;
                    }
                    jobOrderVm.RequestDate = null;
                    if(jobRequest != null && jobRequest.DateCreated!=null)
                    {
                        jobOrderVm.RequestDate = jobRequest.DateCreated;
                    }
                    if (jobRequest != null&&jobRequest.OnlineApprovedBy!=null)
                    {
                        jobOrderVm.InspectorName = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(jobRequest.OnlineApprovedBy));
                        jobOrderVm.ApprovedBySignature = SignatureHelper.GetUserSignature(jobRequest.OnlineApprovedBy);
                        jobOrderVm.InspectorDate = jobRequest.OnlineApprovedTime?.ToString("yyyy-MM-dd") ?? "";
                    }
                    
                    if (fetchExternalJobService != null)
                    {
                        jobOrderVm.AuthorizerName = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(fetchExternalJobService.SecondOnlineApprovedBy));
                        jobOrderVm.AuthorizedBySignature = SignatureHelper.GetUserSignature(fetchExternalJobService.SecondOnlineApprovedBy);
                    }
                    if (fetchExternalJobService.SecondOnlineApprovedTime != null)
                    {
                        jobOrderVm.AuthorizerDate = fetchExternalJobService.SecondOnlineApprovedTime?.ToString("yyyy-MM-dd") ?? "";
                    }
                    

                }

            }
            return jobOrderVm;
        }
       
    }
   
    //public class childClassVM
    //    {
    //        public string VehicleCategory { get; set; }
    //        public string Cause { get; set; }
    //        public string ServiceGroup { get; set; }
    //        public string ServiceName { get; set; }
    //        public string ServiceCenter { get; set; }
    //        public string MechanicName { get; set; }
    //    }
        
    }

