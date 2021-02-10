using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.CMS
{
    public class WelderPerformanceQualification : TrackUserSettingOperation
    {
        [Key]
        public int WelderPerformanceQualificationId { get; set; }
        public int CmsTraineeId { get; set; }
        public CmsTrainee CmsTrainee { get; set; }
        [Display(Name = "Welder’s SS No. ")]
        public string WelderSSNumber { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString ="{0:yyyy-MM-dd}", ApplyFormatInEditMode =true)]
        public DateTime Date { get; set; }
        [Display(Name = "ATF No.")]
        public string ATFNumber { get; set; }
        public string TestNumber { get; set; }
        [Display(Name = "Reference WPS No")]
        public string ReferenceWPSNumber { get; set; }

        public string CodeOrSpecificationUsed { get; set; }
        public string RangeOfCodeOrSpecificationUsed { get; set; }

        public string WeldingProcess { get; set; }
        public string RangeOfWeldingProcess { get; set; }
        public string WeldingType { get; set; }
        public string RangeOfWeldingType { get; set; }

        public UsedNotUsedSelectItem BackingMaterial { get; set; }
        public string RangeOfBackingMaterial { get; set; }

        // base metal
        public string SpecOfBaseMetal { get; set; }
        public string RangeOfSpecOfBaseMetal { get; set; }

        public string GrooveBaseMetalPlateOrPipe { get; set; }
        public string ThicknessOfGrooveBaseMetalPlateOrPipe { get; set; }
        public string RangeThicknessOfGrooveBaseMetalPlateOrPipe { get; set; }

        
        public string FilletBaseMetalPlateOrPipe { get; set; }
        public string ThicknessOfFilletBaseMetalPlateOrPipe { get; set; }
        public string RangeOfThicknessOfFilletBaseMetalPlateOrPipe { get; set; }

        public string GroovePipeOrTubularOutsideDiameter { get; set; }
        public string RangeOfGroovePipeOrTubularOutsideDiameter { get; set; }
        public string FilletPipeOrTubularOutsideDiameter { get; set; }
        public string RangeOfFilletPipeOrTubularOutsideDiameter { get; set; }

        // filler metal
        public string FillerMetalSpecificationNumber { get; set; }
        public string RangeFillerMetalSpecificationNumber { get; set; }

        public string FillerMetalClassificationNumber { get; set; }
        public string RangeOfFillerMetalClassificationNumber { get; set; }

        public string FillerMetalFNumber { get; set; }
        public string RangeOfFillerMetalFNumber { get; set; }

        public string FillerMetalDiameter { get; set; }
        public string RangeOfFillerMetalDiameter { get; set; }

        public YesOrNoSelectItem ConsumableInsert { get; set; }
        public YesOrNoSelectItem PenetrationEnhanceFlux { get; set; }

        // welding metal 
        public string WeldingMetalThickness { get; set; }
        public string RangeOfWeldingMetalThickness { get; set; }

        // polarity
        public string CurrentPolarityType { get; set; }
        public string RangeOfCurrentPolarityType { get; set; }

        public string MetalTransferModel { get; set; }
        public string RangeOfMetalTransferModel { get; set; }

        // torch shielding gas
        public string TypeOfTorchShieldingGas { get; set; }
        public string RangeOfTypeOfTorchShieldingGas { get; set; }
        public string FlowOfTorchShieldingGas { get; set; }
        public string RangeOfFlowOfTorchShieldingGas { get; set; }


        // root shielding gas
        public string TypeOfRootShieldingGas { get; set; }
        public string RangeOfTypeOfRootShieldingGas { get; set; }
        public string FlowOfRootShieldingGas { get; set; }
        public string RangeOfFlowOfRootShieldingGas { get; set; }

        // positions
        public string TestPositions { get; set; }
        public string QualifiedPositions { get; set; }

        public UphilOrDownhillSelectItem VerticalPropagation { get; set; }
        public string RangeOfVerticalPropagation { get; set; }

    
        // other tests
        public bool IsVisualTestingPerformedAndAccepted { get; set; }
        public bool IsRadiographicTestingPerformedAndAccepted { get; set; }
        public bool IsFractureTestingPerformedAndAccepted { get; set; }
        public bool IsBendTestingPerformedAndAccepted { get; set; }
        public bool IsNotchTensileTestingPerformedAndAccepted { get; set; }
        public bool IsMacroscopicExaminationPerformedAndAccepted { get; set; }
        public bool IsAdditionalTestingPerformedAndAccepted { get; set; }






    }
}
