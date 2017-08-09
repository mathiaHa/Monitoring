namespace DMBoxViewer
{
    partial class FormMain
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
            this.linkLogin = new System.Windows.Forms.LinkLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.cbVerb = new System.Windows.Forms.ComboBox();
            this.btOK = new System.Windows.Forms.Button();
            this.jsonViewer = new EPocalipse.Json.Viewer.JsonViewer();
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.btCopyUrl = new System.Windows.Forms.Button();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.lblCredentialsWarning = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // linkLogin
            // 
            this.linkLogin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLogin.AutoSize = true;
            this.linkLogin.Location = new System.Drawing.Point(948, 20);
            this.linkLogin.Name = "linkLogin";
            this.linkLogin.Size = new System.Drawing.Size(33, 13);
            this.linkLogin.TabIndex = 0;
            this.linkLogin.TabStop = true;
            this.linkLogin.Text = "Login";
            this.linkLogin.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLogin_LinkClicked);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "&Verb:";
            // 
            // cbVerb
            // 
            this.cbVerb.FormattingEnabled = true;
            this.cbVerb.Items.AddRange(new object[] {
            "getewons",
            "getewon",
            "getdata",
            "syncdata",
            "delete",
            "clean"});
            this.cbVerb.Location = new System.Drawing.Point(52, 17);
            this.cbVerb.Name = "cbVerb";
            this.cbVerb.Size = new System.Drawing.Size(100, 21);
            this.cbVerb.TabIndex = 2;
            this.cbVerb.Text = "getewons";
            this.cbVerb.SelectedIndexChanged += new System.EventHandler(this.cbVerb_SelectedIndexChanged);
            // 
            // btOK
            // 
            this.btOK.Location = new System.Drawing.Point(167, 15);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(75, 23);
            this.btOK.TabIndex = 5;
            this.btOK.Text = "OK";
            this.btOK.UseVisualStyleBackColor = true;
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // jsonViewer
            // 
            this.jsonViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.jsonViewer.Json = null;
            this.jsonViewer.Location = new System.Drawing.Point(250, 148);
            this.jsonViewer.Name = "jsonViewer";
            this.jsonViewer.Size = new System.Drawing.Size(731, 504);
            this.jsonViewer.TabIndex = 9;
            // 
            // txtUrl
            // 
            this.txtUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUrl.Location = new System.Drawing.Point(250, 59);
            this.txtUrl.Multiline = true;
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.ReadOnly = true;
            this.txtUrl.Size = new System.Drawing.Size(653, 56);
            this.txtUrl.TabIndex = 7;
            // 
            // btCopyUrl
            // 
            this.btCopyUrl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btCopyUrl.Location = new System.Drawing.Point(909, 59);
            this.btCopyUrl.Name = "btCopyUrl";
            this.btCopyUrl.Size = new System.Drawing.Size(75, 23);
            this.btCopyUrl.TabIndex = 8;
            this.btCopyUrl.Text = "&Copy URL";
            this.btCopyUrl.UseVisualStyleBackColor = true;
            this.btCopyUrl.Click += new System.EventHandler(this.btCopyUrl_Click);
            // 
            // propertyGrid
            // 
            this.propertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.propertyGrid.Location = new System.Drawing.Point(16, 59);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(228, 593);
            this.propertyGrid.TabIndex = 10;
            this.propertyGrid.ToolbarVisible = false;
            // 
            // lblCredentialsWarning
            // 
            this.lblCredentialsWarning.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCredentialsWarning.Location = new System.Drawing.Point(251, 115);
            this.lblCredentialsWarning.Name = "lblCredentialsWarning";
            this.lblCredentialsWarning.Size = new System.Drawing.Size(730, 30);
            this.lblCredentialsWarning.TabIndex = 12;
            this.lblCredentialsWarning.Text = "Security good practices: credentials parameters (t2m*) are actually passed as POS" +
    "T parameters rather than URL parameters";
            this.lblCredentialsWarning.Visible = false;
            // 
            // FormMain
            // 
            this.AcceptButton = this.btOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(993, 664);
            this.Controls.Add(this.lblCredentialsWarning);
            this.Controls.Add(this.propertyGrid);
            this.Controls.Add(this.btCopyUrl);
            this.Controls.Add(this.txtUrl);
            this.Controls.Add(this.jsonViewer);
            this.Controls.Add(this.btOK);
            this.Controls.Add(this.cbVerb);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.linkLogin);
            this.Name = "FormMain";
            this.Text = "Talk2M DataMailbox Viewer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel linkLogin;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbVerb;
        private System.Windows.Forms.Button btOK;
        private EPocalipse.Json.Viewer.JsonViewer jsonViewer;
        private System.Windows.Forms.TextBox txtUrl;
        private System.Windows.Forms.Button btCopyUrl;
        private System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.Label lblCredentialsWarning;
    }
}

