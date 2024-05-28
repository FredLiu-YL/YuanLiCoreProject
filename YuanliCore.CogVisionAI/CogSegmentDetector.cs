using Cognex.VisionPro;
using Cognex.VisionPro.Blob;
using Cognex.VisionPro.ViDiEL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Media.Imaging;

using YuanliCore.CameraLib;
using System.Runtime.Remoting.Contexts;
using YuanliCore.Interface;

namespace YuanliCore.CogVisionAI
{
    public class CogSegmentDetector
    {
        private CogSegmentTool segmentTool;
        private CogBlobTool blobTool;
        public CogSegmentDetector()
        {

            segmentTool = new CogSegmentTool();
            blobTool = new CogBlobTool();
            //BlobParams blobparams = new BlobParams();
            /* blobparams.RunParams.SegmentationParams.Mode = CogBlobSegmentationModeConstants.HardFixedThreshold;
             blobparams.RunParams.SegmentationParams.Polarity = CogBlobSegmentationPolarityConstants.DarkBlobs;
             blobparams.RunParams.SegmentationParams.HardFixedThreshold = 17;
             blobparams.RunParams.ConnectivityMinPixels = 13;*/
            //RunParams = blobparams;
        }
        public CogSegmentDetector(CogSegmentTool GiveSegmentTool)
        {
            segmentTool = GiveSegmentTool;
        }
        public CogSegmentDetector(CogBlobTool GiveblobTool)
        {

            blobTool = GiveblobTool;

        }
        public CogSegmentDetector(CogSegmentTool GiveSegmentTool, CogBlobTool GiveblobTool)
        {

            segmentTool = GiveSegmentTool;
        }

        public BitmapSource Run(ICogVisionData image)
        {
            segmentTool.InputImage = image;
            segmentTool.Run();
            if (segmentTool.RunStatus.Result != CogToolResultConstants.Accept)
            {
                throw new Exception(segmentTool.RunStatus.Message);
            }

            // Verify that CogSegmentTool produced a result ...
            int nSegRes = segmentTool.Results.Count;
            if (nSegRes < 1)
            {
                throw new Exception("CogSegmentTool produced no results.");
            }

            // Extract heatmap and class name from CogSegmentTool results ...
            String sName = segmentTool.Results[0].Class;
            CogImage8Grey aHeatMap = segmentTool.Results[0].Heatmap;
            var img = aHeatMap.ToBitmap();

            IntPtr hBitmap = img.GetHbitmap();
            BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            // // 釋放 HBitmap 的資源
            //NativeMethods.DeleteObject(hBitmap);


            return bitmapSource;

        }

        //public Frame<byte[]> Run(Frame<byte[]> image)
        //{
        //    segmentTool.InputImage = image.ColorFrameToColorCogImage() as ICogVisionData;
        //    segmentTool.Run();
        //    if (segmentTool.RunStatus.Result != CogToolResultConstants.Accept)
        //    {
        //        throw new Exception(segmentTool.RunStatus.Message);
        //    }

        //    // Verify that CogSegmentTool produced a result ...
        //    int nSegRes = segmentTool.Results.Count;
        //    if (nSegRes < 1)
        //    {
        //        throw new Exception("CogSegmentTool produced no results.");
        //    }

        //    // Extract heatmap and class name from CogSegmentTool results ...
        //    String sName = segmentTool.Results[0].Class;
        //    CogImage8Grey aHeatMap = segmentTool.Results[0].Heatmap;

        //    var img = aHeatMap.ToBitmap().ToBitmapSource().ToByteFrame();

        //    //img.ToBitmapSource();
        //    //IntPtr hBitmap = img.GetHbitmap();
        //    //BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
        //    //    hBitmap,
        //    //    IntPtr.Zero,
        //    //    Int32Rect.Empty,
        //    //    BitmapSizeOptions.FromEmptyOptions());

        //    // // 釋放 HBitmap 的資源
        //    //NativeMethods.DeleteObject(hBitmap);


        //    return img;

        //}

        public ICogImage Run(ICogImage cogImage)
        {
            segmentTool.InputImage = cogImage as ICogVisionData;
            segmentTool.Run();
            if (segmentTool.RunStatus.Result != CogToolResultConstants.Accept)
            {
                throw new Exception(segmentTool.RunStatus.Message);
            }

            // Verify that CogSegmentTool produced a result ...
            int nSegRes = segmentTool.Results.Count;
            if (nSegRes < 1)
            {
                throw new Exception("CogSegmentTool produced no results.");
            }

            // Extract heatmap and class name from CogSegmentTool results ...
            //String sName = segmentTool.Results[0].Class;
            //CogImage8Grey aHeatMap = segmentTool.Results[0].Heatmap;

            return segmentTool.Results[0].Heatmap;
        }



        //public override CogParameter RunParams { get; set; }

        //public CogSegmentResults Results
        //{
        //    get => segmentTool.Results;
        //}
    }
}
