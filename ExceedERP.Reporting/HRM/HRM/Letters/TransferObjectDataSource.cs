using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.HRM.HRM.Letters
{
    [DataObject]
    public class TransferObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public TransferVM GetEmployeeJob(string Id, object[] letthem, object[] transcript, bool haveFixedAsset, bool header)
        {

            TransferVM EmployeeInfo = new TransferVM();
            db = new ExceedDbContext();

            var ranks = db.Ranks.ToList();
            db = new ExceedDbContext();
            var lookups = db.Lookups.ToList();
            var organization = db.Companies.FirstOrDefault(x => x.Category == "0");//CompanyCategory.MAIN_COMPANY);
            var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeCode == Id && x.IsActive);

            if (employee != null)
            {
                EmployeeInfo = new TransferVM();
                EmployeeInfo.Currents = new Current();
                EmployeeInfo.Previouses = new Previous();
                if (header)
                {
                    if (organization != null)
                    {
                        EmployeeInfo.Name = organization.TradeName;
                        EmployeeInfo.AmharicName = organization.AmharicName;
                        EmployeeInfo.Abbreviation = organization.Abbreviation;

                        var logo =
                            db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                                    a.ReferenceType == AttachmentType.LOGO);
                        if (logo != null)
                        {
                            EmployeeInfo.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                        }
                    }
                }
                    EmployeeInfo.Date = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
                

                EmployeeInfo.Sex = HrHelper.GetGender(employee.Gender);
                EmployeeInfo.FullName = HrHelper.GetEmployeeFullName(employee.EmployeeId); ;
                EmployeeInfo.EmployeeId = employee.EmployeeCode;
                var placements = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == employee.EmployeeId && x.AssignmentType == AssignmentType.TRANSFER);
                if (placements != null)
                {


                    var firstOrDefault = ranks.FirstOrDefault(x => x.RankId == placements.RankId);
                    if (firstOrDefault != null)
                        EmployeeInfo.Currents.CurrentRank = firstOrDefault.RankName;
                    var orDefault = lookups.FirstOrDefault(x => x.LookupId == placements.LookupId);
                    if (orDefault != null)
                        EmployeeInfo.Currents.CurrentJobTitle = orDefault.Description;

                    EmployeeInfo.Currents.CurrentSalary = placements.Salary;
                    var deptt = db.MainCompanyStructures
                        .FirstOrDefault(x => x.OrgStructureId == placements.OrgStructureId);
                    if (deptt != null) EmployeeInfo.Currents.CurrentDepartement = deptt.Name;

                    EmployeeInfo.AssignmentDate = placements.AssignmentDate.ToShortDateString();
                    var branch = db.Branch
                       .FirstOrDefault(x => x.BranchId == placements.BranchId);
                    if (branch != null) EmployeeInfo.Currents.Branch = branch.AmharicName;
                    EmployeeInfo.Currents.CurrentType = HrHelper.GetEmployeeStatus(placements.EmployementType);
                    //EmployeeInfo.Currents.Remark = placements.Remark;
                    if (haveFixedAsset)
                    {
                        EmployeeInfo.Currents.HaveFixedAsset = "አለ";
                    }
                    else
                    {
                        EmployeeInfo.Currents.HaveFixedAsset = "የለም";
                    }
                }
                var check =
                    db.Placements.Where(x => x.isCurrent == false && x.EmployeeId == employee.EmployeeId).ToList();
                if (check.Any())
                {
                    var prevPlaceId = check.Max(x => x.PlacementId);



                    var prevPlacements = db.Placements.FirstOrDefault(x => x.PlacementId == prevPlaceId);
                    if (prevPlacements != null)
                    {
                        var firstOrDefault = ranks.FirstOrDefault(x => x.RankId == prevPlacements.RankId);
                        if (firstOrDefault != null)
                            EmployeeInfo.Previouses.PreviousRank = firstOrDefault.RankName;
                        var orDefault = lookups.FirstOrDefault(x => x.LookupId == prevPlacements.LookupId);
                        if (orDefault != null)
                            EmployeeInfo.Previouses.PreviousJobTitle = orDefault.Description;

                        EmployeeInfo.Previouses.PreviousSalary = prevPlacements.Salary;
                        var deptt = db.MainCompanyStructures
                            .FirstOrDefault(x => x.OrgStructureId == prevPlacements.OrgStructureId);
                        if (deptt != null) EmployeeInfo.Previouses.Departemnt = deptt.Name;
                        var branch = db.Branch
                            .FirstOrDefault(x => x.BranchId == prevPlacements.BranchId);
                        if (branch != null) EmployeeInfo.Previouses.Branch = branch.AmharicName;
                        EmployeeInfo.Previouses.PreviousType = HrHelper.GetEmployeeStatus(prevPlacements.EmployementType);



                    }

                }

            }



            return EmployeeInfo;
        }

        public void Dispose()
        {
            db.Dispose();
        }
        
    }
}