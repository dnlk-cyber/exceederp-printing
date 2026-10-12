using ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource;

namespace ExceedERP.Reporting.HRM.HRM
{
    partial class GenderPercentage
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GenderPercentage));
            Telerik.Reporting.TableGroup tableGroupIndex = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroupGender = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroupCount = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroupPercent = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroupDetailRow = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroupTotalRow = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.InstanceReportSource instanceReportSource1 = new Telerik.Reporting.InstanceReportSource();

            // Parameters Declarations
            Telerik.Reporting.ReportParameter reportParameterBranch = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameterBU = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameterOrg = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameterSex = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameterStatus = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameterEmpType = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameterAge = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameterFormTitle = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameterFormNumber = new Telerik.Reporting.ReportParameter();

            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();

            // Table Column Headers
            this.thIndex = new Telerik.Reporting.TextBox();
            this.thGender = new Telerik.Reporting.TextBox();
            this.thEmployeeCount = new Telerik.Reporting.TextBox();
            this.thPercentage = new Telerik.Reporting.TextBox();

            // Subreport & Data Sources
            this.headerSubReport2 = new ExceedERP.Reporting.SharedDesigners.HeaderSubReport();
            this.BranchObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.BusinessUnitobjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.WorkUnitObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.SexODS = new Telerik.Reporting.ObjectDataSource();
            this.StatusODS = new Telerik.Reporting.ObjectDataSource();
            this.EmployementTypeobjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.ageObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.empSumaryObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.Localization = new Telerik.Reporting.ObjectDataSource();
            this.Organization = new Telerik.Reporting.ObjectDataSource();

            // Detail Section & Table Cells
            this.detail = new Telerik.Reporting.DetailSection();
            this.tblEmployeeSumm = new Telerik.Reporting.Table();
            this.tdIndex = new Telerik.Reporting.TextBox();
            this.tdGender = new Telerik.Reporting.TextBox();
            this.tdEmployeeCount = new Telerik.Reporting.TextBox();
            this.tdPercentage = new Telerik.Reporting.TextBox();
            this.tfTotalLabel = new Telerik.Reporting.TextBox();
            this.tfTotalCount = new Telerik.Reporting.TextBox();
            this.tfTotalPercentage = new Telerik.Reporting.TextBox();

            // Filter Information Badges
            this.textBox3 = new Telerik.Reporting.TextBox();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.textBox7 = new Telerik.Reporting.TextBox();
            this.textBox5 = new Telerik.Reporting.TextBox();
            this.textBox13 = new Telerik.Reporting.TextBox();
            this.textBox12 = new Telerik.Reporting.TextBox();
            this.textBox15 = new Telerik.Reporting.TextBox();
            this.textBox14 = new Telerik.Reporting.TextBox();
            this.textBox21 = new Telerik.Reporting.TextBox();
            this.textBox17 = new Telerik.Reporting.TextBox();

            // Page Footer & Report Header Sections
            this.pageFooterSection1 = new Telerik.Reporting.PageFooterSection();
            this.textBox16 = new Telerik.Reporting.TextBox();
            this.Page = new Telerik.Reporting.TextBox();
            this.reportHeaderSection1 = new Telerik.Reporting.ReportHeaderSection();
            this.shape3 = new Telerik.Reporting.Shape();
            this.subReport1 = new Telerik.Reporting.SubReport();

            ((System.ComponentModel.ISupportInitialize)(this.headerSubReport2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();

            // ========================================================
            // Table Column Headers (180 mm Printable A4 Canvas)
            // ========================================================
            // thIndex (15 mm)
            this.thIndex.Name = "thIndex";
            this.thIndex.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Mm(15D), Telerik.Reporting.Drawing.Unit.Mm(7.5D));
            this.thIndex.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.thIndex.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.thIndex.Style.BorderColor.Default = System.Drawing.Color.Gainsboro;
            this.thIndex.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.thIndex.Style.Font.Bold = true;
            this.thIndex.Style.Font.Name = "Nyala";
            this.thIndex.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.thIndex.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.thIndex.Value = "ተ.ቁ / No";

            // thGender (65 mm)
            this.thGender.Name = "thGender";
            this.thGender.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Mm(65D), Telerik.Reporting.Drawing.Unit.Mm(7.5D));
            this.thGender.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.thGender.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.thGender.Style.BorderColor.Default = System.Drawing.Color.Gainsboro;
            this.thGender.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.thGender.Style.Font.Bold = true;
            this.thGender.Style.Font.Name = "Nyala";
            this.thGender.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Mm(3D);
            this.thGender.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.thGender.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.thGender.Value = "ፆታ / Gender";

            // thEmployeeCount (50 mm)
            this.thEmployeeCount.Name = "thEmployeeCount";
            this.thEmployeeCount.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Mm(50D), Telerik.Reporting.Drawing.Unit.Mm(7.5D));
            this.thEmployeeCount.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.thEmployeeCount.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.thEmployeeCount.Style.BorderColor.Default = System.Drawing.Color.Gainsboro;
            this.thEmployeeCount.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.thEmployeeCount.Style.Font.Bold = true;
            this.thEmployeeCount.Style.Font.Name = "Nyala";
            this.thEmployeeCount.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.thEmployeeCount.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.thEmployeeCount.Value = "የሰራተኞች ብዛት / Number of Employees";

            // thPercentage (50 mm)
            this.thPercentage.Name = "thPercentage";
            this.thPercentage.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Mm(50D), Telerik.Reporting.Drawing.Unit.Mm(7.5D));
            this.thPercentage.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.thPercentage.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.thPercentage.Style.BorderColor.Default = System.Drawing.Color.Gainsboro;
            this.thPercentage.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.thPercentage.Style.Font.Bold = true;
            this.thPercentage.Style.Font.Name = "Nyala";
            this.thPercentage.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.thPercentage.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.thPercentage.Value = "ድርሻ በመቶኛ / Percentage (%)";

            // ========================================================
            // Table Detail Cells (Row 0)
            // ========================================================
            this.tdIndex.Name = "tdIndex";
            this.tdIndex.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Mm(15D), Telerik.Reporting.Drawing.Unit.Mm(7D));
            this.tdIndex.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.tdIndex.Style.BorderColor.Default = System.Drawing.Color.Gainsboro;
            this.tdIndex.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.tdIndex.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.tdIndex.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.tdIndex.Value = "= RowNumber()";

            this.tdGender.Name = "tdGender";
            this.tdGender.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Mm(65D), Telerik.Reporting.Drawing.Unit.Mm(7D));
            this.tdGender.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.tdGender.Style.BorderColor.Default = System.Drawing.Color.Gainsboro;
            this.tdGender.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.tdGender.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Mm(3D);
            this.tdGender.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.tdGender.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.tdGender.Value = "= Fields.Gender";

            this.tdEmployeeCount.Format = "{0:N0}";
            this.tdEmployeeCount.Name = "tdEmployeeCount";
            this.tdEmployeeCount.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Mm(50D), Telerik.Reporting.Drawing.Unit.Mm(7D));
            this.tdEmployeeCount.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.tdEmployeeCount.Style.BorderColor.Default = System.Drawing.Color.Gainsboro;
            this.tdEmployeeCount.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.tdEmployeeCount.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.tdEmployeeCount.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.tdEmployeeCount.Value = "= Fields.NumberOfEmployee";

            this.tdPercentage.Format = "{0:N2}%";
            this.tdPercentage.Name = "tdPercentage";
            this.tdPercentage.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Mm(50D), Telerik.Reporting.Drawing.Unit.Mm(7D));
            this.tdPercentage.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.tdPercentage.Style.BorderColor.Default = System.Drawing.Color.Gainsboro;
            this.tdPercentage.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.tdPercentage.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.tdPercentage.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.tdPercentage.Value = "= Fields.Percentage";

            // ========================================================
            // Table Footer Summary Cells (Row 1)
            // ========================================================
            this.tfTotalLabel.Name = "tfTotalLabel";
            this.tfTotalLabel.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Mm(80D), Telerik.Reporting.Drawing.Unit.Mm(7.5D));
            this.tfTotalLabel.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(248)))));
            this.tfTotalLabel.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.tfTotalLabel.Style.BorderColor.Default = System.Drawing.Color.Gainsboro;
            this.tfTotalLabel.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.tfTotalLabel.Style.Font.Bold = true;
            this.tfTotalLabel.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Mm(3D);
            this.tfTotalLabel.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.tfTotalLabel.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.tfTotalLabel.Value = "ድምር / Total";

            this.tfTotalCount.Format = "{0:N0}";
            this.tfTotalCount.Name = "tfTotalCount";
            this.tfTotalCount.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Mm(50D), Telerik.Reporting.Drawing.Unit.Mm(7.5D));
            this.tfTotalCount.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(248)))));
            this.tfTotalCount.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.tfTotalCount.Style.BorderColor.Default = System.Drawing.Color.Gainsboro;
            this.tfTotalCount.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.tfTotalCount.Style.Font.Bold = true;
            this.tfTotalCount.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.tfTotalCount.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.tfTotalCount.Value = "= Sum(Fields.NumberOfEmployee)";

            this.tfTotalPercentage.Format = "{0:N2}%";
            this.tfTotalPercentage.Name = "tfTotalPercentage";
            this.tfTotalPercentage.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Mm(50D), Telerik.Reporting.Drawing.Unit.Mm(7.5D));
            this.tfTotalPercentage.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(240)))), ((int)(((byte)(242)))));
            this.tfTotalPercentage.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.tfTotalPercentage.Style.BorderColor.Default = System.Drawing.Color.Silver;
            this.tfTotalPercentage.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1.5D);
            this.tfTotalPercentage.Style.Font.Bold = true;
            this.tfTotalPercentage.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.tfTotalPercentage.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.tfTotalPercentage.Value = "= Sum(Fields.Percentage)";

            // ========================================================
            // Table Structure & Layout
            // ========================================================
            this.tblEmployeeSumm.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Mm(15D)));
            this.tblEmployeeSumm.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Mm(65D)));
            this.tblEmployeeSumm.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Mm(50D)));
            this.tblEmployeeSumm.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Mm(50D)));

            this.tblEmployeeSumm.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Mm(7D)));
            this.tblEmployeeSumm.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Mm(7.5D)));

            this.tblEmployeeSumm.Body.SetCellContent(0, 0, this.tdIndex);
            this.tblEmployeeSumm.Body.SetCellContent(0, 1, this.tdGender);
            this.tblEmployeeSumm.Body.SetCellContent(0, 2, this.tdEmployeeCount);
            this.tblEmployeeSumm.Body.SetCellContent(0, 3, this.tdPercentage);

            this.tblEmployeeSumm.Body.SetCellContent(1, 0, this.tfTotalLabel, 1, 2);
            this.tblEmployeeSumm.Body.SetCellContent(1, 2, this.tfTotalCount);
            this.tblEmployeeSumm.Body.SetCellContent(1, 3, this.tfTotalPercentage);

            tableGroupIndex.Name = "groupIndex";
            tableGroupIndex.ReportItem = this.thIndex;

            tableGroupGender.Name = "groupGender";
            tableGroupGender.ReportItem = this.thGender;

            tableGroupCount.Name = "groupEmployeeCount";
            tableGroupCount.ReportItem = this.thEmployeeCount;

            tableGroupPercent.Name = "groupPercentage";
            tableGroupPercent.ReportItem = this.thPercentage;

            this.tblEmployeeSumm.ColumnGroups.Add(tableGroupIndex);
            this.tblEmployeeSumm.ColumnGroups.Add(tableGroupGender);
            this.tblEmployeeSumm.ColumnGroups.Add(tableGroupCount);
            this.tblEmployeeSumm.ColumnGroups.Add(tableGroupPercent);

            this.tblEmployeeSumm.DataSource = this.empSumaryObjectDataSource;
            this.tblEmployeeSumm.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
                this.tdIndex,
                this.tdGender,
                this.tdEmployeeCount,
                this.tdPercentage,
                this.tfTotalLabel,
                this.tfTotalCount,
                this.tfTotalPercentage,
                this.thIndex,
                this.thGender,
                this.thEmployeeCount,
                this.thPercentage
            });

            this.tblEmployeeSumm.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Mm(0D), Telerik.Reporting.Drawing.Unit.Mm(22D));
            this.tblEmployeeSumm.Name = "tblEmployeeSumm";

            tableGroupDetailRow.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroupDetailRow.Name = "detailTableGroup";
            tableGroupTotalRow.Name = "totalGroup";

            this.tblEmployeeSumm.RowGroups.Add(tableGroupDetailRow);
            this.tblEmployeeSumm.RowGroups.Add(tableGroupTotalRow);

            this.tblEmployeeSumm.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Mm(180D), Telerik.Reporting.Drawing.Unit.Mm(22D));
            this.tblEmployeeSumm.Style.Font.Name = "Nyala";

            // ========================================================
            // Data Source Definitions
            // ========================================================
            this.BranchObjectDataSource.DataMember = "GetBranch";
            this.BranchObjectDataSource.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.BranchObjectDataSource);
            this.BranchObjectDataSource.Name = "BranchObjectDataSource";

            this.BusinessUnitobjectDataSource.DataMember = "GetBusinessUnits";
            this.BusinessUnitobjectDataSource.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.BusinessUnitOds);
            this.BusinessUnitobjectDataSource.Name = "BusinessUnitobjectDataSource";

            this.WorkUnitObjectDataSource.DataMember = "GetDepartment";
            this.WorkUnitObjectDataSource.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.DepartmentObjectDataSource);
            this.WorkUnitObjectDataSource.Name = "WorkUnitObjectDataSource";

            this.SexODS.DataMember = "GetSex";
            this.SexODS.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.SexObjectDataSource);
            this.SexODS.Name = "SexODS";

            this.StatusODS.DataMember = "GetStatus";
            this.StatusODS.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.EmployeeStatusODS);
            this.StatusODS.Name = "StatusODS";

            this.EmployementTypeobjectDataSource.DataMember = "GetEmployeeTypess";
            this.EmployementTypeobjectDataSource.DataSource = typeof(ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource.EmployeeTypeObjectDataSource);
            this.EmployementTypeobjectDataSource.Name = "EmployementTypeobjectDataSource";

            this.ageObjectDataSource.DataMember = "GetAge";
            this.ageObjectDataSource.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.AgeObjectDataSource);
            this.ageObjectDataSource.Name = "ageObjectDataSource";

            this.Localization.Name = "Localization";
            this.Organization.DataMember = "GetOrganization";
            this.Organization.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.OrganizationObjectDataSource);
            this.Organization.Name = "Organization";

            // Object Data Source Parameters
            this.empSumaryObjectDataSource.DataMember = "GetGenderPercentage";
            this.empSumaryObjectDataSource.DataSource = typeof(ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource.GenderPercentageObjectDataSource);
            this.empSumaryObjectDataSource.Name = "empSumaryObjectDataSource";
            this.empSumaryObjectDataSource.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
                new Telerik.Reporting.ObjectDataSourceParameter("branchIds", typeof(object[]), "= Parameters.branchId.Value"),
                new Telerik.Reporting.ObjectDataSourceParameter("businessUnitId", typeof(string), "= Parameters.businessunit.Value"),
                new Telerik.Reporting.ObjectDataSourceParameter("orgStructureId", typeof(string), "= Parameters.orgStructureId.Value"),
                new Telerik.Reporting.ObjectDataSourceParameter("status", typeof(string), "= Parameters.status.Value"),
                new Telerik.Reporting.ObjectDataSourceParameter("employementType", typeof(string), "= Parameters.employementType.Value"),
                new Telerik.Reporting.ObjectDataSourceParameter("age", typeof(string), "= Parameters.age.Value")
            });

            // ========================================================
            // Detail Info Cards & Badges
            // ========================================================
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Mm(50D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
                this.tblEmployeeSumm,
                this.textBox3,
                this.textBox1,
                this.textBox7,
                this.textBox5,
                this.textBox13,
                this.textBox12,
                this.textBox15,
                this.textBox14,
                this.textBox21,
                this.textBox17
            });
            this.detail.Name = "detail";
            this.detail.Style.Font.Name = "Nyala";

            this.textBox3.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Mm(0D), Telerik.Reporting.Drawing.Unit.Mm(2D));
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Mm(22D), Telerik.Reporting.Drawing.Unit.Mm(5D));
            this.textBox3.Style.Font.Bold = true;
            resources.ApplyResources(this.textBox3, "textBox3");

            this.textBox1.Format = "{0:d}";
            this.textBox1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Mm(23D), Telerik.Reporting.Drawing.Unit.Mm(2D));
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Mm(65D), Telerik.Reporting.Drawing.Unit.Mm(5D));
            this.textBox1.Style.Font.Name = "Nyala";
            this.textBox1.Value = "= Join(\", \", Parameters.branchId.Label)";
            this.textBox7.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Mm(0D), Telerik.Reporting.Drawing.Unit.Mm(8D));
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Mm(22D), Telerik.Reporting.Drawing.Unit.Mm(5D));
            this.textBox7.Style.Font.Bold = true;
            resources.ApplyResources(this.textBox7, "textBox7");

            this.textBox5.Format = "{0:d}";
            this.textBox5.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Mm(23D), Telerik.Reporting.Drawing.Unit.Mm(8D));
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Mm(65D), Telerik.Reporting.Drawing.Unit.Mm(5D));
            this.textBox5.Style.Font.Name = "Nyala";
            this.textBox5.Value = "= Parameters.orgStructureId.Label";

            this.textBox13.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Mm(95D), Telerik.Reporting.Drawing.Unit.Mm(2D));
            this.textBox13.Name = "textBox13";
            this.textBox13.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Mm(22D), Telerik.Reporting.Drawing.Unit.Mm(5D));
            this.textBox13.Style.Font.Bold = true;
            resources.ApplyResources(this.textBox13, "textBox13");

            this.textBox12.Format = "{0:d}";
            this.textBox12.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Mm(118D), Telerik.Reporting.Drawing.Unit.Mm(2D));
            this.textBox12.Name = "textBox12";
            this.textBox12.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Mm(62D), Telerik.Reporting.Drawing.Unit.Mm(5D));
            this.textBox12.Style.Font.Name = "Nyala";
            this.textBox12.Value = "= Parameters.businessunit.Label";

            this.textBox15.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Mm(95D), Telerik.Reporting.Drawing.Unit.Mm(8D));
            this.textBox15.Name = "textBox15";
            this.textBox15.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Mm(22D), Telerik.Reporting.Drawing.Unit.Mm(5D));
            this.textBox15.Style.Font.Bold = true;
            resources.ApplyResources(this.textBox15, "textBox15");

            this.textBox14.Format = "{0:d}";
            this.textBox14.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Mm(118D), Telerik.Reporting.Drawing.Unit.Mm(8D));
            this.textBox14.Name = "textBox14";
            this.textBox14.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Mm(62D), Telerik.Reporting.Drawing.Unit.Mm(5D));
            this.textBox14.Style.Font.Name = "Nyala";
            this.textBox14.Value = "= Parameters.status.Label";

            this.textBox21.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Mm(0D), Telerik.Reporting.Drawing.Unit.Mm(14D));
            this.textBox21.Name = "textBox21";
            this.textBox21.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Mm(25D), Telerik.Reporting.Drawing.Unit.Mm(5D));
            this.textBox21.Style.Font.Bold = true;
            resources.ApplyResources(this.textBox21, "textBox21");

            this.textBox17.Format = "{0:d}";
            this.textBox17.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Mm(26D), Telerik.Reporting.Drawing.Unit.Mm(14D));
            this.textBox17.Name = "textBox17";
            this.textBox17.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Mm(62D), Telerik.Reporting.Drawing.Unit.Mm(5D));
            this.textBox17.Style.Font.Name = "Nyala";
            this.textBox17.Value = "= Parameters.employementType.Label";

            // ========================================================
            // Page Footer
            // ========================================================
            this.pageFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Mm(8D);
            this.pageFooterSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
                this.textBox16,
                this.Page
            });
            this.pageFooterSection1.Name = "pageFooterSection1";
            this.pageFooterSection1.Style.Font.Name = "Nyala";

            this.textBox16.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Mm(120D), Telerik.Reporting.Drawing.Unit.Mm(1D));
            this.textBox16.Name = "textBox16";
            this.textBox16.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Mm(60D), Telerik.Reporting.Drawing.Unit.Mm(5D));
            this.textBox16.Style.Font.Bold = true;
            this.textBox16.Style.Font.Name = "Nyala";
            this.textBox16.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            resources.ApplyResources(this.textBox16, "textBox16");

            this.Page.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Mm(95D), Telerik.Reporting.Drawing.Unit.Mm(1D));
            this.Page.Name = "Page";
            this.Page.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Mm(23D), Telerik.Reporting.Drawing.Unit.Mm(5D));
            this.Page.Style.Font.Bold = true;
            this.Page.Style.Font.Name = "Nyala";
            this.Page.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            resources.ApplyResources(this.Page, "Page");

            // ========================================================
            // Report Header
            // ========================================================
            this.reportHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Mm(20D);
            this.reportHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
                this.shape3,
                this.subReport1
            });
            this.reportHeaderSection1.Name = "reportHeaderSection1";
            this.reportHeaderSection1.Style.Font.Name = "Nyala";

            this.shape3.Angle = 0D;
            this.shape3.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Mm(0D), Telerik.Reporting.Drawing.Unit.Mm(16D));
            this.shape3.Name = "shape3";
            this.shape3.ShapeType = new Telerik.Reporting.Drawing.Shapes.LineShape(Telerik.Reporting.Drawing.Shapes.LineDirection.NWSE);
            this.shape3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Mm(180D), Telerik.Reporting.Drawing.Unit.Point(3.5D));
            this.shape3.Stretch = true;
            this.shape3.Style.BorderStyle.Bottom = Telerik.Reporting.Drawing.BorderType.Solid;
            this.shape3.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.Solid;
            this.shape3.Style.BorderWidth.Bottom = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.shape3.Style.BorderWidth.Top = Telerik.Reporting.Drawing.Unit.Point(1.5D);
            this.shape3.Style.Color = System.Drawing.Color.Transparent;

            this.headerSubReport2.Name = "HeaderSubReport";
            this.subReport1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Mm(0D), Telerik.Reporting.Drawing.Unit.Mm(0D));
            this.subReport1.Name = "subReport1";
            instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("FormTitle", "= Parameters.FormTitle.Value"));
            instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("FormNumber", "= Parameters.FormNumber.Value"));
            instanceReportSource1.ReportDocument = this.headerSubReport2;
            this.subReport1.ReportSource = instanceReportSource1;
            this.subReport1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Mm(180D), Telerik.Reporting.Drawing.Unit.Mm(15D));

            // ========================================================
            // Report Master Setup: A4 Page
            // ========================================================
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
                this.detail,
                this.pageFooterSection1,
                this.reportHeaderSection1
            });
            this.Name = "GenderPercentage";
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(
                Telerik.Reporting.Drawing.Unit.Mm(15D),
                Telerik.Reporting.Drawing.Unit.Mm(15D),
                Telerik.Reporting.Drawing.Unit.Mm(15D),
                Telerik.Reporting.Drawing.Unit.Mm(15D)
            );
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.A4;
            this.Width = Telerik.Reporting.Drawing.Unit.Mm(180D);

            // ========================================================
            // Report Parameters Definitions (Cleanly Ordered)
            // ========================================================
            // 1. Branch
            reportParameterBranch.AllowNull = true;
            reportParameterBranch.AvailableValues.DataSource = this.BranchObjectDataSource;
            reportParameterBranch.AvailableValues.DisplayMember = "= Fields.Name";
            reportParameterBranch.AvailableValues.ValueMember = "= Fields.BranchId";
            reportParameterBranch.MultiValue = true;
            reportParameterBranch.Name = "branchId";
            resources.ApplyResources(reportParameterBranch, "reportParameter1");
            reportParameterBranch.Visible = true;

            // 2. Business Unit
            reportParameterBU.AllowNull = true;
            reportParameterBU.AvailableValues.DataSource = this.BusinessUnitobjectDataSource;
            reportParameterBU.AvailableValues.DisplayMember = "= Fields.Name";
            reportParameterBU.AvailableValues.ValueMember = "= Fields.BusinessUnitId";
            reportParameterBU.Name = "businessunit";
            resources.ApplyResources(reportParameterBU, "reportParameter2");
            reportParameterBU.Visible = true;

            // 3. Org Structure
            reportParameterOrg.AllowNull = true;
            reportParameterOrg.AvailableValues.DataSource = this.WorkUnitObjectDataSource;
            reportParameterOrg.AvailableValues.DisplayMember = "= Fields.Name";
            reportParameterOrg.AvailableValues.ValueMember = "= Fields.OrgStructureId";
            reportParameterOrg.Name = "orgStructureId";
            resources.ApplyResources(reportParameterOrg, "reportParameter3");
            reportParameterOrg.Visible = true;

            // 4. Sex
            reportParameterSex.AllowNull = true;
            reportParameterSex.AvailableValues.DataSource = this.SexODS;
            reportParameterSex.AvailableValues.DisplayMember = "= Fields.Item";
            reportParameterSex.AvailableValues.ValueMember = "= Fields.Item";
            reportParameterSex.Name = "sex";
            resources.ApplyResources(reportParameterSex, "reportParameter4");
            reportParameterSex.Visible = true;

            // 5. Status
            reportParameterStatus.AllowNull = true;
            reportParameterStatus.AvailableValues.DataSource = this.StatusODS;
            reportParameterStatus.AvailableValues.DisplayMember = "= Fields.Item";
            reportParameterStatus.AvailableValues.ValueMember = "= Fields.Item";
            reportParameterStatus.Name = "status";
            resources.ApplyResources(reportParameterStatus, "reportParameter5");
            reportParameterStatus.Value = "Active";
            reportParameterStatus.Visible = true;

            // 6. Employment Type
            reportParameterEmpType.AllowNull = true;
            reportParameterEmpType.AvailableValues.DataSource = this.EmployementTypeobjectDataSource;
            reportParameterEmpType.AvailableValues.DisplayMember = "= Fields.Item";
            reportParameterEmpType.AvailableValues.ValueMember = "= Fields.Item";
            reportParameterEmpType.Name = "employementType";
            resources.ApplyResources(reportParameterEmpType, "reportParameter9");
            reportParameterEmpType.Visible = true;

            // 7. Age Range
            reportParameterAge.AllowNull = true;
            reportParameterAge.AvailableValues.DataSource = this.ageObjectDataSource;
            reportParameterAge.AvailableValues.DisplayMember = "= Fields.Item";
            reportParameterAge.AvailableValues.ValueMember = "= Fields.Item";
            reportParameterAge.Name = "age";
            reportParameterAge.Text = "Age Range";
            reportParameterAge.Visible = true;

            // 8. Headers
            reportParameterFormTitle.AllowNull = true;
            reportParameterFormTitle.Name = "FormTitle";

            reportParameterFormNumber.AllowNull = true;
            reportParameterFormNumber.Name = "FormNumber";

            // Add Parameters to Report
            this.ReportParameters.Add(reportParameterBranch);
            this.ReportParameters.Add(reportParameterBU);
            this.ReportParameters.Add(reportParameterOrg);
            this.ReportParameters.Add(reportParameterSex);
            this.ReportParameters.Add(reportParameterStatus);
            this.ReportParameters.Add(reportParameterEmpType);
            this.ReportParameters.Add(reportParameterAge);
            this.ReportParameters.Add(reportParameterFormTitle);
            this.ReportParameters.Add(reportParameterFormNumber);

            // Styling Rules
            styleRule1.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
                new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.TextItemBase)),
                new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.HtmlTextBox))
            });
            styleRule1.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(3D);
            styleRule1.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(3D);
            this.StyleSheet.AddRange(new Telerik.Reporting.Drawing.StyleRule[] { styleRule1 });

            ((System.ComponentModel.ISupportInitialize)(this.headerSubReport2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
        }
        #endregion

        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.PageFooterSection pageFooterSection1;
        private Telerik.Reporting.ObjectDataSource Organization;
        private Telerik.Reporting.ObjectDataSource ageObjectDataSource;
        private Telerik.Reporting.ObjectDataSource empSumaryObjectDataSource;
        private Telerik.Reporting.Table tblEmployeeSumm;
        private Telerik.Reporting.TextBox thIndex;
        private Telerik.Reporting.TextBox thGender;
        private Telerik.Reporting.TextBox thEmployeeCount;
        private Telerik.Reporting.TextBox thPercentage;
        private Telerik.Reporting.TextBox tdIndex;
        private Telerik.Reporting.TextBox tdGender;
        private Telerik.Reporting.TextBox tdEmployeeCount;
        private Telerik.Reporting.TextBox tdPercentage;
        private Telerik.Reporting.TextBox tfTotalLabel;
        private Telerik.Reporting.TextBox tfTotalCount;
        private Telerik.Reporting.TextBox tfTotalPercentage;
        private Telerik.Reporting.ReportHeaderSection reportHeaderSection1;
        private Telerik.Reporting.TextBox textBox16;
        private Telerik.Reporting.ObjectDataSource Localization;
        private Telerik.Reporting.ObjectDataSource BranchObjectDataSource;
        private Telerik.Reporting.ObjectDataSource WorkUnitObjectDataSource;
        private Telerik.Reporting.TextBox Page;
        private Telerik.Reporting.ObjectDataSource SexODS;
        private Telerik.Reporting.ObjectDataSource StatusODS;
        private Telerik.Reporting.TextBox textBox3;
        private Telerik.Reporting.TextBox textBox1;
        private Telerik.Reporting.TextBox textBox7;
        private Telerik.Reporting.TextBox textBox5;
        private Telerik.Reporting.TextBox textBox13;
        private Telerik.Reporting.TextBox textBox12;
        private Telerik.Reporting.TextBox textBox15;
        private Telerik.Reporting.TextBox textBox14;
        private Telerik.Reporting.SubReport subReport1;
        private Telerik.Reporting.Shape shape3;
        private SharedDesigners.HeaderSubReport headerSubReport2;
        private Telerik.Reporting.ObjectDataSource EmployementTypeobjectDataSource;
        private Telerik.Reporting.TextBox textBox21;
        private Telerik.Reporting.TextBox textBox17;
        private Telerik.Reporting.ObjectDataSource BusinessUnitobjectDataSource;
    }
}