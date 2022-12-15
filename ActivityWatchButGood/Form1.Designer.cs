
partial class Form1
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea11 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series31 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea12 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.CustomLabel customLabel16 = new System.Windows.Forms.DataVisualization.Charting.CustomLabel();
            System.Windows.Forms.DataVisualization.Charting.CustomLabel customLabel17 = new System.Windows.Forms.DataVisualization.Charting.CustomLabel();
            System.Windows.Forms.DataVisualization.Charting.CustomLabel customLabel18 = new System.Windows.Forms.DataVisualization.Charting.CustomLabel();
            System.Windows.Forms.DataVisualization.Charting.Series series32 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series33 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series34 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series35 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series36 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.tick = new System.Windows.Forms.Timer(this.components);
            this.activitiesListBox = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.categoryComboBox = new System.Windows.Forms.ComboBox();
            this.productivityComboBox = new System.Windows.Forms.ComboBox();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.timeLoggedLabel = new System.Windows.Forms.Label();
            this.categoryName0Label = new System.Windows.Forms.Label();
            this.categoryName1Label = new System.Windows.Forms.Label();
            this.categoryName2Label = new System.Windows.Forms.Label();
            this.categoryName3Label = new System.Windows.Forms.Label();
            this.categoryName4Label = new System.Windows.Forms.Label();
            this.timeViewComboBox = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.chooseDayLabel = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.histogramChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.timelineChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.categoryPct4Label = new System.Windows.Forms.Label();
            this.categoryPct3Label = new System.Windows.Forms.Label();
            this.categoryPct2Label = new System.Windows.Forms.Label();
            this.categoryPct1Label = new System.Windows.Forms.Label();
            this.categoryPct0Label = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.timeByHourLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.activityNameLabel = new System.Windows.Forms.Label();
            this.countLabel = new System.Windows.Forms.Label();
            this.filterLabel = new System.Windows.Forms.Label();
            this.categoryProgress0Panel = new System.Windows.Forms.Panel();
            this.categoryProgress1Panel = new System.Windows.Forms.Panel();
            this.categoryProgress2Panel = new System.Windows.Forms.Panel();
            this.categoryProgress3Panel = new System.Windows.Forms.Panel();
            this.categoryProgress4Panel = new System.Windows.Forms.Panel();
            this.activeAppLabel = new System.Windows.Forms.Label();
            this.statsPanel = new System.Windows.Forms.Panel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startsForNerdsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statsLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.histogramChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.timelineChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.statsPanel.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tick
            // 
            this.tick.Enabled = true;
            this.tick.Interval = 1000;
            this.tick.Tick += new System.EventHandler(this.tick_Tick);
            // 
            // activitiesListBox
            // 
            this.activitiesListBox.FormattingEnabled = true;
            this.activitiesListBox.Location = new System.Drawing.Point(12, 25);
            this.activitiesListBox.Name = "activitiesListBox";
            this.activitiesListBox.Size = new System.Drawing.Size(120, 381);
            this.activitiesListBox.TabIndex = 0;
            this.activitiesListBox.SelectedIndexChanged += new System.EventHandler(this.activitiesListBox_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(138, 106);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Category";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(138, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Productivity";
            // 
            // categoryComboBox
            // 
            this.categoryComboBox.FormattingEnabled = true;
            this.categoryComboBox.Location = new System.Drawing.Point(138, 120);
            this.categoryComboBox.Name = "categoryComboBox";
            this.categoryComboBox.Size = new System.Drawing.Size(141, 21);
            this.categoryComboBox.TabIndex = 2;
            this.categoryComboBox.SelectionChangeCommitted += new System.EventHandler(this.categoryComboBox_SelectedIndexChanged);
            // 
            // productivityComboBox
            // 
            this.productivityComboBox.FormattingEnabled = true;
            this.productivityComboBox.Location = new System.Drawing.Point(138, 81);
            this.productivityComboBox.Name = "productivityComboBox";
            this.productivityComboBox.Size = new System.Drawing.Size(141, 21);
            this.productivityComboBox.TabIndex = 1;
            this.productivityComboBox.SelectionChangeCommitted += new System.EventHandler(this.productivenessComboBox_SelectedIndexChanged);
            // 
            // nameTextBox
            // 
            this.nameTextBox.Location = new System.Drawing.Point(138, 43);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(141, 20);
            this.nameTextBox.TabIndex = 6;
            this.nameTextBox.TextChanged += new System.EventHandler(this.nameTextBox_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(138, 27);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Activity Name";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(292, 32);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "Time logged";
            // 
            // timeLoggedLabel
            // 
            this.timeLoggedLabel.AutoSize = true;
            this.timeLoggedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timeLoggedLabel.Location = new System.Drawing.Point(285, 5);
            this.timeLoggedLabel.Name = "timeLoggedLabel";
            this.timeLoggedLabel.Size = new System.Drawing.Size(103, 31);
            this.timeLoggedLabel.TabIndex = 9;
            this.timeLoggedLabel.Text = "1h 32m";
            // 
            // categoryName0Label
            // 
            this.categoryName0Label.AutoSize = true;
            this.categoryName0Label.Cursor = System.Windows.Forms.Cursors.Hand;
            this.categoryName0Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.categoryName0Label.Location = new System.Drawing.Point(589, 76);
            this.categoryName0Label.Name = "categoryName0Label";
            this.categoryName0Label.Size = new System.Drawing.Size(150, 17);
            this.categoryName0Label.TabIndex = 10;
            this.categoryName0Label.Text = "Software Development";
            this.categoryName0Label.Click += new System.EventHandler(this.categoryName0Label_Click);
            // 
            // categoryName1Label
            // 
            this.categoryName1Label.AutoSize = true;
            this.categoryName1Label.Cursor = System.Windows.Forms.Cursors.Hand;
            this.categoryName1Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.categoryName1Label.Location = new System.Drawing.Point(589, 116);
            this.categoryName1Label.Name = "categoryName1Label";
            this.categoryName1Label.Size = new System.Drawing.Size(96, 17);
            this.categoryName1Label.TabIndex = 11;
            this.categoryName1Label.Text = "Entertainment";
            this.categoryName1Label.Click += new System.EventHandler(this.categoryName1Label_Click);
            // 
            // categoryName2Label
            // 
            this.categoryName2Label.AutoSize = true;
            this.categoryName2Label.Cursor = System.Windows.Forms.Cursors.Hand;
            this.categoryName2Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.categoryName2Label.Location = new System.Drawing.Point(589, 154);
            this.categoryName2Label.Name = "categoryName2Label";
            this.categoryName2Label.Size = new System.Drawing.Size(207, 17);
            this.categoryName2Label.TabIndex = 12;
            this.categoryName2Label.Text = "Communication And Scheduling";
            this.categoryName2Label.Click += new System.EventHandler(this.categoryName2Label_Click);
            // 
            // categoryName3Label
            // 
            this.categoryName3Label.AutoSize = true;
            this.categoryName3Label.Cursor = System.Windows.Forms.Cursors.Hand;
            this.categoryName3Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.categoryName3Label.Location = new System.Drawing.Point(589, 197);
            this.categoryName3Label.Name = "categoryName3Label";
            this.categoryName3Label.Size = new System.Drawing.Size(53, 17);
            this.categoryName3Label.TabIndex = 13;
            this.categoryName3Label.Text = "Utilities";
            this.categoryName3Label.Click += new System.EventHandler(this.categoryName3Label_Click);
            // 
            // categoryName4Label
            // 
            this.categoryName4Label.AutoSize = true;
            this.categoryName4Label.Cursor = System.Windows.Forms.Cursors.Hand;
            this.categoryName4Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.categoryName4Label.Location = new System.Drawing.Point(589, 239);
            this.categoryName4Label.Name = "categoryName4Label";
            this.categoryName4Label.Size = new System.Drawing.Size(120, 17);
            this.categoryName4Label.TabIndex = 14;
            this.categoryName4Label.Text = "Social Networking";
            this.categoryName4Label.Click += new System.EventHandler(this.categoryName4Label_Click);
            // 
            // timeViewComboBox
            // 
            this.timeViewComboBox.FormattingEnabled = true;
            this.timeViewComboBox.Location = new System.Drawing.Point(447, 13);
            this.timeViewComboBox.Name = "timeViewComboBox";
            this.timeViewComboBox.Size = new System.Drawing.Size(59, 21);
            this.timeViewComboBox.TabIndex = 15;
            this.timeViewComboBox.Text = "Weekly";
            this.timeViewComboBox.SelectedIndexChanged += new System.EventHandler(this.timeViewComboBox_SelectedIndexChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(417, 16);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(30, 13);
            this.label13.TabIndex = 16;
            this.label13.Text = "View";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(506, 16);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(41, 13);
            this.label14.TabIndex = 17;
            this.label14.Text = "Activity";
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(669, 14);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(119, 20);
            this.dateTimePicker1.TabIndex = 18;
            this.dateTimePicker1.Value = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this.dateTimePicker1.ValueChanged += new System.EventHandler(this.dateTimePicker1_ValueChanged);
            // 
            // chooseDayLabel
            // 
            this.chooseDayLabel.Location = new System.Drawing.Point(569, 17);
            this.chooseDayLabel.Name = "chooseDayLabel";
            this.chooseDayLabel.Size = new System.Drawing.Size(98, 13);
            this.chooseDayLabel.TabIndex = 19;
            this.chooseDayLabel.Text = "Choose Day";
            this.chooseDayLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(9, 428);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(92, 13);
            this.label16.TabIndex = 21;
            this.label16.Text = "Version 14 (indev)";
            // 
            // histogramChart
            // 
            this.histogramChart.BackColor = System.Drawing.SystemColors.Control;
            chartArea11.AxisX.Interval = 1D;
            chartArea11.AxisX.IsLabelAutoFit = false;
            chartArea11.AxisX.LabelAutoFitStyle = System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.WordWrap;
            chartArea11.AxisX.LabelStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            chartArea11.AxisX.LabelStyle.IsStaggered = true;
            chartArea11.AxisX.LabelStyle.TruncatedLabels = true;
            chartArea11.AxisY.LabelStyle.Format = "H\\h m\\m";
            chartArea11.Name = "ChartArea1";
            this.histogramChart.ChartAreas.Add(chartArea11);
            this.histogramChart.Location = new System.Drawing.Point(112, 270);
            this.histogramChart.Name = "histogramChart";
            this.histogramChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.None;
            this.histogramChart.PaletteCustomColors = new System.Drawing.Color[] {
        System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(104)))), ((int)(((byte)(90))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(24)))), ((int)(((byte)(0))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(177)))), ((int)(((byte)(193)))), ((int)(((byte)(191))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(128)))), ((int)(((byte)(224))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(85)))), ((int)(((byte)(196)))))};
            series31.ChartArea = "ChartArea1";
            series31.Name = "Entries";
            series31.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.String;
            series31.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Time;
            this.histogramChart.Series.Add(series31);
            this.histogramChart.Size = new System.Drawing.Size(688, 179);
            this.histogramChart.TabIndex = 23;
            this.histogramChart.Text = "chart1";
            this.histogramChart.MouseClick += new System.Windows.Forms.MouseEventHandler(this.histogramChart_MouseClick);
            this.histogramChart.MouseMove += new System.Windows.Forms.MouseEventHandler(this.histogramChart_MouseMove);
            // 
            // timelineChart
            // 
            this.timelineChart.BackColor = System.Drawing.SystemColors.Control;
            chartArea12.AlignmentOrientation = ((System.Windows.Forms.DataVisualization.Charting.AreaAlignmentOrientations)((System.Windows.Forms.DataVisualization.Charting.AreaAlignmentOrientations.Vertical | System.Windows.Forms.DataVisualization.Charting.AreaAlignmentOrientations.Horizontal)));
            chartArea12.AxisX.Interval = 1D;
            customLabel16.Text = "a";
            customLabel17.Text = "b";
            customLabel18.Text = "c";
            chartArea12.AxisX2.CustomLabels.Add(customLabel16);
            chartArea12.AxisX2.CustomLabels.Add(customLabel17);
            chartArea12.AxisX2.CustomLabels.Add(customLabel18);
            chartArea12.AxisY.LabelStyle.Format = "{0:#,0;#,0}m";
            chartArea12.Name = "ChartArea1";
            this.timelineChart.ChartAreas.Add(chartArea12);
            this.timelineChart.Location = new System.Drawing.Point(262, 105);
            this.timelineChart.Name = "timelineChart";
            this.timelineChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.None;
            this.timelineChart.PaletteCustomColors = new System.Drawing.Color[] {
        System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(104)))), ((int)(((byte)(90))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(24)))), ((int)(((byte)(0))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(177)))), ((int)(((byte)(193)))), ((int)(((byte)(191))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(128)))), ((int)(((byte)(224))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(85)))), ((int)(((byte)(196)))))};
            series32.ChartArea = "ChartArea1";
            series32.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StackedColumn;
            series32.Name = "Distracting";
            series33.ChartArea = "ChartArea1";
            series33.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StackedColumn;
            series33.Name = "VeryDistracting";
            series34.ChartArea = "ChartArea1";
            series34.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StackedColumn;
            series34.Name = "Neutral";
            series35.ChartArea = "ChartArea1";
            series35.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StackedColumn;
            series35.Name = "Productive";
            series36.ChartArea = "ChartArea1";
            series36.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StackedColumn;
            series36.Name = "VeryProductive";
            this.timelineChart.Series.Add(series32);
            this.timelineChart.Series.Add(series33);
            this.timelineChart.Series.Add(series34);
            this.timelineChart.Series.Add(series35);
            this.timelineChart.Series.Add(series36);
            this.timelineChart.Size = new System.Drawing.Size(296, 178);
            this.timelineChart.TabIndex = 24;
            this.timelineChart.Text = "chart2";
            this.timelineChart.MouseClick += new System.Windows.Forms.MouseEventHandler(this.timelineChart_MouseClick);
            this.timelineChart.MouseMove += new System.Windows.Forms.MouseEventHandler(this.timelineChart_MouseMove);
            // 
            // categoryPct4Label
            // 
            this.categoryPct4Label.AutoSize = true;
            this.categoryPct4Label.Cursor = System.Windows.Forms.Cursors.Hand;
            this.categoryPct4Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.categoryPct4Label.Location = new System.Drawing.Point(548, 239);
            this.categoryPct4Label.Name = "categoryPct4Label";
            this.categoryPct4Label.Size = new System.Drawing.Size(44, 17);
            this.categoryPct4Label.TabIndex = 29;
            this.categoryPct4Label.Text = "100%";
            this.categoryPct4Label.Click += new System.EventHandler(this.categoryName4Label_Click);
            // 
            // categoryPct3Label
            // 
            this.categoryPct3Label.AutoSize = true;
            this.categoryPct3Label.Cursor = System.Windows.Forms.Cursors.Hand;
            this.categoryPct3Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.categoryPct3Label.Location = new System.Drawing.Point(548, 197);
            this.categoryPct3Label.Name = "categoryPct3Label";
            this.categoryPct3Label.Size = new System.Drawing.Size(44, 17);
            this.categoryPct3Label.TabIndex = 28;
            this.categoryPct3Label.Text = "100%";
            this.categoryPct3Label.Click += new System.EventHandler(this.categoryName3Label_Click);
            // 
            // categoryPct2Label
            // 
            this.categoryPct2Label.AutoSize = true;
            this.categoryPct2Label.Cursor = System.Windows.Forms.Cursors.Hand;
            this.categoryPct2Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.categoryPct2Label.Location = new System.Drawing.Point(548, 154);
            this.categoryPct2Label.Name = "categoryPct2Label";
            this.categoryPct2Label.Size = new System.Drawing.Size(44, 17);
            this.categoryPct2Label.TabIndex = 27;
            this.categoryPct2Label.Text = "100%";
            this.categoryPct2Label.Click += new System.EventHandler(this.categoryName2Label_Click);
            // 
            // categoryPct1Label
            // 
            this.categoryPct1Label.AutoSize = true;
            this.categoryPct1Label.Cursor = System.Windows.Forms.Cursors.Hand;
            this.categoryPct1Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.categoryPct1Label.Location = new System.Drawing.Point(548, 116);
            this.categoryPct1Label.Name = "categoryPct1Label";
            this.categoryPct1Label.Size = new System.Drawing.Size(44, 17);
            this.categoryPct1Label.TabIndex = 26;
            this.categoryPct1Label.Text = "100%";
            this.categoryPct1Label.Click += new System.EventHandler(this.categoryName1Label_Click);
            // 
            // categoryPct0Label
            // 
            this.categoryPct0Label.AutoSize = true;
            this.categoryPct0Label.Cursor = System.Windows.Forms.Cursors.Hand;
            this.categoryPct0Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.categoryPct0Label.Location = new System.Drawing.Point(548, 76);
            this.categoryPct0Label.Name = "categoryPct0Label";
            this.categoryPct0Label.Size = new System.Drawing.Size(44, 17);
            this.categoryPct0Label.TabIndex = 25;
            this.categoryPct0Label.Text = "100%";
            this.categoryPct0Label.Click += new System.EventHandler(this.categoryName0Label_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.ContextMenuStrip = this.contextMenuStrip1;
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(189, 181);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(67, 79);
            this.pictureBox1.TabIndex = 30;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // timeByHourLabel
            // 
            this.timeByHourLabel.AutoSize = true;
            this.timeByHourLabel.Location = new System.Drawing.Point(306, 91);
            this.timeByHourLabel.Name = "timeByHourLabel";
            this.timeByHourLabel.Size = new System.Drawing.Size(64, 13);
            this.timeByHourLabel.TabIndex = 32;
            this.timeByHourLabel.Text = "time by hour";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(199, 260);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 13);
            this.label4.TabIndex = 33;
            this.label4.Text = "Time by activity";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(548, 55);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(88, 13);
            this.label15.TabIndex = 34;
            this.label15.Text = "Time by category";
            // 
            // activityNameLabel
            // 
            this.activityNameLabel.AutoSize = true;
            this.activityNameLabel.Location = new System.Drawing.Point(138, 144);
            this.activityNameLabel.Name = "activityNameLabel";
            this.activityNameLabel.Size = new System.Drawing.Size(49, 13);
            this.activityNameLabel.TabIndex = 35;
            this.activityNameLabel.Text = "Category";
            // 
            // countLabel
            // 
            this.countLabel.AutoSize = true;
            this.countLabel.Location = new System.Drawing.Point(138, 157);
            this.countLabel.Name = "countLabel";
            this.countLabel.Size = new System.Drawing.Size(49, 13);
            this.countLabel.TabIndex = 36;
            this.countLabel.Text = "Category";
            // 
            // filterLabel
            // 
            this.filterLabel.AutoSize = true;
            this.filterLabel.Location = new System.Drawing.Point(12, 9);
            this.filterLabel.Name = "filterLabel";
            this.filterLabel.Size = new System.Drawing.Size(32, 13);
            this.filterLabel.TabIndex = 37;
            this.filterLabel.Text = "Filter:";
            // 
            // categoryProgress0Panel
            // 
            this.categoryProgress0Panel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(85)))), ((int)(((byte)(196)))));
            this.categoryProgress0Panel.Location = new System.Drawing.Point(551, 96);
            this.categoryProgress0Panel.Name = "categoryProgress0Panel";
            this.categoryProgress0Panel.Size = new System.Drawing.Size(237, 5);
            this.categoryProgress0Panel.TabIndex = 38;
            // 
            // categoryProgress1Panel
            // 
            this.categoryProgress1Panel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(85)))), ((int)(((byte)(196)))));
            this.categoryProgress1Panel.Location = new System.Drawing.Point(551, 136);
            this.categoryProgress1Panel.Name = "categoryProgress1Panel";
            this.categoryProgress1Panel.Size = new System.Drawing.Size(237, 5);
            this.categoryProgress1Panel.TabIndex = 39;
            // 
            // categoryProgress2Panel
            // 
            this.categoryProgress2Panel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(85)))), ((int)(((byte)(196)))));
            this.categoryProgress2Panel.Location = new System.Drawing.Point(551, 174);
            this.categoryProgress2Panel.Name = "categoryProgress2Panel";
            this.categoryProgress2Panel.Size = new System.Drawing.Size(237, 5);
            this.categoryProgress2Panel.TabIndex = 40;
            // 
            // categoryProgress3Panel
            // 
            this.categoryProgress3Panel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(85)))), ((int)(((byte)(196)))));
            this.categoryProgress3Panel.Location = new System.Drawing.Point(551, 217);
            this.categoryProgress3Panel.Name = "categoryProgress3Panel";
            this.categoryProgress3Panel.Size = new System.Drawing.Size(237, 5);
            this.categoryProgress3Panel.TabIndex = 41;
            // 
            // categoryProgress4Panel
            // 
            this.categoryProgress4Panel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(85)))), ((int)(((byte)(196)))));
            this.categoryProgress4Panel.Location = new System.Drawing.Point(551, 259);
            this.categoryProgress4Panel.Name = "categoryProgress4Panel";
            this.categoryProgress4Panel.Size = new System.Drawing.Size(237, 5);
            this.categoryProgress4Panel.TabIndex = 42;
            // 
            // activeAppLabel
            // 
            this.activeAppLabel.AutoSize = true;
            this.activeAppLabel.Location = new System.Drawing.Point(9, 409);
            this.activeAppLabel.Name = "activeAppLabel";
            this.activeAppLabel.Size = new System.Drawing.Size(57, 13);
            this.activeAppLabel.TabIndex = 43;
            this.activeAppLabel.Text = "active app";
            // 
            // statsPanel
            // 
            this.statsPanel.AutoScroll = true;
            this.statsPanel.Controls.Add(this.statsLabel);
            this.statsPanel.Location = new System.Drawing.Point(342, 48);
            this.statsPanel.Name = "statsPanel";
            this.statsPanel.Size = new System.Drawing.Size(397, 374);
            this.statsPanel.TabIndex = 44;
            this.statsPanel.Visible = false;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshToolStripMenuItem,
            this.startsForNerdsToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(153, 48);
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.refreshToolStripMenuItem.Text = "refresh";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // startsForNerdsToolStripMenuItem
            // 
            this.startsForNerdsToolStripMenuItem.Name = "startsForNerdsToolStripMenuItem";
            this.startsForNerdsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.startsForNerdsToolStripMenuItem.Text = "starts for nerds";
            this.startsForNerdsToolStripMenuItem.Click += new System.EventHandler(this.startsForNerdsToolStripMenuItem_Click);
            // 
            // statsLabel
            // 
            this.statsLabel.AutoSize = true;
            this.statsLabel.Location = new System.Drawing.Point(17, 19);
            this.statsLabel.Name = "statsLabel";
            this.statsLabel.Size = new System.Drawing.Size(29, 13);
            this.statsLabel.TabIndex = 1;
            this.statsLabel.Text = "stats";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.statsPanel);
            this.Controls.Add(this.activeAppLabel);
            this.Controls.Add(this.categoryProgress4Panel);
            this.Controls.Add(this.categoryProgress3Panel);
            this.Controls.Add(this.categoryProgress2Panel);
            this.Controls.Add(this.categoryProgress1Panel);
            this.Controls.Add(this.categoryProgress0Panel);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.filterLabel);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.countLabel);
            this.Controls.Add(this.activityNameLabel);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.timeByHourLabel);
            this.Controls.Add(this.categoryPct4Label);
            this.Controls.Add(this.categoryPct3Label);
            this.Controls.Add(this.activitiesListBox);
            this.Controls.Add(this.categoryPct2Label);
            this.Controls.Add(this.productivityComboBox);
            this.Controls.Add(this.categoryPct1Label);
            this.Controls.Add(this.categoryComboBox);
            this.Controls.Add(this.categoryPct0Label);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.histogramChart);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.chooseDayLabel);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.timeLoggedLabel);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.categoryName0Label);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.categoryName1Label);
            this.Controls.Add(this.timeViewComboBox);
            this.Controls.Add(this.categoryName2Label);
            this.Controls.Add(this.categoryName4Label);
            this.Controls.Add(this.categoryName3Label);
            this.Controls.Add(this.timelineChart);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.Text = "Activity Time";
            this.Activated += new System.EventHandler(this.Form1_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.histogramChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.timelineChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.statsPanel.ResumeLayout(false);
            this.statsPanel.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Timer tick;
    private System.Windows.Forms.ListBox activitiesListBox;
    private System.Windows.Forms.ComboBox productivityComboBox;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.ComboBox categoryComboBox;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.TextBox nameTextBox;
    private System.Windows.Forms.Label categoryName0Label;
    private System.Windows.Forms.Label timeLoggedLabel;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Label categoryName1Label;
    private System.Windows.Forms.Label categoryName2Label;
    private System.Windows.Forms.Label categoryName4Label;
    private System.Windows.Forms.Label categoryName3Label;
    private System.Windows.Forms.ComboBox timeViewComboBox;
    private System.Windows.Forms.Label chooseDayLabel;
    private System.Windows.Forms.DateTimePicker dateTimePicker1;
    private System.Windows.Forms.Label label14;
    private System.Windows.Forms.Label label13;
    private System.Windows.Forms.Label categoryPct4Label;
    private System.Windows.Forms.Label categoryPct3Label;
    private System.Windows.Forms.Label categoryPct2Label;
    private System.Windows.Forms.Label categoryPct1Label;
    private System.Windows.Forms.Label categoryPct0Label;
    private System.Windows.Forms.DataVisualization.Charting.Chart timelineChart;
    private System.Windows.Forms.DataVisualization.Charting.Chart histogramChart;
    private System.Windows.Forms.Label label16;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.Label timeByHourLabel;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label15;
    private System.Windows.Forms.Label activityNameLabel;
    private System.Windows.Forms.Label countLabel;
    private System.Windows.Forms.Label filterLabel;
    private System.Windows.Forms.Panel categoryProgress0Panel;
    private System.Windows.Forms.Panel categoryProgress1Panel;
    private System.Windows.Forms.Panel categoryProgress2Panel;
    private System.Windows.Forms.Panel categoryProgress3Panel;
    private System.Windows.Forms.Panel categoryProgress4Panel;
    private System.Windows.Forms.Label activeAppLabel;
    private System.Windows.Forms.Panel statsPanel;
    private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
    private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem startsForNerdsToolStripMenuItem;
    private System.Windows.Forms.Label statsLabel;
}
