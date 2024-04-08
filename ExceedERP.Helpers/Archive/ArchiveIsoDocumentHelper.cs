using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ExceedERP.Helpers.Archive
{
    public static class ArchiveIsoDocumentHelper
    {
        public static void GetIsoDocumentInfo(Modules module, ArchiveModuleReportType reportType, out string documentTitle, out string documentNumber)
        {
            documentTitle = reportType.ToString().Replace('_', ' ');
            var db = new ExceedDbContext();

            var isoDoc = db.IsoDocuments.FirstOrDefault(d => d.Module == module && d.DocumentType == reportType.ToString());
            documentNumber = isoDoc?.FormNumber;
            if (isoDoc != null && !string.IsNullOrEmpty(isoDoc.FormTitle))
            {
                documentTitle = isoDoc.FormTitle;
            }
        }
    }
}
