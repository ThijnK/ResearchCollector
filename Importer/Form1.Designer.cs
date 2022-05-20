namespace Importer
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.inputPanel = new System.Windows.Forms.Panel();
            this.inputLocation = new System.Windows.Forms.Label();
            this.inputLabel = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.progressLabel = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.inputPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // runBtn
            // 
            this.runBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.runBtn.Location = new System.Drawing.Point(3, 124);
            this.runBtn.Name = "runBtn";
            this.runBtn.Size = new System.Drawing.Size(240, 31);
            this.runBtn.TabIndex = 0;
            this.runBtn.Text = "Run";
            this.runBtn.UseVisualStyleBackColor = true;
            this.runBtn.Click += new System.EventHandler(this.runBtn_Click);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.inputPanel);
            this.panel1.Controls.Add(this.inputLabel);
            this.panel1.Controls.Add(this.runBtn);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(248, 160);
            this.panel1.TabIndex = 1;
            // 
            // inputPanel
            // 
            this.inputPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.inputPanel.Controls.Add(this.inputLocation);
            this.inputPanel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.inputPanel.Location = new System.Drawing.Point(3, 21);
            this.inputPanel.Name = "inputPanel";
            this.inputPanel.Size = new System.Drawing.Size(240, 20);
            this.inputPanel.TabIndex = 13;
            this.inputPanel.Click += new System.EventHandler(this.inputPanel_Click);
            // 
            // inputLocation
            // 
            this.inputLocation.AutoSize = true;
            this.inputLocation.Location = new System.Drawing.Point(3, 2);
            this.inputLocation.Name = "inputLocation";
            this.inputLocation.Size = new System.Drawing.Size(80, 13);
            this.inputLocation.TabIndex = 0;
            this.inputLocation.Text = "No file selected";
            // 
            // inputLabel
            // 
            this.inputLabel.AutoSize = true;
            this.inputLabel.Location = new System.Drawing.Point(3, 5);
            this.inputLabel.Name = "inputLabel";
            this.inputLabel.Size = new System.Drawing.Size(50, 13);
            this.inputLabel.TabIndex = 12;
            this.inputLabel.Text = "Input file:";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 178);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(248, 23);
            this.progressBar.TabIndex = 2;
            // 
            // progressLabel
            // 
            this.progressLabel.Location = new System.Drawing.Point(12, 204);
            this.progressLabel.Name = "progressLabel";
            this.progressLabel.Size = new System.Drawing.Size(248, 16);
            this.progressLabel.TabIndex = 3;
            this.progressLabel.Text = "0%";
            this.progressLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.progressLabel);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "Inserter";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.inputPanel.ResumeLayout(false);
            this.inputPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button runBtn;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel inputPanel;
        private System.Windows.Forms.Label inputLocation;
        private System.Windows.Forms.Label inputLabel;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label progressLabel;
    }
}

