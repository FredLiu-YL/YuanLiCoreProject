using Cognex.VisionPro;
using Cognex.VisionPro.ImageProcessing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using YuanliCore.Interface;

namespace YuanliCore.CameraLib
{
    public static class FrameEX
    {
        /// <summary>
        /// 黑白BitmapSource 轉黑白的CogImage
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public static ICogImage GrayFrameToCogImage(this Frame<byte[]> frame)
        {
            // Create Cognex Root thing.
            var cogRoot = new CogImage8Root();
            CogImage8Grey cogImage = new CogImage8Grey();
            var rawSize = frame.Width * frame.Height;
            SafeMalloc buf = null;
            try
            {
                cogImage = new CogImage8Grey();

                buf = new SafeMalloc(rawSize);

                // Copy from the byte array into the
                // previously allocated. memory
                Marshal.Copy(frame.Data, 0, buf, rawSize);

                // Initialise the image root, the stride is the
                // same as the widthas the input image is byte alligned and
                // has no padding etc.
                cogRoot.Initialize(frame.Width, frame.Height, buf, frame.Width, buf);

                // And set the image roor
                cogImage.SetRoot(cogRoot);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return cogImage;
        }
        /// <summary>
        /// 彩色BitmapSource 轉彩色的CogImage
        /// </summary>
        /// <param name="bitmapSource"></param>
        /// <returns></returns>
        public static ICogImage ColorFrameToColorCogImage(this BitmapSource bitmapSource)
        {
            try
            {

                using (System.Drawing.Bitmap bmp = bitmapSource.ToByteFrame().ToBitmap())
                {
                    return new CogImage24PlanarColor(bmp);

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// frame 轉彩色的CogImage
        /// </summary>
        /// <param name="bitmapSource"></param>
        /// <returns></returns>
        public static ICogImage ColorFrameToColorCogImage(this Frame<byte[]> frame)
        {
            try
            {

                using (System.Drawing.Bitmap bmp = frame.ToBitmap())
                {
                    var cogImg = new CogImage24PlanarColor(bmp);
                    return cogImg;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 彩色Frame 轉黑白的CogImage
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="bayerRedScale"></param>
        /// <param name="bayerGreenScale"></param>
        /// <param name="bayerBlueScale"></param>
        /// <returns></returns>
        public static ICogImage ColorFrameToCogImage(this Frame<byte[]> frame, out ICogImage inputImage, double bayerRedScale = 0.333, double bayerGreenScale = 0.333, double bayerBlueScale = 0.333)
        {
            try
            {

                using (System.Drawing.Bitmap bmp = frame.ToBitmap())
                {

                    CogImage24PlanarColor cogImage = new CogImage24PlanarColor(bmp);

                    using (CogImageConvertTool tool = new CogImageConvertTool())
                    {
                        tool.InputImage = cogImage;
                        tool.RunParams.RunMode = CogImageConvertRunModeConstants.IntensityFromWeightedRGB;

                        tool.RunParams.IntensityFromWeightedRGBRedWeight = bayerRedScale;
                        tool.RunParams.IntensityFromWeightedRGBGreenWeight = bayerGreenScale;
                        tool.RunParams.IntensityFromWeightedRGBBlueWeight = bayerBlueScale;

                        tool.Run();

                        inputImage = cogImage;
                        return (CogImage8Grey)tool.OutputImage;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 彩色BitmapSource 轉黑白的CogImage
        /// </summary>
        /// <param name="bitmapSource"></param>
        /// <param name="bayerRedScale"></param>
        /// <param name="bayerGreenScale"></param>
        /// <param name="bayerBlueScale"></param>
        /// <returns></returns>
        public static ICogImage ColorFrameToCogImage(this BitmapSource bitmapSource, double bayerRedScale = 0.333, double bayerGreenScale = 0.333, double bayerBlueScale = 0.333)
        {
            try
            {

                using (System.Drawing.Bitmap bmp = bitmapSource.ToByteFrame().ToBitmap())
                {
                    CogImage24PlanarColor cogImage = new CogImage24PlanarColor(bmp);

                    using (CogImageConvertTool tool = new CogImageConvertTool())
                    {
                        tool.InputImage = cogImage;
                        tool.RunParams.RunMode = CogImageConvertRunModeConstants.IntensityFromWeightedRGB;

                        tool.RunParams.IntensityFromWeightedRGBRedWeight = bayerRedScale;
                        tool.RunParams.IntensityFromWeightedRGBGreenWeight = bayerGreenScale;
                        tool.RunParams.IntensityFromWeightedRGBBlueWeight = bayerBlueScale;

                        tool.Run();
                        return (CogImage8Grey)tool.OutputImage;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



    }

    class SafeMalloc : SafeBuffer
    {
        /// <summary>
        /// Allocates memory and initialises the SaveBuffer
        /// </summary>
        /// <param name="size">The number of bytes to allocate</param>
        public SafeMalloc(int size) : base(true)
        {
            this.SetHandle(Marshal.AllocHGlobal(size));
            this.Initialize((ulong)size);
        }

        /// <summary>
        /// Called when the object is disposed, ferr the
        /// memory via FreeHGlobal().
        /// </summary>
        /// <returns></returns>
        protected override bool ReleaseHandle()
        {
            Marshal.FreeHGlobal(this.handle);
            return true;
        }

        /// <summary>
        /// Cast to IntPtr
        /// </summary>
        public static implicit operator IntPtr(SafeMalloc h)
        {
            return h.handle;
        }
    }
}
