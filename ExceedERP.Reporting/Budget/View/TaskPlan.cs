namespace ExceedERP.Reporting.Budget.View
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for TaskPlan.
    /// </summary>
    public partial class TaskPlan : Telerik.Reporting.Report
    {
        public TaskPlan()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            this.DataSource = TaskPlanDS;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}