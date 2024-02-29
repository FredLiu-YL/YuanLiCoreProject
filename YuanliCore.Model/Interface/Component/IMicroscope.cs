using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuanliCore.Model.Interface
{
    public interface IMicroscope
    {
        /// <summary>
        /// 目前光強度
        /// </summary>
        int LightValue { get; set; }
        /// <summary>
        /// 目前光圈
        /// </summary>
        int ApertureValue { get; set; }
        /// <summary>
        /// 目前Z軸位置
        /// </summary>
        Task<double> GetZPosition();
        /// <summary>
        /// Z軟體負極限
        /// </summary>
        Task SetZNEL(int position);
        /// <summary>
        /// 取得Z軟體負極限
        /// </summary>
        Task<double> GetZNEL();
        /// <summary>
        /// Z軟體正極限
        /// </summary>
        Task SetZPEL(int position);
        /// <summary>
        /// 取得Z軟體正極限
        /// </summary>
        Task<double> GetZPEL();
        /// <summary>
        /// 準焦位置
        /// </summary>
        Task<double> GetAberationPosition();
        /// <summary>
        /// 取得準焦軟體負極限
        /// </summary>
        Task<int> GetAFNEL();
        /// <summary>
        /// 設定準焦軟體負極限
        /// </summary>
        Task SetAFNEL(int position);
        /// <summary>
        /// 取得準焦軟體正極限
        /// </summary>
        Task<int> GetAFPEL();
        /// <summary>
        /// 設定準焦軟體正極限
        /// </summary>
        Task SetAFPEL(int position);
        /// <summary>
        /// 初始化
        /// </summary>
        void Initial();
        /// <summary>
        /// 復歸
        /// </summary>
        /// <returns></returns>
        Task Home();
        /// <summary>
        /// 更換鏡頭 idx=1~5、6(有5、6孔)
        /// </summary>
        /// <param name="idx"></param>
        Task ChangeLens(int idx);
        /// <summary>
        /// 更換鏡片組BF DF Other
        /// </summary>
        /// <param name="idx"></param>
        Task ChangeCube(int idx);
        /// <summary>
        /// 更換光圈 ApertureValue=0~3113
        /// </summary>
        /// <param name="ApertureValue"></param>
        /// <returns></returns>
        Task ChangeAperture(int ApertureValue);
        /// <summary>
        /// 更換濾片，可裝1~3道濾片(wheelIdx)，每一到濾片可以切換1~6、7、8組(idx)
        /// </summary>
        /// <param name="wheelIdx">1~3</param>
        /// <param name="idx">1~6、7、8</param>
        /// <returns></returns>
        Task ChangeFilter(int wheelIdx, int idx);
        /// <summary>
        /// 更換光亮度 LigntValue=0~120
        /// </summary>
        /// <param name="LigntValue"></param>
        /// <returns></returns>
        Task ChangeLight(int LigntValue);
        /// <summary>
        /// 更換光的發射方式 反射、透射
        /// </summary>
        /// <param name="idx"></param>
        Task ChangeLightSpread(int idx);
        /// <summary>
        /// 一直對焦
        /// </summary>
        Task AF_Trace();
        /// <summary>
        /// 對焦一次
        /// </summary>
        Task AF_OneShot();
        /// <summary>
        /// 停止對焦
        /// </summary>
        Task AF_Off();
        /// <summary>
        /// 設定對焦的參數 FirstZPos(Z初始對焦位置) SearchRange(搜尋對焦的範圍)
        /// </summary>
        /// <param name="FirstZPos"></param>
        /// <param name="SearchRange"></param>
        Task SetSearchRange(double FirstZPos, double Range);
        /// <summary>
        /// 移動對焦軸相對位置
        /// </summary>
        /// <param name="distance"></param>
        Task ZMoveCommand(double distance);
        /// <summary>
        /// 移動對焦軸絕對位置
        /// </summary>
        /// <param name="position"></param>
        Task ZMoveToCommand(double position);
        /// <summary>
        /// 對焦值相對位置
        /// </summary>
        /// <param name="distance"></param>
        Task AberrationMoveCommand(double distance);
        /// <summary>
        /// 對焦值絕對位置
        /// </summary>
        /// <param name="position"></param>
        Task AberrationMoveToCommand(double position);
    }


}
