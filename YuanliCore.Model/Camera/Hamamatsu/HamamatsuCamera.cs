using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using YuanliCore.CameraLib;
using YuanliCore.Interface;

namespace Hamamatsu
{
    public class HamamatsuCamera : IIRCamera
    {
        private CamerErrorCode m_lasterr;
        private IntPtr m_hdcam;
        //   private DCAMCAP_START m_capmode = DCAMCAP_START.SEQUENCE;
        private int cameraID;
        private static Int32 m_devcount;
        private DCAMBUF_FRAME bufframe;
        private DCAMPROP_ATTR m_attr;
        private int frameCount = 3;
        // private MyLut m_lut;
        private HamamatsuImage m_image;
        private readonly object BitmapLock;
        private Bitmap m_bitmap;
        private Subject<Frame<byte[]>> subject = new Subject<Frame<byte[]>>();
        private System.Drawing.Imaging.PixelFormat pixelFormat = System.Drawing.Imaging.PixelFormat.Format24bppRgb;


        /// <summary>
        /// IR相機
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="Exception"></exception>
        public HamamatsuCamera(int id)
        {
            DCAMAPI_INIT param = new DCAMAPI_INIT(0);

        
            m_lasterr = Dcamapidll.dcamapi_init(ref param);
            m_devcount = (m_lasterr.failed() ? 0 : param.iDeviceCount);

            cameraID = id;

            m_image = new HamamatsuImage();


            BitmapLock = new object();

            if (m_lasterr.failed()) throw new Exception("Camera Initial fail");

        }
        public int Width => GetWidth();

        public int Height => GetHeight();

        public double Gain { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double ExposureTime { get => GetExposureTime(); set => throw new NotImplementedException(); }

        public IObservable<Frame<byte[]>> Frames => subject;

        public System.Windows.Media.PixelFormat PixelFormat { get => System.Windows.Media.PixelFormats.Rgb24; set => throw new NotImplementedException(); }


        public int LutMin { get; set; }
        public int LutMax { get; set; }
        public int Camerabpp { get; set; }        // camera bit per pixel.  This sample code only support MONO.
        public int Cameramax { get; set; }



        public void Close()
        {
            Stop();

            if (m_hdcam == IntPtr.Zero)
                return;    // already closed

            m_lasterr = Dcamdev.Close(m_hdcam);
            if (!m_lasterr.failed())
                m_hdcam = IntPtr.Zero;

            if (m_lasterr.failed()) throw new Exception("Camera Close fail");
        }

        public IDisposable Grab()
        {


           
            m_lasterr = Dcamapidll.dcambuf_alloc(m_hdcam, frameCount);
            if (m_lasterr.failed()) throw new Exception("Camera Alloc fail");

            // start acquisition
            //   m_cap_stopping = false;
            //  m_capmode = DCAMCAP_START.SNAP;    // one time capturing.  acqusition will stop after capturing m_nFrameCount frames

            //m_capmode = DCAMCAP_START.SEQUENCE;    // continuous capturing.  continuously acqusition will be done

            m_lasterr = CamcapCapturingControl.Start(m_hdcam, DCAMCAP_START.SEQUENCE);  //開始取像 串流模式 
            if (m_lasterr.failed())
            {
 
                m_lasterr = Dcamapidll.dcambuf_release(m_hdcam, 0);// release unnecessary buffer in DCAM
                throw new Exception("Camera Grab fail");


            }

            //更新 Lut數值 Camerabpp Cameramax  LutMin LutMax
            Update_lut(false, 0, 0);


            if (m_lasterr.failed()) throw new Exception("Camera Release fail");


            int _iprop = CamerPropertyID.TRIGGERSOURCE;
            double value = 0;
            m_lasterr = CameraProperty.Getvalue(m_hdcam, CamerPropertyID.TRIGGERSOURCE, ref value);
            m_attr = new DCAMPROP_ATTR(_iprop);
            if (value == DCAMPROP.TRIGGERSOURCE.SOFTWARE)
            {
                // change dialog FormStatus to AcquiringSoftwareTrigger
            }
            else
            {
                // change dialog FormStatus to Acquiring
            }

            //影像讀取到記憶體
            Task.Run(CaptureImage);         // start monitoring thread

            return Disposable.Create(Stop);
        }

        public BitmapSource GrabAsync()
        {
            var f = Frames.Take(1).Timeout(TimeSpan.FromSeconds(3)).FirstOrDefault();
            var result = f.ToBitmapSource();

            return result;
        }

        public void Open()
        {

            DCAMDEV_OPEN param = new DCAMDEV_OPEN(cameraID);// index of DCAM device.  This is used at allocating mydcam instance.
            m_lasterr = Dcamdev.Open(ref param);

            if (m_hdcam != IntPtr.Zero)
            {
                Dcamdev.Close(m_hdcam);
            }
            m_hdcam = param.hdcam;

            bufframe = new DCAMBUF_FRAME(0);
            if (m_lasterr.failed()) throw new Exception("Camera Open fail");

        }

        public void Stop()
        {

            m_lasterr = CamcapCapturingControl.Stop(m_hdcam);
            
            if (m_lasterr.failed()) throw new Exception("Camera Stop fail");


        }

        public void Dispose()
        {
            m_lasterr = Dcamapidll.dcamapi_uninit();

            if (m_lasterr.failed()) throw new Exception("Camera Dispose fail");

        }
        //自動調整 lut 值  (不知道是做什麼用的)
        public void Auto_lut()
        {

            int min = Cameramax;
            int max = 0;
            if (m_image.isValid())
            {
                int w = m_image.width;
                int h = m_image.height;

                Int16[] s = new Int16[w];

                // Displaying center of the image
                Int32 y0 = (m_image.height - h) / 2;
                Int32 x0 = (m_image.width - w) / 2;

                Int32 y;
                for (y = 0; y < h; y++)
                {
                    Int32 offset;

                    offset = m_image.bufframe.rowbytes * (y + y0) + (x0 * 2);// In bytes, so multiply by bpp
                    Marshal.Copy((IntPtr)(m_image.bufframe.buf.ToInt64() + offset), s, 0, w);

                    Int32 x;
                    for (x = 0; x < w; x++)
                    {
                        UInt16 u = (UInt16)s[x];
                        if (u > max)
                            max = u;
                        else if (u < min)
                            min = u;
                    }
                }
            }
            LutMax = max;
            LutMin = min;
            /*
            if (m_lut.inmax != max)
            {
                m_lut.inmax = max;
                HScrollLutMax.Value = m_lut.inmax;
                EditLutMax.Text = m_lut.inmax.ToString();
                bUpdatePicture = true;
            }

            if (m_lut.inmin != min)
            {
                m_lut.inmin = min;
                HScrollLutMin.Value = m_lut.inmin;
                EditLutMin.Text = m_lut.inmin.ToString();
                bUpdatePicture = true;
            }

            if (bUpdatePicture)
                MyUpdatePicture();
            */
        }
        private void CaptureImage()
        {
            try
            {
 
                bool bContinue = true;
                DCAMWAIT_OPEN param = new DCAMWAIT_OPEN(0);
                param.hdcam = m_hdcam;

                while (bContinue)
                {
                    DCAMWAIT eventmask = DCAMWAIT.CAPEVENT.FRAMEREADY | DCAMWAIT.CAPEVENT.STOPPED;
                    DCAMWAIT eventhappened = DCAMWAIT.NONE;
                    //相機等待事件功能
                    m_lasterr = CamWaitingCapture.Open(ref param);

                    if (m_lasterr.failed()) throw new Exception("Camera Capture fail");


                    //   m_supportevent = param.supportevent;
                    if (CaptureStart(param.hwait, eventmask, ref eventhappened))
                    {

                        if (eventhappened & DCAMWAIT.CAPEVENT.FRAMEREADY)
                        {
                            int iNewestFrame = 0;
                            int iFrameCount = 0;

                            if (Cap_transferinfo(ref iNewestFrame, ref iFrameCount))
                            {
                                CreateImage(iNewestFrame);
                            }
                        }

                        if (eventhappened & DCAMWAIT.CAPEVENT.STOPPED)
                        {
                            bContinue = false;
                            //  if (m_cap_stopping == false && mydcam.m_capmode == DCAMCAP_START.SNAP)
                            {
                                // in this condition, cap_stop() happens automatically, so update the main dialog
                                //   MySnapCaptureFinished();
                            }
                        }
                    }
                    else
                    {
                        if (m_lasterr == CamerErrorCode.TIMEOUT)
                        {
                            // nothing to do
                        }
                        else
                        if (m_lasterr == CamerErrorCode.ABORT)
                        {
                            bContinue = false;
                        }
                    }
                }

             
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        private bool CaptureStart(IntPtr intPtr, DCAMWAIT eventmask, ref DCAMWAIT eventhappened)
        {
            int m_timeout = 1000;
            if (intPtr == IntPtr.Zero)
            {
                m_lasterr = CamerErrorCode.INVALIDWAITHANDLE;
            }
            else
            {
                DCAMWAIT_START param = new DCAMWAIT_START(eventmask);
                param.timeout = m_timeout;
                m_lasterr = CamWaitingCapture.Start(intPtr, ref param);
                if (!m_lasterr.failed())
                    eventhappened = new DCAMWAIT(param.eventhappened);
            }

            return !m_lasterr.failed();
        }

        private bool Cap_transferinfo(ref int nNewestFrameIndex, ref int nFrameCount)
        {
            if (m_hdcam == IntPtr.Zero)
            {
                m_lasterr = CamerErrorCode.INVALIDHANDLE;
            }
            else
            {
                DCAMCAP_TRANSFERINFO param = new DCAMCAP_TRANSFERINFO(0);
                m_lasterr = CamcapCapturingControl.Transferinfo(m_hdcam, ref param);
                if (!m_lasterr.failed())
                {
                    nNewestFrameIndex = param.nNewestFrameIndex;
                    nFrameCount = param.nFrameCount;
                    return true;
                }
            }

            nNewestFrameIndex = -1;
            nFrameCount = 0;
            return false;
        }

        // update LUT condition
        private void Update_lut(bool bUpdatePicture, int lutMin, int lutMax)
        {

            //  MyDcamProp prop = new MyDcamProp(mydcam, CamerPropertyID.BITSPERCHANNEL);

            double v = 0;
            //  prop.getvalue(ref v);
            CameraProperty.Getvalue(m_hdcam, CamerPropertyID.BITSPERCHANNEL, ref v);
            bool reset = false;
            if (Camerabpp > 0 && Camerabpp != (int)v)
            {
                reset = true;
            }

            Camerabpp = (int)v;
            Cameramax = (1 << Camerabpp) - 1;
            LutMax = lutMax;
            LutMin = lutMin;


            if (reset)
            {

                bUpdatePicture = true;
            }

            if (bUpdatePicture)
                CreateBitmap(lutMin, lutMax);

        }
        public void GetPixelFormat()
        {
            double value = 0;
            CameraProperty.Getvalue(m_hdcam, CamerPropertyID.TIMING_EXPOSURE, ref value);
            CameraProperty.Getvalue(m_hdcam, CamerPropertyID.SENSORMODE, ref value);
            CameraProperty.Getvalue(m_hdcam, CamerPropertyID.EXPOSURETIME, ref value);
            CameraProperty.Getvalue(m_hdcam, CamerPropertyID.IMAGE_PIXELTYPE, ref value);
            CameraProperty.Getvalue(m_hdcam, CamerPropertyID.IMAGE_WIDTH, ref value);
            CameraProperty.Getvalue(m_hdcam, CamerPropertyID.IMAGE_HEIGHT, ref value);

        }
        private int GetWidth()
        {
            double value = 0;
            CameraProperty.Getvalue(m_hdcam, CamerPropertyID.IMAGE_WIDTH, ref value);
            return Convert.ToInt32(value);
        }
        private int GetHeight()
        {
            double value = 0;
            CameraProperty.Getvalue(m_hdcam, CamerPropertyID.IMAGE_HEIGHT, ref value);

            return Convert.ToInt32(value);
        }
        private double GetExposureTime()
        {
            double value = 0;
            CameraProperty.Getvalue(m_hdcam, CamerPropertyID.EXPOSURETIME, ref value);

            return value;
        }
        private void CreateImage(int iFrame)
        {
            // lock selected frame by iFrame
            m_image.set_iFrame(iFrame);

       
            m_lasterr = Dcamapidll.dcambuf_lockframe(m_hdcam, ref m_image.bufframe);
            if (m_lasterr.failed())
            {
                m_image.clear();
                throw new Exception("Camera lockframe fail");
            }


            CreateBitmap(LutMin, LutMax);

        }

        private void CreateBitmap(int lutmin, int lutmax)
        {
            try
            {


                if (m_image.isValid())
                {


                    Rectangle rc = new Rectangle(0, 0, m_image.width, m_image.height);
                    lock (BitmapLock)
                    {

                        m_bitmap = new Bitmap(Width, Height, pixelFormat);

                        SubACQError err =  Copydib(ref m_bitmap, m_image.bufframe, ref rc, lutmin, lutmax, Camerabpp);


                        var bitmap = m_bitmap.ToBitmapSource();
                        bitmap = bitmap.FormatConvertTo(PixelFormats.Rgb24);

                        subject.OnNext(bitmap.ToByteFrame());


                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        private  SubACQError Copydib(ref Bitmap bitmap, DCAMBUF_FRAME src, ref Rectangle rect, int lutmax, int lutmin, int bpp)
        {
            int w = rect.Width;
            int h = rect.Height;
            if (w > bitmap.Width) w = bitmap.Width;
            if (h > bitmap.Height) h = bitmap.Height;
            if (w > src.width) w = src.width;
            if (h > src.height) h = src.height;

            SubACQError err;
            // BitmapData dst = bitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData dst = bitmap.LockBits(rect, ImageLockMode.ReadWrite, bitmap.PixelFormat);
            if (src.type == DCAM_PIXELTYPE.MONO16)
                err = Copydib_rgb24_from_mono16(dst.Scan0, dst.Stride, src.buf, src.rowbytes, w, h, lutmax, lutmin, bpp);
            else
                err = SubACQError.INVALID_SRCPIXELTYPE;

            bitmap.UnlockBits(dst);

            return err;
        }

        private  SubACQError Copydib_rgb24_from_mono16(IntPtr dst, Int32 dstrowbytes, IntPtr src, Int32 srcrowbytes, Int32 width, Int32 height, Int32 lutmax, Int32 lutmin, Int32 bpp)
        {
            Int16[] s = new Int16[width];
            byte[] d = new byte[dstrowbytes];

            double gain = 0;
            double inBase = 0;

            if (lutmax != lutmin)
            {
                if (lutmin < lutmax)
                {
                    gain = 256.0 / (lutmax - lutmin + 1);
                    inBase = lutmin;
                }
                else
                if (lutmin > lutmax)
                {
                    gain = 256.0 / (lutmax - lutmin - 1);
                    inBase = lutmax;
                }
            }
            else
            if (lutmin > 0)    // binary threshold
            {
                gain = 0;
                inBase = lutmin;
            }

            Int16 y;
            for (y = 0; y < height; y++)
            {
                Int32 offset;

                offset = srcrowbytes * y;
                Marshal.Copy((IntPtr)(src.ToInt64() + offset), s, 0, width);

                Copydibline_rgb24_from_mono16(d, s, width, gain, inBase, bpp);

                offset = dstrowbytes * y;
                Marshal.Copy(d, 0, (IntPtr)(dst.ToInt64() + offset), dstrowbytes);
            }
            return SubACQError.SUCCESS;
        }

        private  void Copydibline_rgb24_from_mono16(byte[] d, Int16[] s, Int32 width, double gain, double inBase, Int32 bpp)
        {
            Int16 x;
            Int16 i = 0;
            if (gain != 0)
            {
                for (x = 0; x < width; x++)
                {
                    UInt16 u = (UInt16)s[x];

                    double v = gain * (u - inBase);

                    Byte c;
                    if (v > 255)
                        c = 255;
                    else
                    if (v < 0)
                        c = 0;
                    else
                        c = (Byte)v;

                    d[i++] = c;
                    d[i++] = c;
                    d[i++] = c;
                }
            }
            else
            if (inBase > 0)    // binary threshold
            {
                for (x = 0; x < width; x++)
                {
                    UInt16 u = (UInt16)s[x];

                    Byte c = (Byte)(u >= inBase ? 255 : 0);

                    d[i++] = c;
                    d[i++] = c;
                    d[i++] = c;
                }
            }
            else
            {
                for (x = 0; x < width; x++)
                {
                    UInt16 u = (UInt16)s[x];

                    Byte c = (Byte)(u >> (bpp - 8));

                    d[i++] = c;
                    d[i++] = c;
                    d[i++] = c;
                }
            }
        }



    }

    public class HamamatsuImage
    {
        public DCAMBUF_FRAME bufframe;
        public HamamatsuImage()
        {
            bufframe = new DCAMBUF_FRAME(0);
        }
        public int width { get { return bufframe.width; } }
        public int height { get { return bufframe.height; } }
        public DCAM_PIXELTYPE pixeltype { get { return bufframe.type; } }
        public bool isValid()
        {
            if (width <= 0 || height <= 0 || pixeltype == DCAM_PIXELTYPE.NONE)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public void clear()
        {
            bufframe.width = 0;
            bufframe.height = 0;
            bufframe.type = DCAM_PIXELTYPE.NONE;
        }
        public void set_iFrame(int index)
        {
            bufframe.iFrame = index;
        }
    }
}
