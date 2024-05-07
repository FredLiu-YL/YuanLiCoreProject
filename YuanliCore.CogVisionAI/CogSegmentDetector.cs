using Cognex.VisionPro.Blob;
using Cognex.VisionPro.ViDiEL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuanliCore.ImageProcess;
using YuanliCore.ImageProcess.AI;
using YuanliCore.ImageProcess.Blob;
using YuanliCore.Interface;

namespace YuanliCore.CogVisionAI
{
    public class CogSegmentDetector
    {
        private CogSegmentTool segmentTool;

        public CogSegmentDetector()
        {

            segmentTool = new CogSegmentTool();
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
            //BlobParams blobparams = new BlobParams();
            /* blobparams.RunParams.SegmentationParams.Mode = CogBlobSegmentationModeConstants.HardFixedThreshold;
             blobparams.RunParams.SegmentationParams.Polarity = CogBlobSegmentationPolarityConstants.DarkBlobs;
             blobparams.RunParams.SegmentationParams.HardFixedThreshold = 17;
             blobparams.RunParams.ConnectivityMinPixels = 13;*/
            //RunParams = blobparams;
        }

        //public override CogParameter RunParams { get; set; }
        public CogSegmentResult SegmentResult { get; set; }
        public CogSegmentResult SegmentResults { get; set; }
    }
}
