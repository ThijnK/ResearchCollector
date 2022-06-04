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
            this.components = new System.ComponentModel.Container();
            this.runBtn = new System.Windows.Forms.Button();
            this.typeComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.logBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.inputLabel = new System.Windows.Forms.Label();
            this.inputLocation = new System.Windows.Forms.Label();
            this.dtdLabel = new System.Windows.Forms.Label();
            this.outputLabel = new System.Windows.Forms.Label();
            this.outputLocation = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.progressLabel = new System.Windows.Forms.Label();
            this.logCheckBox = new System.Windows.Forms.CheckBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.dToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.filterTab = new System.Windows.Forms.TabPage();
            this.importerTab = new System.Windows.Forms.TabPage();
            this.apiTab = new System.Windows.Forms.TabPage();
            this.runBtnImporter = new System.Windows.Forms.Button();
            this.logCheckBoxImporter = new System.Windows.Forms.CheckBox();
            this.progressLabelImpoter = new System.Windows.Forms.Label();
            this.progressBarImporter = new System.Windows.Forms.ProgressBar();
            this.logBoxImporter = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.contextMenuStrip2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.filterTab.SuspendLayout();
            this.importerTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // runBtn
            // 
            this.runBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.runBtn.Location = new System.Drawing.Point(8, 67);
            this.runBtn.Name = "runBtn";
            this.runBtn.Size = new System.Drawing.Size(174, 37);
            this.runBtn.TabIndex = 0;
            this.runBtn.Text = "Run";
            this.runBtn.UseVisualStyleBackColor = true;
            this.runBtn.Click += new System.EventHandler(this.RunBtn_Click);
            // 
            // typeComboBox
            // 
            this.typeComboBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.typeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.typeComboBox.FormattingEnabled = true;
            this.typeComboBox.Items.AddRange(new object[] {
            "DBLP",
            "PubMed",
            "PURE"});
            this.typeComboBox.Location = new System.Drawing.Point(8, 40);
            this.typeComboBox.Name = "typeComboBox";
            this.typeComboBox.Size = new System.Drawing.Size(174, 21);
            this.typeComboBox.TabIndex = 1;
            this.typeComboBox.SelectedIndexChanged += new System.EventHandler(this.typeComboBox_SelectedIndexChanged);
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
            // logBox
            // 
            this.logBox.Location = new System.Drawing.Point(195, 24);
            this.logBox.Multiline = true;
            this.logBox.Name = "logBox";
            this.logBox.ReadOnly = true;
            this.logBox.Size = new System.Drawing.Size(596, 413);
            this.logBox.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(192, 8);
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
            // inputLocation
            // 
            this.inputLocation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.inputLocation.Cursor = System.Windows.Forms.Cursors.Hand;
            this.inputLocation.Location = new System.Drawing.Point(8, 248);
            this.inputLocation.Name = "inputLocation";
            this.inputLocation.Size = new System.Drawing.Size(174, 20);
            this.inputLocation.TabIndex = 0;
            this.inputLocation.Text = "No file selected";
            this.inputLocation.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.inputLocation.Visible = false;
            this.inputLocation.Click += new System.EventHandler(this.inputLocation_Click);
            // 
            // dtdLabel
            // 
            this.dtdLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtdLabel.Location = new System.Drawing.Point(8, 270);
            this.dtdLabel.Name = "dtdLabel";
            this.dtdLabel.Size = new System.Drawing.Size(177, 56);
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
            // outputLocation
            // 
            this.outputLocation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.outputLocation.Cursor = System.Windows.Forms.Cursors.Hand;
            this.outputLocation.Location = new System.Drawing.Point(8, 196);
            this.outputLocation.Name = "outputLocation";
            this.outputLocation.Size = new System.Drawing.Size(174, 20);
            this.outputLocation.TabIndex = 0;
            this.outputLocation.Text = "No folder selected";
            this.outputLocation.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.outputLocation.Click += new System.EventHandler(this.outputLocation_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(8, 123);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(174, 23);
            this.progressBar.TabIndex = 14;
            // 
            // progressLabel
            // 
            this.progressLabel.Location = new System.Drawing.Point(8, 107);
            this.progressLabel.Name = "progressLabel";
            this.progressLabel.Size = new System.Drawing.Size(174, 13);
            this.progressLabel.TabIndex = 15;
            this.progressLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // logCheckBox
            // 
            this.logCheckBox.AutoSize = true;
            this.logCheckBox.Location = new System.Drawing.Point(8, 153);
            this.logCheckBox.Name = "logCheckBox";
            this.logCheckBox.Size = new System.Drawing.Size(81, 17);
            this.logCheckBox.TabIndex = 16;
            this.logCheckBox.Text = "Log actions";
            this.logCheckBox.UseVisualStyleBackColor = true;
            this.logCheckBox.CheckedChanged += new System.EventHandler(this.logCheckBox_CheckedChanged);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dToolStripMenuItem});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(82, 26);
            // 
            // dToolStripMenuItem
            // 
            this.dToolStripMenuItem.Name = "dToolStripMenuItem";
            this.dToolStripMenuItem.Size = new System.Drawing.Size(81, 22);
            this.dToolStripMenuItem.Text = "d";
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
            this.filterTab.Controls.Add(this.runBtn);
            this.filterTab.Controls.Add(this.typeComboBox);
            this.filterTab.Controls.Add(this.outputLocation);
            this.filterTab.Controls.Add(this.logBox);
            this.filterTab.Controls.Add(this.inputLocation);
            this.filterTab.Controls.Add(this.label2);
            this.filterTab.Controls.Add(this.logCheckBox);
            this.filterTab.Controls.Add(this.inputLabel);
            this.filterTab.Controls.Add(this.progressLabel);
            this.filterTab.Controls.Add(this.outputLabel);
            this.filterTab.Controls.Add(this.progressBar);
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
            this.importerTab.Controls.Add(this.logBoxImporter);
            this.importerTab.Controls.Add(this.label4);
            this.importerTab.Controls.Add(this.runBtnImporter);
            this.importerTab.Controls.Add(this.logCheckBoxImporter);
            this.importerTab.Controls.Add(this.progressLabelImpoter);
            this.importerTab.Controls.Add(this.progressBarImporter);
            this.importerTab.Location = new System.Drawing.Point(4, 22);
            this.importerTab.Name = "importerTab";
            this.importerTab.Padding = new System.Windows.Forms.Padding(3);
            this.importerTab.Size = new System.Drawing.Size(797, 443);
            this.importerTab.TabIndex = 1;
            this.importerTab.Text = "Importer";
            this.importerTab.UseVisualStyleBackColor = true;
            // 
            // apiTab
            // 
            this.apiTab.Location = new System.Drawing.Point(4, 22);
            this.apiTab.Name = "apiTab";
            this.apiTab.Size = new System.Drawing.Size(303, 192);
            this.apiTab.TabIndex = 2;
            this.apiTab.Text = "API";
            this.apiTab.UseVisualStyleBackColor = true;
            // 
            // runBtnImporter
            // 
            this.runBtnImporter.Cursor = System.Windows.Forms.Cursors.Hand;
            this.runBtnImporter.Location = new System.Drawing.Point(8, 24);
            this.runBtnImporter.Name = "runBtnImporter";
            this.runBtnImporter.Size = new System.Drawing.Size(174, 37);
            this.runBtnImporter.TabIndex = 17;
            this.runBtnImporter.Text = "Run";
            this.runBtnImporter.UseVisualStyleBackColor = true;
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
            // 
            // progressLabelImpoter
            // 
            this.progressLabelImpoter.Location = new System.Drawing.Point(8, 64);
            this.progressLabelImpoter.Name = "progressLabelImpoter";
            this.progressLabelImpoter.Size = new System.Drawing.Size(174, 13);
            this.progressLabelImpoter.TabIndex = 19;
            this.progressLabelImpoter.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // progressBarImporter
            // 
            this.progressBarImporter.Location = new System.Drawing.Point(8, 80);
            this.progressBarImporter.Name = "progressBarImporter";
            this.progressBarImporter.Size = new System.Drawing.Size(174, 23);
            this.progressBarImporter.TabIndex = 18;
            // 
            // logBoxImporter
            // 
            this.logBoxImporter.Location = new System.Drawing.Point(195, 24);
            this.logBoxImporter.Multiline = true;
            this.logBoxImporter.Name = "logBoxImporter";
            this.logBoxImporter.ReadOnly = true;
            this.logBoxImporter.Size = new System.Drawing.Size(596, 413);
            this.logBoxImporter.TabIndex = 21;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(192, 8);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(28, 13);
            this.label4.TabIndex = 22;
            this.label4.Text = "Log:";
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
            this.contextMenuStrip2.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.filterTab.ResumeLayout(false);
            this.filterTab.PerformLayout();
            this.importerTab.ResumeLayout(false);
            this.importerTab.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button runBtn;
        private System.Windows.Forms.ComboBox typeComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox logBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label inputLabel;
        private System.Windows.Forms.Label inputLocation;
        private System.Windows.Forms.Label dtdLabel;
        private System.Windows.Forms.Label outputLabel;
        private System.Windows.Forms.Label outputLocation;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label progressLabel;
        private System.Windows.Forms.CheckBox logCheckBox;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem dToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage filterTab;
        private System.Windows.Forms.TabPage importerTab;
        private System.Windows.Forms.TabPage apiTab;
        private System.Windows.Forms.TextBox logBoxImporter;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button runBtnImporter;
        private System.Windows.Forms.CheckBox logCheckBoxImporter;
        private System.Windows.Forms.Label progressLabelImpoter;
        private System.Windows.Forms.ProgressBar progressBarImporter;
    }
}

