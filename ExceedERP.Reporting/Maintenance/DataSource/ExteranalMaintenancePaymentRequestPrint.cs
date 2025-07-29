using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using ExceedERP.Helpers.Common;

using System.Threading.Tasks;
using System.Drawing;

namespace ExceedERP.Reporting.Maintenance.DataSource
{
    [DataObject]
   public class ExteranalMaintenancePaymentRequestPrint
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ExteranalMaintenancePaymentVM> GetPayments(string Id)
        {
            List<ExteranalMaintenancePaymentVM> list = new List<ExteranalMaintenancePaymentVM>();
            var fetchPayedJobs = db.ExternalPaymentRequestJobs.Where(x => x.ExternalMaintenancePaymenntrequestId.ToString() == Id).ToList();
            foreach(var PayedJob in fetchPayedJobs)
            {
                decimal TotalPayed = 0;
                ExteranalMaintenancePaymentVM paymentVM = new ExteranalMaintenancePaymentVM();
                var fetchServiceJob = db.ServiceCenterJob.FirstOrDefault(x => x.ServiceCenterJobId.ToString() == PayedJob.ServiceCenterJobId);
                if(fetchServiceJob!=null)
                {
                    paymentVM.PlateNumber = fetchServiceJob.LicensePlate;
                    var fetchJobOrder = db.JobOrder.FirstOrDefault(x => x.JobOrderID.ToString() == fetchServiceJob.JobOrderID.ToString());
                     if(fetchJobOrder!=null&&fetchJobOrder.Num!=null)
                    {
                        var fetchRequest = db.JobRequest.FirstOrDefault(x => x.Num == fetchJobOrder.Referance);
                        if(fetchRequest!=null)
                        {
                            var fetchPlacement = db.Placements.FirstOrDefault(x => x.EmployeeId == fetchRequest.RequesterId);
                            if(fetchPlacement!=null&&fetchPlacement.BusinessUnitId!=null)
                            {
                                paymentVM.BusinessUnit = db.BusinessUnits.FirstOrDefault(x => x.BusinessUnitId == fetchPlacement.BusinessUnitId)?.Name ?? "";
                            }
                            paymentVM.Branch = db.Branch.FirstOrDefault(x => x.BranchId.ToString() == fetchRequest.FromBranchId)?.Name ?? "";
                        }

                    }
                    paymentVM.Invoice = db.ExternalServiceCenterPayments.FirstOrDefault(x => x.ServiceCenterJobId.ToString() == PayedJob.ServiceCenterJobId.ToString())?.Invoice ?? "";
                    paymentVM.RequestDate = db.ExternalServiceCenterPayments.FirstOrDefault(x => x.ServiceCenterJobId.ToString() == PayedJob.ServiceCenterJobId.ToString())?.Date.ToString("yyyy-MM-dd") ?? "";
                    paymentVM.EquipmentName = fetchServiceJob.EquipmentName;
                }
                paymentVM.JobOrder = db.ExternalMaintenancePaymentRequests.FirstOrDefault(x => x.ExternalMaintenancePaymenntrequestId.ToString() == Id)?.JobNum.ToString() ?? "";
                var fetchPaymentsUnderJob = db.ExternalMaintenancePaymentRequestDetails.Where(x => x.ExternalPaymentRequestJobId == PayedJob.ExternalPaymentRequestJobId).ToList();
                   foreach(var job in fetchPaymentsUnderJob)
                {
                    paymentVM.TotalPerJob += job.Total;
                }
                list.Add(paymentVM);
            }


            return list;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public ExteranalMaintenancePaymentVM GetAuthorizers(string Id)
        {
            ExteranalMaintenancePaymentVM authorizers = new ExteranalMaintenancePaymentVM();
            var fetchAuthorizers = db.ExternalMaintenancePaymentRequests.FirstOrDefault(x => x.ExternalMaintenancePaymenntrequestId.ToString() == Id);
            if (fetchAuthorizers != null)
            {
                authorizers.CreatedBy = "";
                if (fetchAuthorizers.CreatedBy != null)
                {
                    authorizers.CreatedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(fetchAuthorizers.CreatedBy));
                    authorizers.CreatedBySignature = SignatureHelper.GetUserSignature(fetchAuthorizers.CreatedBy);
                }
                authorizers.CheckedDate = null;
                if (fetchAuthorizers.DateCreated != null)
                {
                    authorizers.CreatedDate = fetchAuthorizers.DateCreated.Value.Date.ToString("yyyy-MM-dd");
                }
                authorizers.CheckedDate = null;
                if (fetchAuthorizers.SendForApprovalTime != null)
                {
                    authorizers.CheckedDate = fetchAuthorizers.SendForApprovalTime.Value.Date.ToString("yyyy-MM-dd");
                }
                authorizers.CheckedBy = "";
                if (fetchAuthorizers.SendForApprovalBy != null)
                {
                    authorizers.CheckedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(fetchAuthorizers.SendForApprovalBy));
                    authorizers.CheckedBySignature = SignatureHelper.GetUserSignature(fetchAuthorizers.SendForApprovalBy);
                }
                authorizers.ApprovedDate = null;
                if (fetchAuthorizers.OnlineApprovedTime != null)
                {
                    authorizers.ApprovedDate = fetchAuthorizers.OnlineApprovedTime.Value.Date.ToString("yyyy-MM-dd");
                }
                authorizers.ApprovedBy = "";
                if (fetchAuthorizers.OnlineApprovedBy != null)
                {
                    authorizers.ApprovedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(fetchAuthorizers.OnlineApprovedBy));
                    authorizers.ApprovedBySignature = SignatureHelper.GetUserSignature(fetchAuthorizers.OnlineApprovedBy);
    }


                authorizers.AuthorizationDate = null;
                if (fetchAuthorizers?.DepartmentOnlineApprovedTime != null)
                {
                    authorizers.AuthorizationDate = fetchAuthorizers.DepartmentOnlineApprovedTime.Value.Date.ToString("yyyy-MM-dd");
                }
                authorizers.AuthorizedBy = "";
                if (fetchAuthorizers.DepartmentOnlineApprovedBy != null)
                {
                    authorizers.AuthorizedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(fetchAuthorizers.DepartmentOnlineApprovedBy));
                    authorizers.AuthorizedBySignature = SignatureHelper.GetUserSignature(fetchAuthorizers.DepartmentOnlineApprovedBy);
                }
            }

            return authorizers;

        }
    }
    public class ExteranalMaintenancePaymentVM
    {
        public string JobOrder { get; set; }
        public string PlateNumber { get;set; }
        public string EquipmentName { get; set; }
        public string BusinessUnit { get; set; }
        public string Branch { get; set; }
        public string Invoice { get; set; }
        public string RequestDate { get; set; }
        public decimal TotalPerJob { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public string ApprovedBy { get; set; }
        public string ApprovedDate { get; set; }
        public string CheckedBy { get; set; }
        public string CheckedDate { get; set; }
        public string AuthorizedBy { get; set; }
        public string AuthorizationDate { get; set; }
        public Image ApprovedBySignature { get; set; }
        public Image CreatedBySignature { get; set; }
        public Image CheckedBySignature { get; set; }
        public Image AuthorizedBySignature { get; set; }

    }

}
