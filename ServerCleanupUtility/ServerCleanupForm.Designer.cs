namespace ServerCleanupUtility
{
    partial class ServerCleanupForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServerCleanupForm));
            this.Zip = new System.Windows.Forms.Button();
            this.Delete = new System.Windows.Forms.Button();
            this.logBox = new System.Windows.Forms.TextBox();
            this.instBox = new System.Windows.Forms.TextBox();
            this.instructionsLabel = new System.Windows.Forms.Label();
            this.logLabel = new System.Windows.Forms.Label();
            this.linkBox = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // Zip
            // 
            this.Zip.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Zip.Location = new System.Drawing.Point(130, 276);
            this.Zip.Name = "Zip";
            this.Zip.Size = new System.Drawing.Size(75, 32);
            this.Zip.TabIndex = 0;
            this.Zip.Text = "Archive";
            this.Zip.UseVisualStyleBackColor = true;
            this.Zip.Click += new System.EventHandler(this.Zip_Click);
            // 
            // Delete
            // 
            this.Delete.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Delete.Location = new System.Drawing.Point(354, 276);
            this.Delete.Name = "Delete";
            this.Delete.Size = new System.Drawing.Size(75, 32);
            this.Delete.TabIndex = 1;
            this.Delete.Text = "Clean";
            this.Delete.UseVisualStyleBackColor = true;
            this.Delete.Click += new System.EventHandler(this.Delete_Click);
            // 
            // logBox
            // 
            this.logBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.logBox.Location = new System.Drawing.Point(12, 326);
            this.logBox.Multiline = true;
            this.logBox.Name = "logBox";
            this.logBox.ReadOnly = true;
            this.logBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.logBox.Size = new System.Drawing.Size(558, 220);
            this.logBox.TabIndex = 3;
            // 
            // instBox
            // 
            this.instBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.instBox.Location = new System.Drawing.Point(12, 12);
            this.instBox.Multiline = true;
            this.instBox.Name = "instBox";
            this.instBox.ReadOnly = true;
            this.instBox.Size = new System.Drawing.Size(558, 41);
            this.instBox.TabIndex = 4;
            // 
            // instructionsLabel
            // 
            this.instructionsLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.instructionsLabel.AutoSize = true;
            this.instructionsLabel.Location = new System.Drawing.Point(32, 56);
            this.instructionsLabel.Name = "instructionsLabel";
            this.instructionsLabel.Size = new System.Drawing.Size(80, 17);
            this.instructionsLabel.TabIndex = 5;
            this.instructionsLabel.Text = "Directories:";
            // 
            // logLabel
            // 
            this.logLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.logLabel.AutoSize = true;
            this.logLabel.Location = new System.Drawing.Point(32, 303);
            this.logLabel.Name = "logLabel";
            this.logLabel.Size = new System.Drawing.Size(36, 17);
            this.logLabel.TabIndex = 6;
            this.logLabel.Text = "Log:";
            // 
            // linkBox
            // 
            this.linkBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.linkBox.AutoScroll = true;
            this.linkBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.linkBox.Location = new System.Drawing.Point(12, 76);
            this.linkBox.Name = "linkBox";
            this.linkBox.Size = new System.Drawing.Size(558, 194);
            this.linkBox.TabIndex = 8;
            this.linkBox.MouseHover += new System.EventHandler(this.LinkBox_MouseHover);
            // 
            // ServerCleanupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(582, 555);
            this.Controls.Add(this.linkBox);
            this.Controls.Add(this.logLabel);
            this.Controls.Add(this.instructionsLabel);
            this.Controls.Add(this.instBox);
            this.Controls.Add(this.logBox);
            this.Controls.Add(this.Delete);
            this.Controls.Add(this.Zip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ServerCleanupForm";
            this.Text = "Server Cleanup";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Zip;
        private System.Windows.Forms.Button Delete;
        private System.Windows.Forms.TextBox logBox;
        private System.Windows.Forms.TextBox instBox;
        private System.Windows.Forms.Label instructionsLabel;
        private System.Windows.Forms.Label logLabel;
        private System.Windows.Forms.Panel linkBox;
    }
}

