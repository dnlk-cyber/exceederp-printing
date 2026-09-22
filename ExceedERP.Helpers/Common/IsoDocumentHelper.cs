using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using System;
using System.Linq;

namespace ExceedERP.Helpers.Common
{
    public static class IsoDocumentHelper
    {
       
        public static void AddUpdateDocumentInfo(IsoDocument document)
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                var existingDoc = db.IsoDocuments.FirstOrDefault(d => d.Module == document.Module && d.DocumentType == document.DocumentType);
                if (existingDoc != null)
                {
                    existingDoc.URL = document.URL;


                    db.IsoDocuments.Attach(existingDoc);
                    db.Entry(existingDoc).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    db.IsoDocuments.Add(document);
                    db.SaveChanges();
                }
            }
        }
		public static void GetIsoDocumentInfo(Modules module, string reportType, out string documentTitle, out string documentNumber,out string issueNumber)
		{
			//documentTitle = reportType.ToString().Replace('_', ' ');
			documentTitle = reportType;
            issueNumber = "1";
            //var db = new ExceedDbContext();
            using (ExceedDbContext db = new ExceedDbContext())
            {
                var isoDoc = db.IsoDocuments.FirstOrDefault(d => d.Module == module && d.DocumentType == reportType.ToString());
                documentNumber = isoDoc?.FormNumber;
                if (isoDoc != null && !string.IsNullOrEmpty(isoDoc.FormTitle))
                {
                    documentTitle = isoDoc.FormTitle;
                }
                if(isoDoc!=null&&!string.IsNullOrEmpty(isoDoc.FormNumber))
                {
                    issueNumber = isoDoc.FormNumber;
                }
            }
		}

        public static void GetIsoDocumentInfo(Modules fleetManagement, object totalFleetFuelConsumption, out string title, out string number)
        {
            throw new NotImplementedException();
        }
    }
}
