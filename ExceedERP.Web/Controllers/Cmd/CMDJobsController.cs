using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;
using Hangfire;

namespace ExceedERP.Web.Controllers.Cmd
{
    public partial class CmdController : Controller
    {

        // GET: CMD

        public void AddJobs()
        {
            RecurringJob.AddOrUpdate(() => _financeHelper.DetectAndRemoveDuplicateLedgerLines(), "0 19 * * *");//7pm
            RecurringJob.AddOrUpdate(() => _financeHelper.DetectAndRemoveDuplicateLedgerAccountCode(), "0 22 * * *");//4pm

            int currentYearId=fiscalYearHelper.GetCurrentFYearId();
            
            if(currentYearId!=0)
            {
                int fisrtPeriodId = fiscalYearHelper.GetFiscalYearFirstPeriod(currentYearId);
                RecurringJob.AddOrUpdate(() => _financeHelper.UpdateAllStandardBalancesMonthlyFastP(fisrtPeriodId, User.Identity.Name), "0 21 * * *");//3pm
            }

            
            RecurringJob.AddOrUpdate(() => _accountHelper.SplitBookAccountCombination(), "0 22 * * *");//10pm

            //RecurringJob.AddOrUpdate(() => new AlarmGenerator().GenerateGuaranteeAlarms(), Cron.Daily);
            //RecurringJob.AddOrUpdate(() => new AlarmGenerator().ContractAlarms(), Cron.Daily);
            //RecurringJob.AddOrUpdate(() => TransactionHelper.CloseStoreRequisition(),"0 19 * * *");//1pm
            //RecurringJob.AddOrUpdate(() => TransactionHelper.CloseTransferSend(),  "30 19 * * *");//1pm
            //RecurringJob.AddOrUpdate(() =>_dashboardHelper.UpdateDashboardItemBalance(), Cron.Hourly);
            //RecurringJob.AddOrUpdate(() =>_dashboardHelper.UpdateDashboardDictionary(), Cron.Hourly);
            //RecurringJob.AddOrUpdate(() =>_dashboardHelper.UpdateProcurementDocStatusDict(), Cron.Hourly);
            //RecurringJob.AddOrUpdate(() =>_dashboardHelper.UpdateFinanceDocStatusDict(), Cron.Hourly);
            //RecurringJob.AddOrUpdate(() =>_dashboardHelper.UpdateInventoryDocStatusDict(), Cron.Hourly);
            //RecurringJob.AddOrUpdate(() =>_dashboardHelper.UpdateFixedAssetDocStatusDict(), Cron.Hourly);
            //RecurringJob.AddOrUpdate(() => _financeHelper.TransferGlRecordsToBank(), "0 23 * * *");//5pm
        }

    }
}