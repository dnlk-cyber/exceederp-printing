using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent;
using ExceedERP.Reporting.Fleet.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.DataAccess.Context;
using System.Web.Mvc;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Finance.GL;

namespace ExceedERP.Reporting.Fleet.DataSource
{
    [DataObject]
    public class RentRequestDataSource
    {

        ExceedDbContext db = new ExceedDbContext();



        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SelectListItem> GetCategory()
        {

            var result = EnumExtension.EnumToSelectList(typeof(ExceedERP.Core.Domain.FleetManagement.FleetResource.VehicleDistribution.RequestType));

            return result;

        }
                [DataObjectMethod(DataObjectMethodType.Select)]
        public List<Helpers.Fleet.FleetBindingHelper.HelperObject> GetBranch()
        {

            var result = db.Branch.Select(s => new Helpers.Fleet.FleetBindingHelper.HelperObject
            {
                Name = s.Name,
                Value = s.BranchId.ToString()
            }).ToList();

            return result;

        }

                [DataObjectMethod(DataObjectMethodType.Select)]
                public List<Helpers.Fleet.FleetBindingHelper.HelperObject> GetStructure()
                {

                    var result = db.MainCompanyStructures.Select(s => new Helpers.Fleet.FleetBindingHelper.HelperObject
                    {
                        Name = s.Name,
                        Value = s.OrgStructureId.ToString()
                    }).ToList();

                    return result;

                }
                [DataObjectMethod(DataObjectMethodType.Select)]
                public List<SelectListItem> GetEquipmentStatus()
                {

                    var result = EnumExtension.EnumToSelectList(typeof(ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent.RentRequest.EquipmentStatusenum));

                    return result;

                }

        [DataObjectMethod(DataObjectMethodType.Select)]
                public List<RentRequestViewModel> GetEquipmentRequest(string FiscalYear, string Period, string Branch, string Type, string Status)
        {
            List<RentRequestViewModel> RequestList = new List<RentRequestViewModel>();

            GLPeriod GLPeriod = db.GLPeriods.Where(x => x.GLPeriodId.ToString() == Period && x.GlFiscalYearId.ToString() == FiscalYear).FirstOrDefault();

        
            if (GLPeriod != null)
            {
                DateTime FromDate = GLPeriod.DateFrom;
                DateTime ToDate = GLPeriod.DateFrom;


                if (Type.Equals("Rent", StringComparison.OrdinalIgnoreCase))
                {
                    List<RentRequest> Requests = new List<RentRequest>();

                    if (!string.IsNullOrEmpty(Branch))
                    {
                        Requests = db.RentRequest.Where(x => x.FromBranchName == Branch && x.DateCreated > FromDate && x.DateCreated < ToDate).ToList();

                    }





                    foreach (var c in Requests)
                    {
                        List<RequestedEquipment> equipments = db.RequestedEquipments.Where(x => x.RequestNumber == c.RentRequestID).ToList();

                        if (!string.IsNullOrEmpty(Status))
                        {
                            equipments = equipments.Where(x => x.EquipmentStatus == Status).ToList();

                        }



                        if (equipments.Any())
                        {
                            foreach (var e in equipments)
                            {
                                TimeSpan duration = TimeSpan.Zero;
                                     AssignedEquipment Assigned = db.AssignedEquipment.Where(x => x.EquipmentID == e.RequestedEquipmentId).FirstOrDefault();

                                if(Assigned != null )
                                {
                                    duration = (DateTime.Parse( Assigned.DateCreated.ToString() )- DateTime.Parse( e.DateCreated.ToString()));
                                }
                                var RequestModel = new RentRequestViewModel();

                                RequestModel.Description = c.Description;
                                RequestModel.FromBranchName = c.FromBranchName;
                                RequestModel.FromStructureName = c.FromStructureName;
                                RequestModel.RequestQuantity = e.Quantity;
                                RequestModel.StartingDate = e.StartDate;
                                RequestModel.ToDate = e.EndDate;
                                RequestModel.Type = e.Type;
                                RequestModel.Category = e.EquipmentName;
                                RequestModel.EquipmnetStatus = e.EquipmentStatus;
                                RequestModel.RequestedDate = e.DateCreated;
                                RequestModel.AssignedDuration = duration.TotalDays;

                                 
                                RequestList.Add(RequestModel);
                            }
                        }
                    }
                }
                else if (Type.Equals("Company", StringComparison.OrdinalIgnoreCase))
                {
                    List<VehicleDemand> Requests = new List<VehicleDemand>();

                  if (!string.IsNullOrEmpty(Branch))
                    {
                        Requests = db.VehicleDemand.Where(x => x.FromBranchName == Branch && x.DateCreated > FromDate && x.DateCreated < ToDate).ToList();

                    }

  

                    foreach (var c in Requests)
                    {
                        List<RequestedEquipment> equipments = db.RequestedEquipments.Where(x => x.RequestNumber == c.VehicleDemandId).ToList();

                        if (!string.IsNullOrEmpty(Status))
                        {
                            equipments = equipments.Where(x => x.EquipmentStatus == Status).ToList();

                        }


                        if (equipments.Any())
                        {
                            foreach (var e in equipments)
                            {
                                TimeSpan duration = TimeSpan.Zero;
                                AssignedEquipment Assigned = db.AssignedEquipment.Where(x => x.EquipmentID == e.RequestedEquipmentId).FirstOrDefault();

                                if (Assigned != null)
                                {
                                    duration = (DateTime.Parse(Assigned.DateCreated.ToString()) - DateTime.Parse(e.DateCreated.ToString()));
                                }

                                var RequestModel = new RentRequestViewModel();

                                RequestModel.FromBranchName = c.FromBranchName;
                                RequestModel.FromStructureName = c.FromStructureName;
                                RequestModel.RequestQuantity = e.Quantity;
                                RequestModel.StartingDate = e.StartDate;
                                RequestModel.ToDate = e.EndDate;
                                RequestModel.Type = e.Type;
                                RequestModel.Category = e.EquipmentName;
                                RequestModel.EquipmnetStatus = e.EquipmentStatus;
                                RequestModel.RequestedDate = e.DateCreated;
                                RequestModel.AssignedDuration = duration.TotalDays;


                      
                                RequestList.Add(RequestModel);
                            }
                        }
                    }
                }
            }
            return RequestList;

        }
    }
}
