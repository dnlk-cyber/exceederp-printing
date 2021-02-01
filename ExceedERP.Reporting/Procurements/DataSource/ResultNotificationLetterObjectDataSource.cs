using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Procurements.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Procurements.DataSource
{
   

    [DataObject]
    public class ResultNotificationLetterObjectDataSource
    {
        ExceedDbContext _db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public TemplateVM GetResultNotifEng()
        {

            Organization record = new Organization();

            var organization = _db.Companies.FirstOrDefault(x => x.Category == "0");
            if (organization != null)
            {
                record.Name = organization.TradeName;
                record.AmharicName = organization.AmharicName;
                var address =
                    _db.Addresses.Where(
                        a =>
                            a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType.ToLower() == "company").ToList();
                if (address.Count > 0)
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
                    _db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType == AttachmentType.LOGO);
                if (logo != null)
                {
                    record.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }
            ReportTemplate templateFormat = _db.ReportTemplates.FirstOrDefault(x => x.Status && x.Type == TemplateType.ResultNotifEng);

            TemplateVM template = new TemplateVM();
            template.LogoFileName = record.LogoFileName;
            template.Name = record.Name;
            template.AmharicName = record.AmharicName;




            if (templateFormat != null)
            {
                template.Let1 = templateFormat.Temp1 + " ";

                template.Let2 = templateFormat.Temp2 + " ";


                template.Let3 = templateFormat.Temp3 + " ";
                template.Let4 = templateFormat.Temp4 + " ";

                template.Summary = templateFormat.Temp5 + " ";

                




            }


            return template;
        }


        [DataObjectMethod(DataObjectMethodType.Select)]
        public TemplateVM GetResultNotifAmh()
        {

            Organization record = new Organization();

            var organization = _db.Companies.FirstOrDefault(x => x.Category == "0");
            if (organization != null)
            {
                record.Name = organization.TradeName;
                record.AmharicName = organization.AmharicName;
                var address =
                    _db.Addresses.Where(
                        a =>
                            a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType.ToLower() == "company").ToList();
                if (address.Count > 0)
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
                    _db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType == AttachmentType.LOGO);
                if (logo != null)
                {
                    record.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }
            ReportTemplate templateFormat = _db.ReportTemplates.FirstOrDefault(x => x.Status && x.Type == TemplateType.ResultNotifAmh);

            TemplateVM template = new TemplateVM();
            template.LogoFileName = record.LogoFileName;
            template.Name = record.Name;
            template.AmharicName = record.AmharicName;




            if (templateFormat != null)
            {
                template.Let1 = templateFormat.Temp1 + " ";

                template.Let2 = templateFormat.Temp2 + " ";


                template.Let3 = templateFormat.Temp3 + " ";
                template.Let4 = templateFormat.Temp4 + " ";

                template.Summary = templateFormat.Temp5 + " ";
                
            }


            return template;
        }
    }
}
