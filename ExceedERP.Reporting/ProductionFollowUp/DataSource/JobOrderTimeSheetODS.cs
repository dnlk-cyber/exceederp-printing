using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.ProductionFollowUp.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.ProductionFollowUp.DataSource
{
    [DataObject]
    public class JobOrderTimeSheetODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<JobTimeSheetVM> GetJobOrderTimeSheet(string id)
        {
            List<JobTimeSheetVM> jobTimeSheetList = new List<JobTimeSheetVM>();

            if (id != null)
            {
                int.TryParse(id, out int registerId);
                var jobRegistration = db.JobOrderTimeRegistrations.FirstOrDefault(x => x.JobOrderTimeRegistrationId == registerId);
                if (jobRegistration != null)
                {
                    var registerSheet = new JobTimeSheetVM();
                    registerSheet.JobOrderTimeList = new List<JobOrderTimeListVM>();

                    var singleJob = db.Jobs.FirstOrDefault(x => x.JobId == jobRegistration.JobId);
                    if (singleJob != null)
                    {
                        registerSheet.JobNo = singleJob.JobNo;
                    }

                    var timeSheets = db.JobOrderTimeSheets.Where(x => x.JobOrderTimeRegistrationId == jobRegistration.JobOrderTimeRegistrationId).ToList();
                    foreach (var time in timeSheets)
                    {
                        var sheet = new JobOrderTimeListVM();

                        sheet.Date = time.Date;
                        sheet.Work = time.Work;
                        sheet.StartTime = time.StartTime;
                        sheet.ComplationTime = time.ComplationTime;
                        sheet.NoOfEmployee = time.NoOfEmployee;
                        sheet.Remark = time.Remark;

                        TimeSpan totalTime = time.ComplationTime - time.StartTime;

                        sheet.TimeTook = totalTime;
                        // string totalTimeString = string.Format("{0:%d} days, {0:%h} hours, {0:%m} minutes, {0:%s} seconds", totalTime);
                        string totalTimeString = string.Format("{0:%h}:{0:%m}:{0:%s}", totalTime);

                        sheet.TimeTookString = totalTimeString;

                        registerSheet.JobOrderTimeList.Add(sheet);
                    }

                    if (registerSheet.JobOrderTimeList.Any())
                    {
                        var timeLists = registerSheet.JobOrderTimeList;
                        TimeSpan sum = new TimeSpan();
                        foreach (var tt in timeLists)
                        {
                            sum = sum + tt.TimeTook;
                        }

                        //int hour = sum.Hours;
                        int minute = sum.Minutes;
                        int second = sum.Seconds;

                        double number = sum.TotalHours;
                        int totalHour = (int)Math.Truncate(number);
                        int day = totalHour / 8;
                        int hour = totalHour % 8;

                        registerSheet.TotalSum = day + " day " + hour + " hour " + minute + " minute " + second + " second ";

                        //string formatted = string.Format("{0}{1}{2}{3}",
                        //  sum.Duration().Days > 0 ? string.Format("{0:0} day{1}, ", sum.Days, sum.Days == 1 ? string.Empty : "s") : string.Empty,
                        //  sum.Duration().Hours > 0 ? string.Format("{0:0} hour{1}, ", sum.Hours, sum.Hours == 1 ? string.Empty : "s") : string.Empty,
                        //  sum.Duration().Minutes > 0 ? string.Format("{0:0} minute{1}, ", sum.Minutes, sum.Minutes == 1 ? string.Empty : "s") : string.Empty,
                        //  sum.Duration().Seconds > 0 ? string.Format("{0:0} second{1}", sum.Seconds, sum.Seconds == 1 ? string.Empty : "s") : string.Empty);

                        //if (formatted.EndsWith(", ")) formatted = formatted.Substring(0, formatted.Length - 2);

                        //if (string.IsNullOrEmpty(formatted)) formatted = "0 seconds";

                        //registerSheet.TotalSum = formatted;
                    }

                    jobTimeSheetList.Add(registerSheet);
                }
            }

            return jobTimeSheetList;
        }
    }
}
