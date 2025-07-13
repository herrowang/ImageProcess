
using RoomCV;
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ImageProcess
{
    public partial class frmMain : Form
    {
        private delegate void update_screen(Bitmap srcimg);
        private delegate void ReflashDrawing(TROI roi);
        private Image srcImg;
        private Bitmap bmpSrcImg;
        private PictureBox picLoadImg;
        private bool bBeginDrag;
        private MouseCrossDirectionType directType;
        private List<TROI> rects;
        private ROIACTType roiAct;
        private ImgLib imgProc;
        Point pCenter;
        Rectangle rect;
        TROI gROI;
        Pen pen;
        public frmMain()
        {
            InitializeComponent();
            initial();
        }

        private void initial()
        {
            this.Text = "frmMain";
            this.Width = Screen.PrimaryScreen.Bounds.Width;
            this.Height = Screen.PrimaryScreen.Bounds.Height;
            this.Top = 0;
            this.Left = 0;
            this.DoubleBuffered = true;
            bBeginDrag = false;
            imgProc = null;
            picLoadImg = new PictureBox();
            picLoadImg.Left = this.Left + 20;
            picLoadImg.Top = this.Top + 20;
            picLoadImg.MouseDown += new MouseEventHandler(pictureOnMouseDown);
            picLoadImg.MouseMove += new MouseEventHandler(pictureOnMouseMove);
            picLoadImg.MouseUp += new MouseEventHandler(pictureOnMouseUp);
            picLoadImg.Paint += new PaintEventHandler(picLoadImgOnPaint);
            picLoadImg.DragEnter += new DragEventHandler(pictureMouseDragEnter);
            picLoadImg.DragOver += new DragEventHandler(pictureMouseDragOver);
            pnlMainDisplay.Top = this.Top;
            pnlMainDisplay.Left = this.Left;
            pnlMainDisplay.Width = 1290;
            pnlMainDisplay.Height = 720;
            pnlMainDisplay.BackColor = Color.White;
            labCoordinate.Top = pnlMainDisplay.Bottom + 10;
            labCoordinate.Left = pnlMainDisplay.Left;
            labCoordinate.BackColor = Color.Red;
            labX.Left = labCoordinate.Left;
            labX.Top = labCoordinate.Bottom + 10;
            labX.Width = labCoordinate.Width;
            labY.Top = labX.Top;
            labY.Left = labX.Right + 30;
            labY.Width = labCoordinate.Width;
            btnLoadImage.Top = this.Top;
            btnLoadImage.Left = pnlMainDisplay.Right + 30;
            btnAddROI.Top = btnLoadImage.Top;
            btnAddROI.Left = btnLoadImage.Right + 30;
            btnCircleROI.Top = btnAddROI.Top;
            btnCircleROI.Left = btnAddROI.Right + 30;
            btn2Gray.Top = btnCircleROI.Top;
            btn2Gray.Left = btnCircleROI.Right + 30;
            btnRGB2BW.Top = btn2Gray.Top;
            btnRGB2BW.Left = btn2Gray.Right + 30;
            tackBWTh.Top = btnLoadImage.Bottom + 20; ;
            tackBWTh.Left = btnLoadImage.Left;
            tackBWTh.Width = 200;
            labTrackValue.Top = tackBWTh.Bottom + 10;
            labTrackValue.Left = tackBWTh.Left;
            labTrackValue.Width = tackBWTh.Width;
            labTrackValue.Text = tackBWTh.Value.ToString();
            btnBlob.Top = btnAddROI.Bottom + 10;
            btnBlob.Left = tackBWTh.Right + 10;
            btnBlob.Width = btnAddROI.Width;
            btnBlob.Height = btnAddROI.Height;
            btnMeasureLine.Left = btnRGB2BW.Left;
            btnMeasureLine.Top = btnRGB2BW.Bottom + 10;
            btnMeasureLine.Width = btnRGB2BW.Width;
            btnMeasureLine.Height = btnRGB2BW.Height;
            radBtnWhite.Top = btnBlob.Top;
            radBtnWhite.Left = btnBlob.Right + 10;
            radBtnBlack.Top = radBtnWhite.Bottom + 10;
            radBtnBlack.Left = radBtnWhite.Left;
            rects = new List<TROI>();
        }
        private void pictureMouseDragEnter(object sender, DragEventArgs e)
        {
            if (e.X >= gROI.GetRegion().X
                && e.X <= gROI.GetRegion().Right
                && e.Y <= gROI.GetRegion().Bottom
                && e.Y >= gROI.GetRegion().Top)
            {
                roiAct = ROIACTType.Attach;
                pCenter.X = e.X;
                pCenter.Y = e.Y;
                rect = new Rectangle(pCenter.X - rect.Width / 2, pCenter.Y - rect.Height / 2, rect.Width, rect.Height);
                picLoadImg.Invalidate();
            }
        }

        private void pictureMouseDragOver(object sender, DragEventArgs e)
        {
            if (e.X >= gROI.GetRegion().X
                && e.X <= gROI.GetRegion().Right
                && e.Y <= gROI.GetRegion().Bottom
                && e.Y >= gROI.GetRegion().Top)
            {
                roiAct = ROIACTType.Attach;
                pCenter.X = e.X;
                pCenter.Y = e.Y;
                rect = new Rectangle(pCenter.X - rect.Width / 2, pCenter.Y - rect.Height / 2, rect.Width, rect.Height);
                picLoadImg.Invalidate();
            }
        }
        private void pictureOnMouseDown(object sender, MouseEventArgs e)
        {
            PictureBox p = (PictureBox)sender;

            if (p != null)
            {
                switch (gROI.GetROIType())
                {
                    case ROIType.MeasureLine:
                        gROI.SetStartPosition(new Point(e.X, e.Y));
                        gROI.SetEndPosition(new Point(e.X, e.Y));
                        break;
                    default:
                        if ((Math.Abs(e.X - pCenter.X) <= 20) && (Math.Abs(e.Y - pCenter.Y) <= 20))
                        {
                            if (bBeginDrag == true && e.Button == MouseButtons.Left)
                            {
                                pCenter.X = e.X;
                                pCenter.Y = e.Y;
                                rect.X = (pCenter.X - rect.Width / 2);
                                rect.Y = (pCenter.Y - rect.Height / 2);

                            }
                        }
                        else
                        {
                            Cursor = Cursors.Arrow;
                        }
                        break;
                }
            }
        }

        private void pictureOnMouseUp(object sender, MouseEventArgs e)
        {
            PictureBox p = (PictureBox)sender;

            if (p != null)
            {
                if (gROI == null)
                {
                    return;
                }

                switch (gROI.GetROIType())
                {
                    case ROIType.MeasureLine:
                        gROI.SetEndPosition(new Point(e.X, e.Y));
                        gROI.SetRegionPos(gROI.GetLine());
                        break;
                    default:
                        if ((Math.Abs(e.X - pCenter.X) <= 20) && (Math.Abs(e.Y - pCenter.Y) <= 20))
                        {
                            if (bBeginDrag == true && e.Button == MouseButtons.Left)
                            {
                                pCenter.X = e.X;
                                pCenter.Y = e.Y;
                                rect.X = (pCenter.X - rect.Width / 2);
                                rect.Y = (pCenter.Y - rect.Height / 2);

                            }
                        }
                        else
                        {
                            Cursor = Cursors.Arrow;
                        }
                        break;
                }
            }
            bBeginDrag = false;
        }

        private void pictureOnMouseMove(object sender, MouseEventArgs e)
        {
            PictureBox p = (PictureBox)sender;

            if (p != null)
            {
                if (gROI == null)
                {
                    return;
                }

                if (gROI != null
                    && (e.X >= gROI.GetRegion().X
                    && e.X <= gROI.GetRegion().Right
                    && e.Y <= gROI.GetRegion().Bottom
                    && e.Y >= gROI.GetRegion().Top))
                {
                    if (roiAct != ROIACTType.DrawLine)
                    {
                        roiAct = ROIACTType.Attach;
                    }
                }

                if ((e.X >= picLoadImg.Left && e.Y >= picLoadImg.Top)
                && (e.X <= picLoadImg.Right && e.Y <= picLoadImg.Bottom))
                {
                    labX.Text = "X:" + e.X.ToString();
                    labY.Text = "Y:" + e.Y.ToString();
                }

                Cursor = gROI.GetMouseCursor(new Point(e.X, e.Y), gROI);

                if (e.Button == MouseButtons.Left)
                {
                    gROI.Move(new Point(e.X, e.Y), gROI);
                    rect = gROI.GetRegion();
                }
            }
        }

        private void pnlMainDisplay_MouseMove(object sender, MouseEventArgs e)
        {
            Panel p = (Panel)sender;

            if (p != null)
            {
                if ((e.X >= pnlMainDisplay.Left && e.Y >= pnlMainDisplay.Top)
                    && (e.X <= pnlMainDisplay.Right && e.Y <= pnlMainDisplay.Bottom))
                {
                    labX.Text = "X:" + e.X.ToString();
                    labY.Text = "Y:" + e.Y.ToString();
                }

                if ((Math.Abs(e.X - pCenter.X) <= 10) && (Math.Abs(e.Y - pCenter.Y) <= 10))
                {
                    Cursor = Cursors.Cross;
                }
                else
                {
                    Cursor = Cursors.Arrow;
                }
            }
        }

        public Bitmap ConvertToBitmap(string fileName)
        {
            Bitmap bitmap;
            using (Stream bmpStream = System.IO.File.Open(fileName, System.IO.FileMode.Open))
            {
                Image image = Image.FromStream(bmpStream);

                bitmap = new Bitmap(image);

            }
            return bitmap;
        }

        private void btnLoadImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Title = "Load Images";
            openFile.Filter = "All files (*.*)|*.*|jpeg file (*.jpg)|*.jpg|bmp file (*.bmp)|*.bmp";

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                bmpSrcImg = ConvertToBitmap(openFile.FileName);
                picLoadImg.Image = bmpSrcImg;
                picLoadImg.Top = this.Top;
                picLoadImg.Left = this.Left;
                picLoadImg.Width = bmpSrcImg.Width;
                picLoadImg.Height = bmpSrcImg.Height;
                pnlMainDisplay.Controls.Add(picLoadImg);
                pnlMainDisplay.Height = bmpSrcImg.Height;
                pnlMainDisplay.Width = bmpSrcImg.Width;
                picLoadImg.Update();
                picLoadImg.Refresh();
                pnlMainDisplay.Refresh();
                labCoordinate.Top = pnlMainDisplay.Bottom + 10;
                labCoordinate.Left = pnlMainDisplay.Left;
                labCoordinate.BackColor = Color.Red;
                labX.Left = labCoordinate.Left;
                labX.Top = labCoordinate.Bottom + 10;
                labX.Width = labCoordinate.Width;
                labY.Top = labX.Top;
                labY.Left = labX.Right + 30;
                labY.Width = labCoordinate.Width;
            }
        }

        private void btnAddROI_Click(object sender, EventArgs e)
        {
            roiAct = ROIACTType.Attach;
            gROI = new TROI(this.Top + (picLoadImg.Width / 3), this.Left + (picLoadImg.Height / 3), 100, 100, ROIType.Rectangle);
            if (rects.Count > 0)
            {
                rects.Clear();
            }
            bBeginDrag = true;
            picLoadImg.Invalidate();
        }
        private void UpdateDrawingPicture(TROI roi)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ReflashDrawing(UpdateDrawingPicture), roi);
            }
            else
            {
                Graphics g = picLoadImg.CreateGraphics();

                if (rects.Count > 0)
                {
                    gROI.DrawROIs(rects, gROI, g);
                }
                else
                {
                    gROI.DrawROI(g);
                }

                picLoadImg.Update();
                picLoadImg.Refresh();
            }
        }

        private void picLoadImgOnPaint(object sender, PaintEventArgs e)
        {
            BlobType type = (radBtnWhite.Checked == true ? BlobType.White : BlobType.Black);
            if (gROI != null)
            {

                switch (roiAct)
                {
                    case ROIACTType.DrawBlob:
                        if (rects.Count > 0)
                        {
                            gROI.DrawROIs(rects, gROI, e.Graphics);
                        }
                        break;
                    case ROIACTType.Attach:
                        gROI.DrawROI(e.Graphics);
                        break;
                    case ROIACTType.DrawLine:
                        TROI measure_area = new TROI(gROI.GetLine().start.X - 10, gROI.GetLine().start.Y - 10, ((gROI.GetLine().end.X + 10) - (gROI.GetLine().start.X - 10)), ((gROI.GetLine().end.Y + 10) - (gROI.GetLine().start.Y - 10)), ROIType.Rectangle);
                        measure_area.SetStartPosition(gROI.GetLine().start);
                        measure_area.SetEndPosition(gROI.GetLine().end);
                        

                        if (bBeginDrag == false)
                        {
                            TROI test = imgProc.MeasureLine(measure_area, bmpSrcImg, roiAct, type, tackBWTh.Value);
                            TLine l = new TLine();
                            l.start.X = gROI.GetLine().start.X - 10;
                            l.start.Y = gROI.GetLine().start.Y - 10;
                            l.end.X = gROI.GetLine().end.X + 10;
                            l.end.Y = gROI.GetLine().end.Y + 10;
                            test.SetRegionPos(l);
                            gROI.DrawLine(test, e.Graphics);
                        }
                        else
                        {
                            gROI.DrawLine(gROI, e.Graphics);
                        }


                            break;
                }
                picLoadImg.Update();
                picLoadImg.Refresh();
            }
        }
        private void btn2Gray_Click(object sender, EventArgs e)
        {
            imgProc = new ImgLib(bmpSrcImg);
            Bitmap grayImage = imgProc.RGB2Gray(gROI, bmpSrcImg);

            if (grayImage != null)
            {
                picLoadImg.InitialImage = null;
                picLoadImg.Image = grayImage;
                picLoadImg.Width = grayImage.Width;
                picLoadImg.Height = grayImage.Height;
                picLoadImg.Update();
                picLoadImg.Refresh();
                pnlMainDisplay.Refresh();
            }
        }
        private void btnRGB2BW_Click(object sender, EventArgs e)
        {
            imgProc = new ImgLib(bmpSrcImg);
            Bitmap bwImage = imgProc.RGB2BW(gROI, bmpSrcImg, tackBWTh.Value);

            if (bwImage != null)
            {
                picLoadImg.InitialImage = null;
                picLoadImg.Image = bwImage;
                picLoadImg.Width = bwImage.Width;
                picLoadImg.Height = bwImage.Height;
                picLoadImg.Update();
                picLoadImg.Refresh();
                pnlMainDisplay.Refresh();
            }
        }
        private void tackBWTh_ValueChanged(object sender, EventArgs e)
        {
            TrackBar t = (TrackBar)sender;
            ImgLib imgProc = new ImgLib(bmpSrcImg);
            Bitmap bwImage = imgProc.RGB2BW(gROI, bmpSrcImg, t.Value);
            t.Text = tackBWTh.Value.ToString();
            labTrackValue.Text = t.Value.ToString();
            if (bwImage != null)
            {
                picLoadImg.InitialImage = null;
                picLoadImg.Image = bwImage;
                picLoadImg.Width = bwImage.Width;
                picLoadImg.Height = bwImage.Height;
                picLoadImg.Update();
                picLoadImg.Refresh();
                pnlMainDisplay.Refresh();
            }
        }
        private void btnCircleROI_Click(object sender, EventArgs e)
        {
            roiAct = ROIACTType.Attach;
            gROI = new TROI(this.Top + (picLoadImg.Width / 3), this.Left + (picLoadImg.Height / 3), 100, 100, ROIType.Circle);
            bBeginDrag = true;
            if (rects.Count > 0)
            {
                rects.Clear();
            }
            picLoadImg.Invalidate();
        }

        private void btnBlob_Click(object sender, EventArgs e)
        {
            imgProc = new ImgLib(bmpSrcImg);
            Graphics g = picLoadImg.CreateGraphics();
            BlobType type = (radBtnWhite.Checked == true ? BlobType.White : BlobType.Black);
            roiAct = ROIACTType.DrawBlob;
            lock (rects)
            {
                rects = imgProc.BlobObject(gROI, bmpSrcImg, type, tackBWTh.Value);
            }
        }

        private void btnMeasureLine_Click(object sender, EventArgs e)
        {
            imgProc = new ImgLib(bmpSrcImg);
            Graphics g = picLoadImg.CreateGraphics();
            roiAct = ROIACTType.DrawLine;
            gROI = new TROI(this.Top + (picLoadImg.Width / 3), this.Left + (picLoadImg.Height / 3), 100, 100, ROIType.MeasureLine);
            bBeginDrag = true;
            picLoadImg.Invalidate();
        }
    }
}
