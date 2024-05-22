using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Helpers.Common
{
    public static class IsoDocumentHelper
    {
        public static void AddUpdateDocumentInfo(IsoDocument document)
        {
            var db = new ExceedDbContext();
            var existingDoc = db.IsoDocuments.FirstOrDefault(d => d.Module == document.Module && d.DocumentType == document.DocumentType);
            if(existingDoc != null)
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
		public static void GetIsoDocumentInfo(Modules module, string reportType, out string documentTitle, out string documentNumber)
		{
			//documentTitle = reportType.ToString().Replace('_', ' ');
			documentTitle = reportType;
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
