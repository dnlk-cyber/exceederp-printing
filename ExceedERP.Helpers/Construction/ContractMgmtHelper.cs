using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Helpers.Construction
{
    public static class ContractMgmtHelper
    {
        /// <summary>
        /// update main (sub) contract contents when subcomponents such as extension of time,
        /// contract revision are changed ....
        /// </summary>
        /// <param name="contractId"></param>
        public static void UpdateContract(int contractId)
        {
            var db = new ExceedDbContext();
            var currentContract = db.ProjectContract.Find(contractId);
            if (currentContract != null)
            {
                var allExtensionOfTime = db.ExtensionOfTime.Where(eot => eot.ProjectContractId == contractId).ToList();
                var sumOfApprovedEoT = allExtensionOfTime.Sum(e => e.ApprovedDays);

                var variationOrders = db.ContractVariationOrder.Where(vo => vo.ProjectContractId == contractId).ToList();
                var variationAmnt = variationOrders.Sum(vo => vo.ChangeCost);
                var variationTime = variationOrders.Sum(vo => vo.DaysAdded);

                // changing the decimal values to double is required here because
                // DateTime.AddDays method takes an argument of 'double' data type
                double ctrPeriod = (double)currentContract.ContractPeriod;
                double sumOfEoT = (double)sumOfApprovedEoT;
                double totalOfDaysToAdd = ctrPeriod + sumOfEoT   + (double)variationTime;


                currentContract.ApprovedTimeExtension = sumOfApprovedEoT;
                currentContract.TotalProjectPeriod = currentContract.ContractPeriod + 
                                                    currentContract.ApprovedTimeExtension + 
                                                    variationTime;

                // subcontracts' completion dates (original and revised), should start from
                // contract signing date
                currentContract.ContractTime = IsSubContract(currentContract.ProjectContractId) ? 
                    currentContract.ContractSigningDate.AddDays(ctrPeriod):
                    currentContract.CommencementDate.AddDays(ctrPeriod);

                currentContract.RevisedCompletionDate = IsSubContract(currentContract.ProjectContractId) ?
                    currentContract.ContractSigningDate.AddDays(totalOfDaysToAdd) :
                    currentContract.CommencementDate.AddDays(totalOfDaysToAdd);

                currentContract.Variation = variationAmnt;
                currentContract.TotalProjectAmount =  currentContract.Supplemetary == null ? 
                    currentContract.ContractAmount + variationAmnt :
                    currentContract.ContractAmount  + currentContract.Supplemetary + variationAmnt;
                currentContract.NetGrand = currentContract.TotalProjectAmount - currentContract.ContractAmount * (currentContract.Rebate / 100);

                db.ProjectContract.Attach(currentContract);
                db.Entry(currentContract).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }
        public static ProjectContract GetProjectMainContract(int projectId)
        {
            var db = new ExceedDbContext();
            ProjectContract contract = new ProjectContract();
            contract.CommencementDate = DateTime.Now;
            contract.ContractSigningDate = DateTime.Now;
            contract.SiteHandoverTime = DateTime.Now;

            var sub = db.SubContrat.ToList();
            var allContracts = db.ProjectContract.Where(c => c.ProjectInformationId == projectId);
            var ids = new HashSet<int>(sub.Select(s => s.ProjectContractId));
            var mainContrat = allContracts.Where(main => !ids.Contains(main.ProjectContractId)).FirstOrDefault();
            if (mainContrat != null)
            {
                contract = mainContrat;
            }
            return contract;
        }
        public static ProjectContract GetProjectMainContractIfExists(int projectId) {
            var db = new ExceedDbContext();
            

            var sub = db.SubContrat.ToList();
            var allContracts = db.ProjectContract.Where(c => c.ProjectInformationId == projectId);
            var ids = new HashSet<int>(sub.Select(s => s.ProjectContractId));
            var mainContrat = allContracts.Where(main => !ids.Contains(main.ProjectContractId)).FirstOrDefault();
            
            return mainContrat;
        }
        public static decimal GetProjectTotalAmount(int periodId)
        {
            var db = new ExceedDbContext();
            decimal value = new decimal();
            ProjectInformation currentProject = new ProjectInformation();
            PeriodMapping currentPeriod = db.PeriodMapping.Find(periodId);
            if (currentPeriod != null)
            {
                var tempProj = db.ProjectInformation.Find(currentPeriod.ProjectInformationId);
                if (tempProj != null)
                {
                    currentProject = tempProj;
                }
            }

            var sub = db.SubContrat.ToList();
            var allContracts = db.ProjectContract.Where(c => c.ProjectInformationId == currentProject.ProjectInformationId);
            var ids = new HashSet<int>(sub.Select(s => s.ProjectContractId));
            ProjectContract mainContrat = allContracts.Where(main => !ids.Contains(main.ProjectContractId)).FirstOrDefault();
            if (mainContrat != null)
            {
                if (mainContrat.TotalProjectAmount != null)
                {
                    value = (decimal)mainContrat.TotalProjectAmount;
                }
            }
            return value;
        }
        // just the contract amount(variation, rebate .... excluded)
        public static decimal GetProjectContractAmount(int periodId)
        {
            var db = new ExceedDbContext();
            decimal value = new decimal();
            ProjectInformation currentProject = new ProjectInformation();
            PeriodMapping currentPeriod = db.PeriodMapping.Find(periodId);
            if (currentPeriod != null)
            {
                var tempProj = db.ProjectInformation.Find(currentPeriod.ProjectInformationId);
                if (tempProj != null)
                {
                    currentProject = tempProj;
                }
            }

            var sub = db.SubContrat.ToList();
            var allContracts = db.ProjectContract.Where(c => c.ProjectInformationId == currentProject.ProjectInformationId);
            var ids = new HashSet<int>(sub.Select(s => s.ProjectContractId));
            ProjectContract mainContrat = allContracts.Where(main => !ids.Contains(main.ProjectContractId)).FirstOrDefault();
            if (mainContrat != null)
            {
                if (mainContrat.TotalProjectAmount != null)
                {
                    value = (decimal)mainContrat.ContractAmount;
                }
            }
            return value;
        }
        public static void UpdateProjectAmountAccordingToContract(int projectId, decimal contractAmount)
        {
            var db = new ExceedDbContext();
            var currentProject = db.ProjectInformation.Find(projectId);
            if (currentProject != null)
            {
                currentProject.ContractAmount = contractAmount;

                db.ProjectInformation.Attach(currentProject);
                db.Entry(currentProject).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }
        public static int GetProjectFormulaType(int projectId) {
            var db = new ExceedDbContext();
            var formulaType = 0;
            var temp = db.ProjectPriceAdjustmentFormulaTypes.FirstOrDefault(f => f.ProjectInformationId == projectId);
            if(temp != null)
            {
                formulaType = (int)temp.FormulaType;
            }
            return formulaType;
        }
        private static bool IsSubContract(int contractId) {
            var db = new ExceedDbContext();
            var isSub = false;
            //var contract = db.ProjectContract.FirstOrDefault(c => c.ProjectContractId == contractId);
            var temp = db.SubContrat.FirstOrDefault(sb => sb.ProjectContractId == contractId);
            if(temp != null)
            {
                isSub = true;
            }
            return isSub;
        } 
    }
}
