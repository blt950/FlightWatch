namespace FlightWatch
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
            this.buttonB737 = new System.Windows.Forms.Button();
            this.buttonB777 = new System.Windows.Forms.Button();
            this.buttonB747 = new System.Windows.Forms.Button();
            this.richResponse = new System.Windows.Forms.RichTextBox();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.buttonDisconnect = new System.Windows.Forms.Button();
            this.labelBy = new System.Windows.Forms.Label();
            this.beepTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // buttonB737
            // 
            this.buttonB737.Enabled = false;
            this.buttonB737.Location = new System.Drawing.Point(215, 12);
            this.buttonB737.Name = "buttonB737";
            this.buttonB737.Size = new System.Drawing.Size(125, 23);
            this.buttonB737.TabIndex = 0;
            this.buttonB737.Text = "Watch PMDG 737";
            this.buttonB737.UseVisualStyleBackColor = true;
            this.buttonB737.Click += new System.EventHandler(this.buttonB737_Click);
            // 
            // buttonB777
            // 
            this.buttonB777.Enabled = false;
            this.buttonB777.Location = new System.Drawing.Point(215, 41);
            this.buttonB777.Name = "buttonB777";
            this.buttonB777.Size = new System.Drawing.Size(125, 23);
            this.buttonB777.TabIndex = 1;
            this.buttonB777.Text = "Watch PMDG 777";
            this.buttonB777.UseVisualStyleBackColor = true;
            this.buttonB777.Click += new System.EventHandler(this.buttonB777_Click);
            // 
            // buttonB747
            // 
            this.buttonB747.Enabled = false;
            this.buttonB747.Location = new System.Drawing.Point(215, 70);
            this.buttonB747.Name = "buttonB747";
            this.buttonB747.Size = new System.Drawing.Size(125, 23);
            this.buttonB747.TabIndex = 2;
            this.buttonB747.Text = "Watch PMDG 747";
            this.buttonB747.UseVisualStyleBackColor = true;
            this.buttonB747.Click += new System.EventHandler(this.buttonB747_Click);
            // 
            // richResponse
            // 
            this.richResponse.Location = new System.Drawing.Point(12, 12);
            this.richResponse.Name = "richResponse";
            this.richResponse.ReadOnly = true;
            this.richResponse.Size = new System.Drawing.Size(197, 81);
            this.richResponse.TabIndex = 4;
            this.richResponse.Text = "";
            // 
            // buttonConnect
            // 
            this.buttonConnect.Location = new System.Drawing.Point(12, 99);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(95, 23);
            this.buttonConnect.TabIndex = 5;
            this.buttonConnect.Text = "Connect";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // buttonDisconnect
            // 
            this.buttonDisconnect.Enabled = false;
            this.buttonDisconnect.Location = new System.Drawing.Point(113, 99);
            this.buttonDisconnect.Name = "buttonDisconnect";
            this.buttonDisconnect.Size = new System.Drawing.Size(95, 23);
            this.buttonDisconnect.TabIndex = 6;
            this.buttonDisconnect.Text = "Disconnect";
            this.buttonDisconnect.UseVisualStyleBackColor = true;
            this.buttonDisconnect.Click += new System.EventHandler(this.buttonDisconnect_Click);
            // 
            // labelBy
            // 
            this.labelBy.AutoSize = true;
            this.labelBy.Location = new System.Drawing.Point(216, 104);
            this.labelBy.Name = "labelBy";
            this.labelBy.Size = new System.Drawing.Size(124, 13);
            this.labelBy.TabIndex = 7;
            this.labelBy.Text = "Created by Daniel Lange";
            this.labelBy.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // beepTimer
            // 
            this.beepTimer.Interval = 2000;
            this.beepTimer.Tick += new System.EventHandler(this.beepTimer_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(352, 133);
            this.Controls.Add(this.labelBy);
            this.Controls.Add(this.buttonDisconnect);
            this.Controls.Add(this.buttonConnect);
            this.Controls.Add(this.richResponse);
            this.Controls.Add(this.buttonB747);
            this.Controls.Add(this.buttonB777);
            this.Controls.Add(this.buttonB737);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FlightWatch v.0.1";
            this.Activated += new System.EventHandler(this.Form_GetsFocused);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonB737;
        private System.Windows.Forms.Button buttonB777;
        private System.Windows.Forms.Button buttonB747;
        private System.Windows.Forms.RichTextBox richResponse;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.Button buttonDisconnect;
        private System.Windows.Forms.Label labelBy;
        private System.Windows.Forms.Timer beepTimer;
    }
}

