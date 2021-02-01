using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeFixedAssetListObjectDataSource : List<EmployeeFixedAsset>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeFixedAsset> GetEmployeeFixedAssetList(string employee)
        {
            List<EmployeeFixedAsset> list = new List<EmployeeFixedAsset> { };
            EmployeeFixedAsset EmployeeFixedAsset = new EmployeeFixedAsset();
            UserCardItemsList userCardItemsList = new UserCardItemsList();
            var userCardItems = db.UserCardItems;
            var employeeList = db.Person_Employee.Where(x => x.IsActive);

            var Organization = db.Companies.FirstOrDefault(x => x.Category == "0");
            if (Organization != null)
            {
                EmployeeFixedAsset.Name = Organization.TradeName;
                EmployeeFixedAsset.AmharicName = Organization.AmharicName;
                EmployeeFixedAsset.Abbreviation = Organization.Abbreviation;
                var address =
                    db.Addresses.Where(
                        a =>
                            a.Reference == Organization.MainCompanyId.ToString() &&
                            a.ReferenceType.ToLower() == "company").ToList();
                if (address.Any())
                {
                    foreach (var a in address)
                    {
                        if (a.Category == AddressType.KIFLEKETEMA)
                        {
                            EmployeeFixedAsset.Address = "Subcity: " + a.Value + " ";
                        }
                        if (a.Category == AddressType.KEBELE)
                        {
                            EmployeeFixedAsset.Address += "Kebele: " + a.Value + " ";
                        }
                        if (a.Category == AddressType.CITYPROVINCE)
                        {
                            if (EmployeeFixedAsset.Address == null)
                            {
                                EmployeeFixedAsset.Address += a.Value + " -Ethiopia";
                            }
                        }
                        else if (a.Category == AddressType.POBOX)
                        {

                            EmployeeFixedAsset.Pobox = "P.O.Box: " + a.Value;

                        }
                        else if (a.Category == AddressType.OFFICE_EMAIL)
                        {

                            EmployeeFixedAsset.Email = "Email: " + a.Value;

                        }
                        else if (a.Category == AddressType.WEBSITE)
                        {

                            EmployeeFixedAsset.WebSite = "Website:" + a.Value;

                        }
                        else if (a.Category == AddressType.FAX)
                        {

                            EmployeeFixedAsset.Fax = "Fax:  " + a.Value;

                        }
                        else if (a.Category == AddressType.OFFICE_PHONE)
                        {

                            EmployeeFixedAsset.Tel = "Tel:  " + a.Value;

                        }

                    }

                }
                var logo =
                    db.Attachments.Where(
                        a =>
                            a.Reference == Organization.MainCompanyId.ToString() &&
                            a.ReferenceType == AttachmentType.LOGO).FirstOrDefault();
                if (logo != null)
                {
                    EmployeeFixedAsset.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }
            
            if (employee != null)
            {
                var Oneemployee = employeeList.Where(x => x.EmployeeCode == employee).FirstOrDefault();
                if (Oneemployee != null)
                {
                    var usercards = userCardItems.Where(x => x.EmployeeId == Oneemployee.EmployeeId).ToList();
                    //EmployeeFixedAsset = new EmployeeFixedAsset();
                    EmployeeFixedAsset.UserCardItemsList = new List<UserCardItemsList>();

                    EmployeeFixedAsset.FullName = Oneemployee.FirstName + " " + Oneemployee.MiddleName + " " + Oneemployee.LastName;
                    EmployeeFixedAsset.Id = Oneemployee.EmployeeCode;

                    var placement = db.Placements.FirstOrDefault(x=>x.EmployeeId == Oneemployee.EmployeeId && x.isCurrent);
                    if(placement != null)
                    {
                        var mainCompany = db.MainCompanyStructures.FirstOrDefault(x=>x.OrgStructureId == placement.OrgStructureId);
                        if(mainCompany != null)
                        {
                            EmployeeFixedAsset.Department = mainCompany.Name;
                        }
                        var branch = db.Branch.FirstOrDefault(x=>x.BranchId == placement.BranchId);
                        if(branch != null)
                        {
                            EmployeeFixedAsset.Branch = branch.Name;
                        }
                        var job = db.Lookups.FirstOrDefault(x=>x.LookupId == placement.LookupId && x.LookupType == LookUpTypes.JOB_TITLE);
                        if(job != null)
                        {
                            EmployeeFixedAsset.JobTitle = job.Description;
                        }
                    }
                   
                        foreach (var f in usercards)
                        {
                            userCardItemsList = new UserCardItemsList();
                            userCardItemsList.ItemDesc = f.Name;
                            userCardItemsList.Quanity = f.Quanity;
                            userCardItemsList.UnitPrice = f.UnitPrice;
                            userCardItemsList.TagNumber = f.Tag;
                            userCardItemsList.IssueDate = f.IssueDate.ToShortDateString();

                            var fixedAsset = db.AssetRegistrations.FirstOrDefault(x=>x.AssetRegistrationId == f.AssetRegistrationId);
                            if(fixedAsset != null)
                            {
                                //userCardItemsList.AccNo = "";
                                userCardItemsList.AssetNo = fixedAsset.Tag;
                                userCardItemsList.Description = fixedAsset.Name;
                                userCardItemsList.Size = fixedAsset.Size;
                                userCardItemsList.Model = fixedAsset.ModelNo;
                                userCardItemsList.SerialPartNo = fixedAsset.SerialNo;
                               
                                var assetCondition = db.FixedAssetConditions.FirstOrDefault(x=>x.FixedAssetConditionId == fixedAsset.FixedAssetConditionId);
                                if(assetCondition != null)
                                {
                                    userCardItemsList.Condition = assetCondition.Name;
                                }
                                userCardItemsList.Unit = fixedAsset.Unit;
                                userCardItemsList.Qty = fixedAsset.Quanity.ToString();
                                userCardItemsList.MTITNo = fixedAsset.SIV;
                                // userCardItemsList.MTITDate = fixedAsset.;
                                userCardItemsList.GRNNo = fixedAsset.GRN;
                                userCardItemsList.GRNDate = fixedAsset.GRNDate.Value;
                            }

                            EmployeeFixedAsset.UserCardItemsList.Add(userCardItemsList);

                        }
                    
                    list.Add(EmployeeFixedAsset);
                }
            }
            else
            {
                foreach (var e in employeeList)
                {
                    var fixedAssets = userCardItems.Where(x => x.EmployeeId == e.EmployeeId).ToList();
                    EmployeeFixedAsset = new EmployeeFixedAsset();
                    EmployeeFixedAsset.UserCardItemsList = new List<UserCardItemsList>();
                    EmployeeFixedAsset.FullName = e.FirstName + " " + e.MiddleName + " " + e.LastName;
                    EmployeeFixedAsset.Id = e.EmployeeCode;

                    var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == e.EmployeeId && x.isCurrent);
                    if (placement != null)
                    {
                        var mainCompany = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId);
                        if (mainCompany != null)
                        {
                            EmployeeFixedAsset.Department = mainCompany.Name;
                        }
                        var branch = db.Branch.FirstOrDefault(x => x.BranchId == placement.BranchId);
                        if (branch != null)
                        {
                            EmployeeFixedAsset.Branch = branch.Name;
                        }
                        var job = db.Lookups.FirstOrDefault(x => x.LookupId == placement.LookupId && x.LookupType == LookUpTypes.JOB_TITLE);
                        if (job != null)
                        {
                            EmployeeFixedAsset.JobTitle = job.Description;
                        }
                    }

                    if (fixedAssets.Any())

                        foreach (var f in fixedAssets)
                        {
                            userCardItemsList = new UserCardItemsList();
                            userCardItemsList.ItemDesc = f.Name;
                            userCardItemsList.Quanity = f.Quanity;
                            userCardItemsList.UnitPrice = f.UnitPrice;
                            userCardItemsList.TagNumber = f.Tag;
                            userCardItemsList.IssueDate = f.IssueDate.ToShortDateString();

                            var fixedAsset = db.AssetRegistrations.FirstOrDefault(x => x.AssetRegistrationId == f.AssetRegistrationId);
                            if (fixedAsset != null)
                            {
                                //userCardItemsList.AccNo = "";
                                userCardItemsList.AssetNo = fixedAsset.Tag;
                                userCardItemsList.Description = fixedAsset.Name;
                                userCardItemsList.Size = fixedAsset.Size;
                                userCardItemsList.Model = fixedAsset.ModelNo;
                                userCardItemsList.SerialPartNo = fixedAsset.SerialNo;
                                userCardItemsList.Condition = fixedAsset.Condition.ToString();
                                userCardItemsList.Unit = fixedAsset.Unit;
                                userCardItemsList.Qty = fixedAsset.Quanity.ToString();
                                userCardItemsList.MTITNo = fixedAsset.SIV;
                               // userCardItemsList.MTITDate = fixedAsset.;
                                userCardItemsList.GRNNo = fixedAsset.GRN;
                                userCardItemsList.GRNDate = fixedAsset.GRNDate.Value;
                            }

                            EmployeeFixedAsset.UserCardItemsList.Add(userCardItemsList);

                        }

                }
                list.Add(EmployeeFixedAsset);
            }
            return list;
        }

      
    }
}