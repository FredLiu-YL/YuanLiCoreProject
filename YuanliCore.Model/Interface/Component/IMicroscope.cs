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
        /// 目前Lens在第幾孔
        /// </summary>
        int LensIndex { get; }
        /// <summary>
        /// 目前Cube在第幾孔
        /// </summary>
        int CubeIndex { get; }
        /// <summary>
        /// 目前Filter1在第幾孔
        /// </summary>
        int Filter1Index { get; }
        /// <summary>
        /// 目前Filter2在第幾孔
        /// </summary>
        int Filter2Index { get; }
        /// <summary>
        /// 目前Filter3在第幾孔
        /// </summary>
        int Filter3Index { get; }
        /// <summary>
        /// 目前光強度
        /// </summary>
        int LightValue { get; }
        /// <summary>
        /// 目前光圈
        /// </summary>
        int ApertureValue { get; }
        /// <summary>
        /// 目前Z軸位置
        /// </summary>
        int Position { get; }
        /// <summary>
        /// Z軟體負極限
        /// </summary>
        int NEL { get; set; }
        /// <summary>
        /// Z軟體正極限
        /// </summary>
        int PEL { get; set; }
        /// <summary>
        /// 準焦位置
        /// </summary>
        int AberationPosition { get; }
        /// <summary>
        /// 自動對焦負極限
        /// </summary>
        int AFNEL { get; set; }
        /// <summary>
        /// 自動對焦正極限
        /// </summary>
        int AFPEL { get; set; }
        /// <summary>
        /// TimeOut重送次數
        /// </summary>
        int TimeOutRetryCount { get; set; }
        /// <summary>
        /// 初始化
        /// </summary>
        void Initial();
        /// <summary>
        /// 復歸
        /// </summary>
        /// <returns></returns>
        Task HomeAsync();

        /// <summary>
        /// 更換鏡頭 idx=1~5、6(有5、6孔)
        /// </summary>
        /// <param name="idx"></param>
        Task ChangeLensAsync(int idx);

        /// <summary>
        /// 更換鏡片組BF DF Other,idx=1~2、6、8(有2、6、8種)
        /// </summary>
        /// <param name="idx"></param>
        Task ChangeCubeAsync(int idx);
        /// <summary>
        /// 更換第一道濾片，濾片可以切換1~6、7、8組(idx)
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        Task ChangeFilter1Async(int idx);
        /// <summary>
        /// 更換第二道濾片，濾片可以切換1~6、7、8組(idx)
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        Task ChangeFilter2Async(int idx);
        /// <summary>
        /// 更換第三道濾片，濾片可以切換1~6、7、8組(idx)
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        Task ChangeFilter3Async(int idx);
        /// <summary>
        /// 更換光圈 ApertureValue=0~3113
        /// </summary>
        /// <param name="ApertureValue"></param>
        /// <returns></returns>
        Task ChangeApertureAsync(int ApertureValue);
        /// <summary>
        /// 更換光亮度 LigntValue=0~120
        /// </summary>
        /// <param name="LigntValue"></param>
        /// <returns></returns>
        Task ChangeLightAsync(int LigntValue);
        /// <summary>
        /// 更換光的發射方式 反射、透射
        /// </summary>
        /// <param name="idx"></param>
        Task ChangeLightSpreadAsync(int idx);
        /// <summary>
        /// 一直對焦
        /// </summary>
        void AFTrace();
        /// <summary>
        /// 對焦一次
        /// </summary>
        Task AFOneShotAsync();
        /// <summary>
        /// 停止對焦
        /// </summary>
        void AFOff();
        /// <summary>
        /// 設定對焦的參數 FirstZPos(Z初始對焦位置) SearchRange(搜尋對焦的範圍)，會自動關閉AF
        /// </summary>
        /// <param name="FirstZPos"></param>
        /// <param name="SearchRange"></param>
        void SetSearchRange(double FirstZPos, double Range);
        /// <summary>
        /// 移動對焦軸相對位置
        /// </summary>
        /// <param name="distance"></param>
        Task MoveAsync(double distance);
        /// <summary>
        /// 移動對焦軸絕對位置
        /// </summary>
        /// <param name="position"></param>
        Task MoveToAsync(double position);
        /// <summary>
        /// 對焦值相對位置
        /// </summary>
        /// <param name="distance"></param>
        Task AberrationMoveAsync(double distance);
        /// <summary>
        /// 對焦值絕對位置
        /// </summary>
        /// <param name="position"></param>
        Task AberrationMoveToAsync(double position);
    }

    public interface IMicroscope2
    {

        /// <summary>
        /// 光強度
        /// </summary>
        int LightValue { get; set; }
        /// <summary>
        /// 光圈
        /// </summary>
        int ApertureValue { get; set; }

        /// <summary>
        /// Z軸位置
        /// </summary>
        double Position { get; }

        double PositionPEL { get; set; }
        double PositionNEL { get; set; }

        /// <summary>
        /// 對焦位置
        /// </summary>
        double AberationPosition { get; set; }
        /// <summary>
        /// 對焦範圍正極限
        /// </summary>
        double AutoFocusPEL { get; set; }

        /// <summary>
        /// 對焦範圍負極限
        /// </summary>
        double AutoFocusNEL { get; set; }
        /// <summary>
        /// 鼻輪號碼
        /// </summary>
        int LensNumber { get; set; }

        void Initial();
        void Open();
        void Close();
        void Home();

        void MoveFocusPosition();

        void Dispose();


    }
}
