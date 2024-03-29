﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuanliCore.Motion;

namespace YuanliCore.Interface
{
    public interface IMotionController
    {
        /// <summary>
        /// 軸卡是否正常開啟
        /// </summary>
        bool IsOpen { get; }
        /// <summary>
        /// 軸資訊
        /// </summary>
        Axis[] Axes { get; }
        /// <summary>
        /// Input點位狀態
        /// </summary>
        DigitalInput[] InputSignals { get; }
        /// <summary>
        /// Output點位狀態
        /// </summary>
        IEnumerable<DigitalOutput> OutputSignals { get; }
        /// <summary>
        /// 軸卡初始化
        /// </summary>
        void InitializeCommand();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        DigitalInput[] SetInputNames(IEnumerable<string> names);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        DigitalOutput[] SetOutputNames(IEnumerable<string> names);
        /// <summary>
        /// Output點位設定
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isOn"></param>
        void SetOutputCommand(int id, bool isOn);
        /// <summary>
        /// 取得Input點位狀態
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool GetInputCommand(int id);
        /// <summary>
        /// 設定軸卡參數
        /// </summary>
        /// <param name="axisConfig"></param>
        /// <returns></returns>
        Axis[] SetAxesParam(IEnumerable<AxisConfig> axisConfig);
        /// <summary>
        /// 設定軸開啟/關閉
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isOn"></param>
        void SetServoCommand(int id, bool isOn);
        /// <summary>
        /// 軸移動相對位置
        /// </summary>
        /// <param name="id"></param>
        /// <param name="distance"></param>
        void MoveCommand(int id, double distance);
        /// <summary>
        /// 軸移動絕對位置
        /// </summary>
        /// <param name="id"></param>
        /// <param name="position"></param>
        void MoveToCommand(int id, double position);
        /// <summary>
        /// 軸停止
        /// </summary>
        /// <param name="id"></param>
        void StopCommand(int id);
        /// <summary>
        /// 軸回Home
        /// </summary>
        /// <param name="id"></param>
        void HomeCommand(int id);
        /// <summary>
        /// 取得目前軸位置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        double GetPositionCommand(int id);
        /// <summary>
        /// 取得軸Sensor狀態:ORG、NEL、PEL
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        AxisSensor GetSensorCommand(int id);
        /// <summary>
        /// 取得軸軟體正、負極限
        /// </summary>
        /// <param name="id"></param>
        /// <param name="limitN"></param>
        /// <param name="limitP"></param>
        void GetLimitCommand(int id, out double limitN, out double limitP);
        /// <summary>
        /// 設定軸軟體正、負極限
        /// </summary>
        /// <param name="id"></param>
        /// <param name="minPos"></param>
        /// <param name="maxPos"></param>
        void SetLimitCommand(int id, double minPos, double maxPos);
        /// <summary>
        /// 取得軸速度
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        VelocityParams GetSpeedCommand(int id);
        /// <summary>
        /// 設定軸速度
        /// </summary>
        /// <param name="id"></param>
        /// <param name="motionVelocity"></param>
        void SetSpeedCommand(int id, VelocityParams motionVelocity);
        /// <summary>
        /// 設定軸方向
        /// </summary>
        /// <param name="id"></param>
        /// <param name="direction"></param>
        void SetAxisDirectionCommand(int id, AxisDirection direction);
        /// <summary>
        /// 取得軸方向
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        AxisDirection GetAxisDirectionCommand(int id);

        void ResetAlarmCommand();
    }
}
