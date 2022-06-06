namespace ResearchCollector
{
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
            this.runBtnFilter = new System.Windows.Forms.Button();
            this.typeComboBoxFilter = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.logBoxFilter = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.inputLabel = new System.Windows.Forms.Label();
            this.inputLocationFilter = new System.Windows.Forms.Label();
            this.dtdLabel = new System.Windows.Forms.Label();
            this.outputLabel = new System.Windows.Forms.Label();
            this.outputLocationFilter = new System.Windows.Forms.Label();
            this.progressBarFilter = new System.Windows.Forms.ProgressBar();
            this.progressLabelFilter = new System.Windows.Forms.Label();
            this.logCheckBoxFilter = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.filterTab = new System.Windows.Forms.TabPage();
            this.importerTab = new System.Windows.Forms.TabPage();
            this.logBoxImporter = new System.Windows.Forms.TextBox();
            this.inputLocationImporter = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.runBtnImporter = new System.Windows.Forms.Button();
            this.logCheckBoxImporter = new System.Windows.Forms.CheckBox();
            this.progressLabelImporter = new System.Windows.Forms.Label();
            this.progressBarImporter = new System.Windows.Forms.ProgressBar();
            this.apiTab = new System.Windows.Forms.TabPage();
            this.DownLoad_Articles = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.filterTab.SuspendLayout();
            this.importerTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // runBtnFilter
            // 
            this.runBtnFilter.Cursor = System.Windows.Forms.Cursors.Hand;
            this.runBtnFilter.Location = new System.Drawing.Point(8, 67);
            this.runBtnFilter.Name = "runBtnFilter";
            this.runBtnFilter.Size = new System.Drawing.Size(224, 37);
            this.runBtnFilter.TabIndex = 0;
            this.runBtnFilter.Text = "Run";
            this.runBtnFilter.UseVisualStyleBackColor = true;
            this.runBtnFilter.Click += new System.EventHandler(this.FilterRunBtn_Click);
            // 
            // typeComboBoxFilter
            // 
            this.typeComboBoxFilter.Cursor = System.Windows.Forms.Cursors.Hand;
            this.typeComboBoxFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.typeComboBoxFilter.FormattingEnabled = true;
            this.typeComboBoxFilter.Items.AddRange(new object[] {
            "DBLP",
            "PubMed",
            "PURE"});
            this.typeComboBoxFilter.Location = new System.Drawing.Point(8, 40);
            this.typeComboBoxFilter.Name = "typeComboBoxFilter";
            this.typeComboBoxFilter.Size = new System.Drawing.Size(224, 21);
            this.typeComboBoxFilter.TabIndex = 1;
            this.typeComboBoxFilter.SelectedIndexChanged += new System.EventHandler(this.FilterComboBox_IndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Type of data set:";
            // 
            // logBoxFilter
            // 
            this.logBoxFilter.Location = new System.Drawing.Point(238, 24);
            this.logBoxFilter.Multiline = true;
            this.logBoxFilter.Name = "logBoxFilter";
            this.logBoxFilter.ReadOnly = true;
            this.logBoxFilter.Size = new System.Drawing.Size(553, 413);
            this.logBoxFilter.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(235, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Log:";
            // 
            // inputLabel
            // 
            this.inputLabel.AutoSize = true;
            this.inputLabel.Location = new System.Drawing.Point(8, 231);
            this.inputLabel.Name = "inputLabel";
            this.inputLabel.Size = new System.Drawing.Size(54, 13);
            this.inputLabel.TabIndex = 7;
            this.inputLabel.Text = "Input file*:";
            this.inputLabel.Visible = false;
            // 
            // inputLocationFilter
            // 
            this.inputLocationFilter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.inputLocationFilter.Cursor = System.Windows.Forms.Cursors.Hand;
            this.inputLocationFilter.Location = new System.Drawing.Point(8, 248);
            this.inputLocationFilter.Name = "inputLocationFilter";
            this.inputLocationFilter.Size = new System.Drawing.Size(224, 20);
            this.inputLocationFilter.TabIndex = 0;
            this.inputLocationFilter.Text = "No file selected";
            this.inputLocationFilter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.inputLocationFilter.Visible = false;
            this.inputLocationFilter.Click += new System.EventHandler(this.FilterInputLocation_Click);
            // 
            // dtdLabel
            // 
            this.dtdLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtdLabel.Location = new System.Drawing.Point(8, 270);
            this.dtdLabel.Name = "dtdLabel";
            this.dtdLabel.Size = new System.Drawing.Size(224, 56);
            this.dtdLabel.TabIndex = 13;
            this.dtdLabel.Text = "* DTD file is assumed to be located in the same directory as the input file, as w" +
    "ell as to have the same name as the input file.";
            this.dtdLabel.Visible = false;
            // 
            // outputLabel
            // 
            this.outputLabel.AutoSize = true;
            this.outputLabel.Location = new System.Drawing.Point(8, 179);
            this.outputLabel.Name = "outputLabel";
            this.outputLabel.Size = new System.Drawing.Size(88, 13);
            this.outputLabel.TabIndex = 9;
            this.outputLabel.Text = "Saving output to:";
            // 
            // outputLocationFilter
            // 
            this.outputLocationFilter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.outputLocationFilter.Cursor = System.Windows.Forms.Cursors.Hand;
            this.outputLocationFilter.Location = new System.Drawing.Point(8, 196);
            this.outputLocationFilter.Name = "outputLocationFilter";
            this.outputLocationFilter.Size = new System.Drawing.Size(224, 20);
            this.outputLocationFilter.TabIndex = 0;
            this.outputLocationFilter.Text = "No folder selected";
            this.outputLocationFilter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.outputLocationFilter.Click += new System.EventHandler(this.FilterOutputLocation_Click);
            // 
            // progressBarFilter
            // 
            this.progressBarFilter.Location = new System.Drawing.Point(8, 123);
            this.progressBarFilter.Name = "progressBarFilter";
            this.progressBarFilter.Size = new System.Drawing.Size(224, 23);
            this.progressBarFilter.TabIndex = 14;
            // 
            // progressLabelFilter
            // 
            this.progressLabelFilter.Location = new System.Drawing.Point(8, 107);
            this.progressLabelFilter.Name = "progressLabelFilter";
            this.progressLabelFilter.Size = new System.Drawing.Size(224, 13);
            this.progressLabelFilter.TabIndex = 15;
            this.progressLabelFilter.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // logCheckBoxFilter
            // 
            this.logCheckBoxFilter.AutoSize = true;
            this.logCheckBoxFilter.Location = new System.Drawing.Point(8, 153);
            this.logCheckBoxFilter.Name = "logCheckBoxFilter";
            this.logCheckBoxFilter.Size = new System.Drawing.Size(81, 17);
            this.logCheckBoxFilter.TabIndex = 16;
            this.logCheckBoxFilter.Text = "Log actions";
            this.logCheckBoxFilter.UseVisualStyleBackColor = true;
            this.logCheckBoxFilter.CheckedChanged += new System.EventHandler(this.LogCheckBox_Changed);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.filterTab);
            this.tabControl1.Controls.Add(this.importerTab);
            this.tabControl1.Controls.Add(this.apiTab);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(805, 469);
            this.tabControl1.TabIndex = 19;
            // 
            // filterTab
            // 
            this.filterTab.Controls.Add(this.label1);
            this.filterTab.Controls.Add(this.runBtnFilter);
            this.filterTab.Controls.Add(this.typeComboBoxFilter);
            this.filterTab.Controls.Add(this.outputLocationFilter);
            this.filterTab.Controls.Add(this.logBoxFilter);
            this.filterTab.Controls.Add(this.inputLocationFilter);
            this.filterTab.Controls.Add(this.label2);
            this.filterTab.Controls.Add(this.logCheckBoxFilter);
            this.filterTab.Controls.Add(this.inputLabel);
            this.filterTab.Controls.Add(this.progressLabelFilter);
            this.filterTab.Controls.Add(this.outputLabel);
            this.filterTab.Controls.Add(this.progressBarFilter);
            this.filterTab.Controls.Add(this.dtdLabel);
            this.filterTab.Location = new System.Drawing.Point(4, 22);
            this.filterTab.Name = "filterTab";
            this.filterTab.Padding = new System.Windows.Forms.Padding(3);
            this.filterTab.Size = new System.Drawing.Size(797, 443);
            this.filterTab.TabIndex = 0;
            this.filterTab.Text = "Filter";
            this.filterTab.UseVisualStyleBackColor = true;
            // 
            // importerTab
            // 
            this.importerTab.Controls.Add(this.DownLoad_Articles);
            this.importerTab.Controls.Add(this.logBoxImporter);
            this.importerTab.Controls.Add(this.inputLocationImporter);
            this.importerTab.Controls.Add(this.label5);
            this.importerTab.Controls.Add(this.label4);
            this.importerTab.Controls.Add(this.runBtnImporter);
            this.importerTab.Controls.Add(this.logCheckBoxImporter);
            this.importerTab.Controls.Add(this.progressLabelImporter);
            this.importerTab.Controls.Add(this.progressBarImporter);
            this.importerTab.Location = new System.Drawing.Point(4, 22);
            this.importerTab.Name = "importerTab";
            this.importerTab.Padding = new System.Windows.Forms.Padding(3);
            this.importerTab.Size = new System.Drawing.Size(797, 443);
            this.importerTab.TabIndex = 1;
            this.importerTab.Text = "Importer";
            this.importerTab.UseVisualStyleBackColor = true;
            // 
            // logBoxImporter
            // 
            this.logBoxImporter.Location = new System.Drawing.Point(238, 24);
            this.logBoxImporter.Multiline = true;
            this.logBoxImporter.Name = "logBoxImporter";
            this.logBoxImporter.ReadOnly = true;
            this.logBoxImporter.Size = new System.Drawing.Size(553, 413);
            this.logBoxImporter.TabIndex = 25;
            // 
            // inputLocationImporter
            // 
            this.inputLocationImporter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.inputLocationImporter.Cursor = System.Windows.Forms.Cursors.Hand;
            this.inputLocationImporter.Location = new System.Drawing.Point(8, 156);
            this.inputLocationImporter.Name = "inputLocationImporter";
            this.inputLocationImporter.Size = new System.Drawing.Size(224, 20);
            this.inputLocationImporter.TabIndex = 23;
            this.inputLocationImporter.Text = "No file selected";
            this.inputLocationImporter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.inputLocationImporter.Click += new System.EventHandler(this.ImporterInputLocation_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 139);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(50, 13);
            this.label5.TabIndex = 24;
            this.label5.Text = "Input file:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(235, 8);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(28, 13);
            this.label4.TabIndex = 22;
            this.label4.Text = "Log:";
            // 
            // runBtnImporter
            // 
            this.runBtnImporter.Cursor = System.Windows.Forms.Cursors.Hand;
            this.runBtnImporter.Location = new System.Drawing.Point(8, 24);
            this.runBtnImporter.Name = "runBtnImporter";
            this.runBtnImporter.Size = new System.Drawing.Size(224, 37);
            this.runBtnImporter.TabIndex = 17;
            this.runBtnImporter.Text = "Run";
            this.runBtnImporter.UseVisualStyleBackColor = true;
            this.runBtnImporter.Click += new System.EventHandler(this.ImporterRunBtn_Click);
            // 
            // logCheckBoxImporter
            // 
            this.logCheckBoxImporter.AutoSize = true;
            this.logCheckBoxImporter.Location = new System.Drawing.Point(8, 110);
            this.logCheckBoxImporter.Name = "logCheckBoxImporter";
            this.logCheckBoxImporter.Size = new System.Drawing.Size(81, 17);
            this.logCheckBoxImporter.TabIndex = 20;
            this.logCheckBoxImporter.Text = "Log actions";
            this.logCheckBoxImporter.UseVisualStyleBackColor = true;
            this.logCheckBoxImporter.CheckedChanged += new System.EventHandler(this.LogCheckBox_Changed);
            // 
            // progressLabelImporter
            // 
            this.progressLabelImporter.Location = new System.Drawing.Point(8, 64);
            this.progressLabelImporter.Name = "progressLabelImporter";
            this.progressLabelImporter.Size = new System.Drawing.Size(224, 13);
            this.progressLabelImporter.TabIndex = 19;
            this.progressLabelImporter.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // progressBarImporter
            // 
            this.progressBarImporter.Location = new System.Drawing.Point(8, 80);
            this.progressBarImporter.Name = "progressBarImporter";
            this.progressBarImporter.Size = new System.Drawing.Size(224, 23);
            this.progressBarImporter.TabIndex = 18;
            // 
            // apiTab
            // 
            this.apiTab.Location = new System.Drawing.Point(4, 22);
            this.apiTab.Name = "apiTab";
            this.apiTab.Size = new System.Drawing.Size(797, 443);
            this.apiTab.TabIndex = 2;
            this.apiTab.Text = "API";
            this.apiTab.UseVisualStyleBackColor = true;
            // 
            // DownLoad_Articles
            // 
            this.DownLoad_Articles.Location = new System.Drawing.Point(8, 190);
            this.DownLoad_Articles.Name = "DownLoad_Articles";
            this.DownLoad_Articles.Size = new System.Drawing.Size(224, 23);
            this.DownLoad_Articles.TabIndex = 26;
            this.DownLoad_Articles.Text = "Download Articles";
            this.DownLoad_Articles.UseVisualStyleBackColor = true;
            this.DownLoad_Articles.Click += new System.EventHandler(this.DownLoad_Articles_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(806, 469);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.ShowIcon = false;
            this.Text = "Research Collector";
            this.tabControl1.ResumeLayout(false);
            this.filterTab.ResumeLayout(false);
            this.filterTab.PerformLayout();
            this.importerTab.ResumeLayout(false);
            this.importerTab.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button runBtnFilter;
        private System.Windows.Forms.ComboBox typeComboBoxFilter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox logBoxFilter;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label inputLabel;
        private System.Windows.Forms.Label inputLocationFilter;
        private System.Windows.Forms.Label dtdLabel;
        private System.Windows.Forms.Label outputLabel;
        private System.Windows.Forms.Label outputLocationFilter;
        private System.Windows.Forms.ProgressBar progressBarFilter;
        private System.Windows.Forms.Label progressLabelFilter;
        private System.Windows.Forms.CheckBox logCheckBoxFilter;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage filterTab;
        private System.Windows.Forms.TabPage importerTab;
        private System.Windows.Forms.TabPage apiTab;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button runBtnImporter;
        private System.Windows.Forms.CheckBox logCheckBoxImporter;
        private System.Windows.Forms.Label progressLabelImporter;
        private System.Windows.Forms.ProgressBar progressBarImporter;
        private System.Windows.Forms.Label inputLocationImporter;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox logBoxImporter;
        private System.Windows.Forms.Button DownLoad_Articles;
    }
}

