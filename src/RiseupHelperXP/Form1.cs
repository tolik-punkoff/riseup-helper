using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace RiseupHelperXP
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            if (!CommonFunctions.Preload())
            {
                Application.Exit();
                return;
            }

            foreach (string s in CommonFunctions.ProvidersList)
            {
                cmbProvider.Items.Add(s);
            }

            cmbProvider.SelectedIndex = 0;
        }
        
        #region form_work
        private void btnNetSettings_Click(object sender, EventArgs e)
        {
            frmNetworkSettings fNetworkSettings = new frmNetworkSettings();
            fNetworkSettings.ShowDialog();
        }

        private void DisableAllButtons()
        {
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Button)
                {
                    Button button = (Button)ctrl;
                    button.Enabled = false;
                }
            }
        }

        private void EnableAllButtons()
        {
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Button)
                {
                    Button button = (Button)ctrl;
                    button.Enabled = true;
                }
            }
        }

        private void lvLog_DoubleClick(object sender, EventArgs e)
        {
            string msg = "";
            foreach (ListViewItem item in lvLog.SelectedItems)
            {
                msg = msg + item.Text + "\r\n";
            }
            
            Clipboard.SetText(msg);
            msg = msg + "Текст скопирован в буфер обмена.";
            
            MessageBox.Show(msg, "Сообщение", MessageBoxButtons.OK,
                MessageBoxIcon.Information);

        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show("(L) Tolik Punkoff, 2020", "Riseup Helper XP",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void AddToLog(string message, Color color)
        {
            lvLog.Items.Add(message);
            lvLog.Items[lvLog.Items.Count - 1].ForeColor = color;
            lvLog.TopItem = lvLog.Items[lvLog.Items.Count - 1];
        }
        #endregion

        private void btnProviderKey_Click(object sender, EventArgs e)
        {
            lvLog.Items.Clear();
            MainWorker mw = new MainWorker(cmbProvider.SelectedItem.ToString());
            mw.StatusChanged += new MainWorker.OnStatusChanged(mw_StatusChanged);
            if (!mw.WorkerInit())
            {
                return;
            }
            mw.StartGetProviderKey();
        }

        private void btnUserKey_Click(object sender, EventArgs e)
        {
            lvLog.Items.Clear();
            MainWorker mw = new MainWorker(cmbProvider.SelectedItem.ToString());
            mw.StatusChanged += new MainWorker.OnStatusChanged(mw_StatusChanged);
            if (!mw.WorkerInit())
            {
                return;
            }
            mw.StartGetUserKey();
        }

        void mw_StatusChanged(object sender, WorkerStatusEventArgs e)
        {
            Invoke((MethodInvoker)delegate
            {
                lvLog.Items.Add(e.Message);

                Color c = Color.Brown;

                if (e.Status == WorkerStatus.Start)
                {                    
                    pbConnecting.Image = Properties.Resources.connecting;
                    DisableAllButtons();
                }

                if ((e.Status == WorkerStatus.FatalError) ||
                    (e.Status == WorkerStatus.CompleteAll))
                {
                    pbConnecting.Image = Properties.Resources.riseupvpn_64x64;
                    EnableAllButtons();
                }

                switch (e.Status)
                {
                    case WorkerStatus.Start: c = Color.LightSteelBlue; break;
                    case WorkerStatus.Process: c = Color.DarkGray; break;
                    case WorkerStatus.Complete: c = Color.MediumSeaGreen; break;
                    case WorkerStatus.CompleteAll: c = Color.LimeGreen; break;
                    case WorkerStatus.NotComplete: c = Color.Yellow; break;
                    case WorkerStatus.FatalError: c = Color.Red; break;
                    case WorkerStatus.Wait: c = Color.White; break;
                }

                lvLog.Items[lvLog.Items.Count - 1].ForeColor = c;
                lvLog.TopItem = lvLog.Items[lvLog.Items.Count - 1];
            });

        }

        private void SaveFile(string filename)
        {
            string dir = CommonFunctions.SettingsPath +
                cmbProvider.SelectedItem.ToString() + "\\";

            if (!File.Exists(dir + filename))
            {
                AddToLog("Файл не найден: " + dir + filename, Color.Red);
            }
            else
            {
                dlgSaveKeys.FileName = filename;
                DialogResult Ans = dlgSaveKeys.ShowDialog();
                if (Ans == DialogResult.Cancel)
                {
                    AddToLog("Сохранение " + filename + " отменено.", Color.Red);
                }
                else
                {
                    try
                    {
                        File.Copy(dir + filename, dlgSaveKeys.FileName, true);
                        AddToLog(filename + " сохранен как " +
                            dlgSaveKeys.FileName, Color.LimeGreen);
                    }
                    catch (Exception ex)
                    {
                        AddToLog("Ошибка: " + ex.Message, Color.Red);
                    }
                }
            }
        }

        private void btnSaveKeys_Click(object sender, EventArgs e)
        {
            dlgSaveKeys.InitialDirectory = "C:\\";

            SaveFile("cacert.pem");
            SaveFile("openvpn.pem");
        }

        private void btnDeleteProvData_Click(object sender, EventArgs e)
        {
            DialogResult Ans = MessageBox.Show("Удалить данные " + 
                cmbProvider.SelectedItem.ToString() + "?", "Удаление данных", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (Ans == DialogResult.No) return;

            lvLog.Items.Clear();
            MainWorker mw = new MainWorker(cmbProvider.SelectedItem.ToString());
            mw.StatusChanged += new MainWorker.OnStatusChanged(mw_StatusChanged);            
            mw.StartRemoveWorkdir();
        }

        private void btnDeleteAllData_Click(object sender, EventArgs e)
        {
            DialogResult Ans = MessageBox.Show("Удалить данные всех провайдеров?", 
                "Удаление данных",MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (Ans == DialogResult.No) return;

            lvLog.Items.Clear();
            MainWorker mw = new MainWorker(cmbProvider.SelectedItem.ToString());
            mw.StatusChanged += new MainWorker.OnStatusChanged(mw_StatusChanged);            
            mw.StartRemoveAllProviderData();
        }
    }
}
