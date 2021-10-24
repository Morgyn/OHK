
namespace OBSKeys
{
    partial class DebugLogForm
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
            this.logConsole = new System.Windows.Forms.RichTextBox();
            this.copyButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // logConsole
            // 
            this.logConsole.BackColor = System.Drawing.Color.Black;
            this.logConsole.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.logConsole.Cursor = System.Windows.Forms.Cursors.No;
            this.logConsole.DetectUrls = false;
            this.logConsole.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.logConsole.ForeColor = System.Drawing.Color.Green;
            this.logConsole.Location = new System.Drawing.Point(0, 43);
            this.logConsole.Name = "logConsole";
            this.logConsole.ReadOnly = true;
            this.logConsole.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.logConsole.Size = new System.Drawing.Size(800, 407);
            this.logConsole.TabIndex = 1;
            this.logConsole.Text = "";
            // 
            // copyButton
            // 
            this.copyButton.Location = new System.Drawing.Point(12, 10);
            this.copyButton.Name = "copyButton";
            this.copyButton.Size = new System.Drawing.Size(104, 25);
            this.copyButton.TabIndex = 2;
            this.copyButton.Text = "Copy to clipboard";
            this.copyButton.UseVisualStyleBackColor = true;
            this.copyButton.Click += new System.EventHandler(this.copyButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(122, 10);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 25);
            this.saveButton.TabIndex = 3;
            this.saveButton.Text = "Save to file";
            this.saveButton.UseVisualStyleBackColor = true;
            // 
            // DebugLogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.copyButton);
            this.Controls.Add(this.logConsole);
            this.Name = "DebugLogForm";
            this.RightToLeftLayout = true;
            this.Text = "Debug Log";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DebugLogForm_FormClosing);
            this.ResizeEnd += new System.EventHandler(this.DebugLogForm_ResizeEnd);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox logConsole;
        private System.Windows.Forms.Button copyButton;
        private System.Windows.Forms.Button saveButton;
    }
}