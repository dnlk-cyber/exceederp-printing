using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class CommitmentODS
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<CommitmentVM> GetDeligation(string id)
        {
            List<CommitmentVM> singleCommitment = new List<CommitmentVM>();
            CommitmentVM commit = new CommitmentVM();
            commit.Cosign = new List<CosignVM>();
            var commitment = db.Commitments.FirstOrDefault(x=>x.CommitmentId.ToString() == id);
           

            if(commitment != null)
            {
                var person = db.Person_Employee.FirstOrDefault(x=>x.EmployeeId == commitment.EmployeeId);
                if(person != null)
                {
                    commit.EmployeeCode = person.EmployeeCode;
                    commit.EmployeeName = person.FullNameaEnglish;
                }
                commit.Reason = commitment.Reason;
                commit.CommitmentStartDate = commitment.FromCometAm;
                commit.CommitmentEndDate = commitment.ToCometAm;
                commit.CommitmentValue = commitment.Value.ToString();

                var look = db.Lookups.FirstOrDefault(x=>x.LookupId == commitment.LookupId && x.LookupType == LookUpTypes.COMMITMENT_TYPE);
                if(look != null)
                {
                    commit.Type = look.Description;
                }

                var cosigns = db.Cosigns.Where(x=>x.EmployeeId == commitment.EmployeeId);
                foreach(var cos in cosigns)
                {
                    var cosVM = new CosignVM();

                    var ll = db.Lookups.FirstOrDefault(x=>x.LookupId == cos.LookupId && x.LookupType == LookUpTypes.COSIGN_TYPE);
                    if(ll != null)
                    {
                        cosVM.CosignType = ll.Description;
                    }
                    
                    cosVM.AgreementSignDate = cos.AgreementDateAm;
                    cosVM.FullName = cos.FullName;
                    cosVM.BirthPlace = //cos.BirthPlace;
                    cosVM.WorkOrganization = cos.Workorganization;

                    commit.Cosign.Add(cosVM);

                }
                 singleCommitment.Add(commit);
            }

            return singleCommitment;
        }
    }
}
