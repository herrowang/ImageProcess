
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ImageProcess
{
    public enum MouseCrossDirectionType
    {
        East = 0,
        West,
        North,
        South,
        NorthEast,
        NorthWest,
        SouthEst,
        SouthWest,
        Center,
    }
    public partial class frmMain : Form
    {
        private delegate void update_screen(Bitmap srcimg);
        private Image srcImg;
        private Bitmap bmpSrcImg;
        private PictureBox picLoadImg;
        private bool bBeginDrag;
        private MouseCrossDirectionType directType;
        Point pCenter;
        Rectangle rect;
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
            picLoadImg = new PictureBox();
            picLoadImg.Left = this.Left + 20;
            picLoadImg.Top = this.Top + 20;
            picLoadImg.MouseDown += new MouseEventHandler(pictureOnMouseDown);
            picLoadImg.MouseMove += new MouseEventHandler(pictureOnMouseMove);
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
            btn2Gray.Top = btnAddROI.Top;
            btn2Gray.Left = btnAddROI.Right + 30;
            btnRGB2BW.Top = btn2Gray.Top;
            btnRGB2BW.Left = btn2Gray.Right + 30;
            tackBWTh.Top = btnLoadImage.Bottom + 20; ;
            tackBWTh.Left = btnLoadImage.Left;
            tackBWTh.Width = 200;
        }

        private void pictureMouseDragEnter(object sender, DragEventArgs e)
        {
            pCenter.X = e.X;
            pCenter.Y = e.Y;
            rect = new Rectangle(pCenter.X - rect.Width / 2, pCenter.Y - rect.Height / 2, rect.Width, rect.Height);
            picLoadImg.Invalidate();
        }

        private void pictureMouseDragOver(object sender, DragEventArgs e)
        {
            pCenter.X = e.X;
            pCenter.Y = e.Y;
            rect = new Rectangle(pCenter.X - rect.Width / 2, pCenter.Y - rect.Height / 2, rect.Width, rect.Height);
            picLoadImg.Invalidate();
        }

        private void pictureOnMouseDown(object sender, MouseEventArgs e)
        {
            PictureBox p = (PictureBox)sender;

            if (p != null)
            {
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
            }
        }

        private void pictureOnMouseMove(object sender, MouseEventArgs e)
        {
            PictureBox p = (PictureBox)sender;

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
                    directType = MouseCrossDirectionType.Center;
                }
                else if ((Math.Abs(e.X - rect.X) <= 10) && (Math.Abs(e.Y - rect.Top) <= 10))
                {
                    Cursor = Cursors.PanNW;
                    directType = MouseCrossDirectionType.NorthWest;
                }
                else if ((Math.Abs(e.X - rect.X) <= 10) && (Math.Abs(e.Y - rect.Bottom) <= 10))
                {
                    directType = MouseCrossDirectionType.SouthWest;
                    Cursor = Cursors.PanSW;
                }
                else if ((Math.Abs(e.X - rect.Right) <= 10) && (Math.Abs(e.Y - rect.Y) <= 10))
                {
                    directType = MouseCrossDirectionType.NorthEast;
                    Cursor = Cursors.PanNE;
                }
                else if ((Math.Abs(e.X - rect.Right) <= 10) && (Math.Abs(e.Y - rect.Bottom) <= 10))
                {
                    directType = MouseCrossDirectionType.SouthEst;
                    Cursor = Cursors.PanSE;
                }
                else
                {
                    Cursor = Cursors.Arrow;
                }

                if (e.Button == MouseButtons.Left)
                {
                    picMouseMove(directType, e);
                }
            }
        }

        private void picMouseMove(MouseCrossDirectionType type, MouseEventArgs e)
        {
            Rectangle orgRect = rect;
            int height = 0;
            int width = 0;

            if (rect.X < (picLoadImg.Left + 10)
                || rect.Y < (picLoadImg.Top + 10)
                || rect.Right > picLoadImg.Right - 10
                || rect.Bottom > picLoadImg.Bottom - 10)
            {
                return;
            }

            switch (type)
            {
                case MouseCrossDirectionType.Center:
                    pCenter.X = e.X;
                    pCenter.Y = e.Y;
                    rect.X = (pCenter.X - rect.Width / 2);
                    rect.Y = (pCenter.Y - rect.Height / 2);
                    break;
                case MouseCrossDirectionType.NorthWest:
                    width = Math.Abs(orgRect.Right - e.X);
                    height = Math.Abs(orgRect.Bottom - e.Y);
                    rect = new Rectangle(e.X, e.Y, width, height);
                    pCenter.X = rect.X + rect.Width / 2;
                    pCenter.Y = rect.Y + rect.Height / 2;
                    break;
                case MouseCrossDirectionType.NorthEast:
                    width = e.X - orgRect.X;
                    height = orgRect.Height + (orgRect.Y - e.Y);
                    rect = new Rectangle(orgRect.X, e.Y, width, height);
                    pCenter.X = rect.X + rect.Width / 2;
                    pCenter.Y = rect.Y + rect.Height / 2;
                    break;
                case MouseCrossDirectionType.SouthWest:
                    width = Math.Abs(e.X - orgRect.Right);
                    height = Math.Abs(e.Y - orgRect.Top);
                    rect = new Rectangle(e.X, orgRect.Y, width, height);
                    pCenter.X = rect.X + rect.Width / 2;
                    pCenter.Y = rect.Y + rect.Height / 2;
                    break;
                case MouseCrossDirectionType.SouthEst:
                    width = Math.Abs(e.X - orgRect.X);
                    height = Math.Abs(e.Y - orgRect.Y);
                    rect = new Rectangle(orgRect.X, orgRect.Y, width, height);
                    pCenter.X = rect.X + rect.Width / 2;
                    pCenter.Y = rect.Y + rect.Height / 2;
                    break;
                default:
                    break;
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
            }
        }

        private void btnAddROI_Click(object sender, EventArgs e)
        {
            rect = new Rectangle(this.Top + (picLoadImg.Width/3), this.Left + (picLoadImg.Height/3), 100, 100);
            pen = new Pen(Color.Red, 2);
            pCenter = new Point();
            pCenter.X = rect.Left + rect.Width / 2;
            pCenter.Y = rect.Top + rect.Height / 2;
            bBeginDrag = true;
            picLoadImg.Invalidate();
        }

        private void picLoadImgOnPaint(object sender, PaintEventArgs e)
        {
            if (pen != null && rect != null)
            {
                SolidBrush brush = new SolidBrush(Color.Green);
                Pen p = new Pen(Color.LimeGreen, 5);
                e.Graphics.DrawRectangle(pen, rect);
                e.Graphics.DrawLine(p, new Point(pCenter.X - 10, pCenter.Y), new Point(pCenter.X + 10, pCenter.Y));
                e.Graphics.DrawLine(p, new Point(pCenter.X, pCenter.Y - 10), new Point(pCenter.X, pCenter.Y + 10));
                e.Graphics.FillRectangle(brush, new Rectangle(rect.X - 5, rect.Y - 5, 10, 10));
                e.Graphics.FillRectangle(brush, new Rectangle(rect.X - 5, rect.Bottom - 5, 10, 10));
                e.Graphics.FillRectangle(brush, new Rectangle(rect.Right - 5, rect.Y - 5, 10, 10));
                e.Graphics.FillRectangle(brush, new Rectangle(rect.Right - 5, rect.Bottom - 5, 10, 10));
            }
            picLoadImg.Update();
            picLoadImg.Refresh();
        }

        private Bitmap TransRGB2Gray(Bitmap srcImg)
        {
            int width = srcImg.Width;
            int height = srcImg.Height;
            Bitmap bitmap = new Bitmap(srcImg);
            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            Bitmap grayImage = new Bitmap(width, height, bitmap.PixelFormat);
            BitmapData grayData = grayImage.LockBits(new Rectangle(0, 0, grayImage.Width, grayImage.Height), ImageLockMode.ReadWrite, grayImage.PixelFormat);

            unsafe
            {
                int nRGBOffset = bmpData.Stride - bmpData.Width * (bitmap.PixelFormat == PixelFormat.Format24bppRgb ? 3 : 4);
                int nGrayOffset = grayData.Stride - grayData.Width * (grayImage.PixelFormat == PixelFormat.Format24bppRgb ? 3 : 4);
                byte* rgb_ptr = (byte*)(bmpData.Scan0);
                byte* gray_ptr = (byte*)(grayData.Scan0);
                byte grayVal = 0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (bitmap.PixelFormat == PixelFormat.Format24bppRgb)
                        {
                            grayVal = (byte)((rgb_ptr[2] + rgb_ptr[1] + rgb_ptr[0]) / 3);
                        }
                        else
                        {
                            byte b = (byte)(rgb_ptr[2] * (rgb_ptr[3] / 255) + 255 - rgb_ptr[3]);
                            byte g = (byte)(rgb_ptr[1] * (rgb_ptr[3] / 255) + 255 - rgb_ptr[3]);
                            byte r = (byte)(rgb_ptr[0] * (rgb_ptr[3] / 255) + 255 - rgb_ptr[3]);

                            if ((y > rect.Y && y < rect.Bottom)
                                && (x > rect.X && x < rect.Right))
                            {
                                grayVal = (byte)((b + g + r) / 3);
                                gray_ptr[0] = grayVal;
                                gray_ptr[1] = grayVal;
                                gray_ptr[2] = grayVal;
                                gray_ptr[3] = rgb_ptr[3];
                            }
                            else
                            {
                                gray_ptr[0] = rgb_ptr[0];
                                gray_ptr[1] = rgb_ptr[1];
                                gray_ptr[2] = rgb_ptr[2];
                                gray_ptr[3] = rgb_ptr[3];
                            }

                        }
                        //*gray_ptr = grayVal;
                        rgb_ptr += (bitmap.PixelFormat == PixelFormat.Format24bppRgb ? 3 : 4);
                        gray_ptr += (grayImage.PixelFormat == PixelFormat.Format24bppRgb ? 3 : 4);
                    }
                    rgb_ptr += nRGBOffset;
                    gray_ptr += nGrayOffset;
                }
                bitmap.UnlockBits(bmpData);
                grayImage.UnlockBits(grayData);
            }
            grayImage.Save("test.bmp");
            return grayImage;
        }

        private Bitmap TransRGB2BW(Bitmap srcImg)
        {
            int width = srcImg.Width;
            int height = srcImg.Height;
            Bitmap bitmap = new Bitmap(srcImg);
            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            Bitmap bwImage = new Bitmap(width, height, bitmap.PixelFormat);
            BitmapData bwData = bwImage.LockBits(new Rectangle(0, 0, bwImage.Width, bwImage.Height), ImageLockMode.ReadWrite, bwImage.PixelFormat);

            unsafe
            {
                int nRGBOffset = bmpData.Stride - bmpData.Width * (bitmap.PixelFormat == PixelFormat.Format24bppRgb ? 3 : 4);
                int nGrayOffset = bwData.Stride - bwData.Width * (bwData.PixelFormat == PixelFormat.Format24bppRgb ? 3 : 4);
                byte* rgb_ptr = (byte*)(bmpData.Scan0);
                byte* bw_ptr = (byte*)(bwData.Scan0);
                byte bwVal = 0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (bitmap.PixelFormat == PixelFormat.Format24bppRgb)
                        {
                            bwVal = (byte)((rgb_ptr[2] + rgb_ptr[1] + rgb_ptr[0]) / 3);
                        }
                        else
                        {
                            byte b = (byte)(rgb_ptr[2] * (rgb_ptr[3] / 255) + 255 - rgb_ptr[3]);
                            byte g = (byte)(rgb_ptr[1] * (rgb_ptr[3] / 255) + 255 - rgb_ptr[3]);
                            byte r = (byte)(rgb_ptr[0] * (rgb_ptr[3] / 255) + 255 - rgb_ptr[3]);

                            if ((y > rect.Y && y < rect.Bottom)
                                && (x > rect.X && x < rect.Right))
                            {
                                int val = tackBWTh.Value;
                                bwVal = (byte)(((b + g + r) / 3) >= val ? 255 : 0);
                                bw_ptr[0] = bwVal;
                                bw_ptr[1] = bwVal;
                                bw_ptr[2] = bwVal;
                                bw_ptr[3] = rgb_ptr[3];
                            }
                            else
                            {
                                bw_ptr[0] = rgb_ptr[0];
                                bw_ptr[1] = rgb_ptr[1];
                                bw_ptr[2] = rgb_ptr[2];
                                bw_ptr[3] = rgb_ptr[3];
                            }

                        }
                        //*gray_ptr = grayVal;
                        rgb_ptr += (bitmap.PixelFormat == PixelFormat.Format24bppRgb ? 3 : 4);
                        bw_ptr += (bwImage.PixelFormat == PixelFormat.Format24bppRgb ? 3 : 4);
                    }
                    rgb_ptr += nRGBOffset;
                    bw_ptr += nGrayOffset;
                }
                bitmap.UnlockBits(bmpData);
                bwImage.UnlockBits(bwData);
            }
            bwImage.Save("test.bmp");
            return bwImage;
        }

        private void btn2Gray_Click(object sender, EventArgs e)
        {
            Bitmap grayImage = TransRGB2Gray(bmpSrcImg);

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
            Bitmap bwImage = TransRGB2BW(bmpSrcImg);

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
            Bitmap bwImage = TransRGB2BW(bmpSrcImg);

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
    }
}
