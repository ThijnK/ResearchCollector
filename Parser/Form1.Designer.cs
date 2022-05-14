﻿namespace Parser
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
            this.SuspendLayout();
            // 
            // runBtn
            // 
            this.runBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.runBtn.Location = new System.Drawing.Point(9, 66);
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
            "PubMed"});
            this.typeComboBox.Location = new System.Drawing.Point(9, 39);
            this.typeComboBox.Name = "typeComboBox";
            this.typeComboBox.Size = new System.Drawing.Size(174, 21);
            this.typeComboBox.TabIndex = 1;
            this.typeComboBox.SelectedIndexChanged += new System.EventHandler(this.typeComboBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Type of data set:";
            // 
            // logBox
            // 
            this.logBox.Location = new System.Drawing.Point(192, 25);
            this.logBox.Multiline = true;
            this.logBox.Name = "logBox";
            this.logBox.ReadOnly = true;
            this.logBox.Size = new System.Drawing.Size(596, 413);
            this.logBox.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(189, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Log:";
            // 
            // inputLabel
            // 
            this.inputLabel.AutoSize = true;
            this.inputLabel.Location = new System.Drawing.Point(9, 184);
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
            this.inputLocation.Location = new System.Drawing.Point(9, 201);
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
            this.dtdLabel.Location = new System.Drawing.Point(9, 223);
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
            this.outputLabel.Location = new System.Drawing.Point(9, 286);
            this.outputLabel.Name = "outputLabel";
            this.outputLabel.Size = new System.Drawing.Size(88, 13);
            this.outputLabel.TabIndex = 9;
            this.outputLabel.Text = "Saving output to:";
            // 
            // outputLocation
            // 
            this.outputLocation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.outputLocation.Cursor = System.Windows.Forms.Cursors.Hand;
            this.outputLocation.Location = new System.Drawing.Point(9, 303);
            this.outputLocation.Name = "outputLocation";
            this.outputLocation.Size = new System.Drawing.Size(174, 20);
            this.outputLocation.TabIndex = 0;
            this.outputLocation.Text = "No folder selected";
            this.outputLocation.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.outputLocation.Click += new System.EventHandler(this.outputLocation_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(9, 122);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(174, 23);
            this.progressBar.TabIndex = 14;
            // 
            // progressLabel
            // 
            this.progressLabel.Location = new System.Drawing.Point(9, 106);
            this.progressLabel.Name = "progressLabel";
            this.progressLabel.Size = new System.Drawing.Size(174, 13);
            this.progressLabel.TabIndex = 15;
            this.progressLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // logCheckBox
            // 
            this.logCheckBox.AutoSize = true;
            this.logCheckBox.Location = new System.Drawing.Point(9, 152);
            this.logCheckBox.Name = "logCheckBox";
            this.logCheckBox.Size = new System.Drawing.Size(81, 17);
            this.logCheckBox.TabIndex = 16;
            this.logCheckBox.Text = "Log actions";
            this.logCheckBox.UseVisualStyleBackColor = true;
            this.logCheckBox.CheckedChanged += new System.EventHandler(this.logCheckBox_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.outputLocation);
            this.Controls.Add(this.inputLocation);
            this.Controls.Add(this.logCheckBox);
            this.Controls.Add(this.progressLabel);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.dtdLabel);
            this.Controls.Add(this.outputLabel);
            this.Controls.Add(this.inputLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.logBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.typeComboBox);
            this.Controls.Add(this.runBtn);
            this.Name = "Form1";
            this.Text = "Data set parser";
            this.ResumeLayout(false);
            this.PerformLayout();

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
    }
}

