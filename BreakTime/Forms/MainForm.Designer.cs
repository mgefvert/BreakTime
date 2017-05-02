using System;

namespace BreakTime.Forms
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this._timeLabel = new System.Windows.Forms.Label();
            this._snoozeButton = new System.Windows.Forms.Button();
            this.breakTimer = new System.Windows.Forms.Timer(this.components);
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.displayTimer = new System.Windows.Forms.Timer(this.components);
            this._fortuneLabel = new System.Windows.Forms.Label();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _timeLabel
            // 
            this._timeLabel.AutoSize = true;
            this._timeLabel.BackColor = System.Drawing.Color.Transparent;
            this._timeLabel.Font = new System.Drawing.Font("Segoe UI", 128F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._timeLabel.ForeColor = System.Drawing.Color.White;
            this._timeLabel.Location = new System.Drawing.Point(234, 60);
            this._timeLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._timeLabel.Name = "_timeLabel";
            this._timeLabel.Size = new System.Drawing.Size(408, 227);
            this._timeLabel.TabIndex = 0;
            this._timeLabel.Text = "4:59";
            this._timeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _snoozeButton
            // 
            this._snoozeButton.BackColor = System.Drawing.Color.Transparent;
            this._snoozeButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this._snoozeButton.FlatAppearance.BorderSize = 0;
            this._snoozeButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.SkyBlue;
            this._snoozeButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DimGray;
            this._snoozeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._snoozeButton.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._snoozeButton.ForeColor = System.Drawing.Color.White;
            this._snoozeButton.Location = new System.Drawing.Point(391, 277);
            this._snoozeButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._snoozeButton.Name = "_snoozeButton";
            this._snoozeButton.Size = new System.Drawing.Size(98, 29);
            this._snoozeButton.TabIndex = 1;
            this._snoozeButton.Text = "Snooze";
            this._snoozeButton.UseVisualStyleBackColor = false;
            this._snoozeButton.Click += new System.EventHandler(this.snoozeButton_Click);
            // 
            // breakTimer
            // 
            this.breakTimer.Enabled = true;
            this.breakTimer.Interval = 1000;
            this.breakTimer.Tick += new System.EventHandler(this.breakTimer_Tick);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.BalloonTipText = "Upcoming break in 3 minutes";
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "Breaktime";
            this.notifyIcon1.Visible = true;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripSeparator1,
            this.closeToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(126, 54);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(125, 22);
            this.toolStripMenuItem1.Text = "Settings...";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(122, 6);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // displayTimer
            // 
            this.displayTimer.Tick += new System.EventHandler(this.displayTimer_Tick);
            // 
            // _fortuneLabel
            // 
            this._fortuneLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._fortuneLabel.BackColor = System.Drawing.Color.Transparent;
            this._fortuneLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._fortuneLabel.ForeColor = System.Drawing.Color.DarkGray;
            this._fortuneLabel.Location = new System.Drawing.Point(12, 362);
            this._fortuneLabel.Name = "_fortuneLabel";
            this._fortuneLabel.Size = new System.Drawing.Size(786, 72);
            this._fortuneLabel.TabIndex = 2;
            this._fortuneLabel.Text = "Fortune";
            this._fortuneLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DimGray;
            this.ClientSize = new System.Drawing.Size(810, 443);
            this.Controls.Add(this._fortuneLabel);
            this.Controls.Add(this._snoozeButton);
            this.Controls.Add(this._timeLabel);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.DimGray;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "MainForm";
            this.Opacity = 0.95D;
            this.Text = "Breaktime";
            this.TopMost = true;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.VisibleChanged += new System.EventHandler(this.MainForm_VisibleChanged);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _timeLabel;
        private System.Windows.Forms.Button _snoozeButton;
        private System.Windows.Forms.Timer breakTimer;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.Timer displayTimer;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Label _fortuneLabel;
    }
}

