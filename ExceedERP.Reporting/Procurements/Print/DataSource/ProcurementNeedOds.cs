using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Procurement.Need;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.Procurements.Print.VM;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.Procurements.Print.Print
{
    [DataObject]
    public class ProcurementNeedOds
    {
        ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public ProcurementNeedHeaderVm GetProcurementNeed(int id)
        {
            var needAssesment = _db.AnnualNeedAssesments.Find(id);
            var organization = _db.Companies.FirstOrDefault(x => x.Category == "0");

            ProcurementNeedHeaderVm headerDs = new ProcurementNeedHeaderVm();
            if (needAssesment != null)
            {
                headerDs.VoucherId = needAssesment.AnnualNeedAssesmentID.ToString();

                headerDs.Year = _db.GLFiscalYears.Find(needAssesment.GlFiscalYearId)?.Name;
                headerDs.Branch = _db.Branch.Find(needAssesment.BranchId)?.Name;
                headerDs.WorkUnit = _db.MainCompanyStructures.Find(needAssesment.OrgStructureId)?.Name;
                headerDs.Requester = EmployeeHelper.GetEmployeeEnglishName(needAssesment.EmployeeId);
                headerDs.Remark = needAssesment.Remark;

                headerDs.CreatedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(needAssesment.CreatedBy));
                if (needAssesment.DateCreated != null)
                    headerDs.CreateDate = needAssesment.DateCreated != null ? needAssesment.DateCreated.ToString() : "";
                headerDs.Approver =
                    EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(needAssesment.OnlineApprovedBy));
                headerDs.ApprovedDate = needAssesment.OnlineApprovedTime != null ? needAssesment.OnlineApprovedTime.ToString() : "";


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
        public List<AnnualNeedAssesmentItem> NeedItems(int id)
        {
            return _db.AnnualNeedAssesmentItems.Where(x => x.AnnualNeedAssesmentID == id).ToList();
           
        }

       
    }
}