using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.CRM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.InfoDesk
{
    public class InfoDeskKnowledgeBase : TrackUserSettingOperation
    {
        [Key]
        public int InfoDeskKnowledgeBaseId { get; set; }
        [Required]
        [DisplayName("Problem")]
        public string Problem { get; set; }
        [Required]
        [DisplayName("Solution")]
        public string Solution { get; set; }
        public string Remark { get; set; }
    }
    public class KnowledgeBaseAttachment
    {
        public int KnowledgeBaseAttachmentId { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string ShortPath { get; set; }
        public string FileExtension { get; set; }
        public string Description { get; set; }
        public int InfoDeskKnowledgeBaseId { get; set; }
        public virtual InfoDeskKnowledgeBase KnowledgeBase { get; set; }
    }

}
