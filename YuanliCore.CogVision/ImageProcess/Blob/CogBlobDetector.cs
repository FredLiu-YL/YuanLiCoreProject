using Cognex.VisionPro;
using Cognex.VisionPro.Blob;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using YuanliCore.CameraLib;
using YuanliCore.Interface;

namespace YuanliCore.ImageProcess.Blob
{
    public class CogBlobDetector : CogMethod
    {
        private CogBlobTool blobTool;
        private CogBlobWindow cogBlobWindow;
  
        public CogBlobDetector()
        {

            blobTool = new CogBlobTool();
            BlobParams blobparams = new BlobParams();
           /* blobparams.RunParams.SegmentationParams.Mode = CogBlobSegmentationModeConstants.HardFixedThreshold;
            blobparams.RunParams.SegmentationParams.Polarity = CogBlobSegmentationPolarityConstants.DarkBlobs;
            blobparams.RunParams.SegmentationParams.HardFixedThreshold = 17;
            blobparams.RunParams.ConnectivityMinPixels = 13;*/
            RunParams = blobparams;
        }
        public CogBlobDetector(CogParameter blobParams)
        {

            blobTool = new CogBlobTool();
            RunParams = blobParams;
        }
        public override CogParameter RunParams { get; set; }
        public BlobDetectorResult[] DetectorResults { get; set; }


        public override void Dispose()
        {
            if (cogBlobWindow != null)
                cogBlobWindow.Dispose();
        }

        public override void EditParameter(BitmapSource image)
        {
            // if (cogCaliperWindow == null)
            cogBlobWindow = new CogBlobWindow(image);


            cogBlobWindow.BlobParam = (BlobParams)RunParams;
            cogBlobWindow.ShowDialog();


            RunParams = cogBlobWindow.BlobParam;


            Dispose();
        }

        public override void EditParameter(ICogImage cogImage) 
        {
            cogBlobWindow = new CogBlobWindow(cogImage);
            cogBlobWindow.BlobParam = (BlobParams)RunParams;
            cogBlobWindow.ShowDialog();

            RunParams = cogBlobWindow.BlobParam;

            Dispose();

        }
        public void CogEditParameter()
        {
            if (CogFixtureImage == null) throw new Exception("locate is not yet complete");

            cogBlobWindow = new CogBlobWindow(CogFixtureImage);
            var tempParam = (BlobParams)RunParams;
            if (tempParam.ROI == null)
                tempParam.ROI = new CogRectangle();
            tempParam.ROI.SelectedSpaceName = "@\\Fixture";


            cogBlobWindow.BlobParam = tempParam;
            //   cogBlobWindow.BlobParam.RunParams.ExpectedLineSegment.SelectedSpaceName = "@\\Fixture";

            cogBlobWindow.ShowDialog();
            RunParams = cogBlobWindow.BlobParam;

            Dispose();
        }
        public IEnumerable<BlobDetectorResult> Find(Frame<byte[]> image)
        {
            ICogImage cogImg1 = image.ColorFrameToCogImage(out ICogImage inputImage, 0.333, 0.333, 0.333);
            //ICogImage cogImg1 = image.GrayFrameToCogImage();


            BitmapSource bitmap = cogImg1.ToBitmap().ToBitmapSource();
            bitmap.Save("C:\\AutoDefectDetection\\AutoDefectDetection\\bin\\Debug\\test\\test.bmp");
            //bitmap.Save("C:\\AutoDefectDetection\\AutoDefectDetection\\bin\\Debug\\test\\test.bmp");

            return Find(cogImg1);
        }

        public void Run(ICogImage cogImage)
        {
            CogBlobTool blobTool_1 = new CogBlobTool();
            blobTool_1.InputImage = cogImage;
            var param = RunParams as BlobParams;
            blobTool_1.RunParams = param.RunParams;
            blobTool_1.Region = param.ROI;
            blobTool_1.Run();

            var lastRun1 = blobTool_1.CreateLastRunRecord().SubRecords[0];

            CogBlobTool blobTool_2 = new CogBlobTool();
            blobTool_2.InputImage = cogImage;
            blobTool_2.RunParams = param.RunParams;
            blobTool_2.Region = param.ROI;
            blobTool_2.Run();

            var lastRun2 = blobTool_2.CreateLastRunRecord().SubRecords[0];

            CogRecordDisplay cogDisplay = new CogRecordDisplay();
            cogDisplay.Size = new System.Drawing.Size(cogImage.Width, cogImage.Height);
            cogDisplay.Record = blobTool_1.CreateLastRunRecord();
            cogDisplay.Record.SubRecords.Add(blobTool_2.CreateLastRunRecord());


            List<BlobDetectorResult> results = new List<BlobDetectorResult>();
            var blobResults = blobTool_1.Results.GetBlobs();

            for (int i = 0; i < blobResults.Count; i++) {
                var pose = blobResults[i].CenterOfMassX;

                double x = blobResults[i].CenterOfMassX;
                double y = blobResults[i].CenterOfMassY;
                double area = blobResults[i].Area;

                results.Add(new BlobDetectorResult(new System.Windows.Point(x, y), area, 0));
            }


        }

        public IEnumerable<BlobDetectorResult> Find(ICogImage cogImage)
        {

            blobTool.InputImage = cogImage;
            var param = RunParams as BlobParams;
            blobTool.RunParams = param.RunParams;
            blobTool.Region = param.ROI;
            blobTool.Run();

            List<BlobDetectorResult> results = new List<BlobDetectorResult>();
            if (blobTool.RunStatus.Result == CogToolResultConstants.Accept) {

                var blobResults = blobTool.Results.GetBlobs();

                for (int i = 0; i < blobResults.Count; i++) {
                    var pose = blobResults[i].CenterOfMassX;
                    var radiusH = blobResults[i].GetMeasure(CogBlobMeasureConstants.BoundingBoxExtremaAngleHeight); //得到最大矩形 高
                    var radiusW = blobResults[i].GetMeasure(CogBlobMeasureConstants.BoundingBoxExtremaAngleWidth);//得到最大矩形 寬
                    Vector rect = new Vector(radiusW, radiusH);
                    var angle = blobResults[i].GetMeasure(CogBlobMeasureConstants.Angle);

                    var diameter = rect.Length; //算出最大矩形對角線 當作Blob直徑
                    double x = blobResults[i].CenterOfMassX;
                    double y = blobResults[i].CenterOfMassY;
                    double area = blobResults[i].Area;


                    results.Add(new BlobDetectorResult(new System.Windows.Point(x, y), area, diameter, rect, angle));
                }
                Record = blobTool.CreateLastRunRecord().SubRecords[0];
            }

 
            return results;
        }
        public override void SetCogToolParameter(ICogTool cogTool)
        {
            var tool = cogTool as CogBlobTool;

            var param = RunParams as BlobParams;
            param.RunParams = tool.RunParams;
            param.ROI = tool.Region;
          
        }
        public override ICogTool GetCogTool()
        {
           return blobTool;
        }

        public override void Run()
        {
            DetectorResults = Find(CogFixtureImage).ToArray();
        }


    }
}
