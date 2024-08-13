using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Maintenance
{
    public class MaintenanceCompanyDocumentType 
    {
        public int MaintenanceCompanyDocumentTypeId { get; set; }
        public string DocumentType { get; set; }
    }
    public class MaintenanceCompanyDocument
    {
        public int MaintenanceCompanyDocumentId { get; set; }
        public int MaintenanceCompanyDocumentTypeId { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string ShortPath { get; set; }
        public string FileExtension { get; set; }
        public string Description { get; set; }
    }
}
