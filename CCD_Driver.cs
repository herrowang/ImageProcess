using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DALSA.SaperaLT.SapClassBasic;

namespace ImageProcess
{
    public class CCD_DriverEventNotifier: EventArgs
    {
        public SapManager.EventType eventType;
        public SapManager.Server server;
    }
    public class CCD_Driver
    {
        public event EventHandler<CCD_DriverEventNotifier> CCD_NotifierEventHanlder;
        private SapData acqData;
        private SapLocation acqLocation;
        private SapAcqDevice acqDevice;
        private SapAcquisition acquisition;
        private SapBuffer acqBuffer;
        private SapTransfer Xfer;
        private SapFeature acqFeature;
        private SapView View;
        private SapManager.EventType eventType;
        private String sCCDName;
        public static float lastFrameRate = 0.0f;
        public int nDeviceCount;
        public int nCCDCount;
        public CCD_Driver(string sDevName, int nDevID)
        {
            nDeviceCount = 0;
            nCCDCount = 0;
            sCCDName = string.Empty;
            if (SapManager.DetectAllServers(SapManagerBase.DetectServerType.All))
            {
                nDeviceCount = SapManager.GetServerCount();
            }
            else
            {
                MessageBox.Show("No CCD count is found!!");
            }


            SapManager.ServerNotify += new SapServerNotifyHandler(CCD_Notifier);

            eventType = SapManager.EventType.ServerNew | SapManager.EventType.ServerConnected | SapManager.EventType.ServerDisconnected;
            SapManager.ServerEventType = eventType;

            if (false == GetCCDDByName(nDeviceCount))
            {
                MessageBox.Show("No CCD resource is found!!");
                return;
            }
            else
            {
                if (SapManager.GetResourceCount(sDevName, SapManager.ResourceType.AcqDevice) > 0)
                {
                    acquisition.EnableEvent(SapAcquisition.AcqEventType.StartOfFrame);
                }
                else
                {
                    if (acqDevice != null)
                    {
                        acqDevice.Dispose();
                    }
                    MessageBox.Show("No CCD resource is found!!");
                }
            }
        }

        ~CCD_Driver()
        {
            if (acqBuffer != null)
            {
                acqBuffer.Dispose();
                acqBuffer = null;
            }
            if (Xfer != null)
            {
                Xfer.Dispose();
                Xfer = null;
            }
            if (acquisition != null)
            {
                acquisition.Dispose();
                acquisition = null;
            }
            if (View != null)
            {
                View.Dispose();
                View = null;
            }
        }

        private void DestoryObject()
        {
            if (acqBuffer != null)
            {
                acqBuffer.Dispose();
                acqBuffer = null;
            }
            if (Xfer != null)
            {
                Xfer.Dispose();
                Xfer = null;
            }
            if (acquisition != null)
            {
                acquisition.Dispose();
                acquisition = null;
            }
            if (View != null)
            {
                View.Dispose();
                View = null;
            }
        }

        private bool GetCCDDByName(int nTotalCCD)
        {
            bool ret = false;
            int nCCDIdx = 0;

            for(nCCDIdx = 0; nCCDIdx < nTotalCCD; nCCDIdx++)
            {
                nDeviceCount = SapManager.GetResourceCount(nCCDIdx, SapManager.ResourceType.AcqDevice);
                nCCDCount = SapManager.GetResourceCount(nCCDIdx, SapManager.ResourceType.Acq);
                if ((nDeviceCount + nCCDCount) != 0)
                {
                    sCCDName = SapManager.GetServerName(nCCDIdx);
                    acqLocation = new SapLocation(sCCDName, nCCDIdx);
                    acqDevice = new SapAcqDevice(acqLocation);
                    ret = acqDevice.Create();

                    acquisition = new SapAcquisition(acqLocation, "test");
                    acqBuffer = new SapBuffer(1, acquisition, SapBuffer.MemoryType.ScatterGather);
                    Xfer = new SapAcqToBuf(acquisition, acqBuffer);
                    View = new SapView(acqBuffer);

                    if (!acquisition.Create())
                    {
                        DestoryObject();
                        return false;
                    }

                    // End of frame event
                    Xfer.Pairs[0].EventType = SapXferPair.XferEventType.EndOfFrame;
                    Xfer.XferNotify += new SapXferNotifyHandler(xfer_XferNotify);
                    Xfer.XferNotifyContext = View;

                    if (ret && acqDevice.FeatureCount > 0)
                    {
                        sCCDName = SapManager.GetServerName(nCCDIdx);
                        nCCDIdx++;
                    }
                    break;
                }
            }
            return ret;
        }

        static void xfer_XferNotify(object sender, SapXferNotifyEventArgs args)
        {
            // refresh view
            SapView View = args.Context as SapView;
            View.Show();

            // refresh frame rate
            SapTransfer transfer = sender as SapTransfer;
            if (transfer.UpdateFrameRateStatistics())
            {
                SapXferFrameRateInfo stats = transfer.FrameRateStatistics;
                float framerate = 0.0f;

                if (stats.IsLiveFrameRateAvailable)
                    framerate = stats.LiveFrameRate;

                // check if frame rate is stalled
                if (stats.IsLiveFrameRateStalled)
                {
                    Console.WriteLine("Live Frame rate is stalled.");
                }

                // update FPS only if the value changed by +/- 0.1
                else if ((framerate > 0.0f) && (Math.Abs(lastFrameRate - framerate) > 0.1f))
                {
                    Console.WriteLine("Grabbing at {0} frames/sec", framerate);
                    lastFrameRate = framerate;
                }
            }
        }

        private void CCD_Notifier(object sender, SapServerNotifyEventArgs e)
        {
            switch (e.EventType)
            {
                case SapManager.EventType.ServerNew:
                    break;
                case SapManager.EventType.ServerDisconnected:
                    break;
                case SapManager.EventType.ServerConnected:
                    break;
                case SapManager.EventType.None:
                    break;
            }

            CCD_DriverEventNotifier notifier = new CCD_DriverEventNotifier();
            notifier.eventType = e.EventType;
        }

        public void OnCCDNotifier(CCD_DriverEventNotifier notifier)
        {
            EventHandler<CCD_DriverEventNotifier> handler = CCD_NotifierEventHanlder;
            if (handler != null)
            {
                handler(this, notifier);
            }
        }
    }
}
