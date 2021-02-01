using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM
{
  
    public enum PerformanceSelfEvaluation
    {
        Enable_Self_Evaluation
    }
    public enum HRMSettingCatagories
    {
        EmployeeID,
        Leave,
        Probation_Setting,
        Self_Evaluation
    }
    //////////////////

    public enum ProbationSetting
    {
        Probation_Period
    }

    public enum EmployeeIDAbbreviationType
    {
        Company,
        Year,
        Sequential_Number
    }

    public enum LeaveHrmSetting
    {
        Frequency,
        [Display(Name = "Initial Year/Gerigorian/")]
        InitialYear,
        Maternitiy
    }
    public enum EmployeeIdAttribute
    {
        Auto_Generate
    }

    public enum ProbationMeasurement
    {
        Very_Good,
        Good,
        Enough,
        Low
    }

    public enum LeaveActionTypes
    {
        Without_Plan,
        With_Plan,
        Forced
    }

    public enum TemplateTypes
    {
        ExperianceAm,
        ExperianceEng,
        WarrantyAm,
        TerminationAcceptanceAm
    }
}
