namespace ExceedERP.Reporting.Admin.Views
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for UserGroupsRoles.
    /// </summary>
    public partial class UserGroupsRoles : Telerik.Reporting.Report
    {
        public UserGroupsRoles()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource = Organization;

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}