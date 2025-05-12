using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Printing
{

    public class PrintingProcess : TrackUserSettingOperation
    {
        [Key]
        public int PrintingProcessId { get; set; }
        [Required]
        public PressCategory PressCategory { get; set; }
        [Required]
        public LabourActivity LabourActivity { get; set; }
        [Display(Name = "Machine Type")]
        public int? PrintingMachineTypeId { get; set; }
        [Required]
        public string UOM { get; set; }
        [Required]
        public decimal LaborRate { get; set; } // chargeable hours rate in birr

    }

    public enum PressCategory
    {
        [Display(Name = "Pre Press")]
        PrePress = 1,
        [Display(Name = "Press")]
        Press = 2,
        [Display(Name = "Post Press")]
        PostPress = 3
    }

    public enum LabourActivity
    {
        // Pre Press
        [Display(Name = "Design")]
        Design = 101,
        [Display(Name = "Art Work")]
        ArtWork,
        [Display(Name = "Camera Work")]
        CameraWork,
        [Display(Name = "Color Separation")]
        ColorSeparation,
        [Display(Name = "Composition")]
        Composition,
        [Display(Name = "Cliche")]
        Cliche,
        [Display(Name = "Text Plate Making")]
        TextPlateMaking,
        [Display(Name = "Cover Plate Making")]
        CoverPlateMaking,
        [Display(Name = "CTP")]
        CTP,
        [Display(Name = "Proofing")]
        Proofing,

        // Press
        [Display(Name = "Make Ready Text")]
        MakeReadyText = 201,
        [Display(Name = "Make Ready Cover")]
        MakeReadyCover,
        [Display(Name ="Make Ready Numbering")]
        MakeReadyNumbering,
        [Display(Name = "Text Print")]
        TextPrint,
        [Display(Name = "Cover Print")]
        CoverPrint,
        [Display(Name ="Letter Press Print")]
        LetterPressPrint,
        // Post Press
        [Display(Name = "Cutting For Text")]
        CuttingForText = 301,
        [Display(Name = "Cutting For Cover")]
        CuttingForCover,
        [Display(Name = "Die Cutting")]
        DieCutting,
        [Display(Name = "Drilling")]
        Drilling,
        [Display(Name = "Punching")]
        Punching,
        [Display(Name = "Machine Folding")]
        MachineFolding,
        [Display(Name = "Folding Cover")]
        FoldingCover,
        [Display(Name = "Collating")]
        Collating,
        [Display(Name = "Perforation")]
        Perforation,
        [Display(Name = "Inserting")]
        Inserting,
        [Display(Name = "Side Stitching")]
        SideStitching,
        [Display(Name = "Thread Sewing")]
        ThreadSewing,
        [Display(Name = "Creasing")]
        Creasing,
        [Display(Name = "Peeling")]
        Peeling,
        [Display(Name = "Hand Work")]
        HandWork,
        [Display(Name = "Counting")]
        Counting,
        [Display(Name = "Padding")]
        Padding,
        [Display(Name = "Stitching")]
        Stitching,
        [Display(Name = "Gluing")]
        Gluing,
        [Display(Name = "Hard Cover Making Manual")]
        HardCoverMakingManual,
        [Display(Name = "Gilding")]
        Gilding,
        [Display(Name = "Perfect Binding")]
        PerfectBinding,
        [Display(Name = "Spiral Binding")]
        SpiralBinding,
        [Display(Name = "Trimming")]
        Trimming,
        [Display(Name = "Corner Cutting")]
        CornerCutting,
        [Display(Name = "Eyeleting")]
        Eyeleting,
        [Display(Name = "Checking And Packing")]
        CheckingAndPacking,
        [Display(Name ="Loading/Unloading")]
        LoadingAndUnloading,
        [Display(Name ="Dispatching/Transporting")]
        DispatchingAndTransport,
        [Display(Name = "Manual Folding Cover")]
        ManualFoldingCover,
        CuttingForNumbering,
    }

}
