namespace ClientValidator
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.ElementList = new System.Windows.Forms.ListView();
            this.IDCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.NameCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Orient = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.AngluarVel = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ZoomOut = new System.Windows.Forms.Button();
            this.ZoomIn = new System.Windows.Forms.Button();
            this.StartHost = new System.Windows.Forms.Button();
            this.ViewType = new System.Windows.Forms.ComboBox();
            this.StatusText = new System.Windows.Forms.TextBox();
            this.ConnectButton = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.TurnRight = new System.Windows.Forms.Button();
            this.TurnLeft = new System.Windows.Forms.Button();
            this.Backwards = new System.Windows.Forms.Button();
            this.Forward = new System.Windows.Forms.Button();
            this.Map = new System.Windows.Forms.PictureBox();
            this.StatusLabel = new System.Windows.Forms.Label();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Map)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.StatusLabel);
            this.splitContainer1.Panel1.Controls.Add(this.TurnRight);
            this.splitContainer1.Panel1.Controls.Add(this.TurnLeft);
            this.splitContainer1.Panel1.Controls.Add(this.Backwards);
            this.splitContainer1.Panel1.Controls.Add(this.Forward);
            this.splitContainer1.Panel1.Controls.Add(this.ElementList);
            this.splitContainer1.Panel1.Controls.Add(this.ZoomOut);
            this.splitContainer1.Panel1.Controls.Add(this.ZoomIn);
            this.splitContainer1.Panel1.Controls.Add(this.StartHost);
            this.splitContainer1.Panel1.Controls.Add(this.ViewType);
            this.splitContainer1.Panel1.Controls.Add(this.StatusText);
            this.splitContainer1.Panel1.Controls.Add(this.ConnectButton);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.Map);
            this.splitContainer1.Size = new System.Drawing.Size(1213, 589);
            this.splitContainer1.SplitterDistance = 213;
            this.splitContainer1.TabIndex = 0;
            // 
            // ElementList
            // 
            this.ElementList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ElementList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.IDCol,
            this.NameCol,
            this.Orient,
            this.AngluarVel});
            this.ElementList.Location = new System.Drawing.Point(12, 148);
            this.ElementList.Name = "ElementList";
            this.ElementList.Size = new System.Drawing.Size(189, 189);
            this.ElementList.TabIndex = 7;
            this.ElementList.UseCompatibleStateImageBehavior = false;
            this.ElementList.View = System.Windows.Forms.View.Details;
            // 
            // IDCol
            // 
            this.IDCol.Text = "ID";
            this.IDCol.Width = 26;
            // 
            // NameCol
            // 
            this.NameCol.Text = "Name";
            this.NameCol.Width = 51;
            // 
            // Orient
            // 
            this.Orient.Text = "Orientation";
            this.Orient.Width = 54;
            // 
            // AngluarVel
            // 
            this.AngluarVel.Text = "Angular Velocity";
            // 
            // ZoomOut
            // 
            this.ZoomOut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ZoomOut.Location = new System.Drawing.Point(44, 343);
            this.ZoomOut.Name = "ZoomOut";
            this.ZoomOut.Size = new System.Drawing.Size(31, 23);
            this.ZoomOut.TabIndex = 6;
            this.ZoomOut.Text = "-";
            this.ZoomOut.UseVisualStyleBackColor = true;
            this.ZoomOut.Click += new System.EventHandler(this.ZoomOut_Click);
            // 
            // ZoomIn
            // 
            this.ZoomIn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ZoomIn.Location = new System.Drawing.Point(7, 343);
            this.ZoomIn.Name = "ZoomIn";
            this.ZoomIn.Size = new System.Drawing.Size(31, 23);
            this.ZoomIn.TabIndex = 5;
            this.ZoomIn.Text = "+";
            this.ZoomIn.UseVisualStyleBackColor = true;
            this.ZoomIn.Click += new System.EventHandler(this.ZoomIn_Click);
            // 
            // StartHost
            // 
            this.StartHost.Location = new System.Drawing.Point(93, 12);
            this.StartHost.Name = "StartHost";
            this.StartHost.Size = new System.Drawing.Size(75, 23);
            this.StartHost.TabIndex = 4;
            this.StartHost.Text = "Host";
            this.StartHost.UseVisualStyleBackColor = true;
            this.StartHost.Click += new System.EventHandler(this.StartHost_Click);
            // 
            // ViewType
            // 
            this.ViewType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ViewType.FormattingEnabled = true;
            this.ViewType.Items.AddRange(new object[] {
            "World (last update)",
            "World (extrapolated)",
            "Ship Centric"});
            this.ViewType.Location = new System.Drawing.Point(12, 121);
            this.ViewType.Name = "ViewType";
            this.ViewType.Size = new System.Drawing.Size(155, 21);
            this.ViewType.TabIndex = 3;
            this.ViewType.SelectedIndexChanged += new System.EventHandler(this.ViewType_SelectedIndexChanged);
            // 
            // StatusText
            // 
            this.StatusText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.StatusText.Location = new System.Drawing.Point(13, 42);
            this.StatusText.Multiline = true;
            this.StatusText.Name = "StatusText";
            this.StatusText.ReadOnly = true;
            this.StatusText.Size = new System.Drawing.Size(158, 73);
            this.StatusText.TabIndex = 1;
            // 
            // ConnectButton
            // 
            this.ConnectButton.Location = new System.Drawing.Point(12, 12);
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new System.Drawing.Size(75, 23);
            this.ConnectButton.TabIndex = 0;
            this.ConnectButton.Text = "Connect";
            this.ConnectButton.UseVisualStyleBackColor = true;
            this.ConnectButton.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "SS1.png");
            // 
            // TurnRight
            // 
            this.TurnRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.TurnRight.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.TurnRight.Image = global::ClientValidator.Properties.Resources._1rightarrow;
            this.TurnRight.Location = new System.Drawing.Point(93, 418);
            this.TurnRight.Name = "TurnRight";
            this.TurnRight.Size = new System.Drawing.Size(23, 43);
            this.TurnRight.TabIndex = 11;
            this.TurnRight.UseVisualStyleBackColor = true;
            this.TurnRight.Click += new System.EventHandler(this.Right_Click);
            // 
            // TurnLeft
            // 
            this.TurnLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.TurnLeft.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.TurnLeft.Image = global::ClientValidator.Properties.Resources._1leftarrow;
            this.TurnLeft.Location = new System.Drawing.Point(15, 418);
            this.TurnLeft.Name = "TurnLeft";
            this.TurnLeft.Size = new System.Drawing.Size(23, 43);
            this.TurnLeft.TabIndex = 10;
            this.TurnLeft.UseVisualStyleBackColor = true;
            this.TurnLeft.Click += new System.EventHandler(this.Left_Click);
            // 
            // Backwards
            // 
            this.Backwards.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Backwards.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Backwards.Image = global::ClientValidator.Properties.Resources._1downarrow1;
            this.Backwards.Location = new System.Drawing.Point(44, 454);
            this.Backwards.Name = "Backwards";
            this.Backwards.Size = new System.Drawing.Size(43, 23);
            this.Backwards.TabIndex = 9;
            this.Backwards.UseVisualStyleBackColor = true;
            this.Backwards.Click += new System.EventHandler(this.Backwards_Click);
            // 
            // Forward
            // 
            this.Forward.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Forward.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Forward.Image = global::ClientValidator.Properties.Resources._1downarrow;
            this.Forward.Location = new System.Drawing.Point(44, 404);
            this.Forward.Name = "Forward";
            this.Forward.Size = new System.Drawing.Size(43, 23);
            this.Forward.TabIndex = 8;
            this.Forward.UseVisualStyleBackColor = true;
            this.Forward.Click += new System.EventHandler(this.Forward_Click);
            // 
            // Map
            // 
            this.Map.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Map.Location = new System.Drawing.Point(0, 0);
            this.Map.Name = "Map";
            this.Map.Size = new System.Drawing.Size(996, 589);
            this.Map.TabIndex = 0;
            this.Map.TabStop = false;
            this.Map.Paint += new System.Windows.Forms.PaintEventHandler(this.Map_Paint);
            // 
            // StatusLabel
            // 
            this.StatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.StatusLabel.Location = new System.Drawing.Point(13, 384);
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(188, 17);
            this.StatusLabel.TabIndex = 12;
            this.StatusLabel.Text = "ShipStatus:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1213, 589);
            this.Controls.Add(this.splitContainer1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Map)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button ConnectButton;
        private System.Windows.Forms.TextBox StatusText;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.PictureBox Map;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ComboBox ViewType;
		private System.Windows.Forms.Button StartHost;
		private System.Windows.Forms.Button ZoomOut;
		private System.Windows.Forms.Button ZoomIn;
		private System.Windows.Forms.ListView ElementList;
		private System.Windows.Forms.ColumnHeader IDCol;
		private System.Windows.Forms.ColumnHeader NameCol;
		private System.Windows.Forms.ColumnHeader Orient;
		private System.Windows.Forms.ColumnHeader AngluarVel;
        private System.Windows.Forms.Button Forward;
        private System.Windows.Forms.Button Backwards;
        private System.Windows.Forms.Button TurnLeft;
        private System.Windows.Forms.Button TurnRight;
        private System.Windows.Forms.Label StatusLabel;
    }
}

