namespace ExceedERP.Reporting.Planning.Design
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for PlanReport.
    /// </summary>
    public partial class PlanReport : Telerik.Reporting.Report
    {
        public PlanReport()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            textBox171.Value = "= Fields.Indicators + '\r\nWeight: ' + IIf(Format('{0:0.##}', Fields.Weight) = '', '-', Format('{0:0.##}', Fields.Weight))";
            textBox274.Value = "= Fields.SubIndicators + '\r\nWeight: ' + IIf(Format('{0:0.##}', Fields.Weight) = '', '-', Format('{0:0.##}', Fields.Weight))";
            textBox274.Bindings.Add(new Telerik.Reporting.Binding("Style.Padding.Left",
                "= IIf(Fields.IndicatorLevel > 1, (Fields.IndicatorLevel - 1) * 6, 0) + 'pt'"));
            textBox277.Value = "Sub-Indicators / Weight";

            ReportParameters.Add(new ReportParameter
            {
                Name = "IncludeInactive",
                Type = ReportParameterType.Boolean,
                Value = "= False",
                Visible = false
            });
            PlanODS.Parameters.Add(new ObjectDataSourceParameter(
                "includeInactive",
                typeof(bool),
                "= Parameters.IncludeInactive.Value"));
        }
    }
}
