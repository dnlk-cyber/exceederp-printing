using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
 using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using ExceedERP.DataAccess.Context;
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

namespace ExceedERP.Reporting.Fleet.DataSource
{


          [DataObject]
    public class JobDataSource : List<JobOrderView>
    {
              private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
              public List<JobOrderView> GetJobList(string oId)
        {
            IJobOrderDetailService service=new JobOrderDetailService();
            IJobOrderService JService=new JobOrderService();
            IFleetService fservice= new FleetService();
            IOperatorService oservice = new OperatorService();
            List<JobOrderView> salesOrderViewList = new List<JobOrderView>();
            JobOrderView OrderViewModel;
            List<ExceedERP.Core.Domain.FleetManagement.FleetMaintenance.JobOrder> Job= new List<ExceedERP.Core.Domain.FleetManagement.FleetMaintenance.JobOrder>();;
            var Fleets = fservice.GetAll();
            var operators = oservice.GetAll();



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




            if (!string.IsNullOrEmpty(oId))
            {
                int OrderNum = int.Parse(oId);
              Job = JService.GetAll().Where(so => so.Num == OrderNum).ToList();
      
            }


            string ChassisNum = "Not Set";
            string EngineNum = "Not Set";
            string OdoMeter = "Not Set";
            string Equipment = "External Equipment";
              foreach (var sov in Job)
            {
                var Requests = db.JobRequest.Where(x => x.Num == sov.Referance).FirstOrDefault();

                List<JobOrderDetail> lineItems = new List<JobOrderDetail>();
                  if(Requests != null)
                  {
                      lineItems = service.GetAll().Where(x => x.JobOrderID == sov.JobOrderID || x.JobOrderID == Requests.JobRequestId).ToList();
                  }
                  else
                  {
                      lineItems = service.GetAll().Where(x => x.JobOrderID == sov.JobOrderID).ToList();
                  }
               

                foreach (JobOrderDetail l in lineItems)
                {

                    int uOMID = 0;
                    string uOM = "";
                    var fleet = Fleets.Where(y => y.FleetsID == sov.FleetsID).FirstOrDefault(); //== l.ThingID).FirstOrDefault();
                    var ops = operators.Where(x => x.FleetsID == sov.FleetsID).FirstOrDefault();
                    string joperator = "";
                   
                    if (ops != null)
                    {
                        joperator = ops.FullName;
                    }

                    if(fleet !=null)
                    {
                        Equipment=fleet.EquipmentName + " - " + fleet.PlateNumber;
                        ChassisNum= fleet.ChasisNumber;
                        EngineNum =fleet.EngineNumber;
                        joperator = fleet.OperatorName;
                        OdoMeter= fleet.CurentOdoMeter.ToString();
                    }
                    string ApprovalStatus = Enum.GetName(l.ApprovalStatus.GetType(), l.ApprovalStatus); 
                    OrderViewModel = new JobOrderView();
                    OrderViewModel.JobOrderNum = sov.JobOrderNum;
                    OrderViewModel.ProjectName =Equipment;
                    OrderViewModel.Driver = joperator;
                    OrderViewModel.Type = sov.Type;
                    OrderViewModel.ChassisNumber = ChassisNum;
                    OrderViewModel.EngineNumber = EngineNum;
                    OrderViewModel.odometer =OdoMeter;
                    OrderViewModel.InspectedBy = sov.InspectedBy;
                    OrderViewModel.MaintenanceCenter = sov.MaintenanceCenter;
                    OrderViewModel.DateCompleted = sov.DateCompleted;
                    OrderViewModel.TecnicallyApproved = sov.SecondOnlineApprovedBy;
                    OrderViewModel.AdminstrativeApproved = sov.OnlineApprovedBy;
                    OrderViewModel.ServiceName = l.ServiceName;
                    OrderViewModel.Status = l.Status;
                    OrderViewModel.ApprovalStatus = ApprovalStatus;


                    OrderViewModel.AmharicName = AmharicName;
                    OrderViewModel.Name = Name;
                    OrderViewModel.Address = Address;
                    OrderViewModel.LogoFileName = LogoFileName;

                    salesOrderViewList.Add(OrderViewModel);
                }
            }
            return salesOrderViewList;
        }


    }
}








