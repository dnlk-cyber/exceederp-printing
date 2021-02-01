namespace ExceedERP.Reporting.Fleet.View
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for PlanEvaluation.
    /// </summary>
    public partial class PlanPerEquipmentEvaluation : Telerik.Reporting.Report
    {
        public PlanPerEquipmentEvaluation()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            this.pageHeaderSection1.Report.DataSource = headerComp;
        }
    }
}