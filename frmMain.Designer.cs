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
            tabSystemControl = new TabControl();
            tabPage1 = new TabPage();
            labY = new Label();
            labX = new Label();
            labCoordinate = new Label();
            pnlMainDisplay = new Panel();
            tabPage2 = new TabPage();
            lvImages = new ListView();
            btnImagePath = new Button();
            txtImagePath = new TextBox();
            labImagePath = new Label();
            pnlToolBar = new Panel();
            btnLoadImage = new Button();
            btnRGB2BW = new Button();
            btn2Gray = new Button();
            btnAddROI = new Button();
            btnCircleROI = new Button();
            btnVerticalLine = new Button();
            btnHorizontalLine = new Button();
            radBtnBlack = new RadioButton();
            radBtnWhite = new RadioButton();
            labTrackValue = new Label();
            tackBWTh = new TrackBar();
            btnBlob = new Button();
            tabSystemControl.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            pnlToolBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)tackBWTh).BeginInit();
            SuspendLayout();
            // 
            // tabSystemControl
            // 
            tabSystemControl.Controls.Add(tabPage1);
            tabSystemControl.Controls.Add(tabPage2);
            tabSystemControl.Location = new Point(12, 12);
            tabSystemControl.Name = "tabSystemControl";
            tabSystemControl.SelectedIndex = 0;
            tabSystemControl.Size = new Size(1430, 477);
            tabSystemControl.TabIndex = 16;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(pnlToolBar);
            tabPage1.Controls.Add(labY);
            tabPage1.Controls.Add(labX);
            tabPage1.Controls.Add(labCoordinate);
            tabPage1.Controls.Add(pnlMainDisplay);
            tabPage1.Location = new Point(4, 28);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(1422, 445);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "CCD Capture";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // labY
            // 
            labY.AutoSize = true;
            labY.Font = new Font("Microsoft JhengHei UI", 12F);
            labY.Location = new Point(-186, 242);
            labY.Name = "labY";
            labY.Size = new Size(41, 25);
            labY.TabIndex = 15;
            labY.Text = "Y:0";
            // 
            // labX
            // 
            labX.AutoSize = true;
            labX.Font = new Font("Microsoft JhengHei UI", 12F);
            labX.Location = new Point(-308, 242);
            labX.Name = "labX";
            labX.Size = new Size(42, 25);
            labX.TabIndex = 14;
            labX.Text = "X:0";
            // 
            // labCoordinate
            // 
            labCoordinate.AutoSize = true;
            labCoordinate.Font = new Font("Microsoft JhengHei UI", 12F);
            labCoordinate.Location = new Point(-308, 217);
            labCoordinate.Name = "labCoordinate";
            labCoordinate.Size = new Size(128, 25);
            labCoordinate.TabIndex = 13;
            labCoordinate.Text = "coordinates:";
            // 
            // pnlMainDisplay
            // 
            pnlMainDisplay.Location = new Point(-308, 103);
            pnlMainDisplay.Name = "pnlMainDisplay";
            pnlMainDisplay.Size = new Size(1724, 346);
            pnlMainDisplay.TabIndex = 12;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(lvImages);
            tabPage2.Controls.Add(btnImagePath);
            tabPage2.Controls.Add(txtImagePath);
            tabPage2.Controls.Add(labImagePath);
            tabPage2.Location = new Point(4, 28);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(1174, 445);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "System Setting";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // lvImages
            // 
            lvImages.Location = new Point(18, 69);
            lvImages.Name = "lvImages";
            lvImages.Size = new Size(547, 279);
            lvImages.TabIndex = 3;
            lvImages.UseCompatibleStateImageBehavior = false;
            // 
            // btnImagePath
            // 
            btnImagePath.Font = new Font("Microsoft JhengHei UI", 12F);
            btnImagePath.Location = new Point(420, 22);
            btnImagePath.Name = "btnImagePath";
            btnImagePath.Size = new Size(145, 30);
            btnImagePath.TabIndex = 2;
            btnImagePath.Text = "Set Path";
            btnImagePath.UseVisualStyleBackColor = true;
            btnImagePath.Click += btnImagePath_Click;
            // 
            // txtImagePath
            // 
            txtImagePath.Font = new Font("Microsoft JhengHei UI", 12F);
            txtImagePath.Location = new Point(139, 19);
            txtImagePath.Name = "txtImagePath";
            txtImagePath.Size = new Size(261, 33);
            txtImagePath.TabIndex = 1;
            txtImagePath.Text = "images Path";
            // 
            // labImagePath
            // 
            labImagePath.AutoSize = true;
            labImagePath.Font = new Font("Microsoft JhengHei UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labImagePath.Location = new Point(6, 21);
            labImagePath.Name = "labImagePath";
            labImagePath.Size = new Size(127, 25);
            labImagePath.TabIndex = 0;
            labImagePath.Text = "Image Path:";
            // 
            // pnlToolBar
            // 
            pnlToolBar.Controls.Add(btnBlob);
            pnlToolBar.Controls.Add(radBtnBlack);
            pnlToolBar.Controls.Add(radBtnWhite);
            pnlToolBar.Controls.Add(labTrackValue);
            pnlToolBar.Controls.Add(tackBWTh);
            pnlToolBar.Controls.Add(btnVerticalLine);
            pnlToolBar.Controls.Add(btnHorizontalLine);
            pnlToolBar.Controls.Add(btnCircleROI);
            pnlToolBar.Controls.Add(btnRGB2BW);
            pnlToolBar.Controls.Add(btn2Gray);
            pnlToolBar.Controls.Add(btnAddROI);
            pnlToolBar.Controls.Add(btnLoadImage);
            pnlToolBar.Location = new Point(0, 0);
            pnlToolBar.Name = "pnlToolBar";
            pnlToolBar.Size = new Size(1416, 105);
            pnlToolBar.TabIndex = 16;
            // 
            // btnLoadImage
            // 
            btnLoadImage.Font = new Font("Microsoft JhengHei UI", 12F);
            btnLoadImage.ImageAlign = ContentAlignment.MiddleLeft;
            btnLoadImage.Location = new Point(19, 16);
            btnLoadImage.Name = "btnLoadImage";
            btnLoadImage.Size = new Size(94, 72);
            btnLoadImage.TabIndex = 28;
            btnLoadImage.Text = "Load Image";
            btnLoadImage.UseVisualStyleBackColor = true;
            btnLoadImage.Click += btnLoadImage_Click;
            // 
            // btnRGB2BW
            // 
            btnRGB2BW.Font = new Font("Microsoft JhengHei UI", 12F);
            btnRGB2BW.ImageAlign = ContentAlignment.MiddleLeft;
            btnRGB2BW.Location = new Point(463, 16);
            btnRGB2BW.Name = "btnRGB2BW";
            btnRGB2BW.Size = new Size(94, 72);
            btnRGB2BW.TabIndex = 33;
            btnRGB2BW.Text = "ConvertToBW";
            btnRGB2BW.UseVisualStyleBackColor = true;
            btnRGB2BW.Click += btnRGB2BW_Click;
            // 
            // btn2Gray
            // 
            btn2Gray.Font = new Font("Microsoft JhengHei UI", 12F);
            btn2Gray.ImageAlign = ContentAlignment.MiddleLeft;
            btn2Gray.Location = new Point(363, 16);
            btn2Gray.Name = "btn2Gray";
            btn2Gray.Size = new Size(94, 72);
            btn2Gray.TabIndex = 32;
            btn2Gray.Text = "ConvertToGray";
            btn2Gray.UseVisualStyleBackColor = true;
            btn2Gray.Click += btn2Gray_Click;
            // 
            // btnAddROI
            // 
            btnAddROI.Font = new Font("Microsoft JhengHei UI", 8F);
            btnAddROI.ImageAlign = ContentAlignment.MiddleLeft;
            btnAddROI.Location = new Point(135, 16);
            btnAddROI.Name = "btnAddROI";
            btnAddROI.Size = new Size(94, 72);
            btnAddROI.TabIndex = 31;
            btnAddROI.Text = "Attach Rectangle ROI";
            btnAddROI.UseVisualStyleBackColor = true;
            btnAddROI.Click += btnAddROI_Click;
            // 
            // btnCircleROI
            // 
            btnCircleROI.Font = new Font("Microsoft JhengHei UI", 8F);
            btnCircleROI.ImageAlign = ContentAlignment.MiddleLeft;
            btnCircleROI.Location = new Point(251, 16);
            btnCircleROI.Name = "btnCircleROI";
            btnCircleROI.Size = new Size(94, 72);
            btnCircleROI.TabIndex = 34;
            btnCircleROI.Text = "Attach Circle ROI";
            btnCircleROI.UseVisualStyleBackColor = true;
            btnCircleROI.Click += btnCircleROI_Click;
            // 
            // btnVerticalLine
            // 
            btnVerticalLine.Font = new Font("Microsoft JhengHei UI", 12F);
            btnVerticalLine.Image = Properties.Resources.veritcal;
            btnVerticalLine.Location = new Point(667, 16);
            btnVerticalLine.Name = "btnVerticalLine";
            btnVerticalLine.Size = new Size(94, 72);
            btnVerticalLine.TabIndex = 40;
            btnVerticalLine.Text = "Vertical Line";
            btnVerticalLine.UseVisualStyleBackColor = true;
            btnVerticalLine.Click += btnVeritalLine_Click;
            // 
            // btnHorizontalLine
            // 
            btnHorizontalLine.Font = new Font("Microsoft JhengHei UI", 12F);
            btnHorizontalLine.Image = Properties.Resources.horizontal;
            btnHorizontalLine.Location = new Point(567, 16);
            btnHorizontalLine.Name = "btnHorizontalLine";
            btnHorizontalLine.Size = new Size(94, 72);
            btnHorizontalLine.TabIndex = 39;
            btnHorizontalLine.Text = "Horizontal Line";
            btnHorizontalLine.TextAlign = ContentAlignment.BottomCenter;
            btnHorizontalLine.UseVisualStyleBackColor = true;
            btnHorizontalLine.Click += btnMeasureLine_Click;
            // 
            // radBtnBlack
            // 
            radBtnBlack.AutoSize = true;
            radBtnBlack.Location = new Point(912, 57);
            radBtnBlack.Name = "radBtnBlack";
            radBtnBlack.Size = new Size(101, 23);
            radBtnBlack.TabIndex = 44;
            radBtnBlack.TabStop = true;
            radBtnBlack.Text = "Blob Black";
            radBtnBlack.UseVisualStyleBackColor = true;
            // 
            // radBtnWhite
            // 
            radBtnWhite.AutoSize = true;
            radBtnWhite.Location = new Point(912, 26);
            radBtnWhite.Name = "radBtnWhite";
            radBtnWhite.Size = new Size(106, 23);
            radBtnWhite.TabIndex = 43;
            radBtnWhite.TabStop = true;
            radBtnWhite.Text = "Blob White";
            radBtnWhite.UseVisualStyleBackColor = true;
            // 
            // labTrackValue
            // 
            labTrackValue.AutoSize = true;
            labTrackValue.Location = new Point(1037, 64);
            labTrackValue.Name = "labTrackValue";
            labTrackValue.Size = new Size(18, 19);
            labTrackValue.TabIndex = 42;
            labTrackValue.Text = "0";
            // 
            // tackBWTh
            // 
            tackBWTh.Location = new Point(1019, 24);
            tackBWTh.Maximum = 255;
            tackBWTh.Name = "tackBWTh";
            tackBWTh.Size = new Size(130, 56);
            tackBWTh.TabIndex = 41;
            tackBWTh.Value = 128;
            tackBWTh.Scroll += tackBWTh_ValueChanged;
            // 
            // btnBlob
            // 
            btnBlob.Font = new Font("Microsoft JhengHei UI", 12F);
            btnBlob.ImageAlign = ContentAlignment.MiddleLeft;
            btnBlob.Location = new Point(790, 16);
            btnBlob.Name = "btnBlob";
            btnBlob.Size = new Size(94, 72);
            btnBlob.TabIndex = 45;
            btnBlob.Text = "Blob";
            btnBlob.UseVisualStyleBackColor = true;
            btnBlob.Click += btnBlob_Click;
            // 
            // btnMeasureLine
            // 
            btnMeasureLine.Font = new Font("Microsoft JhengHei UI", 12F);
            btnMeasureLine.ImageAlign = ContentAlignment.MiddleLeft;
            btnMeasureLine.Location = new Point(851, 288);
            btnMeasureLine.Name = "btnMeasureLine";
            btnMeasureLine.Size = new Size(94, 72);
            btnMeasureLine.TabIndex = 14;
            btnMeasureLine.Text = "Draw Line";
            btnMeasureLine.UseVisualStyleBackColor = true;
            btnMeasureLine.Click += btnMeasureLine_Click;
            // 
            // frmMain
            // 
            AutoScaleDimensions = new SizeF(9F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1454, 489);
            Controls.Add(tabSystemControl);
            Name = "frmMain";
            Text = "frmMain";
            FormClosed += frmMain_FormClosed;
            tabSystemControl.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            pnlToolBar.ResumeLayout(false);
            pnlToolBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)tackBWTh).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabSystemControl;
        private TabPage tabPage1;
        private Label labY;
        private Label labX;
        private Label labCoordinate;
        private Panel pnlMainDisplay;
        private TabPage tabPage2;
        private Button btnImagePath;
        private TextBox txtImagePath;
        private Label labImagePath;
        private ListView lvImages;
        private Panel pnlToolBar;
        private Button btnBlob;
        private RadioButton radBtnBlack;
        private RadioButton radBtnWhite;
        private Label labTrackValue;
        private TrackBar tackBWTh;
        private Button btnVerticalLine;
        private Button btnHorizontalLine;
        private Button btnCircleROI;
        private Button btnRGB2BW;
        private Button btn2Gray;
        private Button btnAddROI;
        private Button btnLoadImage;
    }
}
