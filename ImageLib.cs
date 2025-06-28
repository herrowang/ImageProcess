using ImageProcess;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace RoomCV
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

    public enum ROIType
    {
        Rectangle = 0,
        Circle,
        End,
    }

    public enum ROIACTType
    {
        Attach = 1,
        DrawBlob,
    }
    public enum BlobType
    {
        White = 255,
        Black = 0,
        END,
    }


    public class TROI
    {
        private MouseCrossDirectionType directionType { get; set; }
        private ROIType roiType { get; set; }
        private Rectangle rect;
        private int nWidth { get; set; }
        private int nHeight { get; set; }
        private Point Center;
        private Pen pen;

        public TROI(Point pos, Size s, ROIType rType)
        {
            directionType = MouseCrossDirectionType.Center;
            roiType = rType;
            rect = new Rectangle(pos, s);
            pen = new Pen(Color.Red, 2);
            Center = new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
            nWidth = rect.Width;
            nHeight = rect.Height;
        }

        public TROI(int x, int y, int width, int height, ROIType rType)
        {
            directionType = MouseCrossDirectionType.Center;
            roiType = rType;
            rect = new Rectangle(x, y, width, height);
            pen = new Pen(Color.Red, 2);
            Center = new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
            nWidth = rect.Width;
            nHeight = rect.Height;
        }

        public bool SetCenterPos(Point pos)
        {
            Center = new Point(pos.X, pos.Y);

            return true;
        }

        public Point GetCenterPos()
        {
            return Center;
        }

        public Rectangle GetRegion()
        {
            return rect;
        }

        public Pen GetPen()
        {
            return pen;
        }

        public MouseCrossDirectionType GetDirectionType()
        {
            return directionType;
        }

        public ROIType GetROIType()
        {
            return roiType;
        }

        public  Cursor GetMouseCursor(Point mPos, TROI srcROI)
        {
            Cursor cursor = Cursors.Arrow;
            if ((Math.Abs(mPos.X - srcROI.GetCenterPos().X) <= 10) && (Math.Abs(mPos.Y - srcROI.GetCenterPos().Y) <= 10))
            {
                cursor = Cursors.Cross;
                directionType = MouseCrossDirectionType.Center;
            }

            switch (srcROI.GetROIType())
            {
                case ROIType.Circle:
                    if ((Math.Abs(mPos.X - srcROI.GetCenterPos().X) <= 10) && (Math.Abs(mPos.Y - rect.Top) <= 10))
                    {
                        directionType = MouseCrossDirectionType.North;
                        cursor = Cursors.PanNorth;
                    }
                    else if ((Math.Abs(mPos.X - srcROI.GetCenterPos().X) <= 10) && (Math.Abs(mPos.Y - rect.Bottom) <= 10))
                    {
                        directionType = MouseCrossDirectionType.South;
                        cursor = Cursors.PanSouth;
                    }
                    else if ((Math.Abs(mPos.X - rect.Right) <= 10) && (Math.Abs(mPos.Y - srcROI.GetCenterPos().Y) <= 10))
                    {
                        directionType = MouseCrossDirectionType.East;
                        cursor = Cursors.PanEast;
                    }
                    else if ((Math.Abs(mPos.X - rect.X) <= 10) && (Math.Abs(mPos.Y - srcROI.GetCenterPos().Y) <= 10))
                    {
                        directionType = MouseCrossDirectionType.West;
                        cursor = Cursors.PanWest;
                    }
 
                    break;
                case ROIType.Rectangle:
                    if ((Math.Abs(mPos.X - rect.X) <= 10) && (Math.Abs(mPos.Y - rect.Top) <= 10))
                    {
                        cursor = Cursors.PanNW;
                        directionType = MouseCrossDirectionType.NorthWest;
                    }
                    else if ((Math.Abs(mPos.X - rect.X) <= 10) && (Math.Abs(mPos.Y - rect.Bottom) <= 10))
                    {
                        directionType = MouseCrossDirectionType.SouthWest;
                        cursor = Cursors.PanSW;
                    }
                    else if ((Math.Abs(mPos.X - rect.Right) <= 10) && (Math.Abs(mPos.Y - rect.Y) <= 10))
                    {
                        directionType = MouseCrossDirectionType.NorthEast;
                        cursor = Cursors.PanNE;
                    }
                    else if ((Math.Abs(mPos.X - rect.Right) <= 10) && (Math.Abs(mPos.Y - rect.Bottom) <= 10))
                    {
                        directionType = MouseCrossDirectionType.SouthEst;
                        cursor = Cursors.PanSE;
                    }
                    break;
            }

            return cursor;
        }

        public TROI Move(Point mPos, TROI srcROI)
        {
            TROI orgROI = srcROI;
            TROI newROI = new TROI(new Point(orgROI.GetRegion().X, orgROI.GetRegion().Y), new Size(orgROI.GetRegion().Width, orgROI.GetRegion().Height), orgROI.GetROIType());
            int width = 0;
            int height = 0;

            switch (orgROI.GetDirectionType())
            {
                case MouseCrossDirectionType.Center:
                    orgROI.SetCenterPos(mPos);
                    rect.X = orgROI.GetCenterPos().X - (rect.Width  / 2);
                    rect.Y = orgROI.GetCenterPos().Y - (rect.Height / 2);
                    break;
                case MouseCrossDirectionType.North:
                    width = orgROI.GetRegion().Width;
                    height = Math.Abs(orgROI.GetRegion().Bottom - mPos.Y);
                    rect = new Rectangle(Center.X - (width / 2), mPos.Y, width, height);
                    Center.Y = rect.Y + rect.Height / 2;
                    break;
                case MouseCrossDirectionType.South:
                    width = orgROI.GetRegion().Width;
                    height = Math.Abs(orgROI.GetRegion().Top - mPos.Y);
                    rect = new Rectangle(orgROI.GetCenterPos().X - (width / 2), mPos.Y - height, width, height);
                    Center.Y = rect.Y + rect.Height / 2;
                    break;
                case MouseCrossDirectionType.West:
                    width = Math.Abs(orgROI.GetRegion().Right - mPos.X);
                    height = orgROI.GetRegion().Height;
                    rect = new Rectangle(mPos.X, orgROI.GetRegion().Y, width, height);
                    Center.X = rect.X + width / 2;
                    break;
                case MouseCrossDirectionType.East:
                    width = Math.Abs(orgROI.GetRegion().Left - mPos.X);
                    height = orgROI.GetRegion().Height;
                    rect = new Rectangle(mPos.X - width, orgROI.GetRegion().Y, width, height);
                    Center.X = rect.X + width / 2;
                    break;
                case MouseCrossDirectionType.NorthWest:
                    width = Math.Abs(orgROI.GetRegion().Right - mPos.X);
                    height = Math.Abs(orgROI.GetRegion().Bottom - mPos.Y);
                    rect = new Rectangle(mPos.X, mPos.Y, width, height);
                    Center.X = rect.X + rect.Width / 2;
                    Center.Y = rect.Y + rect.Height / 2;
                    break;
                case MouseCrossDirectionType.NorthEast:
                    width = mPos.X - orgROI.GetRegion().X;
                    height = orgROI.GetRegion().Height + (orgROI.GetRegion().Y - mPos.Y);
                    rect = new Rectangle(orgROI.GetRegion().X, mPos.Y, width, height);
                    Center.X = rect.X + rect.Width / 2;
                    Center.Y = rect.Y + rect.Height / 2;
                    break;
                case MouseCrossDirectionType.SouthWest:
                    width = Math.Abs(mPos.X - orgROI.GetRegion().Right);
                    height = Math.Abs(mPos.Y - orgROI.GetRegion().Top);
                    rect = new Rectangle(mPos.X, orgROI.GetRegion().Y, width, height);
                    Center.X = rect.X + rect.Width / 2;
                    Center.Y = rect.Y + rect.Height / 2;
                    break;
                case MouseCrossDirectionType.SouthEst:
                    width = Math.Abs(mPos.X - orgROI.GetRegion().X);
                    height = Math.Abs(mPos.Y - orgROI.GetRegion().Y);
                    rect = new Rectangle(orgROI.GetRegion().X, orgROI.GetRegion().Y, width, height);
                    Center.X = rect.X + rect.Width / 2;
                    Center.Y = rect.Y + rect.Height / 2;
                    break;
                default:
                    break;
            }

            nWidth = width;
            nHeight = height;

            return newROI;
        }   

        public void DrawROI(Graphics g)
        {
            if (g != null)
            {
                SolidBrush brush = new SolidBrush(Color.Aqua);
                Pen p = new Pen(Color.Aqua, 5);
                switch (GetROIType())
                {
                    case ROIType.Rectangle:
                        g.DrawRectangle(GetPen(), GetRegion());
                        g.DrawLine(p, new Point(GetCenterPos().X - 10, GetCenterPos().Y), new Point(GetCenterPos().X + 10, GetCenterPos().Y));
                        g.DrawLine(p, new Point(GetCenterPos().X, GetCenterPos().Y - 10), new Point(GetCenterPos().X, GetCenterPos().Y + 10));
                        g.FillRectangle(brush, new Rectangle(GetRegion().X - 5, GetRegion().Y - 5, 10, 10));
                        g.FillRectangle(brush, new Rectangle(GetRegion().X - 5, GetRegion().Bottom - 5, 10, 10));
                        g.FillRectangle(brush, new Rectangle(GetRegion().Right - 5, GetRegion().Y - 5, 10, 10));
                        g.FillRectangle(brush, new Rectangle(GetRegion().Right - 5, GetRegion().Bottom - 5, 10, 10));
                        break;
                    case ROIType.Circle:
                        g.DrawEllipse(GetPen(), GetRegion());
                        g.DrawLine(p, new Point(GetCenterPos().X - 10, GetCenterPos().Y), new Point(GetCenterPos().X + 10, GetCenterPos().Y));
                        g.DrawLine(p, new Point(GetCenterPos().X, GetCenterPos().Y - 10), new Point(GetCenterPos().X, GetCenterPos().Y + 10));
                        g.FillRectangle(brush, new Rectangle(GetCenterPos().X - 5, GetRegion().Y - 5, 10, 10));
                        g.FillRectangle(brush, new Rectangle(GetRegion().Left - 5, GetCenterPos().Y - 5, 10, 10));
                        g.FillRectangle(brush, new Rectangle(GetRegion().Right - 5, GetCenterPos().Y - 5, 10, 10));
                        g.FillRectangle(brush, new Rectangle(GetCenterPos().X - 5, GetRegion().Bottom - 5, 10, 10));
                        break;
                }
            }
        }

        public void DrawROIs(List<Rectangle> rects, TROI srcROI, Graphics g)
        {
            List<Rectangle> rectlist = new List<Rectangle>();
            foreach (Rectangle r in rects)
            {
                rectlist.Add(r);
            }

            foreach (Rectangle l in rectlist)
            {
                Rectangle n = new Rectangle(srcROI.GetRegion().X + l.X, srcROI.GetRegion().Y + l.Y, l.Width, l.Height);
                g.DrawRectangle(new Pen(Color.Red, 5), n);
            }

        }


        ~TROI()
        {
            directionType = MouseCrossDirectionType.Center;
            roiType = ROIType.End;
        }
    }

    public class TPoint
    {
        public int X { get; set; }
        public int Y { get; set; }
        public BlobType type { get; set; }
        public int nLabel { get; set; }

        public TPoint()
        {
            X = 0;
            Y = 0;
            type = BlobType.END;
            nLabel = 0;
        }

        ~TPoint()
        {
            X = 0;
            Y = 0;
            type = BlobType.END;
            nLabel = 0;
        }
        
    }

    public class ImgLib
    {
        private TROI imageROI;
        private int imageWidth;
        private int imageHeight;
        private Bitmap DrawImage;
        private byte[] imageData;
        private List<TPoint> neighborList;
        private Dictionary<int, int> parent;
        List<Rectangle> rects;
        private int nLabel;
        public ImgLib(Bitmap srcImg)
        {
            DrawImage = srcImg;
            imageWidth = DrawImage.Width;
            imageHeight = DrawImage.Height;
            imageData = new byte[imageWidth * imageHeight];
            neighborList = new List<TPoint>();
            parent = new Dictionary<int, int>();
            nLabel = 100;
            rects = new List<Rectangle>();
        }

        ~ImgLib()
        {
            neighborList.Clear();
            parent.Clear();
            nLabel = 100;
            rects.Clear();
        }

        public Bitmap RGB2BW(TROI roi, Bitmap srcImage, int th)
        {
            imageWidth = srcImage.Width;
            imageHeight = srcImage.Height;
            int thVal = th;
            if(roi == null || srcImage == null)
            {
                return null;
            }
            DrawImage = new Bitmap(srcImage);
            Rectangle rect = roi.GetRegion();
            double RoundLength = Math.Sqrt((double)(Math.Pow(Math.Abs(rect.X - roi.GetCenterPos().X), 2) + Math.Pow(Math.Abs(rect.Y - roi.GetCenterPos().Y), 2)));
            BitmapData bmpData = DrawImage.LockBits(new Rectangle(rect.X, rect.Y, rect.Width, rect.Height), ImageLockMode.ReadWrite, DrawImage.PixelFormat);

            unsafe
            {
                int nRGBOffset = bmpData.Stride - bmpData.Width * (DrawImage.PixelFormat == PixelFormat.Format24bppRgb ? 3 : 4);
                byte* rgb_ptr = (byte*)(bmpData.Scan0);
                byte bwVal = 0;

                for (int y =rect.Y; y < (rect.Y +  rect.Height); y++)
                {
                    for (int x = rect.X; x < (rect.X+rect.Width); x++)
                    {
                        if (DrawImage.PixelFormat == PixelFormat.Format24bppRgb)
                        {
                            bwVal = (byte)((rgb_ptr[2] + rgb_ptr[1] + rgb_ptr[0]) / 3);
                        }
                        else
                        {
                            byte b = (byte)(rgb_ptr[2] * (rgb_ptr[3] / 255) + 255 - rgb_ptr[3]);
                            byte g = (byte)(rgb_ptr[1] * (rgb_ptr[3] / 255) + 255 - rgb_ptr[3]);
                            byte r = (byte)(rgb_ptr[0] * (rgb_ptr[3] / 255) + 255 - rgb_ptr[3]);
                            switch (roi.GetROIType())
                            {
                                case ROIType.Rectangle:
                                    bwVal = (byte)(((b + g + r) / 3) >= thVal ? 255 : 0);
                                    rgb_ptr[0] = bwVal;
                                    rgb_ptr[1] = bwVal;
                                    rgb_ptr[2] = bwVal;
                                    rgb_ptr[3] = rgb_ptr[3];
                                      break;
                                case ROIType.Circle:
                                    double sub_round = Math.Sqrt((double)(Math.Pow(Math.Abs(x - roi.GetCenterPos().X), 2) + Math.Pow(Math.Abs(y - roi.GetCenterPos().Y), 2)));

                                    if (((sub_round < rect.Width / 2)
                                        || (sub_round < rect.Height / 2))
                                        && (sub_round < RoundLength)
                                        )
                                    {
                                        bwVal = (byte)(((b + g + r) / 3) >= thVal ? 255 : 0);
                                        rgb_ptr[0] = bwVal;
                                        rgb_ptr[1] = bwVal;
                                        rgb_ptr[2] = bwVal;
                                        rgb_ptr[3] = rgb_ptr[3];
                                    }
                                        break;
                            }

                        }
                        rgb_ptr += (DrawImage.PixelFormat == PixelFormat.Format24bppRgb ? 3 : 4);
                    }
                    rgb_ptr += nRGBOffset;
                }
                DrawImage.UnlockBits(bmpData);
            }

            return DrawImage;
        }

        public Bitmap RGB2Gray(TROI roi, Bitmap srcImage)
        {
            DrawImage = new Bitmap(srcImage);
            imageWidth = DrawImage.Width;
            imageHeight = DrawImage.Height;
            if (roi == null || srcImage == null)
            {
                return null;
            }
            Rectangle rect = roi.GetRegion();

            double RoundLength = Math.Sqrt((double)(Math.Pow(Math.Abs(rect.X - roi.GetCenterPos().X), 2) + Math.Pow(Math.Abs(rect.Y - roi.GetCenterPos().Y), 2)));
            BitmapData bmpData = DrawImage.LockBits(new Rectangle(rect.X, rect.Y, rect.Width, rect.Height), ImageLockMode.ReadWrite, DrawImage.PixelFormat);

            unsafe
            {
                int nRGBOffset = bmpData.Stride - bmpData.Width * (DrawImage.PixelFormat == PixelFormat.Format24bppRgb ? 3 : 4);
                byte* rgb_ptr = (byte*)(bmpData.Scan0);
                byte grayVal = 0;

                for (int y = rect.Y; y < (rect.Y + rect.Height); y++)
                {
                    for (int x = rect.X; x < (rect.X + rect.Width); x++)
                    {
                        if (DrawImage.PixelFormat == PixelFormat.Format24bppRgb)
                        {
                            grayVal = (byte)((rgb_ptr[2] + rgb_ptr[1] + rgb_ptr[0]) / 3);
                        }
                        else
                        {
                            byte b = (byte)(rgb_ptr[2] * (rgb_ptr[3] / 255) + 255 - rgb_ptr[3]);
                            byte g = (byte)(rgb_ptr[1] * (rgb_ptr[3] / 255) + 255 - rgb_ptr[3]);
                            byte r = (byte)(rgb_ptr[0] * (rgb_ptr[3] / 255) + 255 - rgb_ptr[3]);

                            switch (roi.GetROIType())
                            {
                                case ROIType.Rectangle:
                                    grayVal = (byte)((b + g + r) / 3);
                                    rgb_ptr[0] = grayVal;
                                    rgb_ptr[1] = grayVal;
                                    rgb_ptr[2] = grayVal;
                                    rgb_ptr[3] = rgb_ptr[3];
                                    break;
                                case ROIType.Circle:
                                    double sub_round = Math.Sqrt((double)(Math.Pow(Math.Abs(x - roi.GetCenterPos().X), 2) + Math.Pow(Math.Abs(y - roi.GetCenterPos().Y), 2)));
                                    if (((sub_round < rect.Width / 2)
                                        || (sub_round < rect.Height / 2))
                                        && (sub_round < RoundLength)
                                        )
                                    {
                                        grayVal = (byte)((b + g + r) / 3);
                                        rgb_ptr[0] = grayVal;
                                        rgb_ptr[1] = grayVal;
                                        rgb_ptr[2] = grayVal;
                                        rgb_ptr[3] = rgb_ptr[3];
                                    }
                                        break;
                            }

                        }
                        rgb_ptr += (DrawImage.PixelFormat == PixelFormat.Format24bppRgb ? 3 : 4);
                    }
                    rgb_ptr += nRGBOffset;
                }
                DrawImage.UnlockBits(bmpData);
            }
            return DrawImage;
        }
        private byte[] ConvertBitmapToByteArray(Bitmap srcImage, TROI roi)
        {
            if (roi != null)
            {
                DrawImage = new Bitmap(srcImage);
                imageData = new byte[roi.GetRegion().Width * roi.GetRegion().Height];
                Rectangle rect = roi.GetRegion();
                int posX = rect.X;
                int posY = rect.Y;
                int xbound = rect.X + rect.Width;
                int ybound = rect.Y + rect.Height;
                int x = 0;
                int y = 0;
                double RoundLength = Math.Sqrt((double)(Math.Pow(Math.Abs(rect.X - roi.GetCenterPos().X), 2) + Math.Pow(Math.Abs(rect.Y - roi.GetCenterPos().Y), 2)));
                BitmapData bmpData = DrawImage.LockBits(new Rectangle(rect.X, rect.Y, rect.Width, rect.Height), ImageLockMode.ReadWrite, DrawImage.PixelFormat);
                unsafe
                {
                    int nOffset = bmpData.Stride - bmpData.Width * (DrawImage.PixelFormat == PixelFormat.Format24bppRgb ? 3 : 4);
                    byte* pixel_ptr = (byte*)(bmpData.Scan0);

                    for(posY = rect.Y; posY < ybound; posY++, y++)
                    {
                        int w = (rect.Width * (y + 1));
                        for (posX = rect.X; posX < xbound && x < w; posX++, x++)
                        {
                            switch(roi.GetROIType())
                            { 

                                case ROIType.Rectangle:
                                    imageData[x] = pixel_ptr[0];
                                break;
                                case ROIType.Circle:
                                    double sub_round = Math.Sqrt((double)(Math.Pow(Math.Abs(posX - roi.GetCenterPos().X), 2) + Math.Pow(Math.Abs(posY - roi.GetCenterPos().Y), 2)));
                                    if (((sub_round < rect.Width / 2)
                                        || (sub_round < rect.Height / 2))
                                        && (sub_round < RoundLength)
                                        )
                                    {
                                        imageData[x] = pixel_ptr[0];
                                    }
                                    break;
                            }
                            pixel_ptr += (DrawImage.PixelFormat == PixelFormat.Format24bppRgb ? 3 : 4);
                        }
                        pixel_ptr += nOffset;

                    }
                }
                DrawImage.UnlockBits(bmpData);
            }
            return imageData;
        }

        private byte[,] TransImageDataToDualArray(byte[] srcData, int width, int height)
        {
            byte[,] dstData = new byte[width, height];
            int x = 0;
            int y = 0;

            for (y = 0; y < height - 1; y++)
            {
                int w = width * (y+1);
                for (; x < w; x++)
                {
                    TPoint pos = new TPoint();
                    pos.X = x - (width * y);
                    pos.Y = y;
                    dstData[pos.X, pos.Y] = srcData[x];
                }
            }
            return dstData;
        }

        private byte[,] Labeling(byte[]srcImg, int width, int height, ref int cnt, BlobType btype)
        {
            byte[,] rstLabel = new byte [width, height];
            byte[,] labels = new byte[width, height];
            int label = nLabel;
            rstLabel = TransImageDataToDualArray(srcImg, width, height);
            

            for(int y = 0; y < height; y++)
            {
                for(int x = 0; x < width; x++)
                {
                    if (rstLabel[x, y] == (byte)btype)
                    {
                        if (label >= (int)(width* height))
                        {
                            return null;
                        }
                        LabelSet(rstLabel, x, y, width, height, label, btype);
                        label++;
                    }
                }
            }
            cnt = label - nLabel; 

            return rstLabel;
        }
        private void LabelSet(byte[,] srcData, int sub_x, int sub_y, int width, int height, int label, BlobType blobType)
        {
            int x = 0;
            int y = 0;
            int label_cnt = 0;
            int up = 0;
            int down = 0;
            int left = 0;
            int right = 0;
            srcData[sub_x, sub_y] = (byte)label;

            while (true)
            {
                label_cnt = 0;
                for(y = 0; y < height; y++)
                {
                    for (x = 0;x < width; x++)
                    {
                        if (srcData[x,y] == (byte)label)
                        {
                            up = (y - 1) < 0 ? 0 :(y-1);
                            down = (y + 1) > (height - 1) ? (height - 1) : (y + 1);
                            left = x - 1 < 0 ? 0 : (x - 1);
                            right = (x + 1) > (width-1) ? (width - 1) : (x+1);

                            if (srcData[right,y] == (byte)blobType)
                            {
                                srcData[right, y] = (byte)label;
                                label_cnt++;
                            }
                            if (srcData[right, up] == (byte)blobType)
                            {
                                srcData[right, up] = (byte)label;
                                label_cnt++;
                            }
                            if (srcData[x, up] == (byte)blobType)
                            {
                                srcData[x, up] = (byte)label;
                                label_cnt++;
                            }
                            if (srcData[left, up] == (byte)blobType)
                            {
                                srcData[left, up] = (byte)label;
                                label_cnt++;
                            }
                            if (srcData[left, y] == (byte)blobType)
                            {
                                srcData[left, y] = (byte)label;
                                label_cnt++;
                            }
                            if (srcData[right, down] == (byte)blobType)
                            {
                                srcData[right, down] = (byte)label;
                                label_cnt++;
                            }
                            if (srcData[x, down] == (byte)blobType)
                            {
                                srcData[x, down] = (byte)label;
                                label_cnt++;
                            }
                            if (srcData[right, down] == (byte)blobType)
                            {
                                srcData[right, down] = (byte)label;
                                label_cnt++;
                            }
                        }
                    }
                }
                if (label_cnt ==0)
                {
                    break;
                }
            }
        }

        private Rectangle GroupLabels(byte[,] srcLabelData, int x, int y ,int width, int height, int label, ref bool found)
        {
            Rectangle pos;
            bool isfound = false;
            int lx = 0;
            int ty = 0;
            int rx = 0;
            int by = 0;
            int w = 0;
            int h = 0;

            for(y = 0; y < height - 1; y++)
            {
                for (x = 0; x < width - 1; x++)
                {
                    if (srcLabelData[x, y] == (byte)label)
                    {
                        if (isfound == false)
                        {
                            lx = x;
                            rx = x;
                            ty = y;
                            by = y;
                            isfound = true;
                        }
                        else
                        {
                            lx = (lx >= x ? x : lx);
                            rx = (rx <= x ? x : rx);
                            ty = (ty >= y ? y : ty);
                            by = (by <= y ? y : by);
                        }
                    }
                }
            }

            found = isfound;
            pos = new Rectangle(lx, ty, rx - lx, by - ty);

            return pos;

        }

        public List<Rectangle> BlobObject(TROI roi, Bitmap srcImage, BlobType blobType, int th)
        {
            DrawImage = new Bitmap(srcImage);
            Bitmap bittmp = null;
            imageHeight = srcImage.Height;
            imageWidth = srcImage.Width;
            byte[] tmpData;
            int posX = 0;
            int posY = 0;
            int x = 0;
            int y = 0;
            int xbound = 0;
            int ybound = 0;
            byte[,] newLabels;
            int labelCnt = 0;
            PictureBox pbox = new PictureBox();
            rects.Clear();

            if (roi != null)
            {
                Rectangle rect = roi.GetRegion();
                var rand = new Random();
                var colorMap = new Dictionary<int, Color>();
                colorMap[0] = Color.Black; // 背景
                bittmp = RGB2BW(roi, DrawImage, th);
                double RoundLength = Math.Sqrt((double)(Math.Pow(Math.Abs(rect.X - roi.GetCenterPos().X), 2) + Math.Pow(Math.Abs(rect.Y - roi.GetCenterPos().Y), 2)));
                posX = roi.GetRegion().X;
                posY = roi.GetRegion().Y;
                xbound = posX + roi.GetRegion().Width;
                ybound = posY + roi.GetRegion().Height;
                tmpData = new byte[roi.GetRegion().Width * roi.GetRegion().Height];
                tmpData = ConvertBitmapToByteArray(bittmp, roi);
                newLabels = Labeling(tmpData, roi.GetRegion().Width, roi.GetRegion().Height, ref labelCnt, blobType);

                for(int id = 0; id < labelCnt; id++)
                {
                   bool found = false;
                   Rectangle r = GroupLabels(newLabels, 0, 0, rect.Width, rect.Height, nLabel + id, ref found);
                    if (found == true)
                    {
                        rects.Add(r);
                    }
                }

                return rects;
            }
            return rects;          
        }
    }
}
