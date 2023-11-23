using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cognex.VisionPro;
using Cognex.VisionPro.CalibFix;
using Cognex.VisionPro.Caliper;
using Cognex.VisionPro.ImageProcessing;
using Newtonsoft.Json;

namespace YuanliCore.ImageProcess
{
    public class ImageSharpnessParam : CogParameter
    {
        public ImageSharpnessParam(int id = 0) : base(0)
        {
            CogImageSharpnessTool tool = new CogImageSharpnessTool();

            Id = id;

            RunParams = tool.RunParams;
            Region = tool.Region;
            //  (CogPMAlignRunParams)CogSerializer.LoadObjectFromFile("");
            tool.Dispose();
        }


        /// <summary>
        /// 搜尋參數
        /// </summary>
        [JsonIgnore]//Vision pro 不能序列化  所以要忽略  不然就要用到JsonConvert
        public CogImageSharpness RunParams { get; set; }


        /// <summary>
        /// ROI  Cog用
        /// </summary>
       [JsonIgnore]
        public ICogRegion  Region { get; set; }

        public static ImageSharpnessParam Default(int id = 0)
        {
            CogImageSharpnessTool tool = new CogImageSharpnessTool();
            return Default(tool, id);
        }

        internal static ImageSharpnessParam Default(CogImageSharpnessTool tool, int id)
        {
            return new ImageSharpnessParam(0)
            {

                RunParams = tool.RunParams,
                Region = tool.Region
            };
        }

        protected override void SaveCogRecipe(string directoryPath)
        {

            // var path = CreateFolder(recipeName);

            //還需要補上 cognex 序列化方法
            CogImageSharpnessTool tool = new CogImageSharpnessTool();
            tool.RunParams = RunParams;
            tool.Region = Region;

            CogSerializer.SaveObjectToFile(tool, $"{directoryPath}\\VsTool_{Id}.tool");


            tool.Dispose();
            //   CogSerializer.SaveObjectToFile(CogToolBlock1, @"E:\ToolBlock2.vpp");
            //  string fileName = path + $"\\Commom{Id}.json";
            //  Save(fileName);
        }

        protected override void LoadCogRecipe(string directoryPath, int id)
        {

            CogImageSharpnessTool tool = (CogImageSharpnessTool)CogSerializer.LoadObjectFromFile($"{directoryPath}\\VsTool_{id}.tool");
            RunParams = tool.RunParams;
            Region = tool.Region;
            tool.Dispose();
        }
    }
    
}
