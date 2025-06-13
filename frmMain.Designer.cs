namespace ImageProcess
{
    partial class frmMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pnlMainDisplay = new Panel();
            labCoordinate = new Label();
            labX = new Label();
            labY = new Label();
            btnLoadImage = new Button();
            btnAddROI = new Button();
            btn2Gray = new Button();
            btnRGB2BW = new Button();
            tackBWTh = new TrackBar();
            ((System.ComponentModel.ISupportInitialize)tackBWTh).BeginInit();
            SuspendLayout();
            // 
            // pnlMainDisplay
            // 
            pnlMainDisplay.Location = new Point(3, 1);
            pnlMainDisplay.Name = "pnlMainDisplay";
            pnlMainDisplay.Size = new Size(805, 362);
            pnlMainDisplay.TabIndex = 0;
            pnlMainDisplay.MouseMove += pnlMainDisplay_MouseMove;
            // 
            // labCoordinate
            // 
            labCoordinate.AutoSize = true;
            labCoordinate.Font = new Font("Microsoft JhengHei UI", 12F);
            labCoordinate.Location = new Point(3, 366);
            labCoordinate.Name = "labCoordinate";
            labCoordinate.Size = new Size(128, 25);
            labCoordinate.TabIndex = 2;
            labCoordinate.Text = "coordinates:";
            // 
            // labX
            // 
            labX.AutoSize = true;
            labX.Font = new Font("Microsoft JhengHei UI", 12F);
            labX.Location = new Point(3, 391);
            labX.Name = "labX";
            labX.Size = new Size(42, 25);
            labX.TabIndex = 3;
            labX.Text = "X:0";
            // 
            // labY
            // 
            labY.AutoSize = true;
            labY.Font = new Font("Microsoft JhengHei UI", 12F);
            labY.Location = new Point(125, 391);
            labY.Name = "labY";
            labY.Size = new Size(41, 25);
            labY.TabIndex = 4;
            labY.Text = "Y:0";
            // 
            // btnLoadImage
            // 
            btnLoadImage.Font = new Font("Microsoft JhengHei UI", 12F);
            btnLoadImage.ImageAlign = ContentAlignment.MiddleLeft;
            btnLoadImage.Location = new Point(242, 366);
            btnLoadImage.Name = "btnLoadImage";
            btnLoadImage.Size = new Size(94, 72);
            btnLoadImage.TabIndex = 0;
            btnLoadImage.Text = "Load Image";
            btnLoadImage.UseVisualStyleBackColor = true;
            btnLoadImage.Click += btnLoadImage_Click;
            // 
            // btnAddROI
            // 
            btnAddROI.Font = new Font("Microsoft JhengHei UI", 12F);
            btnAddROI.ImageAlign = ContentAlignment.MiddleLeft;
            btnAddROI.Location = new Point(359, 369);
            btnAddROI.Name = "btnAddROI";
            btnAddROI.Size = new Size(94, 72);
            btnAddROI.TabIndex = 5;
            btnAddROI.Text = "Attach ROI";
            btnAddROI.UseVisualStyleBackColor = true;
            btnAddROI.Click += btnAddROI_Click;
            // 
            // btn2Gray
            // 
            btn2Gray.Font = new Font("Microsoft JhengHei UI", 12F);
            btn2Gray.ImageAlign = ContentAlignment.MiddleLeft;
            btn2Gray.Location = new Point(472, 369);
            btn2Gray.Name = "btn2Gray";
            btn2Gray.Size = new Size(94, 72);
            btn2Gray.TabIndex = 6;
            btn2Gray.Text = "ConvertToGray";
            btn2Gray.UseVisualStyleBackColor = true;
            btn2Gray.Click += btn2Gray_Click;
            // 
            // btnRGB2BW
            // 
            btnRGB2BW.Font = new Font("Microsoft JhengHei UI", 12F);
            btnRGB2BW.ImageAlign = ContentAlignment.MiddleLeft;
            btnRGB2BW.Location = new Point(572, 369);
            btnRGB2BW.Name = "btnRGB2BW";
            btnRGB2BW.Size = new Size(94, 72);
            btnRGB2BW.TabIndex = 7;
            btnRGB2BW.Text = "ConvertToBW";
            btnRGB2BW.UseVisualStyleBackColor = true;
            btnRGB2BW.Click += btnRGB2BW_Click;
            // 
            // tackBWTh
            // 
            tackBWTh.Location = new Point(699, 382);
            tackBWTh.Maximum = 255;
            tackBWTh.Name = "tackBWTh";
            tackBWTh.Size = new Size(130, 56);
            tackBWTh.TabIndex = 8;
            tackBWTh.Value = 128;
            tackBWTh.ValueChanged += tackBWTh_ValueChanged;
            // 
            // frmMain
            // 
            AutoScaleDimensions = new SizeF(9F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1020, 489);
            Controls.Add(tackBWTh);
            Controls.Add(btnRGB2BW);
            Controls.Add(btn2Gray);
            Controls.Add(btnAddROI);
            Controls.Add(btnLoadImage);
            Controls.Add(labY);
            Controls.Add(labX);
            Controls.Add(labCoordinate);
            Controls.Add(pnlMainDisplay);
            Name = "frmMain";
            Text = "frmMain";
            ((System.ComponentModel.ISupportInitialize)tackBWTh).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel pnlMainDisplay;
        private Label labCoordinate;
        private Label labX;
        private Label labY;
        private Button btnLoadImage;
        private Button btnAddROI;
        private Button btn2Gray;
        private Button btnRGB2BW;
        private TrackBar tackBWTh;
    }
}
