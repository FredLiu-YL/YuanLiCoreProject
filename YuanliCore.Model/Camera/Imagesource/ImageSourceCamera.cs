using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using TIS.Imaging;
using YuanliCore.Interface;

namespace YuanliCore.CameraLib.ImageSource
{
    public class ImageSourceCamera : ICamera
    {
        private ICImagingControl iCImaging;
        private bool isOpen;
        private int timeoutSec = 5;
        private Subject<Frame<byte[]>> frames = new Subject<Frame<byte[]>>();
        private Task refreshTask = Task.CompletedTask;
        private PixelFormat format;

        private int width;
        private int height;

        public ImageSourceCamera(string settingFileName)
        {

            iCImaging = new ICImagingControl();
            iCImaging.LiveDisplay = false;
            //如果有檔案存在就讀檔 ， 沒檔案就開啟設定頁面
            if (File.Exists(settingFileName))
                iCImaging.LoadDeviceStateFromFile(settingFileName, true);
            else
            {
                if (!iCImaging.LoadShowSaveDeviceState(settingFileName))
                {
                    throw new Exception("No device was selected.");

                }
            }
            var dpi = iCImaging.DeviceDpi;
            //format = iCImaging.MemoryPixelFormat;
            //目前沒成功撈出 實際用的Format  先用取像轉換後BitmapSource預設格式 ，未來如果有黑白相機再看怎麼改
            format = PixelFormats.Bgr32;
            width = iCImaging.ImageWidth;
            height = iCImaging.ImageHeight;


        }

        public bool IsGrabbing { get; set; }
        public int Width => width;

        public int Height => height;

        public PixelFormat PixelFormat { get => format; set => throw new NotImplementedException(); }

        public IObservable<Frame<byte[]>> Frames => frames;

        public double Gain { get => GetGain(); set => SetGain(value); }
        public double ExposureTime { get => GetExposure(); set => SetExposure(value); }
        public int Brightness { get => GetBrightness(); set => SetBrightness(value); }

        public async void Close()
        {
            if (!isOpen) throw new Exception("The camera is not turned on yet");
            Stop();


            iCImaging.Sink.Dispose();
            await refreshTask;
            isOpen = false;
        }

        public IDisposable Grab()
        {
            if (!isOpen) throw new Exception("The camera is not turned on yet");
            if (IsGrabbing) return null;

            try
            {
                iCImaging.LiveStart();

                refreshTask = Task.Run(RefreshImage);

                return Disposable.Create(Stop);
            }
            catch (Exception ex)
            {
                Stop();
                throw ex;
            }
        }

        public BitmapSource GrabAsync()
        {
            if (!isOpen) throw new Exception("The camera is not turned on yet");
            if (!IsGrabbing) throw new Exception("No video streaming");

            var f = Frames.Take(1).Timeout(TimeSpan.FromSeconds(3)).FirstOrDefault();
            var bmp = f.ToBitmapSource();
            return bmp.FormatConvertTo(PixelFormats.Bgr24);
        }

        public void Open()
        {

            iCImaging.Sink = new FrameSnapSink();
            isOpen = true;
        }

        public void Stop()
        {
            if (!isOpen) throw new Exception("The camera is not turned on yet");
            if (IsGrabbing)
            {

                IsGrabbing = false;
                try
                {
                    refreshTask.Wait(TimeSpan.FromMilliseconds(300));
                }
                catch (Exception)
                {

                }
                iCImaging.LiveStop();
            }
        }

        private async Task RefreshImage()
        {
            FrameSnapSink snapSink = iCImaging.Sink as FrameSnapSink;
            IsGrabbing = true;
            try
            {
                int reTryCount = 0;
                while (IsGrabbing)
                {
                    try
                    {
                        //取像
                        IFrameQueueBuffer snap = snapSink.SnapSingle(TimeSpan.FromSeconds(timeoutSec));
                        Bitmap image = snap.CreateBitmapWrap();

                        //  var frame = image.ImageToBytes();

                        var bmpsource = image.ToBitmapSource();

                        // var frame = new WriteableBitmap(bmpsource);
                        var frame = bmpsource.ToByteFrame();
                        frames.OnNext(frame);

                        await Task.Delay(5);
                        reTryCount = 0;
                    }
                    catch (Exception)
                    {
                        //在其他下中斷點會TimeOut
                        if (reTryCount > 3)
                        {
                            throw;
                        }
                        reTryCount += 1;
                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        private void SetBrightness(int brightnessValue)
        {
            try
            {
                //Brightness 範圍是 0-4095   
                var brightness = iCImaging.VCDPropertyItems.Find(VCDGUIDs.VCDID_Brightness, VCDGUIDs.VCDElement_Value);
                var temp = brightness.get_Item(0);
                VCDRangeProperty rangeProperty = temp as VCDRangeProperty;
                rangeProperty.Value = brightnessValue;
            }
            catch (Exception ex)
            {

                throw ex;
            }


        }
        private int GetBrightness()
        {
            try
            {
                //Brightness 範圍是 0-4095   
                var brightness = iCImaging.VCDPropertyItems.Find(VCDGUIDs.VCDID_Brightness, VCDGUIDs.VCDElement_Value);
                var temp = brightness.get_Item(0);
                VCDRangeProperty rangeProperty = temp as VCDRangeProperty;
                return rangeProperty.Value;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        private void SetExposure(double exposureTime)
        {
            try
            {
                if (iCImaging != null)
                {
                    //Exposure 範圍是 0.01ms-30000ms  範圍很大 ， 所以要從使用端限縮範圍，或是參數直接給(目前不做限制)
                    var exposure = iCImaging.VCDPropertyItems.Find(VCDGUIDs.VCDID_Exposure, VCDGUIDs.VCDElement_Value);
                    var temp = exposure.get_Item(0);
                    VCDRangeProperty rangeProperty = temp as VCDRangeProperty;
                    rangeProperty.Value = (int)exposureTime;

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }


        }
        private double GetExposure()
        {
            try
            {
                if (iCImaging != null)
                {
                    //Exposure 範圍是 0.01ms-30000ms  範圍很大 ， 所以要從使用端限縮範圍
                    var exposure = iCImaging.VCDPropertyItems.Find(VCDGUIDs.VCDID_Exposure, VCDGUIDs.VCDElement_Value);
                    var temp = exposure.get_Item(0);
                    VCDRangeProperty rangeProperty = temp as VCDRangeProperty;
                    return rangeProperty.Value;

                }
                return 0;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        private void SetGain(double gain)
        {
            try
            {
                if (iCImaging != null)
                {
                    //Gain 範圍是 0-480  
                    var vcdGain = iCImaging.VCDPropertyItems.Find(VCDGUIDs.VCDID_Gain, VCDGUIDs.VCDElement_Value);
                    var temp = vcdGain.get_Item(0);
                    VCDRangeProperty rangeProperty = temp as VCDRangeProperty;
                    rangeProperty.Value = (int)gain;

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }


        }
        private double GetGain()
        {

            try
            {
                if (iCImaging != null)
                {
                    //Gain 範圍是 0-480  
                    var gain = iCImaging.VCDPropertyItems.Find(VCDGUIDs.VCDID_Gain, VCDGUIDs.VCDElement_Value);
                    var temp = gain.get_Item(0);
                    VCDRangeProperty rangeProperty = temp as VCDRangeProperty;
                    return rangeProperty.Value;

                }
                return 0;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        private PixelFormat GetFormat(System.Drawing.Imaging.PixelFormat format)
        {
            switch (format)
            {
                case System.Drawing.Imaging.PixelFormat.Indexed:
                    return PixelFormats.Indexed8;

                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                    return PixelFormats.Bgr24;

                case System.Drawing.Imaging.PixelFormat.Format32bppRgb:
                    return PixelFormats.Bgr32;


                default:
                    return PixelFormats.Default;

            }


        }


    }


}
