
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace ExceedERP.Core.Domain.Global
{
    //public class OrgBranchUserMapping //: IValidatableObject
    //{
    //    public int OrgBranchUserMappingID { get; set; }
    //    public DateTime CreatedOn { get; set; }
    //    public string CreatedBy { get; set; }
    //    [Required]
    //    public string Branch { get; set; }
    //    [Required]
    //    public string User { get; set; }
    //    [Display(Name = "Is Default")]
    //    public bool IsDefault { get; set; }
    //    //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    //    //{
    //    //    using (var context = new ExceedDbContext())
    //    //    {
    //    //        var exists = context.OrgBranchUserMappings.ToList().AsQueryable<OrgBranchUserMapping>().Where(x => x.OrgBranchUserMappingID != this.OrgBranchUserMappingID)
    //    //                                                                                               .Where(x => x.User.ToUpper(Common.exceedculture).Equals(this.User.ToUpper(Common.exceedculture)))
    //    //                                                                                               .Where(x => x.Branch.ToUpper(Common.exceedculture).Equals(this.Branch.ToUpper(Common.exceedculture))).ToList();

    //    //        if (exists.Count() > 0)
    //    //        {
    //    //            yield return new ValidationResult(this.User + " and " + this.Branch + " combination is already in the system.");
    //    //        }
    //    //    }
    //    //}
    //}
    public class GlobalUserAccessLog
    {
        public int GlobalUserAccessLogID { get; set; }
        public string IPAddress { get; set; }
        public string UserName { get; set; }
        public DateTime TimeAccessed { get; set; }
        public string Type { get; set; }

        public string Status { get; set; }
        public string Description { get; set; }
    }
    public class GlobalBranchsetup //: IValidatableObject
    {
        public int GlobalBranchsetupID { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        [Required]
        [Display(Name = "Branch Name")]
        public string OrgBranchName { get; set; }
        [Required]
        [Display(Name = "Branch Abbreviation")]
        public string OrgBranchAbbreviation { get; set; }
        [Required]
        [Display(Name = "Account Code")]
        public string BranchAccountCode { get; set; }
        public string Description { get; set; }

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    using (var context = new ExceedDbContext())
        //    {
        //        var branchexists = context.GlobalBranchsetups.ToList().AsQueryable<GlobalBranchsetup>().Where(x => x.GlobalBranchsetupID != this.GlobalBranchsetupID)
        //                                                                                               .Where(x => x.OrgBranchName.ToUpper(Common.exceedculture).Equals(this.OrgBranchName.ToUpper(Common.exceedculture))).ToList();

        //        if (branchexists.Count() > 0)
        //        {
        //            yield return new ValidationResult(this.OrgBranchName + " is already in the system.");
        //        }

        //        var exists = context.GlobalBranchsetups.ToList().AsQueryable<GlobalBranchsetup>().Where(x => x.GlobalBranchsetupID != this.GlobalBranchsetupID)
        //                                                                                               .Where(x => x.BranchAccountCode.ToUpper(Common.exceedculture).Equals(this.BranchAccountCode.ToUpper(Common.exceedculture)))
        //                                                                                               .Where(x => x.OrgBranchName.ToUpper(Common.exceedculture).Equals(this.OrgBranchName.ToUpper(Common.exceedculture))).ToList();

        //        if (exists.Count() > 0)
        //        {
        //            yield return new ValidationResult(this.BranchAccountCode + " and " + this.OrgBranchName + " combination is already in the system.");
        //        }
        //    }
        //}
    }

    public class GlobalOrgInformation //: IValidatableObject
    {
        public int GlobalOrgInformationID { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        [Required]
        [Display(Name = "Organization Name")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Organization Abbreviation")]
        public string Abbreviation { get; set; }
        [RegularExpression(@"^\d{10}$", ErrorMessage = "* Please enter correct phone number with 10 digit")]
        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        [Required]
        public string Phone { get; set; }
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        public string Fax { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }
        public string Description { get; set; }
        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    using (var context = new ExceedDbContext())
        //    {
        //        var branchexists = context.GlobalOrgInformations.ToList().AsQueryable<GlobalOrgInformation>().Where(x => x.GlobalOrgInformationID != this.GlobalOrgInformationID)
        //                                                                                               .Where(x => x.Name.ToUpper(Common.exceedculture).Equals(this.Name.ToUpper(Common.exceedculture))).ToList();

        //        if (branchexists.Count() > 0)
        //        {
        //            yield return new ValidationResult(this.Name + " is already in the system.");
        //        }

        //        var abbrexists = context.GlobalOrgInformations.ToList().AsQueryable<GlobalOrgInformation>().Where(x => x.GlobalOrgInformationID != this.GlobalOrgInformationID)
        //                                                                                              .Where(x => x.Abbreviation.ToUpper(Common.exceedculture).Equals(this.Abbreviation.ToUpper(Common.exceedculture))).ToList();

        //        if (abbrexists.Count() > 0)
        //        {
        //            yield return new ValidationResult(this.Abbreviation + " is already in the system.");
        //        }

        //        var exists = context.GlobalOrgInformations.ToList().AsQueryable<GlobalOrgInformation>().Where(x => x.GlobalOrgInformationID != this.GlobalOrgInformationID)
        //                                                                                               .Where(x => x.Name.ToUpper(Common.exceedculture).Equals(this.Name.ToUpper(Common.exceedculture)))
        //                                                                                               .Where(x => x.Abbreviation.ToUpper(Common.exceedculture).Equals(this.Abbreviation.ToUpper(Common.exceedculture))).ToList();

        //        if (exists.Count() > 0)
        //        {
        //            yield return new ValidationResult(this.Name + " and " + this.Abbreviation + " combination is already in the system.");
        //        }
        //    }
        //}
    }
}
