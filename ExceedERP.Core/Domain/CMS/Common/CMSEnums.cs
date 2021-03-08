using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.CMS
{
    public enum TraineeType
    {
        Individual,
        Company
    }
    public enum TrainingProgramStatus
    {
        Registered = 0,
        OnTraining = 1,
        Completed = 2,
    }
    public enum ISOChecklistValueType
    {
        Yes_Or_No,
        Numeric_Value
    }
    public enum CmsTraineeIdentificationType
    {
        //[Display(Name = "Enum_IdentificationType_RESIDENTIAL_ID", ResourceType = typeof(Resources))]
        RESIDENTIAL_ID = 0,
        //[Display(Name = "Enum_IdentificationType_PENSION_NO", ResourceType = typeof(Resources))]
        PENSION_NO = 1,
        //[Display(Name = "Enum_IdentificationType_STUDENT_ID", ResourceType = typeof(Resources))]
        STUDENT_ID = 2,
        //[Display(Name = "Enum_IdentificationType_WORK_ID", ResourceType = typeof(Resources))]
        WORK_ID = 3,
        //[Display(Name = "Enum_IdentificationType_DRIVING_LICENSE", ResourceType = typeof(Resources))]
        DRIVING_LICENSE = 4,
        //[Display(Name = "Enum_IdentificationType_SMART_CARD", ResourceType = typeof(Resources))]
        SMART_CARD = 5,
        //[Display(Name = "Enum_IdentificationType_PASSPORT", ResourceType = typeof(Resources))]
        PASSPORT=6,
        //[Display(Name = "Enum_IdentificationType_VISA", ResourceType = typeof(Resources))]
        VISA=7,
        //[Display(Name = "Enum_IdentificationType_BIOMETRIC_ID", ResourceType = typeof(Resources))]
        BIOMETRIC_ID=8,
        //[Display(Name = "Enum_IdentificationType_TIN", ResourceType = typeof(Resources))]
        TIN=9,
        //[Display(Name = "Enum_IdentificationType_VAT_NUMBER", ResourceType = typeof(Resources))]
        VAT_NUMBER=10,
        //[Display(Name = "Enum_IdentificationType_LICENSE_NUMBER", ResourceType = typeof(Resources))]
        LICENSE_NUMBER=11
    }
    public enum WeldingProcess
    {
        Manual,
        Mechanized,
        Semiautomatic,
        Automatic
    }
    public enum WeldingType
    {
        BW,
        FW
    }
    public enum UsedNotUsedSelectItem
    {
        Used,
        NotUsed
    }
    public enum PlateOrPipeSelectItem
    {
        Plate,
        Pipe
    }
    public enum YesOrNoSelectItem
    {
        Yes,
        No
    }
    public enum UphilOrDownhillSelectItem
    {
        Uphill,
        Downhill
    }
}
