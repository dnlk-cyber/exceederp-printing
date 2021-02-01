namespace ExceedERP.Reporting.HRM.HRM
{
    using ExceedERP.Core.Localization;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for TerminationLetter.
    /// </summary>
    public partial class EmployeePerformanceSummary : Telerik.Reporting.Report
    {
        public EmployeePerformanceSummary()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

           // reportFooterSection1.Visible = false;
            //
            // TODO: Add any constructor code after InitializeComponent call
            this.DataSource = EvaluationTypeObjectDataSource;

            var culture = /*"am-ET";*/System.Threading.Thread.CurrentThread.CurrentUICulture.ToString();
            var cultures = new CultureInfo(culture);
            this.txtPerformanceEvaluation.Value = Resources.ResourceManager.GetString("Rpt_PerformanceEvaluation", cultures);
            this.txtEvaluationPeriod.Value = Resources.ResourceManager.GetString("Rpt_EvaluationPeriod", cultures);
            this.txtName.Value = Resources.ResourceManager.GetString("Rpt_Name", cultures);
            this.txtDepartment.Value = Resources.ResourceManager.GetString("Rpt_Department", cultures);
            this.txtJobtitle.Value = Resources.ResourceManager.GetString("Rpt_Jobtitle", cultures);
            this.txtRowNo.Value = Resources.ResourceManager.GetString("Rpt_RowNo", cultures);
            this.txtCriteria.Value = Resources.ResourceManager.GetString("Rpt_Criteria", cultures);
            this.txtEvaluationWeight.Value = Resources.ResourceManager.GetString("Rpt_EvaluationWeight", cultures);
            this.txtScore.Value = Resources.ResourceManager.GetString("Rpt_Score", cultures);
            this.txtSum.Value = Resources.ResourceManager.GetString("Rpt_Sum", cultures);
            this.txtCommentFromEvaluator.Value = Resources.ResourceManager.GetString("Rpt_CommentFromEvaluatior", cultures);
            this.txtThingsTobeChanged.Value = Resources.ResourceManager.GetString("Rpt_ThingsTobeChanged", cultures);
            this.txtEmployeeComment.Value = Resources.ResourceManager.GetString("Rpt_EmployeeComment", cultures);
            this.txtSign.Value = Resources.ResourceManager.GetString("Rpt_Sign", cultures);

            this.txtDate.Value = Resources.ResourceManager.GetString("Rpt_Date", cultures);
            this.txtApprovalDate.Value = Resources.ResourceManager.GetString("Rpt_Date", cultures);
            this.txtSignDate.Value = Resources.ResourceManager.GetString("Rpt_Date", cultures);
            this.txtApproverSign.Value = Resources.ResourceManager.GetString("Rpt_Sign", cultures);
            this.txtApproverName.Value = Resources.ResourceManager.GetString("Rpt_Name", cultures);
        }
    }
}