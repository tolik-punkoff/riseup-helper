namespace RiseupHelperXP
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.label1 = new System.Windows.Forms.Label();
            this.cmbProvider = new System.Windows.Forms.ComboBox();
            this.btnAbout = new System.Windows.Forms.Button();
            this.btnNetSettings = new System.Windows.Forms.Button();
            this.btnProviderKey = new System.Windows.Forms.Button();
            this.btnUserKey = new System.Windows.Forms.Button();
            this.btnSaveKeys = new System.Windows.Forms.Button();
            this.btnDeleteProvData = new System.Windows.Forms.Button();
            this.pbConnecting = new System.Windows.Forms.PictureBox();
            this.btnDeleteAllData = new System.Windows.Forms.Button();
            this.dlgSaveKeys = new System.Windows.Forms.SaveFileDialog();
            this.lvLog = new RiseupHelperXP.MyListView();
            this.colMain = new System.Windows.Forms.ColumnHeader();
            ((System.ComponentModel.ISupportInitialize)(this.pbConnecting)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(117, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Выбрать провайдера:";
            // 
            // cmbProvider
            // 
            this.cmbProvider.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProvider.FormattingEnabled = true;
            this.cmbProvider.Location = new System.Drawing.Point(126, 5);
            this.cmbProvider.Name = "cmbProvider";
            this.cmbProvider.Size = new System.Drawing.Size(245, 21);
            this.cmbProvider.TabIndex = 2;
            // 
            // btnAbout
            // 
            this.btnAbout.Location = new System.Drawing.Point(521, 422);
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(106, 27);
            this.btnAbout.TabIndex = 5;
            this.btnAbout.Text = "О программе...";
            this.btnAbout.UseVisualStyleBackColor = true;
            this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
            // 
            // btnNetSettings
            // 
            this.btnNetSettings.Location = new System.Drawing.Point(521, 388);
            this.btnNetSettings.Name = "btnNetSettings";
            this.btnNetSettings.Size = new System.Drawing.Size(106, 27);
            this.btnNetSettings.TabIndex = 6;
            this.btnNetSettings.Text = "Настройки сети...";
            this.btnNetSettings.UseVisualStyleBackColor = true;
            this.btnNetSettings.Click += new System.EventHandler(this.btnNetSettings_Click);
            // 
            // btnProviderKey
            // 
            this.btnProviderKey.Location = new System.Drawing.Point(521, 32);
            this.btnProviderKey.Name = "btnProviderKey";
            this.btnProviderKey.Size = new System.Drawing.Size(106, 35);
            this.btnProviderKey.TabIndex = 7;
            this.btnProviderKey.Text = "Получить ключ провайдера";
            this.btnProviderKey.UseVisualStyleBackColor = true;
            this.btnProviderKey.Click += new System.EventHandler(this.btnProviderKey_Click);
            // 
            // btnUserKey
            // 
            this.btnUserKey.Location = new System.Drawing.Point(523, 73);
            this.btnUserKey.Name = "btnUserKey";
            this.btnUserKey.Size = new System.Drawing.Size(106, 35);
            this.btnUserKey.TabIndex = 8;
            this.btnUserKey.Text = "Получить ключ пользователя";
            this.btnUserKey.UseVisualStyleBackColor = true;
            this.btnUserKey.Click += new System.EventHandler(this.btnUserKey_Click);
            // 
            // btnSaveKeys
            // 
            this.btnSaveKeys.Location = new System.Drawing.Point(523, 114);
            this.btnSaveKeys.Name = "btnSaveKeys";
            this.btnSaveKeys.Size = new System.Drawing.Size(106, 35);
            this.btnSaveKeys.TabIndex = 9;
            this.btnSaveKeys.Text = "Сохранить ключи...";
            this.btnSaveKeys.UseVisualStyleBackColor = true;
            this.btnSaveKeys.Click += new System.EventHandler(this.btnSaveKeys_Click);
            // 
            // btnDeleteProvData
            // 
            this.btnDeleteProvData.Location = new System.Drawing.Point(523, 155);
            this.btnDeleteProvData.Name = "btnDeleteProvData";
            this.btnDeleteProvData.Size = new System.Drawing.Size(106, 35);
            this.btnDeleteProvData.TabIndex = 10;
            this.btnDeleteProvData.Text = "Удалить данные провайдера";
            this.btnDeleteProvData.UseVisualStyleBackColor = true;
            this.btnDeleteProvData.Click += new System.EventHandler(this.btnDeleteProvData_Click);
            // 
            // pbConnecting
            // 
            this.pbConnecting.Image = global::RiseupHelperXP.Properties.Resources.riseupvpn_64x64;
            this.pbConnecting.Location = new System.Drawing.Point(544, 318);
            this.pbConnecting.Name = "pbConnecting";
            this.pbConnecting.Size = new System.Drawing.Size(64, 64);
            this.pbConnecting.TabIndex = 28;
            this.pbConnecting.TabStop = false;
            // 
            // btnDeleteAllData
            // 
            this.btnDeleteAllData.Location = new System.Drawing.Point(523, 196);
            this.btnDeleteAllData.Name = "btnDeleteAllData";
            this.btnDeleteAllData.Size = new System.Drawing.Size(106, 35);
            this.btnDeleteAllData.TabIndex = 29;
            this.btnDeleteAllData.Text = "Удалить все данные ";
            this.btnDeleteAllData.UseVisualStyleBackColor = true;
            this.btnDeleteAllData.Click += new System.EventHandler(this.btnDeleteAllData_Click);
            // 
            // dlgSaveKeys
            // 
            this.dlgSaveKeys.Filter = "Файлы ключей (*.pem)|*.pem|Все файлы (*.*)|*.*";
            // 
            // lvLog
            // 
            this.lvLog.BackColor = System.Drawing.Color.Black;
            this.lvLog.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colMain});
            this.lvLog.ForeColor = System.Drawing.Color.Red;
            this.lvLog.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvLog.Location = new System.Drawing.Point(-1, 32);
            this.lvLog.Name = "lvLog";
            this.lvLog.Size = new System.Drawing.Size(518, 417);
            this.lvLog.TabIndex = 0;
            this.lvLog.UseCompatibleStateImageBehavior = false;
            this.lvLog.View = System.Windows.Forms.View.Details;
            this.lvLog.DoubleClick += new System.EventHandler(this.lvLog_DoubleClick);
            // 
            // colMain
            // 
            this.colMain.Width = 515;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(629, 452);
            this.Controls.Add(this.btnDeleteAllData);
            this.Controls.Add(this.pbConnecting);
            this.Controls.Add(this.btnDeleteProvData);
            this.Controls.Add(this.btnSaveKeys);
            this.Controls.Add(this.btnUserKey);
            this.Controls.Add(this.btnProviderKey);
            this.Controls.Add(this.btnNetSettings);
            this.Controls.Add(this.btnAbout);
            this.Controls.Add(this.cmbProvider);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lvLog);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Riseup Helper XP";
            this.Load += new System.EventHandler(this.frmMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbConnecting)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MyListView lvLog;
        private System.Windows.Forms.ColumnHeader colMain;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbProvider;
        private System.Windows.Forms.Button btnAbout;
        private System.Windows.Forms.Button btnNetSettings;
        private System.Windows.Forms.Button btnProviderKey;
        private System.Windows.Forms.Button btnUserKey;
        private System.Windows.Forms.Button btnSaveKeys;
        private System.Windows.Forms.Button btnDeleteProvData;
        private System.Windows.Forms.PictureBox pbConnecting;
        private System.Windows.Forms.Button btnDeleteAllData;
        private System.Windows.Forms.SaveFileDialog dlgSaveKeys;

        
    }
}

