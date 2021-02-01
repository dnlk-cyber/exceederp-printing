using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Construction
{
    public class ConstructionProjectUserAssignment : TrackUserSettingOperation
    {
        [Key]
        public int ConstructionProjectUserAssignmentId { get; set; }

        [Display(Name ="Project")]
        public int ProjectInformationId { get; set; }

        public virtual ProjectInformation Project { get; set; }

        [Display(Name ="User")]
        public string UserName { get; set; }
    }
    public class ConstructionProjectUserAssignmentViewModel : TrackUserSettingOperation
    {
        public int ConstructionProjectUserAssignmentId { get; set; }

        [Display(Name = "Project")]
        public int ProjectInformationId { get; set; }

        public virtual ProjectInformation Project { get; set; }

        [Display(Name = "User")]
        public string UserName { get; set; }

        public string FullName { get; set; }
    }
    public class Project
    {
        [Key]
        public int ProjectsId { get; set; }

        [DisplayName("Project Name")]
        [StringLength(200)]
        public string ProjectName { get; set; }

        [DisplayName("Project Location")]
        [StringLength(200)]
        public string Location { get; set; }

        [DisplayName("Client Name")]
        [StringLength(200)]
        public string ClientName { get; set; }

        public virtual ICollection<BillOfQuantity> Projects { get; set; }
    }

    //public class ProjectCategory
    //{

    //}

    //public class ProjectType
    //{
    //    [Key]
    //    public int ProjectTypeId { get; set; }
    //    public string Name { get; set; }
    //    public ProjectTypes ProjectTypes { get; set; }
    //   public string Remark { get; set; }

    //}
    public class Block
    {
        [Key]
        public int BlockId { get; set; }
        public string Name { get; set; }
        [Display(Name = "No. of Blocks")]
        public int NumberOfBlocks { get; set; }
        [DisplayName("Project Type")]
        public int ProjectTypeId { get; set; }
        //public virtual ProjectType ProjectType { get; set; }
        public int ProjectInformationId { get; set; }
        [Display(Name = "Type")]
        public int SubProjectTypeId { get; set; }

        public string Remark { get; set; }

    }
    public class ProjectTypeMapping
    {
        [Key]
        public int ProjectTypeMappingId { get; set; }

        //public int ProjectTypeId { get; set; }
        [Display(Name = "Project Type")]
        public virtual AvailableProjectTypes ProjectType { get; set; }
        [Display(Name ="Structure Type")]
        public int ConfigurationId { get; set; }
        public virtual Configuration Configuration { get; set; }
        [Display(Name = "Code/series")]
        public int? CodeOrSeries { get; set; }
        public string Remark { get; set; }

    }

    public class BlockMapping
    {
        [Key]
        public int BlockMappingId { get; set; }

        public int BlockId { get; set; }
        public virtual Block Block { get; set; }
        public int ConfigurationId { get; set; }
        public virtual Configuration Configuration { get; set; }
        public string Remark { get; set; }

    }
    /// <summary>
    /// This is used to hold the information of project specific sub-types
    /// eg. building types in building projects such as 1&2 bed room G+5 blocks
    /// </summary>
    public class SubProjectType : SortableEntityBase
    {
        public int SubProjectTypeId { get; set; }
        public string Name { get; set; }
        [Display(Name = "No. of Blocks")]
        public int NumberOfBlocks { get; set; }
        public int ProjectInformationId { get; set; }
        public string Remark { get; set; }
    }
    public class SubProjectStructureTypeMapping : TrackUserOperation
    {
        public int SubProjectStructureTypeMappingId { get; set; }
        public int SubProjectTypeId { get; set; }
        public virtual SubProjectType SubProject { get; set; }
        public int ConfigurationId { get; set; }
        public virtual Configuration Configuration { get; set; }
        public string Remark { get; set; }
    }
    public class Road
    {
        [Key]
        public int RoadId { get; set; }
        public string Name { get; set; }

        //[DisplayName("Project Type")]
        //public int ProjectTypeId { get; set; }
        //public virtual ProjectType ProjectType { get; set; }
        public int ProjectInformationId { get; set; }
        public virtual ProjectInformation ProjectInformation { get; set; }
        public string Remark { get; set; }

    }
    public class RoadMapping
    {
        [Key]
        public int RoadMappingId { get; set; }

        public int RoadId { get; set; }
        public virtual Road Road { get; set; }
        public int ConfigurationId { get; set; }
        public virtual Configuration Configuration { get; set; }
        public string Remark { get; set; }

    }
}
