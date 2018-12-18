using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SnT.IO.PortSniffer
{
    public partial class FormMain : Form
    {
        private readonly Sniffer.PortSniffer portSniffer;

        public FormMain()
        {
            InitializeComponent();
            Icon = Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
            portSniffer = new Sniffer.PortSniffer();
            portSniffer.DataRowChanged += new EventHandler<Sniffer.DataRowChangedEventArgs>(portSniffer_DataRowChanged);
            portSniffer.DataCleared += new EventHandler(portSniffer_DataCleared);
            LoadSettings();
            propertyGridLeftPort.SelectedObject = portSniffer.LeftSettings;
            propertyGridRightPort.SelectedObject = portSniffer.RightSettings;
        }

        private void SetStatus(string msg)
        {
            statusLabel.Text = String.Format("{0:t} {1}", DateTime.Now, msg);
        }

        void portSniffer_DataCleared(object sender, EventArgs e)
        {
            if (InvokeRequired)
                BeginInvoke(new EventHandler(portSniffer_DataCleared), sender, e);
            else
            {
                listBoxData.Items.Clear();
                SetStatus("Log cleared");
            }
        }

        void portSniffer_DataRowChanged(object sender, Sniffer.DataRowChangedEventArgs e)
        {
            if (InvokeRequired)
                BeginInvoke(new EventHandler<Sniffer.DataRowChangedEventArgs>(portSniffer_DataRowChanged), sender, e);
            else
            {
                if (e.IsNew)
                    listBoxData.Items.Add(e.Row);
                else
                    listBoxData.Items[e.Index] = e.Row;
                listBoxData.SelectedIndex = listBoxData.Items.Count - 1;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (!e.Cancel)
            {
                DialogResult result = MessageBox.Show(this, "Save current configuration?",
                    "Question", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2);
                e.Cancel = (result == DialogResult.Cancel);
                if (result == DialogResult.Yes)
                    e.Cancel = !SaveSettings(); // Když se neuloží, neukončí se aplikace
            }
        }

        private bool SaveSettings()
        {
            try
            {
                string localAppDataFolder =
                    System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "SerialPortSniffer");
                if (!System.IO.Directory.Exists(localAppDataFolder))
                    System.IO.Directory.CreateDirectory(localAppDataFolder);
                portSniffer.SaveSettings(localAppDataFolder);
                return true;
            }
            catch (Exception ex)
            {
                ShowError("Save setting error.", ex);
            }
            return false;
        }

        private void LoadSettings()
        {
            try
            {
                string localAppDataFolder =
                    System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "SerialPortSniffer");
                if (System.IO.Directory.Exists(localAppDataFolder))
                    portSniffer.LoadSettings(localAppDataFolder);
                SetStatus("Settings loaded");
            }
            catch (Exception ex)
            {
                ShowError("Load setting error.", ex);
            }
        }

        private void ShowError(string msg, Exception ex)
        {
            MessageBox.Show(this,
                String.Format("{0}\r\n{1}", msg, ex),
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        protected override void OnClosed(EventArgs e)
        {
            portSniffer.Close();
            base.OnClosed(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            splitContainerPorts.SplitterDistance = this.ClientSize.Width / 2 - splitContainerPorts.SplitterWidth / 2;
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            try
            {
                portSniffer.Open();
                SetStatus("Ports opened");
            }
            catch (Exception ex)
            {
                ShowError("Open exception.", ex);
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            portSniffer.Close();
            SetStatus("Ports closed");
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            portSniffer.Clear();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog()
            {
                AddExtension = true,
                DefaultExt = "txt",
                Filter = "Text Files (*.txt)|*.txt|All Files|*.*",
                FilterIndex = 0,
                CheckPathExists = true,
                OverwritePrompt = true,
                Title = "Save to file",
                ValidateNames = true
            })
            {
                if (sfd.ShowDialog(this) == DialogResult.OK)
                {
                    using (System.IO.Stream stream = sfd.OpenFile())
                    {
                        portSniffer.Save(stream);
                    }
                }
            }
        }

        private void buttonAbout_Click(object sender, EventArgs e)
        {
            using (AboutBox ab = new AboutBox())
            {
                ab.ShowDialog(this);
            }
        }

    }
}
