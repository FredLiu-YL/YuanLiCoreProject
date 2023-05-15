﻿using Cognex.VisionPro;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public static class CamerEX
    {
     

        /// <summary>
        /// 取得指定像素格式中像素的 byte 數。
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public static int GetBytesPerPixel(this PixelFormat format) => (format.BitsPerPixel + 7) / 8;

        /// <summary>
        /// 反轉序列中項目的 Key 與 Value，請注意字典中的 Key / Value 需是 1 對 1 的情況才可正常轉換，若 Value 中若有重複項目則丟出例外。 
        /// </summary>
        /// <typeparam name="Tkey">字典中的索引鍵類型。</typeparam>
        /// <typeparam name="Tvalue">字典中的值類型。</typeparam>
        /// <param name="dictionary">反轉索引鍵及值後的字典。</param>      
        /// <returns></returns>
        public static Dictionary<Tvalue, Tkey> ReverseKeyValue<Tkey, Tvalue>(this Dictionary<Tkey, Tvalue> dictionary)
        {
            return dictionary
                .ToLookup(kvp => kvp.Value)
                .ToDictionary(g => g.Key, g =>
                {
                    if (g.Count() > 1) throw new InvalidOperationException($"Dictionary contain duplicate values.");
                    return g.First().Key;
                });
        }


        #region Drawing Methods

        /// <summary>
        /// 於 BitmapSource 上繪製指定點的十字。
        /// </summary>
        /// <param name="pen">要繪製十字的畫筆。</param>
        /// <param name="point">十字中心點。</param>
        /// <param name="size">十字的大小。</param>
        /// <returns></returns>
        public static RenderTargetBitmap DrawCross(this BitmapSource bs, Point point, double size = 50, Brush brush = null, double thickness = 3)
            => DrawCross(bs, new[] { point }, size, brush, thickness);

        /// <summary>
        /// 於 BitmapSource 上繪製指定點的十字。
        /// </summary>
        /// <param name="points">十字中心點。</param>
        /// <param name="pen">要繪製十字的畫筆。</param>
        /// <param name="size">十字的大小。</param>
        /// <returns></returns>
        public static RenderTargetBitmap DrawCross(this BitmapSource bs, IEnumerable<Point> points, double size = 50, Brush brush = null, double thickness = 3)
        {
            Pen pen = new Pen(brush ?? Brushes.Red, thickness);

            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                dc.DrawImage(bs, new Rect(0, 0, bs.PixelWidth, bs.PixelHeight));
                foreach (var point in points)
                {
                    Point hp1 = new Point(point.X - size, point.Y);
                    Point hp2 = new Point(point.X + size, point.Y);
                    Point vp1 = new Point(point.X, point.Y - size);
                    Point vp2 = new Point(point.X, point.Y + size);

                    dc.DrawLine(pen, hp1, hp2);
                    dc.DrawLine(pen, vp1, vp2);
                }
            }

            GetDpiSetting(out double dpiX, out double dpiY);
            RenderTargetBitmap rtb = new RenderTargetBitmap(bs.PixelWidth, bs.PixelHeight, dpiX, dpiY, PixelFormats.Pbgra32);
            rtb.Render(dv);

            return rtb;
        }

        /// <summary>
        /// 於 BitmapSource 上繪製指定的矩形框。
        /// </summary>
        /// <param name="pen">用來繪製矩形的畫筆。</param>
        /// <param name="rectangle">要繪製的矩形。</param>
        /// <returns></returns>
        //public static RenderTargetBitmap DrawRectangle(this BitmapSource bs, Rect rectangle, Brush brush = null, double thickness = 3, Brush fill = null)
        //    => DrawRectangle(bs, new Rect[] { rectangle }, brush, thickness, fill);

        /// <summary>
        /// 於 BitmapSource 上繪製指定的矩形框。 
        /// </summary>
        /// <param name="pen">用來繪製矩形的畫筆。</param>
        /// <param name="rectangles">要繪製的矩形集合。</param>
        /// <returns></returns>
        //public static RenderTargetBitmap DrawRectangle(this BitmapSource bs, IEnumerable<Rect> rectangles, Brush brush = null, double thickness = 3, Brush fill = null)
        //{
        //    if (rectangles == null) throw new ArgumentNullException(nameof(rectangles));

        //    Pen pen = new Pen(brush ?? Brushes.Red, thickness);
        //    pen.Freeze();

        //    DrawingVisual dv = new DrawingVisual();
        //    using (DrawingContext dc = dv.RenderOpen())
        //    {
        //        dc.DrawImage(bs, new Rect(0, 0, bs.PixelWidth, bs.PixelHeight));
        //        foreach (var rect in rectangles) dc.DrawRectangle(fill, pen, rect);
        //    }

        //    GetDpiSetting(out double dpiX, out double dpiY);
        //    var rtb = bs as RenderTargetBitmap ??
        //        new RenderTargetBitmap(bs.PixelWidth, bs.PixelHeight, dpiX, dpiY, PixelFormats.Pbgra32);

        //    rtb.Render(dv);
        //    return rtb;
        //}

        #endregion

        /// <summary>
        /// 將影像依照指定的方向作翻轉。
        /// </summary
        /// <returns>翻轉後的影像。</returns>
        public static BitmapSource Flip(this BitmapSource image, FlipTypes flip)
        {
            double scaleX = flip.HasFlag(FlipTypes.Horizontal) ? -1 : 1;
            double scaleY = flip.HasFlag(FlipTypes.Vertical) ? -1 : 1;
            var transform = new ScaleTransform(scaleX, scaleY);
            return new TransformedBitmap(image, transform);
        }



        /// <summary>
        /// 判斷指定的檔案是否為影像檔案。
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static bool IsBitmapSource(this string filename)
        {
            try
            {
                if (!File.Exists(filename)) throw new FileNotFoundException("找不到指定的檔案。", filename);
                using (var fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    BitmapDecoder decoder = BitmapDecoder.Create(fileStream, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 將檔案載入為 BitmapSource。
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static BitmapSource LoadBitmapSource(this string filename)
        {
            if (!File.Exists(filename)) throw new FileNotFoundException("找不到指定的檔案。", filename);
            FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
            BitmapDecoder decoder = BitmapDecoder.Create(fileStream, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
            return decoder.Frames[0];
        }

        public static BitmapImage Load(string filename)
        {
            BitmapImage bitmap = new BitmapImage();
            using (FileStream fstream = new FileStream(filename, FileMode.Open))
            {

                bitmap.BeginInit();
                bitmap.StreamSource = fstream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
            }
            return bitmap;
        }

        /// <summary>
        /// 將 BitmapSource 依照指定的編碼器類型儲存為影像檔案。
        /// </summary>
        /// <param name="image"></param>
        /// <param name="filename"></param>
        /// <param name="format"></param>
        public static void Save(this BitmapSource image, string filename, ImageFileFormats format = ImageFileFormats.Bmp)
        {
            if (image == null) throw new ArgumentNullException("image");

            BitmapEncoder encoder = CreateEncoder(format);
            encoder.Frames.Add(BitmapFrame.Create(image));

            string filenameWithExtension = $"{filename}.{format.ToString().ToLower()}";
            using (var filestream = new FileStream(filenameWithExtension, FileMode.Create))
            {
                encoder.Save(filestream);
            }

            // 依照指定的影像格式建立影像編碼器。
            BitmapEncoder CreateEncoder(ImageFileFormats imgFormat)
            {
                switch (imgFormat)
                {
                    case ImageFileFormats.Bmp: return new BmpBitmapEncoder();
                    case ImageFileFormats.Gif: return new GifBitmapEncoder();
                    case ImageFileFormats.Jpeg: return new JpegBitmapEncoder();
                    case ImageFileFormats.Png: return new PngBitmapEncoder();
                    case ImageFileFormats.Tiff: return new TiffBitmapEncoder();
                }

                throw new NotSupportedException(format.ToString());
            }
        }

      

     
        private static Brush RandomBrush()
        {
            var t = typeof(Brushes);
            var p = t.GetProperties(BindingFlags.Public | BindingFlags.Static);

            Random r = new Random();
            int i = r.Next(p.Length);

            return (Brush)p[i].GetValue(null);
        }

        // 取得裝置的 Dpi 設定。
        private static void GetDpiSetting(out double dpiX, out double dpiY)
        {
            // SystemParameters
            const int LOGPIXELSX = 88;
            const int LOGPIXELSY = 90;

            /// get desktop dc
            IntPtr h = GetDC(IntPtr.Zero);

            /// get dpi from dc
            dpiX = GetDeviceCaps(h, LOGPIXELSX);
            dpiY = GetDeviceCaps(h, LOGPIXELSY);
        }

        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int Index);

        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr Hwnd);
    }
    public enum FlipTypes
    {
        /// <summary>
        /// 水平方向的翻轉。
        /// </summary>
        Horizontal = 1,

        /// <summary>
        /// 垂直方向的翻轉。
        /// </summary>
        Vertical = 2,
    }

    /// <summary>
    /// 影像檔案格式。
    /// </summary>
    public enum ImageFileFormats
    {
        /// <summary>
        /// 點陣圖 (BMP) 格式影像。
        /// </summary>
        Bmp,

        /// <summary>
        /// 圖形交換格式 (GIF) 影像。
        /// </summary>
        Gif,

        /// <summary>
        /// Joint Photographics Experts Group (JPEG) 格式影像。
        /// </summary>
        Jpeg,

        /// <summary>
        /// 可攜式網路圖形 (PNG) 格式影像。
        /// </summary>
        Png,

        /// <summary>
        /// 標記的影像檔案格式 (TIFF) 格式影像。
        /// </summary>
        Tiff,

        ///// <summary>
        ///// Microsoft Windows Media Photo 影像。
        ///// </summary>
        //Wmp
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
