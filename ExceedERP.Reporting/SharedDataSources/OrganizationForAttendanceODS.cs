using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using ExceedERP.Core.Domain.Common;

using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.SharedDataSources
{
    

    [DataObject]
    public class OrganizationForAttendanceODS : AttendanceHeaderVM
    {
        private ExceedDbContext db = new ExceedDbContext();


        [DataObjectMethod(DataObjectMethodType.Select)]
        public AttendanceHeaderVM GetOrganization(string year, string period, string week)
        {

            AttendanceHeaderVM record = new AttendanceHeaderVM();

            if(year != null && period != null && week != null)
            {
                var dateR = db.WeeklyAttendances.FirstOrDefault(x => x.GlFiscalYearId.ToString() == year && x.GLPeriodId.ToString() == period && x.Week.ToString() == week);

                if (dateR != null)
                {
                    record.From = XAPI.EthiopicDateTime.GetEthiopicDate(dateR.DayOneDate.Value.Day, dateR.DayOneDate.Value.Month, dateR.DayOneDate.Value.Year);

                    record.To = XAPI.EthiopicDateTime.GetEthiopicDate(dateR.DaySevenDate.Value.Day, dateR.DaySevenDate.Value.Month, dateR.DaySevenDate.Value.Year);
                }
            }
            if (year != null && period != null && week == null)
            {
                var per = db.GLPeriods.FirstOrDefault(x=>x.GlFiscalYearId.ToString() == year && x.GLPeriodId.ToString() == period);
                if (per != null)
                {
                    record.From = XAPI.EthiopicDateTime.GetEthiopicDate(per.DateFrom.Day, per.DateFrom.Month, per.DateFrom.Year);

                    record.To = XAPI.EthiopicDateTime.GetEthiopicDate(per.DateTo.Day, per.DateTo.Month, per.DateTo.Year);
                }
            }


            var Organization = db.Companies.FirstOrDefault(x => x.Category == "0");
            if (Organization != null)
            {
                record.Name = Organization.TradeName;
                record.AmharicName = Organization.AmharicName;
                record.Abbreviation = Organization.Abbreviation;

                var address =
                    db.Addresses.Where(a =>
                            a.Reference == Organization.MainCompanyId.ToString() &&
                            a.ReferenceType.ToLower() == "company").ToList();
                if (address.Any())
                {
                    foreach (var a in address)
                    {

                        if (record.Address == null)
                        {
                            record.Address = a.Category + ":- " + a.Value;
                        }
                        else
                        {
                            record.Address += ", " + a.Category + "-" + a.Value;
                        }


                    }

                }
                var logo =
                    db.Attachments.FirstOrDefault(a => a.Reference == Organization.MainCompanyId.ToString() &&
                            a.ReferenceType == AttachmentType.LOGO);
                if (logo != null)
                {
                    record.LogoFileName = ByteArrayToImage(logo.BinaryFile);
                }

                record.Date =
                    XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
            }
            return record;
        }
        public static Image ByteArrayToImage(byte[] byteArrayIn)
        {
            if (byteArrayIn != null)
            {
                MemoryStream ms = new MemoryStream(byteArrayIn);
                Image returnImage = Image.FromStream(ms);
                return returnImage;
            }
            return null;

        }
    }
}
