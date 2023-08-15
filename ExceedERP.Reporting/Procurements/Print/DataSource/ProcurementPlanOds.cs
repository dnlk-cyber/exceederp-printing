using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Procurement.Plan;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.Procurements.Print.VM;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.Procurements.Print.Print
{
    [DataObject]
    public class ProcurementPlanOds
    {
        ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public ProcurementPlanHeaderVm GetProcurementPlan(int id)
        {
            var procurementPlan = _db.ProcurementPlans.Find(id);
            var organization = _db.Companies.FirstOrDefault(x => x.Category == "0");

            ProcurementPlanHeaderVm headerDs = new ProcurementPlanHeaderVm();
            if (procurementPlan != null)
            {
                headerDs.VoucherId = procurementPlan.ProcurementPlanId.ToString();
              
                headerDs.Year = _db.GLFiscalYears.Find(procurementPlan.GlFiscalYearId)?.Name;
                headerDs.Branch = _db.Branch.Find(procurementPlan.BranchId)?.Name;
                headerDs.Description = procurementPlan.Name;
                headerDs.Remark = procurementPlan.Remark;

                headerDs.CreatedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(procurementPlan.CreatedBy));
                if (procurementPlan.DateCreated != null)
                    headerDs.CreateDate = procurementPlan.DateCreated != null ? procurementPlan.DateCreated.ToString() : "";
                headerDs.Approver =
                    EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(procurementPlan.OnlineApprovedBy));
                headerDs.ApprovedDate = procurementPlan.OnlineApprovedTime != null ? procurementPlan.OnlineApprovedTime.ToString() : "";
                

            }


            if (organization != null)
            {
                headerDs.Name = organization.TradeName;
                headerDs.AmharicName = organization.AmharicName;
                var address =
                    _db.Addresses.Where(
                        a =>
                            a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType.ToLower() == "company").ToList();
                if (address.Count > 0)
                {
                    foreach (var a in address)
                    {

                        if (headerDs.Address == null)
                        {
                            headerDs.Address = a.Category + ":- " + a.Value;
                        }
                        else
                        {
                            headerDs.Address += ", " + a.Category + "-" + a.Value;
                        }

                    }

                }
                var logo =
                    _db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                                                              a.ReferenceType == AttachmentType.LOGO);
                if (logo != null)
                {
                    headerDs.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }
            return headerDs;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ProcurementPlanItem> PlanItems(int id)
        {
           return _db.ProcurementPlanItems.Where(x => x.ProcurementPlanId == id).ToList();
           
        }

        
    }
}