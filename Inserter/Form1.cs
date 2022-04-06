using System;
using System.IO;
using System.Windows.Forms;

namespace Inserter
{
    public partial class Form1 : Form
    {
        private string inputPath;
        private string username;
        private string password;

        public Form1()
        {
            InitializeComponent();

            // Use presets if provided
            if (File.Exists("../../config.txt"))
            {
                using (StreamReader sr = new StreamReader("../../config.txt"))
                {
                    inputPath = sr.ReadLine();
                    username = sr.ReadLine();
                    password = sr.ReadLine();
                    inputLocation.Text = inputPath;
                    usernameInput.Text = username;
                    passwordInput.Text = password;
                }
            }
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
            MessageBox.Show("Congratulations! You have successfully pressed a button.");
        }
    }
}
