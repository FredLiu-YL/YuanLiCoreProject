using Cognex.VisionPro;
using Cognex.VisionPro.Caliper;
using Cognex.VisionPro.ImageProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using YuanliCore.CameraLib;
using YuanliCore.Interface;

namespace YuanliCore.ImageProcess
{
    /// <summary>
    /// Cognex 的卡尺功能 (查找在尺內的一點 或兩點)
    /// </summary>
    public class CogFindImageSharpness : CogMethod
    {
        private CogImageSharpnessTool sharpnessTool;
        private CogFindImageSharpnessWindow cogCaliperWindow;

        public CogFindImageSharpness()
        {

            sharpnessTool = new CogImageSharpnessTool();
            RunParams = new ImageSharpnessParam(0);
        }
        public CogFindImageSharpness(CogParameter caliperParams)
        {

            sharpnessTool = new CogImageSharpnessTool();
            RunParams = caliperParams;
        }

        public override CogParameter RunParams { get; set; }
        public CogSharpnessResult CaliperResults { get; private set; }
        public override void Dispose()
        {
            if (cogCaliperWindow != null)
                cogCaliperWindow.Dispose();
        }

        public override void EditParameter(BitmapSource image)
        {
            if (image == null) throw new Exception("Image is null");
            cogCaliperWindow = new CogFindImageSharpnessWindow(image);


            cogCaliperWindow.CaliperParam = (ImageSharpnessParam)RunParams;
            cogCaliperWindow.ShowDialog();


            RunParams = cogCaliperWindow.CaliperParam;


            Dispose();
        }

        public override void EditParameter(ICogImage cogImage)
        {

            if (cogImage == null) throw new Exception("Image is null");
            cogCaliperWindow = new CogFindImageSharpnessWindow(cogImage);

            cogCaliperWindow.CaliperParam = (ImageSharpnessParam)RunParams;
            cogCaliperWindow.ShowDialog();

            RunParams = cogCaliperWindow.CaliperParam;

            Dispose();
        }
        public void CogEditParameter()
        {
            if (CogFixtureImage == null) throw new Exception("locate is not yet complete");

            cogCaliperWindow = new CogFindImageSharpnessWindow(CogFixtureImage);


            cogCaliperWindow.CaliperParam = (ImageSharpnessParam)RunParams;
            cogCaliperWindow.CaliperParam.Region.SelectedSpaceName = "@\\Fixture";
            //  cogCaliperWindow.CaliperParam.Region.SelectedSpaceName = "@\\Fixture";


            cogCaliperWindow.ShowDialog();


            RunParams = cogCaliperWindow.CaliperParam;


            Dispose();
        }


        public CogSharpnessResult Find(Frame<byte[]> image)
        {

            ICogImage cogImg1 = image.ColorFrameToCogImage(out ICogImage inputImage, 0.333, 0.333, 0.333);
            //  cogImg = cogImg1;
            //     cogRecordsDisplay = new CogRecordsDisplay();
            return Find(cogImg1);


        }
        private CogSharpnessResult Find(ICogImage cogImage)
        {
            sharpnessTool.InputImage = (CogImage8Grey)cogImage;
            var param = (ImageSharpnessParam)RunParams;
            sharpnessTool.RunParams = param.RunParams;
            //   caliperTool.Region = param.Region;
            sharpnessTool.Run();

            List<LineCaliperResult> results = new List<LineCaliperResult>();
       
 
            return new CogSharpnessResult(sharpnessTool.Score);
        }

        //public  void Run(Frame<byte[]> image)
        //{
        //    CaliperResults = Find(image).ToArray();
        //}
        public override void SetCogToolParameter(ICogTool cogTool)
        {
            var tool = cogTool as CogImageSharpnessTool;

            var param = RunParams as ImageSharpnessParam;
            param.RunParams = tool.RunParams;
     
        }
        public override void Run()
        {
            if (CogFixtureImage == null) throw new Exception("Image does not exist");
            CaliperResults = Find(CogFixtureImage);
        }

        public override ICogTool GetCogTool()
        {
            var param = (ImageSharpnessParam)RunParams;
            sharpnessTool.RunParams = param.RunParams;
            return sharpnessTool;
        }
    }

}
