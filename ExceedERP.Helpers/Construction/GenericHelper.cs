using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ExceedERP.Helpers.Construction
{
    public static class GenericHelper
    {
        public static string ToOrdinal(int value)
        {
            // Start with the most common extension.
            string extension = "th";

            // Examine the last 2 digits.
            int last_digits = value % 100;

            // If the last digits are 11, 12, or 13, use th. Otherwise:
            if (last_digits < 11 || last_digits > 13)
            {
                // Check the last digit.
                switch (last_digits % 10)
                {
                    case 1:
                        extension = "st";
                        break;
                    case 2:
                        extension = "nd";
                        break;
                    case 3:
                        extension = "rd";
                        break;
                }
            }

            return extension;
        }
    }



    public class LargeJsonResult : JsonResult
    {
        const string JsonRequest_GetNotAllowed = "This request has been blocked because sensitive information could be disclosed to third party web sites when this is used in a GET request. To allow GET requests, set JsonRequestBehavior to AllowGet.";
        public LargeJsonResult()
        {
            MaxJsonLength = 1024000;
            RecursionLimit = 100;
        }

        public int MaxJsonLength { get; set; }
        public int RecursionLimit { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (JsonRequestBehavior == JsonRequestBehavior.DenyGet &&
                String.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(JsonRequest_GetNotAllowed);
            }

            HttpResponseBase response = context.HttpContext.Response;

            if (!String.IsNullOrEmpty(ContentType))
            {
                response.ContentType = ContentType;
            }
            else
            {
                response.ContentType = "application/json";
            }
            if (ContentEncoding != null)
            {
                response.ContentEncoding = ContentEncoding;
            }
            if (Data != null)
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer() { MaxJsonLength = MaxJsonLength, RecursionLimit = RecursionLimit };
                response.Write(serializer.Serialize(Data));
            }
        }
    }


    public static class BusinessDaysCalculator
    {
        /// <summary>
        /// Calculates number of business days, taking into account:
        ///  - weekends (Saturdays and Sundays)
        ///  - bank holidays in the middle of the week
        /// </summary>
        /// <param name="firstDay">First day in the time interval</param>
        /// <param name="lastDay">Last day in the time interval</param>
        /// <param name="bankHolidays">List of bank holidays excluding weekends</param>
        /// <returns>Number of business days during the 'span'</returns>
        //public static int BusinessDaysUntil(this DateTime firstDay, DateTime lastDay, params DateTime[] bankHolidays)
        public static int Calculate(this DateTime firstDay, DateTime lastDay)

        {
            firstDay = firstDay.Date;
            lastDay = lastDay.Date;
            if (firstDay > lastDay)
            {
                return 0;
            }
            //throw new ArgumentException("Incorrect last day " + lastDay);

            TimeSpan span = lastDay - firstDay;
            int businessDays = span.Days + 1;
            int fullWeekCount = businessDays / 7;
            // find out if there are weekends during the time exceedng the full weeks
            if (businessDays > fullWeekCount * 7)
            {
                // we are here to find out if there is a 1-day or 2-days weekend
                // in the time interval remaining after subtracting the complete weeks
                int firstDayOfWeek = firstDay.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)firstDay.DayOfWeek;
                int lastDayOfWeek = lastDay.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)lastDay.DayOfWeek;
                if (lastDayOfWeek < firstDayOfWeek)
                    lastDayOfWeek += 7;
                if (firstDayOfWeek <= 6)
                {
                    if (lastDayOfWeek >= 7)// Both Saturday and Sunday are in the remaining time interval
                        businessDays -= 2;
                    else if (lastDayOfWeek >= 6)// Only Saturday is in the remaining time interval
                        businessDays -= 1;
                }
                else if (firstDayOfWeek <= 7 && lastDayOfWeek >= 7)// Only Sunday is in the remaining time interval
                    businessDays -= 1;
            }

            // subtract the weekends during the full weeks in the interval
            businessDays -= fullWeekCount + fullWeekCount;

            // subtract the number of bank holidays during the time interval
            //foreach (DateTime bankHoliday in bankHolidays)
            //{
            //    DateTime bh = bankHoliday.Date;
            //    if (firstDay <= bh && bh <= lastDay)
            //        --businessDays;
            //}

            return businessDays;
        }

        public static decimal Calculate(DateTime firstDay, DateTime lastDay,bool startDateIsEndOfPrevious,  int projectId)
        {
            var db = new ExceedDbContext();

            var saturdayIsHalfDayWorkingDay = false;
            var saturdayIsWholeDayWorkingDay = false;

            var holidaysWithinRange = db.ConstructionHolidays.Where(h => h.ProjectInformationId == projectId && h.Date >= firstDay && h.Date <= lastDay).ToList();

            var saturDayWorkingHours = db.ProjectCalander
                .Where(c => c.Day == Core.Domain.Construction.Days.SAT && c.Selected &&
                            c.ProjectInformationId == projectId
                            ).ToList();
            if (saturDayWorkingHours.Any())
            {
                // morning and afternoon 
                if(saturDayWorkingHours.Count >= 2)
                {
                    saturdayIsWholeDayWorkingDay = true;
                }
                else
                {
                    saturdayIsHalfDayWorkingDay = true;
                }
            }
            firstDay = firstDay.Date;
            lastDay = lastDay.Date;
            if (firstDay > lastDay)
            {
                return 0;
            }

            TimeSpan span = lastDay - firstDay;
            //int businessDays = span.Days + 1;
            decimal businessDays = span.Days;
            int fullWeekCount = (int)businessDays / 7;
            // find out if there are weekends during the time exceedng the full weeks
            #region MyRegion
            if (businessDays > fullWeekCount * 7)
            {
                // we are here to find out if there is a 1-day or 2-days weekend
                // in the time interval remaining after subtracting the complete weeks
                int firstDayOfWeek = firstDay.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)firstDay.DayOfWeek;
                int lastDayOfWeek = lastDay.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)lastDay.DayOfWeek;
                if (lastDayOfWeek < firstDayOfWeek)
                {
                    lastDayOfWeek += 7;
                }
                #region if saturday is working day 
                if (saturdayIsWholeDayWorkingDay)
                {
                    if (firstDayOfWeek <= 6)
                    {
                        if (lastDayOfWeek >= 7)// Both Saturday and Sunday are in the remaining time interval
                            businessDays -= 1;
                        //else if (lastDayOfWeek >= 6)// Only Saturday is in the remaining time interval
                        //    businessDays -= 1;
                    }
                    else if (firstDayOfWeek <= 7 && lastDayOfWeek >= 7 && !startDateIsEndOfPrevious)// Only Sunday is in the remaining time interval
                        businessDays -= 1;
                }
                #endregion

                #region if its half
                else if (saturdayIsHalfDayWorkingDay)
                {
                    if (firstDayOfWeek <= 6)
                    {
                        if (lastDayOfWeek >= 7)// Both Saturday and Sunday are in the remaining time interval
                            businessDays -= (decimal)1.5;
                        else if (lastDayOfWeek >= 6)// Only Saturday is in the remaining time interval
                            businessDays -= (decimal)0.5;
                    }
                    else if (firstDayOfWeek <= 7 && lastDayOfWeek >= 7)// Only Sunday is in the remaining time interval
                        businessDays -= 1;
                }
                #endregion

                #region saturday is non-working day

                else
                {
                    if(firstDayOfWeek < 6)
                    {
                        if (lastDayOfWeek >= 7)// Both Saturday and Sunday are in the remaining time interval
                            businessDays -= 2;
                        else if (lastDayOfWeek >= 6)// Only Saturday is in the remaining time interval
                            businessDays -= 1;
                    }
                    else if (firstDayOfWeek == 6 && !startDateIsEndOfPrevious)
                    {
                        if (lastDayOfWeek >= 7)// Both Saturday and Sunday are in the remaining time interval
                            businessDays -= 2;
                        else if (lastDayOfWeek >= 6)// Only Saturday is in the remaining time interval
                            businessDays -= 1;
                    }
                    else if(firstDayOfWeek == 6 && startDateIsEndOfPrevious)
                    {
                        if (lastDayOfWeek >= 7)// Both Saturday and Sunday are in the remaining time interval, but exlude only the last one because the first is excluded as the last date in the previous recursion
                            businessDays -= 1;
                        else if (lastDayOfWeek >= 6)// Only Saturday is in the remaining time interval
                            businessDays -= 1;
                    }
                    else if (firstDayOfWeek <= 7 && lastDayOfWeek >= 7 && !startDateIsEndOfPrevious)// Only Sunday is in the remaining time interval
                        businessDays -= 1;
                    
                }
                #endregion

            }
            #endregion

            // subtract the weekends during the full weeks in the interval
            if (saturdayIsWholeDayWorkingDay)
            {
                businessDays -= fullWeekCount;
            }
            else if (saturdayIsHalfDayWorkingDay)
            {
                businessDays -= fullWeekCount + (decimal)0.5*fullWeekCount;
            }
            else
            {
                businessDays -= fullWeekCount + fullWeekCount;
            }

            // subtract the number of  holidays during the time interval
            var holidays = new HashSet<DateTime>(holidaysWithinRange.Select( h => h.Date));
            foreach (DateTime holiday in holidays)
            {
                DateTime h = holiday.Date;
                if (firstDay <= h && h <= lastDay)
                    --businessDays;
            }

            return businessDays + 1;
        }
        /// <summary>
        /// cualculates the end date given a start date, duration, project id and a flag to check whether the start date is
        /// the end date of a previous recursive loop .. meaning if its and if its non working day, its already excluded in the previous 
        /// recursion ... no need to exclude it twice
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="startDateIsEndOfPrevious"></param>
        /// <param name="duration"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public static DateTime GetCalculatedEndDate(DateTime startDate, bool startDateIsEndOfPrevious, decimal duration, int projectId) {
            var end = startDate.AddDays((int)(duration - 1));

            var durationAfterRemovingNonWorkingDays = Calculate(startDate, end, startDateIsEndOfPrevious, projectId);
            if((int)duration > durationAfterRemovingNonWorkingDays)
            {
                var remainingDuration = duration - durationAfterRemovingNonWorkingDays;
                //end = end.AddDays((int)remainingDuration);
                end = GetCalculatedEndDate(end, true, remainingDuration + 1, projectId);

            }
            return end;
        }
    }
}
