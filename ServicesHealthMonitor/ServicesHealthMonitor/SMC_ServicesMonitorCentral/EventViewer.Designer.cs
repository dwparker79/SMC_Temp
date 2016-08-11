namespace SMC_ServicesMonitorCentral
{
    partial class EventViewer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EventViewer));
            this.refresh_btn = new System.Windows.Forms.Button();
            this.exit_btn = new System.Windows.Forms.Button();
            this.machine_lbl = new System.Windows.Forms.Label();
            this.machine_txtbox = new System.Windows.Forms.TextBox();
            this.services_lbl = new System.Windows.Forms.Label();
            this.services_comboBox = new System.Windows.Forms.ComboBox();
            this.log_GridView = new System.Windows.Forms.DataGridView();
            this.log_date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.log_service_name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.log_message = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.smcEvent_chkbox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.log_GridView)).BeginInit();
            this.SuspendLayout();
            // 
            // refresh_btn
            // 
            this.refresh_btn.BackColor = System.Drawing.SystemColors.ControlLight;
            this.refresh_btn.Location = new System.Drawing.Point(12, 12);
            this.refresh_btn.Name = "refresh_btn";
            this.refresh_btn.Size = new System.Drawing.Size(150, 50);
            this.refresh_btn.TabIndex = 1;
            this.refresh_btn.Text = "Refresh";
            this.refresh_btn.UseVisualStyleBackColor = false;
            this.refresh_btn.Click += new System.EventHandler(this.Refresh_btn_Click);
            // 
            // exit_btn
            // 
            this.exit_btn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.exit_btn.BackColor = System.Drawing.SystemColors.ControlLight;
            this.exit_btn.Location = new System.Drawing.Point(913, 12);
            this.exit_btn.Name = "exit_btn";
            this.exit_btn.Size = new System.Drawing.Size(150, 50);
            this.exit_btn.TabIndex = 2;
            this.exit_btn.Text = "Exit";
            this.exit_btn.UseVisualStyleBackColor = false;
            this.exit_btn.Click += new System.EventHandler(this.exit_btn_Click);
            // 
            // machine_lbl
            // 
            this.machine_lbl.AutoSize = true;
            this.machine_lbl.Location = new System.Drawing.Point(168, 27);
            this.machine_lbl.Name = "machine_lbl";
            this.machine_lbl.Size = new System.Drawing.Size(73, 20);
            this.machine_lbl.TabIndex = 3;
            this.machine_lbl.Text = "Machine:";
            // 
            // machine_txtbox
            // 
            this.machine_txtbox.Location = new System.Drawing.Point(247, 24);
            this.machine_txtbox.Name = "machine_txtbox";
            this.machine_txtbox.Size = new System.Drawing.Size(120, 26);
            this.machine_txtbox.TabIndex = 4;
            this.machine_txtbox.TextChanged += new System.EventHandler(this.machine_txtbox_TextChanged);
            // 
            // services_lbl
            // 
            this.services_lbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.services_lbl.AutoSize = true;
            this.services_lbl.Location = new System.Drawing.Point(707, 27);
            this.services_lbl.Name = "services_lbl";
            this.services_lbl.Size = new System.Drawing.Size(73, 20);
            this.services_lbl.TabIndex = 6;
            this.services_lbl.Text = "Services:";
            // 
            // services_comboBox
            // 
            this.services_comboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.services_comboBox.FormattingEnabled = true;
            this.services_comboBox.Location = new System.Drawing.Point(786, 24);
            this.services_comboBox.Name = "services_comboBox";
            this.services_comboBox.Size = new System.Drawing.Size(120, 28);
            this.services_comboBox.TabIndex = 7;
            // 
            // log_GridView
            // 
            this.log_GridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.log_GridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.log_GridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.log_date,
            this.log_service_name,
            this.log_message});
            this.log_GridView.Location = new System.Drawing.Point(13, 69);
            this.log_GridView.Name = "log_GridView";
            this.log_GridView.RowTemplate.Height = 28;
            this.log_GridView.Size = new System.Drawing.Size(1050, 463);
            this.log_GridView.TabIndex = 8;
            // 
            // log_date
            // 
            this.log_date.HeaderText = "Date";
            this.log_date.Name = "log_date";
            this.log_date.ReadOnly = true;
            this.log_date.Width = 150;
            // 
            // log_service_name
            // 
            this.log_service_name.HeaderText = "Service";
            this.log_service_name.Name = "log_service_name";
            this.log_service_name.ReadOnly = true;
            this.log_service_name.Width = 150;
            // 
            // log_message
            // 
            this.log_message.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.log_message.HeaderText = "Message";
            this.log_message.Name = "log_message";
            this.log_message.ReadOnly = true;
            // 
            // smcEvent_chkbox
            // 
            this.smcEvent_chkbox.AutoSize = true;
            this.smcEvent_chkbox.Location = new System.Drawing.Point(400, 26);
            this.smcEvent_chkbox.Name = "smcEvent_chkbox";
            this.smcEvent_chkbox.Size = new System.Drawing.Size(177, 24);
            this.smcEvent_chkbox.TabIndex = 9;
            this.smcEvent_chkbox.Text = "Include SMC events";
            this.smcEvent_chkbox.UseVisualStyleBackColor = true;
            this.smcEvent_chkbox.CheckedChanged += new System.EventHandler(this.smcEvent_chkbox_CheckedChanged);
            // 
            // EventViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1075, 544);
            this.Controls.Add(this.smcEvent_chkbox);
            this.Controls.Add(this.log_GridView);
            this.Controls.Add(this.services_comboBox);
            this.Controls.Add(this.services_lbl);
            this.Controls.Add(this.machine_txtbox);
            this.Controls.Add(this.machine_lbl);
            this.Controls.Add(this.exit_btn);
            this.Controls.Add(this.refresh_btn);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "EventViewer";
            this.Text = "EventViewer";
            this.TransparencyKey = System.Drawing.Color.Fuchsia;
            this.Load += new System.EventHandler(this.EventViewer_Load);
            ((System.ComponentModel.ISupportInitialize)(this.log_GridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button refresh_btn;
        private System.Windows.Forms.Button exit_btn;
        private System.Windows.Forms.Label machine_lbl;
        private System.Windows.Forms.TextBox machine_txtbox;
        private System.Windows.Forms.Label services_lbl;
        private System.Windows.Forms.ComboBox services_comboBox;
        private System.Windows.Forms.DataGridView log_GridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn log_date;
        private System.Windows.Forms.DataGridViewTextBoxColumn log_service_name;
        private System.Windows.Forms.DataGridViewTextBoxColumn log_message;
        private System.Windows.Forms.CheckBox smcEvent_chkbox;
    }
}