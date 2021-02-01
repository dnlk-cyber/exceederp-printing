using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.HRM.ArchiveManagement
{
     public class AttachmentArchive
    {
        public int AttachmentArchiveId { get; set; }

        public AttachmentType ReferenceType { get; set; }

        public string Reference { get; set; }
        [Display(Name = "Common_Attachment_Description", ResourceType = typeof(Localization.Resources))]

        public string Description { get; set; }

        [Display(Name = "Common_Attachment_URL", ResourceType = typeof(Localization.Resources))]
        public string URL { get; set; }

        public byte[] BinaryFile { get; set; }
        [Display(Name = "Common_Attachment_FileType", ResourceType = typeof(Localization.Resources))]

        public string FileType { get; set; }

        public double Index { get; set; }
        [Display(Name = "Common_Attachment_FileName", ResourceType = typeof(Localization.Resources))]

        public string FileName { get; set; }
        public string Remark { get; set; }

        public string MINUTE { get; set; }
    }
}