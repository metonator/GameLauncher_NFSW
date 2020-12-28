﻿using System.Windows.Forms;
namespace GameLauncher.App
{
    partial class AddServer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddServer));
            this.OkBTN = new System.Windows.Forms.Button();
            this.CancelBTN = new System.Windows.Forms.Button();
            this.ServerNameLabel = new System.Windows.Forms.Label();
            this.ServerName = new System.Windows.Forms.TextBox();
            this.ServerAddress = new System.Windows.Forms.TextBox();
            this.ServerAddressLabel = new System.Windows.Forms.Label();
            this.Error = new System.Windows.Forms.Label();
            this.Version = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // OkBTN
            // 
            this.OkBTN.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(29)))), ((int)(((byte)(38)))));
            this.OkBTN.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(181)))), ((int)(((byte)(191)))));
            this.OkBTN.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(58)))), ((int)(((byte)(76)))));
            this.OkBTN.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.OkBTN.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OkBTN.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.OkBTN.Location = new System.Drawing.Point(309, 108);
            this.OkBTN.Name = "OkBTN";
            this.OkBTN.Size = new System.Drawing.Size(75, 23);
            this.OkBTN.TabIndex = 3;
            this.OkBTN.Text = "OK";
            this.OkBTN.UseVisualStyleBackColor = false;
            this.OkBTN.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // CancelBTN
            // 
            this.CancelBTN.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(29)))), ((int)(((byte)(38)))));
            this.CancelBTN.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(181)))), ((int)(((byte)(191)))));
            this.CancelBTN.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(58)))), ((int)(((byte)(76)))));
            this.CancelBTN.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CancelBTN.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CancelBTN.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.CancelBTN.Location = new System.Drawing.Point(224, 108);
            this.CancelBTN.Name = "CancelBTN";
            this.CancelBTN.Size = new System.Drawing.Size(75, 23);
            this.CancelBTN.TabIndex = 4;
            this.CancelBTN.Text = "Cancel";
            this.CancelBTN.UseVisualStyleBackColor = false;
            this.CancelBTN.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // ServerNameLabel
            // 
            this.ServerNameLabel.AutoSize = true;
            this.ServerNameLabel.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ServerNameLabel.Location = new System.Drawing.Point(13, 13);
            this.ServerNameLabel.Name = "ServerNameLabel";
            this.ServerNameLabel.Size = new System.Drawing.Size(94, 14);
            this.ServerNameLabel.TabIndex = 5;
            this.ServerNameLabel.Text = "Server Name:";
            this.ServerNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ServerName
            // 
            this.ServerName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(29)))), ((int)(((byte)(38)))));
            this.ServerName.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ServerName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(178)))), ((int)(((byte)(210)))), ((int)(((byte)(255)))));
            this.ServerName.Location = new System.Drawing.Point(15, 29);
            this.ServerName.Name = "ServerName";
            this.ServerName.Size = new System.Drawing.Size(369, 21);
            this.ServerName.TabIndex = 1;
            this.ServerName.WordWrap = false;
            // 
            // ServerAddress
            // 
            this.ServerAddress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(29)))), ((int)(((byte)(38)))));
            this.ServerAddress.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ServerAddress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(178)))), ((int)(((byte)(210)))), ((int)(((byte)(255)))));
            this.ServerAddress.Location = new System.Drawing.Point(15, 78);
            this.ServerAddress.Name = "ServerAddress";
            this.ServerAddress.Size = new System.Drawing.Size(369, 21);
            this.ServerAddress.TabIndex = 2;
            // 
            // ServerAddressLabel
            // 
            this.ServerAddressLabel.AutoSize = true;
            this.ServerAddressLabel.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ServerAddressLabel.Location = new System.Drawing.Point(13, 62);
            this.ServerAddressLabel.Name = "ServerAddressLabel";
            this.ServerAddressLabel.Size = new System.Drawing.Size(108, 14);
            this.ServerAddressLabel.TabIndex = 6;
            this.ServerAddressLabel.Text = "Server Address:";
            this.ServerAddressLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Error
            // 
            this.Error.AutoSize = true;
            this.Error.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Error.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(200)))), ((int)(((byte)(0)))));
            this.Error.Location = new System.Drawing.Point(12, 104);
            this.Error.Name = "Error";
            this.Error.Size = new System.Drawing.Size(145, 14);
            this.Error.TabIndex = 0;
            this.Error.Text = "Please check better...";
            this.Error.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Error.Visible = false;
            // 
            // Version
            // 
            this.Version.AutoSize = true;
            this.ServerAddress.Font = new System.Drawing.Font("DejaVu Sans", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Version.Location = new System.Drawing.Point(14, 124);
            this.Version.Name = "Version";
            this.Version.Size = new System.Drawing.Size(156, 13);
            this.Version.TabIndex = 7;
            this.Version.Text = "Version : vX.X.X.X";
            // 
            // AddServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(36)))), ((int)(((byte)(45)))));
            this.ClientSize = new System.Drawing.Size(399, 141);
            this.Controls.Add(this.Version);
            this.Controls.Add(this.Error);
            this.Controls.Add(this.ServerAddress);
            this.Controls.Add(this.ServerAddressLabel);
            this.Controls.Add(this.ServerName);
            this.Controls.Add(this.ServerNameLabel);
            this.Controls.Add(this.CancelBTN);
            this.Controls.Add(this.OkBTN);
            this.Font = new System.Drawing.Font("DejaVu Sans", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddServer";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GameLauncher - Add Server";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();
            this.DoubleBuffered = true;
        }

        #endregion

        private System.Windows.Forms.Button OkBTN;
        private System.Windows.Forms.Button CancelBTN;
        private System.Windows.Forms.Label ServerNameLabel;
        private System.Windows.Forms.TextBox ServerName;
        private System.Windows.Forms.TextBox ServerAddress;
        private System.Windows.Forms.Label ServerAddressLabel;
        private System.Windows.Forms.Label Error;
        private System.Windows.Forms.Label Version;
    }
}