namespace FlightWatch
{
    partial class EULAForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EULAForm));
            this.EULATextbox = new System.Windows.Forms.RichTextBox();
            this.buttonAgree = new System.Windows.Forms.Button();
            this.buttonDisagree = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // EULATextbox
            // 
            this.EULATextbox.Location = new System.Drawing.Point(12, 31);
            this.EULATextbox.Name = "EULATextbox";
            this.EULATextbox.Size = new System.Drawing.Size(660, 334);
            this.EULATextbox.TabIndex = 0;
            this.EULATextbox.Text = resources.GetString("EULATextbox.Text");
            // 
            // buttonAgree
            // 
            this.buttonAgree.Location = new System.Drawing.Point(572, 369);
            this.buttonAgree.Name = "buttonAgree";
            this.buttonAgree.Size = new System.Drawing.Size(100, 25);
            this.buttonAgree.TabIndex = 1;
            this.buttonAgree.Text = "I agree";
            this.buttonAgree.UseVisualStyleBackColor = true;
            this.buttonAgree.Click += new System.EventHandler(this.buttonAgree_Click);
            // 
            // buttonDisagree
            // 
            this.buttonDisagree.Location = new System.Drawing.Point(466, 369);
            this.buttonDisagree.Name = "buttonDisagree";
            this.buttonDisagree.Size = new System.Drawing.Size(100, 25);
            this.buttonDisagree.TabIndex = 2;
            this.buttonDisagree.Text = "I disagree";
            this.buttonDisagree.UseVisualStyleBackColor = true;
            this.buttonDisagree.Click += new System.EventHandler(this.buttonDisagree_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(566, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Due to this software using PMDG for gathering data, you are required to agree to " +
    "PMDG\'s EULA for thirdparty software.";
            // 
            // EULAForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 400);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonDisagree);
            this.Controls.Add(this.buttonAgree);
            this.Controls.Add(this.EULATextbox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "EULAForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FlightWatch EULA";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.EULAClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox EULATextbox;
        private System.Windows.Forms.Button buttonAgree;
        private System.Windows.Forms.Button buttonDisagree;
        private System.Windows.Forms.Label label1;
    }
}