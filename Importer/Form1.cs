﻿using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace Importer
{
    public partial class Form1 : Form
    {
        private string inputPath;

        private BackgroundWorker worker;
        private bool workerInterrupted;
        private Importer importer;

        public Form1()
        {
            InitializeComponent();
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;

            // Use presets if provided
            if (File.Exists("../../config.txt"))
            {
                using (StreamReader sr = new StreamReader("../../config.txt"))
                {
                    inputPath = sr.ReadLine();
                    inputLocation.Text = inputPath;
                }
            }
            importer = new Importer();
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            runBtn.Enabled = true;
            if (e.Cancelled || e.Error != null || workerInterrupted)
            {
                progressLabel.Text = "";
                progressBar.Value = 0;
                workerInterrupted = false;
            }
            else
            {
                progressLabel.Text = "100%";
                progressBar.Value = 100;
                MessageBox.Show("Work completed!");
            }
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
                importer.Run(inputPath, worker);
            try
            {
            }
            catch (Exception ex)
            {
                worker.ReportProgress(progressBar.Value, ex.Message);
                workerInterrupted = true;
            }
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState.ToString() != "")
                Error(e.UserState.ToString());
            progressLabel.Text = $"{e.ProgressPercentage}%";
            progressBar.Value = e.ProgressPercentage;
        }

        private void inputPanel_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                inputPath = dialog.FileName;
                inputLocation.Text = inputPath;
            }
        }

        private void runBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(inputPath))
            {
                Error("No input file given");
                return;
            }

            runBtn.Enabled = false;
            progressLabel.Text = "0%";
            progressBar.Value = 0;
            worker.RunWorkerAsync();
        }

        // Display an error in a message box
        private void Error(string msg)
        {
            MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
