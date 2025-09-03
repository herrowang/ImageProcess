
using DALSA.SaperaLT.SapClassBasic;
using FileOperations;
using FileOperations;
using RoomCV;
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageProcess
{
    public partial class frmMain : Form
    {
        private delegate void updateLoadImages(string sfileName);
        private delegate void updateTimer();
        private delegate void update_screen(Bitmap srcimg);
        private delegate void ReflashDrawing(TROI roi);
        private Image srcImg;
        private Bitmap bmpSrcImg;
        private PictureBox picLoadImg;
        private bool bBeginDrag;
        private MouseCrossDirectionType directType;
        private List<TROI> rects;
        private ROIACTType roiAct;
        private MeasureDirect measureType;
        private ImgLib imgProc;
        private CCD_Driver ccdDev;
        private Thread thRunTask;
        private bool bRunTask;
        private Button btnRunTask;
        private Button btnDualMeasure;
        private Label labTimer;
        private DataFiles cfg_files;
        Point pCenter;
        Rectangle rect;
        TROI gROI;
        private bool startDrawPoints;
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
            bRunTask = false;
            bBeginDrag = false;
            imgProc = null;
            btnRunTask = new Button();
            btnDualMeasure = new Button();
            labTimer = new Label();
            tabSystemControl.Width = this.Width;
            tabSystemControl.Height = this.Height;
            foreach (TabPage page in tabSystemControl.TabPages)
            {
                page.Width = tabSystemControl.Width;
                page.Height = tabSystemControl.Height;
            }
            pnlToolBar.Top = this.Top;
            pnlToolBar.Left = this.Left;
            pnlToolBar.Width = this.Width;
            pnlToolBar.Height = 100;
            pnlToolBar.BackColor = Color.AliceBlue;
            pnlMainDisplay.Top = pnlToolBar.Bottom;
            pnlMainDisplay.Left = pnlToolBar.Left;
            pnlMainDisplay.Width = 1024;
            pnlMainDisplay.Height = 768;
            pnlMainDisplay.BackColor = Color.White;
            pnlMainDisplay.Dock = DockStyle.Fill;
            pnlMainDisplay.AutoSize = false;
            pnlMainDisplay.AutoScrollMinSize = new Size(1024, 768);
            pnlMainDisplay.AutoScroll = true;

            picLoadImg = new PictureBox();
            picLoadImg.Left = this.Left + 20;
            picLoadImg.Top = this.Top + 20;
            picLoadImg.Width = 800;
            picLoadImg.Height = 600;
            picLoadImg.MouseDown += new MouseEventHandler(pictureOnMouseDown);
            picLoadImg.MouseMove += new MouseEventHandler(pictureOnMouseMove);
            picLoadImg.MouseUp += new MouseEventHandler(pictureOnMouseUp);
            picLoadImg.Paint += new PaintEventHandler(picLoadImgOnPaint);
            picLoadImg.DragEnter += new DragEventHandler(pictureMouseDragEnter);
            picLoadImg.DragOver += new DragEventHandler(pictureMouseDragOver);
            ObjectPositionArrangement();
            rects = new List<TROI>();
            measureType = MeasureDirect.Horhorizontal;
            ccdDev = new CCD_Driver("", 0);
            ccdDev.CCD_NotifierEventHanlder += new EventHandler<CCD_DriverEventNotifier>(CCD_NotifierEventHandler);
            thRunTask = new Thread(thRun);
            startDrawPoints = false;
            Configuration_Load();

        }

        private void Configuration_Load()
        {
            DataFiles _cfg = null;
            FolderBrowserDialog folder = new FolderBrowserDialog();
            string[] files = null;
            labImagePath.Top = tabSystemControl.TabPages[1].Top + 10;
            txtImagePath.Top = labImagePath.Bottom + 10;
            txtImagePath.Left = labImagePath.Left;
            btnImagePath.Top = txtImagePath.Top;
            btnImagePath.Left = txtImagePath.Right + 10;
            lvImages.Top = txtImagePath.Bottom + 10;
            lvImages.Left = labImagePath.Left;

            cfg_files = new DataFiles();

            _cfg = cfg_files.LoadConfiguration("./system.cfg");

            if (_cfg != null)
            {
                folder.SelectedPath = _cfg.getDirectoryPath();
                folder.Description = "Select Image Folder";
                files = System.IO.Directory.GetFiles(folder.SelectedPath,"*.*", SearchOption.TopDirectoryOnly)
                    .Where(s => s.EndsWith(".jpg") || s.EndsWith(".png")).ToArray<string>();
                if (files != null && files.Length > 0)
                {
                    cfg_files.setFileNames(files.ToList<string>());
                    txtImagePath.Text = folder.SelectedPath;
                    lvImages.Items.Clear();
                    lvImages.Scrollable = true;
                    lvImages.View = View.Details;
                    ColumnHeader header = new ColumnHeader();
                    header.Text = "images";
                    header.Name = "Images";
                    lvImages.Columns.Add(header);

                    header = new ColumnHeader();
                    header.Text = "size";
                    header.Name = "size";
                    lvImages.Columns.Add(header);

                    header = new ColumnHeader();
                    header.Text = "update Date";
                    header.Name = "Date";
                    lvImages.Columns.Add(header);
                    foreach (ColumnHeader h in lvImages.Columns)
                    {
                        h.Width = 300;
                        h.TextAlign = HorizontalAlignment.Left;
                    }

                    foreach (string f in files)
                    {
                        ListViewItem item = new ListViewItem();
                        item.Text = System.IO.Path.GetFileName(f);
                        item.SubItems.Add((new FileInfo(f).Length / 1024).ToString() + " KB");
                        item.SubItems.Add(File.GetCreationTime(f).ToString("yyyy-MM-dd HH:mm:ss"));
                        lvImages.Items.Add(item);
                    }
                }
            }

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
                if (gROI != null)
                {
                    switch (gROI.GetROIType())
                    {
                        case ROIType.MeasureLine:
                            gROI.SetStartPosition(new Point(e.X, e.Y));
                            gROI.SetEndPosition(new Point(e.X, e.Y));
                            break;
                        case ROIType.DualRectangle:
                            rect = new Rectangle(e.X, e.Y, 0, 0);
                            gROI.updateRegions(rect);
                            bBeginDrag = true;
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
        }

        private void pictureOnMouseUp(object sender, MouseEventArgs e)
        {
            PictureBox p = (PictureBox)sender;
            int nX = 0;
            int nY = 0;
            int width = 0;
            int height = 0;

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
                    case ROIType.DualRectangle:
                        nX = e.X > rect.X ? rect.X : e.X;
                        nY = e.Y > rect.Y ? rect.Y : e.Y;
                        width = Math.Abs(e.X - rect.X);
                        height = Math.Abs(e.Y - rect.Y);
                        rect.X = nX;
                        rect.Y = nY;
                        rect.Width = width;
                        rect.Height = height;
                        gROI.updateRegions(rect);
                        if (gROI.measureRois.Count < gROI.nMaxROIs)
                        {
                            gROI.measureRois.Add(rect);
                        }
                        bBeginDrag = false;
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
            int nX = 0;
            int nY = 0;
            int width = 0;
            int height = 0;

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
                    if (roiAct != ROIACTType.DrawLine
                        && roiAct != ROIACTType.DrawDualRectangle)
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
                    if (gROI.GetROIType() == ROIType.DualRectangle)
                    {
                        nX = e.X > rect.X ? rect.X : e.X;
                        nY = e.Y > rect.Y ? rect.Y : e.Y;
                        width = Math.Abs(e.X - rect.X);
                        height = Math.Abs(e.Y - rect.Y);
                        rect.X = nX;
                        rect.Y = nY;
                        rect.Width = width;
                        rect.Height = height;
                        gROI.updateRegions(rect);
                    }
                    else
                    {
                        gROI.Move(new Point(e.X, e.Y), gROI);
                        rect = gROI.GetRegion();
                    }
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
        private void update_LoadImages(string sfileName)
        {
            if (this.InvokeRequired)
            {
                updateLoadImages upImg = new updateLoadImages(update_LoadImages);
                this.BeginInvoke(upImg, sfileName);
            }
            else
            {
                bmpSrcImg = ConvertToBitmap(sfileName);
                picLoadImg.Image = bmpSrcImg;
                picLoadImg.Top = this.Top;
                picLoadImg.Left = this.Left;
                picLoadImg.Width = bmpSrcImg.Width;
                picLoadImg.Height = bmpSrcImg.Height;
                pnlMainDisplay.Controls.Add(picLoadImg);
                picLoadImg.Update();
                picLoadImg.Refresh();
                pnlMainDisplay.Refresh();
                labCoordinate.Top = picLoadImg.Bottom + 10;
                labCoordinate.Left = picLoadImg.Left;
                labCoordinate.BackColor = Color.Red;
                labX.Left = labCoordinate.Left;
                labX.Top = labCoordinate.Bottom + 10;
                labX.Width = labCoordinate.Width;
                labY.Top = labX.Top;
                labY.Left = labX.Right + 30;
                labY.Width = labCoordinate.Width;
                labTimer.Left = labCoordinate.Right + 20;
                labTimer.Top = labCoordinate.Top + 20;
            }
        }

        private void btnLoadImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Title = "Load Images";
            openFile.Filter = "All files (*.*)|*.*|jpeg file (*.jpg)|*.jpg|bmp file (*.bmp)|*.bmp";

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                update_LoadImages(openFile.FileName);
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

        private void ObjectPositionArrangement()
        {
            labCoordinate.Top = picLoadImg.Bottom + 10;
            labCoordinate.Left = picLoadImg.Left;
            labCoordinate.BackColor = Color.Red;
            labTimer.Left = labCoordinate.Right + 20;
            labTimer.Top = labCoordinate.Top + 20;
            labTimer.Width = 200;
            labTimer.Height = 50;
            labTimer.Font = new Font("Arial", 24, FontStyle.Bold);
            labTimer.AutoSize = true;
            labTimer.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if (pnlMainDisplay.Controls.Contains(labTimer) == false)
            {
                pnlMainDisplay.Controls.Add(labTimer);
            }
            labX.Left = labCoordinate.Left;
            labX.Top = labCoordinate.Bottom + 10;
            labX.Width = labCoordinate.Width;
            labY.Top = labX.Top;
            labY.Left = labX.Right + 30;
            labY.Width = labCoordinate.Width;
            btnLoadImage.Top = this.Top;
            btnLoadImage.Left = this.Left;
            btnAddROI.Top = btnLoadImage.Top;
            btnAddROI.Left = btnLoadImage.Right + 30;
            btnCircleROI.Top = btnAddROI.Top;
            btnCircleROI.Left = btnAddROI.Right + 30;
            btn2Gray.Top = btnAddROI.Top;
            btn2Gray.Left = btnAddROI.Right + 30;
            btnRGB2BW.Top = btn2Gray.Top;
            btnRGB2BW.Left = btn2Gray.Right + 30;
            tackBWTh.Top = btnRGB2BW.Top;
            tackBWTh.Left = btnRGB2BW.Right + 30;
            tackBWTh.Width = 200;
            labTrackValue.Top = tackBWTh.Top;
            labTrackValue.Left = tackBWTh.Right + 30;
            labTrackValue.Width = tackBWTh.Width;
            labTrackValue.Text = tackBWTh.Value.ToString();
            btnBlob.Top = labTrackValue.Top;
            btnBlob.Left = labTrackValue.Right + 30;
            btnBlob.Width = btnAddROI.Width;
            btnBlob.Height = btnAddROI.Height;
            radBtnWhite.Top = btnBlob.Top;
            radBtnWhite.Left = btnBlob.Right + 10;
            radBtnBlack.Top = radBtnWhite.Bottom + 10;
            radBtnBlack.Left = radBtnWhite.Left;
            btnHorizontalLine.Left = radBtnWhite.Right + 30;
            btnHorizontalLine.Top = radBtnWhite.Top;
            btnHorizontalLine.Width = btnBlob.Width;
            btnHorizontalLine.Height = btnBlob.Height;
            btnVerticalLine.Left = btnHorizontalLine.Right + 10;
            btnVerticalLine.Top = btnHorizontalLine.Top;
            btnVerticalLine.Width = btnHorizontalLine.Width;
            btnVerticalLine.Height = btnHorizontalLine.Height;
            btnRunTask.Top = btnVerticalLine.Top;
            btnRunTask.Left = btnVerticalLine.Right + 10;
            btnRunTask.Width = btnVerticalLine.Width;
            btnRunTask.Height = btnVerticalLine.Height;
            btnRunTask.Text = "Run";
            btnRunTask.Click += new EventHandler(btnRunTaskOnClick);
            pnlToolBar.Controls.Add(btnRunTask);
            btnDualMeasure.Top = btnRunTask.Top;
            btnDualMeasure.Left = btnRunTask.Right + 10;
            btnDualMeasure.Width = btnRunTask.Width;
            btnDualMeasure.Height = btnRunTask.Height;
            btnDualMeasure.Text = "Measure";
            btnDualMeasure.Click += new EventHandler(btnDualROIMeasure);
            pnlToolBar.Controls.Add(btnDualMeasure);


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
                BlobType type = (radBtnWhite.Checked == true ? BlobType.White : BlobType.Black);

                if (gROI != null)
                {
                    switch (roiAct)
                    {
                        case ROIACTType.DrawBlob:
                            if (rects.Count > 0)
                            {
                                gROI.DrawROIs(rects, gROI, g);
                            }
                            break;
                        case ROIACTType.DrawDualRectangle:
                            List<Point> ps = null;
                            if (gROI.measureRois.Count == gROI.nMaxROIs)
                            {
                                if (startDrawPoints == false)
                                {
                                    foreach (Rectangle r in gROI.measureRois)
                                    {
                                        ps = imgProc.FindEdge(gROI, bmpSrcImg, roiAct, r, tackBWTh.Value);
                                        gROI.DrawPoints(g, ps, r);
                                    }
                                    startDrawPoints = true;
                                }
                            }
                            else
                            {
                                gROI.DrawROI(g);
                            }
                            break;
                        case ROIACTType.Attach:
                            gROI.DrawROI(g);
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
                                gROI.DrawLine(test, g, measureType);
                            }
                            else
                            {
                                gROI.DrawLine(gROI, g, measureType);
                            }
                            break;
                    }
                    picLoadImg.Update();
                    picLoadImg.Refresh();
                }
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
                    case ROIACTType.DrawDualRectangle:
                        List<Point> ps = null;
                        if (gROI.measureRois.Count == gROI.nMaxROIs)
                        {
                            //if (startDrawPoints == false)
                            {
                                foreach (Rectangle r in gROI.measureRois)
                                {
                                    ps = imgProc.FindEdge(gROI, bmpSrcImg, roiAct, r, tackBWTh.Value);
                                    gROI.DrawPoints(e.Graphics, ps, r);
                                }
                                startDrawPoints = true;
                            }
                        }
                        else
                        {
                            gROI.DrawROI(e.Graphics);
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
                            gROI.DrawLine(test, e.Graphics, measureType);
                        }
                        else
                        {
                            gROI.DrawLine(gROI, e.Graphics, measureType);
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
            measureType = MeasureDirect.Horhorizontal;
            gROI = new TROI(this.Top + (picLoadImg.Width / 3), this.Left + (picLoadImg.Height / 3), 100, 100, ROIType.MeasureLine);
            bBeginDrag = true;
            picLoadImg.Invalidate();
        }

        private void btnVeritalLine_Click(object sender, EventArgs e)
        {
            imgProc = new ImgLib(bmpSrcImg);
            Graphics g = picLoadImg.CreateGraphics();
            roiAct = ROIACTType.DrawLine;
            measureType = MeasureDirect.Vertical;
            gROI = new TROI(this.Top + (picLoadImg.Width / 3), this.Left + (picLoadImg.Height / 3), 100, 100, ROIType.MeasureLine);
            bBeginDrag = true;
            picLoadImg.Invalidate();
        }

        private void btnDualROIMeasure(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            if (btn != null)
            {
                imgProc = new ImgLib(bmpSrcImg);
                gROI = new TROI(new Point(10, 10), new Size(10, 10), ROIType.DualRectangle);
                roiAct = ROIACTType.DrawDualRectangle;
                if (gROI.measureRois.Count >= gROI.nMaxROIs)
                {
                    gROI.measureRois.Clear();
                }
                startDrawPoints = false;
                picLoadImg.Invalidate();
            }
        }
        private void btnRunTaskOnClick(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            if (btn != null)
            {
                if (btn.Text == "Run")
                {
                    if (thRunTask == null)
                    {
                        thRunTask = new Thread(thRun);
                    }
                    bRunTask = true;
                    thRunTask.Start();
                    btn.Text = "Stop";
                }
                else
                {
                    bRunTask = false;
                    thRunTask = null;
                    btn.Text = "Run";
                }

            }
        }

        private void update_timer()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new updateTimer(update_timer));
            }
            else
            {
                labTimer.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                labTimer.Update();
                labTimer.Refresh();
                pnlMainDisplay.Refresh();
            }
        }
        private void thRun()
        {
            while (bRunTask)
            {
                update_timer();

                if (cfg_files != null)
                {
                    foreach(string f in cfg_files.FileNames)
                    {
                        update_LoadImages(f);
                        Thread.Sleep(500);
                    }
                }
                Thread.Sleep(1000);
            }
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (thRunTask != null)
            {
                bRunTask = false;
                thRunTask = null;
            }
        }

        private void CCD_NotifierEventHandler(object sender, CCD_DriverEventNotifier e)
        {
            //TODO: Handle CCD events here
            switch (e.eventType)
            {
                case SapManager.EventType.ServerNew:
                    Console.WriteLine("New CCD server detected.");
                    break;
            }
        }

        private void btnImagePath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.Description = "Select Image Folder";
            if (folder.ShowDialog() == DialogResult.OK)
            {
                var files = System.IO.Directory.GetFiles(folder.SelectedPath, "*.*", SearchOption.TopDirectoryOnly)
                    .Where(s => s.EndsWith(".jpg") || s.EndsWith(".png"));
                txtImagePath.Text = folder.SelectedPath;
                lvImages.Items.Clear();
                lvImages.Scrollable = true;
                lvImages.View = View.Details;
                ColumnHeader header = new ColumnHeader();
                header.Text = "images Name";
                header.Name = "Images";
                lvImages.Columns.Clear();
                lvImages.Columns.Add(header);
                header = new ColumnHeader();
                header.Text = "size";
                header.Name = "size";
                lvImages.Columns.Add(header);
                header = new ColumnHeader();
                header.Text = "update Date";
                header.Name = "update Date";
                lvImages.Columns.Add(header);
                foreach (ColumnHeader h in lvImages.Columns)
                {
                    h.Width = 300;
                    h.TextAlign = HorizontalAlignment.Left;
                }

                foreach (string f in files.ToList<string>())
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = System.IO.Path.GetFileName(f);
                    item.SubItems.Add((new FileInfo(f).Length / 1024).ToString() + " KB");
                    item.SubItems.Add(File.GetCreationTime(f).ToString("yyyy-MM-dd HH:mm:ss"));
                    lvImages.Items.Add(item);
                }
                cfg_files.setDirectoryPath(folder.SelectedPath);
                cfg_files.setFileNames(files.ToList<string>());
                cfg_files.SaveeConfiguration(cfg_files, "./system.cfg");
            }
        }
    }
}
