namespace PingPong
{
    partial class frmPingPong
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
            this.txtApiLink = new System.Windows.Forms.TextBox();
            this.btnStart = new DevExpress.XtraEditors.SimpleButton();
            this.lblStatus = new System.Windows.Forms.Label();
            this.timerTick = new System.Windows.Forms.Timer(this.components);
            this.btnStop = new DevExpress.XtraEditors.SimpleButton();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtApiLink
            // 
            this.txtApiLink.Location = new System.Drawing.Point(12, 39);
            this.txtApiLink.Name = "txtApiLink";
            this.txtApiLink.Size = new System.Drawing.Size(695, 20);
            this.txtApiLink.TabIndex = 0;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(713, 37);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "Start";
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(13, 85);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 13);
            this.lblStatus.TabIndex = 2;
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(794, 37);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 3;
            this.btnStop.Text = "Stop";
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(257, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Ex: http://103.48.194.70/employeetracking_api/api/";
            // 
            // frmPingPong
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(876, 128);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.txtApiLink);
            this.Name = "frmPingPong";
            this.Text = "PingPong";
            this.Load += new System.EventHandler(this.frmPingPong_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtApiLink;
        private DevExpress.XtraEditors.SimpleButton btnStart;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Timer timerTick;
        private DevExpress.XtraEditors.SimpleButton btnStop;
        private System.Windows.Forms.Label label1;
    }
}

