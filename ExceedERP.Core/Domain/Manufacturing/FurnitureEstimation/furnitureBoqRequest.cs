using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation
{
    public class FurnitureBoqRequest
    {
        [Key]
        public int FurnitureBoqRequestId { get; set; }
        public int CustomerId { get; set; }
        public string JobType { get; set; }       
    }

    public class FurnitureBoqDocument
    {
        [Key]
        public int FurnitureBoqDocumentId { get; set; }
        public int FurnitureBoqRequestId { get; set; }

        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string ShortPath { get; set; }
        public string FileExtension { get; set; }
        public string Description { get; set; }
    }
}
