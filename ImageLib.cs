using ImageProcess;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Security.Policy;
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
        DualRectangle = 1,
        Circle,
        MeasureLine,
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

    public enum MeasureDirect
    {
        Horhorizontal = 0,
        Vertical,
        End,
    }

    public class TLine
    {
        public Point start;
        public Point end;

        public TLine()
        {
            start = new Point(0, 0);
            end = new Point(0, 0);
        }

        ~TLine()
        {
            start = new Point(0, 0);
            end = new Point(0, 0);
        }
    }
    public class TROI
    {
        private MouseCrossDirectionType directionType { get; set; }
        private ROIType roiType { get; set; }
        private Rectangle rect;
        public List<Rectangle> rects;
        public List<Rectangle> measureRois;
        public int nMaxROIs;
        private int nWidth { get; set; }
        private int nHeight { get; set; }
        private Point Center;
        private Pen pen;
        private TLine line;
        private List<Point> paths;
        private int nLength;
        public TPoint gravity;

        public TROI(Point pos, Size s, ROIType rType)
        {
            directionType = MouseCrossDirectionType.Center;
            roiType = rType;
            rect = new Rectangle(pos, s);
            pen = new Pen(Color.Red, 2);
            rects = new List<Rectangle>();
            measureRois = new List<Rectangle>();
            paths = new List<Point>();
            Center = new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
            nWidth = rect.Width;
            nHeight = rect.Height;
            nLength = 0;
            gravity = new TPoint();
        }

        public TROI(int x, int y, int width, int height, ROIType rType)
        {
            directionType = MouseCrossDirectionType.Center;
            roiType = rType;
            nMaxROIs = rType == ROIType.DualRectangle ? 2 : 1;
            line = new TLine();
            rect = new Rectangle(x, y, width, height);
            pen = new Pen(Color.Red, 2);
            rects = new List<Rectangle>();
            measureRois = new List<Rectangle>();
            Center = new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
            nWidth = rect.Width;
            nHeight = rect.Height;
            paths = new List<Point>();
            nLength = 0;
            gravity = new TPoint();
        }

        public bool SetCenterPos(Point pos)
        {
            Center = new Point(pos.X, pos.Y);

            return true;
        }

        public bool SetPathes(List<Point> points)
        {
            if (paths == null)
            {
                return false;
            }

            paths = points;

            return true;
        }

        public bool SetPathes(List<Point> points)
        {
            if (paths == null)
            {
                return false;
            }

            paths = points;

            return true;
        }

        public  void SetLength(int length)
        {
            nLength = length;
        }

        public List<Point>GetPathes()
        {
            return paths;
        }

        public Point GetCenterPos()
        {
            return Center;
        }

        public Rectangle GetRegion()
        {
            return rect;
        }

        public void updateRegions(Rectangle new_rect)
        {
            rect = new_rect;
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

        public int GetLength()
        {
            return nLength;
        }

        public void SetStartPosition(Point pos)
        {
            this.line.start.X = pos.X;
            this.line.start.Y = pos.Y;
        }

        public void SetRegionPos(TLine inLine)
        {
            rect.X = inLine.start.X- 10;
            rect.Y = inLine.start.Y - 10;
            rect.Width = ((inLine.end.X >= inLine.start.X) ? (inLine.end.X - rect.X) : (rect.X - inLine.end.X)) + 10;
            rect.Height = ((inLine.end.Y >= inLine.start.Y) ? (inLine.end.Y - rect.Y) : (rect.Y - inLine.end.Y)) + 10;
            Center = new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
        }

        public void SetEndPosition(Point pos)
        {
            this.line.end.X = pos.X;
            this.line.end.Y = pos.Y;
        }

        public TLine GetLine()
        {
            return line;
        }

        public TLine GetHorizontalLine(TROI srcROI)
        {
            TLine found_ine = new TLine();
            foreach (Point max_p in srcROI.GetPathes())
            {
                if (found_ine.start.X >= max_p.X)
                {
                    found_ine.start.X = max_p.X;
                    found_ine.start.Y = max_p.Y;
                }
                if (found_ine.end.X <= max_p.X)
                {
                    found_ine.end.X = max_p.X;
                    found_ine.end.Y = max_p.Y;
                }
            }
            return found_ine;
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

            if (orgROI.roiType == ROIType.MeasureLine)
            {
                this.line.end.X = mPos.X;
                this.line.end.Y = mPos.Y;
            }
            else if (orgROI.roiType == ROIType.End)
            {
                int nX = mPos.X;
                int nY = mPos.Y;
                width = Math.Abs(mPos.X - orgROI.GetRegion().X);
                height = Math.Abs(mPos.Y -  orgROI.GetRegion().Y);
                if (mPos.X < orgROI.GetRegion().X)
                {
                    nX = mPos.X;
                }
                if (mPos.Y < orgROI.GetRegion().Y)
                {
                    nX = mPos.Y;
                }
                rect = new Rectangle(nX, nY, width, height);
            }
            else
            {
                switch (orgROI.GetDirectionType())
                {
                    case MouseCrossDirectionType.Center:
                        orgROI.SetCenterPos(mPos);
                        rect.X = orgROI.GetCenterPos().X - (rect.Width / 2);
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
            }

            nWidth = width;
            nHeight = height;

            return newROI;
        }   

        public void DrawPoints(Graphics g, List<Point>pos_list, Rectangle r)
        {
            foreach(Point p in pos_list)
            {
                g.FillRectangle(Brushes.Red, new Rectangle(r.X + p.X, r.Y + p.Y, 5, 5));
            }
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
                        Point lpoint = new Point(0, 0);
                        Point rpoint = new Point(0, 0);
                        Point tpoint = new Point(0, 0);
                        Point bpoint = new Point(0, 0);
                        Point gravityP = new Point(0, 0);
                        foreach (Point subp in paths)
                        {
                            lpoint = (lpoint.X == 0 || lpoint.X >= subp.X) ? subp : lpoint;
                            rpoint = (rpoint.X == 0 || rpoint.X <= subp.X) ? subp : rpoint;
                            tpoint = (tpoint.Y == 0 || tpoint.Y >= subp.Y) ? subp : tpoint;
                            bpoint = (tpoint.Y == 0 || bpoint.Y <= subp.Y) ? subp : bpoint;
                            Rectangle rp = new Rectangle(GetRegion().X + subp.X, GetRegion().Y + subp.Y, 5, 5);
                            g.FillRectangle(Brushes.Yellow, rp);
                        }
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
                    case ROIType.DualRectangle:
                        foreach (Rectangle r in measureRois)
                        {
                            g.DrawRectangle(GetPen(), r);
                        }
                        if (measureRois.Count < nMaxROIs)
                        {
                            g.DrawRectangle(GetPen(), GetRegion());
                        }
                        break;
                }
            }
        }

        public void DrawROIs(List<TROI> rects, TROI srcROI, Graphics g)
        {
            List<TROI> rectlist = new List<TROI>();
            Random randon = new Random(255);
            foreach (TROI r in rects)
            {
                rectlist.Add(r);
            }

            int idx = 1;
            foreach (TROI l in rectlist)
            {
                Rectangle n = new Rectangle(srcROI.GetRegion().X + l.GetRegion().X, srcROI.GetRegion().Y + l.GetRegion().Y, l.GetRegion().Width, l.GetRegion().Height);
                Color c = Color.FromArgb(randon.Next(0, 255), randon.Next(0, 255), randon.Next(0, 255));
                Point lpoint = new Point(0, 0);
                Point rpoint = new Point(0, 0);
                Point tpoint = new Point(0, 0);
                Point bpoint = new Point(0, 0);
                Point gravityP = new Point(0, 0);
                //g.DrawRectangle(new Pen(Color.Red,5), n);
                foreach (Point p in l.paths)
                {
                    lpoint = (lpoint.X == 0 || lpoint.X >= p.X) ? p : lpoint;
                    rpoint = (rpoint.X == 0 || rpoint.X <= p.X) ? p : rpoint;
                    tpoint = (tpoint.Y == 0 || tpoint.Y >= p.Y) ? p : tpoint;
                    bpoint = (tpoint.Y == 0 || bpoint.Y <= p.Y) ? p : bpoint;
                    Rectangle rp = new Rectangle(srcROI.GetRegion().X +  p.X, srcROI.GetRegion().Y  + p.Y, 5, 5);
                    g.FillRectangle(Brushes.Yellow, rp);
                }
                g.DrawString(idx.ToString(), new Font("新細明體", 10, FontStyle.Bold), Brushes.Red, new Point(n.Left + n.Width / 2, n.Top + n.Height / 2));
#if false
                gravityP = new Point(srcROI.GetRegion().X + l.gravity.X, srcROI.GetRegion().Y + l.gravity.Y);
                lpoint = new Point(srcROI.GetRegion().X + lpoint.X, srcROI.Center.Y);
                rpoint = new Point(srcROI.GetRegion().X + rpoint.X, srcROI.Center.Y);
                tpoint = new Point(srcROI.Center.X, srcROI.GetRegion().Y + tpoint.Y);
                bpoint = new Point(srcROI.Center.X, srcROI.GetRegion().Y + bpoint.Y);
                g.DrawLine(new Pen(Color.Green, 4), lpoint, rpoint);
                g.DrawLine(new Pen(Color.Green, 4), tpoint, bpoint);
                g.DrawLine(new Pen(Color.Red, 4), new Point(lpoint.X, lpoint.Y - 10), new Point(lpoint.X, lpoint.Y + 10));
                g.DrawLine(new Pen(Color.Red, 4), new Point(rpoint.X, rpoint.Y - 10), new Point(rpoint.X, rpoint.Y + 10));
                g.DrawLine(new Pen(Color.Red, 4), new Point(tpoint.X -10, tpoint.Y), new Point(tpoint.X + 10, tpoint.Y));
                g.DrawLine(new Pen(Color.Red, 4), new Point(bpoint.X -10, bpoint.Y), new Point(bpoint.X + 10, bpoint.Y));
                g.DrawLine(new Pen(Color.Cyan, 4), new Point(gravityP.X - 10, gravityP.Y), new Point(gravityP.X + 10, gravityP.Y));
                g.DrawLine(new Pen(Color.Cyan, 4), new Point(gravityP.X, gravityP.Y - 10), new Point(gravityP.X, gravityP.Y + 10));
#endif
                idx++;
            }

        }

        public void DrawLine(TROI roi, Graphics g, MeasureDirect direct)
        {
            SolidBrush brush = new SolidBrush(Color.Aqua);
            Point stringPos = new Point(0, 0);
            int nLength = 0;

            Point lpoint = new Point(0, 0);
            Point rpoint = new Point(0, 0);
            Point tpoint = new Point(0, 0);
            Point bpoint = new Point(0, 0);
            Point gravityP = new Point(0, 0);
            foreach (Point p in roi.paths)
            {
                lpoint = (lpoint.X == 0 || lpoint.X >= p.X)? p : lpoint;
                rpoint = (rpoint.X == 0 || rpoint.X <= p.X) ? p : rpoint;
                tpoint = (tpoint.Y == 0 || tpoint.Y >= p.Y) ? p : tpoint;
                bpoint = (tpoint.Y == 0 || bpoint.Y <= p.Y) ? p : bpoint;
                Rectangle rp = new Rectangle(GetRegion().X + p.X, GetRegion().Y + p.Y, 2, 2);
                g.FillRectangle(Brushes.Red, rp);
            }
            if (roi.paths.Count > 0)
            {
                switch (direct)
                {
                    case MeasureDirect.Horhorizontal:
                        stringPos = new Point(GetRegion().X + lpoint.X + (Math.Abs(lpoint.X - rpoint.X) / 2), GetRegion().Y + lpoint.Y);
                        nLength = Math.Abs(lpoint.X - rpoint.X);
                        g.DrawLine(new Pen(Color.Red, 4), new Point(GetRegion().X + lpoint.X, GetRegion().Y + lpoint.Y), new Point(GetRegion().X + rpoint.X, GetRegion().Y + rpoint.Y));
                        break;
                    case MeasureDirect.Vertical:
                        stringPos = new Point(GetRegion().X + lpoint.X + (Math.Abs(lpoint.X - rpoint.X) / 2), GetRegion().Y + lpoint.Y);
                        nLength = Math.Abs(bpoint.Y - tpoint.Y);
                        g.DrawLine(new Pen(Color.Red, 4), new Point(GetRegion().X + tpoint.X, GetRegion().Y + tpoint.Y), new Point(GetRegion().X + bpoint.X, GetRegion().Y + bpoint.Y));
                        break;

                }
            }
            else
            {
                g.DrawLine(new Pen(Color.Yellow, 4), roi.GetLine().start, roi.GetLine().end);
                switch (direct)
                {
                    case MeasureDirect.Vertical:
                        
                        g.DrawLine(new Pen(Color.Red, 4), new Point(roi.GetLine().start.X - 10, roi.GetLine().start.Y), new Point(roi.GetLine().start.X + 10, roi.GetLine().start.Y));
                        g.DrawLine(new Pen(Color.Red, 4), new Point(roi.GetLine().end.X - 10, roi.GetLine().end.Y), new Point(roi.GetLine().end.X + 10, roi.GetLine().end.Y));
                        break;
                    case MeasureDirect.Horhorizontal:
                        g.DrawLine(new Pen(Color.Red, 4), new Point(roi.GetLine().start.X, roi.GetLine().start.Y - 10), new Point(roi.GetLine().start.X, roi.GetLine().start.Y + 10));
                        g.DrawLine(new Pen(Color.Red, 4), new Point(roi.GetLine().end.X, roi.GetLine().end.Y - 10), new Point(roi.GetLine().end.X, roi.GetLine().end.Y + 10));
                        break;

                }
                stringPos = new Point(roi.GetLine().start.X + (Math.Abs(roi.GetLine().start.X - roi.GetLine().end.X) / 2), roi.GetLine().start.Y);
                nLength = Math.Abs(roi.GetLine().start.X - roi.GetLine().end.X);
            }
            g.DrawString(nLength.ToString(), new Font("新細明體", 10, FontStyle.Bold), Brushes.Red, stringPos);
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
        List<TROI> rects;
        public List<Point> detect_edges;
        private int nLabel;
        public ImgLib(Bitmap srcImg)
        {
            if (srcImg == null)
            {
                return;
            }
            DrawImage = srcImg;
            imageWidth = DrawImage.Width;
            imageHeight = DrawImage.Height;
            imageData = new byte[imageWidth * imageHeight];
            neighborList = new List<TPoint>();
            detect_edges = new List<Point>();
            parent = new Dictionary<int, int>();
            nLabel = 1;
            rects = new List<TROI>();
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

        public List<Point> EdgeDetection(byte[] srcImg, Rectangle detect_range ,int width, int height)
        {
            int x = 0;
            int y = 0;
            int label_cnt = 0;
            int up = 0;
            int down = 0;
            int left = 0;
            int right = 0;
            int nPeakVal = 0;
            Point p = new Point();
            byte[,] edgeArea = new byte[width, height];
            List<Point>edges = new List<Point>();
            edgeArea = TransImageDataToDualArray(srcImg, width, height);

            for (y = 0; y < detect_range.Height; y++)
            {
                for (x = 0; x < detect_range.Width; x++)
                {
                    up = (y - 1) < 0 ? 0 : (y - 1);
                    down = (y + 1) > (height - 1) ? (height - 1) : (y + 1);
                    left = x - 1 < 0 ? 0 : (x - 1);
                    right = (x + 1) > (width - 1) ? (width - 1) : (x + 1);
                    if (nPeakVal <= Math.Abs(edgeArea[x, y] - edgeArea[left, y]))
                    {
                        nPeakVal = Math.Abs(edgeArea[x, y] - edgeArea[left, y]);
                        p.X = x;
                        p.Y = y;
                    }
                    if (nPeakVal <= Math.Abs(edgeArea[x, y] - edgeArea[right, y]))
                    {
                        nPeakVal = Math.Abs(edgeArea[x, y] - edgeArea[right, y]);
                        p.X = x;
                        p.Y = y;
                    }
                    if (nPeakVal <= Math.Abs(edgeArea[x, y] - edgeArea[x, up]))
                    {
                        nPeakVal = Math.Abs(edgeArea[x, y] - edgeArea[x, up]);
                        p.X = x;
                        p.Y = y;
                    }
                    if (nPeakVal <= Math.Abs(edgeArea[x, y] - edgeArea[x, down]))
                    {
                        nPeakVal = Math.Abs(edgeArea[x, y] - edgeArea[x, down]);
                        p.X = x;
                        p.Y = y;
                    }
                }
                if (nPeakVal > 0)
                    edges.Add(p);
            }
            return edges;
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
                                case ROIType.MeasureLine:
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
        /*
         * Function Name: Labeling
         * Description:process labels
         * input:
         *       srcImg:byte type image array
         *       width:ROI width
         *       height:ROI height
         *       label:label id
         *       blobType:Blob type(white or black)
         * oubut: 
         *       cnt:final labels count
         *       dual byte labeled array
         */
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
                        if (label == (int)btype)
                        {
                            label++;
                        }
                        label++;
                    }
                }
            }
            cnt = label - nLabel; 

            return rstLabel;
        }
        /*
         * Function Name: LabelSet
         * Description:mark labels
         * input:
         *       srcData:byte type image array
         *       sub_x, sub_y:started postition
         *       width:ROI width
         *       height:ROI height
         *       label:label id
         *       blobType:Blob type(white or black)
         * oubut: none
         */
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
                         }
                    }
                }
                if (label_cnt ==0)
                {
                    break;
                }
            }
        }

        /*
            * Function Name: TracePath
            * Description:Image contour tracing 
            * input:
            *       srcData:byte type image array
            *       width:ROI width
            *       height:ROI height
            *       label:label id
            * ouput: List<Point>
            */
        private List<Point> TracePath(byte[,] srcLabelData, int width, int height, int label, ref int length)
        {
            List<Point> tracePos = new List<Point>();
            Point pos = new Point(0, 0);
            int x = 0;
            int y = 0;
            int lx = 0;
            int rx = 0;
            int ty = 0;
            int by = 0;
            bool started = false;
            length = 0;
            for (y = 0; y < height - 1; y++)
            {
                for (x = 0; x < width - 1; x++)
                {
                    if (srcLabelData[x, y] == (byte)label)
                    {
                        lx = (x - 1) <= 0 ? x : (x - 1);
                        rx = (x + 1) >= width ? x : (x + 1);
                        ty = (y - 1) <= 0 ? y : (y - 1);
                        by = (y + 1) >= height ? y : (y + 1);
                        if (srcLabelData[rx, y] != (byte)label
                            || srcLabelData[lx, y] != (byte)label
                            || srcLabelData[x, ty] != (byte)label
                            || srcLabelData[x, by] != (byte)label)
                        {
                            pos = new Point(x, y);
                            tracePos.Add(pos);
                            length++;
                        }
                    }
                }
            }
            return tracePos;
        }

        /*
            * Function Name: CalculateGravityPoint
            * Description:Image contour tracing 
            * input:
            *       srcData:byte type image array
            *       width:ROI width
            *       height:ROI height
            *       label:label id
            * ouput: List<Point>
            */
        public TPoint CalculateGravityPoint(byte[,]srcData, int width, int height, int label, ref int nSize )
        {
            TPoint gPoint = new TPoint();
            int x = 0;
            int y = 0;
            int cx = 0;
            int cy = 0;
            int totalSize = 0;

            for(y = 0; y < height; y++)
            {
                for(x = 0; x < width; x++)
                {
                    if (srcData[x, y] == label)
                    {
                        cx+=x;
                        cy+=y;
                        totalSize++;
                    }
                }
            }

            if (totalSize > 0)
            {
                gPoint.nLabel = label;
                gPoint.X = (cx / totalSize);
                gPoint.Y = (cy / totalSize);
                gPoint.type = BlobType.END;
            }

            return gPoint;
        }

        /*
         * Function Name: GroupLabels
         * Description:Group Labels
         * input:srcLabelData, x, y, width, height, label
         * oubut: found, reectangle
         */
        private TROI GroupLabels(byte[,] srcLabelData, int x, int y ,int width, int height, int label, ref bool found)
        {
            TROI pos;
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
            pos = new TROI(new Point(lx, ty), new Size(rx - lx, by - ty), ROIType.Rectangle);

            return pos;

        }

        /*
         * Function Name: BlobObject
         * Description:blob objects
         * input:roi, srcImage, blobType,th
         * oubut: rectangle list
         */
        public List<TROI> BlobObject(TROI roi, Bitmap srcImage, BlobType blobType, int th)
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
                   int length = 0;
                    int ntotalSize = 0;
                   TROI r = GroupLabels(newLabels, 0, 0, rect.Width, rect.Height, nLabel + id, ref found);
                    
                    if (found == true)
                    {
                        r.SetPathes(TracePath(newLabels, rect.Width, rect.Height, nLabel + id, ref length));
                        r.SetLength(length);
                        r.gravity = CalculateGravityPoint(newLabels, rect.Width, rect.Height, nLabel + id, ref ntotalSize);
                        //r.SetGravity(gravityP);
                        rects.Add(r);
                    }
                }

                return rects;
            }
            return rects;          
        }

        public List<Point> FindEdge(TROI roi, Bitmap srcImage, ROIACTType actType, Rectangle edge_roi, int th)
        {
            DrawImage = new Bitmap(srcImage);
            Bitmap bittmp = null;
            imageHeight = srcImage.Height;
            imageWidth = srcImage.Width;
            byte[] tmpData;
            TROI Edge = new TROI(edge_roi.X, edge_roi.Y, edge_roi.Width, edge_roi.Height, ROIType.Rectangle);

            if (Edge != null)
            {
                Rectangle rect = Edge.GetRegion();
                bittmp = RGB2BW(Edge, DrawImage, th);
                tmpData = new byte[edge_roi.Width * edge_roi.Height];
                tmpData = ConvertBitmapToByteArray(bittmp, Edge);
                detect_edges = EdgeDetection(tmpData, edge_roi, edge_roi.Width, edge_roi.Height);
            }
            return detect_edges;
        }
        public TROI MeasureLine(TROI roi, Bitmap srcImage, ROIACTType actType, BlobType blobType ,int th)
        {
            DrawImage = new Bitmap(srcImage);
            Bitmap bittmp = null;
            imageHeight = srcImage.Height;
            imageWidth = srcImage.Width;
            TROI blobArea = new TROI(roi.GetRegion().X, roi.GetRegion().Y, roi.GetRegion().Width, roi.GetRegion().Height, ROIType.Rectangle);
            if (actType != ROIACTType.DrawLine)
            {
                return null;
            }

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
            if (rects != null)
            {
                rects.Clear();
            }

            if (blobArea != null)
            {
                Rectangle rect = blobArea.GetRegion();
                var rand = new Random();
                var colorMap = new Dictionary<int, Color>();
                colorMap[0] = Color.Black; // 背景
                bittmp = RGB2BW(blobArea, DrawImage, th);
                double RoundLength = Math.Sqrt((double)(Math.Pow(Math.Abs(rect.X - blobArea.GetCenterPos().X), 2) + Math.Pow(Math.Abs(rect.Y - blobArea.GetCenterPos().Y), 2)));
                posX = blobArea.GetRegion().X;
                posY = blobArea.GetRegion().Y;
                xbound = posX + blobArea.GetRegion().Width;
                ybound = posY + blobArea.GetRegion().Height;
                tmpData = new byte[blobArea.GetRegion().Width * blobArea.GetRegion().Height];
                tmpData = ConvertBitmapToByteArray(bittmp, blobArea);
                newLabels = Labeling(tmpData, blobArea.GetRegion().Width, blobArea.GetRegion().Height, ref labelCnt, blobType);

                for (int id = 0; id < labelCnt; id++)
                {
                    bool found = false;
                    int length = 0;
                    int ntotalSize = 0;
                    TROI r = GroupLabels(newLabels, 0, 0, rect.Width, rect.Height, nLabel + id, ref found);

                    if (found == true)
                    {
                        r.SetPathes(TracePath(newLabels, rect.Width, rect.Height, nLabel + id, ref length));
                        r.SetLength(length);
                        r.gravity = CalculateGravityPoint(newLabels, rect.Width, rect.Height, nLabel + id, ref ntotalSize);
                        blobArea = r;
                    }
                }
            }
            return blobArea;
        }
    }
}
