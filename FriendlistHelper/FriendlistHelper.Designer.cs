﻿namespace HexetchButBetter
{
    partial class FriendlistHelperForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FriendlistHelperForm));
            this.outputPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.messageTextbox = new System.Windows.Forms.TextBox();
            this.outputPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // outputPanel
            // 
            this.outputPanel.AutoScroll = true;
            this.outputPanel.Controls.Add(this.panel1);
            this.outputPanel.Location = new System.Drawing.Point(13, 109);
            this.outputPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.outputPanel.Name = "outputPanel";
            this.outputPanel.Size = new System.Drawing.Size(438, 320);
            this.outputPanel.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(213, 0);
            this.panel1.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.button1.Location = new System.Drawing.Point(0, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(451, 46);
            this.button1.TabIndex = 3;
            this.button1.Text = "Send message";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.loadFriends_Click);
            // 
            // messageTextbox
            // 
            this.messageTextbox.Location = new System.Drawing.Point(12, 52);
            this.messageTextbox.Multiline = true;
            this.messageTextbox.Name = "messageTextbox";
            this.messageTextbox.Size = new System.Drawing.Size(439, 51);
            this.messageTextbox.TabIndex = 4;
            // 
            // FriendlistHelperForm
            // 
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(464, 441);
            this.Controls.Add(this.messageTextbox);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.outputPanel);
            this.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(15, 15);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MinimumSize = new System.Drawing.Size(480, 480);
            this.Name = "FriendlistHelperForm";
            this.Load += new System.EventHandler(this.FriendlistHelperForm_Load);
            this.outputPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.TextBox messageTextbox;

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.FlowLayoutPanel outputPanel;
        private System.Windows.Forms.Panel panel1;

        #endregion
    }
}