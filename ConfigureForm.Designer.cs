namespace OHK
{
    partial class ConfigureForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.TestConnection = new System.Windows.Forms.Button();
            this.password = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.port = new System.Windows.Forms.TextBox();
            this.ip = new System.Windows.Forms.TextBox();
            this.MapHideGroup = new System.Windows.Forms.GroupBox();
            this.delay = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.hotkey = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.source = new System.Windows.Forms.ComboBox();
            this.scene = new System.Windows.Forms.ComboBox();
            this.SetButtom = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.Save = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.MapHideGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.TestConnection);
            this.groupBox1.Controls.Add(this.password);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.port);
            this.groupBox1.Controls.Add(this.ip);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(274, 82);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Connection";
            // 
            // TestConnection
            // 
            this.TestConnection.Location = new System.Drawing.Point(190, 50);
            this.TestConnection.Name = "TestConnection";
            this.TestConnection.Size = new System.Drawing.Size(59, 20);
            this.TestConnection.TabIndex = 6;
            this.TestConnection.Text = "Test";
            this.TestConnection.UseVisualStyleBackColor = true;
            this.TestConnection.Click += new System.EventHandler(this.TestConnection_Click);
            // 
            // password
            // 
            this.password.Location = new System.Drawing.Point(65, 50);
            this.password.Name = "password";
            this.password.PasswordChar = '*';
            this.password.Size = new System.Drawing.Size(119, 20);
            this.password.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 53);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Password:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(158, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Port:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Host/IP:";
            // 
            // port
            // 
            this.port.Location = new System.Drawing.Point(190, 19);
            this.port.MaxLength = 5;
            this.port.Name = "port";
            this.port.Size = new System.Drawing.Size(59, 20);
            this.port.TabIndex = 1;
            this.port.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.digit_KeyPress);
            // 
            // ip
            // 
            this.ip.Location = new System.Drawing.Point(65, 19);
            this.ip.Name = "ip";
            this.ip.Size = new System.Drawing.Size(91, 20);
            this.ip.TabIndex = 0;
            // 
            // MapHideGroup
            // 
            this.MapHideGroup.Controls.Add(this.delay);
            this.MapHideGroup.Controls.Add(this.label8);
            this.MapHideGroup.Controls.Add(this.hotkey);
            this.MapHideGroup.Controls.Add(this.label6);
            this.MapHideGroup.Controls.Add(this.label5);
            this.MapHideGroup.Controls.Add(this.source);
            this.MapHideGroup.Controls.Add(this.scene);
            this.MapHideGroup.Controls.Add(this.SetButtom);
            this.MapHideGroup.Controls.Add(this.label3);
            this.MapHideGroup.Location = new System.Drawing.Point(12, 100);
            this.MapHideGroup.Name = "MapHideGroup";
            this.MapHideGroup.Size = new System.Drawing.Size(274, 141);
            this.MapHideGroup.TabIndex = 2;
            this.MapHideGroup.TabStop = false;
            this.MapHideGroup.Text = "Map Hide";
            // 
            // delay
            // 
            this.delay.Location = new System.Drawing.Point(176, 108);
            this.delay.Name = "delay";
            this.delay.Size = new System.Drawing.Size(73, 20);
            this.delay.TabIndex = 10;
            this.delay.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.digit_KeyPress);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(11, 111);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(59, 13);
            this.label8.TabIndex = 9;
            this.label8.Text = "Delay (ms):";
            // 
            // hotkey
            // 
            this.hotkey.AutoSize = true;
            this.hotkey.Location = new System.Drawing.Point(62, 30);
            this.hotkey.Name = "hotkey";
            this.hotkey.Size = new System.Drawing.Size(38, 13);
            this.hotkey.TabIndex = 8;
            this.hotkey.Text = "NONE";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 84);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(44, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "Source:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 57);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Scene;";
            // 
            // source
            // 
            this.source.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.source.FormattingEnabled = true;
            this.source.Location = new System.Drawing.Point(65, 81);
            this.source.Name = "source";
            this.source.Size = new System.Drawing.Size(184, 21);
            this.source.TabIndex = 5;
            this.source.DropDown += new System.EventHandler(this.source_DropDown);
            // 
            // scene
            // 
            this.scene.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.scene.FormattingEnabled = true;
            this.scene.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.scene.Location = new System.Drawing.Point(65, 54);
            this.scene.Name = "scene";
            this.scene.Size = new System.Drawing.Size(184, 21);
            this.scene.TabIndex = 4;
            this.scene.DropDown += new System.EventHandler(this.scene_DropDown);
            // 
            // SetButtom
            // 
            this.SetButtom.Location = new System.Drawing.Point(201, 24);
            this.SetButtom.Name = "SetButtom";
            this.SetButtom.Size = new System.Drawing.Size(48, 24);
            this.SetButtom.TabIndex = 3;
            this.SetButtom.Text = "Set";
            this.SetButtom.UseVisualStyleBackColor = true;
            this.SetButtom.Click += new System.EventHandler(this.SetButtom_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 30);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "HotKey:";
            // 
            // Save
            // 
            this.Save.Location = new System.Drawing.Point(209, 247);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(77, 26);
            this.Save.TabIndex = 3;
            this.Save.Text = "Save";
            this.Save.UseVisualStyleBackColor = true;
            this.Save.Click += new System.EventHandler(this.SaveConfig);
            // 
            // Cancel
            // 
            this.Cancel.Location = new System.Drawing.Point(12, 247);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(67, 26);
            this.Cancel.TabIndex = 4;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // ConfigureForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(299, 284);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.Save);
            this.Controls.Add(this.MapHideGroup);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigureForm";
            this.Text = "Configure";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConfigureForm_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.MapHideGroup.ResumeLayout(false);
            this.MapHideGroup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button TestConnection;
        private System.Windows.Forms.TextBox password;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox port;
        private System.Windows.Forms.TextBox ip;
        private System.Windows.Forms.GroupBox MapHideGroup;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label hotkey;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox source;
        private System.Windows.Forms.ComboBox scene;
        private System.Windows.Forms.Button SetButtom;
        private System.Windows.Forms.TextBox delay;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button Save;
        private System.Windows.Forms.Button Cancel;
    }
}